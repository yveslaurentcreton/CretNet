using System.Linq.Expressions;
using CretNet.Platform.Blazor.Services;
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
/// Three generic parameters so this can mirror the legacy
/// <c>CnpEntitySelect&lt;TEntity, TId&gt;</c> ergonomics:
/// </para>
/// <list type="bullet">
///   <item><c>TEntity</c> — the canonical entity type, used to resolve a
///   default label / icon from the registered <c>IEntityDefinition&lt;TEntity, TId&gt;</c>.
///   No data-source / fetch coupling; pure metadata lookup.</item>
///   <item><c>TItem</c> — the per-screen picker projection (a slim
///   <see cref="IPickerItem{TId}"/>) the dropdown actually renders.</item>
///   <item><c>TId</c> — id type shared between both.</item>
/// </list>
/// <para>
/// If a registered <c>IEntityDefinition&lt;TEntity, TId&gt;</c> exists,
/// its <c>Label</c> is used by default; otherwise the page must pass an
/// explicit <see cref="Label"/>.
/// </para>
/// <para>
/// We deliberately do NOT set <c>Autocomplete</c> on the FluentCombobox
/// (any value would re-filter our server-returned items client-side).
/// </para>
/// <para>
/// FluentCombobox raises its <c>oninput</c> event before its internal
/// <c>value</c> updates, so <see cref="ChangeEventArgs.Value"/> is one
/// keystroke behind. We re-read the actual typed text from the shadow
/// DOM <c>&lt;input&gt;</c> via <see cref="IJSRuntime"/> after the
/// debounce window.
/// </para>
/// </remarks>
public partial class CnpEntityPicker<TEntity, TItem, TId>
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

    /// <summary>
    /// Override label. When null (default), the picker uses the registered
    /// <c>IEntityDefinition&lt;TEntity, TId&gt;.Label</c> if available.
    /// </summary>
    [Parameter] public string? Label { get; set; }

    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public Expression<Func<TId?>>? For { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Debounce window in ms before the typed input triggers a fetch. Default 300.</summary>
    [Parameter] public int DebounceMs { get; set; } = 300;

    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    /// <summary>
    /// Optional definition lookup — resolved from DI if registered. Mirrors
    /// the legacy <c>CnpEntitySelect</c>'s pattern of pulling the label from
    /// the entity's canonical Definition. Null for ad-hoc TEntity types
    /// without a registered definition; in that case the page must pass
    /// an explicit <see cref="Label"/>.
    /// </summary>
    [Inject] private IEntityDefinition<TEntity, TId>? EntityDefinition { get; set; }

    private string? DisplayedLabel => Label ?? EntityDefinition?.Label;

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

            // Re-read the actual current typed text from the DOM. The
            // ChangeEventArgs.Value from FluentCombobox's @oninput is one
            // keystroke behind because the web component's internal value
            // attribute updates after the event fires.
            //
            // Critically: the OUTER element's .value is the selected option's
            // value (a Guid string in our case), NOT the typed text. The
            // typed text lives in the shadow DOM <input> inside the web
            // component. FAST UI uses open shadow roots so we can reach it.
            var term = await JSRuntime.InvokeAsync<string?>(
                "eval", ct,
                $"document.getElementById('{_inputId}')?.shadowRoot?.querySelector('input')?.value ?? ''");

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
