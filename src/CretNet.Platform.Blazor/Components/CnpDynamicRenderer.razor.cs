using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDynamicRenderer
{
    [Parameter] public ICnpDynamicRendererParameters? Parameters { get; set; }
}

public class CnpDynamicRendererParameters<TViewModel> : ICnpDynamicRendererParameters
{
    public Type RenderType { get; } = typeof(CnpDynamicRendererHelper<TViewModel>);
    public required TViewModel ViewModel { get; init; }
    public required Type ContentType { get; init; }
}

public interface ICnpDynamicRendererParameters
{
    Type RenderType { get; }
    Type ContentType { get; }
}