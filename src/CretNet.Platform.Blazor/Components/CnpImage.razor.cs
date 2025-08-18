using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpImage
{
    [Parameter, EditorRequired] public string Src { get; set; } = default!;
    [Parameter] public string? Alt { get; set; }
    [Parameter] public bool Rounded { get; set; }
    [Parameter] public string? Style { get; set; }
}