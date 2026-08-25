using Microsoft.Extensions.DependencyInjection;

namespace CretNet.Platform.Blazor.Ui.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers the services behind the Cn component set.
    /// </summary>
    /// <remarks>
    /// The set is lifted from HCMT one component at a time (WAM ADR-007);
    /// registrations appear here as the components that need them arrive
    /// (theme service, dialog service, …). The call site is stable from day
    /// one so consumers wire it once and never chase this signature.
    /// </remarks>
    public static IServiceCollection AddCretNetBlazorUi(this IServiceCollection services)
    {
        services.AddScoped<Theme.CnThemeService>();
        services.AddScoped<Dialogs.CnDialogService>();
        return services;
    }
}
