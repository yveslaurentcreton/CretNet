using CretNet.Platform.Blazor.Ui.Dialogs;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnPickerGridDialog<TRow>
{
    [CascadingParameter] public CnDialogInstance Dialog { get; set; } = default!;

    /// <summary>Server-paged provider, wired exactly like any other CnDataGrid.</summary>
    [Parameter, EditorRequired] public Func<CnGridRequest, Task<CnGridPage<TRow>>> Provider { get; set; } = default!;

    /// <summary>The entity's <c>&lt;CnGridColumn TItem="TRow"&gt;</c> declarations.
    /// Passing the same fragment the entity's own grid renders is the point:
    /// the search dialog then cannot drift from the list people already know.</summary>
    [Parameter, EditorRequired] public RenderFragment Columns { get; set; } = default!;

    /// <summary>Optional filter row above the grid (status chips and the like).</summary>
    [Parameter] public RenderFragment? FilterContent { get; set; }

    [Parameter] public int PageSize { get; set; } = 10;

    // Resource-backed defaults; hosts may override wording with their own resources.
    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    [Parameter] public string SearchPlaceholder { get => field ?? CnLabels.Search; set; } = null!;
    [Parameter] public string PreviousPageTitle { get => field ?? CnLabels.MoveLeft; set; } = null!;
    [Parameter] public string NextPageTitle { get => field ?? CnLabels.MoveRight; set; } = null!;
    [Parameter] public string EmptyText { get => field ?? CnLabels.NothingFound; set; } = null!;
    #pragma warning restore BL0007

    private void OnRowClicked(TRow row) => Dialog.Close(row);
}
