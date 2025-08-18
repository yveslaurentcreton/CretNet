namespace CretNet.Platform.Blazor.Services;

public interface INavigationService
{
    Task NavigateTo(string url, NavigationTarget target = NavigationTarget.Self);
}