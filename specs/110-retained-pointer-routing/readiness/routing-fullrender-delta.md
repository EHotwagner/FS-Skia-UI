# Routing full-render before/after delta (feature 110, T024 / FR-010 / SC-007)

The feature-109 corpus pointer goldens were regenerated (`PERF_CORPUS_REGEN=1`) after wiring the
retained pointer route. The deterministic, byte-stable per-frame `FrameMetrics` for the **pointer**
scenarios show the routing full-render count dropping to **zero**, while the non-pointer (key / tick /
theme) scenarios are unchanged except for the additive `FullRenderFallbackCount=0` field. Every frame of
every scenario reports `FullRenderFallbackCount=0` (SC-005).

Goldens: `specs/109-perf-metrics-baseline/readiness/perf-corpus/<name>.golden.txt`.

## Pointer scenarios — routing full render removed

| Scenario | Field | BEFORE (feature 109) | AFTER (feature 110) |
|----------|-------|----------------------|---------------------|
| `hover-sweep-100` | FullRenderCount | 1 | **0** |
| `hover-sweep-100` | ViewCalled | true | **false** |
| `hover-sweep-1000` | FullRenderCount | 1 | **0** |
| `hover-sweep-5000` | FullRenderCount | 1 | **0** |
| `continuous-drag-400` | FullRenderCount | 1 | **0** |
| `continuous-drag-400` | ViewCalled | true | **false** |

In each case the coalesced move frame previously performed exactly one routing full render (`host.View`
+ `Control.renderTree` just to find what is under the cursor). After feature 110 that render is gone:
the move resolves from the retained frame (here directly to `MapPointer`, the oracle's non-`Click`
path), so `FullRenderCount=0`, `ViewCalled=false`, `FullRenderFallbackCount=0`. The
`PointerSamplesReceived` / `PointerMovesProcessed` coalescing counts are unchanged (FR-012).

## Non-pointer scenarios — unchanged (only the additive field)

`datagrid-100/1000/10000`, `deep-nested-layout`, `text-entry-while-animating`, `theme-switch-dashboard`
keep their exact `ProductModelChanged` / `ViewCalled` / `FullRenderCount` / `RemeasuredNodeCount`
values (their full renders are model-driven re-renders, not routing renders) and simply gain
`FullRenderFallbackCount=0` on every line. This is the FR-011 byte-identity guarantee: only routing
full-render counts (and the new field) move; dispatch outcomes and the at-rest render are untouched.

## Authority

The standing Scene-parity / golden suite run under `Dev` remains the authority for at-rest rendered
output + control geometry byte-identity (see [byte-identity-authority.md](./byte-identity-authority.md));
no scene/geometry golden moved.
