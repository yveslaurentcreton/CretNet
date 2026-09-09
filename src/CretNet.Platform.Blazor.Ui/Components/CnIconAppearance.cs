namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>Outline preserves the original Cn contract; coloured styles are opt-in.</summary>
public enum CnIconStyle { Outline, Natural, Category }

public enum CnIconIntensity { Normal, Quiet }

public enum CnNavigationIconSize { Small = 18, Medium = 22, Large = 26 }

/// <summary>Immutable presentation choices, with persistence owned by the host.</summary>
public sealed record CnIconAppearance(
    CnIconStyle Style = CnIconStyle.Outline,
    CnIconIntensity Intensity = CnIconIntensity.Normal,
    bool Depth = true,
    CnNavigationIconSize NavigationSize = CnNavigationIconSize.Medium);
