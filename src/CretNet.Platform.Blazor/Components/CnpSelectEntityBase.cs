using CommunityToolkit.Diagnostics;
using Fluxor;
using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public class CnpSelectEntityBase<TEntity, TId> : CnpComponent
    where TId : struct
    where TEntity : class, IIdentity<TId>
{
    [Parameter] public TId? EntityId { get; set; }
    [Parameter] public EventCallback<TId?> EntityIdChanged { get; set; }
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool ShowAdd { get; set; }
    [Parameter] public Func<ICnpEntityAction<TEntity?>>? AddAction { get; set; }
    [Parameter] public bool ShowAdvancedSearch { get; set; }
    [Parameter] public Func<ICnpEntityAction<TEntity?>>? AdvancedSearchAction { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    
    [Inject] public IDispatcher Dispatcher { get; set; } = default!;
    
    protected bool ShouldShowAdd => AddAction is not null && ShowAdd && !ReadOnly;
    protected bool ShouldShowAdvancedSearch => AdvancedSearchAction is not null && ShowAdvancedSearch && !ReadOnly;
    
    protected async Task InternalAdd()
    {
        Guard.IsNotNull(AddAction);
        
        var createdEntity = await Dispatcher.DispatchAsync(AddAction());
        
        if (createdEntity != null)
            await EntityIdChanged.InvokeAsync(createdEntity.Id);
    }
    
    protected async Task InternalAdvancedSearch()
    {
        Guard.IsNotNull(AdvancedSearchAction);
        
        var selectedEntity = await Dispatcher.DispatchAsync(AdvancedSearchAction());
        
        if (selectedEntity != null)
            await EntityIdChanged.InvokeAsync(selectedEntity.Id);
    }
}