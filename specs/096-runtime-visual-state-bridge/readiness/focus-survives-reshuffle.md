# Focus indicator survives a sibling-shifting re-render (feature 096, SC-002, FR-007)

evidence-kind=focus-survives-reshuffle
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=ControlRuntime.applyRuntimeVisualState (the real bridge) + RetainedRender.init/step (the live retained path)
hand-seeded-state-by-identity=false
sequence=focus editor -> derive Focused via bridge -> insert banner above (shift) -> re-derive
retained-id-stable-across-shift=true
focused-state-before-shift=Focused
focused-state-after-shift=Focused
baseline-loses-identity-on-shift=true
note=the indicator attaches to the E2 stable retained identity (067/091/092 scheme, consumed not re-derived); the resolved Focused look rides the control's attributes through the keyed diff.
authoritative-test=Feature096LiveBridgeTests/Feature 096 runtime bridge — live retained path
