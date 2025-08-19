#nullable enable

[[UsingsSource]]

namespace [[NamespaceName]];

// Events
public partial class [[SuccessEventName]] : ICnpEvent { }
public partial class [[FailureEventName]] : ICnpEvent { }

// Actions
public partial class [[ClassName]][[InheritanceSource]]
{
    public TaskCompletionSource<[[ResultExpression]]> TaskCompletionSource { get; set; } = new();
    public bool SaveToState { get; set; } = true;

    public static void SubscribeSuccess(IActionSubscriber actionSubscriber, object subscriber, Action<ICnpEntitySuccessAction<[[ResultExpression]]>> callback)
    {
        actionSubscriber.SubscribeToAction<[[SuccessClassName]]>(subscriber, callback);
    }
}
    
public partial class [[SuccessClassName]] : ICnpEntitySuccessAction<[[ResultExpression]]>
{
    public [[ClassName]] SourceAction { get; }
    public [[ResultExpression]] Entity { get; }

    public [[SuccessClassName]]([[ClassName]] sourceAction, [[ResultExpression]] entity)
    {
        SourceAction = sourceAction;
        Entity = entity;
    }
}
    
public partial class [[FailureClassName]]
{
    public [[ClassName]] SourceAction { get; }
    public Exception Exception { get; }

    public [[FailureClassName]]([[ClassName]] sourceAction, Exception exception)
    {
        SourceAction = sourceAction;
        Exception = exception;
    }
}
[[ReducersSource]]
// Effects
public class [[ShortClassName]]Effects
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICnpToastService _toastService;
    private readonly IEntityDefinition? _entityDefinition;

    public [[ShortClassName]]Effects(IServiceProvider serviceProvider, ICnpToastService toastService, IEnumerable<IEntityDefinition> entityDefinitions)
    {
        _serviceProvider = serviceProvider;
        _toastService = toastService;
        _entityDefinition = entityDefinitions.FirstOrDefault(y => y.EntityType == nameof([[EntityClassName]]));
    }
    
    [EffectMethod]
    public async Task Execute([[ClassName]] action, IDispatcher dispatcher)
    {[[DependencyInjectionSource]]
        try
        {
            var entity = await action.Effect(dispatcher);
            dispatcher.Dispatch(new [[SuccessClassName]](action, entity));
        }
        catch (Exception ex) when (ApiExceptionUtils.TryExtractValidationDetails(ex, out var validationMessage))
        {
            dispatcher.Dispatch(new [[FailureClassName]](action, new Exception(validationMessage)));
        }
        catch (Exception ex)
        {
            dispatcher.Dispatch(new [[FailureClassName]](action, ex));
        }
    }

    [EffectMethod]
    public Task Success([[SuccessClassName]] action, IDispatcher dispatcher)
    {[[SuccessMessage]]
        action.SourceAction.TaskCompletionSource.SetResult(action.Entity);[[PostActionsSource]]
        dispatcher.Dispatch(new [[SuccessEventName]]());
        return Task.CompletedTask;
    }
    
    [EffectMethod]
    public Task Failure([[FailureClassName]] action, IDispatcher dispatcher)
    {[[FailureMessage]]
        action.SourceAction.TaskCompletionSource.SetException(action.Exception);
        dispatcher.Dispatch(new [[FailureEventName]]());
        return Task.CompletedTask;
    }
}
