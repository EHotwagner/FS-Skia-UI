# Smoke: idle / unchanged-frame zero-redraw (T019, SC-002 / FR-004 / FR-005 / FR-006)

**Authoritative command:** `dotnet run --project tests/SkiaViewer.Tests` (idle-skip decision test).
**Failure class:** product-defect.

## What was proven

The present interpreter's idle-skip is a **pure decision** `GlHost.shouldPresent prev next sizeChanged`,
asserted directly (`Feature120ReplayCacheTests` idle-skip test):

- first frame (`prev = None`) ⇒ **present** (always paints the first frame);
- unchanged scene + unchanged framebuffer size ⇒ **skip** (no surface clear, no scene walk, no
  draw-call re-issue — the double-buffered front buffer still holds the last presented frame, FR-005);
- a changed scene ⇒ **present** (a forced repaint after the idle run, byte-identical to the first
  frame by the shared painter — SC-002);
- a framebuffer-size change ⇒ **present even if the scene is unchanged** (FR-006 — never leave a
  resized/blank surface).

`renderFrame` consults this decision before any clear/walk/swap and, on a skip, reports
`PaintDuration = ComposeDuration = TimeSpan.Zero` and increments `skippedPresentCount`. The live
windowed stream exercises this same `renderFrame` path on the Mesa reference host (the GL present
path itself was launch-verified in feature 119); the zero-work decision is the deterministic core
proven here.
