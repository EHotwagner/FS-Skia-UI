# Animation repaint stays scoped to the active subtree (feature 099, SC-006/FR-010)

evidence-kind=scoped-repaint
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.step WorkReduction metric on a steady-state animating frame
steady-state-recompute-count=0
steady-state-remeasure-count=0
frame-changes-while-animating=true
note=animation is a paint-level overlay applied to cached STATIC fragments at scene assembly; a structurally-unchanged animating frame takes the Keep fast path (zero re-measure, zero re-paint) while still sampling the clock, so one active animation never invalidates the at-rest fast path for the rest of the tree.
authoritative-test=Feature099AnimationSeamTests/099 scoped repaint — animation does not force a whole-tree repaint/re-measure
