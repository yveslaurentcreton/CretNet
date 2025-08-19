using CretNet.Platform.Blazor.Models;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpFilterButton<TEntity>
{
    private bool _filtersSet;
    private bool _isFilterPopupVisible;

    [Parameter, EditorRequired] public required IEnumerable<EntityFilter<TEntity>> Filters { get; set; } = default!;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        _filtersSet = Filters.Any(f => f.Enabled != f.DefaultEnabled);
    }
}