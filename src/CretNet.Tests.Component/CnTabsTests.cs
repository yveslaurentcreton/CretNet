using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Microsoft.AspNetCore.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnTabsTests : CnTestContext
{
    [Fact]
    public void Render_InitialActiveIndex_SelectsRequestedTab()
    {
        RenderFragment tabs = builder =>
        {
            builder.OpenComponent<CnTab>(0);
            builder.AddAttribute(1, nameof(CnTab.Label), "Dashboard");
            builder.AddAttribute(2, nameof(CnTab.ChildContent),
                (RenderFragment)(content => content.AddContent(0, "dashboard-content")));
            builder.CloseComponent();

            builder.OpenComponent<CnTab>(3);
            builder.AddAttribute(4, nameof(CnTab.Label), "Review");
            builder.AddAttribute(5, nameof(CnTab.ChildContent),
                (RenderFragment)(content => content.AddContent(0, "review-content")));
            builder.CloseComponent();
        };

        var cut = Render<CnTabs>(parameters => parameters
            .Add(component => component.InitialActiveIndex, 1)
            .Add(component => component.ChildContent, tabs));

        cut.Find(".cn-tabs-panel").TextContent.ShouldBe("review-content");
        cut.FindAll("button")[1].ClassList.ShouldContain("cn-tab--active");
    }

    [Fact]
    public void KeyboardNavigation_UsesRovingFocusAndExposesTheActivePanel()
    {
        RenderFragment tabs = builder =>
        {
            builder.OpenComponent<CnTab>(0);
            builder.AddAttribute(1, nameof(CnTab.Label), "Overview");
            builder.AddAttribute(2, nameof(CnTab.ChildContent),
                (RenderFragment)(content => content.AddContent(0, "overview-content")));
            builder.CloseComponent();

            builder.OpenComponent<CnTab>(3);
            builder.AddAttribute(4, nameof(CnTab.Label), "Financials");
            builder.AddAttribute(5, nameof(CnTab.ChildContent),
                (RenderFragment)(content => content.AddContent(0, "financial-content")));
            builder.CloseComponent();
        };

        var cut = Render<CnTabs>(parameters => parameters
            .Add(component => component.ChildContent, tabs));
        var buttons = cut.FindAll("[role=tab]");

        buttons[0].GetAttribute("aria-selected").ShouldBe("true");
        buttons[0].GetAttribute("tabindex").ShouldBe("0");
        buttons[1].GetAttribute("aria-selected").ShouldBe("false");
        buttons[1].GetAttribute("tabindex").ShouldBe("-1");

        buttons[0].KeyDown("ArrowRight");

        buttons = cut.FindAll("[role=tab]");
        buttons[0].GetAttribute("aria-selected").ShouldBe("false");
        buttons[0].GetAttribute("tabindex").ShouldBe("-1");
        buttons[1].GetAttribute("aria-selected").ShouldBe("true");
        buttons[1].GetAttribute("tabindex").ShouldBe("0");
        cut.Find("[role=tabpanel]").GetAttribute("aria-labelledby")
            .ShouldBe(buttons[1].GetAttribute("id"));
        cut.Find("[role=tabpanel]").TextContent.ShouldBe("financial-content");
    }

    [Fact]
    public async Task HostRedraw_ShowsTheNewContentAndCountAtOnce()
    {
        // HCMT BUG-079: the active panel and the count lagged one render
        // behind the host, because CnTabs drew them before the tab received
        // its new parameters.
        var cut = Render<TabsFixture>();
        cut.FindAll(".items li").Count.ShouldBe(1);

        await cut.InvokeAsync(() => cut.Instance.Replace("domain", "hosting", "mail"));

        cut.FindAll(".items li").Count.ShouldBe(3);
        cut.Find(".cn-tab-count").TextContent.ShouldBe("3");
    }
}
