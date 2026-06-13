# Contract — Text-Measure Cache (Feature 117)

Covers FR-001, FR-002, FR-003, FR-004, FR-005, FR-010. The cache wraps `Scene.measureText`
from `Controls` (research R1); the un-cached measurement is the oracle and the cache is a
transparent accelerator.

## C1 — Hit only when every keyed input is unchanged (FR-001/FR-002)

- A measurement request whose key `(Text, Family, Size, Weight)` is **resident** returns the
  cached `TextMetrics` **without** re-invoking `Scene.measureText`, and counts a **hit**.
- A request whose key is **absent or evicted** calls `Scene.measureText`, stores the result,
  and counts a **miss**.
- Two requests differing in **any** keyed field are distinct entries — no stale hit across a
  differing input.

**Proof**: for each field independently (text, family, size, weight), issue two requests
differing only in that field; assert the second is a miss with the correct fresh `TextMetrics`
(`tests/Controls.Tests/Feature117TextCacheTests.fs`).

## C2 — Hit is byte-identical to the un-cached measure (FR-004)

- For any key, the cached `TextMetrics.Width`/`Height`/`Baseline` **equals** the value
  `Scene.measureText` would return for that key.
- No rendered scene, layout box, or fitted font size changes value because the cache exists.

**Proof**: the **always-miss oracle** — run any corpus scenario with `TextCacheEnabled = false`
(cache-off) and `= true` (cache-on); assert identical emitted `SubtreeScene`, identical layout
boxes, and identical fitted font sizes. Plus the standing Scene-parity golden suite under `Dev`.

## C3 — Bounded cap + deterministic eviction (FR-003)

- `Entries.Count <= Cap` at all times (default `Cap = 256`, aligned with 116
  `PictureCacheCap`).
- Eviction is **deterministic LRU**: a scenario measuring more than `Cap` distinct strings
  evicts the least-recently-used entries in a reproducible order.
- An evicted key **re-misses** (re-measures, re-stores) when next needed — never a stale hit.

**Proof**: a layout measuring `> Cap` distinct strings; assert entry count never exceeds
`Cap`, the hit/miss/eviction sequence is reproducible across runs, and a deliberately-evicted
key re-misses (`tests/Controls.Tests/Feature117CacheBoundTests.fs`).

## C4 — Cold-miss → warm-hit, golden-asserted (FR-005/FR-010)

- The cold first frame of a text-heavy scenario reports `TextMeasureCacheMissCount > 0` and
  `HitCount = 0` for first-seen keys.
- A re-layout frame whose text inputs did not change reports `HitCount > 0` and
  `MissCount = 0`.
- A frame that measures no text reports both counters `0`.

**Proof**: `Perf.runScript` text-heavy scenario, deterministic goldens
(`tests/Elmish.Tests/Feature117MetricsTests.fs`; corpus regenerated with `PERF_CORPUS_REGEN=1`).

## C5 — Edge cases

- **Empty / whitespace text**: measures and caches without error; byte-identical to today.
- **Fitted captions** (`fittedFontSize`, `Control.fs:233-256`): each binary-search candidate
  size is a **distinct key**; the cache helps across frames (same caption + same box ⇒ same
  search path, every probe after the cold frame is a hit); the **chosen fitted size is
  unchanged** (each probe returns byte-identical `TextMetrics`, so `fits`/`search` take the
  identical path).
- **Theme switch**: font family/size change ⇒ legitimate misses + re-measure (correct, not a
  cache failure).

## C6 — Interaction resolution (from the spec)

- **Byte-identity (FR-004) vs. caching (FR-001)** — correctness wins: the un-cached
  measurement is the oracle; the cache only changes how fast a value is produced, never what.
- **Bounded cache (FR-003) vs. high hit rate (FR-005)** — the cap wins: under eviction
  pressure hit rate may fall; goldens assert the deterministic outcome at the cap, not a
  maximal hit rate.
