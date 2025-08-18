using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpButton
{
    [Parameter] public Icon? FluentIconStart { get; set; }
    [Parameter] public Icon? FluentIconEnd { get; set; }
    [Parameter] public string? FluentBackgroundColor { get; set; }
    [Parameter] public string? FluentColor { get; set; }
    [Parameter] public bool Responsive { get; set; }
    
    protected ButtonType FluentButtonType => ButtonType.ToFluentUIButtonType();
    protected Appearance FluentAppearance => ButtonRole.ToFluentUIAppearance();
    protected string? FluentColorInternal => !string.IsNullOrEmpty(FluentColor) ? FluentColor : ButtonRole.ToFluentUIColor();
    protected string? InternalClass => Responsive ? "cnp-responsive-button" : null;
}

public static class ButtonTypeExtensions
{
    public static ButtonType ToFluentUIButtonType(this CnpButtonType val)
    {
        var type = val switch
        {
            CnpButtonType.Button => ButtonType.Button,
            CnpButtonType.Submit => ButtonType.Submit,
            CnpButtonType.Reset => ButtonType.Reset,
            _ => throw new ArgumentOutOfRangeException(nameof(val), val, null)
        };
        
        return type;
    }
}

public static class ButtonRoleExtensions
{
    public static Appearance ToFluentUIAppearance(this ButtonRole role)
    {
        var color = role switch
        {
            ButtonRole.Default => Appearance.Neutral,
            ButtonRole.Primary => Appearance.Accent,
            ButtonRole.Secondary => Appearance.Neutral,
            ButtonRole.Tertiary => Appearance.Neutral,
            ButtonRole.Error => Appearance.Neutral,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
        
        return color;
    }
        
    public static string? ToFluentUIColor(this ButtonRole role)
    {
        var color = role switch
        {
            ButtonRole.Default => null,
            ButtonRole.Primary => null,
            ButtonRole.Secondary => null,
            ButtonRole.Tertiary => null,
            ButtonRole.Error => "var(--error)",
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
        
        return color;
    }
}

