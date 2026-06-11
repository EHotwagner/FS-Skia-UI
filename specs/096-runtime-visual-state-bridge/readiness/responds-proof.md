# Input -> visible restyle on the live retained path (feature 096, responds-proof)

evidence-kind=responds-proof
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=ControlRuntime.applyRuntimeVisualState before RetainedRender (the host renderRetained seam)
focus-input-restyles=true
press-input-restyles=true
un-bridged-build-is-inert=true
note=the responds-proof is the bridged frame DIFFERING from the inert/un-bridged frame for the same input; an inert build paints identical frames regardless of interaction state. Structural Scene inequality, not a pixel encoder ([[fs-skia-evidence-mode]]).
authoritative-test=Feature096LiveBridgeTests/Feature 096 runtime bridge — live retained path
