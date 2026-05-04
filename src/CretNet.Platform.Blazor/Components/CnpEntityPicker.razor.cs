using System.Linq.Expressions;
using CretNet.Platform.Querying;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Components;

/// <summary>
/// Search-as-you-type picker bound to a server-side query. Wraps
/// <c>FluentCombobox</c> as a dropdown.
/// </summary>
/// <remarks>
/// <para>
/// We deliberately do NOT set <c>Autocomplete</c> on the FluentCombobox
/// (any value would re-filter our server-returned items client-side).
/// The dropdown shows the items as the server returned them.
/// </para>
/// <para>
/// FluentCombobox raises its <c>oninput</c> event before its internal
/// <c>value</c> attribute updates, which means
/// <see cref="ChangeEventArgs.Value"/> is one keystroke behind. To
/// avoid filtering on stale text we re-read the current value from the
/// DOM via <see cref="IJSRuntime"/> after the debounce window — by then
/// the web component has committed the latest character.
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

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    // Unique id per picker instance so we can read the actual value from
    // the DOM (FluentCombobox's @oninput delivers a stale ChangeEventArgs.Value).
    private readonly string _inputId = $"cnp-picker-{Guid.NewGuid():N}";

    private List<TItem> _items = new();
    private TItem? _selectedItem;
    private CancellationTokenSource? _searchCts;
    private TId? _resolvedSelectionId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await RunSearchAsync(searchTerm: null);
        ResolveSelectionFromItems();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        if (!Nullable.Equals(_resolvedSelectionId, SelectedId))
        {
            _resolvedSelectionId = SelectedId;

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

    private async Task OnTypedAsync(ChangeEventArgs _)
    {
        // Debounce: cancel any in-flight search and start a new one after
        // the typed input has settled.
        _searchCts?.Cancel();
        _searchCts?.Dispose();
        _searchCts = new CancellationTokenSource();
        var ct = _searchCts.Token;

        try
        {
            await Task.Delay(DebounceMs, ct);
            ct.ThrowIfCancellationRequested();

            // Re-read the actual current value from the DOM. The
            // ChangeEventArgs.Value from FluentCombobox's @oninput is one
            // keystroke behind because the web component's internal value
            // attribute updates after the event fires.
            var term = await JSRuntime.InvokeAsync<string?>(
                "eval", ct,
                $"document.getElementById('{_inputId}')?.value");

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
