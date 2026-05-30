# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `docs/generated-apps.md` | `documentation` |
| `docs/template-profile.md` | `documentation` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Package.Tests/PackageApiReferenceTests.fs` | `test-code` |
| `tests/Governance.Tests/ArchiveReadinessApiDocsTests.fs` | `test-code` |

## Required Alignment Classes

- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `docs/template-profile.md` requires `docs-alignment`
- `docs/template-profile.md` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Package.Tests/PackageApiReferenceTests.fs` requires `test-evidence`
- `tests/Package.Tests/PackageApiReferenceTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/ArchiveReadinessApiDocsTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ArchiveReadinessApiDocsTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/036-archive-readiness-api-docs`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
