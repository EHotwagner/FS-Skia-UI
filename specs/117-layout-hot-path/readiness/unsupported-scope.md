# Unsupported scope & deferral (feature 117)

This feature is the performance report's **Phase 8 (Layout Hot-Path Improvements)** — a bounded
text-measure cache + dirty-propagation observability. Explicitly OUT this rung:

- **structural-wrapper flattening** (report task 4) — risks semantic change and has no byte-identical
  guarantee, so it is not attempted here.
- **intrinsic / multi-pass layout introduction** and any multi-pass metric (report optional task 5) — no
  such path exists; this rung must NOT create one (FR-009). The single-measure-pass contract is verified
  negatively: no new multi-pass metric is added and the still-empty layout drift guard holds.
- **`SkiaViewer` frame-scheduling, readback separation, scene-submission / layer-skipping, render-thread /
  compositor split** — Phase 9.
- **GPU / layer caching.**
- **any timing-based pass/fail gate** — the metrics are counts, not durations.
- the text-cache raw entry-count / byte-size as a PUBLIC `FrameMetrics` field — it is an internal
  invariant proven by test (`Entries.Count <= cap`), unlike 116's `PictureCacheEntryCount`.

No renderer rewrite, no platform/release/distribution scope. Features 109–116 are unchanged (FR-008):
feature 113's memo cache, feature 114's virtualization, and feature 116's picture cache + damage rects are
distinct complementary caches/metrics on the same retained step and continue to work; the text-cache +
layout-invalidated counts aggregate correctly over the virtualized (114) row set.

## Applicability of cross-cutting principles

- **Principle IV (Elmish/MVU)** — N/A: `Update` / effects / subscriptions / commands / interpreter are
  unchanged; the text-measure cache is interpreter-edge mutation confined to the retained step
  (constitution III), exactly as the existing id/work counters, the 113 memo cache, and the 116 picture
  cache; `view` / `update` stay pure; dispatch outcomes are byte-identical (FR-004).
- **Interactive-UI run-and-use gate** — N/A: the feature delivers an internal text-measure-cache contract
  + deterministic metrics observable via `ControlsElmish.Perf.runScript`, not a new interactive surface.

## Failure diagnostics

A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier). A
race-like or unknown-concurrent-FAKE failure is reran sequentially before any product-debugging
classification (shared `.fake` state).
