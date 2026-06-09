# Real image evidence (085) — decodable render-distinctness images (SC-001)

evidence-kind=screenshot
status=ok
artifact-kind=image
artifact-decodable=true
proves-scene-rendering=true
proves-desktop-visibility=false
image-artifact=specs/085-showcase-feedback-followups/evidence/render-distinctness/page-a.png

## Captured artifacts (real, decodable)

- **Authoritative command**: `Viewer.captureScreenshotEvidence`
  (`CaptureMode = ViewerRenderTargetPng`) over `SceneNode.Group [ (Control.renderTree
  theme size page).Scene ]`, on the display-capable host (`DISPLAY=:1`, 2026-06-09).
  Script: `readiness/fsi/capture-distinctness.fsx` (LD_LIBRARY_PATH → skiasharp linux-x64).
- **Artifacts**:
  - `evidence/render-distinctness/page-a.png` — **640×480 PNG, 3733 bytes, valid PNG
    signature, pixel-content=non-blank** (`ScreenshotOk`, `proves=true`). Renders the
    `pageA` tree: an "ALPHA" label, a blue "GO" button, and a slider (track + thumb), each
    laid out at its real Yoga-computed bounds — proving nested children are painted, not
    just the outer container (FR-001/FR-002).
  - `evidence/render-distinctness/page-b.png` — **640×480 PNG, 3367 bytes, non-blank**
    (`ScreenshotOk`). Renders the structurally different `pageB` tree (nested sub-stack).
- **SC-001 diff**: the two PNGs differ (3733 ≠ 3367 bytes; byte diff non-empty) — two
  structurally different trees produce visibly different scenes.
- **Failure class**: a metadata-only / 1×1 fallback claimed as visual proof would be a
  defect; both artifacts are real decodable rasters with non-trivial content
  (`PixelContentNonBlank`).

`proves-scene-rendering=true` (the decoded PNGs carry the rendered nested tree);
`proves-desktop-visibility=false` because these are off-window render-target rasters, not
desktop grabs. The desktop-visibility proof is the durable interactive window launch
(US2, T018), recorded in `interactive-visible-window.md`.
