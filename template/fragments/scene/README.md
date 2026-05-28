# Scene Fragment

Adds Scene package references and pure scene authoring guidance.

Generated game, chart, and interaction examples should use shared Scene geometry
and first-class shape primitives when the entity is circular or
elliptical. Prefer `Scene.circle`, `Scene.filledEllipse`, `Circle`, or
`FilledEllipse` over rectangle substitutions for balls, markers, pucks,
projectiles, cursors, and status indicators. Reuse the same bounds evidence for
layout checks, containment checks, collision checks, and rendering facts.
When product code needs app-owned geometry types, use domain names such as
`WorldRect`, `WorldPoint`, `TrackBounds`, `CarPose`, or `CheckpointBounds`.
Avoid using bare `Rect`, `Point`, or `Size` for app-domain concepts while Scene
and layout primitives are in scope.
