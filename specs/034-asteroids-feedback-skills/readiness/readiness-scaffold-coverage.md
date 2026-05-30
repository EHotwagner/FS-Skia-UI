# Readiness Scaffold Coverage

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `.specify/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/templates/tasks-template.md`, `.specify/presets/fsharp-opinionated/commands/speckit.tasks.md`, `.agents/skills/speckit-tasks/SKILL.md`, `template/base/README.md`
observed: `readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`, `readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`, `readiness/generated-guidance-validation.md`, and `readiness/real-image-evidence.md` are discoverable before audit.
missing: none.
failure class: ReadinessScaffoldCoverage.
next action: each scaffold must keep authoritative command, artifact path, failure class, and next action fields.

| Readiness path | Required fields | Status |
|----------------|-----------------|--------|
| `readiness/visual-evidence-honesty.md` | command, artifact path, failure class, next action | observed |
| `readiness/window-visibility.md` | command, artifact path, failure class, next action | observed |
| `readiness/governance-risk-levels.md` | command, artifact path, failure class, next action | observed |
| `readiness/aggregate-hang-diagnostics.md` | command, artifact path, failure class, next action | observed |
| `readiness/runtime-limitations.md` | command, artifact path, failure class, next action | observed |
| `readiness/generated-guidance-validation.md` | command, artifact path, failure class, next action | observed |
| `readiness/real-image-evidence.md` | command, artifact path, failure class, next action | observed |
