using DotNetJet.FluentValidation.DependencyInjection.Extensions;
using FluentValidation;
using CretNet.Platform.Storage.Services;
using CretNet.Platform.Storage.Sharepoint.Models;
using CretNet.Platform.Storage.Sharepoint.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CretNet.Platform.Storage.Sharepoint.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSharepointStorage(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configuration
        services.AddOptions<SharepointSettings>()
            .Bind(configuration.GetSection(SharepointSettings.Section))
            .ValidateFluentValidation()
            .ValidateOnStart();
        
        // General
        services.AddValidatorsFromAssemblyContaining<IAssemblyMarker>();
        
        // Services
        services.AddScoped<ISharepointClientContextProvider, SharepointClientContextProvider>();
        services.AddScoped<ISharepointService, SharepointService>();
        services.AddScoped<IStorageService, StorageService>();
        
        return services;
    }
}

internal interface IAssemblyMarker { }