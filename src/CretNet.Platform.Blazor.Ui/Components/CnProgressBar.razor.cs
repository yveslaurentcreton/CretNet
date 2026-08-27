using System.Globalization;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// How much of a budget is used, and how far past it.
/// </summary>
/// <remarks>
/// <para>
/// The budget line sits at 70&#160;% of the track rather than at its end.
/// A bar that fills to the edge at 100&#160;% has nowhere left to show an
/// overrun, so the two states that matter most — "just finished" and "well
/// over" — look identical. With the line at 70&#160;% the track keeps room
/// for roughly 40&#160;% of overrun on the same scale, and anything beyond
/// that is clamped rather than allowed to escape the container.
/// </para>
/// <para>
/// It measures; it does not judge. Whether an overrun is bad is the
/// caller's business — this component only draws the part past the line in
/// the danger colour because that is what "past the line" means.
/// </para>
/// </remarks>
public partial class CnProgressBar
{
    /// <summary>What has been used. Negative values are treated as zero.</summary>
    [Parameter, EditorRequired] public double Value { get; set; }

    /// <summary>What was budgeted. Zero or less leaves the bar empty:
    /// without a budget there is nothing to be a percentage of.</summary>
    [Parameter, EditorRequired] public double Maximum { get; set; }

    [Parameter] public bool Large { get; set; }

    /// <summary>Screen-reader label. English default, per the coupling-cut
    /// rule — the consuming application supplies its own wording.</summary>
    [Parameter] public string AriaLabel { get; set; } = "Progress";

    [Parameter] public string? Title { get; set; }
    [Parameter] public string? Class { get; set; }

    private double Percentage =>
        Maximum > 0d ? Math.Max(0d, Value) / Maximum * 100d : 0d;

    private string? SizeClass => Large ? "cn-bar--lg" : null;

    private static string Css(double value) =>
        value.ToString("0.###", CultureInfo.InvariantCulture);
}
