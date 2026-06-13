# Text-measure-cache authority (feature 117)

The authoritative definition of the text-measure cache's key, bound, eviction, and always-miss oracle.

## Cache home + interposition

The cache lives on `RetainedRender` (the 113/116 cache home, research R1), threaded frame-to-frame.
`Scene` stays a pure, dependency-light primitive — no mutable state or enable flag is added there. The six
`Control.fs` text-measure call sites + `fittedFontSize` route through `ControlInternals.measureText`,
which consults a `[<ThreadStatic>]`-backed hook the retained `step` installs (a closure over its working
cache + counters) and clears. `[<ThreadStatic>]` so concurrent test `step`s never cross-contaminate; the
persistent cache itself lives on `RetainedRender`, so frame-to-frame reuse is deterministic.

## Correctness key (every input that affects the measured result)

`TextMeasureKey = { Text: string; Family: string option; Size: float; Weight: int option }` — every field
of the `(text, FontSpec)` pair `Scene.measureText` reads. Two requests differing in ANY field are distinct
entries (FR-002): the per-keyed-input miss matrix (`Feature117TextCacheTests`) asserts each of
text/family/size/weight independently forces a miss with the correct fresh metrics, so no input can be
omitted and no stale hit can cross a differing input. The measurement CONSTRAINT (the available-space box
in `fittedFontSize`) is deliberately NOT keyed: it does not change `measureText`'s output, only which
candidate sizes the search probes, and each candidate size is already a distinct key via `Size` (research
R2).

## Bound + deterministic eviction

`RetainedRender.TextMeasureCacheCap = 256` (a plain named value, aligned with `PictureCacheCap`). Recency
is a monotonic `Clock` advanced by measurement order (no wall-clock). Over the cap the least-recently-used
entry is dropped; a dropped key re-misses (re-measures, re-stores) when next needed — never a stale hit
(FR-003). The eviction-pressure scenario drives more distinct strings than the cap; `Entries.Count <= cap`
holds at EVERY insert (`Feature117CacheBoundTests`). Under a working set larger than the cap the cache
thrashes (deterministic re-misses every frame) — the cap wins over hit rate (spec interaction note
FR-003 vs FR-005), captured honestly in the `text-cache-eviction` corpus golden.

## Always-miss oracle (FR-004)

`TextCacheEnabled = false` forces every request to re-measure and count a miss, never consulting or
populating the cache, proving cache-on output/layout ≡ cache-off output/layout. See
[byte-identity-authority.md](./byte-identity-authority.md).

## Pure helper

`RetainedRender.measureTextCached cache enabled text font : TextMetrics * TextMeasureCache * bool` is the
pure, total lookup the step's closure wraps; the unit tests drive it directly (cold→warm, per-keyed-input
miss, empty/whitespace, fitted-candidate keys, bounded eviction) without going through a whole frame.

## Evidence

`tests/Controls.Tests/Feature117TextCacheTests.fs`, `Feature117CacheBoundTests.fs`,
`tests/Elmish.Tests/Feature117MetricsTests.fs`, and the regenerated 109 perf-corpus goldens
(`text-heavy-cold-warm`, `text-cache-eviction`).
