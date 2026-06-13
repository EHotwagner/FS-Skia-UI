# Phase 1 Data Model: Viewport Virtualization (Phase 6)

All types are F# records/DUs. New/changed surface is marked. Anchors reference the current
tree (see plan.md "Key seams").

## 1. `VisibleRange` (existing, `Collections.fsi:4`) — semantics widened, shape unchanged

```fsharp
type VisibleRange =
    { FirstIndex: int   // first realized logical index (now shifted back by overscan, clamped >= 0)
      Count: int        // realized row count (now widened by overscan, clamped to Total)
      Total: int }      // total logical item count (unchanged meaning)
```

The **record shape is unchanged** — overscan only changes how `FirstIndex`/`Count` are
*computed*. With overscan 0 the values are byte-identical to today. `VirtualItemsTotal`
reads `Total`; `VirtualItemsMaterialized` equals the realized `Count` (== number of
materialized `data-grid-row` nodes).

## 2. Overscan-widened realized window (`Collections.visibleRange`, `Collections.fs:28`)

**Computation** (symmetric, edge-clamped; see research (a)):

```
given f, c, t from the overscan-0 slice and overscan N >= 0:
  first' = max 0 (f - N)
  last'  = min (t - 1) (f + c - 1 + N)      // when t > 0
  count' = if t <= 0 then 0 else last' - first' + 1
  result = { FirstIndex = first'; Count = count'; Total = max 0 t }
```

**Signature** (additive trailing parameter):

```fsharp
// before: val visibleRange: rowHeight -> viewportHeight -> scrollOffset -> totalItems -> VisibleRange
// after:  val visibleRange: rowHeight -> viewportHeight -> scrollOffset -> totalItems -> overscan -> VisibleRange
```

Invariants: `0 <= FirstIndex`; `FirstIndex + Count <= Total`; `Count <= (overscan-0
Count) + 2*N`; `N = 0 ⇒ result == overscan-0 result`.

## 3. `CollectionModel` / `DataGridModel` — new `Overscan` field (default 0)

```fsharp
type CollectionModel =          // Collections.fsi:11
    { ControlId: ControlId
      ItemCount: int
      RowHeight: float
      ViewportHeight: float
      ScrollOffset: float
      SelectedKeys: Set<string>
      VisibleRange: VisibleRange
      Overscan: int             // NEW — extra rows realized each side; default 0; clamped >= 0
      RecalculationThresholdMs: int }

type DataGridModel =            // DataGrid.fs:36
    { ControlId: ControlId
      Columns: DataGridColumn list
      RowCount: int
      RowHeight: float
      ViewportHeight: float
      VisibleRange: VisibleRange
      Overscan: int             // NEW — default 0
      SelectedRows: Set<string>
      FocusedCell: DataGridFocusedCell option
      Sort: DataGridSort option
      FilterText: string option
      Diagnostics: ControlDiagnostic list }
```

`Overscan = 0` MUST be added at **every** construction site: `Collections.init`,
`Collections.withRange`, DataGrid model construction, samples (`ControlsGallery`,
`DemoReel`), and FSI preludes (`scripts/*-prelude.fsx`). `Collections.withRange` /
`DataGrid.range` pass `model.Overscan` into `visibleRange`. Negative overscan is clamped to
0 on the way in.

## 4. `WorkReductionRecord` — new internal counts (`RetainedRender.fsi:128`)

```fsharp
type WorkReductionRecord =
    { BaselineNodeCount: int
      RecomputedNodeCount: int
      ChangedSubtreeBound: ...
      ShiftedNodeCount: int
      RemeasuredNodeCount: int
      MemoHits: int             // feature 113
      MemoMisses: int           // feature 113
      VirtualMaterialized: int  // NEW — count of materialized data-grid-row nodes this frame
      VirtualTotal: int }       // NEW — sum of logical Total across virtualized controls this frame
```

Populated in retained `step` (`RetainedRender.fs:426`) by walking the lowered tree:
`VirtualMaterialized` = count of `data-grid-row` kind nodes; `VirtualTotal` = sum of the
`Total` field on each `data-grid` node's `VisibleRange` attr. Walk is read-only — render
output unchanged. Both `0` when no `data-grid` node is present.

## 5. `FrameMetrics` — new public fields (`ControlsElmish.fsi:68`)

```fsharp
type FrameMetrics =
    { ProductModelChanged: bool
      ViewCalled: bool
      ... (existing fields, incl. MemoHitCount / MemoMissCount from 113) ...
      VirtualItemsMaterialized: int   // NEW (public) — = WorkReductionRecord.VirtualMaterialized
      VirtualItemsTotal: int }        // NEW (public) — = WorkReductionRecord.VirtualTotal
```

- `zero` (`ControlsElmish.fs:1332`): both `0`.
- Every per-frame construction site (`:1376`, `:1425`, `:1449`, `:1472`) lifts the two
  counts from `lastWorkReduction` (`:858`), exactly as `MemoHitCount`/`MemoMissCount`.
- Surfaced on `Perf.runScript` (golden) and the live `OnFrameMetrics` sink.

**Semantics:** `VirtualItemsMaterialized <= visibleCount + 2*overscan` always; does not
scale with `Total`; `= VirtualItemsTotal` when the grid fits the realized window; `0`/`0`
on a frame with no virtualized control; aggregates across multiple virtualized controls.

## 6. `AccessibilityMetadata` — new optional total/position field (`Types.fsi:212`)

```fsharp
type CollectionPosition =       // NEW
    { TotalItems: int           // total logical item count (from RowCount / ItemCount)
      FocusedIndex: int option }// current focused logical index within the total, None if unfocused

type AccessibilityMetadata =
    { Role: AccessibilityRole
      NameSource: string
      State: string list
      FocusOrder: int option
      Keyboard: KeyboardOperation
      Contrast: ContrastEvidence option
      Navigation: NavRange option       // feature 100
      Collection: CollectionPosition option }  // NEW — Some for virtualized collections, None otherwise
```

Populated for a virtualized DataGrid from `RowCount` (TotalItems) and the focused row's
logical index (FocusedIndex, derived from `FocusedCell.RowKey`). `None` for all non-
collection controls → at-rest a11y for existing controls byte-identical. Computed from the
**logical** model, never the materialized slice (FR-012).

## 7. Offscreen focus/selection — state transitions (no new messages)

Reuses `DataGridMsg` (`DataGrid.fs:49`). The capability is "these target a logical
index/key that may be offscreen, and relocate the window":

| Msg | On an offscreen target | Window effect | Materialization |
|-----|------------------------|---------------|-----------------|
| `SelectRow key` | sets `SelectedRows = {key}` (logical) | relocate window to `key`'s index | only window rows |
| `ToggleRow key` | toggles `key` in `SelectedRows` (logical) | relocate window to `key`'s index | only window rows |
| `FocusCell (Some cell)` | sets `FocusedCell` (logical) | relocate window to `cell.RowKey`'s index | only window rows |
| `ScrollRowsTo idx` | — | relocate window so `idx` is realized | only window rows |

**Relocation** computes the scroll offset that brings the target index into the realized
window, then recomputes `VisibleRange` via `DataGrid.range` / `Collections.withRange` with
`model.Overscan`. The window's size stays `visible + overscan`; it **relocates**, never
expands (preserves FR-003 — see research (d)). Boundary-crossing navigation (FR-011)
advances the focused logical index by one and relocates so the new index is realized.

**Dispatch outcomes for *materialized* rows are byte-identical** to pre-feature (FR-016).
Offscreen targeting is a newly *reachable* path (previously an offscreen key had no effect
on the realized window), not a changed outcome for an already-materialized row.

## Aggregate invariants (asserted in tests)

- **Bounded:** `VirtualItemsMaterialized <= visibleCount + 2*overscan` for any `Total`.
- **Non-scaling:** identical `VirtualItemsMaterialized` at `Total ∈ {100, 1000, 10000}`
  with the same viewport + overscan.
- **Transparent small grid:** `Total <= visible + 2*overscan ⇒ VirtualItemsMaterialized =
  VirtualItemsTotal`.
- **Default-0 byte-identity:** overscan 0 ⇒ realized rows, geometry, and scene identical to
  pre-feature; `VirtualItemsMaterialized` = prior realized-row count (FR-006/SC-002).
- **Edge-clamp:** at top/bottom no index `< 0` or `>= Total`; only real rows materialized.
- **Idle:** a frame with no virtualized control ⇒ `0`/`0`.
- **113 composition:** a steady-state overscan frame still records memo hits.
