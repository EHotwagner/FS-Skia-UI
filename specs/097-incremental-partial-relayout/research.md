# Phase 0 Research: Incremental Measure / Partial Re-Layout (R2)

All NEEDS CLARIFICATION from the Technical Context are resolved below. Each decision is grounded in
verified source facts (cited inline) and the spec's clarification session (2026-06-11).

## R1 — Where does the genuine incremental evaluator live, and what stays public?

**Decision**: The incremental algorithm lands in the **body** of the already-public
`Layout.evaluateIncremental` (`src/Layout/Layout.fs`), plus a private per-node measure-reuse helper
in the same module. The Controls-side dirty-set derivation, the retained measure cache, the
render-path swap, and the extended `WorkReductionRecord` land in `FS.Skia.UI.Controls`
(`RetainedRender.fs`, `Control.fs`). **No public signature, type, or field is added or moved.**

**Rationale**: `Layout.evaluateIncremental` already has exactly the dirty-set signature the feature
needs — `previous: LayoutResult -> changedNodeIds: LayoutNodeId list -> available -> root ->
LayoutResult` (`src/Layout/Layout.fsi:10`). The spec frames R2 as "make an existing public function
do what its name and signature already promise, and wire it" (spec §Context). Filling the body keeps
the `FS.Skia.UI.Layout` baseline unchanged (SC-006). The cache is a Controls concern (it rides
retained identity, which only exists Controls-side), so it never needs a public `LayoutResult` field.

**Alternatives considered**: (a) A new public `evaluateIncrementalCached` taking an explicit cache —
rejected: adds public surface for no consumer benefit and the cache is identity-keyed, which is
internal. (b) Adding a `Cache` field to `LayoutResult` — rejected: moves the public baseline and
leaks an internal concept (clarification 2026-06-11 explicitly placed the cache **on the retained
node, internal**).

## R2 — What is the layout-dirty set, and how is it derived from the patch?

**Decision**: A node is **layout-dirty** iff, in its `ReconcileResult.Patch` (`NodePatch.Update
UpdatePatch`):
- **(a)** any `AttrChange` in `AttrChanges` is an `AttrSet attr` with `attr.Category =
  AttrCategory.Layout`, **or** an `AttrRemoved name` whose removed attr was `Layout`-category
  (resolved by looking the name up against the **prev** node's attrs, since `AttrRemoved` carries
  only the name); **or**
- **(b)** `Children` contains any `ChildOp` that is `ChildInsert` / `ChildRemove` / `ChildMove`
  (the child set changed → the container must re-measure). `ChildKeep` alone is **not** dirtying.

`NodePatch.Keep` and `NodePatch.Replace` never mark a node measure-dirty: `Keep` is unchanged;
`Replace` is handled as a brand-new subtree (full measure of the replacement, old cache entry
discarded). A content/non-layout `Update` with no dirty descendant is not dirty.

**Rationale**: Driven entirely by the **existing** `AttrCategory.Layout` tag
(`src/Controls/Types.fsi:267`), per FR-003's explicit prohibition on a hand-maintained
attribute-name list that could drift from the category. The patch
(`AttrChange = AttrSet of Attr | AttrRemoved of name`; `ChildOp = ChildKeep | ChildMove |
ChildInsert | ChildRemove`, `src/Controls/Reconcile.fsi`) is a ready-made structural dirty source —
R2 consumes it, re-implementing none of the diff. The dirty set is expressed in the **`LayoutNodeId`
(layout path) domain** that `toLayout "0" control` mints (`src/Controls/Control.fs:1219`), matching
the structural position in the patch, never `ControlId`/`RetainedId`.

**`AttrRemoved` resolution**: because `AttrRemoved` carries only the name, the category is recovered
from the **previous** node's attribute list (which the retained walk already holds via
`pr.Control.Attributes`). A removed `Layout`-category attr (e.g. dropping an explicit `size`) is
dirtying; a removed non-layout attr is not.

**Alternatives considered**: A separate enumerated set of "layout attribute names" — rejected by
FR-003 (drift risk). Treating every `Update` as dirty — rejected: defeats the feature (an R1 hover
restyle is a non-layout `Update` and must stay paint-only, spec edge "Non-layout attribute change").

## R3 — How far does dirt propagate (the conservative flex / fixed-size-ancestor rule)?

**Decision**: Two-stage conservative propagation, per clarification 2026-06-11 and FR-004:
1. **Whole flex line**: a dirty child dirties its **nearest enclosing flex container/line** — every
   sibling on that line is re-measured, because flexbox redistributes free space across the line and
   a per-child re-measure would diverge from a full `evaluate`.
2. **Climb to the first fixed-size ancestor**: the dirt then propagates **upward** until the first
   ancestor whose own `LayoutIntent.Size` is **explicit / content-independent** on the constraining
   axis (a concrete `Some` value, not `None`/auto/content-derived). That ancestor re-measures
   **internally** to redistribute among its children, but because its own box is unchanged, its
   ancestors are **not** dirtied. Propagation **stops** there.
3. **Content-sized chain to root**: if every ancestor up to the root is content-sized (`Size` `None`
   on the constraining axis), the change legitimately resizes the whole chain, so propagation reaches
   the root and the re-measure set is large — the **correct** result, reported honestly by the metric
   (spec edge "Content-sized chain to the root").

**"Fixed-size" precisely**: `LayoutSize = { Width: float option; Height: float option }`
(`src/Layout/Types.fsi:78`). The relevant axis is the **main axis** of the containing flex direction
for size cascades, but conservatively R2 treats an ancestor as fixed (an absorbing boundary) only
when its `Size` is `Some` on the axis along which the child's size change could propagate. When in
doubt (e.g. mixed/ambiguous constraints), the ancestor is treated as **not** fixed → propagation
continues (conservative; never under-dirties). The equivalence invariant (R4) is the backstop that
makes "when in doubt, climb" safe.

**Rationale**: Correctness dominates performance (spec's interacting-requirements note): a
conservatively-widened dirty set yields a *higher* honest re-measure count, never a wrong geometry.
The fixed-size ancestor is exactly the point where a child's size change cannot escape (the
container's own box is pinned), so it is the natural, equivalence-preserving stop.

**Alternatives considered**: Per-node dirtying without the whole-line rule — rejected: diverges from
full evaluate under flex redistribution (acceptance scenario US2.3). Always propagating to the root —
rejected: correct but defeats the speedup; the fixed-size stop is what makes the win real while
staying conservative.

## R4 — How is the equivalence invariant proven (the heaviest budget)?

**Decision**: An FsCheck property suite in `tests/Layout.Tests` (mirrored where needed in
`Controls.Tests` for the patch-derived dirty set) that:
1. Generates a random `LayoutNode` tree (bounded depth/breadth; varied flex directions, fixed vs
   content sizes, padding/gap/flex weights).
2. Generates a random **edit sequence** — attribute changes (layout and non-layout), child inserts,
   removes, moves — applied **cumulatively**.
3. At **each** step applies the edit through both `evaluateIncremental` (carrying the cache forward)
   and a from-scratch full `evaluate`, and asserts the two `LayoutResult.Bounds` sets are
   **byte-identical** (same `NodeId → Bounds` map; `Revision`/`Invalidated` metadata may differ).
4. Runs ≥1000 cases (SC-002), with long sequences specifically to stress **cache staleness** (US2.2):
   carrying the cache across N edits must not drift from the from-scratch result at step N.

The `Bounds` comparison is exact structural equality on `ComputedBounds` (record equality over
`NodeId`, `Bounds`, `Visibility`), so any divergence — including a single sub-pixel difference — fails
the gate. No "close enough" tolerance.

**Rationale**: Incremental flexbox is the classic subtle-bug source (roadmap §10.4); the property is
what makes the fast path adoptable (US2 is equal-priority with US1). Generated (not canned) inputs
satisfy Principle V (no synthetic fixtures). Comparing against the **existing, trusted** full
`evaluate` makes the oracle free and exact.

**Failing-first nuance**: today's stub already returns full-`evaluate` `Bounds`, so the
`Bounds`-equivalence assertion *passes* against the stub. The **decisive** failing-first tests are
therefore the **re-measure-count** assertions (R6) and the **`Invalidated`** assertion (R7), which the
always-full-measure / verbatim-echo stub fails. The equivalence suite is the regression guard that
keeps `Bounds` correct *after* measure is made partial.

**Alternatives considered**: Hand-picked edit scenarios — kept as additional unit cases (fixed-size
stop, content-chain-to-root, each `ChildOp`) but insufficient alone for the cache-staleness exit
criterion. A tolerance-based compare — rejected: FR-008 demands byte-identity.

## R5 — Where does the measure/bounds cache live, and what is its key?

**Decision**: On the **internal** retained node (`RetainedNode`/`RenderFragment`,
`src/Controls/RetainedRender.fsi`), keyed by **retained identity** (`RetainedId`), per clarification
2026-06-11. `RenderFragment` already caches `Box` (the evaluated absolute box); R2 extends the cached
unit to also retain the node's **intrinsic measure** inputs/result so an unchanged subtree can be
reused — or **translated** by an ancestor delta — without recomputation. `evaluateIncremental` reuses
`previous.LayoutResult.Bounds` plus this cache; **no public `LayoutResult` field is added**.

**Purity (FR-002)**: the cache is keyed on the node's content/intrinsic-measure inputs (kind,
content, layout-relevant attrs, available space along the measured axis) — never on wall-clock or
randomness. It lives in the host loop's existing mutable-ref retained state (the interpreter edge),
exactly where the monotonic id counter and work counters already live; nothing mutable escapes
`RetainedRender.step`.

**Ancestor-moved edge (translate, don't re-measure)**: a subtree that did not change but whose
ancestor's layout shifted its origin reuses its **cached intrinsic measure** and has its bounds
**translated** by the ancestor delta — it is not re-measured, only re-positioned. This matches the
existing `box = pr.Fragment.Box` shift handling (`RetainedRender.fs:210`) but without forcing a full
re-measure to discover the shift (spec edge "Ancestor moved, subtree unchanged").

**Rationale**: Retained identity is stable across positional shifts (the whole point of E2), so it is
the correct cache key — a `ControlId`-keyed cache would miss on a sibling reorder (the bug 092 fixed
for state). Keeping the cache internal preserves the public baseline (SC-006).

**Alternatives considered**: A `LayoutNodeId`-keyed cache — rejected: the layout id is path-derived
and unstable across shifts, so it would falsely miss exactly when reuse matters most. A public cache
type — rejected (clarification: internal, on the retained node).

## R6 — How is the re-measure metric reported (and what makes it honest)?

**Decision**: Extend the **internal** `WorkReductionRecord` (`src/Controls/RetainedRender.fsi:70`)
with a **re-measured node count** field alongside `BaselineNodeCount` / `RecomputedNodeCount` /
`ChangedSubtreeBound` / `ShiftedNodeCount`. `RetainedRender.step` counts re-measured nodes at the
interpreter edge (a `mutable` counter beside the existing `recomputed`/`shifted`, with a one-line
disclosure comment per constitution III). Invariants (FR-006):
- localized update → re-measure count **strictly below** `BaselineNodeCount`, consistent with the
  dirty flex-line subtree;
- genuine whole-tree relayout → re-measure count **equals** `BaselineNodeCount` (never under-reports);
- empty (all-`Keep`) patch → re-measure count **zero**.

**Rationale**: Makes the partial-measure exit criterion measurable and regression-proof (a future
change that accidentally re-measures the whole tree shows up). The metric reports the **actual**
re-measured set produced by conservative propagation (spec's FR-006-vs-FR-004 resolution), so a
conservatively-widened dirty set shows a higher (honest) count, not a theoretical minimum.

**Field naming**: follows the existing record's `...NodeCount` convention (e.g. `RemeasuredNodeCount`)
— final name fixed in `data-model.md`. The field is **internal** (the whole record is `type
internal`), so it does not move the public baseline.

**Alternatives considered**: Reusing `RecomputedNodeCount` for measure — rejected: that counts
**paint** recomputes; measure and paint must be separately observable (US3: "reduces both"). A new
public metric type — rejected: internal suffices and avoids a baseline move.

## R7 — What does `LayoutResult.Invalidated` report after R2?

**Decision**: The **actual re-measured set** — the requested `changedNodeIds` **after** conservative
flex-line + fixed-size-ancestor propagation (FR-004), i.e. the nodes genuinely re-measured this call —
replacing today's verbatim echo (`changedNodeIds |> List.distinct`). `Revision` continues to advance
(`previous.Revision + 1`). Only `Bounds` are constrained to byte-identity with full `evaluate`;
`Invalidated`/`Revision` are incremental metadata and intentionally differ from a from-scratch
evaluate (FR-001a).

**Rationale**: Clarification 2026-06-11 — `Invalidated` should be the honest post-propagation set, not
the input echo. This makes the field a usable diagnostic (a caller can see what actually re-measured)
and gives SC-008 a concrete assertion (localized edit → `Invalidated` ⊋ the single requested node,
bounded by the fixed-size-ancestor subtree; empty patch → `Invalidated` empty).

**Alternatives considered**: Keep echoing the input — rejected by clarification (dishonest once measure
is partial). Report the full tree — rejected: wrong (over-reports) and useless as a diagnostic.

## R8 — How is the render-path swap done without breaking E2?

**Decision**: Add a **new internal incremental variant** of `ControlInternals.evaluateLayout`
(`src/Controls/Control.fs:1219`) that takes the previous `LayoutResult` + measure cache + dirty set
and calls `Layout.evaluateIncremental` instead of the unconditional full `Layout.evaluate`, returning
the same `root, boundsById` shape `step` already consumes. `RetainedRender.step`
(`src/Controls/RetainedRender.fs:141`) calls this variant, threading the previous frame's
`LayoutResult`/cache (carried in the retained state alongside `Root`/`NextId`/`StateByIdentity`/
`Theme`) and the patch-derived dirty set. The existing reuse-driven paint walk
(`box = pr.Fragment.Box`, `carry`/`build`) is **unchanged** — it still keys paint reuse on the box,
now sourced from the incremental `boundsById` (byte-identical to the full one, FR-008).

**First-frame / cache-miss**: the first frame has no prior `LayoutResult`, so the variant runs a full
`evaluate` and seeds the cache; incremental reuse begins on frame 2 (same shape as E2's first-frame
full paint, spec edge "Cache miss / first frame").

**Theme change**: `themeChanged` (`RetainedRender.fs:144`) still forces a full **repaint**, but does
**not** dirty **measure** — geometry is theme-independent, so cached bounds are reused across a
theme-only change (spec edge "Theme change"); the equivalence invariant must (and does) hold across
theme changes.

**Rationale**: Threading the previous `LayoutResult` through the existing retained state keeps the
swap a localized seam change; reusing the `root, boundsById` return shape means the paint walk and all
E2 invariants (`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`,
first-frame full paint, `KeyCollision`) are preserved by construction (SC-007).

**Alternatives considered**: Replacing `evaluateLayout` in place (no separate variant) — rejected: the
full variant is still needed for the first frame and for non-retained callers (e.g.
`Control.render`/preview), so a parallel incremental seam is cleaner and lower-risk.

## Resolved unknowns summary

| Unknown | Resolution |
|---|---|
| Where incremental algo lives | `Layout.evaluateIncremental` **body** (public sig unchanged) + Controls-side cache/derivation (R1) |
| Dirty-set source | `ReconcileResult.Patch`: `AttrCategory.Layout` attr change **or** any non-`Keep` `ChildOp` (R2) |
| Propagation rule | Whole flex line → climb to first fixed-`Size` ancestor → stop; content-chain reaches root (R3) |
| Equivalence proof | FsCheck ≥1000 `(tree, cumulative edit-seq)`; exact `Bounds` equality vs full `evaluate` (R4) |
| Cache location/key | Internal `RenderFragment`/`RetainedNode`, keyed by `RetainedId`; translate on ancestor shift (R5) |
| Metric | Internal `WorkReductionRecord` + re-measured count; localized < baseline, whole-tree = baseline, empty = 0 (R6) |
| `Invalidated` value | Honest post-propagation re-measured set, not input echo; `Revision` still advances (R7) |
| Render-path swap | New internal incremental `evaluateLayout` seam; first-frame/theme-change full; paint walk unchanged (R8) |

No NEEDS CLARIFICATION remains.
