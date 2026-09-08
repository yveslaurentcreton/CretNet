using System.Collections;
using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Dialogs;
using CretNet.Platform.Blazor.Ui.Extensions;
using CretNet.Platform.Blazor.Ui.Resources;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

public class CnControlsTests : BunitContext
{
    public CnControlsTests()
    {
        Services.AddCretNetBlazorUi();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js").Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/CretNet.Platform.Blazor.Ui/Components/CnTimeInput.razor.js").Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("./_content/CretNet.Platform.Blazor.Ui/Components/CnRichTextEditor.razor.js").Mode = JSRuntimeMode.Loose;
    }

    [Theory]
    [InlineData("1.234", CnNumberParsingMode.Flexible, "1234")]
    [InlineData("1.234", CnNumberParsingMode.Decimal, "1.234")]
    [InlineData("1,234", CnNumberParsingMode.Decimal, "1.234")]
    [InlineData("1.234,56", CnNumberParsingMode.Decimal, "1234.56")]
    [InlineData("1,234.56", CnNumberParsingMode.Decimal, "1234.56")]
    [InlineData("1\u202f234,56", CnNumberParsingMode.Decimal, "1234.56")]
    public void Number_ParsingPolicy_PreservesChosenMeaning(string input, CnNumberParsingMode mode, string expected)
    {
        decimal? value = null;
        var cut = Render<CnNumberField>(p => p
            .Add(x => x.ParsingMode, mode)
            .Add(x => x.ValueChanged, next => value = next));

        cut.Find("input").Change(input);

        value.ShouldBe(decimal.Parse(expected, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Number_ClearAndZero_AreDistinct()
    {
        decimal? value = 5;
        var cut = Render<CnNumberField>(p => p.Add(x => x.ValueChanged, next => value = next));
        cut.Find("input").Change("");
        value.ShouldBeNull();
        cut.Find("input").Change("0");
        value.ShouldBe(0m);
    }

    [Fact]
    public void Currency_ClearAndZero_AreDistinct()
    {
        decimal? value = 5;
        var cut = Render<CnCurrencyField>(p => p.Add(x => x.ValueChanged, next => value = next));
        cut.Find("input").Change("");
        value.ShouldBeNull();
        cut.Find("input").Change("0");
        value.ShouldBe(0m);
    }

    [Theory]
    [InlineData("1.2.3")]
    [InlineData("1,2,3")]
    [InlineData("invalid")]
    public void DecimalParsing_InvalidInput_DoesNotInventAnAmount(string input) =>
        CnAmountParser.Parse(input, CnNumberParsingMode.Decimal).ShouldBeNull();

    [Fact]
    public void Number_PrecisionAndBounds_AreIndependentOfParsing()
    {
        using var culture = new CultureScope("nl-BE");
        decimal? value = null;
        var cut = Render<CnNumberField>(p => p
            .Add(x => x.Value, 1.234m)
            .Add(x => x.MaxDecimals, 3)
            .Add(x => x.Min, 0m).Add(x => x.Max, 2m)
            .Add(x => x.ParsingMode, CnNumberParsingMode.Decimal)
            .Add(x => x.ValueChanged, next => value = next));
        cut.Find("input").GetAttribute("value").ShouldBe("1,234");
        cut.Find("input").Change("3.456");
        value.ShouldBe(2m);
        CnAmountParser.Parse("1,234", CnNumberParsingMode.Culture).ShouldBe(1.234m);
    }

    [Fact]
    public void Resources_DutchAndFallback_CoverEveryOwnedLabel()
    {
        var english = CnLabels.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false)!;
        var dutch = CnLabels.ResourceManager.GetResourceSet(CultureInfo.GetCultureInfo("nl"), true, false)!;
        foreach (DictionaryEntry entry in english)
        {
            dutch.GetString((string)entry.Key).ShouldNotBeNullOrWhiteSpace();
            typeof(CnLabels).GetProperty((string)entry.Key).ShouldNotBeNull();
        }
        using (new CultureScope("nl-BE"))
        {
            CnLabels.Search.ShouldBe("Zoeken");
            CnLabels.Save.ShouldBe("Opslaan");
            Render<CnDialogHost>().Markup.ShouldNotBeNull();
            var date = Render<CnDateField>(p => p.Add(x => x.Value, new DateTime(2026, 9, 5)));
            date.Find(".cn-date-clear").GetAttribute("title").ShouldBe("Wissen");
        }
        using (new CultureScope("fr-FR"))
            CnLabels.Search.ShouldBe("Search");
    }

    [Theory]
    [InlineData("nl-BE", "1234,5")]
    [InlineData("en-GB", "1234.5")]
    public void DecimalMode_DisplayCanBeCommittedWithoutChangingMagnitude(string cultureName, string expectedText)
    {
        using var culture = new CultureScope(cultureName);
        decimal? value = null;
        var cut = Render<CnNumberField>(p => p
            .Add(x => x.Value, 1234.5m)
            .Add(x => x.ParsingMode, CnNumberParsingMode.Decimal)
            .Add(x => x.ValueChanged, next => value = next));
        var input = cut.Find("input");
        input.GetAttribute("value").ShouldBe(expectedText);
        input.Change(expectedText);
        value.ShouldBe(1234.5m);
    }

    [Fact]
    public void Picker_DutchResources_LocalizeChromeWithoutHostLabels()
    {
        using var culture = new CultureScope("nl-BE");
        var cut = Render<CnPicker>(p => p
            .Add(x => x.Value, Guid.NewGuid())
            .Add(x => x.ResolveLabel, _ => Task.FromResult<string?>("Selected"))
            .Add(x => x.Provider, _ => Task.FromResult<IReadOnlyList<CnPickerItem>>([])));
        cut.Find("input").GetAttribute("placeholder").ShouldBe("Selecteren…");
        cut.Find("button").GetAttribute("title").ShouldBe("Wissen");
    }

    [Fact]
    public void Date_TypingAndClearing_UsesTheSharedMaskedInput()
    {
        DateTime? value = null;
        var cut = Render<CnDateField>(p => p.Add(x => x.ValueChanged, next => value = next));
        cut.Find("input").Input("05092026");
        value.ShouldBe(new DateTime(2026, 9, 5));
        cut.Find("input").Input("");
        value.ShouldBeNull();
    }

    [Fact]
    public void RichTextBox_OpensSharedEditor_AndCancelsDraft()
    {
        var cut = Render<CnRichTextBox>(p => p.Add(x => x.Placeholder, "Write a note").Add(x => x.PostLabel, "Post"));
        cut.Find("[role=textbox]").Click();
        cut.FindAll(".rte-editor").Count.ShouldBe(1);
        cut.FindComponents<CnButton>()[1].Find("button").Click();
        cut.FindAll(".rte-editor").ShouldBeEmpty();
    }

    [Fact]
    public async Task DateAndTime_DisposedAfterHostChange_DoNotRestoreFocus()
    {
        var date = Render<CnDateInput>();
        var time = Render<CnTimeInput>();
        await date.Instance.DisposeAsync();
        await time.Instance.DisposeAsync();
        var callsBefore = JSInterop.Invocations.Count;
        await date.Instance.FocusAsync(true);
        await time.Instance.FocusAsync(true);
        JSInterop.Invocations.Count.ShouldBe(callsBefore);
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;
        public CultureScope(string name) => CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        public void Dispose()
        {
            CultureInfo.CurrentCulture = _culture;
            CultureInfo.CurrentUICulture = _uiCulture;
        }
    }
}
