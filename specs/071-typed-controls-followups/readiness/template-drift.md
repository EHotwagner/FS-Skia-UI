# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `samples/ControlsGallery/Program.fs` | `sample-code` |
| `src/Controls/Catalog.fs` | `source-code` |
| `src/Controls/catalog.yml` | `source-code` |
| `tests/Controls.Tests/AccessibilityTests.fs` | `test-code` |
| `tests/Controls.Tests/CatalogTests.fs` | `test-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/RenderingTests.fs` | `test-code` |
| `tests/Controls.Tests/TypedGalleryPanel.fs` | `test-code` |

## Required Alignment Classes

- `samples/ControlsGallery/Program.fs` requires `sample-contract`
- `samples/ControlsGallery/Program.fs` requires `active-feature-evidence`
- `src/Controls/Catalog.fs` requires `source-contract`
- `src/Controls/Catalog.fs` requires `active-feature-evidence`
- `src/Controls/catalog.yml` requires `source-contract`
- `src/Controls/catalog.yml` requires `active-feature-evidence`
- `tests/Controls.Tests/AccessibilityTests.fs` requires `test-evidence`
- `tests/Controls.Tests/AccessibilityTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/CatalogTests.fs` requires `test-evidence`
- `tests/Controls.Tests/CatalogTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/RenderingTests.fs` requires `test-evidence`
- `tests/Controls.Tests/RenderingTests.fs` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedGalleryPanel.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedGalleryPanel.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/071-typed-controls-followups`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
