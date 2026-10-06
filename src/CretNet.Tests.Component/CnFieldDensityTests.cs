using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// The date field's frame acts as the field (a click beside the text puts the
/// caret at its end), the compact variant for dialog header rows and line
/// cells, and a unit that keeps room for its own width.
/// </summary>
public sealed class CnFieldDensityTests : CnTestContext
{
    private const string DateModule = "./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js";

    private readonly BunitJSModuleInterop _dateModule;

    public CnFieldDensityTests()
    {
        _dateModule = JSInterop.SetupModule(DateModule);
        _dateModule.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void Date_ClickBesideTheText_OpensAndPutsTheCaretAtTheEnd()
    {
        var cut = Render<CnDateField>(p => p.Add(x => x.Value, new DateTime(2026, 10, 6)));

        cut.Find(".cn-date-shell").Click();

        _dateModule.VerifyInvoke("focusAtEnd");
        cut.FindAll(".cn-date-pop").Count.ShouldBe(1);
    }

    [Fact]
    public void Date_TextTakesTheRoomUpToTheClearButton()
    {
        var cut = Render<CnDateField>(p => p.Add(x => x.Value, new DateTime(2026, 10, 6)));

        cut.Find("input").ClassList.ShouldContain("cn-date-input--fill");
    }

    [Fact]
    public void Date_ExtraAttributes_LandOnTheInput_LikeTheOtherFields()
    {
        var cut = Render<CnDateField>(p => p.AddUnmatched("data-receipt-date", true));

        cut.Find("input").HasAttribute("data-receipt-date").ShouldBeTrue();
    }

    [Theory]
    [InlineData(true, false)]
    [InlineData(false, true)]
    public void Date_ReadOnlyOrDisabled_FrameClickDoesNotFocus(bool readOnly, bool disabled)
    {
        var cut = Render<CnDateField>(p => p
            .Add(x => x.Value, new DateTime(2026, 10, 6))
            .Add(x => x.ReadOnly, readOnly)
            .Add(x => x.Disabled, disabled));

        cut.Find(".cn-date-shell").Click();

        _dateModule.Invocations.Where(call => call.Identifier == "focusAtEnd").ShouldBeEmpty();
        cut.FindAll(".cn-date-pop").ShouldBeEmpty();
    }

    [Fact]
    public void Compact_MarksEveryFieldWrapper_AndNormalModeStaysUnchanged()
    {
        Wrapper(Render<CnDateField>(p => p.Add(x => x.Label, "Ontvangen op").Add(x => x.Compact, true))).ClassList.ShouldContain("cn-field--compact");
        Wrapper(Render<CnCurrencyField>(p => p.Add(x => x.Compact, true))).ClassList.ShouldContain("cn-field--compact");
        Wrapper(Render<CnNumberField>(p => p.Add(x => x.Compact, true))).ClassList.ShouldContain("cn-field--compact");
        Wrapper(Render<CnPercentField>(p => p.Add(x => x.Compact, true))).ClassList.ShouldContain("cn-field--compact");
        Wrapper(Render<CnTextField>(p => p.Add(x => x.Compact, true))).ClassList.ShouldContain("cn-field--compact");

        Wrapper(Render<CnDateField>()).ClassList.ShouldNotContain("cn-field--compact");
        Wrapper(Render<CnCurrencyField>()).ClassList.ShouldNotContain("cn-field--compact");
        Wrapper(Render<CnNumberField>()).ClassList.ShouldNotContain("cn-field--compact");
        Wrapper(Render<CnPercentField>()).ClassList.ShouldNotContain("cn-field--compact");
        Wrapper(Render<CnTextField>()).ClassList.ShouldNotContain("cn-field--compact");
    }

    [Fact]
    public void Compact_KeepsTheLabelAndTheValue()
    {
        decimal? value = null;
        var cut = Render<CnCurrencyField>(p => p
            .Add(x => x.Label, "Bedrag")
            .Add(x => x.Compact, true)
            .Add(x => x.ValueChanged, next => value = next));

        cut.Find(".cn-label").TextContent.ShouldBe("Bedrag");
        cut.Find("input").Change("1.234,56");

        value.ShouldBe(1234.56m);
    }

    [Theory]
    [InlineData("dagen", "--cn-unit-chars: 5")]
    [InlineData("%", "--cn-unit-chars: 1")]
    public void Unit_TellsTheFieldHowMuchRoomItNeeds(string unit, string style)
    {
        var number = Render<CnNumberField>(p => p.Add(x => x.Unit, unit));
        var currency = Render<CnCurrencyField>(p => p.Add(x => x.Unit, unit));

        number.Find(".cn-money").GetAttribute("style").ShouldBe(style);
        currency.Find(".cn-money").GetAttribute("style").ShouldBe(style);
    }

    [Fact]
    public void Unit_WithoutOne_SetsNoRoom()
    {
        var cut = Render<CnNumberField>();

        cut.Find(".cn-money").HasAttribute("style").ShouldBeFalse();
    }

    [Fact]
    public void Stylesheet_SizesTheUnit_FillsTheDate_AndHasTheCompactVariant()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));

        css.ShouldContain(".cn-money-input--unit { padding-right: max(30px, calc(var(--cn-unit-chars, 2) * .5em + 18px)); }");
        css.ShouldNotContain(".cn-money-input--unit { padding-right: 30px; }");
        css.ShouldContain(".cn-date-input--fill { flex: 1 1 74px; min-width: 74px; }");
        css.ShouldContain(".cn-field--compact {");
        css.ShouldContain(".cn-field--compact .cn-input { height: 26px; font-size: 13px; }");
    }

    private static AngleSharp.Dom.IElement Wrapper<TComponent>(IRenderedComponent<TComponent> cut)
        where TComponent : Microsoft.AspNetCore.Components.IComponent =>
        cut.Find(".cn-field");
}
