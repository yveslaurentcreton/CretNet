using System.Globalization;
using AngleSharp.Dom;

namespace CretNet.RichText;

/// <summary>
/// The formatting an element expresses through its tag or inline style, read
/// the same way for Markdown conversion and HTML sanitising. Pasted Word and
/// Google Docs content carries most of its meaning in <c>style</c>.
/// </summary>
internal readonly record struct CnInlineStyle(
    bool? Bold,
    bool? Italic,
    bool Underline,
    bool Strikethrough,
    string? Color,
    bool Font,
    bool Background,
    bool ListMarker)
{
    public static CnInlineStyle Read(IElement element)
    {
        var tag = element.LocalName;
        var style = Parse(element.GetAttribute("style"));

        bool? bold = null;
        if (style.TryGetValue("font-weight", out var weight))
            bold = IsBoldWeight(weight);
        if (tag is "b" or "strong")
            // Google Docs wraps the whole fragment in <b style="font-weight:normal">.
            bold = bold ?? true;

        bool? italic = null;
        if (style.TryGetValue("font-style", out var fontStyle))
            italic = fontStyle.StartsWith("italic", StringComparison.OrdinalIgnoreCase) || fontStyle.StartsWith("oblique", StringComparison.OrdinalIgnoreCase);
        if (tag is "i" or "em")
            italic = italic ?? true;

        var decoration = (style.GetValueOrDefault("text-decoration") ?? string.Empty)
                         + " " + (style.GetValueOrDefault("text-decoration-line") ?? string.Empty);
        var underline = tag is "u" or "ins" || decoration.Contains("underline", StringComparison.OrdinalIgnoreCase);
        var strike = tag is "s" or "strike" or "del" || decoration.Contains("line-through", StringComparison.OrdinalIgnoreCase);

        var color = style.GetValueOrDefault("color");
        if (color is null && tag == "font")
            color = element.GetAttribute("color");

        var font = style.ContainsKey("font-family") || style.ContainsKey("font-size") || style.ContainsKey("font")
                   || (tag == "font" && (element.HasAttribute("face") || element.HasAttribute("size")));
        var background = (style.TryGetValue("background-color", out var bg) || style.TryGetValue("background", out bg))
                         && CnCssColor.TryParse(bg, out var br, out var bgg, out var bb) && !(br > 245 && bgg > 245 && bb > 245);
        var marker = style.TryGetValue("mso-list", out var msoList) && msoList.Equals("ignore", StringComparison.OrdinalIgnoreCase);

        return new(bold, italic, underline, strike, color, font, background, marker);
    }

    /// <summary>Splits an inline style into lower-case property names and values.
    /// Only whole property names match, so <c>mso-bidi-font-weight</c> never reads
    /// as <c>font-weight</c>.</summary>
    public static Dictionary<string, string> Parse(string? style)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(style))
            return result;

        foreach (var declaration in style.Split(';'))
        {
            var colon = declaration.IndexOf(':');
            if (colon <= 0)
                continue;

            var name = declaration[..colon].Trim().ToLowerInvariant();
            var value = declaration[(colon + 1)..].Replace("!important", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
            if (name.Length > 0)
                result[name] = value;
        }

        return result;
    }

    private static bool IsBoldWeight(string weight)
    {
        if (weight.StartsWith("bold", StringComparison.OrdinalIgnoreCase))
            return true;

        return int.TryParse(weight, NumberStyles.Integer, CultureInfo.InvariantCulture, out var number) && number >= 600;
    }
}
