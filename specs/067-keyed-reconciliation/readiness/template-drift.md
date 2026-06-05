# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls/Controls.fsproj` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `src/Controls/Reconcile.fs` | `source-code` |
| `src/Controls/Reconcile.fsi` | `source-code` |
| `tests/Controls.Tests/ReconcileTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls/Controls.fsproj` requires `source-contract`
- `src/Controls/Controls.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `src/Controls/Reconcile.fs` requires `source-contract`
- `src/Controls/Reconcile.fs` requires `active-feature-evidence`
- `src/Controls/Reconcile.fsi` requires `source-contract`
- `src/Controls/Reconcile.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/ReconcileTests.fs` requires `test-evidence`
- `tests/Controls.Tests/ReconcileTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/067-keyed-reconciliation`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
