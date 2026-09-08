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
