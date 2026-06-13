# Contract: Fully-Keyed, Bounded Picture Cache

Surface: public `PictureCacheHitCount` / `PictureCacheMissCount` / `PictureCacheEntryCount`
`FrameMetrics` fields (`ControlsElmish.fsi`); internal correctness key + bounded LRU cache +
`PictureCacheEnabled` always-miss flag on `RetainedRender`. (FR-005, FR-006, FR-007, FR-008,
FR-009, FR-010; US2, US3.)

## The cache boundary

The existing reusable retained fragment (`{ pr with Control = nc }`, `RetainedRender.fs:543`)
**is** the picture-cache boundary for a stable subtree. This feature names it, widens its
key, counts its outcome, and bounds its cross-frame store.

## Correctness key (complete)

A subtree is reused (a **hit**) **only** when *every* render-affecting input is unchanged:

- **theme**, **box**, **clip**, **opacity**, **transform**, **font/text**, **visual-state**.

A change to **any** one keyed input invalidates the entry (a **miss**, repainting the
subtree). No keyed input may be omitted — a stale hit is a defect.

### Guarantees

1. **Hit ⇒ byte-identical.** A `PictureCacheHitCount` hit produces rendered output byte-
   identical to repainting the subtree fresh (the hit reuses the same fragment instance).
2. **Per-input invalidation.** Changing exactly one keyed input (theme | box | clip |
   opacity | transform | font/text | visual-state) independently forces a
   `PictureCacheMissCount` miss with correct fresh output — for **every** input. (This is the
   proof the key omits none.)
3. **Cache-on ≡ cache-off.** Under the `PictureCacheEnabled = false` always-miss oracle
   (mirroring 113's `MemoEnabled`), every subtree repaints; the rendered scene is byte-
   identical to the cache-enabled build. `PictureCacheHitCount = 0` when disabled.

## Bounded memory

The cross-frame picture cache is **capped by entry count** with **deterministic LRU
eviction**. The cap is the named constant `PictureCacheCap = 256` entries
(`RetainedRender.fs`) — above every corpus scene's stable-subtree count (so steady-state
corpus frames never evict) and below the eviction-pressure scenario, which drives **320**
distinct cacheable row identities (`1.25 × cap`, forcing ≥ 64 evictions). These two
numbers (`256` cap, `320` distinct identities) are what the bound/eviction tests assert.

### Guarantees

4. **Bounded.** `PictureCacheEntryCount <= cap` at all times, even under eviction pressure
   (more distinct cacheable subtrees than the cap).
5. **Deterministic eviction.** The same input sequence yields the same surviving entries
   (recency derives from deterministic traversal order, not wall-clock).
6. **Evicted ⇒ re-miss, never stale.** An evicted entry recomputes as a miss (fresh, correct
   paint) when next needed.
7. **Observable.** `PictureCacheEntryCount` reflects live size deterministically; raw byte
   size is a **non-golden** diagnostic only (excluded, like `FrameDuration`).

## Optional backend realization (byte-identical raster)

The optional Skia picture record/replay (`SceneRenderer`, FR-008) wraps a stable cached
boundary in the existing `Scene.Picture`/`PictureNode` and records/replays a real
`SKPicture`. Its contract is **byte-identical raster**; it does **not** change the
deterministic flat `SubtreeScene` the goldens assert (the deterministic contract is the
hit/miss counts + damage metrics at the scene-list level). At-rest fallback is the existing
passthrough (`SceneRenderer.fs:393`).

## Evidence

- `tests/Controls.Tests/Feature116PictureCacheTests.fs` — per-keyed-input miss + hit byte-
  identity + always-miss oracle (cache-on ≡ cache-off).
- `tests/Controls.Tests/Feature116CacheBoundTests.fs` — `EntryCount <= cap`, deterministic
  eviction, evicted-entry re-miss.
- `tests/Elmish.Tests/Feature116MetricsTests.fs` + regenerated corpus goldens (stable-subtree
  reuse + cache-cap eviction scenarios, `PERF_CORPUS_REGEN=1`).
- SKPicture byte-identical raster via the standing Scene-parity suite under `Dev`.
