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

    // Chrome strings as parameters with English defaults — the RCL carries no
    // resource dependency; hosts localise by passing their own labels.
    [Parameter] public string SearchPlaceholder { get; set; } = "Search";
    [Parameter] public string PreviousPageTitle { get; set; } = "Move left";
    [Parameter] public string NextPageTitle { get; set; } = "Move right";
    [Parameter] public string EmptyText { get; set; } = "Nothing found";

    private void OnRowClicked(TRow row) => Dialog.Close(row);
}
