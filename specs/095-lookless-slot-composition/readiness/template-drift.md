# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Controls/Control.fs` | `source-code` |
| `src/Controls/Control.fsi` | `source-code` |
| `src/Controls/Types.fs` | `source-code` |
| `src/Controls/Types.fsi` | `source-code` |
| `src/Controls/Widgets/Containers.fs` | `source-code` |
| `src/Controls/Widgets/Containers.fsi` | `source-code` |
| `src/Controls/Widgets/Primitives.fs` | `source-code` |
| `src/Controls/Widgets/Primitives.fsi` | `source-code` |
| `src/Controls/skill/SKILL.md` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature095SlotCompositionTests.fs` | `test-code` |

## Required Alignment Classes

- `src/Controls/Control.fs` requires `source-contract`
- `src/Controls/Control.fs` requires `active-feature-evidence`
- `src/Controls/Control.fsi` requires `source-contract`
- `src/Controls/Control.fsi` requires `active-feature-evidence`
- `src/Controls/Types.fs` requires `source-contract`
- `src/Controls/Types.fs` requires `active-feature-evidence`
- `src/Controls/Types.fsi` requires `source-contract`
- `src/Controls/Types.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/Containers.fs` requires `source-contract`
- `src/Controls/Widgets/Containers.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/Containers.fsi` requires `source-contract`
- `src/Controls/Widgets/Containers.fsi` requires `active-feature-evidence`
- `src/Controls/Widgets/Primitives.fs` requires `source-contract`
- `src/Controls/Widgets/Primitives.fs` requires `active-feature-evidence`
- `src/Controls/Widgets/Primitives.fsi` requires `source-contract`
- `src/Controls/Widgets/Primitives.fsi` requires `active-feature-evidence`
- `src/Controls/skill/SKILL.md` requires `source-contract`
- `src/Controls/skill/SKILL.md` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature095SlotCompositionTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature095SlotCompositionTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/095-lookless-slot-composition`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
