using Fluxor.Blazor.Web.Components;
using CretNet.Platform.Blazor.State.Actions;

namespace CretNet.Platform.Blazor.Components;

public class CnpComponent : FluxorComponent
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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        
        if (disposing)
        {
            OnCleanup();
        }
    }

    protected virtual void OnCleanup()
    {
    }
}