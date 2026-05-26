# Evidence Obligations

Task: T002

## Tier 1 Scope

This feature is broad Tier 1 because it changes public SkiaViewer contracts, generated graphical product defaults, package verification behavior, readiness evidence contracts, and evidence/audit governance.

## Public API Impact

`src/SkiaViewer/SkiaViewer.fsi` is in scope for explicit interactive/evidence launch modes, launch outcome fields, desktop diagnostics, and MVU lifecycle boundary contracts. Surface baseline evidence is required before completion.

## Generated Product Impact

Generated graphical products must default to a persistent interactive launch path. Bounded launch, first-frame, input-dispatch, screenshot, and pixel-readback evidence must be opt-in commands or flags and must not be reported as ongoing play.

## Package Impact

Generated verification must record package sources plus requested and resolved `FS.Skia.UI.*` versions. `NU1603` or exact-version drift is a verification failure, not a warning.

## Unsupported Scope

No new game engine, unrelated chart/control/DataGrid changes, release automation, marketplace distribution, or non-game generated app changes are required beyond shared launch and verification contracts.

## Required Evidence Files

- `readiness/interactive-lifecycle.md`
- `readiness/evidence-launch-mode.md`
- `readiness/container-session-diagnostics.md`
- `readiness/package-resolution.md`
- `readiness/generated-verify.md`
- `readiness/game-visual-evidence.md`
- `readiness/task-workflow-guidance.md`
- `readiness/evidence-graph.md`
- `readiness/evidence-audit.md`
