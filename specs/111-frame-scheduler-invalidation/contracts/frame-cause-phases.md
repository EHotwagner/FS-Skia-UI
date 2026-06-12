# Contract: `FrameCause` + `FrameMetrics` phase record (breaking `.fsi` extension)

**Module**: `FS.Skia.UI.Controls.Elmish` — `ControlsElmish.fsi`
**Change**: add a public closed `FrameCause` DU and four `FrameMetrics` additions
(`FrameCause` + `DiffRan` + `LayoutRan` + `PaintRan`; `ViewCalled` is the fourth/view
phase, reused). **Breaking public `.fsi` change** (Tier 1) — `Route` escalates to
controls-public-surface and BOTH the surface and per-package baselines regenerate
(the surface baseline gains the new `FrameCause` type + cases this time).

## Signature (post-feature)

```fsharp
/// Feature 111 (US1, FR-001): the closed trigger taxonomy naming WHY a frame ran. The
/// scheduler classifies each produced frame and runs only the phases the cause requires.
/// RequireQualifiedAccess — the case names Key/Tick/Idle would shadow a consumer's own
/// Msg cases on `open` (mirrors FrameInput).
[<RequireQualifiedAccess>]
type FrameCause =
    | Idle
    | PointerMove
    | PointerDiscrete
    | Key
    | Tick
    | Resize
    | Theme

type FrameMetrics =
    { /// A product message changed the model this frame.
      ProductModelChanged: bool
      /// THE VIEW PHASE: host.View actually ran. Feature 111 narrows this — it is `false`
      /// on a model-unchanged frame (including an animation-only tick, formerly `true`)
      /// because the scheduler reuses the already-produced view tree (FR-003/FR-011).
      ViewCalled: bool
      /// Genuine host.View + Control.renderTree materializations. Drops to 0 on an
      /// animation-only tick (no view ran). A model-driven re-render still counts.
      FullRenderCount: int
      RemeasuredNodeCount: int
      PointerSamplesReceived: int
      PointerMovesProcessed: int
      FullRenderFallbackCount: int
      /// Feature 111 (FR-001): the trigger that caused this frame. Deterministic, golden-asserted.
      FrameCause: FrameCause
      /// Feature 111 (FR-002): the DIFF phase ran — a newly-produced view tree was reconciled
      /// against the retained tree this frame (in this pipeline view→diff are coupled, so this
      /// equals ViewCalled; a distinct field for the explicit phase record).
      DiffRan: bool
      /// Feature 111 (FR-002): the LAYOUT phase ran — >=1 node was re-measured this frame.
      LayoutRan: bool
      /// Feature 111 (FR-002): the PAINT phase ran — the painted scene (a model render) or the
      /// animation overlay was (re)assembled this frame. `true` on model frames AND animation-only
      /// ticks; `false` on idle / pure routing frames.
      PaintRan: bool
      /// Real wall-clock duration; excluded from goldens.
      FrameDuration: TimeSpan }
```

XML-doc on `FrameCause` and on each new field is **required** (doc-preservation gate);
`ViewCalled`'s doc narrows.

## The phase record

The four work-phase booleans are `{ ViewCalled (view), DiffRan, LayoutRan, PaintRan }`.
Per-frame truth table:

| Frame | FrameCause | ViewCalled | DiffRan | LayoutRan | PaintRan |
|-------|-----------|------------|---------|-----------|----------|
| Idle | `Idle` | false | false | false | false |
| Pointer-move, no msg | `PointerMove` | false | false | false | false |
| Animation-only tick | `Tick` | **false** | false | false | **true** |
| Discrete pointer, no binding | `PointerDiscrete` | false | false | false | false |
| Key/pointer/tick → model change, geometry moves | (`Key`/…) | true | true | true | true |
| Model change, no visual diff | (`Key`/…) | true | true | false | true |

**Hit-test is NOT a phase field** (clarified 2026-06-12): `Perf.runScript` does not
hit-test coalesced hover/drag moves (only discrete clicks hit-test, feature 110), so
a hit-test bool would read `false` across the move-burst corpus; hit-test work stays
covered by `PointerSamplesReceived`/`PointerMovesProcessed`/`FullRenderFallbackCount`.

## Construction / read sites (update all in one change)

- Perf `zero` (`ControlsElmish.fs:1231`) → `{ FrameCause = Idle; DiffRan = false;
  LayoutRan = false; PaintRan = false; ... }`.
- Perf coalesced move (`:1247`), tick (`:1273`), key (`:1307`), discrete (`:1325`) —
  set `FrameCause` + the three phase bools per branch (the tick branch also performs
  the animation-only view-skip).
- Live `emitFrameMetrics` (`ControlsElmish.fs:918`) — add `FrameCause` + the three
  bools as arguments; `mapPointer`/`wrappedTick` classify the cause.
- `tests/Elmish.Tests/Feature109CorpusTests.fs:153` `serialize` — include
  `FrameCause` + `DiffRan`/`LayoutRan`/`PaintRan` in the golden line.
- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` (gains `FrameCause` +
  cases) + `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt` —
  regenerate via `./fake.sh build -t RefreshSurfaceBaselines`.

## Compatibility / migration

Additive fields on a record + a new public type: consumers that pattern-match or
construct `FrameMetrics` must add the fields; the new `FrameCause` is a closed DU
(`RequireQualifiedAccess`). This is the documented breaking note; the live
`OnFrameMetrics` sink and `Perf.runScript` return type are otherwise unchanged.
`ViewCalled`/`FullRenderCount` keep their definitions; their values change on
model-unchanged frames (the documented view-skip effect, FR-008/FR-011).
