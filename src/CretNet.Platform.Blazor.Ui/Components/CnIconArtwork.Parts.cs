namespace CretNet.Platform.Blazor.Ui.Components;

// Geometry of the drawn styles (Porcelain, Line) on a 24x24 grid, one entry per kind:
// Body   = closed silhouettes (porcelain fill with a tinted outline; stroked in Line),
// Detail = neutral inner lines and seams (grey in Porcelain),
// Accent = the one highlighted element (arrow, check, spark), in the accent colour.
// Own is the most saturated non-neutral material the Natural artwork paints (never paper,
// glass, steel, graphite or wood trim); icons with only neutral materials use their
// category colour. Category is the Category artwork colour.
// Edit the geometry here; CnIconArtwork.Drawn.cs renders both styles from it.
internal static partial class CnIconArtwork
{
    internal readonly record struct IconParts(string Body, string Detail, string Accent, string Own, string Category);

    internal static IconParts Parts(CnIconKind kind) => kind switch
    {
        CnIconKind.Add => new(
            "",
            "",
            """<path d="M12 5v14M5 12h14"/>""",
            "#358851", "#34945c"),
        CnIconKind.Archive => new(
            """<path d="M5 9h14v10H5z"/><path d="M3 5h18v4H3z"/>""",
            "",
            """<path d="M10 13h4"/>""",
            "#ca965b", "#eba545"),
        CnIconKind.ArrowDownload => new(
            """<rect x="4" y="18" width="16" height="3" rx="1.5"/>""",
            "",
            """<path d="M12 3.5v11M7.5 10l4.5 4.5 4.5-4.5"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.ArrowLeft => new(
            "",
            "",
            """<path d="M19 12H5M11 6l-6 6 6 6"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.ArrowRight => new(
            "",
            "",
            """<path d="M5 12h14M13 6l6 6-6 6"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.ArrowSync => new(
            "",
            "",
            """<path d="M4 11a8 8 0 0 1 14.2-4.2L20 8.5"/><path d="M20 3.5v5h-5"/><path d="M20 13a8 8 0 0 1-14.2 4.2L4 15.5"/><path d="M4 20.5v-5h5"/>""",
            "#458fdc", "#2daf9f"),
        CnIconKind.ArrowUndo => new(
            "",
            "",
            """<path d="M9 14L4 9l5-5"/><path d="M4 9h10.5a5.5 5.5 0 0 1 0 11H11"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.Bell => new(
            """<path d="M6 17V11a6 6 0 0 1 12 0v6l2 2H4z"/>""",
            "",
            """<path d="M10 21h4"/>""",
            "#e9b449", "#eba545"),
        CnIconKind.BookOpen => new(
            """<path d="M3 5.5c3-1.2 6-1 9 1V20c-3-2-6-2.2-9-1z"/><path d="M21 5.5c-3-1.2-6-1-9 1V20c3-2 6-2.2 9-1z"/>""",
            """<path d="M5.5 9c1.5-.4 3-.3 4.3.3M5.5 12c1.5-.4 3-.3 4.3.3M14.2 9.3c1.3-.6 2.8-.7 4.3-.3"/>""",
            """<path d="M14.2 12.3c1.3-.6 2.8-.7 4.3-.3"/>""",
            "#9162dc", "#9162dc"),
        CnIconKind.Box => new(
            """<path d="M12 3l8 4v10l-8 4-8-4V7z"/>""",
            """<path d="M4 7l8 4 8-4M12 11v10"/>""",
            """<path d="M8 5l8 4v3.5"/>""",
            "#ca965b", "#eba545"),
        CnIconKind.Briefcase => new(
            """<rect x="3" y="7" width="18" height="13" rx="2"/>""",
            """<path d="M3 12.5h18"/>""",
            """<path d="M9 7V5a1 1 0 0 1 1-1h4a1 1 0 0 1 1 1v2"/>""",
            "#e9b449", "#388ce7"),
        CnIconKind.Calculator => new(
            """<rect x="5" y="3" width="14" height="18" rx="2.2"/>""",
            """<rect x="7.6" y="11.6" width="2.4" height="2" rx=".6"/><rect x="10.8" y="11.6" width="2.4" height="2" rx=".6"/><rect x="14" y="11.6" width="2.4" height="2" rx=".6"/><rect x="7.6" y="15.6" width="2.4" height="2" rx=".6"/><rect x="10.8" y="15.6" width="2.4" height="2" rx=".6"/>""",
            """<rect x="7.6" y="5.6" width="8.8" height="3.8" rx="1"/><rect data-solid="1" x="14" y="15.6" width="2.4" height="2" rx=".6"/>""",
            "#4da96d", "#39ad64"),
        CnIconKind.CheckCircle => new(
            """<circle cx="12" cy="12" r="8.5"/>""",
            "",
            """<path d="M8 12.3l2.7 2.7L16 9.5"/>""",
            "#4da96d", "#39ad64"),
        CnIconKind.Checkmark => new(
            "",
            "",
            """<path d="M5 12.5l4.5 4.5L19 7.5"/>""",
            "#4da96d", "#39ad64"),
        CnIconKind.Clock => new(
            """<circle cx="12" cy="12" r="8.5"/>""",
            "",
            """<path d="M12 7v5l3 2"/>""",
            "#388ce7", "#388ce7"),
        CnIconKind.Coins => new(
            """<ellipse cx="9" cy="6.5" rx="5" ry="2.5"/><path d="M4 6.5v4c0 1.4 2.2 2.5 5 2.5s5-1.1 5-2.5v-4c0 1.4-2.2 2.5-5 2.5S4 7.9 4 6.5z"/>""",
            "",
            """<path d="M10 15.2c.8 1 2.8 1.8 5 1.8 2.8 0 5-1.1 5-2.5v-4c0-1.4-2.2-2.5-5-2.5"/><path d="M20 14.5v4c0 1.4-2.2 2.5-5 2.5s-5-1.1-5-2.5v-3"/>""",
            "#e9b449", "#eba545"),
        CnIconKind.Copy => new(
            """<rect x="8" y="8" width="12" height="12" rx="2"/>""",
            "",
            """<path d="M16 8V5a1 1 0 0 0-1-1H5a1 1 0 0 0-1 1v10a1 1 0 0 0 1 1h3"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Delete => new(
            """<path d="M6 7h12l-1 13H7z"/>""",
            """<path d="M10 11v6M14 11v6"/>""",
            """<path d="M4 7h16M9 7V4h6v3"/>""",
            "#ea6975", "#ea6975"),
        CnIconKind.Dismiss => new(
            "",
            "",
            """<path d="M6 6l12 12M18 6L6 18"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.Document => new(
            """<path d="M6 3h8l4 4v14H6z"/>""",
            """<path d="M9 11h6M9 14h6M9 17h4"/>""",
            """<path d="M14 3v4h4"/>""",
            "#388ce7", "#388ce7"),
        CnIconKind.DocumentAdd => new(
            """<path d="M6 3h8l4 4v4.5a4 4 0 0 0-5 6.5H6z"/>""",
            """<path d="M9 10h5M9 13h3"/>""",
            """<circle cx="17" cy="17.5" r="3.8"/><path d="M17 15.7v3.6M15.2 17.5h3.6"/>""",
            "#4da96d", "#39ad64"),
        CnIconKind.DocumentStack => new(
            """<path d="M4 7h9l3 3v11H4z"/>""",
            """<path d="M13 7v3h3M7 14h6M7 17h4"/>""",
            """<path d="M8 7V3h9l3 3v12h-4"/>""",
            "#efc46d", "#9162dc"),
        CnIconKind.Edit => new(
            """<path d="M4 20l.9-4L15 5.9l3.1 3.1L8 19.1z"/>""",
            "",
            """<path d="M15 5.9l1.6-1.6a1.4 1.4 0 0 1 2 0l1.1 1.1a1.4 1.4 0 0 1 0 2L18.1 9"/>""",
            "#e9b449", "#eba545"),
        CnIconKind.ErrorCircle => new(
            """<circle cx="12" cy="12" r="8.5"/>""",
            "",
            """<path d="M9 9l6 6M15 9l-6 6"/>""",
            "#df7865", "#ea6975"),
        CnIconKind.Folder => new(
            """<path d="M3 7a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>""",
            "",
            """<path d="M3 9.5h18"/>""",
            "#efc46d", "#eba545"),
        CnIconKind.Home => new(
            """<path d="M4.5 10.2L12 4.2l7.5 6V20h-15z"/>""",
            "",
            """<path d="M2.8 11.4L12 3.8l9.2 7.6"/><path d="M10 20v-4.8h4V20"/>""",
            "#df7865", "#39ad64"),
        CnIconKind.InfoCircle => new(
            """<circle cx="12" cy="12" r="8.5"/>""",
            "",
            """<path d="M12 11v5.5M12 7.8h.01"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Location => new(
            """<path d="M12 21s-6.5-6-6.5-11a6.5 6.5 0 0 1 13 0c0 5-6.5 11-6.5 11z"/>""",
            "",
            """<circle cx="12" cy="10" r="2.4"/>""",
            "#df7865", "#ea6975"),
        CnIconKind.Menu => new(
            "",
            "",
            """<path d="M4 6.5h16M4 12h16M4 17.5h16"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.PanelLeft => new(
            """<rect x="3" y="4" width="18" height="16" rx="2"/>""",
            """<path d="M12 8.5h6M12 11.5h4"/>""",
            """<path d="M9 4H5a2 2 0 0 0-2 2v12a2 2 0 0 0 2 2h4z"/>""",
            "#458fdc", "#7c91ac"),
        CnIconKind.People => new(
            """<circle cx="9" cy="8" r="3.2"/><path d="M3 20c.6-3.4 3-5.2 6-5.2s5.4 1.8 6 5.2z"/>""",
            "",
            """<circle cx="16.8" cy="9" r="2.5"/><path d="M15.7 14.3c2.8.2 4.7 1.9 5.3 4.7"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Play => new(
            "",
            "",
            """<path d="M8 5.5v13l10.5-6.5z"/>""",
            "#4da96d", "#39ad64"),
        CnIconKind.Save => new(
            """<path d="M4 5a2 2 0 0 1 2-2h10l4 4v12a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2z"/><rect x="7" y="13" width="10" height="8" rx="1"/>""",
            """<path d="M9.5 16.5h5"/>""",
            """<path d="M8 3v4.5h7V3M12.5 4.8v1"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Search => new(
            """<circle cx="10.5" cy="10.5" r="6.5"/>""",
            """<path d="M7.8 8.4a3.2 3.2 0 0 1 3-1.8"/>""",
            """<path d="M20 20l-4.8-4.8"/>""",
            "#388ce7", "#388ce7"),
        CnIconKind.Send => new(
            """<path d="M21 3L2.8 9.9l7.6 3.7L14.1 21z"/>""",
            "",
            """<path d="M21 3L10.4 13.6"/>""",
            "#388ce7", "#388ce7"),
        CnIconKind.Settings => new(
            """<path d="M10.60 4.94 L10.78 2.88 L13.22 2.88 L13.40 4.94 L16.00 6.01 L17.58 4.69 L19.31 6.42 L17.99 8.00 L19.06 10.60 L21.12 10.78 L21.12 13.22 L19.06 13.40 L17.99 16.00 L19.31 17.58 L17.58 19.31 L16.00 17.99 L13.40 19.06 L13.22 21.12 L10.78 21.12 L10.60 19.06 L8.00 17.99 L6.42 19.31 L4.69 17.58 L6.01 16.00 L4.94 13.40 L2.88 13.22 L2.88 10.78 L4.94 10.60 L6.01 8.00 L4.69 6.42 L6.42 4.69 L8.00 6.01Z"/>""",
            "",
            """<circle cx="12" cy="12" r="2.9"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.Shop => new(
            """<path d="M5 11.5h14V20H5z"/><path d="M3.5 9L5 4h14l1.5 5z"/>""",
            """<path d="M3.5 9a2.8 2.8 0 0 0 5.7 0 2.8 2.8 0 0 0 5.6 0 2.8 2.8 0 0 0 5.7 0"/>""",
            """<path d="M10 20v-4.5h4V20"/>""",
            "#4da96d", "#2daf9f"),
        CnIconKind.Stop => new(
            "",
            "",
            """<rect x="6" y="6" width="12" height="12" rx="2.5"/>""",
            "#df7865", "#ea6975"),
        CnIconKind.Subtract => new(
            "",
            "",
            """<path d="M5 12h14"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.SwapArrows => new(
            "",
            "",
            """<path d="M8 3L4 7l4 4M4 7h16"/><path d="M16 21l4-4-4-4M20 17H4"/>""",
            "#458fdc", "#2daf9f"),
        CnIconKind.Warning => new(
            """<path d="M12 4l9 16H3z"/>""",
            "",
            """<path d="M12 10v4M12 17h.01"/>""",
            "#e9b449", "#eba545"),
        CnIconKind.TaskCheck => new(
            """<rect x="5" y="4" width="14" height="17" rx="2"/>""",
            """<rect x="9" y="2.5" width="6" height="3.5" rx="1"/>""",
            """<path d="M8.5 13l2.5 2.5 4.5-5"/>""",
            "#ca965b", "#39ad64"),
        CnIconKind.DocumentSpark => new(
            """<path d="M9 6V3h8l3 3v9h-4"/><path d="M5 7h8l3 3v11H5z"/>""",
            """<path d="M8 14h5M8 17h3"/>""",
            """<path d="M19 15.3l.8 1.9 1.9.8-1.9.8-.8 1.9-.8-1.9-1.9-.8 1.9-.8z"/>""",
            "#e9b449", "#9162dc"),
        CnIconKind.DocumentTag => new(
            """<path d="M5 3h9l4 4v4h-4v10H5z"/>""",
            """<path d="M8 10h5M8 13h3"/>""",
            """<path d="M13.5 14.5h5l2.5 2.75-2.5 2.75h-5z"/><path d="M16 17.25h.01"/>""",
            "#e9b449", "#9162dc"),
        CnIconKind.DocumentFlow => new(
            """<rect x="3" y="3" width="8" height="6" rx="1.2"/><rect x="13" y="15" width="8" height="6" rx="1.2"/>""",
            """<path d="M15 17.3h4M15 19h2M5 5.3h4"/>""",
            """<path d="M7 9v4a2 2 0 0 0 2 2h4"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.ClipboardArrowRight => new(
            """<path d="M6 4h12a1 1 0 0 1 1 1v8h-5v8H6a1 1 0 0 1-1-1V5a1 1 0 0 1 1-1z"/>""",
            """<rect x="8" y="2.5" width="8" height="3.5" rx="1"/><path d="M8 10h7M8 13h4"/>""",
            """<path d="M14 18h7M18 15l3 3-3 3"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Truck => new(
            """<path d="M2 6h11v10H2z"/><path d="M13 9h4.2L21 13v3h-8z"/>""",
            """<path d="M15 10.5h1.8l1.8 2.5H15z"/>""",
            """<circle cx="6" cy="18" r="2"/><circle cx="17" cy="18" r="2"/>""",
            "#df7865", "#388ce7"),
        CnIconKind.InvoiceArrowRight => new(
            """<path d="M5 3h12v10h-3v6.3l-2 1.2-2.3-1.5L7.3 20.5 5 21z"/>""",
            """<path d="M8 8h6M8 11h4"/>""",
            """<path d="M14 17h7M18 14l3 3-3 3"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.ClipboardArrowLeft => new(
            """<path d="M6 4h12a1 1 0 0 1 1 1v8h-5v8H6a1 1 0 0 1-1-1V5a1 1 0 0 1 1-1z"/>""",
            """<rect x="8" y="2.5" width="8" height="3.5" rx="1"/><path d="M8 10h7M8 13h4"/>""",
            """<path d="M21 18h-7M17 15l-3 3 3 3"/>""",
            "#ca965b", "#2daf9f"),
        CnIconKind.BoxCheck => new(
            """<path d="M11 3l7 3.8v5l-4.5 7.4-2.5 1L4 15.4V6.8z"/>""",
            """<path d="M4 6.8l7 3.8 7-3.8M11 10.6v8.6"/>""",
            """<circle cx="17.5" cy="17.5" r="3.8"/><path d="M15.8 17.6l1.2 1.2 2.3-2.4"/>""",
            "#ca965b", "#2daf9f"),
        CnIconKind.InvoiceArrowLeft => new(
            """<path d="M5 3h12v10h-3v6.3l-2 1.2-2.3-1.5L7.3 20.5 5 21z"/>""",
            """<path d="M8 8h6M8 11h4"/>""",
            """<path d="M21 17h-7M17 14l-3 3 3 3"/>""",
            "#4da96d", "#2daf9f"),
        CnIconKind.Inbox => new(
            """<path d="M3 13l2.8-8h12.4L21 13v6a1 1 0 0 1-1 1H4a1 1 0 0 1-1-1z"/>""",
            """<path d="M8 8.5h8"/>""",
            """<path d="M3 13h5l1.2 3h5.6L16 13h5"/>""",
            "#efc46d", "#2daf9f"),
        CnIconKind.LinkedBlocks => new(
            """<rect x="3" y="4" width="8" height="7" rx="1.4"/><rect x="13" y="13" width="8" height="7" rx="1.4"/>""",
            """<path d="M5.5 7.5h3M15.5 16.5h3"/>""",
            """<path d="M11 7.5h3a2 2 0 0 1 2 2V13"/>""",
            "#458fdc", "#2daf9f"),
        CnIconKind.BoxLabel => new(
            """<path d="M12 3l8 4v10l-8 4-8-4V7z"/>""",
            """<path d="M4 7l8 4 8-4M12 11v10"/>""",
            """<path d="M14.5 14.3l3-1.5v2.8l-3 1.5z"/>""",
            "#ca965b", "#eba545"),
        CnIconKind.Boxes => new(
            """<rect x="3" y="12.5" width="8" height="7.5" rx="1"/><rect x="13" y="12.5" width="8" height="7.5" rx="1"/><rect x="8" y="4" width="8" height="7.5" rx="1"/>""",
            "",
            """<path d="M6 12.5v2.5M16 12.5v2.5M11 4v2.5"/>""",
            "#ca965b", "#eba545"),
        CnIconKind.BoxChecklist => new(
            """<rect x="5" y="3.5" width="14" height="17" rx="2"/>""",
            """<path d="M9 3.5V2.5h6v1M14 9h2M14 15h2"/>""",
            """<path d="M8 9l1.4 1.4L12 8M8 15l1.4 1.4L12 14"/>""",
            "#ca965b", "#eba545"),
        CnIconKind.Wrench => new(
            """<path d="M14.7 6.3a4.2 4.2 0 0 0-5.6 5.6L3.8 17.2l3 3 5.3-5.3a4.2 4.2 0 0 0 5.6-5.6l-2.5 2.5-2.7-.3-.3-2.7z"/>""",
            "",
            """<path d="M5.6 18.4h.01"/>""",
            "#eba545", "#eba545"),
        CnIconKind.FolderPlan => new(
            """<path d="M3 7a2 2 0 0 1 2-2h4l2 2h8a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2z"/>""",
            """<path d="M3 9h18"/>""",
            """<path d="M8 16.5l3-3 2 2 3.5-4"/>""",
            "#efc46d", "#eba545"),
        CnIconKind.TaskBoard => new(
            """<rect x="3" y="4" width="18" height="16" rx="2"/>""",
            """<path d="M9 4v16M15 4v16M5.3 8h1.5M17.2 8h1.5"/>""",
            """<path d="M11 8h2M11 11.5h2"/>""",
            "#e9b449", "#eba545"),
        CnIconKind.CalendarClock => new(
            """<path d="M5 5h14a2 2 0 0 1 2 2v4.5l-4 1.5-3.5 7H5a2 2 0 0 1-2-2V7a2 2 0 0 1 2-2z"/>""",
            """<path d="M8 3v4M16 3v4M3 10h11"/>""",
            """<circle cx="17.5" cy="17.5" r="3.8"/><path d="M17.5 15.8v1.9l1.2.9"/>""",
            "#df7865", "#eba545"),
        CnIconKind.Wallet => new(
            """<path d="M4 7h14a2 2 0 0 1 2 2v9a2 2 0 0 1-2 2H6a2 2 0 0 1-2-2z"/>""",
            """<path d="M4 7.5L15 4v3"/>""",
            """<path d="M20 11.5h-3.5a2 2 0 0 0 0 4H20z"/>""",
            "#e9b449", "#39ad64"),
        CnIconKind.CreditCard => new(
            """<rect x="3" y="5.5" width="18" height="13" rx="2"/>""",
            """<path d="M7 15h3"/>""",
            """<path d="M3 10h18"/>""",
            "#458fdc", "#39ad64"),
        CnIconKind.Transfer => new(
            "",
            "",
            """<path d="M4 8h15M15 4l4 4-4 4"/><path d="M20 16H5M9 12l-4 4 4 4"/>""",
            "#458fdc", "#39ad64"),
        CnIconKind.DocumentMatch => new(
            """<path d="M6 3h8l4 4v4.5a4 4 0 0 0-5 6.5H6z"/>""",
            """<path d="M9 10h5M9 13h3"/>""",
            """<circle cx="17" cy="17.5" r="3.8"/><path d="M15.3 17.6l1.2 1.2 2.3-2.4"/>""",
            "#efc46d", "#39ad64"),
        CnIconKind.IdBadge => new(
            """<rect x="4" y="4" width="16" height="17" rx="2"/>""",
            """<path d="M9.5 4V2.5h5V4"/>""",
            """<circle cx="12" cy="10.5" r="2.5"/><path d="M8 17.5c.5-2 2-3.2 4-3.2s3.5 1.2 4 3.2"/>""",
            "#458fdc", "#7c91ac"),
        CnIconKind.LedgerExport => new(
            """<path d="M5 3h11a2 2 0 0 1 2 2v8.5l-3.5 3.5V21H5z"/>""",
            """<path d="M9 3v18"/>""",
            """<path d="M15 18h6M18 15l3 3-3 3"/>""",
            "#458fdc", "#7c91ac"),
        CnIconKind.More => new(
            "",
            "",
            """<circle cx="5" cy="12" r="1.2"/><circle cx="12" cy="12" r="1.2"/><circle cx="19" cy="12" r="1.2"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.Link => new(
            "",
            "",
            """<path d="M10 14a4 4 0 0 0 5.7 0l3-3a4 4 0 0 0-5.7-5.7l-1 1"/><path d="M14 10a4 4 0 0 0-5.7 0l-3 3a4 4 0 0 0 5.7 5.7l1-1"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Assistant => new(
            "",
            "",
            """<path d="M10 4.5c.8 4.9 2.8 7.7 8.5 8.5c-5.7 .8-7.7 3.6-8.5 8.5c-.8-4.9-2.8-7.7-8.5-8.5c5.7-.8 7.7-3.6 8.5-8.5z"/><path data-solid="1" d="M18.5 1.5c.33 2.02 1.15 3.17 3.5 3.5c-2.35 .33-3.17 1.48-3.5 3.5c-.33-2.02-1.15-3.17-3.5-3.5c2.35-.33 3.17-1.48 3.5-3.5z"/>""",
            "#8a63d2", "#9162dc"),
        CnIconKind.History => new(
            """<circle cx="14.5" cy="13.5" r="6.5"/>""",
            """<path d="M14.5 10.5v3.2l2.3 1.4"/>""",
            """<path d="M20.6 6.2A9.5 9.5 0 0 0 5 13.5"/><path d="M2.5 11l2.5 2.5L7.5 11"/>""",
            "#458fdc", "#388ce7"),
        CnIconKind.Compose => new(
            """<path d="M16.96 4.5H6a2.5 2.5 0 0 0-2.5 2.5v11A2.5 2.5 0 0 0 6 20.5h11a2.5 2.5 0 0 0 2.5-2.5V7.04l-6.61 6.61-3.39.85.85-3.39z"/>""",
            """<path d="M6.5 17.5h5"/>""",
            """<path d="M9.5 14.5l.85-3.39 8.84-8.84 2.54 2.54-8.84 8.84z"/><path d="M17.42 4.04l2.54 2.54"/>""",
            "#e9b449", "#9162dc"),
        CnIconKind.Shield => new(
            """<path d="M12 3l7.5 2.8v5.7c0 4.6-3.2 8-7.5 9.5-4.3-1.5-7.5-4.9-7.5-9.5V5.8z"/>""",
            "",
            """<path d="M8.8 12.2l2.2 2.2 4.3-4.6"/>""",
            "#458fdc", "#7c91ac"),
        CnIconKind.ChevronDown => new(
            "",
            "",
            """<path d="M6 9l6 6 6-6"/>""",
            "#7c91ac", "#7c91ac"),
        CnIconKind.LayoutNarrow => new(
            """<rect x="3" y="4" width="18" height="16" rx="2"/>""",
            """<path d="M6 8.5h5M6 11.5h3"/>""",
            """<path d="M15 4h4a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-4z"/>""",
            "#8a63d2", "#9162dc"),
        CnIconKind.LayoutHalf => new(
            """<rect x="3" y="4" width="18" height="16" rx="2"/>""",
            """<path d="M6 8.5h3M6 11.5h2"/>""",
            """<path d="M12 4h7a2 2 0 0 1 2 2v12a2 2 0 0 1-2 2h-7z"/>""",
            "#8a63d2", "#9162dc"),
        CnIconKind.LayoutFull => new(
            "",
            "",
            """<rect x="3" y="4" width="18" height="16" rx="2"/><path d="M8.5 4v16"/><path data-solid="1" d="M14.8 8c.38 2.31 1.32 3.62 4 4c-2.68 .38-3.62 1.69-4 4c-.38-2.31-1.32-3.62-4-4c2.68-.38 3.62-1.69 4-4z"/>""",
            "#8a63d2", "#9162dc"),
        CnIconKind.LayoutWindow => new(
            """<rect x="3" y="4" width="18" height="16" rx="2"/>""",
            """<path d="M6 6.5h5"/>""",
            """<rect x="6.5" y="8.5" width="11" height="9" rx="1.5"/><path d="M6.5 11h11"/>""",
            "#8a63d2", "#9162dc"),
        _ => default,
    };
}
