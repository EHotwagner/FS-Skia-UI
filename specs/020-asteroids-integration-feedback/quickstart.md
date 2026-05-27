# Quickstart: Asteroids Integration Feedback

## Failing-First Checks

1. Add semantic tests for the public layout evidence contract through the
   packed or prelude-loaded `.fsi` surface.
2. Add generated validation tests that fail when HUD text overlaps other HUD
   text or gameplay bounds at default and constrained sizes.
3. Add guidance tests that require qualified `Product.Program.view`,
   `Product.Program.generatedHost`, and `Product.Program.update` examples.
4. Add governance tests requiring `fs-skia-layout-evidence` in task metadata
   for layout/evidence/guidance/warning-classification tasks.
5. Add readiness classification tests that keep known benign host warnings
   non-fatal only when launch/render/layout/package evidence is otherwise
   acceptable.

## Expected Commands

```bash
dotnet test tests/Scene.Tests/Scene.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
dotnet test tests/Governance.Tests/Governance.Tests.fsproj
dotnet fake build -t GeneratedGuidanceCheck
dotnet fake build -t GeneratedProductCheck
dotnet fake build -t TemplateCheck
dotnet fake build -t EvidenceGraph
dotnet fake build -t EvidenceAudit
```

## Required Readiness Files

- `specs/020-asteroids-integration-feedback/readiness/hud-layout-readability.md`
- `specs/020-asteroids-integration-feedback/readiness/public-contract-guidance.md`
- `specs/020-asteroids-integration-feedback/readiness/layout-evidence.md`
- `specs/020-asteroids-integration-feedback/readiness/host-warning-classification.md`
- `specs/020-asteroids-integration-feedback/readiness/generated-validation.md`
- `specs/020-asteroids-integration-feedback/readiness/evidence-audit.md`

Each readiness file must state the command run, artifact paths, pass/fail or
unsupported status, and whether the evidence proves readable layout,
deterministic rendering only, or an unsupported condition.

## Implementation Order

1. Create `fs-skia-layout-evidence` and wire it into the capability/task
   inventory.
2. Define public `.fsi` contracts for layout evidence and generated validation.
3. Add failing tests and guidance checks against the new public contracts.
4. Update generated game layout so HUD and gameplay regions are separate.
5. Add evidence/report generation and overlap validation.
6. Add host warning classification reporting.
7. Produce required readiness artifacts and rerun graph/audit checks.
