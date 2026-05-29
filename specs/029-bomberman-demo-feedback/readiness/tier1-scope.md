# Tier 1 Scope Notes

Task: T004
Captured: 2026-05-29T11:46:09+02:00

## Scope

- Tier: Tier 1 contracted change.
- Affected layers: framework packages, generated template, generated build commands, generated guidance, and readiness evidence.
- Candidate public surfaces: `SkiaViewer`, `Testing`, `Elmish`, `Scene`, and `Layout`.
- Template ownership points: `template/base/build.fsx`, `template/base/src/Product/EvidenceCommands.fs`, generated guidance fragments, `.template.config/template.json`, and generated profile guidance.

## Public API Impact

Any public helper must be declared in the matching `.fsi` before implementation in `.fs`. Package surface baselines and compatibility notes are required for intentional public additions.

## MVU / Effect Applicability

Generated game wiring is MVU-bearing. Pure app state, messages, effects, `init`, `update`, `view`, key mapping, and tick mapping must remain app-owned. Viewer launch, screenshot capture, file writing, process execution, native window work, and package validation stay at interpreter or build-target edges.

## Aggregate Reporting

Aggregate targets such as `Verify` are summaries unless the target itself is the documented authority for a task. Story readiness must point to the authoritative generated checkout, package test, FSI transcript, screenshot artifact, or guidance validation log.
