using System.Linq.Expressions;
using CretNet.Platform.Querying;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CretNet.Platform.Blazor.Components;

/// <summary>
/// Search-as-you-type combobox bound to a server-side picker query.
/// The page supplies a <see cref="SearchAsync"/> callback (typically
/// dispatching a Fluxor action that hits a <c>{Entity}PickerEndpoint</c>);
/// this component handles debounced typing, cancellation, and binding the
/// resulting items into the FluentCombobox.
/// </summary>
/// <remarks>
/// <para>
/// Sibling of <c>CnpEntitySelect</c>. The legacy select loads the entire
/// entity list into memory and filters client-side; this picker fetches
/// matches per keystroke and is what BackedBy screens use for typeahead.
/// </para>
/// <para>
/// On selection or initial mount with a non-null <see cref="SelectedId"/>,
/// the picker calls <see cref="SearchAsync"/> with that id in
/// <c>includeIds</c> so the selected item appears in the dropdown even
/// when the user has typed something that wouldn't match it.
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
    /// Called on every typed-input change (after debouncing) and to resolve
    /// the initial / current selection.
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

    private IReadOnlyList<TItem> _items = Array.Empty<TItem>();
    private TItem? _selectedItem;
    private CancellationTokenSource? _searchCts;
    private TId? _lastResolvedSelectionId;

    // FluentCombobox keys its rendering on its initial state; bumping this
    // re-runs Items binding cleanly when we replace _items wholesale.
    private int _renderKey;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        // Initial search: empty term + the selected id (if any) so the
        // dropdown shows top-N results AND keeps the selected one visible.
        await RefetchAsync(string.Empty);
        ResolveSelectionFromItems();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // SelectedId changed externally — re-resolve the displayed item.
        if (!Nullable.Equals(_lastResolvedSelectionId, SelectedId))
        {
            _lastResolvedSelectionId = SelectedId;
            ResolveSelectionFromItems();
        }
    }

    private void ResolveSelectionFromItems()
    {
        if (SelectedId is null)
        {
            _selectedItem = default;
            return;
        }

        _selectedItem = _items.FirstOrDefault(i => EqualityComparer<TId>.Default.Equals(i.Id, SelectedId.Value));
    }

    private async Task OnInputChanged(ChangeEventArgs e)
    {
        // Debounce: cancel any in-flight search and start a new one after the
        // typed-input has settled. Last-write-wins via cancellation; a fast
        // typer never sees stale results.
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var ct = _searchCts.Token;

        var term = e.Value?.ToString();

        try
        {
            await Task.Delay(DebounceMs, ct);
            ct.ThrowIfCancellationRequested();
            await RefetchAsync(term);
            ResolveSelectionFromItems();
            StateHasChanged();
        }
        catch (OperationCanceledException)
        {
            // Newer keystroke superseded this fetch; nothing to do.
        }
    }

    private async Task RefetchAsync(string? term)
    {
        var includeIds = SelectedId is { } id
            ? new[] { id }
            : null;

        var ct = _searchCts?.Token ?? CancellationToken.None;

        try
        {
            _items = await SearchAsync(term, includeIds, ct);
            _renderKey++;
        }
        catch (OperationCanceledException)
        {
            // Cancellation is benign here too.
        }
    }

    private async Task OnSelectionChanged(TItem? item)
    {
        _selectedItem = item;
        var newId = item is null ? (TId?)null : item.Id;
        _lastResolvedSelectionId = newId;

        if (SelectedIdChanged.HasDelegate)
            await SelectedIdChanged.InvokeAsync(newId);

        if (SelectedItemChanged.HasDelegate)
            await SelectedItemChanged.InvokeAsync(item);
    }
}
