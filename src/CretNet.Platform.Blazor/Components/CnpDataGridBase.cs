using CretNet.Platform.Blazor.Interfaces;
using CretNet.Platform.Data;
using Microsoft.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public abstract class CnpDataGridBase<T> : CnpComponent
    where T : IIdentity
{
    [CascadingParameter] public IHandleSelection? HandleSelection { get; set; }
    [Parameter, EditorRequired] public IEnumerable<T> Items { get; set; } = default!;
    [Parameter] public ICollection<T> SelectedItems { get; set; } = new List<T>();
    [Parameter] public EventCallback<ICollection<T>> SelectedItemsChanged { get; set; }
    [Parameter] public bool MultiSelect { get; set; }
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool ShowAdd { get; set; }
    [Parameter] public bool EnableAdd { get; set; } = true;
    [Parameter] public EventCallback OnAdd { get; set; }
    [Parameter] public bool ShowEdit { get; set; }
    [Parameter] public Func<T, bool> EnableEdit { get; set; } = _ => true;
    [Parameter] public EventCallback<T> OnEdit { get; set; }
    [Parameter] public bool ShowRemove { get; set; }
    [Parameter] public Func<IEnumerable<T>, bool> EnableRemove { get; set; } = _ => true;
    [Parameter] public EventCallback<IEnumerable<T>> OnRemove { get; set; }
    [Parameter] public bool ShowNavigate { get; set; }
    [Parameter] public EventCallback<T> OnNavigate { get; set; }
    [Parameter] public RenderFragment<IEnumerable<T>>? Actions { get; set; }
    [Parameter, EditorRequired] public RenderFragment Columns { get; set; } = default!;
    [Parameter] public EventCallback<string?> OnSearch { get; set; }
    [Parameter] public Func<IQueryable<T>, string?, IQueryable<T>>? Filter { get; set; }
    [Parameter] public bool IsPrimary { get; set; } = true;

    public string? Search { get; set; }

    protected IQueryable<T> FilteredItems => InternalFilter();
    protected bool MultiSelection => MultiSelect || HandleSelection?.GetMultiSelection() == true;
    protected bool ShouldShowAdd => OnAdd.HasDelegate && ShowAdd;
    protected bool ShouldEnableAdd => EnableAdd;
    protected bool ShouldShowEdit => OnEdit.HasDelegate && ShowEdit;
    protected bool ShouldEnableEdit => SelectedItems.Count == 1 && EnableEdit(SelectedItems.Single());
    protected bool ShouldShowRemove => OnRemove.HasDelegate && ShowRemove;
    protected bool ShouldEnableRemove => SelectedItems.Any() && EnableRemove(SelectedItems);
    protected bool ShouldShowSearch => Filter is not null || OnSearch.HasDelegate;
    protected bool ShouldShowSelection => ShouldShowEdit || ShouldShowRemove || HandleSelection is not null;
    protected bool ShouldShowNavigation => OnNavigate.HasDelegate && ShowNavigate;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        
        RemoveNonExistentItems();
    }

    protected IQueryable<T> InternalFilter()
    {
        var query = Items.AsQueryable();
        
        var selectionFilter = HandleSelection?.GetSelectionFilter<T>();
        if (selectionFilter is not null)
            query = selectionFilter(query);

        if (Filter is not null)
            query = Filter(query, Search);
        
        return query;
    }

    protected async Task AutoUpdateSelection(T? entity)
    {
        if (entity is null)
            return;
        
        var alreadySelected = SelectedItems.Any(x => x.HasSameIdAs(entity));
        
        await UpdateSelection(entity, !alreadySelected);
    }
    
    protected async Task UpdateAll(bool selection)
    {
        foreach (var entity in FilteredItems)
        {
            if (selection)
            {
                SelectItem(entity);
            }
            else
            {
                DeselectItem(entity);
            }
        }
        
        if (SelectedItemsChanged.HasDelegate)
            await SelectedItemsChanged.InvokeAsync(SelectedItems);

        HandleSelection?.SelectedItemsChanged(SelectedItems.Cast<object>());
    }

    protected async Task UpdateSelection(T? entity, bool selection)
    {
        if (entity is null)
            return;

        if (selection)
        {
            SelectItem(entity);
        }
        else
        {
            DeselectItem(entity);
        }
        
        if (SelectedItemsChanged.HasDelegate)
            await SelectedItemsChanged.InvokeAsync(SelectedItems);

        HandleSelection?.SelectedItemsChanged(SelectedItems.Cast<object>());
    }
    
    private void SelectItem(T entity)
    {
        if (!MultiSelection)
            SelectedItems.Clear();
        
        if (!SelectedItems.Contains(entity))
            SelectedItems.Add(entity);
    }

    private void DeselectItem(T? entity)
    {
        foreach (var item in SelectedItems.Where(x => x.HasSameIdAs(entity)).ToList())
        {
            SelectedItems.Remove(item);
        }
    }
    
    protected void RemoveNonExistentItems()
    {
        var itemsToRemove = SelectedItems.Where(selectedItem => !Items.Any(item => item.HasSameIdAs(selectedItem))).ToList();

        foreach (var item in itemsToRemove)
        {
            SelectedItems.Remove(item);
        }
    }
    
    protected bool IsSelected(T entity)
    {
        var isSelected = SelectedItems.Any(x => x.HasSameIdAs(entity));
        return isSelected;
    }
    
    protected bool? IsAllSelected()
    {
        var isAllSelected = FilteredItems.All(IsSelected);
        return isAllSelected;
    }
    
    protected async Task Add()
    {
        if (OnAdd.HasDelegate)
            await OnAdd.InvokeAsync();
    }
    
    protected async Task Edit()
    {
        if (OnEdit.HasDelegate)
            await OnEdit.InvokeAsync(SelectedItems.Single());
    }
    
    protected async Task Remove()
    {
        if (OnRemove.HasDelegate)
            await OnRemove.InvokeAsync(SelectedItems);
    }
    
    protected async Task Navigate(T entity)
    {
        if (OnNavigate.HasDelegate)
            await OnNavigate.InvokeAsync(entity);
    }
    
    protected async Task InternalOnSearch()
    {
        if (OnSearch.HasDelegate)
            await OnSearch.InvokeAsync(Search);
    }
}
