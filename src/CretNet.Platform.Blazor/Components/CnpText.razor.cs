using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpText
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public CnpTypography Typography { get; set; }
    
    protected Microsoft.FluentUI.AspNetCore.Components.Typography FluentTypo => Typography.ToFluent();
}

public enum CnpTypography
{
    H1,
    H2,
    H3,
    H4,
    H5,
    H6,
    PageTitle,
    CardTitle,
    DialogTitle,
    DialogSubtitle
}

public static class CnpTextExtensions
{
    public static  Microsoft.FluentUI.AspNetCore.Components.Typography ToFluent(this CnpTypography typography)
    {
        var fluentOrientation = typography switch
        {
            CnpTypography.H1 => Microsoft.FluentUI.AspNetCore.Components.Typography.H1,
            CnpTypography.H2 => Microsoft.FluentUI.AspNetCore.Components.Typography.H2,
            CnpTypography.H3 => Microsoft.FluentUI.AspNetCore.Components.Typography.H3,
            CnpTypography.H4 => Microsoft.FluentUI.AspNetCore.Components.Typography.H4,
            CnpTypography.H5 => Microsoft.FluentUI.AspNetCore.Components.Typography.H5,
            CnpTypography.H6 => Microsoft.FluentUI.AspNetCore.Components.Typography.H6,
            CnpTypography.PageTitle => Microsoft.FluentUI.AspNetCore.Components.Typography.PageTitle,
            CnpTypography.CardTitle => Microsoft.FluentUI.AspNetCore.Components.Typography.H4,
            CnpTypography.DialogTitle => Microsoft.FluentUI.AspNetCore.Components.Typography.PaneHeader,
            CnpTypography.DialogSubtitle => Microsoft.FluentUI.AspNetCore.Components.Typography.H5,
            _ => throw new ArgumentOutOfRangeException(nameof(typography), typography, null)
        };

        return fluentOrientation;
    }
}