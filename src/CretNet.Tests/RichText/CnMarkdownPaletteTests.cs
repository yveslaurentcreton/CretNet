using CretNet.RichText;
using Shouldly;

namespace CretNet.Tests.RichText;

public class CnMarkdownPaletteTests
{
    [Fact]
    public void Palette_HasTheFourDocumentColours()
    {
        CnMarkdownPalette.Colors.Select(color => color.Name).ShouldBe(["accent", "blauw", "oranje", "rood"]);
        CnMarkdownPalette.Colors.Select(color => color.PrintHex).ShouldBe(["#128a30", "#3159a7", "#b35f00", "#c42b2f"]);
        CnMarkdownPalette.Accent.CssClass.ShouldBe("cn-md-c-accent");
        CnMarkdownPalette.Accent.CssVariable.ShouldBe("--cn-md-accent");
    }

    [Theory]
    [InlineData("accent", "accent")]
    [InlineData("ROOD", "rood")]
    [InlineData("groen", null)]
    [InlineData(null, null)]
    public void Find_ByStoredName(string? name, string? expected) =>
        (CnMarkdownPalette.Find(name)?.Name).ShouldBe(expected);

    [Theory]
    [InlineData("#00B050", "accent")]
    [InlineData("#0f0", "accent")]
    [InlineData("rgb(204, 0, 0)", "rood")]
    [InlineData("rgba(49,89,167,.5)", "blauw")]
    [InlineData("rgb(255 153 0)", "oranje")]
    [InlineData("navy", "blauw")]
    [InlineData("#C00000 !important", "rood")]
    [InlineData("#ffff00", "oranje")]
    public void Nearest_MapsColoursToThePalette(string css, string expected) =>
        CnMarkdownPalette.Nearest(css)!.Name.ShouldBe(expected);

    [Theory]
    [InlineData("#000")]
    [InlineData("#1b1b1b")]
    [InlineData("gray")]
    [InlineData("white")]
    [InlineData("transparent")]
    [InlineData("#ff000000")]
    [InlineData("var(--x)")]
    [InlineData("")]
    public void Nearest_IsNullForNoColour(string css) =>
        CnMarkdownPalette.Nearest(css).ShouldBeNull();
}
