using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDialog : IDialogContentComponent
{
    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
    public DialogParameters<ICnpDialogParameters> FluentDialogParameters => (DialogParameters<ICnpDialogParameters>)Dialog.Instance.Parameters;
    public ICnpDialogParameters DialogParameters => FluentDialogParameters.Content;

    public Icon? Icon => DialogParameters.Icon as Icon;
    
    private bool _isSaving;

    private async Task SaveAsync()
    {
        bool isSuccess;
        object? result = null;
        
        _isSaving = true;
        
        try
        {
            result = await DialogParameters.InvokeOnOk();
            isSuccess = true;
        }
        catch (Exception)
        {
            isSuccess = false;
        }
        
        _isSaving = false;
        StateHasChanged();

        if (isSuccess)
            await Dialog.CloseAsync(result);
    }

    private async Task CancelAsync()
    {
        await Dialog.CancelAsync();
    }
}