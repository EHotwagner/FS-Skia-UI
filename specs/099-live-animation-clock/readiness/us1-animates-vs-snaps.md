# A visual-state transition animates on the live host (feature 099, SC-001)

evidence-kind=animates-vs-snaps
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=ControlRuntime.applyRuntimeVisualState + RetainedRender.advance (Tick) + RetainedRender.step (the real runInteractiveApp seam)
representative-kind=Button (R1-migrated; hover on the opacity channel)
default-transition-duration-ms=150
easing=EaseOut
injected-delta-ms=16
frames-captured=16
intermediate-frames-before-target=10
converges-to-exact-snap-target=true
first-frame-snaps-to-target=false
no-seam-counterfactual=a build without the seam paints the snapped target on frame 0 (no intermediate) and fails the intermediate-frame assertion
note=AUTHORITATIVE proof is the captured sampled frame sequence: ≥1 intermediate appearance (structurally distinct from the target) precedes a frame byte-equal to the static snapped target. Structural Scene equality, no pixel encoder ([[fs-skia-evidence-mode]]).
authoritative-test=Feature099AnimationSeamTests/099 US1 a visual-state transition animates (not snaps) on the live seam
