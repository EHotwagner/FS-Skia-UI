# Implementation Plan: Viewport Virtualization for Repeated Controls

**Branch**: `114-viewport-virtualization` | **Date**: 2026-06-13 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/114-viewport-virtualization/spec.md`

## Summary

Features 109–113 hardened the retained hot path (honest metrics + corpus, retained
pointer routing, view-skip scheduler, targeted runtime visual-state stamp, control-
internal memoization seam + stability diagnostics). The performance report's **Phase 6**
("Do first" #4) is the one remaining item that no faster diff or memo cache can solve: a
large repeated control — a DataGrid with thousands of logical rows — must stop turning
every logical item into a materialized control. The framework already slices a DataGrid
to its visible range (`Collections.visibleRange`, `DataGrid.visibleRows`), but that
slicing is **not yet a framework-level contract**: it is unobservable (no metric proves a
10000-row grid materialized only its window), there is no overscan buffer, and offscreen
logical items are not addressable by focus/selection/a11y.

**Technical approach (Phase 6 of the performance report):**

1. **Overscan on the realized-window computation** (`FS.Skia.UI.Controls`,
   `Collections.visibleRange` / `DataGrid` slicing). Add an **overscan** count (extra
   logical rows realized beyond the visible window, clamped at the logical edges),
   **defaulting to 0** so the default realized window is exactly today's visible slice and
   at-rest rendered output stays **byte-identical** (FR-002/FR-006/FR-016). Non-zero
   overscan is an explicit opt-in: the window widens by up to `N` correct, real, edge-
   clamped adjacent rows (FR-007). Materialization is the existing
   `visibleRows rows visibleRange |> List.map (rowControl columns)` in `DataGrid.create`
   (`DataGrid.fs:214`/`:223`); overscan widens the `VisibleRange` it consumes, not the
   materialization loop.

2. **Public `VirtualItemsMaterialized` / `VirtualItemsTotal` `FrameMetrics` fields**
   (breaking `ControlsElmish.fsi` change, two new fields — precedent 109/110/111/113).
   `VirtualItemsMaterialized` counts the row items actually materialized this frame;
   `VirtualItemsTotal` counts the logical items the virtualized control(s) represent.
   Threaded from the retained step / `WorkReductionRecord` (the 113 `MemoHits`/`MemoMisses`
   pattern), surfaced on the deterministic `Perf.runScript` path (golden-asserted) and
   through the live `OnFrameMetrics` sink. Both `0` on a frame that evaluates no
   virtualized control; aggregate across multiple virtualized controls in a frame
   (FR-013/FR-014). Bounded-materialization (`materialized <= visible + overscan`,
   non-scaling with total) becomes a regression-proof golden contract.

3. **Offscreen addressability** (`FS.Skia.UI.Controls`, `DataGrid` model + a11y). Keyboard
   **focus** and **selection** can target a logical row **outside** the realized window:
   the row's focus/selection state is a property of the logical item (row key / index), so
   targeting an offscreen row records it and **relocates** the realized window to it
   (`ScrollRowsTo`) without materializing every intervening row (FR-009/FR-010/FR-011). The
   bound in FR-003 holds at all times — the window **relocates**, it does not expand to
   cover the path. **Accessibility metadata** reports the **total** logical item count and
   the **current focused position** (index within the total), computed from the logical
   model (`RowCount` + focused index), not from the materialized slice (FR-012).

4. **10000-row DataGrid evidence scenario** in the `Perf.runScript` corpus (alongside the
   existing 100/1000-row variants from feature 109's Phase 0) asserting
   `VirtualItemsMaterialized <= visible + overscan` while `VirtualItemsTotal = 10000`,
   proving materialization does not scale with total row count (FR-015).

This is **additive virtualization-contract + observability + offscreen-correctness only**
(FR-016): with default overscan, at-rest rendered output, control geometry,
focus/keyboard routing for materialized rows, and every dispatch outcome stay
**byte-identical**. The only intended observable deltas are (a) opt-in overscan
materializing up to `N` extra real rows, (b) the two new `FrameMetrics` fields, and (c)
offscreen focus/selection addressability + a11y totals. Feature 113's DataGrid
memoization continues to work over the (now overscan-widened) virtualized row set
(FR-017). This rung keeps **uniform fixed `RowHeight`**; variable/measured heights and all
row/column/text **measurement caches** are **deferred to Phase 8** (FR-018).

## Technical Context

**Language/Version**: F# / .NET `net10.0`
**Primary Dependencies**: No new dependencies. Edits `FS.Skia.UI.Controls`
(`Collections.visibleRange` overscan parameter; `DataGrid` realized-window + offscreen
focus/selection targeting; `AccessibilityMetadata` total/position; the `RetainedRender`
step threading the materialized/total counts) and `FS.Skia.UI.Controls.Elmish` (the
`FrameMetrics` record + threading through the retained step / `Perf.runScript` /
`OnFrameMetrics`). Consumes existing `VisibleRange`, `CollectionModel`, `DataGridModel`,
`RetainedRender.step`, `WorkReductionRecord`, `AccessibilityMetadata`.
**Testing**: Expecto + FsCheck. Overscan / bounded-materialization / offscreen-
addressability / a11y-total tests in `tests/Controls.Tests` (reaching internal seams via
`InternalsVisibleTo "Controls.Tests"`); `VirtualItemsMaterialized`/`VirtualItemsTotal`
corpus goldens (including the 10000-row scenario) in `tests/Elmish.Tests` over
`ControlsElmish.Perf.runScript`; the standing Scene-parity golden suite under `Dev` for
at-rest byte-identity; FAKE targets.
**Target Platform**: Windows and Linux (no platform-specific code; no Vulkan/Skia/visual-
output change).

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

**Change classification — Tier 1 (contracted change).** Two new **public** `FrameMetrics`
fields (`ControlsElmish.fsi`), a possible additive `overscan` parameter / field and a11y
total+position field on the `Controls` `Collections` / `DataGrid` / `Types` `.fsi`
surfaces, plus internal threading of the counts through the retained step. The top-level
surface baseline and per-package baselines move; the full artifact chain applies (`.fsi`
updates, baseline regeneration, test evidence, XML-doc). `Route` escalates to the
**controls-public-surface** tier.

**Principle compliance.**
- *I (Spec→FSI→Tests→Impl)*: the overscan parameter, the `FrameMetrics` fields, the
  offscreen-targeting surface, and the a11y total/position are drafted in `.fsi` signature
  form first and exercised from FSI/tests; the bounded-materialization corpus assertion,
  the default-overscan byte-identity proof, and the offscreen-focus/selection tests are
  the failing-first proofs.
- *II (Visibility in `.fsi`)*: the metric fields, the overscan parameter, and the a11y
  total/position are public, declared in their `.fsi`; the count-threading seam through
  `WorkReductionRecord`/`RetainedRender.step` is internal (declared in the owning
  `Controls` `.fsi`, reached via `InternalsVisibleTo`). No access modifiers in `.fs`.
- *III (Idiomatic simplicity)*: overscan is a single clamped integer widening the existing
  `VisibleRange` computation; the count is a plain sum of materialized `data-grid-row`
  nodes plus the logical `Total`; offscreen targeting reuses the existing `ScrollRowsTo` /
  `SelectRow` / `FocusCell` messages over row keys/indices. No SRTP/reflection/type-
  providers. Any `mutable` count accumulator is disclosed at the use site.
- *IV (Elmish/MVU boundary)*: unchanged — `Update`, effects, subscriptions, commands,
  interpreter are untouched. `CollectionModel`/`DataGridModel` gain an overscan field and
  offscreen focus/selection targeting, but dispatch *outcomes* for materialized rows are
  byte-identical (FR-016); offscreen focus/selection is a new *capability* (previously
  unreachable), not a changed outcome.
- *V (Synthetic disclosure)*: none expected — bounded-materialization is proven over real
  100/1000/10000-row grids on the real `Perf.runScript` path; byte-identity uses the real
  Scene-parity suite; offscreen targeting / a11y totals run over real `DataGridModel`s.
  Any unavoidable stub returns to task review for `[S]` disclosure.
- *VI (Test evidence)*: bounded materialization (non-scaling across 100/1000/10000),
  default-overscan byte-identity, opt-in overscan correctness (edge-clamped, real
  adjacent rows only), offscreen focus/selection targeting, boundary-crossing navigation,
  a11y total+position, and idle = 0/0 metrics all fail before / pass after; no assertion
  weakening.
- *VII (Observability)*: `VirtualItemsMaterialized`/`VirtualItemsTotal` make a regression
  that re-materializes every row, or an overscan exceeding its bound, visible as a golden
  change instead of silent CPU/memory cost.

### Repository Governance Decisions

- **Template ownership**: N/A — no `template/**`, sample, or command-surface change; the
  overscan parameter, the metric fields, and offscreen addressability do not alter
  `.template.config/template.json`. (The merge-time template package-pin bump is the
  standard post-merge step, not a content change in this feature.)
- **Dependency impact**: N/A — no new package; `Directory.Packages.props`,
  `docs/dependencies.md`, and `DependencyReport` are unchanged.
- **Command-surface impact**: No new gate. Escalated **controls-public-surface** set
  because the `Controls` and `Controls.Elmish` `.fsi` surfaces change; run `Route` first
  and obey its printed minimal list. `RefreshSurfaceBaselines` regenerates the top-level +
  per-package baselines after the `FrameMetrics` / `Collections` / `DataGrid` / `Types`
  additions; the `Perf.runScript` corpus goldens are regenerated (`PERF_CORPUS_REGEN=1`)
  to carry the two new metric fields and the 10000-row scenario. FAKE-backed commands run
  sequentially in the deterministic order:
  1. `./fake.sh build -t Dev`
  2. `./fake.sh build -t GeneratedGuidanceCheck`
  3. `./fake.sh build -t TemplateCheck`
  4. `./fake.sh build -t GeneratedProductCheck`
  5. `./fake.sh build -t EvidenceGraph`
  6. `./fake.sh build -t EvidenceAudit`
- **Generated project impact**: N/A to behaviour — generated default/minimal contents and
  generated `Dev` behaviour are unchanged. Generated projects gain the two new public
  `FrameMetrics` fields and the additive overscan/a11y surface transitively (additive;
  `OnFrameMetrics` default stays `ignore`, overscan defaults to 0, byte-identical at rest).
- **Evidence paths**: bounded-materialization + non-scaling + opt-in-overscan tests under
  `tests/Controls.Tests/Feature114*.fs`; offscreen focus/selection + boundary-crossing +
  a11y-total tests under `tests/Controls.Tests/Feature114*.fs`; the
  `VirtualItemsMaterialized`/`VirtualItemsTotal` corpus goldens (incl. the 10000-row
  scenario) under `specs/109-perf-metrics-baseline/readiness/perf-corpus/*.golden.txt`
  (regenerated) + asserted in `tests/Elmish.Tests/Feature114*.fs`; at-rest byte-identity
  via the standing Scene-parity suite under `Dev`; skill-loading evidence in
  `readiness/skill-loading-evidence.md`; the window-visibility not-applicable set;
  `readiness/evidence-audit.md` (verdict token); generated-validation package-resolution
  tokens; surface/per-package baselines under `readiness/surface-baselines/` +
  `readiness/per-package-surface/`.
- **`.fsi` / contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  two new public fields `VirtualItemsMaterialized` / `VirtualItemsTotal` (with XML-doc;
  doc-preservation gate). Additive `Controls` surface: an `overscan` parameter on the
  realized-window computation (`Collections.visibleRange` and/or a `DataGrid` slicing
  function) and a count field on `CollectionModel`/`DataGridModel`; an a11y total+position
  field on `AccessibilityMetadata` (`Types.fsi`); any offscreen-targeting `val`s. The
  count-threading seam through `WorkReductionRecord` is internal. The top-level surface
  baseline changes (the `FrameMetrics` fields); per-package Controls + Controls.Elmish
  baselines regenerate. **Phase 0 decides** whether the `visibleRange` signature change is
  additive (an extra parameter) or a breaking signature change, and whether overscan rides
  on `CollectionModel` as a new field (defaulted 0 at every construction site).
- **MVU/effect boundary**: Unchanged (preserved, not modified). `Model`/`Msg`/`Effect`/
  `init`/`update`/interpreter are untouched; offscreen focus/selection targeting reuses
  the existing `DataGridMsg` set (`ScrollRowsTo`/`SelectRow`/`ToggleRow`/`FocusCell`).
  Dispatch outcomes for materialized rows are byte-identical; offscreen targeting is a new
  reachable capability, not a changed outcome.
- **Synthetic evidence**: None planned. Bounded materialization = real counts over real
  100/1000/10000-row grids on the real `Perf.runScript` path; byte-identity = the real
  Scene-parity suite; offscreen targeting / a11y totals = real `DataGridModel`s. Any
  unavoidable stub returns to task review for `[S]` disclosure.
- **Test evidence**: failing-first — `VirtualItemsMaterialized <= visible + overscan` and
  non-scaling across 100/1000/10000; `VirtualItemsMaterialized = VirtualItemsTotal` when
  the grid fits the window; default-overscan scene byte-identity over the corpus; opt-in
  overscan materializes only real edge-clamped adjacent rows; offscreen focus/selection
  records the logical row + relocates the window without materializing the path; boundary-
  crossing navigation lands on the correct next logical row; a11y reports total + focused
  index; idle frame reports 0/0.
- **Observability**: `VirtualItemsMaterialized`/`VirtualItemsTotal` (public, deterministic,
  golden-asserted via `Perf.runScript`, plus live `OnFrameMetrics`). No unsupported-
  environment message change.
- **Deferred scope**: Phase 6 only. OUT: **variable / measured row heights** and
  **row/column/text measurement caches** (Phase 8); **paint caches / damage rectangles /
  Skia picture boundaries** (Phase 7); **layout hot-path / text-measurement caches &
  layout-boundary hints** (Phase 8); **`SkiaViewer` backend / render-thread / compositor
  review** (Phase 9); **generalizing virtualization to non-DataGrid list/collection
  surfaces** (DataGrid is the representative this rung; the shared `Collections` model MAY
  be extended to carry overscan for future reuse); **horizontal / column virtualization**
  (rows only). No renderer rewrite, no Avalonia/WPF redesign, no platform/release/
  distribution scope. Features 110/111/112/113 are unchanged.

**Gate result: PASS.** No unjustified violations. Tier 1 obligations (`.fsi`, baselines,
tests, docs) are enumerated above and carried into Phase 1.

## Project Structure

Edited / added paths for this feature:

```
src/Controls/
  Collections.fsi             # visibleRange gains an overscan param (additive, default 0); CollectionModel
                              #   gains an Overscan field (defaulted 0 at construction) (+ XML-doc)
  Collections.fs              # overscan-widened realized window: shift FirstIndex back by overscan (clamp >= 0),
                              #   extend Count by up to 2*overscan, clamp to Total; default 0 == today's slice
  DataGrid.fs                 # realized window consumes overscan-widened VisibleRange; offscreen SelectRow/
                              #   ToggleRow/FocusCell/ScrollRowsTo targeting relocates the window to the
                              #   logical key/index without materializing the path
  DataGrid.fsi (if present)   # overscan + offscreen-targeting surface (additive where possible)
  Types.fsi / Types.fs        # AccessibilityMetadata gains total item-count + current focused position
                              #   (additive field, e.g. a CollectionPosition option) (+ XML-doc)
  RetainedRender.fsi          # WorkReductionRecord gains VirtualMaterialized / VirtualTotal counts (internal)
  RetainedRender.fs           # step counts materialized data-grid-row nodes + reads logical Total; populates
                              #   the two counts on the step result (byte-identical render; counting only)

src/Controls.Elmish/
  ControlsElmish.fsi          # FrameMetrics gains public VirtualItemsMaterialized / VirtualItemsTotal (+ XML-doc)
  ControlsElmish.fs           # thread the counts from the retained step into FrameMetrics (zero record +
                              #   every per-frame construction site); Perf.runScript + OnFrameMetrics surface

readiness/surface-baselines/  +  readiness/per-package-surface/
  FS.Skia.UI.Controls*.txt    # regenerated (RefreshSurfaceBaselines): top-level (FrameMetrics fields) +
                              #   per-package (Collections/DataGrid overscan, AccessibilityMetadata total/pos)

specs/109-perf-metrics-baseline/readiness/perf-corpus/
  datagrid-*.golden.txt       # regenerated (PERF_CORPUS_REGEN=1) to carry VirtualItemsMaterialized /
                              #   VirtualItemsTotal; the 10000-row scenario asserts the bound

tests/Controls.Tests/
  Feature114OverscanTests.fs        # FR-001/002/003/004/007 bounded + non-scaling + edge-clamp + transparent
  Feature114OffscreenTests.fs       # FR-009/010/011 offscreen focus/selection targeting + boundary crossing
  Feature114AccessibilityTests.fs   # FR-012 a11y total + current focused position from the logical model

tests/Elmish.Tests/
  Feature114VirtualMetricsTests.fs  # FR-013/014/015 VirtualItemsMaterialized/Total goldens (bounded, non-
                                     #   scaling 100/1000/10000, idle 0/0, aggregate) over Perf.runScript

specs/114-viewport-virtualization/
  spec.md  plan.md  research.md  data-model.md  quickstart.md
  contracts/virtualization-contract.md  contracts/offscreen-addressability.md
  readiness/   # evidence-audit.md, skill-loading-evidence.md, byte-identity authority, window-visibility set
```

**Key seams (file:line anchors):**
- Realized-window computation to extend with overscan: `Collections.visibleRange`
  `Collections.fs:28` (sig `Collections.fsi:35`); `CollectionModel` `Collections.fsi:11`;
  `VisibleRange` `Collections.fsi:4`.
- DataGrid materialization site (where rows become controls): `DataGrid.visibleRows`
  `DataGrid.fs:214`; `DataGrid.create` slice `DataGrid.fs:223`/`:230`; `DataGrid.range`
  `DataGrid.fs:67`. The materialized node is `data-grid-row` (`rowControl` `DataGrid.fs:207`).
- DataGrid model + offscreen-targeting messages: `DataGridModel` `DataGrid.fs:36`;
  `ScrollRowsTo`/`SelectRow`/`ToggleRow`/`FocusCell` `DataGrid.fs:50-53`; `ReplaceRowCount`
  realized-window recompute `DataGrid.fs:156`.
- Accessibility total/position to add: `AccessibilityMetadata` `Types.fsi:212` (precedent:
  the additive `Navigation: NavRange option` field added by feature 100 at `:219`).
- Count threading precedent (how `MemoHits`/`MemoMisses` reach `FrameMetrics`):
  `WorkReductionRecord` `RetainedRender.fsi:128`; `lastWorkReduction` `ControlsElmish.fs:858`;
  retained `step` `RetainedRender.fsi:232` / `RetainedRender.fs:426`; 113 memoize seam
  `RetainedRender.fs:102`/call site `:480`.
- `FrameMetrics` type + every construction site: `ControlsElmish.fsi:68`; `.fs` type `:46`;
  `zero` record `ControlsElmish.fs:1332`; per-frame records `:1376`, `:1425`, `:1449`,
  `:1472`.
- Deterministic corpus driver: `ControlsElmish.Perf.runScript` `ControlsElmish.fs:1240`;
  existing DataGrid corpus goldens `specs/109-perf-metrics-baseline/readiness/perf-corpus/
  datagrid-{100,1000,10000}.golden.txt`.

## Phase 0: Research

See [research.md](./research.md). Resolves: (a) the **overscan model** — whether overscan
is symmetric (rows before *and* after the visible window) or one-sided, how it shifts
`FirstIndex` and widens `Count`, the edge-clamp at top/bottom (no index `< 0`, none
`>= Total`), and why default 0 reproduces today's slice byte-identically; (b) the
**signature decision** — additive `overscan` parameter on `visibleRange` vs a breaking
signature change, and whether overscan rides on `CollectionModel`/`DataGridModel` as a
defaulted field; (c) the **count carrier** — how `VirtualItemsMaterialized` (count of
materialized `data-grid-row` nodes) and `VirtualItemsTotal` (logical row total) reach
`FrameMetrics`: counted in the retained `step` over the lowered tree + read from the
`data-grid` node's `VisibleRange`/`RowCount`, threaded through `WorkReductionRecord`
exactly as 113's `MemoHits`/`MemoMisses`, and why this is byte-identical (counting only,
no render change); (d) **offscreen targeting** — how a focus/selection on an offscreen
logical key/index relocates the realized window (`ScrollRowsTo`) without materializing the
intervening rows, why the FR-003 bound holds (the window *relocates*, it does not expand),
and boundary-crossing navigation; (e) **accessibility total/position** — the shape of the
additive `AccessibilityMetadata` field (total item count + current focused index from the
logical model), and why it must be computed from `RowCount`/focused index not the
materialized slice; (f) **113 interaction** — confirming the DataGrid `gridGeom`
memoization still works over the overscan-widened row set; (g) the **10000-row corpus
scenario** layout and the non-scaling assertion across 100/1000/10000.

## Phase 1: Design & Contracts

- [data-model.md](./data-model.md): the overscan-widened `VisibleRange` semantics, the
  `Overscan` field placement on `CollectionModel`/`DataGridModel` (defaulted 0), the
  `AccessibilityMetadata` total+position addition, the `WorkReductionRecord`
  `VirtualMaterialized`/`VirtualTotal` counts and how they aggregate into the
  `FrameMetrics` `VirtualItemsMaterialized`/`VirtualItemsTotal` fields, the offscreen-
  targeting state transitions (focus/selection on an offscreen key → window relocation),
  and the idle = 0/0 rule.
- [contracts/virtualization-contract.md](./contracts/virtualization-contract.md): the
  overscan + bounded-materialization contract — `materialized <= visibleCount + overscan`
  at all totals; non-scaling with total; `materialized = total` when the grid fits the
  window; default-0 byte-identity (FR-006); opt-in overscan materializes only real, edge-
  clamped adjacent rows (FR-007); keyed row reuse on scroll preserved (FR-008); the count
  semantics (idle = 0/0, aggregate across grids).
- [contracts/offscreen-addressability.md](./contracts/offscreen-addressability.md): the
  offscreen focus/selection contract — focusing/selecting an offscreen logical row records
  it and relocates the realized window to it without materializing the path (FR-009/010);
  boundary-crossing navigation lands on the correct next logical row (FR-011); a11y
  metadata reports total logical count + current focused position from the logical model
  (FR-012); the FR-003 bound holds throughout (relocate, do not expand).
- [quickstart.md](./quickstart.md): how to run the overscan / offscreen / a11y / metrics
  tests, regenerate the corpus goldens (`PERF_CORPUS_REGEN=1`, incl. the 10000-row
  scenario) and surface baselines (`RefreshSurfaceBaselines`), and run the escalated gate
  set.
- Agent context update: `AGENTS.md` SPECKIT marker repointed to this plan.

## Phase 2: Planning complete

Stop after design. `tasks.md` is produced by `/speckit.tasks`.
