using Fluxor;
using CretNet.Platform.Blazor.Components;
using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.State;

public class OpenCnpSearchDialogEffects
{
    private readonly IDialogService _dialogService;

    public OpenCnpSearchDialogEffects(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
    
    [EffectMethod]
    public async Task Open(IOpenCnpSearchDialogAction action, IDispatcher _)
    {
        var parameters = new DialogParameters<ICnpSearchDialogParameters>()
        {
            Title = action.Title,
            Width = action.Size.ToPixels(),
            Modal = true,
            PreventScroll = true,
            TrapFocus = false,
            Content = action.BuildParameters()
        };
        var dialog = await _dialogService.ShowDialogAsync<CnpSearchDialog>(parameters);
        var result = await dialog.Result;

        if (!result.Cancelled)
            await action.InvokeOnSelect(result.Data);

        if (result.Cancelled)
            await action.InvokeOnCancel();
        
        action.SetResult(result.Data);
    }
}
