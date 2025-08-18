using CretNet.Platform.Data.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CretNet.Platform.Data.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCnpData<TAssembly>(
        this IServiceCollection services)
    {
        services.Scan(scan => scan.FromAssemblyOf<TAssembly>()
            .AddClasses(classes => classes.AssignableTo(typeof(Repository<,>))
            .Where(type => !type.IsGenericTypeDefinition && !type.IsAbstract), publicOnly: false)
            .AsImplementedInterfaces()
            .WithScopedLifetime());
        services.TryAddScoped(typeof(IRepository<,>), typeof(Repository<,>));

        services.Scan(scan => scan.FromAssemblyOf<TAssembly>()
            .AddClasses(classes => classes.AssignableTo(typeof(IEntityDefaultSpecification<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        
        services.Scan(scan => scan.FromAssemblyOf<TAssembly>()
            .AddClasses(classes => classes.AssignableTo(typeof(IEntitySearchSpecification<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());
        
        return services;
    }
}
