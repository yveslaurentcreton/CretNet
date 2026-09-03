# [0.9.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.8.0...v0.9.0) (2026-09-03)


### Bug Fixes

* **blazor-ui:** a missing inbox transport must not take the render tree ([b9cd1da](https://github.com/yveslaurentcreton/CretNet/commit/b9cd1daec446a9f99ae9019f6d0b389922f00f97)), closes [#3159a7](https://github.com/yveslaurentcreton/CretNet/issues/3159a7)
* **blazor-ui:** CnButton busy-format falls back on foreign placeholders ([bd08730](https://github.com/yveslaurentcreton/CretNet/commit/bd087304c354d7e4c55ec600c45e82a13e836e56))
* **blazor-ui:** format CnButton Title through Smart.Format like Label ([ecd9f35](https://github.com/yveslaurentcreton/CretNet/commit/ecd9f354f2942c8e04498b563011fd560c91a5a4))
* **blazor:** CnpEntityPicker definition lookup is truly optional ([9c534ac](https://github.com/yveslaurentcreton/CretNet/commit/9c534acd96bd2bc1d87f2fed5a3bded9915f1e85))
* **CnNumberField:** a text field, not a number input ([88ec656](https://github.com/yveslaurentcreton/CretNet/commit/88ec656f39a3f152ffc75765a2f2f6adb562816d))
* **ui:** a conditional grid column never left the header ([a8b2ad7](https://github.com/yveslaurentcreton/CretNet/commit/a8b2ad76d6f7bdce5d88564a6b0f72eec65d498c))
* **ui:** a grid's own columns no longer jump in front of a child's ([69a53e4](https://github.com/yveslaurentcreton/CretNet/commit/69a53e4f483ead93200b0f0df0d0077cb9ce6114))
* **ui:** close the date popover on leaving, lift it out of dialogs, clear both halves ([70639ad](https://github.com/yveslaurentcreton/CretNet/commit/70639adb612dad0fffe26bc679548c1256cb0f28))
* **ui:** cn-theme.js decided the accent before the host could ([f33195c](https://github.com/yveslaurentcreton/CretNet/commit/f33195c8f793f0d304cb57eae40b650083e7d5a6)), closes [#17af3](https://github.com/yveslaurentcreton/CretNet/issues/17af3)
* **ui:** CnTimeline styles use a .cn-gantt prefix ([2a38447](https://github.com/yveslaurentcreton/CretNet/commit/2a38447370a625f415abb762ff07758e14d4eb17))
* **ui:** country flags render on Windows ([c1076f0](https://github.com/yveslaurentcreton/CretNet/commit/c1076f0130da1eb2266ace27f43659b112176ecf))
* **ui:** keep the date popover from stealing the field's focus ([ec2d0b0](https://github.com/yveslaurentcreton/CretNet/commit/ec2d0b04bacf772cbf9b3a206847f0547a8eecb1))
* **ui:** put the caret back in the field after clearing a date ([1cb8ccf](https://github.com/yveslaurentcreton/CretNet/commit/1cb8ccf9cbe30e7d268d80b751f5d796699b4ab3))
* **ui:** the company chooser was clipped, and the sidebar now reads like the mock ([b816887](https://github.com/yveslaurentcreton/CretNet/commit/b81688749d047e5ee1e3a654a69eb1f7b28f5a8a))


### Features

* **blazor-ui:** Cn toasts, notification inbox and a side panel ([682c315](https://github.com/yveslaurentcreton/CretNet/commit/682c31507219c71e0ce791760b4658fde7278985))
* **blazor-ui:** CnPickerGridDialog — the advanced-search half of CnPicker ([751bd18](https://github.com/yveslaurentcreton/CretNet/commit/751bd182cf27f540ceb0d7b5eb74151df022cbed))
* **blazor-ui:** lift CnBadge; add CnDataGrid.ResetAsync for scope changes ([a74e0de](https://github.com/yveslaurentcreton/CretNet/commit/a74e0de0c25e12d4c620ef8ee3e54a420ef52adb))
* **blazor-ui:** lift CnThemeService, cn-theme.js, cn-shell.css and CnAvatar ([7dfdb91](https://github.com/yveslaurentcreton/CretNet/commit/7dfdb912aac6fffd958a2c79396e400a85b0bf87))
* **blazor-ui:** lift the data grid family from HCMT ([a9e4c2f](https://github.com/yveslaurentcreton/CretNet/commit/a9e4c2fdb280ccf680dd21c317955a657e5e614f))
* **blazor-ui:** lift the dialog family, CnSelect, CnTabs and CnBreadcrumb ([a74b3a2](https://github.com/yveslaurentcreton/CretNet/commit/a74b3a2c8cc3c2e6130b15091228407d5feaedde))
* **blazor-ui:** scaffold CretNet.Platform.Blazor.Ui with the cn-ui stylesheet ([2f5ef64](https://github.com/yveslaurentcreton/CretNet/commit/2f5ef643acd6e630a2162288e96ccb1c6d7b9702))
* **blazor-ui:** timer pill grows a detail popover ([b200482](https://github.com/yveslaurentcreton/CretNet/commit/b200482b0b593a0b278d6cfa4579c6d027a343f6))
* **CnDateRangeField:** Subtle, the in-grid variant ([eed3775](https://github.com/yveslaurentcreton/CretNet/commit/eed3775479e4b9386a27c6916f5a4323280b7fc0))
* **CnSortable:** a board mode, and a placeholder a table row can draw ([f7974b5](https://github.com/yveslaurentcreton/CretNet/commit/f7974b5afea67b60622a25fb0fa131c14ab2b292))
* **CnSortable:** drag-to-reorder that feels like holding the row ([183f0ab](https://github.com/yveslaurentcreton/CretNet/commit/183f0ab416f8b6d4bf473e734721262667da313a))
* **CnSortable:** the lifted item leaves the flow; a dashed placeholder keeps its place ([0403b19](https://github.com/yveslaurentcreton/CretNet/commit/0403b193073dae33f6d32417ef6379fdada3bcc5))
* **CnTabs:** ActivateAsync, a tab activated from code ([2e9946d](https://github.com/yveslaurentcreton/CretNet/commit/2e9946dadb52574cf29469f5d9dee279d4210383))
* **inputs:** CnCurrencyField and CnNumberField, lifted from HCMT ([a8f79b7](https://github.com/yveslaurentcreton/CretNet/commit/a8f79b75013b1747f4c7d144ce3b15f1c5b43fa4))
* **ui:** a host can name its own default accent ([ef99941](https://github.com/yveslaurentcreton/CretNet/commit/ef999412f7915e71230f54afe3e687a1374c52e4))
* **ui:** CnDateField and CnDateRangeField — typing-first date controls ([f46826a](https://github.com/yveslaurentcreton/CretNet/commit/f46826acc6a61dfd1fe8723175c569e62a318223))
* **ui:** CnDateField and CnTextArea inputs ([4a21397](https://github.com/yveslaurentcreton/CretNet/commit/4a213977827e29357577166501f5bb5a9d0b3960))
* **ui:** CnDialogFooter can hide Save & open while keeping it wired ([2250907](https://github.com/yveslaurentcreton/CretNet/commit/2250907ed00fe2d3f7a42dd4a52fb4870f1c6702))
* **ui:** CnDialogFooter, CnDialogResult and CnReorder ([86a78d7](https://github.com/yveslaurentcreton/CretNet/commit/86a78d7607de1259d953a3e6e12cec8e03ad6d81))
* **ui:** CnIconKind.Clock and CnIconKind.Location ([338060d](https://github.com/yveslaurentcreton/CretNet/commit/338060db9b5081378bc63763e83c663662da8ec2))
* **Ui:** CnLoading, CnPageTitle and CnBreadcrumbService ([1f1684a](https://github.com/yveslaurentcreton/CretNet/commit/1f1684ad8b6ac7afa1b5b80221a600cdfb8d5455))
* **ui:** CnPicker — generic typeahead entity combobox ([15fb4fc](https://github.com/yveslaurentcreton/CretNet/commit/15fb4fc8d99fbf804c16670dcbd63d4a7e0d4423))
* **ui:** CnProgressBar and CnTimeline ([da713e2](https://github.com/yveslaurentcreton/CretNet/commit/da713e21a777925e00ed4b31beca46fe67afea6d))
* **ui:** CnStatusPicker — read and change a status in place ([de83b5a](https://github.com/yveslaurentcreton/CretNet/commit/de83b5a2f9fe217ba93a05113549877dc4ce9bbf))
* **ui:** CnTimeField and CnDateTimeField — a time of day, and a moment ([68d542a](https://github.com/yveslaurentcreton/CretNet/commit/68d542a39b8f798ef49c4d7c6c74dfa3cb1fddfc))
* **ui:** company badge and switcher styles ([a932719](https://github.com/yveslaurentcreton/CretNet/commit/a93271977348c9a47ffde0817c7ea81160a0dd9d))
* **ui:** company swatch and accent palette styles ([b1af1da](https://github.com/yveslaurentcreton/CretNet/commit/b1af1dabd3b7ea34cae0a38202b72b928842d656))


### Performance Improvements

* **CnSortable:** a board measures a column once, not on every pointer event ([d81888c](https://github.com/yveslaurentcreton/CretNet/commit/d81888ca7663fbecfb00bdc71b463b087e4cad2b))
* **CnSortable:** only the items in view glide ([c2d1a5e](https://github.com/yveslaurentcreton/CretNet/commit/c2d1a5e0f079d2c80dc78684df2dfa841e56127e))

# [0.8.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.7.0...v0.8.0) (2026-07-23)


### Features

* **blazor:** add durable notification inbox ([998ebb1](https://github.com/yveslaurentcreton/CretNet/commit/998ebb1f34853ca9585139521a99834e6c57cdb3))

# [0.7.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.6.1...v0.7.0) (2026-07-13)


### Bug Fixes

* **blazor:** entity picker validation message no longer drops the add button ([c4a4591](https://github.com/yveslaurentcreton/CretNet/commit/c4a459191586c7247586fd8b523d671422af4e66))
* **blazor:** unique filter-popover anchor ids per component instance ([22d2ce6](https://github.com/yveslaurentcreton/CretNet/commit/22d2ce6fb0f51b7f947577bdd4a1eee59210d9b2))


### Features

* **picker+datasource:** ShowAdd, FluentCombobox sizing, BackedBy lifecycle ([14495e1](https://github.com/yveslaurentcreton/CretNet/commit/14495e15e1471238d5b7113a15c0e6da9aaf54bc))

## [0.6.1](https://github.com/yveslaurentcreton/CretNet/compare/v0.6.0...v0.6.1) (2026-02-26)


### Bug Fixes

* make CnpSiteState.CurrentCulture non-nullable ([a04c319](https://github.com/yveslaurentcreton/CretNet/commit/a04c319c9879b7adb0a1a07ebdd5194abb777e1b))

# [0.6.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.5.0...v0.6.0) (2026-02-10)


### Features

* upgrade to .NET 10 ([5026963](https://github.com/yveslaurentcreton/CretNet/commit/5026963a9ddc96e421e734236208c9832c4630d4))

# [0.5.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.4.3...v0.5.0) (2025-09-15)


### Features

* Add option to define the items per page on a CnpEntityDataGrid ([93aa694](https://github.com/yveslaurentcreton/CretNet/commit/93aa69406eda048f972f79971df395455fef182e))
* Introduced EntityFilterType to make it possible which filters are used on an CnpDataSource component. The default entity filters or none of the filters. ([18530ab](https://github.com/yveslaurentcreton/CretNet/commit/18530abe41dbf180b8a232c2ff319fd4fcd4e664))
* Promote the CnpEntityView as the only way to display the entity information. ([3f45617](https://github.com/yveslaurentcreton/CretNet/commit/3f4561701f3cb6a023393f12c4180d25fe740fa7))

## [0.4.3](https://github.com/yveslaurentcreton/CretNet/compare/v0.4.2...v0.4.3) (2025-09-01)


### Bug Fixes

* Fix currency field value rendering issue. ([e3dc97e](https://github.com/yveslaurentcreton/CretNet/commit/e3dc97e22ca30ffb719ede0e58b3a23cedbea60b))

## [0.4.2](https://github.com/yveslaurentcreton/CretNet/compare/v0.4.1...v0.4.2) (2025-09-01)


### Bug Fixes

* Fixed label issue for currency field ([45a0a87](https://github.com/yveslaurentcreton/CretNet/commit/45a0a877d6abf0d2db3c9b2c7f257719174dfaa9))

## [0.4.1](https://github.com/yveslaurentcreton/CretNet/compare/v0.4.0...v0.4.1) (2025-09-01)


### Bug Fixes

* Improve parsing for currency and percentage fields ([f7dae28](https://github.com/yveslaurentcreton/CretNet/commit/f7dae28f5da33e46b43dbb496fb3748e9c691c9b))

# [0.4.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.3.1...v0.4.0) (2025-08-25)


### Features

* EntityDataGrid: Add property so communicate when openen from EntitySelect ([4b14322](https://github.com/yveslaurentcreton/CretNet/commit/4b143225fc012c36a36fb68e07a8ae3d9434d88b))

## [0.3.1](https://github.com/yveslaurentcreton/CretNet/compare/v0.3.0...v0.3.1) (2025-08-19)


### Bug Fixes

* Remove empty readme.md to fix NuGet package generation ([a2aa565](https://github.com/yveslaurentcreton/CretNet/commit/a2aa5650d02f4454c203de273191d488687ca8d4))

## [0.3.1](https://github.com/yveslaurentcreton/CretNet/compare/v0.3.0...v0.3.1) (2025-08-19)


### Bug Fixes

* Remove empty readme.md to fix NuGet package generation ([a2aa565](https://github.com/yveslaurentcreton/CretNet/commit/a2aa5650d02f4454c203de273191d488687ca8d4))

# [0.3.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.2.1...v0.3.0) (2025-08-19)


### Features

* Add platform ([6c97e3a](https://github.com/yveslaurentcreton/CretNet/commit/6c97e3afad759b4c81e26274e93e443244af1b46))
* Introduce a dedicated documentation site ([acf4fec](https://github.com/yveslaurentcreton/CretNet/commit/acf4fecfbfada6f79a10521570126bf94315201f))

# [0.3.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.2.1...v0.3.0) (2025-08-19)


### Features

* Add platform ([6c97e3a](https://github.com/yveslaurentcreton/CretNet/commit/6c97e3afad759b4c81e26274e93e443244af1b46))
* Introduce a dedicated documentation site ([acf4fec](https://github.com/yveslaurentcreton/CretNet/commit/acf4fecfbfada6f79a10521570126bf94315201f))

# [0.3.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.2.1...v0.3.0) (2025-08-19)


### Features

* Add platform ([6c97e3a](https://github.com/yveslaurentcreton/CretNet/commit/6c97e3afad759b4c81e26274e93e443244af1b46))
* Introduce a dedicated documentation site ([acf4fec](https://github.com/yveslaurentcreton/CretNet/commit/acf4fecfbfada6f79a10521570126bf94315201f))

# [0.3.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.2.1...v0.3.0) (2025-08-19)


### Features

* Add platform ([6c97e3a](https://github.com/yveslaurentcreton/CretNet/commit/6c97e3afad759b4c81e26274e93e443244af1b46))
* Introduce a dedicated documentation site ([acf4fec](https://github.com/yveslaurentcreton/CretNet/commit/acf4fecfbfada6f79a10521570126bf94315201f))

## [0.2.1](https://github.com/yveslaurentcreton/CretNet/compare/v0.2.0...v0.2.1) (2025-07-11)


### Bug Fixes

* Correct capitalization and links in documentation and metadata ([40c4c8d](https://github.com/yveslaurentcreton/CretNet/commit/40c4c8de0e4272293fe2704328bf1ec591cb6929))

# [0.2.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.1.0...v0.2.0) (2025-07-04)


### Features

* Rebranded to CretNet ([2a94ecd](https://github.com/yveslaurentcreton/CretNet/commit/2a94ecd82c038837ff43eeeb58cb579fe4acf40d))

# [0.1.0](https://github.com/yveslaurentcreton/CretNet/compare/v0.0.0...v0.1.0) (2025-06-03)


### Features

* Created initial version ([da09faf](https://github.com/yveslaurentcreton/CretNet/commit/da09faf0f0eca84a89d62d16362be9fa921ee192))
