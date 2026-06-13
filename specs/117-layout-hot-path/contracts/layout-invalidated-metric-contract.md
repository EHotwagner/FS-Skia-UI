# Contract — Layout-Invalidated Node Count + Style-Only Zero-Work (Feature 117)

Covers FR-006, FR-007, FR-008, FR-009. Reporting-only; reduces no work itself.

## L1 — `LayoutInvalidatedNodeCount` is the pre-pinning dirty set (FR-006)

- The new public `FrameMetrics.LayoutInvalidatedNodeCount` is the **size of the dirty set fed
  into incremental layout** — `Set.count` of `layoutDirtySet` (`RetainedRender.fs:497-504`),
  the set passed to `Layout.evaluateIncremental` (`Control.fs:1307`, `Set.toList dirty`).
- It is **distinct** from `RemeasuredNodeCount`, which is the **post-pinning** re-measured set
  (`RetainedRender.fs:575` = `layoutResult.Invalidated |> List.length`, where `Layout.fs:718`
  builds `Invalidated` from nodes actually re-measured after fixed-size-ancestor pinning).
- **`LayoutInvalidatedNodeCount <= RemeasuredNodeCount`** always **(direction corrected
  2026-06-13)** — in this codebase `layoutDirtySet` is the small patch-derived *self-dirty* set, and
  `Layout.evaluateIncremental` *expands* each dirty node up to its first fixed-size ancestor and
  re-measures that whole boundary subtree, so the post-pinning re-measured set is a *superset* of the
  pre-pinning dirty set. Research R6 originally asserted the reverse (`>=`) on a mental model where the
  dirty set is large and pinning reduces it; that model does not match this implementation. The honest,
  code-guaranteed relationship is `<=`. See `readiness/layout-invalidated-authority.md`.

**Proof**: a geometry-changing frame (width/height/orientation) asserts a bounded, explainable
`LayoutInvalidatedNodeCount` that is `<= RemeasuredNodeCount`
(`tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs`,
`tests/Elmish.Tests/Feature117MetricsTests.fs`).

## L2 — Zero on idle and style-only / visual-state frames (FR-006/FR-007)

- An **idle** frame reports `LayoutInvalidatedNodeCount = 0` and `RemeasuredNodeCount = 0`.
- A **style-only or runtime-visual-state-only** frame (hover, focus, press, animation tick)
  reports `LayoutInvalidatedNodeCount = 0`, `RemeasuredNodeCount = 0`, and
  `TextMeasureCacheMissCount = 0` (all unchanged text served from the warm cache), while
  staying byte-identical at rest.
- This holds because none of those updates touch a layout-affecting attribute
  (`layoutAffectingAttrNames` = `{ width, height, orientation }` only, `Control.fs:1252`), so
  `layoutDirtySet` is empty.

**Proof**: a scripted hover/focus/anim-tick frame over a text-bearing control asserts all three
quantities are `0` with byte-identical rendered output (SC-003).

## L3 — Drift guard stays in force (FR-008)

- Feature 101's `layoutDriftReport` / single-sourced `layoutAffectingAttrNames`
  (`Control.fs:1252`) MUST remain in force and MUST continue to cover any layout-affecting
  attribute.
- This rung adds **no new geometry-driving attribute**; the drift guard MUST still report an
  **empty** drift. If one were added, the guard MUST still report empty drift.

**Proof**: the feature 101 behavioral drift probe continues to assert empty drift over the
unchanged `layoutAffectingAttrNames`.

## L4 — No multi-pass / intrinsic path introduced (FR-009)

- The codebase performs a **single** measure pass with no intrinsic-sizing path.
- This feature introduces **no** multi-pass / intrinsic path and adds **no** multi-pass metric
  (a no-op against the report's optional Phase 8 task 5). The layout pass-count contract is
  unchanged.

**Proof**: no new layout pass is added; the existing single-pass behaviour is unchanged
(asserted by the standing layout suite + byte-identity).
