# Contract: Shared scene renderer (FR-001/002)

## Surface

Internal to `src/SkiaViewer/` (non-public — no `.fsi` export, no SkiaViewer
surface-baseline change):

```fsharp
module internal SceneRenderer =
    val paintNode: canvas: SKCanvas -> node: SceneNode -> unit
```

Both `drawScene` (interactive, `Host/Vulkan.fs`) and `drawScreenshotScene`
(evidence, `SkiaViewer.fs`) call `SceneRenderer.paintNode` for every node; neither
keeps its own `match` over `SceneNode`.

## Behavioral contract

- **Exhaustive**: `paintNode` matches every `SceneNode` case with **no wildcard**.
  A new `SceneNode` case is a compile error until handled — the two render paths
  cannot diverge again.
- **Pixel-faithful in evidence mode**: on the raster evidence canvas (`SKBitmap`),
  `Line` → `DrawLine`, `Path` → `DrawPath` (via `toSkPath`), `Arc` → `DrawArc`,
  `Points` → `DrawPoint`, `Vertices` → `DrawVertices`, `Ellipse`/`FilledEllipse` →
  `DrawOval`, `Image` → `DrawImage`, `RegionNode`/`Chart` → their existing draws,
  `Text`/`TextRun` → **real glyphs** via `drawTextWithFallback`. No node maps to a
  placeholder rectangle.
- **No placeholder wildcard**: the prior `_ -> DrawRect(8,8,48,48 teal)` at
  `SkiaViewer.fs:1804-1806` is deleted.

## Verification

- **Unit/golden**: render a scene with `Line` + `Path` + `Text` through the
  evidence path; assert the decoded PNG has non-blank pixels in the expected
  regions (not a single 40×40 block), and that `Text` produces glyph pixels.
- **Before/after capture (SC-001)**: a scene of a polyline terrain (`Line` nodes) +
  filled ground (`Path`) renders blank/placeholder before, full terrain after.
- **Exhaustiveness (SC-002)**: the build fails if any `SceneNode` case is unhandled
  (compiler) — proven by the absence of a wildcard in `paintNode`.
- **Parity**: interactive and evidence renders of the same scene agree on which
  primitives appear (shared painter).

## Out of scope

- No `.fsi`/public-surface change for SkiaViewer (D11). No new
  `ScreenshotPixelContentValidation` case (D3). Interactive `Vulkan.fs` keeps full
  coverage (only refactored to delegate).
