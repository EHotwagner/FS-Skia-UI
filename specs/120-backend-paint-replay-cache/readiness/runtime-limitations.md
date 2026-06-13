# Runtime limitations & failure diagnostics (feature 120)

## Platform / runtime support boundary

Feature 120 makes the OpenGL backend paint path do honest, observable, work-skipping rendering:
per-phase paint/compose timing (US1), an unchanged-frame present skip (US2), a load-bearing
`SKPicture` record/replay cache keyed by a collision-resistant structural fingerprint (US3), and
honesty cleanups (US4). It runs on a .NET 10 desktop host rendering through OpenGL via the SkiaSharp
preview native binding, on Windows and Linux desktop (`net10.0`).

Platform boundary (single-line, exact tokens): .NET 10 desktop, OpenGL, SkiaSharp preview, unsupported macOS/mobile/browser, no software-renderer fallback — a host without a working GL context classifies as `UnsupportedEnvironment`, not a product defect.

This feature is no package/API/runtime support expansion in the consumer sense — it is an additive
backend optimization with source-stable consumer entry points and additive public
`FrameMetrics`/`Scene` surface.

## Unsupported scope (explicitly deferred, not closed)

- **No damage-rect GPU clip** of the redraw. FR-015 ships the union damage METRIC only; using it to
  clip the GL redraw is a separate follow-on.
- **No render-thread / compositor split.** The report's "do later" item, gated on CPU-bound metrics.
- **Windows GL not launch-verified.** All live pixel-readback and timing evidence is captured on the
  **Linux/AMD Mesa OpenGL** reference environment (feature 119's reference host). Windows GL
  portability is asserted by the shared code path, **not** launch-verified — a residual risk called
  out here rather than closed.
- **No broadening of the replay-boundary heuristic** beyond the prior-frame-stability gate (FR-012).

## Safe-failure diagnostics

- A failed `SKPicture` record degrades explicitly to the **direct walk** (`PictureReplayCache`
  records-then-draws; a record exception is not swallowed into a blank or stale subtree — the boundary
  paints through its `paintScene` fallback). `CachedSubtree` is transparent to every IR consumer, so a
  disabled or absent cache recurses into the wrapped scene identically to the pre-120 direct walk.
- The **idle-skip degrades to painting** whenever the dirty signal is uncertain: `GlHost.shouldPresent`
  returns `true` on the first frame, on any scene change, and on any framebuffer-size change, so a
  resize or an ambiguous frame always repaints rather than risk a stale/blank front buffer.
- The replay cache is **bounded** (`PictureReplayCache.cap`, mirroring `PictureCacheCap = 256`) with
  deterministic min-stamp LRU eviction and **explicit `Dispose`** of every native `SKPicture` on
  eviction, replacement, and teardown — native memory is observable (`stats().NativeBytes`,
  `FrameMetrics.ReplayCacheNativeBytes`) and does not grow unbounded across a long scripted run.

## Determinism & oracle backstop

The replay counters surfaced in `FrameMetrics` are a **deterministic model** computed in
`RetainedRender.step` (the load-bearing realization coincides with the picture-cache outcomes by
construction); the per-phase `PaintDuration`/`ComposeDuration` are **live-only, non-golden**
(`TimeSpan.Zero` on the deterministic `Perf.runScript` path) so the count goldens stay byte-identical
when timing is added (SC-001/FR-002). Byte-identity of replayed pixels is backstopped by the
always-direct **replay-disable oracle** (`PictureReplayCache.create false`, FR-011): a fingerprint
collision degrades to a missed optimization, never a wrong pixel.
