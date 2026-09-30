using System.Net;
using System.Text;
using AngleSharp.Dom;

namespace CretNet.RichText;

/// <summary>
/// Reduces HTML to the rich-text allow-list of <c>CnRichTextEditor</c> (CretNet
/// S-348), for servers that store HTML and for the editor's paste.
/// </summary>
/// <remarks>
/// <para>Allowed: <c>p</c>, <c>br</c>, <c>h2</c>, <c>h3</c>, <c>strong</c>/<c>b</c>,
/// <c>em</c>/<c>i</c>, <c>u</c>, nested <c>ul</c>/<c>ol</c>/<c>li</c>,
/// <c>a[href]</c> with http, https or mailto, and <c>span[data-color]</c> with a
/// palette name. No other attribute survives, so no <c>style</c>, <c>on*</c>
/// handler or <c>javascript:</c> URL can be stored.</para>
/// <para>Before styles are dropped their meaning is kept: weight 600+ becomes
/// <c>strong</c>, italic <c>em</c>, underline <c>u</c>, a colour the nearest
/// palette <c>span[data-color]</c>; Google Docs' <c>&lt;b style="font-weight:normal"&gt;</c>
/// wrapper becomes nothing. h1 maps to h2, h4–h6 to h3, other blocks (div,
/// section, Word list paragraphs, …) to paragraphs or lists, and loose text
/// between blocks is wrapped in a paragraph. Script-like elements are removed
/// with their content; everything else is unwrapped to its text. The result is
/// stable: sanitising it again changes nothing.</para>
/// </remarks>
public static class CnRichTextSanitizer
{
    /// <summary>The elements a sanitised value can contain.</summary>
    public static IReadOnlySet<string> AllowedElements { get; } =
        new HashSet<string>(["p", "br", "h2", "h3", "strong", "b", "em", "i", "u", "ul", "ol", "li", "a", "span"]);

    public static string Sanitize(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return string.Empty;

        var output = new StringBuilder();
        new Writer().Blocks([.. CnHtmlShape.ParseBody(html).ChildNodes], default, output);
        return output.ToString();
    }

    private readonly record struct State(bool Bold, bool Italic, bool Underline, CnPaletteColor? Color);

    private sealed class Writer
    {
        public void Blocks(IReadOnlyList<INode> nodes, State state, StringBuilder output)
        {
            var run = new List<INode>();

            void Flush()
            {
                if (run.Count == 0)
                    return;

                var content = Trim(Wrap(Inline(run, state), default, state));
                run.Clear();
                if (content.Length > 0)
                    output.Append("<p>").Append(content).Append("</p>");
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
                        .Select(item => (item.Level, CnHtmlShape.IsWordListOrdered(item.Paragraph), ItemContent([.. item.Paragraph.ChildNodes], Next(state, item.Paragraph))))
                        .ToList();
                    foreach (var list in CnListModel.FromLevels(entries))
                        List(list, output);
                    continue;
                }

                switch (tag)
                {
                    case "h1" or "h2" or "h3" or "h4" or "h5" or "h6":
                        Flush();
                        var level = tag is "h1" or "h2" ? "h2" : "h3";
                        var headingState = Next(state, element);
                        var heading = Trim(Wrap(Inline([.. element.ChildNodes], headingState with { Bold = true }), default, headingState with { Bold = false }));
                        if (heading.Length > 0)
                            output.Append('<').Append(level).Append('>').Append(heading).Append("</").Append(level).Append('>');
                        break;
                    case "ul" or "ol":
                        Flush();
                        List(ReadList(element, state), output);
                        break;
                    case "tr":
                        Flush();
                        // Tables are not part of the allow-list: one paragraph per row.
                        var cells = element.Children
                            .Where(cell => cell.LocalName is "td" or "th")
                            .Select(cell => ItemContent([.. cell.ChildNodes], Next(state, cell)).Replace("<br>", " "))
                            .Where(cell => cell.Length > 0);
                        var row = string.Join(" · ", cells);
                        if (row.Length > 0)
                            output.Append("<p>").Append(row).Append("</p>");
                        break;
                    case "pre":
                        Flush();
                        var lines = element.TextContent.Replace("\r\n", "\n").Split('\n').Select(line => Encode(line.TrimEnd()));
                        var text = Trim(string.Join("<br>", lines));
                        if (text.Length > 0)
                            output.Append("<p>").Append(Wrap(text, default, Next(state, element))).Append("</p>");
                        break;
                    case "hr" or "img":
                        Flush();
                        break;
                    default:
                        if (CnHtmlShape.IsBlock(element) || CnHtmlShape.HasBlockDescendant(element))
                        {
                            Flush();
                            var inner = Next(state, element);
                            if (CnHtmlShape.HasBlockDescendant(element))
                                Blocks([.. element.ChildNodes], inner, output);
                            else if (Trim(Wrap(Inline([.. element.ChildNodes], inner), default, inner)) is { Length: > 0 } content)
                                output.Append("<p>").Append(content).Append("</p>");
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

        private string Inline(IReadOnlyList<INode> nodes, State state)
        {
            var html = new StringBuilder();
            foreach (var node in nodes)
            {
                if (node.NodeType == NodeType.Text)
                {
                    html.Append(Encode(node.TextContent));
                    continue;
                }

                if (node is not IElement element || CnHtmlShape.Skipped.Contains(element.LocalName) || element.LocalName == "img")
                    continue;

                if (element.LocalName == "br")
                {
                    html.Append("<br>");
                    continue;
                }

                if (CnInlineStyle.Read(element).ListMarker)
                    continue;

                var inner = Next(state, element);
                var content = Inline([.. element.ChildNodes], inner);
                if (element.LocalName == "a" && element.GetAttribute("href") is { } href && CnMarkdown.IsSafeUrl(href) && content.Trim().Length > 0)
                    content = "<a href=\"" + WebUtility.HtmlEncode(href.Trim()) + "\">" + content + "</a>";

                html.Append(Wrap(content, state, inner));
            }

            return html.ToString();
        }

        private static State Next(State parent, IElement element)
        {
            var style = CnInlineStyle.Read(element);
            var color = parent.Color;
            if (element.LocalName == "span" && CnMarkdownPalette.Find(element.GetAttribute("data-color")) is { } palette)
                color = palette;
            else if (style.Color is not null && CnCssColor.TryParse(style.Color, out var r, out var g, out var b))
                color = CnMarkdownPalette.Nearest(r, g, b);

            return new(style.Bold ?? parent.Bold, style.Italic ?? parent.Italic, parent.Underline || style.Underline, color);
        }

        private static string Wrap(string content, State outer, State state)
        {
            if (Trim(content).Length == 0)
                return content;

            if (state.Underline && !outer.Underline)
                content = "<u>" + content + "</u>";
            if (state.Italic && !outer.Italic)
                content = "<em>" + content + "</em>";
            if (state.Bold && !outer.Bold)
                content = "<strong>" + content + "</strong>";
            if (state.Color is not null && state.Color != outer.Color)
                content = "<span data-color=\"" + state.Color.Name + "\">" + content + "</span>";
            return content;
        }

        private CnListModel ReadList(IElement element, State state)
        {
            var list = new CnListModel(element.LocalName == "ol");
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

                        var model = new CnListItemModel(ItemContent(content, Next(state, item)));
                        model.Children.AddRange(nested);
                        list.Items.Add(model);
                        break;
                    case IElement { LocalName: "ul" or "ol" } sublist:
                        if (list.Items.Count == 0)
                            list.Items.Add(new CnListItemModel(string.Empty));
                        list.Items[^1].Children.Add(ReadList(sublist, state));
                        break;
                    case IElement other when !CnHtmlShape.Skipped.Contains(other.LocalName):
                        list.Items.Add(new CnListItemModel(ItemContent([other], state)));
                        break;
                    case { NodeType: NodeType.Text } text when !string.IsNullOrWhiteSpace(text.TextContent):
                        list.Items.Add(new CnListItemModel(ItemContent([text], state)));
                        break;
                }
            }

            return list;
        }

        /// <summary>Inline content of a list item or cell: its paragraphs are
        /// joined with line breaks, because an li holds no p here.</summary>
        private string ItemContent(IReadOnlyList<INode> nodes, State state)
        {
            var blocks = new StringBuilder();
            Blocks(nodes, state, blocks);
            var html = blocks.ToString();
            if (html.Length == 0)
                return string.Empty;

            return Trim(html
                .Replace("</p><p>", "<br>").Replace("</h2>", "<br>").Replace("</h3>", "<br>")
                .Replace("<p>", string.Empty).Replace("</p>", string.Empty)
                .Replace("<h2>", string.Empty).Replace("<h3>", string.Empty));
        }

        private static void List(CnListModel list, StringBuilder output)
        {
            var tag = list.Ordered ? "ol" : "ul";
            output.Append('<').Append(tag).Append('>');
            foreach (var item in list.Items)
            {
                output.Append("<li>").Append(item.Content);
                foreach (var child in item.Children)
                    List(child, output);
                output.Append("</li>");
            }

            output.Append("</").Append(tag).Append('>');
        }

        // Source line breaks and tabs are layout, not content; typed double
        // spaces arrive as &nbsp; and are kept.
        private static string Encode(string text)
        {
            var html = new StringBuilder(text.Length);
            var previousSpace = false;
            foreach (var c in text)
            {
                var space = c is ' ' or '\t' or '\r' or '\n';
                if (space && previousSpace)
                    continue;

                previousSpace = space;
                html.Append(c switch
                {
                    '&' => "&amp;",
                    '<' => "&lt;",
                    '>' => "&gt;",
                    '"' => "&quot;",
                    ' ' => "&nbsp;",
                    _ when space => " ",
                    _ => c.ToString(),
                });
            }

            return html.ToString();
        }

        /// <summary>Removes whitespace and line breaks around content.</summary>
        private static string Trim(string html)
        {
            var result = html.Trim();
            bool changed;
            do
            {
                changed = false;
                foreach (var edge in new[] { "<br>", "&nbsp;" })
                {
                    if (result.StartsWith(edge, StringComparison.Ordinal))
                    {
                        result = result[edge.Length..].TrimStart();
                        changed = true;
                    }

                    if (result.EndsWith(edge, StringComparison.Ordinal))
                    {
                        result = result[..^edge.Length].TrimEnd();
                        changed = true;
                    }
                }
            } while (changed);

            return result;
        }
    }
}
