using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSectionContent
{
    [Parameter, EditorRequired] public object CategoryId { get; set; } = default!;
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = default!;
    
    [Inject] public ICnpSectionService SectionService { get; set; } = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        SectionService.AddSection(CategoryId, this);
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();
        
        SectionService.RemoveSection(this);
    }
}