using System.Collections.Concurrent;
using System.Globalization;
using System.Text;

namespace CretNet.Platform.Blazor.Ui.Components;

// Renders the drawn styles from the shared geometry in CnIconArtwork.Parts.cs (24x24 grid).
// Porcelain: white porcelain body with a thin outline tinted by the accent, grey details and
// one coloured accent, soft drop shadow. Line: the same geometry as thin round strokes.
// Intensity and depth stay CSS concerns (.cn-icon--quiet, .cn-icon--flat), like the artwork.
internal static partial class CnIconArtwork
{
    private const string PorcelainDefs =
        """<defs><linearGradient id="__CN_ICON__pap" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#f4f6f8">"""
        + """<stop class="cn-icon-tone-hi" stop-color="#ffffff"/><stop offset=".55" stop-color="#f4f6f8"/>"""
        + """<stop class="cn-icon-tone-lo" offset="1" stop-color="#dfe5eb"/></linearGradient>"""
        + """<filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">"""
        + """<feDropShadow dx="0" dy=".45" stdDeviation=".4" flood-color="#354354" flood-opacity=".22"/></filter></defs>""";

    private const string SolidMarker = " data-solid=\"1\"";
    private static readonly ConcurrentDictionary<(CnIconKind, CnIconStyle), string> DrawnCache = new();

    /// <summary>Porcelain or Line markup for the kind, or empty for any other style.</summary>
    internal static string Drawn(CnIconKind kind, CnIconStyle style) =>
        style.Family() is CnIconFamily.Porcelain or CnIconFamily.Line
            ? DrawnCache.GetOrAdd((kind, style), key => Draw(key.Item1, key.Item2))
            : string.Empty;

    private static string Draw(CnIconKind kind, CnIconStyle style)
    {
        var parts = Parts(kind);
        if (parts == default) return string.Empty;
        var byCategory = style.ColorMode() == CnIconColorMode.Category;
        return style.Family() == CnIconFamily.Line ? Line(parts, byCategory) : Porcelain(parts, byCategory ? parts.Category : parts.Own);
    }

    private static string Porcelain(IconParts parts, string accent)
    {
        var html = new StringBuilder(PorcelainDefs).Append("""<g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">""");
        var outline = Mix(accent, (150, 164, 178), .45);
        foreach (var element in Elements(parts.Body))
            html.Append(element).Append($""" fill="url(#__CN_ICON__pap)" stroke="{outline}" stroke-width="1.05" stroke-linejoin="round"/>""");
        foreach (var element in Elements(parts.Detail))
            html.Append(element).Append(IsClosed(element)
                ? """ fill="#c3ced8"/>"""
                : """ fill="none" stroke="#a3b1be" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round"/>""");
        var tint = Mix(accent, (255, 255, 255), .22);
        foreach (var element in Elements(parts.Accent))
        {
            if (element.Contains(SolidMarker, StringComparison.Ordinal))
            {
                html.Append(element.Replace(SolidMarker, string.Empty, StringComparison.Ordinal)).Append($""" fill="{accent}"/>""");
                continue;
            }
            html.Append(element).Append($""" fill="{(IsClosed(element) ? tint : "none")}" stroke="{accent}" stroke-width="1.8" stroke-linecap="round" stroke-linejoin="round"/>""");
        }
        return html.Append("</g>").ToString();
    }

    // Own colours follow the text colour (currentColor), so active/selected states keep their accent.
    private static string Line(IconParts parts, bool byCategory)
    {
        var stroke = byCategory ? Mix(parts.Category, (255, 255, 255), .82) : "currentColor";
        var html = new StringBuilder($"""<g fill="none" stroke="{stroke}" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round">""");
        foreach (var element in Elements(parts.Body + parts.Detail + parts.Accent))
            html.Append(element.Replace(SolidMarker, string.Empty, StringComparison.Ordinal)).Append("/>");
        return html.Append("</g>").ToString();
    }

    /// <summary>Self-closing elements without their closing "/>".</summary>
    private static IEnumerable<string> Elements(string markup) =>
        markup.Split("/>", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static bool IsClosed(string element) =>
        element.StartsWith("<circle", StringComparison.Ordinal) || element.StartsWith("<rect", StringComparison.Ordinal)
        || element.StartsWith("<ellipse", StringComparison.Ordinal)
        || element.EndsWith("z\"", StringComparison.OrdinalIgnoreCase);

    /// <summary>colour * weight + other * (1 - weight), as #rrggbb.</summary>
    private static string Mix(string hex, (int R, int G, int B) other, double weight)
    {
        static int Channel(string hex, int index) => int.Parse(hex.AsSpan(index, 2), NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        static int Blend(int value, int other, double weight) => (int)Math.Round(value * weight + other * (1 - weight), MidpointRounding.AwayFromZero);
        return string.Create(CultureInfo.InvariantCulture,
            $"#{Blend(Channel(hex, 1), other.R, weight):x2}{Blend(Channel(hex, 3), other.G, weight):x2}{Blend(Channel(hex, 5), other.B, weight):x2}");
    }
}
