using CretNet.Platform.Blazor.Models;
using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpBreadcrumb
{
    [Inject] private IBreadcrumbService BreadcrumbService { get; set; } = default!;
    
    public IReadOnlyList<BreadcrumbItem> BreadcrumbItems => BreadcrumbService.GetBreadcrumbs();

    private void OnBreadcrumbChanged(object? sender, EventArgs args)
    {
        InvokeAsync(StateHasChanged);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        BreadcrumbService.OnChanged += OnBreadcrumbChanged;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        
        BreadcrumbService.OnChanged -= OnBreadcrumbChanged;
    }
}