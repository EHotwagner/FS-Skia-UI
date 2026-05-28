# Generated Guidance Contract

## Scope

Applies to generated sample code, generated docs, public docs, guidance
validators, and readiness examples that discuss app geometry names or common
scene/layout integration patterns.

## Required Behavior

- Guidance MUST warn against app-domain records named only `Rect`, `Point`, or
  `Size` when `FS.Skia.UI.Scene` or layout primitives are opened/in scope.
- Guidance MUST include at least three domain-specific geometry examples, such
  as `WorldRect`, `WorldPoint`, `TrackBounds`, `CarPose`, or
  `CheckpointBounds`.
- Generated examples MUST remain understandable without requiring extra type
  annotations solely to resolve common naming collisions.
- Guidance checks MUST fail when reviewed generated guidance recommends generic
  app-domain geometry names in collision-prone contexts.

## Evidence

- `specs/024-racer-feedback-followups/readiness/generated-guidance-validation.md`
  records checked files, validation commands, accepted examples, and rejected
  generic names.
