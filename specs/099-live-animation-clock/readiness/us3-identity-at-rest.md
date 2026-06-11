# Identity-at-rest — a no-active-clock frame is byte-identical to the pre-R4 golden (feature 099, SC-003/FR-005)

evidence-kind=identity-at-rest
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.init/step (the live retained path) + Control.renderTree (the static golden)
no-active-clock-byte-identical-to-static=true
at-rest-recompute-count=0
at-rest-remeasure-count=0
settled-return-to-normal-clock-dropped=true
note=`Animation.applyAt`'s identity-at-rest lowering + dropping a settled return-to-Normal clock means an at-rest identity emits NO animation attribute; the wired scene equals the full static rebuild byte-for-byte.
authoritative-test=Feature099AnimationClockTests/099 US3 identity-at-rest (byte-identical, zero recompute)
