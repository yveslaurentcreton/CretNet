using AngleSharp.Html.Parser;
using CretNet.RichText;
using Shouldly;

namespace CretNet.Tests.RichText;

public class CnRichTextSanitizerTests
{
    private static string Clean(string html) => CnRichTextSanitizer.Sanitize(html);

    [Theory]
    [InlineData("<p>a</p><script>alert(1)</script>", "<p>a</p>")]
    [InlineData("<p onclick=\"alert(1)\" onmouseover=\"x()\">a</p>", "<p>a</p>")]
    [InlineData("<p><img src=x onerror=alert(1)>a</p>", "<p>a</p>")]
    [InlineData("<p><a href=\"javascript:alert(1)\">a</a></p>", "<p>a</p>")]
    [InlineData("<p><a href=\" JaVaScRiPt:alert(1)\">a</a></p>", "<p>a</p>")]
    [InlineData("<p><a href=\"data:text/html,x\">a</a></p>", "<p>a</p>")]
    [InlineData("<iframe src=\"https://evil\"></iframe><p>a</p>", "<p>a</p>")]
    [InlineData("<style>p{}</style><p style=\"font-size:20px;font-family:Arial\">a</p>", "<p>a</p>")]
    [InlineData("<svg onload=alert(1)><script>x</script></svg><p>a</p>", "<p>a</p>")]
    public void DangerousContent_IsRemoved(string html, string expected) =>
        Clean(html).ShouldBe(expected);

    [Theory]
    [InlineData("<p><span style=\"font-weight:700\">a</span></p>", "<p><strong>a</strong></p>")]
    [InlineData("<p><span style=\"font-style:italic\">a</span></p>", "<p><em>a</em></p>")]
    [InlineData("<p><span style=\"text-decoration:underline\">a</span></p>", "<p><u>a</u></p>")]
    [InlineData("<p><span style=\"color:#00B050\">a</span></p>", "<p><span data-color=\"accent\">a</span></p>")]
    [InlineData("<p><span style=\"color:#333\">a</span></p>", "<p>a</p>")]
    [InlineData("<p><font color=\"red\">a</font></p>", "<p><span data-color=\"rood\">a</span></p>")]
    [InlineData("<p><b>a</b> <i>b</i> <u>c</u></p>", "<p><strong>a</strong> <em>b</em> <u>c</u></p>")]
    public void StyleFormatting_BecomesTagsBeforeStylesAreDropped(string html, string expected) =>
        Clean(html).ShouldBe(expected);

    [Fact]
    public void GoogleDocsWrapper_IsNotBold() =>
        Clean("<meta charset=\"utf-8\"><b style=\"font-weight:normal;\" id=\"docs-internal-guid-1\"><p dir=\"ltr\"><span style=\"font-weight:700\">Titel</span></p><p><span style=\"font-weight:400\">Tekst</span></p></b>")
            .ShouldBe("<p><strong>Titel</strong></p><p>Tekst</p>");

    [Fact]
    public void LooseTopLevelText_IsWrappedInParagraphs() =>
        Clean("Hallo <b>daar</b><div>Tweede</div>los<br>einde").ShouldBe("<p>Hallo <strong>daar</strong></p><p>Tweede</p><p>los<br>einde</p>");

    [Fact]
    public void Headings_MapToH2AndH3() =>
        Clean("<h1>A</h1><h2><b>B</b></h2><h4>C</h4><h6>D</h6>").ShouldBe("<h2>A</h2><h2>B</h2><h3>C</h3><h3>D</h3>");

    [Fact]
    public void NestedLists_AreKept() =>
        Clean("<ul><li>a<ol><li>b</li></ol></li><li><p>c</p><p>d</p></li></ul>")
            .ShouldBe("<ul><li>a<ol><li>b</li></ol></li><li>c<br>d</li></ul>");

    [Fact]
    public void WordListParagraphs_BecomeLists() =>
        Clean("<p class=MsoListParagraph style='mso-list:l0 level1 lfo1'><![if !supportLists]><span><span style='mso-list:Ignore'>·<span>&nbsp;</span></span></span><![endif]>Een</p>" +
              "<p class=MsoListParagraph style='mso-list:l0 level2 lfo1'><![if !supportLists]><span><span style='mso-list:Ignore'>o<span>&nbsp;</span></span></span><![endif]>Genest</p>")
            .ShouldBe("<ul><li>Een<ul><li>Genest</li></ul></li></ul>");

    [Fact]
    public void PaletteSpans_AreKeptAndOtherSpansUnwrapped()
    {
        Clean("<p><span data-color=\"blauw\" class=\"x\" style=\"color:red\">a</span> <span data-color=\"paars\">b</span> <span class=\"y\">c</span></p>")
            .ShouldBe("<p><span data-color=\"blauw\">a</span> b c</p>");
    }

    [Fact]
    public void SafeLinks_KeepOnlyTheirHref() =>
        Clean("<p><a href=\"https://gyves.be\" target=\"_blank\" onclick=\"x()\" style=\"color:red\">site</a> <a href=\"mailto:a@b.be\">mail</a></p>")
            .ShouldBe("<p><span data-color=\"rood\"><a href=\"https://gyves.be\">site</a></span> <a href=\"mailto:a@b.be\">mail</a></p>");

    [Fact]
    public void Text_IsEncoded() =>
        Clean("<p>&lt;script&gt;alert(1)&lt;/script&gt; &amp; \"q\" é&nbsp;x</p>")
            .ShouldBe("<p>&lt;script&gt;alert(1)&lt;/script&gt; &amp; &quot;q&quot; é&nbsp;x</p>");

    [Fact]
    public void Output_ContainsOnlyTheAllowList()
    {
        const string html = "<div style=\"color:#c00\" onclick=\"x\"><table><tr><td><b>a</b></td><td>b</td></tr></table>" +
                            "<blockquote>c</blockquote><section><h5 id=\"h\">d</h5></section><pre>e\nf</pre><hr>" +
                            "<a href=\"vbscript:x\">g</a><form><input value=\"h\"></form><sup>i</sup><code>j</code></div>";
        var body = new HtmlParser().ParseDocument(Clean(html)).Body!;
        foreach (var element in body.QuerySelectorAll("*"))
        {
            CnRichTextSanitizer.AllowedElements.ShouldContain(element.LocalName);
            foreach (var attribute in element.Attributes)
                attribute.Name.ShouldBeOneOf("href", "data-color");
        }

        body.TextContent.ShouldContain("a · b");
        body.TextContent.ShouldNotContain("h");
    }

    [Theory]
    [InlineData("<meta charset=\"utf-8\"><b style=\"font-weight:normal;\"><p><span style=\"font-weight:700;color:#cc0000\">T</span></p><ol><li>a<ul><li>b</li></ul></li></ol></b>")]
    [InlineData("Hallo<div>Wereld <span style=\"font-style:italic;text-decoration:underline\">x</span></div>")]
    [InlineData("<p>a&nbsp;&nbsp;b<br><br></p><h1>k</h1>")]
    public void Sanitising_IsStable(string html)
    {
        var once = Clean(html);
        Clean(once).ShouldBe(once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("<p> </p><div><br></div>")]
    public void Empty_GivesEmpty(string? html) => CnRichTextSanitizer.Sanitize(html).ShouldBe(string.Empty);
}
