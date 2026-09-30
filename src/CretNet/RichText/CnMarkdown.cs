using System.Text;
using Markdig;
using Markdig.Extensions.Tables;
using Markdig.Helpers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace CretNet.RichText;

/// <summary>
/// The one Markdown interpretation for rich business text (HCMT ADR-054): the
/// editor preview, PDF renderers and search indexing all go through this
/// pipeline, so what the preview shows is what prints.
/// </summary>
/// <remarks>
/// <para>CommonMark (paragraphs, lists, emphasis, links, quotes, code, breaks)
/// plus strikethrough (<c>~~x~~</c>) and pipe tables. Raw HTML is disabled: it
/// stays literal text. Bare URLs are not linked; <c>&lt;https://…&gt;</c> is.</para>
/// <para>The parsed document is normalised before anyone sees it, so every
/// consumer walks the same tree:</para>
/// <list type="bullet">
/// <item><c>[text]{.name}</c> becomes a <see cref="CnMarkdownColorInline"/> for a
/// palette name; any other name leaves the plain text without brackets.</item>
/// <item>Headings have level 2 or 3 only: h1 becomes h2, h4–h6 become h3.</item>
/// <item>Links other than http, https and mailto, and all images, become
/// their text.</item>
/// <item>No element carries attributes except safe link targets.</item>
/// <item>A single line break is a line break, as people expect when typing a
/// bold subtitle above its text; a blank line starts a new paragraph.</item>
/// </list>
/// </remarks>
public static class CnMarkdown
{
    /// <summary>The configured pipeline. Use it with Markdig's own renderers when
    /// needed; <see cref="Parse"/> and <see cref="ToSafeHtml"/> already apply it.</summary>
    public static MarkdownPipeline Pipeline { get; } = CreatePipeline();

    private static MarkdownPipeline CreatePipeline()
    {
        var builder = new MarkdownPipelineBuilder()
            .UseEmphasisExtras(Markdig.Extensions.EmphasisExtras.EmphasisExtraOptions.Strikethrough)
            .UsePipeTables()
            // Business text: a line typed under a bold subtitle stays on its own line.
            .UseSoftlineBreakAsHardlineBreak()
            .DisableHtml()
            .Use<CnPaletteExtension>();
        builder.DocumentProcessed += Normalize;
        return builder.Build();
    }

    /// <summary>Parses and normalises Markdown into the tree renderers walk.</summary>
    public static MarkdownDocument Parse(string? markdown) =>
        Markdig.Markdown.Parse(Clean(markdown), Pipeline);

    /// <summary>Renders Markdown to HTML that is safe to inject: no raw HTML,
    /// no script-capable URLs, palette colours as <c>cn-md-c-*</c> classes.</summary>
    public static string ToSafeHtml(string? markdown) =>
        string.IsNullOrWhiteSpace(markdown) ? string.Empty : Markdig.Markdown.ToHtml(Clean(markdown), Pipeline);

    /// <summary>The readable text without any Markdown syntax, one block per
    /// line, for search indexes and plain-text previews.</summary>
    public static string ToPlainText(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
            return string.Empty;

        var lines = new List<string>();
        foreach (var block in Parse(markdown))
            CollectText(block, lines);

        return string.Join('\n', lines.Select(line => line.Trim()).Where(line => line.Length > 0));
    }

    // A NUL is replaced by U+FFFD anyway; normalising line endings keeps
    // source positions and the editor's character counter in agreement.
    private static string Clean(string? markdown) =>
        (markdown ?? string.Empty).Replace("\r\n", "\n").Replace('\r', '\n');

    private static void Normalize(MarkdownDocument document)
    {
        foreach (var attribute in document.Descendants<CnPaletteAttributeInline>().ToList())
            ResolvePalette(attribute);

        foreach (var node in document.Descendants().ToList())
        {
            if (node is not CnMarkdownColorInline && node.TryGetAttributes() is not null)
                node.SetAttributes(new HtmlAttributes());

            switch (node)
            {
                case HeadingBlock heading:
                    heading.Level = heading.Level <= 2 ? 2 : 3;
                    break;
                case LinkInline link when link.IsImage || !IsSafeUrl(link.Url):
                    Unwrap(link);
                    break;
                case LinkInline link:
                    MarkExternal(link);
                    break;
                case AutolinkInline autolink when !autolink.IsEmail && !IsSafeUrl(autolink.Url):
                    autolink.ReplaceBy(new LiteralInline(autolink.Url) { Span = autolink.Span });
                    break;
                case AutolinkInline autolink:
                    MarkExternal(autolink);
                    break;
            }
        }
    }

    /// <summary>Only schemes that navigate: never javascript:, data:, relative
    /// paths or anything a renderer might resolve against a host.</summary>
    public static bool IsSafeUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        var trimmed = url.Trim();
        return trimmed.StartsWith("https://", StringComparison.OrdinalIgnoreCase)
               || trimmed.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
               || trimmed.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase);
    }

    private static void MarkExternal(Inline link)
    {
        var attributes = link.GetAttributes();
        attributes.AddPropertyIfNotExist("target", "_blank");
        attributes.AddPropertyIfNotExist("rel", "noopener noreferrer");
    }

    private static void ResolvePalette(CnPaletteAttributeInline attribute)
    {
        var closing = attribute.PreviousSibling as LiteralInline;
        var opening = closing is null ? null : FindOpeningBracket(closing);
        if (closing is null || opening is null)
        {
            // Not a bracketed text after all: keep what was typed.
            attribute.ReplaceBy(new LiteralInline(attribute.Source) { Span = attribute.Span });
            return;
        }

        TrimClosingBracket(closing);

        var color = CnMarkdownPalette.Find(attribute.ClassName);
        if (color is null)
        {
            // Unknown class: the plain text, without brackets or attribute.
            opening.Remove();
            attribute.Remove();
            return;
        }

        var span = new CnMarkdownColorInline
        {
            Color = color,
            Span = new SourceSpan(opening.Span.Start, attribute.Span.End),
            Line = opening.Line,
            Column = opening.Column,
        };

        var child = opening.NextSibling;
        opening.ReplaceBy(span);
        while (child is not null && child != attribute)
        {
            var next = child.NextSibling;
            child.Remove();
            span.AppendChild(child);
            child = next;
        }

        attribute.Remove();
    }

    /// <summary>The unmatched '[' that a failed link leaves as its own literal,
    /// skipping balanced inner brackets such as <c>[a [b] c]{.accent}</c>.</summary>
    private static LiteralInline? FindOpeningBracket(LiteralInline closing)
    {
        // The closing literal's own trailing ']' is the one being matched.
        var depth = Count(closing.Content.ToString()[..^1], ']') - Count(closing.Content.ToString()[..^1], '[');
        if (depth < 0)
            return null;

        for (var node = closing.PreviousSibling; node is not null; node = node.PreviousSibling)
        {
            if (node is not LiteralInline literal)
                continue;

            var content = literal.Content.ToString();
            if (content == "[" && IsUnescapedBracket(literal))
            {
                if (depth == 0)
                    return literal;
                depth--;
                continue;
            }

            depth += Count(content, ']') - Count(content, '[');
            if (depth < 0)
                return null;
        }

        return null;
    }

    // An escaped "\[" is also a one-character literal; its slice starts right
    // after the backslash.
    private static bool IsUnescapedBracket(LiteralInline literal) =>
        !(literal.Content.Start > 0 && literal.Content.Text is { } text && text[literal.Content.Start - 1] == '\\');

    private static int Count(string text, char c) => text.Count(x => x == c);

    private static void TrimClosingBracket(LiteralInline closing)
    {
        var content = closing.Content;
        if (content.Length <= 1)
        {
            closing.Remove();
            return;
        }

        closing.Content = new StringSlice(content.Text, content.Start, content.End - 1);
    }

    private static void Unwrap(ContainerInline container)
    {
        var child = container.FirstChild;
        while (child is not null)
        {
            var next = child.NextSibling;
            child.Remove();
            container.InsertBefore(child);
            child = next;
        }

        container.Remove();
    }

    private static void CollectText(Block block, List<string> lines)
    {
        switch (block)
        {
            case LeafBlock { Inline: { } inline }:
                var text = new StringBuilder();
                AppendText(inline, text);
                lines.AddRange(text.ToString().Split('\n'));
                break;
            case Table table:
                foreach (var row in table.OfType<TableRow>())
                {
                    var cells = row.OfType<TableCell>().Select(cell =>
                    {
                        var cellLines = new List<string>();
                        foreach (var child in cell)
                            CollectText(child, cellLines);
                        return string.Join(' ', cellLines).Trim();
                    });
                    lines.Add(string.Join(" ", cells.Where(cell => cell.Length > 0)));
                }
                break;
            case LeafBlock leaf when leaf is CodeBlock:
                lines.AddRange(leaf.Lines.Lines.Take(leaf.Lines.Count).Select(line => line.Slice.ToString()));
                break;
            case ContainerBlock container:
                foreach (var child in container)
                    CollectText(child, lines);
                break;
        }
    }

    private static void AppendText(Inline inline, StringBuilder text)
    {
        switch (inline)
        {
            case LiteralInline literal:
                text.Append(literal.Content.ToString());
                break;
            case CodeInline code:
                text.Append(code.Content);
                break;
            case AutolinkInline autolink:
                text.Append(autolink.Url);
                break;
            case HtmlEntityInline entity:
                text.Append(entity.Transcoded.ToString());
                break;
            case LineBreakInline lineBreak:
                text.Append(lineBreak.IsHard ? '\n' : ' ');
                break;
            case ContainerInline container:
                foreach (var child in container)
                    AppendText(child, text);
                break;
        }
    }
}
