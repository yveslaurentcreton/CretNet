# Rich text: Markdown and hardened HTML

Rich business text (a proposal introduction, a project brief, a comment) is
written with a small toolbar and must print exactly as it was seen. CretNet
offers two editors for it, and one rule behind both: only a fixed set of
formatting exists, so nothing unexpected reaches a document.

| | `CnMarkdownEditor` | `CnRichTextEditor` / `CnRichTextBox` |
|---|---|---|
| Stored value | Markdown | HTML from an allow-list |
| Editing surface | Plain textarea + live preview | contenteditable |
| Server side | `CnMarkdown` (render, parse, plain text) | `CnRichTextSanitizer` |
| Use for | New fields; text that is printed | Existing HTML consumers |

Both use the same four palette colours, in light and dark mode.

## The shared pipeline (`CretNet` package, namespace `CretNet.RichText`)

The core package has no UI dependency, so an API or PDF renderer interprets
Markdown exactly like the editor preview.

```csharp
MarkdownDocument doc = CnMarkdown.Parse(markdown);   // for renderers (PDF)
string html = CnMarkdown.ToSafeHtml(markdown);       // for previews
string text = CnMarkdown.ToPlainText(markdown);      // for search indexes
```

`CnMarkdown.Pipeline` is one Markdig pipeline: CommonMark (paragraphs, lists,
emphasis, links, quotes, breaks), `~~strikethrough~~` and pipe tables. Raw
HTML is disabled: `<b>`, `<script>` or `<img onerror>` stay literal text. Bare
URLs are not linked; `<https://…>` is.

Every parsed document is normalised before anyone sees it:

- `[text]{.accent}` becomes a `CnMarkdownColorInline` (HTML:
  `<span class="cn-md-c-accent">`) for a palette name. Any other name renders
  the plain text without brackets. Only this `{.name}` form after brackets is
  recognised: Markdig's full generic-attributes extension would pass any
  attribute (including `onclick`) and swallow ordinary text in braces.
- Headings have level 2 or 3: `#` becomes h2, `####`–`######` become h3.
- Only http, https and mailto links survive; other links and all images
  become their text. Links get `target="_blank" rel="noopener noreferrer"`.

### Palette

`CnMarkdownPalette.Colors` lists `accent`, `blauw`, `oranje` and `rood`. Each
`CnPaletteColor` has its stored `Name`, `CssClass` (`cn-md-c-*`), the Cn
token `CssVariable` (`--cn-md-*`), `LightHex`, `DarkHex` and the `PrintHex`
for PDFs (#128a30, #3159a7, #b35f00, #c42b2f). `CnMarkdownPalette.Nearest`
maps any CSS colour to the nearest palette colour; greys, black and white map
to none.

### Converting HTML to Markdown

```csharp
CnHtmlToMarkdownResult result = CnHtmlToMarkdown.Convert(html);
// result.Markdown, result.Dropped (Underline, FontFamilyOrSize, BackgroundColor, Image, UnsupportedLink)
```

Used for paste and for one-off migrations of stored HTML. Bold and italic come
from tags and inline styles (weight 600+); Google Docs' `<b style="font-weight:normal">`
wrapper is ignored. h1/h2 become `##`, h3–h6 `###`. Lists nest under their
parent's marker (two spaces under `- `, three under `1. `), including desktop
Word's `mso-list` paragraphs. Tables become pipe tables, colours the nearest
palette colour. Loose text between blocks becomes its own paragraph, and text
that merely looks like Markdown (`1. `, `*`, `#`) is escaped.

### Sanitising stored HTML

`CnRichTextSanitizer.Sanitize(html)` keeps `p`, `br`, `h2`, `h3`,
`strong`/`b`, `em`/`i`, `u`, nested `ul`/`ol`/`li`, `a[href]` (http, https,
mailto) and `span[data-color]` with a palette name. Before styles go, their
meaning is kept (bold, italic, underline, colour). h1 maps to h2, h4–h6 to h3,
divs and Word list paragraphs to paragraphs and lists; loose text is wrapped
in paragraphs. The result is stable: sanitising it again changes nothing.

## `CnMarkdownEditor`

```razor
<CnMarkdownEditor @bind-Value="proposal.Introduction" Label="Introduction"
                  For="() => proposal.Introduction" MaxLength="20000" Hint="preview = PDF" />

<CnMarkdownEditor @bind-Value="comment" Compact Placeholder="Add a comment" />
```

| Parameter | |
|---|---|
| `Value` / `ValueChanged` / `For` / `Label` | As `CnTextArea`, including EditForm validation chrome |
| `Placeholder`, `ReadOnly`, `Disabled`, `Class`, `Style` | As other Cn inputs; extra attributes go to the textarea |
| `MaxLength` | Shows a counter; over the limit it turns red. The text is never cut |
| `Compact` | Bold, italic, bullet list, link; write/preview only |
| `DefaultMode` | `Write`, `Preview` or `Split` (default; `Write` when compact) |
| `Paper` | Preview on a light sheet with document typography (default unless compact) |
| `Hint` | A note in the footer |
| `PasteConverted` | Raised with the `CnHtmlToMarkdownResult` after an HTML paste |

The toolbar has bold, italic, heading (H2), subtitle (H3), bullet and
numbered list, link, a palette colour menu and clear formatting; Ctrl+B,
Ctrl+I and Ctrl+K run bold, italic and link. Commands work on the textarea
selection through `CnMarkdownTextCommands` and keep the browser's undo
history. Side by side stacks below 760 px. The textarea grows with its text.

Pasting HTML (Word, Google Docs, a web page) inserts the converted Markdown at
the caret and shows a toast naming what was left out, when a
`CnToastService` is registered (`AddCretNetBlazorUi`). Plain text pastes as is.

`CnMarkdownView` renders read-only Markdown with the same pipeline
(`Value`, `Paper`, `Class`).

## Hardened `CnRichTextEditor`

The value is held to the sanitiser's allow-list when it is set, on every
input and on paste. Pasted HTML is sanitised by .NET before the browser
inserts it; the browser also cleans the DOM structurally after each input
(unknown elements unwrapped, attributes stripped, loose text wrapped). The
toolbar gains a palette colour menu with "no colour"; colours are stored as
`span[data-color]`. `CnRichTextBox` uses the same engine.

## Styling

`cn-ui.css` defines `--cn-md-accent`, `--cn-md-blauw`, `--cn-md-oranje` and
`--cn-md-rood` for both themes, the editor (`.cn-md*`), the palette menu
(`.cn-palette*`) and rendered Markdown (`.cn-md-doc`, `.cn-md-paper`). The
paper sheet stays light in dark mode and uses the print colours, like the
PDF.
