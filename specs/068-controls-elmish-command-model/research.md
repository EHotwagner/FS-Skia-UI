# Phase 0 Research — Controls.Elmish Command Model (Widget View + Cmd Alignment)

All Technical Context unknowns are resolved below. No `NEEDS CLARIFICATION` remains.

## R1 — How to expose a `Widget`-returning view without changing `AdapterProgram`

**Decision**: keep the `AdapterProgram<'model,'msg>` record exactly as-is (`View: 'model ->
Control<'msg>`) and add two **additive** entry points to `module ControlsElmish`:

- `widgetView : view:('model -> Widget<'msg>) -> ('model -> Control<'msg>)` — the pure
  adapter, defined as `view >> Widget.toControl`.
- `programOfWidget : init -> update -> view:('model -> Widget<'msg>) -> subscriptions ->
  AdapterProgram<'model,'msg>` — a constructor that internally calls
  `program init update (widgetView view) subscriptions`.

**Rationale**:
- F# records cannot overload a field, so a `Widget`-returning `View` cannot be added to the
  existing record without either a breaking change or a parallel record. A constructor
  function is the idiomatic additive choice and keeps the record (and every existing
  consumer) untouched (FR-002).
- The lowering stays in the **one** documented seam `Widget.toControl` (065 §3.2), so the
  IR the renderer/adapter consumes is byte-identical to the legacy boundary
  `view >> Widget.toControl` — this is exactly the parity FR-004/SC-002 require, and it is
  the shim that `tests/Elmish.Tests/TypedControlsAdapterTests.fs` (shipped in 065) currently
  writes by hand. 068 moves that shim out of product code and into the adapter.
- `widgetView` is also exported on its own so a product that wants to keep using the
  existing `program` can still drop the manual `Widget.toControl` (compose `widgetView` once).

**Alternatives considered**:
- *Second record type `WidgetProgram`* — duplicates the whole program shape and the
  `run`/host wiring; rejected as non-additive surface bloat.
- *Add a `WidgetView` field to `AdapterProgram`* — breaks the record's construction and
  every existing `program` caller; rejected (violates FR-002).

## R2 — The `AdapterCommand<'msg>` ↔ Elmish `Cmd<'msg>` total mapping

**Decision**: add `module AdapterCmd` with:

```
val none            : Cmd<'msg>                                             // = Cmd.none
val ofMessage       : msg:'msg -> AdapterCommand<'msg>                      // [ DispatchProductMessage msg ]
val productMessages : command:AdapterCommand<'msg> -> 'msg list            // ordered DispatchProductMessage payloads
val toCmd           : route:(AdapterEffect<'msg> -> 'msg)                   // total over EVERY effect case
                        -> command:AdapterCommand<'msg> -> Cmd<'msg>
```

`toCmd route command` builds one Elmish command per effect, in order, each dispatching
`route effect`; `route` is supplied by the product and is **total** over the
`AdapterEffect<'msg>` union, so no case is silently dropped (FR-003). The empty command maps
to `Cmd.none` (the empty edge). `productMessages` is the round-trip oracle: for a command of
only `DispatchProductMessage` payloads, dispatching `toCmd DispatchProductMessage-identity
command` delivers exactly `productMessages command`, same order, same multiset (FR-008,
SC-003).

**Rationale**:
- Elmish `Cmd<'msg>` is `Cmd.Effect<'msg> list` where each effect is `Dispatch<'msg> ->
  unit`; building a `Cmd` that dispatches a fixed list of messages in order is the standard
  `Cmd.ofMsg`-style construction and is **pure** to build (the dispatch happens later under
  Elmish's own loop), so the conversion respects the MVU boundary (Principle IV) — no I/O in
  `AdapterCmd`.
- The four non-product effect cases (`DispatchControlRuntimeMessage`,
  `DispatchKeyboardMessage`, `DispatchHostCommand`, `ReportAdapterDiagnostic`) cannot become
  a `'msg` on their own. Making `route` a **required total mapping** (rather than the library
  silently discarding them) is the single documented rule that keeps the alignment total and
  honest — the product decides how each non-product effect folds into its own message space.
  This satisfies the spec edge case "non-product effects MUST be carried, not discarded".
- `ofMessage`/`none` give the inverse direction needed to keep `init`/`update` returning the
  adapter's own `AdapterCommand` while still interoperating with `Cmd.none`/single-message
  ergonomics.

**Alternatives considered**:
- *`toCmd : AdapterCommand<'msg> -> Cmd<'msg>` (no `route`)* — would have to silently drop
  the four non-product cases to type-check; rejected (violates FR-003 totality / the
  no-silent-drop edge case).
- *Change `AdapterCommand` to be `Cmd<'msg>` directly* — a breaking change to a shipped type
  and would re-home effect interpretation; rejected (violates FR-002, FR-009).

## R3 — How the `FS.Skia.UI.Controls.Elmish` public surface is tracked (and what regenerates)

**Decision**: the adapter surface is governed by the **reflection-based** baseline
`readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` (fully-qualified type/member
names) plus the raw `.fsi` snapshot
`readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`. **Both** regenerate via
`./fake.sh build -t RefreshSurfaceBaselines`; the additive delta is reviewed in the diff.

**Rationale**:
- `FS.Skia.UI.Controls.Elmish` is **not** a `capabilities.yml` capability and is **not** in
  any capability's `contracts:` list (verified: the `Controls` capability lists only the
  `src/Controls/**` `.fsi` files). So — unlike the catalog/Controls path — its surface is
  **not** derived from `capabilities.yml`.
- It is tracked instead by: `build/Governance/Engine/Update.fs` (RequireFiles the stable
  baseline `FS.Skia.UI.Controls.Elmish.txt`, lines ~135/145), `build/Governance/PerPackageSurface.fs`
  (enumerates `FS.Skia.UI.Controls.Elmish`, lines ~41/60), and `build/Governance/Front/Helpers.fs:35`
  (maps `src/Controls.Elmish/Controls.Elmish.fsproj` → `FS.Skia.UI.Controls.Elmish`).
  `PackageSurfaceCheck` diffs the regenerated reflection surface against the committed
  baseline; `PerPackageSurfaceDiff` diffs the raw `.fsi` snapshot.
- Consequence for 068: the change **does** have a real, additive surface delta (the new
  `programOfWidget`/`widgetView`/`AdapterCmd.*` symbols appear in both baselines) — this is
  the key difference from `067`, which declared `module internal` and had **zero** delta.
  SC-006 is "additive-only delta confined to this one package", not "zero delta".

**Alternatives considered**:
- *Add `ControlsElmish.fsi` to a `capabilities.yml` `contracts:` list* — unnecessary
  (the reflection baseline already tracks it) and would be a template-governance change out
  of scope; rejected.

## R4 — No new dependency; base Controls stays Fable.Elmish-free

**Decision**: name `Cmd<'msg>` from the **already-referenced** `Fable.Elmish` (the
`Elmish` namespace) inside `ControlsElmish.fsi`/`.fs`; add nothing to
`Controls.Elmish.fsproj` or `Directory.Packages.props`.

**Rationale**: `src/Controls.Elmish/Controls.Elmish.fsproj` already has
`<PackageReference Include="Fable.Elmish" />` (it is the Elmish adapter package). The base
`FS.Skia.UI.Controls` references only `Scene`/`Layout`/`KeyboardInput` and must stay
`Fable.Elmish`-free (FR-006, SC-005) — the existing `ControlsElmishAdapterContractTests.fs`
dependency guard already asserts this and is retained.

## R5 — Test strategy (failing-first; FsCheck for the round-trip)

**Decision**: extend `tests/Elmish.Tests/` (Expecto). Add `FsCheck 3.3.3` (already pinned)
as a **test-only** `<PackageReference>` on `Elmish.Tests.fsproj` for the command round-trip
property (US2/FR-008, ≥1000 cases). US1 parity, US3 legacy-unchanged, and US4 mixed-migration
are deterministic Expecto unit tests. The US2 round-trip uses a **recording dispatcher**
(a `ResizeArray` capturing dispatched messages) to observe order/multiset without a live
Elmish loop.

**Rationale**: mirrors the `067` approach (Expecto + FsCheck, test-only FsCheck reference)
and reuses the existing `tests/Elmish.Tests/` harness and repo-root locator already present
in `TypedControlsAdapterTests.fs`. A recording dispatcher keeps the property pure and
deterministic (no timing, no real host).

**Alternatives considered**:
- *Drive a real `Program.run` loop* — introduces host/timing nondeterminism into a property
  test; rejected in favor of the recording dispatcher.
