# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `docs/reports/dependencies.md` | `documentation` |
| `scripts/template-drift.fsx` | `governance-script` |
| `src/Lib/InternalsVisibleTo.fs` | `source-code` |
| `src/Lib/Lib.fsproj` | `source-code` |
| `src/Lib/Library.fs` | `source-code` |
| `src/Lib/Library.fsi` | `source-code` |
| `tests/Controls.Tests/DiagnosticsTests.fs` | `test-code` |
| `tests/Governance.Tests/AgentValidationFrameworkTests.fs` | `test-code` |
| `tests/Governance.Tests/ArtifactPathTests.fs` | `test-code` |
| `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` | `test-code` |
| `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` | `test-code` |
| `tests/Governance.Tests/DependencyGovernanceTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs` | `test-code` |
| `tests/Governance.Tests/PerPackageSurfaceTests.fs` | `test-code` |
| `tests/Governance.Tests/PublicRecordInvariantTests.fs` | `test-code` |
| `tests/Governance.Tests/RoutingTests.fs` | `test-code` |
| `tests/Governance.Tests/RuntimeOrganizationTests.fs` | `test-code` |
| `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` | `test-code` |
| `tests/Package.Tests/Package.Tests.fsproj` | `test-code` |
| `tests/Package.Tests/SurfaceAreaTests.fs` | `test-code` |
| `tests/Package.Tests/Tests.fs` | `test-code` |
| `docs/adr/0012-monolith-retirement-closeout.md` | `documentation` |
| `docs/migration/v2-to-v3.md` | `documentation` |
| `docs/reports/_baselines/2026-06-02-v3-after.md` | `documentation` |

## Required Alignment Classes

- `docs/reports/dependencies.md` requires `docs-alignment`
- `docs/reports/dependencies.md` requires `active-feature-evidence`
- `scripts/template-drift.fsx` requires `template-drift-docs`
- `scripts/template-drift.fsx` requires `active-feature-evidence`
- `src/Lib/InternalsVisibleTo.fs` requires `source-contract`
- `src/Lib/InternalsVisibleTo.fs` requires `active-feature-evidence`
- `src/Lib/Lib.fsproj` requires `source-contract`
- `src/Lib/Lib.fsproj` requires `active-feature-evidence`
- `src/Lib/Library.fs` requires `source-contract`
- `src/Lib/Library.fs` requires `active-feature-evidence`
- `src/Lib/Library.fsi` requires `source-contract`
- `src/Lib/Library.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/DiagnosticsTests.fs` requires `test-evidence`
- `tests/Controls.Tests/DiagnosticsTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ArtifactPathTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ArtifactPathTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AsteroidsFeedbackSkillGuidanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ControlsBoundaryCompositionTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/DependencyGovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/DependencyGovernanceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PerPackageSurfaceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PerPackageSurfaceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `test-evidence`
- `tests/Governance.Tests/PublicRecordInvariantTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/RoutingTests.fs` requires `test-evidence`
- `tests/Governance.Tests/RoutingTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/RuntimeOrganizationTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `test-evidence`
- `tests/Governance.Tests/UpgradeSkiaSpecKitTests.fs` requires `active-feature-evidence`
- `tests/Package.Tests/Package.Tests.fsproj` requires `test-evidence`
- `tests/Package.Tests/Package.Tests.fsproj` requires `active-feature-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `test-evidence`
- `tests/Package.Tests/SurfaceAreaTests.fs` requires `active-feature-evidence`
- `tests/Package.Tests/Tests.fs` requires `test-evidence`
- `tests/Package.Tests/Tests.fs` requires `active-feature-evidence`
- `docs/adr/0012-monolith-retirement-closeout.md` requires `docs-alignment`
- `docs/adr/0012-monolith-retirement-closeout.md` requires `active-feature-evidence`
- `docs/migration/v2-to-v3.md` requires `docs-alignment`
- `docs/migration/v2-to-v3.md` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-06-02-v3-after.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-06-02-v3-after.md` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/053-v3-monolith-retirement`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
