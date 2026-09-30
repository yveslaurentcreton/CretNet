using Bunit;
using CretNet.Platform.Blazor.Ui.Components;
using Shouldly;

namespace CretNet.Tests.Component;

/// <summary>
/// S-348: whatever reaches or leaves CnRichTextEditor is reduced to the
/// allow-list — the initial value, later values, every input and pastes.
/// The browser-side structural clean-up is covered in the sample (see
/// docs/docs/cn-rich-text.md); these tests pin the .NET guarantees.
/// </summary>
public sealed class CnRichTextEditorHardeningTests : CnTestContext
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnRichTextEditor.razor.js";
    private readonly BunitJSModuleInterop _module;

    public CnRichTextEditorHardeningTests()
    {
        _module = JSInterop.SetupModule(ModulePath);
        _module.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public void InitialValue_IsSanitisedBeforeItReachesTheDom()
    {
        Render<CnRichTextEditor>(p => p.Add(x => x.Value, "<p onclick=\"x()\">a<script>b</script></p><div style=\"font-weight:700\">c</div>"));

        var init = _module.Invocations["init"].ShouldHaveSingleItem();
        init.Arguments[3].ShouldBe("<p>a</p><p><strong>c</strong></p>");
    }

    [Fact]
    public void LaterValue_IsSanitisedToo()
    {
        var cut = Render<CnRichTextEditor>(p => p.Add(x => x.Value, "<p>a</p>"));

        cut.Render(p => p.Add(x => x.Value, "<p><a href=\"javascript:alert(1)\">b</a><img src=x onerror=alert(1)></p>"));

        _module.Invocations["setContent"].ShouldHaveSingleItem().Arguments[1].ShouldBe("<p>b</p>");
    }

    [Fact]
    public async Task EveryInput_StoresOnlyTheAllowList()
    {
        string? stored = null;
        var cut = Render<CnRichTextEditor>(p => p.Add(x => x.ValueChanged, value => stored = value));

        await cut.InvokeAsync(() => cut.Instance.OnContentChanged(
            "loose <span style=\"color:#00B050\">groen</span><p style=\"x\" onmouseover=\"y\">p</p><h1>k</h1>"));

        stored.ShouldBe("<p>loose <span data-color=\"accent\">groen</span></p><p>p</p><h2>k</h2>");
        foreach (var forbidden in new[] { "style", "onmouseover", "script", "javascript:" })
            stored!.ShouldNotContain(forbidden);
    }

    [Fact]
    public async Task EmptyEditor_StoresEmpty()
    {
        string? stored = "x";
        var cut = Render<CnRichTextEditor>(p => p.Add(x => x.ValueChanged, value => stored = value));

        await cut.InvokeAsync(() => cut.Instance.OnContentChanged("<p><br></p>"));

        stored.ShouldBe(string.Empty);
    }

    [Theory]
    [InlineData(
        "<meta charset=\"utf-8\"><b style=\"font-weight:normal;\" id=\"docs-internal-guid-1\"><p dir=\"ltr\"><span style=\"font-size:11pt;font-weight:700;\">Voorwaarden</span></p><ol><li><span style=\"font-size:11pt;color:#cc0000\">Levering</span></li></ol></b>",
        "<p><strong>Voorwaarden</strong></p><ol><li><span data-color=\"rood\">Levering</span></li></ol>")]
    [InlineData(
        "<p class=MsoNormal><b><span style=\"font-family:Calibri\">Overdracht</span></b><o:p></o:p></p><ul><li>Documentatie<ul><li><i>genest</i></li></ul></li></ul>",
        "<p><strong>Overdracht</strong></p><ul><li>Documentatie<ul><li><em>genest</em></li></ul></li></ul>")]
    [InlineData(
        "Hallo<div>Beste <span style=\"text-decoration:underline\">klant</span></div><script>alert(1)</script>",
        "<p>Hallo</p><p>Beste <u>klant</u></p>")]
    public void Paste_KeepsFormattingAndDropsTheRest(string html, string expected)
    {
        var cut = Render<CnRichTextEditor>();
        cut.Instance.SanitizePaste(html).ShouldBe(expected);
    }

    [Fact]
    public void Toolbar_HasAPaletteButtonThatColoursTheSelection()
    {
        var cut = Render<CnRichTextEditor>(p => p.Add(x => x.Value, "<p>a</p>"));

        cut.Find(".rte-toolbar .cn-palette button[aria-label=Colour]").Click();
        cut.FindAll(".cn-palette__item").Count.ShouldBe(5);
        cut.Find(".cn-palette__item[data-color=blauw]").Click();

        _module.Invocations["applyColor"].ShouldHaveSingleItem().Arguments[1].ShouldBe("blauw");

        cut.Find(".rte-toolbar .cn-palette button[aria-label=Colour]").Click();
        cut.Find(".cn-palette__item[data-color='']").Click();
        _module.Invocations["applyColor"][1].Arguments[1].ShouldBeNull();
    }

    [Fact]
    public void ReadOnly_DisablesThePaletteButton()
    {
        var cut = Render<CnRichTextEditor>(p => p.Add(x => x.ReadOnly, true));
        cut.Find(".cn-palette button").HasAttribute("disabled").ShouldBeTrue();
    }

    [Fact]
    public void Css_DefinesPaletteColoursForBothThemes()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));
        foreach (var name in new[] { "accent", "blauw", "oranje", "rood" })
            css.ShouldContain($".cn-md-c-{name} {{ color: var(--cn-md-{name}); }}");
        css.ShouldContain("--cn-md-accent: #3fbf5f;");
    }
}
