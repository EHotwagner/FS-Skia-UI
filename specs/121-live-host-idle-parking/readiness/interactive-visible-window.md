# interactive-visible-window — applicability (feature 121)

status=not-applicable
mode=deterministic-pacing-evidence
window-visible=not-applicable
accessible-window=not-applicable
first-frame-presented=not-applicable
self-closed-for-evidence=not-applicable

Feature 121 changes the **live** persistent loop (an additive `ViewerOptions.FrameRateCap` that gates
update+present cadence, US1) and the live per-tick clock advance (allocation-free when idle, US2), plus
documentation of an existing public surface (US3). It ships **no new persistent/graphical entry point**
and drives **no interactive window** in this evidence. The persistent window is not drivable in the
headless / no-compositor CI environment (recorded in `runtime-limitations.md`); the loop change is
proven on the extracted pure pacing decision `GlHost.shouldAdvanceFrame` (unit-tested in isolation) plus
reasoning, and the offscreen/evidence (`runBounded`) path — which does not use the persistent event loop
— is unaffected. The `runInteractiveApp` / `runInteractiveViewer` launch contract is unchanged. No
interactive-window pass is claimed; the free-run on a no-compositor host is an environment limitation,
not a product defect.
</content>
