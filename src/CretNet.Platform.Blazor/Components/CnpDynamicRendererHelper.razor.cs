using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDynamicRendererHelper<TViewModel>
{
    [CascadingParameter] public ICnpDynamicRendererParameters RendererParameters { get; set; } = default!;
    public CnpDynamicRendererParameters<TViewModel>? Parameters => RendererParameters as CnpDynamicRendererParameters<TViewModel>;
}