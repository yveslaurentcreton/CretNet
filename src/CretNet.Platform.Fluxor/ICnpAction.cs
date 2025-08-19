using Fluxor;

namespace CretNet.Platform.Fluxor;

public interface ICnpAction
{
    Task Task { get; }
}

public interface ICnpActionEffect
{
    Task Effect(IDispatcher dispatcher);
}

public interface ICnpActionReducer<TState>
{
    TState Reduce(TState state);
}