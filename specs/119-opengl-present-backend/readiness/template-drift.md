# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `Directory.Packages.props` | `dependency-policy` |
| `docs/architecture/host-skiaviewer.md` | `documentation` |
| `src/SkiaViewer/Host/Diagnostics.fs` | `source-code` |
| `src/SkiaViewer/Host/Diagnostics.fsi` | `source-code` |
| `src/SkiaViewer/Host/Viewer.fs` | `source-code` |
| `src/SkiaViewer/Host/Vulkan.fs` | `source-code` |
| `src/SkiaViewer/Host/Vulkan.fsi` | `source-code` |
| `src/SkiaViewer/PresentMode.fs` | `source-code` |
| `src/SkiaViewer/PresentMode.fsi` | `source-code` |
| `src/SkiaViewer/SceneRenderer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fs` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsi` | `source-code` |
| `src/SkiaViewer/SkiaViewer.fsproj` | `source-code` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/DependencyGovernanceTests.fs` | `test-code` |
| `tests/Governance.Tests/Feature061GovernanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GovernanceEvidenceTests.fs` | `test-code` |
| `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` | `test-code` |
| `tests/Governance.Tests/RuntimeOrganizationTests.fs` | `test-code` |
| `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` | `test-code` |
| `tests/Lib.Tests/Tests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/NativeStartupCleanupTests.fs` | `test-code` |
| `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` | `test-code` |
| `tests/SkiaViewer.Tests/Tests.fs` | `test-code` |
| `src/SkiaViewer/Host/OpenGl.fs` | `source-code` |
| `src/SkiaViewer/Host/OpenGl.fsi` | `source-code` |
| `tests/SkiaViewer.Tests/Feature119OpenGlHostTests.fs` | `test-code` |

## Required Alignment Classes

- `Directory.Packages.props` requires `dependency-docs`
- `Directory.Packages.props` requires `active-feature-evidence`
- `docs/architecture/host-skiaviewer.md` requires `docs-alignment`
- `docs/architecture/host-skiaviewer.md` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `source-contract`
- `src/SkiaViewer/Host/Diagnostics.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Viewer.fs` requires `source-contract`
- `src/SkiaViewer/Host/Viewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Vulkan.fs` requires `source-contract`
- `src/SkiaViewer/Host/Vulkan.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/Vulkan.fsi` requires `source-contract`
- `src/SkiaViewer/Host/Vulkan.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/PresentMode.fs` requires `source-contract`
- `src/SkiaViewer/PresentMode.fs` requires `active-feature-evidence`
- `src/SkiaViewer/PresentMode.fsi` requires `source-contract`
- `src/SkiaViewer/PresentMode.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SceneRenderer.fs` requires `source-contract`
- `src/SkiaViewer/SceneRenderer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fs` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fs` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsi` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsi` requires `active-feature-evidence`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `source-contract`
- `src/SkiaViewer/SkiaViewer.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/DependencyGovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/DependencyGovernanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Feature061GovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/Feature061GovernanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GovernanceEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PersistentViewerEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/SyntheticErrorEvidenceTests.fs` requires `active-feature-evidence`
- `tests/Lib.Tests/Tests.fs` requires `test-evidence`
- `tests/Lib.Tests/Tests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature118PresentModeTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/NativeStartupCleanupTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/NativeStartupCleanupTests.fs` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `test-evidence`
- `tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Tests.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fs` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fs` requires `active-feature-evidence`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `source-contract`
- `src/SkiaViewer/Host/OpenGl.fsi` requires `active-feature-evidence`
- `tests/SkiaViewer.Tests/Feature119OpenGlHostTests.fs` requires `test-evidence`
- `tests/SkiaViewer.Tests/Feature119OpenGlHostTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/119-opengl-present-backend`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
