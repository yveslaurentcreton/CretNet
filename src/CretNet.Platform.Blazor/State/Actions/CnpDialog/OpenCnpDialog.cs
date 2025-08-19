using Fluxor;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.State.Actions.CnpDialog;

public class OpenCnpDialogEffects
{
    private readonly IDialogService _dialogService;

    public OpenCnpDialogEffects(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }
        
    [EffectMethod]
    public async Task Open(IOpenCnpDialogAction action, IDispatcher _)
    {
        var parameters = new DialogParameters<ICnpDialogParameters>()
        {
            Title = action.Title,
            Width = action.Size.ToPixels(),
            Modal = true,
            PreventScroll = true,
            TrapFocus = false,
            Content = action.BuildParameters()
        };
        var dialog = await _dialogService.ShowDialogAsync<Components.CnpDialog>(parameters);
        var result = await dialog.Result;

        if (!result.Cancelled)
            await action.InvokeOnSuccess();

        if (result.Cancelled)
            await action.InvokeOnCancel();
            
        action.SetResult(result.Data);
    }
}