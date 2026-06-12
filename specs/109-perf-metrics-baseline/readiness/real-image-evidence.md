# Real-image evidence — applicability (feature 109, T001/T028)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 109 is **not** a new persistent graphical-viewer feature (a visible decision recorded in T001):
it is observation-and-evidence only. Its stories are proven by deterministic per-frame `FrameMetrics`
through `ControlsElmish.Perf.runScript` and committed count goldens, not by a captured image. No new
window, host-launch, or screenshot is introduced.

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — the metric goldens are the COUNTS the production render path performs
  (full renders / remeasured nodes), read structurally; no pixel is asserted.
- proves-desktop-visibility=false — feature 109 makes no desktop-visibility claim; the user-reachable
  surface is the `FrameMetrics` / `Perf.runScript` observability contract, not a new window.
