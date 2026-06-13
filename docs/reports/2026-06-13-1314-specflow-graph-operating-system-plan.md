---
title: SpecFlow Graph Operating System Implementation Plan
---

# SpecFlow Graph Operating System Implementation Plan

- **Timestamp:** 2026-06-13T13:14:00+02:00
- **Author:** Codex
- **Status:** Proposed complete redesign
- **Scope:** Replace the Spec Kit-shaped artifact workflow with a repo-native,
  typed F# feature graph. No backward compatibility with existing agent
  workflows, `.specify` scripts, phase command names, or hand-authored
  `tasks.md` / `tasks.deps.yml` semantics is required.
- **Constraint for this report:** No governance machinery was run for this
  report. Research used source inspection, upstream Spec Kit release/catalog
  data, official Spec Kit extension documentation, the community extension
  catalog, and official FSharp.Formatting content guidance.

## Executive Decision

Build a new **SpecFlow Graph Operating System** inside `FS.Skia.UI.Build`.

The feature graph becomes the single authoritative state model for:

- feature identity and lifecycle;
- requirements, success criteria, scenarios, and unsupported scope;
- design decisions, research claims, and source citations;
- implementation tasks, dependencies, ownership, status, and completion proofs;
- evidence artifacts, command runs, generated outputs, approvals, and synthetic
  disclosures;
- route expectations, actual diff classification, required gates, and gate
  results;
- agent context packs and phase-specific file-reading budgets.

Markdown remains useful, but not authoritative. `spec.md`, `plan.md`, `tasks.md`,
readiness reports, Mermaid diagrams, and context prompts become deterministic
projections from the graph. If a projection is edited by hand, the next graph
check fails and names the stale projection. The graph owns the state.

The redesign is intentionally breaking:

1. Delete the active `.specify` runtime shape instead of wrapping it.
2. Retire upstream Spec Kit version and integration metadata as active inputs.
3. Replace `speckit-*` phase skills with graph-aware `specflow-*` skills or
   a single generated context-pack skill.
4. Replace hand-authored `tasks.deps.yml` with graph-owned dependencies.
5. Replace task checkboxes as source of truth with evidence-backed graph
   task completion.
6. Replace readiness file discovery as the audit source with a structured
   evidence index.

The destination is not a marketplace, workflow engine, or generic Spec Kit clone.
It is a repo-specific specification and evidence operating system for
FS.Skia.UI.

## Research Summary

### Upstream Spec Kit

The current upstream Spec Kit release checked for this report is `v0.10.2`,
published 2026-06-11. The release notes show the install path is still a Python
tool via `uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.10.2`.
Relevant release details:

- `v0.10.0` moved toward explicit integrations and removed legacy AI flags.
- `v0.10.0` made the Git extension opt-in.
- `v0.10.0` added per-event hook lists with priorities.
- `v0.10.1` added integration status and catalog payload validation.
- `v0.10.2` added `category` and `effect` as first-class extension fields and
  expanded the community catalog.
- The upstream `speckit` workflow remains a generic sequence:
  `specify -> review-spec -> plan -> review-plan -> tasks -> implement`.

Conclusion: upstream is optimizing a generic multi-project, multi-agent
installer/runtime. This repository should not carry that generality. It already
has a compiled F# governance engine, typed FAKE targets, route selection, and
evidence audits.

### Extension Catalog

The upstream extension README makes the important trust model explicit:
community extensions are independently maintained; maintainers verify catalog
entry shape, not extension code. Organizations are expected to curate their own
catalog if they want trusted extension availability.

The raw community catalog currently contains 113 entries. The observed metadata
shape is useful:

| Field | Observation |
|---|---|
| `category` | Mostly `process`, `docs`, `code`, `integration`, `visibility`; two entries currently lack category. |
| `effect` | Mostly `read-write`; some `read-only`; two entries currently lack effect. |
| `provides.commands` / `provides.hooks` | Useful for status and health reporting. |

Representative patterns worth absorbing locally:

| Catalog idea | Local interpretation |
|---|---|
| Project Status / Status Report / Doctor | Graph-derived status and health commands. |
| Spec Trace / Verify / Verify Tasks | Requirement-to-task-to-evidence traceability and phantom-completion detection. |
| Plan Review Gate / Spec Validate / Staff Review | Structured approval artifacts, not vague prose. |
| Architecture Guard / DocGuard / Coding Standards Drift | Deterministic drift checks over repo-owned sources. |
| Token Budget / Token Analyzer | Phase context packs and file-reading budgets. |
| Worktrees / Worktree Isolation | Worktree-first feature execution with isolated cache state. |
| Research Harness / Version Guard | Typed research claims with URLs, retrieval dates, and checked versions. |

Conclusion: the catalog validates the capability taxonomy, not the plugin
runtime. We should implement capabilities as local graph modules, not install
third-party packages.

### FSharp.Formatting

Official FSharp.Formatting content guidance confirms that `docs/**/*.md` is
processed as content and frontmatter is optional. It also confirms that
frontmatter controls navigation metadata such as `title`, `category`,
`categoryindex`, `index`, `description`, and `keywords`. This report keeps the
existing local report convention: minimal frontmatter with `title`, because
`docs/reports/**` already follows that pattern.

### Local Repository Inventory

The local codebase already contains most of the mechanics needed for a graph
operating system:

| Existing module | Current responsibility | Reuse in redesign |
|---|---|---|
| `build/Governance/Engine/Model.fs` | Build model, active feature resolution, effect DU. | Extend with graph effects and paths, then delete `.specify` active-feature resolution. |
| `build/Governance/Engine/Update.fs` | Pure target-to-effect decision function. | Keep the pure/effect boundary for graph projection, graph validation, and graph mutation commands. |
| `build/Governance/Evidence/TaskParser.fs` | Parses `tasks.md` checkboxes, task IDs, skillist mirrors, synthetic metadata. | Mine for migration/import only; replace checkbox authority with graph task status. |
| `build/Governance/Evidence/DepsParser.fs` | Parses `tasks.deps.yml`. | Replace with graph dependency fields; keep parser only for one-time import or historical tooling. |
| `build/Governance/Evidence/Graph.fs` | Builds task DAG, detects cycles, topological sort, propagates synthetic taint. | Promote the algorithm into graph validation over `FeatureGraph.Tasks`. |
| `build/Governance/Evidence/Render.fs` | Renders task graph JSON, Markdown, Mermaid. | Reuse rendering style, but render from graph-owned state. |
| `build/Governance/Evidence/Audit.fs` | Merges task/deps, validates skills, owns vocabulary, synthetic disclosures, readiness scans. | Split into reusable invariant modules against typed graph data. |
| `build/Governance/Evidence/Scans.fs` | Scans readiness files for required evidence formats. | Replace most scans with structured evidence rows; keep selected scanners as projection sanity checks. |
| `build/Governance/SymbolCrossCheck.fs` | Extracts FR/SC and structural symbols across plan/data-model/tasks. | Generalize into graph traceability checks. |
| `build/Governance/Routing.fs` | Selects tier and gates from working-tree diff. | Add graph-declared impact planning and compare declared route to actual diff route. |
| `build/Governance/TargetMetadata.fs` | Validates runnable target metadata and renders JSON/Markdown. | Attach target metadata to graph route expectations and gate result evidence. |
| `build/Governance/Guidance.fs` | Checks generated guidance obligations. | Replace template prose checks with generated context-pack checks. |
| `SkillTreeGen` / `SkillSync` | `.agents` canonical, `.claude` generated. | Keep generation mechanism if needed, but replace `speckit-*` skills with `specflow-*` skills. |

The local artifact set also exposes the current design problem. In feature 116,
`tasks.md` currently shows tasks marked `[X]`, while
`readiness/task-graph.json` can still contain `declared: "pending"` if the
projection has not been regenerated. That is exactly the failure mode the
redesign should remove: a generated readiness artifact cannot be allowed to
compete with the current source artifact for authority.

### Current Spec Kit Coupling To Delete

The active coupling to delete or archive:

- `.specify/scripts/bash/common.sh`
- `.specify/scripts/bash/create-new-feature.sh`
- `.specify/scripts/bash/setup-plan.sh`
- `.specify/scripts/bash/setup-tasks.sh`
- `.specify/scripts/bash/check-prerequisites.sh`
- `.specify/extensions/**`
- `.specify/presets/**`
- `.specify/templates/**`
- `.specify/workflows/**`
- `.specify/init-options.json`
- `.specify/integration.json`
- `.specify/integrations/**`
- `.specify/feature.json`
- `.agents/skills/speckit-*` as active guidance
- generated `.claude/skills/speckit-*` peers

The replacement can keep `.specify/memory/constitution.md` only if it is renamed
or imported into a repo-owned governance source.

## Corrected Assumptions

The initial radical sketch was directionally right but too casual in several
places. Research corrected these assumptions:

1. **Do not build a second task graph parser.** The repo already has cycle
   detection, topo sort, synthetic propagation, skill registry validation, and
   Mermaid rendering. The redesign should promote those algorithms, not rewrite
   them.
2. **Do not keep Markdown as editable workflow state.** Keeping hand-editable
   Markdown while adding a graph creates two authorities. Markdown must become a
   projection.
3. **Do not preserve upstream extension semantics.** `category` and `effect`
   are useful metadata; external extension installation is not.
4. **Do not make `tasks.md` checkboxes authoritative.** Completion must be
   evidence-backed and graph-owned.
5. **Do not make approvals prose-only.** Review gates need structured approval
   artifacts with scope, author, time, and graph hash.
6. **Do not cache verdicts.** Cache parsed inputs and projections only. Gate
   results and audit verdicts are evidence events, not reusable truths.
7. **Do not derive active feature from git branch.** Feature activation is an
   explicit graph/workspace state.

## Target Architecture

Add a new `SpecFlow` subsystem under the governance build project:

```text
build/Governance/SpecFlow/
  GraphModel.fs
  GraphJson.fs
  GraphSchema.fs
  GraphHash.fs
  GraphValidation.fs
  GraphMutation.fs
  GraphImport.fs
  Projection.fs
  ProjectionCheck.fs
  RequirementTrace.fs
  EvidenceIndex.fs
  RoutePlanning.fs
  GateEvidence.fs
  Approval.fs
  ResearchClaims.fs
  ContextPack.fs
  Workspaces.fs
  CommandModel.fs
  CommandRender.fs
  CommandInterpret.fs
```

The modules follow the existing build engine rule:

- pure modules compute plans, mutations, projections, diagnostics, and reports;
- interpreter modules perform filesystem, git, process, and console IO;
- command output supports `--json`, `--plain`, and human-rich modes;
- generated files are deterministic and do not contain wall-clock timestamps
  unless the timestamp is an evidence event explicitly committed to the graph.

## Authority Model

### Single Source

Each active feature has one canonical graph:

```text
specs/<feature-id>/feature.graph.json
```

The active feature pointer becomes:

```text
.specflow/current.json
```

`feature.graph.json` is the only authored workflow state file. The graph may
embed Markdown strings for long-form prose, but those strings are fields in a
typed schema, not free-floating files with implied semantics.

### Generated Projections

The following become generated projections:

```text
specs/<feature-id>/spec.md
specs/<feature-id>/plan.md
specs/<feature-id>/tasks.md
specs/<feature-id>/readiness/index.md
specs/<feature-id>/readiness/task-graph.md
specs/<feature-id>/readiness/task-graph.json
specs/<feature-id>/readiness/traceability.md
specs/<feature-id>/readiness/context/<phase>.json
specs/<feature-id>/readiness/context/<phase>.md
```

`tasks.deps.yml` is deleted as an active artifact. If a YAML dependency view is
still useful for review, it is generated as:

```text
specs/<feature-id>/readiness/task-deps.generated.yml
```

Projection files carry a small generated header:

```text
<!-- GENERATED FROM feature.graph.json sha256:<hash>; DO NOT EDIT -->
```

`SpecFlowProjectionCheck` recomputes every projection and fails on drift.

### Evidence State

Evidence is stored as structured graph data, not inferred from arbitrary files.
Evidence may point to files, logs, images, generated outputs, and command
transcripts, but the graph row is authoritative.

Example:

```json
{
  "id": "EV-0027",
  "kind": "gate-run",
  "gate": "Dev",
  "command": "./fake.sh build -t Dev",
  "status": "pass",
  "authoritative": true,
  "started_at_utc": "2026-06-13T10:22:11Z",
  "completed_at_utc": "2026-06-13T10:24:03Z",
  "log_paths": [
    "specs/116-paint-cache-damage-rects/readiness/logs/test.txt"
  ],
  "covers": {
    "requirements": ["FR-014", "SC-007"],
    "tasks": ["T026"]
  }
}
```

The file can be missing only if the evidence row says `status = missing` or
`status = external`. A pass row pointing at a missing file is a graph error.

## Graph Schema

Use strict JSON with deterministic field ordering on write. F# owns the schema;
JSON Schema may be generated for editor support, but the compiled F# validator is
the authority.

### Top-Level Shape

```fsharp
type FeatureGraph =
  { SchemaVersion: SchemaVersion
    GraphId: string
    Feature: FeatureHeader
    Requirements: Requirement list
    Scenarios: Scenario list
    Decisions: Decision list
    ResearchClaims: ResearchClaim list
    Impact: ImpactModel
    Tasks: TaskNode list
    Evidence: EvidenceItem list
    Approvals: Approval list
    Route: RouteState
    Projections: ProjectionState
    ContextPacks: ContextPackSpec list
    Lifecycle: LifecycleState }
```

### Feature Header

```fsharp
type FeatureHeader =
  { Id: string
    Slug: string
    Title: string
    BranchName: string option
    Workspace: WorkspaceState
    CreatedAtUtc: DateTimeOffset
    Status: FeatureStatus
    SourcePrompt: string option
    Owners: string list }

type FeatureStatus =
  | Draft
  | Planned
  | Tasked
  | Implementing
  | ReadyForReview
  | Merged
  | Archived
```

`CreatedAtUtc` is allowed here because feature creation is a real event. It is
not regenerated.

### Requirements

```fsharp
type RequirementKind =
  | Functional
  | SuccessCriterion
  | NonFunctional
  | Constraint
  | UnsupportedScope

type Requirement =
  { Id: string
    Kind: RequirementKind
    Priority: int option
    Text: string
    Rationale: string option
    Acceptance: AcceptanceCheck list
    Parent: string option
    Tags: string list
    Status: RequirementStatus }

type AcceptanceCheck =
  { Id: string
    Text: string
    EvidenceKinds: EvidenceKind list
    Required: bool }
```

Rules:

- Requirement IDs are stable and unique.
- `FR-###` and `SC-###` remain acceptable display IDs, but the graph does not
  infer meaning from prose alone.
- Unsupported scope is represented explicitly, not buried in plan prose.
- Each buildable requirement must link to at least one task or be explicitly
  deferred.

### Decisions

```fsharp
type Decision =
  { Id: string
    Title: string
    Context: string
    Decision: string
    Consequences: string list
    Alternatives: Alternative list
    Supersedes: string list
    AppliesTo: string list }
```

Design decisions replace scattered plan paragraphs when a choice matters for
future maintenance.

### Research Claims

```fsharp
type ResearchClaim =
  { Id: string
    Claim: string
    Source: ResearchSource
    RetrievedAtUtc: DateTimeOffset option
    CheckedVersion: string option
    Confidence: ClaimConfidence
    UsedBy: string list }

type ResearchSource =
  | Url of string
  | LocalPath of string
  | CommandOutput of evidenceId: string
```

Rules:

- Current-version claims need `Url`, `RetrievedAtUtc`, and `CheckedVersion`.
- Network lookups are never part of deterministic validation gates.
- Research claims can be stale; the validator reports staleness by policy, not
  by silently refreshing the internet.

### Impact Model

```fsharp
type ImpactModel =
  { RuntimePackages: string list
    PublicSurfaces: SurfaceImpact list
    Templates: TemplateImpact
    GeneratedProducts: GeneratedProductImpact
    Governance: GovernanceImpact
    Docs: DocsImpact
    Risk: RiskLevel
    DeclaredChangedPaths: string list
    ExpectedRoute: ExpectedRoute option }
```

The impact model makes route expectations visible before implementation.

Rules:

- A public `.fsi` change must be declared before implementation tasks can be
  completed.
- A generated-template change must declare generated-product impact.
- A governance/build-path change must declare governance impact.
- `RoutePlanning` compares declared impact to actual git diff classification.

### Tasks

```fsharp
type TaskNode =
  { Id: string
    Title: string
    Body: string option
    Phase: string
    Story: string option
    Status: TaskStatus
    Dependencies: string list
    Parallel: bool
    SkillIds: string list
    Owns: EvidenceOwnership list
    ExpectedChanges: ExpectedChange list
    Covers: TraceLink list
    Completion: CompletionProof option }

type TaskStatus =
  | Pending
  | InProgress
  | Done
  | Skipped of reason: string
  | Failed of reason: string
  | Deferred of reason: string
  | SyntheticDone of reason: string

type CompletionProof =
  { CompletedAtUtc: DateTimeOffset
    EvidenceIds: string list
    CommitIds: string list
    Notes: string option }
```

Rules:

- `Done` requires at least one evidence row or an explicit `no-evidence-required`
  reason for administrative tasks.
- `SyntheticDone` is distinct from `Done`; synthetic taint still propagates.
- Task completion cannot be represented by editing `[X]` in Markdown.
- A task that owns graph validation or audit evidence must cite the matching
  evidence row.
- A task that declares `ExpectedChanges` but has no matching git diff or commit
  evidence is suspicious and reported.

### Evidence

```fsharp
type EvidenceItem =
  { Id: string
    Kind: EvidenceKind
    Status: EvidenceStatus
    Authoritative: bool
    Synthetic: SyntheticClass option
    Paths: string list
    Command: string option
    Gate: string option
    Environment: EvidenceEnvironment option
    Covers: TraceLink list
    CreatedAtUtc: DateTimeOffset option
    Notes: string option }

type EvidenceKind =
  | GateRun
  | TestRun
  | FsiTranscript
  | SurfaceBaseline
  | GoldenFile
  | GeneratedProduct
  | Screenshot
  | Research
  | ApprovalEvidence
  | ManualObservation

type EvidenceStatus =
  | Pass
  | Fail
  | Missing
  | NotApplicable
  | Deferred
```

Rules:

- Evidence rows are typed.
- Evidence files are attachments, not semantic sources.
- `Authoritative = true` means the row can satisfy a requirement.
- Non-authoritative aggregate logs can be stored but cannot satisfy a blocking
  acceptance check unless a policy explicitly allows it.

### Approvals

```fsharp
type Approval =
  { Id: string
    Scope: ApprovalScope
    Decision: ApprovalDecision
    Reviewer: string
    ApprovedAtUtc: DateTimeOffset
    GraphHash: string
    EvidenceIds: string list
    Notes: string option }
```

Rules:

- Approval applies to a graph hash. If the graph changes, the approval becomes
  stale unless its scope is explicitly still valid.
- Approval can be required for high-risk or public-surface features.
- AI review is advisory unless represented as an approval by a human or policy.

### Route State

```fsharp
type RouteState =
  { Declared: ExpectedRoute option
    LastActual: ActualRoute option
    RequiredGates: string list
    GateEvidence: string list
    Drift: RouteDrift list }
```

`RouteState` makes validation obligations visible in the graph. It does not
replace the actual `Route` target; it records expectations and actual evidence.

## Command Surface

Create a new root launcher:

```bash
./specflow <command> [args]
```

No `speckit` aliases are required.

### Feature Lifecycle

```bash
./specflow new "paint cache damage rects" --id 116-paint-cache-damage-rects
./specflow activate specs/116-paint-cache-damage-rects
./specflow status
./specflow archive 116-paint-cache-damage-rects
```

`new` creates `feature.graph.json` and projections. It does not call upstream
Spec Kit.

### Graph Editing

The first implementation can support structured commands for the common graph
mutations:

```bash
./specflow requirement add FR-001 --kind functional --text "..."
./specflow requirement defer FR-008 --reason "..."
./specflow decision add DEC-001 --title "..."
./specflow task add T001 --phase setup --covers FR-001 --title "..."
./specflow task depend T009 T008
./specflow task complete T009 --evidence EV-004 --commit HEAD
./specflow evidence add-gate EV-026 --gate Dev --status pass --log readiness/logs/test.txt
./specflow approval add plan-reviewed --scope plan --reviewer <name>
```

Direct graph edits are allowed for bulk work, but the validator is strict and
the projection check catches drift.

### Projection

```bash
./specflow project
./specflow project --check
./specflow graph validate
./specflow graph explain --json
./specflow graph mermaid
```

`project --check` is the replacement for stale Markdown/YAML readiness checks.

### Context Packs

```bash
./specflow context --phase specify --json
./specflow context --phase plan --json
./specflow context --phase tasks --json
./specflow context --phase implement --json
./specflow context --task T012 --json
```

Context packs include:

- graph summary;
- required files to read;
- files not to read unless needed;
- relevant skills;
- route expectations;
- active blockers;
- allowed mutations;
- required evidence shape;
- token estimate if available.

This replaces long agent prompt prose as the primary operational surface.

### Health And Traceability

```bash
./specflow doctor --json
./specflow trace --json
./specflow route-plan --json
./specflow route-actual --json
./specflow evidence status --json
./specflow approvals status --json
```

These commands are read-only except where explicitly named otherwise.

### Workspaces

```bash
./specflow workspace create --layout sibling
./specflow workspace create --layout nested
./specflow workspace status
./specflow workspace dispose
```

Workspaces are graph-owned. The workspace state records:

- git worktree path;
- branch;
- cache namespace;
- FAKE state namespace or confirmed shared-state policy;
- active feature graph path.

## Agent Model

Delete the active `speckit-*` skills and generate a smaller set:

```text
.agents/skills/specflow-author/SKILL.md
.agents/skills/specflow-plan/SKILL.md
.agents/skills/specflow-task/SKILL.md
.agents/skills/specflow-implement/SKILL.md
.agents/skills/specflow-review/SKILL.md
.agents/skills/specflow-evidence/SKILL.md
```

Each skill becomes thin:

1. Run the matching `./specflow context --phase ... --json`.
2. Read only the files listed in the context pack unless local discovery shows a
   clear need.
3. Mutate the graph through `./specflow` commands or direct graph edits.
4. Run `./specflow project`.
5. Do not hand-edit generated projections.

The skill prose should not duplicate the governance policy. The graph and
context-pack generator are the policy source.

`.claude` mirroring can be retained if the repo still wants Claude support, but
it is generated from the new `specflow-*` skills.

## Build Targets

Add or replace targets in `Targets.fs`:

```fsharp
| SpecFlowGraphCheck
| SpecFlowProjectionCheck
| SpecFlowTraceCheck
| SpecFlowContextCheck
| SpecFlowAudit
```

Recommended mapping:

| Old target | New target |
|---|---|
| `EvidenceGraph` | `SpecFlowGraphCheck` |
| `EvidenceAudit` | `SpecFlowAudit` |
| `GeneratedGuidanceCheck` | `SpecFlowContextCheck` plus projection checks |
| `PhaseHookParityCheck` | Delete or replace with `SpecFlowContextCheck` |
| `SkillSyncCheck` | Keep only if `.claude` mirroring remains |
| `TargetMetadataDrift` | Keep, and add graph target references |

The old targets may be removed once the new graph targets exist. No aliasing is
required.

## Validation Invariants

### Graph Validity

- Graph schema version is supported.
- Every ID is unique within its namespace.
- Every dependency references an existing task.
- The task graph is acyclic.
- Every task has a phase.
- Every `Done` task has completion proof.
- Every completion proof references existing evidence.
- Every evidence path exists unless explicitly `Missing`, `Deferred`,
  `NotApplicable`, or `External`.
- Every buildable requirement is covered by a task or explicit deferral.
- Every required acceptance check is covered by authoritative evidence or
  explicit deferral.
- Synthetic completion propagates through real dependencies.
- Approval graph hashes are current or reported stale.

### Projection Validity

- Every generated projection matches the current graph.
- No projection contains stale graph hash.
- No hand-authored `tasks.deps.yml` exists in active feature directories.
- No active `spec.md`, `plan.md`, or `tasks.md` lacks the generated header.

### Traceability

- Requirement -> task -> evidence paths are complete.
- Tests and goldens can be linked to requirements.
- Orphan evidence is reported.
- Orphan tests are advisory unless a policy makes them blocking.
- Tasks marked done without implementation/evidence are blockers.

### Route Consistency

- Declared impact selects an expected route before implementation.
- Actual working-tree diff route is captured after implementation.
- Declared and actual route mismatch is blocking unless explicitly waived.
- Route-required gates have corresponding evidence rows.
- Gate evidence rows reference logs.
- Logs for authoritative pass evidence exist.

### Research Honesty

- Current-version claims have URL, retrieved date, and checked version.
- Local source claims cite local paths or evidence IDs.
- Network-dependent claims cannot be required by deterministic gates.

### Context-Pack Validity

- Each phase has a context pack.
- Context packs list graph, projections, relevant source files, skills, and
  forbidden/generated files.
- Large artifacts are flagged.
- Context packs are deterministic over the graph and repository snapshot.

## Projection Design

### `spec.md`

Generated from:

- `FeatureHeader`
- `Requirements`
- `Scenarios`
- `UnsupportedScope`
- selected `ResearchClaims`

It should read like the current feature specs, but the graph owns all IDs and
requirement text.

### `plan.md`

Generated from:

- `ImpactModel`
- `Decisions`
- `RouteState.Declared`
- `ResearchClaims`
- planned evidence obligations
- implementation stages derived from tasks

The current "Repository Governance Decisions" section should become graph data.
The projection can still render it for review.

### `tasks.md`

Generated from `TaskNode list`.

Rules:

- Checkbox status comes from `TaskStatus`.
- Dependencies are rendered inline for humans only.
- Skill lists are rendered from graph `SkillIds`.
- Synthetic disclosures are generated from synthetic evidence rows.
- The file is never edited by agents.

### Readiness

Replace ad-hoc readiness scattering with an index:

```text
readiness/index.md
readiness/evidence.json
readiness/traceability.md
readiness/task-graph.md
readiness/context/*.json
readiness/context/*.md
readiness/logs/**
readiness/attachments/**
```

Specialized readiness files can exist when they carry meaningful narrative, but
they must be referenced by an `EvidenceItem`.

## Destructive Migration Plan

Because backward compatibility is irrelevant, use a clean cut.

### Delete Active Spec Kit Runtime

Remove:

```text
.specify/scripts/**
.specify/extensions/**
.specify/presets/**
.specify/templates/**
.specify/workflows/**
.specify/integration.json
.specify/integrations/**
.specify/init-options.json
.specify/feature.json
```

Replace with:

```text
.specflow/current.json
.specflow/config.json
.specflow/schema/feature-graph.schema.json
```

### Replace Skills

Delete or archive active:

```text
.agents/skills/speckit-*
.claude/skills/speckit-*
```

Add:

```text
.agents/skills/specflow-*
```

Regenerate `.claude` only if still desired.

### Replace Feature Artifacts

For active and future features:

- add `feature.graph.json`;
- regenerate `spec.md`, `plan.md`, `tasks.md`;
- delete `tasks.deps.yml` or move it to generated readiness if needed;
- replace readiness semantic files with `readiness/evidence.json` plus
  generated reports.

Historical feature directories can be left alone initially if they are not
active. The graph tools should operate on the active feature unless explicitly
asked to import history.

## Implementation Stages

### Stage 0 - Commit The Breaking Decision

Deliverables:

- This report.
- ADR: "Feature graph is the workflow authority".
- Update AGENTS.md to stop mentioning `.specify` scripts and `speckit-*`
  commands.
- Decide whether `.claude` remains a generated mirror.

Exit criteria:

- Maintainers accept deletion of old Spec Kit surfaces.
- No compatibility window is expected.

### Stage 1 - Add Core Graph Model

Deliverables:

- `SpecFlow/GraphModel.fs(i)` with the core records and discriminated unions.
- `SpecFlow/GraphJson.fs` deterministic parser/writer.
- `SpecFlow/GraphHash.fs` canonical hash computation.
- `SpecFlow/GraphSchema.fs` generated JSON Schema output.
- Golden fixture for a minimal feature graph.

Tests:

- JSON round-trip is stable.
- Hash is stable over field-order normalization.
- Unknown schema version fails clearly.
- Duplicate IDs fail clearly.

Design constraints:

- No runtime reflection for schema behavior.
- No generated F# quotations.
- Keep JSON formatting deterministic and reviewable.

### Stage 2 - Add Graph Validation Kernel

Deliverables:

- `GraphValidation.fs`.
- Port task DAG cycle/toposort/synthetic propagation from
  `Evidence/Graph.fs` to operate on `TaskNode`.
- Port owns/skill validation from `Audit.fs` to graph task fields.
- Add requirement coverage validation.
- Add evidence path validation.
- Add approval freshness validation.

Tests:

- Acyclic graph passes.
- Cycle fails with named cycle.
- Dangling dependency fails.
- Done task without completion proof fails.
- Completion proof with missing evidence fails.
- Synthetic taint propagates through real dependencies.
- Accepted synthetic deferral stops taint only when approval is current.

### Stage 3 - Add Projection Generator

Deliverables:

- `Projection.fs` renders `spec.md`, `plan.md`, `tasks.md`, readiness index,
  task graph JSON, task graph Markdown, and Mermaid.
- `ProjectionCheck.fs` recomputes and diffs projections.
- Generated header format with graph hash.

Tests:

- Golden projections for the minimal fixture.
- Projection check passes immediately after generation.
- Hand-edited projection fails with a small diff.
- Projection output does not include nondeterministic timestamps.

### Stage 4 - Add `specflow` CLI

Deliverables:

- Root `specflow` launcher.
- `build/SpecFlow.Tool/SpecFlow.Tool.fsproj` or equivalent command path.
- Commands:
  - `new`
  - `activate`
  - `status`
  - `graph validate`
  - `project`
  - `project --check`
  - `trace --json`
  - `context --phase`

Tests:

- CLI returns pure JSON in `--json` mode.
- Non-TTY plain output contains no ANSI.
- Status reads `.specflow/current.json`.
- Missing active feature fails with clear next command.

### Stage 5 - Import One Active Feature And Cut Over

Deliverables:

- `GraphImport.fs` imports one active feature from current
  `spec.md` / `plan.md` / `tasks.md` / `tasks.deps.yml`.
- Import is one-way and allowed to be lossy only when it reports every lost
  field.
- Generate `feature.graph.json` for the active feature.
- Regenerate projections from the graph.
- Delete active `tasks.deps.yml`.

Tests:

- Import feature 116 fixture.
- Imported task count matches source.
- Imported dependencies match `tasks.deps.yml`.
- Imported requirements include every FR/SC found by symbol extraction.
- Projection check passes after cutover.

Important:

This is not compatibility support. The importer exists to bootstrap the new
authority from the current active feature, then can be removed or left as a
historical tool.

### Stage 6 - Replace Evidence Graph And Audit Targets

Deliverables:

- Add `SpecFlowGraphCheck`.
- Add `SpecFlowAudit`.
- Wire them into `Targets.fs`, `TargetMetadata.fs`, and `Routing.fs`.
- Remove or stop routing old `EvidenceGraph` / `EvidenceAudit`.
- Write graph reports to:
  - `readiness/task-graph.md`
  - `readiness/task-graph.json`
  - `readiness/evidence.json`
  - `readiness/traceability.md`

Tests:

- New graph check catches every old graph fixture class.
- New audit catches missing readiness/evidence path rows.
- Route selects new graph/audit targets for graph and evidence changes.
- Old target names no longer appear in validation contract unless deliberately
  retained.

### Stage 7 - Add Route Planning To The Graph

Deliverables:

- `RoutePlanning.fs`.
- Graph-declared impact to expected route selection.
- Actual git diff route capture.
- Drift diagnostics:
  - undeclared public surface change;
  - declared public surface but no actual diff;
  - missing expected gate evidence;
  - gate evidence for gate not selected by route;
  - dogfood escalation mismatch.

Tests:

- Public `.fsi` declared impact selects surface gates.
- Template declared impact selects template/generated gates.
- Governance impact selects graph/audit gates.
- Actual diff mismatch is blocking.
- Expected gates render in `plan.md` projection.

### Stage 8 - Add Evidence Command Rows

Deliverables:

- `GateEvidence.fs`.
- Commands:
  - `evidence add-gate`
  - `evidence add-file`
  - `evidence add-research`
  - `evidence add-manual`
  - `task complete`
- Optional helper to run a gate and record evidence:
  - `specflow gate run Dev`

Tests:

- Gate evidence row with missing log fails.
- Gate evidence row for failing command cannot satisfy requirement.
- Task completion records evidence and graph hash.
- Re-running projection updates task status in generated `tasks.md`.

### Stage 9 - Add Context Packs And New Skills

Deliverables:

- `ContextPack.fs`.
- Generated `specflow-*` skills.
- Delete active `speckit-*` skills.
- `specflow context --phase` returns:
  - graph hash;
  - active route expectations;
  - current blockers;
  - required reads;
  - optional reads;
  - generated files not to edit;
  - allowed graph mutations;
  - required evidence shape.

Tests:

- Each phase has a context pack.
- Context pack includes the active graph and excludes stale projections as
  authority.
- Generated skills mention `specflow context`.
- No active skill references `.specify/scripts`.
- No active skill instructs hand-editing generated projections.

### Stage 10 - Delete Spec Kit Runtime Surfaces

Deliverables:

- Delete `.specify/scripts/**`.
- Delete `.specify/extensions/**`.
- Delete `.specify/presets/**`.
- Delete `.specify/templates/**`.
- Delete `.specify/workflows/**`.
- Delete `.specify/init-options.json`.
- Delete `.specify/integration.json`.
- Delete `.specify/integrations/**`.
- Delete `.specify/feature.json`.
- Add `.specflow/current.json` and `.specflow/config.json`.
- Update docs and AGENTS.md.

Tests:

- Repository search finds no active `.specify/scripts` references.
- Repository search finds no active `speckit` command references except
  historical reports.
- `specflow status` works from a fresh checkout.
- `SpecFlowProjectionCheck` passes.

### Stage 11 - Worktree-First Execution

Deliverables:

- `Workspaces.fs`.
- `specflow workspace create`.
- Feature workspace state in graph.
- Cache namespace per feature:
  - `.fs-skia-cache/specflow/<feature-id>/`
- Explicit policy for FAKE state:
  - either isolate `.fake` by workspace, or prove shared `.fake` state is safe.

Tests:

- Sibling worktree creation.
- Nested worktree creation if supported.
- Cache writes are feature-namespaced.
- Two workspaces for two features do not share generated-product output paths.
- Workspace disposal does not delete committed evidence.

### Stage 12 - Approval And Review Gates

Deliverables:

- `Approval.fs`.
- Approval scopes:
  - spec;
  - plan;
  - public surface;
  - security;
  - architecture;
  - release.
- Commands:
  - `approval add`
  - `approval status`
  - `approval revoke`
- Route policy can require approvals for selected impact classes.

Tests:

- Approval with stale graph hash is stale.
- Public surface impact requires approval when policy says so.
- Revoked approval cannot satisfy route.
- Approval projection renders in `plan.md` and readiness index.

### Stage 13 - Research Claim Verification

Deliverables:

- `ResearchClaims.fs`.
- Commands:
  - `research add-url`
  - `research status`
  - `research stale`
- Optional non-gate helper:
  - `research refresh` that performs network lookup and records a new claim.

Tests:

- Current-version claim without URL fails.
- URL claim without retrieval date fails if marked current.
- Old retrieval date reports stale warning by policy.
- Network refresh is never required by deterministic gates.

### Stage 14 - Historical Import And Archive

Deliverables:

- Optional import of recent historical features to graph format.
- Archive old readiness files not referenced by evidence rows.
- Generate historical feature index from graphs.

Tests:

- Import can skip unsupported historical shapes with explicit diagnostics.
- Archive inventory lists moved files.
- Active feature graph remains unaffected by historical import failures.

## Testing Strategy

### Unit Tests

- JSON parse/write round-trip.
- Graph hash determinism.
- DAG cycle detection and topo order.
- Synthetic taint propagation.
- Requirement coverage.
- Evidence path validation.
- Route declared/actual drift.
- Approval freshness.
- Projection rendering.
- Context-pack generation.

### Property Tests

- Any generated acyclic task graph topologically sorts all nodes.
- Adding a dependency never removes an existing cycle diagnostic.
- Projection render followed by parse/import does not invent IDs.
- Graph hash changes when semantic fields change.
- Graph hash does not change when JSON object field order changes.

### Golden Tests

- Minimal feature graph projections.
- Public-surface feature graph projections.
- Template-impact feature graph projections.
- Synthetic-deferral feature graph projections.
- Approval-required feature graph projections.
- Context-pack JSON for each phase.

### Integration Tests

- `specflow new` creates graph and projections.
- `specflow activate` changes `.specflow/current.json`.
- `specflow task complete` updates graph and projections.
- `specflow project --check` fails after hand-editing generated `tasks.md`.
- `SpecFlowGraphCheck` and `SpecFlowAudit` run through FAKE target dispatch.
- Worktree command creates isolated workspace paths.

## Failure Modes And Diagnostics

| Failure | Diagnostic requirement |
|---|---|
| Projection edited by hand | Name projection, graph hash, and first differing hunk. |
| Done task lacks evidence | Name task, expected evidence kinds, and suggested `specflow evidence` command. |
| Evidence path missing | Name evidence ID, path, and task/requirement it was meant to satisfy. |
| Route mismatch | Show declared impact, actual diff rule matches, expected gates, actual gates. |
| Stale approval | Show approval ID, approved graph hash, current graph hash. |
| Uncovered requirement | Show requirement ID, text, and nearest candidate tasks if any. |
| Orphan evidence | Show evidence ID, path, and no covering requirement/task. |
| Synthetic taint | Show root synthetic task and downstream affected tasks. |
| Context pack stale | Show phase, projection path, expected graph hash. |

## Data Migration Rules

No backward compatibility is required, but data loss should still be explicit.

Rules:

- Import current active feature first.
- Preserve requirement IDs and task IDs when possible.
- Preserve long-form prose by embedding it in graph fields.
- Preserve old readiness files only if referenced by an evidence row.
- Convert `tasks.deps.yml` dependencies into `TaskNode.Dependencies`.
- Convert `owns:` values into `TaskNode.Owns`.
- Convert skill lists into `TaskNode.SkillIds`.
- Convert checkboxes into initial `TaskStatus`, then stop reading checkboxes.
- Record import warnings in `readiness/import-report.md`.

After import, the old artifacts are generated or deleted.

## Route Policy Changes

Current `Route` reads working-tree diff. Keep that, but add graph planning.

New flow:

1. During planning, graph `ImpactModel` declares expected impact.
2. `RoutePlanning` computes expected gates from declared impact.
3. During implementation, actual git diff is classified by existing
   `Routing.fs`.
4. `SpecFlowAudit` compares expected route to actual route.
5. Missing gate evidence is blocking.

This makes validation obligations visible before implementation and catches
under-declared features.

## Evidence Policy Changes

Current readiness scanning infers too much from file names and tokens. Replace it
with structured evidence rows.

Keep token scans only for:

- legacy import diagnostics;
- validating generated reports include required summary tokens;
- selected environment evidence formats where key-value text remains the most
  practical attachment format.

Every blocking evidence obligation should have a typed `EvidenceItem`.

## Context Budgeting

Context budgeting is not file compaction. It is phase-specific file selection.

Each context pack includes:

```json
{
  "phase": "implement",
  "graph_hash": "...",
  "required_reads": [],
  "optional_reads": [],
  "generated_do_not_edit": [],
  "allowed_mutations": [],
  "required_evidence": [],
  "route_expectations": [],
  "blockers": [],
  "large_artifacts": []
}
```

Rules:

- Generated files are marked `do_not_edit`.
- Large logs and readiness attachments are opt-in unless needed.
- Skills are selected from task graph `SkillIds`.
- The context pack is deterministic and projection-checked.

## Security And Trust Model

- No third-party extension code is installed by default.
- No community catalog entry is trusted as executable input.
- All executable behavior is repo-owned F# or existing repo scripts.
- Network lookup helpers are opt-in and non-gating.
- Approval artifacts bind to graph hash.
- Evidence rows point to local files or explicit external references.

## Performance And Caching

Cache only derived data:

- parsed graph;
- graph hash;
- projection render results;
- context-pack render results;
- skill registry enumeration;
- target metadata enumeration.

Do not cache:

- gate pass/fail verdicts;
- route actual diff results;
- evidence audit verdicts;
- approval status against current graph hash.

Cache path:

```text
.fs-skia-cache/specflow/<graph-hash>/
```

Workspace-specific cache path:

```text
.fs-skia-cache/specflow/<feature-id>/<workspace-id>/
```

## Acceptance Criteria

The redesign is complete when:

- Active feature state is stored in `feature.graph.json`.
- `.specflow/current.json` replaces `.specify/feature.json`.
- `spec.md`, `plan.md`, and `tasks.md` are generated projections.
- `tasks.deps.yml` is not an active authored artifact.
- Task completion is graph-owned and evidence-backed.
- Requirement-to-task-to-evidence traceability is machine-checked.
- Route expectations are graph-visible before implementation.
- Actual diff route is compared with declared impact.
- Gate evidence is stored as typed evidence rows.
- Approval artifacts bind to graph hash.
- Context packs replace long phase prompt instructions as operational input.
- Active `speckit-*` skills and `.specify/scripts/**` are gone.
- New graph FAKE targets replace `EvidenceGraph` / `EvidenceAudit`.
- Projection drift is a hard failure.
- A fresh checkout can run `./specflow status` and explain the active feature.

## Risks And Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Graph schema becomes too large | Hard to author and review | Keep prose fields simple; generate projections for human review; add focused mutation commands. |
| Generated Markdown becomes unreadable | Maintainers stop trusting projections | Golden-review projections early; keep current spec/plan readability as a design constraint. |
| Direct JSON edits are painful | Slower authoring | Provide `specflow requirement/task/evidence/approval` mutation commands. |
| Route planning duplicates `Routing.fs` | Drift between declared and actual route | Reuse `Routing.rules`; do not create a parallel rule set. |
| Evidence rows become busywork | Agents add low-value evidence | Context packs list required evidence per task; audit distinguishes authoritative and informational evidence. |
| Deleting `.specify` breaks hidden assumptions | Build/skills fail in unexpected places | Search and remove references in the same cutover feature; add `specflow status` and projection tests first. |
| Worktrees fight shared state | Parallel runs corrupt cache or FAKE state | Namespace caches; explicitly audit FAKE state before default worktree mode. |
| Approval hashes churn too often | Review gates become noisy | Scope approvals; allow narrow approval reuse only when validator proves affected graph fields unchanged. |

## Recommended First Feature Cut

Because the redesign is breaking, the first cut should still be narrow and
mechanically decisive:

1. Add graph model, JSON parser/writer, hash, and validator.
2. Add projection generator for `spec.md`, `plan.md`, `tasks.md`,
   `task-graph.json`, and `task-graph.md`.
3. Add `specflow status`, `specflow graph validate`, and
   `specflow project --check`.
4. Import the active feature into `feature.graph.json`.
5. Regenerate projections and delete `tasks.deps.yml` for the active feature.
6. Add `SpecFlowGraphCheck` and `SpecFlowProjectionCheck`.
7. Delete `.specify/scripts/**` and active `speckit-*` skill references in the
   same feature if the graph commands are usable.

Do not start with worktrees, approvals, research refresh, or historical import.
Those are valuable, but the core authority transfer must land first.

## Source Notes

- GitHub Spec Kit latest release API, checked 2026-06-13:
  <https://api.github.com/repos/github/spec-kit/releases/latest>
- GitHub Spec Kit `v0.10.2` release page:
  <https://github.com/github/spec-kit/releases/tag/v0.10.2>
- Spec Kit extensions README:
  <https://github.com/github/spec-kit/blob/main/extensions/README.md>
- Spec Kit community extension catalog:
  <https://speckit-community.github.io/extensions/all-extensions>
- Spec Kit raw community catalog:
  <https://github.com/github/spec-kit/blob/main/extensions/catalog.community.json>
- Spec Kit built-in workflow:
  <https://github.com/github/spec-kit/blob/main/workflows/speckit/workflow.yml>
- FSharp.Formatting content guidance:
  <https://fsprojects.github.io/FSharp.Formatting/content.html>
- Local modules inspected:
  `build/Governance/Engine/{Model,Update}.fs`,
  `build/Governance/Evidence/{TaskParser,DepsParser,Graph,Audit,Render,Scans}.fs`,
  `build/Governance/{Routing,Targets,TargetMetadata,Guidance,SymbolCrossCheck}.fs`,
  `.specify/**`, `.agents/skills/**`, and current feature artifacts under
  `specs/116-paint-cache-damage-rects/**`.
