using System.Linq.Expressions;
using CretNet.Platform.Querying;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CretNet.Platform.Blazor.Components;

/// <summary>
/// Search-as-you-type picker bound to a server-side query. Wraps
/// <c>FluentCombobox</c> as a dropdown — the page supplies a
/// <see cref="SearchAsync"/> callback (typically a Fluxor dispatch
/// hitting a <c>{Entity}PickerEndpoint</c>); typing the input fires the
/// callback (debounced + last-write-wins-cancelled), and the result is
/// shown in the dropdown.
/// </summary>
/// <remarks>
/// <para>
/// We deliberately do NOT set <c>Autocomplete</c> on the FluentCombobox
/// (it would re-filter our server-returned items client-side, ending up
/// over-filtered or empty when the typed text doesn't match the
/// localised label exactly). The dropdown shows the items as the server
/// returned them.
/// </para>
/// <para>
/// On selection, we keep the picked item in <see cref="_items"/> so the
/// FluentCombobox can show its label even if subsequent searches don't
/// return it, and we also pass the picked id as <c>includeIds</c> on
/// every fetch so the server-side handler keeps it in the result set.
/// </para>
/// </remarks>
public partial class CnpEntityPicker<TItem, TId>
    where TItem : IPickerItem<TId>
    where TId : struct
{
    [Parameter] public TId? SelectedId { get; set; }
    [Parameter] public EventCallback<TId?> SelectedIdChanged { get; set; }
    [Parameter] public EventCallback<TItem?> SelectedItemChanged { get; set; }

    /// <summary>
    /// Async fetcher: <c>(searchTerm, includeIds, cancellationToken) =&gt; items</c>.
    /// Called on every typed-input change (after debouncing) and during
    /// init to resolve the selected item's display label.
    /// </summary>
    [Parameter, EditorRequired]
    public required Func<string?, IReadOnlyList<TId>?, CancellationToken, Task<IReadOnlyList<TItem>>> SearchAsync { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public Expression<Func<TId?>>? For { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Debounce window in ms before the typed input triggers a fetch. Default 300.</summary>
    [Parameter] public int DebounceMs { get; set; } = 300;

    private List<TItem> _items = new();
    private TItem? _selectedItem;
    private CancellationTokenSource? _searchCts;
    private TId? _resolvedSelectionId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // Initial fetch: empty term + the selected id (if any) so the
        // dropdown is populated AND the selected item is in _items so
        // FluentCombobox can show its label.
        await RunSearchAsync(searchTerm: null);
        ResolveSelectionFromItems();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (!Nullable.Equals(_resolvedSelectionId, SelectedId))
        {
            _resolvedSelectionId = SelectedId;

            // External change. Make sure the selected one is in _items.
            if (SelectedId is { } id && !_items.Any(i => EqualityComparer<TId>.Default.Equals(i.Id, id)))
                await RunSearchAsync(searchTerm: null);

            ResolveSelectionFromItems();
        }
    }

    private void ResolveSelectionFromItems()
    {
        _selectedItem = SelectedId is { } id
            ? _items.FirstOrDefault(i => EqualityComparer<TId>.Default.Equals(i.Id, id))
            : default;
    }

    private async Task OnTypedAsync(ChangeEventArgs e)
    {
        // Debounce: cancel any in-flight search and start a new one after
        // the typed input has settled. Last-write-wins via cancellation;
        // a fast typer never sees stale results.
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var ct = _searchCts.Token;

        var term = e.Value?.ToString();

        try
        {
            await Task.Delay(DebounceMs, ct);
            ct.ThrowIfCancellationRequested();
            await RunSearchAsync(term, ct);
            ResolveSelectionFromItems();
            StateHasChanged();
        }
        catch (OperationCanceledException)
        {
            // Newer keystroke superseded this fetch.
        }
    }

    private async Task RunSearchAsync(string? searchTerm, CancellationToken cancellationToken = default)
    {
        var includeIds = SelectedId is { } id ? new[] { id } : null;

        try
        {
            var results = await SearchAsync(searchTerm, includeIds, cancellationToken);

            // Preserve the currently-selected item even if it's missing from
            // the results — FluentCombobox needs it in Items to render the
            // selected label.
            var merged = results.ToList();
            if (_selectedItem is not null
                && !merged.Any(i => EqualityComparer<TId>.Default.Equals(i.Id, _selectedItem.Id)))
            {
                merged.Insert(0, _selectedItem);
            }

            _items = merged;
        }
        catch (OperationCanceledException)
        {
            // Cancellation is benign.
        }
    }

    private async Task OnSelectionChangedAsync(TItem? item)
    {
        _selectedItem = item;
        var newId = item is null ? (TId?)null : item.Id;
        _resolvedSelectionId = newId;

        if (SelectedIdChanged.HasDelegate)
            await SelectedIdChanged.InvokeAsync(newId);

        if (SelectedItemChanged.HasDelegate)
            await SelectedItemChanged.InvokeAsync(item);
    }
}
