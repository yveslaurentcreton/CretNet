namespace CretNet.Platform.Querying;

/// <summary>
/// Identifies a sort column and direction on an <see cref="IPagedQuery{TRow}"/>.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="Field"/> names a property on the row or entity being sorted.
/// The handler is responsible for mapping the field name onto the
/// underlying query — typically via a small switch or a registry that
/// constrains which fields are sortable.
/// </para>
/// <para>
/// The handler must reject unknown field names rather than silently
/// fall back to a default sort, so a typo in the UI surfaces as an
/// error instead of silently misordering rows.
/// </para>
/// </remarks>
public sealed record SortSpec(string Field, SortDirection Direction = SortDirection.Ascending);

public enum SortDirection
{
    Ascending,
    Descending,
}
