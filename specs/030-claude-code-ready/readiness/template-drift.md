# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.template.config/template.json` | `template-manifest` |
| `build.fsx` | `command-surface` |
| `scripts/template-drift.fsx` | `governance-script` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/Governance.Tests/ProcessReliabilityContractTests.fs` | `test-code` |
| `.template.config/generated/.claude/settings.json` | `template-manifest` |
| `.template.config/generated/CLAUDE.md` | `template-manifest` |
| `tests/Governance.Tests/ClaudeCodeReadyTests.fs` | `test-code` |

## Required Alignment Classes

- `.template.config/template.json` requires `template-profile`
- `.template.config/template.json` requires `active-feature-evidence`
- `build.fsx` requires `command-docs`
- `build.fsx` requires `active-feature-evidence`
- `scripts/template-drift.fsx` requires `template-drift-docs`
- `scripts/template-drift.fsx` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ProcessReliabilityContractTests.fs` requires `active-feature-evidence`
- `.template.config/generated/.claude/settings.json` requires `template-profile`
- `.template.config/generated/.claude/settings.json` requires `active-feature-evidence`
- `.template.config/generated/CLAUDE.md` requires `template-profile`
- `.template.config/generated/CLAUDE.md` requires `active-feature-evidence`
- `tests/Governance.Tests/ClaudeCodeReadyTests.fs` requires `test-evidence`
- `tests/Governance.Tests/ClaudeCodeReadyTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, command-docs, dependency-docs, docs-alignment, sample-contract, source-contract, template-drift-docs, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/030-claude-code-ready`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
