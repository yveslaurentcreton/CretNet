using Fluxor;

namespace CretNet.Platform.Fluxor;

public static class DispatcherExtensions
{
    public static Task DispatchAsync(this IDispatcher dispatcher, ICnpAction action)
    {
        dispatcher.Dispatch(action);
        return action.Task;
    }
    
    public static Task<TResult> DispatchAsync<TResult>(this IDispatcher dispatcher, ICnpEntityAction<TResult> action, bool saveToState = false)
    {
        action.SaveToState = saveToState;
        var tcs = action.TaskCompletionSource;
        dispatcher.Dispatch(action);
        return tcs.Task;
    }
}