namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Row shape returned by a <see cref="CnPicker"/>'s search Provider.
/// Rows optionally carry a secondary context line (e.g. a task's
/// "project · customer"), a right-hand meta badge (e.g. a status) and a
/// derived-recency flag the picker groups on when the query is empty.
/// Absent context renders nothing.
/// </summary>
public record CnPickerItem(
    Guid Id,
    string Label,
    string? Context = null,
    string? Meta = null,
    bool MetaAccent = false,
    bool Recent = false);
