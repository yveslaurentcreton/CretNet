using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>HCMT M-064 / S-491: CnTimeline scrolls through time.</summary>
public sealed class CnTimelineScrollTests : CnTestContext
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnTimeline.razor.js";

    // 01/03/2024 – 31/10/2028: 56 whole months; today is month 31.
    private static readonly DateOnly AxisFrom = new(2024, 3, 15);
    private static readonly DateOnly AxisTo = new(2028, 10, 1);
    private static readonly DateOnly Today = new(2026, 10, 10);

    private readonly BunitJSModuleInterop _module;
    private readonly CultureInfo _culture = CultureInfo.CurrentUICulture;

    public CnTimelineScrollTests()
    {
        _module = JSInterop.SetupModule(ModulePath);
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
    }

    protected override void Dispose(bool disposing)
    {
        CultureInfo.CurrentUICulture = _culture;
        base.Dispose(disposing);
    }

    private static CnTimelineRow PriceRow() => new()
    {
        Label = "Sales",
        Value = "€ 179,00",
        SecondValue = "29 %",
        Lanes =
        [
            new CnTimelineLane
            {
                Segments =
                [
                    new CnTimelineSegment { From = new(2026, 1, 1), To = new(2026, 12, 31), Tone = CnTimelineSegmentTone.Due, Text = "€ 179,00", Key = "now" },
                    new CnTimelineSegment { From = new(2027, 1, 1), To = DateOnly.MaxValue, OpenEnd = true, Tone = CnTimelineSegmentTone.Later, Text = "€ 185,00", Key = "later" },
                ],
            },
        ],
    };

    private IRenderedComponent<CnTimeline> RenderScrollable(
        IReadOnlyList<CnTimelineRow>? rows = null,
        int? visibleMonths = null,
        string? storageKey = null,
        bool showValue = true,
        Action<CnTimelineSegmentClick>? segmentClick = null) =>
        Render<CnTimeline>(p =>
        {
            p.Add(x => x.Rows, rows ?? [PriceRow()])
             .Add(x => x.From, AxisFrom)
             .Add(x => x.To, AxisTo)
             .Add(x => x.Today, Today)
             .Add(x => x.Scrollable, true)
             .Add(x => x.ShowValue, showValue);
            if (visibleMonths is { } months)
                p.Add(x => x.VisibleMonths, months);
            if (storageKey is not null)
                p.Add(x => x.ZoomStorageKey, storageKey);
            if (segmentClick is not null)
                p.Add(x => x.OnSegmentClick, segmentClick);
        });

    [Fact]
    public void Scrollable_WrapsTheGridInAScrollerUnderTheNavigator()
    {
        var cut = RenderScrollable();

        var frame = cut.Find(".cn-gantt-frame");
        frame.Children.Select(x => x.ClassName).ShouldBe(["cn-gantt-tools", "cn-gantt-scroll"]);
        var scroller = cut.Find(".cn-gantt-scroll");
        scroller.GetAttribute("tabindex").ShouldBe("0");
        scroller.GetAttribute("aria-label").ShouldBe("Timeline");
        scroller.Children.Single().ClassList.ShouldContain("cn-gantt--scroll");

        // Today a third from the left of a one-year view: June to May.
        cut.Find(".cn-gantt-range").TextContent.ShouldBe("jun 2026 – may 2027");
        cut.FindAll("[data-cn-gantt-go]").Select(x => x.GetAttribute("aria-label") ?? x.TextContent)
            .ShouldBe(["Earlier", "Today", "Later"]);
        cut.FindAll(".cn-gantt-zoom [role=tab]").Select(x => x.TextContent).ShouldBe(["6 mo", "1 year", "3 years", "All"]);
        cut.Find(".cn-gantt-zoom [aria-selected=true]").TextContent.ShouldBe("1 year");
    }

    [Fact]
    public void Scrollable_ControlsAreLocalized()
    {
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
        var cut = RenderScrollable();

        cut.Find(".cn-gantt-scroll").GetAttribute("aria-label").ShouldBe("Tijdlijn");
        cut.FindAll("[data-cn-gantt-go]").Select(x => x.GetAttribute("aria-label") ?? x.TextContent)
            .ShouldBe(["Eerder", "Vandaag", "Later"]);
        cut.FindAll(".cn-gantt-zoom [role=tab]").Select(x => x.TextContent).ShouldBe(["6 mnd", "1 jaar", "3 jaar", "Alles"]);
        cut.Find(".cn-gantt-zoom").GetAttribute("aria-label").ShouldBe("Zoom");
    }

    [Fact]
    public void Scrollable_SizesTheTrackInWholeMonthsOfTheVisibleWidth()
    {
        var cut = RenderScrollable();

        var grid = cut.Find(".cn-gantt");
        grid.GetAttribute("style").ShouldBe("--cn-gantt-months: 56; --cn-gantt-visible: 12; --cn-gantt-year-at: 10");
        grid.GetAttribute("data-months").ShouldBe("56");
        grid.GetAttribute("data-today").ShouldBe("0.558756");
        grid.ClassList.ShouldNotContain("cn-gantt--dense");

        // Dates are placed within their month: 2026 is months 22–33 of 56.
        var segments = cut.FindAll(".cn-gantt-seg");
        segments[0].GetAttribute("style").ShouldBe("left: calc(39.286% + 1px); width: max(2px, calc(21.429% - 2px))");
        segments[1].ClassList.ShouldContain("cn-gantt-seg--open-r");
        segments[1].ClassList.ShouldNotContain("cn-gantt-seg--cut-r");
        cut.Find(".cn-gantt-today").GetAttribute("style").ShouldBe("left: 55.876%");
    }

    [Fact]
    public void Scrollable_NamesEveryMonthUnderAStickyYearRow()
    {
        var cut = RenderScrollable();

        var years = cut.FindAll(".cn-gantt-yr");
        years.Select(x => x.TextContent).ShouldBe(["2024", "2025", "2026", "2027", "2028"]);
        years[0].GetAttribute("style").ShouldBe("left: 0%; width: 17.857%");
        years[1].GetAttribute("style").ShouldBe("left: 17.857%; width: 21.429%");

        var months = cut.FindAll(".cn-gantt-mo");
        months.Count.ShouldBe(56);
        months[0].TextContent.ShouldBe("mar");
        months[10].TextContent.ShouldBe("jan");
        months[10].ClassList.ShouldContain("cn-gantt-mo--year");
        months[31].ClassList.ShouldContain("cn-gantt-mo--now");
        months[31].ClassList.ShouldNotContain("cn-gantt-mo--minor"); // October starts a quarter
        months[32].ClassList.ShouldContain("cn-gantt-mo--minor");
        months[33].ClassList.ShouldContain("cn-gantt-mo--alt"); // December
    }

    [Fact]
    public void Scrollable_MakesEveryRowASubgridRowWithStickyFigures()
    {
        var plain = new CnTimelineRow { Label = "Plain", From = new(2026, 2, 1), To = new(2026, 3, 31), Value = "1", SecondValue = "2" };
        var cut = RenderScrollable([PriceRow(), plain]);

        cut.FindAll(".cn-gantt-row").Count.ShouldBe(2);
        cut.Find(".cn-gantt-h--v1").TextContent.ShouldBe("Value");
        cut.Find(".cn-gantt-h--v2").TextContent.ShouldBe("Amount");
        cut.FindAll(".cn-gantt-n--v1").Count.ShouldBe(2);
        cut.FindAll(".cn-gantt-n--v2").Count.ShouldBe(2);
        cut.Find(".cn-gantt-bar").GetAttribute("style").ShouldBe("left: 41.071%; width: 3.571%");
    }

    [Fact]
    public void ShowValueFalse_DropsBothFigureColumns()
    {
        var cut = RenderScrollable(showValue: false);

        cut.Find(".cn-gantt").ClassList.ShouldContain("cn-gantt--novalue");
        cut.FindAll(".cn-gantt-n").ShouldBeEmpty();
        cut.FindAll(".cn-gantt-h--r").ShouldBeEmpty();
    }

    [Fact]
    public void Zoom_ChangesTheVisibleMonthsAndIsRemembered()
    {
        var cut = RenderScrollable(storageKey: "item-price");

        cut.FindAll(".cn-gantt-zoom [role=tab]")[2].Click();

        var grid = cut.Find(".cn-gantt");
        grid.GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 36;");
        grid.ClassList.ShouldContain("cn-gantt--labels-quarter");
        cut.Find(".cn-gantt-zoom [aria-selected=true]").TextContent.ShouldBe("3 years");
        _module.VerifyInvoke("writeZoom").Arguments.ShouldBe(new object?[] { "item-price", 36 });

        cut.FindAll(".cn-gantt-zoom [role=tab]")[3].Click();
        cut.Find(".cn-gantt").GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 56;");
        _module.VerifyInvoke("writeZoom", 2)[1].Arguments.ShouldBe(new object?[] { "item-price", 0 });
    }

    [Fact]
    public void Zoom_WithoutAKey_IsNotRemembered()
    {
        var cut = RenderScrollable();

        cut.FindAll(".cn-gantt-zoom [role=tab]")[0].Click();

        cut.Find(".cn-gantt").GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 6;");
        _module.VerifyNotInvoke("writeZoom");
        _module.VerifyNotInvoke("readZoom");
    }

    [Fact]
    public void RememberedZoom_IsReadBeforeTheTimelineStartsAtToday()
    {
        _module.Setup<int?>("readZoom", "subscription").SetResult(6);

        var cut = RenderScrollable(storageKey: "subscription");

        cut.WaitForAssertion(() => cut.Find(".cn-gantt").GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 6;"));
        cut.WaitForAssertion(() => _module.VerifyInvoke("scrollToToday"));
        _module.VerifyInvoke("attach");
        _module.VerifyInvoke("scrollToToday").Arguments[1].ShouldBe(false);
        cut.Find(".cn-gantt-range").TextContent.ShouldBe("aug 2026 – jan 2027");
    }

    [Fact]
    public void RememberedZoom_OutsideTheChoices_IsIgnored()
    {
        _module.Setup<int?>("readZoom", "subscription").SetResult(7);

        var cut = RenderScrollable(storageKey: "subscription");

        cut.WaitForAssertion(() => _module.VerifyInvoke("attach"));
        cut.Find(".cn-gantt").GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 12;");
    }

    [Fact]
    public async Task ReportedView_UpdatesTheRangeAndDisablesTheArrowAtAnEnd()
    {
        var cut = RenderScrollable();
        cut.WaitForAssertion(() => _module.VerifyInvoke("attach"));

        await cut.InvokeAsync(() => cut.Instance.OnViewChanged(0, 11, atStart: true, atEnd: false));

        cut.Find(".cn-gantt-range").TextContent.ShouldBe("mar 2024 – feb 2025");
        cut.Find("[data-cn-gantt-go='-1']").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("[data-cn-gantt-go='1']").HasAttribute("disabled").ShouldBeFalse();

        await cut.InvokeAsync(() => cut.Instance.OnViewChanged(44, 55, atStart: false, atEnd: true));

        cut.Find(".cn-gantt-range").TextContent.ShouldBe("nov 2027 – oct 2028");
        cut.Find("[data-cn-gantt-go='-1']").HasAttribute("disabled").ShouldBeFalse();
        cut.Find("[data-cn-gantt-go='1']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Navigator_MovesHalfAViewOrBackToToday()
    {
        var cut = RenderScrollable();
        cut.WaitForAssertion(() => _module.VerifyInvoke("attach"));

        cut.Find("[data-cn-gantt-go='-1']").Click();
        cut.Find("[data-cn-gantt-go='1']").Click();
        cut.Find("[data-cn-gantt-go='0']").Click();

        var pages = _module.VerifyInvoke("page", 2);
        pages[0].Arguments[1].ShouldBe(-1);
        pages[1].Arguments[1].ShouldBe(1);
        var today = _module.VerifyInvoke("scrollToToday", 2);
        today[1].Arguments[1].ShouldBe(true);
    }

    [Fact]
    public void SegmentClick_StillReachesTheHost()
    {
        var clicks = new List<CnTimelineSegmentClick>();
        var cut = RenderScrollable(segmentClick: clicks.Add);

        cut.FindAll("button.cn-gantt-seg")[1].Click();

        clicks.Single().Segment.Key.ShouldBe("later");
    }

    [Fact]
    public void VisibleMonthsZero_ShowsTheWholeAxis()
    {
        var cut = RenderScrollable(visibleMonths: 0);

        var grid = cut.Find(".cn-gantt");
        grid.GetAttribute("style")!.ShouldContain("--cn-gantt-visible: 56;");
        grid.ClassList.ShouldContain("cn-gantt--labels-quarter");
        cut.Find(".cn-gantt-zoom [aria-selected=true]").TextContent.ShouldBe("All");
        cut.Find(".cn-gantt-range").TextContent.ShouldBe("mar 2024 – oct 2028");
        cut.Find("[data-cn-gantt-go='-1']").HasAttribute("disabled").ShouldBeTrue();
        cut.Find("[data-cn-gantt-go='1']").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void NotScrollable_RendersNoNavigatorAndNoModule()
    {
        var cut = Render<CnTimeline>(p => p
            .Add(x => x.Rows, [PriceRow()])
            .Add(x => x.From, new DateOnly(2026, 1, 1))
            .Add(x => x.To, new DateOnly(2026, 12, 31))
            .Add(x => x.Today, Today));

        cut.FindAll(".cn-gantt-frame").ShouldBeEmpty();
        cut.FindAll(".cn-gantt-yr").ShouldBeEmpty();
        var grid = cut.Find(".cn-gantt");
        grid.HasAttribute("data-months").ShouldBeFalse();
        grid.HasAttribute("data-today").ShouldBeFalse();
        grid.ClassList.ShouldNotContain("cn-gantt--scroll");
        cut.FindAll(".cn-gantt-n--v1").ShouldBeEmpty();
        _module.VerifyNotInvoke("attach");
    }

    [Theory]
    [InlineData("2026-10-10", "2026-06-01", "2027-10-31")] // nothing yet: four months back, a year ahead
    [InlineData("2021-03-01", "2021-03-01", "2027-10-31")] // back to the first date, however old
    [InlineData("2028-09-25", "2026-06-01", "2029-09-30")] // a year past the last planned piece
    public void Extent_RunsFromTheFirstDateToAYearPastTodayOrTheLastDate(string date, string from, string to)
    {
        var extent = CnTimelineAxis.Extent([DateOnly.Parse(date, CultureInfo.InvariantCulture), DateOnly.MaxValue, DateOnly.MinValue], Today);

        extent.ShouldBe((DateOnly.Parse(from, CultureInfo.InvariantCulture), DateOnly.Parse(to, CultureInfo.InvariantCulture)));
    }
}
