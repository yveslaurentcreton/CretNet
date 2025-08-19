using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDialogComponentSimpleRenderer
{
    [CascadingParameter] public ICnpDialogParameters DialogParameters { get; set; } = default!;
}