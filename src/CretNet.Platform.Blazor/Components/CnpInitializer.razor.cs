using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpInitializer
{
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    [Inject] public ITimeService TimeService { get; set; } = default!;
    
    private bool _isInitialized;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await TimeService.InitializeAsync();
        
        _isInitialized = true;
    }
}