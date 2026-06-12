# Real-image evidence — applicability (feature 108, T002/T003/T040)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 108 is **not** a new persistent graphical-viewer feature (a visible decision recorded in T003):
its stories are pure-transition / host-seam framework changes proven by deterministic structural-Scene
assertions through `Control.renderTree` / `RetainedRender.step` (`Perf.runScript`) and an interactive
responds-proof, not by a captured image. No new window, host-launch, or screenshot is introduced.

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — the focus-ring proof is the Scene DESCRIPTION the production
  `Control.renderTree` path emits (exactly one control carries the `Focused` stamp), read structurally.
- proves-desktop-visibility=false — feature 108 makes no desktop-visibility claim; the user-reachable
  surfaces are the retained render/route path and `markFocused`, not a new window.
