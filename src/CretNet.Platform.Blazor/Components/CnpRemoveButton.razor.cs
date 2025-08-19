using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpRemoveButton
{
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool IsPrimary { get; set; } = true;
    
    protected ButtonRole ButtonRole => IsPrimary ? ButtonRole.Primary : ButtonRole.Default;
}