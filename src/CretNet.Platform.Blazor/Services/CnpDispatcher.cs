using Fluxor;

namespace CretNet.Platform.Blazor.Services;

public static class CnpDispatcherExtensions
{
    public static ICnpDispatcher AsCnp(this IDispatcher dispatcher)
    {
        return new CnpDispatcher(dispatcher);
    }
}

public interface ICnpDispatcher
{
    void Dispatch(object action);
}

public class CnpDispatcher : ICnpDispatcher
{
    private readonly IDispatcher _dispatcher;
    
    internal CnpDispatcher(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    public void Dispatch(object action)
    {
        _dispatcher.Dispatch(action);
    }
}