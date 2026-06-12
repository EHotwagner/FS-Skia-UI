# View-free / cause-phase before/after delta (feature 111, T018 / FR-010 / SC-006)

The feature-109 corpus goldens were regenerated (`PERF_CORPUS_REGEN=1`) after the cause/phase fields +
the view-skip landed. Every golden line gains `FrameCause=... DiffRan=... LayoutRan=... PaintRan=...`.
The **animation-tick** frames additionally flip to view-free; all other frames keep their exact prior
count/bool values (FR-008 byte-identity of behaviour).

Goldens: `specs/109-perf-metrics-baseline/readiness/perf-corpus/<name>.golden.txt`.

## Animation ticks — now view-free (FR-004)

`text-entry-while-animating` (the only corpus scenario with animation-only ticks) — its tick frames
(frames 3 and 5) flip:

| Field | BEFORE (feature 110) | AFTER (feature 111) |
|-------|----------------------|---------------------|
| ViewCalled | true | **false** |
| FullRenderCount | 1 | **0** |
| (new) FrameCause | — | **Tick** |
| (new) DiffRan | — | **false** |
| (new) PaintRan | — | **true** |

The KEY (model-change) frames of the same scenario stay `ViewCalled=true`, `FullRenderCount=1`, and
gain `FrameCause=Key`, `DiffRan=true`, `PaintRan=true` — a model frame runs every phase.

## All scenarios — additive cause/phase fields (no behaviour delta)

`hover-sweep-*` (pure routing frame) → `FrameCause=PointerMove`, all four phase bools `false`
(view/diff/layout/paint all skipped — a move that routes only). `datagrid-*`/`deep-nested-layout`/
`theme-switch-dashboard` (model frames) → `FrameCause=Key`, `DiffRan=true`, `LayoutRan=(remeasure>0)`,
`PaintRan=true`, with their existing `ProductModelChanged`/`ViewCalled`/`FullRenderCount`/
`RemeasuredNodeCount` values unchanged. `continuous-drag-400` → `FrameCause=PointerMove`, all phase
bools `false`.

## Authority

The standing Scene-parity / golden suite under `Dev` remains the authority for at-rest rendered output +
geometry byte-identity ([byte-identity-authority.md](./byte-identity-authority.md)); no scene/geometry
golden moved — only the metric fields above changed.
