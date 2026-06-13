# US1 independent validation — text-heavy frames stop re-measuring identical text

**Story**: A consumer renders a text-dense surface; a warm frame whose text inputs did not change reuses
cached measurements (hits, zero misses); a cold frame measures fresh (misses).

## Path

Drive a repeated-caption text-heavy layout cold→warm through `ControlsElmish.Perf.runScript` (a
style-flip repaints every row each step, re-measuring the UNCHANGED text); assert the cold frame reports
text-cache misses and the warm frame reports hits + zero misses. Exercise the per-keyed-input miss matrix,
the hit byte-identity, and the always-miss oracle directly over the pure `RetainedRender.measureTextCached`.

## Evidence

- `tests/Controls.Tests/Feature117TextCacheTests.fs` — a cold key misses; the identical key hits with
  byte-identical metrics; each of text/family/size/weight independently forces a miss with the correct
  fresh metrics (FR-002); the always-miss oracle re-measures every request and never populates the cache
  (cache-on ≡ cache-off, FR-004); empty/whitespace text caches without error; a fitted-caption's distinct
  candidate sizes are distinct keys that hit across frames; a real retained step with the cache disabled
  emits a byte-identical scene + bounds + remeasure count vs cache-enabled, and equals a fresh full rebuild.
- `tests/Controls.Tests/Feature117CacheBoundTests.fs` — `Entries.Count <= cap` at every insert,
  deterministic eviction, evicted-entry re-miss (FR-003, SC-005).
- `tests/Elmish.Tests/Feature117MetricsTests.fs` — over `Perf.runScript`, the cold frame reports
  `TextMeasureCacheMissCount > 0` / `HitCount = 0` and the warm frame reports `HitCount > 0` /
  `MissCount = 0` (SC-001/SC-002).
- 109 perf-corpus `text-heavy-cold-warm.golden.txt` — frame 2: `Hit=0 Miss=40`; frame 3: `Hit=40 Miss=0`
  (cold → warm, deterministic).

Result: PASS — cold frames measure fresh; warm frames reuse cached measurements (SC-001/SC-002/SC-004).
