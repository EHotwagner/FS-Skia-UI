# Contract: Launch Visibility

## Public API Shape

Implementation may refine names, but the public `.fsi` surface must preserve the separation below:

```fsharp
type ViewerCloseReason =
    | UserClose
    | AppRequestedClose
    | EvidenceRequestedClose
    | FrameworkRequestedClose
    | HostSystemClose
    | Timeout
    | FailureDrivenClose

type ViewerObservedStatus =
    | Observed
    | Unsupported
    | Unavailable

type ViewerWindowBehaviorRequest =
    { ResizePolicy: string
      MaximizePolicy: string
      StartupState: string
      StartupPosition: string option
      BackendPreference: string option }

type ViewerWindowStateDiagnostic =
    { WindowInitialized: bool
      Visible: string
      Focusable: string
      Focused: string
      Minimized: string
      Maximized: string
      ClientSize: string
      RenderableSurfaceAvailable: string
      Backend: string option
      InputDevicesAvailable: string
      Message: string }

type ViewerLaunchOutcome =
    { Status: string
      Mode: string
      WindowOpened: bool
      WindowVisible: string
      FirstFramePresented: bool
      CloseReason: ViewerCloseReason option
      UserCloseObserved: bool
      AppCloseObserved: bool
      EvidenceCloseObserved: bool
      InputDispatch: string
      WindowDiagnostics: ViewerWindowStateDiagnostic list
      FailureClass: string option
      Message: string }
```

Existing names may be retained when they provide equivalent fields and semantics.

## Interactive Visibility Requirements

- Validate viewer options, desktop-session readiness, and requested window behavior before entering native app lifecycle.
- Open a native window and record whether a handle/surface exists.
- Render at least one frame before claiming first-frame success.
- Continue the event/render loop after first-frame presentation.
- Classify taskbar-only, hidden, off-screen, unmapped, minimized-only, zero-sized, or surface-less launches as degraded/failed interactive launches.
- Return `Mode=interactive-window` for normal generated app runs.
- Set `UserCloseObserved=true` only after a native/user close event.

## Close Reason Requirements

- `UserClose` requires a user/native close event.
- `AppRequestedClose` requires a generated app command/effect requesting close.
- `EvidenceRequestedClose` is valid only for explicit evidence modes.
- `FrameworkRequestedClose` requires runtime/framework shutdown not caused by user/app/evidence.
- `HostSystemClose` requires host/window-system shutdown signal when observable.
- `Timeout` requires configured timeout expiration.
- `FailureDrivenClose` requires a failure diagnostic.

Boolean compatibility fields must derive from `CloseReason`, not the other way around.

## Window Behavior Requirements

- Resize and maximize policies must be applied before the window is presented when supported.
- Startup state and startup position must be applied before or during first presentation depending on host capability.
- Backend preference must be recorded as requested and observed.
- Every requested option must produce `honored`, `degraded`, `unsupported`, or `failed` diagnostics.
- Unsupported options must not be silently ignored.

## Evidence Mode Requirements

- Bounded first-frame, screenshot/image, pixel, and metadata evidence require explicit API/CLI/workflow selection.
- Evidence mode may close itself after targets complete or fail.
- Evidence close must not be reported as user close.
- Evidence results must state whether they prove scene rendering, desktop visibility, or both.

## Compatibility

Existing generated apps that call `Viewer.runApp` should remain interactive by default. Existing bounded smoke commands may remain, but their output fields must stop implying visible interactive availability unless native visibility was observed.
