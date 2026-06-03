# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `docs/reports/_baselines/2026-06-02-foundations-after.md` | `documentation` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/GuidanceValidatorTests.fs` | `test-code` |
| `tests/Governance.Tests/ProseSizeAccountingTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-06-02-foundations-after.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-06-02-foundations-after.md` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/GuidanceValidatorTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GuidanceValidatorTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ProseSizeAccountingTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ProseSizeAccountingTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, generated-guidance, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/055-decouple-guidance-anchors`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
