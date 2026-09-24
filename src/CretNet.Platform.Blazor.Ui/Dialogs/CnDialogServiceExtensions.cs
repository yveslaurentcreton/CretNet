using CretNet.Platform.Blazor.Ui.Components;

namespace CretNet.Platform.Blazor.Ui.Dialogs;

public static class CnDialogServiceExtensions
{
    /// <summary>Yes/No confirmation on the Cn dialog stack; false on cancel.
    /// Optional yes/no labels override the CretNet resources; <paramref name="destructive"/>
    /// renders the yes button as a red danger button.</summary>
    public static Task<bool> ConfirmAsync(this CnDialogService service, string title, string message, string? yesLabel = null, string? noLabel = null, bool destructive = false)
    {
        var parameters = new Dictionary<string, object> { [nameof(CnConfirmDialog.Message)] = message };
        if (yesLabel is not null)
            parameters[nameof(CnConfirmDialog.YesLabel)] = yesLabel;
        if (noLabel is not null)
            parameters[nameof(CnConfirmDialog.NoLabel)] = noLabel;
        if (destructive)
            parameters[nameof(CnConfirmDialog.Destructive)] = true;

        return service.ShowAsync<CnConfirmDialog, bool>(title, parameters, width: "440px");
    }
}
