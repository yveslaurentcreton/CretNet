using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnPageTitleTests : BunitContext
{
    public CnPageTitleTests() => Services.AddSingleton<CnBreadcrumbService>();

    [Fact]
    public void Title_OptionalIconAndHide_PreserveHeadingAndBreadcrumbText()
    {
        var cut = Render<CnPageTitle>(p => p.Add(x => x.Title, "Projects").Add(x => x.IsMain, true));
        cut.Find("h1").TextContent.Trim().ShouldBe("Projects");
        cut.FindAll("svg").ShouldBeEmpty();

        cut.Render(p => p.Add(x => x.Icon, CnIconKind.FolderPlan));
        cut.Find("h1").ClassList.ShouldContain("cn-page-title--with-icon");
        cut.Find("h1 > span").TextContent.ShouldBe("Projects");
        cut.FindComponent<CnIcon>().Instance.Kind.ShouldBe(CnIconKind.FolderPlan);
        cut.Find("svg").GetAttribute("aria-hidden").ShouldBe("true");
        Services.GetRequiredService<CnBreadcrumbService>().Items.Single().Text.ShouldBe("Projects");

        cut.Render(p => p.Add(x => x.Hide, true).Add(x => x.Title, "Renamed project"));
        cut.FindAll("h1,svg").ShouldBeEmpty();
        Services.GetRequiredService<CnBreadcrumbService>().Items.Single().Text.ShouldBe("Renamed project");
    }

    [Fact]
    public void Title_ScopedAppearanceChanges_UpdatesDecorativeIcon()
    {
        RenderFragment content = builder =>
        {
            builder.OpenComponent<CnPageTitle>(0);
            builder.AddAttribute(1, nameof(CnPageTitle.Title), "Project with a long name");
            builder.AddAttribute(2, nameof(CnPageTitle.Icon), CnIconKind.FolderPlan);
            builder.CloseComponent();
        };
        var cut = Render<CnIconScope>(p => p
            .Add(x => x.Appearance, new(CnIconStyle.Natural))
            .Add(x => x.ActionAppearance, new CnActionIconAppearance())
            .Add(x => x.ChildContent, content));
        cut.Find("h1 svg").ClassList.ShouldContain("cn-icon--natural");
        cut.Find("h1 svg").GetAttribute("data-icon-role").ShouldBe("entity");
        cut.Render(p => p.Add(x => x.Appearance, new(CnIconStyle.Category, CnIconIntensity.Quiet, false, CnNavigationIconSize.Large)));
        var icon = cut.Find("h1 svg");
        icon.ClassList.ShouldContain("cn-icon--category");
        icon.ClassList.ShouldContain("cn-icon--quiet");
        icon.ClassList.ShouldContain("cn-icon--flat");
        cut.Find("h1 > span").TextContent.ShouldBe("Project with a long name");
    }
}
