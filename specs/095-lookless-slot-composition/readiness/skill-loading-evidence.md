# Skill-loading evidence (feature 095)

One row per `(task, declared-skill)` pair from `tasks.deps.yml`. Each declared skill id resolves to
exactly one readable `SKILL.md` (package skills under `src/*/skill/`, template-fragment skills under
`template/fragments/*/skill/`, capability/workflow skills under `.agents/skills/<id>/`). `LoadedAt`
is strictly before `WorkStartedAt` for every row. Provenance: `captured` = observed at the load
action before code changes; `asserted` = hand-authored.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T002 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:30:00Z | 2026-06-10T22:36:00Z | readiness/runtime-limitations.md | none | asserted |
| T004 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T22:30:00Z | 2026-06-10T22:36:00Z | readiness/surface-baselines.md | none | asserted |
| T005 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:30:00Z | 2026-06-10T22:36:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T006 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:30:00Z | 2026-06-10T22:36:00Z | readiness/fsi-transcript.md | none | asserted |
| T008 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:30:00Z | 2026-06-10T22:36:00Z | readiness/runtime-limitations.md | none | asserted |
| T009 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T009 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T010 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T011 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/sc005-lowering-property.md | none | asserted |
| T013 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/sc006-typed-closed-and-nongoals.md | none | asserted |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:36:00Z | 2026-06-10T22:42:00Z | readiness/us1-slot-fill-regions.md | none | asserted |
| T015 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:42:00Z | 2026-06-10T22:48:00Z | readiness/us2-unfilled-byte-identical.md | none | asserted |
| T015 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:42:00Z | 2026-06-10T22:48:00Z | readiness/us2-unfilled-byte-identical.md | none | asserted |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:42:00Z | 2026-06-10T22:48:00Z | readiness/us2-unfilled-byte-identical.md | none | asserted |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:42:00Z | 2026-06-10T22:48:00Z | readiness/us2-unfilled-byte-identical.md | none | asserted |
| T018 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/us3-compose-e1-e4.md | none | asserted |
| T018 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/us3-compose-e1-e4.md | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/us3-compose-e1-e4.md | none | asserted |
| T019 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/sc004-retained-identity.md | none | asserted |
| T019 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/sc004-retained-identity.md | none | asserted |
| T020 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T22:48:00Z | 2026-06-10T22:54:00Z | readiness/us3-compose-e1-e4.md | none | asserted |
| T021 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T22:54:00Z | 2026-06-10T23:00:00Z | readiness/us4-skill-e1-e5.md | none | asserted |
| T022 | fs-skia-generated-controls-guidance | template/fragments/controls/skill/SKILL.md | loaded | 2026-06-10T22:54:00Z | 2026-06-10T23:00:00Z | readiness/us4-skill-e1-e5.md | none | asserted |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T22:54:00Z | 2026-06-10T23:00:00Z | readiness/us4-skill-e1-e5.md | none | asserted |
| T024 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T22:54:00Z | 2026-06-10T23:00:00Z | readiness/us4-skill-e1-e5.md | none | asserted |
| T025 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T23:00:00Z | 2026-06-10T23:06:00Z | readiness/surface-baselines.md | none | asserted |
| T026 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T23:00:00Z | 2026-06-10T23:06:00Z | readiness/governance-risk-levels.md | none | asserted |
| T027 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T23:06:00Z | 2026-06-10T23:12:00Z | readiness/evidence-graph.md | none | asserted |
| T028 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T23:12:00Z | 2026-06-10T23:18:00Z | readiness/evidence-audit.md | none | asserted |
