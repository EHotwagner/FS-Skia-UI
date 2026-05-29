# Template Drift Report

FAIL

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Lib/Library.fs` | `source-code` |

## Required Alignment Classes

- `src/Lib/Library.fs` requires `source-contract`
- `src/Lib/Library.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: ``
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/031-serialize-fake-runs`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- src/Lib/Library.fs: path class `source-code` is missing same-diff required alignment class `source-contract`.
- src/Lib/Library.fs: path class `source-code` is missing active feature evidence naming the changed path or affected feature area; required alignment class `active-feature-evidence`.
