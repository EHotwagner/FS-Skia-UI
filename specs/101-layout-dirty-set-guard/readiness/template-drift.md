# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls/Control.fs` | `source-code` |
| `src/Controls/RetainedRender.fs` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls/Control.fs` requires `source-contract`
- `src/Controls/Control.fs` requires `active-feature-evidence`
- `src/Controls/RetainedRender.fs` requires `source-contract`
- `src/Controls/RetainedRender.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature101LayoutDriftGuardTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/101-layout-dirty-set-guard`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
