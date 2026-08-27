// Theme + accent runtime for the Cn design system (lifted from HCMT S-083).
// Hosts inline a minimal copy of this logic in index.html so the first paint
// already has the right theme; this file is the API CnThemeService drives
// afterwards.
window.cnTheme = (function () {
    const THEME_KEY = 'cn-theme';    // 'light' | 'dark' | 'system'
    const ACCENT_KEY = 'cn-accent';  // '#rrggbb'
    // Only a last resort for a malformed value. What a host opens in when
    // nothing is stored is the host's business, and arrives as an argument to
    // load() — assuming it here is how a blue app opened green.
    const FALLBACK_ACCENT = '#17af3d';

    function shade(hex, factor) {
        const n = parseInt(hex.slice(1), 16);
        const r = Math.round(((n >> 16) & 255) * factor);
        const g = Math.round(((n >> 8) & 255) * factor);
        const b = Math.round((n & 255) * factor);
        return '#' + ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1);
    }

    function applyAccent(accent) {
        if (!/^#[0-9a-fA-F]{6}$/.test(accent)) accent = FALLBACK_ACCENT;
        const root = document.documentElement.style;
        root.setProperty('--cn-accent', accent);
        root.setProperty('--cn-accent-hover', shade(accent, 0.78));
        root.setProperty('--cn-accent-tint', accent + '14'); // 8% alpha
    }

    function applyMode(mode) {
        const html = document.documentElement;
        if (mode === 'light' || mode === 'dark') {
            html.setAttribute('data-cn-theme', mode);
        } else {
            html.removeAttribute('data-cn-theme'); // system: media query decides
        }
        // Effective-theme hint for host splash screens that want to match
        // before the app boots (HCMT S-048 reads its own copy of this).
        const effective = mode === 'system' || !mode
            ? (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light')
            : mode;
        html.setAttribute('data-cn-splash-theme', effective);
    }

    return {
        load: function (defaultAccent) {
            const mode = localStorage.getItem(THEME_KEY) || 'system';
            const accent = localStorage.getItem(ACCENT_KEY) || defaultAccent || FALLBACK_ACCENT;
            applyMode(mode);
            applyAccent(accent);
            return { mode: mode, accent: accent };
        },
        apply: function (mode, accent) {
            localStorage.setItem(THEME_KEY, mode);
            localStorage.setItem(ACCENT_KEY, accent);
            applyMode(mode);
            applyAccent(accent);
        },
    };
})();
