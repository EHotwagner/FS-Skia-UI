# Data Model: Breakout Demo Feedback

## Generated Viewer Contract

- **Fields**: contract name, package source, generated source reference,
  generated test reference, quickstart reference, readiness wording.
- **Relationships**: Used by generated app source, tests, docs, and
  `GeneratedGuidanceCheck`.
- **Validation rules**: Every referenced public name must exist in the packed
  package consumed by a fresh generated app. Stale names fail guidance checks.
- **State transitions**: candidate -> packed-surface verified -> generated
  guidance adopted -> readiness audited.

## Scene Shape Primitive

- **Fields**: shape kind, center or bounds, radius where applicable, fill color,
  optional paint, deterministic evidence bounds, placement.
- **Relationships**: Public Scene `.fsi`, renderer implementation, deterministic
  evidence, generated visual examples, surface baselines.
- **Validation rules**: Circle and ellipse output must expose deterministic
  bounds, fill, and placement evidence; filled shape support is required before
  painted variants are considered complete.
- **State transitions**: signature drafted -> semantic tests fail -> renderer
  implemented -> generated examples verified.

## Screenshot Evidence Result

- **Fields**: status, command, output path, screenshot path, width, height,
  renderer mode, frames rendered, diagnostics, unsupported-host reason,
  deterministic fallback.
- **Relationships**: SkiaViewer screenshot API, generated evidence command,
  generated report convention, readiness artifact.
- **Validation rules**: A success result must include bounded screenshot facts.
  An unsupported result must include command, status, reason, and fallback, and
  must not claim screenshot proof.
- **State transitions**: requested -> capture attempted -> captured or
  unsupported -> report written -> audit classified.

## Effect Boundary Example

- **Fields**: model, msg, app command, pure update, host update, viewer effect,
  view function, interpreter boundary.
- **Relationships**: Generated source, Elmish/SkiaViewer guidance, generated
  tests, docs.
- **Validation rules**: Pure app update must not render, write files, open
  windows, or capture screenshots. Viewer effects must be emitted at the host
  boundary or interpreted by evidence commands.
- **State transitions**: app message -> pure model transition -> app command
  emitted -> host maps next model to viewer effects.

## Evidence Report Convention

- **Fields**: status, command, output, evidence kind, unsupported-host reason,
  fallback, diagnostics, exit code, ordered key-value lines.
- **Relationships**: Generated evidence commands, Testing helpers, readiness
  files, stdout output, governance checks.
- **Validation rules**: Parent directories are created, fields are written in
  stable order, stdout echoes file content, normalized statuses are used, and
  success/unsupported/failure exit behavior is consistent.
- **State transitions**: fields built -> normalized -> directory ensured ->
  file written -> stdout echoed -> exit code returned.

## Geometry Guidance

- **Fields**: Rect, Point, Size, optional app alias, collision bounds, layout
  evidence bounds, rendering bounds.
- **Relationships**: Scene primitives, generated game guidance, layout evidence,
  app-owned gameplay model.
- **Validation rules**: Generated apps should reuse Scene geometry when it fits
  the app model and avoid duplicate record labels that create F# inference
  ambiguity. App-owned geometry is allowed when domain semantics require it.
