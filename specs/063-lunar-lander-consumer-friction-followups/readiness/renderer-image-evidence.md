# Real image-evidence proof (FR-001/002, SC-001/SC-002)

Before/after proof that the image-evidence renderer faithfully draws every scene
primitive. The evidence is captured by `tests/SkiaViewer.Tests/Feature063RendererTests.fs`,
which drives the **public** image-evidence entry point
`Viewer.captureScreenshotEvidence` (raster `SKBitmap`, no window — headless-safe) and
inspects the **decoded PNG pixels**. The discriminator is grounded in the pre-fix
defect: `drawScreenshotScene` routed `Line`/`Path`/… to a single 40×40 teal placeholder
at (8,8)-(48,48) and drew `Text` as a solid filled block.

## The defect (pre-fix)

`src/SkiaViewer/SkiaViewer.fs` `drawScreenshotScene` had its own stunted `match` with a
catch-all `_ -> DrawRect(8,8,48,48 teal)` wildcard, while the interactive
`Vulkan.drawScene` handled every case. Two divergent renderers; the evidence one lied.

## BEFORE (failing-first, pre-fix renderer)

`dotnet run --project tests/SkiaViewer.Tests -- --filter "Feature063 image-evidence renderer"`
→ **0 passed, 3 failed**:

```
Line and Path render to pixels beyond the placeholder box (SC-001)
  → Expected a (0) to be greater than b (0).
    (no lit pixels right of the placeholder box — Line/Path collapsed onto the 40×40 block)
Text renders real glyphs, not a solid filled block (SC-001)
  → Expected a (1.0) to be less than b (0.85).
    (lit-pixel fill fraction of the text bounding box = 1.0 — a solid rectangle)
node count is structural; the image is the visual proof on a Line-only scene (SC-002)
  → Expected a (0) to be greater than b (0).
    (Scene.describe reports a Line, but the image has 0 lit pixels beyond the placeholder)
```

## THE FIX

One shared exhaustive painter `SceneRenderer.paintNode` (`src/SkiaViewer/SceneRenderer.fs`,
non-public). Both `Vulkan.drawScene` and `drawScreenshotScene` delegate to it; the
placeholder wildcard is deleted; `Text`/`TextRun` render as **real glyphs** via the
shared `drawTextWithFallback`. The `match` has **no wildcard**, so the two paths can never
diverge again (compile guard = SC-002).

## AFTER (post-fix renderer)

Same command → **3 passed, 0 failed**:

```
[19:49:29 INF] EXPECTO! 3 tests run … for Feature063 image-evidence renderer
  – 3 passed, 0 ignored, 0 failed, 0 errored. Success!
```

Measured discriminators on the decoded PNGs:
- **Line + Path** (terrain polyline (20,180)→(300,190) + filled ground polygon): lit
  pixels now appear **well to the right of x=108** (outside the old placeholder box) —
  the primitives paint real geometry, not a single block. (SC-001)
- **Text** ("HUD" at (120,120)): the tight bounding box of lit text pixels has a fill
  fraction **< 0.85** (glyph interiors have gaps) — real glyphs, not a solid rectangle.
  (SC-001)
- **Line-only scene**: `Scene.describe` reports `LineElement` (structural), **and** the
  image now shows lit pixels along the line beyond the placeholder box (visual proof) —
  node count and the image are no longer conflated. (SC-002)

## Regression scope

Full `tests/SkiaViewer.Tests` suite: **51 passed, 0 failed** after the refactor (the
`libdecor-gtk.so` line is the known benign headless host warning, not a test failure).
The interactive Vulkan renderer keeps full primitive coverage — it now calls the same
shared painter. SkiaViewer's public surface is unchanged (the shared module is
`internal`).
