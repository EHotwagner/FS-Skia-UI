# Phase 1 Data Model: Incremental Measure / Partial Re-Layout (R2)

R2 adds **no public type**. It changes the **behavior** of one public function, extends one
**internal** record, and reuses existing public/internal types. This document names every entity it
touches, the new internal shape, and the invariants that bind them.

## Reused public types (unchanged shape)

### `LayoutResult` (`src/Layout/Types.fsi`)
```fsharp
type LayoutResult =
    { Bounds: ComputedBounds list      // constrained to byte-identity with full evaluate (FR-008)
      Diagnostics: LayoutDiagnostic list
      Invalidated: LayoutNodeId list   // VALUE changes: honest post-propagation set (FR-001a)
      Revision: int64 }                // continues to advance: previous.Revision + 1
```
- **No field added.** Only the runtime **value** of `Invalidated` changes (stub echo → actual
  re-measured set) and `Bounds` is now produced incrementally (same values).

### `ComputedBounds` (`src/Layout/Types.fsi`)
```fsharp
type ComputedBounds =
    { NodeId: LayoutNodeId; Bounds: LayoutBounds; Visibility: LayoutVisibility }
```
- The equivalence oracle compares the `NodeId → ComputedBounds` map by exact record equality.

### `LayoutNode` / `LayoutIntent` / `LayoutSize` (`src/Layout/Types.fsi`)
```fsharp
type LayoutNode = { Id: LayoutNodeId; Intent: LayoutIntent; Measure: ContentMeasure option
                    Content: Scene option; Children: LayoutNode list }
type LayoutIntent = { Direction; Wrap; AlignItems; AlignSelf; JustifyContent
                      Padding; Margin; Gap; Size: LayoutSize; MinSize; MaxSize
                      FlexGrow: float; FlexShrink: float; FlexBasis: float option }
type LayoutSize = { Width: float option; Height: float option }
```
- **Fixed-size ancestor (FR-004)**: a `LayoutNode` whose `Intent.Size` is `Some` on the constraining
  axis. `None` on an axis = content-derived = **not** an absorbing boundary on that axis.
- `LayoutNodeId = string` — the **layout path** domain `toLayout "0" control` mints; the dirty set
  lives in this domain.

### `Reconcile` patch types (`src/Controls/Reconcile.fsi`, `module internal`)
```fsharp
type NodePatch<'msg> = Keep | Replace of Control<'msg> | Update of UpdatePatch<'msg>
and UpdatePatch<'msg> =
    { AttrChanges: AttrChange<'msg> list; ContentChange; AccessibilityChange
      Children: ChildOp<'msg> list }
and AttrChange<'msg> = AttrSet of Attr<'msg> | AttrRemoved of name: string
and ChildOp<'msg> = ChildKeep of int*NodePatch | ChildMove of int*int*NodePatch
                  | ChildInsert of int*Control | ChildRemove of ControlId option*int
```
- The **dirty source**. Consumed read-only; `diff` is unchanged.

### `Attr` / `AttrCategory` (`src/Controls/Types.fsi`)
```fsharp
and Attr<'msg> = { Name: string; Category: AttrCategory; Value: AttrValue<'msg> }
type AttrCategory = Content | Children | Layout | Style | Theme | State
                  | Validation | Accessibility | Event | Data | Slot
```
- **`AttrCategory.Layout`** is the authoritative classifier of layout-affecting attrs (size/min/max,
  padding/margin/gap, flex grow/shrink/basis, direction/wrap/align/justify). R2 reads
  `attr.Category` — never a hand-maintained name list (FR-003).

## Changed-behavior public entity

### `Layout.evaluateIncremental` (`src/Layout/Layout.fsi:10` — signature unchanged)
```fsharp
val evaluateIncremental :
    previous: LayoutResult ->
    changedNodeIds: LayoutNodeId list ->
    available: AvailableSpace ->
    root: LayoutNode ->
    LayoutResult
```
- **Before (stub)**: `let result = evaluate available root in { result with Revision = previous.Revision + 1L; Invalidated = changedNodeIds |> List.distinct }` — full measure, echoed input.
- **After (R2)**: re-measures only the propagated dirty set, reuses `previous.Bounds` for the rest,
  returns `Bounds` byte-identical to `evaluate available root`, `Invalidated` = the actual
  re-measured set (post-propagation), `Revision = previous.Revision + 1`.

## New / extended internal entities

### Retained measure cache — rides `RenderFragment` / `RetainedNode` (`src/Controls/RetainedRender.fsi`, internal)
```fsharp
// RenderFragment today caches Box; R2 extends the cached unit so an unchanged subtree's
// intrinsic measure + bounds survive across frames, keyed by the node's stable RetainedId.
type internal RenderFragment =
    { OwnScene: Scene list
      SubtreeScene: Scene list
      Box: Rect option
      // + cached measure inputs/result sufficient to reuse-or-translate without re-measuring
      //   (intrinsic measured size + the ComputedBounds for this node's layout id) }
```
- **Key**: `RetainedId` (stable across positional shifts — the E2 identity). A `LayoutNodeId`-keyed
  cache would falsely miss on a sibling reorder (the bug 092 fixed for state).
- **Purity (FR-002)**: keyed on content/intrinsic-measure inputs only (kind, content,
  layout-relevant attrs, available space on the measured axis); no clock/randomness; confined to the
  host loop's mutable-ref retained state (interpreter edge).
- **Translate-don't-re-measure**: when an ancestor moved but the subtree is unchanged, reuse the
  cached intrinsic measure and **translate** bounds by the ancestor delta (spec edge "Ancestor
  moved").

### Previous-`LayoutResult` carry — `RetainedRender<'msg>` (internal)
```fsharp
type internal RetainedRender<'msg> =
    { Root: RetainedNode<'msg>
      NextId: uint64
      StateByIdentity: Map<RetainedId, RetainedUiState>
      Theme: Theme
      // + the previous frame's LayoutResult, so evaluateIncremental can reuse its Bounds }
```
- Threaded into `RetainedRender.step` and passed to the incremental `evaluateLayout` seam. Absent on
  the first frame → full `evaluate` + cache seed (spec edge "Cache miss / first frame").

### `WorkReductionRecord` (`src/Controls/RetainedRender.fsi:70`, internal — extended)
```fsharp
type internal WorkReductionRecord =
    { BaselineNodeCount: int        // full-rebuild work (== N)
      RecomputedNodeCount: int      // paint recomputes (existing)
      ChangedSubtreeBound: int      // genuinely-changed paint work (existing)
      ShiftedNodeCount: int         // paint recomputed due to upstream relayout (existing, 092)
      RemeasuredNodeCount: int }    // NEW (FR-006): nodes actually re-measured this frame
```
- **Internal** record → no public baseline move (SC-006).

## Dirty-set derivation (pure function, Controls-side)

```
layoutDirtySet (prev: Control) (patch: NodePatch) : Set<LayoutNodeId>
  walk the patch in the LayoutNodeId (layout-path) domain:
    Keep            -> {}                                 // unchanged
    Replace _       -> {}  (handled as new subtree: full-measure the replacement, discard old cache)
    Update u at id  -> let selfDirty =
                          (u.AttrChanges has an AttrSet whose attr.Category = Layout)
                          || (u.AttrChanges has an AttrRemoved whose prev attr.Category = Layout)
                          || (u.Children has any ChildInsert | ChildRemove | ChildMove)
                       (if selfDirty then {id} else {}) ∪ (recurse children)
  then PROPAGATE (FR-004):
    for each dirty id: add its whole nearest flex container/line (all siblings on the line),
                       then climb to the first ancestor with fixed Intent.Size on the
                       constraining axis (inclusive) and STOP; if none, reach the root.
```
- `AttrRemoved` category is recovered from the **prev** node's attrs (the retained walk holds them).
- "When in doubt, dirty" — an ambiguous fixed-size determination treats the ancestor as **not**
  fixed (keep climbing); the equivalence invariant is the backstop.

## State transitions (per frame, on the wired retained path)

```
frame N (prev: RetainedRender carrying LayoutResult_{N-1} + measure cache)
  -> Reconcile.diff prev.Root.Control next            = patch          (E2, unchanged)
  -> dirty = layoutDirtySet prev.Root.Control patch  (LayoutNodeId domain, propagated FR-004)
  -> root, boundsById, LayoutResult_N =
        if first frame OR no prev LayoutResult: full evaluate (seed cache)
        else ControlInternals.evaluateLayoutIncremental size next LayoutResult_{N-1} cache dirty
                 -> Layout.evaluateIncremental LayoutResult_{N-1} dirty available root
  -> reuse-driven paint walk keyed on box (= boundsById)  (E2 carry/build, unchanged)
  -> WorkReductionRecord { ...; RemeasuredNodeCount = |re-measured| }
  -> next RetainedRender carries LayoutResult_N + updated measure cache
```

## Invariants

| # | Invariant | Source |
|---|---|---|
| INV-1 | `evaluateIncremental(...).Bounds` byte-identical to `evaluate available root` for **every** frame & edit sequence | FR-001, FR-008, SC-002, SC-005 |
| INV-2 | Dirty node ⇔ patch sets/removes an `AttrCategory.Layout` attr **or** carries a non-`Keep` `ChildOp` | FR-003, SC-004 |
| INV-3 | Dirt = whole nearest flex line, climbing to (incl.) first fixed-`Size` ancestor, stopping there; content-chain reaches root | FR-004, SC-004 |
| INV-4 | `Invalidated` = actual re-measured set (post-propagation); `Revision = previous.Revision + 1` | FR-001a, SC-008 |
| INV-5 | `RemeasuredNodeCount`: localized < baseline; whole-tree = baseline; empty patch = 0 | FR-006, SC-003 |
| INV-6 | Cache keyed by `RetainedId`; unchanged-but-shifted subtree is **translated**, not re-measured | FR-002, spec edge |
| INV-7 | Theme-only change does **not** dirty measure (geometry theme-independent); still full repaint | FR-008, spec edge |
| INV-8 | All E2 determinism invariants (`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`, first-frame full paint, `KeyCollision`) hold on the wired path | SC-007 |
| INV-9 | Public `FS.Skia.UI.Layout` surface baseline unchanged; cache + metric remain internal | FR-009, SC-006 |
