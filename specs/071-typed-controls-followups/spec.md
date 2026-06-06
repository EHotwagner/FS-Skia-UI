# Feature Specification: Close Out the Deferred Typed-Controls-Migration Follow-Ups

**Feature Branch**: `071-typed-controls-followups`
**Created**: 2026-06-06
**Status**: Draft
**Input**: User description: "create specs for the deferred tasks of the last feature." — the last feature is `070-typed-controls-migration`; this feature specifies the completion of its four deferred (`[ ]`) tasks.

## Overview

Feature `070` (migrate the remaining 41 controls to the typed `FS.Skia.UI.Controls.Typed`
Props/MVU front door) landed on `main` with **41 of its 45 tasks complete**. Four tasks
were explicitly deferred and carried forward as named follow-ups; this feature closes them
out. They split into two independent themes:

- **Catalog single-source completion** (deferred `070` task T007). The catalog
  cross-check single source — `CatalogGen.catalogFacts` in `build/Governance/CatalogGen.fs`,
  which regenerates `src/Controls/catalog.yml` and `Catalog.fs` — currently generates only
  the original **6** reference rows (`TextBlock`, `Button`, `TextBox`, `CheckBox`,
  `DataGrid`, `Stack`). The other **41** catalog rows exist with correct module names but
  are still **hand-maintained**, outside the generation markers. `070` delivered the
  typed-Props ⟷ catalog *cross-check* substance standalone (`070` task T036, a green test
  over all 41 controls), but the *single-source currency* substance — every catalog row
  generated from one fact table, never hand-edited — is only proven for 6 of 47. This
  feature brings all **47** rows under generation so `ControlsCatalogGenerationCheck`
  enforces currency over the complete set.

- **Typed gallery panel coverage and render evidence** (deferred `070` tasks T037–T039).
  `070`'s lowering-parity guarantee makes the existing render/accessibility suites
  transparent to the typed surface, but no test, sample panel, or captured render evidence
  yet exercises a *typed-authored* gallery panel end-to-end. This feature adds a
  representative typed-authoring panel (≥1 control per mechanic group), extends the
  rendering/accessibility suites to cover it at ≥2 viewports, surfaces it in the persistent
  `ControlsGallery` sample, and captures deterministic viewport render evidence.

Both themes are pure completion of in-flight `070` scope. They reopen no `070`/`065` design
decision, add no new control beyond the existing 47 catalog rows, and change no shipped
public `.fsi` signature — `070` already shipped the 41 typed modules and their parity
tests. The deferred-scope boundary `070` drew (catalog **expansion**, overlays,
virtualization, motion → `071+`) stays out of scope here; this feature is the *housekeeping*
`071`, not the breadth-expansion `071+`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Every catalog row is generated from the single fact source (Priority: P1)

A framework maintainer who needs to update a catalog row's facts (a control's module name,
its required attribute, its evidence pointer) edits **one** source — `CatalogGen.catalogFacts`
— and regenerates, for **any** of the 47 controls. No catalog row is hand-maintained in
`catalog.yml`/`Catalog.fs`, and the currency gate (`ControlsCatalogGenerationCheck`) fails
if any generated row drifts from the fact table for the full 47, not just the original 6.

**Why this priority**: This is the load-bearing single-source guarantee the `066`/`070`
catalog-generation strategy promises. Today a maintainer editing one of the 41
hand-maintained rows gets no currency enforcement, so the rows can silently drift from the
typed surface they are supposed to mirror. Closing the 6→47 gap is what makes "the catalog
is generated, never hand-edited" true for the whole catalog.

**Independent Test**: Extend `catalogFacts` to all 47 ids, regenerate via
`./fake.sh build -t RefreshSurfaceBaselines`, then hand-edit one generated row in
`catalog.yml` and confirm `ControlsCatalogGenerationCheck` fails (currency enforced); revert
and confirm it passes.

**Acceptance Scenarios**:

1. **Given** the extended fact table, **When** `catalog.yml` and `Catalog.fs` are
   regenerated, **Then** all **47** rows are produced from `catalogFacts` — each wrapped in
   its `BEGIN/END GENERATED: typed-catalog/<id>` markers — and **zero** catalog rows remain
   hand-maintained outside the markers.
2. **Given** the regenerated artifacts, **When** `ControlsCatalogGenerationCheck` runs,
   **Then** it passes and reports currency over all 47 facts (not 6).
3. **Given** any single generated row is manually altered, **When** the currency gate runs,
   **Then** it fails and names the stale `typed-catalog/<id>` region and the regeneration
   command.
4. **Given** the `066` fixture-iteration cross-check, **When** it reads one fixture per
   fact, **Then** a fixture exists for every one of the 47 facts and the cross-check stays
   green.

### User Story 2 - A typed-authored gallery panel is rendered, tested, and evidenced (Priority: P2)

A maintainer (and a consumer reading the gallery sample) can see the typed front door
exercised end-to-end: a gallery panel authored entirely through
`FS.Skia.UI.Controls.Typed.*` `view` functions — at least one control per mechanic group —
renders in the persistent `ControlsGallery`, is covered by the rendering and accessibility
suites at ≥2 viewports, and has deterministic captured render evidence in the feature's
readiness folder.

**Why this priority**: `070`'s parity guarantee proves typed views lower identically, but a
visible, captured typed-authored panel is the human-facing proof that the migrated surface
actually composes and renders as a real panel. It is lower-risk than US1 (it adds tests,
sample code, and evidence — no generated governance artifact changes), so it ships second.

**Independent Test**: Build the extended `ControlsGallery` sample, render the typed panel at
two viewports through the existing render-smoke path, and assert the rendering/accessibility
suites covering the typed panel pass; confirm the captured evidence file exists and is
deterministic (re-running produces identical bytes).

**Acceptance Scenarios**:

1. **Given** the typed-authoring gallery panel, **When** the rendering and accessibility
   suites run against it at ≥2 viewports, **Then** they pass and the panel includes at least
   one control from each mechanic group (display, input, stateful input, layout container,
   navigation/composite, overlay, selection collection, charts/graph).
2. **Given** the persistent `ControlsGallery` sample, **When** it is launched, **Then** the
   typed-authored panel appears alongside the existing panels as a render/interaction smoke
   over the migrated surface.
3. **Given** the deterministic render path, **When** the typed gallery viewport evidence is
   captured to the readiness folder, **Then** the evidence is real (render-only, no `[S]`
   synthetic disclosure) and re-capture is byte-identical.

### Edge Cases

- **Chart/DataGrid evidence special-case**: one catalog fact carries the chart/data-grid
  evidence pointer (the `renderFSharpRow` special-case that mirrors the legacy
  `withChartDataGridEvidence` row). Expanding to 47 facts MUST preserve this special-case so
  the chart/graph rows generate with their evidence pointer intact, not a generic blank.
- **Marker insertion is generated, not hand-placed**: the `BEGIN/END GENERATED:
  typed-catalog/<id>` marker pairs for the 41 newly-generated rows MUST be produced by the
  generator on first regeneration, not hand-typed, so the markers and inner rows always
  match the fact table.
- **`custom-control` row**: `custom-control` has no fabricated typed `Props` schema (it
  bridges via `Widget.ofControl`, per `070` FR-006). Its catalog fact MUST still generate a
  valid row consistent with how `070`'s standalone cross-check (T036) treats it
  (bridge-typed), without inventing a required attribute it does not have.
- **Fixture count**: the `066` fixture-iteration test reads one fixture per fact; extending
  from 6 to 47 facts requires a fixture for each newly-generated id so the cross-check does
  not fail on a missing fixture.
- **Gallery panel breadth vs. launch cost**: the typed panel covers ≥1 control per mechanic
  group (representative), not all 47 — full per-control coverage already lives in `070`'s
  parity/contract suites; the panel is a render/interaction smoke, deliberately bounded.
- **Render evidence is render-only**: the captured typed-gallery evidence is deterministic
  render-only proof and is **not** a substitute for the persistent gallery launch; it
  carries no synthetic `[S]` disclosure.

> Interacting / conflicting requirements: "all 47 rows generated, never hand-edited (US1)"
> vs. "`070`'s standalone cross-check (T036) already proved the typed-Props ⟷ catalog
> consistency on the 6-fact single source." Resolution: T036's *consistency* proof stays as
> the typed-surface cross-check; US1 adds the *currency* proof by bringing the remaining 41
> rows under generation. The two are complementary, not duplicative — neither replaces the
> other.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `CatalogGen.catalogFacts` MUST be extended from the current **6** entries to
  all **47** catalog ids, and `src/Controls/catalog.yml` + `src/Controls/Catalog.fs` MUST be
  **regenerated** from it via the existing `RegenerateCatalog` / `./fake.sh build -t
  RefreshSurfaceBaselines` path — never hand-edited.
- **FR-002**: After regeneration, **zero** catalog rows may remain hand-maintained outside
  the `BEGIN/END GENERATED: typed-catalog/<id>` markers; every one of the 47 rows MUST live
  inside its generated region in both generated files.
- **FR-003**: `ControlsCatalogGenerationCheck` MUST pass and enforce currency over the full
  47-fact set — a manual edit to any generated row MUST make the gate fail, naming the stale
  `typed-catalog/<id>` region and the regeneration command.
- **FR-004**: The `renderFSharpRow` chart/data-grid evidence special-case MUST be preserved
  (and extended as needed) so the chart/graph and DataGrid rows generate with their evidence
  pointer intact across the full set.
- **FR-005**: The `066` fixture-iteration cross-check MUST stay green over all 47 facts — a
  fixture MUST exist for every generated id, and the hand-maintained `typedPropsById` map (or
  its equivalent) MUST be in lockstep with the 47 facts so the "`catalogFacts` ids == typed
  ids" and "each `requiredAttribute` PascalCased is a `Props` field" assertions hold for the
  full set.
- **FR-006**: The `custom-control` row MUST generate consistently with its bridge-typed
  treatment (no fabricated required attribute), matching how `070`'s standalone cross-check
  classifies it.
- **FR-007**: A representative **typed-authoring gallery panel** — authored only through
  `FS.Skia.UI.Controls.Typed.*` `view` functions, covering **≥1 control per mechanic group**
  — MUST be added to the persistent `samples/ControlsGallery/Program.fs`, providing a
  render/interaction smoke over the migrated surface.
- **FR-008**: The rendering and accessibility test suites (`RenderingTests.fs` /
  `AccessibilityTests.fs`) MUST be extended to cover the typed gallery panel at **≥2
  viewports** and MUST pass.
- **FR-009**: Deterministic typed-gallery viewport render evidence MUST be captured to
  `specs/071-typed-controls-followups/readiness/controls-rendering.md` (or the feature's
  readiness folder), render-only and re-capture byte-identical, carrying **no** `[S]`
  synthetic disclosure.
- **FR-010**: No shipped public `FS.Skia.UI.Controls` `.fsi` signature may change — the 41
  typed modules already shipped in `070`; this feature MUST be additive/internal at the
  package surface (test code, sample code, generated governance artifacts, and the fact
  table only). Any per-package surface baseline regeneration MUST show an additive-only (or
  empty) delta.
- **FR-011**: No `070`/`065` design decision is reopened, and no control beyond the existing
  47 catalog rows is added — catalog **expansion**, overlays, virtualization, and motion
  remain deferred to a later `071+` feature.

> Interacting / conflicting requirements: "bring all 47 rows under generation (FR-001/FR-002)"
> vs. "change no shipped public surface (FR-010)". Resolution: catalog generation touches
> only the governance artifacts (`catalog.yml`/`Catalog.fs`) and the fact table, which are
> generated/internal cross-check inputs — not the package's public typed `.fsi`. Regenerating
> the catalog row for a control does not alter that control's shipped typed module signature.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package identity, version, or shipped-content change in this
  feature. `FS.Skia.UI.Controls` typed modules shipped in `070`; here only generated
  governance artifacts (`catalog.yml`, `Catalog.fs`), the build-side fact table
  (`build/Governance/CatalogGen.fs`), test code, sample code, and readiness evidence change.
  Version bump/pack and template-pin refresh remain post-merge concerns owned by
  `speckit-merge` / `fs-skia-template-update`.
- **Public contract impact**: **None to shipped `.fsi`.** No public typed/legacy signature
  is added, removed, or changed. The catalog single source (`catalog.yml`/`Catalog.fs`) is
  regenerated; any per-package surface baseline delta is additive-only or empty.
- **State workflow impact**: **None.** No new command/effect/subscription/interpreter
  behavior; the typed gallery panel reuses the already-shipped typed façades and their
  reused MVU models. No I/O is added to any `update`.
- **Layout/rendering impact**: **None observable beyond new coverage.** The typed gallery
  panel lowers via the same parity-tested `Widget.toControl` path; rendering/accessibility
  suites gain coverage of it but the render pipeline, Skia/Vulkan output, and
  unsupported-environment diagnostics are unchanged. Captured render evidence is
  deterministic render-only.
- **Evidence obligations**: Required real evidence paths under
  `specs/071-typed-controls-followups/readiness/`:
  - `readiness/catalog-single-source.md` — the 6→47 fact-table extension, the regeneration
    rationale, and a statement that all 47 rows are generated (zero hand-maintained).
  - `readiness/controls-rendering.md` — deterministic typed gallery viewport render evidence
    (render-only, no `[S]`).
- **Unsupported scope**: No catalog **expansion** (new controls beyond the 47 rows), no
  overlays/virtualization, no motion/animation, no live Penpot/MCP integration, no
  design-token value changes, no legacy-API deprecation, no keyed-reconciliation (`067`) or
  `Controls.Elmish` (`068`) change.
- **Build-target impact**: No **new** build target. The fact-table/catalog regeneration
  exercises the existing `RefreshSurfaceBaselines` / `ControlsCatalogGenerationCheck` path;
  the test/sample changes route through the standard controls gates. Run **only** the gates
  `./fake.sh build -t Route` prints for this diff, FAKE-backed gates sequentially.
  `validation.contract.yml` stays generated from `Routing.fs`.

## Key Entities

- **`CatalogGen.catalogFacts`** (existing, `build/Governance/CatalogGen.fs`): the single
  fact source for catalog rows; extended from 6 to 47 entries here.
- **Generated catalog artifacts** (existing, `src/Controls/catalog.yml`,
  `src/Controls/Catalog.fs`): regenerated from `catalogFacts` with one
  `BEGIN/END GENERATED: typed-catalog/<id>` region per row; all 47 generated, none
  hand-maintained.
- **`066` catalog cross-check** (existing, fixture-iteration test + `typedPropsById` map):
  reads one fixture per fact and asserts typed-Props ⟷ catalog consistency; extended to all
  47 in lockstep.
- **Typed gallery panel** (new, in `samples/ControlsGallery/Program.fs`): a representative
  typed-authored panel (≥1 control per mechanic group) over the `070` migrated surface.
- **Rendering/accessibility suites** (existing, `RenderingTests.fs` /
  `AccessibilityTests.fs`): extended to cover the typed gallery panel at ≥2 viewports.
- **Typed gallery render evidence** (new, `readiness/controls-rendering.md`): deterministic
  render-only viewport evidence for the typed panel.

## Success Criteria *(mandatory)*

- **SC-001**: `CatalogGen.catalogFacts` contains all **47** catalog ids, and **100%** of the
  47 catalog rows in `catalog.yml` and `Catalog.fs` are generated from it (zero rows
  hand-maintained outside the generation markers).
- **SC-002**: `ControlsCatalogGenerationCheck` passes and enforces currency over the full 47
  facts — a deliberate edit to any one generated row makes the gate fail and names the stale
  region.
- **SC-003**: The `066` fixture-iteration cross-check passes with a fixture present for every
  one of the 47 facts, and the typed-id/`requiredAttribute` assertions hold for the full set.
- **SC-004**: The persistent `ControlsGallery` sample contains a typed-authoring panel
  covering **≥1 control per mechanic group**, authored only through
  `FS.Skia.UI.Controls.Typed.*` (no `Attr`/`*.create` call in the panel code).
- **SC-005**: The rendering and accessibility suites cover the typed gallery panel at **≥2
  viewports** and pass.
- **SC-006**: Deterministic typed-gallery render evidence exists under the feature readiness
  folder, is render-only with **no** `[S]`/`[S*]` disclosure, and re-capture is
  byte-identical.
- **SC-007**: The `FS.Skia.UI.Controls` per-package surface baseline delta is additive-only
  or empty (no shipped public signature changed) — verified by `PackageSurfaceCheck` /
  `PerPackageSurfaceDiff`.
- **SC-008**: `./fake.sh build -t Route` over the branch diff prints the applicable gate set
  and **every printed gate passes**; the `EvidenceAudit` verdict is **PASS** with no
  `[S]`/`[S*]` disclosures.

## Assumptions

- "The deferred tasks of the last feature" are exactly the four `[ ]` tasks remaining in
  `specs/070-typed-controls-migration/tasks.md`: **T007** (catalog single source 6→47),
  **T037** (rendering/accessibility coverage of a typed gallery panel), **T038** (persistent
  gallery typed panel), and **T039** (typed gallery render evidence). T007 is the only task
  explicitly tagged `**DEFERRED** … Tracked as follow-up`; T037–T039 are the remaining
  unchecked gallery/evidence tasks carried with it.
- The 41 typed modules, their lowering-parity tests, the typed-Props ⟷ catalog standalone
  cross-check (`070` T036), and the `fs-skia-typed-controls` skill all **already shipped in
  `070`** and are not reauthored here — this feature only completes the generation currency
  and the gallery/evidence coverage.
- This is the *housekeeping* `071` (completing `070`'s in-flight scope). The breadth
  **expansion** that `070` deferred to "`071+`" (new controls, overlays, virtualization,
  motion) is a separate, later feature and is out of scope here.
- The catalog generation mechanism (per-row `BEGIN/END GENERATED` markers, the
  `renderFSharpRow` chart/data-grid evidence special-case, the `066` one-fixture-per-fact
  iteration) is reused as-is and extended to 47 — no new generation mechanism is introduced.
- The typed gallery panel is representative (≥1 per mechanic group), not exhaustive —
  full per-control coverage already exists in `070`'s parity/contract suites.

## Out of Scope

- Catalog **expansion** — any control beyond the existing 47 catalog rows
  (buttons/pickers/date-time), overlays/virtualization, and motion/animation — deferred to a
  later `071+` feature.
- Reauthoring or changing any of the 41 typed modules, their `.fsi` signatures, or their
  lowering-parity tests (shipped in `070`).
- Any change to shipped design-token values or the `069` token layer; any live Penpot/MCP
  integration.
- Removal, deprecation-flagging, or behavioral change of the legacy `Attr` /
  `Control.create` / per-control `*.create` API (frozen peer).
- Changes to the keyed-reconciliation internals (`067`) or the `Controls.Elmish` adapter
  signature / command model (`068`).
- Re-opening any `065`/`070` design decision, introducing a new `Widget` representation, or
  changing `Control.render` / IR semantics.
- Version bump/pack and template-pin refresh (owned post-merge by `speckit-merge` /
  `fs-skia-template-update`).
