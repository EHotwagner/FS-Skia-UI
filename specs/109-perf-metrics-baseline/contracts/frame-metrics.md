# Contract: `FrameMetrics` surface & golden serialization (feature 109)

The single shipped contract change. Breaking `.fsi` edit in
`src/Controls.Elmish/ControlsElmish.fsi`; escalates `Route` to the
controls-public-surface tier.

## Before (feature 108 — current)

```fsharp
type FrameMetrics =
    { RemeasuredNodeCount: int
      PointerSamplesReceived: int
      PointerMovesProcessed: int
      ViewRebuilt: bool          // conflates "model changed" with "view ran"
      FrameDuration: TimeSpan }
```

## After (feature 109)

```fsharp
/// Feature 108/109: the per-frame structured work/timing signal the host loop and the
/// deterministic `Perf.runScript` driver both produce. The six count/bool fields are the
/// byte-stable determinism surface; `FrameDuration` is reported for real perf observation
/// but EXCLUDED from golden assertions (it varies run to run).
type FrameMetrics =
    { /// A product message actually changed the model this frame (FR-001/003/005). `false`
      /// for a no-message frame, a pure hover/focus frame, and an animation-only tick.
      ProductModelChanged: bool
      /// `host.View size model` ran this frame to (re)produce a tree (FR-001). Equals
      /// `FullRenderCount > 0`.
      ViewCalled: bool
      /// Number of full `host.View` + `Control.renderTree` materializations this frame
      /// performed — the routing render and the retained-step render where they occur
      /// (FR-015). The baseline "how many full renders for this interaction".
      FullRenderCount: int
      /// Nodes re-measured this frame (from `WorkReductionRecord.RemeasuredNodeCount`);
      /// 0 on idle, bounded (overlay-assembly) on an animation-only frame.
      RemeasuredNodeCount: int
      /// Raw pointer samples that arrived this frame, including deferred/queued moves
      /// carried from a prior boundary (K before coalescing) (FR-008).
      PointerSamplesReceived: int
      /// Pointer MOVES applied after coalescing — at most one per frame (FR-009).
      PointerMovesProcessed: int
      /// Wall-clock duration of the frame's work (live loop) — reported, EXCLUDED from
      /// the golden/determinism surface (FR-012).
      FrameDuration: TimeSpan }
```

- **Removed**: `ViewRebuilt` (no deprecated alias — FR-002).
- **Added**: `ProductModelChanged`, `ViewCalled`, `FullRenderCount`.
- **Doc-preservation gate**: every field carries `///` XML-doc; the type's doc
  precedes the type, attributes (none here) before doc before type.
- **`Perf.runScript` signature**: unchanged
  (`host -> size -> FrameInput<'msg> list -> FrameMetrics list`).

## Golden serialization (deterministic — counts + booleans only)

One line per produced frame, fixed field order, **no** `FrameDuration`/allocation:

```
ProductModelChanged=<bool> ViewCalled=<bool> FullRenderCount=<int> RemeasuredNodeCount=<int> PointerSamplesReceived=<int> PointerMovesProcessed=<int>
```

- Stored at `specs/109-perf-metrics-baseline/readiness/perf-corpus/<scenario>.golden.txt`.
- Re-run MUST be byte-identical (SC-005). No timing/allocation field may appear
  (SC-009).

## Non-golden report (separate file, never gates)

`docs/reports/_baselines/2026-06-12-controls-corpus-{before,after}.md` carries
per-scenario `TimingMs`, `AllocatedBytes`, a cross-link to the count snapshot,
the count-first regression thresholds (FR-018), and an explicit `MissingCounters`
line naming the phase counters not yet captured (paint/composite/hit-test/layout —
FR-015). The hover-burst scenario records both a `before` (pre-108-coalescing) and
`after` baseline (FR-019/SC-007).

## Baselines regenerated after the change

- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`
- `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`

via `./fake.sh build -t RefreshSurfaceBaselines`.

## Invariant (FR-020 / SC-008)

The rendered scene, control geometry, dispatch outcomes, and the default
(non-observing, `OnFrameMetrics = ignore`) host path remain **byte-identical** to
the pre-feature state. Only the observability surface (this record's shape) and
evidence artifacts change.
