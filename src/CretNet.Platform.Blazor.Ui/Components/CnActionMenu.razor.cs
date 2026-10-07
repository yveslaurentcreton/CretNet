using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// A "More" button that opens a compact menu of <see cref="CnActionMenuItem"/>
/// and <see cref="CnActionMenuSeparator"/> children. Closes on an outside click,
/// Escape, Tab and after an item is chosen. While open, the list renders in
/// the browser's top layer at the trigger's position, flipping above or
/// inward near the viewport edge, so a scrolling grid, document lines or a
/// dialog body never clips it.
/// </summary>
public partial class CnActionMenu : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnActionMenu.razor.js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public RenderFragment? ChildContent { get; set; }

    #pragma warning disable BL0007 // Pure resource fallback stays culture-aware; explicit parameter values remain unchanged.
    /// <summary>Trigger text, and the menu's accessible name. Defaults to "More".</summary>
    [Parameter] public string Label { get => field ?? CnLabels.More; set; } = null!;
    [Parameter] public string CloseLabel { get => field ?? CnLabels.Close; set; } = null!;
    #pragma warning restore BL0007

    [Parameter] public CnIconKind? Icon { get; set; } = CnIconKind.More;
    [Parameter] public CnButtonRole Role { get; set; } = CnButtonRole.Neutral;

    /// <summary>Renders only the icon; <see cref="Label"/> becomes the tooltip.</summary>
    [Parameter] public bool IconOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Aligns the list with the trigger's left edge instead of its right
    /// edge; use it for a trigger near the start of a narrow screen.</summary>
    [Parameter] public bool AlignStart { get; set; }
    [Parameter] public string? Class { get; set; }

    private readonly string _menuId = $"cn-action-menu-{Guid.NewGuid():N}";
    private readonly List<CnActionMenuItem> _items = [];
    private CnButton? _trigger;
    private ElementReference _root;
    private ElementReference _list;
    private ElementReference _backdrop;
    private IJSObjectReference? _module;
    private bool _placed;
    private bool _open;
    private bool _focusFirstAfterRender;
    private int _focusedIndex = -1;

    /// <summary>True while the menu list is shown.</summary>
    public bool IsOpen => _open;

    protected override void OnParametersSet()
    {
        if (Disabled)
            _open = false;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await PlaceAsync();

        if (!_focusFirstAfterRender)
            return;

        _focusFirstAfterRender = false;
        await FocusItemAsync(0);
    }

    internal void Register(CnActionMenuItem item)
    {
        if (!_items.Contains(item))
            _items.Add(item);
    }

    internal void Unregister(CnActionMenuItem item) => _items.Remove(item);

    internal void OnItemFocused(CnActionMenuItem item) => _focusedIndex = _items.IndexOf(item);

    internal async Task SelectAsync(CnActionMenuItem item)
    {
        if (!item.CanActivate)
            return;

        var callback = item.OnClick;
        _open = false;
        _focusedIndex = -1;
        StateHasChanged();
        await FocusTriggerAsync();
        await callback.InvokeAsync();
    }

    private Task ToggleAsync()
    {
        if (Disabled)
            return Task.CompletedTask;

        _open = !_open;
        _focusFirstAfterRender = _open;
        _focusedIndex = -1;
        return Task.CompletedTask;
    }

    private async Task CloseAsync()
    {
        _open = false;
        _focusedIndex = -1;
        await FocusTriggerAsync();
    }

    private async Task HandleKeyDownAsync(KeyboardEventArgs args)
    {
        if (!_open)
        {
            if (args.Key is "ArrowDown" && !Disabled)
            {
                _open = true;
                _focusFirstAfterRender = true;
            }
            return;
        }

        switch (args.Key)
        {
            case "Escape":
                await CloseAsync();
                return;
            case "Tab":
                _open = false;
                _focusedIndex = -1;
                return;
        }

        if (_items.Count == 0)
            return;

        var target = args.Key switch
        {
            "ArrowDown" => (_focusedIndex + 1) % _items.Count,
            "ArrowUp" => _focusedIndex <= 0 ? _items.Count - 1 : _focusedIndex - 1,
            "Home" => 0,
            "End" => _items.Count - 1,
            _ => -1,
        };

        if (target >= 0)
            await FocusItemAsync(target);
    }

    private async Task FocusItemAsync(int index)
    {
        if (index < 0 || index >= _items.Count)
            return;

        _focusedIndex = index;
        await _items[index].FocusAsync();
    }

    /// <summary>
    /// Lifts the open list out of its scrolling ancestors and pins it to the
    /// trigger; after every render while open (items may change), and once on
    /// close so the module stops following the trigger. A placement failure
    /// never breaks the menu: it falls back to its absolute CSS position.
    /// </summary>
    private async Task PlaceAsync()
    {
        if (!_open && !_placed)
            return;

        try
        {
            _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
            if (_open)
            {
                _placed = true;
                await _module.InvokeVoidAsync("show", _menuId, _list, _backdrop, _root, AlignStart);
            }
            else
            {
                _placed = false;
                await _module.InvokeVoidAsync("hide", _menuId);
            }
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException exception)
        {
            Console.Error.WriteLine($"CnActionMenu placement failed: {exception.Message}");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        try
        {
            if (_placed)
                await _module.InvokeVoidAsync("hide", _menuId);
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
        catch (JSException)
        {
        }

        GC.SuppressFinalize(this);
    }

    private async Task FocusTriggerAsync()
    {
        if (_trigger is not null)
            await _trigger.FocusAsync();
    }
}
