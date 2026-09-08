using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// S-265: the from-scratch place indicator (CretNet-bound). Pins the collapse
/// contract: everything beyond section + parent folds behind the "…" menu and
/// the last item always renders as the ink "here" span.
/// </summary>
public class CnBreadcrumbTests : CnTestContext
{
    [Fact]
    public void Render_WithoutItems_RendersNothing()
    {
        var cut = Render<CnBreadcrumb>();

        cut.Markup.Trim().ShouldBeEmpty();
    }

    [Fact]
    public void Render_SingleItem_IsTheCurrentPlaceOnly()
    {
        var cut = Render<CnBreadcrumb>(parameters => parameters
            .Add(p => p.Items, [new CnBreadcrumbItem("Projecten")]));

        cut.Find(".cnb-here").TextContent.ShouldBe("Projecten");
        cut.FindAll("a").ShouldBeEmpty();
        cut.FindAll(".cnb-sep").ShouldBeEmpty();
    }

    [Fact]
    public void Render_TwoLevels_LinksTheAncestorAndInksTheCurrent()
    {
        var cut = Render<CnBreadcrumb>(parameters => parameters
            .Add(p => p.Items,
            [
                new CnBreadcrumbItem("Projecten", "/operations/projects"),
                new CnBreadcrumbItem("C0043 · testeray", "/operations/projects/abc"),
            ]));

        var link = cut.Find("a");
        link.TextContent.ShouldBe("Projecten");
        link.GetAttribute("href").ShouldBe("/operations/projects");
        cut.Find(".cnb-here").TextContent.ShouldBe("C0043 · testeray");
        cut.Find(".cnb-here").GetAttribute("title").ShouldBe("C0043 · testeray");
        cut.FindAll(".cnb-more").ShouldBeEmpty();
    }

    [Fact]
    public void Render_DeepTrail_CollapsesTheMiddleBehindTheEllipsisMenu()
    {
        var cut = Render<CnBreadcrumb>(parameters => parameters
            .Add(p => p.Items,
            [
                new CnBreadcrumbItem("Verkoop", "/sales"),
                new CnBreadcrumbItem("Klanten", "/customers"),
                new CnBreadcrumbItem("Kallogy bvba", "/customers/1"),
                new CnBreadcrumbItem("Facturen", "/customers/1/invoices"),
                new CnBreadcrumbItem("SI0222"),
            ]));

        // Visible: first ancestor, …, last ancestor, current.
        var links = cut.FindAll("nav > a");
        links.Count.ShouldBe(2);
        links[0].TextContent.ShouldBe("Verkoop");
        links[1].TextContent.ShouldBe("Facturen");
        cut.Find(".cnb-here").TextContent.ShouldBe("SI0222");

        // The middle levels sit behind the "…" menu.
        cut.Find(".cnb-more").Click();
        var menuLinks = cut.FindAll(".cnb-menu a");
        menuLinks.Count.ShouldBe(2);
        menuLinks[0].TextContent.ShouldBe("Klanten");
        menuLinks[1].TextContent.ShouldBe("Kallogy bvba");

        // Backdrop click closes it again.
        cut.Find(".cnb-backdrop").Click();
        cut.FindAll(".cnb-menu").ShouldBeEmpty();
    }
}
