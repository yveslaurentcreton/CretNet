using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpStack
{
    [Parameter] public RenderFragment ChildContent { get; set; } = default!;
    [Parameter] public CnpOrientation Orientation { get; set; } = CnpOrientation.Horizontal;
    [Parameter] public CnpHorizontalAlignment HorizontalAlignment { get; set; } = CnpHorizontalAlignment.Left;
    [Parameter] public CnpVerticalAlignment VerticalAlignment { get; set; } = CnpVerticalAlignment.Top;
    [Parameter] public int? Spacing { get; set; }
    [Parameter] public string? Style { get; set; }
    
    public int? HorizontalGap => Spacing is null ? 10 : Spacing * 4;
    public int? VerticalGap => Spacing is null ? 10 : Spacing * 4;
}

public enum CnpOrientation
{
    Horizontal,
    Vertical
}

public enum CnpHorizontalAlignment
{
    Left,
    Center,
    Right,
    Stretch
}

public enum CnpVerticalAlignment
{
    Top,
    Center,
    Bottom,
    Stretch
}

public static class CnpStackExtensions
{
    public static Microsoft.FluentUI.AspNetCore.Components.Orientation ToFluentUI(this CnpOrientation orientation)
    {
        var fluentOrientation = orientation switch
        {
            CnpOrientation.Horizontal => Microsoft.FluentUI.AspNetCore.Components.Orientation.Horizontal,
            CnpOrientation.Vertical => Microsoft.FluentUI.AspNetCore.Components.Orientation.Vertical,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation), orientation, null)
        };

        return fluentOrientation;
    }

    public static Microsoft.FluentUI.AspNetCore.Components.HorizontalAlignment ToFluentUI(
        this CnpHorizontalAlignment alignment)
    {
        var fluentAlignment = alignment switch
        {
            CnpHorizontalAlignment.Left => Microsoft.FluentUI.AspNetCore.Components.HorizontalAlignment.Left,
            CnpHorizontalAlignment.Center => Microsoft.FluentUI.AspNetCore.Components.HorizontalAlignment.Center,
            CnpHorizontalAlignment.Right => Microsoft.FluentUI.AspNetCore.Components.HorizontalAlignment.Right,
            CnpHorizontalAlignment.Stretch => Microsoft.FluentUI.AspNetCore.Components.HorizontalAlignment.Stretch,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };

        return fluentAlignment;
    }
    
    public static Microsoft.FluentUI.AspNetCore.Components.VerticalAlignment ToFluentUI(
        this CnpVerticalAlignment alignment)
    {
        var fluentAlignment = alignment switch
        {
            CnpVerticalAlignment.Top => Microsoft.FluentUI.AspNetCore.Components.VerticalAlignment.Top,
            CnpVerticalAlignment.Center => Microsoft.FluentUI.AspNetCore.Components.VerticalAlignment.Center,
            CnpVerticalAlignment.Bottom => Microsoft.FluentUI.AspNetCore.Components.VerticalAlignment.Bottom,
            CnpVerticalAlignment.Stretch => Microsoft.FluentUI.AspNetCore.Components.VerticalAlignment.Top,
            _ => throw new ArgumentOutOfRangeException(nameof(alignment), alignment, null)
        };

        return fluentAlignment;
    }
}