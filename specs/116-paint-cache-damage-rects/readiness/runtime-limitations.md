# Runtime limitations & failure diagnostics (feature 116)

## Documented evidence path

Feature 116 is a **paint-cache + damage-observability + bounded-memory + advisory-diagnostic** change
proven by deterministic, headless evidence; a live Vulkan window is **not required** (spec *Unsupported
scope* / Assumptions). The asserted surfaces:

- the damage set (`RepaintedNodeCount` / `DirtyRectCount` / `DirtyArea`), exercised from `Controls.Tests`
  — small localized hover vs frame-spanning theme switch vs idle-zero, deterministic integers
  (`Feature116DamageTests`);
- the fully-keyed picture cache — two-frame stable-subtree hit byte-identity, per-keyed-input miss, and
  the `PictureCacheEnabled` always-miss oracle (cache-on ≡ cache-off) (`Feature116PictureCacheTests`);
- the bounded cross-frame LRU — `PictureCacheEntryCount <= PictureCacheCap` under eviction pressure,
  deterministic eviction, evicted-entry re-miss (`Feature116CacheBoundTests`);
- the advisory offscreen-effect diagnostic — the pure `RetainedRender.offscreenEffect` detector + the
  wired `step` `Diagnostics` emission, fires/does-not-fire, output byte-identical
  (`Feature116OffscreenDiagTests`);
- the deterministic six `FrameMetrics` fields over `ControlsElmish.Perf.runScript`
  (`Feature116MetricsTests`) + the regenerated 109 perf-corpus goldens (incl. the new
  `picture-cache-reuse` / `picture-cache-eviction` scenarios);
- the standing Scene-parity golden suite under `Dev` for at-rest rendered-output + geometry byte-identity
  (FR-014) and for the optional SKPicture byte-identical-raster path.

A live window CAN open via the X11 path, but it is not part of this feature's required evidence — the
damage/cache metrics + the advisory diagnostic are observable via the deterministic `Perf.runScript`
metrics and the internal seam tests, not a live window. The live render staying byte-identical at rest is
covered by the Scene-parity suite under `Dev`.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A regression that repaints a stable subtree, widens localized damage to the whole frame, or blows the
  cache cap surfaces as a moved golden (`Feature116MetricsTests` + the 109 corpus) instead of silent cost.
- A stale picture-cache hit (a keyed input omitted from the correctness key) fails the per-keyed-input
  miss matrix in `Feature116PictureCacheTests`.
- A cache that grows unbounded fails the `PictureCacheEntryCount <= cap` assertion in
  `Feature116CacheBoundTests`.
- A non-byte-identical at-rest scene (cache-on ≠ cache-off, or the diagnostic altering output) fails the
  always-miss oracle / advisory-only assertions and/or the Scene-parity suite under `Dev`.

## Platform / runtime support boundary

Feature 116 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope. The 116 evidence is GPU-free deterministic damage/cache/diagnostic/metrics assembly over scene
lists, so it does not depend on the live Vulkan surface. The optional SKPicture record/replay (FR-008,
deferred this rung) would be a backend raster optimization with a byte-identical-pixels contract.
