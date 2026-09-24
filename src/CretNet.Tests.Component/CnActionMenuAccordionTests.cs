using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Dialogs;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnActionMenuAccordionTests : CnTestContext
{
    [Fact]
    public void ActionMenu_TriggerOpensMenuAndOutsideClickCloses()
    {
        var cut = Render<ActionMenuFixture>();
        var trigger = cut.Find(".cn-action-menu > button.cn-btn");
        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        cut.FindAll("[role=menu]").ShouldBeEmpty();

        trigger.Click();

        cut.Find(".cn-action-menu > button.cn-btn").GetAttribute("aria-expanded").ShouldBe("true");
        var menu = cut.Find("[role=menu]");
        cut.Find(".cn-action-menu > button.cn-btn").GetAttribute("aria-controls").ShouldBe(menu.Id);
        cut.FindAll("[role=menuitem]").Count.ShouldBe(3);
        cut.FindAll("[role=separator].cn-action-menu__separator").Count.ShouldBe(1);

        cut.Find(".cn-action-menu__backdrop").Click();

        cut.FindAll("[role=menu]").ShouldBeEmpty();
        cut.Instance.Menu.IsOpen.ShouldBeFalse();
    }

    [Fact]
    public void ActionMenu_EscapeCloses_AndChoosingAnItemClosesAndInvokesIt()
    {
        var cut = Render<ActionMenuFixture>();
        cut.Find(".cn-action-menu > button.cn-btn").Click();
        cut.Find(".cn-action-menu").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        cut.FindAll("[role=menu]").ShouldBeEmpty();

        cut.Find(".cn-action-menu > button.cn-btn").Click();
        cut.FindAll("[role=menuitem]")[0].Click();

        cut.Instance.Clicked.ShouldBe([nameof(CnIconKind.Edit)]);
        cut.FindAll("[role=menu]").ShouldBeEmpty();
    }

    [Fact]
    public void ActionMenu_DisabledItemShowsReasonAndIgnoresClicks()
    {
        var cut = Render<ActionMenuFixture>();
        cut.Find(".cn-action-menu > button.cn-btn").Click();
        var archive = cut.FindAll("[role=menuitem]")[1];

        archive.GetAttribute("aria-disabled").ShouldBe("true");
        archive.ClassList.ShouldContain("is-disabled");
        archive.QuerySelector(".cn-action-menu__reason")!.TextContent.ShouldBe("Has open orders");
        cut.FindAll("[role=menuitem]")[0].QuerySelector(".cn-action-menu__reason").ShouldBeNull();

        archive.Click();

        cut.Instance.Clicked.ShouldBeEmpty();
        cut.FindAll("[role=menu]").Count.ShouldBe(1);
    }

    [Fact]
    public void ActionMenu_DangerItemGetsDangerClassOnly()
    {
        var cut = Render<ActionMenuFixture>();
        cut.Find(".cn-action-menu > button.cn-btn").Click();
        var items = cut.FindAll("[role=menuitem]");

        items[2].ClassList.ShouldContain("cn-action-menu__item--danger");
        items[0].ClassList.ShouldNotContain("cn-action-menu__item--danger");
        items[2].GetAttribute("aria-disabled").ShouldBe("false");
    }

    [Fact]
    public void ActionMenu_ArrowDownOnClosedTriggerOpensTheMenu()
    {
        var cut = Render<ActionMenuFixture>();
        cut.Find(".cn-action-menu").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("[role=menuitem]").Count.ShouldBe(3);
    }

    [Fact]
    public void ActionMenu_DefaultLabelAndIconComeFromTheLibrary()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            var cut = Render<CnActionMenu>();
            cut.Find("button.cn-btn span").TextContent.ShouldBe("Meer");
            cut.Find("button.cn-btn svg.cn-icon circle").ShouldNotBeNull();
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            cut.Render();
            cut.Find("button.cn-btn span").TextContent.ShouldBe("More");
            cut.Render(p => p.Add(x => x.IconOnly, true));
            cut.Find("button.cn-btn").ClassList.ShouldContain("cn-btn--icononly");
            cut.Find("button.cn-btn").GetAttribute("title").ShouldBe("More");
        }
        finally { CultureInfo.CurrentUICulture = original; }
    }

    [Fact]
    public void Accordion_ToggleUpdatesAriaExpandedContentAndBinding()
    {
        var cut = Render<AccordionFixture>();
        var header = cut.FindAll(".cn-accordion__header")[0];
        header.GetAttribute("aria-expanded").ShouldBe("false");
        cut.FindAll(".products-body").ShouldBeEmpty();
        var panelId = header.GetAttribute("aria-controls")!;
        cut.Find($"#{panelId}").HasAttribute("hidden").ShouldBeTrue();

        header.Click();

        cut.FindAll(".cn-accordion__header")[0].GetAttribute("aria-expanded").ShouldBe("true");
        cut.Find($"#{panelId}").HasAttribute("hidden").ShouldBeFalse();
        cut.Find(".products-body").TextContent.ShouldBe("product rows");
        cut.Instance.ProductsOpen.ShouldBeTrue();
        cut.FindAll(".cn-accordion__item")[0].ClassList.ShouldContain("is-open");

        cut.FindAll(".cn-accordion__header")[0].Click();
        cut.Instance.ProductsOpen.ShouldBeFalse();
        cut.FindAll(".products-body").ShouldBeEmpty();
    }

    [Fact]
    public void Accordion_ShowsCountAndDimmedState()
    {
        var cut = Render<AccordionFixture>();
        var items = cut.FindAll(".cn-accordion__item");
        items[0].QuerySelector(".cn-accordion__count")!.TextContent.ShouldBe("3");
        items[1].QuerySelector(".cn-accordion__count").ShouldBeNull();
        items[1].ClassList.ShouldContain("cn-accordion__item--dimmed");
        items[0].ClassList.ShouldNotContain("cn-accordion__item--dimmed");
    }

    [Fact]
    public async Task Accordion_ExpandAllAndCollapseAll_ChangeEveryItem()
    {
        var cut = Render<AccordionFixture>();

        await cut.InvokeAsync(() => cut.Instance.Accordion.ExpandAllAsync());

        cut.FindAll(".cn-accordion__header").Select(x => x.GetAttribute("aria-expanded")).ShouldAllBe(x => x == "true");
        cut.Find(".services-body").ShouldNotBeNull();
        cut.Instance.ProductsOpen.ShouldBeTrue();

        await cut.InvokeAsync(() => cut.Instance.Accordion.CollapseAllAsync());

        cut.FindAll(".cn-accordion__header").Select(x => x.GetAttribute("aria-expanded")).ShouldAllBe(x => x == "false");
        cut.FindAll(".products-body, .services-body").ShouldBeEmpty();
        cut.Instance.ProductsOpen.ShouldBeFalse();
    }

    [Fact]
    public async Task Confirm_Destructive_RendersSolidDangerYesButton()
    {
        var host = Render<CnDialogHost>();
        var dialogs = Services.GetRequiredService<CnDialogService>();

        Task<bool> result = null!;
        await host.InvokeAsync(() => { result = dialogs.ConfirmAsync("Delete", "Delete this item?", destructive: true); });
        var yes = host.WaitForElement(".cn-dialog-footer .cn-btn--danger");

        yes.ClassList.ShouldContain("cn-btn--solid");
        host.FindAll(".cn-dialog-footer .cn-btn--accent").ShouldBeEmpty();
        yes.Click();
        (await result).ShouldBeTrue();
    }

    [Fact]
    public async Task Confirm_DefaultAndPositionalLabels_KeepAccentYesButton()
    {
        var host = Render<CnDialogHost>();
        var dialogs = Services.GetRequiredService<CnDialogService>();

        Task<bool> result = null!;
        await host.InvokeAsync(() => { result = dialogs.ConfirmAsync("Save", "Save changes?", "Ja", "Nee"); });
        var yes = host.WaitForElement(".cn-dialog-footer .cn-btn--accent");

        yes.TextContent.Trim().ShouldBe("Ja");
        host.FindAll(".cn-dialog-footer .cn-btn--danger").ShouldBeEmpty();
        host.FindAll(".cn-dialog-footer .cn-btn")[0].Click();
        (await result).ShouldBeFalse();
    }
}
