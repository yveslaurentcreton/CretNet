using CretNet.Platform.Data;

namespace CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;

public class CnpSearchDialogParameters<TEntity> : ICnpSearchDialogParameters, IHandleSelectionParameters<TEntity>
    where TEntity : IIdentity
{
    public required Type DialogContentType { get; init; }
    public bool MultiSelection { get; set; }
    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? Filter { get; set; }
    public Func<TEntity, bool>? CustomFilterFunc { get; set; }
    public Func<object>? DependencyArgsFunc { get; set; }
}

public interface ICnpSearchDialogParameters
{
    Type DialogContentType { get; }
    bool MultiSelection { get; }
}

public interface IHandleSelectionParameters<TEntity>
    where TEntity : IIdentity
{
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? Filter { get; }
    Func<TEntity, bool>? CustomFilterFunc { get; }
    Func<object>? DependencyArgsFunc { get; }
}
