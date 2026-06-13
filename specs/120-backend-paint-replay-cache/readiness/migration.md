# Migration notes (feature 120)

**Additive only — no removals, no signature breaks.** Consumers need no source changes.

New public surface (captured in the regenerated per-package + top-level baselines):

- `FS.Skia.UI.Scene`: new `SceneNode.CachedSubtree of CacheBoundary` case + `CacheBoundary` record
  (`CacheId`/`Fingerprint`/`Scene`). TRANSPARENT to every consumer except the GL painter — `describe`,
  diagnostics, `measure`, and all retained walks recurse into `CacheBoundary.Scene`.
- `FS.Skia.UI.Controls.Elmish.FrameMetrics`: `+PaintDuration`/`+ComposeDuration` (`TimeSpan`,
  live-only non-golden) and `+ReplayHitCount`/`+ReplayMissCount`/`+ReplayRecordCount`/
  `+ReplaySkippedNodeCount`/`+ReplayCacheNativeBytes` (`int`, golden). `DirtyArea` docstring corrected
  to the union semantics (value now never exceeds the frame area).
- `FS.Skia.UI.SkiaViewer`: `ViewerOptions.PresentMode` docstring corrected to name the shipped
  `DirectToSwapchain` default; new `GlHost.lastPresentTiming: unit -> TimeSpan * TimeSpan` and
  `GlHost.shouldPresent: Scene option -> Scene -> bool -> bool`.

Internal (no public surface): `RetainedRender.hashScene`/`unionArea`, `PictureCacheKey` now keyed by
`Fingerprint`, `RenderFragment.Fingerprint`; new internal `PictureReplayCache` module.

Behavior is additive and gated behind oracles (`PictureCacheEnabled` / `PictureReplayCache` enabled
flag): presented pixels and deterministic count goldens stay byte-identical at rest.
