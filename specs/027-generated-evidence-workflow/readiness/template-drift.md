# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `docs/evidence.md` | `documentation` |
| `docs/generated-apps.md` | `documentation` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs` | `test-code` |
| `tests/Governance.Tests/GovernanceEvidenceTests.fs` | `test-code` |
| `tests/Governance.Tests/SkillValidationTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/SkillValidationTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, generated-guidance, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/027-generated-evidence-workflow`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
