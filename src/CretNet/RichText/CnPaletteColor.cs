namespace CretNet.RichText;

/// <summary>
/// One colour of the fixed rich-text palette. Text can only be coloured with
/// these, so nothing unexpected reaches a printed document.
/// </summary>
/// <param name="Name">Stored name, e.g. <c>accent</c> in <c>[text]{.accent}</c>
/// or <c>&lt;span data-color="accent"&gt;</c>.</param>
/// <param name="LightHex">Screen colour on a light background.</param>
/// <param name="DarkHex">Screen colour on a dark background.</param>
/// <param name="PrintHex">Colour on paper (PDF renderers).</param>
public sealed record CnPaletteColor(string Name, string LightHex, string DarkHex, string PrintHex)
{
    /// <summary>The class the Markdown preview puts on a coloured span.</summary>
    public string CssClass => $"cn-md-c-{Name}";

    /// <summary>The Cn token that resolves to <see cref="LightHex"/> or
    /// <see cref="DarkHex"/> for the active theme.</summary>
    public string CssVariable => $"--cn-md-{Name}";
}

/// <summary>
/// The rich-text palette shared by <c>CnMarkdown</c>, <c>CnHtmlToMarkdown</c>,
/// <c>CnRichTextSanitizer</c> and both Cn editors.
/// </summary>
public static class CnMarkdownPalette
{
    public static CnPaletteColor Accent { get; } = new("accent", "#128a30", "#3fbf5f", "#128a30");
    public static CnPaletteColor Blue { get; } = new("blauw", "#3159a7", "#6b8fd4", "#3159a7");
    public static CnPaletteColor Orange { get; } = new("oranje", "#b35f00", "#e8913a", "#b35f00");
    public static CnPaletteColor Red { get; } = new("rood", "#c42b2f", "#e4696d", "#c42b2f");

    /// <summary>All palette colours, in toolbar order.</summary>
    public static IReadOnlyList<CnPaletteColor> Colors { get; } = [Accent, Blue, Orange, Red];

    /// <summary>The palette colour with this stored name, or null.</summary>
    public static CnPaletteColor? Find(string? name) =>
        string.IsNullOrEmpty(name)
            ? null
            : Colors.FirstOrDefault(color => string.Equals(color.Name, name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Maps any CSS colour (hex, rgb(), a common name) to the nearest palette
    /// colour. Greys, black, white, transparent and unreadable values map to
    /// null: they are "no colour", not a faint version of one.
    /// </summary>
    public static CnPaletteColor? Nearest(string? cssColor) =>
        CnCssColor.TryParse(cssColor, out var r, out var g, out var b) ? Nearest(r, g, b) : null;

    /// <inheritdoc cref="Nearest(string?)"/>
    public static CnPaletteColor? Nearest(int r, int g, int b)
    {
        // Low saturation reads as text colour (black, grey, white), never as
        // a highlight; the same threshold as the approved editor mock.
        if (Math.Max(r, Math.Max(g, b)) - Math.Min(r, Math.Min(g, b)) < 40)
            return null;

        CnPaletteColor? best = null;
        var bestDistance = int.MaxValue;
        foreach (var color in Colors)
        {
            CnCssColor.TryParse(color.PrintHex, out var pr, out var pg, out var pb);
            var distance = (pr - r) * (pr - r) + (pg - g) * (pg - g) + (pb - b) * (pb - b);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                best = color;
            }
        }

        return best;
    }
}
