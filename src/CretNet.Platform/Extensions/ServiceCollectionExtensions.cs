using Microsoft.Extensions.DependencyInjection;

namespace CretNet.Platform.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDecoratedSingleton<TService, TDecorator, TImplementation>(this IServiceCollection services, object key)
        where TService : class
        where TImplementation : class, TService
        where TDecorator : class, TService
    {
        services.AddKeyedSingleton<TService, TImplementation>(key);
        services.AddSingleton<TService, TDecorator>();
        return services;
    }
}