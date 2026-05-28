# Baseline Status

Feature: `023-phased-refactor-cleanup`

Captured at: `2026-05-27T22:33:40+02:00`

## Branch And Status

Branch: `023-phased-refactor-cleanup`

Initial `git status --short`:

```text
 M AGENTS.md
 M specs/023-phased-refactor-cleanup/spec.md
?? specs/023-phased-refactor-cleanup/contracts/
?? specs/023-phased-refactor-cleanup/data-model.md
?? specs/023-phased-refactor-cleanup/plan.md
?? specs/023-phased-refactor-cleanup/quickstart.md
?? specs/023-phased-refactor-cleanup/readiness/
?? specs/023-phased-refactor-cleanup/research.md
?? specs/023-phased-refactor-cleanup/tasks.deps.yml
?? specs/023-phased-refactor-cleanup/tasks.md
```

## Scope And Constraints

- Feature tier: Tier 2 internal cleanup.
- Public contract constraint: no public `.fsi` signature, surface baseline, package ID, generated profile name, generated command name, report field, status vocabulary, output path, exit-code meaning, FAKE target name, target dependency semantic, or readiness path may change in this feature.
- MVU/effect-boundary constraint: product and viewer state workflows, commands, effects, subscriptions, and host interpretation behavior remain unchanged; cleanup may only move ownership boundaries.
- Required real evidence paths:
  - `specs/023-phased-refactor-cleanup/readiness/baseline-status.md`
  - `specs/023-phased-refactor-cleanup/readiness/generated-evidence-cleanup.md`
  - `specs/023-phased-refactor-cleanup/readiness/template-split-validation.md`
  - `specs/023-phased-refactor-cleanup/readiness/build-governance-decomposition.md`
  - `specs/023-phased-refactor-cleanup/readiness/viewer-internal-boundary.md`

## Initial Baseline Commands

| Command | Exit code | Result | Notes |
|---------|-----------|--------|-------|
| `dotnet test tests/Testing.Tests/Testing.Tests.fsproj` | 0 | PASS | 28 passed, 0 failed, 0 skipped. |
| `dotnet test tests/Scene.Tests/Scene.Tests.fsproj` | 0 | PASS | 11 passed, 0 failed, 0 skipped. |
| `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` | 1 | PRE-EXISTING FAILURE | Test host crashed after 32 passing tests: `Failed to load plugin 'libdecor-gtk.so': failed to init`; record as unsupported/host-environment baseline issue. |
| `./fake.sh build -t TemplateCheck` | 0 | PASS | Completed `TemplatePack`, install, instantiate, smoke, and `TemplateCheck`; existing FAKE/F# warnings only. |

## Verdict

Initial baseline captured. The only pre-existing focused failure is the
SkiaViewer test host crash caused by `libdecor-gtk.so` initialization in the
current host environment.
