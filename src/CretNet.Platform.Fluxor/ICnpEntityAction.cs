using Fluxor;

namespace CretNet.Platform.Fluxor;

public interface ICnpEntityAction<TEntity>
{
    TaskCompletionSource<TEntity> TaskCompletionSource { get; }
    bool SaveToState { get; set; }
    Task<TEntity> Effect(IDispatcher dispatcher);
}

public interface ICnpEntitySuccessAction<TEntity>
{
    TEntity Entity { get; }
}

public interface ICnpEntityActionReducer<TEntity, TState>
{
    TState Reduce(TState state, ICnpEntitySuccessAction<TEntity> action);
}

public interface ICnpEntityActionPostActions<TEntity>
{
    Task PostActions(ICnpEntitySuccessAction<TEntity> action, IDispatcher dispatcher);
}