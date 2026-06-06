# Feature Specification: Controls.Elmish Command Model (Widget View + Cmd Alignment)

**Feature Branch**: `068-controls-elmish-command-model`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "implement the next part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md and update the plan with progress" — roadmap feature **068 — `Controls.Elmish` command model (`Widget` view + `Cmd<'msg>` alignment)** (§13). Resolves plan open question **Q3** (§12): `AdapterProgram.View` gains a `Widget`-returning path now, instead of forcing consumers to bridge with `Widget.toControl`.

## Overview

Features `065` (typed controls front door), `066` (typed catalog generation), and
`067` (internal keyed reconciliation) have landed on `main`. The next roadmap step
is to **converge the Elmish adapter onto the typed authoring surface**.

Today the adapter's program contract is `AdapterProgram.View: 'model -> Control<'msg>`
(`src/Controls.Elmish/ControlsElmish.fsi`). Because `065` made `Widget<'msg>` the
*preferred* return type of every typed `view`, a product that authors with the typed
front door must end its own `view` with a manual `Widget.toControl` shim to satisfy
the adapter — the seam `065` §6 deliberately left as a temporary bridge and §12 Q3
scheduled for this feature. In parallel, the adapter models commands as its own
`AdapterCommand<'msg> = AdapterEffect<'msg> list` with no documented relationship to
Elmish's standard `Cmd<'msg>`, so adapter effects cannot participate in an ordinary
Elmish command/dispatch model without bespoke host glue.

This feature **additively** closes both gaps:

1. A program can supply a `view` that returns `Widget<'msg>` directly; the adapter
   lowers it internally via the existing `Widget.toControl` seam, so typed authoring
   composes end-to-end with no boundary shim.
2. A documented, total mapping aligns `AdapterCommand<'msg>` with Elmish `Cmd<'msg>`,
   so adapter effects bridge into the standard Elmish command model.

It is **additive and consumer-contract-affecting** but confined to the
`FS.Skia.UI.Controls.Elmish` package: the existing `Control<'msg>`-returning program
constructor, `AdapterCommand`/`AdapterEffect`/`AdapterSubscription`, and the effect
interpreters all stay byte-for-byte and behaviorally unchanged. The base
`FS.Skia.UI.Controls` package is untouched and keeps its `Fable.Elmish`-free
dependency split.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Typed program with no boundary shim (Priority: P1)

A product author builds a view entirely with the typed front door
(`FS.Skia.UI.Controls.Typed.*` composing into a `Widget<'msg>`) and hands it to the
Elmish adapter without ending the view in `Widget.toControl`. The adapter accepts the
`Widget<'msg>`-returning view and lowers it internally.

**Why this priority**: This is the defining behavior of the feature — making the
preferred typed authoring surface (`065`/`066`) compose through the adapter without a
manual lowering shim. If only this story ships, the feature delivers its core value.

**Independent Test**: Construct an adapter program whose `view` returns `Widget<'msg>`
(built from typed modules); run it through the adapter and assert the rendered control
tree is produced with no `Widget.toControl` call appearing in product code.

**Acceptance Scenarios**:

1. **Given** a program whose `view: 'model -> Widget<'msg>` is built from typed
   modules, **When** it is constructed via the new Widget-view path and rendered,
   **Then** it produces a render result without the product calling `Widget.toControl`.
2. **Given** the same logical view written two ways — once returning `Widget<'msg>`
   via the new path, once returning `Control<'msg>` via `view >> Widget.toControl` on
   the existing path — **When** both are rendered, **Then** the resulting
   `Control<'msg>` trees are structurally equal.

### User Story 2 - Adapter effects in a standard Elmish command model (Priority: P1)

A product author already using Elmish `Cmd<'msg>` wants adapter-produced effects
(keyboard/control-runtime/host/diagnostic) to flow through the same standard command
pipeline. A documented bridge converts an `AdapterCommand<'msg>` into an Elmish
`Cmd<'msg>` (and back), so adapter effects dispatch their product messages through
ordinary Elmish dispatch.

**Why this priority**: Command-model alignment is the second half of "converge the
adapter onto `Widget`/`Cmd<'msg>`" (plan §6, §13). Without it the adapter's command
list is an island that every host must hand-wire.

**Independent Test**: Take an `AdapterCommand<'msg>` carrying two `DispatchProductMessage`
effects; convert it to a `Cmd<'msg>`, run it through a recording dispatcher, and assert
exactly those two product messages are dispatched in order.

**Acceptance Scenarios**:

1. **Given** an `AdapterCommand<'msg>` containing product-message effects, **When** it
   is converted to a `Cmd<'msg>` and dispatched, **Then** exactly those product
   messages are delivered, in order, with none dropped or duplicated.
2. **Given** an empty `AdapterCommand<'msg>` (no effects), **When** converted, **Then**
   the result is the Elmish no-op command and dispatch delivers nothing.

### User Story 3 - Legacy Control-returning program unchanged (Priority: P1)

A product author with an existing program whose `view` returns `Control<'msg>`
recompiles against the new package version and observes no source change required and
no behavioral difference.

**Why this priority**: The adapter is a shipped public contract; converging onto the
typed surface must not break the existing one. Additive-only is the compatibility
guarantee.

**Independent Test**: Compile and run an existing `Control<'msg>`-view program against
the new package; assert it builds unchanged and produces identical render/command
output to the prior version.

**Acceptance Scenarios**:

1. **Given** an existing program built with `ControlsElmish.program (view: _ -> Control<'msg>)`,
   **When** compiled against the new package, **Then** it compiles with no source edit
   and behaves identically.
2. **Given** the existing `AdapterCommand`/`AdapterEffect`/`AdapterSubscription` types
   and the effect interpreters, **When** used as before, **Then** their signatures and
   behavior are unchanged.

### User Story 4 - Mixed migration (Priority: P3)

A product mid-migration runs some screens authored with the typed `Widget` surface and
others still authored with legacy `Control<'msg>` builders, and can host both through
the adapter. A `Widget` that wraps a legacy control via `Widget.ofControl` lowers
identically to authoring that control directly.

**Why this priority**: Real migrations are incremental; the feature should not force an
all-at-once switch. Lower priority because both endpoints already work independently
(US1, US3) — this story only asserts they coexist.

**Independent Test**: In one program, compose a typed `Widget` subtree alongside a
`Widget.ofControl`-wrapped legacy control; render and assert the lowered tree matches
the equivalent all-legacy authoring.

**Acceptance Scenarios**:

1. **Given** a `Widget<'msg>` built by wrapping a legacy `Control<'msg>` with
   `Widget.ofControl`, **When** lowered through the adapter, **Then** the result equals
   rendering that legacy control directly.
2. **Given** a product using the Widget-view path for one program and the
   Control-view path for another, **When** both are hosted, **Then** each renders and
   dispatches correctly with no interference.

### Edge Cases

- **Widget wrapping a legacy control**: `Widget.ofControl c |> (adapter lowering)` MUST
  equal rendering `c` directly — the bridge is identity on the lowering seam
  (`toControl (ofControl c) = c`, established in `065`).
- **Empty command**: an `AdapterCommand<'msg>` with no effects maps to the Elmish no-op
  command and back without inventing or dropping effects.
- **Effect ordering**: converting a multi-effect `AdapterCommand` preserves effect
  order through the `Cmd<'msg>` bridge so dispatch order is deterministic.
- **Non-product effects**: adapter effects that are *not* product messages
  (`DispatchControlRuntimeMessage`, `DispatchKeyboardMessage`, `DispatchHostCommand`,
  `ReportAdapterDiagnostic`) MUST be carried by the bridge under a single documented
  rule, not silently discarded, so the `Cmd<'msg>` alignment is total over every
  `AdapterEffect` case.

> Interacting / conflicting requirements: the package now offers **two** view return
> types (`Widget<'msg>` and the legacy `Control<'msg>`). Resolution: they are **peers**,
> with the `Widget` path documented as **preferred** and the `Control` path retained as
> a frozen peer — mirroring the `065` Q1 decision to keep the legacy authoring path as a
> permanent peer rather than deprecating it in the same feature. Neither path is removed
> or behaviorally changed by introducing the other.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The adapter MUST expose an additive way to construct a program whose view
  returns `Widget<'msg>` (`'model -> Widget<'msg>`), lowering it internally via the
  existing `Widget.toControl` seam, so typed authoring needs no boundary shim in product
  code.
- **FR-002**: The change MUST be **additive** — the existing
  `AdapterProgram.View: 'model -> Control<'msg>`, `ControlsElmish.program`,
  `AdapterCommand<'msg>`, `AdapterEffect<'msg>`, `AdapterSubscription<'msg>`, and the
  effect interpreters keep their current signatures and behavior; every existing program
  compiles with no source edit.
- **FR-003**: The adapter MUST provide a documented, **total** mapping between its
  `AdapterCommand<'msg>` (effect list) and Elmish `Cmd<'msg>`, enabling adapter effects
  to participate in a standard Elmish command/dispatch model. Every `AdapterEffect<'msg>`
  case MUST be accounted for by the mapping under a single documented rule.
- **FR-004**: The `Widget<'msg>`-returning view MUST lower to exactly the `Control<'msg>`
  that `Widget.toControl` produces — structurally equal to the legacy boundary
  `view >> Widget.toControl` result (lowering parity).
- **FR-005**: Lowering and command bridging MUST be **pure** — no I/O, no mutation, no
  reliance on wall-clock or randomness; identical inputs produce identical results.
- **FR-006**: The feature MUST add **no new package dependency**. The
  `FS.Skia.UI.Controls.Elmish` package already references `Fable.Elmish`, which supplies
  `Cmd<'msg>`; the base `FS.Skia.UI.Controls` package MUST remain free of any
  `Fable.Elmish` reference, preserving the existing dependency split.
- **FR-007**: The public-surface change MUST be confined to the
  `FS.Skia.UI.Controls.Elmish` package (`src/Controls.Elmish/ControlsElmish.fsi`). That
  package's surface baseline is updated to reflect the **additive-only** delta (no
  removed or changed signatures); every other package's baseline stays unchanged.
- **FR-008**: Converting an `AdapterCommand<'msg>` to a `Cmd<'msg>` and dispatching it
  MUST deliver exactly the product messages the adapter command carried — same multiset,
  same order, none dropped or duplicated. This round-trip MUST be property-tested.
- **FR-009**: The adapter's effect/subscription interpretation
  (`interpretKeyboardEffect`, `interpretControlEffect`, `subscriptions`) MUST remain
  behaviorally unchanged; this feature adds a view path and a command bridge, not new
  interpreter semantics.
- **FR-010**: The Widget-view path and the legacy Control-view path MUST coexist as
  peers in one package; constructing or using one MUST NOT alter the other (see the
  interacting-requirements note).

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The active package is `FS.Skia.UI.Controls.Elmish`
  (`src/Controls.Elmish/**`), which gains **additive** public API. No package identity
  or contents move; the base `FS.Skia.UI.Controls` package is untouched. No legacy Charts
  package migration is involved. The version bump/pack and template pin are post-merge
  concerns owned by the `speckit-merge` and `fs-skia-template-update` skills, not this
  spec.
- **Public contract impact**: **Yes, additive.** `src/Controls.Elmish/ControlsElmish.fsi`
  gains new signatures (a `Widget<'msg>`-view program path and the `AdapterCommand`↔
  `Cmd<'msg>` bridge); no existing signature is removed or changed. The
  `FS.Skia.UI.Controls.Elmish` per-package surface baseline is updated; no other `.fsi`
  or baseline changes. Samples that demonstrate the typed adapter path may be extended.
- **State workflow impact**: The command model gains a documented `Cmd<'msg>` bridge over
  the existing `AdapterCommand`/`AdapterEffect` effect list; effect and subscription
  **interpreter** semantics are unchanged, and no new I/O is introduced.
- **Layout/rendering impact**: **None.** No layout, charts, DataGrid, rendering,
  screenshot, Vulkan, Skia, visual-output, or unsupported-environment diagnostic behavior
  changes; the Widget-view path lowers to the same IR the existing path renders.
- **Evidence obligations**: The `package-surface` routing rule
  (`build/Governance/Routing.fs`, paths `src/**/*.fsi`) matches
  `src/Controls.Elmish/ControlsElmish.fsi` and requires
  `readiness/package-surface-expectations.md` under this feature's spec dir (recording the
  additive `FS.Skia.UI.Controls.Elmish` delta). A feature-specific evidence artifact
  `readiness/controls-elmish-command-model.md` MUST record the Widget-view path, the
  `AdapterCommand`↔`Cmd<'msg>` mapping rule, the lowering-parity result, and the
  command round-trip property-test results.
- **Unsupported scope**: No design-token/Penpot work, no catalog change, no migration of
  the remaining 41 controls, no wiring of the `067` keyed reconciliation into the adapter
  or a live incremental renderer, and no change to the base `FS.Skia.UI.Controls` public
  surface or dependency set. Visual output, release, platform, and distribution are
  unchanged.
- **Build-target impact**: No semantic change to `Dev`, `Verify`, `Ci`, `PackLocal`,
  `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, `TemplateDrift`,
  `EvidenceGraph`, or `EvidenceAudit`. Because the change edits a public `src/**/*.fsi`,
  `Route` escalates to the `package-surface` gate set (`PackageSurfaceCheck`,
  `FsiTranscripts`, `PerPackageSurfaceDiff`); run **only** the gates `Route` prints, and
  run FAKE-backed gates sequentially.

## Key Entities

- **AdapterProgram** (existing): the program record. Gains an additive construction path
  whose `view` returns `Widget<'msg>`; the existing `View: 'model -> Control<'msg>` field
  and constructor are unchanged.
- **AdapterCommand / AdapterEffect** (existing): the adapter's command list and effect
  union. Unchanged in shape; now additionally bridgeable to Elmish `Cmd<'msg>`.
- **Widget<'msg>** (existing, from `065`): the opaque typed view tree. Becomes a
  first-class adapter view return type via the lowering seam `Widget.toControl`.
- **Cmd<'msg>** (Elmish, already a dependency of this package): the standard command model
  the adapter aligns to via the documented mapping.

## Success Criteria *(mandatory)*

- **SC-001**: A program whose `view` returns `Widget<'msg>` runs through the adapter
  end-to-end with **zero** `Widget.toControl` calls in product code (verified by the
  User Story 1 test).
- **SC-002**: The Widget-view path renders a `Control<'msg>` tree structurally equal to
  the same program written as `view >> Widget.toControl` on the existing path (lowering
  parity).
- **SC-003**: Converting any `AdapterCommand<'msg>` to a `Cmd<'msg>` and dispatching it
  delivers exactly its product messages — same multiset and order — across at least
  1,000 generated commands with no counterexample (round-trip property, FR-008).
- **SC-004**: Every existing `FS.Skia.UI.Controls.Elmish` program and test compiles
  against the new package with no source edit and exhibits zero behavioral diff
  (additive guarantee).
- **SC-005**: `FS.Skia.UI.Controls` still declares **no** `Fable.Elmish` reference — the
  dependency split is preserved (dependency-governance guard).
- **SC-006**: The regenerated surface baseline delta is confined to the
  `FS.Skia.UI.Controls.Elmish` package and is **additive-only** (no removed or changed
  signatures), verified by `PerPackageSurfaceDiff` / `PackageSurfaceCheck`.
- **SC-007**: `./fake.sh build -t Route` over the branch diff prints the `package-surface`
  escalation and **every printed gate passes**.

## Assumptions

- `Widget.toControl` (`src/Controls/Widget.fsi`, shipped in `065`) is the lowering seam
  the adapter consumes internally; the invariant `toControl (ofControl c) = c` lets a
  legacy control bridge through `Widget` with no behavioral change (confirmed against
  source).
- `FS.Skia.UI.Controls.Elmish` already references `Fable.Elmish` (confirmed in
  `src/Controls.Elmish/Controls.Elmish.fsproj`), so aligning the command model with
  `Cmd<'msg>` adds no new dependency. `Cmd<'msg>` is the Elmish standard command type.
- The legacy `Control<'msg>`-view program path is kept as a **permanent peer** (preferred
  path is `Widget`), mirroring the `065` Q1 decision; this feature performs the `Widget`/
  `Cmd<'msg>` convergence that `065` §6 and §12 Q3 explicitly deferred to `068`.
- The feature is implemented in compiled F# and tested with the repo's existing
  Expecto + FsCheck harness (per `fsharp-build-orchestration`); no new test framework is
  introduced. Parity and command round-trip are the two keystone property tests.
- The `067` keyed reconciliation remains internal and is **not** consumed by the adapter
  in this feature.

## Out of Scope

- Wiring the `067` keyed reconciler or any incremental-rendering path into the adapter.
- Any change to the base `FS.Skia.UI.Controls` public surface or dependency set.
- Design tokens, Penpot, catalog regeneration, and migrating the remaining 41 controls.
- Removing or deprecating the legacy `Control<'msg>`-view path (it stays a peer).
- Performance tuning or benchmarking of the command bridge beyond correctness and
  determinism.
