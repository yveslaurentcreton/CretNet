using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnTimelineTests : CnTestContext
{
    private static readonly DateOnly AxisFrom = new(2026, 1, 1);
    private static readonly DateOnly AxisTo = new(2026, 12, 31);

    private IRenderedComponent<CnTimeline> RenderTimeline(
        IReadOnlyList<CnTimelineRow> rows,
        Action<CnTimelineRow>? rowClick = null,
        Action<CnTimelineSegmentClick>? segmentClick = null,
        object? selectedKey = null) =>
        Render<CnTimeline>(p =>
        {
            p.Add(x => x.Rows, rows)
             .Add(x => x.From, AxisFrom)
             .Add(x => x.To, AxisTo)
             .Add(x => x.Today, new DateOnly(2026, 7, 2))
             .Add(x => x.SelectedKey, selectedKey);
            if (rowClick is not null)
            {
                p.Add(x => x.OnRowClick, rowClick);
            }

            if (segmentClick is not null)
            {
                p.Add(x => x.OnSegmentClick, segmentClick);
            }
        });

    private static CnTimelineRow LaneRow(string key = "svc-1") => new()
    {
        Label = "Microsoft 365",
        Note = "M365-BP",
        Key = key,
        Value = "01/10/2026",
        Lanes =
        [
            new CnTimelineLane
            {
                Label = "A",
                Thin = true,
                Segments =
                [
                    new CnTimelineSegment { From = new(2025, 10, 1), To = new(2026, 9, 30), Tone = CnTimelineSegmentTone.Purchase, Title = "Purchase", Key = "pp-1" },
                ],
            },
            new CnTimelineLane
            {
                Label = "V",
                Segments =
                [
                    new CnTimelineSegment { From = new(2026, 1, 1), To = new(2026, 3, 31), Tone = CnTimelineSegmentTone.Billed, Title = "Q1 billed", Key = "sp-1" },
                    new CnTimelineSegment { From = new(2026, 4, 1), To = new(2026, 6, 30), Tone = CnTimelineSegmentTone.Concept, Title = "Q2 concept", Key = "sp-2" },
                    new CnTimelineSegment { From = new(2026, 7, 1), To = new(2026, 9, 30), Tone = CnTimelineSegmentTone.Due, Title = "Q3 due", Key = "sp-3" },
                    new CnTimelineSegment { From = new(2026, 10, 1), To = new(2027, 3, 31), Tone = CnTimelineSegmentTone.Uncovered, Key = "sp-4" },
                    new CnTimelineSegment { From = new(2027, 4, 1), To = new(2027, 6, 30), Tone = CnTimelineSegmentTone.Later, Title = "Off the axis" },
                ],
            },
        ],
    };

    [Fact]
    public void RowWithoutLanes_KeepsTheOriginalFlatMarkup()
    {
        var cut = RenderTimeline(
        [
            new CnTimelineRow { Label = "Design", Note = "phase 1", From = new(2026, 1, 1), To = new(2026, 4, 1), Fill = .5, Value = "120 h", SecondValue = "9,600" },
        ]);

        cut.FindAll(".cn-gantt-row").ShouldBeEmpty();
        var children = cut.Find(".cn-gantt").Children.Skip(4).ToList();
        children.Select(x => x.ClassName!.Trim()).ShouldBe(["cn-gantt-name", "cn-gantt-track", "cn-gantt-n", "cn-gantt-n"]);
        children[0].InnerHtml.ShouldContain("<span>Design</span>");
        cut.FindAll("button").ShouldBeEmpty();

        var bar = cut.Find(".cn-gantt-bar");
        bar.GetAttribute("style").ShouldBe("left: 0%; width: 24.725%");
        bar.QuerySelector("i")!.GetAttribute("style").ShouldBe("width: 50%");
        cut.FindAll(".cn-gantt-seg").ShouldBeEmpty();
    }

    [Fact]
    public void Axis_PutsTheYearOnJanuaryAndMarksAlternateMonths()
    {
        var cut = Render<CnTimeline>(p => p
            .Add(x => x.Rows, [])
            .Add(x => x.From, new DateOnly(2025, 10, 1))
            .Add(x => x.To, new DateOnly(2026, 4, 30)));

        var ticks = cut.FindAll(".cn-gantt-mo");
        ticks.Select(x => x.TextContent).ShouldBe(["oct", "nov", "dec", "2026", "feb", "mar", "apr"]);
        ticks[3].ClassList.ShouldContain("cn-gantt-mo--year");
        // Even months give way when narrow, so January keeps its year label.
        ticks.Select(x => x.ClassList.Contains("cn-gantt-mo--alt")).ShouldBe([true, false, true, false, true, false, true]);
        cut.Find(".cn-gantt").GetAttribute("style").ShouldBe("--cn-gantt-months: 6.932");
    }

    [Fact]
    public void Lanes_RenderSegmentsWithToneCutAndPosition()
    {
        var cut = RenderTimeline([LaneRow()]);

        var row = cut.Find(".cn-gantt-row");
        row.ClassList.ShouldContain("cn-gantt-row--lanes");
        row.ClassList.ShouldNotContain("cn-gantt-row--click");
        cut.FindAll(".cn-gantt-lane-tag").Select(x => x.TextContent).ShouldBe(["A", "V"]);

        var lanes = cut.FindAll(".cn-gantt-lane");
        lanes.Count.ShouldBe(2);
        lanes[0].ClassList.ShouldContain("cn-gantt-lane--thin");

        var purchase = lanes[0].QuerySelector(".cn-gantt-seg")!;
        purchase.ClassList.ShouldContain("cn-gantt-seg--purchase");
        purchase.ClassList.ShouldContain("cn-gantt-seg--thin");
        purchase.ClassList.ShouldContain("cn-gantt-seg--cut-l");
        purchase.ClassList.ShouldNotContain("cn-gantt-seg--cut-r");
        purchase.GetAttribute("style").ShouldBe("left: calc(0% + 0px); width: max(2px, calc(75% - 1px))");

        var sales = lanes[1].QuerySelectorAll(".cn-gantt-seg");
        sales.Length.ShouldBe(4); // the fifth lies wholly past the axis
        sales.Select(x => x.ClassList.Skip(1).First()).ShouldBe(
            ["cn-gantt-seg--billed", "cn-gantt-seg--concept", "cn-gantt-seg--due", "cn-gantt-seg--uncovered"]);
        sales[0].GetAttribute("style").ShouldBe("left: calc(0% + 1px); width: max(2px, calc(24.725% - 2px))");
        sales[1].GetAttribute("style").ShouldBe("left: calc(24.725% + 1px); width: max(2px, calc(25% - 2px))");
        sales[3].ClassList.ShouldContain("cn-gantt-seg--cut-r");
        sales[3].GetAttribute("style").ShouldBe("left: calc(75% + 1px); width: max(2px, calc(25% - 1px))");

        // Without a delegate a segment is not a control, but still has a name.
        sales[0].TagName.ShouldBe("SPAN");
        sales[0].GetAttribute("role").ShouldBe("img");
        sales[0].GetAttribute("aria-label").ShouldBe("Q1 billed");
        sales[3].GetAttribute("title")!.ShouldContain("2026");
        cut.FindAll("button").ShouldBeEmpty();
        cut.FindAll(".cn-gantt-bar").ShouldBeEmpty();
        cut.Find(".cn-gantt-row .cn-gantt-today").ShouldNotBeNull();
    }

    [Fact]
    public void GroupRow_SpansTheLabelAndKeepsItsFigures()
    {
        var cut = RenderTimeline(
        [
            new CnTimelineRow { Label = "Office subscription", Note = "AB-0012", IsGroup = true, SecondValue = "€ 480" },
            LaneRow(),
        ]);

        var group = cut.Find(".cn-gantt-row--group");
        group.Children.Select(x => x.ClassName!.Trim()).ShouldBe(["cn-gantt-name", "cn-gantt-n", "cn-gantt-n"]);
        group.QuerySelector(".cn-gantt-note")!.TextContent.ShouldBe("AB-0012");
        group.QuerySelectorAll(".cn-gantt-n")[1].TextContent.Trim().ShouldBe("€ 480");
        group.QuerySelector(".cn-gantt-track").ShouldBeNull();
    }

    [Fact]
    public void RowClick_IsRaisedFromTheRowAndItsLabelButton()
    {
        var clicked = new List<CnTimelineRow>();
        var group = new CnTimelineRow { Label = "Group", IsGroup = true, Key = "ab-1" };
        var plain = new CnTimelineRow { Label = "Plain", From = new(2026, 2, 1), To = new(2026, 3, 1) };
        var cut = RenderTimeline([group, plain], clicked.Add);

        var rows = cut.FindAll(".cn-gantt-row");
        rows.Count.ShouldBe(2);
        rows.ShouldAllBe(x => x.ClassList.Contains("cn-gantt-row--click"));
        cut.FindAll("button.cn-gantt-label").Select(x => x.TextContent).ShouldBe(["Group", "Plain"]);

        cut.FindAll(".cn-gantt-row")[1].Click();
        cut.FindAll("button.cn-gantt-label")[0].Click();

        clicked.ShouldBe([plain, group]);
    }

    [Fact]
    public void SegmentClick_RaisesRowLaneAndSegmentWithoutTheRowClick()
    {
        var rows = new List<CnTimelineRow>();
        var segments = new List<CnTimelineSegmentClick>();
        var row = LaneRow();
        var cut = RenderTimeline([row], rows.Add, segments.Add);

        var buttons = cut.FindAll("button.cn-gantt-seg");
        buttons.Count.ShouldBe(5);
        buttons[2].GetAttribute("aria-label").ShouldBe("Q2 concept");
        buttons[2].GetAttribute("type").ShouldBe("button");

        buttons[2].Click();

        segments.Count.ShouldBe(1);
        segments[0].Row.ShouldBeSameAs(row);
        segments[0].Lane.ShouldBeSameAs(row.Lanes![1]);
        segments[0].Segment.Key.ShouldBe("sp-2");
        rows.ShouldBeEmpty();
    }

    [Fact]
    public void SelectedKey_HighlightsTheMatchingRowOrSegment()
    {
        var cut = RenderTimeline([LaneRow("svc-1"), LaneRow("svc-2")], segmentClick: _ => { }, selectedKey: "svc-2");

        var rows = cut.FindAll(".cn-gantt-row");
        rows[0].ClassList.ShouldNotContain("cn-gantt-row--selected");
        rows[1].ClassList.ShouldContain("cn-gantt-row--selected");
        cut.FindAll(".cn-gantt-seg--selected").ShouldBeEmpty();

        cut.Render(p => p.Add(x => x.SelectedKey, "sp-3"));

        var selected = cut.FindAll(".cn-gantt-seg--selected");
        selected.Count.ShouldBe(2); // both rows share the demo keys
        selected.ShouldAllBe(x => x.GetAttribute("aria-current") == "true");
        cut.FindAll(".cn-gantt-row--selected").ShouldBeEmpty();
    }
}
