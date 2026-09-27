using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Microsoft.AspNetCore.Components.Web;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnViewSwitchAndGridTemplateTests : CnTestContext
{
    public enum Period { Day, Week, Month }

    private IRenderedComponent<CnViewSwitch<Period>> RenderSwitch(Period value, Action<Period> changed) =>
        Render<CnViewSwitch<Period>>(p => p
            .Add(x => x.Options, Enum.GetValues<Period>())
            .Add(x => x.Value, value)
            .Add(x => x.OptionText, period => period.ToString().ToLowerInvariant())
            .Add(x => x.ValueChanged, changed));

    [Fact]
    public void ViewSwitch_RendersTablistWithRovingTabindex()
    {
        var cut = RenderSwitch(Period.Week, _ => { });

        cut.Find(".cn-view-switch").GetAttribute("role").ShouldBe("tablist");
        var tabs = cut.FindAll("[role=tab]");
        tabs.Select(x => x.TextContent).ShouldBe(["day", "week", "month"]);
        tabs.Select(x => x.GetAttribute("aria-selected")).ShouldBe(["false", "true", "false"]);
        tabs.Select(x => x.GetAttribute("tabindex")).ShouldBe(["-1", "0", "-1"]);
        tabs[1].ClassList.ShouldContain("cn-view-switch__option--active");
        tabs[0].ClassList.ShouldNotContain("cn-view-switch__option--active");
    }

    [Fact]
    public void ViewSwitch_ClickRaisesValueChangedAndMovesSelection()
    {
        var changes = new List<Period>();
        var cut = RenderSwitch(Period.Day, changes.Add);

        cut.FindAll("[role=tab]")[2].Click();
        cut.FindAll("[role=tab]")[2].Click();

        changes.ShouldBe([Period.Month]);
        cut.FindAll("[role=tab]")[2].GetAttribute("aria-selected").ShouldBe("true");
        cut.FindAll("[role=tab]")[2].GetAttribute("tabindex").ShouldBe("0");
        cut.FindAll("[role=tab]")[0].GetAttribute("tabindex").ShouldBe("-1");
    }

    [Theory]
    [InlineData(1, "ArrowRight", Period.Month)]
    [InlineData(1, "ArrowDown", Period.Month)]
    [InlineData(1, "ArrowLeft", Period.Day)]
    [InlineData(0, "ArrowLeft", Period.Month)]
    [InlineData(2, "ArrowRight", Period.Day)]
    [InlineData(1, "Home", Period.Day)]
    [InlineData(0, "End", Period.Month)]
    public void ViewSwitch_KeyboardSelectsWithWrap(int start, string key, Period expected)
    {
        var changes = new List<Period>();
        var cut = RenderSwitch((Period)start, changes.Add);

        cut.FindAll("[role=tab]")[start].KeyDown(new KeyboardEventArgs { Key = key });

        changes.ShouldBe([expected]);
        cut.FindAll("[role=tab]")[(int)expected].GetAttribute("aria-selected").ShouldBe("true");
    }

    [Fact]
    public void ViewSwitch_OtherKeysAndDefaultsLeaveSelectionAlone()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            var changes = new List<Period>();
            var cut = RenderSwitch(Period.Week, changes.Add);
            cut.FindAll("[role=tab]")[1].KeyDown(new KeyboardEventArgs { Key = "a" });
            changes.ShouldBeEmpty();
            cut.Find(".cn-view-switch").GetAttribute("aria-label").ShouldBe("Weergave");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            cut.Render();
            cut.Find(".cn-view-switch").GetAttribute("aria-label").ShouldBe("View");
        }
        finally { CultureInfo.CurrentUICulture = original; }
    }

    [Fact]
    public void Grid_ItemsTemplate_RendersCurrentPageInsteadOfTable()
    {
        var cut = Render<GridTemplateFixture>();

        cut.WaitForAssertion(() => cut.FindAll("article.card").Count.ShouldBe(2));
        cut.FindAll("table").ShouldBeEmpty();
        cut.FindAll("article.card").Select(x => x.TextContent).ShouldBe(["Drill", "Saw"]);
        cut.Find(".cn-grid-footer .cn-card-hint").TextContent.ShouldBe("1/2 — 3");

        cut.FindAll(".cn-grid-footer button")[1].Click();

        cut.WaitForAssertion(() => cut.FindAll("article.card").Select(x => x.TextContent).ShouldBe(["Hammer"]));
        cut.Instance.Requests.Last().PageIndex.ShouldBe(2);
    }

    [Fact]
    public async Task Grid_ItemsTemplate_ShowsEmptyTextAndReloads()
    {
        var cut = Render<GridTemplateFixture>(p => p.Add(x => x.Rows, []));

        cut.WaitForAssertion(() => cut.Find("div.cn-grid-empty").TextContent.ShouldBe("No products"));
        cut.FindAll(".cards").ShouldBeEmpty();

        cut.Instance.Rows.Add("Ladder");
        await cut.InvokeAsync(() => cut.Instance.Grid.ReloadAsync());

        cut.WaitForAssertion(() => cut.FindAll("article.card").Select(x => x.TextContent).ShouldBe(["Ladder"]));
        cut.FindAll(".cn-grid-empty").ShouldBeEmpty();
    }

    [Fact]
    public void Grid_WithoutItemsTemplate_KeepsTheTable()
    {
        var cut = Render<GridTemplateFixture>(p => p.Add(x => x.UseTemplate, false));

        cut.WaitForAssertion(() => cut.FindAll("tbody tr").Count.ShouldBe(2));
        cut.Find("table.cn-grid-table th").TextContent.Trim().ShouldBe("Name");
        cut.FindAll("article.card").ShouldBeEmpty();
    }
}
