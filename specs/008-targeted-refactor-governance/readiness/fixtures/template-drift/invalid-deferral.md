# Template Drift Report

FAIL

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `scripts/template-drift.fsx` | `governance-script` |

## Required Alignment Classes

- `scripts/template-drift.fsx` requires `template-drift-docs`
- `scripts/template-drift.fsx` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `template-drift-docs`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/106-controls-api-discoverability`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- deferral `fixture-invalid` is missing owner, target_phase
- scripts/template-drift.fsx: path class `governance-script` is missing active feature evidence naming the changed path or affected feature area; required alignment class `active-feature-evidence`.
