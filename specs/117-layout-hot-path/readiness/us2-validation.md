# US2 independent validation — layout dirty propagation is observable by count

**Story**: A maintainer can see how far a layout invalidation propagated — the size of the dirty set fed
into incremental layout (`LayoutInvalidatedNodeCount`), distinct from the post-pinning re-measured set
(`RemeasuredNodeCount`).

## Path

Drive (a) an idle frame, (b) a style-only frame, and (c) a geometry-changing frame (width) through the
retained `RetainedRender.step` / `Perf.runScript`; assert the idle and style-only frames report
`LayoutInvalidatedNodeCount = 0` and `RemeasuredNodeCount = 0`, and the geometry frame reports a bounded,
explainable `LayoutInvalidatedNodeCount` that is `<= RemeasuredNodeCount` (the honest, code-guaranteed
direction — see [layout-invalidated-authority.md](./layout-invalidated-authority.md) for the spec
correction). Confirm the feature-101 drift-guard attribute set is unchanged (FR-008).

## Evidence

- `tests/Controls.Tests/Feature117LayoutInvalidatedTests.fs` — idle = 0; style-only = 0/0; geometry frame
  `invalidated >= 1`, `invalidated <= remeasured`, `invalidated <= total`; `layoutAffectingAttrNames =
  { width; height; orientation }` unchanged.
- `tests/Elmish.Tests/Feature117MetricsTests.fs` — over `Perf.runScript`, the geometry frame asserts
  `LayoutInvalidatedNodeCount <= RemeasuredNodeCount` with `invalidated >= 1`; the idle frame reports all
  three new fields `0`.
- 109 perf-corpus goldens carry `LayoutInvalidatedNodeCount` per frame.

Result: PASS — dirty propagation is observable and bounded (SC-006), with `LayoutInvalidatedNodeCount <=
RemeasuredNodeCount`.
