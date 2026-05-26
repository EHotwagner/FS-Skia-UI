# Data Model: Persistent GUI Runtime

## Interactive Launch Session

Represents a normal generated game run that remains available until explicit user or host close.

**Fields**
- `Options`: `ViewerOptions`
- `Host`: `GeneratedAppHost<'model,'msg>`
- `Mode`: `interactive-window`
- `WindowOpened`: `bool`
- `FirstFramePresented`: `bool`
- `UserCloseObserved`: `bool`
- `InputDispatch`: `verified | not-verified | not-required`
- `Diagnostics`: `ViewerDiagnosticsOptions`

**Validation**
- Title and initial size are valid before native work.
- A usable desktop session exists before switching into interactive mode.
- First-frame presentation alone must not complete the session.
- Successful completion requires explicit user/host close, not evidence self-close.

**State transitions**
- `NotStarted` -> `CheckingDesktopSession`
- `CheckingDesktopSession` -> `StartingWindow` when usable
- `CheckingDesktopSession` -> `Unsupported` when unusable
- `StartingWindow` -> `InteractiveRunning`
- `InteractiveRunning` -> `FirstFramePresented`
- `FirstFramePresented` -> `InteractiveRunning`
- `InteractiveRunning` -> `Closing` on user/host close or `CloseWindow`
- Any state -> `Failed` on product defect

## Evidence Launch Session

Represents an explicit bounded run for launch, frame, input, screenshot, or pixel evidence.

**Fields**
- `Request`: `ViewerRunRequest`
- `Mode`: `persistent-evidence`
- `EvidenceTargets`: first frame, frame count, duration, input dispatch, screenshot, pixel readback
- `SelfClosedForEvidence`: `bool`
- `Timeout`: `TimeSpan`
- `EvidencePath`: `string option`

**Validation**
- Evidence mode requires explicit API/CLI/workflow choice.
- Timeout and frame targets must be positive.
- Self-close is allowed only after evidence targets complete or timeout/failure is reported.
- Evidence output must not claim an ongoing interactive session.

## Launch Outcome

The structured result reported by interactive and evidence launch paths.

**Fields**
- `Status`: `ok | unsupported | failed`
- `Mode`: `interactive-window | persistent-evidence | bounded-smoke | scene-evidence`
- `Command`: `string option`
- `RendererMode`: `string`
- `WindowOpened`: `bool`
- `FirstFramePresented`: `bool`
- `UserCloseObserved`: `bool`
- `SelfClosedForEvidence`: `bool`
- `InputDispatch`: `verified | not-verified | not-required`
- `BlockedStage`: `ViewerRunBlockedStage option`
- `Classification`: `UnsupportedEnvironment | ProductDefect option`
- `Category`: `ViewerDiagnosticCategory option`
- `Message`: `string`

**Validation**
- `interactive-window` success cannot have `SelfClosedForEvidence = true`.
- Evidence success must disclose whether it self-closed.
- Unsupported results must identify missing environment/session prerequisites.
- Failed results must identify package, verification-depth, lifecycle, or product-defect class.

## Desktop Session Diagnostic

Represents pre-launch host/container graphical readiness.

**Fields**
- `RuntimeDirectory`: path, exists, owner suitable, permission suitable
- `Display`: `DISPLAY` or `WAYLAND_DISPLAY`
- `DisplaySocket`: path and exists/is-socket status
- `SessionBus`: `DBUS_SESSION_BUS_ADDRESS option`
- `FallbackRuntimeDirectory`: path option
- `FallbackIsFullDesktopSession`: always `false`
- `DiagnosticClass`: `environment-session`
- `Message`: `string`

**Validation**
- Interactive launch fails fast when no usable display/session exists.
- Private runtime fallback must be labeled diagnostic/evidence-only.
- Display variable without a socket is unsupported.

## Package Resolution Evidence

Represents exact package resolution for generated framework consumers.

**Fields**
- `RequestedPackages`: package id/version list
- `ResolvedPackages`: package id/version list
- `PackageSources`: source name/path/url list
- `Warnings`: restore warning list
- `ExactMatch`: `bool`
- `FailureReason`: `string option`

**Validation**
- `NU1603` or any requested/resolved mismatch fails verification.
- Local framework versions require a configured local source in the generated project or workflow.
- Evidence must be recorded in readiness output.

## Generated Verification Run

Represents generated product verification depth.

**Fields**
- `RestoreRan`: `bool`
- `GeneratedTestsExist`: `bool`
- `GeneratedTestsRan`: `bool`
- `VerifyRan`: `bool`
- `Authoritative`: `bool`
- `NonAuthoritativeReason`: `string option`

**Validation**
- If generated tests exist, verification fails unless they run.
- Placeholder targets must be labeled non-authoritative.
- Source scans alone cannot satisfy generated verification.

## Visual Game Evidence

Represents proof that a generated game surface is readable and interactive.

**Fields**
- `EvidenceKind`: `screenshot | pixel-readback | unsupported-host`
- `Path`: `string option`
- `BoardReadable`: `bool option`
- `InputOrProgressObserved`: `bool option`
- `FallbackReason`: `string option`
- `UnsupportedReason`: `string option`

**Validation**
- Screenshot is preferred on supported hosts with capture support.
- Pixel-readback is valid only when screenshot capture is unavailable and rendered pixels can be inspected.
- Unsupported-host diagnostics must be explicit when neither visual path is possible.

## Task Workflow Evidence

Represents workflow guidance for cohesive implementation batches and red-green test clusters.

**Fields**
- `BatchName`: `string`
- `Tasks`: task id list
- `SharedEvidence`: path list
- `GraphBefore`: path
- `GraphAfter`: path
- `RedGreenEntries`: command, failing assertion, change reference, final passing command

**Validation**
- Batch guidance cannot skip skill loading or evidence graph validation.
- Red-green logs must preserve at least one failing-first and passing-after command for each related test cluster.
