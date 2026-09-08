using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnResourceCultureTests : CnTestContext
{
    [Fact]
    public void ResourceDefaults_ExistingControls_UseTheCurrentLanguage()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            var picker = new CnPicker();
            var date = new CnDateField();
            var bell = new CnNotificationBell();
            var toast = new CnToastHost();
            var grid = new CnDataGrid<string>();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            picker.NothingFoundLabel.ShouldBe("Niets gevonden");
            date.ClearLabel.ShouldBe("Wissen");
            bell.Title.ShouldBe("Meldingen");
            toast.AriaLabel.ShouldBe("Meldingen");
            grid.SearchPlaceholder.ShouldBe("Zoeken");
        }
        finally { CultureInfo.CurrentUICulture = original; }
    }

    [Fact]
    public void ResourceDefaults_RenderedControl_ChangesLanguageAndKeepsHostOverrides()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            var cut = Render<CnLoading>(p => p.Add(x => x.IsLoading, true));
            cut.Find("[role=progressbar]").GetAttribute("aria-label").ShouldBe("Loading");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            cut.Render();
            cut.Find("[role=progressbar]").GetAttribute("aria-label").ShouldBe("Laden");
            cut.Render(p => p.Add(x => x.IsLoading, true).Add(x => x.AriaLabel, "HCMT import"));
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            cut.Render();
            cut.Find("[role=progressbar]").GetAttribute("aria-label").ShouldBe("HCMT import");
            cut.Render(p => p.Add(x => x.IsLoading, true).Add(x => x.AriaLabel, ""));
            cut.Find("[role=progressbar]").GetAttribute("aria-label").ShouldBe("");
        }
        finally { CultureInfo.CurrentUICulture = original; }
    }
}
