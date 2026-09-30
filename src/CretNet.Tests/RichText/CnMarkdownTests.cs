using AngleSharp.Html.Parser;
using CretNet.RichText;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Shouldly;

namespace CretNet.Tests.RichText;

public class CnMarkdownTests
{
    private static AngleSharp.Dom.IElement Html(string markdown) =>
        new HtmlParser().ParseDocument(CnMarkdown.ToSafeHtml(markdown)).Body!;

    [Fact]
    public void BoldSubtitle_RendersBold() =>
        CnMarkdown.ToSafeHtml("**Subtitel**").Trim().ShouldBe("<p><strong>Subtitel</strong></p>");

    [Theory]
    [InlineData("<b>test</b>")]
    [InlineData("<script>alert(1)</script>")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("tekst <img src=x onerror=alert(1)> midden")]
    [InlineData("<div onclick=\"alert(1)\">x</div>")]
    [InlineData("<!-- comment --><iframe src=\"https://evil\"></iframe>")]
    public void RawHtml_RendersAsLiteralText(string markdown)
    {
        var body = Html(markdown);
        body.QuerySelectorAll("script, img, div, iframe, b").ShouldBeEmpty();
        body.QuerySelectorAll("*").SelectMany(element => element.Attributes).ShouldBeEmpty();
        body.TextContent.Trim().ShouldBe(markdown);
    }

    [Fact]
    public void ScriptAndImage_AreNeverElementsAnywhere()
    {
        var body = Html("# <script>x</script>\n\n- <img src=x onerror=alert(1)>\n\n| a | <script>b</script> |\n|---|---|\n| <img onerror=y> | c |");
        body.QuerySelectorAll("script, img").ShouldBeEmpty();
        body.TextContent.ShouldContain("<script>x</script>");
        body.TextContent.ShouldContain("<img src=x onerror=alert(1)>");
    }

    [Theory]
    [InlineData("accent")]
    [InlineData("blauw")]
    [InlineData("oranje")]
    [InlineData("rood")]
    public void PaletteClass_RendersColouredSpan(string name) =>
        CnMarkdown.ToSafeHtml($"Let op: [dit]{{.{name}}} nu.").Trim()
            .ShouldBe($"<p>Let op: <span class=\"cn-md-c-{name}\">dit</span> nu.</p>");

    [Fact]
    public void PaletteSpan_KeepsInnerFormatting() =>
        CnMarkdown.ToSafeHtml("a [**vet** en *schuin*]{.accent} b").Trim()
            .ShouldBe("<p>a <span class=\"cn-md-c-accent\"><strong>vet</strong> en <em>schuin</em></span> b</p>");

    [Fact]
    public void PaletteSpan_InsideEmphasisAndLists()
    {
        CnMarkdown.ToSafeHtml("**[x]{.rood}**").Trim().ShouldBe("<p><strong><span class=\"cn-md-c-rood\">x</span></strong></p>");
        var list = Html("- item [k]{.oranje}\n  - nested [n]{.blauw}");
        list.QuerySelectorAll("li > span.cn-md-c-oranje").Length.ShouldBe(1);
        list.QuerySelectorAll("li li > span.cn-md-c-blauw").Length.ShouldBe(1);
    }

    [Theory]
    [InlineData("[tekst]{.groen}", "<p>tekst</p>")]
    [InlineData("[tekst]{.Onclick}", "<p>tekst</p>")]
    [InlineData("a [b]{.foo} c", "<p>a b c</p>")]
    public void UnknownClass_RendersPlainTextWithoutBracketsOrAttributes(string markdown, string expected) =>
        CnMarkdown.ToSafeHtml(markdown).Trim().ShouldBe(expected);

    [Theory]
    [InlineData("\\[x]{.accent}", "<p>[x]{.accent}</p>")]
    [InlineData("[x]{.accent", "<p>[x]{.accent</p>")]
    [InlineData("x {.accent} y {a=b}", "<p>x {.accent} y {a=b}</p>")]
    [InlineData("`[c]{.accent}`", "<p><code>[c]{.accent}</code></p>")]
    public void BracesThatAreNotPalette_StayAsTyped(string markdown, string expected) =>
        CnMarkdown.ToSafeHtml(markdown).Trim().ShouldBe(expected);

    [Fact]
    public void GenericAttributes_AreNotRendered()
    {
        var body = Html("## Kop {#id .accent onclick=alert(1)}\n\nTekst {style=color:red}");
        body.QuerySelectorAll("*").SelectMany(element => element.Attributes).ShouldBeEmpty();
        body.TextContent.ShouldContain("{#id .accent onclick=alert(1)}");
    }

    [Fact]
    public void NestedBrackets_BelongToTheColouredText() =>
        CnMarkdown.ToSafeHtml("[a [b] c]{.blauw}").Trim().ShouldBe("<p><span class=\"cn-md-c-blauw\">a [b] c</span></p>");

    [Fact]
    public void Parse_ExposesPaletteColourToRenderers()
    {
        var document = CnMarkdown.Parse("Tekst [belangrijk]{.rood}");
        var color = document.Descendants<CnMarkdownColorInline>().ShouldHaveSingleItem();
        color.Color.ShouldBe(CnMarkdownPalette.Red);
        color.Color.PrintHex.ShouldBe("#c42b2f");
        color.Descendants<LiteralInline>().Single().Content.ToString().ShouldBe("belangrijk");
    }

    [Theory]
    [InlineData("# Een", 2)]
    [InlineData("## Twee", 2)]
    [InlineData("### Drie", 3)]
    [InlineData("#### Vier", 3)]
    [InlineData("###### Zes", 3)]
    [InlineData("Setext\n===", 2)]
    public void Headings_MapToLevelsTwoAndThree(string markdown, int level)
    {
        CnMarkdown.Parse(markdown).Descendants<HeadingBlock>().Single().Level.ShouldBe(level);
        CnMarkdown.ToSafeHtml(markdown).ShouldStartWith($"<h{level}>");
    }

    [Theory]
    [InlineData("[x](javascript:alert(1))")]
    [InlineData("[x](JAVASCRIPT:alert(1))")]
    [InlineData("[x](data:text/html,hi)")]
    [InlineData("[x](/relative/path)")]
    [InlineData("[x](vbscript:msgbox)")]
    [InlineData("![x](https://example.com/a.png)")]
    public void UnsafeLinksAndImages_RenderTheirText(string markdown)
    {
        var body = Html(markdown);
        body.QuerySelectorAll("a, img").ShouldBeEmpty();
        body.TextContent.Trim().ShouldBe("x");
    }

    [Theory]
    [InlineData("[site](https://gyves.be)", "https://gyves.be")]
    [InlineData("[site](http://gyves.be/a?b=c)", "http://gyves.be/a?b=c")]
    [InlineData("[mail](mailto:info@gyves.be)", "mailto:info@gyves.be")]
    [InlineData("<https://gyves.be>", "https://gyves.be")]
    public void SafeLinks_AreLinks(string markdown, string href)
    {
        var link = Html(markdown).QuerySelector("a").ShouldNotBeNull();
        link.GetAttribute("href").ShouldBe(href);
        link.GetAttribute("rel").ShouldBe("noopener noreferrer");
    }

    [Fact]
    public void UnsafeAutolink_RendersItsText()
    {
        var body = Html("<javascript:alert(1)>");
        body.QuerySelectorAll("a").ShouldBeEmpty();
        body.TextContent.Trim().ShouldBe("javascript:alert(1)");
    }

    [Fact]
    public void BareUrls_AreNotLinked() =>
        Html("zie https://gyves.be of www.gyves.be").QuerySelectorAll("a").ShouldBeEmpty();

    [Fact]
    public void TablesAndStrikethrough_AreSupported()
    {
        var body = Html("| Fase | Wanneer |\n|---|---|\n| Netwerk | week 44 |\n\n~~oud~~");
        body.QuerySelectorAll("table th").Length.ShouldBe(2);
        body.QuerySelector("td")!.TextContent.ShouldBe("Netwerk");
        body.QuerySelector("del")!.TextContent.ShouldBe("oud");
    }

    [Fact]
    public void Subscript_IsNotAnExtra() =>
        Html("H~2~O").QuerySelectorAll("sub").ShouldBeEmpty();

    [Fact]
    public void PlainText_DropsAllSyntax()
    {
        const string markdown = "## Wat we voorstellen\n\n**Netwerk** met [kleur]{.accent} en een [link](https://x.be).\n\n- een\n  - twee\n1. drie\n\n| Fase | Wanneer |\n|---|---|\n| Netwerk | week 44 |\n\nregel  \nbreuk <b>ruw</b>";
        CnMarkdown.ToPlainText(markdown).ShouldBe(
            "Wat we voorstellen\nNetwerk met kleur en een link.\neen\ntwee\ndrie\nFase Wanneer\nNetwerk week 44\nregel\nbreuk <b>ruw</b>");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void EmptyInput_GivesEmptyOutput(string? markdown)
    {
        CnMarkdown.ToSafeHtml(markdown).ShouldBe(string.Empty);
        CnMarkdown.ToPlainText(markdown).ShouldBe(string.Empty);
        CnMarkdown.Parse(markdown).Count.ShouldBe(0);
    }

    [Fact]
    public void Pipeline_IsOneSharedInstance() =>
        CnMarkdown.Pipeline.ShouldBeSameAs(CnMarkdown.Pipeline);
}
