using System.Globalization;

namespace CretNet.RichText;

/// <summary>
/// Reads the CSS colour notations that pasted Word, Google Docs and web
/// fragments use: hex, rgb()/rgba() and the common colour names.
/// </summary>
internal static class CnCssColor
{
    private static readonly Dictionary<string, string> Names = new(StringComparer.OrdinalIgnoreCase)
    {
        ["black"] = "#000000", ["white"] = "#ffffff", ["windowtext"] = "#000000",
        ["gray"] = "#808080", ["grey"] = "#808080", ["silver"] = "#c0c0c0", ["darkgray"] = "#a9a9a9",
        ["darkgrey"] = "#a9a9a9", ["dimgray"] = "#696969", ["dimgrey"] = "#696969", ["lightgray"] = "#d3d3d3",
        ["red"] = "#ff0000", ["darkred"] = "#8b0000", ["maroon"] = "#800000", ["crimson"] = "#dc143c",
        ["firebrick"] = "#b22222", ["indianred"] = "#cd5c5c", ["brown"] = "#a52a2a", ["tomato"] = "#ff6347",
        ["orangered"] = "#ff4500", ["orange"] = "#ffa500", ["darkorange"] = "#ff8c00", ["chocolate"] = "#d2691e",
        ["goldenrod"] = "#daa520", ["gold"] = "#ffd700", ["yellow"] = "#ffff00", ["olive"] = "#808000",
        ["green"] = "#008000", ["darkgreen"] = "#006400", ["forestgreen"] = "#228b22", ["seagreen"] = "#2e8b57",
        ["mediumseagreen"] = "#3cb371", ["limegreen"] = "#32cd32", ["lime"] = "#00ff00", ["teal"] = "#008080",
        ["blue"] = "#0000ff", ["navy"] = "#000080", ["darkblue"] = "#00008b", ["mediumblue"] = "#0000cd",
        ["royalblue"] = "#4169e1", ["steelblue"] = "#4682b4", ["dodgerblue"] = "#1e90ff",
        ["cornflowerblue"] = "#6495ed", ["midnightblue"] = "#191970", ["aqua"] = "#00ffff", ["cyan"] = "#00ffff",
        ["purple"] = "#800080", ["fuchsia"] = "#ff00ff", ["magenta"] = "#ff00ff",
    };

    public static bool TryParse(string? value, out int r, out int g, out int b)
    {
        r = g = b = 0;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var text = value.Trim().TrimEnd(';').Replace("!important", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();
        if (Names.TryGetValue(text, out var named))
            text = named;

        if (text.StartsWith('#'))
            return TryParseHex(text[1..], out r, out g, out b);

        if (text.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
            return TryParseRgb(text, out r, out g, out b);

        // Legacy HTML colour attributes sometimes omit the '#'.
        return text.Length == 6 && TryParseHex(text, out r, out g, out b);
    }

    private static bool TryParseHex(string hex, out int r, out int g, out int b)
    {
        r = g = b = 0;
        if (hex.Length is 3 or 4)
            hex = string.Concat(hex.Select(c => new string(c, 2)));

        if (hex.Length is not (6 or 8) || !int.TryParse(hex[..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb))
            return false;

        // A fully transparent colour paints nothing.
        if (hex.Length == 8 && hex[6..] == "00")
            return false;

        r = (rgb >> 16) & 0xff;
        g = (rgb >> 8) & 0xff;
        b = rgb & 0xff;
        return true;
    }

    private static bool TryParseRgb(string text, out int r, out int g, out int b)
    {
        r = g = b = 0;
        var open = text.IndexOf('(');
        var close = text.LastIndexOf(')');
        if (open < 0 || close <= open)
            return false;

        var parts = text[(open + 1)..close]
            .Split([',', ' ', '/'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length < 3)
            return false;

        if (parts.Length > 3 && double.TryParse(parts[3].TrimEnd('%'), NumberStyles.Float, CultureInfo.InvariantCulture, out var alpha) && alpha <= 0)
            return false;

        return TryChannel(parts[0], 255, out r) && TryChannel(parts[1], 255, out g) && TryChannel(parts[2], 255, out b);
    }

    private static bool TryChannel(string part, int scale, out int value)
    {
        value = 0;
        var percent = part.EndsWith('%');
        if (!double.TryParse(percent ? part[..^1] : part, NumberStyles.Float, CultureInfo.InvariantCulture, out var number))
            return false;

        value = (int)Math.Round(Math.Clamp(percent ? number / 100 * scale : number, 0, scale));
        return true;
    }
}
