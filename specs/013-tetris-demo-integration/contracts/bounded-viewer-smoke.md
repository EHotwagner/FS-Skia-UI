# Contract: Bounded Viewer Smoke

## Public Surface

Add bounded execution helpers equivalent to:

```fsharp
type ViewerEvidenceTarget =
    | FirstFrame
    | FrameCount of int
    | Duration of System.TimeSpan

type ViewerRunEvidence =
    { FramesRendered: int
      Elapsed: System.TimeSpan
      InitialOutputSize: int * int
      RendererMode: string
      LastDiagnosticSummary: string option }

type ViewerRunFailure =
    { BlockedStage: string
      Classification: string
      DiagnosticCategory: string
      Message: string
      LastDiagnosticSummary: string option }

module Viewer =
    val runUntilFirstFrame :
        ViewerProgram<'model,'msg> -> Result<ViewerRunEvidence, ViewerRunFailure>

    val runForFrames :
        frameCount:int ->
        ViewerProgram<'model,'msg> ->
            Result<ViewerRunEvidence, ViewerRunFailure>
```

Names and records may be adapted to existing viewer conventions, but the
observable contract must include the same evidence and failure semantics.

## Required Behavior

- First-frame mode exits successfully after at least one rendered frame.
- Frame-count mode exits after the requested positive frame count.
- Pre-frame failures name the blocked stage.
- Unsupported host conditions are classified separately from product defects.
- Success does not rely on external shell timeout or log scraping.

## Evidence

- Bounded viewer success test on supported host or explicit unsupported-host
  evidence.
- Forced pre-frame failure tests for structured stage diagnostics.
- Generated consumer graphical smoke command evidence.
- Readiness: `readiness/bounded-viewer-smoke.md`.
