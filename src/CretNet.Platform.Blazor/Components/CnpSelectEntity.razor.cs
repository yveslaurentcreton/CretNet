using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSelectEntity<TEntity, TId>
    where TId : struct
    where TEntity : class, IIdentity<TId>
{
    private IEnumerable<EntitySelector<TId>> _entities = [];
    
    [Parameter, EditorRequired] public Func<ICnpEntityAction<IEnumerable<TEntity>>> FetchAction { get; set; } = default!;
    [Parameter] public Func<IEnumerable<TEntity>, IEnumerable<TEntity>>? Filter { get; set; }
    [Parameter, EditorRequired] public Func<TEntity?, string> DisplayProperty { get; set; } = default!;
    
    protected bool IsLoading { get; set; }
    private EntitySelector<TId>? SelectedEntity => _entities.FirstOrDefault(x => x.Id.Equals(EntityId));


    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        await FetchEntities();
    }
    
    protected async Task FetchEntities()
    {
        IsLoading = true;
        
        var entities = await Dispatcher.DispatchAsync(FetchAction());
        
        if (Filter is not null)
            entities = Filter(entities);
        
        _entities = entities.Select(x => new EntitySelector<TId>
        {
            Id = x.Id,
            Name = DisplayProperty(x)
        });
        
        IsLoading = false;
    }

    private async Task SetSelectedEntity(EntitySelector<TId>? entity)
    {
        await EntityIdChanged.InvokeAsync(entity?.Id);
    }
}

public record EntitySelector<TId>
    where TId : struct
{
    public required TId Id { get; init; }
    public required string Name { get; init; }
}