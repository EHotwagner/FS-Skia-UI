# Implementation Plan: Template Framework Governance

**Branch**: `006-template-framework-governance` | **Date**: 2026-05-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/006-template-framework-governance/spec.md`

## Summary

Deliver v1 of Template Framework Governance by adding a canonical repository command surface, wiring existing build/test/package/evidence work behind named targets, moving package surface baselines to a stable current location, documenting the v1 workflow and roadmap boundary, and updating generated task guidance so future task lists call the canonical workflow instead of duplicating command order. V1 intentionally excludes template instantiation, dependency governance, new layout/visual gates, package consumer smoke, and release validation.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; FAKE build script in F#; shell entry points for Bash and Windows command environments  
**Primary Dependencies**: Existing .NET SDK, solution projects, Expecto test projects, F# scripts under `scripts/`, Spec Kit evidence extension, and FAKE as a repo-local .NET tool; no new runtime package dependency for `src/*`  
**Storage**: Filesystem only: `.config/dotnet-tools.json`, `build.fsx`, wrapper scripts, docs, root-level stable baselines, local package output under `~/.local/share/nuget-local/`, and feature readiness logs under `specs/006-template-framework-governance/readiness/`  
**Testing**: Canonical targets wrap `dotnet restore`, `dotnet build`, selected `dotnet test` projects, existing FSI/prelude scripts, stable baseline refresh/check workflows, sample contract smoke, task graph validation, evidence audit, and local pack verification  
**Target Platform**: Windows and Linux developer/CI environments that can run the current .NET solution; v1 full verification is non-visual and does not require GPU/window availability  
**Project Type**: Governed F# library repository with samples, tests, scripts, and Spec Kit workflow assets  
**Performance Goals**: `Dev` completes in 10 minutes or less on a supported development machine; `Verify` produces every required v1 artifact class in one run without hidden manual steps  
**Constraints**: V1 must not expand into template packaging, package consumer smoke, dependency governance, new visual/layout evidence gates, or release validation; package consumer restore tests must be outside the v1 verification path unless explicitly run as deferred roadmap work; no runtime `.fsi` API changes are expected  
**Scale/Scope**: Add the command surface, docs, current baseline path, existing evidence wiring, touched automation updates, and generated task guidance updates for this repository only

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. The spec now declares Tier 1 contracted governance and command-surface impact. There is no runtime F# public API change; the contracted surface is documented in [contracts/canonical-workflow.md](./contracts/canonical-workflow.md). Tests and quickstart validation exercise the command surface rather than private helpers.
- **Principle II - Visibility Lives in `.fsi`**: PASS. No new public runtime module is planned. If implementation introduces reusable F# source under `src/`, it must include `.fsi`; `build.fsx` remains a repo-local script, not a packaged public module.
- **Principle III - Idiomatic Simplicity**: PASS. The build graph uses named targets and simple process/file operations. No custom operators, SRTP, reflection, dynamic dispatch, type providers, or non-trivial computation expressions are planned.
- **Principle IV - Elmish/MVU Boundary**: PASS. This is repository automation, not product state or UI workflow, but it is process/file I/O-bearing. The build workflow must therefore expose a local effect boundary in `build.fsx`: `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure `update`, and an interpreter that executes process/file effects at the edge. No runtime application `Model`/`Msg`/`Effect` surface is introduced.
- **Principle V - Synthetic Evidence Disclosure**: PASS. V1 uses real command execution and real filesystem artifacts. No synthetic-only evidence is planned.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Tasks must add failing-first or currently failing checks for command availability, workflow transition/effect behavior, stable baseline resolution, docs references, and generated task guidance where applicable, then make them pass.
- **Principle VII - Observability and Safe Failure**: PASS. Canonical targets must emit actionable logs and fail when required v1 artifact classes are missing. Visual/GPU and package consumer smoke are explicitly deferred rather than silently skipped.
- **Change Classification**: PASS. Tier 1 contracted governance/command-surface change; no runtime `.fsi` public API impact.
- **Engineering Constraints**: PASS. F#/.NET remains the exclusive stack, local package output follows `~/.local/share/nuget-local/`, and no new runtime dependency is introduced.

## Project Structure

### Documentation (this feature)

```text
specs/006-template-framework-governance/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── canonical-workflow.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Phase 2 output from /speckit-tasks
```

### Source Code (repository root)

```text
.config/
└── dotnet-tools.json            # add FAKE local tool manifest

build.fsx                        # add canonical target graph
fake.sh                          # add Bash entry point
fake.cmd                         # add Windows cmd entry point

docs/
├── build.md                     # add canonical command usage
├── evidence.md                  # add v1 artifact and readiness policy
└── testing.md                   # add target-to-test mapping

readiness/
└── surface-baselines/
    ├── FS.Skia.UI.txt
    ├── FS.Skia.UI.Charts.txt
    └── FS.Skia.UI.Layout.txt    # stable current package surface baselines

scripts/
├── refresh-surface-baselines.fsx # update to write stable current baselines
├── prelude.fsx
├── charts-prelude.fsx
├── input-prelude.fsx
├── layout-prelude.fsx
└── parity-evidence.fsx          # wrap existing scripts from targets

tests/
├── Package.Tests/
│   ├── SurfaceAreaTests.fs      # update stable baseline path
│   └── Tests.fs                 # keep package consumer smoke out of v1 targets
└── Smoke.Tests/
    └── Tests.fs                 # existing sample smoke coverage

.specify/
├── presets/fsharp-opinionated/templates/tasks-template.md
└── workflows/speckit/workflow.yml # touch only if needed to call canonical targets
```

**Structure Decision**: Add a root-level FAKE command surface because it is the shared entry point for humans, agents, and automation. Keep stable current baselines at root-level `readiness/surface-baselines/` so package surface tests no longer depend on historical feature readiness folders. Keep feature-specific planning evidence in `specs/006-template-framework-governance/readiness/`.

## Complexity Tracking

No constitution violations require justification.

## Phase 0: Research

See [research.md](./research.md). Decisions cover the canonical target set, local tool manifest and wrapper strategy, stable baseline path, package consumer smoke deferral, v1 evidence artifact set, automation alignment, generated task guidance update, and roadmap boundaries.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/canonical-workflow.md](./contracts/canonical-workflow.md), and [quickstart.md](./quickstart.md).

Design summary:

- `build.fsx` owns target sequencing for `Dev`, `Verify`, `Ci`, `PackLocal`, `RefreshSurfaceBaselines`, `PackageSurfaceCheck`, `FsiTranscripts`, `SampleContractSmoke`, `EvidenceGraph`, and `EvidenceAudit`.
- Process and filesystem work in `build.fsx` is modeled through a local workflow effect algebra before execution: `BuildModel`, `BuildMsg`, `BuildEffect`, `init`, pure `update`, and an interpreter at the edge.
- `fake.sh` and `fake.cmd` are thin wrappers around the repo-local FAKE tool.
- `Dev` is the fast local path: restore, build, and default non-visual tests.
- `Verify` is the full v1 path: `Dev` plus package surface checks, FSI transcripts, sample smoke output, task graph validation, evidence audit, and required log capture.
- `PackLocal` writes packable project outputs to `~/.local/share/nuget-local/`.
- `RefreshSurfaceBaselines` writes `readiness/surface-baselines/*.txt`; `PackageSurfaceCheck` reads those same files and fails on stale or missing public surface names.
- Package consumer smoke is not part of v1 `Verify`; if preserved, it must live under an explicit deferred target outside v1 pass/fail criteria.
- Documentation explains target responsibilities, output locations, and deferred roadmap categories.
- Generated task guidance is updated to call canonical targets rather than repeating raw `dotnet` and evidence command order.

## Constitution Check - Post Design

- **Principle I**: PASS. The command contract and quickstart define the user-facing target surface before implementation tasks.
- **Principle II**: PASS. No runtime public F# module is added by the design.
- **Principle III**: PASS. The target graph is simple and named; no advanced F# features are required.
- **Principle IV**: PASS. The target graph remains the operator-facing command surface, and process/file I/O is modeled through a local workflow effect boundary before interpretation.
- **Principle V**: PASS. All planned evidence is real command output or file output; no synthetic-only artifacts are planned.
- **Principle VI**: PASS. Verification tasks will assert workflow transitions and emitted effects, target behavior, artifact creation, and docs/template references.
- **Principle VII**: PASS. Required artifact classes and deferred scopes are explicit, and missing artifacts must fail with actionable output.
- **Engineering Constraints**: PASS. F#/.NET stack and local package output conventions are preserved.
