# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `Directory.Packages.props` | `dependency-policy` |
| `docs/reports/dependencies.md` | `documentation` |
| `scripts/dependency-report.fsx` | `dependency-policy` |
| `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` | `test-code` |

## Required Alignment Classes

- `Directory.Packages.props` requires `dependency-docs`
- `Directory.Packages.props` requires `active-feature-evidence`
- `docs/reports/dependencies.md` requires `docs-alignment`
- `docs/reports/dependencies.md` requires `active-feature-evidence`
- `scripts/dependency-report.fsx` requires `dependency-docs`
- `scripts/dependency-report.fsx` requires `active-feature-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `test-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/115-dependency-updates`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
