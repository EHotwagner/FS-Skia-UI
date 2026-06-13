# Phase 1 Data Model — Layout Hot-Path Improvements (Feature 117)

This rung adds **no new domain entity**. It adds an internal cache + flag on `RetainedRender`,
three internal carrier fields on `WorkReductionRecord`, and three public fields on
`FrameMetrics`. Everything else is byte-identical.

## 1. Text-measure cache key (internal)

The cache key is the value tuple of every input `Scene.measureText` reads:

```fsharp
// internal — lives with RetainedRender (Controls), reached via InternalsVisibleTo
type internal TextMeasureKey =
    { Text: string
      Family: string
      Size: float
      Weight: FontWeight option }   // = the (text, FontSpec) pair, structurally compared
```

- Built from `(text, font)` at each call site (`FontSpec = { Family; Size; Weight }`).
- Structural equality / hashing (F# default on the record) → deterministic lookup.
- Two requests differing in **any** field are distinct entries (FR-002). The measurement
  *constraint* (available width/height in `fittedFontSize`) is **not** a key field — it does
  not change `measureText`'s output (research R2).

## 2. Bounded text-measure cache state (internal, on `RetainedRender`)

```fsharp
// internal cache state, mirroring the 116 PictureCache discipline
//   - fixed capacity (default 256, aligned with PictureCacheCap)
//   - deterministic LRU eviction (recency structure, reproducible order)
//   - mutable accumulator confined to the retained step  // mutable: hot path
type internal TextMeasureCache =
    { Entries: Map<TextMeasureKey, TextMetrics>   // or Dictionary; Phase: see quickstart
      Recency: ...                                 // deterministic last-use order
      Cap: int }
```

- **Lookup**: resident key → return `TextMetrics`, bump recency, count a **hit**.
- **Miss**: absent/evicted key → `Scene.measureText text font`, insert (evicting LRU if at
  cap), count a **miss**. An evicted key re-misses next time (never a stale hit) (FR-003).
- **Invariant**: `Entries.Count <= Cap` always. This is an **internal invariant proven by
  test**, not a public `FrameMetrics` field (the spec adds no public entry-count counter,
  unlike 116's `PictureCacheEntryCount`).

## 3. Always-miss oracle flag (internal, on `RetainedRender`)

```fsharp
TextCacheEnabled: bool   // default true; threaded frame-to-frame like MemoEnabled / PictureCacheEnabled
```

- `false` → every request misses and re-measures, never consulting/populating the cache.
- Used by the byte-identity oracle test: cache-on output/layout ≡ cache-off output/layout
  (FR-004). Mirrors `MemoEnabled` (`RetainedRender.fs:81`/`.fsi:130`) and `PictureCacheEnabled`
  (`:87`/`.fsi:145`), including init (`:479/:482`) and frame-to-frame carry (`:932/:935`).

## 4. `WorkReductionRecord` carriers (internal, additive)

Three new fields on the existing `internal WorkReductionRecord` (`RetainedRender.fsi:157-205`):

```fsharp
TextMeasureCacheHits: int          // resident-key reuses this frame
TextMeasureCacheMisses: int         // fresh measures this frame
LayoutInvalidatedNodeCount: int     // Set.count of the dirty set fed to incremental layout
                                    //   (pre-pinning), distinct from RemeasuredNodeCount (post-pinning)
```

- Constructed at `RetainedRender.fs:943` alongside `RemeasuredNodeCount = remeasured`.
- `LayoutInvalidatedNodeCount = Set.count dirty` where `dirty` is the `layoutDirtySet` output
  (`RetainedRender.fs:497-504`) fed to `evaluateIncremental` (`Control.fs:1307`). Always
  `<= RemeasuredNodeCount` (the pre-pinning dirty set is a subset of the post-pinning re-measured boundary subtrees; direction corrected 2026-06-13).

## 5. `FrameMetrics` public fields (additive, breaking `ControlsElmish.fsi`)

Three new public integer fields on `FrameMetrics` (`ControlsElmish.fsi:68-174`), each with
`///` XML-doc (attribute-before-doc-before-type ordering preserved):

```fsharp
/// Count of text-measurement cache hits this frame (resident key reused without re-shaping).
TextMeasureCacheHitCount: int
/// Count of text-measurement cache misses this frame (text measured fresh and cached).
TextMeasureCacheMissCount: int
/// Size of the layout dirty set fed into incremental layout this frame (pre-pinning),
/// distinct from and <= RemeasuredNodeCount.
LayoutInvalidatedNodeCount: int
```

- Added to the `zero` record (`ControlsElmish.fs:1366-1388`) initialized to `0`.
- Threaded from `WorkReductionRecord` at every construction site:
  - live `OnFrameMetrics` sink (`ControlsElmish.fs:1003`),
  - `Perf.runScript` move/tick/key/pointer frames (`:1421-1442`, `:1478-1496`, following the
    `{ zero with … }` pattern), mirroring `RemeasuredNodeCount`/`MemoHitCount`.
- `0` on a frame that measures no text / changes no geometry (FR-005/FR-006 zero rule).

## 6. Field-value rules (the contract, by frame type)

| Frame type | TextCacheHit | TextCacheMiss | LayoutInvalidated | Remeasured |
|---|---|---|---|---|
| Idle | 0 | 0 | 0 | 0 |
| Cold text-heavy (first) | 0 (first-seen) | > 0 | (per geometry) | (per geometry) |
| Warm text-heavy (unchanged text) | > 0 | 0 | 0 (if no geometry change) | 0 |
| Style-only / visual-state | 0 misses; hits = unchanged-text measures (often 0 if no re-measure) | 0 | 0 | 0 |
| Geometry (width/height/orientation) | per text reuse | per new text | bounded, **<= Remeasured** | >= Invalidated |
| Cache-cap eviction | deterministic | deterministic (evicted re-miss) | per geometry | per geometry |

- **Byte-identity (FR-004)**: for any key, cached `TextMetrics` == un-cached `TextMetrics`;
  rendered scene, layout boxes, and fitted font sizes are unchanged across the whole corpus.
- **Theme switch**: font family/size change → legitimate misses + re-measure (correct, not a
  cache failure).

## 7. Unchanged (explicitly preserved)

- `Model` / `Msg` / `Effect` / `init` / `update` / subscriptions / interpreter — untouched.
- `Scene.measureText` stays a pure function (the cache wraps it from `Controls`, research R1).
- `layoutAffectingAttrNames` (`Control.fs:1252` = `{ AttrWidth; AttrHeight; AttrOrientation }`)
  and `layoutDriftReport` (feature 101) — unchanged; no new geometry-driving attribute (FR-008).
- No multi-pass / intrinsic layout path is introduced; no multi-pass metric is added (FR-009).
- 113 memo cache, 114 virtualization, 116 picture cache + damage rects — unaffected,
  independent counters on the same step (research R8).
