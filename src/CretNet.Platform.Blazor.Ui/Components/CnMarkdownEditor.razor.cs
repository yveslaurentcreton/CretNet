using CretNet.Platform.Blazor.Ui.Toasts;
using CretNet.RichText;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Components;

/// <summary>
/// Markdown editor with toolbar and live preview (CretNet S-349). The value is
/// Markdown in <see cref="CnMarkdown"/> syntax; the preview renders it with the
/// same pipeline as server-side documents. Toolbar commands and shortcuts
/// (Ctrl+B, Ctrl+I, Ctrl+K) run <see cref="CnMarkdownTextCommands"/> on the
/// textarea selection. Pasted HTML (Word, Google Docs, web) is converted by
/// <see cref="CnHtmlToMarkdown"/> and a toast names what was left out; plain
/// text pastes as is.
/// </summary>
public partial class CnMarkdownEditor : IAsyncDisposable
{
    private const string ModulePath = "./_content/CretNet.Platform.Blazor.Ui/Components/CnMarkdownEditor.razor.js";

    [Inject] private IJSRuntime JsRuntime { get; set; } = default!;
    [Inject] private IServiceProvider Services { get; set; } = default!;

    [Parameter] public string? Placeholder { get; set; }

    /// <summary>Shows a character counter against this length. The editor does
    /// not cut text off; validation stays with the host.</summary>
    [Parameter] public int? MaxLength { get; set; }

    /// <summary>Minimal toolbar (bold, italic, bullet list, link) and only
    /// write/preview, for comments and short notes.</summary>
    [Parameter] public bool Compact { get; set; }

    /// <summary>The first mode shown. Defaults to side by side, or write for
    /// <see cref="Compact"/>. Side by side stacks below tablet width.</summary>
    [Parameter] public CnMarkdownEditorMode? DefaultMode { get; set; }

    /// <summary>Preview on a light sheet with document typography. Defaults to
    /// true except for <see cref="Compact"/>.</summary>
    [Parameter] public bool? Paper { get; set; }

    /// <summary>Optional note in the footer, e.g. "preview = PDF".</summary>
    [Parameter] public string? Hint { get; set; }

    /// <summary>Raised after pasted HTML was converted and inserted.</summary>
    [Parameter] public EventCallback<CnHtmlToMarkdownResult> PasteConverted { get; set; }

    private readonly string _id = $"cn-md-{Guid.NewGuid():N}";
    private ElementReference _textarea;
    private IJSObjectReference? _module;
    private DotNetObjectReference<CnMarkdownEditor>? _dotNetRef;
    private CnMarkdownEditorMode _mode;
    private bool _modeChanged;
    private bool _disposed;

    /// <summary>The mode currently shown.</summary>
    public CnMarkdownEditorMode Mode => _mode;

    private IEnumerable<CnMarkdownEditorMode> Modes => Compact
        ? [CnMarkdownEditorMode.Write, CnMarkdownEditorMode.Preview]
        : [CnMarkdownEditorMode.Write, CnMarkdownEditorMode.Preview, CnMarkdownEditorMode.Split];

    private string ModeName => _mode.ToString().ToLowerInvariant();

    private int Length => Value?.Length ?? 0;

    private bool CanFormat => !ReadOnly && !Disabled && _mode != CnMarkdownEditorMode.Preview;

    private static string ModeLabel(CnMarkdownEditorMode mode) => mode switch
    {
        CnMarkdownEditorMode.Write => CnLabels.Write,
        CnMarkdownEditorMode.Preview => CnLabels.Preview,
        _ => CnLabels.SideBySide,
    };

    protected override void OnInitialized()
    {
        _mode = DefaultMode ?? (Compact ? CnMarkdownEditorMode.Write : CnMarkdownEditorMode.Split);
        if (Compact && _mode == CnMarkdownEditorMode.Split)
            _mode = CnMarkdownEditorMode.Write;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_disposed)
            return;

        try
        {
            if (firstRender)
            {
                _dotNetRef = DotNetObjectReference.Create(this);
                _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
                if (_disposed || _module is null)
                    return;

                await _module.InvokeVoidAsync("init", _textarea, _dotNetRef);
                return;
            }

            if (_modeChanged && _module is not null)
            {
                // A textarea hidden in preview mode has no height to grow to;
                // measure again once it is visible.
                _modeChanged = false;
                await _module.InvokeVoidAsync("autosize", _textarea);
                if (_mode == CnMarkdownEditorMode.Write)
                    await _module.InvokeVoidAsync("focus", _textarea);
            }
        }
        catch (ObjectDisposedException)
        {
        }
        catch (JSDisconnectedException)
        {
        }
    }

    /// <summary>Runs a toolbar command on the current selection.</summary>
    public async Task ExecuteAsync(CnMarkdownCommand command, CnPaletteColor? colour = null)
    {
        if (!CanFormat)
            return;

        await ApplyAsync(command, await GetSelectionAsync(), colour);
    }

    private async Task ApplyAsync(CnMarkdownCommand command, CnTextSelection selection, CnPaletteColor? colour = null)
    {
        var edit = CnMarkdownTextCommands.Apply(Value, selection, command, colour);
        if (edit is null)
        {
            Services.GetService<CnToastService>()?.Information(CnLabels.SelectTextToClear);
            return;
        }

        await CommitAsync(edit.Value);
    }

    [JSInvokable]
    public async Task OnShortcutAsync(string key, int start, int end)
    {
        if (!CanFormat)
            return;

        var command = key switch
        {
            "b" => CnMarkdownCommand.Bold,
            "i" => CnMarkdownCommand.Italic,
            "k" => CnMarkdownCommand.Link,
            _ => (CnMarkdownCommand?)null,
        };
        if (command is { } known)
            await ApplyAsync(known, new CnTextSelection(start, end));
    }

    /// <summary>Converts clipboard HTML to Markdown and inserts it at the selection.</summary>
    [JSInvokable]
    public async Task OnPasteHtmlAsync(string html, string? plainText, int start, int end)
    {
        if (ReadOnly || Disabled)
            return;

        var result = CnHtmlToMarkdown.Convert(html);
        var markdown = result.Markdown.Length > 0 ? result.Markdown : (plainText ?? string.Empty).Replace("\r\n", "\n");
        if (markdown.Length == 0)
            return;

        await CommitAsync(CnMarkdownTextCommands.Insert(Value, new CnTextSelection(start, end), markdown));

        var toasts = Services.GetService<CnToastService>();
        if (result.Dropped.Count > 0)
            toasts?.Information(CnLabels.PastedAsMarkdown, CnLabels.Format(CnLabels.PasteLeftOut, string.Join(", ", result.Dropped.Select(DroppedLabel))));
        else if (!string.Equals(Normalize(result.Markdown), Normalize(plainText), StringComparison.Ordinal))
            // Formatting was converted; a plain word copied from a web page
            // (still HTML on the clipboard) is not worth a toast.
            toasts?.Information(CnLabels.PastedAsMarkdown);

        await PasteConverted.InvokeAsync(result);
    }

    private static string Normalize(string? text) =>
        string.Join(' ', (text ?? string.Empty).Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static string DroppedLabel(CnDroppedFormatting kind) => kind switch
    {
        CnDroppedFormatting.Underline => CnLabels.DroppedUnderline,
        CnDroppedFormatting.FontFamilyOrSize => CnLabels.DroppedFont,
        CnDroppedFormatting.BackgroundColor => CnLabels.DroppedBackground,
        CnDroppedFormatting.Image => CnLabels.DroppedImages,
        _ => CnLabels.DroppedLinks,
    };

    private void SetMode(CnMarkdownEditorMode mode)
    {
        if (mode == _mode)
            return;

        _mode = mode;
        _modeChanged = true;
    }

    private async Task<CnTextSelection> GetSelectionAsync()
    {
        var end = Value?.Length ?? 0;
        if (_module is null)
            return new(end, end);

        try
        {
            return await _module.InvokeAsync<CnTextSelection?>("getSelection", _textarea) ?? new(end, end);
        }
        catch (JSException)
        {
            return new(end, end);
        }
    }

    /// <summary>Puts the edited text in the textarea (keeping the browser's undo
    /// history where it can) and raises the value change once.</summary>
    private async Task CommitAsync(CnTextEdit edit)
    {
        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("apply", _textarea, edit.Text, edit.SelectionStart, edit.SelectionEnd);
            }
            catch (JSDisconnectedException)
            {
            }
        }

        await SetValueAsync(edit.Text);
    }

    public async ValueTask DisposeAsync()
    {
        _disposed = true;
        if (_module is not null)
        {
            try
            {
                await _module.InvokeVoidAsync("destroy", _textarea);
                await _module.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
            }
        }

        _dotNetRef?.Dispose();
        Dispose();
        GC.SuppressFinalize(this);
    }
}
