using CretNet.Platform.Blazor.Interfaces;
using CretNet.Platform.Blazor.State.Actions.CnpSearchDialog;
using CretNet.Platform.Data;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

public partial class CnpSearchDialog : IDialogContentComponent, IHandleSelection
{
    [CascadingParameter] public FluentDialog Dialog { get; set; } = default!;
    public DialogParameters<ICnpSearchDialogParameters> FluentDialogParameters => (DialogParameters<ICnpSearchDialogParameters>)Dialog.Instance.Parameters;
    public ICnpSearchDialogParameters DialogParameters => FluentDialogParameters.Content;

    private IEnumerable<object>? SelectedItems { get; set; }
    
    public bool GetMultiSelection()
    {
        return DialogParameters.MultiSelection;
    }
    
    

    public void SelectedItemsChanged(IEnumerable<object> entities)
    {
        SelectedItems = entities;
        StateHasChanged();
    }

    public Func<IQueryable<TEntity>, IQueryable<TEntity>>? GetSelectionFilter<TEntity>()
        where TEntity : IIdentity
    {
        return (DialogParameters as IHandleSelectionParameters<TEntity>)?.Filter;
    }

    public Func<TEntity, bool>? GetCustomFilterFunc<TEntity>()
        where TEntity : IIdentity
    {
        return (DialogParameters as IHandleSelectionParameters<TEntity>)?.CustomFilterFunc;
    }

    public int GetItemsPerPage()
    {
        return 10;
    }

    public Func<object>? GetDependencyArgsFunc<TEntity>()
        where TEntity : IIdentity
    {
        return (DialogParameters as IHandleSelectionParameters<TEntity>)?.DependencyArgsFunc;
    }
    
    private async Task SelectAsync()
    {
        await Dialog.CloseAsync(SelectedItems);
    }

    private async Task CancelAsync()
    {
        await Dialog.CancelAsync();
    }
}