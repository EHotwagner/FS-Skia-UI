# Package Boundary

Feature: `021-persistent-launch-evidence`

- SkiaViewer owns native viewer launch, first-frame, input dispatch, window
  observation, and controlled-close effects.
- Testing owns generated validation, artifact validation, host warning
  classification, and readiness discovery helpers.
- Scene/layout evidence remains separate from persistent-window evidence.
- Generated product reducers remain pure and do not perform viewer, filesystem,
  process, or window-system effects.

