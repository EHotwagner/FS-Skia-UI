# Phase 1 Contracts: `.fsi` deltas

All deltas are **additive** (new cases/fields/modules). No removals, no signature breaks.
Surface baselines (top-level + per-package) regenerate via `RefreshSurfaceBaselines`.

## 1. `src/Scene/Scene.fsi` — additive public case

```fsharp
/// A reuse-stable subtree marked as a backend replay-cache boundary (feature 120, FR-007).
/// TRANSPARENT to every Scene-IR consumer except the backend painter: describe/diagnostics/
/// measure and all retained walks recurse straight into `Scene`, so deterministic goldens and
/// at-rest pixels are unchanged. Only the GL painter consults the SKPicture replay cache here;
/// with replay disabled it recurses into `Scene` identically to the direct walk.
type SceneNode =
    | ...                         // existing 24 cases unchanged
    | CachedSubtree of CacheBoundary

and CacheBoundary =
    { /// Stable subtree identity (from RetainedId) — the replay cache slot.
      CacheId: uint64
      /// Collision-resistant structural fingerprint of the wrapped subtree's render-affecting
      /// inputs; replay is valid iff a cached picture's fingerprint matches this.
      Fingerprint: uint64
      /// The wrapped subtree — record source and transparent fallback.
      Scene: Scene }
```

## 2. `src/Controls.Elmish/ControlsElmish.fsi` — additive `FrameMetrics` fields

```fsharp
type FrameMetrics =
    { ...                                  // existing fields unchanged
      /// Feature 120 (US1): scene→canvas paint-walk time. Live diagnostic only — excluded from
      /// count goldens (mirrors `FrameDuration`); `TimeSpan.Zero` on the deterministic path.
      PaintDuration: TimeSpan
      /// Feature 120 (US1): flush + buffer-swap present time. Live diagnostic only; non-golden.
      ComposeDuration: TimeSpan
      /// Feature 120 (US3): replay HITS — boundaries drawn from a valid recorded picture.
      ReplayHitCount: int
      /// Feature 120 (US3): replay MISSES — boundaries (re)recorded (cold / changed / evicted).
      ReplayMissCount: int
      /// Feature 120 (US3): pictures recorded this frame.
      ReplayRecordCount: int
      /// Feature 120 (US3): subtree paint-nodes skipped by replay — the work-reduction signal.
      ReplaySkippedNodeCount: int
      /// Feature 120 (US3, FR-013): native bytes held by the replay cache after this frame
      /// (bounded by the cap; observable for memory regression). }
      ReplayCacheNativeBytes: int }
```

`DirtyArea` doc corrected: "the integer area of the **union** of distinct damage rectangles
(no longer the sum); never exceeds the frame area" (FR-015).

## 3. `src/SkiaViewer/SkiaViewer.fsi` — corrected docstring (FR-016)

```fsharp
type ViewerOptions =
    { Title: string
      InitialSize: Size
      /// Live present mechanism. Defaults to `ViewerPresentMode.DirectToSwapchain` — the
      /// readback-free direct present on the OpenGL backend (feature 119). Set to
      /// `ViewerPresentMode.OffscreenReadback` only for evidence/screenshot capture that needs
      /// a GPU→CPU readback.
      PresentMode: ViewerPresentMode }
```

> Note: this corrects stale feature-118 text that named `OffscreenReadback` as the default.
> `ViewerOptions` carries no intrinsic default; the shipped default lives at the
> `Viewer.defaultConfiguration` construction site, which already uses `DirectToSwapchain`
> (feature 119) — the docstring is brought into agreement with it.

## 4. `src/SkiaViewer/PictureReplayCache.fsi` — NEW internal module

```fsharp
namespace FS.Skia.UI.SkiaViewer

/// Internal: bounded LRU of recorded SKPictures keyed by CachedSubtree CacheId, validated by
/// Fingerprint. Owns native picture lifetime. Not part of the public package surface.
module internal PictureReplayCache =

    /// Default capacity (mirrors RetainedRender.PictureCacheCap = 256).
    val cap: int

    type internal Cache

    /// Create an empty cache; `enabled=false` makes every paint recurse directly (parity oracle).
    val create: enabled: bool -> Cache

    /// Paint a CachedSubtree boundary: replay on a valid hit, else record-then-draw; updates LRU
    /// and disposes evicted/replaced pictures. `paintScene` is the direct walk used to record.
    /// Returns the per-boundary counters folded into FrameMetrics.
    val paintBoundary:
        cache: Cache ->
        canvas: SkiaSharp.SKCanvas ->
        paintScene: (SkiaSharp.SKCanvas -> FS.Skia.UI.Scene.Scene -> unit) ->
        boundary: FS.Skia.UI.Scene.CacheBoundary ->
            unit   // counters accumulated on the cache; surfaced via a stats accessor

    /// Live entry count / native byte total for metrics (FR-013/FR-014).
    val stats: Cache -> {| Entries: int; NativeBytes: int; Hits: int; Misses: int; Records: int; SkippedNodes: int |}

    /// Dispose all resident pictures (teardown).
    val dispose: Cache -> unit
```

*(Exact shape — tuple vs. anonymous record vs. a small stats record — is finalized in FSI per
Constitution Principle I before the `.fs` body; the contract above is the sketch to exercise.)*

## 5. `src/Controls/RetainedRender.fsi` — internal only (no public surface delta)

- `PictureCacheKey` becomes `{ Box: Box; Fingerprint: uint64 }` (was `{ Box; Picture: string }`).
- New internal `val hashScene: FS.Skia.UI.Scene.SceneNode -> uint64` (structural fingerprint).
- New internal seam emitting `CachedSubtree` for prior-frame-stable subtrees during `sceneList`
  assembly. `Fragment` record gains internal `Fingerprint: uint64`.
- All internal reduction / virtual-items / damage walks updated to see through `CachedSubtree`.

## Parity & determinism contract (cross-cutting)

- **FR-009/FR-011**: for every corpus scene, `render(replay=on)` and `render(replay=off)` MUST
  produce byte-identical readback pixels. Proven on real Mesa hardware.
- **FR-002**: adding `PaintDuration`/`ComposeDuration` MUST NOT change any count golden. The
  new replay counters add **new** golden lines but MUST NOT alter existing count lines.
- **FR-010**: any render-affecting change flips `Fingerprint` → miss → re-record; a constructed
  structural difference that `%A` would have collided on MUST produce a miss.
