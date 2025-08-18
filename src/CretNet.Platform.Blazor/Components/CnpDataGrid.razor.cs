using CretNet.Platform.Data;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpDataGrid<TGridItem> : CnpDataGridBase<TGridItem>
    where TGridItem : IIdentity
{
    private PaginationState _pagination = new() { ItemsPerPage = 10 };
}