# Implementation Plan: Controls.Elmish Command Model (Widget View + Cmd Alignment)

**Branch**: `068-controls-elmish-command-model` | **Date**: 2026-06-06 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/068-controls-elmish-command-model/spec.md`

## Summary

Additively converge the Elmish adapter (`FS.Skia.UI.Controls.Elmish`) onto the typed
authoring surface shipped in `065`, on two axes:

1. **Widget-returning view path.** Add `ControlsElmish.programOfWidget` (a program
   constructor whose `view` is `'model -> Widget<'msg>`) and a `ControlsElmish.widgetView`
   adapter (`('model -> Widget<'msg>) -> ('model -> Control<'msg>)`, = `view >> Widget.toControl`).
   The adapter performs the lowering internally, so a product authored entirely with
   `FS.Skia.UI.Controls.Typed.*` needs **no** `Widget.toControl` shim in its own code
   (FR-001, FR-004). The `AdapterProgram` record is **unchanged** — `View` stays
   `'model -> Control<'msg>`; the shim simply moves out of product code and into the
   adapter.

2. **`Cmd<'msg>` alignment.** Add an `AdapterCmd` module that bridges the adapter's
   `AdapterCommand<'msg> = AdapterEffect<'msg> list` to Elmish `Cmd<'msg>` under a single
   documented, **total** rule: `toCmd (route: AdapterEffect<'msg> -> 'msg)` maps every
   effect case (product and non-product alike) to a `'msg` preserving order, with
   `[] -> Cmd.none`; `productMessages` projects the ordered `DispatchProductMessage`
   payloads (the round-trip oracle); `ofMessage`/`none` are the inverse helpers (FR-003,
   FR-008).

The change is **Tier 1 (contracted)** but **additive-only** and **confined to
`src/Controls.Elmish/`**: every existing signature (`AdapterProgram.View`,
`ControlsElmish.program`, `AdapterCommand`/`AdapterEffect`/`AdapterSubscription`, the
effect interpreters) is byte-for-byte unchanged (FR-002, FR-009). No new dependency:
`Cmd<'msg>` comes from `Fable.Elmish`, which this package already references (FR-006);
the base `FS.Skia.UI.Controls` package stays `Fable.Elmish`-free (FR-006, SC-005).

**Technical approach**: edit the single compile-unit pair
`src/Controls.Elmish/ControlsElmish.fsi` / `ControlsElmish.fs` (the package's only
source). Regenerate the package's reflection-based public-surface baseline
(`readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`) and raw per-package
snapshot (`readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`) via
`RefreshSurfaceBaselines` — the additive delta is reviewed in the diff. Extend the
existing `tests/Elmish.Tests/` suite (failing-first), adding an FsCheck reference for
the command round-trip property.

## Technical Context

**Language/Version**: F# / .NET `net10.0` (matches `Controls.Elmish.fsproj` / repo TFM).
**Primary Dependencies**: none new. `FS.Skia.UI.Controls.Elmish` already references
`FS.Skia.UI.Controls`, `FS.Skia.UI.KeyboardInput`, and **`Fable.Elmish`** (which supplies
`Cmd<'msg>`) — confirmed in `src/Controls.Elmish/Controls.Elmish.fsproj`. The
`Widget<'msg>` / `Widget.toControl` lowering seam comes from the already-referenced
`FS.Skia.UI.Controls` (`src/Controls/Widget.fsi`, shipped in `065`). Tests add an FsCheck
`<PackageReference>` to `tests/Elmish.Tests/Elmish.Tests.fsproj` (version already pinned
in `Directory.Packages.props`: `FsCheck 3.3.3`) — a test-only reference, not a product
dependency.
**Testing**: Expecto (existing `tests/Elmish.Tests/` harness) + FsCheck for the command
round-trip property. New tests extend `TypedControlsAdapterTests.fs` and
`ControlsElmishAdapterContractTests.fs`; a new `AdapterCmdTests.fs` carries the bridge
properties. Failing-first.
**Target Platform**: Windows and Linux (pure F# adapter; no platform/Skia/Vulkan surface).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification**: **Tier 1 (contracted change)** — adds public API surface to
`FS.Skia.UI.Controls.Elmish` (`ControlsElmish.programOfWidget`, `ControlsElmish.widgetView`,
and the `AdapterCmd` module). Per Change Classification, the `.fsi` contract and the
package's surface baselines MUST be updated in the same change. The delta is **additive
only** — no existing signature is removed or modified (FR-002), so the baseline diff is
append-only and every existing consumer compiles unchanged (SC-004).

**Principle alignment**:
- **I (Spec → FSI → Tests → Impl)**: the new public contract is sketched in
  `contracts/controls-elmish.fsi` and pinned by failing-first tests
  (`tests/Elmish.Tests/`) before the `.fs` body. Because this is a genuine public
  surface addition, the Principle-I "FSI transcript through the packed library"
  sub-requirement applies and is satisfied by the `FsiTranscripts` gate loading the new
  `ControlsElmish.programOfWidget` / `AdapterCmd.toCmd` symbols from the packed
  `FS.Skia.UI.Controls.Elmish` (unlike `067`, which had no public symbol to load).
- **II (Visibility in `.fsi`)**: every new public function/module is declared in
  `ControlsElmish.fsi`; the `.fs` adds no public binding without an `.fsi` counterpart.
  No per-binding `internal` is introduced.
- **III (Idiomatic simplicity)**: pure function composition (`view >> Widget.toControl`)
  and a `List.map`/`List.choose` over the effect list into a `Cmd`. No SRTP, type
  providers, custom operators, or non-trivial computation expressions. No
  justification-requiring feature is used.
- **IV (MVU boundary)**: this package **is** the Elmish/MVU boundary. The feature keeps
  `init`/`update` pure and the effect model explicit; the `Cmd<'msg>` bridge is a **pure**
  mapping from the existing `AdapterEffect` union to Elmish `Cmd<'msg>` (no I/O performed
  by the conversion — the resulting `Cmd` dispatches through Elmish's standard
  dispatcher at runtime, exactly as any `Cmd` does). The effect **interpreters**
  (`interpretKeyboardEffect`/`interpretControlEffect`) are unchanged (FR-009). `route`
  is total over every `AdapterEffect` case so no effect is silently dropped (FR-003,
  Principle VII).
- **V (Synthetic evidence)**: **none planned.** The Widget-view lowering and the `Cmd`
  bridge are real; the parity and round-trip tests exercise real values. No
  `[S]`/`[S*]`/`[SEH]` is anticipated; the evidence file states this explicitly.
- **VI (Test evidence)**: failing-first tests for US1 (Widget-view parity), US2 (command
  round-trip), US3 (legacy unchanged), US4 (mixed migration) plus an FsCheck round-trip
  property; they fail before the `.fs`/`.fsi` additions and pass after.
- **VII (Observability & safe failure)**: the `Cmd` bridge is **total** — `route` covers
  every `AdapterEffect` case, `[] -> Cmd.none`, order preserved, nothing thrown or
  dropped. No new log path or unsupported-environment message is introduced (pure
  mapping, no I/O).

### Repository Governance Decisions

- **Template ownership**: **N/A — no template-source change.** The feature edits the
  shipped `FS.Skia.UI.Controls.Elmish` package API, not `.template.config/template.json`,
  `template/capabilities.yml`, or `template/fragments/**`. The adapter package is already
  referenced by generated products (`GeneratedProduct.fs` `controls-elmish-reference`
  rule); adding API does not change which files the template emits. Template **pin**
  refresh to the bumped package version is a post-merge concern owned by the
  `fs-skia-template-update` skill, not this plan.
- **Dependency impact**: **N/A — no product dependency change.**
  `src/Controls.Elmish/Controls.Elmish.fsproj` already references `Fable.Elmish`
  (source of `Cmd<'msg>`); no new `<PackageReference>` and no `Directory.Packages.props`
  / `docs/dependencies.md` / `DependencyReport` change. The base `FS.Skia.UI.Controls`
  reference set is untouched — in particular it gains **no** `Fable.Elmish` (FR-006,
  SC-005). The only added reference is **test-only**: `FsCheck` (already pinned 3.3.3)
  on `tests/Elmish.Tests/Elmish.Tests.fsproj`.
- **Command-surface impact**: **No build-target semantics change.** No edit to
  `build.fsx`/`build/Governance/**`, `Dev`, `Verify`, `Ci`, `TemplateCheck`,
  `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, or
  `EvidenceAudit`. `Route` already escalates a public `src/**/*.fsi` change to the
  `package-surface` rule (`Routing.fs`, `PackageSurfaceCheck` / `FsiTranscripts` /
  `PerPackageSurfaceDiff`); no routing edit is needed. FAKE-backed commands share
  `.fake` state and are **not** safe to run concurrently — run them sequentially in the
  deterministic order below; safe non-FAKE reads/checks may parallelize. Example order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t Verify`
- **Generated project impact**: **Behavior-neutral, surface-touching.** Generated apps
  already reference `FS.Skia.UI.Controls.Elmish` and wire it via `ControlsElmish.program`;
  the additive `programOfWidget`/`AdapterCmd` API is available to generated products but
  changes no default/minimal generated contents, selected-Controls guidance, local
  skills, validation logs, or placeholder/excluded-history scans. `GeneratedProduct.fs`
  references `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` (line ~2023) and
  the adapter dependency-ownership note; both stay consistent after the baseline
  regenerates (the note is unchanged; the baseline gains the additive entries).
- **Evidence paths**:
  - Spec-dir (`specs/068-controls-elmish-command-model/readiness/`):
    - `readiness/package-surface-expectations.md` — required by the `package-surface`
      routing rule; records the **additive-only** `FS.Skia.UI.Controls.Elmish` delta and
      the regenerated-baseline rationale (SC-006).
    - `readiness/controls-elmish-command-model.md` — feature-specific (spec §Evidence
      obligations): the Widget-view path, the `AdapterCommand`↔`Cmd<'msg>` mapping rule,
      the lowering-parity result, and the command round-trip property results.
    - Plus the FAKE-emitted gate logs the escalated path produces.
  - Repo-root baselines regenerated by `RefreshSurfaceBaselines` (reviewed in the diff):
    - `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` (reflection FQ-name
      public surface) — gains the new symbols.
    - `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt` (raw `.fsi`
      snapshot for `PerPackageSurfaceDiff`) — gains the new signature lines.
- **`.fsi` / contract impact**: **Public contract change (Tier 1, additive).**
  `src/Controls.Elmish/ControlsElmish.fsi` gains `ControlsElmish.programOfWidget`,
  `ControlsElmish.widgetView`, and the `AdapterCmd` module
  (`none`/`ofMessage`/`productMessages`/`toCmd`). No existing signature is removed or
  changed (FR-002, FR-007). The two `FS.Skia.UI.Controls.Elmish` baselines are
  regenerated; **no other package's `.fsi` or baseline changes** (SC-006). A short
  compatibility note (additive; `Widget` path preferred, `Control` path frozen peer) goes
  in the evidence file (interacting-requirements resolution; mirrors `065` Q1).
- **MVU/effect boundary**: This package is the boundary. `Model`/`Msg` are
  product-owned; `init`/`update` stay pure; the effect channel is the existing
  `AdapterCommand`/`AdapterEffect`. The new `AdapterCmd.toCmd`/`productMessages`/
  `ofMessage`/`none` are pure conversions between that effect list and Elmish `Cmd<'msg>`;
  no interpreter behavior changes (FR-009). Real interpreter evidence is the existing
  adapter tests, unchanged.
- **Synthetic evidence**: **none.** No mocks/fakes/placeholders/canned responses — the
  Widget lowering and the `Cmd` bridge are real and the property test runs real generated
  commands. No `[S]` disclosure expected; if any task ships placeholder logic it must
  carry the Principle V disclosure, but the plan's intent is fully-real evidence.
- **Test evidence**: failing-first semantic tests in `tests/Elmish.Tests/` (committed red,
  then greened): US1 Widget-view parity (`programOfWidget` lowers equal to
  `program (view >> Widget.toControl)`), US2 command round-trip (recording dispatcher),
  US3 legacy `Control`-view program unchanged, US4 mixed migration (`Widget.ofControl`
  bridge), plus the FsCheck round-trip property (`productMessages (cmd)` dispatched by
  `toCmd` ≥1000 cases, no counterexample). The existing
  `ControlsElmishAdapterContractTests.fs` dependency guard (no `Fable.Elmish` in base
  Controls) is retained/extended (SC-005). Governance: the `package-surface` escalated
  gate set plus the serialized six-target order.
- **Observability**: the `Cmd` bridge is total and never throws (Principle VII); `route`
  covers every `AdapterEffect` case so non-product effects are carried, not dropped
  (FR-003). No new diagnostic, log path, or unsupported-environment message is introduced.
- **Deferred scope**: current obligation is the additive Widget-view + `Cmd` bridge, the
  two evidence artifacts, the regenerated baselines, and the escalated gate pass.
  **Deferred** (not this feature): wiring `067` keyed reconciliation into the adapter,
  any incremental-rendering path, any base `FS.Skia.UI.Controls` surface/dependency
  change, deprecating the legacy `Control`-view path (stays a peer), design-token/Penpot
  work (`069`), catalog regeneration, and migrating the remaining 41 controls (`070`).

**Gate result**: PASS — no unjustified violations. Two `N/A`s (Template ownership,
Dependency impact) carry one-line rationales as required by `GeneratedGuidanceCheck`; all
other decision areas are filled.

## Project Structure

### Source (edited — `src/Controls.Elmish/`)

```
src/Controls.Elmish/
  ControlsElmish.fsi    # EDIT — add programOfWidget, widgetView, and module AdapterCmd
  ControlsElmish.fs     # EDIT — implementations (pure composition + effect->Cmd mapping)
  Controls.Elmish.fsproj# UNCHANGED — Fable.Elmish already referenced; no new dependency
```

No new compile unit and no `<Compile>` reordering — the additions live in the package's
existing single source pair. `open Elmish` (the `Fable.Elmish` namespace) is added to name
`Cmd<'msg>` in the `.fsi`/`.fs`.

### Tests (new / edited — `tests/Elmish.Tests/`)

```
tests/Elmish.Tests/
  TypedControlsAdapterTests.fs       # EDIT — add US1 programOfWidget parity (no Widget.toControl
                                     #        in product code) alongside the existing 065 widgetView test
  AdapterCmdTests.fs                 # NEW  — US2 round-trip unit tests + FsCheck round-trip/order property
  ControlsElmishAdapterContractTests.fs # EDIT — assert the new .fsi surface; retain the
                                     #        base-Controls "no Fable.Elmish" dependency guard (SC-005)
  Elmish.Tests.fsproj                # EDIT — add AdapterCmdTests.fs (before Program.fs) + FsCheck PackageReference
```

### Sample (optional — `samples/`)

`samples/ControlsGallery/Program.fs` already uses the adapter; a small typed-authoring
panel may be switched to `programOfWidget` to smoke the no-shim path. Optional, not
required by a gate.

### Evidence

```
specs/068-controls-elmish-command-model/readiness/
  package-surface-expectations.md      # required by the package-surface rule (additive delta)
  controls-elmish-command-model.md      # feature-specific: Widget-view + Cmd mapping + parity/round-trip
  (+ FAKE-emitted gate logs)

readiness/                              # repo-root baselines regenerated by RefreshSurfaceBaselines
  surface-baselines/FS.Skia.UI.Controls.Elmish.txt        # + new public symbols
  per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt   # + new .fsi lines
```

### Untouched (explicitly)

The base `FS.Skia.UI.Controls` package and its `.fsi`/baseline, every other package, the
renderer/layout/diagnostics, the effect interpreters, `template/**`, `build/Governance/**`,
`Routing.fs`, and the `AdapterProgram`/`AdapterCommand`/`AdapterEffect`/`AdapterSubscription`
shapes.

## Routing & validation

Run **`./fake.sh build -t Route` first** and run only the gates it prints. Because the diff
edits a public `src/**/*.fsi` (`src/Controls.Elmish/ControlsElmish.fsi`), `Route` escalates
to the **`package-surface`** rule (tier `FocusedAuthority`) and prints `PackageSurfaceCheck`,
`FsiTranscripts`, `PerPackageSurfaceDiff`. (Note: the adapter `.fsi` lives in
`src/Controls.Elmish/`, a sibling of `src/Controls/`, so it does **not** match the
`controls-public-surface` rule — that rule is `src/Controls/**` only. `Route` is
authoritative; run exactly what it prints.) `Route --enforce` requires
`readiness/package-surface-expectations.md`.

Serialized six-target escalated/maintainer-verify order (run sequentially — FAKE state is
not concurrency-safe), as this is a consumer-contract change:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

## Phase 0 — Research

See [research.md](./research.md). All Technical Context items are resolved (no
`NEEDS CLARIFICATION` remains): the Widget-view construction shape, the
`AdapterCommand`↔`Cmd<'msg>` total-mapping rule, how the adapter surface is tracked
(reflection baseline + per-package snapshot, not `capabilities.yml` contracts), the
no-new-dependency confirmation, and the FsCheck round-trip strategy are all decided there.

## Phase 1 — Design & Contracts

- [data-model.md](./data-model.md) — the additive API algebra: `programOfWidget`,
  `widgetView`, and the `AdapterCmd` functions, plus the mapping/round-trip rules over the
  unchanged `AdapterEffect<'msg>` union and Elmish `Cmd<'msg>`.
- [contracts/controls-elmish.fsi](./contracts/controls-elmish.fsi) — the additive public
  signature sketch the failing-first tests pin and the implementation must satisfy
  (existing signatures shown for context, marked unchanged).
- [quickstart.md](./quickstart.md) — a product author's before/after: a typed view that
  used to end in `Widget.toControl` now uses `programOfWidget`; and folding adapter effects
  into a standard Elmish `Cmd<'msg>` via `AdapterCmd.toCmd`.
- Agent context: `AGENTS.md` SPECKIT plan reference updated to this plan.

## Phase 2 — Task planning approach (not executed here)

`speckit-tasks` will produce the dependency-ordered breakdown. Expected shape (mirrors the
spec's user-story priorities, failing-first):

1. Wire-up: add the FsCheck reference + `AdapterCmdTests.fs` to `Elmish.Tests.fsproj`;
   stub the new `.fsi`/`.fs` symbols; confirm `Dev` builds.
2. Failing-first tests: US1 `programOfWidget` parity (P1), US2 command round-trip (P1) +
   FsCheck property, US3 legacy-unchanged (P1), US4 mixed-migration (P3), and the
   dependency guard.
3. Implement `widgetView`/`programOfWidget` (pure composition) and the `AdapterCmd` module
   (effect->`Cmd` mapping) to green each story in priority order.
4. Regenerate both `FS.Skia.UI.Controls.Elmish` baselines (`RefreshSurfaceBaselines`);
   confirm the delta is additive-only.
5. Evidence: author the two `readiness/*.md` artifacts.
6. `Route` + serialized six-target order; fix to green.

Critical path: wire-up → US1/US2 red → `programOfWidget`/`AdapterCmd` → round-trip
property → baseline regen → evidence → gates.
