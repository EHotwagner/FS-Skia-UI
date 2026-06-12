# Phase 1 Data Model: feature 109

Observation-only feature — the only **shipped** data-model change is the
`FrameMetrics` record's field set. The rest are **test/evidence** entities (no
shipped `Controls.Elmish` API; clarified 2026-06-12).

## 1. `FrameMetrics` (shipped — breaking field change)

The per-frame work signal produced by both the live host loop (`emitFrameMetrics`)
and the deterministic `Perf.runScript` driver. Each field has **one precise
meaning** (SC-011).

| Field | Type | Golden? | Meaning |
|---|---|---|---|
| `ProductModelChanged` | `bool` | yes | A product message actually changed the model this frame (reference identity of the folded model changed across `host.Update`). `false` for a no-message frame, a pure-hover/focus frame, and an animation-only tick. (FR-001/003/005) |
| `ViewCalled` | `bool` | yes | `host.View size model` ran this frame to (re)produce a tree. `= FullRenderCount > 0`. (FR-001) |
| `FullRenderCount` | `int` | yes | Number of full `host.View` + `Control.renderTree` materializations the frame performed (routing render + retained-step render where they occur). The baseline answer to "how many full renders for this interaction"; Phase 2 drives the hot-path value toward 0. (FR-015) |
| `RemeasuredNodeCount` | `int` | yes | Nodes re-measured this frame (from `WorkReductionRecord.RemeasuredNodeCount`); 0 on idle, bounded (overlay-assembly) on an animation-only frame. (unchanged from 108) |
| `PointerSamplesReceived` | `int` | yes | Raw native pointer samples that arrived this frame, **including** deferred/queued moves carried from a prior boundary (K before coalescing). (FR-008) |
| `PointerMovesProcessed` | `int` | yes | Pointer moves applied after coalescing — `≤ 1` per frame. (FR-009) |
| `FrameDuration` | `TimeSpan` | **no** | Real wall-clock duration of the frame's work in the live loop; `TimeSpan.Zero`/unobserved in `Perf.runScript`. **Excluded** from goldens (FR-012). |

**Removed**: `ViewRebuilt: bool` (FR-002 — no conflating name survives; not kept as
a deprecated alias).

**Validation / invariants**:
- `ViewCalled = (FullRenderCount > 0)`.
- `ProductModelChanged = false` ⇒ the frame produced no model-changing product
  message; it MAY still have `ViewCalled = true` only via an active animation/tick
  overlay (FR-005/006), otherwise `ViewCalled = false`.
- Idle frame ⇒ all of `RemeasuredNodeCount = 0`, `PointerMovesProcessed = 0`,
  `ViewCalled = false`, `FullRenderCount = 0`, `ProductModelChanged = false`
  (FR-006/SC-004), unless an animation clock or explicit tick is active.
- Coalesced move burst of N ⇒ `PointerSamplesReceived = N`, `PointerMovesProcessed
  ≤ 1` (FR-008/009/SC-002).

## 2. `PerformanceScenario` (test/evidence entity — not shipped)

A named, parameterized interaction in the corpus.

| Field | Type | Meaning |
|---|---|---|
| `Name` | `string` | Stable scenario id (also the golden filename stem). |
| `Parameters` | record / tuple | Control count, row count, nesting depth, sample count — whatever parameterizes the scenario. |
| `Host` | `InteractiveAppHost<'model,'msg>` | The host under test (built from existing control kinds / current DataGrid path). |
| `Size` | `Size` | Viewport the script runs against. |
| `Script` | `FrameInput<'msg> list` | The ordered driver script (see §4). |

Required corpus members (FR-013): hover sweep 100/1000/5000 simple controls;
DataGrid 100/1000/10000 rows (fully-materialized path); deep nested layout of
repeated labels+buttons; focused text entry while siblings animate; theme switch
across a moderate dashboard; continuous drag of hundreds of raw samples.

Each scenario yields a committed deterministic golden (counts+booleans only) at
`readiness/perf-corpus/<Name>.golden.txt`.

## 3. `BaselineRecord` (evidence entity — not shipped)

Stored under `docs/reports/_baselines/`. Human-facing, non-gating.

| Field | Type | Meaning |
|---|---|---|
| `Scenario` | `string` | Scenario name. |
| `Phase` | `before` \| `after` | For the coalescing hover-burst, both exist (FR-019/SC-007). |
| `TimingMs` | real | Wall-clock per frame / per run (environment-dependent). |
| `AllocatedBytes` | real | `GC.GetAllocatedBytesForCurrentThread()` delta over the run. |
| `CounterSnapshot` | counts | The deterministic counts (cross-link to the golden). |
| `MissingCounters` | string list | Phase counters not yet captured (paint/composite/hit-test/layout) — stated explicitly (FR-015). |

Regression thresholds are defined **counts-first, timing-second** (FR-018).

## 4. `Perf script` / `FrameInput<'msg>` (existing — unchanged)

An ordered driver step (already shipped in 108; **no change**):

```fsharp
[<RequireQualifiedAccess>]
type FrameInput<'msg> =
    | Key of ViewerKey * KeyModifiers
    | Pointer of PointerInteraction
    | Tick of TimeSpan
    | Idle
```

`Perf.runScript : InteractiveAppHost<'model,'msg> -> Size -> FrameInput<'msg> list
-> FrameMetrics list` — signature unchanged; only the returned record's field set
changes. `toFrames` coalescing (consecutive `HoverEnter`/`HoverLeave`/`DragMove`
collapse into one frame; everything else is its own frame) is unchanged.
