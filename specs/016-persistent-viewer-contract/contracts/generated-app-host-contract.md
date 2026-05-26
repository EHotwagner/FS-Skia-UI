# Contract: Generated App Host

## Public API Shape

```fsharp
type GeneratedAppHost<'model,'msg> =
    { Init: unit -> 'model * ViewerEffect list
      Update: 'msg -> 'model -> 'model * ViewerEffect list
      View: 'model -> SceneNode
      MapKey: ViewerKey -> bool -> 'msg option
      Tick: TimeSpan -> 'msg option
      Diagnostics: ViewerDiagnosticsOptions }

module Viewer =
    val runApp :
        options: ViewerOptions ->
        host: GeneratedAppHost<'model,'msg> ->
            Result<ViewerLaunchOutcome, ViewerRunFailure>
```

## Required Behavior

- Call `host.Init` once during startup.
- Open the persistent viewer window through the viewer edge.
- Render `host.View model` after initialization and after model changes.
- Dispatch keyboard events through `host.MapKey`, then route resulting messages through `host.Update`.
- Process `host.Tick` on a frame or time cadence when it returns a message.
- Interpret `ViewerEffect` values only at the viewer edge.
- Close intentionally on host close or `CloseWindow`.
- Return a structured launch outcome for success, unsupported environment, or failure.

## Generated Template Requirements

Generated viewer-backed graphical apps must define:

- `Model`
- `Msg`
- `init`
- `update`
- `view`
- `mapKey`
- `tick`
- `viewerOptions`
- `generatedHost`

The default executable path must call `Viewer.runApp viewerOptions generatedHost`. Bounded smoke, frame diagnostics, and scene evidence must remain behind explicit flags.

## Keyboard Requirements

Keyboard-capable profiles must provide a `MapKey` implementation that covers declared keyboard behavior. Launch evidence may mark input dispatch as not applicable only when the feature scope excludes keyboard behavior.
