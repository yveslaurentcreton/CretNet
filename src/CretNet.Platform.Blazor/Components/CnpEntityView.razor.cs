using Fluxor;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpEntityView<TEntity, TId>
    where TEntity : class, IIdentity<TId>
    where TId : notnull
{
    private Guid _id = Guid.NewGuid();

    [Parameter, EditorRequired] public TEntity Entity { get; set; } = default!;
    [Parameter] public Icon? Icon { get; set; }
    [Parameter, EditorRequired] public RenderFragment<TEntity> ChildContent { get; set; } = default!;
    
    [Inject] public IDispatcher Dispatcher { get; set; } = default!;
    [Inject] public IEntityDefinition<TEntity, TId> EntityDefinition { get; set; } = default!;
    
    protected bool ShouldShowNavigation => EntityDefinition.HasNavigationAction;
    
    protected void Navigate()
    {
        var navigationAction = EntityDefinition.CreateNavigationAction(Entity);
        
        Dispatcher.Dispatch(navigationAction);
    }
}