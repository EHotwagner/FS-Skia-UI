# Contract: Incremental Measure / Partial Re-Layout (R2)

This feature **preserves** public surface. The only public contract touched is the **behavior** (not
signature) of `Layout.evaluateIncremental`. Everything else is internal. This document states the
behavioral contracts the implementation and tests bind to.

## C1 — Public: `Layout.evaluateIncremental` behavior (signature unchanged)

```fsharp
// src/Layout/Layout.fsi:10 — UNCHANGED
val evaluateIncremental :
    previous: LayoutResult ->
    changedNodeIds: LayoutNodeId list ->
    available: AvailableSpace ->
    root: LayoutNode ->
    LayoutResult
```

**Contract**:
- **Bounds equivalence (INV-1)**: `(evaluateIncremental previous dirty available root).Bounds` is
  **byte-identical** (exact `NodeId → ComputedBounds` record equality) to
  `(evaluate available root).Bounds`, for any `previous`, any `dirty` (subset/superset of the true
  change set), any `available`, any `root`. Correctness does **not** depend on the caller passing a
  minimal or even correct dirty set — a wrong `dirty` may cost extra re-measure but **never** wrong
  geometry. (`dirty` is a performance hint, not a correctness input.)
- **Invalidated (INV-4)**: the returned `Invalidated` is the **actual re-measured set** — `dirty`
  after flex-line + fixed-size-ancestor propagation (C3) — not the verbatim input.
- **Revision (INV-4)**: returned `Revision = previous.Revision + 1L`.
- **Diagnostics**: preserved verbatim from the underlying measure (no new diagnostic class).
- **Totality**: total; never throws; a cache miss / unknown dirty id degrades to a full re-measure of
  the affected subtree (conservative).

**Caller note**: because Bounds equivalence holds for *any* `dirty`, a future caller change cannot
break geometry; the worst case is a slower frame, caught by the re-measure metric (C5).

## C2 — Internal: layout-dirty-set derivation from the patch

```
// Controls-side, pure. LayoutNodeId (layout-path) domain.
layoutDirtySet : prev: Control<'msg> -> patch: Reconcile.NodePatch<'msg> -> Set<LayoutNodeId>
```

**Contract (INV-2)** — a node `id` is **self-dirty** iff its patch is `Update u` and:
- `u.AttrChanges` contains an `AttrSet attr` with `attr.Category = AttrCategory.Layout`, **or**
- `u.AttrChanges` contains an `AttrRemoved name` whose attribute on the **prev** node had
  `Category = AttrCategory.Layout`, **or**
- `u.Children` contains any `ChildInsert` / `ChildRemove` / `ChildMove` (a `ChildKeep` alone is not
  dirtying).

`NodePatch.Keep` and `NodePatch.Replace` contribute **no** self-dirt (Keep = unchanged; Replace =
new subtree, full-measured fresh, old cache entry discarded). A `Content`/`Style`/`State`/
`Accessibility` (incl. R1 `visualState`) `Update` with no dirty descendant contributes nothing — a
hover restyle stays paint-only.

**Prohibition (FR-003)**: classification is driven by `attr.Category`, never a hand-maintained
attribute-name list.

## C3 — Internal: conservative propagation

**Contract (INV-3)** — given the self-dirty set, the **propagated** set is computed by:
1. For each self-dirty node, add **every sibling on its nearest enclosing flex container/line** (flex
   redistributes free space across the line).
2. Climb upward, adding ancestors, **until** (and including) the first ancestor whose
   `LayoutIntent.Size` is `Some` (fixed/content-independent) on the constraining axis; **stop** there
   — that ancestor's own box is pinned, so its ancestors stay clean.
3. If no fixed-`Size` ancestor exists before the root (every ancestor content-sized on the axis), the
   set reaches the **root** (correct, not degenerate).
4. **When in doubt**, treat an ancestor as **not** fixed (keep climbing) — never under-dirty.

The propagated set is what `evaluateIncremental` re-measures and what it reports in `Invalidated`.

## C4 — Internal: render-path seam

```fsharp
// New internal variant alongside ControlInternals.evaluateLayout (src/Controls/Control.fs:1219)
evaluateLayoutIncremental :
    size: Size -> control: Control<'msg> ->
    previous: LayoutResult -> cache: <measure cache> -> dirty: Set<LayoutNodeId> ->
    LayoutNode * Map<LayoutNodeId, Rect> * LayoutResult
```

**Contract (INV-8)**:
- Returns the same `root, boundsById` shape `RetainedRender.step` already consumes, so the
  reuse-driven paint walk (`box = pr.Fragment.Box`, `carry`/`build`) is unchanged.
- `RetainedRender.step` calls it instead of the unconditional full `evaluateLayout`
  (`RetainedRender.fs:141`), threading `previous` (the carried `LayoutResult_{N-1}`), the measure
  cache, and the `dirty` set from C2/C3.
- **First frame / no previous**: runs full `evaluate`, seeds the cache, returns `Revision = 1`.
- **Theme change (INV-7)**: `themeChanged` still forces a full **repaint** (paint concern) but does
  **not** add measure-dirt (geometry is theme-independent); cached bounds are reused.
- All E2 invariants (`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`,
  first-frame full paint, `KeyCollision` diagnostics) are preserved.

## C5 — Internal: extended `WorkReductionRecord`

```fsharp
type internal WorkReductionRecord =
    { BaselineNodeCount; RecomputedNodeCount; ChangedSubtreeBound; ShiftedNodeCount
      RemeasuredNodeCount: int }   // NEW
```

**Contract (INV-5)**:
- localized update → `RemeasuredNodeCount < BaselineNodeCount`, consistent with the dirty flex-line
  subtree;
- genuine whole-tree relayout → `RemeasuredNodeCount = BaselineNodeCount` (never under-reports);
- empty (all-`Keep`) patch → `RemeasuredNodeCount = 0`.

Internal record → public baseline unchanged (INV-9).

## C6 — Equivalence gate (test contract, FR-007)

A property test asserts C1's Bounds equivalence over **generated** trees and **cumulative** edit
sequences:
- ≥1000 cases (SC-002);
- each edit applied through both `evaluateIncremental` (cache carried forward) and full `evaluate`;
- exact `Bounds` equality at **every** step (including long sequences stressing cache staleness);
- any divergence fails the gate (no tolerance).

## Surface posture

| Surface | Change |
|---|---|
| `Layout.evaluateIncremental` signature | **none** (body only) |
| `LayoutResult` shape | **none** (`Invalidated` **value** changes) |
| `FS.Skia.UI.Layout` baseline | **unchanged** (SC-006) |
| measure cache (`RenderFragment`/`RetainedNode`) | internal |
| `WorkReductionRecord` re-measure field | internal |
| consumer `view : 'model -> Control<'msg>` contract | **unchanged** (FR-009) |
