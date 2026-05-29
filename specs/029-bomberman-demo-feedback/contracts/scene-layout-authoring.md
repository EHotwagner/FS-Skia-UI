# Contract: Scene And Layout Authoring

## Required Guidance Categories

Consumer-facing examples must cover these ambiguity categories:

- coordinates: `Point`, tuple coordinates, and `ViewerWindowPosition.Coordinates`
- dimensions: `Size`, layout bounds, image dimensions, and output size
- diagnostics: viewer diagnostics, layout diagnostics, and evidence report diagnostics
- state: app model state, viewer lifecycle state, and workflow state
- positions: text positions, vertex positions, window positions, and layout positions

## Accepted Patterns

Examples may use:

- explicit type annotations at record construction
- module-qualified helper functions
- small construction helpers in the owning package
- local named values with concrete types before record literals

Examples must avoid:

- relying on ambiguous record field inference near multiple open modules
- broad public renaming solely to avoid sample ambiguity
- adding viewer or host dependencies to Scene/Layout packages

## Validation

Guidance is valid only when `GeneratedGuidanceCheck` or targeted package tests prove the examples remain accurate. If helpers become public API, update `.fsi`, surface baselines, and compatibility notes.
