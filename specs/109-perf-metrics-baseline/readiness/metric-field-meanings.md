# FrameMetrics field meanings & US1 independent validation (feature 109, T015/T016)

## Each field has ONE reviewer-nameable precise meaning (SC-011)

| Field | Type | Golden? | Precise single meaning |
|---|---|---|---|
| `ProductModelChanged` | bool | yes | A product message actually changed the model this frame (the reference identity of the folded model moved across `host.Update`). `false` for a no-message frame, a pure hover/focus frame, and an animation-only tick. (FR-001/003/005) |
| `ViewCalled` | bool | yes | `host.View size model` ran this frame to (re)produce a tree. Equals `FullRenderCount > 0`. (FR-001) |
| `FullRenderCount` | int | yes | Number of full `host.View` + `Control.renderTree` materializations the frame performed (the routing render and the retained-step render where each occurs). The baseline "how many full renders for this interaction"; Phase 2 drives the hot-path value toward 0. (FR-015) |
| `RemeasuredNodeCount` | int | yes | Nodes re-measured this frame (from `WorkReductionRecord.RemeasuredNodeCount`); 0 on idle, bounded on an animation-only frame. (unchanged from 108) |
| `PointerSamplesReceived` | int | yes | Raw native pointer samples that arrived this frame, including deferred/queued moves (K before coalescing). (FR-008) |
| `PointerMovesProcessed` | int | yes | Pointer moves applied after coalescing — at most one per frame. (FR-009) |
| `FrameDuration` | TimeSpan | **no** | Real wall-clock duration of the frame's work in the live loop; `TimeSpan.Zero` in `Perf.runScript`. EXCLUDED from goldens (FR-012). |

`ViewRebuilt` was REMOVED (no deprecated alias) — it conflated "the model changed" with "the view ran",
and those facts genuinely DIVERGE (an animation-only tick: `ProductModelChanged = false`,
`ViewCalled = true`). No surviving field conflates them (FR-002 / SC-011).

## Invariants asserted

- `ViewCalled = (FullRenderCount > 0)` for every produced frame (`Feature109MetricsHonestyTests`).
- Idle frame ⇒ `RemeasuredNodeCount = 0`, `PointerMovesProcessed = 0`, `ViewCalled = false`,
  `FullRenderCount = 0`, `ProductModelChanged = false` (SC-004).
- `ProductModelChanged` detection adds NO `'model` equality constraint to the public host signature:
  it is `not (List.isEmpty msgs) && not (obj.ReferenceEquals(before, after))` — precise for reference-
  type models (an idempotent `update` returning the same instance ⇒ `false`) and honest for value-type
  models (the empty-message guard keeps a no-message frame `false`).

## US1 independent validation path (T016)

Drive the three scripted frames + idle through `ControlsElmish.Perf.runScript` and assert each
view/model field against the code-path fact in every case:

1. a no-product-message frame (idle / pure hover) ⇒ `ProductModelChanged = false`;
2. a product message that changes the model with no visual difference ⇒ `ProductModelChanged = true`,
   `RemeasuredNodeCount = 0` (no field over-reports work);
3. a host-owned animation change with no product message (the cross-fade tick) ⇒
   `ProductModelChanged = false`, `ViewCalled = true` — the two facts diverge.

Evidence: `tests/Elmish.Tests/Feature109MetricsHonestyTests.fs` (11 tests, all green).

## Once-per-frame emission (FR-007 / SC-010)

`Perf.runScript` yields exactly one `FrameMetrics` per produced frame — a coalesced move burst of N is
ONE record with `PointerSamplesReceived = N`, never N records (asserted). The live `runInteractiveApp`
emit path calls `emitFrameMetrics` exactly once per processed frame BY CONSTRUCTION (the two code paths
in `mapPointer` each emit at most once per sample boundary); it is the BEST-EFFORT sink, and the
authoritative once-per-frame surface is `Perf.runScript` (the documented evidence path).
