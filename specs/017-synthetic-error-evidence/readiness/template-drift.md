# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/constitution-template.md` | `spec-kit-guidance` |
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | `spec-kit-guidance` |
| `.specify/templates/constitution-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `build.fsx` | `command-surface` |
| `docs/evidence.md` | `documentation` |
| `docs/speckit.md` | `documentation` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Smoke.Tests/Tests.fs` | `test-code` |
| `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/constitution-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/constitution-template.md` requires `active-feature-evidence`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `active-feature-evidence`
- `.specify/templates/constitution-template.md` requires `generated-guidance`
- `.specify/templates/constitution-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `docs/speckit.md` requires `docs-alignment`
- `docs/speckit.md` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Smoke.Tests/Tests.fs` requires `test-evidence`
- `tests/Smoke.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, generated-guidance, sample-contract, source-contract, speckit-docs, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/017-synthetic-error-evidence`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
