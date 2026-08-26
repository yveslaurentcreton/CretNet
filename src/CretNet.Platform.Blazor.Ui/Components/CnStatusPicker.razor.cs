using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Reads a status and, when the caller allows it, changes it in place.
/// The component owns the interaction and the presentation only: which
/// destinations are reachable is a domain question, so the caller supplies
/// <see cref="Options"/> already filtered.
/// </summary>
public partial class CnStatusPicker<TStatus> where TStatus : struct, Enum
{
    [Parameter, EditorRequired] public TStatus Value { get; set; }

    /// <summary>The destinations to offer, in the order they should read.</summary>
    [Parameter, EditorRequired] public IReadOnlyList<CnStatusOption<TStatus>> Options { get; set; } = [];

    [Parameter] public EventCallback<TStatus> ValueChanged { get; set; }

    /// <summary>False renders the same pill as a plain, unclickable badge.</summary>
    [Parameter] public bool CanChange { get; set; }

    /// <summary>Blocks input while a change is in flight.</summary>
    [Parameter] public bool Loading { get; set; }

    /// <summary>Optional word before the value, e.g. "Status".</summary>
    [Parameter] public string? Prefix { get; set; }

    [Parameter] public string AriaLabel { get; set; } = "Status";
    [Parameter] public string MenuLabel { get; set; } = "Change status";
    [Parameter] public string CloseLabel { get; set; } = "Close";
    [Parameter] public string? Class { get; set; }

    private readonly string _menuId = $"cn-status-{Guid.NewGuid():N}";
    private readonly List<StatusItem> _items = [];
    private ElementReference _trigger;
    private bool _focusCurrentAfterRender;
    private bool _open;

    private CnStatusOption<TStatus>? CurrentOption => Options.FirstOrDefault(option =>
        EqualityComparer<TStatus>.Default.Equals(option.Value, Value));

    private string CurrentLabel => CurrentOption?.Label ?? Value.ToString();

    protected override void OnParametersSet()
    {
        _items.Clear();
        _items.AddRange(Options.Select(option => new StatusItem(option)));

        if (!CanChange || Loading)
            _open = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!_focusCurrentAfterRender)
            return;

        _focusCurrentAfterRender = false;

        var currentIndex = _items.FindIndex(item =>
            EqualityComparer<TStatus>.Default.Equals(item.Option.Value, Value));

        await FocusOptionAsync(currentIndex < 0 ? 0 : currentIndex);
    }

    private Task ToggleAsync()
    {
        if (Loading || !CanChange)
            return Task.CompletedTask;

        _open = !_open;
        _focusCurrentAfterRender = _open;
        return Task.CompletedTask;
    }

    private async Task SelectAsync(CnStatusOption<TStatus> option)
    {
        if (Loading || option.Disabled || EqualityComparer<TStatus>.Default.Equals(option.Value, Value))
            return;

        _open = false;
        await ValueChanged.InvokeAsync(option.Value);
    }

    private async Task CloseAsync()
    {
        _open = false;
        await _trigger.FocusAsync();
    }

    private async Task HandlePickerKeyDownAsync(KeyboardEventArgs args)
    {
        if (args.Key == "Escape" && _open)
            await CloseAsync();
    }

    private async Task HandleOptionKeyDownAsync(KeyboardEventArgs args, int currentIndex)
    {
        var targetIndex = args.Key switch
        {
            "ArrowDown" or "ArrowRight" => NextEnabledIndex(currentIndex, 1),
            "ArrowUp" or "ArrowLeft" => NextEnabledIndex(currentIndex, -1),
            "Home" => NextEnabledIndex(-1, 1),
            "End" => NextEnabledIndex(0, -1),
            _ => currentIndex,
        };

        if (targetIndex != currentIndex)
            await FocusOptionAsync(targetIndex);
    }

    private int NextEnabledIndex(int start, int direction)
    {
        if (_items.Count == 0)
            return -1;

        for (var offset = 1; offset <= _items.Count; offset++)
        {
            var index = (start + direction * offset) % _items.Count;
            if (index < 0)
                index += _items.Count;

            if (!_items[index].Option.Disabled)
                return index;
        }

        return -1;
    }

    private async Task FocusOptionAsync(int index)
    {
        if (index >= 0 && index < _items.Count)
            await _items[index].Button.FocusAsync();
    }

    private static string ToneClass(CnStatusTone? tone) => tone switch
    {
        CnStatusTone.Accent => "is-accent",
        CnStatusTone.Warning => "is-warning",
        CnStatusTone.Danger => "is-danger",
        _ => "is-neutral",
    };

    private sealed class StatusItem(CnStatusOption<TStatus> option)
    {
        public CnStatusOption<TStatus> Option { get; } = option;
        public ElementReference Button { get; set; }
    }
}
