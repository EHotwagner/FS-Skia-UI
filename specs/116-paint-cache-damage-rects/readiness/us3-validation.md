# US3 independent validation — picture-cache memory is bounded and observable

**Story**: The cross-frame picture cache is capped by entry count with deterministic LRU eviction; an
evicted entry re-misses (never stale).

## Path

Drive more distinct cacheable rows than the cap (320 distinct rows = 1.25 × the 256 cap) and assert
`PictureCacheEntryCount <= PictureCacheCap` at all times; assert the surviving-entry set is identical
across runs (deterministic eviction); assert the recomputed scene under eviction pressure is byte-
identical to a fresh rebuild (an evicted entry re-misses with correct fresh paint, never a stale hit).

## Evidence

- `tests/Controls.Tests/Feature116CacheBoundTests.fs` — a 10-row grid keeps 10 live entries with 10 hits
  (no eviction below the cap); a 320-row grid keeps `PictureCacheEntryCount = PictureCacheCap = 256`
  (`<= cap`); the surviving-entry identity set is identical across two runs (deterministic eviction); the
  recomputed scene under pressure is byte-identical to `Control.renderTree` (evicted → re-miss → fresh
  correct paint, never stale).
- `tests/Elmish.Tests/Feature116MetricsTests.fs` — a 320-row grid over `Perf.runScript` reports
  `PictureCacheEntryCount <= cap` (== 256).
- 109 perf-corpus golden `picture-cache-eviction.golden.txt` (regenerated) — `PictureCacheEntryCount=256`
  with `PictureCacheMissCount=320` under eviction pressure.

Cap (pinned in [picture-cache-authority.md](./picture-cache-authority.md)): `RetainedRender.PictureCacheCap
= 256`, above a small grid's stable-row count and below the 320-row eviction scenario. Recency derives
from the frame's deterministic traversal order (no wall-clock).

Result: PASS — the cache is bounded, deterministic, and never serves a stale hit (SC-004).
