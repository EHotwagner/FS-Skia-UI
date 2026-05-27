# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `docs/generated-apps.md` | `documentation` |
| `src/Scene/Scene.fs` | `source-code` |
| `src/Scene/Scene.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `src/Testing/Testing.fs` | `source-code` |
| `src/Testing/Testing.fsi` | `source-code` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Package.Tests/SurfaceAreaTests.fs` | `test-code` |
| `tests/Scene.Tests/Tests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `tests/Testing.Tests/Tests.fs` | `test-code` |

## Required Alignment Classes

- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `src/Scene/Scene.fs` requires `source-contract`
- `src/Scene/Scene.fs` requires `active-feature-evidence`
- `src/Scene/Scene.fsi` requires `source-contract`
- `src/Scene/Scene.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `src/Testing/Testing.fs` requires `source-contract`
- `src/Testing/Testing.fs` requires `active-feature-evidence`
- `src/Testing/Testing.fsi` requires `source-contract`
- `src/Testing/Testing.fsi` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `test-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `active-feature-evidence`
- `tests/Scene.Tests/Tests.fs` requires `test-evidence`
- `tests/Scene.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Testing.Tests/Tests.fs` requires `test-evidence`
- `tests/Testing.Tests/Tests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/022-breakout-demo-feedback`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
