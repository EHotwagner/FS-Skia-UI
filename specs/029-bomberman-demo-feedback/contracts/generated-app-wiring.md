# Contract: Generated App Wiring

## Purpose

Generated game apps need one standard path from pure application state to viewer events, viewer effects, and persistent launch behavior.

## Candidate Public Shape

The implementation may reuse or extend `FS.Skia.UI.SkiaViewer.GeneratedAppHost<'model,'msg>`.

Required logical fields:

```fsharp
type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }
```

If app-owned effects are introduced, the public shape must keep them separate from viewer effects until an explicit host adapter interprets them.

## Required Behavior

- `Init` creates app state without native or filesystem work.
- `Update` is pure and returns next app state plus app-owned effects or viewer effects produced by an adapter.
- `View` renders the current model to a `SceneNode`.
- `MapKey` maps viewer key events into app messages without mutating state.
- `Tick` maps elapsed time into optional app messages.
- `runApp` launches the default persistent interactive host.
- `runAppEvidence` launches an explicit bounded evidence mode and must not be used as the default launch path.

## Test Obligations

- Pure transition tests assert next model and emitted app effects.
- Host adapter tests assert viewer effects are emitted only at the boundary.
- Generated product tests assert key input, tick input, scene rendering, and persistent launch are wired.
- FSI transcripts exercise the public host value through the packed package or prelude.
