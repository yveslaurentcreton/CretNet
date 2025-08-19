using Fluxor;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Data;
using CretNet.Platform.Fluxor;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpEntityInfo<TEntity, TId>
    where TId : struct
    where TEntity : class, IIdentity<TId>
{
    [Parameter] public TId? EntityId { get; set; }
    [Parameter] public TEntity? Entity { get; set; }
    [Parameter, EditorRequired] public RenderFragment<TEntity> ChildContent { get; set; } = default!;

    [Inject] private IDispatcher Dispatcher { get; set; } = default!;
    [Inject] private IEntityDefinition<TEntity, TId> EntityDefinition { get; set; } = default!;

    public override Task SetParametersAsync(ParameterView parameters)
    {
        var ret = base.SetParametersAsync(parameters);

        if (parameters.TryGetValue<TId?>(nameof(EntityId), out var entityId))
        {
            if (entityId is null)
                return ret;
            
            InvokeAsync(async () =>
            {
                var fetchAction = EntityDefinition.CreateFetchAction(entityId.Value);
                var fetchedEntity = await Dispatcher.DispatchAsync(fetchAction);
                Entity = fetchedEntity;
                
                StateHasChanged();
            });
        }

        if (parameters.TryGetValue<TEntity?>(nameof(Entity), out var newEntity))
        {
            Entity = newEntity;
            
            InvokeAsync(StateHasChanged);
        }

        return ret;
    }
}