using CretNet.Platform.Blazor.Ui.Extensions;
using CretNet.Platform.Blazor.Ui.Sample;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddCretNetBlazorUi();
builder.Services.AddScoped<CretNet.Platform.Blazor.Ui.Notifications.ICnNotificationClient, SampleNotifications>();
var app = builder.Build();
app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("nl-BE").AddSupportedCultures("nl-BE", "en-GB")
    .AddSupportedUICultures("nl-BE", "en-GB"));
app.MapStaticAssets();
app.UseAntiforgery();
app.MapGet("/culture/{culture}", (string culture, HttpContext context) =>
{
    if (culture is not ("nl-BE" or "en-GB"))
        return Results.BadRequest();

    // The SignalR circuit needs the same culture as the initial HTML request.
    context.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
        CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
        new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Lax, IsEssential = true });
    return Results.LocalRedirect("/");
});
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
