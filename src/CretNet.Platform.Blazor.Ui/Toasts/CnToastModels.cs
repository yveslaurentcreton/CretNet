namespace CretNet.Platform.Blazor.Ui.Toasts;

public enum CnToastSeverity
{
    Information,
    Success,
    Warning,
    Error,
}

/// <summary>Where the host anchors its stack. The newest toast is always the
/// one nearest the chosen corner, whichever that is.</summary>
public enum CnToastPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
}

/// <summary>
/// One toast on the stack.
/// </summary>
/// <param name="Duration">How long it stays once it is actually on screen.
/// A toast waiting behind the visible cap has not started counting yet —
/// otherwise a burst expires things nobody ever saw.</param>
/// <param name="ActionLabel">Optional single action ("Undo", "Open"). One,
/// deliberately: a toast that needs a choice is a dialog wearing the wrong
/// clothes.</param>
public sealed record CnToastItem(
    Guid Id,
    CnToastSeverity Severity,
    string Title,
    string? Message,
    TimeSpan Duration,
    string? ActionLabel = null,
    Func<Task>? Action = null);
