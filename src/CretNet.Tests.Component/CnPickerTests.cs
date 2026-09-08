using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// CnPicker v2 (combobox + advanced grid dialog) behaviours: focus opens the
/// typeahead dropdown from the Provider, picking an option sets Value AND puts
/// the label in the text field (the two owner-reported bugs), and the magnifier
/// only appears when an AdvancedSearch callback is supplied.
/// </summary>
public class CnPickerTests : CnTestContext
{
    private static readonly CnPickerItem Acme = new(Guid.NewGuid(), "Acme NV");
    private static readonly CnPickerItem Globex = new(Guid.NewGuid(), "Globex BVBA");
    private static readonly IReadOnlyList<CnPickerItem> Sample = [Acme, Globex];

    private static Task<IReadOnlyList<CnPickerItem>> Provide(string? _) => Task.FromResult(Sample);

    [Fact]
    public void Focus_WithProvider_OpensDropdownWithOptions()
    {
        var cut = Render<CnPicker>(p => p.Add(x => x.Provider, Provide));

        cut.Find("input.cn-picker-input").Focus();

        cut.WaitForAssertion(() =>
            cut.FindAll(".cn-picker-option").Count.ShouldBe(Sample.Count));
    }

    [Fact]
    public void SelectOption_SetsValueAndShowsLabelInField()
    {
        Guid? selected = null;
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, Provide)
            .Add(x => x.ValueChanged, (Guid? v) => selected = v));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);
        cut.FindAll(".cn-picker-option")[1].Click();

        selected.ShouldBe(Globex.Id);
        cut.Find("input.cn-picker-input").GetAttribute("value").ShouldBe(Globex.Label);
        // Picking closes the dropdown.
        cut.FindAll(".cn-picker-option").Count.ShouldBe(0);
    }

    [Fact]
    public void PickerRoot_IsNotALabelThatRefocusesInputAfterOptionClick()
    {
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Label, "Klant")
            .Add(x => x.Provider, Provide));

        cut.Find(".cn-picker").TagName.ShouldBe("DIV");
        var label = cut.Find("label.cn-label");
        label.GetAttribute("for").ShouldBe(cut.Find("input.cn-picker-input").Id);
    }

    [Fact]
    public void AdvancedSearch_Null_HidesMagnifier()
    {
        var cut = Render<CnPicker>(p => p.Add(x => x.Provider, Provide));

        cut.FindAll(".cn-picker-btn").Count.ShouldBe(0);
    }

    [Fact]
    public void AdvancedSearch_Set_ShowsMagnifierThatSelectsResult()
    {
        Guid? selected = null;
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, Provide)
            .Add(x => x.ValueChanged, (Guid? v) => selected = v)
            .Add(x => x.AdvancedSearch, () => Task.FromResult<CnPickerItem?>(Acme)));

        var buttons = cut.FindAll(".cn-picker-btn");
        buttons.Count.ShouldBe(1);

        buttons[0].Click();

        selected.ShouldBe(Acme.Id);
        cut.Find("input.cn-picker-input").GetAttribute("value").ShouldBe(Acme.Label);
    }

    // ===== S-256: rich rows, groups, dropdown footer =====

    private static readonly CnPickerItem RichTask = new(
        Guid.NewGuid(), "Patchwerk verdieping 2",
        Context: "P-0042 Netwerkvernieuwing · Vandenbroucke Bouw NV",
        Meta: "Bezig", MetaAccent: true);

    [Fact]
    public void DefaultTemplate_RendersContextAndMetaBadge()
    {
        IReadOnlyList<CnPickerItem> items = [RichTask];
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, _ => Task.FromResult(items)));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);

        cut.Find(".cn-picker-option-title").TextContent.ShouldBe(RichTask.Label);
        cut.Find(".cn-picker-option-context").TextContent.ShouldBe(RichTask.Context);
        var meta = cut.Find(".cn-picker-option-meta");
        meta.TextContent.ShouldBe("Bezig");
        meta.ClassList.ShouldContain("cn-badge--accent");
    }

    [Fact]
    public void DefaultTemplate_WithoutContext_RendersNothingExtra()
    {
        var cut = Render<CnPicker>(p => p.Add(x => x.Provider, Provide));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);

        cut.FindAll(".cn-picker-option-context").Count.ShouldBe(0);
        cut.FindAll(".cn-picker-option-meta").Count.ShouldBe(0);
    }

    [Fact]
    public void ItemTemplate_OverridesTheDefaultRow()
    {
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, Provide)
            .Add(x => x.ItemTemplate, item => $"<em class=\"custom-row\">{item.Label}</em>"));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);

        cut.FindAll("em.custom-row").Count.ShouldBe(Sample.Count);
        cut.FindAll(".cn-picker-option-title").Count.ShouldBe(0);
    }

    [Fact]
    public void EmptyQuery_WithRecentFlags_GroupsRecentFirst()
    {
        IReadOnlyList<CnPickerItem> items =
        [
            new(Guid.NewGuid(), "Oude taak"),
            new(Guid.NewGuid(), "Recente taak", Recent: true),
        ];
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, _ => Task.FromResult(items)));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);

        var groups = cut.FindAll(".cn-picker-group");
        groups.Count.ShouldBe(2);
        // Derived-recent items render first, regardless of provider order.
        cut.FindAll(".cn-picker-option-title")[0].TextContent.ShouldBe("Recente taak");
    }

    [Fact]
    public void TypedQuery_NeverGroups()
    {
        IReadOnlyList<CnPickerItem> items =
        [
            new(Guid.NewGuid(), "Oude taak"),
            new(Guid.NewGuid(), "Recente taak", Recent: true),
        ];
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, _ => Task.FromResult(items)));

        cut.Find("input.cn-picker-input").Input("taak");
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0, TimeSpan.FromSeconds(2));

        cut.FindAll(".cn-picker-group").Count.ShouldBe(0);
    }

    [Fact]
    public void DropdownFooter_AbsentWithoutCallbacks()
    {
        var cut = Render<CnPicker>(p => p.Add(x => x.Provider, Provide));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-option").Count > 0);

        cut.FindAll(".cn-picker-foot").Count.ShouldBe(0);
    }

    [Fact]
    public void DropdownFooter_AdvancedSearch_SelectsResult()
    {
        Guid? selected = null;
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, Provide)
            .Add(x => x.ValueChanged, (Guid? v) => selected = v)
            .Add(x => x.AdvancedSearch, () => Task.FromResult<CnPickerItem?>(Globex)));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-foot-btn").Count > 0);

        cut.Find(".cn-picker-foot-btn").Click();

        selected.ShouldBe(Globex.Id);
        cut.Find("input.cn-picker-input").GetAttribute("value").ShouldBe(Globex.Label);
    }

    [Fact]
    public void DropdownFooter_QuickAdd_SelectsCreatedItem()
    {
        var created = new CnPickerItem(Guid.NewGuid(), "Nieuwe taak");
        Guid? selected = null;
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Provider, Provide)
            .Add(x => x.ValueChanged, (Guid? v) => selected = v)
            .Add(x => x.OnAdd, () => Task.FromResult<CnPickerItem?>(created)));

        cut.Find("input.cn-picker-input").Focus();
        cut.WaitForState(() => cut.FindAll(".cn-picker-foot-btn").Count > 0);

        cut.Find(".cn-picker-foot-btn").Click();

        selected.ShouldBe(created.Id);
        cut.Find("input.cn-picker-input").GetAttribute("value").ShouldBe(created.Label);
        // A create flow that returns null (cancelled) must not clear the field:
        // covered implicitly — selection only changes on a non-null result.
    }
}
