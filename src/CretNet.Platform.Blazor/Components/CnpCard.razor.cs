using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public abstract class CnpCardBase : CnpComponent
{
    [Parameter] public RenderFragment CardHeaderContent { get; set; } = default!;
    [Parameter] public RenderFragment CardHeaderActions { get; set; } = default!;
    [Parameter] public RenderFragment CardContent { get; set; } = default!;
}