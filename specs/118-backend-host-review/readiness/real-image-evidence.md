# real-image-evidence — feature 118 (US1, T017)

evidence-kind=real-image-evidence
status=applicable
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false

Feature 118 launches a real persistent Vulkan window and captures on-demand screenshots through
the offscreen readback routine under both present modes
(`smoke/direct-mode-smoke.md`, `default-byte-identity.md`). The artifacts
`smoke/direct-frame.png` and `smoke/offscreen-frame.png` are decodable PNGs (480×320, 8-bit
RGBA; `file` confirms) and are **byte-identical** (sha256
`098bae46…49fee8`), proving the scene renders identically regardless of present mode.

artifact-decodable=true — both captures decode as valid PNGs.
proves-scene-rendering=true — the captured pixels are the rendered scene (rectangles + text),
produced by the production `SceneRenderer` path the live host uses.
proves-desktop-visibility=false — these are **pixel-readback** captures (the offscreen readback
routine), and pixel-readback alone cannot prove desktop visibility; no external desktop/window
grab tool was available in this environment. Live desktop presentation IS evidenced
independently by the windowed run completing 40 presented frames on the real backend
(`RESULT: ok frames=40`), but that lifecycle signal is recorded separately from this image
artifact, which only proves scene rendering.
