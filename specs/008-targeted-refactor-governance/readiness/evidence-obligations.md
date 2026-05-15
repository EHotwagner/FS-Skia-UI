# Evidence Obligations

## Feature Tier

Tier 1 governance and observable diagnostics with a constrained Tier 2 internal
runtime refactor.

## Public API Impact

`src/Lib/Library.fsi`, layout/chart signatures, documented public APIs, samples,
and package surface baselines are expected to remain stable. Public constructor
or validation API recommendations are recorded as follow-ups only.

## MVU And Effect Boundaries

- Runtime behavior remains reachable through `ViewerProgram`, `ViewerEffect`,
  `Viewer.run`, and the internal `VulkanHost.run` interpreter.
- Build workflow keeps `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`,
  emitted effects, and edge `interpret`.
- Layout workflow keeps `LayoutWorkflowModel`, `LayoutWorkflowMsg`,
  `LayoutWorkflowEffect`, pure `updateWorkflow`, and `interpretWorkflowEffect`.

## Synthetic Native Evidence Policy

Deterministic native acquisition failure tests use synthetic/instrumented handle
names so every failure stage can be forced without mutating the workstation GPU
or driver. These tests carry `Synthetic` in their names and code comments at the
fixture use site. Real native smoke remains required where the local environment
supports it; unsupported environments must record diagnostics separately from
implementation defects.

## Unsupported Scope

No broad rewrite, public API redesign, new renderer, package identity change,
release validation, external repository split, or distribution automation is in
scope.

## Required Evidence

| Obligation | Artifact |
|------------|----------|
| Public surface stability | `public-surface.txt`, package test logs |
| Semantic behavior | `semantic-tests.txt`, focused test logs |
| Native cleanup | `native-startup-cleanup.md`, `native-startup-cleanup-tests.txt`, `native-smoke.txt` |
| Generated guidance | `generated-guidance.md` |
| Template drift | `template-drift.md` |
| Yoga fallback diagnostics | `yoga-fallback-diagnostics.txt` |
| Record invariants | `record-invariants.md`, `follow-ups.md` |
| Evidence graph/audit | `task-graph.json`, `task-graph.md`, `logs/evidence-audit.txt` |
