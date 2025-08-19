using CommunityToolkit.Diagnostics;
using CretNet.Platform.Blazor.Resources;
using CretNet.Platform.Blazor.Services;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.State.Actions.CnpDialog;

public static class OpenCnpDialogActionBuilderExtensions
{
    public static OpenCnpDialogActionBuilder CreateDialog(this ICnpDispatcher dispatcher)
    {
        return new OpenCnpDialogActionBuilder(dispatcher);
    }
    
    public static OpenCnpDialogActionBuilder<TViewModel, TViewModel> CreateDialogFor<TViewModel>(this ICnpDispatcher dispatcher)
        where TViewModel : class
    {
        return new OpenCnpDialogActionBuilder<TViewModel, TViewModel>(dispatcher, false, CnpLabels.Ok);
    }
    
    public static OpenCnpDialogActionBuilder<TViewModel, TResult> CreateDialogFor<TViewModel, TResult>(this ICnpDispatcher dispatcher)
        where TViewModel : class
        where TResult : class
    {
        return new OpenCnpDialogActionBuilder<TViewModel, TResult>(dispatcher, true, CnpLabels.Save);
    }
    
    public static OpenCnpDialogActionBuilder<TResult, TViewModel> WithFluentIcon<TResult, TViewModel>(this OpenCnpDialogActionBuilder<TResult, TViewModel> builder, Icon icon)
        where TResult : class
        where TViewModel : class
    {
        return builder.WithIcon(icon);
    }
}

public class OpenCnpDialogActionBuilder
{
    private readonly ICnpDispatcher? _dispatcher;
    private string? _title;
    private DialogSize _size = DialogSize.Medium;
    private string? _okLabel;
    private object? _icon;
    private Type? _dialogContentType;
    private Action<TaskCompletionSource>? _onOk;
    private Func<TaskCompletionSource, Task>? _onOkAsync;
    private Action? _onCancel;
    private Func<Task>? _onCancelAsync;
    private Action? _onSuccess;
    private Func<Task>? _onSuccessAsync;
    
    internal OpenCnpDialogActionBuilder(ICnpDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public OpenCnpDialogActionBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithSize(DialogSize size)
    {
        _size = size;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOkLabel(string okLabel)
    {
        _okLabel = okLabel;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithIcon(object icon)
    {
        _icon = icon;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithDialogContent<TCnpDataGrid>()
    {
        _dialogContentType = typeof(TCnpDataGrid);
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnOk(Action<TaskCompletionSource> onOk)
    {
        _onOk = onOk;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnOkAsync(Func<TaskCompletionSource, Task> onOkAsync)
    {
        _onOkAsync = onOkAsync;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnCancel(Action onCancel)
    {
        _onCancel = onCancel;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnCancelAsync(Func<Task> onCancelAsync)
    {
        _onCancelAsync = onCancelAsync;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnSuccess(Action onSuccess)
    {
        _onSuccess = onSuccess;
        return this;
    }
    
    public OpenCnpDialogActionBuilder WithOnSuccessAsync(Func<Task> onSuccessAsync)
    {
        _onSuccessAsync = onSuccessAsync;
        return this;
    }
    
    private IOpenCnpDialogSimpleAction Build()
    {
        Guard.IsNotNull(_dialogContentType);
        
        return new OpenCnpDialogAction()
        {
            Title = _title,
            Size = _size,
            OkLabel = _okLabel,
            Icon = _icon,
            DialogContentType = _dialogContentType,
            OnOk = _onOk,
            OnOkAsync = _onOkAsync,
            OnCancel = _onCancel,
            OnCancelAsync = _onCancelAsync,
            OnSuccess = _onSuccess,
            OnSuccessAsync = _onSuccessAsync,
        };
    }

    public async Task AndOpen()
    {
        Guard.IsNotNull(_dispatcher);
        
        var action = Build();
        _dispatcher.Dispatch(action);
        await action.DialogTaskCompletionSource.Task;
    }
}

public class OpenCnpDialogActionBuilder<TViewModel, TResult>
    where TResult : class
    where TViewModel : class
{
    private readonly ICnpDispatcher? _dispatcher;
    private readonly bool _hasResult;
    private string? _title;
    private DialogSize _size = DialogSize.Medium;
    private string? _okLabel;
    private object? _icon;
    private Type? _dialogContentType;
    private TViewModel? _viewModel;
    private Action<TViewModel, TaskCompletionSource<TResult>>? _onOk;
    private Func<TViewModel, TaskCompletionSource<TResult>, Task>? _onOkAsync;
    private Action? _onCancel;
    private Func<Task>? _onCancelAsync;
    private Action? _onSuccess;
    private Func<Task>? _onSuccessAsync;
    
    internal OpenCnpDialogActionBuilder(ICnpDispatcher dispatcher, bool hasResult, string okLabel)
    {
        _dispatcher = dispatcher;
        _hasResult = hasResult;
        _okLabel = okLabel;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithTitle(string title)
    {
        _title = title;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithSize(DialogSize size)
    {
        _size = size;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOkLabel(string okLabel)
    {
        _okLabel = okLabel;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithIcon(object icon)
    {
        _icon = icon;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithDialogContent<TCnpDataGrid>()
    {
        _dialogContentType = typeof(TCnpDataGrid);
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithViewModel(TViewModel viewModel)
    {
        _viewModel = viewModel;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnOk(Action<TViewModel, TaskCompletionSource<TResult>> onOk)
    {
        _onOk = onOk;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnOkAsync(Func<TViewModel, TaskCompletionSource<TResult>, Task> onOkAsync)
    {
        _onOkAsync = onOkAsync;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnCancel(Action onCancel)
    {
        _onCancel = onCancel;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnCancelAsync(Func<Task> onCancelAsync)
    {
        _onCancelAsync = onCancelAsync;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnSuccess(Action onSuccess)
    {
        _onSuccess = onSuccess;
        return this;
    }
    
    public OpenCnpDialogActionBuilder<TViewModel, TResult> WithOnSuccessAsync(Func<Task> onSuccessAsync)
    {
        _onSuccessAsync = onSuccessAsync;
        return this;
    }
    
    private IOpenCnpDialogTypedAction<TViewModel, TResult> Build()
    {
        Guard.IsNotNull(_dialogContentType);
        Guard.IsNotNull(_viewModel);
        
        return new OpenCnpDialogAction<TViewModel, TResult>()
        {
            Title = _title,
            Size = _size,
            OkLabel = _okLabel,
            Icon = _icon,
            DialogContentType = _dialogContentType,
            ViewModel = _viewModel,
            OnOk = _onOk,
            OnOkAsync = _onOkAsync,
            OnCancel = _onCancel,
            OnCancelAsync = _onCancelAsync,
            OnSuccess = _onSuccess,
            OnSuccessAsync = _onSuccessAsync,
            HasResult = _hasResult
        };
    }

    public async Task<TResult?> AndOpen()
    {
        Guard.IsNotNull(_dispatcher);
        
        var action = Build();
        _dispatcher.Dispatch(action);
        var result = await action.DialogTaskCompletionSource.Task;
        return result;
    }
}