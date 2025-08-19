using CretNet.Platform.Blazor.Models;

namespace CretNet.Platform.Blazor.Services;

public interface IBreadcrumbService
{
    event EventHandler? OnChanged;
    IReadOnlyList<BreadcrumbItem> GetBreadcrumbs();
    void SetBreadcrumb(BreadcrumbItem item);
    void ClearBreadcrumbs();
}