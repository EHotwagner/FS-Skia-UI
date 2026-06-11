# Final-frame byte-identity — a settled transition equals the snapped static render (feature 103, SC-003/INV-2)

evidence-kind=final-frame-identity
renderer-mode=DeterministicRenderOnly
status=pass
driven-through=RetainedRender.step advanced past the transition duration with a large injected delta
settled-frame-byte-identical-to-static-hover=true
channels=every animated channel (the settled clock is inactive ⇒ the node paints ownStatic verbatim)
note=once Elapsed ≥ Duration the clock is inactive (clockActive=false); the assemble walk paints ownStatic with no composite, so the final frame equals Control.renderTree's static paint of the new state byte-for-byte (FR-005). The settle path is NOT modified by R6.
authoritative-test=Feature103CrossFadeTests/103 US2 at-rest and settled output is byte-identical to the static render
