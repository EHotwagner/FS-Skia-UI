# Data Model: Tetris Demo Integration Improvements

## ViewerKey

Stable public input value derived from viewer key-down/key-up events.

Fields and cases:
- `ArrowLeft`, `ArrowRight`, `ArrowUp`, `ArrowDown`
- `Enter`, `Space`, `Escape`, `Backspace`
- `Letter` with normalized uppercase or lowercase policy documented by the
  public contract
- `Digit` with integer value `0` through `9`
- `Function` with integer value for function keys
- `Unknown` with preserved raw string

Validation rules:
- Common alternate raw names for the same key map to one value.
- Unknown or unsupported raw values remain observable and do not throw.
- Key-down and key-up preserve the normalized key and down/up state.

## ViewerRunRequest

Bounded real viewer execution request.

Fields:
- `EvidenceTarget`: first frame, exact frame count, or bounded duration
- `Timeout`: maximum elapsed time before structured failure
- `Diagnostics`: selected diagnostic level/categories/sampling/sink
- `RendererMode`: requested live viewer renderer path

Validation rules:
- Frame count must be positive.
- Timeout must be positive.
- Unsupported renderer/window/surface conditions are classified separately
  from app/rendering defects.

## ViewerRunEvidence

Structured evidence returned by a successful bounded real viewer run.

Fields:
- `FramesRendered`
- `Elapsed`
- `InitialOutputSize`
- `RendererMode`
- `LastDiagnosticSummary`
- `EvidencePath`

Validation rules:
- First-frame success requires `FramesRendered >= 1`.
- Output size must be known for successful graphical startup evidence.
- Evidence must not be inferred from process timeout or stderr scraping.

## ViewerRunFailure

Structured failure returned by bounded real viewer execution.

Fields:
- `BlockedStage`: window, surface, renderer, swapchain, scene, readback, app,
  timeout, or unknown
- `Classification`: unsupported environment or product defect
- `DiagnosticCategory`
- `Message`
- `LastDiagnosticSummary`

Validation rules:
- Every failure names the blocked stage.
- Unsupported host capabilities are not reported as successful product
  evidence.

## ViewerDiagnosticEvent

Capturable viewer diagnostic event.

Fields:
- `Level`
- `Category`
- `Message`
- `FrameIndex` optional
- `Stage` optional
- `Timestamp` or elapsed offset

Validation rules:
- Startup-only configuration excludes repeated frame-loop messages.
- Frame diagnostics appear only when frame category or sampling enables them.
- Tests can capture events through an in-process sink.

## SceneEvidenceRequest

Deterministic non-window scene evidence request.

Fields:
- `Scene`
- `OutputSize`
- `EvidenceFormat`: hash, PNG, or metadata report
- `RendererMode`: deterministic scene-level renderer

Validation rules:
- Does not open a native desktop window.
- Returns explicit unsupported-environment diagnostics when required
  rendering/readback capabilities are unavailable.
- Does not replace bounded real viewer startup evidence.

## GeneratedInputFlow

Template-owned input flow that a generated graphical app must expose and test.

Fields:
- `Screen`: initial, options, main interaction, pause/back, end/restart
- `ViewerEvent`
- `NormalizedInput`
- `ExpectedTransition`
- `EvidencePath`

Validation rules:
- Start/options/primary interaction/restart flows are covered when the
  generated screen exists.
- At least one automated flow starts from the initial screen through a viewer
  key event.

## LocalConsumerPackageReport

Local package/feed guidance output for generated consumers.

Fields:
- `FeedPath`
- `PackageIdentities`
- `PackageVersions`
- `ConsumerConfigSnippet`
- `RestoreCommand`
- `DriftDiagnostics`

Validation rules:
- Missing or stale packages are setup drift, not app source failures.
- Output is sufficient to configure a generated consumer against the local
  feed without manual package inventory work.
