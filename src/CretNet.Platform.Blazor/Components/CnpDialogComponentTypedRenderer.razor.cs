using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDialogComponentTypedRenderer<TViewModel>
{
    [CascadingParameter] public ICnpDialogParameters DialogParameters { get; set; } = default!;

    public ICnpDialogParameters<TViewModel> Parameters => DialogParameters as ICnpDialogParameters<TViewModel>;
}