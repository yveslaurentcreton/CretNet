using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Fluxor;
using CretNet.Platform.Fluxor;
// using CretNet.Platform.Fluxor.Generators; // not required in consumer code

namespace [[NamespaceName]];

// Events
public partial class [[PostEventName]] : ICnpEvent { }

public partial class [[ClassName]][[InheritanceSource]]
{
    public TaskCompletionSource TaskCompletionSource { get; } = new();
    public Task Task => TaskCompletionSource.Task;
}
[[ReducersSource]]
// Effects
public class [[ShortClassName]]Effects
{
    private IServiceProvider _serviceProvider;

    public [[ShortClassName]]Effects(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    [EffectMethod]
    public async Task Execute([[ClassName]] action, IDispatcher dispatcher)
    {[[DependencyInjectionSource]][[EffectSource]]
        action.TaskCompletionSource.SetResult();
        dispatcher.Dispatch(new [[PostEventName]]());
        await Task.CompletedTask;
    }
}
