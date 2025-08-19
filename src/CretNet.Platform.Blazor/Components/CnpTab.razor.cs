using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpTab
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public RenderFragment? ChildContent { get; set; }
}