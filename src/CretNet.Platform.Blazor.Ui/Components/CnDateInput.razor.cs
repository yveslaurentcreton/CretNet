using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnDateInput : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public DateTime? Value { get; set; }
    [Parameter] public EventCallback<DateTime?> ValueChanged { get; set; }

    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool Invalid { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    /// <summary>Raised while typing: the date so far (null while it is still
    /// incomplete), so a calendar can follow along.</summary>
    [Parameter] public EventCallback<DateTime?> OnTyping { get; set; }

    /// <summary>Raised when the field is left (Tab, Enter, blur) with whatever
    /// the half-typed digits complete to.</summary>
    [Parameter] public EventCallback<DateTime?> OnCommitted { get; set; }

    /// <summary>Digits typed past the end of this date — the caller decides
    /// where they go (the second half of a range, usually).</summary>
    [Parameter] public EventCallback<string> OnOverflow { get; set; }

    [Parameter] public EventCallback OnFocused { get; set; }
    [Parameter] public EventCallback OnEnter { get; set; }
    [Parameter] public EventCallback OnEscape { get; set; }
    [Parameter] public EventCallback OnArrowDown { get; set; }

    /// <summary>Backspace at the very start of an empty field: the caller can
    /// hop to whatever sits before it.</summary>
    [Parameter] public EventCallback OnBackspaceAtStart { get; set; }

    public ElementReference Element { get; private set; }

    private IJSObjectReference? _module;
    private string _text = string.Empty;
    private DateTime? _pushed;
    private bool _suppressFocusCallback;

    protected override void OnParametersSet()
    {
        // Only follow the value when it changed outside this field; otherwise
        // the text the user is typing would be overwritten on every render.
        if (Value == _pushed)
            return;

        _pushed = Value;
        _text = CnDateMask.Format(Value);
    }

    private async Task<IJSObjectReference> ModuleAsync() =>
        _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);

    /// <summary>Puts the caret in this field. <paramref name="selectAll"/>
    /// makes typing replace what is there.</summary>
    public async Task FocusAsync(bool selectAll)
    {
        _suppressFocusCallback = true;
        try
        {
            await Element.FocusAsync();
            if (selectAll)
            {
                var module = await ModuleAsync();
                await module.InvokeVoidAsync("selectAll", Element);
            }
        }
        catch (JSDisconnectedException)
        {
        }
        finally
        {
            _suppressFocusCallback = false;
        }
    }

    /// <summary>Seeds this field with digits that overflowed from another one:
    /// they start it over rather than appending to what stood here.</summary>
    public async Task TakeOverflowAsync(string digits)
    {
        if (string.IsNullOrEmpty(digits))
            return;

        var capped = digits.Length > CnDateMask.MaxDigits ? digits[..CnDateMask.MaxDigits] : digits;
        _text = CnDateMask.Mask(capped);
        _pushed = CnDateMask.Strict(capped);
        await WriteAsync(_text);
        await ValueChanged.InvokeAsync(_pushed);
        await OnTyping.InvokeAsync(_pushed);
    }

    private async Task WriteAsync(string text)
    {
        try
        {
            var module = await ModuleAsync();
            await module.InvokeVoidAsync("setText", Element, text);
        }
        catch (JSDisconnectedException)
        {
        }
    }

    private async Task OnInputAsync(ChangeEventArgs args)
    {
        var raw = args.Value?.ToString() ?? string.Empty;
        var digits = CnDateMask.Digits(raw);

        // A separator typed straight after a single digit means "0x" — the
        // shortcut everybody reaches for ("1/" is the first of the month).
        if (raw.Length > 0 && !char.IsAsciiDigit(raw[^1]))
            digits = CnDateMask.PadStartedSegment(digits);

        var overflow = new string(raw.Where(char.IsAsciiDigit).Skip(CnDateMask.MaxDigits).ToArray());

        _text = CnDateMask.Mask(digits);
        _pushed = CnDateMask.Strict(digits);

        try
        {
            var module = await ModuleAsync();
            await module.InvokeVoidAsync("applyMask", Element, _text);
        }
        catch (JSDisconnectedException)
        {
        }

        await ValueChanged.InvokeAsync(_pushed);
        await OnTyping.InvokeAsync(_pushed);

        if (overflow.Length > 0)
            await OnOverflow.InvokeAsync(overflow);
    }

    private Task OnBlurAsync() => CompleteAsync();

    private async Task OnFocusAsync()
    {
        if (_suppressFocusCallback)
            return;

        await OnFocused.InvokeAsync();
    }

    /// <summary>Fills in what was not typed and pulls impossible values back to
    /// the nearest day that exists — see <see cref="CnDateMask.Complete"/>.</summary>
    private async Task CompleteAsync()
    {
        var digits = CnDateMask.Digits(_text);
        var completed = CnDateMask.Complete(digits, DateTime.Today);

        _pushed = completed;
        _text = CnDateMask.Format(completed);
        await WriteAsync(_text);

        await ValueChanged.InvokeAsync(completed);
        await OnCommitted.InvokeAsync(completed);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Tab":
                // The browser moves focus right after this; completing first
                // means Tab both finishes the date and steps on.
                await CompleteAsync();
                break;
            case "Enter":
                await CompleteAsync();
                await OnEnter.InvokeAsync();
                break;
            case "Escape":
                await OnEscape.InvokeAsync();
                break;
            case "ArrowDown":
                await OnArrowDown.InvokeAsync();
                break;
            case "Backspace" when _text.Length == 0 && OnBackspaceAtStart.HasDelegate:
                await OnBackspaceAtStart.InvokeAsync();
                break;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is null)
            return;

        try
        {
            await _module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
        }
    }
}
