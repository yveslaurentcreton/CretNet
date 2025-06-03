using Microsoft.AspNetCore.Components;

namespace DotNetJet.Blazor.Utilities;

/// <summary>
/// Provides extension methods for NavigationManager to simplify common operations, such as retrieving the relative URI and its parts.
/// </summary>
public static class JetNavigationManagerUtilities
{
    /// <summary>
    /// Retrieves the relative URI from the NavigationManager's absolute URI.
    /// </summary>
    /// <param name="navigationManager">The NavigationManager instance.</param>
    /// <returns>The relative URI as a string.</returns>
    public static string GetRelativeUri(this NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);
        
        return navigationManager.ToBaseRelativePath(navigationManager.Uri);
    }

    /// <summary>
    /// Splits the relative URI into its constituent parts.
    /// </summary>
    /// <param name="navigationManager">The NavigationManager instance.</param>
    /// <returns>An IEnumerable of strings, each representing a part of the relative URI.</returns>
    public static IEnumerable<string> GetRelativeUriParts(this NavigationManager navigationManager)
    {
        ArgumentNullException.ThrowIfNull(navigationManager);

        var relativeUri = navigationManager.GetRelativeUri();
        
        return string.IsNullOrEmpty(relativeUri) ? Enumerable.Empty<string>() : relativeUri.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
    }
}
