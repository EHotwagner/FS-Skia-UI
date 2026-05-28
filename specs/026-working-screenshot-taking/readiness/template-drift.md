# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.template.package/FS.Skia.UI.Template.fsproj` | `template-manifest` |
| `docs/evidence.md` | `documentation` |
| `docs/template-profile.md` | `documentation` |
| `docs/testing.md` | `documentation` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsproj` | `source-code` |
| `src/Testing/Testing.fs` | `source-code` |
| `src/Testing/Testing.fsi` | `source-code` |
| `src/Testing/Testing.fsproj` | `source-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `tests/Testing.Tests/Tests.fs` | `test-code` |

## Required Alignment Classes

- `.template.package/FS.Skia.UI.Template.fsproj` requires `template-profile`
- `.template.package/FS.Skia.UI.Template.fsproj` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `docs/template-profile.md` requires `docs-alignment`
- `docs/template-profile.md` requires `active-feature-evidence`
- `docs/testing.md` requires `docs-alignment`
- `docs/testing.md` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `active-feature-evidence`
- `src/Testing/Testing.fs` requires `source-contract`
- `src/Testing/Testing.fs` requires `active-feature-evidence`
- `src/Testing/Testing.fsi` requires `source-contract`
- `src/Testing/Testing.fsi` requires `active-feature-evidence`
- `src/Testing/Testing.fsproj` requires `source-contract`
- `src/Testing/Testing.fsproj` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/Testing.Tests/Tests.fs` requires `test-evidence`
- `tests/Testing.Tests/Tests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/026-working-screenshot-taking`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
