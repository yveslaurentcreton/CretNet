using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpLoading
{
    [Parameter, EditorRequired] public bool IsLoading { get; set; }
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;
}