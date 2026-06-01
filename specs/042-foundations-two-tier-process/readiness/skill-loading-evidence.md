# Skill Loading Evidence

Capability skills declared in `tasks.deps.yml` were resolved from
`.agents/skills/*/SKILL.md` and consulted as implementation aids before the
corresponding code changes. The cookbooks' recorded verdicts match the
implemented approach exactly: `fsharp-io-globbing` → fnmatch-style glob predicates
compiled to anchored regex over the `Diff` path set (the `RoutingRule.Matches`
predicates, `Routing.fs`); `fsharp-build-orchestration` → `Fake.Core.Target`
dispatch on the typed `Targets.Target`, the in-process `Route` edge wiring, and
Expecto typed-selector tests; `fsharp-code-generation` → deterministic,
byte-comparable text rendering (the `ContractView.render` emitter for
`validation.contract.yml`); `speckit-evidence-graph` / `speckit-evidence-audit` →
the governance graph/merge-gate workflow run for this dogfood feature.

| Task | Skill id | Resolved path | Load result | loaded_at | work_started_at | Evidence path | Reviewer exception |
|------|----------|---------------|-------------|-----------|-----------------|---------------|--------------------|
| T008 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:52Z | `tests/Governance.Tests/RoutingTests.fs` | none |
| T009 | fsharp-io-globbing | `.agents/skills/fsharp-io-globbing/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:55Z | `build/Governance/Routing.fs` | none |
| T010 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T01:05Z | `build.fsx` (Route edge + RouteSelect effect) | none |
| T012 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:52Z | `tests/Governance.Tests/RoutingTests.fs` (escalation cases) | none |
| T014 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:52Z | `tests/Governance.Tests/RoutingTests.fs` (--enforce core) | none |
| T015 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T01:05Z | `build.fsx` (runRouteSelection --enforce) | none |
| T017 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:52Z | `tests/Governance.Tests/RoutingTests.fs` (dogfood case) | none |
| T019 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:52Z | `tests/Governance.Tests/ContractViewTests.fs` | none |
| T020 | fsharp-code-generation | `.agents/skills/fsharp-code-generation/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T00:58Z | `build/Governance/ContractView.fs` | none |
| T023 | fsharp-build-orchestration | `.agents/skills/fsharp-build-orchestration/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T01:25Z | `tests/Governance.Tests/SequentialFakeGuidanceTests.fs` | none |
| T029 | speckit-evidence-graph | `.agents/skills/speckit-evidence-graph/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T01:30Z | `readiness/evidence-graph.md` | none |
| T030 | speckit-evidence-audit | `.agents/skills/speckit-evidence-audit/SKILL.md` | loaded | 2026-06-01T00:40Z | 2026-06-01T01:31Z | `readiness/evidence-audit.md` | none |
