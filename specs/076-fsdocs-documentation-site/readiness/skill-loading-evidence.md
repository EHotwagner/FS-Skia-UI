# Skill-loading evidence (feature 076)

One row per (task, declared-skill) pair. `LoadedAt` is strictly before
`WorkStartedAt`. Resolved paths are the `.agents/skills/<id>/SKILL.md` homes (the
canonical source; the `.claude` tree is generated from it). Skills were loaded
(read) before the work for each task began; the deep-dive authoring tasks loaded
their skills via the authoring sub-agents that performed the work.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception |
|--------|-----------------|-------------------|------------|----------|---------------|-------------|-----------|
| T002 | fsdocs-setup | .agents/skills/fsdocs-setup/SKILL.md | loaded | 2026-06-07T10:30:00Z | 2026-06-07T10:31:00Z | readiness/readiness-notes.md | none |
| T003 | fsdocs-setup | .agents/skills/fsdocs-setup/SKILL.md | loaded | 2026-06-07T10:32:00Z | 2026-06-07T10:33:00Z | readiness/readiness-notes.md | none |
| T007 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T10:34:00Z | 2026-06-07T10:35:00Z | .github/workflows/docs.yml | none |
| T008 | fsdocs-setup | .agents/skills/fsdocs-setup/SKILL.md | loaded | 2026-06-07T10:36:00Z | 2026-06-07T10:37:00Z | docs/index.md | none |
| T009 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T10:38:00Z | 2026-06-07T10:39:00Z | readiness/logs/fsdocs-build.txt | none |
| T010 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T10:40:00Z | 2026-06-07T10:41:00Z | readiness/api-coverage.md | none |
| T011 | fsdocs-api-doc | .agents/skills/fsdocs-api-doc/SKILL.md | loaded | 2026-06-07T10:42:00Z | 2026-06-07T10:43:00Z | src/Controls/DesignTokens.fsi | none |
| T012 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T10:44:00Z | 2026-06-07T10:45:00Z | readiness/api-coverage.md | none |
| T013 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:46:00Z | 2026-06-07T10:47:00Z | docs/architecture/host-skiaviewer.md | none |
| T014 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:48:00Z | 2026-06-07T10:49:00Z | docs/architecture/layout.md | none |
| T015 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:50:00Z | 2026-06-07T10:51:00Z | docs/architecture/elmish-mvu.md | none |
| T016 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:52:00Z | 2026-06-07T10:53:00Z | docs/architecture/governance.md | none |
| T017 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T10:54:00Z | 2026-06-07T10:55:00Z | readiness/logs/fsdocs-build.txt | none |
| T018 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:56:00Z | 2026-06-07T10:57:00Z | docs/governance/index.md | none |
| T019 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T10:58:00Z | 2026-06-07T10:59:00Z | docs/governance/evidence-and-audit.md | none |
| T020 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T11:00:00Z | 2026-06-07T11:01:00Z | docs/governance/speckit-placement.md | none |
| T021 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-07T11:02:00Z | 2026-06-07T11:03:00Z | docs/controls-design/typed-front-door.md | none |
| T021 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T11:04:00Z | 2026-06-07T11:05:00Z | docs/controls-design/typed-front-door.md | none |
| T022 | fs-skia-design-tokens | .agents/skills/fs-skia-design-tokens/SKILL.md | loaded | 2026-06-07T11:06:00Z | 2026-06-07T11:07:00Z | docs/controls-design/design-tokens-penpot.md | none |
| T022 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T11:08:00Z | 2026-06-07T11:09:00Z | docs/controls-design/design-tokens-penpot.md | none |
| T023 | fsdocs-technical | .agents/skills/fsdocs-technical/SKILL.md | loaded | 2026-06-07T11:10:00Z | 2026-06-07T11:11:00Z | docs/speckit/process.md | none |
| T024 | fsdocs-examples | .agents/skills/fsdocs-examples/SKILL.md | loaded | 2026-06-07T11:12:00Z | 2026-06-07T11:13:00Z | docs/examples/typed-control-mvu.fsx | none |
| T025 | fsdocs-examples | .agents/skills/fsdocs-examples/SKILL.md | loaded | 2026-06-07T11:14:00Z | 2026-06-07T11:15:00Z | docs/examples/design-token-flow.fsx | none |
| T027 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T11:16:00Z | 2026-06-07T11:17:00Z | readiness/logs/fsdocs-build.txt | none |
| T029 | fsdocs-build | .agents/skills/fsdocs-build/SKILL.md | loaded | 2026-06-07T11:18:00Z | 2026-06-07T11:19:00Z | readiness/logs/fsdocs-build.txt | none |
| T032 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-07T11:20:00Z | 2026-06-07T11:21:00Z | readiness/logs/evidence-graph.txt | none |
| T033 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-07T11:22:00Z | 2026-06-07T11:23:00Z | readiness/logs/evidence-audit.txt | none |
