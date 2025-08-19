using Fluxor;
using Fluxor.DependencyInjection;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Blazor.Services.Countries;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCnpBlazor(
        this IServiceCollection services,
        Action<FluxorOptions>? configureFluxor = null)
    {
        services.AddFluxor(options =>
        {
            options.UseRouting();
            options.ScanAssemblies(typeof(IAssemblyMarker).Assembly);
            configureFluxor?.Invoke(options);
        });
        
        // Fluent UI
        services.AddFluentUIComponents();
        
        // Implementations
        services.AddScoped<ICnpToastService, CnpToastService>();
        services.AddScoped<IBreadcrumbService, BreadcrumbService>();
        services.AddScoped<ICnpSectionService, CnpSectionService>();
        services.AddScoped<INavigationService, NavigationService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<ITimeService, TimeService>();

        services.Scan(scan =>
        {
            scan.FromAssembliesOf(typeof(IAssemblyMarker))
                .AddClasses(classes => classes.AssignableTo(typeof(ICnpDataSource<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime();
        });
        
        return services;
    }
}

internal interface IAssemblyMarker;
