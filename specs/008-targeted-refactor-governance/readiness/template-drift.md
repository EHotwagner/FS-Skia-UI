# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `build.fsx` | `command-surface` |
| `docs/build.md` | `documentation` |
| `docs/evidence.md` | `documentation` |
| `docs/speckit.md` | `documentation` |
| `docs/testing.md` | `documentation` |
| `scripts/template-drift.fsx` | `governance-script` |
| `src/Layout/Layout.fs` | `source-code` |
| `src/Lib/Lib.fsproj` | `source-code` |
| `src/Lib/Library.fs` | `source-code` |
| `tests/Governance.Tests/CommandContractTests.fs` | `test-code` |
| `tests/Governance.Tests/DocsGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/TemplateDriftTests.fs` | `test-code` |
| `tests/Governance.Tests/TestSupport.fs` | `test-code` |
| `tests/Layout.Tests/Layout.Tests.fsproj` | `test-code` |
| `tests/Lib.Tests/Lib.Tests.fsproj` | `test-code` |
| `tests/Package.Tests/SurfaceAreaTests.fs` | `test-code` |
| `src/Lib/InternalsVisibleTo.fs` | `source-code` |
| `src/Lib/VulkanResources.fs` | `source-code` |
| `src/Lib/VulkanResources.fsi` | `source-code` |
| `src/Lib/VulkanStartup.fs` | `source-code` |
| `src/Lib/VulkanStartup.fsi` | `source-code` |
| `tests/Governance.Tests/PublicRecordInvariantTests.fs` | `test-code` |
| `tests/Governance.Tests/RuntimeOrganizationTests.fs` | `test-code` |
| `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` | `test-code` |
| `tests/Lib.Tests/NativeStartupCleanupTests.fs` | `test-code` |

## Required Alignment Classes

- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/build.md` requires `docs-alignment`
- `docs/build.md` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `docs/speckit.md` requires `docs-alignment`
- `docs/speckit.md` requires `active-feature-evidence`
- `docs/testing.md` requires `docs-alignment`
- `docs/testing.md` requires `active-feature-evidence`
- `scripts/template-drift.fsx` requires `template-drift-docs`
- `scripts/template-drift.fsx` requires `active-feature-evidence`
- `src/Layout/Layout.fs` requires `source-contract`
- `src/Layout/Layout.fs` requires `active-feature-evidence`
- `src/Lib/Lib.fsproj` requires `source-contract`
- `src/Lib/Lib.fsproj` requires `active-feature-evidence`
- `src/Lib/Library.fs` requires `source-contract`
- `src/Lib/Library.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/DocsGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/DocsGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/TemplateDriftTests.fs` requires `test-evidence`
- `tests/Governance.Tests/TemplateDriftTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `test-evidence`
- `tests/Governance.Tests/TestSupport.fs` requires `active-feature-evidence`
- `tests/Layout.Tests/Layout.Tests.fsproj` requires `test-evidence`
- `tests/Layout.Tests/Layout.Tests.fsproj` requires `active-feature-evidence`
- `tests/Lib.Tests/Lib.Tests.fsproj` requires `test-evidence`
- `tests/Lib.Tests/Lib.Tests.fsproj` requires `active-feature-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `test-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `active-feature-evidence`
- `src/Lib/InternalsVisibleTo.fs` requires `source-contract`
- `src/Lib/InternalsVisibleTo.fs` requires `active-feature-evidence`
- `src/Lib/VulkanResources.fs` requires `source-contract`
- `src/Lib/VulkanResources.fs` requires `active-feature-evidence`
- `src/Lib/VulkanResources.fsi` requires `source-contract`
- `src/Lib/VulkanResources.fsi` requires `active-feature-evidence`
- `src/Lib/VulkanStartup.fs` requires `source-contract`
- `src/Lib/VulkanStartup.fs` requires `active-feature-evidence`
- `src/Lib/VulkanStartup.fsi` requires `source-contract`
- `src/Lib/VulkanStartup.fsi` requires `active-feature-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `active-feature-evidence`
- `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` requires `test-evidence`
- `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` requires `active-feature-evidence`
- `tests/Lib.Tests/NativeStartupCleanupTests.fs` requires `test-evidence`
- `tests/Lib.Tests/NativeStartupCleanupTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, generated-guidance, sample-contract, source-contract, speckit-docs, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/008-targeted-refactor-governance`

## Diagnostics

- No drift blockers.

## Post-Analysis Documentation Alignment

- `docs/V2Analysis.md` was updated after the 008 implementation pass to record
  the deterministic harness versus nondeterministic AI boundary analysis. This
  is documentation alignment for the active governance feature, not a template
  runtime or public API change.
