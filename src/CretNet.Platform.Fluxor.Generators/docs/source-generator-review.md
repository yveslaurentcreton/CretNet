# Source Generators Review (CretNet.Platform.Fluxor.Generators)

Date: 2025-08-14

## scope and goals

- Review current source generators for correctness, incremental design, performance, and packaging.
- Identify bugs, risks, and design gaps.
- Propose actionable user stories with priorities.

## high-level summary

- Overall structure and use of Incremental Generators are solid. Templates and models are clean.
- A few correctness issues need attention (duplicate/misplaced attribute emission, missing resource, static mutable state in generators).
- Some implementation details can be hardened for determinism and performance (attribute discovery, resource caching, robust type handling, generated naming).

---

## findings

### generator setup

- Uses `IIncrementalGenerator` with `SyntaxProvider` and post-initialization attribute injection.
- Two action generators: `CnpActionGenerator`, `CnpEntityActionGenerator`, plus `GeneralGenerator`.
- Embedded resource templates for generated files are a good choice.

### correctness and determinism

- Duplicate/misplaced attribute emission: `CnpActionGenerator` registers a file named `CnpEntityStateAttribute.g.cs` but loads `CnpStateAttribute.cs`. Also, `CnpEntityActionGenerator` adds `CnpEntityStateAttribute` again. Attribute emission should be centralized to avoid duplication and confusion.
- Missing resource: `CnpActionGenerator` registers `CnpActionDispatcherExtensions` but `Resources/CnpActionDispatcherExtensions.cs` isn’t present; there is a runtime `DispatcherExtensions` in `CretNet.Platform.Fluxor` already.
- Static mutable state: both generators keep `_countPerFileName` to alter emitted content; this is not thread-safe and breaks determinism across incremental builds.
- Potential `AddSource` collisions: partials or multi-partial classes with attributes can cause duplicates; the counter currently masks that instead of preventing it.

### pipeline and performance

- Current discovery scans “classes with any attributes” and filters semantically. It works but can be more efficient and stable using `ForAttributeWithMetadataName`.
- [x] Embedded resource files are reloaded on each generation; caching improves performance. (done)

### codegen templates

- `EntityState.GetReducerSource` emits `Enumerable.Empty<T>()` but generated usings lack `using System.Linq;` (or fully-qualify).
- Templates add `using CretNet.Platform.Fluxor.Generators;` to consumer code—usually unnecessary; minimize generated usings to required runtime APIs.
- Reducers: `CnpAction_Reducers.cs` returns unchanged state when no custom reducer is present; consider omitting that reducer entirely unless `customReducer`.
- Naming collisions: using `ShortClassName` (removing `Action`) for emitted types like `[[ShortClassName]]Effects/Reducers/Events` can collide when you have both `Foo` and `FooAction` in the same namespace.
- State ‘with’ usage: `CnpEntityAction_Reducers.cs` assumes state is a record (or supports `with`). If that’s a contract, document or emit a diagnostic when it isn’t met.

### semantic handling

- Attribute parameter parsing relies on strings (e.g., generic type split via `SplitGenericType`). Prefer using `ITypeSymbol`/`INamedTypeSymbol` for robustness (handles nested generics, whitespace, arity, nullability).
- Nullability: `CnpEntityStateAttribute` has nullable constructor args but exposes non-nullable `string` properties; this will warn in `<Nullable>enable` consumer projects. Prefer `string?`.

### packaging and dependencies

- Generator packaging under `analyzers/dotnet/cs` is correct.
- Generated code references runtime types in `CretNet.Platform.Fluxor`; ensure the generator package depends on the runtime package so consumers don’t need to manually add it. Avoid `ProjectReference` in the generator; use a `PackageReference` on the published runtime package instead.

### API ergonomics

- DI via `[CnpInject]` on action public properties works but is unconventional; typical Fluxor effects inject services and act on action data. This is acceptable if intended; document clearly.
- Success/failure toasts: Nice opt-in via label attributes; ensure nullability is correct and messages degrade gracefully when labels or entity definitions are missing.

---

## bugs and risks (prioritized)

- [x] P0: Incorrect/misleading attribute emission (fixed)
  - `CnpActionGenerator.Initialize` adds `CnpEntityStateAttribute.g.cs` but loads `CnpStateAttribute.cs`.
  - `CnpEntityActionGenerator.Initialize` also emits `CnpEntityStateAttribute`; centralize to avoid duplicates.

- [x] P0: Missing resource referenced (fixed)
  - `CnpActionGenerator` attempts to embed `CnpActionDispatcherExtensions` that doesn’t exist. Remove or provide the resource (prefer remove; runtime has `DispatcherExtensions`).

- [x] P0: Static mutable `_countPerFileName` (fixed)
  - Not thread-safe; introduces non-deterministic outputs. Remove the counter, use deterministic hint names, and dedupe via the incremental pipeline.

- [x] P0: Missing using for `Enumerable.Empty<T>()` (fixed)
  - Generated code requires `using System.Linq;` or `System.Linq.Enumerable.Empty<T>()`.

- [x] P1: Potential `AddSource` collisions
  - Duplicated partials/attributes can emit the same hint name; use `ForAttributeWithMetadataName` or `.Distinct()`/comparers keyed by symbol to guarantee uniqueness. (done)

- P1: Nullability mismatch in attributes
  - `CnpEntityStateAttribute` properties should be `string?` to match constructor defaults and avoid consumer warnings.

- P1: Type parsing robustness
  - Replace string-based generic splitting with symbol-based analysis (`INamedTypeSymbol` and `TypeArguments`).

- P1: Naming collisions in generated types
  - `[[ShortClassName]]`-based names can collide; switch to `[[ClassName]]...` or nest generated types inside the action class.

- [x] P2: Unnecessary `using CretNet.Platform.Fluxor.Generators;` in generated files
  - Remove unless absolutely needed by the generated code. (done)

- P2: Redundant reducer generation when no state/custom reducer
  - Skip generating action reducer if it does nothing.

- P2: Cancellation tokens ignored in effect methods
  - Consider supporting a token via dispatcher or context if needed.

---

## user stories (with acceptance criteria)

- [x] P0: Centralized attribute emission
  - As a maintainer, I want all generator helper attributes emitted by `GeneralGenerator` only, so we avoid duplicate files and confusion.
  - Acceptance:
    - `CnpActionGenerator` and `CnpEntityActionGenerator` do not call `RegisterPostInitializationOutput` for attributes.
    - `GeneralGenerator` emits: `CnpActionAttribute`, `CnpEntityActionAttribute`, `CnpStateAttribute`, `CnpEntityStateAttribute`, `CnpInjectAttribute`, `CnpActionLabelAttribute`, `CnpSuccessLabelAttribute`, `CnpFailureLabelAttribute`.

- [x] P0: Deterministic generation without static counters
  - As a user, I want consistent codegen across builds, without hidden counters.
  - Acceptance:
    - `_countPerFileName` removed.
    - Hint names are stable and based on fully-qualified names.
    - No duplicate `AddSource` exceptions under normal builds.

- [x] P0: Remove invalid resource emission
  - As a developer, I want the generator not to reference non-existent resources.
  - Acceptance:
    - No `CnpActionDispatcherExtensions` emission.

- [x] P0: Generated code compiles without extra usings
  - As a consumer, I want out-of-the-box compile success.
  - Acceptance:
    - Generated usings include `System.Linq` when using `Enumerable.Empty<T>()` (or fully-qualify).

- [x] P1: Robust attribute/type handling
  - As a maintainer, I want symbol-based parsing of attribute types and generics for correctness.
  - Acceptance:
    - Use `INamedTypeSymbol` and `TypeArguments` to compute `ResultExpression`, namespaces, nullability.

- P1: Safe naming to avoid collisions
  - As a consumer, I want generated type names to be unique.
  - Acceptance:
    - Effects/Reducers/Events names either use full `[[ClassName]]` or are nested under the action.

- [x] P1: Attribute nullability correctness
  - As a consumer with `<Nullable>enable`, I want no nullability warnings from helper attributes.
  - Acceptance:
    - `CnpEntityStateAttribute` properties `EntityProperty`/`LoadingProperty` are `string?` and files have `#nullable enable` as needed.

- P1: Packaging includes runtime dependency
  - As a consumer, installing the generator should also add the runtime package automatically.
  - Acceptance:
    - Generator NuGet has a `PackageReference` dependency on the published `CretNet.Platform.Fluxor` runtime package (not `ProjectReference`).

- P2: Optimize template loading
  - As a maintainer, I want fewer allocations and faster builds.
  - Acceptance:
    - Resource content cached via static `Lazy<string>` or similar helper.

- P2: Conditional reducer emission
  - As a consumer, I want less noise in generated code.
  - Acceptance:
    - Skip `Action` reducer when no state or custom reducer.

---

## recommendations (how to implement)

1. Centralize attribute emission

- Move all `RegisterPostInitializationOutput` calls into `GeneralGenerator`. Remove them from `ActionGenerator` and `EntityActionGenerator`.
- Fix the mismatched file name for `CnpStateAttribute` (use a hint name that matches the type for clarity).

1. Remove invalid resource emission

- Delete the registration for `CnpActionDispatcherExtensions` in `ActionGenerator`. Keep `CretNet.Platform.Fluxor/DispatcherExtensions.cs` as the runtime extension.

1. Make generation deterministic

- Remove `_countPerFileName` and the `[[Counter]]` placeholder from templates (`Resources/CnpAction.cs` and `Resources/CnpEntityAction.cs`).
- Ensure uniqueness with either:
  - `context.SyntaxProvider.ForAttributeWithMetadataName("CretNet.Platform.Fluxor.Generators.CnpActionAttribute", ...)`, or
  - `.Distinct()` on your model with a comparer keyed to the `INamedTypeSymbol` (SymbolEqualityComparer.Default).

1. Fix missing using

- Add `using System.Linq;` in `GetUsings` for entity actions, or emit `System.Linq.Enumerable.Empty<...>()`.

1. Harden type handling

- In `ArgumentParameter.DetermineParameters`, capture `ITypeSymbol` for constructor args (when type parameters are used) and derive `namespace`/`name` from symbols.
- Replace `SplitGenericType` with symbol-based construction of `ResultExpression` (handles nested generics and nullability).

1. Safer names for generated types

- Prefer `[[ClassName]]Effects/Reducers/PostEvent` or nest `Effects/Reducers` classes inside the partial action class to avoid namespace collisions.

1. Nullability

- Update `CnpEntityStateAttribute` properties to `string?`, and enable nullable in attribute resources where needed.

1. Packaging

- Replace the generator’s `ProjectReference` to `CretNet.Platform.Fluxor` with a `PackageReference` to the published runtime package in the generator `.csproj`. This ensures consumers get the runtime API automatically.

1. Template caching

- Cache embedded resources in `Helpers` via `ConcurrentDictionary<string, string>` or `Lazy<string>` to avoid repeated stream reads.

---

## suggested backlog (prioritized)

- P0
  - Centralize attribute emission; remove duplicates and mismatched hint name.
  - Remove `CnpActionDispatcherExtensions` emission.
  - Remove static counters and `[[Counter]]` placeholders; ensure unique hint names in pipeline.
  - Add missing `System.Linq` using (or fully-qualify `Enumerable.Empty`).

- P1
  - Switch discovery to `ForAttributeWithMetadataName` or add `.Distinct()` with symbol comparer.
  - Update `CnpEntityStateAttribute` nullability.
  - Symbol-based type parsing; remove string-based generic splitting.
  - Update generated names to avoid collisions (use full `ClassName` or nested types).
  - Change generator packaging to depend on runtime package instead of project reference.

- P2
  - Remove unnecessary `using CretNet.Platform.Fluxor.Generators;` from generated code.
  - Omit no-op reducers when `customReducer` is false or no state.
  - Cache embedded resources.
  - Consider cancellation token support in effects.

---

## appendix: file references
- Generators: `CretNet.Platform.Fluxor.Generators/ActionGenerator.cs`, `EntityActionGenerator.cs`, `GeneralGenerator.cs`
- Models: `Models/*`
- Templates: `Resources/CnpAction.cs`, `Resources/CnpEntityAction.cs`, `Resources/CnpAction_Reducers.cs`, `Resources/CnpEntityAction_Reducers.cs`
- Attributes: `Resources/Cnp*.cs`
- Runtime ext: `CretNet.Platform.Fluxor/DispatcherExtensions.cs`

---

## completion
This document records current issues and an ordered plan. Next steps: implement P0 items, verify build, then iterate through P1 hardening tasks.
