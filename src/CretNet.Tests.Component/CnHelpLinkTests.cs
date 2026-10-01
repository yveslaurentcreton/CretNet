using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>The "?" next to a title opens the manual at its section in a new tab.</summary>
public sealed class CnHelpLinkTests : BunitContext
{
    [Fact]
    public void Renders_AQuietQuestionMark_ThatOpensTheManualInANewTab()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("nl-BE");

        var cut = Render<CnHelpLink>(p => p.Add(x => x.Href, "/docs/company/fiscal-years/#what-follows"));

        var link = cut.Find("a.cn-help-link");
        link.GetAttribute("href").ShouldBe("/docs/company/fiscal-years/#what-follows");
        link.GetAttribute("target").ShouldBe("_blank");
        link.GetAttribute("rel").ShouldBe("noopener");
        link.GetAttribute("aria-label").ShouldBe("Meer in de handleiding");
        var icon = cut.FindComponent<CnIcon>().Instance;
        icon.Kind.ShouldBe(CnIconKind.QuestionCircle);
        icon.Appearance.ShouldBe(new CnIconAppearance(CnIconStyle.Line, CnIconIntensity.Quiet));
    }

    [Fact]
    public void Title_NamesWhatTheManualExplains()
    {
        var cut = Render<CnHelpLink>(p => p
            .Add(x => x.Href, "/docs/en/company/fiscal-years/")
            .Add(x => x.Title, "Fiscal years in the manual"));

        cut.Find("a").GetAttribute("title").ShouldBe("Fiscal years in the manual");
        cut.Find("a").GetAttribute("aria-label").ShouldBe("Fiscal years in the manual");
    }
}
