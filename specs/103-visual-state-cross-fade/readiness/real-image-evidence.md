# Real-image evidence — applicability (feature 103, R6, T002/T003)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

R6 is **not** a persistent graphical viewer feature (a visible decision recorded in T003): it is a
framework-internal behavior change (the live visual-state transition becomes a snapshot-composite
cross-fade) proven by structural Scene assertions through `RetainedRender.step`, not by a captured
image. No window, host-launch, or user-driven interactive surface is introduced and no image artifact
is produced:

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — the cross-fade proof is the scene DESCRIPTION the production assemble
  path emits (colours strictly between the endpoints), read structurally; R6 makes no pixel claim.
- proves-desktop-visibility=false — pixel-readback alone cannot prove desktop visibility, and R6 makes
  no desktop-visibility claim. The user-reachable surface is the retained assemble path, not a window.
