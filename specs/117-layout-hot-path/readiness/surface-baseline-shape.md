# Surface baseline shape (feature 117, T006)

The intended public + per-package surface delta this feature introduces. The authoritative regeneration is
`RefreshSurfaceBaselines` (T021); this note records the expected shape so the diff is reviewable.

## Top-level public surface (additive, no top-level baseline change)

- `FS.Skia.UI.Controls.Elmish.FrameMetrics` gains three public fields:
  `TextMeasureCacheHitCount: int`, `TextMeasureCacheMissCount: int`, `LayoutInvalidatedNodeCount: int`.

The top-level surface baseline (`readiness/surface-baselines/`) tracks type/member NAMES, not record
field details, so adding three `FrameMetrics` record fields leaves it unchanged (the `FrameMetrics` type
name was already listed). The field-level delta is captured by the per-package surface diff below.

## Per-package surface

- `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt` — the three `FrameMetrics` fields.
- `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` — the internal `RetainedRender` additions
  visible on the internal surface (`TextMeasureKey` / `TextMeasureCache` types, the `TextCache` /
  `TextCacheEnabled` fields, the `WorkReductionRecord` `TextMeasureCacheHits` / `TextMeasureCacheMisses` /
  `LayoutInvalidatedNodeCount` carriers, the `TextMeasureCacheCap` / `measureTextCached` vals) + the
  `ControlInternals` `measureText` / `setMeasureTextHook` internal vals.

No existing public signature changed shape; the public delta is purely additive (three fields). The
internal `RetainedRender` text-measure-cache / always-miss-flag / hook additions stay internal (reached
via `InternalsVisibleTo`).
