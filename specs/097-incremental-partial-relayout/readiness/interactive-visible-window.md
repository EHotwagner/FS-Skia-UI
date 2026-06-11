# Interactive visible window — NOT APPLICABLE (feature 097, R2)

status=not-applicable
mode=non-interactive
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

R2 (incremental measure / partial re-layout) is performance-and-metric-only: it changes the AMOUNT of
layout work and the reported metric, never the visible output (FR-008). It ships no host app, no window,
and no interactive surface. The viewer-launch / persistent-window task-generation rule does not apply
(recorded as a visible decision in T003). Proof is structural `Bounds`/`Scene` equality + the FsCheck
equivalence invariant — no window to present.
