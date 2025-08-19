using System.Globalization;
using Blazored.LocalStorage;
using Fluxor;
using CretNet.Platform.Blazor.State.Actions;

namespace CretNet.Platform.Blazor.Server.State.Actions;

// Effects
public class InitializeCultureEffects
{
    private readonly ILocalStorageService _localStorageService;

    public InitializeCultureEffects(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }
    
    [EffectMethod]
    public async Task Initialize(InitializeCultureAction action, IDispatcher dispatcher)
    {
        const string key = "cnp-language";
        string? storedCulture = null;
        
        if (await _localStorageService.ContainKeyAsync(key))
            storedCulture = await _localStorageService.GetItemAsStringAsync(key);

        var culture = !string.IsNullOrEmpty(storedCulture) ? storedCulture : "en-US";
        dispatcher.Dispatch(new ChangeCultureAction(new CultureInfo(culture)));
    }
}