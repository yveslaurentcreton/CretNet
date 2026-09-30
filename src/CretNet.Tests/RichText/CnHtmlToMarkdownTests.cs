using AngleSharp.Html.Parser;
using CretNet.RichText;
using Shouldly;

namespace CretNet.Tests.RichText;

public class CnHtmlToMarkdownTests
{
    // Word (web/online and older builds) puts real lists on the clipboard.
    private const string Word =
        "<html xmlns:o=\"urn:schemas-microsoft-com:office:office\"><body>" +
        "<p class=MsoNormal><b><span style=\"font-size:12pt;font-family:Calibri\">Overdracht</span></b><o:p></o:p></p>" +
        "<p class=MsoNormal><span style=\"font-family:Calibri\">Na de installatie krijgt u een <u>korte opleiding</u> en een overzicht van alle <span style=\"color:#00B050\">wachtwoorden in de kluis</span>.</span></p>" +
        "<ul><li><span style=\"font-family:Calibri\">Documentatie van het netwerk</span></li><li><span style=\"font-family:Calibri\">Contactpersoon voor <i>support</i></span><ul><li>tijdens de kantooruren</li></ul></li></ul></body></html>";

    // Desktop Word pastes lists as mso-list paragraphs with a hidden marker.
    private const string WordDesktop =
        "<html><head><style>p.MsoNormal{margin:0}</style><!--[if gte mso 9]><xml><o:OfficeDocumentSettings/></xml><![endif]--></head><body lang=NL-BE>" +
        "<!--StartFragment--><p class=MsoNormal><b><span lang=NL-BE>Planning<o:p></o:p></span></b></p>" +
        "<p class=MsoListParagraphCxSpFirst style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span style='font-family:Symbol'><span style='mso-list:Ignore'>·<span style='font:7.0pt \"Times New Roman\"'>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; </span></span></span><![endif]><span lang=NL-BE>Netwerk<o:p></o:p></span></p>" +
        "<p class=MsoListParagraphCxSpMiddle style='margin-left:72.0pt;mso-add-space:auto;text-indent:-18.0pt;mso-list:l0 level2 lfo1'><![if !supportLists]><span style='font-family:\"Courier New\"'><span style='mso-list:Ignore'>o<span style='font:7.0pt \"Times New Roman\"'>&nbsp;&nbsp; </span></span></span><![endif]><span lang=NL-BE>Firewall met <b>gescheiden</b> netwerken<o:p></o:p></span></p>" +
        "<p class=MsoListParagraphCxSpLast style='text-indent:-18.0pt;mso-list:l0 level1 lfo1'><![if !supportLists]><span style='font-family:Symbol'><span style='mso-list:Ignore'>·<span>&nbsp; </span></span></span><![endif]><span lang=NL-BE>Werkplekken<o:p></o:p></span></p>" +
        "<p class=MsoNormal><span lang=NL-BE style='color:#C00000'>Let op</span><span lang=NL-BE>: prijzen <i>exclusief</i> btw.<o:p></o:p></span></p>" +
        "<p class=MsoListParagraphCxSpFirst style='mso-list:l1 level1 lfo2'><![if !supportLists]><span><span style='mso-list:Ignore'>1.<span>&nbsp;&nbsp; </span></span></span><![endif]>Bestellen</p>" +
        "<p class=MsoListParagraphCxSpLast style='mso-list:l1 level1 lfo2'><![if !supportLists]><span><span style='mso-list:Ignore'>2.<span>&nbsp;&nbsp; </span></span></span><![endif]>Leveren</p>" +
        "<!--EndFragment--></body></html>";

    private const string GoogleDocs =
        "<meta charset=\"utf-8\"><b style=\"font-weight:normal;\" id=\"docs-internal-guid-1\"><p dir=\"ltr\"><span style=\"font-size:11pt;font-weight:700;\">Voorwaarden</span></p>" +
        "<p dir=\"ltr\"><span style=\"font-size:11pt;font-weight:400;\">Betaling binnen 30 dagen. </span><span style=\"font-size:11pt;font-weight:400;color:#cc0000;\">Voorschot van 30 % bij bestelling.</span></p>" +
        "<ol><li dir=\"ltr\"><span style=\"font-size:11pt;\">Levering na ontvangst voorschot</span></li><li dir=\"ltr\"><span style=\"font-size:11pt;background-color:#ffff00;\">Installatie op afspraak</span></li></ol></b>";

    private static string Markdown(string html) => CnHtmlToMarkdown.Convert(html).Markdown;

    [Fact]
    public void Word_KeepsBoldColourAndNestedLists()
    {
        var result = CnHtmlToMarkdown.Convert(Word);
        result.Markdown.ShouldBe(
            "**Overdracht**\n\n" +
            "Na de installatie krijgt u een korte opleiding en een overzicht van alle [wachtwoorden in de kluis]{.accent}.\n\n" +
            "- Documentatie van het netwerk\n" +
            "- Contactpersoon voor *support*\n" +
            "  - tijdens de kantooruren");
        result.Dropped.ShouldBe([CnDroppedFormatting.FontFamilyOrSize, CnDroppedFormatting.Underline]);
    }

    [Fact]
    public void WordDesktop_ListParagraphsBecomeNestedLists()
    {
        var result = CnHtmlToMarkdown.Convert(WordDesktop);
        result.Markdown.ShouldBe(
            "**Planning**\n\n" +
            "- Netwerk\n" +
            "  - Firewall met **gescheiden** netwerken\n" +
            "- Werkplekken\n\n" +
            "[Let op]{.rood}: prijzen *exclusief* btw.\n\n" +
            "1. Bestellen\n" +
            "2. Leveren");
        result.Markdown.ShouldNotContain("·");
        result.Markdown.ShouldNotContain("StartFragment");
        result.Markdown.ShouldNotContain("MsoNormal");
    }

    [Fact]
    public void GoogleDocs_WrapperIsNotBoldAndStyleBoldIs()
    {
        var result = CnHtmlToMarkdown.Convert(GoogleDocs);
        result.Markdown.ShouldBe(
            "**Voorwaarden**\n\n" +
            "Betaling binnen 30 dagen. [Voorschot van 30 % bij bestelling.]{.rood}\n\n" +
            "1. Levering na ontvangst voorschot\n" +
            "2. Installatie op afspraak");
        result.Dropped.ShouldBe([CnDroppedFormatting.FontFamilyOrSize, CnDroppedFormatting.BackgroundColor]);
    }

    [Fact]
    public void ConvertedSamples_PreviewMatchesTheSource()
    {
        var word = Preview(Markdown(Word));
        word.QuerySelector("p > strong")!.TextContent.ShouldBe("Overdracht");
        word.QuerySelector("span.cn-md-c-accent")!.TextContent.ShouldBe("wachtwoorden in de kluis");
        word.QuerySelector("ul > li > ul > li")!.TextContent.ShouldBe("tijdens de kantooruren");

        var desktop = Preview(Markdown(WordDesktop));
        desktop.QuerySelector("ul > li > ul > li")!.TextContent.ShouldBe("Firewall met gescheiden netwerken");
        desktop.QuerySelectorAll("ol > li").Select(li => li.TextContent).ShouldBe(["Bestellen", "Leveren"]);

        var docs = Preview(Markdown(GoogleDocs));
        docs.QuerySelectorAll("strong").Select(b => b.TextContent).ShouldBe(["Voorwaarden"]);
        docs.QuerySelectorAll("ol > li").Length.ShouldBe(2);
    }

    [Fact]
    public void LooseFirstLine_BecomesItsOwnParagraph() =>
        Markdown("Hallo<div>Beste klant,</div><div><br></div><div>Tot snel</div>").ShouldBe("Hallo\n\nBeste klant,\n\nTot snel");

    [Fact]
    public void TopLevelBoldSubtitle_StaysBold() =>
        Markdown("<b>Subtitel</b><div>De tekst eronder.</div>").ShouldBe("**Subtitel**\n\nDe tekst eronder.");

    [Fact]
    public void NestedDivsWithSeveralParagraphs_KeepEveryParagraph() =>
        Markdown("<div><div><p>Een</p><p>Twee <em>schuin</em></p></div><div><p>Drie</p></div></div><p>Vier</p>")
            .ShouldBe("Een\n\nTwee *schuin*\n\nDrie\n\nVier");

    [Fact]
    public void NestedLists_IndentUnderTheParentMarker()
    {
        var markdown = Markdown("<ol><li>A<ol><li>A1</li><li>A2<ul><li>diep</li></ul></li></ol></li><li>B</li></ol>");
        markdown.ShouldBe("1. A\n   1. A1\n   2. A2\n      - diep\n2. B");
        Preview(markdown).QuerySelector("ol > li > ol > li > ul > li")!.TextContent.ShouldBe("diep");
    }

    [Fact]
    public void BulletLists_NestWithTwoSpaces() =>
        Markdown("<ul><li>a<ul><li>b<ul><li>c</li></ul></li></ul></li></ul>").ShouldBe("- a\n  - b\n    - c");

    [Fact]
    public void ListDirectlyInsideList_BelongsToThePreviousItem() =>
        Markdown("<ul><li>x</li><ul><li>y</li></ul></ul>").ShouldBe("- x\n  - y");

    [Fact]
    public void ListItemParagraphs_StayInTheItem()
    {
        var markdown = Markdown("<ul><li><p>Eerste</p><p>vervolg</p></li><li>Tweede</li></ul>");
        markdown.ShouldBe("- Eerste  \n  vervolg\n- Tweede");
        Preview(markdown).QuerySelectorAll("ul > li").Length.ShouldBe(2);
    }

    [Fact]
    public void OrderedListStart_IsKept() =>
        Markdown("<ol start=\"3\"><li>drie</li><li>vier</li></ol>").ShouldBe("3. drie\n4. vier");

    [Theory]
    [InlineData("<h1>Titel</h1>", "## Titel")]
    [InlineData("<h2><b>Titel</b></h2>", "## Titel")]
    [InlineData("<h3>Sub</h3>", "### Sub")]
    [InlineData("<h5><strong>Klein</strong> kop</h5>", "### Klein kop")]
    [InlineData("<h2><span style=\"color:#3159a7\">Blauw</span></h2>", "## [Blauw]{.blauw}")]
    public void Headings_MapToTwoLevelsWithoutInnerBold(string html, string expected) =>
        Markdown(html).ShouldBe(expected);

    [Theory]
    [InlineData("<span style=\"font-weight:bold\">x</span>", "**x**")]
    [InlineData("<span style=\"font-weight:600\">x</span>", "**x**")]
    [InlineData("<span style=\"font-weight:500\">x</span>", "x")]
    [InlineData("<b style=\"font-weight:400\">x</b>", "x")]
    [InlineData("<span style=\"mso-bidi-font-weight:bold\">x</span>", "x")]
    [InlineData("<span style=\"font-style:italic\">x</span>", "*x*")]
    [InlineData("<i style=\"font-style:normal\">x</i>", "x")]
    [InlineData("<b><i>x</i></b>", "***x***")]
    [InlineData("<s>weg</s>", "~~weg~~")]
    [InlineData("a<b> vet </b>b", "a **vet** b")]
    public void InlineFormatting_FromTagsAndStyles(string html, string expected) =>
        Markdown(html).ShouldBe(expected);

    [Theory]
    [InlineData("#00B050", "accent")]
    [InlineData("#cc0000", "rood")]
    [InlineData("rgb(49, 89, 167)", "blauw")]
    [InlineData("blue", "blauw")]
    [InlineData("#e69138", "oranje")]
    public void Colours_MapToTheNearestPaletteColour(string css, string palette) =>
        Markdown($"<span style=\"color:{css}\">x</span>").ShouldBe($"[x]{{.{palette}}}");

    [Theory]
    [InlineData("#000000")]
    [InlineData("#777777")]
    [InlineData("#ffffff")]
    [InlineData("windowtext")]
    [InlineData("rgba(255, 0, 0, 0)")]
    public void GreysBlackAndWhite_AreNoColour(string css) =>
        Markdown($"<span style=\"color:{css}\">x</span>").ShouldBe("x");

    [Fact]
    public void FontColourAttribute_IsAColour() =>
        Markdown("<font color=\"#c00000\">x</font>").ShouldBe("[x]{.rood}");

    [Fact]
    public void BlackInsideAColour_EndsTheColourWhereMarkdownCan() =>
        Markdown("<p style=\"color:#c00000\">rood <span style=\"color:black\">zwart</span></p>").ShouldBe("[rood zwart]{.rood}");

    [Fact]
    public void Links_KeepSafeSchemesOnly()
    {
        var result = CnHtmlToMarkdown.Convert(
            "<p><a href=\"https://gyves.be/a b\">site</a>, <a href=\"mailto:info@gyves.be\">mail</a>, " +
            "<a href=\"javascript:alert(1)\">kwaad</a>, <a href=\"#anker\">anker</a>, <a href=\"/pad\">relatief</a></p>");
        result.Markdown.ShouldBe("[site](https://gyves.be/a%20b), [mail](mailto:info@gyves.be), kwaad, anker, relatief");
        result.Dropped.ShouldBe([CnDroppedFormatting.UnsupportedLink]);
    }

    [Fact]
    public void Tables_BecomePipeTables()
    {
        var markdown = Markdown("<table><tr><th>Fase</th><th>Wanneer</th></tr><tr><td><p class=MsoNormal>Net|werk</p></td><td>week <b>44</b></td></tr><tr><td>Los</td></tr></table>");
        markdown.ShouldBe("| Fase | Wanneer |\n| --- | --- |\n| Net\\|werk | week **44** |\n| Los |  |");
        var table = Preview(markdown);
        table.QuerySelectorAll("tbody tr").Length.ShouldBe(2);
        table.QuerySelector("td")!.TextContent.ShouldBe("Net|werk");
    }

    [Fact]
    public void ScriptsStylesAndOfficeTags_AreDropped()
    {
        var result = CnHtmlToMarkdown.Convert("<style>p{color:red}</style><script>alert(1)</script><p>Tekst<o:p>&nbsp;</o:p></p><img src=x onerror=alert(1)><noscript>nee</noscript>");
        result.Markdown.ShouldBe("Tekst");
        result.Dropped.ShouldBe([CnDroppedFormatting.Image]);
    }

    [Fact]
    public void NonBreakingSpaces_BecomeSpaces() =>
        Markdown("<p>a&nbsp;&nbsp;b&nbsp;c</p>").ShouldBe("a b c");

    [Fact]
    public void LineBreaks_BecomeHardBreaks() =>
        Markdown("<p>regel<br>volgende<br></p>").ShouldBe("regel  \nvolgende");

    [Fact]
    public void TextThatLooksLikeMarkdown_IsEscaped()
    {
        const string html = "<p>1. geen lijst &amp; *geen* nadruk [x]{.accent} # geen kop &lt;b&gt;x&lt;/b&gt; snake_case _a_ &amp;copy;</p><p>- geen opsomming</p><p># geen kop</p><p>---</p>";
        var markdown = Markdown(html);
        var body = Preview(markdown);
        body.QuerySelectorAll("ol, ul, h2, h3, em, strong, span, b, hr").ShouldBeEmpty();
        body.QuerySelectorAll("p").Select(p => p.TextContent).ShouldBe(
        [
            "1. geen lijst & *geen* nadruk [x]{.accent} # geen kop <b>x</b> snake_case _a_ &copy;",
            "- geen opsomming",
            "# geen kop",
            "---",
        ]);
    }

    [Fact]
    public void LegacyEditorHtml_Converts()
    {
        Markdown("<p>Vrije <b>opmaak</b>.</p><h3>Planning</h3><ul><li>Een</li><li>Twee</li></ul><div>Chrome-regel</div>")
            .ShouldBe("Vrije **opmaak**.\n\n### Planning\n\n- Een\n- Twee\n\nChrome-regel");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("<p> </p><div><br></div>")]
    public void Empty_GivesNoMarkdown(string? html)
    {
        var result = CnHtmlToMarkdown.Convert(html);
        result.Markdown.ShouldBe(string.Empty);
        result.Dropped.ShouldBeEmpty();
    }

    [Fact]
    public void RenderedMarkdown_ConvertsBackToTheSameMarkdown()
    {
        const string markdown = "**Situatie**\n\n## Wat we voorstellen\n\n- Firewall\n  - kantoor\n  - gasten\n- [Access points]{.accent}\n\n1. Een\n2. Twee *schuin*\n\n| Fase | Wanneer |\n| --- | --- |\n| Netwerk | week 44 |\n\nMeer: [gyves.be](https://gyves.be)";
        var html = CnMarkdown.ToSafeHtml(markdown).Replace("class=\"cn-md-c-accent\"", "style=\"color:#128a30\"");
        Markdown(html).ShouldBe(markdown);
    }

    private static AngleSharp.Dom.IElement Preview(string markdown) =>
        new HtmlParser().ParseDocument(CnMarkdown.ToSafeHtml(markdown)).Body!;
}
