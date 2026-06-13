# Tasks: Viewport Virtualization for Repeated Controls

**Feature branch**: `114-viewport-virtualization`
**Spec**: `specs/114-viewport-virtualization/spec.md`
**Plan**: `specs/114-viewport-virtualization/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is `[S]` or
`[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by the evidence audit.
See `readiness/task-graph.md` for the propagated view.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, **[US3]**, **[US4]** — user-story scope
- This whole feature is **Tier 1** (a breaking `ControlsElmish.fsi` `FrameMetrics`
  change — two new public fields — plus additive `Controls` `Collections` / `DataGrid` /
  `Types` `.fsi` surface for overscan + a11y total/position, and an internal
  `WorkReductionRecord` count seam — the top-level surface baseline and per-package
  baselines move); per-task `[T1]/[T2]` annotations are omitted because every phase
  matches the feature tier.

## Elmish/MVU applicability

Principle IV's dedicated `Model`/`Msg`/`Effect`/`init`/`update`/interpreter tasks are
**N/A** for this feature: `Update`, effects, subscriptions, commands, and the interpreter
are unchanged (FR-016/FR-017). The overscan-widened realized window and the
`VirtualItemsMaterialized`/`VirtualItemsTotal` counts live in the control-lowering /
retained step, not in `update`; dispatch *outcomes* for materialized rows stay
byte-identical. Offscreen focus/selection reuses the existing `DataGridMsg` set
(`ScrollRowsTo`/`SelectRow`/`ToggleRow`/`FocusCell`) over logical row keys/indices — it is
a newly *reachable* capability (an offscreen key previously had no realized-window effect),
not a changed transition. The interactive-UI run-and-use gate is **N/A** — the feature
delivers an internal virtualization contract + deterministic metrics observable via
`ControlsElmish.Perf.runScript`, plus offscreen addressability / a11y totals on the logical
model, not a new interactive surface. Recorded in the evidence-obligations task (T003 / T008).

## Governance risk level

**Medium** governance risk: the breaking `FrameMetrics` `.fsi` change + the additive
`Collections`/`DataGrid`/`Types` `.fsi` surface (overscan param/field, a11y
total/position) escalate `Route` to the **controls-public-surface** tier and move the
top-level + per-package surface baselines, but there is **no new gate**, no dependency
change, and no template-content change. Focused validation = the escalated gate set
`Route` prints (T023). Broad validation (full `Verify`) is not required because the change
set is two packages' contents plus the regenerated baselines + perf-corpus goldens.
Non-authoritative aggregate results are recorded as "focused rerun" notes in
`readiness/aggregate-hang-diagnostics.md`.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Scaffold `specs/114-viewport-virtualization/` and confirm spec + plan + research + data-model + contracts (`virtualization-contract.md`, `offscreen-addressability.md`) + quickstart + checklist are linked and current
- [X] T002 [P] [skillist: []] Create the `specs/114-viewport-virtualization/readiness/` scaffolds discoverable before implementation — `evidence-audit.md`, `evidence-graph.md`, `skill-loading-evidence.md`, `governance-risk-levels.md`, `aggregate-hang-diagnostics.md`, `runtime-limitations.md`, `generated-validation.md`, `byte-identity-authority.md`, `virtual-metrics-authority.md`, and the window-visibility not-applicable set — each naming its authoritative command, artifact path, failure class, and next action
- [X] T003 [skillist: []] Record feature Tier (Tier 1), affected packages (`FS.Skia.UI.Controls` — `Collections.visibleRange` overscan param + `CollectionModel`/`DataGridModel` `Overscan` field, DataGrid offscreen `SelectRow`/`ToggleRow`/`FocusCell`/`ScrollRowsTo` targeting, `AccessibilityMetadata` total/position, the internal `WorkReductionRecord` `VirtualMaterialized`/`VirtualTotal` counts; `FS.Skia.UI.Controls.Elmish` — the public `FrameMetrics` `VirtualItemsMaterialized`/`VirtualItemsTotal` fields), public-API impact (breaking `FrameMetrics` `.fsi` + additive `Collections`/`DataGrid`/`Types` surface + internal count seam reached via `InternalsVisibleTo`), Elmish/MVU + interactive-UI applicability (both N/A with the rationale above), and the required evidence obligations (bounded + non-scaling materialization 100/1000/10000, default-0 byte-identity, opt-in overscan edge-clamp correctness, offscreen focus/selection + boundary-crossing nav, a11y total + focused position, deterministic `VirtualItems*` goldens + idle 0/0, the 10000-row corpus scenario, baselines, XML-doc, 113 composition over the overscan-widened row set)

---

## Phase 2: Foundation

- [X] T004 [skillist: fs-skia-ui-widgets, fs-skia-reconciliation] Draft the public + internal surfaces as `.fsi` signatures (XML-doc each): in `src/Controls/Collections.fsi` add a trailing `overscan` parameter to `visibleRange` (additive) and an `Overscan: int` field on `CollectionModel` (default 0); on `DataGrid` add the `Overscan: int` field to `DataGridModel` (and its `.fsi` if present); in `src/Controls/Types.fsi` add `type CollectionPosition = { TotalItems: int; FocusedIndex: int option }` and an additive `Collection: CollectionPosition option` field on `AccessibilityMetadata` (precedent: feature 100's `Navigation: NavRange option`); in `src/Controls/RetainedRender.fsi` add the internal `VirtualMaterialized: int` / `VirtualTotal: int` fields to `WorkReductionRecord`; in `src/Controls.Elmish/ControlsElmish.fsi` add the public `VirtualItemsMaterialized: int` / `VirtualItemsTotal: int` `FrameMetrics` fields. Build compiles (signatures only)
- [X] T005 [skillist: fs-skia-ui-widgets] Implement the overscan-widened realized window in `src/Controls/Collections.fs` `visibleRange`: from the overscan-0 slice `(f, c, t)` and `N >= 0` compute `first' = max 0 (f - N)`, `last' = min (t - 1) (f + c - 1 + N)`, `count' = if t <= 0 then 0 else last' - first' + 1`, clamp negative `N` to 0; `N = 0` reproduces today's slice byte-identically (`FirstIndex + Count <= Total`, `0 <= FirstIndex`). Build compiles
- [X] T006 [skillist: fs-skia-ui-widgets] Exercise the drafted seam from FSI (call `visibleRange` with overscan 0 then `N`, at the top edge, the bottom edge, and a small `Total <= visible + 2N`; print each `VisibleRange` to show the default-0 byte-identity, the symmetric widening, and the edge clamp) and capture the session transcript to `readiness/fsi-session.txt`
- [X] T007 [skillist: []] Capture the intended top-level (`FrameMetrics` fields) + per-package (`Collections`/`DataGrid` `Overscan`, `Types` `CollectionPosition`/`AccessibilityMetadata.Collection`, internal `WorkReductionRecord` counts) surface baseline shape (the authoritative regen happens in T021) and note it in `readiness/`
- [X] T008 [skillist: []] Record unsupported-scope handling and failure diagnostics: OUT this rung — variable/measured row heights + row/column/text measurement caches (Phase 8), paint caches / damage rectangles / Skia picture boundaries (Phase 7), layout hot-path / text-measurement caches & layout-boundary hints (Phase 8), `SkiaViewer` backend / render-thread / compositor review (Phase 9), generalizing virtualization to non-DataGrid list/collection surfaces (DataGrid is the representative; the shared `Collections` model MAY carry overscan for future reuse), horizontal/column virtualization (rows only); uniform fixed `RowHeight` only (FR-018); features 110/111/112/113 unchanged (FR-017); Principle IV + interactive-UI gate N/A

**Checkpoint**: Foundation ready — story implementation may begin.

---

## Phase 3: User Story 1 (US1) — A large repeated control materializes only its visible window plus overscan

### Tests First (Principle I, Principle VI)

- [X] T009 [P] [US1] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature114OverscanTests` in `tests/Controls.Tests` (reaching internal seams via `InternalsVisibleTo "Controls.Tests"`): build a DataGrid with a bounded viewport realizing `V` rows and assert the realized-window count (= materialized `data-grid-row` nodes) `<= V + 2*overscan` at `Total ∈ {100, 1000, 10000}` and is **identical** across those totals (does not scale, FR-003, SC-001); a grid whose `Total <= V + 2*overscan` realizes the whole set (`materialized = Total`, transparent, FR-004); the realized window is edge-clamped at top/bottom (no index `< 0`, none `>= Total`, FR-002/FR-007)
- [X] T010 [US1] [skillist: fs-skia-ui-widgets] Wire the overscan-widened `VisibleRange` through `DataGrid.range` / `Collections.withRange` (pass `model.Overscan` into `visibleRange`) so `DataGrid.visibleRows rows visibleRange |> List.map (rowControl columns)` (`DataGrid.fs:214`/`:223`) materializes exactly the realized window; add `Overscan = 0` at **every** construction site (`Collections.init`, `Collections.withRange`, DataGrid model construction, `ReplaceRowCount` recompute, samples `ControlsGallery`/`DemoReel`, FSI preludes `scripts/*-prelude.fsx`). Make T009 pass (FR-001/FR-002/FR-003/FR-004)
- [X] T011 [US1] [skillist: []] Document the US1 independent validation path (render a 100-, 1000-, and 10000-row DataGrid scenario with the same bounded viewport + overscan; assert the materialized count is bounded by `V + 2*overscan` and identical across totals while the logical total scales) in `readiness/us1-validation.md`

**Checkpoint**: User Story 1 is functional and independently testable.

---

## Phase 4: User Story 2 (US2) — Overscan is opt-in and at-rest output is byte-identical

### Tests First

- [X] T012 [P] [US2] [skillist: fs-skia-ui-widgets, fs-skia-evidence-mode] Add a failing-first `Feature114OverscanParityTests` in `tests/Controls.Tests`: with overscan at its default (0) the realized rows, control geometry, and rendered scene for the corpus DataGrid scenarios are **byte-identical** to the pre-feature baseline — structural `Scene` equality (controls have no value equality) — and `VirtualItemsMaterialized` equals the prior realized-row count (FR-006, SC-002); with opt-in overscan `N` exactly the visible rows plus up to `N` correct adjacent logical rows materialize, the visible rows are unchanged, and overscan is clamped at the top/bottom edges with no fabricated/duplicated rows (FR-007, SC-003)
- [X] T013 [US2] [skillist: fs-skia-ui-widgets, fs-skia-reconciliation] Confirm overscan default 0 ⇒ realized window == today's visible slice byte-identical (FR-006) and that opt-in overscan materializes only real edge-clamped adjacent rows without shifting the visible region (FR-007); verify scrolling the realized window reuses row containers where the keyed diff permits (stable `row.Key` → reuse; this feature MUST NOT regress it, FR-008). Make T012 pass (FR-006/FR-007/FR-008, SC-002/SC-003/SC-007)

**Checkpoint**: User Story 2 is functional and independently testable.

---

## Phase 5: User Story 3 (US3) — Focus, selection, and accessibility remain correct across the visible/offscreen boundary

### Tests First

- [X] T014 [P] [US3] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input] Add a failing-first `Feature114OffscreenTests` in `tests/Controls.Tests`: focusing/selecting/toggling an **offscreen** logical row by key records it on the logical model and **relocates** the realized window to it without materializing every intervening row (the materialized count stays `<= V + 2*overscan`, FR-009/FR-010, SC-004); keyboard navigation moving focus past the last realized row lands on the correct next **logical** row and advances the realized window to include it (FR-011, SC-005); the FR-003 bound holds throughout (the window relocates, it does not expand)
- [X] T015 [US3] [skillist: fs-skia-ui-widgets, fs-skia-keyboard-input] Implement offscreen targeting in `src/Controls/DataGrid.fs`: `SelectRow`/`ToggleRow`/`FocusCell`/`ScrollRowsTo` on a (possibly offscreen) logical key/index update the logical model and compute the scroll offset that brings the target index into the realized window, then recompute `VisibleRange` via `DataGrid.range` / `Collections.withRange` with `model.Overscan` (relocate, never expand); boundary-crossing focus advances the focused logical index by one and relocates. Dispatch outcomes for already-materialized rows stay byte-identical (FR-016). Make T014 pass (FR-009/FR-010/FR-011)
- [X] T016 [P] [US3] [skillist: fs-skia-ui-widgets] Add a failing-first `Feature114AccessibilityTests` in `tests/Controls.Tests`: a virtualized DataGrid's `AccessibilityMetadata.Collection` reports `TotalItems` = the logical `RowCount` and `FocusedIndex` = the focused row's logical index (from `FocusedCell.RowKey`), independent of how many rows are materialized (FR-012, SC-005); a non-collection control reports `Collection = None` so at-rest a11y for existing controls is byte-identical
- [X] T017 [US3] [skillist: fs-skia-ui-widgets] Implement the additive `AccessibilityMetadata.Collection` / `CollectionPosition` population in `src/Controls/Types.fs` + the DataGrid a11y path — computed from the **logical** model (`RowCount` for `TotalItems`, the focused logical index for `FocusedIndex`), never the materialized slice; `None` for all non-collection controls. Make T016 pass (FR-012)

**Checkpoint**: User Story 3 is functional and independently testable.

---

## Phase 6: User Story 4 (US4) — The virtualization contract is observable as deterministic metrics

### Tests First

- [X] T018 [P] [US4] [skillist: fs-skia-controls-host, fs-skia-evidence-mode] Add a failing-first `Feature114VirtualMetricsTests` in `tests/Elmish.Tests` over `ControlsElmish.Perf.runScript`: a frame that builds a virtualized DataGrid records `VirtualItemsMaterialized <= visibleCount + 2*overscan` and `VirtualItemsTotal = RowCount`, deterministically and golden-asserted; a frame that evaluates no virtualized control reports both `0`; multiple virtualized controls in one frame aggregate the counts; the materialized count does not scale with total across 100/1000/10000 (FR-013/FR-014, SC-006)
- [X] T019 [US4] [skillist: fs-skia-controls-host, fs-skia-reconciliation] Populate `WorkReductionRecord.VirtualMaterialized`/`VirtualTotal` in the retained `step` (`RetainedRender.fs:426`) by a read-only walk of the lowered tree — `VirtualMaterialized` = count of `data-grid-row` kind nodes, `VirtualTotal` = sum of the `Total` on each `data-grid` node's `VisibleRange` (byte-identical render, counting only) — then thread them into `FrameMetrics.VirtualItemsMaterialized`/`VirtualItemsTotal` in `src/Controls.Elmish/ControlsElmish.fs`: the `zero` record carries both `0` and **every** per-frame construction site (`:1376`, `:1425`, `:1449`, `:1472`) lifts them from `lastWorkReduction` (`:858`), exactly as `MemoHitCount`/`MemoMissCount`; surface through `Perf.runScript` and the live `OnFrameMetrics` sink. Make T018 pass (FR-013/FR-014)
- [X] T020 [US4] [skillist: fs-skia-evidence-mode] Add the **10000-row DataGrid** scenario to the `Perf.runScript` corpus (alongside the existing 100/1000-row variants) and regenerate the corpus goldens (`PERF_CORPUS_REGEN=1`) so they carry the two new metric fields and the 10000-row golden asserts `VirtualItemsMaterialized <= visible + overscan` while `VirtualItemsTotal = 10000` (materialization does not scale with total); confirm the rendered scenes are otherwise unchanged (additive only) (FR-015, SC-001)

**Checkpoint**: User Story 4 is functional and independently testable.

---

## Phase 7: Integration & Polish

- [X] T021 [skillist: fs-skia-ui-widgets] Run `./fake.sh build -t RefreshSurfaceBaselines` to regenerate the top-level public surface baseline (the new `FrameMetrics.VirtualItemsMaterialized`/`VirtualItemsTotal` fields) and the per-package Controls/Controls.Elmish baselines (the `Collections`/`DataGrid` `Overscan`, the `Types` `CollectionPosition`/`AccessibilityMetadata.Collection`, the internal `WorkReductionRecord` counts); update any construction sites or sample preludes it flags
- [X] T022 [skillist: fs-skia-ui-widgets] Confirm the new `FrameMetrics` fields, the `Collections`/`DataGrid` overscan surface, the `Types` a11y total/position, and the internal `WorkReductionRecord` counts satisfy the doc-preservation / XML-doc gate, and that no unrelated public function signature changed (only `visibleRange` gains its additive trailing `overscan` parameter)
- [X] T023 [skillist: fs-skia-template-update, fs-skia-controls-host] Run the escalated controls-public-surface gates sequentially as `Route` prints them — `Dev`, `GeneratedGuidanceCheck`, `TemplateCheck`, `GeneratedProductCheck`, the package/per-package surface diffs, `FsiTranscripts`, the controls catalog/doc/interaction/rendering checks, and `TemplateDrift` — confirming the standing Scene-parity golden suite (at-rest byte-identity, FR-006/SC-007) under `Dev` passes, and record the focused governance risk level + non-authoritative aggregate notes in `readiness/`
- [X] T024 [skillist: speckit-evidence-graph] Run `./fake.sh build -t EvidenceGraph` — confirm no cycles, no dangling refs, no `[S*]` surprises, and the echoed `feature-directory`/`tasks=<n>` match this feature
- [X] T025 [skillist: speckit-evidence-audit] Run `./fake.sh build -t EvidenceAudit` — confirm verdict PASS with no remaining `[S]`/`[S*]` and no diff-scan hits, or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is the source
for the PR description's synthetic-evidence section. For `[SEH]` rows, include the
approval label, design-phase source, synthetic input class, expected error behavior, and
reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
