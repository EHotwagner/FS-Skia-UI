---
title: Governance Kernel Extraction Implementation Plan
category: Design
categoryindex: 4
index: 18
description: Implementation plan for splitting the FS.Skia.UI governance rules into a dedicated pure F# project consumed by the build front-end and generated-product evidence runner.
---

# Governance Kernel Extraction Implementation Plan

- **Timestamp:** 2026-06-06T10:55:00+02:00
- **Author:** Codex
- **Status:** Implementation plan, not implemented
- **Audience:** Maintainers and agents working on the FS.Skia.UI governance system
- **Related analysis:** `docs/reports/2026-06-05-2237-governance-system-comprehensive-analysis.md`

## Executive Summary

The governance system should be split into a dedicated F# governance kernel project,
consumed by the current `FS.Skia.UI.Build` package and FAKE front-end. The split should
start inside this repository, not as an external repository. The goal is not to create a
generic policy-engine product on day one. The goal is to make the repository's governance
facts, rules, path classifiers, route decisions, artifact expectations, and explanations
explicitly testable outside the already complex build interpreter.

The recommended shape is:

```text
build/Governance.Core/FS.Skia.UI.Governance.Core.fsproj
  Pure facts, classifiers, rules, route selection, contract rendering,
  evidence graph/audit algorithms, artifact registries, and explain outputs.

build/Governance/FS.Skia.UI.Build.fsproj
  Packable build/evidence package. Keeps FAKE-facing target execution,
  filesystem/git/process edges, report writing, publish checks, generated-product
  orchestration, and the existing generated-product runner facade.

build/Build.fsproj
  Executable FAKE front-end. Still registers and dispatches targets.

tests/Governance.Core.Tests/Governance.Core.Tests.fsproj
  Fast deterministic unit/property/golden tests for the pure kernel.

tests/Governance.Tests/Governance.Tests.fsproj
  Integration and compatibility tests for the build package/front-end.
```

The implementation should be staged as an assembly extraction first. Namespace cleanup,
public package positioning, generalized rule DSLs, and external repository extraction are
later decisions. The first successful milestone is boring: `Route`, generated validation
contract rendering, evidence graph/audit outputs, and generated-product evidence behavior
remain byte-compatible while the pure logic compiles and tests in a smaller project.

## Problem Statement

`FS.Skia.UI.Build` currently carries several responsibilities in one packable project:

- target identity and target metadata;
- route selection;
- generated validation contract rendering;
- evidence graph and audit algorithms;
- skill discovery and validation models;
- capability catalog validation;
- generated product validation;
- package surface checks;
- generated guidance checks;
- preflight and process health checks;
- publish/pre-publish checks;
- FAKE target interpretation;
- filesystem, git, process, and report-writing effects.

Many modules already follow a pure-core / I/O-edge style, but the project boundary does not
make that separation visible. This makes the governance logic harder to review because a
reader must mentally distinguish pure policy from build orchestration while navigating a
large build package.

The current structure also makes tests less focused. `tests/Governance.Tests` validates a
wide mix of pure algorithms, generated-product behavior, process diagnostics, docs
guidance, and integration fixtures. That is useful for end-to-end confidence, but it is
not ideal for evaluating a governance knowledge system on its own terms.

## Decision

Create a separate local F# project named `FS.Skia.UI.Governance.Core` for the pure
governance kernel. `FS.Skia.UI.Build` will reference it and keep the current build
front-end and generated-product facade. The new project should be packable if it becomes
a runtime dependency of the packable `FS.Skia.UI.Build` package.

This is intentionally a local solution split first:

- one repository;
- one branch;
- one version line;
- one CI/build route;
- no cross-repository release choreography;
- no promise yet that the kernel is a reusable product outside FS.Skia.UI.

The implementation should preserve existing behavior first, then introduce clearer
knowledge-system APIs such as route explanations and JSON outputs.

## Design Principles

### 1. Extract the Pure Kernel, Not the Build Front-End

Move logic that is deterministic over supplied data:

- target identity and metadata;
- path classification;
- route selection;
- generated validation contract views;
- route explanations;
- artifact expectation and provenance models;
- evidence graph/audit algorithms;
- skill registry facts over supplied file lists/text;
- capability catalog validation over supplied YAML/text;
- generated guidance checks over supplied documents;
- package-surface comparison over supplied snapshots.

Keep effectful work in `FS.Skia.UI.Build`:

- git discovery;
- filesystem walking and file reads;
- process execution;
- `dotnet` commands;
- FAKE target registration and execution;
- package packing and template installation;
- generated product instantiation and smoke execution;
- report and readiness file writes;
- publish/pre-publish process interactions;
- concurrency locks.

### 2. Prefer Typed F# Rules Over a Generic Rule Engine

The kernel should use ordinary F# modules, records, discriminated unions, active patterns,
and pure functions. It should not start as a generalized Datalog, Prolog, OPA/Rego, or
custom rules DSL.

Good:

```fsharp
type GovernanceFact =
    | ChangedPath of string
    | ActiveFeature of string
    | RouteRule of RouteRuleFacts
    | ExpectedArtifact of ArtifactFacts
    | Skill of SkillFacts

type GovernanceConclusion =
    | SelectedGate of target: Target * reason: string
    | MissingArtifact of artifactId: string * reason: string
    | StaleArtifact of artifactId: string * reason: string
    | NextAction of command: string * reason: string
```

Risky:

```fsharp
type Rule =
    { Name: string
      When: obj list -> bool
      Then: obj list -> obj list }
```

The second version hides too much behind untyped plumbing and makes the system harder to
debug than the current direct code.

### 3. Use Active Patterns Where They Clarify Classification

Active patterns and partial active patterns are a good fit for path and artifact
classification. They should make policy read like policy:

```fsharp
let (|PublicFsiSurface|_|) path = ...
let (|TemplateContractPath|_|) path = ...
let (|GovernanceImplementationPath|_|) path = ...
let (|GeneratedGuidancePath|_|) path = ...
let (|HistoricalReport|ActiveGuidance|GeneratedView|) path = ...

let classifyChangedPath path =
    match normalizeRepoPath path with
    | PublicFsiSurface packageId -> PublicSurface packageId
    | TemplateContractPath capability -> TemplateContract capability
    | GovernanceImplementationPath area -> GovernanceImplementation area
    | HistoricalReport -> Documentation Historical
    | ActiveGuidance -> Documentation ActiveInstruction
    | GeneratedView -> GeneratedView
    | other -> UnknownPath other
```

They should not be used as ornament. If a record field or a simple helper is clearer,
use that.

### 4. Keep Compatibility Facades Until the Split Is Proven

The safest first extraction keeps public module names stable where practical. That can
mean the new assembly initially contains modules under the existing `FS.Skia.UI.Build`
namespace, even though the assembly/package is named `FS.Skia.UI.Governance.Core`.

This avoids a high-churn namespace rewrite in the same change as the assembly split. A
later cleanup can introduce `FS.Skia.UI.Governance` namespaces plus compatibility wrappers
if that proves valuable.

### 5. Make Tests Smaller and More Direct

The new test project should prove the kernel without running FAKE targets or touching
the real working tree. Integration tests should stay in `tests/Governance.Tests`.

Core tests should answer questions like:

- Does this path classify as active guidance, generated view, template contract, or
  historical report?
- Does adding a changed path ever lower the selected tier?
- Are selected gates de-duplicated in target registry order?
- Does the generated contract match the route-rule table?
- Does a fixture task graph produce the expected synthetic propagation?
- Does a stale artifact conclusion identify the producer target and route rule?
- Does route explanation name the matched paths and rule ids?

## Proposed Project Layout

### New Core Project

```text
build/Governance.Core/
  FS.Skia.UI.Governance.Core.fsproj
  README.md
  Findings.fsi
  Findings.fs
  Targets.fsi
  Targets.fs
  PathPatterns.fsi
  PathPatterns.fs
  PathClassification.fsi
  PathClassification.fs
  Routing.fsi
  Routing.fs
  RouteExplain.fsi
  RouteExplain.fs
  ArtifactRegistry.fsi
  ArtifactRegistry.fs
  ContractView.fsi
  ContractView.fs
  Capabilities.fsi
  Capabilities.fs
  ApiSurfaceGen.fsi
  ApiSurfaceGen.fs
  SkillFacts.fsi
  SkillFacts.fs
  SkillQuality.fsi
  SkillQuality.fs
  SkillSync.fsi
  SkillSync.fs
  Evidence/
    EvidenceFormatSchema.fsi
    EvidenceFormatSchema.fs
    TaskParser.fsi
    TaskParser.fs
    DepsParser.fsi
    DepsParser.fs
    SkillRegistry.fsi
    SkillRegistry.fs
    Graph.fsi
    Graph.fs
    StatusRegion.fsi
    StatusRegion.fs
    Scans.fsi
    Scans.fs
    DiffScan.fsi
    DiffScan.fs
    Audit.fsi
    Audit.fs
    Render.fsi
    Render.fs
    Engine.fsi
    Engine.fs
```

This is the target shape, not the first commit shape. Extraction should proceed in
dependency order and stop after each phase once parity tests pass.

### Existing Build Project After Extraction

```text
build/Governance/
  FS.Skia.UI.Build.fsproj
  AgentValidation.fsi
  AgentValidation.fs          # facade or adapter over Core routing
  GeneratedProduct.fsi
  GeneratedProduct.fs         # effectful generated-product validation
  GeneratedProductContract.fsi
  GeneratedProductContract.fs
  Preflight.fsi
  Preflight.fs
  Publish.fsi
  Publish.fs
  PrePublish.fsi
  PrePublish.fs
  Evidence/
    GeneratedRunner.fsi
    GeneratedRunner.fs        # stable generated-product facade, delegates to Core
  Front/
    BuildPaths.fs
    BuildEnvironment.fs
    BuildProcess.fs
    BuildReports.fs
    BuildGeneratedScanning.fs
    BuildPackageResolution.fs
    BuildTemplateValidation.fs
    BuildProcessHealth.fs
    Support.fs
    Helpers.fs
    Governance.fs
  Engine/
    Model.fsi
    Model.fs
    Update.fsi
    Update.fs
    Interpret.fsi
    Interpret.fs
```

The exact final ownership of `GeneratedProductContract`, `Guidance`, `PerPackageSurface`,
and `SkillistReference` should be decided by dependency pressure. If a module is pure over
supplied data and useful for explanations, it belongs in Core. If it shells out, writes
files, instantiates templates, resolves packages, or depends on build model state, it
belongs in Build.

### New Core Test Project

```text
tests/Governance.Core.Tests/
  Governance.Core.Tests.fsproj
  TestSupport.fs
  PathClassificationTests.fs
  TargetRegistryTests.fs
  RoutingKernelTests.fs
  RouteExplainTests.fs
  ContractViewTests.fs
  ArtifactRegistryTests.fs
  SkillFactsTests.fs
  EvidenceParserTests.fs
  EvidenceGraphTests.fs
  EvidenceAuditTests.fs
  CapabilityCatalogTests.fs
  Program.fs
```

`tests/Governance.Tests` should remain, but it should gradually become the integration
layer around `FS.Skia.UI.Build`.

## Packaging Decision

Because `FS.Skia.UI.Build` is packable and generated products consume it through a
reflected generated-product runner, a real project reference from `FS.Skia.UI.Build` to
`FS.Skia.UI.Governance.Core` changes the package dependency graph.

Recommended initial package stance:

- `FS.Skia.UI.Governance.Core` is packable from the first commit where
  `FS.Skia.UI.Build` depends on it.
- It shares the repository version line with the rest of `FS.Skia.UI.*`.
- `FS.Skia.UI.Build` declares a normal `ProjectReference` to it.
- The packed `FS.Skia.UI.Build.nupkg` should carry a dependency on
  `FS.Skia.UI.Governance.Core` with the same version.
- Generated products should still reference only `FS.Skia.UI.Build`; NuGet should restore
  the core package transitively.
- `GeneratedRunner.run` remains in `FS.Skia.UI.Build` so generated `template/base/build.fsx`
  does not need to learn a new reflection entry point.

This does introduce one more package. That cost is preferable to hiding the core assembly
inside the build package or relying on a non-packable project reference that may fail at
runtime when generated products restore only `FS.Skia.UI.Build`.

Acceptance tests must explicitly prove the generated-product reflection path still works.

## Compile And Dependency Strategy

### Core Dependencies

Start with the minimum existing dependency set:

- `FSharp.Core` from central package management.
- `YamlDotNet` only for pure parsers that already parse YAML from supplied strings.
- `DiffPlex` only if the extracted pure package-surface comparison needs it.
- `FS.Skia.UI.SkillSupport` only if extracted skill checks need shipped helper APIs.

Avoid dependencies on:

- FAKE packages;
- `System.Diagnostics.Process` helpers;
- `dotnet` command wrappers;
- repository-local path discovery modules;
- Skia, Silk.NET, UI runtime packages;
- test-only packages.

### Build Dependencies

`FS.Skia.UI.Build` keeps:

- `Fake.Core.Target` indirectly through `build/Build.fsproj`, not through Core;
- process and filesystem code;
- generated-product execution;
- publish/pre-publish code;
- report writing;
- route command-line parsing;
- concurrency lock acquisition.

### F# Compile Order

The extraction should preserve explicit compile order. A practical initial core order is:

```text
Findings
Targets
PathPatterns
PathClassification
Routing
RouteExplain
ContractView
ArtifactRegistry
SkillTreeGen
SkillSync
SkillQuality
Capabilities
ApiSurfaceGen
SkillContractPath
TemplateUpdatePackage
SkillistView
ConstitutionFragments
GovernedBlocks
CatalogGen
Evidence/*
SkillistReference
SymbolCrossCheck
PerPackageSurface
Guidance
```

This list should be adjusted by actual dependencies during implementation. The important
constraint is that `Front/*`, `Engine/Model`, `Engine/Update`, and `Engine/Interpret`
should not leak into Core.

## Feature Scope

### In Scope

- Create `FS.Skia.UI.Governance.Core` as a local F# library project.
- Create `Governance.Core.Tests` as a fast unit/property test project.
- Move pure governance modules from `FS.Skia.UI.Build` into Core in phases.
- Keep `FS.Skia.UI.Build` as the packable generated-product and FAKE-facing package.
- Add or preserve facades where needed so current commands and generated products keep
  working.
- Introduce path classification types and active patterns for route and retired-term
  decisions.
- Add a typed route explanation model usable by future `Route --json`.
- Preserve current route text output and generated `validation.contract.yml` parity.
- Preserve evidence graph/audit artifact parity for representative fixtures.
- Prove the generated-product evidence runner still loads and runs from the packed
  `FS.Skia.UI.Build` package.

### Out Of Scope For The First Implementation

- Moving governance into a separate Git repository.
- Replacing FAKE.
- Replacing direct F# validators with a generic rule engine.
- Renaming every public namespace to `FS.Skia.UI.Governance`.
- Changing generated-product `template/base/build.fsx` reflection entry points.
- Changing the route-selected gate policy except where explicit governance path coverage
  is already needed.
- Adding a full stale-artifact provenance system.
- Adding worktree-level governance locks, unless the implementation naturally touches the
  target entry point and can do it safely.

## Implementation Phases

### Phase 0: Baseline And Safety Check

Purpose: capture current behavior before moving anything.

Tasks:

1. Run `./fake.sh build -t Route` and follow only the printed gates for the actual branch
   before implementation starts.
2. Capture current `Route` text output for focused fixture diffs in tests.
3. Capture current generated `validation.contract.yml` output in existing contract-view
   tests.
4. Capture current evidence graph/audit golden outputs for representative fixtures.
5. Capture generated-product evidence runner behavior through the existing generated
   product tests.
6. Record current package dependency behavior for `FS.Skia.UI.Build`.

Deliverables:

- No code movement yet.
- A short readiness note for baseline commands and results.
- A list of parity tests that must stay green throughout extraction.

Acceptance:

- Baseline tests are green before extraction begins.
- Any failing existing test is either fixed first or explicitly excluded from this feature
  with a documented reason.

### Phase 1: Add Empty Core Project And Test Harness

Purpose: introduce the project boundary without moving policy.

Tasks:

1. Add `build/Governance.Core/FS.Skia.UI.Governance.Core.fsproj`.
2. Add package metadata:
   - `PackageId` = `FS.Skia.UI.Governance.Core`
   - `AssemblyName` = `FS.Skia.UI.Governance.Core`
   - `IsPackable` = `true`
   - description = pure governance kernel consumed by `FS.Skia.UI.Build`
3. Add `build/Governance.Core/README.md`.
4. Add `tests/Governance.Core.Tests/Governance.Core.Tests.fsproj`.
5. Add both projects to `FS-Skia-UI.sln` under the existing build/test solution folders.
6. Add a trivial smoke module and test to prove project wiring.
7. Add a `ProjectReference` from `tests/Governance.Core.Tests` to the core project.
8. Do not reference Core from `FS.Skia.UI.Build` yet unless the smoke module is needed.

Deliverables:

- Empty but compiling core project.
- Empty but compiling test project.
- No behavior changes.

Acceptance:

- Solution builds.
- Core test smoke passes.
- No package dependency behavior changes yet.

### Phase 2: Move Target Identity And Routing

Purpose: extract the highest-value pure policy while preserving route behavior.

Candidate moved modules:

- `Findings`
- `Targets`
- `Routing`
- `ContractView`

New modules:

- `PathPatterns`
- `PathClassification`
- `RouteExplain`

Tasks:

1. Move `Findings`, `Targets`, `Routing`, and `ContractView` source files to Core.
2. Keep their module names stable initially if that reduces churn.
3. Add `ProjectReference` from `FS.Skia.UI.Build` to Core.
4. Remove moved compile includes from `FS.Skia.UI.Build.fsproj`.
5. Update `tests/Governance.Tests` to reference Core directly where needed.
6. Move pure tests for targets, routing, and contract rendering into
   `tests/Governance.Core.Tests`.
7. Keep integration tests for the `Route` FAKE target in `tests/Governance.Tests`.
8. Add path-classification tests for:
   - `build/Governance/**`
   - `build/Program.fs`
   - `build/Build.fsproj`
   - root `fake.sh`
   - `template/base/build.fsx`
   - `src/**/*.fsi`
   - `.specify/**`
   - `.agents/skills/**`
   - `.claude/skills/**`
   - historical `docs/reports/**`
9. Add route explanation types, but do not change the default `Route` text output.

Acceptance:

- Existing `Route` output is unchanged.
- `validation.contract.yml` rendering is unchanged.
- Core routing tests pass without filesystem or git access.
- Build front-end still compiles and dispatches typed targets.
- `Route` can still be run through `./fake.sh`.

### Phase 3: Move Evidence Graph And Audit Core

Purpose: isolate the most formal governance subsystem in Core.

Candidate moved modules:

- `Evidence/EvidenceFormatSchema`
- `Evidence/TaskParser`
- `Evidence/DepsParser`
- `Evidence/SkillRegistry`
- `Evidence/Graph`
- `Evidence/StatusRegion`
- `Evidence/Scans`
- `Evidence/DiffScan`
- `Evidence/Audit`
- `Evidence/Render`
- `Evidence/Engine`

Tasks:

1. Move evidence modules to Core in compile order.
2. Keep `Evidence/GeneratedRunner` in `FS.Skia.UI.Build` as a stable facade.
3. Update `GeneratedRunner` to call `FS.Skia.UI.Governance.Core` evidence APIs.
4. Move pure parser, graph, audit, scan, render, and golden tests to
   `Governance.Core.Tests`.
5. Keep generated-product runner tests in `Governance.Tests` or `Package.Tests`.
6. Ensure every evidence input remains supplied as data. Do not move filesystem reads into
   Core.
7. Add property tests for graph invariants:
   - cycle detection reports at least one participating task;
   - topological order respects dependencies;
   - synthetic propagation is monotonic;
   - accepted `[SEH]` summaries are explicit and do not hide blocking statuses.

Acceptance:

- Evidence graph/audit artifacts are byte-compatible for fixtures.
- Generated-product evidence runner still works through `FS.Skia.UI.Build`.
- Core evidence tests require no git repository and no FAKE target.
- No active feature state is read by Core directly.

### Phase 4: Move Skill, Capability, Contract, And Generated-View Pure Logic

Purpose: expand the kernel to the rest of the deterministic governance model.

Candidate moved modules:

- `SkillTreeGen`
- `SkillSync`
- `SkillQuality`
- `SkillContractPath`
- `SkillistView`
- `SkillistReference`
- `Capabilities`
- `ApiSurfaceGen`
- `TemplateUpdatePackage`
- `ConstitutionFragments`
- `GovernedBlocks`
- `CatalogGen`
- `SymbolCrossCheck`
- pure portions of `Guidance`
- pure portions of `PerPackageSurface`

Tasks:

1. Move one dependency cluster at a time.
2. Split any mixed module before moving it:
   - pure evaluator to Core;
   - filesystem/process wrapper remains in Build.
3. Move corresponding tests into `Governance.Core.Tests` where they are pure.
4. Keep generated file writing and currency-check command execution in Build.
5. Add tests that distinguish:
   - canonical source;
   - generated view;
   - active guidance;
   - historical report;
   - fixture.
6. Add retired-term policy tests using path roles rather than repo-wide raw string scans.

Acceptance:

- `GeneratedGuidanceCheck` behavior is unchanged from the outside.
- `.agents` to `.claude` skill sync behavior is unchanged.
- Capability catalog validation behavior is unchanged.
- Generated API-surface docs are unchanged for current catalog fixtures.
- Core tests cover the rule decisions directly.

### Phase 5: Introduce The Governance Snapshot Model

Purpose: make the knowledge-system boundary explicit without changing existing gates.

New core concepts:

```fsharp
type RouteScope =
    | WholeWorkspace
    | ExplicitPaths of string list
    | StagedChanges
    | SinceBase of string

type GovernanceSnapshot =
    { ChangedPaths: string list
      ActiveFeature: string option
      Targets: TargetFacts list
      RouteRules: RouteRuleFacts list
      Skills: SkillFacts list
      Capabilities: CapabilityFacts list
      Artifacts: ArtifactFacts list
      ConcurrentRuns: ConcurrentRunFacts list }

type GovernanceQuery =
    | ExplainRoute of RouteScope
    | ExplainArtifacts of RouteScope
    | ExplainSkills of string option
    | ExplainNextActions of RouteScope

type Explanation =
    { Summary: string
      Conclusions: GovernanceConclusion list
      Provenance: ExplanationProvenance list }
```

Tasks:

1. Add typed fact models for targets, route rules, skills, artifacts, capabilities, and
   changed paths.
2. Add pure query functions over `GovernanceSnapshot`.
3. Add JSON-friendly DTO renderers in Core or a small rendering submodule.
4. Add Markdown rendering for human route explanations.
5. Wire only read-only build commands to gather snapshots at the edge.
6. Keep current `Route` output stable by default.
7. Add tests for explanation provenance:
   - every selected gate names a rule or default-deny reason;
   - every missing artifact names its expected producer or requiring rule;
   - every path-scoped decision lists the scoped paths used.

Acceptance:

- Snapshot evaluation is pure and testable.
- Existing route behavior remains the default.
- New explanation APIs can support future `Route --json` and `Route --explain`.
- No gate starts expensive product validation merely to answer explain-only questions.

### Phase 6: Build Front-End Integration

Purpose: consume the kernel through stable build commands.

Tasks:

1. Update `Front/Governance.fs` and `Engine/Interpret.fs` to gather facts and call Core.
2. Keep command-line parsing and filesystem reads in Build.
3. Add `Route --json` using Core renderers.
4. Optionally add `Route --paths <path...>` if the scope model is ready.
5. Keep `Route --enforce` behavior as artifact presence unless a provenance feature is
   implemented in the same change.
6. Add a route report artifact only if the implementation includes the required writer and
   tests.
7. Ensure FAKE-backed target execution remains serialized and does not get hidden inside
   the core project.

Acceptance:

- `./fake.sh build -t Route` text output remains stable.
- `./fake.sh build -t Route --json` produces deterministic JSON.
- `Route --json` does not run FAKE-backed gates.
- `Route --paths docs/reports/example.md --json` can explain scoped authoring decisions
  if implemented.
- `Route --enforce` diagnostics remain clear and tested.

### Phase 7: Package And Generated Product Validation

Purpose: prove the package dependency graph and reflection runner.

Tasks:

1. Pack `FS.Skia.UI.Governance.Core`.
2. Pack `FS.Skia.UI.Build`.
3. Inspect `FS.Skia.UI.Build.nuspec` to confirm it depends on
   `FS.Skia.UI.Governance.Core` at the same version.
4. Install or instantiate a generated product that references `FS.Skia.UI.Build`.
5. Confirm NuGet restores the transitive Core package.
6. Confirm `template/base/build.fsx` still reflection-loads
   `FS.Skia.UI.Build.Evidence.GeneratedRunner.run`.
7. Confirm `GeneratedRunner.run` can call into Core at runtime.

Acceptance:

- Generated product does not reference Core directly.
- Transitive restore brings Core into the generated product dependency closure.
- Generated evidence graph/audit still runs.
- No duplicate assembly or binding conflict appears in the generated product.

### Phase 8: Documentation And Guidance Update

Purpose: make the new boundary visible to maintainers and agents.

Tasks:

1. Update `build/Governance/README.md` to explain the split:
   - Core owns pure rules and evidence algorithms;
   - Build owns FAKE/effects/generated-product facade.
2. Add `build/Governance.Core/README.md`.
3. Update active guidance that references governance source homes.
4. Update generated validation contract documentation if route explanation fields are
   added.
5. Update `docs/reports/build.md` or another active architecture page if it describes
   the old single-project model.
6. Avoid changing historical reports except to add a new superseding report link if the
   docs pattern requires it.

Acceptance:

- Active docs name the new source homes.
- Historical report content is not rewritten as if it were active policy.
- Generated guidance checks pass.

## Test Plan

### Core Unit Tests

Add focused tests in `Governance.Core.Tests`:

- `PathClassificationTests`
  - normalizes slash direction;
  - handles root-relative and `./` paths;
  - distinguishes `template/base/build.fsx` from retired root `build.fsx`;
  - classifies `build/Governance/**` as governance implementation;
  - classifies `.agents` as canonical skills and `.claude` as generated skills;
  - classifies timestamped reports as historical docs unless explicitly active.

- `TargetRegistryTests`
  - `Targets.spec` is total over dispatch targets;
  - all target names are unique;
  - runnable targets exclude non-registry dispatch-only targets;
  - dependency rows are deterministic.

- `RoutingKernelTests`
  - highest tier wins;
  - unmatched paths default-deny;
  - consumer-agent floor composes with path escalation;
  - dogfood override forces full pipeline;
  - gates are de-duplicated in registry order;
  - adding changed paths never lowers the selected tier.

- `ContractViewTests`
  - rendered contract is stable;
  - every rendered rule corresponds to a typed route rule;
  - every rendered gate is a typed target.

- `RouteExplainTests`
  - every matched rule has matched paths;
  - every selected gate has a reason;
  - default-deny conclusions are explicit;
  - JSON rendering is deterministic.

- `ArtifactRegistryTests`
  - route-required artifacts know their producer target where known;
  - feature-relative artifacts resolve with the active feature;
  - presence-only and freshness-aware checks are distinct.

- `EvidenceGraphTests` and `EvidenceAuditTests`
  - fixture parity for graph JSON/Markdown;
  - cycle detection;
  - synthetic propagation;
  - `[SEH]` accepted summary behavior;
  - diff-scan blocking/advisory classification.

### Property Tests

Use FsCheck where invariants are clearer as properties:

- route tier monotonicity as paths are added;
- gate de-duplication preserves first registry occurrence;
- path normalization is idempotent;
- contract rendering and parsed compatibility views agree on rule ids;
- task graph topological order respects every dependency;
- artifact requirement union is order-stable.

### Integration Tests

Keep these in `Governance.Tests`, `Package.Tests`, or existing generated-product tests:

- `./fake.sh build -t Route` command behavior;
- FAKE target dispatch and dependency wiring;
- generated-product reflection runner;
- template pack/install/instantiate paths;
- package nuspec dependency inspection;
- report writing and readiness artifact locations;
- preflight, process health, publish/pre-publish checks.

### Golden Parity

Before and after each movement phase, verify:

- route text output for representative diffs;
- `validation.contract.yml`;
- target metadata output where affected;
- evidence graph JSON/Markdown for fixtures;
- audit hit JSON for fixtures;
- generated API-surface docs if capability modules move;
- skill sync generated tree if skill modules move.

## Validation Plan

For the actual implementation branch, follow the repository rule:

1. Run `./fake.sh build -t Route`.
2. Run only the gates it prints, sequentially.
3. If the route escalates to maintainer verification, use the serialized order:
   - `./fake.sh build -t Dev`
   - `./fake.sh build -t GeneratedGuidanceCheck`
   - `./fake.sh build -t TemplateCheck`
   - `./fake.sh build -t GeneratedProductCheck`
   - `./fake.sh build -t EvidenceGraph`
   - `./fake.sh build -t EvidenceAudit`

Expected route characteristics:

- Edits under `build/Governance/**`, new `build/Governance.Core/**`, project files,
  tests, package metadata, and active guidance should route beyond the inner loop.
- Because the current route rules do not yet explicitly cover all governance
  implementation paths, the implementation may default-deny to broad verification until
  route coverage is added.
- If route coverage is added in the same feature, its contract rendering and
  `validation.contract.yml` currency must be part of the evidence.

Additional manual checks for this feature:

- Inspect packed `FS.Skia.UI.Build.nupkg` dependencies.
- Instantiate a generated product and run its evidence command.
- Confirm no runtime UI package references appear in Core.
- Confirm no FAKE package references appear in Core.
- Confirm no filesystem/process code appears in Core except unavoidable BCL value types.

## Migration Strategy

### Keep Existing Names First

First move files, not concepts. It is acceptable for the new assembly to contain modules
under the old namespace temporarily. This keeps the diff reviewable and focuses on
project boundary correctness.

Later, introduce `FS.Skia.UI.Governance` namespaces if there is a clear benefit. If that
happens, provide compatibility wrappers in `FS.Skia.UI.Build` for any external or
generated-product API that should remain stable.

### Move Tests With The Modules

When a pure module moves, move its direct tests to `Governance.Core.Tests`. Keep tests that
exercise FAKE, filesystem, generated products, packaging, or command-line behavior in the
existing integration test project.

### Split Mixed Modules Before Moving

Do not move mixed modules wholesale if they perform filesystem or process effects. Split
them first:

```text
CurrentModule.fs
  read files
  parse
  evaluate
  render
  write files

Core/CurrentRules.fs
  parse supplied text
  evaluate supplied facts
  render string/DTO outputs

Build/CurrentCommand.fs
  read files
  call Core
  write files
```

### Preserve Generated Product Entry Points

Generated products should not need to change during the extraction. Keep:

```text
FS.Skia.UI.Build.Evidence.GeneratedRunner.run
```

as the reflection-invoked facade. Internally it can delegate to Core.

## Risks And Mitigations

### Risk: Package Dependency Breaks Generated Products

If `FS.Skia.UI.Build` depends on Core but the Core assembly is not restored into generated
products, the reflection runner can fail at runtime.

Mitigation:

- make Core packable;
- verify `FS.Skia.UI.Build.nuspec` dependency;
- run generated-product validation from packed packages;
- keep generated products referencing only `FS.Skia.UI.Build`.

### Risk: Namespace Rename Churn Hides Behavior Changes

Renaming modules while moving assemblies can make review noisy.

Mitigation:

- keep namespaces stable in the first extraction;
- add namespace cleanup only after parity is proven;
- keep compatibility wrappers for generated-product APIs.

### Risk: Core Slowly Gains Effects

A separate project is not useful if it starts reading the real repository or shelling out.

Mitigation:

- ban FAKE/process dependencies in Core;
- write tests using supplied strings and in-memory facts;
- review dependencies in the project file;
- keep filesystem reads in Build.

### Risk: Generic Expert-System Abstraction Becomes Opaque

A generalized rule engine could make direct policy harder to understand.

Mitigation:

- use typed F# records/unions/functions first;
- use active patterns only for classification clarity;
- require every conclusion to carry provenance;
- prefer ordinary exhaustive pattern matches over stringly runtime rules.

### Risk: Duplicate Source During Migration

Moving modules between projects can accidentally leave duplicate module definitions.

Mitigation:

- remove compile includes from Build in the same patch that adds them to Core;
- keep compile order explicit;
- build after each cluster;
- avoid large "move everything" commits.

### Risk: Tests Become Fragmented

Splitting tests can make it unclear where a failure belongs.

Mitigation:

- Core tests own pure behavior;
- Build tests own integration/effects;
- test names should state the boundary they prove;
- keep a small parity matrix in the readiness evidence.

## Acceptance Criteria

The feature is complete when:

- `FS.Skia.UI.Governance.Core` exists and contains the pure governance kernel.
- `FS.Skia.UI.Build` references Core and keeps effectful build/front-end work.
- `GeneratedRunner.run` remains available from `FS.Skia.UI.Build`.
- Core has focused unit/property/golden tests.
- Existing integration tests still validate FAKE commands and generated products.
- `Route` text output is unchanged unless an intentional route feature is included.
- `validation.contract.yml` remains generated from typed route facts.
- Evidence graph/audit fixture outputs remain compatible.
- Packed `FS.Skia.UI.Build` restores Core transitively.
- Generated products can still run evidence validation.
- Active guidance documents name the new source homes.
- Route-selected gates pass in the order printed by `Route`.

## Recommended Task Breakdown

1. Create the Spec Kit feature and classify it as governance/build-contract work.
2. Add baseline route/contract/evidence/generated-product parity tests if any are missing.
3. Add empty Core project and Core test project.
4. Move `Findings`, `Targets`, `Routing`, and `ContractView`.
5. Add path classification active patterns and tests.
6. Add route explanation models and tests, without changing default `Route` output.
7. Move evidence parser/graph/audit core and tests.
8. Keep `GeneratedRunner` in Build and delegate to Core.
9. Move skill/capability/generated-view pure modules cluster by cluster.
10. Introduce `GovernanceSnapshot` and pure explanation queries.
11. Wire optional `Route --json` through the build edge.
12. Prove pack dependency and generated-product reflection behavior.
13. Update active docs and guidance.
14. Run route-selected gates sequentially.
15. Review the diff for accidental namespace churn, dependency leaks, and generated-product
    package regressions.

## Final Recommendation

The split makes sense, but it should be framed as a local kernel extraction rather than a
new generalized governance platform. The first implementation should make the current
rules easier to test and explain without weakening the existing route-selected gates.

The most valuable early outcome is a small, fast test surface for governance decisions:
path classification, target identity, route selection, contract rendering, artifact
expectations, evidence graph/audit algorithms, and explanation provenance. Once that is
stable, higher-level features such as `Route --json`, scoped authoring validation,
artifact freshness, and concurrency-aware explanations become much easier to add without
turning the build front-end into an even larger policy module.
