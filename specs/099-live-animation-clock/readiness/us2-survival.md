# An in-flight animation survives an unrelated re-render and completes (feature 099, SC-002/FR-004)

evidence-kind=survival
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.advance (Tick) + RetainedRender.step over the existing RetainedId-keyed StateByIdentity carry (the real seam)
hand-seeded-clock=false
replaces=Feature092LiveSurvivalTests hand-seeded startedClock() PRECONDITION
sequence=hover button -> tick 3 frames (clock mid-flight) -> insert banner above (sibling shift) -> continue ticking to completion
identity-stable-across-shift=true
elapsed-before-shift-ms=32.000000
elapsed-after-shift-ms=48.000000
clock-continued-not-reset=true
shifted-trajectory-equals-unshifted=true
note=the clock rides the E2 stable RetainedId map; the sibling shift moves the button's position but not its identity, so the carried clock keeps advancing to completion. No parallel identity scheme (FR-008).
authoritative-test=Feature099AnimationSeamTests/099 US2 an in-flight animation survives an unrelated re-render and completes
