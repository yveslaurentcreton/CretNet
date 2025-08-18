using CretNet.Platform.Blazor.Components;

namespace CretNet.Platform.Blazor.State.Actions.CnpDialog;

public class CnpDialogParameters : ICnpDialogParameters
{
    public Type DialogContentRendererType => typeof(CnpDialogComponentSimpleRenderer);
    public required Type DialogContentType { get; init; }
    public object? Icon { get; init; }
    public string? OkLabel { get; init; }
    public Action<TaskCompletionSource>? OnOk { get; init; }
    public Func<TaskCompletionSource, Task>? OnOkAsync { get; init; }
    
    public object? GetViewModel() => null;
    public async Task<object> InvokeOnOk()
    {
        var tcs = new TaskCompletionSource();
        
        if (OnOk != null)
            OnOk(tcs);
        
        if (OnOkAsync != null)
            await OnOkAsync(tcs);
        
        return tcs.Task;
    }
}

public class CnpDialogParameters<TViewModel> : CnpDialogParameters<TViewModel, TViewModel>
    where TViewModel : class
{
}

public class CnpDialogParameters<TViewModel, TResult> : ICnpDialogParameters<TViewModel>
    where TViewModel : class
    where TResult : class
{
    public Type DialogContentRendererType => typeof(CnpDialogComponentTypedRenderer<TViewModel>);
    public required Type DialogContentType { get; init; }
    public required TViewModel ViewModel { get; init; }
    public object? Icon { get; init; }
    public string? OkLabel { get; init; }
    public Action<TViewModel, TaskCompletionSource<TResult>>? OnOk { get; init; }
    public Func<TViewModel, TaskCompletionSource<TResult>, Task>? OnOkAsync { get; init; }
    public bool HasResult { get; init; }
    
    public object? GetViewModel() => ViewModel;
    public async Task<object> InvokeOnOk()
    {
        var tcs = new TaskCompletionSource<TResult>();
        
        if (OnOk != null)
            OnOk(ViewModel, tcs);
        
        if (OnOkAsync != null)
            await OnOkAsync(ViewModel, tcs);
        
        if (HasResult)
            return await tcs.Task;
        
        return ViewModel;
    }
}

public interface ICnpDialogParameters
{
    Type DialogContentRendererType { get; }
    Type DialogContentType { get; }
    object? Icon { get; }
    string? OkLabel { get; }
    
    object? GetViewModel();
    Task<object> InvokeOnOk();
}

public interface ICnpDialogParameters<TViewModel> : ICnpDialogParameters
{
    TViewModel ViewModel { get; init; }
}