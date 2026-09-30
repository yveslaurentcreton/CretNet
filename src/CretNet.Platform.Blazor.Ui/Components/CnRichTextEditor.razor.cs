using CretNet.RichText;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Minimal rich-text editor (bold/italic/underline, subtitle, lists, palette
/// colour) around a contenteditable div. Produces limited HTML that
/// <c>a host HTML renderer</c> renders into the proposal PDF.
/// The JS module owns the toolbar state; .NET only round-trips the HTML value.
/// </summary>
/// <remarks>
/// S-348: the value is always reduced to the <see cref="CnRichTextSanitizer"/>
/// allow-list — when set from outside, on every input and on paste. Pasted
/// HTML is sanitised by .NET (styles become tags and palette colours first),
/// so the browser and a server that stores HTML clean identically.
/// </remarks>
public partial class CnRichTextEditor : IAsyncDisposable
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;

    [Parameter] public string? Value { get; set; }
    [Parameter] public EventCallback<string?> ValueChanged { get; set; }
    [Parameter] public bool ReadOnly { get; set; }

    // S-264: the CnRichTextBox opens on click — the caret must land inside.
    [Parameter] public bool AutoFocus { get; set; }

    private ElementReference _editorElement;
    private ElementReference _toolbarElement;
    private IJSObjectReference? _module;
    private DotNetObjectReference<CnRichTextEditor>? _dotNetRef;
    private string? _renderedValue;
    private bool _renderedReadOnly;
    private bool _disposed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        // Navigating away right after an action (e.g. accepting a proposal jumps
        // to the quote) disposes this component while the first-render interop is
        // still in flight; serializing the disposed DotNetObjectReference then
        // crashed the renderer (owner bug). Guard around every await.
        if (_disposed)
            return;

        try
        {
            if (firstRender)
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/CretNet.Platform.Blazor.Ui/Components/CnRichTextEditor.razor.js");
                if (_disposed)
                    return;

                await _module.InvokeVoidAsync("init", _editorElement, _toolbarElement, _dotNetRef, CnRichTextSanitizer.Sanitize(Value), ReadOnly);
                if (AutoFocus && !ReadOnly && !_disposed)
                    await _module.InvokeVoidAsync("focusEditor", _editorElement);
                _renderedValue = Value;
                _renderedReadOnly = ReadOnly;
                return;
            }

            if (_module is null)
                return;

            // Push external changes into the DOM; edits coming from the editor
            // itself already match _renderedValue and are skipped.
            if (!string.Equals(Value ?? string.Empty, _renderedValue ?? string.Empty, StringComparison.Ordinal))
            {
                await _module.InvokeVoidAsync("setContent", _editorElement, CnRichTextSanitizer.Sanitize(Value));
                _renderedValue = Value;
            }

            if (ReadOnly != _renderedReadOnly)
            {
                await _module.InvokeVoidAsync("setReadOnly", _editorElement, ReadOnly);
                _renderedReadOnly = ReadOnly;
            }
        }
        catch (ObjectDisposedException)
        {
            // Disposed mid-flight during navigation — nothing to render into.
        }
        catch (JSDisconnectedException)
        {
        }
    }

    [JSInvokable]
    public async Task OnContentChanged(string html)
    {
        // The DOM is already cleaned structurally; this is the guarantee for
        // what gets stored. An empty editor ("<p><br></p>") becomes "".
        var clean = CnRichTextSanitizer.Sanitize(html);
        _renderedValue = clean;
        Value = clean;
        await ValueChanged.InvokeAsync(clean);
    }

    /// <summary>Cleans clipboard HTML before the browser inserts it.</summary>
    [JSInvokable]
    public string SanitizePaste(string html) => CnRichTextSanitizer.Sanitize(html);

    private async Task ApplyColourAsync(CnPaletteColor? colour)
    {
        if (_module is null || ReadOnly || _disposed)
            return;

        try
        {
            await _module.InvokeVoidAsync("applyColor", _editorElement, colour?.Name);
        }
        catch (JSDisconnectedException)
        {
        }
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;

        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("destroy", _editorElement);
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                // The circuit/runtime is gone; nothing left to clean up.
            }
        }

        _dotNetRef?.Dispose();
    }
}
