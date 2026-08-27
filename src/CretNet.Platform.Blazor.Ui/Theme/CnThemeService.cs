using Microsoft.JSInterop;

namespace CretNet.Platform.Blazor.Ui.Theme;

public enum CnThemeMode
{
    System,
    Light,
    Dark,
}

/// <summary>
/// Per-user theme (light/dark/system) and accent colour for the Cn design
/// system. Values persist in localStorage; nothing is hardcoded — the accent
/// drives the --cn-accent custom properties at runtime. Lifted from
/// HCMT.WasmApp/Ui/Theme (S-083) verbatim; the host serves the companion
/// cn-theme.js (a static web asset of this package) and calls
/// <see cref="InitializeAsync"/> once at boot.
/// </summary>
public class CnThemeService
{
    /// <summary>
    /// The accent a host gets when it does not name one. HCMT's brand green,
    /// because that is where this component came from — every other host is
    /// expected to say what its own accent is.
    /// </summary>
    public const string DefaultAccent = "#17af3d";

    private readonly IJSRuntime _jsRuntime;
    private readonly string _fallbackAccent;

    /// <param name="fallbackAccent">
    /// This host's accent, used until the user picks one. Without it a host
    /// with a blue brand would open green on every first visit.
    /// </param>
    public CnThemeService(IJSRuntime jsRuntime, string? fallbackAccent = null)
    {
        _jsRuntime = jsRuntime;
        _fallbackAccent = string.IsNullOrWhiteSpace(fallbackAccent) ? DefaultAccent : fallbackAccent;
        Accent = _fallbackAccent;
    }

    public CnThemeMode Mode { get; private set; } = CnThemeMode.System;
    public string Accent { get; private set; }

    public event Action? Changed;

    public async Task InitializeAsync()
    {
        // The script paints before this returns, so it needs the host's accent
        // rather than falling back to one of its own.
        var stored = await _jsRuntime.InvokeAsync<StoredTheme>("cnTheme.load", _fallbackAccent);
        Mode = Enum.TryParse<CnThemeMode>(stored.Mode, true, out var mode) ? mode : CnThemeMode.System;
        Accent = stored.Accent ?? _fallbackAccent;
        Changed?.Invoke();
    }

    public Task SetModeAsync(CnThemeMode mode) => ApplyAsync(mode, Accent);

    public Task SetAccentAsync(string accent) => ApplyAsync(Mode, accent);

    private async Task ApplyAsync(CnThemeMode mode, string accent)
    {
        Mode = mode;
        Accent = accent;
        await _jsRuntime.InvokeVoidAsync("cnTheme.apply", mode.ToString().ToLowerInvariant(), accent);
        Changed?.Invoke();
    }

    private sealed record StoredTheme(string? Mode, string? Accent);
}
