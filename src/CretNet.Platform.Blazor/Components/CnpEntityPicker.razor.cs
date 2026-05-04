using System.Linq.Expressions;
using CretNet.Platform.Querying;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;

namespace CretNet.Platform.Blazor.Components;

/// <summary>
/// Search-as-you-type picker bound to a server-side query. Wraps
/// <see cref="FluentAutocomplete{TOption}"/> in single-selection mode
/// (<c>MaximumSelectedOptions=1</c>). The page supplies a
/// <see cref="SearchAsync"/> callback (typically dispatching a Fluxor
/// action that hits a <c>{Entity}PickerEndpoint</c>); this component
/// drives the FluentAutocomplete's <c>OnOptionsSearch</c> event with
/// it and translates selections back into <see cref="SelectedId"/>.
/// </summary>
/// <remarks>
/// <para>
/// FluentAutocomplete is the right primitive for server-side typeahead
/// (FluentCombobox is built for static lists with client-side filtering).
/// The selected pick renders as a chip above the input and stays visible
/// while the user types something else, so we don't need to pin the
/// current selection into every search result — though we still pass
/// it as <c>includeIds</c> so the dropdown also surfaces it on a brand
/// new search session.
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
    /// Called on every typed-input change (FluentAutocomplete debounces
    /// internally) and once during init to resolve the selected item's
    /// label.
    /// </summary>
    [Parameter, EditorRequired]
    public required Func<string?, IReadOnlyList<TId>?, CancellationToken, Task<IReadOnlyList<TItem>>> SearchAsync { get; set; }

    [Parameter] public string? Label { get; set; }
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public Expression<Func<TId?>>? For { get; set; }
    [Parameter] public bool Required { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Hard cap on results returned per search. Forwarded to FluentAutocomplete's MaximumOptionsSearch.</summary>
    [Parameter] public int MaximumResults { get; set; } = 50;

    private List<TItem> _selectedOptions = new();
    private TId? _resolvedSelectionId;

    protected override async Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();
        await ResolveInitialSelectionAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // External SelectedId change → re-resolve the displayed chip.
        if (!Nullable.Equals(_resolvedSelectionId, SelectedId))
            await ResolveInitialSelectionAsync();
    }

    private async Task ResolveInitialSelectionAsync()
    {
        _resolvedSelectionId = SelectedId;

        if (SelectedId is null)
        {
            _selectedOptions = new List<TItem>();
            return;
        }

        // Already in the chips list — nothing to do (avoids needless fetch).
        if (_selectedOptions.Any(i => EqualityComparer<TId>.Default.Equals(i.Id, SelectedId.Value)))
            return;

        try
        {
            var items = await SearchAsync(null, new[] { SelectedId.Value }, CancellationToken.None);
            var resolved = items.FirstOrDefault(i => EqualityComparer<TId>.Default.Equals(i.Id, SelectedId.Value));
            _selectedOptions = resolved is null ? new List<TItem>() : new List<TItem> { resolved };
        }
        catch
        {
            _selectedOptions = new List<TItem>();
        }
    }

    private async Task OnOptionsSearchAsync(OptionsSearchEventArgs<TItem> args)
    {
        var includeIds = SelectedId is { } id ? new[] { id } : null;

        try
        {
            var items = await SearchAsync(args.Text, includeIds, CancellationToken.None);
            args.Items = items;
        }
        catch
        {
            // Surface to ICnpToastService if needed; for now silently drop —
            // a failed search just shows no matches, the selected chip stays.
            args.Items = Array.Empty<TItem>();
        }
    }

    private async Task OnSelectedOptionsChangedAsync(IEnumerable<TItem>? selected)
    {
        var list = selected?.ToList() ?? new List<TItem>();
        _selectedOptions = list;

        var pick = list.FirstOrDefault();
        var newId = pick is null ? (TId?)null : pick.Id;
        _resolvedSelectionId = newId;

        if (SelectedIdChanged.HasDelegate)
            await SelectedIdChanged.InvokeAsync(newId);

        if (SelectedItemChanged.HasDelegate)
            await SelectedItemChanged.InvokeAsync(pick);
    }
}
