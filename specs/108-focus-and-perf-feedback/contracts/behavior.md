# Behaviour Contract — focus order, per-frame metrics, pointer coalescing

Companion to the `.fsi` sketches. Defines the host-edge behaviour that the pure
`Perf.runScript` driver and the live `runInteractiveApp` loop share.

## Focus order → ring (US1)

- The reachable set is exactly `Focus.order control |> .Stops`. `markFocused`
  stamps `VisualState.Focused` on the stop whose `ControlId` (= `Key ?? structural
  path`) equals `focused`; all other stops keep their prior state.
- A `focused` id not present in `order` (stale/removed control) stamps nothing — no
  crash, no ring (mirrors `traverse` stale-target recovery).
- A control already carrying a consumer-set non-Normal state that is *not* Focused
  (e.g. `Disabled`) keeps it — Focused does not override (spec edge case: disabled
  focusable controls receive no ring and are skipped in traversal).
- Slot/lookless subtree: the stamp lands on the focused leaf control, never on the
  carrier (spec edge case) — because `order` enumerates the lowered leaves.

## Per-frame metrics (US2)

For each frame the host/driver advances, it produces one `FrameMetrics`:

| Field | Source | Determinism |
|---|---|---|
| `RemeasuredNodeCount` | `RetainedRenderStep.WorkReduction.RemeasuredNodeCount` | golden |
| `PointerSamplesReceived` | count of raw pointer samples buffered this frame | golden |
| `PointerMovesProcessed` | moves actually applied after coalescing (≤1) | golden |
| `ViewRebuilt` | `true` iff `host.View` ran this frame (model/size changed) | golden |
| `FrameDuration` | wall-clock of the frame's work | **excluded** |

- An animation cross-fade frame reports the overlay-assembly remeasure count, NOT a
  false whole-tree rebuild (`ViewRebuilt = false` when only a clock advanced — spec
  edge case; consistent with feature 099 sample-on-paint overlay).
- `OnFrameMetrics` is called once per frame with this record after the frame's work;
  default sink ignores it. `Perf.runScript` collects the list directly.

## Pointer-move coalescing (US4)

Within one frame the host buffers raw samples, then processes:

1. **Moves** (`HoverEnter` / `HoverLeave` / `DragMove`): collapse to the **latest**
   position → at most one processed move, one hit-test, one visual-state update
   (FR-011). For a `DragMove` sequence the coalesced move **retains the intermediate
   path** so a freehand/drag consumer keeps fidelity (FR-012).
2. **Discrete** (`PressedDown`, `ReleasedUp`, `Click`, `DragBegin`, `DragEnd`,
   `DragCancelled`, `Scroll`, secondary): processed in arrival order, **never
   coalesced, never dropped**. A `Click` interleaved with moves is processed within
   the same frame it arrived (SC-006).
3. The coalescing buffer is a per-frame `mutable` accumulator (`// mutable: hot path /
   per frame`), reset at the frame boundary.

Ordering rule: discrete interactions keep their relative order; the single coalesced
move is applied at the position of the latest move sample. A press/release straddling
moves is never reordered behind the coalesced move in a way that drops it.

## Event-driven tick (US4, FR-013)

- The documented default interactive `Tick` is `fun _ -> None` (event-driven): no
  input → no frame work scheduled, no `host.View` rebuild.
- Animation clocks still advance from the injected per-frame delta even on an idle
  tick (the host's existing `wrappedTick` clock-advance, feature 099), so active
  animations are unaffected while idle frames do zero re-measure work.

## Determinism boundary (US3)

`Perf.runScript` is pure: same `host` + `size` + `script` → identical `FrameMetrics`
count/bool fields across runs and machines. It calls the SAME coalescing + 
`RetainedRender.step` code path the live host uses, so a regression that
un-coalesces moves or reintroduces a per-hover full rebuild fails the golden
(SC-004/005) rather than shipping.
