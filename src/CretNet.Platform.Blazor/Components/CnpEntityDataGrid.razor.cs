using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using CretNet.Platform.Blazor.Interfaces;
using CretNet.Platform.Blazor.Services;
using CretNet.Platform.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

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
    [Parameter] public bool IsPrimary { get; set; } = true;
    [Parameter] public Func<TGridItem, bool>? CustomFilterFunc { get; set; }
    [Parameter] public Func<object>? DependencyArgs { get; set; }
    private readonly BehaviorSubject<int> _itemsPerPageSubject = new(15);
    [Parameter] public int ItemsPerPage
    {
        get => _itemsPerPageSubject.Value;
        set => _itemsPerPageSubject.OnNext(value);
    }
    [Inject] public ICnpDataSource<TGridItem, TId> DataSource { get; set; } = default!;
    
    private SelectColumn<TGridItem>? _selectColumn;
    private readonly CompositeDisposable _disposables = new();
    private PaginationState _pagination = new();
    private CancellationTokenSource? _searchDebounce;

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
        DataSource.OnStateHasChanged += StateHasChanged;

        await DataSource.Init();

        if (DataSource.IsServerPaged)
        {
            _pagination.TotalItemCountChanged += OnTotalItemCountChanged;
            await _pagination.SetTotalItemCountAsync(DataSource.TotalCount);
        }
    }

    private void OnTotalItemCountChanged(object? sender, int? totalCount)
    {
        // When paginator changes page, reload from server
        if (DataSource.IsServerPaged)
        {
            var pageIndex = _pagination.CurrentPageIndex + 1; // PaginationState is 0-based
            var pageSize = _pagination.ItemsPerPage;
            _ = InvokeAsync(async () => await DataSource.LoadPageAsync(pageIndex, pageSize, DataSource.Filter));
        }
    }

    private async Task OnSearchChanged()
    {
        if (!DataSource.IsServerPaged)
            return;

        _searchDebounce?.Cancel();
        _searchDebounce = new CancellationTokenSource();

        try
        {
            await Task.Delay(300, _searchDebounce.Token);
            await _pagination.SetCurrentPageIndexAsync(0);
            await DataSource.LoadPageAsync(1, _pagination.ItemsPerPage, DataSource.Filter);
            await _pagination.SetTotalItemCountAsync(DataSource.TotalCount);
        }
        catch (TaskCanceledException)
        {
            // Debounce cancelled — newer search is pending
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

        _pagination.TotalItemCountChanged -= OnTotalItemCountChanged;
        _searchDebounce?.Dispose();
        _disposables.Dispose();
    }
}