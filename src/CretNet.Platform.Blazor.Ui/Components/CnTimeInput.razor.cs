using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

public partial class CnTimeInput : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnDateInput.razor.js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public TimeOnly? Value { get; set; }
    [Parameter] public EventCallback<TimeOnly?> ValueChanged { get; set; }

    /// <summary>Ask for seconds and the field becomes hh:mm:ss — mask, typing
    /// rules and completion all follow.</summary>
    [Parameter] public bool Seconds { get; set; }

    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public bool ReadOnly { get; set; }
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    [Parameter] public EventCallback<TimeOnly?> OnTyping { get; set; }
    [Parameter] public EventCallback<TimeOnly?> OnCommitted { get; set; }

    /// <summary>Raised the moment the time is complete, so a host can move on
    /// to whatever comes after it.</summary>
    [Parameter] public EventCallback OnFilled { get; set; }

    /// <summary>Raised on every focus, carrying whether the code moved it
    /// rather than the user. Hosts count focus either way — otherwise moving
    /// the caret themselves looks like the control being left — but only open
    /// their popover when the user did it.</summary>
    [Parameter] public EventCallback<bool> OnFocused { get; set; }
    [Parameter] public EventCallback OnBlurred { get; set; }
    [Parameter] public EventCallback OnEnter { get; set; }
    [Parameter] public EventCallback OnEscape { get; set; }

    /// <summary>Backspace at the very start of an empty field: the caller can
    /// carry on in whatever sits to the left of it.</summary>
    [Parameter] public EventCallback OnBackspaceAtStart { get; set; }

    public ElementReference Element { get; private set; }

    private IJSObjectReference? _module;
    private string _text = string.Empty;
    private TimeOnly? _pushed;
    private bool _programmaticFocus;

    protected override void OnParametersSet()
    {
        if (Value == _pushed)
            return;

        _pushed = Value;
        _text = CnTimeMask.Format(Value, Seconds);
    }

    private async Task<IJSObjectReference> ModuleAsync() =>
        _module ??= await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);

    public async Task FocusAsync(bool selectAll)
    {
        _programmaticFocus = true;
        try
        {
            await Element.FocusAsync();
            var module = await ModuleAsync();
            if (selectAll)
                await module.InvokeVoidAsync("selectAll", Element);
        }
        catch (JSDisconnectedException)
        {
        }
        finally
        {
            _programmaticFocus = false;
        }
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
        var digits = CnTimeMask.Digits(raw, Seconds);

        _text = CnTimeMask.Mask(digits);
        _pushed = CnTimeMask.Strict(digits, Seconds);

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

        // A finished time has nothing left to type.
        if (_pushed is not null && digits.Length == (Seconds ? 6 : 4))
            await OnFilled.InvokeAsync();
    }

    private async Task OnBlurAsync()
    {
        await CompleteAsync();
        await OnBlurred.InvokeAsync();
    }

    private Task OnFocusAsync() => OnFocused.InvokeAsync(_programmaticFocus);

    /// <summary>Fills in what was not typed and pulls impossible values back to
    /// the nearest time that exists — see <see cref="CnTimeMask.Complete"/>.</summary>
    private async Task CompleteAsync()
    {
        var digits = CnTimeMask.Digits(_text, Seconds);
        var completed = CnTimeMask.Complete(digits);

        _pushed = completed;
        _text = CnTimeMask.Format(completed, Seconds);
        await WriteAsync(_text);

        await ValueChanged.InvokeAsync(completed);
        await OnCommitted.InvokeAsync(completed);
    }

    private async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Tab":
                await CompleteAsync();
                break;
            case "Enter":
                await CompleteAsync();
                await OnEnter.InvokeAsync();
                break;
            case "Escape":
                await OnEscape.InvokeAsync();
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
