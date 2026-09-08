using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnStatusPickerTests : CnTestContext
{
    private static readonly IReadOnlyList<CnStatusOption<TestStatus>> Options =
    [
        new(TestStatus.Planned, "Planned"),
        new(TestStatus.Active, "Active", CnStatusTone.Accent),
        new(TestStatus.Blocked, "Blocked", CnStatusTone.Warning),
    ];

    [Fact]
    public void InteractivePicker_ExposesCurrentValueAsASelectedMenuItem()
    {
        var cut = Render<CnStatusPicker<TestStatus>>(parameters => parameters
            .Add(component => component.Value, TestStatus.Active)
            .Add(component => component.Options, Options)
            .Add(component => component.CanChange, true)
            .Add(component => component.Prefix, "Status")
            .Add(component => component.AriaLabel, "Project status"));

        var trigger = cut.Find("button.cn-status-picker__trigger");
        trigger.GetAttribute("aria-haspopup").ShouldBe("menu");
        trigger.GetAttribute("aria-expanded").ShouldBe("false");
        trigger.TextContent.ShouldContain("Status");
        trigger.TextContent.ShouldContain("Active");

        trigger.Click();

        cut.Find("[role='menu']").ShouldNotBeNull();
        var current = cut.FindAll("[role='menuitemradio']")
            .Single(item => item.TextContent.Contains("Active", StringComparison.Ordinal));
        current.GetAttribute("aria-checked").ShouldBe("true");
        current.GetAttribute("aria-disabled").ShouldBe("true");
    }

    [Fact]
    public void SelectingCurrentValueIsANoOp_AndAnotherValueIsEmittedWithoutOptimisticMutation()
    {
        var changes = new List<TestStatus>();
        var cut = Render<CnStatusPicker<TestStatus>>(parameters => parameters
            .Add(component => component.Value, TestStatus.Active)
            .Add(component => component.Options, Options)
            .Add(component => component.CanChange, true)
            .Add(component => component.ValueChanged, status => changes.Add(status)));

        cut.Find("button.cn-status-picker__trigger").Click();
        cut.FindAll("[role='menuitemradio']")
            .Single(item => item.TextContent.Contains("Active", StringComparison.Ordinal))
            .Click();
        changes.ShouldBeEmpty();

        cut.FindAll("[role='menuitemradio']")
            .Single(item => item.TextContent.Contains("Blocked", StringComparison.Ordinal))
            .Click();

        changes.ShouldBe([TestStatus.Blocked]);
        cut.Find("button.cn-status-picker__trigger").TextContent.ShouldContain("Active");
    }

    [Fact]
    public void ReadOnlyPicker_PreservesStatusWithoutAnInteractiveAffordance()
    {
        var cut = Render<CnStatusPicker<TestStatus>>(parameters => parameters
            .Add(component => component.Value, TestStatus.Active)
            .Add(component => component.Options, Options)
            .Add(component => component.CanChange, false)
            .Add(component => component.AriaLabel, "Project status"));

        cut.FindAll("button").ShouldBeEmpty();
        cut.Find(".cn-status-picker__readonly").TextContent.ShouldContain("Active");
        cut.Find(".cn-status-picker__readonly").GetAttribute("aria-label").ShouldBe("Project status");
    }

    private enum TestStatus
    {
        Planned,
        Active,
        Blocked,
    }
}
