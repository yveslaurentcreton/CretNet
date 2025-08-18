namespace CretNet.Platform.Blazor.State.Actions.CnpDialog;

public record OpenCnpDialogAction : IOpenCnpDialogAction, IOpenCnpDialogSimpleAction
{
    public required Type DialogContentType { get; init; }
    public Action<TaskCompletionSource>? OnOk { get; init; }
    public Func<TaskCompletionSource, Task>? OnOkAsync { get; init; }
    public Action? OnCancel { get; init; }
    public Func<Task>? OnCancelAsync { get; init; }
    public Action? OnSuccess { get; init; }
    public Func<Task>? OnSuccessAsync { get; init; }
    public string? Title { get; init; }
    public DialogSize Size { get; init; }
    public string? OkLabel { get; init; }
    public object? Icon { get; init; }
    public TaskCompletionSource DialogTaskCompletionSource { get; } = new();
    
    public ICnpDialogParameters BuildParameters()
    {
        return new CnpDialogParameters()
        {
            DialogContentType = DialogContentType,
            Icon = Icon,
            OkLabel = OkLabel,
        };
    }
    
    public async Task InvokeOnCancel()
    {
        if (OnCancel != null)
            OnCancel();
        
        if (OnCancelAsync != null)
            await OnCancelAsync();
    }
    
    public async Task InvokeOnSuccess()
    {
        if (OnSuccess != null)
            OnSuccess();
        
        if (OnSuccessAsync != null)
            await OnSuccessAsync();
    }
    
    public void SetResult(object? result)
    {
        DialogTaskCompletionSource.SetResult();
    }
}

public record OpenCnpDialogAction<TViewModel, TResult> : IOpenCnpDialogAction, IOpenCnpDialogTypedAction<TViewModel, TResult>
    where TResult : class
    where TViewModel : class
{
    public string? Title { get; init; }
    public DialogSize Size { get; init; } = DialogSize.Medium;
    public string? OkLabel { get; init; }
    public object? Icon { get; init; }
    public required Type DialogContentType { get; init; }
    public required TViewModel ViewModel { get; init; }
    public Action<TViewModel, TaskCompletionSource<TResult>>? OnOk { get; init; }
    public Func<TViewModel, TaskCompletionSource<TResult>, Task>? OnOkAsync { get; init; }
    public Action? OnCancel { get; init; }
    public Func<Task>? OnCancelAsync { get; init; }
    public Action? OnSuccess { get; init; }
    public Func<Task>? OnSuccessAsync { get; init; }
    public TaskCompletionSource<TResult?> DialogTaskCompletionSource { get; } = new();
    public bool HasResult { get; init; }
    
    public ICnpDialogParameters BuildParameters()
    {
        return new CnpDialogParameters<TViewModel, TResult>
        {
            DialogContentType = DialogContentType,
            ViewModel = ViewModel,
            Icon = Icon,
            OkLabel = OkLabel,
            OnOk = OnOk,
            OnOkAsync = OnOkAsync,
            HasResult = HasResult
        };
    }
    
    public void SetResult(object? result)
    {
        DialogTaskCompletionSource.SetResult(result as TResult);
    }
    
    public async Task InvokeOnCancel()
    {
        if (OnCancel != null)
            OnCancel();
        
        if (OnCancelAsync != null)
            await OnCancelAsync();
    }
    
    public async Task InvokeOnSuccess()
    {
        if (OnSuccess != null)
            OnSuccess();
        
        if (OnSuccessAsync != null)
            await OnSuccessAsync();
    }
}

public interface IOpenCnpDialogAction
{
    string? Title { get; }
    DialogSize Size { get; }
    ICnpDialogParameters BuildParameters();
    Task InvokeOnCancel();
    Task InvokeOnSuccess();
    void SetResult(object? result);
}

public interface IOpenCnpDialogSimpleAction
{
    Type DialogContentType { get; }
    Action<TaskCompletionSource>? OnOk { get; }
    Func<TaskCompletionSource, Task>? OnOkAsync { get; init; }
    Action? OnCancel { get; }
    Func<Task>? OnCancelAsync { get; init; }
    string? Title { get; }
    TaskCompletionSource DialogTaskCompletionSource { get; }
}

public interface IOpenCnpDialogTypedAction<TViewModel, TResult>
{
    Type DialogContentType { get; }
    Action<TViewModel, TaskCompletionSource<TResult>>? OnOk { get; }
    Func<TViewModel, TaskCompletionSource<TResult>, Task>? OnOkAsync { get; init; }
    Action? OnCancel { get; }
    Func<Task>? OnCancelAsync { get; init; }
    string? Title { get; }
    TaskCompletionSource<TResult?> DialogTaskCompletionSource { get; }
}

public enum DialogSize
{
    Small,
    Medium,
    Large,
    ExtraLarge,
    FullScreen
}

public static class DialogSizeExtensions
{
    public static string ToPixels(this DialogSize size)
    {
        return size switch
        {
            DialogSize.Small => "300px",
            DialogSize.Medium => "500px",
            DialogSize.Large => "1140px",
            DialogSize.ExtraLarge => "1500px",
            DialogSize.FullScreen => "100%",
            _ => throw new NotImplementedException()
        };
    }
}