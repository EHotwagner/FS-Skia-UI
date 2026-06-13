# Phase 0 Research: Viewport Virtualization (Phase 6)

This resolves the open design questions the spec deferred to planning. No NEEDS
CLARIFICATION remains after this phase.

## (a) Overscan model — symmetric, FirstIndex-shifting, edge-clamped

**Decision.** Overscan `N` widens the realized window **symmetrically**: up to `N` rows
*before* the visible first index and up to `N` rows *after* the visible last index, each
side independently clamped at the logical edges. Concretely, given today's slice
`{ FirstIndex = f; Count = c; Total = t }`:

- `first' = max 0 (f - N)`
- `last  = f + c - 1` (last visible logical index)
- `last' = min (t - 1) (last + N)`
- `count' = last' - first' + 1` (0 when `t = 0`)

So the realized window is `[first', last']`, and `VirtualItemsMaterialized = count'`.

**Rationale.** The report's acceptance criterion is `materialized <= visible + overscan`,
and a fast scroll reveals un-materialized rows on **both** edges, so a symmetric buffer is
the cross-framework norm (Flutter `cacheExtent`, Avalonia/WPF buffer rows, Compose
`beyondBoundsItemCount`). The spec's bound is stated loosely as `visible + overscan`; with
symmetric overscan the tight bound is `visible + 2*overscan` away from the edges and
exactly `visible + overscan` at an edge. The corpus assertion uses `visible + 2*overscan`
as the upper bound (documented in the contract) so it is honest about both edges; FR-003's
"does not scale with total" holds either way.

**Alternatives considered.** *One-sided (trailing-only) overscan* — simpler bound
(`visible + overscan`) but asymmetric jank on upward scroll; rejected as a weaker match to
the report and to peer frameworks. *Pixel-based `cacheExtent`* — defers to Phase 8
measurement work (variable heights); rejected for this fixed-`RowHeight` rung.

**Default 0 ⇒ byte-identical.** With `N = 0`: `first' = f`, `last' = last`,
`count' = c` — exactly today's slice. No row set, geometry, or scene changes (FR-002 /
FR-006 / FR-016).

## (b) Signature decision — additive parameter + defaulted field

**Decision.** `Collections.visibleRange` gains a **trailing `overscan` parameter** with a
companion default-0 helper, and `CollectionModel` / `DataGridModel` gain an **`Overscan:
int` field** defaulted to `0` at every construction site. The existing call
`Collections.visibleRange rowHeight viewportHeight scrollOffset totalItems` becomes
`... totalItems overscan`; internal callers pass `model.Overscan`. Whether this reads as
additive or breaking on the `.fsi` surface is determined by `RefreshSurfaceBaselines` —
either way the per-package Controls baseline regenerates and the change is documented.

**Rationale.** A defaulted field threaded from the model keeps overscan a single source of
truth per grid and keeps the pure `visibleRange` function honest (overscan is an input,
not hidden state). Adding the field forces a defaulted construction at every site
(`Collections.init`, `Collections.withRange`, `DataGrid` model construction, and any
sample/FSI prelude) — caught by `RefreshSurfaceBaselines` Build and the surface gates, the
same discipline feature 100 used when it added record fields. **Implementation note:** add
`Overscan = 0` at *every* `CollectionModel`/`DataGridModel` literal including samples
(`ControlsGallery`/`DemoReel`) and FSI preludes (`scripts/*-prelude.fsx`), or the build
fails on missing-field.

**Alternatives considered.** *Overscan as a free attr only (no model field)* — keeps the
record stable but scatters the value and makes the realized-window computation read it
from the attr bag; rejected as less honest than a typed field. *Breaking `visibleRange`
signature with overscan in the middle* — gratuitous churn; the trailing-parameter form is
the minimal change.

## (c) Count carrier — counted in the retained `step`, threaded via WorkReductionRecord

**Decision.** `VirtualItemsMaterialized` and `VirtualItemsTotal` are computed in the
retained `step` (`RetainedRender.fs:426`) by walking the lowered tree: count nodes of kind
`data-grid-row` (the materialized unit, `DataGrid.fs:207`) for *materialized*, and read the
logical `Total` from the owning `data-grid` node's `VisibleRange` attr (`Total` field) for
*total*. The two counts ride on `WorkReductionRecord` (`RetainedRender.fsi:128`) as new
internal fields `VirtualMaterialized` / `VirtualTotal`, exactly as feature 113 added
`MemoHits` / `MemoMisses`, then `ControlsElmish.fs` lifts them onto `FrameMetrics`
(`VirtualItemsMaterialized` / `VirtualItemsTotal`) at every per-frame construction site
(`:1376`, `:1425`, `:1449`, `:1472`) and into `zero` (`:1332`, both `0`).

**Rationale.** This is the established 113 pattern and keeps the metric on the
deterministic `Perf.runScript` path so it is golden-assertable (FR-013). Counting walks
the *already-lowered* tree and never alters it — render output is byte-identical;
observability is pure read. When multiple virtualized controls build in one frame, the
walk sums their row counts and totals (FR-014 aggregate). A frame with no `data-grid`
node yields `0`/`0` (FR-013/SC-006). Per-control attribution is available in tests (count
per `data-grid` subtree) without bloating the aggregate metric (FR-014).

**Alternatives considered.** *DataGrid emits the counts as attrs the step reads back* —
couples the Controls layer to the metric and risks the attr being stale vs the actually-
materialized children; rejected. *Count inside `DataGrid.create`* — `create` is pure and
has no frame-metric channel; the retained step is the correct seam (it already owns
`WorkReductionRecord`).

## (d) Offscreen targeting — relocate the window, never expand it

**Decision.** Focus/selection on an offscreen logical row is expressed through the existing
`DataGridMsg` set over the **logical key/index**: `SelectRow key` / `ToggleRow key` set
selection state on the logical item (selection is already a `Set<string>` of keys, not a
property of a materialized control — `DataGrid.fs:43`); `FocusCell` sets the focused
logical cell; and the update **relocates** the realized window to the target by computing
the scroll offset that brings the target index into view (`ScrollRowsTo targetIndex`,
reusing `DataGrid.range` / `Collections.withRange`). The window's `FirstIndex` jumps to
(near) the target; it does **not** grow to span the path.

**Rationale.** Because selection/focus live on logical keys/indices, targeting an
offscreen row needs no materialized control — the model records it directly. Relocating the
window (vs expanding it) is what keeps FR-003's bound intact: after relocation the realized
window is still `visible + overscan` rows, just centred elsewhere (the FR-009/FR-011 vs
FR-003 conflict resolution in the spec). Boundary-crossing navigation (FR-011: move focus
past the last realized row) advances the focused index by one and relocates the window so
the new index is realized — landing on the correct next *logical* row, not the next
*materialized* one.

**Alternatives considered.** *Materialize the whole range up to the target* — defeats
virtualization (unbounded materialization); rejected outright by FR-003. *A separate
offscreen-focus message* — unnecessary; the existing `FocusCell`/`SelectRow`/`ScrollRowsTo`
already address rows by logical identity, so the capability is "let these target an
offscreen index and relocate," not "add new messages."

## (e) Accessibility total + position — from the logical model

**Decision.** `AccessibilityMetadata` (`Types.fsi:212`) gains an **additive optional
field** carrying the total logical item count and the current focused position, e.g.
`Collection: CollectionPosition option` where `CollectionPosition = { TotalItems: int;
FocusedIndex: int option }`. For a virtualized DataGrid it is populated from `RowCount`
(total) and the focused row's logical index (derived from `FocusedCell.RowKey` against the
logical row order), `None` for non-collection controls.

**Rationale.** The report requires a11y to "describe total counts and current position"
even though only the window is materialized (FR-012). Computing it from the logical model
(`RowCount` + focused index) — not from the realized slice — is the only way it reports the
true size/position. An optional field keeps it additive (precedent: feature 100's
`Navigation: NavRange option`, `Types.fsi:219`) so non-collection controls are unchanged
and at-rest a11y for existing controls is byte-identical.

**Alternatives considered.** *Reuse `Navigation: NavRange`* — `NavRange` is `{Step;Min;Max}`
for slider/value roles, semantically wrong for item counts; rejected. *Two flat fields on
`AccessibilityMetadata`* — pollutes every control's record with collection-only data;
`option`-wrapped sub-record keeps it scoped.

## (f) Feature 113 interaction — memo still works over the widened row set

**Decision / confirmation.** Feature 113's DataGrid `gridGeom` memoization (the retained
memoize seam at `RetainedRender.fs:102`/call site `:480`, keyed by `ControlId` +
deterministic dependency) keys on the **rows actually present in the lowered subtree** —
i.e. the realized window. Overscan widens that window, so the memo dependency reflects the
overscan-widened cell set; a hit reuses the projection for the same realized rows, a miss
recomputes when the realized set changes (e.g. on scroll/relocation). No change to the 113
seam is required (FR-017); the only interaction is that the dependency now covers `2*N`
more rows when overscan is opted into, which is correct.

**Rationale.** 113 memoizes the projection of the materialized rows, and virtualization
only changes *which* rows are materialized; the memo key is downstream of materialization,
so it composes for free. A Phase 1 test asserts a steady-state overscan frame still
records memo hits (113's metric) to prove the composition.

## (g) 10000-row corpus scenario + non-scaling assertion

**Decision.** Extend the existing perf corpus (`specs/109-perf-metrics-baseline/readiness/
perf-corpus/datagrid-{100,1000,10000}.golden.txt`) so each DataGrid golden carries the new
`VirtualItemsMaterialized` / `VirtualItemsTotal` fields. The 10000-row golden asserts
`VirtualItemsMaterialized <= visible + 2*overscan` (bounded) and `VirtualItemsTotal =
10000`; a cross-scenario test compares the 100/1000/10000 goldens and asserts
`VirtualItemsMaterialized` is **identical** across them (non-scaling) while
`VirtualItemsTotal` tracks the actual total (FR-003/FR-015/SC-001). Goldens regenerate
with `PERF_CORPUS_REGEN=1`.

**Rationale.** The 109 corpus already specified 100/1000/10000-row DataGrid scenarios, so
the bounded-materialization proof rides on existing infrastructure — the new fields are the
only golden churn, accepted exactly as 113's memo-count churn was. A frame with no
virtualized control (e.g. a `controls`/`hover-sweep` scenario) asserts `0`/`0` (SC-006).

## Summary of decisions

| # | Question | Decision |
|---|----------|----------|
| a | Overscan shape | Symmetric, `FirstIndex`-shifting, edge-clamped; default 0 = today's slice |
| b | Signature | Trailing `overscan` param on `visibleRange` + defaulted `Overscan` field on the models |
| c | Count carrier | Counted in retained `step` over the lowered tree; threaded via `WorkReductionRecord` (113 pattern) |
| d | Offscreen targeting | Existing `SelectRow`/`FocusCell`/`ScrollRowsTo` over logical key/index; window **relocates**, never expands |
| e | A11y total/position | Additive `Collection: CollectionPosition option` on `AccessibilityMetadata`, from the logical model |
| f | 113 interaction | No change; memo keys downstream of materialization, composes over the widened set |
| g | Corpus | Extend 109's datagrid-{100,1000,10000} goldens; non-scaling cross-scenario assertion; idle 0/0 |
