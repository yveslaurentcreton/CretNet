using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;

namespace CretNet.RichText;

/// <summary>Formatting that Markdown (and so the document) cannot carry and that
/// a conversion left out; the editor names these after a paste.</summary>
public enum CnDroppedFormatting
{
    Underline,
    FontFamilyOrSize,
    BackgroundColor,
    Image,
    UnsupportedLink,
}

/// <param name="Markdown">The converted text, in <see cref="CnMarkdown"/> syntax.</param>
/// <param name="Dropped">What was left out, each kind once, in order of appearance.</param>
public sealed record CnHtmlToMarkdownResult(string Markdown, IReadOnlyList<CnDroppedFormatting> Dropped);

/// <summary>
/// Converts pasted or stored HTML (Word, Google Docs, web pages, the old
/// contenteditable editors) to Markdown for <see cref="CnMarkdown"/>.
/// </summary>
/// <remarks>
/// Bold and italic come from tags and from inline styles (weight 600+, italic);
/// Google Docs' <c>&lt;b style="font-weight:normal"&gt;</c> wrapper is not bold.
/// h1/h2 become <c>##</c>, h3–h6 <c>###</c>. Lists nest by indenting under the
/// parent's marker (two spaces under <c>- </c>, three under <c>1. </c>),
/// including Word's <c>mso-list</c> paragraphs. http, https and mailto links
/// are kept, tables become pipe tables, colours map to the nearest palette
/// colour (greys, black and white to none). Loose text between blocks becomes
/// its own paragraph and text that looks like Markdown is escaped, so nothing
/// is lost or reinterpreted.
/// </remarks>
public static partial class CnHtmlToMarkdown
{
    // Stands for a hard line break until the final text is assembled, so
    // whitespace clean-up cannot eat the two trailing spaces of "  \n".
    private const char Break = '\u0001';

    public static CnHtmlToMarkdownResult Convert(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return new(string.Empty, []);

        var converter = new Converter();
        var blocks = new List<string>();
        converter.Blocks(CnHtmlShape.ParseBody(html).ChildNodes.ToList(), default, blocks);

        var markdown = string.Join("\n\n", blocks.Where(block => block.Length > 0)).Replace(Break.ToString(), "  \n");
        markdown = BlankLines().Replace(markdown, "\n\n").Trim('\n');
        return new(markdown, converter.Dropped);
    }

    private readonly record struct State(bool Bold, bool Italic, bool Strike, CnPaletteColor? Color);

    private sealed class Converter
    {
        public List<CnDroppedFormatting> Dropped { get; } = [];

        public void Blocks(IReadOnlyList<INode> nodes, State state, List<string> output)
        {
            var run = new List<INode>();

            void Flush()
            {
                if (run.Count == 0)
                    return;

                var text = Paragraph(Run(run, state));
                run.Clear();
                if (text.Length > 0)
                    output.Add(text);
            }

            for (var i = 0; i < nodes.Count; i++)
            {
                var node = nodes[i];
                if (node is not IElement element)
                {
                    if (node.NodeType == NodeType.Text)
                        run.Add(node);
                    continue;
                }

                var tag = element.LocalName;
                if (CnHtmlShape.Skipped.Contains(tag))
                    continue;

                if (CnHtmlShape.WordListLevel(element) is not null)
                {
                    Flush();
                    var entries = CnHtmlShape.TakeWordList(nodes, ref i)
                        .Select(item => (item.Level, CnHtmlShape.IsWordListOrdered(item.Paragraph), ItemText([.. item.Paragraph.ChildNodes], Next(state, item.Paragraph))))
                        .ToList();
                    foreach (var list in CnListModel.FromLevels(entries))
                        output.Add(List(list));
                    continue;
                }

                switch (tag)
                {
                    case "h1" or "h2" or "h3" or "h4" or "h5" or "h6":
                        Flush();
                        // A heading is bold by itself: no ** inside it.
                        var headingState = Next(state, element);
                        var heading = Paragraph(Wrap(Inline([.. element.ChildNodes], headingState with { Bold = true }), default, headingState with { Bold = false }))
                            .Replace(Break, ' ');
                        if (heading.Length > 0)
                            output.Add((tag is "h1" or "h2" ? "## " : "### ") + heading);
                        break;
                    case "ul" or "ol":
                        Flush();
                        output.Add(List(ReadList(element, state)));
                        break;
                    case "table":
                        Flush();
                        output.Add(Table(element, state));
                        break;
                    case "hr":
                        Flush();
                        output.Add("---");
                        break;
                    case "pre":
                        Flush();
                        var lines = element.TextContent.Replace("\r\n", "\n").Split('\n')
                            .Select(line => Escape(CnHtmlShape.CollapseWhitespace(line).Trim()));
                        output.Add(Paragraph(string.Join(Break, lines)));
                        break;
                    case "img":
                        Drop(CnDroppedFormatting.Image);
                        break;
                    default:
                        if (CnHtmlShape.IsBlock(element) || CnHtmlShape.HasBlockDescendant(element))
                        {
                            Flush();
                            var inner = Next(state, element);
                            if (CnHtmlShape.HasBlockDescendant(element))
                                Blocks([.. element.ChildNodes], inner, output);
                            else if (Paragraph(Run([.. element.ChildNodes], inner)) is { Length: > 0 } text)
                                output.Add(text);
                        }
                        else
                        {
                            run.Add(element);
                        }
                        break;
                }
            }

            Flush();
        }

        /// <summary>Inline nodes as one piece of Markdown, wrapped in the
        /// formatting inherited from unwrapped block containers.</summary>
        private string Run(IReadOnlyList<INode> nodes, State state) =>
            Wrap(Inline(nodes, state), default, state);

        private string Inline(IReadOnlyList<INode> nodes, State state)
        {
            var text = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node.NodeType == NodeType.Text)
                {
                    text.Append(Escape(CnHtmlShape.CollapseWhitespace(node.TextContent)));
                    continue;
                }

                if (node is not IElement element || CnHtmlShape.Skipped.Contains(element.LocalName))
                    continue;

                switch (element.LocalName)
                {
                    case "br":
                        text.Append(Break);
                        continue;
                    case "img":
                        Drop(CnDroppedFormatting.Image);
                        continue;
                }

                var style = CnInlineStyle.Read(element);
                if (style.ListMarker)
                    continue;

                var inner = Next(state, element);
                var content = Inline([.. element.ChildNodes], inner);
                if (element.LocalName == "a")
                    content = Link(element, content);

                text.Append(Wrap(content, state, inner));
            }

            return text.ToString();
        }

        private string Link(IElement anchor, string content)
        {
            var href = anchor.GetAttribute("href")?.Trim();
            if (string.IsNullOrEmpty(href) || href.StartsWith('#'))
                return content;

            var label = content.Replace(Break, ' ').Trim();
            if (!CnMarkdown.IsSafeUrl(href))
            {
                Drop(CnDroppedFormatting.UnsupportedLink);
                return content;
            }

            if (label.Length == 0)
                return content;

            var url = href.Replace(" ", "%20").Replace("(", "%28").Replace(")", "%29").Replace("<", "%3C").Replace(">", "%3E");
            return Lead(content) + "[" + label + "](" + url + ")" + Trail(content);
        }

        private State Next(State parent, IElement element)
        {
            var style = CnInlineStyle.Read(element);
            if (style.Underline)
                Drop(CnDroppedFormatting.Underline);
            if (style.Font)
                Drop(CnDroppedFormatting.FontFamilyOrSize);
            if (style.Background || element.LocalName == "mark")
                Drop(CnDroppedFormatting.BackgroundColor);

            var color = parent.Color;
            if (style.Color is not null && CnCssColor.TryParse(style.Color, out var r, out var g, out var b))
                color = CnMarkdownPalette.Nearest(r, g, b);

            return new(
                style.Bold ?? parent.Bold,
                style.Italic ?? parent.Italic,
                parent.Strike || style.Strikethrough,
                color);
        }

        /// <summary>Adds the markers <paramref name="state"/> has beyond
        /// <paramref name="outer"/>. Surrounding spaces move outside, because
        /// <c>** bold**</c> is not bold in CommonMark.</summary>
        private static string Wrap(string content, State outer, State state)
        {
            var core = content.Trim(' ', Break);
            if (core.Length == 0)
                return content.Contains(' ') ? " " : content.Contains(Break) ? Break.ToString() : string.Empty;

            if (state.Strike && !outer.Strike)
                core = "~~" + core + "~~";
            if (state.Bold && !outer.Bold)
                core = "**" + core + "**";
            if (state.Italic && !outer.Italic)
                core = "*" + core + "*";
            if (state.Color is not null && state.Color != outer.Color)
                core = "[" + core + "]{." + state.Color.Name + "}";

            return Lead(content) + core + Trail(content);
        }

        private static string Lead(string content) => content.Length > 0 && content[0] is ' ' or Break ? content[0].ToString() : string.Empty;

        private static string Trail(string content) => content.Length > 0 && content[^1] is ' ' or Break ? content[^1].ToString() : string.Empty;

        private CnListModel ReadList(IElement element, State state)
        {
            var list = new CnListModel(element.LocalName == "ol", int.TryParse(element.GetAttribute("start"), out var start) ? start : 1);
            foreach (var child in element.ChildNodes)
            {
                switch (child)
                {
                    case IElement { LocalName: "li" } item:
                        var content = new List<INode>();
                        var nested = new List<CnListModel>();
                        foreach (var part in item.ChildNodes)
                        {
                            if (part is IElement { LocalName: "ul" or "ol" } sublist)
                                nested.Add(ReadList(sublist, state));
                            else
                                content.Add(part);
                        }

                        var model = new CnListItemModel(ItemText(content, Next(state, item)));
                        model.Children.AddRange(nested);
                        list.Items.Add(model);
                        break;
                    case IElement { LocalName: "ul" or "ol" } sublist:
                        // Invalid but common: a list directly inside a list
                        // belongs to the item before it.
                        if (list.Items.Count == 0)
                            list.Items.Add(new CnListItemModel(string.Empty));
                        list.Items[^1].Children.Add(ReadList(sublist, state));
                        break;
                    case IElement other when !CnHtmlShape.Skipped.Contains(other.LocalName):
                        list.Items.Add(new CnListItemModel(ItemText([other], state)));
                        break;
                    case { NodeType: NodeType.Text } text when !string.IsNullOrWhiteSpace(text.TextContent):
                        list.Items.Add(new CnListItemModel(ItemText([text], state)));
                        break;
                }
            }

            return list;
        }

        /// <summary>An item's own text; paragraphs inside it are kept apart
        /// with line breaks.</summary>
        private string ItemText(IReadOnlyList<INode> nodes, State state)
        {
            var parts = new List<string>();
            Blocks(nodes, state, parts);
            return string.Join(Break, parts);
        }

        private static string List(CnListModel list, string indent = "")
        {
            var lines = new List<string>();
            var number = list.Start;
            foreach (var item in list.Items)
            {
                var marker = list.Ordered ? $"{number++}. " : "- ";
                var childIndent = indent + new string(' ', marker.Length);
                var itemLines = item.Content.Split(Break);
                lines.Add((indent + marker + itemLines[0]).TrimEnd());
                foreach (var line in itemLines.Skip(1))
                {
                    lines[^1] += "  ";
                    lines.Add(childIndent + line);
                }

                lines.AddRange(item.Children.Select(child => List(child, childIndent)));
            }

            return string.Join('\n', lines);
        }

        private string Table(IElement table, State state)
        {
            var rows = table.QuerySelectorAll("tr")
                .Where(row => row.Closest("table") == table)
                .Select(row => row.Children
                    .Where(cell => cell.LocalName is "td" or "th")
                    .Select(cell => Cell(cell, state))
                    .ToList())
                .Where(row => row.Count > 0)
                .ToList();
            if (rows.Count == 0)
                return string.Empty;

            var width = rows.Max(row => row.Count);
            foreach (var row in rows)
                row.AddRange(Enumerable.Repeat(string.Empty, width - row.Count));

            var lines = new List<string> { Row(rows[0]), Row(Enumerable.Repeat("---", width)) };
            lines.AddRange(rows.Skip(1).Select(Row));
            return string.Join('\n', lines);

            static string Row(IEnumerable<string> cells) => "| " + string.Join(" | ", cells) + " |";
        }

        private string Cell(IElement cell, State state)
        {
            var parts = new List<string>();
            Blocks([.. cell.ChildNodes], Next(state, cell), parts);
            // A pipe-table cell is one line; its pipes must not split it.
            return string.Join(' ', parts).Replace(Break, ' ').Replace("\n", " ").Replace("|", "\\|").Trim();
        }

        private void Drop(CnDroppedFormatting kind)
        {
            if (!Dropped.Contains(kind))
                Dropped.Add(kind);
        }

        /// <summary>One paragraph: tidy spacing, no leading/trailing breaks, and
        /// each line protected against reading as a heading, list or quote.</summary>
        private static string Paragraph(string text)
        {
            text = Spaces().Replace(text, " ");
            text = SpacedBreak().Replace(text, Break.ToString()).Trim(' ', Break);
            return string.Join(Break, text.Split(Break).Select(line => EscapeLineStart(line.Trim())));
        }
    }

    /// <summary>Escapes characters that would otherwise start Markdown
    /// formatting inside ordinary text.</summary>
    private static string Escape(string text)
    {
        var result = new StringBuilder(text.Length + 8);
        for (var i = 0; i < text.Length; i++)
        {
            var c = text[i];
            var previous = i > 0 ? text[i - 1] : ' ';
            var next = i + 1 < text.Length ? text[i + 1] : ' ';
            var escape = c switch
            {
                '\\' or '*' or '`' or '[' or ']' or '<' => true,
                // snake_case stays readable: intraword underscores never emphasise.
                '_' => !(char.IsLetterOrDigit(previous) && char.IsLetterOrDigit(next)),
                '~' => previous == '~' || next == '~',
                '&' => Entity().IsMatch(text.AsSpan(i)),
                _ => false,
            };
            if (escape)
                result.Append('\\');
            result.Append(c);
        }

        return result.ToString();
    }

    private static string EscapeLineStart(string line)
    {
        if (line.Length == 0)
            return line;

        if (Rule().IsMatch(line))
            return "\\" + line;

        var ordered = OrderedStart().Match(line);
        if (ordered.Success)
            return line.Insert(ordered.Groups[1].Length, "\\");

        return line[0] switch
        {
            '#' or '>' => "\\" + line,
            '-' or '+' when line.Length == 1 || line[1] == ' ' => "\\" + line,
            _ => line,
        };
    }

    [GeneratedRegex(@"\n{3,}")]
    private static partial Regex BlankLines();

    [GeneratedRegex(@" {2,}")]
    private static partial Regex Spaces();

    [GeneratedRegex(" ?\u0001 ?")]
    private static partial Regex SpacedBreak();

    [GeneratedRegex(@"^&#?[A-Za-z0-9]+;")]
    private static partial Regex Entity();

    [GeneratedRegex(@"^(=+|-+)$")]
    private static partial Regex Rule();

    [GeneratedRegex(@"^(\d{1,9})[.)](\s|$)")]
    private static partial Regex OrderedStart();
}
