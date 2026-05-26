# Contract: Launch Runtime

## Public API Shape

Implementation may refine names, but the public `.fsi` surface must preserve the separation below:

```fsharp
type ViewerLaunchMode =
    | InteractiveWindow
    | PersistentEvidence

type ViewerInputDispatchStatus =
    | Verified
    | NotVerified
    | NotRequired

type ViewerLaunchOutcome =
    { Status: string
      Mode: string
      Command: string option
      RendererMode: string
      WindowOpened: bool
      FirstFramePresented: bool
      UserCloseObserved: bool
      SelfClosedForEvidence: bool
      InputDispatch: string
      BlockedStage: ViewerRunBlockedStage option
      Classification: ViewerRunFailureClassification option
      Category: ViewerDiagnosticCategory option
      Message: string }

module Viewer =
    val runApp :
        options: ViewerOptions ->
        host: GeneratedAppHost<'model,'msg> ->
            Result<ViewerLaunchOutcome, ViewerRunFailure>

    val runAppEvidence :
        request: ViewerRunRequest ->
        options: ViewerOptions ->
        host: GeneratedAppHost<'model,'msg> ->
            Result<ViewerLaunchOutcome, ViewerRunFailure>
```

If implementation keeps existing bounded APIs instead of adding `runAppEvidence`, generated CLI flags must still call an explicit evidence path and produce the required outcome fields.

## Interactive Requirements

- Validate viewer options and desktop-session readiness before entering native app lifecycle.
- Call `host.Init` once.
- Open the native window and render the host view.
- Continue the event/render loop after first-frame presentation.
- Dispatch keyboard input through `host.MapKey` and `host.Update`.
- Process `host.Tick` for time-based game progression.
- Close only when the user/host closes the window or the host emits `CloseWindow`.
- Return `Mode=interactive-window`, `SelfClosedForEvidence=false`, and `UserCloseObserved=true` only when an explicit close occurred.

## Evidence Requirements

- Require explicit evidence API, flag, or workflow selection.
- Allow self-close only after evidence targets complete or timeout/failure is reported.
- Report `Mode=persistent-evidence` or equivalent evidence-specific mode.
- Include `FirstFramePresented`, `InputDispatch`, and `SelfClosedForEvidence`.
- Never label bounded evidence as successful interactive play.

## Desktop Session Requirements

Interactive launch on Linux/container hosts must validate:

- `XDG_RUNTIME_DIR` is set, exists, has suitable ownership, and has suitable permissions.
- `DISPLAY` or `WAYLAND_DISPLAY` is set.
- For Wayland, `$XDG_RUNTIME_DIR/$WAYLAND_DISPLAY` exists as a socket.
- For X11, `/tmp/.X11-unix/X${DISPLAY#:}` exists as a socket when applicable.
- `DBUS_SESSION_BUS_ADDRESS` is passed through when needed by the host environment.

A private runtime directory fallback may be reported for diagnostics/evidence workflows, but must be labeled as not equivalent to a full desktop session.

## Compatibility

Existing generated apps that call `Viewer.runApp` should become interactive by default. Existing bounded smoke commands may remain, but their output fields must stop implying persistent interactive availability.
