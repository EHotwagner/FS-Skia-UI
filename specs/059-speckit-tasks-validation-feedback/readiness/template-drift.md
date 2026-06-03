# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` | `spec-kit-guidance` |
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/SkillValidationTests.fs` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` | `test-code` |
| `tests/Governance.Tests/OwnsValidationTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-deps-template.yml` requires `active-feature-evidence`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/OwnsValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/OwnsValidationTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, generated-guidance, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/059-speckit-tasks-validation-feedback`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
