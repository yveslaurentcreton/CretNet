namespace CretNet.Platform.Blazor.Ui.Dialogs;

/// <summary>
/// What a create/edit dialog hands back: the saved thing, and whether the
/// user asked to go there next.
/// </summary>
/// <remarks>
/// The opener decides what "open" means — it knows the route, the dialog
/// does not. Keeping the flag beside the value rather than as a second
/// dialog result type means one <c>ShowAsync</c> call and one null check,
/// whichever button was pressed.
/// </remarks>
public sealed record CnDialogResult<T>(T Value, bool OpenAfter)
{
    public static CnDialogResult<T> Saved(T value) => new(value, OpenAfter: false);

    public static CnDialogResult<T> SavedAndOpen(T value) => new(value, OpenAfter: true);
}
