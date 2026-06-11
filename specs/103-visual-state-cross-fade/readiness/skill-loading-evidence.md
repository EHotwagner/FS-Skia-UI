# Skill-loading evidence — feature 103 (True Visual-State Cross-Fade, R6)

One row per (task, declared-skill). Capability skills were resolved and read at the load action
**before** any code change for the task (loaded_at strictly < work_started_at). The two speckit
gate skills (T019/T020) were loaded at the gate step. Provenance: `captured` = observed during this
run and recorded at the load action; `asserted` = hand-authored.

Columns (in order): TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T18:58:00Z | 2026-06-11T19:02:00Z | specs/103-visual-state-cross-fade/readiness/visual-evidence-honesty.md | none | captured |
| T004 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:05:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:05:00Z | specs/103-visual-state-cross-fade/research.md | none | captured |
| T005 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-11T18:58:30Z | 2026-06-11T19:05:00Z | specs/103-visual-state-cross-fade/research.md | none | captured |
| T006 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:10:00Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T006 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T18:58:40Z | 2026-06-11T19:10:00Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T007 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:06:00Z | src/Controls/RetainedRender.fs | none | captured |
| T008 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-11T18:58:30Z | 2026-06-11T19:07:00Z | src/Controls/RetainedRender.fs | none | captured |
| T008 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:07:00Z | src/Controls/RetainedRender.fs | none | captured |
| T009 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:08:00Z | src/Controls/RetainedRender.fs | none | captured |
| T010 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T18:58:00Z | 2026-06-11T19:12:00Z | specs/103-visual-state-cross-fade/readiness/mid-flight-interpolation.md | none | captured |
| T011 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:12:00Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T011 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T18:58:40Z | 2026-06-11T19:12:00Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T18:58:40Z | 2026-06-11T19:13:00Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T013 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T18:58:00Z | 2026-06-11T19:14:00Z | specs/103-visual-state-cross-fade/readiness/at-rest-byte-identity.md | none | captured |
| T014 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:13:30Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T014 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-11T18:58:40Z | 2026-06-11T19:13:30Z | tests/Controls.Tests/Feature103CrossFadeTests.fs | none | captured |
| T015 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T18:57:00Z | 2026-06-11T19:15:00Z | src/Controls/RetainedRender.fsi | none | captured |
| T019 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T19:18:00Z | 2026-06-11T19:19:00Z | specs/103-visual-state-cross-fade/readiness/evidence-graph.md | none | captured |
| T020 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T19:20:00Z | 2026-06-11T19:21:00Z | specs/103-visual-state-cross-fade/readiness/evidence-audit.md | none | captured |
