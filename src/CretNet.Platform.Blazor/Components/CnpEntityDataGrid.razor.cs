using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CretNet.Platform.Blazor.Interfaces;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Data;
using CretNet.Platform.Querying;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.FluentUI.AspNetCore.Components;
using QuerySortDirection = CretNet.Platform.Querying.SortDirection;
using FluentSortDirection = Microsoft.FluentUI.AspNetCore.Components.SortDirection;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpEntityDataGrid<TGridItem, TId> : CnpComponent
    where TGridItem : IIdentity<TId>
    where TId : notnull
{
    [CascadingParameter] public IHandleSelection? HandleSelection { get; set; }
    [Parameter] public EventCallback<IEnumerable<TGridItem>> SelectedItemsChanged { get; set; }
    [Parameter] public bool MultiSelect { get; set; }
    [Parameter] public bool ShowSelect { get; set; }
    [Parameter] public bool ShowAdd { get; set; }
    [Parameter] public bool EnableAdd { get; set; } = true;
    [Parameter] public bool ShowEdit { get; set; }
    [Parameter] public Func<TGridItem, bool> EnableEdit { get; set; } = _ => true;
    [Parameter] public bool ShowRemove { get; set; }
    [Parameter] public Func<IEnumerable<TGridItem>, bool> EnableRemove { get; set; } = _ => true;
    [Parameter] public bool ShowNavigate { get; set; }
    [Parameter] public RenderFragment<IEnumerable<TGridItem>>? PrimaryActions { get; set; }
    [Parameter] public RenderFragment<IEnumerable<TGridItem>>? SecondaryActions { get; set; }
    [Parameter] public RenderFragment<IEnumerable<TGridItem>>? CustomFilters { get; set; }
    [Parameter, EditorRequired] public RenderFragment Columns { get; set; } = default!;

    /// <summary>
    /// Optional content rendered when the fetch succeeded but returned no
    /// rows. Defaults to a simple "No results match your filters." message.
    /// </summary>
    [Parameter] public RenderFragment? EmptyContent { get; set; }

    /// <summary>
    /// Optional content rendered when <c>DataSource.LastError</c> is set.
    /// Receives the exception. Defaults to a FluentMessageBar with the
    /// exception message and a Retry button calling <c>DataSource.Reload</c>.
    /// </summary>
    [Parameter] public RenderFragment<Exception>? ErrorContent { get; set; }

    /// <summary>
    /// Optional content rendered inside a filter-button popover in the toolbar.
    /// Use for <c>CnpBindCheckboxGroup</c> components on a BackedBy grid —
    /// they live inside the popover, mutating the QueryState.
    /// When set, takes precedence over the legacy <c>EntityFilters</c>
    /// rendering (which uses <c>CnpFilterButton</c>).
    /// </summary>
    [Parameter] public RenderFragment? FilterControls { get; set; }

    private bool _filterControlsOpen;

    // Anchor id must be unique per grid instance — a page can render several
    // grids (e.g. one per tab on a detail page), and FluentPopover anchors to
    // the first element carrying the id, which may be a hidden one.
    private readonly string _filterControlsButtonId = Identifier.NewId();

    [Parameter] public bool IsPrimary { get; set; } = true;
    [Parameter] public Func<TGridItem, bool>? CustomFilterFunc { get; set; }
    [Parameter] public Func<object>? DependencyArgs { get; set; }
    /// <summary>
    /// Optional hook fired after <see cref="ICnpDataSource{TGridItem, TId}.Init"/>
    /// completes. Use this to attach a per-screen <c>QueryState&lt;TQuery&gt;</c>
    /// on a <c>BackedBy&lt;TQuery&gt;</c> data source — the page knows the query
    /// type, the grid does not.
    /// </summary>
    [Parameter] public Func<ICnpDataSource<TGridItem, TId>, Task>? AfterInit { get; set; }
    private readonly BehaviorSubject<int> _itemsPerPageSubject = new(DefaultItemsPerPage);
    [Parameter] public int ItemsPerPage
    {
        get => _itemsPerPageSubject.Value;
        set => _itemsPerPageSubject.OnNext(value);
    }
    [Inject] public ICnpDataSource<TGridItem, TId> DataSource { get; set; } = default!;
    [Inject] public ILogger<CnpEntityDataGrid<TGridItem, TId>> Logger { get; set; } = default!;

    private const int DefaultItemsPerPage = 15;

    private SelectColumn<TGridItem>? _selectColumn;
    private readonly CompositeDisposable _disposables = new();
    // Initialize PaginationState with the same default page size as the items-per-page subject, so
    // the first LoadServerPageAsync doesn't race against the async SetItemsPerPageAsync setup and
    // end up fetching with the PaginationState default (10) instead of our intended page size.
    private PaginationState _pagination = new() { ItemsPerPage = DefaultItemsPerPage };
    private CancellationTokenSource? _searchDebounce;
    private bool _isServerLoading;
    private int _lastLoadedPageIndex;

    // When server-paging is active we feed FluentDataGrid with the current page's items via Items,
    // but FluentDataGrid would otherwise overwrite Pagination.TotalItemCount with Items.Count()
    // (= page size). Providing a non-null RefreshItems callback disables that overwrite so our
    // explicit SetTotalItemCountAsync(DataSource.TotalCount) takes effect. The callback also
    // detects column-sort changes (FluentDataGrid signals sort via this callback) and folds them
    // into the QueryState in BackedBy mode.
    private Func<GridItemsProviderRequest<TGridItem>, Task>? _serverRefreshItems;

    // Last sort spec we observed on a RefreshItems request. Used to detect *changes* — the
    // callback is also fired on initial load and unrelated triggers, and we only want to push
    // an UpdateSort on actual user-driven sort changes.
    private (string? Column, bool Ascending)? _lastObservedSort;

    public string? Search { get; set; }

    protected IEnumerable<TGridItem> Items => DataSource.Entities ?? Enumerable.Empty<TGridItem>();
    protected bool Loading => DataSource.IsLoading;
    public IEnumerable<TGridItem> SelectedItems => DataSource.SelectedEntities?.ToList() ?? [];
    protected bool MultiSelection => MultiSelect || HandleSelection?.GetMultiSelection() == true;
    protected bool ShouldShowAdd => DataSource.CanAdd && ShowAdd;
    protected bool ShouldEnableAdd => EnableAdd;
    protected bool ShouldShowEdit => DataSource.CanEdit && ShowEdit;
    protected bool ShouldEnableEdit => SelectedItems.Count() == 1 && EnableEdit(SelectedItems.Single());
    protected bool ShouldShowRemove => DataSource.CanRemove && ShowRemove;
    protected bool ShouldEnableRemove => SelectedItems.Any() && EnableRemove(SelectedItems);
    protected bool ShouldShowSearch => true;
    protected bool ShouldShowSelection => ShowSelect || ShouldShowEdit || ShouldShowRemove || HandleSelection is not null;
    protected bool ShouldShowNavigation => DataSource.CanNavigate && ShowNavigate;

    protected override async Task OnInitializedAsync()
    {
        _itemsPerPageSubject
            .DistinctUntilChanged()
            .Subscribe(itemsPerPage =>
            {
                InvokeAsync(() =>
                {
                    _pagination.SetItemsPerPageAsync(HandleSelection?.GetItemsPerPage() ?? itemsPerPage);
                });
            })
            .DisposeWith(_disposables);
        
        await base.OnInitializedAsync();
        
        DataSource.MultiSelect = MultiSelection;
        DataSource.CustomFilterFunc = HandleSelection?.GetCustomFilterFunc<TGridItem>() ?? CustomFilterFunc;
        DataSource.DependencyArgs = HandleSelection?.GetDependencyArgsFunc<TGridItem>() ?? DependencyArgs;
        DataSource.SelectedEntitiesChanged = selectedEntities => SelectedItemsChanged.InvokeAsync(selectedEntities);
        DataSource.SelectedEntitiesCleared = () => _selectColumn?.ClearSelection();

        // In server-paged or BackedBy mode the DataSource.TotalCount is the
        // authoritative paginator total (FluentDataGrid would otherwise overwrite
        // it with the current page's item count). Sync TotalItemCount on every
        // state change so paginator stays in step with the latest fetch.
        DataSource.OnStateHasChanged += () =>
        {
            if ((DataSource.IsServerPaged || DataSource.IsBackedByQuery)
                && _pagination.TotalItemCount != DataSource.TotalCount)
            {
                _ = _pagination.SetTotalItemCountAsync(DataSource.TotalCount);
            }
            StateHasChanged();
        };

        await DataSource.Init();

        if (AfterInit is not null)
            await AfterInit(DataSource);

        if (DataSource.IsServerPaged || DataSource.IsBackedByQuery)
        {
            // Disable FluentDataGrid's "TotalItemCount = Items.Count()" overwrite — our
            // DataSource.TotalCount is authoritative in server-paged / BackedBy mode.
            // For BackedBy, also detect column-sort changes coming through this callback
            // and fold them into the QueryState.
            _serverRefreshItems = HandleRefreshItems;
        }

        if (DataSource.IsServerPaged)
        {
            // Initial page load — sets TotalCount and populates the first page
            await LoadServerPageAsync(1);
        }
        // BackedBy: AttachQueryState (called from AfterInit) already kicked off the
        // initial fetch via the QueryState BehaviorSubject — no LoadServerPageAsync
        // needed here.
    }

    private Task HandleRefreshItems(GridItemsProviderRequest<TGridItem> request)
    {
        // Always a no-op for items: our DataSource owns the rows. We're only here
        // for the side-effect of detecting sort changes (and to disable FluentDataGrid's
        // TotalItemCount overwrite).
        if (!DataSource.IsBackedByQuery)
            return Task.CompletedTask;

        // Derive the sort field from PropertyColumn.Property (the expression) rather
        // than ColumnBase.Title — Title is localised and would break when the user
        // switches language; the underlying property name is the stable identifier.
        // GetSortByProperties() returns one entry per active sort rule, ordered by
        // priority. We honour the first; multi-column sort can come later if a
        // screen actually needs it.
        //
        // For PropertyColumn FluentDataGrid surfaces the property name from the
        // Property expression; for TemplateColumn the explicit SortBy expression is
        // used. SortByColumn on its own gives us the column instance but no field
        // name; GetSortByProperties is the supported way to reach the underlying
        // property name without reflecting on the column.
        var sortBy = request.GetSortByProperties().FirstOrDefault();
        var currentField = sortBy.PropertyName;
        var currentAscending = sortBy.Direction != FluentSortDirection.Descending;
        var current = (currentField, currentAscending);

        if (_lastObservedSort is { } last && last == current)
            return Task.CompletedTask;

        _lastObservedSort = current;

        if (string.IsNullOrEmpty(currentField))
        {
            DataSource.UpdateSort(null);
        }
        else
        {
            DataSource.UpdateSort(new SortSpec(
                currentField,
                currentAscending ? QuerySortDirection.Ascending : QuerySortDirection.Descending));
        }

        return Task.CompletedTask;
    }

    private async Task OnPageIndexChanged(int newZeroBasedPageIndex)
    {
        // Same path for server-paged (legacy) and BackedBy: LoadServerPageAsync
        // calls DataSource.LoadPageAsync, which the data source dispatches to
        // either its legacy fetch action or the BackedBy paging mutator.
        if (!DataSource.IsServerPaged && !DataSource.IsBackedByQuery)
            return;

        await LoadServerPageAsync(newZeroBasedPageIndex + 1);
    }

    private async Task LoadServerPageAsync(int pageIndex, bool force = false)
    {
        if (_isServerLoading)
            return;
        if (!force && pageIndex == _lastLoadedPageIndex)
            return;

        _isServerLoading = true;
        try
        {
            await DataSource.LoadPageAsync(pageIndex, _pagination.ItemsPerPage, DataSource.Filter);
            await _pagination.SetTotalItemCountAsync(DataSource.TotalCount);
            // Record the successfully loaded page only after the fetch completes, so a failed load
            // doesn't mark the page as "already loaded" and silently block subsequent retries.
            _lastLoadedPageIndex = pageIndex;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load server-paged data for page {PageIndex}", pageIndex);
        }
        finally
        {
            _isServerLoading = false;
        }
    }

    private async Task OnSearchChanged()
    {
        if (!DataSource.IsServerPaged && !DataSource.IsBackedByQuery)
            return;

        // Cancel and dispose the previous debounce token before replacing it
        var previousDebounce = _searchDebounce;
        _searchDebounce = new CancellationTokenSource();
        previousDebounce?.Cancel();
        previousDebounce?.Dispose();

        var currentToken = _searchDebounce.Token;

        try
        {
            await Task.Delay(300, currentToken);

            // Reset paginator to page 1 and force a reload with the new filter
            await _pagination.SetCurrentPageIndexAsync(0);
            await LoadServerPageAsync(1, force: true);
        }
        catch (OperationCanceledException)
        {
            // Debounce cancelled — a newer search is pending
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Server-paged search failed for filter '{Filter}'", DataSource.Filter);
        }
    }

    protected async Task UpdateAll(bool selection)
    {
        foreach (var entity in DataSource.Entities ?? Enumerable.Empty<TGridItem>())
        {
            if (selection)
            {
                DataSource.SelectItem(entity);
            }
            else
            {
                DataSource.DeselectItem(entity);
            }
        }
        
        if (SelectedItemsChanged.HasDelegate)
            await SelectedItemsChanged.InvokeAsync(SelectedItems);

        HandleSelection?.SelectedItemsChanged(SelectedItems.Cast<object>());
    }

    protected async Task UpdateSelection(TGridItem? entity, bool selection)
    {
        if (entity is null)
            return;

        if (selection)
        {
            DataSource.SelectItem(entity);
        }
        else
        {
            DataSource.DeselectItem(entity);
        }
        
        if (SelectedItemsChanged.HasDelegate)
            await SelectedItemsChanged.InvokeAsync(SelectedItems);

        HandleSelection?.SelectedItemsChanged(SelectedItems.Cast<object>());
    }
    
    protected async Task Add()
    {
        await DataSource.Add();
    }
    
    protected async Task Edit(TGridItem entity)
    {
        await DataSource.Edit(entity);
    }
    
    protected async Task Remove(IEnumerable<TGridItem> entities)
    {
        await DataSource.Remove(entities);
    }
    
    protected Task Navigate(TGridItem entity)
    {
        DataSource.Navigate(entity);

        return Task.CompletedTask;
    }

    protected override void OnCleanup()
    {
        base.OnCleanup();

        _searchDebounce?.Dispose();
        _disposables.Dispose();
    }
}