# Pointer-move coalescing proof (T024, SC-004/006, FR-011/012)

enforcing-test=tests/Elmish.Tests/Feature108MetricsTests.fs

The authoritative coalescing surface is the pure `ControlsElmish.Perf.runScript` driver, which shares
the coalescing predicate and the message→update→`RetainedRender.step` path with the live
`runInteractiveApp` loop.

- **N moves → 1 processed move (SC-004).** A script of K consecutive `Pointer` MOVE interactions
  (`HoverEnter`/`HoverLeave`/`DragMove`) coalesces into a SINGLE frame: `PointerSamplesReceived = K`,
  `PointerMovesProcessed = 1`. Asserted with K = 5 (samples=5, moves=1).
- **Click during a move burst is processed within one frame (SC-006).** Script
  `[move; move; Click "btn"]` produces TWO frames: the two moves coalesce (samples=2, moves=1), and the
  discrete click is its own frame and IS processed (its `onClick` dispatches `Inc`, so the click frame
  reports `ViewRebuilt = true`). Discrete interactions are never coalesced or dropped (FR-011).
- **Drag-path fidelity (FR-012).** Coalescing collapses the EXPENSIVE per-move render/hit-test to one
  per frame; the drag's latest position is the processed one. The live loop retains the most recent move
  in its per-frame accumulator (`pendingMove`) and flushes it at the next sample boundary, so a discrete
  `DragEnd` always processes the final position. Backend motion-event compression below the SkiaViewer
  per-sample callback is out of scope ([runtime-limitations.md](../runtime-limitations.md)).
- **Idle event-driven tick (FR-013).** An `Idle` frame reports `RemeasuredNodeCount = 0`,
  `ViewRebuilt = false`; a `Tick` frame advances animation clocks from the injected delta without a
  whole-tree rebuild (`ViewRebuilt = false`).
