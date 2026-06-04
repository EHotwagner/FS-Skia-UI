# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/plan-template.md` | `spec-kit-guidance` |
| `.specify/templates/plan-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `src/Elmish/skill/SKILL.md` | `source-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/Feature061GovernanceTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/plan-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/plan-template.md` requires `active-feature-evidence`
- `.specify/templates/plan-template.md` requires `generated-guidance`
- `.specify/templates/plan-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `src/Elmish/skill/SKILL.md` requires `source-contract`
- `src/Elmish/skill/SKILL.md` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/Feature061GovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/Feature061GovernanceTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, generated-guidance, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/061-breakout-consumer-friction-followups`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
