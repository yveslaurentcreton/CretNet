using System.Globalization;
using CretNet.Platform.Blazor.Ui.Components;
using CretNet.RichText;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class CnMarkdownTextCommandsTests
{
    private static CnTextEdit Apply(string text, int start, int end, CnMarkdownCommand command, CnPaletteColor? colour = null)
    {
        var original = CultureInfo.CurrentUICulture;
        try
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo("nl");
            return CnMarkdownTextCommands.Apply(text, new(start, end), command, colour).ShouldNotBeNull();
        }
        finally
        {
            CultureInfo.CurrentUICulture = original;
        }
    }

    [Fact]
    public void Bold_KeepsSelectedSpacesOutsideTheMarkers() =>
        Apply("a woord b", 2, 8, CnMarkdownCommand.Bold).ShouldBe(new CnTextEdit("a **woord** b", 4, 9));

    [Fact]
    public void Bold_OnCaret_InsertsASelectedPlaceholder() =>
        Apply("", 0, 0, CnMarkdownCommand.Bold).ShouldBe(new CnTextEdit("**vetgedrukt**", 2, 12));

    [Theory]
    [InlineData("**x**", 0, 5, CnMarkdownCommand.Bold, "x")]
    [InlineData("*x*", 1, 2, CnMarkdownCommand.Italic, "x")]
    [InlineData("***x***", 3, 4, CnMarkdownCommand.Bold, "*x*")]
    [InlineData("***x***", 3, 4, CnMarkdownCommand.Italic, "**x**")]
    [InlineData("**x**", 2, 3, CnMarkdownCommand.Italic, "***x***")]
    public void Emphasis_TogglesWithoutConfusingBoldAndItalic(string text, int start, int end, CnMarkdownCommand command, string expected) =>
        Apply(text, start, end, command).Text.ShouldBe(expected);

    [Fact]
    public void BulletList_PrefixesEverySelectedLineAndTogglesOff()
    {
        var on = Apply("een\n\ntwee\ndrie", 0, 13, CnMarkdownCommand.BulletList);
        on.Text.ShouldBe("- een\n\n- twee\n- drie");
        Apply(on.Text, on.SelectionStart, on.SelectionEnd, CnMarkdownCommand.BulletList).Text.ShouldBe("een\n\ntwee\ndrie");
    }

    [Fact]
    public void NumberedList_ReplacesBullets() =>
        Apply("- een\n- twee", 0, 12, CnMarkdownCommand.NumberedList).Text.ShouldBe("1. een\n2. twee");

    [Fact]
    public void Heading_ReplacesAnotherLevelAndTogglesOff()
    {
        Apply("### Titel", 0, 0, CnMarkdownCommand.Heading).Text.ShouldBe("## Titel");
        Apply("## Titel", 0, 0, CnMarkdownCommand.Heading).Text.ShouldBe("Titel");
    }

    [Fact]
    public void LineCommands_OnlyTouchTheCurrentLine() =>
        Apply("een\ntwee\ndrie", 6, 6, CnMarkdownCommand.Subheading).Text.ShouldBe("een\n### twee\ndrie");

    [Fact]
    public void Link_WithSelectedUrl_UsesItAsTarget() =>
        Apply("zie https://gyves.be", 4, 20, CnMarkdownCommand.Link).Text.ShouldBe("zie [https://gyves.be](https://gyves.be)");

    [Fact]
    public void Colour_RecoloursInsteadOfNesting()
    {
        Apply("[x]{.rood}", 0, 10, CnMarkdownCommand.Colour, CnMarkdownPalette.Blue).Text.ShouldBe("[x]{.blauw}");
        Apply("[x]{.rood}", 1, 2, CnMarkdownCommand.Colour, CnMarkdownPalette.Accent).Text.ShouldBe("[x]{.accent}");
    }

    [Fact]
    public void Clear_WithoutSelection_IsNotApplicable() =>
        CnMarkdownTextCommands.Apply("**x**", new(1, 1), CnMarkdownCommand.ClearFormatting).ShouldBeNull();

    [Fact]
    public void Clear_KeepsIntrawordUnderscores() =>
        Apply("snake_case _x_ ~~weg~~", 0, 22, CnMarkdownCommand.ClearFormatting).Text.ShouldBe("snake_case x weg");

    [Theory]
    [InlineData("Intro", 5, "- a\n- b", "Intro\n\n- a\n- b\n")]
    [InlineData("Intro\n", 6, "## Kop", "Intro\n\n## Kop\n")]
    [InlineData("a\n\nb", 1, "- x", "a\n\n- x\n\nb")]
    [InlineData("", 0, "- x", "- x\n")]
    [InlineData("ab", 1, "**x**", "a**x**b")]
    public void Insert_GluesBlocksButNotInlineText(string text, int at, string markdown, string expected) =>
        CnMarkdownTextCommands.Insert(text, new(at, at), markdown).Text.ShouldBe(expected);

    [Fact]
    public void Selection_IsClampedToTheText() =>
        Apply("ab", 5, -3, CnMarkdownCommand.Bold).Text.ShouldBe("**ab**");
}
