# Contract: `FrameMetrics` (breaking `.fsi` extension)

**Module**: `FS.Skia.UI.Controls.Elmish` — `ControlsElmish.fsi`
**Change**: add a deterministic int `FullRenderFallbackCount` (FR-009). This is a
**breaking public `.fsi` change** (Tier 1), so `Route` escalates to
controls-public-surface and the surface + per-package baselines regenerate.

## Signature (post-feature)

```fsharp
type FrameMetrics =
    { /// A product message changed the model this frame.
      ProductModelChanged: bool
      /// host.View actually ran this frame. Routing via the retained path does not set this.
      ViewCalled: bool
      /// Genuine host.View + Control.renderTree materializations this frame.
      /// Routing via the retained path does NOT increment this (feature 110);
      /// a model-driven re-render after a dispatched message still does.
      FullRenderCount: int
      /// Layout nodes remeasured by the retained step this frame.
      RemeasuredNodeCount: int
      /// Raw native pointer samples received this frame, including deferred moves.
      PointerSamplesReceived: int
      /// Pointer moves actually processed this frame (<= 1 per burst, feature 108/109).
      PointerMovesProcessed: int
      /// Times retained pointer routing fell back to a full render to route an
      /// event this frame. Zero for every normal scripted pointer scenario
      /// (feature 110, FR-009); non-zero only signals a routing escape hatch.
      FullRenderFallbackCount: int
      /// Real wall-clock duration of the frame's work. Excluded from goldens.
      FrameDuration: TimeSpan }
```

XML-doc on `FullRenderFallbackCount` is **required** (doc-preservation gate).

## Semantics

| Scenario | `FullRenderCount` | `FullRenderFallbackCount` | `ViewCalled` |
|----------|-------------------|---------------------------|--------------|
| Pointer move routed via retained path | 0 (for routing) | 0 | false (for routing) |
| Pointer click routed via retained path | 0 (for routing) | 0 | false (for routing) |
| Click whose message changes the model → next frame re-renders | 1 (the model render) | 0 | true |
| Burst of N moves in one frame | 0 (routing) | 0 | false (routing) |
| Forced unroutable case → oracle fallback | 1 (the fallback render) | 1 | true |

## Construction / read sites (update all in one change)

- `ControlsElmish.fs:804` `emitFrameMetrics` (live sink) — add a frame-local
  fallback accumulator argument.
- `ControlsElmish.fs:1076` `zero`, `1107` move, `1144` tick, `1162` key, `1178`
  discrete — set `FullRenderFallbackCount` per branch (0 on normal scenarios).
- `tests/Elmish.Tests/Feature109CorpusTests.fs:153` `serialize` — include the new
  field in the golden line so goldens regenerate deterministically.
- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` + per-package
  baselines — regenerate via `./fake.sh build -t RefreshSurfaceBaselines`.

## Compatibility / migration

Additive field on a record: consumers that pattern-match or construct
`FrameMetrics` must add the field. This is the documented breaking note; the live
`OnFrameMetrics` sink and `Perf.runScript` return type are otherwise unchanged.
