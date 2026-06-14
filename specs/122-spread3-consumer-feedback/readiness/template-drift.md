# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.specify/presets/fsharp-opinionated/templates/tasks-template.md` | `spec-kit-guidance` |
| `.specify/templates/tasks-template.md` | `spec-kit-guidance` |
| `docs/controls/catalog.md` | `documentation` |
| `docs/controls/custom-control.md` | `documentation` |
| `src/Controls.Elmish/ControlsElmish.fs` | `source-code` |
| `src/Controls.Elmish/ControlsElmish.fsi` | `source-code` |
| `src/Controls/Catalog.fs` | `source-code` |
| `src/Controls/CustomControl.fs` | `source-code` |
| `src/Controls/catalog.yml` | `source-code` |
| `src/Controls/skill/SKILL.md` | `source-code` |
| `src/SkiaViewer/Host/OpenGl.fs` | `source-code` |
| `src/SkiaViewer/Host/OpenGl.fsi` | `source-code` |
| `tests/Controls.Tests/Controls.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` | `test-code` |
| `tests/Controls.Tests/Feature122CustomControlTests.fs` | `test-code` |
| `tests/Governance.Tests/Feature122TemplateThreadingTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature122PresentPathTests.fs` | `test-code` |

## Required Alignment Classes

- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `generated-guidance`
- `.specify/presets/fsharp-opinionated/templates/tasks-template.md` requires `active-feature-evidence`
- `.specify/templates/tasks-template.md` requires `generated-guidance`
- `.specify/templates/tasks-template.md` requires `active-feature-evidence`
- `docs/controls/catalog.md` requires `docs-alignment`
- `docs/controls/catalog.md` requires `active-feature-evidence`
- `docs/controls/custom-control.md` requires `docs-alignment`
- `docs/controls/custom-control.md` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fs` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fs` requires `active-feature-evidence`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `source-contract`
- `src/Controls.Elmish/ControlsElmish.fsi` requires `active-feature-evidence`
- `src/Controls/Catalog.fs` requires `source-contract`
- `src/Controls/Catalog.fs` requires `active-feature-evidence`
- `src/Controls/CustomControl.fs` requires `source-contract`
- `src/Controls/CustomControl.fs` requires `active-feature-evidence`
- `src/Controls/catalog.yml` requires `source-contract`
- `src/Controls/catalog.yml` requires `active-feature-evidence`
- `src/Controls/skill/SKILL.md` requires `source-contract`
- `src/Controls/skill/SKILL.md` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fs` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `test-evidence`
- `tests/Controls.Tests/Controls.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `test-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `active-feature-evidence`
- `tests/Controls.Tests/Feature122CustomControlTests.fs` requires `test-evidence`
- `tests/Controls.Tests/Feature122CustomControlTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Feature122TemplateThreadingTests.fs` requires `test-evidence`
- `tests/Governance.Tests/Feature122TemplateThreadingTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature122PresentPathTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature122PresentPathTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, generated-guidance, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/122-spread3-consumer-feedback`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
