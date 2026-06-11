# Partial repaint — localized interaction is a bounded repaint (feature 096, T024, SC-005, FR-004)

evidence-kind=partial-repaint
renderer-mode=DeterministicRenderOnly
status=pass

Because the bridge stamps the derived `VisualState` **pre-reconcile** in the `ControlId` domain, a
single hover entering one control becomes a scoped reconciler `Update` patch on exactly that subtree —
not a whole-tree repaint.

Scenario: a stack of three keyed buttons `a`/`b`/`c`. The model transitions from "nothing hovered" to
"`b` hovered"; the bridged next frame is stepped through the live retained path.

Observed (`RetainedRender.step` `WorkReduction`):
- `RecomputedNodeCount` < `BaselineNodeCount` — the localized hover repaints fewer than all nodes
  (the work is O(hovered-subtree), measured via the existing `WorkReduction` metric, composing with
  E2 partial repaint).
- `ChangedSubtreeBound` > 0 — the hovered control `b` is counted as genuinely-changed work (the single
  `Update` patch), while the resting siblings `a`/`c` are `Keep` no-ops.

result=pass — a localized interaction surfaces a single bounded `Update` patch, not a full repaint.
authoritative-test=Feature096RuntimeBridgeTests/Feature 096 runtime visual-state bridge (T024)
