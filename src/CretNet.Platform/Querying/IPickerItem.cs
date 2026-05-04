namespace CretNet.Platform.Querying;

/// <summary>
/// Minimum shape a row type must satisfy to be rendered by
/// <c>CnpEntityPicker&lt;TItem, TId&gt;</c>. Constrains TItem to a stable
/// identifier and a user-visible label so the picker can render and
/// resolve selections without per-item callbacks at the call site.
/// </summary>
public interface IPickerItem<out TId>
{
    TId Id { get; }
    string DisplayLabel { get; }
}
