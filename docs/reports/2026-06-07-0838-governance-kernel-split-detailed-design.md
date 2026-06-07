---
title: Governance Kernel Split Detailed Design
category: Design
categoryindex: 4
index: 19
description: Detailed design for splitting the governance kernel from the FAKE-facing build package while keeping rules typed, explainable, and reusable without inventing a new rule language.
---

# Governance Kernel Split Detailed Design

- **Timestamp:** 2026-06-07T08:38:00+02:00
- **Author:** Codex
- **Status:** Detailed design, not implemented
- **Audience:** Maintainers and agents extracting the FS.Skia.UI governance kernel
- **Supersedes/extends:** `docs/reports/2026-06-06-1055-governance-kernel-extraction-implementation-plan.md`
- **External design reference:** [Hopac/Hopac](https://github.com/Hopac/Hopac), especially the split between `Libs/Hopac.Core` and `Libs/Hopac`

## Executive Summary

Split the governance system into a packable pure kernel project and a smaller
FAKE/build-edge project, but do **not** introduce a new rule language. The reusable part
should be a typed F# inference substrate: supplied facts in, derived facts and query
conclusions out, with deterministic fixed-point evaluation and provenance for every
derived claim. Domain rules should remain ordinary F# discriminated unions, records,
active patterns, and pure functions.

Interfaces and inheritance should be used sparingly. They are useful at binary or
effectful boundaries, and possibly for internal optimized evaluator machinery after
profiling. They should not be the primary model for facts, explanations, route rules, or
governance conclusions. Those are better represented as closed union cases, nominal ID
types, immutable records, and modules with explicit pure functions.

The Hopac lesson is not "copy its class hierarchy." Hopac exposes a compact F# surface
with concepts such as `Job<'x>`, `Alt<'x>`, channels, promises, and module functions, while
its low-level `Hopac.Core` uses abstract/internal machinery for runtime execution and
performance. The governance split should follow that shape: a small, strong conceptual
surface for users of the kernel; hidden or internal implementation types only where they
buy real semantics, performance, or interop.

## Design Position

### The Primary Decision

The split has two goals:

1. Make the current governance rules, route selection, evidence graph/audit, artifact
   expectations, and generated contract rendering testable without running FAKE or reading
   the live repository.
2. Create a reusable inference substrate that can later support other rule-based contexts
   without forcing those contexts to learn a custom language.

The first implementation should therefore be:

```text
ordinary typed F# domain model
  + small generic fixed-point evaluator
  + provenance-rich explanations
  + generated DTO/rendering surfaces
  + compatibility facades for existing generated products
```

It should **not** be:

```text
new policy syntax
  + parser
  + type checker
  + runtime object model
  + editor/tooling gap
  + custom debugging story
```

The system is already written in F#. F# already gives the important rule-authoring
features: closed unions, exhaustiveness checking, pattern matching, active patterns,
records, modules, functions as values, property testing, and good compiler diagnostics.
The split should amplify those strengths rather than hide them behind stringly or
objectly rule plumbing.

### What To Reuse Outside Governance

If another context needs a rule-based engine, reuse the substrate, not the governance
domain:

```text
Reusable substrate:
  RuleId / FactId / QueryId nominal IDs
  source locations
  provenance graph
  deterministic fact store
  fixed-point evaluator
  rule trace
  diagnostics
  explanation rendering helpers
  property-testable invariants

Governance-specific domain:
  Target
  Tier
  DeveloperClass
  RouteRuleFacts
  PathRole
  ArtifactFacts
  EvidenceStatus
  AgentAction
  AgentDecision
```

This keeps the reusable engine small and makes the first non-governance adoption cheap:
a second context defines its own fact union, identity function, rules, queries, and
renderers.

## Hopac-Inspired Boundaries

Hopac is a useful reference because it separates a small conceptual API from an optimized
runtime substrate:

- `Libs/Hopac/Hopac.fsi` presents the public F# concepts and module functions. The surface
  is semantic: `Job<'x>`, `Alt<'x>`, channels, latches, mailboxes, promises, and
  documented reference implementations.
- `Libs/Hopac/TopLevel.fsi` exposes convenience bindings in an auto-open module, keeping
  the user-facing layer approachable.
- `Libs/Hopac.Core` contains the low-level runtime. It uses abstract classes such as
  `Handler` and `Work` to model execution machinery and scheduling details.

The governance kernel should borrow three ideas:

1. **Small public concepts.** Make the conceptual model compact enough that users can hold
   it in their head: facts, rules, derivations, explanations, queries.
2. **Reference semantics in docs/tests.** For each rule primitive, document the simple
   reference behavior and allow optimized internals later.
3. **Implementation machinery stays behind the surface.** If the evaluator later needs
   indexes, mutable work queues, or internal classes, hide them behind modules and
   signatures.

It should not borrow object orientation as a default public modeling style. Hopac uses
abstract classes because it is a scheduler/runtime. The governance kernel is mostly
deterministic data transformation. Its public substance should come from type precision
and exhaustive rules, not inheritance depth.

## Project Architecture

### Target Projects

```text
build/Governance.Core/FS.Skia.UI.Governance.Core.fsproj
  Packable pure kernel package.
  No FAKE dependency.
  No process execution.
  No live repository reads.
  No report writes.
  Owns domain facts, rules, evidence algorithms, contract views, renderers,
  and the generic inference substrate.

build/Governance/FS.Skia.UI.Build.fsproj
  Packable FAKE/generated-product edge package.
  References Governance.Core.
  Owns target execution, filesystem/git/process reads, generated-product
  orchestration, report writing, package/template validation edges, and
  compatibility facades that generated products reflection-load.

build/Build.fsproj
  Executable FAKE front-end.
  Registers targets and dispatches through FS.Skia.UI.Build.

tests/Governance.Core.Tests/Governance.Core.Tests.fsproj
  Pure unit/property/golden tests over Governance.Core.

tests/Governance.Tests/Governance.Tests.fsproj
  Integration tests over Build, FAKE target behavior, package edges, generated
  products, report writing, and compatibility entry points.
```

### Package Graph

`FS.Skia.UI.Build` is already packable and consumed by generated products. Once it
references `FS.Skia.UI.Governance.Core`, the core must also be packable.

```text
Generated product
  references FS.Skia.UI.Build
    transitively restores FS.Skia.UI.Governance.Core
```

Generated products should continue to reflection-load:

```text
FS.Skia.UI.Build.Evidence.GeneratedRunner.run
```

That facade remains in `FS.Skia.UI.Build` and delegates to the core evidence engine.

### Dependency Policy

`Governance.Core` dependencies:

- `FSharp.Core`
- BCL collections, regex, JSON/text primitives as needed
- `YamlDotNet` only for pure parsing of supplied YAML strings
- `DiffPlex` only for pure diff comparison/rendering that already uses it
- `FS.Skia.UI.SkillSupport` only where shipped skill helper APIs are part of the pure
  validation contract

`Governance.Core` must not depend on:

- FAKE packages
- `System.Diagnostics.Process` wrappers
- repository root discovery
- live filesystem walking
- git commands
- Skia/Silk/UI runtime packages
- generated-product instantiation code

## Layered Design

### Layer 0: Primitive IDs And Diagnostics

Create nominal ID types. Do not use raw strings throughout the kernel.

```fsharp
namespace FS.Skia.UI.Governance

[<Struct>]
type RuleId = private RuleId of string

[<Struct>]
type FactId = private FactId of string

[<Struct>]
type QueryId = private QueryId of string

[<Struct>]
type ArtifactId = private ArtifactId of string

[<Struct>]
type RepoPath = private RepoPath of string

type DiagnosticSeverity =
    | Info
    | Warning
    | Error

type KernelDiagnostic =
    { Code: string
      Severity: DiagnosticSeverity
      Message: string
      Source: SourceRef option }
```

Each private constructor gets a companion module:

```fsharp
module RuleId =
    val create: string -> Result<RuleId, string>
    val unsafe: string -> RuleId
    val value: RuleId -> string
```

The `unsafe` constructor is acceptable inside static rule tables where invalid literals
should fail tests and review. User/input paths should use validating constructors.

### Layer 1: Generic Inference Substrate

The substrate is reusable and domain-agnostic. It should not know about targets, routes,
features, skills, or artifacts.

```fsharp
namespace FS.Skia.UI.Governance.Inference

type SourceRef =
    | Input of label: string
    | File of path: string * line: int option
    | Rule of RuleId
    | Generated of label: string

type ProvenanceStep =
    { Rule: RuleId
      Inputs: FactId list
      Reason: string
      Source: SourceRef option }

type FactAssertion<'fact> =
    { Id: FactId
      Value: 'fact
      Provenance: ProvenanceStep list }

type DerivedFact<'fact> =
    { Value: 'fact
      Provenance: ProvenanceStep }

type FactSet<'fact>

type Rule<'fact> =
    { Id: RuleId
      Description: string
      Apply: FactSet<'fact> -> DerivedFact<'fact> list }

type EvaluationOptions =
    { MaxIterations: int
      TraceRules: bool }

type RuleTrace =
    { Iteration: int
      Rule: RuleId
      Produced: FactId list }

type EvaluationResult<'fact> =
    { Facts: FactAssertion<'fact> list
      Trace: RuleTrace list
      Diagnostics: KernelDiagnostic list
      Converged: bool }

type FactIdentity<'fact> = 'fact -> FactId

module FixedPoint =
    val evaluate:
        options: EvaluationOptions ->
        identify: FactIdentity<'fact> ->
        rules: Rule<'fact> list ->
        supplied: FactAssertion<'fact> list ->
            EvaluationResult<'fact>
```

The evaluator contract:

- deterministic rule order
- deterministic output order
- stable de-duplication by `FactId`
- idempotent repeated evaluation
- bounded iteration with a diagnostic if rules fail to converge
- no side effects
- no reflection
- no dynamic casts

This is the key rule-engine abstraction. It is enough for route/explain/agent queries
without inventing another syntax.

### Layer 2: Governance Domain Facts

The governance domain uses a closed union for facts. Closed unions give robust substance:
the compiler forces every evaluator, renderer, or query to acknowledge new fact kinds.

```fsharp
namespace FS.Skia.UI.Governance.Domain

type DeveloperClass =
    | FrameworkAuthor
    | ConsumerAgent

type Tier =
    | InnerLoop
    | FocusedAuthority
    | AgentReady
    | MaintainerVerify
    | AutomationFinal

type PathRole =
    | FrameworkImplementation
    | PublicPackageSurface of packageId: string
    | TemplateContract
    | GeneratedGuidanceSource
    | GeneratedView
    | CanonicalSkill
    | GeneratedSkillMirror
    | GovernanceImplementation
    | EvidenceArtifact
    | HistoricalReport
    | ActiveDocumentation
    | UnknownPath

type GovernanceFact =
    | DeveloperClassFact of DeveloperClass
    | ActiveFeatureFact of featureId: string
    | ChangedPathFact of RepoPath
    | PathRoleFact of path: RepoPath * role: PathRole
    | TargetFact of Targets.TargetSpec
    | RouteRuleFact of Routing.RouteRuleFacts
    | MatchedRouteRuleFact of ruleId: RuleId * paths: RepoPath list
    | SelectedTierFact of Tier
    | RequiredGateFact of target: Targets.Target * reason: RuleId
    | ExpectedArtifactFact of artifact: ArtifactId * reason: RuleId
    | ArtifactPresentFact of ArtifactId
    | MissingArtifactFact of ArtifactId * reason: RuleId
    | EvidenceStatusFact of artifact: ArtifactId * status: EvidenceStatus
    | BlockerFact of blockerId: string * reason: string
    | NextActionFact of command: string * reason: string
    | AgentDecisionFact of action: AgentAction * decision: AgentDecision
```

The domain supplies the identity function:

```fsharp
module GovernanceFact =
    val identify: GovernanceFact -> FactId
    val describe: GovernanceFact -> string
```

This avoids an `IFact` interface while still giving every fact a stable ID. It also keeps
facts serializable and pattern-matchable.

### Layer 3: Path Classification

Path classification is the right place for active patterns.

```fsharp
module PathPatterns =
    val normalize: string -> RepoPath

    val (|PublicFsiSurface|_|): RepoPath -> string option
    val (|TemplateContractPath|_|): RepoPath -> unit option
    val (|GovernanceImplementationPath|_|): RepoPath -> unit option
    val (|CanonicalSkillPath|_|): RepoPath -> string option
    val (|GeneratedSkillMirrorPath|_|): RepoPath -> string option
    val (|HistoricalReportPath|_|): RepoPath -> unit option

module PathClassification =
    val classify: RepoPath -> PathRole list
```

Use active patterns where they make policy read like policy:

```fsharp
let classify path =
    match path with
    | PublicFsiSurface packageId -> [ PublicPackageSurface packageId ]
    | TemplateContractPath -> [ TemplateContract ]
    | GovernanceImplementationPath -> [ GovernanceImplementation ]
    | CanonicalSkillPath skillId -> [ CanonicalSkill ]
    | GeneratedSkillMirrorPath skillId -> [ GeneratedSkillMirror ]
    | HistoricalReportPath -> [ HistoricalReport ]
    | _ -> [ UnknownPath ]
```

Do not use active patterns as decoration. If a direct helper or record field is clearer,
use that.

### Layer 4: Rule Modules

Rules are grouped by domain concern. They emit `DerivedFact<GovernanceFact>` values and
attach provenance.

```text
RoutingRules.fs
  changed path -> path role
  path role -> matched route rule
  matched route rules + developer class -> selected tier
  matched route rules -> required gates
  matched route rules -> expected artifacts

ArtifactRules.fs
  expected artifact + present artifact -> missing artifact
  missing artifact -> blocker

EvidenceRules.fs
  evidence graph/audit status -> blockers and next actions

AgentRules.fs
  requested action + route facts + blockers -> allow/deny/needs-evidence
```

Example:

```fsharp
module RoutingRules =
    let classifyPaths : Rule<GovernanceFact> =
        { Id = RuleId.unsafe "route.classify-paths"
          Description = "Classify every changed path into governance path roles."
          Apply =
            fun facts ->
                facts
                |> FactSet.chooseChangedPaths
                |> List.collect (fun path ->
                    PathClassification.classify path
                    |> List.map (fun role ->
                        { Value = PathRoleFact(path, role)
                          Provenance =
                            { Rule = RuleId.unsafe "route.classify-paths"
                              Inputs = [ GovernanceFact.changedPathId path ]
                              Reason = "Changed path matched path classification policy."
                              Source = Some(SourceRef.Rule(RuleId.unsafe "route.classify-paths")) } })) }
```

This is an embedded rule system, but it stays F#. The compiler sees every type and every
case. There is no parser and no custom runtime type system.

### Layer 5: Queries And Explanations

Queries are typed views over the evaluated fact set. They should not run rules, read files,
or execute gates. They only interpret facts.

```fsharp
type RouteScope =
    | WholeWorkspace
    | ExplicitPaths of RepoPath list
    | StagedChanges
    | SinceBase of string

type GovernanceQuery =
    | ExplainRoute of RouteScope
    | ExplainArtifacts of RouteScope
    | ExplainEvidence of RouteScope
    | ExplainNextActions of RouteScope
    | AuthorizeAgentAction of AgentAction * RouteScope

type ExplanationConclusion =
    | SelectedTier of Tier
    | SelectedGate of Targets.Target
    | RequiredArtifact of ArtifactId
    | MissingArtifact of ArtifactId
    | BlocksAction of AgentAction * reason: string
    | AllowsAction of AgentAction * reason: string
    | NextAction of command: string * reason: string

type Explanation =
    { Query: GovernanceQuery
      Summary: string
      Conclusions: ExplanationConclusion list
      Provenance: ProvenanceStep list
      Diagnostics: KernelDiagnostic list }
```

Again, no `IExplanation` interface is needed for the core. A closed union of conclusion
kinds gives better review behavior: adding a new conclusion kind forces renderers and
tests to account for it.

Renderers convert the explanation into stable outputs:

```fsharp
module ExplanationRender =
    val toMarkdown: Explanation -> string
    val toJson: Explanation -> string
```

The JSON surface should use DTO records, not raw F# union serialization, so the external
schema is stable even if the internal union evolves.

### Layer 6: Compatibility And DTO Facades

The existing `AgentValidation` surface exposes string aliases such as
`ValidationGate = string` and contract DTOs. It should not become the new core model.

Keep it as compatibility:

```text
FS.Skia.UI.Build.AgentValidation
  existing generated-product/public contract DTOs
  parse/render JSON
  maps typed Governance.Core results into old DTOs
```

Longer term, `Governance.Core` may expose stable DTOs under:

```text
FS.Skia.UI.Governance.Dto
```

But the internal engine should keep using typed `Targets.Target`, `Tier`, `RuleId`,
`ArtifactId`, `RepoPath`, and `GovernanceFact`.

## Interfaces And Inheritance Decision

### Recommended Rule

Use interfaces for **capability boundaries**, not for the domain model.

Use inheritance for **internal implementation machinery**, not for facts or explanations.

### Good Interface Candidates

Interfaces make sense where binary or host boundaries matter.

```fsharp
type IRuleSet<'fact> =
    abstract Name: string
    abstract SchemaVersion: int
    abstract Identify: 'fact -> FactId
    abstract Rules: Rule<'fact> list
```

Use this only if a host needs to accept independent compiled rule-set providers. The first
implementation can use plain records:

```fsharp
type RuleSet<'fact> =
    { Name: string
      SchemaVersion: int
      Identify: 'fact -> FactId
      Rules: Rule<'fact> list }
```

Interfaces also make sense at the Build edge if test seams become noisy:

```fsharp
type IWorkspaceSnapshotReader =
    abstract ReadChangedPaths: scope: RouteScope -> Result<RepoPath list, KernelDiagnostic list>
    abstract ReadArtifactPresence: ArtifactId list -> Set<ArtifactId>
```

But in F#, records of functions are usually simpler and easier to construct in tests:

```fsharp
type WorkspaceSnapshotReader =
    { ReadChangedPaths: RouteScope -> Result<RepoPath list, KernelDiagnostic list>
      ReadArtifactPresence: ArtifactId list -> Set<ArtifactId> }
```

Prefer records of functions unless binary substitutability is required.

### Poor Interface Candidates

Avoid these:

```fsharp
type IFact =
    abstract Id: FactId
    abstract Kind: string

type IExplanation =
    abstract Summary: string

type IRule =
    abstract Apply: obj list -> obj list
```

These lose the important benefits of the current codebase:

- exhaustive pattern matching
- typed target names
- typed tiers
- typed artifact expectations
- compile-time drift detection
- readable rule code
- simple property tests

They also recreate the worst version of a rule engine: not a real language, not a strong
F# model, and not pleasant to debug.

### Good Inheritance Candidates

Inheritance can be considered later for internal evaluator mechanics if benchmarks show a
problem:

```fsharp
[<AbstractClass>]
type internal WorkItem() =
    abstract Execute: FactStore -> DerivedFact<GovernanceFact> list
```

or an indexed fact-store implementation:

```fsharp
[<AbstractClass>]
type internal FactIndex<'fact>() =
    abstract Add: FactAssertion<'fact> -> bool
    abstract Contains: FactId -> bool
    abstract FactsByKind: string -> FactAssertion<'fact> list
```

Do not add this up front. Plain immutable maps and arrays are enough until profiling says
otherwise.

### Poor Inheritance Candidates

Avoid class hierarchies like:

```fsharp
type Fact = abstract ...
type ChangedPathFact inherit Fact
type RouteRuleFact inherit Fact
type Explanation inherit ...
type RouteExplanation inherit Explanation
```

This makes the system harder to pattern match, harder to serialize predictably, and easier
to extend in ways that bypass exhaustive review. It also makes the kernel feel like a
framework instead of a small typed library.

## Core Data Flow

```text
Build edge
  reads git/files/feature metadata/artifact presence
  normalizes to supplied GovernanceFact list
      |
      v
Governance.Core
  FixedPoint.evaluate
      |
      +--> derived route facts
      +--> derived artifact facts
      +--> derived evidence facts
      +--> derived blocker / next-action / agent-decision facts
      |
      v
Queries
  ExplainRoute
  ExplainArtifacts
  AuthorizeAgentAction
      |
      v
Renderers
  stable text for current Route
  JSON for tools
  Markdown for readiness reports
      |
      v
Build edge
  prints text
  writes artifacts
  decides which FAKE-backed targets to run
```

The core never invokes FAKE. It can recommend `Dev` or `EvidenceAudit`; it cannot run them.

## Concrete Module Plan

### New `build/Governance.Core`

```text
Primitives.fsi / .fs
  RuleId, FactId, ArtifactId, RepoPath, diagnostics, stable ordering helpers.

Inference.fsi / .fs
  FactAssertion, FactSet, Rule, RuleSet, FixedPoint.evaluate, trace.

Explanation.fsi / .fs
  SourceRef, ProvenanceStep, Explanation, explanation DTOs/render helpers.

Targets.fsi / .fs
  Move existing typed target identity and metadata.

PathPatterns.fsi / .fs
  Glob normalization and path active patterns.

PathClassification.fsi / .fs
  PathRole classification.

Routing.fsi / .fs
  Existing route policy plus typed rule facts. Keep old namespace initially if needed.

RouteRules.fsi / .fs
  GovernanceFact-producing fixed-point rules for route selection.

RouteExplain.fsi / .fs
  ExplainRoute query view, JSON/Markdown DTOs.

ContractView.fsi / .fs
  Existing validation.contract.yml rendering from typed route facts.

ArtifactRegistry.fsi / .fs
  Artifact IDs, route-required artifact facts, producer target mapping.

AgentPlanning.fsi / .fs
  AgentAction, AgentDecision, authorization/next-action rules.

Evidence/*
  Move pure evidence schema, parsers, graph, status region, scans, diff scan,
  audit, render, and pure engine.

Skill/Capability/Guidance modules
  Move pure generators/checkers cluster by cluster after route/evidence parity.
```

### Remaining `build/Governance`

```text
Evidence/GeneratedRunner.fsi / .fs
  Stable generated-product reflection facade. Delegates to Core.

Front/*
  Workspace paths, process execution, package/template validation edges,
  report writing, generated scanning, environment classification.

Engine/*
  FAKE target command model, update, interpret.

GeneratedProduct*
  Product instantiation and smoke/evidence execution.

Publish / PrePublish / Preflight
  Process, package, and environment edges.

AgentValidation
  Existing public DTO compatibility facade. Can delegate to Core.
```

### Namespace Strategy

Phase 1 should minimize churn. Moved modules may temporarily keep
`FS.Skia.UI.Build.*` module names inside the `Governance.Core` assembly if that keeps the
diff reviewable.

After parity is proven, add the clearer long-term namespace:

```text
FS.Skia.UI.Governance
FS.Skia.UI.Governance.Inference
FS.Skia.UI.Governance.Domain
FS.Skia.UI.Governance.Evidence
FS.Skia.UI.Governance.Dto
```

Then keep thin compatibility wrappers under `FS.Skia.UI.Build` for any generated-product
or historical API that must remain stable.

## Route Selection Design

The current `Routing.select` should remain behaviorally stable. The new evaluator should
first reproduce it in fact form.

Supplied facts:

```fsharp
DeveloperClassFact FrameworkAuthor
ChangedPathFact (RepoPath.unsafe "src/Scene/Scene.fs")
RouteRuleFact ...
TargetFact ...
```

Derived facts:

```fsharp
PathRoleFact ("src/Scene/Scene.fs", FrameworkImplementation)
SelectedTierFact InnerLoop
RequiredGateFact (Targets.Dev, RuleId.unsafe "route.default-inner-loop")
```

For a package surface change:

```fsharp
PathRoleFact ("src/Scene/Animation.fsi", PublicPackageSurface "FS.Skia.UI.Scene")
MatchedRouteRuleFact ("package-surface", [ "src/Scene/Animation.fsi" ])
SelectedTierFact FocusedAuthority
RequiredGateFact (Targets.PackageSurfaceCheck, "package-surface")
RequiredGateFact (Targets.FsiTranscripts, "package-surface")
RequiredGateFact (Targets.PerPackageSurfaceDiff, "package-surface")
ExpectedArtifactFact ("readiness/package-surface-expectations.md", "package-surface")
```

The old text output remains a renderer over the derived facts:

```text
developer-class=framework-author
tier=focused-authority
gates=PackageSurfaceCheck, FsiTranscripts, PerPackageSurfaceDiff
dogfood-forced=false
matched-rules=package-surface
```

The new JSON output can expose the trace:

```json
{
  "tier": "focused-authority",
  "gates": [
    {
      "name": "PackageSurfaceCheck",
      "reason": "package-surface",
      "matchedPaths": ["src/Scene/Animation.fsi"]
    }
  ],
  "expectedArtifacts": [
    {
      "path": "readiness/package-surface-expectations.md",
      "reason": "package-surface"
    }
  ],
  "provenance": [
    {
      "rule": "route.match.package-surface",
      "inputs": ["changed-path:src/Scene/Animation.fsi"]
    }
  ]
}
```

## Evidence Engine Design

The existing evidence modules are already close to the target shape:

- task parsing is pure over supplied text
- dependency parsing is pure over supplied YAML
- graph cycle detection/topological order/propagation are pure
- audit verdict aggregation is pure
- rendering returns strings to write later

Move them with minimal conceptual change:

```text
EvidenceFormatSchema
TaskParser
DepsParser
SkillRegistry
Graph
StatusRegion
Scans
DiffScan
Audit
Render
Engine
```

Keep this invariant:

```text
read files at Build edge
parse/evaluate/render in Core
write artifacts at Build edge
```

Do not let the evidence core read `tasks.md`, list `readiness/**`, inspect git diffs, or
check file existence itself. Those inputs remain explicit fields on input records or
functions injected by the Build edge.

## Agent Authorization Design

The kernel should be able to answer "may the agent do this?" without executing anything.

```fsharp
type AgentAction =
    | ReadPath of RepoPath
    | EditPath of RepoPath
    | RunTarget of Targets.Target
    | WriteArtifact of ArtifactId
    | CommitChanges
    | PushBranch
    | RequestHumanInput of reason: string

type AgentDecision =
    | Allowed of reason: string
    | Denied of reason: string
    | NeedsEvidence of ArtifactId * reason: string
    | NeedsGate of Targets.Target * reason: string
    | NeedsHuman of reason: string
```

Rules:

- read-only explanation queries are allowed
- generated-view writes are denied and should name the canonical source
- running a FAKE target outside the route-selected gate list is denied
- committing or pushing with missing route-required artifacts is denied or marked
  `NeedsEvidence`
- unknown effectful actions deny by default
- the kernel recommends commands but never executes them

This is where the design becomes useful beyond current governance. Other contexts can
reuse the same action/decision pattern with their own domain fact union.

## Standalone Engine Reuse Path

Do not create a separate standalone package before a second real context exists. Instead:

1. Put the generic inference substrate under `FS.Skia.UI.Governance.Inference`.
2. Keep it free of governance-specific types.
3. Add tests that use a small non-governance fixture domain, for example:

   ```fsharp
   type ToyFact =
       | Number of int
       | Even of int
       | NeedsReview of int
   ```

4. If a second real context adopts it, extract only the substrate into a smaller package:

   ```text
   FS.Skia.UI.RuleKernel
     or
   FS.Skia.RuleKernel
   ```

5. Leave governance facts/rules in `FS.Skia.UI.Governance.Core`.

This prevents premature generalization while keeping the design honest about reuse.

## Testing Strategy

### Core Tests

`Governance.Core.Tests` should cover:

- `RuleId`, `FactId`, `RepoPath` validation and round trips
- path normalization idempotence
- active pattern classification for governance paths
- route tier monotonicity as changed paths are added
- gate de-duplication in target registry order
- default-deny for unmatched non-inner-loop paths
- dogfood override behavior
- generated `validation.contract.yml` parity
- fixed-point idempotence
- fixed-point convergence diagnostics
- every derived fact has non-empty provenance
- every selected gate names a rule or default reason
- every missing artifact names a requiring rule
- evidence graph cycle detection
- topological order respects dependencies
- synthetic propagation monotonicity
- audit verdict aggregation
- agent decision default-deny for unknown/effectful actions
- generated-view write denial with canonical source guidance

### Integration Tests

Keep these outside Core:

- `./fake.sh build -t Route` command behavior
- FAKE target registration and dependency wiring
- route-selected target execution
- report writing
- package packing
- generated-product reflection runner
- generated-product template instantiation
- `FS.Skia.UI.Build.nuspec` dependency on `FS.Skia.UI.Governance.Core`

### Golden Parity

Before and after each movement phase, compare:

- current `Route` text output
- `validation.contract.yml`
- target metadata JSON
- evidence graph JSON/Markdown
- audit hit JSON files
- skill sync generated tree
- capability/API-surface generated docs
- generated-product evidence output

## Implementation Phases

### Phase 0: Baseline

- Run `./fake.sh build -t Route`.
- Run only the printed gates.
- Capture current route/contract/evidence/generated-product parity.
- Document any existing failure before moving files.

### Phase 1: Empty Core Project

- Add `build/Governance.Core/FS.Skia.UI.Governance.Core.fsproj`.
- Add `tests/Governance.Core.Tests`.
- Add a trivial primitive module and smoke test.
- Do not move behavior yet.

### Phase 2: Move Target And Routing Kernel

- Move `Findings`, `Targets`, `Routing`, and `ContractView`.
- Add `PathPatterns` and `PathClassification`.
- Keep route text stable.
- Move pure tests into `Governance.Core.Tests`.
- Keep FAKE command tests in `Governance.Tests`.

### Phase 3: Add Inference Substrate

- Add primitive IDs, provenance, `FactSet`, `Rule`, `FixedPoint.evaluate`.
- Add toy-domain tests to prove substrate reuse.
- Add governance route rules that derive facts equivalent to `Routing.select`.
- Keep `Routing.select` as the compatibility function until the fact-based route is
  byte-compatible.

### Phase 4: Move Evidence Core

- Move pure evidence modules cluster by cluster.
- Keep `GeneratedRunner` in `FS.Skia.UI.Build`.
- Prove graph/audit artifact parity.

### Phase 5: Move Skill/Capability/Generated-View Pure Logic

- Move only pure portions.
- Split mixed modules before moving.
- File reads and writes stay in Build.
- Add path-role tests for generated/canonical/historical distinctions.

### Phase 6: Add Query/Explanation APIs

- Add `ExplainRoute`, `ExplainArtifacts`, `ExplainEvidence`, and `ExplainNextActions`.
- Add JSON DTOs and Markdown renderers.
- Wire `Route --json` through the Build edge.
- Keep default `Route` text unchanged.

### Phase 7: Add Agent Authorization

- Add `AgentAction` and `AgentDecision`.
- Deny unsafe/effectful actions by default.
- Surface missing evidence/gates as explicit decisions.
- Keep the kernel side-effect free.

### Phase 8: Package And Generated Product Verification

- Pack Core and Build.
- Inspect `FS.Skia.UI.Build.nuspec`.
- Instantiate generated products.
- Confirm the generated-product evidence runner reflection path still works.

### Phase 9: Namespace Cleanup

- Add `FS.Skia.UI.Governance.*` namespaces.
- Keep compatibility wrappers where needed.
- Avoid combining namespace cleanup with behavior movement.

## Acceptance Criteria

The split is complete when:

- `FS.Skia.UI.Governance.Core` exists and is packable.
- `FS.Skia.UI.Build` references Core and keeps FAKE/effect edges.
- Core has no FAKE/process/live-repository dependency.
- Existing route text output is unchanged.
- `validation.contract.yml` remains generated from typed route facts.
- Evidence graph/audit outputs are byte-compatible for fixtures.
- Every derived fact used in route/explain/agent decisions has provenance.
- The fixed-point evaluator is deterministic, idempotent, and bounded.
- Generated products still reference only `FS.Skia.UI.Build`.
- `FS.Skia.UI.Build.nupkg` restores Core transitively.
- `GeneratedRunner.run` remains available from `FS.Skia.UI.Build`.
- Active docs explain the split.
- Route-selected gates pass in the order printed by `Route`.

## Final Recommendation

Use F# types as the rule language. Build a small fixed-point inference substrate with
nominal IDs, immutable fact assertions, deterministic rule execution, and provenance-rich
explanations. Keep facts and conclusions as closed discriminated unions in each domain.
Use active patterns for classification. Use records of functions for simple test seams.
Use interfaces only when binary interop or independently compiled rule-set providers are
actually needed. Reserve inheritance for hidden optimized evaluator internals if profiling
justifies it.

That design gives the system "robust substance" without creating an unsupported language.
It also follows the useful part of Hopac's shape: a compact F# surface backed by carefully
bounded internals, with the semantics documented and tested independently from the
implementation mechanics.
