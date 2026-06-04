# Template Drift Report

PASS

## Changed Template-Owned Paths

| Path | Path Class |
|------|------------|
| `.template.config/template.json` | `template-manifest` |
| `src/Elmish/skill/SKILL.md` | `source-code` |
| `src/Scene/skill/SKILL.md` | `source-code` |
| `src/SkiaViewer/skill/SKILL.md` | `source-code` |
| `src/SkillSupport/SkillSupport.fsproj` | `source-code` |
| `tests/Governance.Tests/Governance.Tests.fsproj` | `test-code` |
| `tests/SkillSupport.Tests/Tests.fs` | `test-code` |
| `src/SkillSupport/Hud.fs` | `source-code` |
| `src/SkillSupport/Hud.fsi` | `source-code` |
| `src/SkillSupport/Random.fs` | `source-code` |
| `src/SkillSupport/Random.fsi` | `source-code` |
| `tests/Governance.Tests/Feature062GovernanceTests.fs` | `test-code` |

## Required Alignment Classes

- `.template.config/template.json` requires `template-profile`
- `.template.config/template.json` requires `active-feature-evidence`
- `src/Elmish/skill/SKILL.md` requires `source-contract`
- `src/Elmish/skill/SKILL.md` requires `active-feature-evidence`
- `src/Scene/skill/SKILL.md` requires `source-contract`
- `src/Scene/skill/SKILL.md` requires `active-feature-evidence`
- `src/SkiaViewer/skill/SKILL.md` requires `source-contract`
- `src/SkiaViewer/skill/SKILL.md` requires `active-feature-evidence`
- `src/SkillSupport/SkillSupport.fsproj` requires `source-contract`
- `src/SkillSupport/SkillSupport.fsproj` requires `active-feature-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `test-evidence`
- `tests/Governance.Tests/Governance.Tests.fsproj` requires `active-feature-evidence`
- `tests/SkillSupport.Tests/Tests.fs` requires `test-evidence`
- `tests/SkillSupport.Tests/Tests.fs` requires `active-feature-evidence`
- `src/SkillSupport/Hud.fs` requires `source-contract`
- `src/SkillSupport/Hud.fs` requires `active-feature-evidence`
- `src/SkillSupport/Hud.fsi` requires `source-contract`
- `src/SkillSupport/Hud.fsi` requires `active-feature-evidence`
- `src/SkillSupport/Random.fs` requires `source-contract`
- `src/SkillSupport/Random.fs` requires `active-feature-evidence`
- `src/SkillSupport/Random.fsi` requires `source-contract`
- `src/SkillSupport/Random.fsi` requires `active-feature-evidence`
- `tests/Governance.Tests/Feature062GovernanceTests.fs` requires `test-evidence`
- `tests/Governance.Tests/Feature062GovernanceTests.fs` requires `active-feature-evidence`

## Alignment

- Changed alignment classes: `active-feature-evidence, dependency-docs, docs-alignment, source-contract, template-profile, test-evidence`
- Deferral file: `/home/developer/projects/FS-Skia-UI/readiness/template-deferrals.yml`
- Active feature evidence: `specs/062-space-invaders-consumer-friction-followups`

## Controls Boundary Guidance

- PASS: generated guidance names Controls ownership, DataGrid, adapter wiring, and Charts migration without stale generated terms.

## Agent Artifact Sync

- PASS: repository Codex and Claude skills, template Claude skill mappings, and project-shareable settings are synchronized.

## Diagnostics

- No drift blockers.
