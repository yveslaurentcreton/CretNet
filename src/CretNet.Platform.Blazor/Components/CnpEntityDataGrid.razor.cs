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
    
    [Inject] public ICnpDataSource<TGridItem, TId> DataSource { get; set; } = default!;
    
    private SelectColumn<TGridItem>? _selectColumn;
    private PaginationState _pagination = new() { ItemsPerPage = 10 };

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
    protected bool ShouldShowSelection => ShouldShowEdit || ShouldShowRemove || HandleSelection is not null;
    protected bool ShouldShowNavigation => DataSource.CanNavigate && ShowNavigate;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        
        DataSource.MultiSelect = MultiSelection;
        DataSource.DependencyArgs = DependencyArgs;
        DataSource.CustomFilterFunc = HandleSelection?.GetCustomFilterFunc<TGridItem>() ?? CustomFilterFunc;
        DataSource.SelectedEntitiesChanged = selectedEntities => SelectedItemsChanged.InvokeAsync(selectedEntities);
        DataSource.SelectedEntitiesCleared = () => _selectColumn?.ClearSelection();
        DataSource.OnStateHasChanged += StateHasChanged;

        await DataSource.Init();
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
}