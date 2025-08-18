using System.Globalization;
using Blazored.LocalStorage;
using Fluxor;
using CretNet.Platform.Blazor.State.Actions;

namespace CretNet.Platform.Blazor.Server.State.Actions;

// Effects
public class ChangeLanguageEffects
{
    private readonly ILocalStorageService _localStorageService;

    public ChangeLanguageEffects(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    
    [EffectMethod]
    public async Task Change(ChangeCultureAction action, IDispatcher dispatcher)
    {
        const string key = "cnp-language";
        var newCulture = action.Culture;
        
        CultureInfo.DefaultThreadCurrentCulture = newCulture;
        CultureInfo.DefaultThreadCurrentUICulture = newCulture;
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;
        
        await _localStorageService.SetItemAsStringAsync(key, action.Culture.ToString());
        
        dispatcher.Dispatch(new ChangeCultureSuccessAction());
    }
}