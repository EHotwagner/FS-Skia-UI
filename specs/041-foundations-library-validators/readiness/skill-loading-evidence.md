# Skill Loading Evidence

Capability skills declared in `tasks.deps.yml` were resolved from `.agents/skills/*/SKILL.md`
and consulted as implementation aids before the corresponding code changes. The cookbooks'
recorded verdicts match the implemented approach exactly: `fsharp-parsing` → YamlDotNet
17.1.0 deserialized into immutable F# records behind the typed model (Capabilities.fs);
`fsharp-code-generation` → deterministic, byte-comparable text rendering (the report
renderers moved verbatim); `fsharp-build-orchestration` → `Fake.Core.Target` dispatch,
golden-diff parity, and Expecto typed-finding tests.

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T001 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:14Z | `readiness/report-parity.md` | none |
| T006 | fsharp-code-generation | `.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:18Z | `build/Governance/Findings.fs` | none |
| T009 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:34Z | `tests/Governance.Tests/TargetMetadataTests.fs` | none |
| T011 | fsharp-code-generation | `.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:20Z | `build/Governance/TargetMetadata.fs` | none |
| T012 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:27Z | `build.fsx` (typed StartTarget dispatch) | none |
| T013 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:34Z | `tests/Governance.Tests/CapabilityCatalogTests.fs` | none |
| T014 | fsharp-parsing | `.agents/skills/fsharp-parsing/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:21Z | `build/Governance/Capabilities.fs` | none |
| T015 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:35Z | `tests/Governance.Tests/ReportParityTests.fs` | none |
| T016 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:27Z | `build.fsx` (#load + in-process calls) | none |
| T017 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:32Z | `readiness/report-parity.md` | none |
| T018 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T23:44Z | `readiness/governance-tests.md` | none |
| T022 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T21:55Z | `readiness/evidence-graph.md` | none |
| T023 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-05-31T21:53Z | 2026-05-31T21:57Z | `readiness/evidence-audit.md` | none |
