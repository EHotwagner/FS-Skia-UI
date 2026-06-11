# Quickstart: Incremental Measure / Partial Re-Layout (R2)

How to exercise R2's behavior — public `evaluateIncremental`, the dirty-set derivation, the
re-measure metric, and the equivalence invariant. R2 is performance-and-metric-only: the visible
output never changes, so the "proof" is in the metric and the equivalence, not on screen.

## 1. The public `evaluateIncremental` (FSI)

```fsharp
#r "nuget: FS.Skia.UI.Layout"   // or reference the locally-packed DLL
open FS.Skia.UI.Layout

// Build a small flex tree (a row with three leaves), evaluate it fully once:
let available : AvailableSpace =
    { Width = 800.0; WidthMode = Exactly; Height = 200.0; HeightMode = Exactly }
let root0 : LayoutNode = (* a flex-row with leaves "a","b","c" *) ...
let full0 = Layout.evaluate available root0

// Change ONE leaf's content (no layout-affecting attr) and re-evaluate incrementally,
// passing the layout id of the changed leaf as the dirty set:
let root1 : LayoutNode = (* same tree, leaf "b" content changed *) ...
let inc1 = Layout.evaluateIncremental full0 [ "0/1" (* leaf b's LayoutNodeId *) ] available root1

// 1a. Bounds are byte-identical to a full re-evaluate (INV-1):
let full1 = Layout.evaluate available root1
inc1.Bounds = full1.Bounds      // -> true

// 1b. Invalidated reports the ACTUAL re-measured set (post flex-line/fixed-size propagation),
//     NOT the verbatim ["0/1"] input (FR-001a / INV-4):
inc1.Invalidated                // -> the propagated set (>= ["0/1"], bounded by the absorbing ancestor)

// 1c. Revision advances:
inc1.Revision = full0.Revision + 1L   // -> true
```

> Before R2, step 1b returned exactly `["0/1"]` (the echoed input) and the call internally
> re-measured the **whole** tree. After R2, only the dirty flex line (up to the first fixed-size
> ancestor) is re-measured, and `Invalidated` is honest.

## 2. The dirty-set rule (what does / doesn't re-measure)

| Change on one node | Re-measures? | Why |
|---|---|---|
| Content-only (e.g. label text), no layout attr | the node's **flex line** only | content can change intrinsic size → its line redistributes |
| `AttrCategory.Layout` attr (size/padding/margin/gap/flex/direction/align) | nearest flex line, up to first fixed-`Size` ancestor | flexbox redistributes across the line (FR-004) |
| Non-layout attr (style/state/**`visualState`**/content-class) | **nothing** | geometry unaffected — paint-only (R1 hover stays paint-only) |
| `ChildInsert` / `ChildRemove` / `ChildMove` on a parent | the parent container | child set changed → container re-measures |
| `Replace` (kind/key changed) | the replaced subtree (as new) | cache entry discarded, full-measured fresh |
| Empty patch (all `Keep`) | **nothing** | cached bounds reused verbatim (identity at rest) |
| Theme-only change | **nothing** (but full **repaint**) | geometry is theme-independent (INV-7) |

The classification reads `attr.Category = AttrCategory.Layout` off the reconcile patch — never a
hand-maintained name list (FR-003).

## 3. The re-measure metric (internal, via the wired host)

On the retained render path, `WorkReductionRecord` now carries `RemeasuredNodeCount` beside the
paint counts. Exercised from `Controls.Tests` (internals reachable via `InternalsVisibleTo`):

```fsharp
// localized leaf edit:
step.WorkReduction.RemeasuredNodeCount < step.WorkReduction.BaselineNodeCount   // true (SC-003)
step.WorkReduction.RecomputedNodeCount < step.WorkReduction.BaselineNodeCount   // paint also reduced (US3)

// root-level layout attr change (genuine whole-tree relayout):
step.WorkReduction.RemeasuredNodeCount = step.WorkReduction.BaselineNodeCount   // true (never under-reports)

// empty patch (frame re-rendered with no model change):
step.WorkReduction.RemeasuredNodeCount = 0                                      // true (identity at rest)
```

## 4. The equivalence invariant (the gate)

```fsharp
// tests/Layout.Tests — FsCheck property (sketch)
property "evaluateIncremental ≡ evaluate over cumulative edits" <| fun (tree, edits) ->
    let mutable prev = Layout.evaluate available (toNode tree)
    let mutable cur  = tree
    edits |> List.forall (fun edit ->
        let next = applyEdit edit cur
        let dirty = dirtyIdsOf edit cur next
        let inc  = Layout.evaluateIncremental prev dirty available (toNode next)
        let full = Layout.evaluate available (toNode next)
        prev <- inc; cur <- next
        inc.Bounds = full.Bounds)        // byte-identical at EVERY step, incl. long sequences
```

- ≥1000 generated `(tree, edit-sequence)` cases (SC-002).
- Cumulative sequences carry the cache forward to stress **cache staleness** — the incremental result
  at step N must equal a from-scratch evaluate at step N.
- Any divergence fails the gate (no "close enough").

## 5. Byte-identity of the rendered output (FR-008)

Every tested frame — localized or whole-tree — produces a `Scene` byte-identical to the pre-R2
full-re-measure build. Verified by structural `Scene` equality (the `SceneEvidence` render functions
are deterministic capability-hash functions, not pixel encoders), not a live screenshot. R2 changes
**work and metrics, never geometry or pixels**.

## What you will NOT see

- No new public type, function, or `LayoutResult` field (SC-006).
- No change to the `view : 'model -> Control<'msg>` consumer contract (FR-009).
- No virtualization, no new layout algorithm (deferred / non-goal).
- No visual difference at all — R2's win is measured, not seen.
