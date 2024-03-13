// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Components;

public static class NavigationManagerExtensions
{
    public static string GetRelativeUri(this NavigationManager navigationManager)
    {
        return navigationManager.ToBaseRelativePath(navigationManager.Uri);
    }

    public static IEnumerable<string> GetRelativeUriParts(this NavigationManager navigationManager)
    {
        return navigationManager.GetRelativeUri().Split('/');
    }
}
