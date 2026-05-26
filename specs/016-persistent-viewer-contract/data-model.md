# Data Model: Persistent Viewer Contract

## Persistent Viewer Contract

Represents the public package capability to open and maintain a graphical desktop window.

**Fields**
- `Options`: `ViewerOptions`
- `Scene`: `SceneNode`
- `Mode`: `persistent-window`
- `Diagnostics`: `ViewerDiagnosticsOptions`

**Validation**
- Title must be non-empty.
- Initial size must have positive width and height.
- Host must support a desktop window system or return an unsupported-environment outcome.

**State transitions**
- `NotStarted` -> `StartingWindow`
- `StartingWindow` -> `Rendering`
- `Rendering` -> `Closing` on user exit or close effect
- Any state -> `Failed` on product defect
- Any state -> `Unsupported` on unsupported environment

## Generated App Host

Represents the model-driven persistent app contract used by generated graphical apps.

**Fields**
- `Init`: `unit -> 'model * ViewerEffect list`
- `Update`: `'msg -> 'model -> 'model * ViewerEffect list`
- `View`: `'model -> SceneNode`
- `MapKey`: `ViewerKey -> bool -> 'msg option`
- `Tick`: `TimeSpan -> 'msg option`
- `Diagnostics`: `ViewerDiagnosticsOptions`

**Validation**
- `Init`, `Update`, and `View` must be callable without native side effects.
- Keyboard-capable profiles must provide a `MapKey` path that dispatches declared keyboard behavior.
- `Tick` may return `None` when the feature has no time-based behavior.
- Effects are data until interpreted by the viewer edge.

**Relationships**
- Uses `Persistent Viewer Contract` for native window lifecycle.
- Emits `ViewerEffect` values interpreted by the viewer edge.
- Produces `Graphical Launch Evidence`.

## Runtime Capability Result

Reports package and host capability before or during launch.

**Fields**
- `SupportsPersistentWindow`: `bool`
- `SupportsBoundedSmoke`: `bool`
- `SupportsKeyboardInput`: `bool`
- `RendererMode`: `string`
- `UnsupportedReason`: `string option`
- `Classification`: `ViewerRunFailureClassification option`
- `BlockedStage`: `ViewerRunBlockedStage option`

**Validation**
- Missing package/product capability is classified as `ProductDefect`.
- Missing display/window system support is classified as `UnsupportedEnvironment`.
- Bounded smoke support must not imply persistent window support.

## Launch Outcome

The structured result from persistent scene or app launch.

**Fields**
- `Status`: `ok | unsupported | failed`
- `Mode`: `persistent-window`
- `Command`: `string`
- `WindowOpened`: `bool`
- `InputDispatchVerified`: `bool option`
- `ExitPathVerified`: `bool`
- `BlockedStage`: `ViewerRunBlockedStage option`
- `Classification`: `ViewerRunFailureClassification option`
- `DiagnosticCategory`: `ViewerDiagnosticCategory option`
- `Message`: `string`

**Validation**
- `ok` requires `WindowOpened = true` and `ExitPathVerified = true`.
- Keyboard-capable profiles require `InputDispatchVerified = Some true`.
- `unsupported` cannot satisfy completion evidence by itself.
- `failed` must identify whether the gap is product capability or another product defect.

## Graphical Launch Evidence

Readiness artifact proving or diagnosing the default persistent graphical app launch path.

**Fields**
- `status`
- `mode`
- `command`
- `window-opened`
- `input-dispatch`
- `exit-path`
- `blocked-stage`
- `classification`
- `category`
- `message`

**Validation**
- Must be distinct from bounded smoke and scene evidence artifacts.
- Completion requires at least one supported-host `status=ok` artifact.
- Unsupported-host diagnostics may supplement but not replace supported-host evidence.

## Bounded Evidence Artifact

CI or diagnostic artifact for bounded viewer or deterministic scene behavior.

**Fields**
- `status`
- `smoke` or `scene-evidence`
- `frames-rendered` or deterministic scene value
- `renderer-mode`
- `diagnostic-mode`
- `message`

**Validation**
- Must be produced only by explicit evidence commands or flags.
- Must not be counted as persistent graphical launch readiness.
