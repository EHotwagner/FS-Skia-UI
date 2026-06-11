# Skill-loading evidence — feature 105 (housekeeping code-quality)

One row per (task, declared-skill). `LoadedAt` is strictly before `WorkStartedAt`.
`ResolvedSkillPath` is the `.agents/skills/<id>/SKILL.md` home. `Provenance = captured`
means the load was observed during the run and recorded at the load action before any
code change for the task began. Rows are appended as each phase's skills are loaded and
its tasks completed — not pre-filled.

| TaskId | DeclaredSkillId | ResolvedSkillPath | LoadResult | LoadedAt | WorkStartedAt | EvidencePath | Exception | Provenance |
|--------|-----------------|-------------------|------------|----------|---------------|--------------|-----------|------------|
| T005 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:43:00Z | specs/105-housekeeping-code-quality/readiness/parity-baseline.md | none | captured |
| T006 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:43:30Z | specs/105-housekeeping-code-quality/readiness/parity-baseline.md | none | captured |
| T007 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:44:00Z | tests/Controls.Tests/Feature105ParityTests.fs | none | captured |
| T008 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:46:00Z | specs/105-housekeeping-code-quality/readiness/dedup-grep.md | none | captured |
| T009 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:48:00Z | specs/105-housekeeping-code-quality/readiness/dedup-grep.md | none | captured |
| T010 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:51:00Z | specs/105-housekeeping-code-quality/readiness/dedup-grep.md | none | captured |
| T011 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T21:52:00Z | specs/105-housekeeping-code-quality/readiness/dedup-grep.md | none | captured |
| T012 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T21:42:30Z | 2026-06-11T21:55:00Z | specs/105-housekeeping-code-quality/readiness/qualifier-cleanup.md | none | captured |
| T013 | fs-skia-reconciliation | .agents/skills/fs-skia-reconciliation/SKILL.md | loaded | 2026-06-11T21:42:30Z | 2026-06-11T21:57:00Z | specs/105-housekeeping-code-quality/readiness/qualifier-cleanup.md | none | captured |
| T014 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T23:58:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T015 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-12T00:05:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T016 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T21:58:30Z | 2026-06-12T00:00:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T017 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T21:59:30Z | 2026-06-12T00:02:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T017 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T21:58:30Z | 2026-06-12T00:02:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T018 | fs-skia-evidence-mode | .agents/skills/fs-skia-evidence-mode/SKILL.md | loaded | 2026-06-11T21:58:30Z | 2026-06-12T00:08:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T018 | fs-skia-viewer-host | .agents/skills/fs-skia-viewer-host/SKILL.md | loaded | 2026-06-11T21:59:30Z | 2026-06-12T00:08:00Z | specs/105-housekeeping-code-quality/readiness/internal-du-evidence.md | none | captured |
| T021 | fs-skia-typed-controls | .agents/skills/fs-skia-typed-controls/SKILL.md | loaded | 2026-06-11T21:42:10Z | 2026-06-11T22:08:00Z | specs/105-housekeeping-code-quality/readiness/focused-gates.md | none | captured |
| T022 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-11T22:30:50Z | 2026-06-11T22:31:30Z | specs/105-housekeeping-code-quality/readiness/evidence-graph.md | none | captured |
| T023 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-11T22:30:50Z | 2026-06-11T22:33:00Z | specs/105-housekeeping-code-quality/readiness/evidence-audit.md | none | captured |
