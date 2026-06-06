# Skill-loading evidence — Catalog Breadth Expansion (072)

Each skilled task loads its declared `skillist` (in order) before any code change
for that task begins. `loaded_at` precedes `work_started_at` for every row.
Resolved paths are the canonical registry homes: `.agents/skills/<id>/SKILL.md`
for governance/capability skills and `src/Controls/skill/SKILL.md` for the
`fs-skia-ui-widgets` capability skill (the registry scans `.agents/skills`,
`src/*/skill`, and `template/fragments/*/skill`).

## Selected skills

- `fs-skia-typed-controls` — `.agents/skills/fs-skia-typed-controls/SKILL.md`
- `fsharp-code-generation` — `.agents/skills/fsharp-code-generation/SKILL.md`
- `fs-skia-ui-widgets` — `src/Controls/skill/SKILL.md`
- `fs-skia-evidence-mode` — `.agents/skills/fs-skia-evidence-mode/SKILL.md`
- `fsharp-build-orchestration` — `.agents/skills/fsharp-build-orchestration/SKILL.md`
- `speckit-evidence-graph` — `.agents/skills/speckit-evidence-graph/SKILL.md`
- `speckit-evidence-audit` — `.agents/skills/speckit-evidence-audit/SKILL.md`

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T004 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:05:00Z | this file + readiness/typed-controls-front-door.md | none |
| T005 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:10:00Z | this file + src/Controls/Widgets/Buttons.fsi + src/Controls/Widgets/Pickers.fsi | none |
| T006 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T18:02:00Z | 2026-06-06T18:14:00Z | this file + build/Governance/CatalogGen.fs | none |
| T007 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T18:02:00Z | 2026-06-06T18:18:00Z | this file + src/Controls/catalog.yml + src/Controls/Catalog.fs | none |
| T008 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T18:03:00Z | 2026-06-06T18:20:00Z | this file + src/Controls/Controls.fsproj | none |
| T010 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:24:00Z | this file + readiness/package-surface-expectations.md | none |
| T011 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T18:04:00Z | 2026-06-06T18:26:00Z | this file + readiness/runtime-limitations.md + readiness/governance-risk-levels.md + readiness/aggregate-hang-diagnostics.md | none |
| T012 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:30:00Z | this file + tests/Controls.Tests/TypedExpansionTests.fs | none |
| T013 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:32:00Z | this file + tests/Controls.Tests/InteractionTests.fs | none |
| T014 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T18:04:00Z | 2026-06-06T18:34:00Z | this file + tests/Controls.Tests/RenderingTests.fs + tests/Controls.Tests/AccessibilityTests.fs | none |
| T015 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T18:40:00Z | this file + src/Controls/Widgets/Pickers.fs | none |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T18:04:00Z | 2026-06-06T18:44:00Z | this file + readiness/controls-rendering.md | none |
| T017 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T18:03:00Z | 2026-06-06T18:48:00Z | this file + samples/ControlsGallery/Program.fs | none |
| T019 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T18:02:00Z | 2026-06-06T18:52:00Z | this file + tests/Controls.Tests/CatalogTests.fs | none |
| T020 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T18:02:00Z | 2026-06-06T18:56:00Z | this file + src/Controls/catalog.yml + src/Controls/Catalog.fs | none |
| T021 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-06T18:02:00Z | 2026-06-06T19:00:00Z | this file + specs/066-typed-catalog-generation/readiness/parity-fixtures/ | none |
| T022 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T18:06:00Z | 2026-06-06T19:04:00Z | this file + readiness/control-catalog-generation.md | none |
| T024 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:08:00Z | this file + tests/Controls.Tests/TypedExpansionTests.fs | none |
| T025 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:10:00Z | this file + tests/Controls.Tests/InteractionTests.fs | none |
| T026 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T18:04:00Z | 2026-06-06T19:12:00Z | this file + tests/Controls.Tests/RenderingTests.fs + tests/Controls.Tests/AccessibilityTests.fs | none |
| T027 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:18:00Z | this file + src/Controls/Widgets/Buttons.fs | none |
| T028 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:22:00Z | this file + src/Controls/Widgets/Pickers.fs | none |
| T029 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-06T18:04:00Z | 2026-06-06T19:26:00Z | this file + readiness/controls-rendering.md + readiness/typed-lowering-parity.md | none |
| T030 | fs-skia-ui-widgets | src/Controls/skill/SKILL.md | loaded | 2026-06-06T18:03:00Z | 2026-06-06T19:30:00Z | this file + samples/ControlsGallery/Program.fs | none |
| T032 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:36:00Z | this file + readiness/per-package-surface-diff.md | none |
| T033 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-06T18:06:00Z | 2026-06-06T19:40:00Z | this file + readiness/focused-gates.md | none |
| T034 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-06T18:00:00Z | 2026-06-06T19:44:00Z | this file + readiness/focused-gates.md | none |
| T036 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-06T19:46:00Z | 2026-06-06T19:48:00Z | this file + readiness/evidence-graph.md | none |
| T037 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-06T19:50:00Z | 2026-06-06T19:52:00Z | this file + readiness/evidence-audit.md | none |
