# Real-image evidence — applicability (feature 111, T002)

evidence-kind=real-image-evidence
status=not-applicable
artifact-decodable=not-applicable
proves-scene-rendering=false
proves-desktop-visibility=false

Feature 111 is **not** a new persistent graphical-viewer feature (a visible decision recorded in T001):
it is a per-frame scheduling/observability change. Its stories are proven by deterministic per-frame
`FrameMetrics` (cause + phase record) through `ControlsElmish.Perf.runScript`, the regenerated count
goldens, and the internal `RetainedRender` step — not by a captured image. No new window, host-launch,
or screenshot is introduced.

- artifact-decodable=not-applicable — no image/screenshot is produced; there is nothing to decode.
- proves-scene-rendering=false — the goldens are the COUNTS/phases the production render path performs,
  read structurally; no pixel is asserted. At-rest rendered-output byte-identity is the standing
  Scene-parity golden suite's job ([byte-identity-authority.md](./byte-identity-authority.md)).
- proves-desktop-visibility=false — feature 111 makes no desktop-visibility claim; the user-reachable
  surface is the `FrameMetrics`/`FrameCause` observability contract and the frame scheduler, not a window.
