using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using CretNet.Platform.Data;

namespace CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;

public record OpenCnpSearchDialogAction<TEntity> : IOpenCnpSearchDialogAction, IOpenCnpSearchDialogAction<TEntity>
    where TEntity : class, IIdentity
{
    public required Type DialogContentType { get; init; }
    public Action<IEnumerable<TEntity>>? OnSelect { get; init; }
    public Func<IEnumerable<TEntity>, Task>? OnSelectAsync { get; init; }
    public Action? OnCancel { get; init; }
    public Func<Task>? OnCancelAsync { get; init; }
    public string? Title { get; init; }
    public DialogSize Size { get; init; }
    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? Filter { get; init; }
    public Func<TEntity, bool>? CustomFilterFunc { get; init; }
    public bool MultiSelection { get; init; }
    public TaskCompletionSource<IEnumerable<TEntity>?> DialogTaskCompletionSource { get; } = new();
    
    public ICnpSearchDialogParameters BuildParameters()
    {
        return new CnpSearchDialogParameters<TEntity>()
        {
            DialogContentType = DialogContentType,
            Filter = Filter,
            CustomFilterFunc = CustomFilterFunc,
            MultiSelection = MultiSelection
        };
    }
    
    public async Task InvokeOnSelect(object result)
    {
        if (result is not IEnumerable<TEntity> typedEntitiesEnumerable)
            throw new InvalidOperationException($"Entities must be of type {typeof(TEntity).Name}");
            
        var typedEntities = typedEntitiesEnumerable.ToList();
        
        if (OnSelect != null)
            OnSelect(typedEntities);
        
        if (OnSelectAsync != null)
            await OnSelectAsync(typedEntities);
    }
    
    public async Task InvokeOnCancel()
    {
        if (OnCancel != null)
            OnCancel();
        
        if (OnCancelAsync != null)
            await OnCancelAsync();
    }
    
    public void SetResult(object? result)
    {
        if (result is not IEnumerable<TEntity> typedEntities)
        {
            DialogTaskCompletionSource.SetResult(null);
            return;
        }
        
        DialogTaskCompletionSource.SetResult(typedEntities);
    }
}

public interface IOpenCnpSearchDialogAction
{
    string? Title { get; }
    DialogSize Size { get; }
    Task InvokeOnSelect(object result);
    Task InvokeOnCancel();
    void SetResult(object? result);
    ICnpSearchDialogParameters BuildParameters();
}

public interface IOpenCnpSearchDialogAction<TEntity>
    where TEntity : IIdentity
{
    Type DialogContentType { get; }
    Action<IEnumerable<TEntity>>? OnSelect { get; }
    Func<IEnumerable<TEntity>, Task>? OnSelectAsync { get; init; }
    Action? OnCancel { get; }
    Func<Task>? OnCancelAsync { get; init; }
    string? Title { get; }
    bool MultiSelection { get; }
    TaskCompletionSource<IEnumerable<TEntity>?> DialogTaskCompletionSource { get; }
}