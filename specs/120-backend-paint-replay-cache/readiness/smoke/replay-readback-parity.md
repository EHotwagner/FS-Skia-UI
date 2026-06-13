# Smoke: replay-cache pixel-readback parity (T027, SC-003 / FR-009 / FR-011)

**Authoritative command:** `dotnet run --project tests/SkiaViewer.Tests` → test
`Feature 120 backend replay cache … cache-on ≡ cache-off pixel readback parity`.
**Artifact:** Expecto test result (80 passed) + this note. **Failure class:** product-defect.

## What was proven (real pixels, real SkiaSharp painter)

A scene of three `CachedSubtree` replay boundaries is rendered to a real raster `SKSurface`
through the **production shared painter** `SceneRenderer.paintNode` — the exact per-`SceneNode`
walk the GL host uses (`GlHost.drawScene` delegates to it). The rendered surface is snapshotted and
PNG-encoded, and the byte arrays are compared:

- **cache-OFF (direct walk)** vs **disabled oracle** (`PictureReplayCache.create false`): byte-identical.
- **cache-OFF (direct walk)** vs **warmed replay** (`create true`, 3 frames: frame 1 records, frames
  2–3 replay the recorded `SKPicture` via `DrawPicture`): **byte-identical**.

So a replayed boundary produces pixels byte-identical to walking and painting the subtree directly
(SC-003), and the always-direct oracle proves cache-on ≡ cache-off (FR-011). The painter is shared
between the raster and GL backends, so this is the same paint output the GL present produces;
feature 119 separately proved the GL present path on real AMD Mesa hardware.

## SC-004 work-reduction signal (deterministic)

`Feature120MetricsTests` (Elmish) asserts a warm stable grid reports `ReplaySkippedNodeCount > 0`
(the subtree paint nodes the replay avoids) and `ReplayHitCount = PictureCacheHitCount`. The
non-golden paint-duration delta is the directional perf bar recorded in the timing baseline, not a
deterministic golden.
