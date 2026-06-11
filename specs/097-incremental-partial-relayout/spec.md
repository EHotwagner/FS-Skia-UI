# Feature Specification: Incremental Measure / Partial Re-Layout

**Feature Branch**: `097-incremental-partial-relayout`
**Created**: 2026-06-11
**Status**: Draft
**Input**: User description: "create the next part" of `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md` — the controls architecture-evolution roadmap. Steps E1–E5 (features 090, 091+092, 093, 094, 095) have landed; the roadmap's §10 post-implementation audit then defines five live-path remediation features **R1–R5** with the recommended order **R1 → {R3, R2} → R4 → R5**. R1 (the runtime visual-state bridge) shipped as feature 096. This feature is **R2 — incremental measure / partial re-layout** (roadmap §10.4): it makes per-frame *layout* work proportional to what changed, finishing E2's partial-update promise (today only *paint* is partial, while *measure* is still O(whole-tree) every frame).

## Context & Motivation *(informative)*

E2 (features 091 + 092) wired the keyed reconciler onto the live render path: a per-frame diff produces a `ReconcileResult.Patch`, the retained walk reuses unchanged painted subtrees, and `WorkReductionRecord` proves that **paint** is partial (`RecomputedNodeCount < BaselineNodeCount` for a localized change). But **layout was never made partial**. `RetainedRender.step` calls full-tree `ControlInternals.evaluateLayout size next` on **every** frame (`src/Controls/RetainedRender.fs:141`), which always runs full `Layout.evaluate` (`src/Controls/Control.fs:1219`, `src/Layout/Layout.fs:502`). The paint-reuse decision even *depends* on that full re-measure: a subtree is reused only when `box = pr.Fragment.Box` (`src/Controls/RetainedRender.fs:210`), i.e. after the whole tree has been re-measured to recompute every box. So measure stays O(whole-tree) and `WorkReductionRecord` counts paint-node recomputes only — E2's FR-004 "only the changed subtree is re-measured" is unmet (roadmap §10.1).

The framework already carries the *affordances* for incremental layout but never populates them:

- `Layout.evaluateIncremental` exists in the **public** surface (`src/Layout/Layout.fsi:10`) with the dirty-set signature `previous: LayoutResult -> changedNodeIds: LayoutNodeId list -> available -> root -> LayoutResult`, **but its body is a stub** (`src/Layout/Layout.fs:540`) that calls full `evaluate` and merely stamps `Revision + 1` and an `Invalidated` list. It re-measures everything; the `changedNodeIds` argument changes only the reported metadata, not the work done.
- `LayoutResult` already has `Revision` and `Invalidated` fields (`src/Layout/Types.fsi:171`–`172`) — designed for incremental recompute but never driven from real reuse.
- The reconcile patch already classifies attributes: every `Attr` carries an `AttrCategory` (`src/Controls/Types.fsi:267`), and `AttrCategory.Layout` is exactly the set of changes that can move geometry. The patch's `UpdatePatch.AttrChanges`/`Children` (`src/Controls/Reconcile.fsi:34`–`48`) are therefore a ready-made dirty source.

The consequence is that the roadmap's E2 promise — "partial-update performance" — is delivered today for *paint* but not for *measure*: a single hover-driven leaf change (now produced live by R1) still pays a full-tree re-measure every frame. R2 makes layout work proportional to the patch: it turns the stub `evaluateIncremental` into a genuine incremental evaluator backed by a per-node measure cache on the retained tree, derives the dirty set from the reconcile patch (with conservative flex-line propagation), and swaps it onto the render path — while a hard equivalence invariant guarantees the incremental result is **byte-identical** to a full `Layout.evaluate`. It is architecture-preserving and non-goal-preserving: it introduces no virtualization, no new layout algorithm, and no new public layout type — it makes an existing public function do what its name and signature already promise, and wires it.

## Clarifications

### Session 2026-06-11

- Q: In a uniformly-flex tree, how far up should the dirty set propagate / re-measure climb when a
  child's size change cascades? → A: **Stop at the first fixed-size ancestor.** Dirt propagates up
  until the first ancestor whose own `Size` is **explicit / content-independent** (a concrete
  `LayoutIntent.Size`, not auto/content-derived). That ancestor re-measures internally to
  redistribute among its children, but because its own box is unchanged, its ancestors stay clean.
  This maximizes reuse while preserving the equivalence invariant. (The whole-flex-line rule still
  holds *within* each re-measured container — a dirty flex child dirties its whole line.)
- Q: Where should the per-node measure/bounds cache live, and what is the public-surface posture? →
  A: **On the retained node, internal.** The cache rides the internal `RetainedNode` (Controls
  side), keyed by retained identity; `evaluateIncremental` reuses `LayoutResult.Bounds` plus that
  cache. **No public `LayoutResult` field is added and the `evaluateIncremental` signature is
  preserved**, so the `FS.Skia.UI.Layout` surface baseline does not move.
- Q: After R2 makes it real, what should the public `LayoutResult.Invalidated` field report (today
  the stub echoes the requested `changedNodeIds`)? → A: **The actual re-measured set** — the
  requested dirty set **after** conservative flex-line / fixed-size-ancestor propagation, i.e. the
  nodes genuinely re-measured this call. This changes the observable value from today's verbatim
  echo of the input to the honest post-propagation set.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - A localized edit re-measures only its subtree, not the whole tree (Priority: P1)

A consumer runs an app whose model changes one localized thing per frame — a counter label deep in a panel, or (via R1) a single control's hover state. The host re-renders. Today every such frame re-measures the entire control tree; with R2, the host re-measures **only the affected flex container subtree** and reuses the cached bounds of everything else. The visible output is unchanged; the per-frame layout work drops from O(whole-tree) to O(changed-subtree).

**Why this priority**: This is the headline R2 capability and the direct fix for the audit finding that E2's partial-update promise covers paint but not measure. Without it, every live frame — including every R1 hover restyle — pays a full re-measure, so the "partial update" claim is only half true.

**Independent Test**: Render two frames whose patch touches a single leaf (a content-only change with no layout-affecting attribute and no child reorder under it). Assert, via the extended `WorkReductionRecord`, that the count of **re-measured** nodes equals the changed leaf's enclosing flex-line subtree, not the baseline node count, and that the resolved `Scene` is byte-identical to a full-rebuild frame.

**Acceptance Scenarios**:

1. **Given** a retained tree and a next frame whose only change is one leaf's content (no `AttrCategory.Layout` change, no child op), **When** the host renders, **Then** only that leaf's enclosing flex container/line is re-measured and every other subtree reuses its cached bounds.
2. **Given** a change to a layout-affecting attribute (size/padding/margin/gap/flex/direction/align) on one node, **When** the host renders, **Then** that node's nearest flex container/line is re-measured (because flexbox redistributes across the line) and ancestors above the absorbing container are not.
3. **Given** a localized change anywhere in the tree, **When** the host renders, **Then** the produced `Scene` is byte-identical to the `Scene` produced by a full `Layout.evaluate` for the same frame.

---

### User Story 2 - Incremental layout is provably identical to full layout under any edit sequence (Priority: P1)

A maintainer must trust that the fast path never diverges from the slow path. For any sequence of edits — attribute changes, insertions, removals, moves, in any order — the incremental evaluator produces exactly the bounds a from-scratch full evaluation would. There is no "close enough": divergence is a gate failure.

**Why this priority**: Incremental flexbox is the classic subtle-bug source (roadmap §10.4 risks). The equivalence invariant is what makes the whole feature safe to wire; it is equal-priority with US1 because without the proof, the speedup is not adoptable. It is the determinism exit criterion the roadmap calls the "heaviest budget".

**Independent Test**: A property test generates random control trees and random edit sequences, applies each edit both through `evaluateIncremental` (carrying the cache) and through full `evaluate`, and asserts the two `LayoutResult` bounds sets are byte-identical at every step (the `Revision`/`Invalidated` metadata may differ; the computed geometry may not).

**Acceptance Scenarios**:

1. **Given** any generated tree and any single edit, **When** both evaluators run, **Then** their computed bounds are byte-identical.
2. **Given** a random sequence of N edits applied cumulatively, **When** the incremental cache is carried across all N and compared to a full evaluate at each step, **Then** they remain byte-identical for the whole sequence (no cache staleness drift).
3. **Given** an edit that dirties a flex child, **When** the incremental evaluator runs, **Then** it re-measures the whole flex line (conservative propagation) and the result matches full evaluate even though sibling space was redistributed.

---

### User Story 3 - The partial-layout speedup is measured and reported, not assumed (Priority: P2)

A maintainer reviewing a change can see, in the same metric that already proves partial paint, that partial **measure** also happened. `WorkReductionRecord` reports re-measured node counts alongside re-painted node counts, so a localized update visibly reduces both, and a whole-tree change visibly reduces neither.

**Why this priority**: It makes the exit criterion measurable and prevents silent regression (a future change that accidentally re-measures the whole tree would show up in the metric). It is lower urgency than the behavior and the equivalence proof, but it is what makes the win durable and reviewable.

**Independent Test**: For a localized edit, assert the extended `WorkReductionRecord` reports a re-measure count strictly below the baseline node count and consistent with the dirty flex-line subtree; for a root-level layout change, assert the re-measure count equals the baseline (no false reduction claimed).

**Acceptance Scenarios**:

1. **Given** a localized leaf edit, **When** the host renders, **Then** `WorkReductionRecord` reports both a re-measure reduction and a re-paint reduction for that frame.
2. **Given** a change to a root-level layout attribute that genuinely relays the whole tree, **When** the host renders, **Then** the re-measure count equals the baseline node count (the metric does not under-report real work).
3. **Given** an at-rest frame (empty patch), **When** the host renders, **Then** the re-measure count is zero (cached bounds reused wholesale), matching the existing `Keep → reuse` paint fast path.

---

### Edge Cases

- **Empty patch (identity at rest)**: a frame whose patch is all-`Keep` re-measures nothing — the cached `LayoutResult`/bounds are reused verbatim, and the output stays byte-identical to the un-incremental build (preserving E2's `Keep → reuse` paint fast path).
- **Ancestor moved, subtree unchanged**: a subtree that did not change but whose ancestor's layout shifted its origin reuses its **cached intrinsic measure** but has its bounds **translated** by the ancestor delta — it is not re-measured, only re-positioned (matching the existing `box = pr.Fragment.Box` shift handling without a full re-measure).
- **Flex sibling redistribution**: dirtying one flex child must dirty its **whole flex container/line**, because flexbox redistributes free space across the line; per-node dirtying would diverge from full evaluate. Dirt then climbs to the first ancestor whose own `Size` is explicit/content-independent (that ancestor redistributes internally without changing its own box), and stops there.
- **Content-sized chain to the root**: if every ancestor of a changed node is content-sized (no fixed `Size`), the change legitimately resizes the whole chain, so propagation reaches the root and the re-measure set is large — this is the *correct* result, not a propagation failure, and the metric reports it honestly (no false reduction).
- **Child insert/remove/move**: any `ChildOp` (`ChildInsert`/`ChildRemove`/`ChildMove`) dirties the parent container (the child set changed, so the container must re-measure), independent of attribute changes.
- **Non-layout attribute change**: an `AttrChange` whose attribute `Category` is not `Layout` (e.g. a `Content`, `Style`, `State`, or R1 `visualState` change) does **not**, on its own, dirty measure — it can repaint without re-measuring, so an R1 hover restyle stays paint-only unless it also changes geometry.
- **Replace patch**: a `NodePatch.Replace` (kind/key changed → a different node) re-measures the replaced subtree as new; its cache entry is discarded, not reused under the old identity.
- **Cache miss / first frame**: the first rendered frame has no prior `LayoutResult` to reuse, so it runs a full `evaluate` and seeds the cache; incremental reuse begins on frame 2 (the same shape as E2's first-frame full paint).
- **Theme change**: a per-frame theme change already forces a full repaint (`themeChanged`, `src/Controls/RetainedRender.fs:144`). Theme is a paint concern, not a measure concern; a theme-only change does **not** dirty measure (geometry is theme-independent), so cached bounds are still reused — but the equivalence invariant must hold across theme changes too.
- **Node-id domain**: the dirty set is expressed in the **`LayoutNodeId` (layout path) domain** that `toLayout "0" control` mints (`src/Controls/Control.fs:1219`), the same domain `evaluateIncremental` and `LayoutResult.Bounds` already use — derived from the reconcile patch's structural position, never the `ControlId`/`RetainedId` identity domains.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: `Layout.evaluateIncremental` MUST become a **genuine incremental evaluator**: given a previous `LayoutResult`, a dirty set `changedNodeIds`, an available space, and the new root, it MUST re-measure **only** the dirty nodes and their conservatively-propagated flex containers, reuse cached measures/bounds for everything else, and return a `LayoutResult` whose computed `Bounds` are **byte-identical** to a full `Layout.evaluate available root`. Today's stub (full `evaluate` + metadata stamp) is replaced by real reuse. The existing **public signature is preserved** (it already takes the dirty set), so this is a behavior change to an existing public function, not a new public surface.
- **FR-001a**: The returned `LayoutResult.Invalidated` field MUST report the **actual re-measured set** — the requested `changedNodeIds` **after** conservative flex-line and fixed-size-ancestor propagation (FR-004), i.e. the nodes genuinely re-measured this call — replacing today's stub behavior of echoing the input verbatim. `Revision` MUST continue to advance (`previous.Revision + 1`). Only the `Bounds` are constrained to byte-identity with full `evaluate`; `Invalidated`/`Revision` are incremental metadata and intentionally differ from a from-scratch evaluate.
- **FR-002**: The system MUST maintain a **per-node measure/bounds cache** on the retained tree, **lookup-keyed by retained identity** (`RetainedId`, stable across positional shifts), so that an unchanged subtree's intrinsic measurement and computed bounds survive across frames and can be reused (translated if an ancestor moved) without recomputation. The cache MUST be **pure** — its **validity is a function of the node's content/intrinsic-measure inputs** (kind, content, layout-relevant attrs, available space on the measured axis) — with no wall-clock, randomness, or mutation escaping the per-step interpreter edge (constitution III). (Lookup index and validity predicate are distinct: entries are *found* by `RetainedId` and *invalidated* by changed measure inputs.)
- **FR-003**: The system MUST derive the **layout-dirty set directly from `ReconcileResult.Patch`**. A node is layout-dirty if (a) its `UpdatePatch.AttrChanges` set or removes an attribute whose `Category` is `AttrCategory.Layout` (size/min/max, padding/margin/gap, flex grow/shrink/basis, direction/wrap/align/justify), or (b) it carries any `ChildOp` (`ChildInsert`/`ChildRemove`/`ChildMove`). A `Keep`, a `Replace` (handled as new), or a content/non-layout-`Update` with no dirty descendant MUST NOT mark a node measure-dirty. The classification MUST be driven by the existing `AttrCategory.Layout` tag — it MUST NOT hard-code a separate, hand-maintained attribute-name list that could drift from the category.
- **FR-004**: Dirty propagation across flex MUST be **conservative at the flex-line granularity** and MUST climb until a node's size stops changing: dirtying one child MUST dirty its **whole nearest flex container/line** (flexbox redistributes free space across the line, so a per-child re-measure would diverge from a full evaluate), and the dirt MUST then propagate **upward** to the **first ancestor whose own `Size` is explicit / content-independent** (a concrete `LayoutIntent.Size`, not auto/content-derived). That fixed-size ancestor re-measures internally to redistribute among its children, but because its own box is unchanged, its ancestors are NOT dirtied. Propagation MUST stop there and climb no further. If no fixed-size ancestor exists between the change and the root (every ancestor is content-sized), propagation reaches the root — that is the correct, not the degenerate, result.
- **FR-005**: `RetainedRender.step` MUST drive layout through the incremental evaluator instead of the unconditional full `evaluateLayout` (`src/Controls/RetainedRender.fs:141`), threading the previous `LayoutResult`/cache and the patch-derived dirty set, while preserving the existing reuse-driven paint walk (the `box = pr.Fragment.Box` shift handling) and every E2 determinism invariant. A new internal incremental variant of `ControlInternals.evaluateLayout` (`src/Controls/Control.fs:1219`) MUST provide this seam.
- **FR-006**: `WorkReductionRecord` MUST be extended to count **re-measured nodes** alongside the existing re-painted (`RecomputedNodeCount`) and shifted (`ShiftedNodeCount`) counts, so the partial-measure exit criterion is measurable. For a localized update the re-measure count MUST be strictly below the baseline node count and consistent with the dirty flex-line subtree; for a genuine whole-tree relayout it MUST equal the baseline (the metric never under-reports real work); for an empty patch it MUST be zero.
- **FR-007**: The feature MUST add the **strongest possible determinism gate**: a property test asserting `evaluateIncremental` (carrying its cache) is **byte-identical to a full `Layout.evaluate`** over randomized trees and randomized cumulative edit sequences (attribute changes, inserts, removes, moves). Any divergence — including cache-staleness drift across a long sequence — MUST fail the gate.
- **FR-008**: Output rendering MUST remain **byte-identical** to the pre-R2 (full-re-measure) build for **every** frame, localized or not. Incremental layout is a performance-and-metric change only; it MUST NOT alter any computed bound, scene, or pixel. (Identity-at-rest and partial-paint behavior from E2 are preserved unchanged.)
- **FR-009**: The feature MUST be **additive and non-goal-preserving**: it introduces no virtualization (deferred per §6.2), no new layout algorithm, no new public layout type, and no change to the `view : 'model -> Control<'msg>` consumer contract. The only public surface touched is the **behavior** of the already-public `evaluateIncremental` (signature unchanged); no data-binding, observable, dependency/attached-property, lookless-template, or CSS-selector capability is introduced (permanent roadmap non-goals).

> Interacting / conflicting requirements: FR-001/FR-008 (incremental result byte-identical to full evaluate) vs FR-004 (re-measure only the dirty flex line) — resolution: correctness dominates performance. The dirty set is always taken **conservatively** (whole flex line, propagate up to the absorbing ancestor); when in doubt a node is treated as dirty and re-measured, never reused, so the equivalence invariant can never be traded away for a smaller re-measure count. FR-006 (re-measure metric) vs FR-004 (conservative propagation) — resolution: the metric reports the **actual** re-measured set produced by conservative propagation, so a conservatively-widened dirty set shows a *higher* (honest) re-measure count rather than the theoretical minimum; the metric measures real work done, not an idealized lower bound.

### Framework Governance Prompts *(mandatory)*

> **Exempt from the "no implementation details" rule (feature 085, FR-014).** Unlike the rest of the
> spec, this section is *expected* to name concrete packages, `.fsi` signatures, build targets,
> effects, and evidence paths — that is its purpose.

- **Package impact**: No package identity or set changes. The genuine incremental evaluator lands in the existing **`FS.Skia.UI.Layout`** package (`src/Layout/Layout.fs` `evaluateIncremental` body + a per-node measure-reuse helper). The dirty-set derivation, the retained measure cache, the render-path swap, and the extended `WorkReductionRecord` land in the existing **`FS.Skia.UI.Controls`** package (`src/Controls/RetainedRender.fs`, `src/Controls/Control.fs`). No DTCG token, no new control type. All packable libraries are version-bumped and template pins refreshed on merge per the standard flow.
- **Public contract impact**: This feature **preserves** public surface. `Layout.evaluateIncremental` is **already** in `src/Layout/Layout.fsi` with the dirty-set signature R2 needs; R2 changes only its **body** (stub → real), so the `FS.Skia.UI.Layout` signature baseline is **unchanged**. `LayoutResult` already carries `Revision`/`Invalidated` (no field added). The retained measure cache and the extended `WorkReductionRecord` are **internal** (`RetainedRender.fs` types are `internal`). If the equivalence/caching work turns out to require a public signature or `LayoutResult` field change, that escalates to a surface-baseline recapture; the design intent (cache on the retained node, reuse `LayoutResult.Bounds`) is to **avoid** any public change.
- **State workflow impact**: None to the consumer state model. The incremental evaluator and the measure cache are pure functions of the previous `LayoutResult`, the patch-derived dirty set, and the new tree; they own no mutable state beyond the per-step interpreter-edge counters/cache already confined in `RetainedRender.step`. No new effect, command, subscription, or interpreter behavior. The existing `LayoutWorkflowModel`/`Msg`/`Effect` surface is untouched.
- **Layout/rendering impact**: This is a **layout** feature. Rendering output MUST be byte-identical to the full-re-measure build for every frame (FR-008); only the *amount of measure work* and the reported metric change. No new Skia/Vulkan surface. Evidence is deterministic structural `Scene`/bounds equality (the SceneEvidence render functions are deterministic capability-hash functions, not pixel encoders) plus the property-test equivalence proof; no live-window screenshot is required for the equivalence/metric claims.
- **Evidence obligations**: Real, in-repo readiness artifacts under `specs/097-incremental-partial-relayout/readiness/` proving: (a) a localized leaf edit re-measures only its enclosing flex-line subtree, reported via the extended `WorkReductionRecord`, with a byte-identical `Scene` vs full rebuild (US1); (b) `evaluateIncremental` is byte-identical to full `evaluate` over randomized trees and cumulative edit sequences, property-tested for the equivalence invariant including long-sequence cache-staleness (US2/FR-007); (c) `WorkReductionRecord` reports both re-measure and re-paint reductions for a localized update, equals baseline for a whole-tree relayout, and is zero for an empty patch (US3/FR-006); (d) an at-rest frame stays `Scene`-byte-identical and re-measures nothing (FR-008); (e) all E2 determinism invariants still hold on the wired path.
- **Unsupported scope**: Out of scope — virtualization / windowing of large collections (a later layer per §6.2), the runtime visual-state bridge (R1, shipped as 096), binding-aware unkeyed dispatch (R3), the live animation clock and animated transitions (R4), general navigation-key delivery (R5); any new layout algorithm or new public layout type; any change to computed geometry (R2 is performance-and-metric-only). CSS selectors, attached/dependency properties, lookless templates, data binding remain permanent non-goals.
- **Build-target impact**: Run `Route` first and run only the gates it prints. The change touches `src/Layout/**` and `src/Controls/**`; the `evaluateIncremental` **body** change does not move the `.fsi` signature, so if no public signature changes, the change routes to the lighter inner-loop tier plus the layout/controls determinism tests. If any `.fsi` does change, it escalates to the serialized `Dev → GeneratedGuidanceCheck → TemplateCheck → GeneratedProductCheck → EvidenceGraph → EvidenceAudit` path. No new gate is added; the equivalence property test is added to the existing Layout/Controls test projects.

## Success Criteria *(mandatory)*

- **SC-001**: A localized leaf edit re-measures **only its enclosing flex container/line subtree** — reported via the extended `WorkReductionRecord` re-measure count — not the whole tree, while producing a `Scene` byte-identical to a full-rebuild frame.
- **SC-002**: `evaluateIncremental` (carrying its cache) is **byte-identical** to a full `Layout.evaluate` across at least 1000 generated `(tree, edit-sequence)` cases — including cumulative multi-edit sequences that stress cache staleness — with zero divergences; any divergence fails the gate.
- **SC-003**: `WorkReductionRecord` reports a **re-measure count** alongside the existing re-paint count, such that a localized update shows a re-measure count strictly below baseline, a genuine whole-tree relayout shows a re-measure count equal to baseline, and an empty patch shows a re-measure count of zero.
- **SC-004**: A change to a layout-affecting attribute (an `AttrCategory.Layout` attr) dirties the nearest flex line and propagates **up to (and including) the first fixed-size ancestor, stopping there** — verified that a subtree under a fixed-`Size` container does not dirty that container's ancestors, and that a fully content-sized chain dirties up to the root. A change to a non-layout attribute (content/style/state/`visualState`) dirties **no** measure work. Both cases verified against the dirty-derivation.
- **SC-008**: After an incremental call, `LayoutResult.Invalidated` reports the **actual re-measured set** (post flex-line/fixed-size-ancestor propagation), not the verbatim requested dirty set — verified for a localized edit (Invalidated ⊋ the single requested node, bounded by the fixed-size-ancestor subtree) and for an empty patch (Invalidated empty).
- **SC-005**: The full per-frame render output remains **byte-identical** to the pre-R2 full-re-measure build for every tested frame, localized or whole-tree — R2 changes work and metrics, never geometry or pixels.
- **SC-006**: The public `FS.Skia.UI.Layout` surface baseline is **unchanged** (the `evaluateIncremental` signature and `LayoutResult` shape are preserved); the measure cache and extended `WorkReductionRecord` remain internal.
- **SC-007**: All E2 determinism invariants (`RecomputedNodeCount = ChangedSubtreeBound + ShiftedNodeCount`, `Keep → reuse`, first-frame full paint, KeyCollision diagnostics) continue to hold on the incremental-layout-wired path, demonstrated on the live render seam.

## Assumptions

- E2 (features 091 + 092) has landed: the keyed reconciler is on the live render path (`RetainedRender.step`), produces a `ReconcileResult.Patch` per frame, and the retained walk already reuses unchanged painted subtrees — R2 consumes this patch and identity scheme and re-implements none of it.
- The existing public `Layout.evaluateIncremental` signature (`previous -> changedNodeIds -> available -> root -> LayoutResult`) is the right shape for genuine incremental layout — it already takes the dirty set — so R2 fills in its body rather than adding a new public function; `LayoutResult.Revision`/`Invalidated` are the intended incremental metadata.
- The memoized measure/bounds cache lives on the **retained node** (Controls-side, internal), keyed by retained identity, so no field needs to be added to the public `LayoutResult`; the incremental evaluator reuses `LayoutResult.Bounds` plus the retained cache.
- `AttrCategory.Layout` is the authoritative classifier of layout-affecting attributes; R2 derives the dirty set from the category on the patch's `AttrChanges`, not from a separately-maintained attribute-name list.
- Flexbox is the layout model whose cross-sibling space redistribution drives the conservative whole-line dirtying rule; the re-arrange unit is the nearest flex container that absorbs the change.
- Per the architecture-evolution decision, this is incremental MVU-core evolution toward declarative-retained parity (the R-series finishing the E-series live path), not a redesign; no virtualization, new layout algorithm, or property-system surface is introduced.
- R2 is independent of R1/R3/R4/R5 (roadmap §10.8): it neither depends on nor blocks the visual-state bridge (R1/096), binding-aware recovery (R3), the animation clock (R4), or general navigation (R5) — it shares only the E2 retained tree and patch as its dirty source.

## Key Entities

- **Reconcile patch**: the per-frame `ReconcileResult.Patch` (the `NodePatch`/`UpdatePatch`/`ChildOp` tree) — the authoritative source from which the layout-dirty set is derived.
- **Layout-dirty set**: the set of `LayoutNodeId`s that must be re-measured this frame — nodes whose patch sets an `AttrCategory.Layout` attribute or carries a child op, conservatively widened to the nearest absorbing flex container/line.
- **Retained measure cache**: the per-node memoized intrinsic measure and computed bounds, keyed by retained identity, that lets an unchanged subtree reuse (or translate) its geometry without recomputation.
- **Incremental evaluator**: `Layout.evaluateIncremental` made genuine — re-measures only the dirty set and reuses cached bounds for the rest, returning a `LayoutResult` byte-identical to a full `evaluate`.
- **Work-reduction record**: `WorkReductionRecord` extended to count re-measured nodes alongside re-painted and shifted nodes, making the partial-measure win measurable and regression-proof.
- **Equivalence invariant**: the hard property `evaluateIncremental ≡ evaluate` (computed bounds) over random trees and edit sequences — the gate that makes the fast path adoptable.
