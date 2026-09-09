# Shared Cn controls and HCMT adoption

## Ownership

CretNet.Platform.Blazor.Ui owns all reusable controls, their English/Dutch
resources and generic interaction. Hosts own business resources, routes,
API transports, profile persistence and domain-specific composition.
The controls do not reference HCMT, WAM or FluentUI. Resource-backed defaults
can be overridden by explicitly supplied host labels. New resource keys need
both translations and a strongly typed CnLabels accessor.

## Numeric input

Nullable values distinguish an empty field from zero. Parsing and precision are
separate per-control policies; hosts must choose explicitly for ambiguous
separators. Required model properties can use explicit nullable-to-required
binding conversions while optional properties bind directly. Do not change
business nullability merely to make a component compile.

`CnNumberField`, `CnCurrencyField` and `CnPercentField` all expose
`decimal? Value` and `EventCallback<decimal?> ValueChanged`:

| `ParsingMode` | Meaning of `1.234` | Use |
|---|---|---|
| `Flexible` | 1234 | Existing currency/grouping convention; default for number/currency |
| `Decimal` | 1.234 | Quantities and percentages; either single separator is decimal |
| `Culture` | CurrentCulture decides | Inputs with a strict locale convention |

With both separators, Flexible/Decimal use the rightmost as the decimal
separator. Decimal rejects repeated identical separators and displays without
grouping so editable text retains its meaning. Blank/unreadable input gives null.
Number `MaxDecimals` controls display precision independently of parsing; `Min`
and `Max` clamp committed values. Percent defaults to Decimal mode.

```razor
<CnNumberField @bind-Value="optionalQuantity"
               ParsingMode="CnNumberParsingMode.Decimal" MaxDecimals="4" />
<CnCurrencyField Value="requiredAmount"
                 ValueChanged="value => requiredAmount = value ?? 0m" />
```

Here `optionalQuantity` is `decimal?` and `requiredAmount` is `decimal`. Only
choose zero as the required-field fallback where the host's domain permits it;
use a nullable form draft with validation when a blank must remain visible.

## Lifted behavior

The remaining HCMT controls now live here: `CnCard`, `CnCheckbox`, `CnFlowRail`,
`CnMessageBar`, `CnPercentField`, `CnRichTextBox`, `CnStat` and `CnStatStrip`,
with their enums, models, drag ordering helper and `CnRichTextEditor` engine.
`CnDataGrid.RowDetail` and `RowExpanded` provide optional detail rows while
retaining ordered, removable columns. Hosts still own loading and expansion state.

`CnAvatarSource` implements shared photo caching, request coalescing, invalidation
and change notifications. Derive a scoped host provider and implement
`LoadPartyAvatarUrlAsync`; call `SetPartyAvatarUrl` after a successful upload.
Register that same instance as `ICnAvatarSource`. URLs/data URLs and missing photos
are supported; failed transports can retry. `CnAvatar` ignores stale lookups after
a photo update or party change. `CnAvatarImage.ReadPngAsync` also owns configurable browser-side PNG resizing
and bounded reading (defaults: 256px, 512 KiB). HCMT supplies its existing photo API; a WAM
provider can later resolve Microsoft 365 photos without changing the control.

Date/time mask, completion, keyboard navigation and popup placement continue to
come from the existing `CnDateField`/`CnTimeField` family. HCMT adapts `DateOnly?`
at its form boundary. Calendar chrome now uses the library resources.

## Browser sample

### Configurable icon appearance

`CnIcon` retains its original outline rendering when no appearance is supplied.
`CnIconScope Appearance="..."` opts a subtree into the dimensional Natural or
Category styles; an individual `CnIcon.Appearance` overrides that scope.
`CnIconAppearance` also carries Normal/Quiet intensity, subtle depth and sidebar
sizes 18/22/26 px. Immutable replacement updates existing descendants, including
buttons and hosted dialogs. Scope a dialog host together with the calling app.

The 66 shared kinds cover the accepted 71 navigation/action proposals; five
identical symbols are reused. Existing enum values remain stable; additional
object silhouettes are appended. SVG geometry and paint live in
`CnIconArtwork.Natural.cs` and `CnIconArtwork.Category.cs`. Each component uses
a stable, unique prefix for its gradient/filter IDs. No runtime JavaScript,
external images or network access is needed to render an icon.

`CnIconSettings` provides resource-backed colour, intensity, size and depth
controls. Hosts own persistence and identity, and use `ValueChanged` to replace
the scoped appearance. `--cn-nav-icon-size` only sizes navigation icons; other
contexts retain their allocated sizes. Scope wrappers use `display: contents`.

#### Entity and action roles

`CnIconRole.Entity` identifies objects, destinations, search results and timeline
records. `CnIconRole.Action` covers buttons and system controls, and is the default
role of a standalone icon. The role describes its placement, not its shape: the
same document symbol can identify an entity or appear on an action button.
Hosts retain their own entity-to-symbol registry. Shared `CnIconKind` names describe
generic objects and operations; their existing names and numeric values stay stable.

Supply `CnIconScope.ActionAppearance` to opt into independent action rendering:

```razor
<CnIconScope Appearance="entityIcons" ActionAppearance="actionIcons">
    <CnIcon Kind="CnIconKind.FolderPlan" Role="CnIconRole.Entity" />
    <CnButton Icon="CnIconKind.Edit" Label="Edit" />
</CnIconScope>
```

`CnActionIconAppearance` provides `Style`, `Depth` and `DestructiveColor`.
`CnActionIconStyle.Monochrome` uses foreground outlines; `Functional` adds semantic
colours; `Colored` uses coloured object surfaces; `Natural` uses material artwork.
Depth applies only to the last two styles. Entity intensity and navigation size
never alter action icons. Destructive colour changes the actual Delete artwork,
including red versus steel material in coloured styles. Status/error symbols retain
their semantic meaning; explicit button roles still control the button's own colour.

`CnIconSettings.ActionValue`/`ActionValueChanged` adds a second, translated group
with four style previews, depth and destructive colour. Leaving it null retains the
original settings layout. Hosts persist both values per their own account model;
the recommended new action default is monochrome, no depth and destructive red.

Leaving `ActionAppearance` null retains the original single-appearance contract,
including outline rendering without any scope. `CnPageTitle.Icon` uses Entity;
`CnButton` assigns Action to its leading, trailing and composed icons. Individual
`CnIcon.Appearance` overrides remain strongest, followed by the button override
(including Accent foreground contrast), role preferences and the legacy scope.
This remains one shared renderer; there are no per-page alignment or colour fixes.

`CnButton` chooses foreground outline icons for the Accent role so they inherit
the button text colour on a solid accent background. This applies to `Icon`,
`IconEnd` and icons composed through `ChildContent`; Quiet/depth preferences do
not weaken the foreground variant. Neutral, Subtle and Danger buttons follow
scoped action preferences when supplied, or the legacy appearance otherwise.
`IconAppearance` provides an explicit per-button
override, while an individual `CnIcon.Appearance` remains the most specific
override. Role and appearance changes update existing button icons immediately.
The Add symbol uses a slim, rounded cross in all styles; coloured variants keep
a restrained gradient and optional depth instead of a filled block.
Beside a button label, all icons receive the same 1px downward optical correction
to align with the shell font's visible lettering. This rule covers every button
role and icon appearance, including leading/trailing icons. Icon-only buttons
keep geometric centering. The correction belongs to shared button composition;
there are no symbol-specific offsets or application-level exceptions.

This extension is source-verified before publication. Consumers must adopt the
owner-approved published package and pass their NuGet gate before merge.

### Standalone page titles

`CnPageTitle` leaves 16 px below its visible heading so following grid search
toolbars or forms do not touch it. Hosts can override `--cn-page-title-spacing`
when their layout supplies spacing. `Hide` adds no heading or spacing; composed
headers using `.cn-page-title` keep their own layout.

The optional `Icon` parameter adds a decorative 28 px `CnIcon`, inheriting the
host's appearance scope. The same `.cn-page-title--with-icon` class can be used
on a composed heading containing a `CnIcon` and a text `span`. It retains a
10 px gap and allows long names to wrap. `Hide` still omits the entire heading;
icons never enter the document title or breadcrumb text.

### Field widths

Date, time, date-time and date-range borders fill the width allocated to their
outer field, like other Cn inputs. The masked text keeps its readable intrinsic
width. Hosts control layout through their grid/flex container or the field's
`Style`/`Class`; an explicit `Style="width:240px"` applies to the whole control.
Inline flex toolbars keep natural compact sizing. For a compact field in block
flow, use `Style="width:fit-content"`. Clearing a value must not shrink a field
whose host allocated a fixed/grid width.

### Running the sample

The sample includes all four date/time variants plus explicit 240px and
fit-content fields for checking allocated widths before/after typing and clearing.

Run from the repository root:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet run --project src/CretNet.Platform.Blazor.Ui.Sample -- --urls http://localhost:5218
```

The sample provides nullable numbers, dates/times, a picker, expandable grid,
rich-text editor, confirmation dialogs, toasts and a notification bell with
Dutch/English and theme switches. Notification actions use an in-memory client
scoped to the sample circuit. It supplements consumer component and browser tests.
Static styles use `MapStaticAssets` and fingerprinted `Assets` paths so a rebuild
and reload selects the updated CSS.

## Feature validation during adoption

Temporarily, automatic CI/release runs on main pushes only. Feature commits do
not consume Actions minutes or publish prereleases. Use workflow_dispatch on the
feature ref for build/test validation; it never invokes semantic-release or
publishes packages. The previous develop/*, feature/*, bugfix/* and hotfix/* push
filters can be restored when the owner ends this temporary period. Local builds
and tests remain required. Push filters take effect on the pushed branch. GitHub
requires the workflow_dispatch declaration on the default branch before offering
manual runs; until then run the same build/test commands locally.

## Local verification before publishing

The HCMT adoption branch uses the modified sources. Its 0.9.0 NuGet pins are the
last published baseline, not a claim that 0.9.0 includes this work. Source builds
must pass now; HCMT's final NuGet gate and pin update follow the owner-approved
semantic-release publication. Do not merge the consumer before that gate.

## Delivery checklist (HCMT S-076, 2026-09-05)

- [x] Remaining generic controls and rich-text editor live in the RCL.
- [x] Grid details preserve serialized stocktake interaction.
- [x] Numeric policies distinguish empty/zero and ambiguous separators.
- [x] All owned labels use resources, including calendar, picker and notifications.
- [x] Shared photo caching supports host transports and refresh after upload.
- [x] Automated tests pass; HCMT source adoption and browser interactions verified.
- [ ] Published package adoption verified before HCMT merge (owner release).

## Verification recorded 2026-09-05

Release solution build passed (existing legacy/Sharepoint/playground warnings;
no warnings in the changed Cn library). 68 existing tests, 53 component tests and
four browser-helper tests passed. Run the last group with
`node --test tests/ui/*.test.mjs`; both manual feature validation and main release
CI include it. Browser checks cover nullable values/precision, resources, editor,
dialogs, grid details and HCMT date/time integration. The consumer's S-076 records
the exact scope and the pending published-package gate.

## Verification recorded 2026-09-08

All 53 component tests passed. The running sample verified notification read and
archive actions, filtering, confirmation accept/cancel and toasts, enum selection,
Dutch/English resources and dark theme. Notification rows now explicitly use
border-box sizing and a shrinkable text column: in a host without a global sizing
reset, the panel's 420px client width previously had a 450px scroll width; both
measure 420px after the correction. This layout check requires a browser and is
not represented as a component-test CSS assertion.

HCMT's initial browser/integration attempt was blocked by Docker startup. After
the owner restarted Docker, all 184 integration tests passed without skips.
The secure Aspire app then verified enum persistence, confirmation cancellation,
notification API read/archive/navigation, responsive layout, breadcrumbs and
invoice/project picker interactions using disposable HCMT fixtures. The source
build passed without warnings; 1,961 unit and 273 component tests passed.

Two shared defects found during that browser run were corrected:

- CnPicker cancels consumed native key defaults synchronously at its input.
  Blazor still handles selection, but Enter in an open popup cannot implicitly
  submit the enclosing form. Ordinary typing, Tab and closed-popup Enter retain
  their defaults. Connect/disconnect owns the listener lifetime. A browser check
  proved an unrelated unsaved field remained unpersisted after keyboard selection.
- Resource-backed parameter defaults resolve from the current UI culture on
  access instead of at construction. Pure fallback getters preserve explicit
  host values, including empty strings, and the public string parameter API.
  Their narrow BL0007 suppressions document why the getters intentionally are
  not auto-properties. Hosts must still cause a render when culture changes;
  the library does not subscribe to application-specific language services.

Both regressions failed before their corrections. Final shared verification:
55 component tests and seven Node browser-helper tests passed. Reopening the
HCMT notification panel after English/Dutch switches refreshed all owned labels;
dark presentation was visually checked. HCMT S-076 records the precise browser
scope, remaining host-child label refresh issue and pending published-package gate.

## Standalone title verification (2026-09-09)

All 55 component tests and HCMT's 11 focused inbox/title tests passed. HCMT's
Debug and Release frontend source builds passed without warnings or errors.
The running inbox measured a 16 px gap from the standalone heading to its search
field. This is browser layout evidence; no component-test CSS string assertion
is used as a substitute. The change is local on `codex/page-title-spacing` and
has not been published as a package.
