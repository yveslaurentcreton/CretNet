using System.Globalization;
using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.Platform.Blazor.Ui.Resources;
using CretNet.Platform.Blazor.Ui.Toasts;
using CretNet.RichText;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// CnMarkdownEditor (S-349): toolbar commands edit the Markdown at the
/// textarea selection, modes switch between text and the shared preview, the
/// compact variant keeps a minimal toolbar, and pasted HTML is converted.
/// </summary>
public sealed class CnMarkdownEditorTests : CnTestContext
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnMarkdownEditor.razor.js";
    private readonly BunitJSModuleInterop _module;
    private readonly CultureScope _culture = new("en");

    public CnMarkdownEditorTests()
    {
        _module = JSInterop.SetupModule(ModulePath);
        _module.Mode = JSRuntimeMode.Loose;
    }

    protected override void Dispose(bool disposing)
    {
        _culture.Dispose();
        base.Dispose(disposing);
    }

    private void Select(int start, int end) =>
        _module.Setup<CnTextSelection?>("getSelection", _ => true).SetResult(new CnTextSelection(start, end));

    private IRenderedComponent<CnMarkdownEditor> Editor(string? value, Action<string?> changed, Action<ComponentParameterCollectionBuilder<CnMarkdownEditor>>? extra = null) =>
        Render<CnMarkdownEditor>(p =>
        {
            p.Add(x => x.Value, value).Add(x => x.ValueChanged, next => changed(next));
            extra?.Invoke(p);
        });

    private static string[] ToolbarLabels(IRenderedComponent<CnMarkdownEditor> cut) =>
        cut.FindAll(".cn-md-bar .cn-md-btn").Select(button => button.GetAttribute("aria-label")!).ToArray();

    [Fact]
    public void FullToolbar_FollowsTheMock()
    {
        var cut = Editor("", _ => { });

        ToolbarLabels(cut).ShouldBe(["Bold", "Italic", "Heading", "Subtitle", "Bulleted list", "Numbered list", "Link", "Colour", "Clear formatting"]);
        cut.FindAll(".cn-md-mode").Select(mode => mode.TextContent).ShouldBe(["Write", "Preview", "Side by side"]);
        cut.Find(".cn-md").GetAttribute("data-mode").ShouldBe("split");
        cut.Find(".cn-md-mode[aria-pressed=true]").TextContent.ShouldBe("Side by side");
        cut.Find("[title='Bold (Ctrl+B)']").ShouldNotBeNull();
    }

    [Fact]
    public void CompactVariant_HasMinimalToolbarAndTwoModes()
    {
        var cut = Editor("*x*", _ => { }, p => p.Add(x => x.Compact, true));

        ToolbarLabels(cut).ShouldBe(["Bold", "Italic", "Bulleted list", "Link"]);
        cut.FindAll(".cn-md-mode").Select(mode => mode.TextContent).ShouldBe(["Write", "Preview"]);
        cut.Find(".cn-md").ClassList.ShouldContain("cn-md--compact");
        cut.Find(".cn-md").GetAttribute("data-mode").ShouldBe("write");
        cut.FindAll(".cn-md-preview").ShouldBeEmpty();
        cut.FindAll(".cn-md-foot").ShouldBeEmpty();

        cut.FindAll(".cn-md-mode")[1].Click();
        cut.Find(".cn-md-preview .cn-md-doc em").TextContent.ShouldBe("x");
        cut.FindAll(".cn-md-paper").ShouldBeEmpty();
    }

    [Theory]
    [InlineData("Bold", 0, 5, "**Hallo** wereld", 2, 7)]
    [InlineData("Italic", 6, 12, "Hallo *wereld*", 7, 13)]
    [InlineData("Italic", 5, 5, "Hallo*italic text* wereld", 6, 17)]
    [InlineData("Heading", 3, 3, "## Hallo wereld", 15, 15)]
    [InlineData("Subtitle", 0, 0, "### Hallo wereld", 16, 16)]
    [InlineData("Bulleted list", 0, 0, "- Hallo wereld", 14, 14)]
    [InlineData("Numbered list", 0, 0, "1. Hallo wereld", 15, 15)]
    [InlineData("Link", 6, 12, "Hallo [wereld](https://)", 7, 13)]
    public void ToolbarButton_EditsTheSelection(string label, int start, int end, string expected, int selStart, int selEnd)
    {
        string? value = null;
        Select(start, end);
        var cut = Editor("Hallo wereld", next => value = next);

        cut.Find($".cn-md-btn[aria-label='{label}']").Click();

        value.ShouldBe(expected);
        cut.Find("textarea").GetAttribute("value").ShouldBe(expected);
        var apply = _module.Invocations["apply"].ShouldHaveSingleItem();
        apply.Arguments.Skip(1).ShouldBe([expected, selStart, selEnd]);
    }

    [Fact]
    public void BoldTwice_TogglesItOff()
    {
        string? value = "a **b** c";
        Select(4, 5);
        var cut = Editor(value, next => value = next);

        cut.Find(".cn-md-btn[aria-label='Bold']").Click();

        value.ShouldBe("a b c");
    }

    [Fact]
    public void ColourMenu_WrapsSelectionInPaletteSyntax()
    {
        string? value = null;
        Select(0, 5);
        var cut = Editor("Hallo wereld", next => value = next);

        cut.Find(".cn-md-btn[aria-label='Colour']").Click();
        cut.FindAll(".cn-palette__item").Select(item => item.TextContent.Trim()).ShouldBe(["Green.accent", "Blue.blauw", "Orange.oranje", "Red.rood"]);
        cut.Find(".cn-palette__item[data-color=rood]").Click();

        value.ShouldBe("[Hallo]{.rood} wereld");
        cut.FindAll(".cn-palette__list").ShouldBeEmpty();
    }

    [Fact]
    public void ClearFormatting_WithoutSelection_ExplainsInAToast()
    {
        string? value = "**x**";
        Select(2, 2);
        var cut = Editor(value, next => value = next);

        cut.Find(".cn-md-btn[aria-label='Clear formatting']").Click();

        value.ShouldBe("**x**");
        Services.GetRequiredService<CnToastService>().Items.ShouldHaveSingleItem().Title.ShouldBe(CnLabels.SelectTextToClear);
    }

    [Fact]
    public void ClearFormatting_RemovesMarkdownFromTheSelection()
    {
        string? value = null;
        Select(0, 39);
        var cut = Editor("## **Vet** [rood]{.rood} [l](https://x)", next => value = next);

        cut.Find(".cn-md-btn[aria-label='Clear formatting']").Click();

        value.ShouldBe("Vet rood l");
    }

    [Fact]
    public async Task Shortcuts_RunTheirCommands()
    {
        string? value = "Hallo wereld";
        var cut = Editor(value, next => value = next);

        await cut.InvokeAsync(() => cut.Instance.OnShortcutAsync("b", 0, 5));
        value.ShouldBe("**Hallo** wereld");

        cut.Render(p => p.Add(x => x.Value, value));
        await cut.InvokeAsync(() => cut.Instance.OnShortcutAsync("k", 10, 16));
        value.ShouldBe("**Hallo** [wereld](https://)");
    }

    [Fact]
    public void Modes_SwitchBetweenTextAndPreview()
    {
        var cut = Editor("**Subtitel**\n\n<script>alert(1)</script> <img src=x onerror=alert(1)>", _ => { });

        cut.Find(".cn-md-preview .cn-md-paper strong").TextContent.ShouldBe("Subtitel");
        cut.FindAll(".cn-md-preview script, .cn-md-preview img").ShouldBeEmpty();
        cut.Find(".cn-md-preview").TextContent.ShouldContain("<script>alert(1)</script>");

        cut.Find(".cn-md-mode[data-mode=write]").Click();
        cut.Find(".cn-md").GetAttribute("data-mode").ShouldBe("write");
        cut.FindAll(".cn-md-preview").ShouldBeEmpty();

        cut.Find(".cn-md-mode[data-mode=preview]").Click();
        cut.Find(".cn-md").GetAttribute("data-mode").ShouldBe("preview");
        cut.FindAll(".cn-md-preview").Count.ShouldBe(1);
        cut.FindAll(".cn-md-btn").ShouldAllBe(button => button.HasAttribute("disabled"));
        cut.FindAll("textarea").Count.ShouldBe(1);
    }

    [Fact]
    public void DefaultMode_IsAParameter()
    {
        var cut = Editor("x", _ => { }, p => p.Add(x => x.DefaultMode, CnMarkdownEditorMode.Preview));
        cut.Find(".cn-md").GetAttribute("data-mode").ShouldBe("preview");
        cut.Instance.Mode.ShouldBe(CnMarkdownEditorMode.Preview);
    }

    [Fact]
    public void Preview_FollowsTypedValue()
    {
        var cut = Editor("", _ => { });

        cut.Find("textarea").Input("[Let op]{.accent} **nu**");

        cut.Find(".cn-md-preview span.cn-md-c-accent").TextContent.ShouldBe("Let op");
        cut.Find(".cn-md-preview strong").TextContent.ShouldBe("nu");
    }

    [Fact]
    public void Counter_ShowsLengthAgainstMaxLength()
    {
        using var culture = new CultureScope("nl-BE");
        var cut = Editor("Hallo", _ => { }, p => p.Add(x => x.MaxLength, 20000).Add(x => x.Hint, "voorbeeld = PDF"));

        cut.Find(".cn-md-count").TextContent.ShouldBe("5 / 20.000");
        cut.Find(".cn-md-count").ClassList.ShouldNotContain("cn-md-count--over");
        cut.Find(".cn-md-foot").TextContent.ShouldContain("voorbeeld = PDF");

        cut.Render(p => p.Add(x => x.Value, new string('x', 20001)));
        cut.Find(".cn-md-count").ClassList.ShouldContain("cn-md-count--over");
    }

    [Fact]
    public async Task PastedHtml_IsConvertedInsertedAndReported()
    {
        string? value = "Intro";
        CnHtmlToMarkdownResult? converted = null;
        var cut = Editor(value, next => value = next, p => p.Add(x => x.PasteConverted, result => converted = result));

        await cut.InvokeAsync(() => cut.Instance.OnPasteHtmlAsync(
            "<p><b>Titel</b></p><ul><li><u>een</u></li><li>twee<ul><li>drie</li></ul></li></ul>",
            "Titel\neen\ntwee\ndrie", 5, 5));

        value.ShouldBe("Intro\n\n**Titel**\n\n- een\n- twee\n  - drie\n");
        converted.ShouldNotBeNull().Dropped.ShouldBe([CnDroppedFormatting.Underline]);
        var toast = Services.GetRequiredService<CnToastService>().Items.ShouldHaveSingleItem();
        toast.Title.ShouldBe("Pasted as Markdown.");
        toast.Message.ShouldBe("Left out: underline.");
    }

    [Fact]
    public async Task PastedWord_FromAWebPage_GoesInlineWithoutToast()
    {
        string? value = "Zie  hier";
        var cut = Editor(value, next => value = next);

        await cut.InvokeAsync(() => cut.Instance.OnPasteHtmlAsync("<span style=\"color:#333\">daar</span>", "daar", 4, 4));

        value.ShouldBe("Zie daar hier");
        Services.GetRequiredService<CnToastService>().Items.ShouldBeEmpty();
    }

    [Fact]
    public async Task ReadOnly_DisablesFormattingButKeepsModes()
    {
        string? value = "x";
        var cut = Editor(value, next => value = next, p => p.Add(x => x.ReadOnly, true));

        cut.FindAll(".cn-md-btn").ShouldAllBe(button => button.HasAttribute("disabled"));
        cut.Find("textarea").HasAttribute("readonly").ShouldBeTrue();
        cut.FindAll(".cn-md-mode").ShouldAllBe(mode => !mode.HasAttribute("disabled"));

        await cut.InvokeAsync(() => cut.Instance.OnShortcutAsync("b", 0, 1));
        await cut.InvokeAsync(() => cut.Instance.OnPasteHtmlAsync("<b>y</b>", "y", 0, 0));
        value.ShouldBe("x");
    }

    [Fact]
    public void Validation_UsesFieldChrome()
    {
        var model = new Model();
        var context = new EditContext(model);
        var messages = new ValidationMessageStore(context);
        messages.Add(context.Field(nameof(Model.Text)), "Te lang");

        var cut = Render<CnMarkdownEditor>(p => p
            .AddCascadingValue(context)
            .Add(x => x.Label, "Inleiding")
            .Add(x => x.For, () => model.Text));

        cut.Find(".cn-md").ClassList.ShouldContain("cn-md--invalid");
        cut.Find(".cn-field-error").TextContent.ShouldBe("Te lang");
        cut.Find("label.cn-label").GetAttribute("for").ShouldBe(cut.Find("textarea").Id);
    }

    [Fact]
    public void Labels_FollowTheUiCulture()
    {
        using var culture = new CultureScope("nl-BE");
        var cut = Editor("", _ => { });

        ToolbarLabels(cut).ShouldBe(["Vet", "Cursief", "Kop", "Tussentitel", "Opsomming", "Genummerde lijst", "Link", "Kleur", "Opmaak wissen"]);
        cut.FindAll(".cn-md-mode").Select(mode => mode.TextContent).ShouldBe(["Schrijven", "Voorbeeld", "Naast elkaar"]);
    }

    [Fact]
    public void MarkdownView_RendersSafeHtml()
    {
        var cut = Render<CnMarkdownView>(p => p.Add(x => x.Value, "# Kop\n\n[x]{.blauw} <b>y</b>").Add(x => x.Paper, true));

        cut.Find(".cn-md-doc.cn-md-paper h2").TextContent.ShouldBe("Kop");
        cut.Find("span.cn-md-c-blauw").TextContent.ShouldBe("x");
        cut.FindAll("b").ShouldBeEmpty();
    }

    private sealed class Model
    {
        public string? Text { get; set; }
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

        public CultureScope(string name)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _culture;
            CultureInfo.CurrentUICulture = _uiCulture;
        }
    }
}
