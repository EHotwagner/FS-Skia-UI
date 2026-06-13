# Feature Specification: Viewport Virtualization for Repeated Controls (Observable Virtualization Contract + Overscan + Offscreen Addressability)

**Feature Branch**: `114-viewport-virtualization`
**Created**: 2026-06-13
**Status**: Draft
**Input**: User description: "do next part."

**Source report** (local in-repo report, not a remote URL — no `source-spec.md`
snapshot per the specify FR-016 no-op rule):
`docs/reports/2026-06-12-1422-controls-performance-framework-research.md`. This
feature implements the **next part** of that report's staged plan after feature 113
(which delivered Phase 5: view memoization + stability diagnostics) — namely **Phase 6:
Viewport Virtualization for Repeated Controls**, also the report's "Do first" priority
**#4** ("Add DataGrid/list virtualization"). Everything from Phase 7 onward (paint
caches / damage rectangles, layout/text-measurement caches, backend review) remains
**out of scope** — see *Unsupported scope*.

## Why this feature (context)

Features 109–113 hardened the retained hot path: honest frame metrics + a perf corpus
(109), retained pointer routing (110), a frame scheduler that skips `host.View` on a
model-unchanged frame (111), a targeted runtime visual-state stamp (112), and a
control-internal memoization seam + stability diagnostics (113). What remains high on
the report's priority list is the one item that does **not** yield to a faster diff or a
memo cache: a large repeated control — a DataGrid or list with thousands of logical rows
— must stop turning every logical item into a materialized control. The report is blunt:
"Optimizing a 10000-row fully materialized tree is the wrong first fight." The cross-
framework answer (Flutter lazy builders, Avalonia/WPF UI virtualization, Compose lazy
layouts) is the same: **materialize the visible window plus a small overscan, not every
logical item**, while keeping focus, selection, keyboard navigation, and accessibility
correct for the items that are *not* materialized.

The framework is **partway there already**. The internal `Collections` model
(`VisibleRange`, `Collections.visibleRange`, `CollectionModel` with `ScrollOffset` /
`ViewportHeight` / `RowHeight`) and `DataGrid` (`DataGrid.visibleRows`, `ScrollRowsTo`,
`DataGridVisibleRangeChanged`) already slice a DataGrid to the rows in its visible range
rather than materializing every row. But that slicing is **not yet a framework-level
*contract***:

1. **It is unobservable.** There is no metric proving that a 10000-row grid materialized
   only its visible window. A regression that re-materialized every row — or a consumer
   that forgot to supply a visible range — would slip through silently because nothing
   counts materialized-vs-total items. The report lists `VirtualItemsMaterialized` /
   `VirtualItemsTotal` as exactly the deterministic fields this gap needs.
2. **There is no overscan.** Today the slice realizes exactly `VisibleRange.Count` rows
   with no buffer, so a fast scroll reveals un-materialized rows at the edge. The report's
   virtualization model explicitly includes an **overscan count** with a bounded-
   materialization acceptance criterion. This feature realizes overscan **symmetrically**
   (up to the overscan count of rows on *each* side of the visible window), so the
   framework bound is `VirtualItemsMaterialized <= visible + 2*overscan`.
3. **Offscreen items are not guaranteed addressable.** The report requires that keyboard
   focus and selection can target an **offscreen** logical item without that item's
   visual control existing, and that accessibility metadata still describes **total**
   item count and current position. Today focus/selection/a11y are defined over the
   *materialized* controls, so an offscreen row is effectively invisible to navigation.

This feature turns the existing row-slicing into an **observable, correct, framework-
level virtualization contract**:

- (a) public, deterministic **`VirtualItemsMaterialized` / `VirtualItemsTotal`**
  per-frame metrics (golden-asserted via the `Perf.runScript` corpus, threaded through
  the existing `WorkReductionRecord` → `FrameMetrics` path — the 113 pattern);
- (b) an **overscan** parameter on the virtualization model, **defaulting to 0** so at-
  rest rendered output stays **byte-identical** to today, with non-zero overscan opt-in
  (`materialized <= visible + 2*overscan`);
- (c) **offscreen addressability**: keyboard focus and selection can target a logical row
  outside the realized window (scrolling/targeting it) without materializing every
  intervening row, and accessibility metadata reports the **total** item count and the
  current position — not just the realized slice;
- (d) a **10000-row DataGrid evidence scenario** in the corpus asserting the materialized
  count is bounded by visible + 2*overscan regardless of total row count.

Per the report's staging, this rung deliberately keeps **uniform fixed `RowHeight`**
virtualization (the model already assumes it) and **defers all row/column/text
measurement caching to Phase 8** (its headline), and defers paint/damage caches (Phase 7)
and backend review (Phase 9). DataGrid is the **representative** virtualized surface;
generalizing virtualization to other list/collection surfaces is out of scope this rung.

## Clarifications

### Session 2026-06-13

- Q: DataGrid already slices to exactly the visible rows. How should the report's
  overscan buffer affect at-rest output? → A: **Add overscan, default 0.** Introduce an
  overscan parameter on the virtualization model but default it to **0**, so at-rest
  rendered output stays **byte-identical** to today; non-zero overscan is **opt-in**
  (`materialized <= visible + 2*overscan`). Keeps the additive-safety precedent of 112/113
  while still landing the overscan mechanism the report's acceptance criterion requires.
- Q: Should keyboard focus/selection be able to target an offscreen logical row, and
  should accessibility metadata report total item count + current position? → A: **In
  scope this rung.** Phase 6 guarantees focus/selection can address an offscreen logical
  item without materializing it (targeting/scrolling to it), and accessibility metadata
  reports the **total** item count + current position — matching the report's Phase 6
  acceptance criteria ("keyboard navigation across visible/offscreen boundaries remains
  correct"; "accessibility metadata can still describe total counts and current
  position").
- Q: Where do the report's row/column measurement caches (task 2) belong — Phase 6 or
  Phase 8? → A: **Defer to Phase 8.** Keep Phase 6 on **uniform fixed `RowHeight`**
  virtualization (as the model already is); defer all row/column/text measurement caching
  to Phase 8, where measurement caches are the headline. Avoids overlap and keeps this
  rung tractable.
- Q: Should `VirtualItemsMaterialized` / `VirtualItemsTotal` be public golden-asserted
  `FrameMetrics` fields or internal counts? → A: **Public `FrameMetrics` fields**
  (informed default, matching 109/110/111/113). Virtualization runs on the deterministic
  `Perf.runScript` render path, so the counts are reproducible and golden-assertable,
  making the bounded-materialization guarantee regression-proof. This is a **breaking
  `ControlsElmish.fsi` `FrameMetrics` change** (two new fields) and incurs corpus-golden
  churn, accepted for the same reason 113 made the memo counts public.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A large repeated control materializes only its visible window plus overscan (Priority: P1)

A framework maintainer renders a DataGrid backed by a very large logical row set (e.g.
10000 rows) within a bounded viewport. Today the grid already slices to its visible
range, but nothing proves it. After this feature, the frame records that the number of
materialized row items is bounded by `visible + 2*overscan` — far smaller than the total —
and the total logical count is reported alongside it, so the virtualization is an
observable, regression-proof contract rather than an unverified implementation detail.

**Why this priority**: This is the report's Phase 6 headline and "Do first" #4. It is the
load-bearing guarantee: a large grid must not pay per-frame cost proportional to its total
logical size. It is independently valuable — even with no other change, the bounded-
materialization metric lets a regression that re-materializes every row fail a golden.

**Independent Test**: Render a 10000-row DataGrid scenario through the deterministic
`Perf.runScript` path with a bounded viewport and overscan `N`; assert
`VirtualItemsMaterialized <= visibleCount + 2*N` and `VirtualItemsTotal = 10000`, and that
the materialized count does **not** grow with total row count (compare the 100-, 1000-,
and 10000-row variants: materialized stays bounded while total scales).

**Acceptance Scenarios**:

1. **Given** a DataGrid with 10000 logical rows and a bounded viewport realizing `V`
   visible rows with overscan `N`, **When** a frame is built, **Then**
   `VirtualItemsMaterialized <= V + 2*N` and `VirtualItemsTotal = 10000`.
2. **Given** the same scenario at 100, 1000, and 10000 total rows, **When** each frame is
   built, **Then** `VirtualItemsMaterialized` stays bounded by `V + 2*N` (does not scale
   with total) while `VirtualItemsTotal` reflects the actual total each time.
3. **Given** a grid whose total logical rows already fit within `V + 2*N`, **When** a frame
   is built, **Then** `VirtualItemsMaterialized = VirtualItemsTotal` (everything is
   realized; virtualization is transparent).

---

### User Story 2 - Overscan is opt-in and at-rest output is byte-identical (Priority: P1)

A maintainer must trust that adding the virtualization contract changed **nothing**
observable for existing consumers. Overscan defaults to **0**, so a DataGrid that does
not opt into overscan materializes exactly the rows it does today and renders a byte-
identical scene. A consumer that opts into overscan `N` materializes up to `N` extra
rows on each side of the visible window (up to `2*N` total, which are correct, real rows),
and that is the only change.

**Why this priority**: A virtualization contract that silently changed which rows render
for every existing grid would be a regression, not a feature. Default-0 overscan keeps
the additive-safety property the prior rungs (112/113) held. P1 because the mechanism
(US1) cannot land without proving it is non-disruptive by default.

**Independent Test**: For the existing corpus DataGrid scenarios (no overscan opt-in),
assert the rendered scenes are byte-identical to the pre-feature baseline. For a scenario
that opts into overscan `N`, assert exactly the visible rows plus up to `N` correct
adjacent rows on each side (up to `2*N` total) are materialized and that the visible region
itself is unchanged.

**Acceptance Scenarios**:

1. **Given** a DataGrid with no overscan opt-in (default 0), **When** the corpus runs,
   **Then** every rendered scene is byte-identical to the pre-feature baseline and
   `VirtualItemsMaterialized` equals the prior realized-row count.
2. **Given** a DataGrid opting into overscan `N`, **When** a frame is built, **Then** up
   to `N` correct rows adjacent to the visible window are additionally materialized, the
   visible rows are unchanged, and `VirtualItemsMaterialized <= visible + 2*N`.
3. **Given** overscan `N` and a grid scrolled to a boundary (top/bottom), **When** the
   frame is built, **Then** overscan is clamped at the logical edges (no negative indices,
   no rows beyond `Total`) and only real rows are materialized.

---

### User Story 3 - Focus, selection, and accessibility remain correct across the visible/offscreen boundary (Priority: P1)

A user navigates a virtualized DataGrid with the keyboard and selects rows, including
rows that are **not currently materialized**. Focusing or selecting an offscreen logical
row targets that row (bringing it into the realized window / recording it as focused/
selected) without requiring every intervening row's control to exist. Assistive technology
reading the grid sees the **total** logical row count and the current focused position —
not merely the handful of realized rows.

**Why this priority**: Virtualization is only acceptable if it is invisible to
correctness. "Keyboard navigation across visible/offscreen boundaries remains correct" and
"accessibility metadata can still describe total counts and current position" are explicit
report acceptance criteria. A virtualized grid that loses keyboard reachability or
under-reports its size to assistive tech is broken. P1 because it is a correctness gate,
not a nicety.

**Independent Test**: With a virtualized grid whose focused/selected row is outside the
realized window, assert (a) focusing/selecting an offscreen logical row by key targets it
without materializing the whole range, (b) keyboard navigation that crosses the visible/
offscreen boundary lands on the correct logical row, and (c) the grid's accessibility
metadata reports the total logical row count and the current focused index.

**Acceptance Scenarios**:

1. **Given** a virtualized grid and an offscreen logical row, **When** that row is focused
   or selected by key, **Then** the model records it as focused/selected and the realized
   window targets it, without materializing every intervening row.
2. **Given** keyboard navigation moving focus from the last visible row past the
   visible/offscreen boundary, **When** the move is applied, **Then** focus lands on the
   correct next logical row and the realized window advances to include it.
3. **Given** a virtualized grid of `Total` logical rows, **When** accessibility metadata
   is queried, **Then** it reports the **total** logical row count and the current focused
   position, independent of how many rows are materialized.

---

### User Story 4 - The virtualization contract is observable as deterministic metrics (Priority: P2)

A maintainer needs `VirtualItemsMaterialized` and `VirtualItemsTotal` in the per-frame
metrics so a regression that defeats virtualization (e.g. materializing every row, or an
overscan that blows past its bound) shows up in the goldens instead of silently costing
CPU and memory.

**Why this priority**: The report requires tracking `VirtualItemsMaterialized` /
`VirtualItemsTotal`. P2 because it hardens and proves US1/US2/US3 rather than delivering
the mechanism itself.

**Independent Test**: Run corpus scenarios at varying total counts and overscan values and
assert the two metrics are deterministic and golden-asserted: materialized bounded by
visible + 2*overscan, total equal to the logical count, and both `0` on a frame that
evaluates no virtualized control.

**Acceptance Scenarios**:

1. **Given** a frame that builds a virtualized DataGrid, **When** the frame is recorded,
   **Then** `VirtualItemsMaterialized` and `VirtualItemsTotal` reflect that grid's
   realized window and logical total, deterministically and golden-asserted.
2. **Given** a frame that evaluates no virtualized control, **When** the frame is
   recorded, **Then** both counts are `0` (no spurious virtualization accounting).
3. **Given** multiple virtualized controls in a frame, **When** the frame is recorded,
   **Then** the counts aggregate across them (per-control attribution available in tests,
   not in the aggregate metric).

---

## Requirements *(mandatory)*

### Functional Requirements

**Virtualization contract (Phase 6 core)**

- **FR-001**: The framework MUST materialize, for a virtualized repeated control
  (DataGrid), only the rows in its realized window — the visible range plus overscan —
  rather than one control per logical item. The realized-window computation builds on the
  existing internal `Collections.visibleRange` / `DataGrid.visibleRows` slicing.
- **FR-002**: The virtualization model MUST gain an **overscan** parameter (a per-side
  count of extra rows realized beyond the visible window — **symmetric**: up to `overscan`
  rows on *each* side, so up to `2*overscan` extra rows total), **defaulting to 0**. With
  overscan 0 the realized window equals today's visible slice; with overscan `N` the
  realized window includes up to `N` additional adjacent logical rows on each side (up to
  `2*N` total), clamped at the logical edges (no index `< 0`, none `>= Total`).
- **FR-003**: For any total logical row count, `VirtualItemsMaterialized <= visibleCount +
  2*overscan` (symmetric overscan — up to `overscan` rows on each side). The materialized
  count MUST NOT scale with the total logical count; a 100-,
  1000-, and 10000-row grid with the same viewport and overscan MUST materialize the same
  bounded number of rows.
- **FR-004**: When the total logical rows already fit within `visibleCount + 2*overscan`,
  the realized window MUST be the whole set (`VirtualItemsMaterialized =
  VirtualItemsTotal`); virtualization is transparent for small grids.
- **FR-005**: DataGrid is the **representative** virtualized surface this rung. Generalizing
  the virtualization contract to other list/collection surfaces is **out of scope**
  (deferred). The shared internal `Collections` model MAY be extended to carry overscan so
  a future surface can reuse it.

**Correctness & parity (Phase 6 invariants)**

- **FR-006**: With overscan at its default (0) and no other opt-in, at-rest rendered
  output, control geometry, and the realized rows MUST be **byte-identical** to the pre-
  feature state for every existing corpus scenario. The virtualization contract is
  **additive**: turning it on with default overscan changes nothing observable except the
  two new metric fields.
- **FR-007**: With overscan `N`, the realized window MUST contain only **correct, real**
  rows (the visible rows unchanged, plus up to `N` correct adjacent logical rows **on each
  side**, up to `2*N` total); it MUST NOT fabricate rows, duplicate rows, or shift the
  visible rows. Overscan MUST be clamped at the logical top/bottom edges.
- **FR-008**: Scrolling the realized window MUST reuse row containers where the keyed diff
  permits (stable row keys → reuse), so a scroll does not rebuild unchanged rows. (The
  existing keyed identity over `row.Key` is the reuse basis; this feature MUST NOT
  regress it.)

**Offscreen addressability (Phase 6 correctness — clarified in scope)**

- **FR-009**: Keyboard **focus** MUST be targetable to a logical row that is **outside**
  the realized window. Focusing an offscreen logical row by key MUST record it as focused
  and bring the realized window to it (so it materializes), **without** materializing every
  intervening logical row.
- **FR-010**: **Selection** MUST be targetable to an offscreen logical row by key (a row's
  selection state is a property of the logical item, not of its materialized control), so
  selecting/toggling an offscreen row updates the model without materializing the whole
  range.
- **FR-011**: Keyboard navigation that **crosses** the visible/offscreen boundary (e.g.
  moving focus past the last realized row) MUST land on the correct next **logical** row
  and advance the realized window to include it.
- **FR-012**: **Accessibility metadata** for a virtualized control MUST report the
  **total** logical item count and the **current focused position** (index within the
  total), independent of how many items are materialized — so assistive technology sees
  the true size and position, not the realized slice.

**Observability (Phase 6 metrics)**

- **FR-013**: The framework MUST expose deterministic **`VirtualItemsMaterialized`** and
  **`VirtualItemsTotal`** per-frame metrics. Both are **public `FrameMetrics` fields**
  (clarified 2026-06-13), threaded through the existing `WorkReductionRecord` →
  `FrameMetrics` path (the 113 pattern), reproducible and **golden-asserted** via the
  `Perf.runScript` corpus. A frame that evaluates no virtualized control reports both as
  `0`.
- **FR-014**: `VirtualItemsMaterialized` MUST count the row items actually materialized
  this frame and `VirtualItemsTotal` MUST count the logical items the virtualized
  control(s) represent; when multiple virtualized controls build in one frame the counts
  aggregate across them. The metrics MUST make a regression that materializes every row,
  or an overscan exceeding its bound, visible as a golden change.

**Evidence (Phase 6 proof)**

- **FR-015**: The `Perf.runScript` corpus MUST include a **10000-row DataGrid** scenario
  (alongside the existing 100/1000-row variants from feature 109's Phase 0 corpus)
  asserting `VirtualItemsMaterialized` is bounded by visible + 2*overscan while
  `VirtualItemsTotal = 10000`, proving materialization does not scale with total count.

**Behaviour preservation (cross-cutting)**

- **FR-016**: This feature is **additive virtualization-contract + observability +
  offscreen-correctness only**. With default overscan, at-rest rendered output, control
  geometry, focus/keyboard routing semantics for materialized rows, and every existing
  dispatch outcome MUST remain **byte-identical** to the pre-feature state. The only
  intended observable changes are (a) opt-in overscan materializing up to `N` extra real
  rows, (b) the new `VirtualItemsMaterialized` / `VirtualItemsTotal` `FrameMetrics`
  fields, and (c) offscreen focus/selection addressability + a11y totals.
- **FR-017**: Features 113 (memoization seam — the DataGrid `gridGeom` memoization MUST
  continue to work over the virtualized row set), 112 (targeted runtime stamp), 111
  (scheduler/view-skip), 110 (retained routing), and the retained render pipeline are
  unchanged; this feature only extends the existing virtualization slice with overscan +
  offscreen addressability + the two metric fields.
- **FR-018**: This rung keeps **uniform fixed `RowHeight`** virtualization. Variable /
  measured row heights and all row/column/text **measurement caches** are **deferred to
  Phase 8** (clarified 2026-06-13).

> Interacting / conflicting requirements:
> - **FR-002/FR-007 (overscan materializes extra rows) vs FR-006/FR-016 (byte-identical
>   at rest)** — resolution: overscan **defaults to 0**, so the default realized window is
>   exactly today's visible slice and output is byte-identical. Overscan `> 0` is an
>   explicit consumer opt-in; only then does the realized window grow, and the extra rows
>   are correct real rows (FR-007). A grid never silently materializes more than today
>   unless it opts in.
> - **FR-009/FR-011 (offscreen focus targets a row and advances the window) vs FR-003
>   (materialized count bounded)** — resolution: focusing an offscreen row **moves** the
>   realized window to that row (so it and its neighbours within visible + 2*overscan
>   materialize); it does **not** materialize every intervening row. The bound in FR-003
>   holds at all times — the window relocates, it does not expand to cover the path.
> - **FR-012 (a11y reports total + position) vs FR-001 (only the window is materialized)**
>   — resolution: accessibility metadata is computed from the **logical** model (total
>   count, focused index), not from the materialized controls, so it reports the true size
>   and position even though only the window exists as controls.
> - **FR-013 (`VirtualItemsTotal` golden-asserted) vs the live host** — resolution: like
>   113's memo counts, virtualization runs on the **deterministic `Perf.runScript` render
>   path**, so the counts are reproducible there and are the authoritative golden evidence;
>   the live `OnFrameMetrics` sink reports the same fields. The counts harden without
>   depending on a live window.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: Touches `FS.Skia.UI.Controls` — the internal `Collections` /
  `DataGrid` virtualization model gains an **overscan** parameter and offscreen
  focus/selection targeting + accessibility total/position reporting; and
  `FS.Skia.UI.Controls.Elmish` — the `FrameMetrics` record gains the public
  `VirtualItemsMaterialized` / `VirtualItemsTotal` fields, threaded from the retained step
  / `WorkReductionRecord` and surfaced through `Perf.runScript` and the live
  `OnFrameMetrics` sink. No package identity changes; package **contents** change and
  versions bump on merge. DataGrid is the active controls authoring path (no legacy Charts
  migration involved).
- **Public contract impact**: **Breaking** `ControlsElmish.fsi` `FrameMetrics` change —
  two new public fields (`VirtualItemsMaterialized`, `VirtualItemsTotal`), so the top-level
  surface baseline changes (precedent: 109/110/111/113 each added `FrameMetrics` fields).
  Possible additive surface in the `Controls` `Collections` / `DataGrid` `.fsi` for the
  overscan parameter and any offscreen-addressability / accessibility-total `val`s
  (additive where possible; the realized-window and a11y functions may change signature to
  carry overscan / total). No public consumer-facing virtualization primitive beyond the
  existing DataGrid/Collections surface. `Route` is expected to escalate to the
  **controls-public-surface** tier because the Controls (and `Controls.Elmish`
  `FrameMetrics`) `.fsi` surfaces change; run `Route` first and obey its printed list.
- **State workflow impact**: None to MVU semantics — `Update`, effects, subscriptions,
  commands, and interpreter behaviour are unchanged. The `CollectionModel` / `DataGridModel`
  gain an overscan field and offscreen focus/selection targeting, but dispatch *outcomes*
  for materialized rows are byte-identical (FR-016); offscreen focus/selection is a new
  *capability* (previously unreachable), not a changed outcome.
- **Layout/rendering impact**: With default overscan (0), rendered output, geometry, and
  the retained step are byte-identical (FR-006/FR-016). With opt-in overscan, up to `N`
  additional correct rows materialize (more `data-grid-row` controls), which is an
  intended, bounded change. Uniform fixed `RowHeight` only (FR-018). No Vulkan/Skia change;
  no unsupported-environment diagnostic change. DataGrid visible-region rendering stays
  byte-identical.
- **Evidence obligations**: bounded-materialization evidence over the 100/1000/10000-row
  corpus (FR-003/FR-015: materialized `<= visible + 2*overscan`, total scales); default-
  overscan byte-identity vs the pre-feature baseline (FR-006); opt-in overscan correctness
  (only real adjacent rows, edge-clamped, FR-007); offscreen focus/selection
  addressability + boundary-crossing navigation (FR-009/FR-010/FR-011); accessibility
  total + position reporting (FR-012); `VirtualItemsMaterialized` / `VirtualItemsTotal`
  metric evidence (steady vs no-virtualized-control frame, FR-013/FR-014); the regenerated
  `Perf.runScript` corpus goldens carrying the two new metric fields; at-rest byte-identity
  (the standing Scene-parity golden suite under `Dev`); skill-loading evidence; the
  window-visibility not-applicable set; `readiness/evidence-audit.md` with a verdict token;
  the generated-validation package-resolution tokens. The escalated `maintainer-verify`
  readiness set applies because of the Controls `.fsi` change.
- **Unsupported scope**: This feature is **Phase 6 only**. Explicitly OUT: **variable /
  measured row heights** and **row/column/text measurement caches** (Phase 8 — deferred);
  **paint caches / damage rectangles / Skia picture boundaries** (Phase 7); **layout
  hot-path / text-measurement caches & layout-boundary hints** (Phase 8); **`SkiaViewer`
  backend / render-thread / compositor review** (Phase 9); generalizing virtualization to
  **non-DataGrid list/collection surfaces** (DataGrid is the representative this rung);
  horizontal/column virtualization (rows only this rung). No renderer rewrite, no
  Avalonia/WPF redesign, no platform/release/distribution scope.
- **Build-target impact**: Escalation to the controls-public-surface set is expected because
  the Controls (and `Controls.Elmish` `FrameMetrics`) `.fsi` surfaces change; run `Route`
  first and obey its printed minimal list (`Dev`, the package/per-package surface diffs,
  `FsiTranscripts`, the controls catalog/doc/interaction/rendering checks,
  `GeneratedGuidanceCheck`, `TemplateDrift`, `EvidenceGraph`, `EvidenceAudit`).
  `RefreshSurfaceBaselines` must regenerate the top-level + per-package baselines after the
  `FrameMetrics` / `Collections` / `DataGrid` additions, and the `Perf.runScript` corpus
  goldens must be regenerated (`PERF_CORPUS_REGEN=1`) to carry the two new metric fields and
  the 10000-row scenario. No new gate.

## Success Criteria *(mandatory)*

- **SC-001**: A 10000-row DataGrid with a bounded viewport realizing `V` visible rows and
  overscan `N` materializes at most `V + 2*N` row items (`VirtualItemsMaterialized <= V + 2*N`)
  while `VirtualItemsTotal = 10000`; the materialized count is identical at 100, 1000, and
  10000 total rows (does not scale with total).
- **SC-002**: With default overscan (0), **100%** of existing corpus DataGrid scenes are
  byte-identical to the pre-feature baseline, and `VirtualItemsMaterialized` equals the
  prior realized-row count.
- **SC-003**: With opt-in overscan `N`, only the visible rows plus up to `N` correct
  adjacent logical rows **on each side** (up to `2*N` total) materialize (edge-clamped, no
  fabricated/duplicated rows), and the visible rows are unchanged.
- **SC-004**: Focusing or selecting an offscreen logical row by key targets it (records it
  + relocates the realized window) without materializing every intervening row, and the
  materialized count stays within `V + 2*N`.
- **SC-005**: Keyboard navigation crossing the visible/offscreen boundary lands on the
  correct next logical row, and accessibility metadata reports the total logical row count
  and current focused position independent of materialization.
- **SC-006**: `VirtualItemsMaterialized` and `VirtualItemsTotal` are deterministic, golden-
  asserted `FrameMetrics` fields: a virtualized-grid frame reports the bounded materialized
  count + true total, and a frame with no virtualized control reports both `0`.
- **SC-007**: At-rest rendered output, control geometry, materialized-row focus/keyboard
  routing semantics, and all existing dispatch outcomes are byte-identical to the pre-
  feature state; feature 113's DataGrid memoization continues to work over the virtualized
  row set.

## Key Entities

- **Realized window / `VisibleRange`** (internal, `Controls`): the slice of logical rows
  currently materialized — `FirstIndex` / `Count` within `Total`, now widened by overscan.
  The existing `Collections.visibleRange` computes it; this feature extends it with
  overscan and keeps `VirtualItemsMaterialized <= visible + 2*overscan` at all times.
- **Overscan** (new, default 0): a count of extra logical rows realized beyond the visible
  window (symmetric: up to `N` on each side, up to `2*N` total), clamped at the logical
  edges. Default 0 ⇒ realized window == today's visible slice ⇒ byte-identical at rest;
  opt-in `N` ⇒ up to `N` extra correct adjacent rows on each side.
- **`VirtualItemsMaterialized` / `VirtualItemsTotal`**: public, deterministic `FrameMetrics`
  fields counting, per frame, the row items actually materialized vs the logical items the
  virtualized control(s) represent; golden-asserted via `Perf.runScript`; both `0` on a
  frame with no virtualized control; aggregate across multiple virtualized controls. The
  observability surface this feature adds (public, like 113's memo counts, because
  virtualization runs on the deterministic render path).
- **Offscreen logical row**: a logical item outside the realized window with **no**
  materialized control, which MUST still be focusable/selectable by key (targeting it
  relocates the window) and MUST be counted in accessibility totals + position.
- **Accessibility total / position**: the total logical item count and current focused
  index a virtualized control reports to assistive technology, computed from the logical
  model (not the materialized slice).
- **10000-row DataGrid corpus scenario**: the evidence scenario proving materialization is
  bounded by visible + 2*overscan regardless of total logical count.

## Assumptions

- The realized window and its overscan are computed by extending the existing internal
  `Collections.visibleRange` / `DataGrid.visibleRows` path; the plan phase decides the exact
  signature change (e.g. an added `overscan` parameter / `CollectionModel` field) and
  whether it is additive or a breaking `.fsi` signature change.
- `VirtualItemsMaterialized` / `VirtualItemsTotal` are threaded through the same
  `WorkReductionRecord` → `FrameMetrics` mechanism feature 113 used for `MemoHits` /
  `MemoMisses`; the plan picks the exact carrier field names.
- "Representative virtualized surface" means the DataGrid (Collections-backed). Other
  list/collection surfaces are out of scope this rung; the shared `Collections` model may be
  extended so a future surface can reuse the overscan/metric machinery.
- Offscreen focus/selection targeting operates on the **logical** model (row keys / indices),
  consistent with the existing `DataGrid` `SelectRow` / `FocusCell` / `ScrollRowsTo`
  messages over `row.Key`; the plan decides how a focus/selection on an offscreen key
  relocates the realized window.
- Accessibility total + position are derived from `RowCount` / `Total` and the focused
  index, surfaced through the existing accessibility-metadata path; the plan picks the exact
  field(s).
- Uniform fixed `RowHeight` is assumed (the model already assumes it); variable/measured
  heights and measurement caches are Phase 8.
- Overscan is rows-only (vertical); column/horizontal virtualization is out of scope.
- The 10000-row corpus scenario extends feature 109's Phase 0 corpus (which already
  specified 100/1000/10000-row DataGrid scenarios) with the bounded-materialization
  assertions; the plan picks the exact corpus file/golden layout.
