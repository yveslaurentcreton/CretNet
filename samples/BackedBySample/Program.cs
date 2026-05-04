using BackedBySample;
using BackedBySample.Models;
using BackedBySample.Services;
using CretNet.Platform.Blazor.Services;
using Fluxor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluxor(o => o.ScanAssemblies(typeof(Program).Assembly));
builder.Services.AddSingleton<InMemoryCustomerService>();
builder.Services.AddSingleton<IEntityDefinition<SampleCustomer, Guid>, SampleCustomersDefinition>();
builder.Services.AddSingleton<IEntityDefinition>(sp => sp.GetRequiredService<IEntityDefinition<SampleCustomer, Guid>>());

await builder.Build().RunAsync();
