# Real-image evidence — applicability (feature 110, T002)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 110 is **not** a new persistent graphical-viewer feature (a visible decision recorded in T001):
it is a hot-path routing MECHANISM change. Its stories are proven by deterministic per-frame
`FrameMetrics` through `ControlsElmish.Perf.runScript`, the regenerated count goldens, and the internal
retained-route seams compared against the preserved full-render oracle — not by a captured image. No new
window, host-launch, or screenshot is introduced.

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — the metric goldens are the COUNTS the production render path performs
  (full renders / fallbacks / remeasured nodes), read structurally; no pixel is asserted. At-rest
  rendered output byte-identity is the standing Scene-parity golden suite's job
  ([byte-identity-authority.md](./byte-identity-authority.md)), not a new image here.
- proves-desktop-visibility=false — feature 110 makes no desktop-visibility claim; the user-reachable
  surface is the `FrameMetrics` / `Perf.runScript` observability contract and the retained routing
  mechanism, not a new window.
