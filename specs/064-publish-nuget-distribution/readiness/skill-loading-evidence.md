# Skill-loading evidence

Each skilled task loads its declared `skillist` (in order) before any code change for that
task begins, per the implementation-loading discipline. `loaded_at` precedes
`work_started_at` for every row. Resolved paths are the canonical `.agents/skills/**` homes
for governance/authoring skills. This log is read from the **feature** readiness dir
(`specs/064-publish-nuget-distribution/readiness/`, not repo-root) and is enforced only once
tasks flip to `[X]`. One row per (task, declared-skill) pair.

| task | skill id | resolved path | load result | loaded_at | work_started_at | evidence path | reviewer exception |
|---|---|---|---|---|---|---|---|
| T006 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T22:30:00Z | this file + build/Governance/Targets.fs | none |
| T007 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T22:35:00Z | this file + build/Governance/Routing.fs | none |
| T011 | fsharp-code-generation | .agents/skills/fsharp-code-generation/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T22:45:00Z | this file + build/Governance/GeneratedProduct.fs | none |
| T012 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-04T23:00:00Z | this file + build/Governance/GeneratedProduct.fs | none |
| T013 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-04T23:10:00Z | this file + readiness/fresh-consumer-restore.md | none |
| T014 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T23:20:00Z | this file + tests/Governance.Tests/Feature064PublishTests.fs | none |
| T015 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T23:25:00Z | this file + tests/Governance.Tests/Feature064PublishTests.fs | none |
| T018 | fsharp-shell-process | .agents/skills/fsharp-shell-process/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T23:40:00Z | this file + build/Governance/Engine/Interpret.fs | none |
| T019 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-04T23:50:00Z | this file + validation.contract.yml | none |
| T020 | fsharp-shell-process | .agents/skills/fsharp-shell-process/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T00:00:00Z | this file + readiness/publish-dry-run.md | none |
| T021 | fsharp-shell-process | .agents/skills/fsharp-shell-process/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T00:10:00Z | this file + readiness/publish-idempotency.md | none |
| T022 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T00:20:00Z | this file + tests/Governance.Tests/Feature064PublishTests.fs | none |
| T023 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T00:30:00Z | this file + template/base/Directory.Packages.props | none |
| T024 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T00:40:00Z | this file + template/base/build.fsx | none |
| T026 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T00:50:00Z | this file + readiness/single-edit-upgrade.md | none |
| T027 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T01:00:00Z | this file + tests/Governance.Tests/Feature064PublishTests.fs | none |
| T028 | fsharp-parsing | .agents/skills/fsharp-parsing/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T01:10:00Z | this file + build/Governance/PrePublish.fs | none |
| T029 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T01:20:00Z | this file + build/Governance/Engine/Update.fs | none |
| T030 | fsharp-io-globbing | .agents/skills/fsharp-io-globbing/SKILL.md | loaded | 2026-06-05T01:25:00Z | 2026-06-05T01:30:00Z | this file + src/Scene/README.md | none |
| T031 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T01:40:00Z | this file + .template.package/README.md | none |
| T032 | fsharp-build-orchestration | .agents/skills/fsharp-build-orchestration/SKILL.md | loaded | 2026-06-04T22:05:00Z | 2026-06-05T01:50:00Z | this file + readiness/prepublish-check.md | none |
| T036 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T02:00:00Z | this file + readiness/validation-contract.md | none |
| T037 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T02:10:00Z | this file + .template.package/FS.Skia.UI.Template.fsproj | none |
| T038 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T02:20:00Z | this file + readiness/target-metadata.md | none |
| T039 | speckit-evidence-graph | .agents/skills/speckit-evidence-graph/SKILL.md | loaded | 2026-06-05T02:25:00Z | 2026-06-05T02:30:00Z | this file + readiness/evidence-graph.md | none |
| T040 | speckit-evidence-audit | .agents/skills/speckit-evidence-audit/SKILL.md | loaded | 2026-06-05T02:35:00Z | 2026-06-05T02:40:00Z | this file + readiness/evidence-audit.md | none |
| T041 | fs-skia-template-update | .agents/skills/fs-skia-template-update/SKILL.md | loaded | 2026-06-04T22:50:00Z | 2026-06-05T02:50:00Z | this file + readiness/production-publish.md | none |
