using CretNet.Platform.Blazor.Ui.Components;

namespace CretNet.Platform.Blazor.Ui.Navigation;

/// <summary>
/// The trail a shell breadcrumb renders. Pages feed it — a
/// <see cref="CnPageTitle"/> does so as it renders — and the shell listens.
/// Setting an item whose <c>Href</c> is already on the trail trims
/// everything after it and takes its place, so landing on an ancestor
/// reads as going back, not as going deeper.
/// </summary>
public sealed class CnBreadcrumbService
{
    private readonly List<CnBreadcrumbItem> _items = [];

    /// <summary>Root first; the last item is the current place.</summary>
    public IReadOnlyList<CnBreadcrumbItem> Items => _items;

    public event Action? Changed;

    public void Set(CnBreadcrumbItem item)
    {
        var index = item.Href is null
            ? -1
            : _items.FindIndex(i => string.Equals(i.Href, item.Href, StringComparison.OrdinalIgnoreCase));
        if (index >= 0)
            _items.RemoveRange(index, _items.Count - index);

        _items.Add(item);
        Changed?.Invoke();
    }

    public void Clear()
    {
        if (_items.Count == 0)
            return;

        _items.Clear();
        Changed?.Invoke();
    }
}
