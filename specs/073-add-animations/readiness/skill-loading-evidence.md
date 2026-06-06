# Skill-loading evidence — Add Animations (073)

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins; `LoadedAt` strictly precedes `WorkStartedAt` for every row.
Resolved paths are the canonical registry homes: `.agents/skills/<id>/SKILL.md` for
governance/evidence skills, and `src/Scene/skill/SKILL.md` / `src/Elmish/skill/SKILL.md`
for the `fs-skia-scene` / `fs-skia-elmish` capability skills (the registry scans
`.agents/skills`, `src/*/skill`, and `template/fragments/*/skill`).

## Selected skills

- `fs-skia-scene` — `src/Scene/skill/SKILL.md`
- `fs-skia-elmish` — `src/Elmish/skill/SKILL.md`
- `fs-skia-evidence-mode` — `.agents/skills/fs-skia-evidence-mode/SKILL.md`
- `fsharp-build-orchestration` — `.agents/skills/fsharp-build-orchestration/SKILL.md`
- `speckit-evidence-graph` — `.agents/skills/speckit-evidence-graph/SKILL.md`
- `speckit-evidence-audit` — `.agents/skills/speckit-evidence-audit/SKILL.md`

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|---|---|---|---|---|---|---|---|
| T004 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T21:55:00Z | this file + readiness/animation-front-door.md | none |
| T005 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:00:00Z | this file + src/Scene/Animation.fsi | none |
| T006 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T21:51:00Z | 2026-06-06T22:05:00Z | this file + src/Elmish/AnimationTick.fsi | none |
| T007 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:08:00Z | this file + src/Scene/Scene.fsproj + src/Scene/Animation.fs | none |
| T008 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T21:51:00Z | 2026-06-06T22:10:00Z | this file + src/Elmish/Elmish.fsproj | none |
| T010 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:14:00Z | this file + readiness/package-surface-expectations.md | none |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T22:16:00Z | this file + readiness/runtime-limitations.md + readiness/governance-risk-levels.md + readiness/aggregate-hang-diagnostics.md | none |
| T012 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:20:00Z | this file + tests/Scene.Tests/AnimationTests.fs | none |
| T013 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:24:00Z | this file + tests/Scene.Tests/AnimationTests.fs | none |
| T014 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:28:00Z | this file + tests/Parity.Tests/AnimationOutputTests.fs | none |
| T015 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:32:00Z | this file + src/Scene/Animation.fs | none |
| T016 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:38:00Z | this file + src/Scene/Animation.fs | none |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T22:44:00Z | this file + readiness/animation-front-door.md + readiness/fsi/animation-session.txt | none |
| T019 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:48:00Z | this file + tests/Scene.Tests/AnimationTests.fs | none |
| T020 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T21:51:00Z | 2026-06-06T22:52:00Z | this file + tests/Elmish.Tests/AnimationTickTests.fs | none |
| T021 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T22:56:00Z | this file + src/Scene/Animation.fs | none |
| T022 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T21:51:00Z | 2026-06-06T23:00:00Z | this file + src/Elmish/AnimationTick.fs | none |
| T023 | fs-skia-elmish | src/Elmish/skill/SKILL.md | loaded | 2026-06-06T21:51:00Z | 2026-06-06T23:04:00Z | this file + readiness/redraw-gating.md | none |
| T025 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T23:08:00Z | this file + tests/Parity.Tests/AnimationOutputTests.fs | none |
| T026 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T23:14:00Z | this file + tests/Parity.Tests/fixtures/v3-host-golden/scene-output/animation-entrance-mid.txt | none |
| T027 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T23:18:00Z | this file + readiness/deterministic-sampling.md | none |
| T029 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T23:22:00Z | this file + tests/Parity.Tests/AnimationOutputTests.fs | none |
| T030 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T21:52:00Z | 2026-06-06T23:26:00Z | this file + readiness/settled-static-parity.md | none |
| T032 | fs-skia-scene | src/Scene/skill/SKILL.md | loaded | 2026-06-06T21:50:00Z | 2026-06-06T23:32:00Z | this file + readiness/per-package-surface-diff.md | none |
| T033 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T21:53:00Z | 2026-06-06T23:36:00Z | this file + readiness/package-surface-expectations.md | none |
| T034 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T21:53:00Z | 2026-06-06T23:40:00Z | this file + readiness/focused-gates.md | none |
| T036 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T23:44:00Z | 2026-06-06T23:46:00Z | this file + readiness/evidence-graph.md | none |
| T037 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T23:50:00Z | 2026-06-06T23:52:00Z | this file + readiness/evidence-audit.md | none |
