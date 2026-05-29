# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `build.fsx` | `command-surface` |
| `docs/build.md` | `documentation` |
| `docs/controls.md` | `documentation` |
| `docs/evidence.md` | `documentation` |
| `docs/generated-apps.md` | `documentation` |
| `docs/testing.md` | `documentation` |
| `fake.sh` | `command-surface` |
| `src/Controls/Attributes.fs` | `source-code` |
| `src/Controls/Catalog.fs` | `source-code` |
| `src/Controls/Charts.fs` | `source-code` |
| `src/Controls/Control.fs` | `source-code` |
| `src/Controls/DataGrid.fs` | `source-code` |
| `src/Controls/Types.fs` | `source-code` |
| `src/Controls/Types.fsi` | `source-code` |
| `src/Lib/AgentValidation.fs` | `source-code` |
| `src/Lib/AgentValidation.fsi` | `source-code` |
| `tests/Controls.Tests/TypedControlContractTests.fs` | `test-code` |
| `tests/Governance.Tests/AgentValidationFrameworkTests.fs` | `test-code` |
| `tests/Governance.Tests/CommandContractTests.fs` | `test-code` |
| `tests/Governance.Tests/GeneratedProjectValidationTests.fs` | `test-code` |

## Required Alignment Classes

- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `docs/build.md` requires `docs-alignment`
- `docs/build.md` requires `active-feature-evidence`
- `docs/controls.md` requires `docs-alignment`
- `docs/controls.md` requires `active-feature-evidence`
- `docs/evidence.md` requires `docs-alignment`
- `docs/evidence.md` requires `active-feature-evidence`
- `docs/generated-apps.md` requires `docs-alignment`
- `docs/generated-apps.md` requires `active-feature-evidence`
- `docs/testing.md` requires `docs-alignment`
- `docs/testing.md` requires `active-feature-evidence`
- `fake.sh` requires `command-docs`
- `fake.sh` requires `active-feature-evidence`
- `src/Controls/Attributes.fs` requires `source-contract`
- `src/Controls/Attributes.fs` requires `active-feature-evidence`
- `src/Controls/Catalog.fs` requires `source-contract`
- `src/Controls/Catalog.fs` requires `active-feature-evidence`
- `src/Controls/Charts.fs` requires `source-contract`
- `src/Controls/Charts.fs` requires `active-feature-evidence`
- `src/Controls/Control.fs` requires `source-contract`
- `src/Controls/Control.fs` requires `active-feature-evidence`
- `src/Controls/DataGrid.fs` requires `source-contract`
- `src/Controls/DataGrid.fs` requires `active-feature-evidence`
- `src/Controls/Types.fs` requires `source-contract`
- `src/Controls/Types.fs` requires `active-feature-evidence`
- `src/Controls/Types.fsi` requires `source-contract`
- `src/Controls/Types.fsi` requires `active-feature-evidence`
- `src/Lib/AgentValidation.fs` requires `source-contract`
- `src/Lib/AgentValidation.fs` requires `active-feature-evidence`
- `src/Lib/AgentValidation.fsi` requires `source-contract`
- `src/Lib/AgentValidation.fsi` requires `active-feature-evidence`
- `tests/Controls.Tests/TypedControlContractTests.fs` requires `test-evidence`
- `tests/Controls.Tests/TypedControlContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `test-evidence`
- `tests/Governance.Tests/AgentValidationFrameworkTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/CommandContractTests.fs` requires `active-feature-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `test-evidence`
- `tests/Governance.Tests/GeneratedProjectValidationTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/028-agent-validation-framework`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Diagnostics

- No drift blockers.
