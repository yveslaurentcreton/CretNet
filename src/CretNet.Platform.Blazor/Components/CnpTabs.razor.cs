using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpTabs
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
}