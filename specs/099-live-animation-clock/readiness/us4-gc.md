# A removed identity's animation clock is garbage-collected (feature 099, SC-005/FR-007)

evidence-kind=gc
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.step (the existing liveIds filter; no new GC code)
sequence=hover button (clock active) -> re-render with the button removed -> inspect next frame's StateByIdentity
clock-present-while-live=true
clock-absent-after-removal=true
note=the generalized animation slot rides the same RetainedUiState the liveIds filter already drops for removed identities; matching the existing focus/text GC behavior, the clock leaves with its identity (no parallel identity scheme, no dangling animation state).
authoritative-test=Feature099AnimationSeamTests/099 US4 a removed identity's animation clock is garbage-collected
