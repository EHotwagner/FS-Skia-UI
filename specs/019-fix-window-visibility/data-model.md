# Data Model: Fix Window Visibility

## Interactive Visible Launch Session

Represents a normal generated game run that remains available until explicit user, app, host, or failure close.

**Fields**
- `Options`: `ViewerOptions`
- `WindowBehavior`: `WindowBehaviorRequest`
- `Host`: `GeneratedAppHost<'model,'msg>`
- `Mode`: `interactive-window`
- `WindowCreated`: `bool`
- `WindowVisible`: `ObservedValue<bool>`
- `Focusable`: `ObservedValue<bool>`
- `FirstFramePresented`: `bool`
- `CloseReason`: `ViewerCloseReason option`
- `Diagnostics`: `WindowStateDiagnostic list`

**Validation**
- Viewer options and requested initial size are valid before native work.
- A usable desktop session exists before switching into interactive mode.
- Process/taskbar presence alone is not success.
- Successful visible launch requires a created window plus visible/focusable or explicitly equivalent host-observed accessibility.
- First-frame presentation alone must not complete the session.

**State transitions**
- `NotStarted` -> `CheckingDesktopSession`
- `CheckingDesktopSession` -> `StartingWindow` when usable
- `CheckingDesktopSession` -> `Unsupported` when unusable
- `StartingWindow` -> `WindowCreated`
- `WindowCreated` -> `CheckingVisibility`
- `CheckingVisibility` -> `InteractiveRunning` when accessible
- `CheckingVisibility` -> `InaccessibleWindow` when taskbar-only, hidden, off-screen, zero-sized, unmapped, or surface-less
- `InteractiveRunning` -> `FirstFramePresented`
- `FirstFramePresented` -> `InteractiveRunning`
- `InteractiveRunning` -> `Closing` on user/app/host close
- Any state -> `Failed` on product defect

## Window Behavior Request

Represents expected native window behavior for generated graphical apps.

**Fields**
- `ResizePolicy`: `resizable | fixed-size`
- `MaximizePolicy`: `maximizable | not-maximizable`
- `StartupState`: `normal | minimized | maximized | fullscreen`
- `StartupPosition`: `centered | host-default | coordinates of x:int * y:int`
- `BackendPreference`: `default | skia-gl | skia-vulkan | skia-software`

**Validation**
- Initial size must be positive and within platform-supported limits.
- Startup coordinates must be finite screen coordinates.
- Unsupported backend or window-manager capabilities must produce option diagnostics.
- Unsupported settings must not be silently ignored.

## Window Option Result

Represents how the host handled one requested behavior.

**Fields**
- `Option`: `resize | maximize | startup-state | startup-position | backend`
- `Requested`: `string`
- `Observed`: `string option`
- `Status`: `honored | degraded | unsupported | failed`
- `Message`: `string`

**Validation**
- `honored` requires observable matching behavior or a host API success signal.
- `degraded` requires the fallback behavior to be named.
- `unsupported` requires an environment or backend reason.
- `failed` requires a failure class and actionable message.

## Window State Diagnostic

Represents observable native window facts.

**Fields**
- `WindowInitialized`: `bool`
- `NativeHandleAvailable`: `ObservedValue<bool>`
- `Visible`: `ObservedValue<bool>`
- `Focusable`: `ObservedValue<bool>`
- `Focused`: `ObservedValue<bool>`
- `Closing`: `bool`
- `Minimized`: `ObservedValue<bool>`
- `Maximized`: `ObservedValue<bool>`
- `ClientSize`: `ObservedValue<int * int>`
- `RenderableSurfaceAvailable`: `ObservedValue<bool>`
- `Backend`: `string option`
- `InputDevicesAvailable`: `ObservedValue<bool>`
- `FailureClass`: `environment-session | window-visibility | app-lifecycle option`
- `Message`: `string`

**Validation**
- Inaccessible windows must be classified as degraded or failed interactive launches.
- Zero-sized, hidden, minimized-only, off-screen, unmapped, or surface-less states must be called out when observable.
- Unsupported/unobservable fields must be explicit rather than omitted.

## Launch Outcome

The structured result reported by interactive and evidence launch paths.

**Fields**
- `Status`: `ok | degraded | unsupported | failed`
- `Mode`: `interactive-window | persistent-evidence | bounded-smoke | scene-evidence`
- `WindowOpened`: `bool`
- `WindowVisible`: `ObservedValue<bool>`
- `FirstFramePresented`: `bool`
- `CloseReason`: `ViewerCloseReason option`
- `UserCloseObserved`: `bool`
- `AppCloseObserved`: `bool`
- `EvidenceCloseObserved`: `bool`
- `InputDispatch`: `verified | not-verified | not-required`
- `WindowDiagnostics`: `WindowStateDiagnostic list`
- `OptionResults`: `WindowOptionResult list`
- `FailureClass`: `environment-session | window-visibility | window-options | visual-evidence | package-verification | app-lifecycle | product-defect option`
- `Message`: `string`

**Validation**
- `UserCloseObserved = true` only when `CloseReason = user-close`.
- Interactive success cannot have `EvidenceCloseObserved = true`.
- Taskbar-only or invisible launches cannot be `Status=ok`.
- Unsupported results must identify missing environment/session/window capability.

## Visual Evidence Artifact

Represents proof of scene rendering, native desktop visibility, metadata/hash consistency, or unsupported host state.

**Fields**
- `EvidenceKind`: `image | pixel-readback | metadata-hash | unsupported-host`
- `Path`: `string option`
- `ImageDecodable`: `bool option`
- `ProvesSceneRendering`: `bool`
- `ProvesDesktopVisibility`: `bool`
- `Hash`: `string option`
- `FallbackReason`: `string option`
- `UnsupportedReason`: `string option`

**Validation**
- `EvidenceKind=image` requires a decodable image file.
- Metadata/hash artifacts cannot be named or reported as screenshots.
- Pixel-readback may prove scene rendering but not desktop visibility unless paired with native visibility evidence.
- Unsupported-host diagnostics must be explicit when no image evidence can be captured.

## Generated Validation Run

Represents generated product verification depth for this feature.

**Fields**
- `PackageResolutionExact`: `bool`
- `GeneratedTestsExist`: `bool`
- `GeneratedTestsRan`: `bool`
- `InteractiveVisibleWindowChecked`: `bool`
- `CloseReasonsChecked`: `bool`
- `WindowOptionsChecked`: `bool`
- `ImageEvidenceChecked`: `bool`
- `Authoritative`: `bool`
- `FailureClass`: `string option`

**Validation**
- Package drift or `NU1603` fails validation.
- Generated tests must run when present.
- Placeholder or metadata-only image evidence makes validation non-authoritative or failed, depending on host support.
- Bounded evidence cannot substitute for normal interactive visibility.

## Observed Value

Represents a native fact that may not be introspectable on every host.

**Fields**
- `Status`: `observed | unsupported | unavailable`
- `Value`: value option
- `Source`: `native-api | window-manager | compositor | screenshot | generated-runtime | diagnostic`
- `Message`: `string option`

**Validation**
- `observed` requires `Value`.
- `unsupported` requires a host/platform reason.
- `unavailable` requires a transient or failure reason.
