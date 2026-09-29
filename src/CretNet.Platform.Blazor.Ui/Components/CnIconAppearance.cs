namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Outline preserves the original Cn contract; coloured styles are opt-in.
/// Each coloured style combines a <see cref="CnIconFamily"/> with a <see cref="CnIconColorMode"/>;
/// values are appended so persisted names and numbers keep their meaning.
/// </summary>
public enum CnIconStyle
{
    Outline,
    /// <summary>Natural artwork in its own colours.</summary>
    Natural,
    /// <summary>Natural artwork in its category colour.</summary>
    Category,
    /// <summary>White porcelain body with one accent in the icon's own colour.</summary>
    Porcelain,
    /// <summary>White porcelain body with one accent in the category colour.</summary>
    PorcelainCategory,
    /// <summary>Thin strokes in the current text colour.</summary>
    Line,
    /// <summary>Thin strokes in a lightened category colour.</summary>
    LineCategory,
}

/// <summary>The drawing of an icon (the "style" axis of the icon settings).</summary>
public enum CnIconFamily { Outline, Natural, Porcelain, Line }

/// <summary>The colouring of an icon (the "colour" axis of the icon settings).</summary>
public enum CnIconColorMode { Own, Category }

public static class CnIconStyleExtensions
{
    public static CnIconFamily Family(this CnIconStyle style) => style switch
    {
        CnIconStyle.Natural or CnIconStyle.Category => CnIconFamily.Natural,
        CnIconStyle.Porcelain or CnIconStyle.PorcelainCategory => CnIconFamily.Porcelain,
        CnIconStyle.Line or CnIconStyle.LineCategory => CnIconFamily.Line,
        _ => CnIconFamily.Outline,
    };

    public static CnIconColorMode ColorMode(this CnIconStyle style) =>
        style is CnIconStyle.Category or CnIconStyle.PorcelainCategory or CnIconStyle.LineCategory
            ? CnIconColorMode.Category : CnIconColorMode.Own;

    /// <summary>Combines both axes; Outline has no colour mode.</summary>
    public static CnIconStyle Compose(CnIconFamily family, CnIconColorMode color) => (family, color) switch
    {
        (CnIconFamily.Natural, CnIconColorMode.Category) => CnIconStyle.Category,
        (CnIconFamily.Natural, _) => CnIconStyle.Natural,
        (CnIconFamily.Porcelain, CnIconColorMode.Category) => CnIconStyle.PorcelainCategory,
        (CnIconFamily.Porcelain, _) => CnIconStyle.Porcelain,
        (CnIconFamily.Line, CnIconColorMode.Category) => CnIconStyle.LineCategory,
        (CnIconFamily.Line, _) => CnIconStyle.Line,
        _ => CnIconStyle.Outline,
    };

    public static CnIconStyle WithFamily(this CnIconStyle style, CnIconFamily family) => Compose(family, style.ColorMode());

    public static CnIconStyle WithColorMode(this CnIconStyle style, CnIconColorMode color) => Compose(style.Family(), color);

    /// <summary>Depth (gradient and shadow) has no effect on thin strokes.</summary>
    public static bool SupportsDepth(this CnIconStyle style) => style.Family() is CnIconFamily.Natural or CnIconFamily.Porcelain;
}

public enum CnIconIntensity { Normal, Quiet }

public enum CnNavigationIconSize { Small = 18, Medium = 22, Large = 26 }

/// <summary>Immutable presentation choices, with persistence owned by the host.</summary>
public sealed record CnIconAppearance(
    CnIconStyle Style = CnIconStyle.Outline,
    CnIconIntensity Intensity = CnIconIntensity.Normal,
    bool Depth = true,
    CnNavigationIconSize NavigationSize = CnNavigationIconSize.Medium);
