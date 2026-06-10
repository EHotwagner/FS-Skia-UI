# Skill-loading evidence (feature 094)

One row per `(task, declared-skill)` pair from `tasks.deps.yml`. Each declared skill id resolves to
exactly one readable `SKILL.md` (package skills under `src/*/skill/`, capability skills under
`.agents/skills/<id>/`). `LoadedAt` is strictly before `WorkStartedAt` for every row. Provenance:
`captured` = observed at the load action before code changes; `asserted` = hand-authored.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T005 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/fsi-transcript.md | none | asserted |
| T005 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/fsi-transcript.md | none | asserted |
| T006 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T006 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T007 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/sc007-validate-order.md | none | asserted |
| T007 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/sc007-validate-order.md | none | asserted |
| T007 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/sc007-validate-order.md | none | asserted |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:40:00Z | 2026-06-10T20:46:00Z | readiness/fsi-transcript.md | none | asserted |
| T011 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T011 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T012 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T012 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T013 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T014 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T015 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T015 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-10T20:46:00Z | 2026-06-10T20:52:00Z | readiness/us1-tab-traversal.md | none | asserted |
| T016 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T016 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T016 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T017 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T017 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T017 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T018 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-text-seam-preserved.md | none | asserted |
| T018 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-text-seam-preserved.md | none | asserted |
| T019 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T019 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T020 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T020 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T020 | fs-skia-keyboard-input | src/KeyboardInput/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T021 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T021 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-text-seam-preserved.md | none | asserted |
| T022 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T022 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:52:00Z | 2026-06-10T20:58:00Z | readiness/us2-focused-key-delivery.md | none | asserted |
| T023 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-stability.md | none | asserted |
| T023 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-stability.md | none | asserted |
| T024 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/sc007-validate-order.md | none | asserted |
| T024 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/sc007-validate-order.md | none | asserted |
| T025 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-stability.md | none | asserted |
| T025 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-stability.md | none | asserted |
| T026 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-indicator.md | none | asserted |
| T026 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/us3-focus-indicator.md | none | asserted |
| T027 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/responds-proof.md | none | asserted |
| T027 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-10T20:58:00Z | 2026-06-10T21:04:00Z | readiness/responds-proof.md | none | asserted |
| T028 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T21:04:00Z | 2026-06-10T21:10:00Z | readiness/sc006-determinism-property.md | none | asserted |
| T028 | fs-skia-testing | src/Testing/skill/SKILL.md | loaded | 2026-06-10T21:04:00Z | 2026-06-10T21:10:00Z | readiness/sc006-determinism-property.md | none | asserted |
| T029 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-10T21:04:00Z | 2026-06-10T21:10:00Z | readiness/surface-baselines.md | none | asserted |
| T029 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-10T21:04:00Z | 2026-06-10T21:10:00Z | readiness/surface-baselines.md | none | asserted |
| T030 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-10T21:10:00Z | 2026-06-10T21:16:00Z | readiness/generated-guidance-validation.md | none | asserted |
| T031 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-10T21:16:00Z | 2026-06-10T21:22:00Z | readiness/evidence-graph.md | none | asserted |
| T032 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-10T21:22:00Z | 2026-06-10T21:28:00Z | readiness/evidence-audit.md | none | asserted |
