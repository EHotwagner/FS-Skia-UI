# Template Drift Report

FAIL

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `src/Scene/Scene.fs` | `source-code` |

## Required Alignment Classes

- `src/Scene/Scene.fs` requires `source-contract`
- `src/Scene/Scene.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: ``
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/088-governance-precision-hardening`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- src/Scene/Scene.fs: path class `source-code` is missing same-diff required alignment class `source-contract`.
- src/Scene/Scene.fs: path class `source-code` is missing active feature evidence naming the changed path or affected feature area; required alignment class `active-feature-evidence`.
