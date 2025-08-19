using CommunityToolkit.Diagnostics;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Blazor.State.Actions.CnpDialog;
using CretNet.Platform.Data;

namespace CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;

public static class OpenCnpSearchDialogActionBuilderExtensions
{
    public static OpenCnpSearchDialogActionBuilder<TEntity> CreateSearchDialogFor<TEntity>(this ICnpDispatcher dispatcher)
        where TEntity : class, IIdentity
    {
        return new OpenCnpSearchDialogActionBuilder<TEntity>(dispatcher);
    }
}

public class OpenCnpSearchDialogActionBuilder<TEntity>
    where TEntity : class, IIdentity
{
    private readonly ICnpDispatcher? _dispatcher;
    private string? _title;
    private DialogSize _size = DialogSize.Large;
    private Func<IQueryable<TEntity>, IQueryable<TEntity>>? _filter;
    private Func<TEntity, bool>? _customFilterFunc;
    private bool _multiSelection;
    private Type? _dialogContentType;
    private Action<IEnumerable<TEntity>>? _onSelect;
    private Func<IEnumerable<TEntity>, Task>? _onSelectAsync;
    private Action? _onCancel;
    private Func<Task>? _onCancelAsync;
    
    internal OpenCnpSearchDialogActionBuilder(ICnpDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithSize(DialogSize size)
    {
        _size = size;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithFilter(Func<IQueryable<TEntity>, IQueryable<TEntity>>? filter)
    {
        _filter = filter;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithCustomFilterFunc(Func<TEntity, bool>? customFilterFunc)
    {
        _customFilterFunc = customFilterFunc;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithMultiSelection()
    {
        _multiSelection = true;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithDialogContent(Type dialogContentType)
    {
        _dialogContentType = dialogContentType;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithDialogContent<TCnpDataGrid>()
    {
        _dialogContentType = typeof(TCnpDataGrid);
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithOnSelect(Action<IEnumerable<TEntity>> onSelect)
    {
        _onSelect = onSelect;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithOnSelectAsync(Func<IEnumerable<TEntity>, Task> onSelectAsync)
    {
        _onSelectAsync = onSelectAsync;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithOnCancel(Action onCancel)
    {
        _onCancel = onCancel;
        return this;
    }
    
    public OpenCnpSearchDialogActionBuilder<TEntity> WithOnCancelAsync(Func<Task> onCancelAsync)
    {
        _onCancelAsync = onCancelAsync;
        return this;
    }
    
    private IOpenCnpSearchDialogAction<TEntity> Build()
    {
        Guard.IsNotNull(_dialogContentType);
        
        return new OpenCnpSearchDialogAction<TEntity>()
        {
            Title = _title,
            Size = _size,
            Filter = _filter,
            CustomFilterFunc = _customFilterFunc,
            MultiSelection = _multiSelection,
            DialogContentType = _dialogContentType,
            OnSelect = _onSelect,
            OnSelectAsync = _onSelectAsync,
            OnCancel = _onCancel,
            OnCancelAsync = _onCancelAsync,
        };
    }

    public async Task<IEnumerable<TEntity>?> AndOpen()
    {
        Guard.IsNotNull(_dispatcher);
        
        var action = Build();
        _dispatcher.Dispatch(action);
        var result = await action.DialogTaskCompletionSource.Task;
        return result;
    }
}