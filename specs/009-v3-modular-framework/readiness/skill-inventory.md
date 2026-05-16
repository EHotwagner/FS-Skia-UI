# Skill Inventory

Date: 2026-05-16

## Current Repository Skills

| Skill class | Source | Notes |
|-------------|--------|-------|
| Spec Kit workflow skills | `.agents/skills/speckit-*` | Repository-level workflow automation for specs, plans, tasks, evidence, and git helpers. |
| Generated AGENTS guidance | `.template.config/generated/AGENTS.md` | Template-level guidance, not capability-owned skills. |

## V3 Candidate Capability Skills

| Capability | Planned source skill |
|------------|----------------------|
| Scene | `src/Scene/skill/SKILL.md` |
| SkiaViewer | `src/SkiaViewer/skill/SKILL.md` |
| Elmish | `src/Elmish/skill/SKILL.md` |
| KeyboardInput | `src/KeyboardInput/skill/SKILL.md` |
| Layout | `src/Layout/skill/SKILL.md` |
| Charts | `src/Charts/skill/SKILL.md` |
| Testing | `src/Testing/skill/SKILL.md` |
| Samples | `template/fragments/samples/skill/SKILL.md` |

## Required Generated Product Rule

Generated products receive the project skill plus only skills for selected
capabilities and prerequisite capabilities. Framework-maintenance-only skills
remain in the framework repository.
