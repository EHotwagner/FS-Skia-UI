# Package Surface Expectations

Recorded: 2026-05-28T17:30:00+02:00

## Changed Public Contracts

- `FS.Skia.UI` gains `FS.Skia.UI.AgentValidation` with validation selection
  MVU contracts, target metadata, verdict values, failure classification, and
  serializer front doors.
- `FS.Skia.UI.Controls` gains typed standard control kinds, event kinds,
  attribute names, standard attribute values, control schema values, typed
  standard creation/lowering helpers, visibly custom helpers, schema access,
  and schema-owned diagnostics.

## Baselines Expected To Change

- `readiness/surface-baselines/FS.Skia.UI.txt`
- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`

## Baselines Expected To Stay Stable

- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`
- `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
- `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`
- `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`
- `readiness/surface-baselines/FS.Skia.UI.Layout.txt`
- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`
- `readiness/surface-baselines/FS.Skia.UI.Testing.txt`

Intentional baseline refresh remains deferred until the implementation tasks
that own the public behavior are complete and `RefreshSurfaceBaselines` is run
through governed validation.
