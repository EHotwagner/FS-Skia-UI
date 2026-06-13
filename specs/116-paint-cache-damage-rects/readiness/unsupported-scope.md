# Unsupported scope & deferral (feature 116)

This feature is the performance report's **Phase 7 in full** (paint cache + damage rectangles + bounded
cache memory + offscreen-effect diagnostics). Explicitly OUT this rung:

- **layout hot-path / text-measurement caches & layout-boundary hints / structural flattening** — Phase 8.
- **`SkiaViewer` frame-scheduling, readback separation, scene-submission / layer-skipping, render-thread /
  compositor split** — Phase 9 (beyond the byte-identical SKPicture record/replay this rung *optionally*
  adds; that optional backend realization is T023, deferred `[-]` — the optional MAY, FR-008).
- **non-axis-aligned or sub-pixel damage rectangles** — axis-aligned integer only (FR-016).
- **draw-call batching** (Qt-style).
- **damage-driven partial-present** — this rung adds the damage *signal*, not damage-scoped presentation
  (the backend still presents the whole frame).
- **a spatial union/merge coalescer for `DirtyArea`** — the summed distinct-box area is the deterministic
  default this rung (research §a); union is a deferred plan option.
- **generalizing the picture cache beyond `data-grid-row`** — the row is the representative boundary this
  rung (the 113 data-grid-only precedent).

No renderer rewrite, no Avalonia/WPF redesign, no platform/release/distribution scope. Features 109–114
are unchanged (FR-015): feature 113's memo cache (a distinct complementary cache) and feature 114's
virtualization continue to work — the damage/cache metrics aggregate correctly over the virtualized row
set.

## Applicability of cross-cutting principles

- **Principle IV (Elmish/MVU)** — N/A: `Update` / effects / subscriptions / commands / interpreter are
  unchanged; the picture cache is interpreter-edge mutation confined to the retained step (constitution
  III), exactly as the existing id/work counters and the 113 memo cache; `view` / `update` stay pure;
  dispatch outcomes are byte-identical (FR-014).
- **Interactive-UI run-and-use gate** — N/A: the feature delivers an internal damage/picture-cache
  contract + deterministic metrics observable via `ControlsElmish.Perf.runScript` plus an advisory
  diagnostic, not a new interactive surface.

## Failure diagnostics

A missing required evidence artifact fails `Route --enforce` (it names the artifact + requiring tier). A
race-like or unknown-concurrent-FAKE failure is reran sequentially before any product-debugging
classification (shared `.fake` state).
