# Governance Risk Levels

Status: foundation complete.

Task: T014
Captured: 2026-05-29T11:53:00+02:00

## Risk Level

Overall risk: broad.

Reasons:

- Public `.fsi` surfaces may change in SkiaViewer, Testing, Elmish, Scene, and Layout.
- Generated template behavior and generated app defaults may change.
- Screenshot evidence and generated app launch behavior require real host evidence.
- Aggregate `Verify` output is non-authoritative unless it points to story-specific logs.

## Story Checks

| Story | Focused checks | Broad checks | Real evidence obligation |
|-------|----------------|--------------|--------------------------|
| US1 Reliable evidence commands | Template/generated command tests, governance command validation | `TemplateCheck`, `GeneratedProductCheck`, generated checkout smoke | `readiness/evidence-graph-invocation.md`, `readiness/verify-log-cleanliness.md` |
| US2 Truthful screenshot evidence | `SkiaViewer.Tests`, `Testing.Tests`, screenshot report parser/validator tests | generated screenshot command, supported-host artifact or deferred supported-host path | `readiness/screenshot-evidence-probe.md` |
| US3 Generated game wiring | `Elmish.Tests`, `SkiaViewer.Tests`, generated product tests | generated app persistent launch smoke and FSI/smoke host exercise | `readiness/generated-app-wiring.md` |
| US4 Scene/layout authoring | `Scene.Tests`, `Layout.Tests`, generated guidance validation | `GeneratedGuidanceCheck`, dependency boundary scan | `readiness/scene-layout-authoring.md` |

## Risk Vocabulary

- small: docs-only or generated guidance text would use focused guidance validation.
- medium: package helper, generated command, or template behavior uses focused package and generated checks.
- broad: this feature uses broad validation because it touches generated behavior, screenshot proof, public package surfaces, and host wiring.
- required evidence: named readiness files, package tests, generated validation, graph/audit, and final `Verify`.
- broad validation: `TemplateCheck`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `PackageSurfaceCheck`, `EvidenceGraph`, `EvidenceAudit`, and `Verify`.

## Authority Rules

- `Verify` is a final aggregate summary, not a substitute for per-story evidence.
- Generated validation is authoritative only when it runs inside or against the generated checkout path.
- Package surface checks are authoritative for `.fsi`/baseline drift, not for generated app launch success.
- Screenshot success requires a decodable nonblank image artifact; deterministic metadata alone is not screenshot proof.
