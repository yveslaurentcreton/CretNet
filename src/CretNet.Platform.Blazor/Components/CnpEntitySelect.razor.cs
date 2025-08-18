using System.Linq.Expressions;
using Fluxor;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;
using CretNet.Platform.Data;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpEntitySelect<TEntity, TId>
    where TId : struct
    where TEntity : class, IIdentity<TId>
{
    [Parameter] public TId? EntityId { get; set; }
    [Parameter] public EventCallback<TId?> EntityIdChanged { get; set; }
    [Parameter] public TEntity? Entity { get; set; }
    [Parameter] public EventCallback<TEntity?> EntityChanged { get; set; }
    [Parameter] public Expression<Func<TId?>>? For { get; set; }
    [Parameter] public string? Label { get; set; }
    [Parameter] public bool UseCustomLabel { get; set; }
    [Parameter] public bool ShowAdd { get; set; }
    [Parameter] public bool ShowAdvancedSearch { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public Func<TEntity, bool>? CustomFilterFunc { get; set; }
    [Parameter] public Func<object>? DependencyArgs { get; set; }
    
    [Inject] public IDispatcher Dispatcher { get; set; } = default!;
    [Inject] public ICnpDataSource<TEntity, TId> DataSource { get; set; } = default!;
    [Inject] public IEntityDefinition<TEntity, TId> EntityDefinition { get; set; } = default!;
    
    private IEnumerable<EntitySelector<TId>> _entities = [];
    
    private EntitySelector<TId>? SelectedEntity => _entities.FirstOrDefault(x => x.Id.Equals(EntityId));
    protected bool ShouldShowAdd => DataSource.CanAdd && ShowAdd && !ReadOnly;
    protected bool ShouldShowAdvancedSearch => EntityDefinition.HasDataGrid && ShowAdvancedSearch && !ReadOnly;
    
    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        DataSource.DependencyArgs = DependencyArgs;
        DataSource.CustomFilterFunc = CustomFilterFunc;
        DataSource.OnStateHasChanged += StateHasChanged;

        await DataSource.Init();
        
        await FetchEntities();
        await SetSelectedEntity(SelectedEntity);
    }

    private Task FetchEntities()
    {
        var entities = DataSource.Entities ?? Enumerable.Empty<TEntity>();
        
        _entities = entities.Select(x => new EntitySelector<TId>
        {
            Id = x.Id,
            Name = EntityDefinition.GetDisplayName(x) ?? string.Empty
        });
        
        return Task.CompletedTask;
    }

    private async Task SetSelectedEntity(EntitySelector<TId>? entity)
    {
        if (EntityIdChanged.HasDelegate)
            await EntityIdChanged.InvokeAsync(entity?.Id);
        
        if (EntityChanged.HasDelegate)
        {
            Entity = DataSource.Entities?.FirstOrDefault(x => x.Id.Equals(entity?.Id));
            await EntityChanged.InvokeAsync(Entity);
        }
    }
        
    protected async Task InternalAdd()
    {
        var createdEntity = await DataSource.Add();
        
        if (createdEntity != null)
            await EntityIdChanged.InvokeAsync(createdEntity.Id);
    }
    
    protected virtual async Task<TEntity?> InternalAdvancedSearch()
    {
        var label = EntityDefinition.Label;
        var dataGridType = EntityDefinition.DataGridType;
        
        var selectedEntities = await Dispatcher.AsCnp().CreateSearchDialogFor<TEntity>()
            .WithTitle($"Search {label.ToLower()}")
            .WithCustomFilterFunc(CustomFilterFunc)
            .WithDialogContent(dataGridType)
            .AndOpen();
        
        var selectedEntity = selectedEntities?.FirstOrDefault();

        if (selectedEntity is not null)
        {
            await EntityIdChanged.InvokeAsync(selectedEntity.Id);
            await EntityChanged.InvokeAsync(selectedEntity);
        }
        
        return selectedEntity;
    }
}