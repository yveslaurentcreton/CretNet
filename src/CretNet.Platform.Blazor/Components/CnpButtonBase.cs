using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CretNet.Platform.Blazor.Components;

public abstract class CnpButtonBase : CnpComponent
{
    [Parameter] public string? Label { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public RenderFragment? LoadingChildContent { get; set; }
    [Parameter] public CnpButtonType ButtonType { get; set; }
    [Parameter] public ButtonRole ButtonRole { get; set; } = ButtonRole.Default;
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Style { get; set; }
    
    protected bool UseLabel => !string.IsNullOrEmpty(Label);
    protected bool IsIconOnly => !UseLabel && ChildContent is null;
    protected bool DisabledInternal => Disabled || Loading;

    protected async Task OnClickInternal(MouseEventArgs args)
    {
        if (Loading || ButtonType == CnpButtonType.Submit)
            return;
        
        if (OnClick.HasDelegate)
        {
            Loading = true;
            await OnClick.InvokeAsync(args);
            Loading = false;
        }
    }
}

public enum CnpButtonType
{
    Button,
    Submit,
    Reset
}

public enum ButtonRole
{
    Default,
    Primary,
    Secondary,
    Tertiary,
    Error
}
