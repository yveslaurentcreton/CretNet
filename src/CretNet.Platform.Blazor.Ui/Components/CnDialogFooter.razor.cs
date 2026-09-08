using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// The footer of a create/edit dialog: cancel, save, and — when the opener
/// wires it — save and open.
/// </summary>
/// <remarks>
/// <para>
/// "Save &amp; open" is not a second dialog and not a setting: it is a
/// callback the opener either provides or does not. A dialog for something
/// with a page of its own gets the third button; a dialog for something
/// without one never shows it, and the dialog content does not know the
/// difference.
/// </para>
/// <para>
/// Labels default to CretNet resources and may be overridden by the
/// consuming application. They go through <see cref="CnButton"/>'s busy
/// convention, so <c>{0:Saving|Save}</c> works here as anywhere.
/// </para>
/// </remarks>
public partial class CnDialogFooter
{
    [Parameter, EditorRequired] public EventCallback OnSave { get; set; }
    [Parameter, EditorRequired] public EventCallback OnCancel { get; set; }

    /// <summary>Absent, the middle button is not rendered at all.</summary>
    [Parameter] public EventCallback OnSaveAndOpen { get; set; }

    /// <summary>
    /// Hides the middle button while the callback stays wired — for a dialog
    /// that creates and edits with the same markup and only offers "open"
    /// on create.
    /// </summary>
    [Parameter] public bool ShowSaveAndOpen { get; set; } = true;

    [Parameter] public bool CanSave { get; set; } = true;
    [Parameter] public bool Saving { get; set; }

    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    [Parameter] public string SaveLabel { get => field ?? CnLabels.Save; set; } = null!;
    [Parameter] public string SaveAndOpenLabel { get => field ?? CnLabels.SaveOpen; set; } = null!;
    [Parameter] public string CancelLabel { get => field ?? CnLabels.Cancel; set; } = null!;
    #pragma warning restore BL0007
    [Parameter] public string? Class { get; set; }
}
