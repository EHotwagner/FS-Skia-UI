# Real image evidence (084) — decodable windowed-fullscreen launch image

evidence-kind=screenshot
status=ok
artifact-kind=image
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false
image-artifact=readiness/screenshots/windowed-fullscreen-launch.png

## Captured artifact (real, decodable)

- **Authoritative command**: `Viewer.captureScreenshotEvidence` (`CaptureMode = ViewerRenderTargetPng`) over the launch scene on the display-capable host (`DISPLAY=:1`, 2026-06-09).
- **Artifact**: `readiness/screenshots/windowed-fullscreen-launch.png` — **1280×800 RGBA PNG, 5907 bytes, valid PNG signature, pixel-content=non-blank** (`ScreenshotOk`).
- **Failure class**: a metadata-only/1×1 fallback claimed as visual proof would be a defect; this is a real decodable raster with non-trivial content.

`proves-scene-rendering=true` (the decoded PNG carries the rendered scene);
`proves-desktop-visibility=false` because this artifact is an off-window render-target
raster, **not** a desktop grab. The **desktop-visibility** proof is the real visible
window itself — `window-visible=observed:true` for the no-flag windowed-fullscreen
default and every supported startup state — recorded in `interactive-visible-window.md`
and `supported-host-persistent-launch.txt`. Together they discharge SC-001/SC-002:
a real visible window for each state plus a decodable image of the rendered content.
