using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Microsoft.AspNetCore.Components.Forms;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// A quantity and its unit in one field: the number reads like CnNumberField
/// (empty is not zero, a Belgian comma), the unit is a host type chosen from
/// a list, and a unit outside that list is never silently dropped.
/// </summary>
public sealed class CnQuantityFieldTests : CnTestContext
{
    private readonly record struct TimeUnit(string Code);

    private static readonly TimeUnit Minute = new("min");
    private static readonly TimeUnit Hour = new("h");
    private static readonly TimeUnit Day = new("d");
    private static readonly TimeUnit[] Units = [Minute, Hour];

    private static string Dutch(TimeUnit unit, decimal? quantity) => (unit.Code, quantity == 1m) switch
    {
        ("min", true) => "minuut",
        ("min", false) => "minuten",
        ("h", _) => "uur",
        ("d", true) => "dag",
        _ => "dagen",
    };

    private IRenderedComponent<CnQuantityField<TimeUnit>> RenderField(
        Action<ComponentParameterCollectionBuilder<CnQuantityField<TimeUnit>>>? configure = null) =>
        Render<CnQuantityField<TimeUnit>>(p =>
        {
            p.Add(x => x.Units, Units).Add(x => x.UnitText, Dutch);
            configure?.Invoke(p);
        });

    [Fact]
    public void Renders_LabelNumberAndUnitSelect_InOneFrame()
    {
        var cut = RenderField(p => p
            .Add(x => x.Label, "Duur")
            .Add(x => x.Value, 15m)
            .Add(x => x.Unit, Minute)
            .Add(x => x.Id, "duration")
            .Add(x => x.UnitAriaLabel, "Eenheid van de duur"));

        cut.Find(".cn-label").TextContent.ShouldBe("Duur");
        var frame = cut.Find(".cn-quantity-frame");
        frame.ClassList.ShouldContain("cn-input");

        var input = frame.QuerySelector("input")!;
        input.GetAttribute("type").ShouldBe("text");
        input.GetAttribute("inputmode").ShouldBe("decimal");
        input.GetAttribute("id").ShouldBe("duration");
        input.GetAttribute("aria-label").ShouldBe("Duur");
        input.GetAttribute("value").ShouldBe("15");

        var select = frame.QuerySelector("select")!;
        select.GetAttribute("aria-label").ShouldBe("Eenheid van de duur");
        select.QuerySelectorAll("option").Select(o => o.TextContent).ShouldBe(["minuten", "uur"]);
        select.QuerySelector("option[selected]")!.TextContent.ShouldBe("minuten");

        // Tab order follows the markup: the number, then the unit.
        frame.Children.Select(e => e.TagName).ShouldBe(["INPUT", "SELECT"]);
    }

    [Fact]
    public void Typing_ABelgianDecimal_SetsTheQuantity()
    {
        decimal? value = null;
        var cut = RenderField(p => p.Add(x => x.ValueChanged, next => value = next));

        cut.Find("input").Change("0,25");

        value.ShouldBe(0.25m);
    }

    [Fact]
    public void Clearing_GivesNull_NotZero_AndZeroStaysZero()
    {
        decimal? value = 5m;
        var cut = RenderField(p => p
            .Add(x => x.Value, 5m)
            .Add(x => x.ValueChanged, next => value = next));

        cut.Find("input").Change("");
        value.ShouldBeNull();
        cut.Find("input").GetAttribute("value").ShouldBe("");

        cut.Find("input").Change("0");
        value.ShouldBe(0m);
        cut.Find("input").GetAttribute("value").ShouldBe("0");
    }

    [Fact]
    public void Display_FollowsTheNumberField_UngroupedInDecimalMode()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("nl-BE");
            var cut = RenderField(p => p.Add(x => x.Value, 1234.56789m));

            cut.Find("input").GetAttribute("value").ShouldBe("1234,5679");

            cut.Render(p => p.Add(x => x.Value, 1234.5m).Add(x => x.MaxDecimals, 1));
            cut.Find("input").GetAttribute("value").ShouldBe("1234,5");
        }
        finally { CultureInfo.CurrentCulture = original; }
    }

    [Fact]
    public void Min_ClampsTheCommittedQuantity()
    {
        decimal? value = null;
        var cut = RenderField(p => p
            .Add(x => x.Min, 0m)
            .Add(x => x.ValueChanged, next => value = next));

        cut.Find("input").Change("-3");

        value.ShouldBe(0m);
    }

    [Fact]
    public void ChoosingAUnit_RaisesUnitChanged()
    {
        TimeUnit? chosen = null;
        var cut = RenderField(p => p
            .Add(x => x.Unit, Minute)
            .Add(x => x.UnitChanged, next => chosen = next));

        cut.Find("select").Change("1");

        chosen.ShouldBe(Hour);
        cut.Find("option[selected]").TextContent.ShouldBe("uur");
    }

    [Fact]
    public void UnitText_ReceivesTheQuantity_ForSingularOrPlural()
    {
        var one = RenderField(p => p.Add(x => x.Value, 1m).Add(x => x.Unit, Minute));
        var many = RenderField(p => p.Add(x => x.Value, 15m).Add(x => x.Unit, Minute));

        one.Find("option[selected]").TextContent.ShouldBe("minuut");
        many.Find("option[selected]").TextContent.ShouldBe("minuten");

        // Typing a new quantity re-words the unit.
        one.Find("input").Change("2");
        one.Find("option[selected]").TextContent.ShouldBe("minuten");
    }

    [Fact]
    public void Compact_AddsTheCompactClass_AndNormalModeStaysUnchanged()
    {
        RenderField(p => p.Add(x => x.Label, "Duur").Add(x => x.Compact, true))
            .Find(".cn-field").ClassList.ShouldContain("cn-field--compact");
        RenderField().Find(".cn-field").ClassList.ShouldNotContain("cn-field--compact");
    }

    [Fact]
    public void Subtle_GoesOnTheFrame()
    {
        RenderField(p => p.Add(x => x.Subtle, true))
            .Find(".cn-quantity-frame").ClassList.ShouldContain("cn-input--subtle");
    }

    [Fact]
    public void Disabled_LocksBoth_UnitDisabled_LocksOnlyTheUnit()
    {
        var disabled = RenderField(p => p.Add(x => x.Disabled, true));
        disabled.Find("input").HasAttribute("disabled").ShouldBeTrue();
        disabled.Find("select").HasAttribute("disabled").ShouldBeTrue();
        disabled.Find(".cn-quantity-frame").ClassList.ShouldContain("cn-quantity-frame--disabled");

        var unitLocked = RenderField(p => p.Add(x => x.UnitDisabled, true));
        unitLocked.Find("input").HasAttribute("disabled").ShouldBeFalse();
        unitLocked.Find("select").HasAttribute("disabled").ShouldBeTrue();
        unitLocked.Find(".cn-quantity-frame").ClassList.ShouldNotContain("cn-quantity-frame--disabled");
    }

    [Fact]
    public void AUnitOutsideTheList_IsStillShownAndSelected()
    {
        TimeUnit? chosen = null;
        var cut = RenderField(p => p
            .Add(x => x.Value, 3m)
            .Add(x => x.Unit, Day)
            .Add(x => x.UnitChanged, next => chosen = next));

        cut.FindAll("option").Select(o => o.TextContent).ShouldBe(["minuten", "uur", "dagen"]);
        cut.Find("option[selected]").TextContent.ShouldBe("dagen");

        // Choosing it again hands back the same unit, not a default.
        cut.Find("select").Change("2");
        chosen.ShouldBe(Day);
    }

    [Fact]
    public void NoUnit_ShowsAnEmptyChoice_AndAddsNoDefaultUnit()
    {
        var cut = RenderField(p => p.Add(x => x.Value, 3m));

        var options = cut.FindAll("option");
        options.Count.ShouldBe(3);
        options[0].GetAttribute("value").ShouldBe("-1");
        options[0].HasAttribute("selected").ShouldBeTrue();
    }

    [Fact]
    public void NullableStructUnits_Work()
    {
        TimeUnit? chosen = Minute;
        var cut = Render<CnQuantityField<TimeUnit?>>(p => p
            .Add(x => x.Units, [Minute, Hour])
            .Add(x => x.UnitText, (unit, _) => unit?.Code ?? "")
            .Add(x => x.UnitChanged, next => chosen = next));

        cut.Find("option[selected]").GetAttribute("value").ShouldBe("-1");
        cut.Find("select").Change("-1");
        chosen.ShouldBeNull();
    }

    [Fact]
    public void For_ShowsTheQuantityValidationMessage()
    {
        var model = new Line();
        var context = new EditContext(model);
        var messages = new ValidationMessageStore(context);
        messages.Add(context.Field(nameof(Line.Quantity)), "Verplicht");

        var cut = RenderField(p => p
            .AddCascadingValue(context)
            .Add(x => x.Label, "Aantal")
            .Add(x => x.For, () => model.Quantity));

        cut.Find(".cn-field-error").TextContent.ShouldBe("Verplicht");
        cut.Find(".cn-quantity-frame").ClassList.ShouldContain("cn-input--invalid");
    }

    [Fact]
    public void UnitAriaLabel_ComesFromTheLibraryResources()
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            RenderField().Find("select").GetAttribute("aria-label").ShouldBe("Eenheid");
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("en");
            RenderField().Find("select").GetAttribute("aria-label").ShouldBe("Unit");
        }
        finally { CultureInfo.CurrentUICulture = original; }
    }

    [Fact]
    public void Stylesheet_FramesBothParts_AndLightsUpOnFocus()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));

        css.ShouldContain(".cn-input.cn-quantity-frame {");
        css.ShouldContain(".cn-quantity-frame:focus-within {");
        css.ShouldContain(".cn-field--compact .cn-quantity-unit {");
        // Beside its label: a 100% basis would wrap the frame under the label.
        css.ShouldContain(".cn-field--compact > .cn-quantity-frame { width: auto; }");
    }

    private sealed class Line
    {
        public decimal? Quantity { get; set; }
    }
}
