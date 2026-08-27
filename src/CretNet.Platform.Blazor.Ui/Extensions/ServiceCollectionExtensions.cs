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
    /// <param name="accent">
    /// This host's accent, used until the user picks one. Omit it and every
    /// first visit opens in HCMT's brand green, which is only right for HCMT.
    /// </param>
    public static IServiceCollection AddCretNetBlazorUi(this IServiceCollection services, string? accent = null)
    {
        services.AddScoped(provider => new Theme.CnThemeService(
            provider.GetRequiredService<Microsoft.JSInterop.IJSRuntime>(), accent));
        services.AddScoped<Dialogs.CnDialogService>();
        services.AddScoped<Toasts.CnToastService>();

        // The inbox needs a transport the host owns; without one there is
        // nothing to keep state about, so the state only registers when a
        // client has been registered before this call.
        services.AddScoped<Notifications.CnNotificationState>();
        return services;
    }
}
