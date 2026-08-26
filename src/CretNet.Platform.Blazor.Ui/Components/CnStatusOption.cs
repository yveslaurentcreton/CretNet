namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// How much attention a status deserves. Neutral is the resting state; the
/// other three borrow the theme's semantic colours.
/// </summary>
public enum CnStatusTone
{
    Neutral,
    Accent,
    Warning,
    Danger,
}

/// <summary>
/// One domain-approved destination rendered by <see cref="CnStatusPicker{TStatus}"/>.
/// The owning screen decides which transitions are valid; the picker owns only
/// their shared interaction and presentation.
/// </summary>
public sealed record CnStatusOption<TStatus>(
    TStatus Value,
    string Label,
    CnStatusTone Tone = CnStatusTone.Neutral,
    bool Disabled = false,
    string? Description = null);
