# Real image evidence (080) — decoded per-control confirmation

- **Authoritative command**: `dotnet run --project tests/ControlsPreview.Harness -- --render`
  (regenerate) then `-- --fidelity` (decode each `docs/img/controls/<id>.png`).
- **Artifact path**: `docs/img/controls/*.png` (51 Demonstrative previews, 320×160, renderer
  mode `viewer-render-target`/`ViewerRenderTargetPng`); decoded results in
  `readiness/control-fidelity.md`. `custom-control` is honestly `Unsupported` (no image).
- **Failure class**: a Demonstrative preview whose decoded content is a label-on-a-box (≈0
  coverage outside the title band) is a product defect; a metadata-only or 1×1 fallback does
  not satisfy visual proof.

## Family-recognition check (T014, SC-002)

Each family was rendered through the real render-only path and decoded; each shows
control-specific structure, **not** a single word on a box (coverage = fraction of
non-background pixels outside the 28-px title band; kinds from the live `Scene.describe`):

| family | exemplar | coverage | distinct colours | primitive kinds | recognized as |
|--------|----------|----------|------------------|-----------------|---------------|
| chart (line) | line-chart | 0.4281 | 22 | PathElement, CircleElement | polyline + filled area + point markers |
| chart (bar) | bar-chart | 0.3863 | 4 | RectangleElement ×4 | one bar per data point |
| chart (pie) | pie-chart | 0.1062 | 28 | ArcElement ×3 | three coloured slices |
| chart (scatter) | scatter-plot | 0.0056 | — | CircleElement ×4 | a mark per point |
| chart (graph) | graph-view | 0.0218 | — | CircleElement + LineElement | nodes joined by edges |
| collection | list-box | 0.5917 | 53 | RectangleElement rows + TextRunElement | distinct item rows, one highlighted |
| value / selection | slider | 0.0301 | 2 | RectangleElement ×2 + CircleElement | track + thumb mid-track |
| selection | radio-group | 0.0310 | 14 | CircleElement ×3 | options, one marked |
| value | progress-bar | 0.1076 | 2 | RectangleElement ×2 | track + partial fill |
| image | image | 0.0819 | 29 | RectangleElement frame + LineElement | framed placeholder |
| icon | icon | 0.0228 | 3 | PathElement | font-independent vector glyph (no `.notdef`) |
| layout | stack | 0.4091 | 4 | RectangleElement + TextRunElement | stacked child rows |

## Per-control decoded confirmation (T026, SC-004)

All 51 Demonstrative previews PASS their per-control `ContentSignature` and the
`custom-control` entry is `Unsupported` with no committed image — see the full per-row table in
[`control-fidelity.md`](./control-fidelity.md). The gate is **build-enforced** by
`ControlFidelityCheck` (FAKE target, exit 0). Zero unverifiable per-control visual claims
remain: every claim in the catalog Preview sections is "a deterministic render-only preview
… rendered against the default theme", which the decoded image content supports.

The retained fixture matrix proves discrimination: all 13 `lowfi` (079 label-on-box) fixtures
**fail** their signature (coverage 0.0000 outside the band) and all 13 `faithful` counterparts
**pass** — 26/26 fixtures match expectation.
