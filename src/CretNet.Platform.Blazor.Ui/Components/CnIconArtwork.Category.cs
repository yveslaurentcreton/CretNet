namespace CretNet.Platform.Blazor.Ui.Components;

// Canonical vector artwork promoted from the accepted icon proposal.
// Edit the SVG geometry/paint here; instance prefixes are replaced by CnIcon.
internal static partial class CnIconArtwork
{
    internal static string Category(CnIconKind kind) => kind switch
    {
        CnIconKind.Home => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M10 22 24 9 38 22V41H10Z" fill="url(#__CN_ICON__d)" />
            <rect x="12" y="20" width="24" height="22" rx="2" fill="url(#__CN_ICON__p)" />
            <path d="M4 22 22 5Q24 3 26 5L44 22 40 27 24 12 8 27Z" fill="url(#__CN_ICON__g)" />
            <rect x="21" y="28" width="8" height="14" rx="1" fill="url(#__CN_ICON__d)" />
            <rect x="15" y="25" width="4" height="5" rx="1" fill="#99e4b1" />
            <path d="M8 22 24 7 40 22" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".5"/>
            </g>
            """,
        CnIconKind.DocumentSpark => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop class="cn-icon-tone-hi" stop-color="#c6acff"/>
            <stop offset=".55" stop-color="#9162dc"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#9162dc"/>
            <stop offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#c6acff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#5b389e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="15" y="4" width="26" height="32" rx="4" fill="url(#__CN_ICON__d)" />
            <rect x="10" y="9" width="26" height="32" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="14" width="26" height="29" rx="4" fill="url(#__CN_ICON__p)" />
            <rect x="9" y="19" width="14" height="4" rx="1.5" fill="#c6acff" />
            <path d="M9 28H24 M9 33H20" fill="none" stroke="#9162dc" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#5b389e" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <path d="M35 24 38 31 45 34 38 37 35 44 32 37 25 34 32 31Z" fill="#fff" />
            </g>
            """,
        CnIconKind.DocumentTag => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop class="cn-icon-tone-hi" stop-color="#c6acff"/>
            <stop offset=".55" stop-color="#9162dc"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#9162dc"/>
            <stop offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#c6acff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#5b389e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M13 5H30L38 13V39Q38 43 34 43H13Q9 43 9 39V9Q9 5 13 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M14 7H29L36 14V38Q36 41 33 41H14Q11 41 11 38V10Q11 7 14 7Z" fill="url(#__CN_ICON__p)" />
            <path d="M29 7V14H36" fill="#c6acff" />
            <path d="M16 19H28 M16 24H28 M16 29H23" fill="none" stroke="#9162dc" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <g transform="rotate(-15 34 33)">
            <path d="M26 23H39L46 30V37L39 43H26Q23 43 23 40V26Q23 23 26 23Z" fill="url(#__CN_ICON__g)" />
            <circle cx="39" cy="32" r="2" fill="url(#__CN_ICON__p)" />
            <path d="M28 29V36 M31 27V34" fill="none" stroke="#fff" stroke-width="2.7" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.BookOpen => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop class="cn-icon-tone-hi" stop-color="#c6acff"/>
            <stop offset=".55" stop-color="#9162dc"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#9162dc"/>
            <stop offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#c6acff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#5b389e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 9Q14 4 24 9Q34 4 43 9V39Q34 35 24 40Q14 35 5 39Z" fill="url(#__CN_ICON__d)" />
            <path d="M7 7Q16 3 24 9V36Q16 31 7 35Z" fill="url(#__CN_ICON__p)" />
            <path d="M24 9Q32 3 41 7V35Q32 31 24 36Z" fill="url(#__CN_ICON__p)" />
            <path d="M24 10V37" fill="none" stroke="#9162dc" stroke-width="2.3" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M11 14Q15 12 19 14 M11 20Q15 18 19 20 M29 14Q33 12 37 14 M29 20Q33 18 37 20" fill="none" stroke="#c6acff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.People => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(32 12) scale(0.8)">
            <circle cx="0" cy="0" r="7" fill="url(#__CN_ICON__g)" />
            <path d="M-12 22V18C-12 10-8 8 0 8S12 10 12 18V22Q0 27-12 22Z" fill="url(#__CN_ICON__g)" />
            <path d="M-10 17C-9 11-5 10 0 10" fill="#fff" opacity=".15"/>
            </g>
            <g stroke="#fff" stroke-width="1">
            <g transform="translate(18 16) scale(1)">
            <circle cx="0" cy="0" r="7" fill="url(#__CN_ICON__g)" />
            <path d="M-12 22V18C-12 10-8 8 0 8S12 10 12 18V22Q0 27-12 22Z" fill="url(#__CN_ICON__g)" />
            <path d="M-10 17C-9 11-5 10 0 10" fill="#fff" opacity=".15"/>
            </g>
            </g>
            </g>
            """,
        CnIconKind.DocumentFlow => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M12 15H35V34H16" fill="none" stroke="#2458a8" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
            <rect x="4" y="4" width="17" height="20" rx="3" fill="url(#__CN_ICON__g)" />
            <rect x="25" y="24" width="17" height="20" rx="3" fill="url(#__CN_ICON__p)" stroke="#388ce7" stroke-width="1.3"/>
            <path d="M9 10H15 M9 15H14" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M29 31H37 M29 36H35" fill="none" stroke="#388ce7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M32 19 35 24 38 19Z" fill="#2458a8" />
            </g>
            """,
        CnIconKind.ClipboardArrowRight => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="9" y="8" width="28" height="35" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="12" y="11" width="22" height="29" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="18" y="5" width="11" height="8" rx="3" fill="url(#__CN_ICON__d)" />
            <rect x="20" y="5" width="7" height="3" rx="1" fill="#92cdff" />
            <path d="M17 21H28 M17 27H26" fill="none" stroke="#388ce7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#2458a8" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 34) ">
            <path d="M-5 0H5 M1-4 5 0 1 4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.Truck => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="3" y="11" width="26" height="24" rx="4" fill="url(#__CN_ICON__g)" />
            <path d="M29 19H37L44 27V35H29Z" fill="url(#__CN_ICON__d)" />
            <path d="M32 22H36L41 28H32Z" fill="url(#__CN_ICON__p)" />
            <rect x="4" y="33" width="39" height="5" rx="2" fill="#2458a8" />
            <circle cx="12" cy="37" r="6" fill="#344459" />
            <circle cx="12" cy="37" r="2.5" fill="url(#__CN_ICON__p)" />
            <circle cx="36" cy="37" r="6" fill="#344459" />
            <circle cx="36" cy="37" r="2.5" fill="url(#__CN_ICON__p)" />
            <path d="M7 15H25" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".55"/>
            </g>
            """,
        CnIconKind.InvoiceArrowRight => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M8 5H33Q36 5 36 8V43L31 39 26 43 21 39 16 43 11 39 7 43V9Q7 5 8 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M11 8H32V34H11Z" fill="url(#__CN_ICON__p)" />
            <path d="M15 14H28 M15 19H24" fill="none" stroke="#388ce7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M15 26H23" fill="none" stroke="#2458a8" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#2458a8" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 34) ">
            <path d="M-5 0H5 M1-4 5 0 1 4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.Shop => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="7" y="18" width="34" height="25" rx="3" fill="url(#__CN_ICON__p)" stroke="#2daf9f" stroke-width="1.2"/>
            <rect x="12" y="26" width="9" height="11" rx="1" fill="url(#__CN_ICON__glass)" />
            <rect x="26" y="25" width="10" height="18" rx="1" fill="url(#__CN_ICON__d)" />
            <path d="M8 6H40L44 19H4Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 6H21L20 19H12Z M29 6H35L37 19H29Z" fill="url(#__CN_ICON__p)" />
            <path d="M4 19Q8 25 12 19Q16 25 20 19Q24 25 28 19Q32 25 36 19Q40 25 44 19" fill="url(#__CN_ICON__d)" />
            <path d="M8 7H39" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".4"/>
            </g>
            """,
        CnIconKind.ClipboardArrowLeft => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="9" y="8" width="28" height="35" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="12" y="11" width="22" height="29" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="18" y="5" width="11" height="8" rx="3" fill="url(#__CN_ICON__d)" />
            <rect x="20" y="5" width="7" height="3" rx="1" fill="#91e4dc" />
            <path d="M17 21H28 M17 27H26" fill="none" stroke="#2daf9f" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#17766e" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 34) rotate(180)">
            <path d="M-5 0H5 M1-4 5 0 1 4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.BoxCheck => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(5 6) scale(0.96)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#91e4dc" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <path d="M4 33H15L19 38H29L33 33H44V43H4Z" fill="url(#__CN_ICON__d)" />
            <circle cx="35" cy="34" r="10" fill="#17766e" class="cn-icon-bevel"/>
            <circle cx="35" cy="33" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 33) scale(1)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.InvoiceArrowLeft => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M8 5H33Q36 5 36 8V43L31 39 26 43 21 39 16 43 11 39 7 43V9Q7 5 8 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M11 8H32V34H11Z" fill="url(#__CN_ICON__p)" />
            <path d="M15 14H28 M15 19H24" fill="none" stroke="#2daf9f" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M15 26H23" fill="none" stroke="#17766e" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#17766e" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 34) rotate(180)">
            <path d="M-5 0H5 M1-4 5 0 1 4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.Inbox => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="16" y="4" width="22" height="26" rx="3" fill="url(#__CN_ICON__d)" />
            <rect x="10" y="8" width="22" height="26" rx="3" fill="url(#__CN_ICON__p)" stroke="#91e4dc" stroke-width="1.5"/>
            <path d="M15 15H25 M15 21H23" fill="none" stroke="#2daf9f" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M8 24H38L44 36H3Z" fill="#91e4dc" />
            <path d="M3 33H16L20 38H28L32 33H44V41Q44 44 40 44H7Q3 44 3 40Z" fill="url(#__CN_ICON__g)" />
            <path d="M7 34H15" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".65"/>
            </g>
            """,
        CnIconKind.LinkedBlocks => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M13 15 34 15 34 35 14 35" fill="none" stroke="#17766e" stroke-width="4" stroke-linecap="round" stroke-linejoin="round" />
            <rect x="3" y="5" width="20" height="20" rx="5" fill="url(#__CN_ICON__g)" />
            <rect x="25" y="5" width="20" height="20" rx="5" fill="url(#__CN_ICON__p)" stroke="#2daf9f" stroke-width="1.5"/>
            <rect x="4" y="27" width="20" height="17" rx="4" fill="url(#__CN_ICON__g)" />
            <path d="M10 15H16 M32 15H38 M11 35H17" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M32 15H38" fill="none" stroke="#2daf9f" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.BoxLabel => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(6 6) scale(1.08)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <rect x="8" y="26" width="12" height="10" rx="2" fill="url(#__CN_ICON__p)" />
            <path d="M11 29V33 M14 29V33 M17 29V33" fill="none" stroke="#aa651e" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Boxes => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(15 3) scale(0.64)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <g transform="translate(2 19) scale(0.69)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <g transform="translate(25 19) scale(0.69)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <rect x="3" y="41" width="43" height="3" rx="1" fill="#aa651e" />
            </g>
            """,
        CnIconKind.BoxChecklist => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(3 14) scale(0.82)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            <rect x="24" y="5" width="20" height="33" rx="3" fill="url(#__CN_ICON__p)" stroke="#eba545" stroke-width="1.5"/>
            <rect x="29" y="3" width="11" height="6" rx="2" fill="url(#__CN_ICON__g)" />
            <g transform="translate(33 19) scale(0.85)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#aa651e" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            <path d="M29 28H38 M29 32H35" fill="none" stroke="#eba545" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Wrench => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M30 5C22 4 17 11 19 18L5 32Q1 36 6 41Q10 45 14 41L28 26C36 28 43 22 42 14L35 21 28 19 27 12 34 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M6 36 22 20 25 23 10 41Z" fill="url(#__CN_ICON__d)" />
            <circle cx="10" cy="36" r="2" fill="url(#__CN_ICON__p)" />
            <path d="M22 14 24 22 32 25" fill="none" stroke="#fff" stroke-width="1.4" stroke-linecap="round" stroke-linejoin="round" opacity=".4"/>
            </g>
            """,
        CnIconKind.FolderPlan => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 13Q5 9 9 9H20L24 13H39Q43 13 43 17V38H5Z" fill="url(#__CN_ICON__d)" />
            <path d="M5 18H43L40 39Q40 42 36 42H10Q6 42 6 38Z" fill="url(#__CN_ICON__g)" />
            <path d="M9 19H39" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".55"/>
            <rect x="22" y="19" width="19" height="23" rx="3" fill="url(#__CN_ICON__p)" />
            <path d="M27 25H36 M27 30H33" fill="none" stroke="#eba545" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M27 38V34H30V31H34V27H37" fill="#aa651e" />
            </g>
            """,
        CnIconKind.TaskBoard => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="4" y="7" width="40" height="35" rx="5" fill="url(#__CN_ICON__g)" />
            <rect x="8" y="12" width="14" height="25" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="26" y="12" width="14" height="16" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="11" y="16" width="8" height="3" rx="1" fill="#ffdb8e" />
            <rect x="29" y="16" width="8" height="3" rx="1" fill="#ffdb8e" />
            <path d="M11 24H18 M11 29H16" fill="none" stroke="#eba545" stroke-width="1.7" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="36" r="10" fill="#aa651e" class="cn-icon-bevel"/>
            <circle cx="35" cy="35" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 35) scale(0.8)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.CalendarClock => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="5" y="7" width="34" height="34" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="8" y="16" width="28" height="22" rx="2" fill="url(#__CN_ICON__p)" />
            <path d="M13 5V12 M30 5V12" fill="none" stroke="#aa651e" stroke-width="3" stroke-linecap="round" stroke-linejoin="round" />
            <rect x="12" y="21" width="5" height="5" rx="1" fill="#ffdb8e" />
            <rect x="21" y="21" width="5" height="5" rx="1" fill="#ffdb8e" />
            <rect x="12" y="29" width="5" height="5" rx="1" fill="#ffdb8e" />
            <circle cx="34" cy="35" r="12" fill="#aa651e" class="cn-icon-bevel"/>
            <circle cx="34" cy="34" r="12" fill="url(#__CN_ICON__g)" />
            <circle cx="34" cy="34" r="9" fill="url(#__CN_ICON__p)" />
            <path d="M34 29V34L40 38" fill="none" stroke="#aa651e" stroke-width="2.8" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="34" cy="34" r="1.8" fill="#eba545" />
            </g>
            """,
        CnIconKind.Wallet => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M9 16 32 5 39 22Z" fill="url(#__CN_ICON__d)" />
            <rect x="12" y="3" width="18" height="26" rx="3" fill="url(#__CN_ICON__p)" transform="rotate(13 21 16)" stroke="#99e4b1" stroke-width="1.5"/>
            <path d="M18 10 26 12 M17 15 25 17" fill="none" stroke="#39ad64" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <rect x="4" y="17" width="38" height="26" rx="5" fill="url(#__CN_ICON__g)" />
            <rect x="27" y="26" width="18" height="11" rx="3" fill="url(#__CN_ICON__d)" />
            <circle cx="33" cy="31.5" r="2" fill="url(#__CN_ICON__p)" />
            </g>
            """,
        CnIconKind.CreditCard => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="5" y="6" width="37" height="28" rx="4" fill="url(#__CN_ICON__d)" transform="rotate(-8 24 20)"/>
            <rect x="3" y="14" width="42" height="29" rx="5" fill="url(#__CN_ICON__g)" />
            <rect x="3" y="19" width="42" height="6" rx="0" fill="#227847" opacity=".6"/>
            <rect x="9" y="29" width="9" height="7" rx="1.6" fill="#f9e6b8" />
            <path d="M13 30V35 M10 32H17" fill="none" stroke="#b89958" stroke-width="0.8" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M27 34H37" fill="none" stroke="#fff" stroke-width="2.6" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Transfer => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 10H30V4L44 17 30 29V22H5Z" fill="url(#__CN_ICON__g)" />
            <path d="M43 28H18V22L4 35 18 47V40H43Z" fill="url(#__CN_ICON__d)" />
            <path d="M8 12H29" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".5"/>
            </g>
            """,
        CnIconKind.DocumentMatch => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="4" y="6" width="23" height="34" rx="4" fill="url(#__CN_ICON__d)" />
            <rect x="22" y="10" width="23" height="34" rx="4" fill="url(#__CN_ICON__p)" stroke="#39ad64" stroke-width="1.5"/>
            <path d="M10 13H20 M10 18H18" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M29 17H38 M29 22H37" fill="none" stroke="#39ad64" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="24" cy="35" r="10" fill="#227847" class="cn-icon-bevel"/>
            <circle cx="24" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(24 34) scale(1)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.IdBadge => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="6" y="9" width="36" height="34" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="10" y="13" width="28" height="26" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="18" y="4" width="12" height="10" rx="3" fill="url(#__CN_ICON__d)" />
            <circle cx="19" cy="23" r="4" fill="#7c91ac" />
            <path d="M12 35Q12 28 19 28T26 35Z" fill="url(#__CN_ICON__g)" />
            <path d="M29 24H33 M29 29H33" fill="none" stroke="#7c91ac" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.LedgerExport => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="7" y="5" width="30" height="38" rx="4" fill="url(#__CN_ICON__d)" />
            <rect x="12" y="6" width="25" height="35" rx="3" fill="url(#__CN_ICON__p)" />
            <path d="M18 13H30 M18 19H30 M18 25H25" fill="none" stroke="#7c91ac" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M9 10V35" fill="none" stroke="#bbcada" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#4b6081" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <g transform="translate(35 34) ">
            <path d="M-5 0H5 M1-4 5 0 1 4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.Settings => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <polygon points="24.00,3.00 27.32,7.33 30.51,8.29 35.67,6.54 38.85,9.15 38.13,14.56 39.71,17.49 44.60,19.90 45.00,24.00 40.67,27.32 39.71,30.51 41.46,35.67 38.85,38.85 33.44,38.13 30.51,39.71 28.10,44.60 24.00,45.00 20.68,40.67 17.49,39.71 12.33,41.46 9.15,38.85 9.87,33.44 8.29,30.51 3.40,28.10 3.00,24.00 7.33,20.68 8.29,17.49 6.54,12.33 9.15,9.15 14.56,9.87 17.49,8.29 19.90,3.40" fill="url(#__CN_ICON__d)" transform="translate(0 1.4)" class="cn-icon-bevel"/>
            <polygon points="24.00,3.00 27.32,7.33 30.51,8.29 35.67,6.54 38.85,9.15 38.13,14.56 39.71,17.49 44.60,19.90 45.00,24.00 40.67,27.32 39.71,30.51 41.46,35.67 38.85,38.85 33.44,38.13 30.51,39.71 28.10,44.60 24.00,45.00 20.68,40.67 17.49,39.71 12.33,41.46 9.15,38.85 9.87,33.44 8.29,30.51 3.40,28.10 3.00,24.00 7.33,20.68 8.29,17.49 6.54,12.33 9.15,9.15 14.56,9.87 17.49,8.29 19.90,3.40" fill="url(#__CN_ICON__g)"/>
            <circle cx="24" cy="24" r="10" fill="#4b6081" />
            <circle cx="24" cy="23" r="7" fill="url(#__CN_ICON__p)" />
            <circle cx="23" cy="21" r="5" fill="#fff" opacity=".6"/>
            </g>
            """,
        CnIconKind.Add => """
            <defs>
              <linearGradient id="__CN_ICON__-add" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#34945c">
                <stop class="cn-icon-tone-hi" stop-color="#51b578"/>
                <stop offset=".5" stop-color="#34945c"/>
                <stop class="cn-icon-tone-lo" offset="1" stop-color="#236c43"/>
              </linearGradient>
            </defs>
            <path class="cn-icon-bevel" d="M24 9v30M9 24h30" transform="translate(0 1)" fill="none" stroke="#236c43" stroke-width="4.5" stroke-linecap="round" opacity=".18"/>
            <path d="M24 9v30M9 24h30" fill="none" stroke="url(#__CN_ICON__-add)" stroke-width="4.5" stroke-linecap="round"/>
            """,
        CnIconKind.Archive => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="7" y="14" width="34" height="28" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="4" y="7" width="40" height="10" rx="3" fill="#ffdb8e" />
            <rect x="17" y="23" width="14" height="6" rx="2" fill="url(#__CN_ICON__d)" />
            <path d="M20 25H28" fill="none" stroke="#fff" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.ArrowDownload => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(4 -1) scale(.82)">
            <g transform="rotate(-90 24 24)">
            <path d="M20 5 3 23 20 42V30H44V17H20Z" fill="url(#__CN_ICON__g)" />
            <path d="M8 23 18 12" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".4"/>
            </g>
            </g>
            <rect x="5" y="39" width="38" height="6" rx="2" fill="url(#__CN_ICON__d)" />
            </g>
            """,
        CnIconKind.ArrowLeft => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M20 5 3 23 20 42V30H44V17H20Z" fill="url(#__CN_ICON__g)" />
            <path d="M8 23 18 12" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".4"/>
            </g>
            """,
        CnIconKind.ArrowRight => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="rotate(180 24 24)">
            <path d="M20 5 3 23 20 42V30H44V17H20Z" fill="url(#__CN_ICON__g)" />
            <path d="M8 23 18 12" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".4"/>
            </g>
            </g>
            """,
        CnIconKind.ArrowSync => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M7 25A17 17 0 0 1 34 9L40 3V23H20L27 16A9 9 0 0 0 15 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M41 24A17 17 0 0 1 14 40L8 46V26H28L21 33A9 9 0 0 0 33 24Z" fill="url(#__CN_ICON__d)" />
            </g>
            """,
        CnIconKind.ArrowUndo => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M20 5 3 19 20 34V24H29Q36 24 36 34V40H44V32Q44 15 29 15H20Z" fill="url(#__CN_ICON__g)" />
            </g>
            """,
        CnIconKind.Bell => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M10 31V21Q10 9 23 8V5H27V8Q39 10 39 21V31L44 36H5Z" fill="url(#__CN_ICON__g)" />
            <path d="M5 35H44V38H5Z" fill="url(#__CN_ICON__d)" />
            <path d="M19 40Q24 49 30 40Z" fill="url(#__CN_ICON__d)" />
            <path d="M15 26V21Q15 14 21 13" fill="none" stroke="#fff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" opacity=".55"/>
            </g>
            """,
        CnIconKind.Box => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="translate(6 6) scale(1.1)">
            <path d="M0 8 15 0 30 8 15 16Z" fill="#ffdb8e" />
            <path d="M0 8 15 16V33L0 25Z" fill="url(#__CN_ICON__g)" />
            <path d="M15 16 30 8V25L15 33Z" fill="url(#__CN_ICON__d)" />
            <path d="M9 3 24 11V17L20 19V13L5 6Z" fill="#fff" opacity=".55"/>
            <path d="M2 9 15 16 28 9" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".38"/>
            </g>
            </g>
            """,
        CnIconKind.Briefcase => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M16 15V8Q16 5 19 5H30Q33 5 33 8V15" fill="url(#__CN_ICON__d)" />
            <rect x="19" y="8" width="11" height="9" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="4" y="14" width="40" height="29" rx="5" fill="url(#__CN_ICON__d)" />
            <rect x="4" y="13" width="40" height="17" rx="5" fill="url(#__CN_ICON__g)" />
            <path d="M8 28H40" fill="none" stroke="#92cdff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" />
            <rect x="21" y="25" width="7" height="8" rx="2" fill="url(#__CN_ICON__p)" />
            </g>
            """,
        CnIconKind.Calculator => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="7" y="3" width="34" height="42" rx="6" fill="url(#__CN_ICON__p)" stroke="#a4b8ac" stroke-width="1.2"/>
            <rect x="12" y="9" width="24" height="10" rx="2.5" fill="#e3f5e9" stroke="#39ad64" stroke-width="1.3"/>
            <rect x="12" y="24" width="6" height="5" rx="1.5" fill="#bdc9c5" />
            <rect x="21" y="24" width="6" height="5" rx="1.5" fill="#bdc9c5" />
            <rect x="30" y="24" width="6" height="5" rx="1.5" fill="#bdc9c5" />
            <rect x="12" y="33" width="6" height="5" rx="1.5" fill="#bdc9c5" />
            <rect x="21" y="33" width="6" height="5" rx="1.5" fill="#bdc9c5" />
            <rect x="30" y="33" width="6" height="5" rx="1.5" fill="url(#__CN_ICON__g)" />
            </g>
            """,
        CnIconKind.CheckCircle => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <circle cx="24" cy="25" r="20" fill="#227847" class="cn-icon-bevel"/>
            <circle cx="24" cy="23" r="20" fill="url(#__CN_ICON__g)" />
            <g transform="translate(24 23) scale(1.6)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        CnIconKind.Checkmark => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M3 24 10 17 20 27 38 6 45 13 21 42Z" fill="url(#__CN_ICON__g)" />
            <path d="M3 24 21 42 45 13 45 17 21 46 3 28Z" fill="url(#__CN_ICON__d)" class="cn-icon-bevel"/>
            </g>
            """,
        CnIconKind.Clock => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <circle cx="24" cy="25" r="18" fill="#2458a8" class="cn-icon-bevel"/>
            <circle cx="24" cy="24" r="18" fill="url(#__CN_ICON__g)" />
            <circle cx="24" cy="24" r="15" fill="url(#__CN_ICON__p)" />
            <path d="M24 13V24L30 28" fill="none" stroke="#2458a8" stroke-width="2.8" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="24" cy="24" r="1.8" fill="#388ce7" />
            </g>
            """,
        CnIconKind.Coins => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 23H28V37Q28 43 16.5 43T5 37Z" fill="url(#__CN_ICON__d)" />
            <ellipse cx="16.5" cy="23" rx="11.5" ry="5" fill="url(#__CN_ICON__g)"/>
            <path d="M6 30Q16 36 27 30 M6 36Q16 42 27 36" fill="none" stroke="#ffdb8e" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M20 10H43V28Q43 34 31.5 34T20 28Z" fill="url(#__CN_ICON__g)" />
            <ellipse cx="31.5" cy="10" rx="11.5" ry="5" fill="#ffdb8e"/>
            <path d="M21 18Q31 24 42 18 M21 24Q31 30 42 24" fill="none" stroke="#aa651e" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" />
            <path d="M29 8H34" fill="none" stroke="#aa651e" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Copy => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="5" y="5" width="28" height="33" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="16" y="14" width="27" height="31" rx="4" fill="url(#__CN_ICON__p)" stroke="#388ce7" stroke-width="1.5"/>
            <path d="M22 23H35 M22 29H35 M22 35H30" fill="none" stroke="#388ce7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Delete => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop class="cn-icon-tone-hi" stop-color="#ffb8b3"/>
            <stop offset=".55" stop-color="#ea6975"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#ea6975"/>
            <stop offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffb8b3"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa3c54" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M10 15H38L35 42H13Z" fill="url(#__CN_ICON__g)" />
            <rect x="7" y="9" width="34" height="6" rx="2" fill="url(#__CN_ICON__d)" />
            <path d="M18 10V6Q18 4 21 4H27Q30 4 30 6V10" fill="url(#__CN_ICON__g)" />
            <path d="M18 21 19 36 M25 21V36 M32 21 31 36" fill="none" stroke="#fff" stroke-width="2.2" stroke-linecap="round" stroke-linejoin="round" opacity=".85"/>
            </g>
            """,
        CnIconKind.Dismiss => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="rotate(45 24 24)">
            <path d="M19 5H29V19H43V29H29V43H19V29H5V19H19Z" fill="url(#__CN_ICON__g)" />
            </g>
            </g>
            """,
        CnIconKind.Document => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M13 5H30L38 13V39Q38 43 34 43H13Q9 43 9 39V9Q9 5 13 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M14 7H29L36 14V38Q36 41 33 41H14Q11 41 11 38V10Q11 7 14 7Z" fill="url(#__CN_ICON__p)" />
            <path d="M29 7V14H36" fill="#92cdff" />
            <path d="M16 19H28 M16 24H28 M16 29H23" fill="none" stroke="#388ce7" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.DocumentAdd => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M13 5H30L38 13V39Q38 43 34 43H13Q9 43 9 39V9Q9 5 13 5Z" fill="url(#__CN_ICON__g)" />
            <path d="M14 7H29L36 14V38Q36 41 33 41H14Q11 41 11 38V10Q11 7 14 7Z" fill="url(#__CN_ICON__p)" />
            <path d="M29 7V14H36" fill="#99e4b1" />
            <path d="M16 19H28 M16 24H28 M16 29H23" fill="none" stroke="#39ad64" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="35" cy="35" r="10" fill="#227847" class="cn-icon-bevel"/>
            <circle cx="35" cy="34" r="10" fill="url(#__CN_ICON__g)" />
            <path d="M35 29V39 M30 34H40" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.DocumentStack => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop class="cn-icon-tone-hi" stop-color="#c6acff"/>
            <stop offset=".55" stop-color="#9162dc"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#9162dc"/>
            <stop offset="1" stop-color="#5b389e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#9162dc">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#c6acff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#5b389e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="15" y="4" width="26" height="32" rx="3" fill="url(#__CN_ICON__d)" />
            <rect x="10" y="9" width="26" height="32" rx="3" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="14" width="26" height="29" rx="3" fill="url(#__CN_ICON__p)" />
            <path d="M11 22H25 M11 28H23 M11 34H19" fill="none" stroke="#9162dc" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Edit => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <g transform="rotate(39 24 24)">
            <rect x="18" y="3" width="13" height="33" rx="2" fill="url(#__CN_ICON__g)" />
            <rect x="18" y="3" width="13" height="7" rx="2" fill="url(#__CN_ICON__d)" />
            <rect x="18" y="10" width="13" height="3" rx="0" fill="url(#__CN_ICON__p)" />
            <path d="M18 36 24.5 46 31 36Z" fill="#ecd4ac" />
            <path d="M21.5 41.5 24.5 46 27.5 41.5Z" fill="#3c485c" />
            <rect x="20" y="14" width="3" height="22" rx="1" fill="#ffdb8e" />
            </g>
            </g>
            """,
        CnIconKind.ErrorCircle => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop class="cn-icon-tone-hi" stop-color="#ffb8b3"/>
            <stop offset=".55" stop-color="#ea6975"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#ea6975"/>
            <stop offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffb8b3"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa3c54" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <circle cx="24" cy="25" r="20" fill="#aa3c54" class="cn-icon-bevel"/>
            <circle cx="24" cy="23" r="20" fill="url(#__CN_ICON__g)" />
            <path d="M17 16 31 30 M31 16 17 30" fill="none" stroke="#fff" stroke-width="3.7" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Folder => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 13Q5 9 9 9H20L24 13H39Q43 13 43 17V38H5Z" fill="url(#__CN_ICON__d)" />
            <path d="M5 18H43L40 39Q40 42 36 42H10Q6 42 6 38Z" fill="url(#__CN_ICON__g)" />
            <path d="M9 19H39" fill="none" stroke="#fff" stroke-width="1" stroke-linecap="round" stroke-linejoin="round" opacity=".55"/>
            </g>
            """,
        CnIconKind.InfoCircle => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <circle cx="24" cy="25" r="20" fill="#2458a8" class="cn-icon-bevel"/>
            <circle cx="24" cy="23" r="20" fill="url(#__CN_ICON__g)" />
            <circle cx="24" cy="14" r="2.3" fill="url(#__CN_ICON__p)" />
            <path d="M24 22V33" fill="none" stroke="#fff" stroke-width="3.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Location => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop class="cn-icon-tone-hi" stop-color="#ffb8b3"/>
            <stop offset=".55" stop-color="#ea6975"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#ea6975"/>
            <stop offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffb8b3"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa3c54" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M24 45C20 38 8 27 8 19A16 16 0 0 1 40 19C40 27 28 39 24 45Z" fill="url(#__CN_ICON__g)" />
            <circle cx="24" cy="19" r="7" fill="url(#__CN_ICON__p)" />
            <path d="M13 17Q15 10 21 9" fill="none" stroke="#fff" stroke-width="1.5" stroke-linecap="round" stroke-linejoin="round" opacity=".5"/>
            </g>
            """,
        CnIconKind.Menu => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="5" y="10" width="38" height="5" rx="2.5" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="14" width="38" height="2" rx="1" fill="#4b6081" class="cn-icon-bevel"/>
            <rect x="5" y="22" width="38" height="5" rx="2.5" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="26" width="38" height="2" rx="1" fill="#4b6081" class="cn-icon-bevel"/>
            <rect x="5" y="34" width="38" height="5" rx="2.5" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="38" width="38" height="2" rx="1" fill="#4b6081" class="cn-icon-bevel"/>
            </g>
            """,
        CnIconKind.PanelLeft => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="4" y="7" width="40" height="34" rx="5" fill="url(#__CN_ICON__d)" />
            <rect x="6" y="8" width="36" height="30" rx="3" fill="url(#__CN_ICON__p)" />
            <rect x="6" y="8" width="12" height="30" rx="3" fill="url(#__CN_ICON__g)" />
            <path d="M22 14H36 M22 20H32" fill="none" stroke="#bbcada" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Play => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M10 7Q10 2 15 5L42 21Q47 24 42 27L15 44Q10 47 10 41Z" fill="url(#__CN_ICON__g)" />
            <path d="M11 40 42 22 42 27 15 44Q10 47 10 41Z" fill="url(#__CN_ICON__d)" class="cn-icon-bevel"/>
            </g>
            """,
        CnIconKind.Save => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M9 5H34L43 14V40Q43 44 39 44H9Q5 44 5 40V9Q5 5 9 5Z" fill="url(#__CN_ICON__g)" />
            <rect x="12" y="5" width="20" height="13" rx="1" fill="url(#__CN_ICON__p)" />
            <rect x="25" y="7" width="4" height="8" rx="0" fill="url(#__CN_ICON__d)" />
            <rect x="12" y="27" width="24" height="17" rx="2" fill="url(#__CN_ICON__p)" />
            <path d="M17 33H30 M17 38H27" fill="none" stroke="#92cdff" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Search => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M30 29 44 40Q47 43 43 46Q41 47 38 44L26 32Z" fill="url(#__CN_ICON__d)" />
            <circle cx="20" cy="20" r="16" fill="url(#__CN_ICON__g)" />
            <circle cx="20" cy="20" r="12" fill="url(#__CN_ICON__p)" />
            <circle cx="20" cy="20" r="9" fill="url(#__CN_ICON__glass)" />
            <path d="M14 18Q15 13 20 13" fill="none" stroke="#fff" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            """,
        CnIconKind.Send => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop class="cn-icon-tone-hi" stop-color="#92cdff"/>
            <stop offset=".55" stop-color="#388ce7"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#388ce7"/>
            <stop offset="1" stop-color="#2458a8"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#388ce7">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#92cdff"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#2458a8" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M4 19 45 3 33 45 23 29Z" fill="url(#__CN_ICON__g)" />
            <path d="M4 19 23 24 45 3Z" fill="#92cdff" />
            <path d="M23 24 23 36 29 31 45 3Z" fill="url(#__CN_ICON__d)" />
            <path d="M23 24 41 7" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".6"/>
            </g>
            """,
        CnIconKind.Stop => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop class="cn-icon-tone-hi" stop-color="#ffb8b3"/>
            <stop offset=".55" stop-color="#ea6975"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#ea6975"/>
            <stop offset="1" stop-color="#aa3c54"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#ea6975">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffb8b3"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa3c54" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="6" y="7" width="36" height="37" rx="7" fill="url(#__CN_ICON__d)" />
            <rect x="6" y="5" width="36" height="36" rx="7" fill="url(#__CN_ICON__g)" />
            <path d="M12 8H34" fill="none" stroke="#fff" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round" opacity=".5"/>
            </g>
            """,
        CnIconKind.Subtract => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop class="cn-icon-tone-hi" stop-color="#bbcada"/>
            <stop offset=".55" stop-color="#7c91ac"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#7c91ac"/>
            <stop offset="1" stop-color="#4b6081"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#7c91ac">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#bbcada"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#4b6081" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="5" y="18" width="38" height="12" rx="3" fill="url(#__CN_ICON__g)" />
            <rect x="5" y="28" width="38" height="3" rx="1" fill="url(#__CN_ICON__d)" class="cn-icon-bevel"/>
            </g>
            """,
        CnIconKind.SwapArrows => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop class="cn-icon-tone-hi" stop-color="#91e4dc"/>
            <stop offset=".55" stop-color="#2daf9f"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#2daf9f"/>
            <stop offset="1" stop-color="#17766e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#2daf9f">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#91e4dc"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#17766e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M5 10H30V4L44 17 30 29V22H5Z" fill="url(#__CN_ICON__g)" />
            <path d="M43 28H18V22L4 35 18 47V40H43Z" fill="url(#__CN_ICON__d)" />
            <path d="M8 12H29" fill="none" stroke="#fff" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round" opacity=".5"/>
            </g>
            """,
        CnIconKind.Warning => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop class="cn-icon-tone-hi" stop-color="#ffdb8e"/>
            <stop offset=".55" stop-color="#eba545"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#eba545"/>
            <stop offset="1" stop-color="#aa651e"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#eba545">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#ffdb8e"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#aa651e" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <path d="M20 5Q24-1 28 5L46 38Q48 43 42 43H6Q0 43 3 38Z" fill="url(#__CN_ICON__g)" />
            <path d="M24 14V28" fill="none" stroke="#aa651e" stroke-width="3.7" stroke-linecap="round" stroke-linejoin="round" />
            <circle cx="24" cy="35" r="2.2" fill="#aa651e" />
            </g>
            """,
        CnIconKind.TaskCheck => """
            <defs>
            <linearGradient id="__CN_ICON__g" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop class="cn-icon-tone-hi" stop-color="#99e4b1"/>
            <stop offset=".55" stop-color="#39ad64"/>
            <stop class="cn-icon-tone-lo" offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__d" x1="0" y1="0" x2=".8" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#39ad64"/>
            <stop offset="1" stop-color="#227847"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__p" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#e5edf3"/>
            </linearGradient>
            <linearGradient id="__CN_ICON__glass" x1="0" y1="0" x2="1" y2="1" style="--cn-icon-mid:#39ad64">
            <stop stop-color="#fff"/>
            <stop offset="1" stop-color="#99e4b1"/>
            </linearGradient>
            <filter id="__CN_ICON__s" x="-20%" y="-20%" width="140%" height="150%">
            <feDropShadow dx="0" dy=".8" stdDeviation=".65" flood-color="#227847" flood-opacity=".2"/>
            </filter>
            </defs>
            <g class="cn-icon-shadow" filter="url(#__CN_ICON__s)">
            <rect x="9" y="8" width="28" height="35" rx="4" fill="url(#__CN_ICON__g)" />
            <rect x="12" y="11" width="22" height="29" rx="2" fill="url(#__CN_ICON__p)" />
            <rect x="18" y="5" width="11" height="8" rx="3" fill="url(#__CN_ICON__d)" />
            <rect x="20" y="5" width="7" height="3" rx="1" fill="#99e4b1" />
            <g transform="translate(24 26) scale(1.25)">
            <path d="M-5 0 -1.5 3.5 5 -4" fill="none" stroke="#227847" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" />
            </g>
            </g>
            """,
        _ => string.Empty,
    };
}
