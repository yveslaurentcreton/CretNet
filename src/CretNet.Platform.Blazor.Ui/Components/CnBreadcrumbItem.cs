namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// One step in a <see cref="CnBreadcrumb"/> trail. Href-less items render as
/// plain text (an unnavigable level); the last item is always the current
/// place regardless of its Href.
/// </summary>
public sealed record CnBreadcrumbItem(string Text, string? Href = null);
