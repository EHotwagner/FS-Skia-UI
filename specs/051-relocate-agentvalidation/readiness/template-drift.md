# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Lib/Lib.fsproj` | `source-code` |
| `tests/Governance.Tests/AgentValidationFrameworkTests.fs` | `test-code` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |

## Required Alignment Classes

- `src/Lib/Lib.fsproj` requires `source-contract`
- `src/Lib/Lib.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/051-relocate-agentvalidation`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
