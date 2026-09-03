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
/// Labels follow the coupling-cut rule: English defaults, overridden by the
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

    [Parameter] public bool CanSave { get; set; } = true;
    [Parameter] public bool Saving { get; set; }

    [Parameter] public string SaveLabel { get; set; } = "{0:Saving|Save}";
    [Parameter] public string SaveAndOpenLabel { get; set; } = "Save & open";
    [Parameter] public string CancelLabel { get; set; } = "Cancel";
    [Parameter] public string? Class { get; set; }
}
