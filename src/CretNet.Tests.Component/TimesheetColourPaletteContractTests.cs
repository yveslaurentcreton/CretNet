using System.Text.RegularExpressions;
using Shouldly;

namespace CretNet.Tests.Component;

public sealed class TimesheetColourPaletteContractTests
{
    private static readonly IReadOnlyDictionary<string, string> LightPalette =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["blue"] = "#3f51b5",
            ["purple"] = "#8e24aa",
            ["teal"] = "#33b679",
            ["pink"] = "#e67c73",
            ["orange"] = "#f4511e",
            ["sky"] = "#039be5",
            ["yellow"] = "#f6bf26",
            ["slate"] = "#616161",
        };

    private static readonly IReadOnlyDictionary<string, string> DarkPalette =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["blue"] = "#7986cb",
            ["purple"] = "#b39ddb",
            ["teal"] = "#57bb8a",
            ["pink"] = "#ef9a9a",
            ["orange"] = "#ff8a65",
            ["sky"] = "#4fc3f7",
            ["yellow"] = "#ffd54f",
            ["slate"] = "#bdbdbd",
        };

    [Fact]
    public void TimesheetTags_UseTheApprovedGoogleCalendarPaletteInBothThemes()
    {
        var css = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cn-ui.css"));

        foreach (var (name, colour) in LightPalette)
            TokenOccurrences(css, name, colour).ShouldBe(1, $"Light token '{name}' should occur once.");

        foreach (var (name, colour) in DarkPalette)
            TokenOccurrences(css, name, colour).ShouldBe(2, $"Dark token '{name}' should cover system and explicit dark themes.");
    }

    private static int TokenOccurrences(string css, string name, string colour) =>
        Regex.Matches(
            css,
            $@"--cn-tag-{Regex.Escape(name)}\s*:\s*{Regex.Escape(colour)}\s*;",
            RegexOptions.CultureInvariant | RegexOptions.IgnoreCase).Count;

}
