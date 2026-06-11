# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls/Control.fs` | `source-code` |
| `src/Controls/Control.fsi` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `src/Controls/RetainedRender.fsi` | `source-code` |
| `src/Layout/Layout.fs` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Layout.Tests/Layout.Tests.fsproj` | `test-code` |
| `tests/Layout.Tests/Tests.fs` | `test-code` |
| `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` | `test-code` |
| `tests/Controls.Tests/Feature097WiringTests.fs` | `test-code` |
| `tests/Layout.Tests/Feature097IncrementalTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls/Control.fs` requires `source-contract`
- `src/Controls/Control.fs` requires `active-feature-evidence`
- `src/Controls/Control.fsi` requires `source-contract`
- `src/Controls/Control.fsi` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fsi` requires `source-contract`
- `src/Controls/RetainedRender.fsi` requires `active-feature-evidence`
- `src/Layout/Layout.fs` requires `source-contract`
- `src/Layout/Layout.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Layout.Tests/Layout.Tests.fsproj` requires `test-evidence`
- `tests/Layout.Tests/Layout.Tests.fsproj` requires `active-feature-evidence`
- `tests/Layout.Tests/Tests.fs` requires `test-evidence`
- `tests/Layout.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` requires `test-evidence`
- `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature097WiringTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature097WiringTests.fs` requires `active-feature-evidence`
- `tests/Layout.Tests/Feature097IncrementalTests.fs` requires `test-evidence`
- `tests/Layout.Tests/Feature097IncrementalTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/097-incremental-partial-relayout`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
