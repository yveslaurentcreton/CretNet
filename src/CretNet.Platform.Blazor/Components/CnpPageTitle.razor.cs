using CretNet.Blazor.Utilities;
using CretNet.Platform.Blazor.Models;
using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpPageTitle
{
    [Parameter, EditorRequired] public string Title { get; set; } = default!;
    [Parameter] public bool IsMain { get; set; }
    [Parameter] public bool Hide { get; set; }
    
    [Inject] private IBreadcrumbService BreadcrumbService { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);

        if (firstRender)
        {
            if (IsMain)
                BreadcrumbService.ClearBreadcrumbs();
            
            var text = Title;
            var url = NavigationManager.GetRelativeUri();
            
            if (!string.IsNullOrWhiteSpace(text))
                BreadcrumbService.SetBreadcrumb(new BreadcrumbItem { Text = text, NavigationUrl = url });
        }
    }
}