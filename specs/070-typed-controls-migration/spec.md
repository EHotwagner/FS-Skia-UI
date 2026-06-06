# Feature Specification: Migrate Remaining 41 Controls to the Typed Props/MVU Front Door

**Feature Branch**: `070-typed-controls-migration`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "implement the next part of docs/reports/2026-06-05-1802-typed-controls-front-door-implementation-plan.md and update the plan with progress" — roadmap feature **070 — Migrate remaining 41 controls to typed Props/MVU** (§13).

## Overview

Features `065` (typed controls front door), `066` (typed catalog generation), `067`
(internal keyed reconciliation), `068` (`Controls.Elmish` command model), and `069`
(design tokens + Penpot DTCG → generated F#) have all landed on `main` (post-`069`
lib bump `0.1.73-preview.1`, template pins `45aa4d5`). The roadmap's stated sequencing
is "type the authoring layer first, wire tokens second, **migrate breadth last**" (plan
§13). This feature is the migrate-breadth-last step — the one that *completes* the typed
front door.

Feature `065` proved an additive, compile-time-typed authoring surface
(`Widget<'msg>` + per-control immutable `Props` records, plus per-control
`Model`/`Msg`/`update` for stateful controls) that lowers to the existing
`Control<'msg>` IR, on a **six-control reference slice**: `TextBlock`, `Button`,
`CheckBox`, `Stack`, `TextBox`, `DataGrid` (under the `FS.Skia.UI.Controls.Typed`
namespace, `src/Controls/Widget.fs*` + `src/Controls/Widgets/*`). The catalog
(`src/Controls/catalog.yml`) lists **47 supported controls**; **41 remain legacy-only**
— they can only be authored through the weakly-typed `Attr<'msg> list` / per-control
`*.create` API.

This feature migrates those **41 remaining controls** to the typed front door, so every
catalog control has an immutable, compiler-checked `Props` record + `defaults` + `view`
(and, for stateful controls, a typed `init`/`update` façade over the **existing** MVU
model). It applies the **exact template `065` established** — pick fields from the
variable taxonomy, write the `Props` record + `defaults`, write a `view` that lowers to
`Control<'msg>`, and prove it with a per-control **lowering-parity test** (typed `view` ≡
legacy builder output). It is **additive**: the legacy `Attr` / `Control.create` /
per-control `*.create` API stays byte-frozen, so `PackageSurfaceCheck` sees only
additions.

Per plan §16.4, the new **`fs-skia-typed-controls`** capability skill — which does not
yet exist — is authored in this same branch, because "each new skill should land in the
same feature branch that first needs it." `070` is essentially "run that skill 41 times,"
so the skill must be validated against this real migration work.

The 41 remaining controls (catalog ids), grouped by the distinct mechanic each
exercises:

- **Pure display**: `rich-text`, `label`, `image`, `icon`, `separator`, `badge`,
  `progress-bar`, `spinner`, `validation-message`
- **Pure input / command**: `icon-button`, `numeric-input`, `radio-group`, `switch`,
  `slider`
- **Stateful input (reuse existing MVU)**: `text-area` (reuses `TextInput`)
- **Layout containers (over `Widget` children)**: `grid`, `dock`, `wrap`, `border`,
  `panel`, `scroll-viewer`, `split-view`
- **Navigation / composite**: `tabs`, `menu`, `context-menu`, `toolbar`
- **Overlay / transient**: `tooltip`, `dialog`, `toast`, `overlay`
- **Selection collections (reuse existing MVU)**: `list-view`, `list-box`,
  `multi-select-list`, `combo-box`, `tree-view`
- **Charts / graph (reuse existing models)**: `line-chart`, `bar-chart`, `pie-chart`,
  `scatter-plot`, `graph-view`
- **Escape hatch**: `custom-control`

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every catalog control is authorable through the typed front door (Priority: P1)

A product author building a view with `FS.Skia.UI.Controls` can author **any** of the 47
catalog controls — not just the original six — using a typed, immutable `Props` record
and a `view` returning `Widget<'msg>`, with the F# compiler checking field names, types,
and required values. The weakly-typed `Attr<'msg> list` keyed by strings is no longer the
only way to reach 41 of the controls.

**Why this priority**: This is the defining value of the feature — completing the typed
authoring surface so the typed front door is the universal, preferred authoring path for
the whole control suite. If only this story ships, the feature delivers its core
capability.

**Independent Test**: For each of the 41 migrated controls, author a small view through
its typed `FS.Skia.UI.Controls.Typed.<Module>.view { defaults with … }` and assert it
compiles and produces a `Widget<'msg>` that renders, without using any `Attr`/`*.create`
call.

**Acceptance Scenarios**:

1. **Given** the migrated typed surface, **When** an author composes a view referencing a
   typed control's `Props` fields, **Then** it compiles, mistyped/missing required fields
   are compile errors, and the `view` returns a `Widget<'msg>`.
2. **Given** a typed control nested in a typed `Stack`/layout container's `Children`,
   **When** the view is built, **Then** child `Widget<'msg>` values compose without any
   `Widget.toControl`/`Widget.ofControl` call appearing in author code.
3. **Given** the complete typed suite, **When** a maintainer enumerates the catalog,
   **Then** **all 47** catalog controls have a typed `FS.Skia.UI.Controls.Typed` module
   exposing `defaults` + `view`.

### User Story 2 - Typed views lower to byte-identical legacy IR (Priority: P1)

A maintainer needs certainty that the typed surface is a faithful façade and changes no
downstream behavior. For every migrated control, the typed `view` lowers to a
`Control<'msg>` **structurally equal** to what the equivalent legacy `*.create`/`Attr`
call produces today — so the entire downstream pipeline (render → layout → diagnostics →
accessibility → event bindings → evidence) is exercised unchanged, and no typed-only
behavioral divergence can slip in.

**Why this priority**: Lowering parity is the load-bearing correctness guarantee of the
whole front-door strategy (it is what made `065` safe to ship). It protects every existing
render/a11y/interaction test without duplicating them, and it is the gate that catches a
typed façade silently diverging from the legacy builder.

**Independent Test**: For each migrated control, build the same logical control two ways —
legacy `*.create [ … ]` and typed `Typed.<Module>.view { defaults with … } |>
Widget.toControl` — normalize attribute ordering, and assert structural `Control<'msg>`
equality.

**Acceptance Scenarios**:

1. **Given** a migrated control, **When** its typed `view` output is lowered via
   `Widget.toControl` and normalized, **Then** it is structurally equal to the legacy
   builder's `Control<'msg>` for the same logical inputs (same `Kind`, `Key`, attributes,
   children, content, accessibility).
2. **Given** an optional event prop set to `None` (e.g. a button-like `OnClick`, a
   toggle's `OnChanged`), **When** the typed `view` lowers, **Then** it emits **no** event
   binding (never a default/placeholder message), matching the legacy path.
3. **Given** the existing render, accessibility, and interaction test suites,
   **When** they run against the lowered typed controls, **Then** they pass unchanged
   (parity makes the typed surface transparent to them).

### User Story 3 - Stateful controls reuse existing MVU models, not forks (Priority: P1)

A maintainer migrating a stateful control (text area, the selection collections, charts,
graph) gets a typed `Props`/`init`/`update`/`view` façade that **delegates to the existing
MVU model** (e.g. `TextInput`, the `Collections`/`DataGrid` models, the chart/graph
models) rather than inventing a parallel model. State logic stays single-sourced; the
typed layer is a pure typed façade.

**Why this priority**: Forking MVU models would double the state logic and the test
surface and create drift between the typed and legacy stateful paths. Reuse keeps the
migration additive at the model layer (the `065`/`068` discipline) and keeps `init`/
`update` behavior provably identical.

**Independent Test**: For each stateful migrated control, dispatch a representative `Msg`
through the typed façade's `update` and assert the resulting model/effects equal those of
the existing model's `update` for the same input (delegate, don't fork).

**Acceptance Scenarios**:

1. **Given** a stateful control's typed façade, **When** `init`/`update` is called,
   **Then** the model and effects are exactly those the existing reused model produces (no
   new effect types, no I/O in `update`).
2. **Given** a stateful control, **When** its public surface is inspected, **Then** it
   reuses the existing `Model`/`Msg`/`Effect` types (no parallel/duplicate model type is
   introduced).

### User Story 4 - Legacy authoring stays a frozen, compiling peer (Priority: P2)

An existing consumer who authored controls with the legacy `Attr`/`Control.create`/
per-control `*.create` API recompiles against the new package and observes **no** change:
every legacy signature still exists and compiles, with no behavioral diff. The typed
surface is purely additive.

**Why this priority**: `FS.Skia.UI.Controls` is a shipped public contract; the `065`
decision (Q1) was to keep the legacy API as a permanent peer with no deprecation in this
window. Migration must not break or flag the legacy path.

**Independent Test**: Build the existing controls samples/tests against the new package
with no source edits; assert `PackageSurfaceCheck` reports an **additive-only** delta (new
typed modules; zero removed or changed legacy signatures).

**Acceptance Scenarios**:

1. **Given** the regenerated `FS.Skia.UI.Controls` surface baseline, **When**
   `PackageSurfaceCheck` runs, **Then** the delta is additive-only — no legacy signature is
   removed, renamed, or changed.
2. **Given** the existing legacy-authored samples and tests, **When** compiled against the
   new package, **Then** they build and pass with no edit.

### Edge Cases

- **Module-name collision**: legacy `module Button`/`TextBox`/`Stack`/… live in the
  root `FS.Skia.UI.Controls` namespace. Every typed module MUST live under
  `FS.Skia.UI.Controls.Typed.*` (the `065` Q2 decision) so the clean names are reused
  without shadowing the legacy modules.
- **Container children**: layout/navigation/overlay containers (`grid`, `dock`, `wrap`,
  `border`, `panel`, `scroll-viewer`, `split-view`, `tabs`, `toolbar`, `dialog`,
  `overlay`, …) take `Widget<'msg>` children/content, lowered via `Widget.toControl`,
  with child order preserved — exactly as the `065` `Stack` façade does.
- **`custom-control`**: the catalog's escape-hatch control has no fixed attribute schema.
  Its typed affordance is `Widget.ofControl` (lift an author-built `Control<'msg>` into
  the typed tree); it does not gain a synthetic `Props` record of made-up fields. This is
  called out explicitly so SC-001's "all 47 typed" is satisfied honestly, not by
  fabricating a schema.
- **Optional events lower to no binding**: any optional event prop (`'msg option` or
  `(_ -> 'msg) option`) set to `None` MUST lower to **no** event binding, never a default
  message — matching the `065` `Button.OnClick`/`CheckBox.OnChanged` behavior.
- **No `obj` / no string-keyed escape in the typed surface**: no migrated `Props` field
  may be typed `obj`, an untyped value, or a stringly-typed event name. Required values are
  non-optional fields; optional values get a default via `defaults`.
- **Charts / graph data**: chart and graph controls carry product-owned data
  (series/points/nodes/edges) as typed `Data`-class fields reusing the existing chart/graph
  data types; the typed façade does not redefine the data model.
- **Catalog cross-check currency**: feature `066` cross-checks each catalog row's `Module`
  fact against the `FS.Skia.UI.Controls.Typed` surface. As rows become typed, the catalog
  fact source and the typed surface MUST stay consistent so `ControlsCatalogGenerationCheck`
  passes; any catalog `Module`-fact update is a regeneration, never a hand-edit.

> Interacting / conflicting requirements: "every catalog control becomes typed (SC-001)"
> vs. "`custom-control` has no fixed schema". Resolution: `custom-control`'s typed
> affordance is the `Widget.ofControl` bridge (already public from `065`), not a fabricated
> `Props` record — it is "typed" in that it produces a `Widget<'msg>` through the front
> door, satisfying the universal-authoring goal without inventing fields that do not exist.

> Interacting / conflicting requirements: "additive-only, legacy frozen (US4)" vs. "typed
> modules reuse clean names like `Button`/`Stack`". Resolution: the `FS.Skia.UI.Controls.Typed`
> namespace segment isolates the typed modules so reuse of clean names is purely additive and
> never shadows or changes a legacy signature.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Each of the **41 remaining catalog controls** (every catalog id except the
  six already typed: `text-block`, `button`, `check-box`, `text-box`, `stack`, `data-grid`)
  MUST gain a typed authoring surface under the `FS.Skia.UI.Controls.Typed` namespace,
  declared in a curated `.fsi`, following the `065` template: an immutable `Props` record,
  a `defaults` value, and a `view` returning `Widget<'msg>`.
- **FR-002**: Every migrated typed `view` MUST lower to a `Control<'msg>` **structurally
  equal** to the `Control<'msg>` the equivalent legacy `*.create`/`Attr` builder produces
  for the same logical inputs (proven by a per-control lowering-parity test), so the
  downstream render/layout/diagnostics/accessibility/event pipeline is byte-unchanged.
- **FR-003**: Each migrated `Props` record MUST draw its fields from the fixed variable
  taxonomy (Identity, Content, Data, Behavior, Variant, Layout, Theme/style, Accessibility,
  Events) defined in plan §3.4. **No** field may be typed `obj`, carry an untyped/`Untyped`
  payload, or use a string-keyed event name. Required values are non-optional fields;
  optional values resolve through `defaults`.
- **FR-004**: Stateful migrated controls (at minimum `text-area`, the selection
  collections `list-view`/`list-box`/`multi-select-list`/`combo-box`/`tree-view`, and the
  chart/graph controls that own runtime state) MUST expose a typed `init`/`update`/`view`
  façade that **delegates to the existing MVU model** (`TextInput`, the
  `Collections`/`DataGrid`/chart/graph models) — reusing the existing `Model`/`Msg`/`Effect`
  types, never forking them, with no I/O in `update`.
- **FR-005**: Optional event props set to `None` MUST lower to **no** event binding (never a
  default message), matching the `065` event-lowering behavior.
- **FR-006**: The `custom-control` catalog row's typed affordance MUST be the existing
  `Widget.ofControl` bridge (lift an author-built `Control<'msg>` into the typed tree); it
  MUST NOT be given a fabricated `Props` schema of fields it does not have.
- **FR-007**: The legacy authoring API — `Control.create`, `Control.standard`, `Attr.*`,
  and all per-control `*.create` modules — MUST remain **byte-frozen** (no signature
  added, removed, or changed), so the public-surface delta is **additive-only**.
- **FR-008**: The migration MUST add **no new package dependency** to
  `FS.Skia.UI.Controls` (in particular not `Fable.Elmish`); typed modules depend only on
  `Widget` + the existing control/model modules already in the package.
- **FR-009**: Each typed module MUST live under `FS.Skia.UI.Controls.Typed.*` and reuse the
  control's clean name without shadowing the legacy module of the same name (the `065` Q2
  decision).
- **FR-010**: The affected `FS.Skia.UI.Controls` per-package surface baseline(s) MUST be
  regenerated and reviewed in the diff; the regenerated delta MUST be additive-only.
- **FR-011**: Migrated typed views and façades MUST carry **no `[S]` synthetic disclosure** —
  every lowering is **real** and lowering-parity-tested (US2). If any single control cannot
  be lowered with real parity in this feature, that control MUST carry the `[S]` disclosure
  per Constitution Principle V and be named explicitly in the evidence (the intent is zero
  `[S]`).
- **FR-012**: The `066` catalog cross-check MUST stay green: as catalog rows become typed,
  the catalog `Module`/required-attribute facts and the `FS.Skia.UI.Controls.Typed` surface
  MUST stay consistent, and any catalog fact update MUST be a **regeneration** (via the
  `066` single-source path), never a hand-edit, so `ControlsCatalogGenerationCheck` passes.
- **FR-013**: A new **`fs-skia-typed-controls`** capability skill MUST be authored in this
  branch (canonical `.agents/skills/`, `.claude` peer regenerated via
  `RefreshSurfaceBaselines`), teaching how to author with the typed front door and how to
  add a typed control: pick taxonomy fields, write `Props` + `defaults` + `view`, add the
  mandatory lowering-parity test, and reuse existing MVU models for stateful controls. It
  MUST pass `SkillSyncCheck`/`SkillQualityCheck`.
- **FR-014**: The controls gallery sample (`samples/ControlsGallery/Program.fs`) MUST be
  extended to author a representative set of the newly typed controls through the typed
  front door, providing a render/interaction smoke over the migrated surface.

> Interacting / conflicting requirements: "all 41 migrated in one feature (FR-001)" vs.
> the routing escalation and surface-review burden of a large public-surface delta.
> Resolution: the 41 are migrated as one coherent feature but **grouped by mechanic** into
> the existing `Widgets/*` file structure, each control independently parity-tested, so the
> diff is reviewable per group and any single control can be deferred (carrying `[S]` and
> named in evidence per FR-011) without blocking the rest.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: The active package is `FS.Skia.UI.Controls` (`src/Controls/**`),
  which gains **additive** typed modules for the 41 remaining controls under
  `FS.Skia.UI.Controls.Typed.*`. No package identity moves; no legacy Charts package
  migration is involved (the typed chart façades ship in the same `FS.Skia.UI.Controls`
  package as their legacy peers). Version bump/pack and template pin are post-merge concerns
  owned by the `speckit-merge` and `fs-skia-template-update` skills.
- **Public contract impact**: **Yes, additive.** New typed-module `.fsi` files are added
  under `src/Controls/`; no legacy `.fsi` signature changes. The `FS.Skia.UI.Controls`
  per-package surface baseline is regenerated; the delta is additive-only.
- **State workflow impact**: **None new.** Stateful typed façades delegate to the
  **existing** MVU models (`TextInput`, `Collections`/`DataGrid`/chart/graph); no new
  command/effect/subscription/interpreter behavior is introduced and no I/O is added to any
  `update`.
- **Layout/rendering impact**: **None observable.** Every typed `view` lowers to the same
  `Control<'msg>` the legacy builder emits (lowering parity), so `Control.render`, layout,
  charts, DataGrid, screenshots, Vulkan, Skia, visual output, and unsupported-environment
  diagnostics are unchanged. A gallery render smoke confirms stable output.
- **Evidence obligations**: Required real evidence paths under
  `specs/070-typed-controls-migration/readiness/`:
  - `readiness/typed-controls-migration.md` — the migration design, the 41-control
    grouping, the per-control taxonomy choices, and an explicit statement that lowering is
    **real** (no `[S]`).
  - `readiness/typed-lowering-parity.md` — the parity matrix (41 controls × legacy ≡ typed
    structural equality), the keystone proof.
  - `readiness/package-surface-expectations.md` — the additive `FS.Skia.UI.Controls`
    surface delta and the regenerated-baseline rationale.
  - `readiness/controls-rendering.md` — viewport render evidence for the typed gallery panel.
- **Unsupported scope**: No new controls beyond the existing 47 catalog rows; no catalog
  **expansion** (new buttons/pickers/date-time — that is `071+`); no overlays/virtualization
  or motion work (`071+`); no live Penpot/MCP integration; no design-token value changes
  (`069` shipped the token layer); no removal or deprecation-flagging of the legacy `Attr`
  API; no change to the keyed-reconciliation internals (`067`) or the `Controls.Elmish`
  adapter signature (`068`).
- **Build-target impact**: No **new** build target is required for the controls migration
  itself — the change is confined to `src/Controls/**`, which already routes via
  `controls-public-surface`. The new `fs-skia-typed-controls` **skill** edit routes through
  the skill gate set (`SkillSyncCheck`, `SkillQualityCheck`, `SkillContractPathCheck`,
  `TemplateUpdateSkillPackageCheck`) and `RefreshSurfaceBaselines` regenerates the `.claude`
  peer. `validation.contract.yml` stays generated from `Routing.fs`. Because the change
  edits public `src/Controls/**` `.fsi`, `Route` escalates to the `controls-public-surface`
  gate set (plus the skill gates for the skill edit); run **only** the gates `Route` prints,
  FAKE-backed gates sequentially.

## Key Entities

- **`Widget<'msg>`** (existing, `src/Controls/Widget.fs*`): the opaque typed return type;
  unchanged. All 41 migrated `view` functions return it; `Widget.ofControl`/`toControl`
  are the bridges (`custom-control` uses `ofControl`).
- **Typed control modules** (new, under `src/Controls/Widgets/*`, namespace
  `FS.Skia.UI.Controls.Typed`): the 41 added `Props` + `defaults` + `view` (+ `init`/
  `update` for stateful) modules, grouped by mechanic.
- **Existing MVU models** (existing, reused — `TextInput`, `Collections`, `DataGrid`,
  chart/graph models): consumed by the stateful typed façades; not forked.
- **Legacy per-control modules** (existing, `Control.fsi` + `DataGrid`/`Collections`/
  `Charts`/`RichText` `.fsi`): the frozen peer authoring API; byte-unchanged.
- **Catalog facts** (existing, `066` — `build/Governance/CatalogGen.fs`, `catalog.yml`,
  `Catalog.fs`): cross-checked against the typed surface; kept current by regeneration.
- **`fs-skia-typed-controls`** (new skill): the capability skill teaching the typed-control
  authoring/migration flow, validated against this migration.

## Success Criteria *(mandatory)*

- **SC-001**: **All 47** catalog controls have a typed `FS.Skia.UI.Controls.Typed` module
  reachable through the front door — the original 6 plus the 41 migrated here (`custom-control`
  satisfied via the `Widget.ofControl` bridge, per FR-006).
- **SC-002**: For **100%** of the 41 migrated controls, the typed `view` lowers to a
  `Control<'msg>` structurally equal to the legacy builder's output (lowering-parity matrix;
  no control divergent).
- **SC-003**: Every stateful migrated control's typed `update` produces model/effects
  identical to the existing reused model's `update` for the same input (no forked model
  type introduced) — verified for each stateful control.
- **SC-004**: The `FS.Skia.UI.Controls` regenerated surface baseline delta is **additive-only**
  (new typed modules; **zero** removed or changed legacy signatures), verified by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff`.
- **SC-005**: No migrated `Props` field is typed `obj` / untyped / string-keyed-event
  (verified by inspection/grep of the new `.fsi`).
- **SC-006**: `FS.Skia.UI.Controls` declares **no** new package dependency (in particular
  not `Fable.Elmish`).
- **SC-007**: `ControlsCatalogGenerationCheck` passes — the catalog facts and the typed
  surface are consistent and current (no hand-edited catalog artifact).
- **SC-008**: The `.claude` skill tree is a current regeneration of `.agents`, and the new
  `fs-skia-typed-controls` skill passes `SkillSyncCheck`/`SkillQualityCheck`.
- **SC-009**: The existing legacy-authored samples and `Controls.Tests` build and pass
  against the new package with no source edit (legacy peer unbroken).
- **SC-010**: `./fake.sh build -t Route` over the branch diff prints the escalated
  `controls-public-surface` gate set (plus the skill gates for the skill edit) and **every
  printed gate passes**; `EvidenceAudit` verdict is **PASS** with no `[S]`/`[S*]`
  disclosures.

## Assumptions

- The set to migrate is exactly the 41 catalog ids that are not in the `065` six-control
  slice: `rich-text`, `label`, `image`, `icon`, `separator`, `badge`, `icon-button`,
  `text-area`, `numeric-input`, `radio-group`, `switch`, `slider`, `list-view`, `list-box`,
  `multi-select-list`, `combo-box`, `tree-view`, `grid`, `dock`, `wrap`, `border`, `panel`,
  `scroll-viewer`, `split-view`, `tabs`, `menu`, `context-menu`, `toolbar`, `tooltip`,
  `dialog`, `toast`, `overlay`, `progress-bar`, `spinner`, `validation-message`,
  `line-chart`, `bar-chart`, `pie-chart`, `scatter-plot`, `graph-view`, `custom-control`.
- The migration is performed as **one feature** (per plan §13/§16.4 "run the skill 41
  times"), grouped by mechanic into the existing `src/Controls/Widgets/*` file structure,
  rather than split across multiple `NNN-*` features — each control independently
  parity-tested so the work and its review stay tractable.
- The migration applies the **exact** `065` template (`Widget<'msg>`, taxonomy-driven
  immutable `Props`, `defaults`, lowering `view`, per-control lowering-parity test) and the
  `065` decisions (Q1 legacy kept as peer; Q2 `FS.Skia.UI.Controls.Typed` namespace; Q4
  sealed `Widget`; Q5 reuse existing models) — none are reopened here.
- Stateful controls reuse their already-shipped MVU models (`TextInput` for text-area;
  the `Collections`/`DataGrid` models for the selection collections; the existing
  chart/graph models) via a typed façade; no parallel model is invented (the `065`/`068`
  discipline).
- `custom-control` is "typed" via the existing `Widget.ofControl` bridge, not a fabricated
  `Props` schema, because it is a deliberate escape hatch with no fixed attribute set.
- The `fs-skia-typed-controls` skill lands in this branch (plan §16.4) so its guidance is
  validated against the real 41-control migration rather than written speculatively; it
  becomes the operative skill for the later `071+` new-control work.
- No `Routing.fs` rule change is required for the controls migration itself (it stays inside
  the existing `controls-public-surface` path); the only governance edits are the additive
  skill and any catalog-fact regeneration the typed-surface cross-check requires.

## Out of Scope

- Catalog **expansion** — new controls beyond the existing 47 rows (buttons/pickers/
  date-time), overlays/virtualization, and motion/animation — all deferred to `071+`.
- Any change to shipped design-token **values** or the `069` token layer; any live
  Penpot/MCP integration.
- Removal, deprecation-flagging, or behavioral change of the legacy `Attr` /
  `Control.create` / per-control `*.create` API (it stays a frozen peer; deprecation is a
  later, separate decision).
- Changes to the keyed-reconciliation internals (`067`) or the `Controls.Elmish` adapter
  signature / command model (`068`) — those are settled in prior features.
- Re-opening any `065` design decision (Q1–Q5), introducing a new `Widget` representation,
  or changing `Control.render` / IR semantics.
- Version bump/pack and template-pin refresh (owned post-merge by `speckit-merge` /
  `fs-skia-template-update`).
