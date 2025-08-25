using CretNet.Platform.Data;

namespace CretNet.Platform.Blazor.Interfaces;

public interface IHandleSelection
{
    bool GetMultiSelection();
    void SelectedItemsChanged(IEnumerable<object> entities);
    Func<IQueryable<TEntity>, IQueryable<TEntity>>? GetSelectionFilter<TEntity>() where TEntity : IIdentity;
    public Func<TEntity, bool>? GetCustomFilterFunc<TEntity>() where TEntity : IIdentity;
    Func<object>? GetDependencyArgsFunc<TEntity>() where TEntity : IIdentity;
}