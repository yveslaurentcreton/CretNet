using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace CretNet.RichText;

/// <summary>
/// How pasted or stored HTML is read, shared by <see cref="CnHtmlToMarkdown"/>
/// and <see cref="CnRichTextSanitizer"/> so both treat Word, Google Docs and
/// web fragments the same way.
/// </summary>
internal static partial class CnHtmlShape
{
    /// <summary>Removed with everything inside: never visible text.</summary>
    public static readonly HashSet<string> Skipped =
    [
        "script", "style", "meta", "link", "title", "head", "noscript", "template", "iframe", "object", "embed",
        "svg", "math", "canvas", "video", "audio", "input", "button", "select", "textarea", "o:p", "xml",
    ];

    /// <summary>Elements that start a new block; everything else flows inline.</summary>
    public static readonly HashSet<string> Blocks =
    [
        "p", "div", "section", "article", "blockquote", "header", "footer", "main", "aside", "nav", "figure",
        "figcaption", "address", "center", "dl", "dt", "dd", "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li",
        "table", "thead", "tbody", "tfoot", "tr", "td", "th", "caption", "hr", "pre", "form", "fieldset",
        "details", "summary", "body", "html",
    ];

    private static readonly HtmlParser Parser = new();

    public static IElement ParseBody(string html) =>
        Parser.ParseDocument(html).Body ?? throw new InvalidOperationException("The HTML parser produced no body.");

    public static bool IsBlock(IElement element) => Blocks.Contains(element.LocalName);

    public static bool HasBlockDescendant(IElement element) =>
        element.Children.Any(child => !Skipped.Contains(child.LocalName) && (IsBlock(child) || HasBlockDescendant(child)));

    /// <summary>Plain text whitespace: runs of spaces, tabs, newlines and
    /// non-breaking spaces become one space.</summary>
    public static string CollapseWhitespace(string text) => Whitespace().Replace(text, " ");

    /// <summary>
    /// Desktop Word does not paste lists as ul/ol but as paragraphs styled
    /// <c>mso-list:l0 level2 lfo1</c>, with the bullet in a
    /// <c>mso-list:Ignore</c> span. Returns the nesting level (1-based).
    /// </summary>
    public static int? WordListLevel(IElement element)
    {
        if (element.LocalName is not ("p" or "div" or "h1" or "h2" or "h3" or "h4" or "h5" or "h6"))
            return null;

        var style = CnInlineStyle.Parse(element.GetAttribute("style"));
        if (!style.TryGetValue("mso-list", out var value) || value.Equals("ignore", StringComparison.OrdinalIgnoreCase)
            || value.Equals("none", StringComparison.OrdinalIgnoreCase))
            return null;

        var match = WordLevel().Match(value);
        return match.Success ? int.Parse(match.Groups[1].Value) : 1;
    }

    /// <summary>A Word list paragraph is numbered when its marker reads like
    /// "1.", "a)" or "iv." rather than a bullet glyph.</summary>
    public static bool IsWordListOrdered(IElement paragraph)
    {
        var marker = paragraph.Descendants<IElement>().FirstOrDefault(element => CnInlineStyle.Read(element).ListMarker);
        var text = CollapseWhitespace(marker?.TextContent ?? string.Empty).Trim();
        return OrderedMarker().IsMatch(text);
    }

    /// <summary>Consecutive Word list paragraphs, skipping the whitespace and
    /// conditional comments between them.</summary>
    public static List<(IElement Paragraph, int Level)> TakeWordList(IReadOnlyList<INode> nodes, ref int index)
    {
        var items = new List<(IElement, int)>();
        var i = index;
        while (i < nodes.Count)
        {
            var node = nodes[i];
            if (node is IElement element && WordListLevel(element) is { } level)
            {
                items.Add((element, level));
                index = i;
            }
            else if (!(node.NodeType == NodeType.Comment || (node.NodeType == NodeType.Text && string.IsNullOrWhiteSpace(node.TextContent))))
            {
                break;
            }

            i++;
        }

        return items;
    }

    [GeneratedRegex(@"[\s ]+")]
    private static partial Regex Whitespace();

    [GeneratedRegex(@"level(\d+)", RegexOptions.IgnoreCase)]
    private static partial Regex WordLevel();

    [GeneratedRegex(@"^\(?([0-9]+|[a-zA-Z]|[ivxlcdmIVXLCDM]+)[.)]$")]
    private static partial Regex OrderedMarker();
}

/// <summary>A list read from HTML ul/ol or from Word list paragraphs, before
/// it is written as Markdown or clean HTML.</summary>
internal sealed class CnListModel(bool ordered, int start = 1)
{
    public bool Ordered { get; } = ordered;

    public int Start { get; } = start;

    public List<CnListItemModel> Items { get; } = [];

    /// <summary>Nests Word list paragraphs by their level.</summary>
    public static List<CnListModel> FromLevels(IEnumerable<(int Level, bool Ordered, string Content)> entries)
    {
        var roots = new List<CnListModel>();
        var stack = new List<(int Level, CnListModel List)>();
        foreach (var (level, ordered, content) in entries)
        {
            while (stack.Count > 0 && stack[^1].Level > level)
                stack.RemoveAt(stack.Count - 1);

            if (stack.Count == 0 || stack[^1].Level < level)
            {
                var list = new CnListModel(ordered);
                if (stack.Count == 0)
                {
                    roots.Add(list);
                }
                else
                {
                    var parent = stack[^1].List;
                    if (parent.Items.Count == 0)
                        parent.Items.Add(new CnListItemModel(string.Empty));
                    parent.Items[^1].Children.Add(list);
                }

                stack.Add((level, list));
            }

            stack[^1].List.Items.Add(new CnListItemModel(content));
        }

        return roots;
    }
}

internal sealed class CnListItemModel(string content)
{
    public string Content { get; set; } = content;

    public List<CnListModel> Children { get; } = [];
}
