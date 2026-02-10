using Fluxor.Blazor.Web.Components;
using CretNet.Platform.Blazor.State.Actions;

namespace CretNet.Platform.Blazor.Components;

public class CnpComponent : FluxorComponent, IDisposable
{
    protected override void OnInitialized()
    {
        base.OnInitialized();
        
        SubscribeToAction<ChangeCultureSuccessAction>(_ => InvokeAsync(StateHasChanged));
    }
    
    public void SubscribeToActionAsync<TAction>(Func<TAction, Task> asyncActionFunc)
    {
        SubscribeToAction<TAction>((action) =>
        {
            Task.Run(async () => await asyncActionFunc(action));
        });
    }

    void IDisposable.Dispose()
    {
        OnCleanup();
        GC.SuppressFinalize(this);
    }

    protected virtual void OnCleanup()
    {
    }
}