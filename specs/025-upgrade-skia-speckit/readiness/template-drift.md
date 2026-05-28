# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.template.package/FS.Skia.UI.Template.fsproj` | `template-manifest` |
| `Directory.Packages.props` | `dependency-policy` |
| `build.fsx` | `command-surface` |
| `docs/dependencies.md` | `documentation` |
| `docs/template-profile.md` | `documentation` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` | `test-code` |

## Required Alignment Classes

- `.template.package/FS.Skia.UI.Template.fsproj` requires `template-profile`
- `.template.package/FS.Skia.UI.Template.fsproj` requires `active-feature-evidence`
- `Directory.Packages.props` requires `dependency-docs`
- `Directory.Packages.props` requires `active-feature-evidence`
- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/dependencies.md` requires `docs-alignment`
- `docs/dependencies.md` requires `active-feature-evidence`
- `docs/template-profile.md` requires `docs-alignment`
- `docs/template-profile.md` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `test-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/025-upgrade-skia-speckit`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
