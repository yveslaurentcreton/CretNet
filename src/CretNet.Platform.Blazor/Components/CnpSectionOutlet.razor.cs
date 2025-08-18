using CretNet.Platform.Blazor.Services;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSectionOutlet
{
    [Parameter, EditorRequired] public object CategoryId { get; set; } = default!;
    
    [Inject] public ICnpSectionService SectionService { get; set; } = default!;

    public IEnumerable<object> Sections => SectionService.GetSections(CategoryId);

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SectionService.OnChange += StateHasChanged;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        SectionService.OnChange -= StateHasChanged;
    }
}