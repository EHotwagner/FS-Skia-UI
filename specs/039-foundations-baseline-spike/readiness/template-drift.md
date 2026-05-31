# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `Directory.Packages.props` | `dependency-policy` |
| `docs/reports/dependencies.md` | `documentation` |
| `docs/adr/0001-governance-library-placement-and-distribution.md` | `documentation` |
| `docs/adr/0002-build-front-end-form.md` | `documentation` |
| `docs/adr/0003-generated-product-contract-versioning.md` | `documentation` |
| `docs/adr/0004-spec-kit-fork-stance.md` | `documentation` |
| `docs/adr/0005-configuration-representation.md` | `documentation` |
| `docs/reports/_baselines/2026-05-31-foundations.md` | `documentation` |
| `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md` | `documentation` |
| `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/audit-counts.txt` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/audit-counts.txt` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/audit-counts.txt` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` | `test-code` |
| `tests/Governance.Tests/fixtures/evidence-golden/README.md` | `test-code` |

## Required Alignment Classes

- `Directory.Packages.props` requires `dependency-docs`
- `Directory.Packages.props` requires `active-feature-evidence`
- `docs/reports/dependencies.md` requires `docs-alignment`
- `docs/reports/dependencies.md` requires `active-feature-evidence`
- `docs/adr/0001-governance-library-placement-and-distribution.md` requires `docs-alignment`
- `docs/adr/0001-governance-library-placement-and-distribution.md` requires `active-feature-evidence`
- `docs/adr/0002-build-front-end-form.md` requires `docs-alignment`
- `docs/adr/0002-build-front-end-form.md` requires `active-feature-evidence`
- `docs/adr/0003-generated-product-contract-versioning.md` requires `docs-alignment`
- `docs/adr/0003-generated-product-contract-versioning.md` requires `active-feature-evidence`
- `docs/adr/0004-spec-kit-fork-stance.md` requires `docs-alignment`
- `docs/adr/0004-spec-kit-fork-stance.md` requires `active-feature-evidence`
- `docs/adr/0005-configuration-representation.md` requires `docs-alignment`
- `docs/adr/0005-configuration-representation.md` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-05-31-foundations.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-05-31-foundations.md` requires `active-feature-evidence`
- `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md` requires `docs-alignment`
- `docs/reports/_baselines/2026-05-31-spike-d2-outcome.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/audit-counts.txt` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/audit-counts.txt` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/036-archive-readiness-api-docs/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/audit-counts.txt` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/audit-counts.txt` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/037-authoring-audit-robustness/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/audit-counts.txt` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/audit-counts.txt` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.json` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/038-authoring-guidance-consistency/task-graph.md` requires `active-feature-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/README.md` requires `test-evidence`
- `tests/Governance.Tests/fixtures/evidence-golden/README.md` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, sample-contract, source-contract, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/039-foundations-baseline-spike`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
