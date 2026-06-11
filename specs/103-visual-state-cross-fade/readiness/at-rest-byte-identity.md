# At-rest byte-identity — a no-active-clock frame equals the static render (feature 103, SC-002/INV-1)

evidence-kind=at-rest-byte-identity
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.init/step (the live retained path) + Control.renderTree (the static reference)
no-active-clock-byte-identical-to-static=true
no-animation-attribute-at-rest=true
at-rest-recompute-count=0
at-rest-remeasure-count=0
note=the cross-fade is an assembly-time overlay gated to active (mid-flight) clocks only; with no active clock the assemble fast path returns the cached SubtreeScene verbatim, so the at-rest frame is byte-identical to the static render and the settle/fast path is UNCHANGED (FR-004).
authoritative-test=Feature103CrossFadeTests/103 US2 at-rest and settled output is byte-identical to the static render
