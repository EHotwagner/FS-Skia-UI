# Phase 0 Research — Layout Hot-Path Improvements (Feature 117)

All `NEEDS CLARIFICATION` items from Technical Context resolved below. Each decision names
the chosen approach, rationale, and the alternatives considered.

## R1 — Cache home: where the text-measure cache lives

- **Decision**: **Option B — interpose from `Controls`**, not inside `Scene.measureText`.
  The cache lives on `RetainedRender` (the same owner as the 116 picture cache and the 113
  memo cache) and is threaded through the retained `step`; the six `Control.fs` call sites
  and `fittedFontSize` resolve text measurement through a single internal helper
  (`measureTextCached cache font text`) that consults the cache, calls the un-cached
  `Scene.measureText` on a miss, and records hit/miss into the step's `WorkReductionRecord`.
- **Rationale**: keeps `Scene` dependency-light and **pure** (constitution: Scene is the
  dependency-light primitive layer; introducing per-call mutable state and an enable flag
  there would make a leaf primitive stateful and complicate the Scene-parity goldens).
  Hit/miss counting and the always-miss flag already have their home and precedent on
  `RetainedRender` (`MemoEnabled`/`PictureCacheEnabled`); reusing that seam means the counts
  thread to `FrameMetrics` by the proven 113/114/116 path with no new plumbing pattern.
- **Alternatives considered**: (A) a module-level cache inside `Scene.measureText` — rejected:
  global mutable state in a pure primitive, no clean per-frame hit/miss attribution, and an
  always-miss flag would have to be a `Scene`-level global, polluting the parity surface.
  (C) a `[<ThreadStatic>]` cache — rejected: non-deterministic across the test harness and
  unnecessary (the retained step is single-threaded per host).

## R2 — Cache key shape

- **Decision**: the key is the value tuple **`(text: string, family: string, size: float,
  weight: FontWeight option)`** — every field of the `(text, FontSpec)` pair that
  `Scene.measureText` reads (`Scene.fs:524-530` reads `font.Size` and `text.Length`; the
  full `FontSpec` is `{ Family; Size; Weight }`). Compared structurally (F# structural
  equality on the tuple). The measurement **constraint** (the available width/height passed
  to `fittedFontSize`) is **not** in the key.
- **Rationale**: FR-002 requires every input that affects the measured *result* to be keyed.
  `Scene.measureText` is a pure function of `(text, font)` only — the constraint does not
  change its output, it only steers which candidate sizes `fittedFontSize` probes, and each
  candidate size is already a distinct key via `font.Size`. Including the constraint would
  fragment the cache (lower hit rate) without any correctness benefit. Using the whole
  `FontSpec` value as part of the key (rather than just `Size`) future-proofs the key if
  `measureText` later reads `Family`/`Weight`; it is keyed now so a later read cannot
  silently serve a stale hit (FR-002 "no stale hit across a differing input").
- **Alternatives considered**: keying only on `(text, size)` — rejected: violates FR-002 if
  `Family`/`Weight` ever affect shaping; the edge case "changing only font weight/family
  MUST miss" demands they be keyed. Hashing the key to an `int` — deferred to implementation;
  a structural tuple key in a `Map`/`Dictionary` is simplest and deterministic.

## R3 — Hit/miss counting + always-miss oracle

- **Decision**: a resident key returns the cached `TextMetrics` and increments
  `TextMeasureCacheHits`; a missing/evicted key calls `Scene.measureText`, stores the
  result, and increments `TextMeasureCacheMisses`. A per-`RetainedRender` boolean
  **`TextCacheEnabled`** (default `true`, threaded frame-to-frame like `MemoEnabled`
  `RetainedRender.fs:932` and `PictureCacheEnabled` `:935`) forces every request to miss and
  re-measure when `false`, never consulting nor populating the cache.
- **Rationale**: this is the exact 113/116 oracle pattern. `TextCacheEnabled = false`
  produces the un-cached behaviour, so a test can assert cache-on output ≡ cache-off output
  and cache-on layout ≡ cache-off layout over the whole corpus (FR-004), and that the *only*
  observable delta is the hit/miss counts.
- **Alternatives considered**: a global "disable cache" env var — rejected: not per-host, not
  composable with the test harness; the per-record flag matches precedent and is reachable
  via `InternalsVisibleTo`.

## R4 — Bounded cache + deterministic eviction

- **Decision**: a fixed entry cap with **deterministic LRU eviction**, mirroring 116's
  `PictureCache` (`PictureCacheCap = 256`, deterministic traversal-order clock/LRU). Default
  cap **256** entries unless Phase 1 sizing finds a text-specific number; eviction order is a
  deterministic recency structure (insertion/last-use order), so a scenario measuring more
  than `cap` distinct strings evicts the least-recently-used entries in a reproducible order.
  An evicted key re-misses (re-measures, re-stores) when next needed — never a stale hit.
- **Rationale**: FR-003 demands a memory bound *and* reproducible hit/miss/eviction for
  goldens. Reusing the 116 cap + LRU shape keeps one cache discipline in the codebase and a
  known-deterministic eviction. The cap is the memory bound; under eviction pressure hit rate
  may fall and that is correct (spec interaction note: cap wins over hit rate).
- **Alternatives considered**: unbounded `Map` — rejected by FR-003 (unbounded growth).
  Random/`Date.now`-based eviction — rejected: non-deterministic, breaks goldens (and
  `Date.now`/`Math.random` are unavailable on this path anyway). FIFO instead of LRU —
  acceptable but LRU matches 116 and better fits the repeated-caption access pattern.

## R5 — Byte-identity

- **Decision**: caching changes only *how fast* a `TextMetrics` is produced, never *what*
  value. Because `Scene.measureText` is a pure deterministic function of `(text, font)`
  (`Scene.fs:524-530`: `Width = max 1.0 (size*0.58) * text.Length`, `Height = max 1.0
  font.Size`, `Baseline = size*0.8`), the cached value for a key equals the un-cached value
  for that key by construction. No layout box, fitted font size, DataGrid geometry, chart,
  or emitted flat `SubtreeScene` changes value. Proven by (a) the always-miss oracle
  (cache-on ≡ cache-off, FR-004) and (b) the standing Scene-parity golden suite under `Dev`.
- **Rationale**: the un-cached measurement is the oracle and the cache is a transparent
  accelerator (spec interaction note). Determinism of `measureText` is the property that
  makes a correct cache trivially byte-identical; the oracle test guards against any future
  impurity sneaking in.
- **Alternatives considered**: none — byte-identity is a hard requirement, not a design choice.

## R6 — Layout-invalidated node count (FR-006)

- **Decision**: `LayoutInvalidatedNodeCount` is the **size of the dirty set fed into
  incremental layout** — `Set.count` of the set produced by `layoutDirtySet`
  (`RetainedRender.fs:497-504`) and passed to `Layout.evaluateIncremental` (via
  `Control.fs:1307` `Set.toList dirty`). This is the **pre-fixed-size-ancestor-pinning**
  invalidated set, distinct from the existing `RemeasuredNodeCount`, which is the
  **post-pinning** re-measured set (`RetainedRender.fs:575` = `layoutResult.Invalidated |>
  List.length`, where `Layout.fs:718` builds `Invalidated` from the nodes actually
  re-measured after pinning). It is threaded into `WorkReductionRecord` as a new internal
  field alongside `RemeasuredNodeCount`.
- **Rationale**: FR-006 wants the dirty propagation *before* pinning reduced the work.
  `layoutDirtySet` already computes exactly that set; surfacing its count is reporting-only
  and adds no new mechanism. `invalidated >= remeasured` holds because pinning to a
  fixed-size ancestor can only *remove* nodes from the re-measured set relative to the dirty
  set (it never adds), so the post-pinning `Invalidated` list is a subset-cardinality of the
  pre-pinning dirty set. Idle and style-only / visual-state frames produce an **empty** dirty
  set (no layout-affecting attr changed — `layoutAffectingAttrNames` = `{width, height,
  orientation}` only, `Control.fs:1252`), so both counts are `0` (FR-007).
- **Threading detail to confirm in Phase 1**: the dirty set is computed in the retained
  `step` (RetainedRender) and the layout call may occur via `Control.fs:1307`; Phase 1 wires
  the `Set.count` at the point the set is built so `WorkReductionRecord` carries it without a
  second traversal. If the live path and `Perf.runScript` path compute the dirty set in
  different functions, both construction sites thread the same count (mirroring how
  `RemeasuredNodeCount` is threaded at `ControlsElmish.fs:1003/1425/1482`).
- **Alternatives considered**: reporting the post-pinning count again under a new name —
  rejected: redundant with `RemeasuredNodeCount` and fails the `>= RemeasuredNodeCount`
  contract (it would be equal, never the genuine dirty-propagation signal the spec wants).

## R7 — Fitted-caption interaction (`fittedFontSize`)

- **Decision**: `fittedFontSize` (`Control.fs:233-256`) calls `measureText` for the upper
  bound and for each binary-search candidate size — distinct `font.Size` values, hence
  **distinct cache keys**. The cache helps **across frames**: the same caption in the same
  box re-runs the *same* search path (same candidate sizes), so every probe after the cold
  frame is a hit. The cache MUST NOT change the chosen fitted size — guaranteed because each
  probe returns the byte-identical `TextMetrics` (R5), so the `fits`/`search` branches take
  the identical path.
- **Rationale**: the edge case requires the fitted size to be unchanged and the cache to help
  across frames. Distinct candidate sizes as distinct keys is the natural consequence of
  keying on `size`; no special handling is needed.
- **Alternatives considered**: caching the *fitted result* (caption + box → chosen size) —
  deferred; out of scope (it is a different, higher-level cache; this rung caches the
  measurement primitive, and the per-candidate cache already removes the repeated
  per-frame search cost).

## R8 — Interaction with 113 (memo), 114 (virtualization), 116 (picture cache)

- **Decision**: the text-measure cache is **distinct** from the 113 memo cache (which memoizes
  the DataGrid `gridGeom` projection) and the 116 picture cache (which caches repaint
  fragments). The three caches compose: a frame can report memo hits, picture-cache hits, and
  text-cache hits independently. Over a virtualized row set (114), only materialized rows
  measure text, so `TextMeasureCacheHits/Misses` aggregate over the materialized rows; a row
  scrolled out stops contributing; a cached text key survives until evicted. The new
  `LayoutInvalidatedNodeCount` aggregates over the same step as `RemeasuredNodeCount`.
- **Rationale**: each cache keys on a different thing (gridGeom dep vs. render fragment vs.
  text+font), so there is no double-counting; the metrics are independent counters on the
  same `WorkReductionRecord`.
- **Alternatives considered**: folding text measurement into the memo cache — rejected: the
  memo cache keys on control identity + boxed deps, not on `(text, font)`; merging would lose
  the cross-control reuse of identical captions that is the whole point of FR-001.

## R9 — Corpus scenarios (FR-010)

- **Decision**: add deterministic `Perf.runScript` scenarios to the 109 corpus:
  1. **Text-heavy cold→warm**: a surface with many repeated identical captions/labels.
     Frame 1 (cold) reports `TextMeasureCacheMissCount > 0`, `HitCount = 0` for first-seen
     keys. A re-layout frame with unchanged text reports `HitCount > 0`, `MissCount = 0`.
     (SC-001/SC-002.)
  2. **Style-only / visual-state frame**: a hover/focus/anim-tick over a text-bearing control
     reports `RemeasuredNodeCount = 0`, `LayoutInvalidatedNodeCount = 0`,
     `TextMeasureCacheMissCount = 0`. (SC-003.)
  3. **Idle frame**: all three new fields `0`. (FR-005/FR-006 zero rule.)
  4. **Geometry frame**: a width/height/orientation change reports
     `LayoutInvalidatedNodeCount >= RemeasuredNodeCount`, both bounded/explainable. (SC-006.)
  5. **Cache-cap eviction**: a layout measuring more than `cap` distinct strings completes
     with bounded memory and a deterministic hit/miss/eviction sequence. (SC-005.)
- **Rationale**: these scenarios are the deterministic proofs for SC-001..SC-006 and are
  golden-asserted on the real `Perf.runScript` path (no synthetic evidence). Regenerated with
  `PERF_CORPUS_REGEN=1`; the three new metric fields are carried in every existing golden too
  (they read `0` on frames that measure no text / change no geometry).
- **Alternatives considered**: asserting timing — rejected: timing-based gates are explicitly
  out of scope; counts are the deterministic contract.

## Open items carried to Phase 1

- Exact cache-state representation (`Dictionary` + recency list vs. an LRU record) and whether
  the entry count is exposed as a non-golden internal invariant (proven by test) rather than a
  `FrameMetrics` field — Phase 1 / data-model decides (the spec adds **no** public entry-count
  field, unlike 116's `PictureCacheEntryCount`; the cap is an internal invariant).
- Whether the live path and `Perf.runScript` path share one `measureTextCached` helper or
  thread the count at two construction sites — Phase 1 wires it to match the
  `RemeasuredNodeCount` threading shape.
