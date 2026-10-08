using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// <see cref="CnGridPage.From{TItem}"/>: an embedded list the host already holds
/// gets the grid's own search and paging in memory.
/// </summary>
public class CnGridPageTests : CnTestContext
{
    private static readonly string[] Rows = ["Monitor", "Mouse", "Keyboard", "Modem"];

    [Fact]
    public void From_NoSearch_PagesTheItemsAndCountsThemAll()
    {
        var page = CnGridPage.From(Rows, new CnGridRequest(null, 2, 3, null, false));

        page.Items.ShouldBe(["Modem"]);
        page.TotalCount.ShouldBe(4);
    }

    [Fact]
    public void From_Search_FiltersBeforePagingAndCountsTheHits()
    {
        var page = CnGridPage.From(Rows, new CnGridRequest("  mo ", 1, 2, null, false), (row, search) => CnGridPage.Matches(search, row));

        page.Items.ShouldBe(["Monitor", "Mouse"]);
        page.TotalCount.ShouldBe(3);
    }

    [Fact]
    public void From_SearchWithoutMatcher_IsIgnored()
    {
        CnGridPage.From(Rows, new CnGridRequest("zzz", 1, 10, null, false)).TotalCount.ShouldBe(4);
    }

    [Fact]
    public void Matches_AnyTextIgnoringCase_NullsSkipped()
    {
        CnGridPage.Matches("KEY", null, "Keyboard").ShouldBeTrue();
        CnGridPage.Matches("pen", null, "Keyboard").ShouldBeFalse();
    }

    [Fact]
    public void Grid_WithAnInMemoryProvider_SearchesFromItsOwnToolbar()
    {
        var cut = Render<GridLocalFixture>();
        cut.WaitForAssertion(() => cut.FindAll("tbody tr").Count.ShouldBe(2));

        cut.Find(".cn-grid-search input").Input("key");

        cut.WaitForAssertion(() =>
        {
            cut.FindAll("tbody tr").Count.ShouldBe(1);
            cut.Find("tbody td").TextContent.ShouldBe("Keyboard");
        }, TimeSpan.FromSeconds(3));
    }
}
