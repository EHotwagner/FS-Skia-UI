# Contract: Headless Scene Evidence

## Public Surface

Add official non-window scene evidence helpers equivalent to:

```fsharp
type SceneEvidenceFormat =
    | Hash
    | Png
    | Metadata

type SceneEvidence =
    { Format: SceneEvidenceFormat
      OutputSize: int * int
      RendererMode: string
      Value: string }

module SceneEvidence =
    val renderHash :
        size:int * int -> scene:SceneNode -> Result<SceneEvidence, ViewerRunFailure>

    val renderPng :
        size:int * int -> scene:SceneNode -> Result<byte[], ViewerRunFailure>
```

The actual package/module placement must follow existing Scene/Testing/viewer
ownership and may use deterministic scene-level rendering rather than the live
viewer backend.

## Required Behavior

- Produces deterministic visual evidence for a representative generated app
  scene without opening a native desktop window.
- Reports unsupported-environment diagnostics when rendering/readback
  capability is unavailable.
- Does not satisfy or replace bounded real-viewer startup evidence.

## Evidence

- Deterministic scene evidence for at least one generated graphical app scene,
  or explicit unsupported-host evidence.
- Test that no native viewer/window path is required for scene evidence.
- Readiness: `readiness/headless-scene-evidence.md`.
