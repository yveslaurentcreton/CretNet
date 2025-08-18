using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Services;

public class NavigationService : INavigationService
{
    private readonly IJSRuntime _jsRuntime;

    public NavigationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task NavigateTo(string url, NavigationTarget target = NavigationTarget.Self)
    {
        var targetString = target switch
        {
            NavigationTarget.Blank => "_blank",
            NavigationTarget.Parent => "_parent",
            NavigationTarget.Self => "_self",
            NavigationTarget.Top => "_top",
            _ => throw new ArgumentOutOfRangeException(nameof(target), target, null)
        }; 
        await _jsRuntime.InvokeVoidAsync("open", url, targetString);
    }
}

public enum NavigationTarget
{
    Blank,
    Self,
    Parent,
    Top
}