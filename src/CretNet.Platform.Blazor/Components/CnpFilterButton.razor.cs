using CretNet.Platform.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpFilterButton<TEntity>
{
    private bool _filtersSet;
    private bool _isFilterPopupVisible;

    // Anchor id must be unique per instance — multiple filter buttons can be
    // on one page and FluentPopover anchors to the first element with the id.
    private readonly string _filterButtonId = Identifier.NewId();

    [Parameter, EditorRequired] public required IEnumerable<EntityFilter<TEntity>> Filters { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _filtersSet = Filters.Any(f => f.Enabled != f.DefaultEnabled);
    }
}