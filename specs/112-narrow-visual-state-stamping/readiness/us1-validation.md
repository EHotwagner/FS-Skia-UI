# US1 independent validation path (feature 112, T011)

Build a tree of many controls, move hover/focus between two controls, and assert the targeted stamp's
`RuntimeStateTouchedNodeCount`:

- hover/focus move A→B over an N-control tree → touched = A's path + B's path (the two changed leaves +
  their shared ancestor), far below N — it does NOT scale with the control count (SC-001).
- hover persists on the same control, or a fully at-rest frame → touched = 0, and the whole tree is
  reused (`obj.ReferenceEquals(result.Stamped, prevStamped)`) (SC-003/FR-004).
- across a hover sweep, every step touches a small, bounded count (≤ 3 for a flat stack), proportional to
  the affected controls, not N (SC-006).

Evidence: `Feature112TouchedCountTests` (Controls.Tests, internal seam via InternalsVisibleTo). The live
host (`renderRetained`) routes its model-unchanged repaints through the same targeted stamp via the pure
`ControlRuntime.runtimeStampFor` helper, whose route choice (`Some(prior)` → targeted; `None` → oracle)
is itself asserted in `Feature112TargetedStampParityTests` (FR-002).
