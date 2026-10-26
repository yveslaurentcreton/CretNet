using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>The calendar under a date field opens on the month of the value it
/// holds, every time it opens, and on today's month only when it is empty.</summary>
public class CnDateCalendarOpeningTests : BunitContext
{
    private static readonly DateTime March15 = new(2024, 3, 15);

    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

    public CnDateCalendarOpeningTests()
    {
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
        Services.AddCretNetBlazorUi();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js").Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/CretNet.Platform.Blazor.Ui/Components/CnTimeInput.razor.js").Mode = JSRuntimeMode.Loose;
    }

    protected override void Dispose(bool disposing)
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
        base.Dispose(disposing);
    }

    private static string MonthTitle(DateTime date) =>
        $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(date.Month)} {date.Year}";

    private static IReadOnlyList<string> Titles(IRenderedComponent<CnDateField> cut) =>
        cut.FindAll(".cn-cal-title").Select(t => t.TextContent.Trim()).ToList();

    [Fact]
    public void DateField_WithValue_OpensOnTheValuesMonth()
    {
        var cut = Render<CnDateField>(p => p.Add(x => x.Value, March15));

        cut.Find(".cn-date-shell").Click();

        Titles(cut).ShouldBe([MonthTitle(March15)]);
    }

    [Fact]
    public void DateField_ReopenedAfterNavigating_ReturnsToTheValuesMonth()
    {
        var cut = Render<CnDateField>(p => p.Add(x => x.Value, March15));
        cut.Find(".cn-date-shell").Click();
        cut.FindAll(".cn-cal-nav")[1].Click();
        cut.FindAll(".cn-cal-nav")[1].Click();
        Titles(cut).ShouldBe([MonthTitle(March15.AddMonths(2))]);

        cut.Find("input").KeyDown("Escape");
        cut.FindAll(".cn-cal").ShouldBeEmpty();
        cut.Find(".cn-date-shell").Click();

        Titles(cut).ShouldBe([MonthTitle(March15)]);
    }

    [Fact]
    public void DateField_ReopenedAfterZoomingOut_ReturnsToTheValuesDays()
    {
        var cut = Render<CnDateField>(p => p.Add(x => x.Value, March15));
        cut.Find(".cn-date-shell").Click();
        cut.Find(".cn-cal-title").Click();
        cut.FindAll(".cn-cal-day").ShouldBeEmpty();

        cut.Find("input").KeyDown("Escape");
        cut.Find(".cn-date-shell").Click();

        Titles(cut).ShouldBe([MonthTitle(March15)]);
    }

    [Fact]
    public void DateField_Empty_OpensOnTodaysMonth()
    {
        var cut = Render<CnDateField>();

        cut.Find(".cn-date-shell").Click();

        Titles(cut).ShouldBe([MonthTitle(DateTime.Today)]);
    }

    [Fact]
    public void DateField_ValueSetAfterFirstRender_IsWhereTheCalendarOpens()
    {
        var cut = Render<CnDateField>();
        cut.Find(".cn-date-shell").Click();
        cut.Find("input").KeyDown("Escape");

        cut.Render(p => p.Add(x => x.Value, March15));
        cut.Find(".cn-date-shell").Click();

        Titles(cut).ShouldBe([MonthTitle(March15)]);
    }

    [Fact]
    public void DateRangeField_OpensOnTheStartsMonth()
    {
        var cut = Render<CnDateRangeField>(p => p
            .Add(x => x.From, March15)
            .Add(x => x.To, new DateTime(2024, 6, 2)));

        cut.Find(".cn-date-shell").Click();

        cut.FindAll(".cn-cal-title").Select(t => t.TextContent.Trim())
            .ShouldBe([MonthTitle(March15), MonthTitle(March15.AddMonths(1))]);
    }

    [Fact]
    public void DateRangeField_WithOnlyAnEnd_OpensOnTheEndsMonth()
    {
        var end = new DateTime(2024, 7, 4);
        var cut = Render<CnDateRangeField>(p => p.Add(x => x.To, end));

        cut.Find(".cn-date-shell").Click();

        cut.FindAll(".cn-cal-title")[0].TextContent.Trim().ShouldBe(MonthTitle(end));
    }

    [Fact]
    public void DateRangeField_OpenedFromTheEnd_ShowsTheEndsMonth()
    {
        var end = new DateTime(2024, 6, 2);
        var cut = Render<CnDateRangeField>(p => p
            .Add(x => x.From, March15)
            .Add(x => x.To, end));

        cut.FindAll("input")[1].Focus();

        cut.FindAll(".cn-cal-title").Select(t => t.TextContent.Trim())
            .ShouldBe([MonthTitle(end.AddMonths(-1)), MonthTitle(end)]);
    }

    [Fact]
    public void DateRangeField_ReopenedAfterNavigating_ReturnsToTheStartsMonth()
    {
        var cut = Render<CnDateRangeField>(p => p
            .Add(x => x.From, March15)
            .Add(x => x.To, new DateTime(2024, 4, 2)));
        cut.Find(".cn-date-shell").Click();
        cut.FindAll(".cn-cal-nav")[0].Click();

        cut.FindAll("input")[0].KeyDown("Escape");
        cut.Find(".cn-date-shell").Click();

        cut.FindAll(".cn-cal-title")[0].TextContent.Trim().ShouldBe(MonthTitle(March15));
    }

    [Fact]
    public void DateTimeField_WithValue_OpensOnTheValuesMonth()
    {
        var cut = Render<CnDateTimeField>(p => p.Add(x => x.Value, March15.AddHours(10.5)));

        cut.Find(".cn-date-shell").Click();

        cut.FindAll(".cn-cal-title").Select(t => t.TextContent.Trim()).ShouldBe([MonthTitle(March15)]);
    }
}
