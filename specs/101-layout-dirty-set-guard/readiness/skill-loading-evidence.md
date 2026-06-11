# Skill-loading evidence — feature 101 (R7, Layout Dirty-Set Anti-Drift Guard)

One row per `(task, declared-skill)` pair. Every declared skill was resolved to
exactly one readable `SKILL.md` and loaded **before** the task's code/evidence
work began (`LoadedAt` strictly before `WorkStartedAt`). The skill-loading
contract is enforced when a task flips to `[X]`.

Required tokens: TaskId, DeclaredSkillId, ResolvedSkillPath, LoadResult, LoadedAt, WorkStartedAt, EvidencePath, Exception, Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/ (scaffold) | none | captured |
| T005 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/runtime-limitations.md | none | captured |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/surface-baseline.pre.txt | none | captured |
| T007 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/drift-guard.md | none | captured |
| T007 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/drift-guard.md | none | captured |
| T008 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/category-honoring.md | none | captured |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/category-honoring.md | none | captured |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/drift-guard.md | none | captured |
| T009 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/drift-guard.md | none | captured |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/drift-guard.md | none | captured |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/single-source.md | none | captured |
| T012 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:06:00Z | readiness/single-source.md | none | captured |
| T013 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/r2-preservation.md | none | captured |
| T013 | fs-skia-layout | src/Layout/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/r2-preservation.md | none | captured |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/r2-preservation.md | none | captured |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/r2-preservation.md | none | captured |
| T015 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/surface-baseline.md | none | captured |
| T016 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:20:00Z | readiness/validation-log.md | none | captured |
| T017 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:30:00Z | readiness/evidence-graph.md | none | captured |
| T018 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T16:02:44Z | 2026-06-11T16:30:00Z | readiness/evidence-audit.md | none | captured |
