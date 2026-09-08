using System.Text.RegularExpressions;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class PickerWithAddAlignmentContractTests
{
    [Fact]
    public void PickerCreateAction_UsesTheSameHeightAsTheFieldControl()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));
        var inputHeight = ReadPixelProperty(css, @"\.cn-input\s*\{", "height");
        var pickerActionHeight = ReadPixelProperty(css, @"\.cn-picker-with-add\s*>\s*\.cn-btn--icononly\s*\{", "height");

        pickerActionHeight.ShouldBe(inputHeight);
    }

    private static int ReadPixelProperty(string css, string selectorPattern, string property)
    {
        var match = Regex.Match(
            css,
            $@"{selectorPattern}(?<body>[^}}]*)}}",
            RegexOptions.CultureInvariant);

        match.Success.ShouldBeTrue($"CSS selector '{selectorPattern}' should exist.");

        var propertyMatch = Regex.Match(
            match.Groups["body"].Value,
            $@"\b{Regex.Escape(property)}\s*:\s*(?<value>\d+)px\s*;",
            RegexOptions.CultureInvariant);

        propertyMatch.Success.ShouldBeTrue($"CSS property '{property}' should use a pixel value.");
        return int.Parse(propertyMatch.Groups["value"].Value, System.Globalization.CultureInfo.InvariantCulture);
    }

}
