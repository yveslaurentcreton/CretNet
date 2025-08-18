using CretNet.Platform.Blazor.Models;

namespace CretNet.Platform.Blazor.Services;

public class BreadcrumbService : IBreadcrumbService
{
    public event EventHandler? OnChanged;

    private List<BreadcrumbItem> BreadcrumbItems { get; set; } = new List<BreadcrumbItem>();

    public IReadOnlyList<BreadcrumbItem> GetBreadcrumbs() => BreadcrumbItems;

    public void SetBreadcrumb(BreadcrumbItem item)
    {
        var current = BreadcrumbItems.FirstOrDefault(x => x.NavigationUrl == item.NavigationUrl);

        if (current is null)
        {
            BreadcrumbItems.Add(item);
            OnChanged?.Invoke(this, new());
            return;
        }
        
        var currentIndex = BreadcrumbItems.IndexOf(current);
        BreadcrumbItems.RemoveAll(x => BreadcrumbItems.IndexOf(x) > currentIndex);
        OnChanged?.Invoke(this, new());
    }

    public void ClearBreadcrumbs()
    {
        BreadcrumbItems.Clear();
        OnChanged?.Invoke(this, new());
    }
}
