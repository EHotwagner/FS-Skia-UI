# Runtime limitations & failure diagnostics (feature 117)

## Documented evidence path

Feature 117 is a **text-measure-cache + dirty-propagation-observability** change proven by deterministic,
headless evidence; a live Vulkan window is **not required** (spec *Unsupported scope*). The asserted
surfaces:

- the bounded text-measure cache — cold→warm (cold miss, warm hit), per-keyed-input miss
  (text/family/size/weight), hit byte-identity, empty/whitespace, fitted-caption distinct keys, and the
  `TextCacheEnabled` always-miss oracle (cache-on ≡ cache-off) (`Feature117TextCacheTests`);
- the bounded eviction — `Entries.Count <= TextMeasureCacheCap` at every insert, deterministic eviction,
  evicted-entry re-miss (`Feature117CacheBoundTests`);
- the `LayoutInvalidatedNodeCount` — idle = 0, style-only = 0/0, geometry bounded with
  `LayoutInvalidatedNodeCount <= RemeasuredNodeCount`, drift-guard set unchanged
  (`Feature117LayoutInvalidatedTests`);
- the three deterministic `FrameMetrics` fields over `ControlsElmish.Perf.runScript`
  (`Feature117MetricsTests`) + the regenerated 109 perf-corpus goldens (incl. the new
  `text-heavy-cold-warm` / `text-cache-eviction` scenarios);
- the standing Scene-parity golden suite under `Dev` for at-rest rendered-output + geometry byte-identity
  (FR-004).

A live window CAN open via the X11 path, but it is not part of this feature's required evidence — the
cache + invalidated metrics are observable via the deterministic `Perf.runScript` metrics and the internal
seam tests, not a live window. The live render staying byte-identical at rest is covered by the
Scene-parity suite under `Dev`.

## Failure diagnostics

- A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier).
- A regression that re-shapes identical text or silently widens the dirty set surfaces as a moved golden
  (`Feature117MetricsTests` + the 109 corpus) instead of silent cost.
- A stale text-cache hit (a keyed input omitted from the key) fails the per-keyed-input miss matrix in
  `Feature117TextCacheTests`.
- A cache that grows unbounded fails the `Entries.Count <= cap` assertion in `Feature117CacheBoundTests`.
- A non-byte-identical at-rest scene (cache-on ≠ cache-off) fails the always-miss oracle assertion and/or
  the Scene-parity suite under `Dev`.

## Platform / runtime support boundary

Feature 117 touches the framework wherever it runs: a **.NET 10 desktop** host rendering through
**Vulkan** via the **SkiaSharp preview** native binding, on Windows and Linux desktop (`net10.0`).
**unsupported macOS/mobile/browser** — there is **no software-renderer fallback**; those targets are out
of scope. The 117 evidence is GPU-free deterministic cache/metrics assembly over scene-list measurement,
so it does not depend on the live Vulkan surface.
