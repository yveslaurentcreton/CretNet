using Bunit;
using CretNet.Platform.Blazor.Ui.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace CretNet.Tests.Component;

public class CnTestContext : BunitContext
{
    public CnTestContext()
    {
        Services.AddCretNetBlazorUi();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }
}
