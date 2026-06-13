# Layout-invalidated-count authority (feature 117) — spec-direction correction

The authoritative definition of `LayoutInvalidatedNodeCount` and the **correction** to FR-006/SC-006/
contract L1's asserted direction, approved during implementation.

## Definition

`LayoutInvalidatedNodeCount = Set.count` of the **pre-pinning** dirty set fed into incremental layout —
`layoutDirtySet` (`RetainedRender.fs`), the patch-derived self-dirty nodes (attr/child/replace changes),
threaded into `WorkReductionRecord` distinct from `RemeasuredNodeCount`.

`RemeasuredNodeCount = layoutResult.Invalidated.Length` — the **post-pinning** set
`Layout.evaluateIncremental` actually re-measured: each dirty node is propagated up to its first
fixed-size ancestor and that ancestor's WHOLE subtree is re-laid-out (`Layout.fs:646-719`).

## The corrected relationship: `LayoutInvalidatedNodeCount <= RemeasuredNodeCount`

The spec (FR-006/SC-006/contract L1) originally asserted `LayoutInvalidatedNodeCount >=
RemeasuredNodeCount`, on the mental model that the dirty set is large and fixed-size-ancestor pinning
*reduces* it. **In this codebase that direction is reversed**: `layoutDirtySet` returns only the small
self-dirty set, and `evaluateIncremental` *expands* it into the boundary subtree. So the pre-pinning
dirty set is a SUBSET of the re-measured boundary subtrees, and the honest, code-guaranteed relationship
is `LayoutInvalidatedNodeCount <= RemeasuredNodeCount`.

This was surfaced as a spec-vs-code conflict and resolved (maintainer decision, 2026-06-13) by **flipping
the contract to the honest `<=` direction** — the relationship the framework actually guarantees. The
field is still distinct from `RemeasuredNodeCount`, still `0` on idle / style-only / visual-state-only
frames (empty dirty set), and still bounded and explainable on a geometry frame. The amendment is recorded
in spec.md FR-006/SC-006, the layout-invalidated metric contract, and tasks.md.

Why `<=` always holds: pinning + propagation can only ADD nodes to the re-measured set relative to the
self-dirty set (a changed leaf drags in its boundary subtree's siblings/ancestors), never remove the
changed node itself, so `|dirty| <= |Invalidated|`.

## Zero on idle / style-only / visual-state-only (FR-006/FR-007)

A hover/focus/press/animation-tick or a pure style change touches no `layoutAffectingAttrNames`
(`{ width, height, orientation }` only), so `layoutDirtySet` is empty → `LayoutInvalidatedNodeCount = 0`
and `RemeasuredNodeCount = 0`. The feature-101 drift guard's attribute set is unchanged (FR-008) — no new
geometry-driving attribute is added.

## Evidence

`tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs` (idle = 0; style-only = 0/0; geometry bounded
with `invalidated <= remeasured`; drift-guard set unchanged), `tests/Elmish.Tests/Feature117MetricsTests.fs`
(the geometry-frame `<=` assertion + idle all-zero over `Perf.runScript`).
