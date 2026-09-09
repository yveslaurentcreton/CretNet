namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>The placement of an icon, independent of the symbol it depicts.</summary>
public enum CnIconRole { Action, Entity }

/// <summary>Monochrome and Functional use outlines; Colored and Natural use object artwork.</summary>
public enum CnActionIconStyle { Monochrome, Functional, Colored, Natural }

/// <summary>Independent action/system preferences; hosts own account persistence.</summary>
public sealed record CnActionIconAppearance(
    CnActionIconStyle Style = CnActionIconStyle.Monochrome,
    bool Depth = false,
    bool DestructiveColor = true)
{
    public bool SupportsDepth => Style is CnActionIconStyle.Colored or CnActionIconStyle.Natural;

    internal CnIconAppearance For(CnIconKind kind)
    {
        var style = Style switch
        {
            CnActionIconStyle.Colored => CnIconStyle.Category,
            CnActionIconStyle.Natural => CnIconStyle.Natural,
            _ => CnIconStyle.Outline,
        };

        // Both object palettes share this silhouette. Choose its red/steel
        // material explicitly so the destructive toggle changes the actual paint.
        if (kind == CnIconKind.Delete && SupportsDepth)
            style = DestructiveColor ? CnIconStyle.Category : CnIconStyle.Natural;

        return new(style, Depth: SupportsDepth && Depth);
    }

    internal string? Tone(CnIconKind kind)
    {
        if (kind == CnIconKind.Delete) return DestructiveColor ? "danger" : null;
        // Status symbols retain their semantic meaning in both outline modes.
        var status = kind switch
        {
            CnIconKind.ErrorCircle => "danger",
            CnIconKind.Warning => "warning",
            CnIconKind.CheckCircle => "success",
            CnIconKind.InfoCircle => "info",
            _ => null,
        };
        if (status is not null || Style != CnActionIconStyle.Functional) return status;
        return kind switch
        {
            CnIconKind.Add or CnIconKind.Save or CnIconKind.Checkmark or CnIconKind.Play => "success",
            CnIconKind.Edit or CnIconKind.Copy or CnIconKind.ArrowDownload or CnIconKind.Send => "info",
            CnIconKind.Stop => "danger",
            _ => null,
        };
    }
}
