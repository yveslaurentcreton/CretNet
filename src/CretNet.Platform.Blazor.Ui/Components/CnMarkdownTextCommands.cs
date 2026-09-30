using System.Text.RegularExpressions;
using CretNet.RichText;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>A formatting action of the <see cref="CnMarkdownEditor"/> toolbar.</summary>
public enum CnMarkdownCommand
{
    Bold,
    Italic,
    Heading,
    Subheading,
    BulletList,
    NumberedList,
    Link,
    Colour,
    ClearFormatting,
}

/// <summary>Which of the editor's panes are visible.</summary>
public enum CnMarkdownEditorMode
{
    Write,
    Preview,
    Split,
}

/// <summary>A textarea selection; equal ends are a caret.</summary>
public readonly record struct CnTextSelection(int Start, int End);

/// <summary>The text after a command, with the selection to restore.</summary>
public readonly record struct CnTextEdit(string Text, int SelectionStart, int SelectionEnd);

/// <summary>
/// The Markdown editor's toolbar as plain text operations, so every command
/// behaves identically whether it comes from a button, a shortcut or a test.
/// Formatting wraps the selection (or a placeholder word that is then
/// selected); line formats apply to every selected line and toggle off when
/// all lines already have them.
/// </summary>
public static partial class CnMarkdownTextCommands
{
    /// <summary>Applies <paramref name="command"/>; null when it needs a selection
    /// and there is none (clear formatting).</summary>
    public static CnTextEdit? Apply(string? text, CnTextSelection selection, CnMarkdownCommand command, CnPaletteColor? colour = null)
    {
        text ??= string.Empty;
        var start = Math.Clamp(Math.Min(selection.Start, selection.End), 0, text.Length);
        var end = Math.Clamp(Math.Max(selection.Start, selection.End), 0, text.Length);

        return command switch
        {
            CnMarkdownCommand.Bold => Emphasis(text, start, end, 2, CnLabels.BoldPlaceholder),
            CnMarkdownCommand.Italic => Emphasis(text, start, end, 1, CnLabels.ItalicPlaceholder),
            CnMarkdownCommand.Heading => Lines(text, start, end, lines => Heading(lines, "## ")),
            CnMarkdownCommand.Subheading => Lines(text, start, end, lines => Heading(lines, "### ")),
            CnMarkdownCommand.BulletList => Lines(text, start, end, BulletList),
            CnMarkdownCommand.NumberedList => Lines(text, start, end, NumberedList),
            CnMarkdownCommand.Link => Link(text, start, end),
            CnMarkdownCommand.Colour => Colour(text, start, end, colour ?? CnMarkdownPalette.Accent),
            CnMarkdownCommand.ClearFormatting => Clear(text, start, end),
            _ => null,
        };
    }

    /// <summary>
    /// Inserts pasted Markdown at the selection. Inline text goes in place;
    /// blocks (several lines, lists, headings) get blank lines around them so
    /// they never glue onto the paragraph they were pasted into.
    /// </summary>
    public static CnTextEdit Insert(string? text, CnTextSelection selection, string markdown)
    {
        text ??= string.Empty;
        var start = Math.Clamp(Math.Min(selection.Start, selection.End), 0, text.Length);
        var end = Math.Clamp(Math.Max(selection.Start, selection.End), 0, text.Length);
        var before = text[..start];
        var after = text[end..];

        var block = markdown.Contains('\n') || BlockStart().IsMatch(markdown);
        var insert = markdown;
        if (block)
        {
            var glue = before.Length == 0 || before.EndsWith("\n\n") ? string.Empty : before.EndsWith('\n') ? "\n" : "\n\n";
            var tail = after.Length == 0 ? "\n" : after.StartsWith("\n\n") ? string.Empty : after.StartsWith('\n') ? "\n" : "\n\n";
            insert = glue + markdown + tail;
        }

        var caret = start + insert.Length;
        return new(before + insert + after, caret, caret);
    }

    private static CnTextEdit Emphasis(string text, int start, int end, int stars, string placeholder)
    {
        var selected = text[start..end];

        // Toggle off: the markers are inside the selection or right around it.
        var inside = LeadingStars(selected);
        if (selected.Length > 2 * stars && HasEmphasis(inside, stars) && HasEmphasis(TrailingStars(selected), stars))
        {
            var inner = selected[stars..^stars];
            return new(text[..start] + inner + text[end..], start, start + inner.Length);
        }

        if (HasEmphasis(TrailingStars(text[..start]), stars) && HasEmphasis(LeadingStars(text[end..]), stars))
            return new(text[..(start - stars)] + selected + text[(end + stars)..], start - stars, end - stars);

        return Wrap(text, start, end, new string('*', stars), new string('*', stars), placeholder);
    }

    // One star is italic, two bold, three both; "has italic" and "has bold"
    // must not mistake one for the other.
    private static bool HasEmphasis(int run, int stars) => stars == 1 ? run is 1 or 3 : run >= 2;

    private static int LeadingStars(string text) => Math.Min(3, text.TakeWhile(c => c == '*').Count());

    private static int TrailingStars(string text) => Math.Min(3, text.Reverse().TakeWhile(c => c == '*').Count());

    private static CnTextEdit Link(string text, int start, int end)
    {
        var selected = text[start..end].Trim();
        if (Url().IsMatch(selected))
        {
            var link = $"[{selected}]({selected})";
            return new(text[..start] + link + text[end..], start + 1, start + 1 + selected.Length);
        }

        return Wrap(text, start, end, "[", "](https://)", CnLabels.LinkPlaceholder);
    }

    private static CnTextEdit Colour(string text, int start, int end, CnPaletteColor colour)
    {
        var selected = text[start..end];
        var suffix = "]{." + colour.Name + "}";

        // Recolour a selected or surrounding coloured text instead of nesting.
        var whole = PaletteSpan().Match(selected);
        if (whole.Success && whole.Length == selected.Length)
        {
            var inner = whole.Groups[1].Value;
            return new(text[..start] + "[" + inner + suffix + text[end..], start + 1, start + 1 + inner.Length);
        }

        var around = PaletteSuffix().Match(text, end);
        if (start > 0 && text[start - 1] == '[' && around.Success && around.Index == end)
            return new(text[..end] + suffix + text[(end + around.Length)..], start, end);

        return Wrap(text, start, end, "[", suffix, CnLabels.ColourPlaceholder);
    }

    private static CnTextEdit? Clear(string text, int start, int end)
    {
        if (start == end)
            return null;

        var selected = text[start..end];
        selected = PaletteSpan().Replace(selected, "$1");
        selected = MarkdownLink().Replace(selected, "$1");
        selected = selected.Replace("**", string.Empty).Replace("~~", string.Empty).Replace("*", string.Empty);
        selected = Underscores().Replace(selected, string.Empty);
        selected = HeadingPrefix().Replace(selected, string.Empty);
        return new(text[..start] + selected + text[end..], start, start + selected.Length);
    }

    /// <summary>Wraps the selection; spaces a double-click selected along stay
    /// outside the markers, where CommonMark needs them.</summary>
    private static CnTextEdit Wrap(string text, int start, int end, string before, string after, string placeholder)
    {
        var selected = text[start..end];
        var core = selected.Trim();
        if (core.Length == 0)
            core = placeholder;

        var lead = selected.Length - selected.TrimStart().Length;
        var trail = selected.Trim().Length == 0 ? 0 : selected.Length - selected.TrimEnd().Length;
        var from = start + lead;
        var to = end - trail;
        if (selected.Trim().Length == 0)
            (from, to) = (start, end);

        var result = text[..from] + before + core + after + text[to..];
        return new(result, from + before.Length, from + before.Length + core.Length);
    }

    private static CnTextEdit Lines(string text, int start, int end, Func<string[], string[]> transform)
    {
        var lineStart = start == 0 ? 0 : text.LastIndexOf('\n', start - 1) + 1;
        // A selection that ends at the very start of a line does not include it.
        var lastLineEnd = end > start && text[end - 1] == '\n' ? end - 1 : end;
        var lineEnd = text.IndexOf('\n', lastLineEnd);
        if (lineEnd < 0)
            lineEnd = text.Length;

        var block = string.Join('\n', transform(text[lineStart..lineEnd].Split('\n')));
        var result = text[..lineStart] + block + text[lineEnd..];
        return start == end
            ? new(result, lineStart + block.Length, lineStart + block.Length)
            : new(result, lineStart, lineStart + block.Length);
    }

    private static string[] Heading(string[] lines, string prefix)
    {
        var content = lines.Where(line => line.Trim().Length > 0).ToList();
        var remove = content.Count > 0 && content.All(line => line.StartsWith(prefix) && !line.StartsWith(prefix.TrimEnd() + "#"));
        return Transform(lines, line => remove ? line[prefix.Length..] : prefix + HeadingPrefix().Replace(line, string.Empty));
    }

    private static string[] BulletList(string[] lines)
    {
        var content = lines.Where(line => line.Trim().Length > 0).ToList();
        var remove = content.Count > 0 && content.All(line => BulletPrefix().IsMatch(line));
        return Transform(lines, line => remove
            ? BulletPrefix().Replace(line, "$1")
            : "- " + NumberPrefix().Replace(line, "$1").TrimStart());
    }

    private static string[] NumberedList(string[] lines)
    {
        var content = lines.Where(line => line.Trim().Length > 0).ToList();
        var remove = content.Count > 0 && content.All(line => NumberPrefix().IsMatch(line));
        var number = 0;
        return Transform(lines, line => remove
            ? NumberPrefix().Replace(line, "$1")
            : $"{++number}. " + BulletPrefix().Replace(line, "$1").TrimStart());
    }

    // Blank lines inside a multi-line selection stay blank; a single empty
    // line (just a caret) still gets the prefix to type after.
    private static string[] Transform(string[] lines, Func<string, string> change) =>
        lines.Length == 1
            ? [change(lines[0])]
            : lines.Select(line => line.Trim().Length == 0 ? line : change(line)).ToArray();

    [GeneratedRegex(@"\[([^\[\]\n]*)\]\{\.[A-Za-z][\w-]*\}")]
    private static partial Regex PaletteSpan();

    [GeneratedRegex(@"\]\{\.[A-Za-z][\w-]*\}")]
    private static partial Regex PaletteSuffix();

    [GeneratedRegex(@"\[([^\[\]\n]*)\]\([^)\n]*\)")]
    private static partial Regex MarkdownLink();

    [GeneratedRegex(@"(?<![\p{L}\p{N}])_+|_+(?![\p{L}\p{N}])")]
    private static partial Regex Underscores();

    [GeneratedRegex(@"^#{1,6}\s*", RegexOptions.Multiline)]
    private static partial Regex HeadingPrefix();

    [GeneratedRegex(@"^(\s*)[-*+] ")]
    private static partial Regex BulletPrefix();

    [GeneratedRegex(@"^(\s*)\d+[.)] ")]
    private static partial Regex NumberPrefix();

    [GeneratedRegex(@"^(https?://|mailto:)\S+$", RegexOptions.IgnoreCase)]
    private static partial Regex Url();

    [GeneratedRegex(@"^(#{1,6}\s|[-*+]\s|\d+[.)]\s|\||>)")]
    private static partial Regex BlockStart();
}
