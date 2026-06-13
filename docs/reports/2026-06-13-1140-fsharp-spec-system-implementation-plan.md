---
title: F# Spec System Implementation Plan
---

# F# Spec System Implementation Plan

- **Timestamp:** 2026-06-13T11:40:03+02:00
- **Author:** Codex
- **Status:** Proposed implementation plan
- **Scope:** Replace upstream Spec Kit as an installer/runtime dependency for
  this repository by finishing the repo-owned F#/FAKE implementation, while
  preserving the useful Spec Kit artifact model: `spec.md`, `plan.md`,
  `tasks.md`, `tasks.deps.yml`, readiness artifacts, phase skills, and
  agent-facing prompts.
- **Constraint for this report:** No governance machinery was run. Research used
  file inspection, web/version lookup, the community extension catalog at
  `speckit-community.github.io`, and a temporary read-only clone of
  `github/spec-kit` at `v0.10.2`.

## Executive Decision

Finish the migration, but define the target precisely:

1. **Replace upstream Spec Kit as a dependency.** Do not keep `specify-cli`,
   `uv`, Python runtime assumptions, or upstream managed-file version markers in
   the active path.
2. **Keep the Spec Kit-shaped workflow.** The repository should still have
   features under `specs/<id>/`, an active feature context, phase skills, task
   metadata, evidence graph/audit, and generated readiness artifacts.
3. **Move remaining mechanical behavior into compiled F#.** The residual shell
   scripts should become typed modules and commands in `FS.Skia.UI.Build`, with
   pure cores and filesystem/git/process effects at the interpreter edge.
4. **Keep Markdown for agent prompts.** Prompts, task-generation guidance, and
   human-facing examples should stay in `.agents/skills/**` and templates. F#
   should generate, validate, select, and report on them; it should not turn
   prompt prose into large string literals.
5. **Use Spectre.Console for human UX, with strict machine-output modes.**
   Human output should become richer and more legible. JSON and artifact formats
   must remain plain, deterministic, and stable.

The important nuance: this is not a "rewrite Spec Kit in F# for everyone"
project. It is a repo-owned spec workflow for FS.Skia.UI, taking lessons from
latest upstream Spec Kit `v0.10.2` while making stronger local assumptions than
upstream can make.

## Latest Upstream Research

The local `.specify/integration.json` and integration manifests still carry
`0.8.16`, while `.specify/init-options.json` says `0.10.2`. The upstream point
of comparison for this plan is **not** either local marker by itself; it is the
current upstream release.

As of 2026-06-13, the latest GitHub Spec Kit release is **`v0.10.2`**, released
2026-06-11. Its release notes list the install command:

```bash
uv tool install specify-cli --from git+https://github.com/github/spec-kit.git@v0.10.2
```

Relevant upstream changes from the `0.9.x` / `0.10.x` line:

- `v0.10.0` made the Git extension opt-in and removed `--no-git`.
- `v0.10.0` removed legacy `--ai`, `--ai-commands-dir`, and `--ai-skills`
  flags in favor of `--integration` and `--integration-options`.
- `v0.10.0` added per-event hook lists with priority ordering.
- `v0.10.1` added integration status reporting and catalog payload-shape
  validation.
- `v0.10.2` added extension `category` and `effect` fields, hardened preset URL
  installs, fixed recovered-file overwrite checks, and expanded community
  catalog entries.
- The latest Codex integration is skills-first: Codex uses
  `.agents/skills/speckit-<name>/SKILL.md`, and `--skills` defaults to `true`.
- Upstream now bundles workflows; the built-in `speckit` workflow runs
  `specify -> plan -> tasks -> implement` with review gates.

The latest upstream scripts also changed the core feature-context stance:
current core `common.sh` resolves active feature context from
`SPECIFY_FEATURE_DIRECTORY` or `.specify/feature.json`, and fails if neither
exists. Branch inference moved out of the core path. In this repo, local scripts
still carry additional git/branch fallbacks and custom hard-fail behavior. The
F# replacement should follow the **newer explicit feature context model**, with
git branch creation as an optional feature-creation behavior, not as a hidden
fallback for every phase.

## Community Extension Catalog Research

The community extension catalog currently lists **113** extensions and explicitly
states that it is community-maintained and not affiliated with GitHub. Treat it
as an idea source, not an authority, installer source, or dependency. The repo
should not import third-party extensions wholesale. It should incorporate the
useful patterns as typed, repo-owned F# commands and diagnostics.

Functionality worth folding into this plan:

- **Visibility and health surfaces.** `Project Status`, `Status Report`,
  `Project Health Check`, and `Spec Diagram` all point to the same gap: the new
  `spec status` command should not only show active feature context. It should
  also summarize artifact presence, workflow phase, task completion, effective
  extensions/hooks, and next actionable command. Add `spec doctor --json` for a
  read-only structural diagnosis, and `spec graph --mermaid` for the existing
  task dependency graph when an agent or maintainer needs a visual artifact.
- **Traceability and completion honesty.** `Spec Trace`, `Verify`, and
  `Verify Tasks` overlap with this repository's existing evidence graph and
  audit model. Add a local `spec trace --json` report that connects requirement
  IDs, task IDs, declared evidence paths, tests, and readiness artifacts. It
  should flag untested requirements, orphan tests/evidence, and tasks marked
  complete without matching implementation or evidence.
- **Review gates as advisory reports first.** `Plan Review Gate`, `Spec
  Validate`, `Spec Critique`, `Staff Review`, `Architecture Guard`, `DocGuard`,
  and `Security Review` are useful as report shapes, but most are too subjective
  or environment-dependent to become hard gates immediately. Encode the durable
  parts as deterministic checks: spec/plan/tasks existence, stale generated
  docs, architecture drift against repo governance, security-review checklist
  reminders, and missing approval metadata. Keep AI or human-review conclusions
  advisory unless the repository later adopts an explicit approval artifact.
- **Worktree-aware feature creation.** `Worktree Isolation` and `Worktrees`
  show that parallel feature execution is a first-class workflow concern. Add
  optional `spec new --worktree sibling|nested` and `spec activate --worktree`
  behavior after branch creation is ported. Do not make worktrees default until
  cache paths, FAKE `.fake` state, and generated artifact paths are audited for
  worktree safety.
- **Token and context budgeting.** `Token Budget` and `Token Consumption
  Analyzer` fit this repository's large spec/readiness artifacts. Add a
  read-only `spec context --phase <phase> --json` command that returns the
  minimal artifact set an agent should read for a phase, large-artifact warnings,
  and optional token estimates. Avoid in-place compaction in the first pass; the
  current governance value is reproducible evidence, not shorter files.
- **Research and version evidence.** `Research Harness` and `Version Guard`
  reinforce a local need already visible in this report: current-version claims
  should carry source URLs, lookup dates, and the exact package/release checked.
  Add a lightweight research-evidence schema for reports that need online
  version checks, but keep network lookups out of deterministic validation
  gates.
- **Catalog metadata model.** Several catalog entries expose `category`,
  `effect`, `commands`, and `hooks`. Stage 4 should parse and validate those
  fields for local extension manifests and status output, because they are
  useful operator-facing metadata even when no community extension is installed.

Functionality to leave out of the first migration:

- Tracker/vendor integrations (`Jira`, `Linear`, Azure DevOps, Microsoft 365,
  Confluence, GitHub Projects) because this repository has no current
  requirement to synchronize external product-management systems.
- Full multi-agent orchestration, scheduling, memory, RAG, and multi-model
  review engines. They are workflow policy choices, not prerequisites for
  replacing shell mechanics with compiled F#.
- Third-party catalog install/update behavior. The durable local contract is
  `.agents` skills plus repo-owned manifests, not a marketplace runtime.

## Spectre.Console Research

Use **Spectre.Console**, not Python Rich, because the build tool is already .NET
10 / F# and the output can be injected/tested without adding another runtime.

Current package state:

- Spectre.Console `0.57.0` was released 2026-06-11.
- Spectre.Console.Cli `0.55.0` is stable and targets `net8.0`, `net9.0`, and
  `net10.0`; however, Spectre.Console.Cli moved to a separate repository and
  its versioning path is distinct from Spectre.Console.

Recommendation:

- Add `Spectre.Console` only to build tooling, not shipped runtime packages.
- Do **not** take a first dependency on `Spectre.Console.Cli` unless the command
  parser becomes genuinely complex. The first implementation can parse a small
  command vocabulary manually and use Spectre.Console only for rendering.
- Inject `IAnsiConsole` at the program edge so tests can use
  `TestConsole`/recorded output and pure modules remain independent.

## Current Local Inventory

### Already F#-Native

These are already owned by compiled F#:

- `fake.sh` runs `dotnet run --project build/Build.fsproj`, not a `build.fsx`
  script runner.
- `build/Program.fs` registers typed FAKE targets from `Targets.dispatchTargets`.
- `build/Governance/Targets.fs` is the single typed target registry and
  dependency graph.
- `build/Governance/Engine/{Model,Update,Interpret}.fs` implements an MVU-style
  build engine: pure update emits effects, the interpreter performs filesystem,
  process, git, and write operations.
- `build/Governance/Routing.fs` selects tiers/gates from the working-tree diff.
- `build/Governance/Evidence/**` owns task parsing, dependency parsing, graph
  validation, synthetic propagation, diff scan, evidence audit, rendering, and
  generated-product evidence runner.
- `SkillTreeGen` and `SkillSync` make `.agents/skills/**` canonical and
  `.claude/skills/**` generated.
- `PhaseHookParity`, `SkillQuality`, `Guidance`, `GeneratedProduct`,
  `GeneratedProductContract`, `TargetMetadata`, `ContractView`,
  `PackageSkew`, and related modules already encode governance rules that
  upstream Spec Kit would keep as scripts, Python, or installer-managed assets.

### Still Coupled To Spec Kit Shape

The remaining active coupling is not large, but it is workflow-critical:

- `.specify/scripts/bash/common.sh`
- `.specify/scripts/bash/create-new-feature.sh`
- `.specify/scripts/bash/setup-plan.sh`
- `.specify/scripts/bash/setup-tasks.sh`
- `.specify/scripts/bash/check-prerequisites.sh`
- `.specify/extensions/git/scripts/**`
- `.specify/init-options.json`, `.specify/integration.json`, and integration
  manifests recording upstream versions and managed-file hashes
- `.specify/extensions.yml`, per-extension manifests, preset registry, workflow
  registry, and template override stack
- `.agents/skills/speckit-*` phase skills that still describe Spec Kit
  command/script conventions

Local-vs-upstream `v0.10.2` comparison found every local core bash script
differs from upstream. Local templates also differ, as expected for this
framework, and the local git extension differs from upstream. That confirms the
repo is already a fork in practice. The plan should stop pretending the
upstream installer is authoritative.

## Assumptions We Can Make

Upstream Spec Kit has to support many agents, new/old projects, git/non-git
projects, shell flavors, Python availability, optional jq, online/offline
install paths, user-managed presets, and arbitrary extension catalogs. This
repository can assume much more:

1. **The project is always this repository.** Root discovery can walk upward for
   `FS-Skia-UI.sln` and `.specify/feature.json`; no parent-repo fallback is
   needed.
2. **Git is available.** Feature creation and route/audit diffing can treat git
   as required. No non-git project mode is needed.
3. **.NET 10 and F# are available.** All mechanical workflow logic can be
   compiled and tested as normal F#.
4. **No Python, `uv`, `jq`, `PyYAML`, or Bash is required in the active path.**
   Bash wrappers can remain temporarily as compatibility launchers only.
5. **Codex and Claude are the only maintained agent surfaces.** `.agents` is
   canonical. `.claude` is generated. Other upstream integrations do not need to
   be modeled except as historical migration data.
6. **Templates and prompts are repo-owned.** The template stack is not an
   arbitrary marketplace; it is a controlled local source with a small number of
   known overrides/presets/extensions.
7. **The workflow is not a generic workflow engine requirement.** A minimal
   phase/hook executor and status reporter is enough unless the repo later
   chooses to dogfood upstream-style workflow YAML.
8. **Machine outputs outrank terminal beauty.** Rich console output is for
   operators; `--json`, generated Markdown, and evidence artifacts must stay
   deterministic and simple.

These assumptions justify a smaller and more reliable design than upstream:
typed F# records instead of shell environment interpolation, direct JSON/YAML
parsing instead of tool fallback chains, a closed integration set, and cache
keys built from repo-local source hashes.

## Target Architecture

Add a repo-owned "spec workflow" area inside `FS.Skia.UI.Build`. Suggested
namespace/module root:

```text
build/Governance/SpecFlow/
  Console.fs
  FeatureContext.fs
  FeatureNaming.fs
  FeatureScaffold.fs
  FeaturePaths.fs
  TemplateResolver.fs
  IntegrationState.fs
  ExtensionManifest.fs
  HookRegistry.fs
  Status.fs
  Doctor.fs
  Traceability.fs
  ContextBudget.fs
  WorktreeIsolation.fs
  PhaseCommand.fs
  Cache.fs
  Reports.fs
```

`SpecFlow` is an internal name for the compiled replacement. User-facing text
can continue to say "Spec Kit workflow" during migration if that avoids churn,
but the implementation should no longer report upstream Spec Kit as the owner.

### Command Surface

Keep `fake.sh build -t <Target>` as the validation/gate path. Add a separate
spec workflow command path so scaffolding commands do not masquerade as
validation targets.

Preferred shape:

```bash
./spec.sh new "dependency updates" --short-name dependency-updates
./spec.sh activate specs/115-dependency-updates
./spec.sh paths --json
./spec.sh setup-plan --json
./spec.sh setup-tasks --json
./spec.sh prereq --json --require-tasks --include-tasks
./spec.sh hooks before_plan --json
./spec.sh status
./spec.sh doctor --json
./spec.sh trace --json
./spec.sh graph --mermaid
./spec.sh context --phase plan --json
./spec.sh templates tasks-template --explain
```

Implementation options:

- **Preferred:** add `build/SpecTool.fsproj`, referencing
  `FS.Skia.UI.Build`, and a root `spec.sh` launcher. This keeps FAKE target
  registration clean.
- **Acceptable:** add a pre-FAKE dispatch branch in `build/Program.fs` for
  `spec ...` commands before `Target.runOrDefaultWithArguments`.

Either way, the command implementation should reuse the same pure modules.
The commands are scaffolding/diagnostic tools, not FAKE gates.

### Domain Model

Use small typed records and discriminated unions instead of stringly shell state:

```fsharp
type FeatureId =
    | Sequential of number: int * slug: string
    | Timestamped of timestamp: DateTimeOffset * slug: string

type FeatureContext =
    { RepositoryRoot: string
      FeatureDirectory: string
      FeatureId: string
      Source: FeatureContextSource }

type FeatureContextSource =
    | ExplicitEnvironment
    | FeatureJson
    | CreatedNow

type FeaturePaths =
    { FeatureDir: string
      Spec: string
      Plan: string
      Tasks: string
      Research: string
      DataModel: string
      Quickstart: string
      ContractsDir: string }

type OutputMode =
    | Human
    | Json
```

Key rule: feature context resolution is explicit and durable. `new` writes
`.specify/feature.json`. Phase commands read it. Branch names are no longer the
ordinary source of truth after creation.

### Effect Boundary

Follow the existing build engine style:

- Pure modules produce plans, diagnostics, and write intents.
- Interpreter modules perform reads, writes, git calls, and process calls.
- Console rendering is at the outer edge and consumes already-computed models.

This keeps the new workflow testable and consistent with
`Engine/Update.fs` + `Engine/Interpret.fs`.

## Spectre.Console Design

Create a thin local wrapper instead of scattering `AnsiConsole.MarkupLine`
through the codebase:

```fsharp
type ConsoleMode =
    | Plain
    | Rich
    | Json

type SpecConsole =
    { Mode: ConsoleMode
      Out: Spectre.Console.IAnsiConsole }
```

Use Spectre.Console for:

- **Status panels** for feature creation and activation.
- **Tables** for active feature, required files, available docs, hook lists,
  template source, traceability gaps, context budgets, doctor findings, and
  cache hits/misses.
- **Trees** for template resolution and extension/preset layers.
- **Mermaid output** for task and evidence graphs only when explicitly requested
  by a machine-stable command such as `spec graph --mermaid`.
- **Progress/status** for multi-file scaffold writes, but only when the console
  is interactive.
- **Rules/panels** for blocking diagnostics with clear next commands.

Required output modes:

- `--json`: writes only JSON to stdout, no ANSI, no prose.
- `--plain` or non-TTY fallback: no color, no live progress.
- `--no-color`: Spectre profile with color disabled.
- Normal interactive terminal: rich tables/panels/progress.

Do not use Spectre output inside generated artifacts. Reports under
`readiness/` and `docs/` stay Markdown/JSON.

## Performance And Caching Decisions

### 1. Avoid `dotnet run` Work On Every Invocation

The current `fake.sh` uses:

```bash
dotnet run --project build/Build.fsproj -- "$@"
```

That is already better than a script runner, but `dotnet run` still performs
restore/build checks before executing. For high-frequency scaffolding commands,
prefer an executable cache:

1. Compute a build-tool input stamp from:
   - `build/Build.fsproj`
   - `build/SpecTool.fsproj` if introduced
   - `build/Governance/**/*.fs`
   - `Directory.Packages.props`
   - `Directory.Build.props`
   - `build/**/project.assets.json`
2. If the stamp matches the cached executable, launch `dotnet exec` directly.
3. If stale, run `dotnet build build/SpecTool.fsproj -m:1 --nologo` once, write
   the stamp, then launch `dotnet exec`.

This gives a fast hot path without weakening correctness. The existing
validation gates can keep their current wrapper until a separate performance
feature changes them.

### 2. Cache Derived Data, Not Verdicts

Safe to cache:

- Parsed `.specify/feature.json`
- Resolved template layer plans
- Parsed extension manifests and hook registry
- Skill registry enumeration
- File-list manifests for `.agents`/`.claude` when keyed by content hash
- Latest feature directory list

Do **not** cache:

- Build/test/audit pass/fail results
- Git diffs
- EvidenceAudit verdicts
- Generated product validation outcomes

Cache key rule: a cache entry must be invalidated by the content hash of every
file that can change the result. When that is awkward, prefer process-local
memoization only.

Suggested cache location:

```text
.fs-skia-cache/specflow/
  build-tool.stamp.json
  template-resolver.<hash>.json
  hook-registry.<hash>.json
  skill-registry.<hash>.json
```

Add `.fs-skia-cache/` to `.gitignore` in the implementation feature if it is
not already ignored.

### 3. Build Around Git Cheaply

Feature numbering can avoid `git fetch` by default:

- Sequential number source: `specs/<number>-*` directories plus local branch
  names.
- Remote refs are consulted only with an explicit `--include-remotes` or
  `--remote-check` option.
- Existing remote lookups use `git ls-remote` with
  `GIT_TERMINAL_PROMPT=0`, never `git fetch --all`, unless the user explicitly
  asks.

### 4. Prefer One Repository Snapshot Per Command

Many current gates repeatedly enumerate the same trees. The new command path
should build one `RepoSnapshot` per invocation:

```fsharp
type RepoSnapshot =
    { Root: string
      SpecDirs: string list
      AgentSkillFiles: string list
      ClaudeSkillFiles: string list
      SpecifyFiles: string list
      TemplateFiles: string list }
```

Pure functions consume the snapshot instead of calling `Directory.GetFiles`
independently. That reduces filesystem churn and makes tests simpler.

### 5. No FSharp.Compiler.Service Runtime Loading

Do not introduce runtime-compiled `.fsx` configuration. Compiled F# is the
project's advantage: mistakes fail at build time, target names are typed, and
per-run compile tax is avoided.

## Implementation Stages

### Stage 0 — Lock The Decision And Scope

Deliverables:

- This report.
- Optional ADR follow-up: "Repo-owned F# spec workflow replaces upstream
  Spec Kit installer/runtime dependency."

Exit criteria:

- The team agrees on the boundary: keep artifacts/prompts, replace installer
  and residual shell mechanics.

### Stage 1 — Add Console And Command Skeleton

Deliverables:

- Add build-tooling-only `PackageVersion Include="Spectre.Console"` pinned to
  the selected stable version.
- Add `SpecFlow.Console` wrapper with `Human`, `Plain`, and `Json` modes.
- Add `build/SpecTool.fsproj` or a pre-FAKE `spec` dispatch path.
- Add root launcher `spec.sh` and, if Windows command support is required,
  `spec.cmd`.
- Implement `spec status` as a read-only command that reports active feature,
  artifact presence, inferred workflow phase, task completion if `tasks.md`
  exists, effective extension/hook summary, and next suggested local command.

Tests:

- Unit tests for mode selection and JSON output.
- Snapshot tests for plain output if useful.

Notes:

- Do not touch `.specify/scripts/**` yet.
- Do not add behavior beyond status and command plumbing.

### Stage 2 — Port Feature Context And Prerequisite Checks

Deliverables:

- `FeatureContext.fs`: resolve `SPECIFY_FEATURE_DIRECTORY`, then
  `.specify/feature.json`; fail if absent.
- `FeaturePaths.fs`: produce the same path payloads as `common.sh`.
- `Prerequisites.fs`: implement:
  - `paths --json`
  - `prereq --json`
  - `--require-tasks`
  - `--include-tasks`
  - `--paths-only`
- Compatibility shell wrappers that delegate to `spec.sh` but preserve current
  script names and JSON shape.

Tests:

- Fixture repos in temporary directories:
  - valid feature context
  - missing feature context
  - relative and absolute feature directories
  - missing `plan.md`
  - missing `tasks.md`
  - available docs list variants
- Exact JSON shape parity with current scripts where the shape is still
  consumed by skills.

Design correction from upstream:

- Do not infer active feature from current git branch during ordinary phase
  commands. Use `.specify/feature.json` or explicit environment.

### Stage 3 — Port Feature Creation And Activation

Deliverables:

- `FeatureNaming.fs`: slug cleaning, stop-word filtering, sequential numbering,
  timestamp numbering, max branch/feature-name length.
- `FeatureScaffold.fs`: create `specs/<id>/`, write `spec.md` from template,
  write/update `.specify/feature.json`.
- `spec new`, `spec activate`, `spec list`.
- Git behavior:
  - default for this repo: create/switch feature branch on `spec new`;
  - option: `--no-branch` for branchless activation;
  - option: `--worktree sibling|nested` to create an isolated worktree after
    branch creation, once cache and `.fake` state are audited for that layout;
  - option: `--allow-existing`.
- Migrate `speckit-git-feature` skill guidance to call the F# command.

Tests:

- Sequential numbering from existing `specs/**`.
- Timestamp numbering format.
- Slug edge cases.
- Existing feature reuse.
- Branch command planning with git mocked at the process wrapper boundary.

Risk:

- Branch creation semantics are user-visible. Keep a compatibility window where
  the old script still works by delegating to the new command.

### Stage 4 — Port Template, Preset, And Extension Resolution

Deliverables:

- `TemplateResolver.fs`:
  - project overrides
  - preset priority registry
  - extension templates
  - core templates
  - composition strategies currently implemented by shell
- `ExtensionManifest.fs`:
  - schema version
  - `category`
  - `effect`
  - command and hook counts for status/doctor output
  - commands
  - per-event hook lists
  - priority
  - optional/mandatory flags
- `HookRegistry.fs`:
  - merge root `.specify/extensions.yml`
  - merge `.specify/extensions/*/*.yml`
  - sort by priority
  - de-duplicate by `(extension, command)`
  - render effective hooks for a phase
- `spec templates <name> --explain`
- `spec hooks <phase> --json`

Tests:

- Template priority fixtures.
- Preset registry priority fixtures.
- Extension hook priority/dedupe fixtures.
- Category/effect/provides validation fixtures based on upstream `v0.10.2`
  schema and the community catalog metadata shape.

Decision:

- Keep `.specify/extensions.yml` as the durable authored data for now. Do not
  invent a new config location until the mechanical behavior is fully ported.

### Stage 5 — Port Setup Plan And Setup Tasks

Deliverables:

- `spec setup-plan --json`:
  - require/resolve feature context
  - ensure feature dir exists
  - copy resolved plan template to `plan.md` if appropriate
  - output `FEATURE_SPEC`, `IMPL_PLAN`, `SPECS_DIR`, `BRANCH` if still needed,
    and `HAS_GIT` only if a compatibility consumer still expects it
- `spec setup-tasks --json`:
  - require `spec.md` and `plan.md`
  - resolve tasks template
  - output `FEATURE_DIR`, `AVAILABLE_DOCS`, `TASKS_TEMPLATE`
- Shell wrappers delegate to these commands.
- Phase skills updated from script paths to `spec.sh` commands.

Tests:

- Exact JSON fixtures for `setup-plan` and `setup-tasks`.
- Template missing diagnostics.
- Existing file preservation behavior.

### Stage 6 — Migrate Agent Skills And Generated Claude Mirror

Deliverables:

- Update canonical `.agents/skills/speckit-*` phase skills:
  - use `./spec.sh ...` for scaffolding/prereq commands;
  - describe explicit feature context;
  - remove upstream installer/update language where it is no longer true;
  - keep prompt responsibilities in Markdown.
- Regenerate `.claude/skills/**` through the existing skill tree generation
  path in the implementation feature.
- Add a small "SpecFlow command reference" doc if needed.

Tests:

- Existing `SkillSyncCheck`/skill-quality tests should cover the mirror and
  section quality when the implementation feature validates.
- Add unit tests for command names in skills if not already covered by
  `PhaseHookParity`.

### Stage 7 — Retire Upstream Version Markers

Deliverables:

- Replace `.specify/init-options.json`/integration manifest dependence with a
  repo-owned state file, for example:

```json
{
  "schema_version": 1,
  "owner": "FS.Skia.UI",
  "workflow": "fsharp-specflow",
  "active_feature_source": ".specify/feature.json",
  "canonical_agent_tree": ".agents/skills",
  "generated_agent_trees": [".claude/skills"]
}
```

- Keep `.specify/feature.json` because the path is already embedded in build
  evidence logic and generated consumers.
- Mark old upstream manifests as archived/compatibility-only or delete them
  once no code/skills read them.

Tests:

- No active code path reads `speckit_version` for behavior.
- Any retained file is documented as compatibility metadata, not authority.

### Stage 8 — Remove Residual Shell Scripts

Deliverables:

- Delete or archive:
  - `.specify/scripts/bash/common.sh`
  - `.specify/scripts/bash/create-new-feature.sh`
  - `.specify/scripts/bash/setup-plan.sh`
  - `.specify/scripts/bash/setup-tasks.sh`
  - `.specify/scripts/bash/check-prerequisites.sh`
- Delete or replace `.specify/extensions/git/scripts/**` if every git extension
  skill now calls the F# command.
- Update generated product/template package only if those scripts are shipped
  to consumers.

Tests:

- Search proof: no phase skill references deleted paths.
- Generated product file-list expectations updated if necessary.

### Stage 9 — Add Cache Hot Path

Deliverables:

- `.fs-skia-cache/specflow` cache writer/reader.
- Build-tool executable stamp for `spec.sh`.
- Template/hook/skill-registry cache keyed by content hashes.
- `spec cache status` and `spec cache clear`.

Tests:

- Cache hit/miss determinism.
- Stale invalidation when an input file changes.
- No cache use in `--json` changes output semantics.

### Stage 10 — Add Local Visibility, Traceability, And Context Reports

Deliverables:

- `spec doctor --json`:
  - validate repository shape, active feature context, expected artifacts,
    canonical/generated skill trees, extension manifests, and compatibility
    wrappers;
  - classify findings as blocking, advisory, or informational;
  - never run FAKE gates or network lookups.
- `spec trace --json`:
  - map requirement IDs to task IDs, evidence paths, tests, and readiness
    artifacts;
  - flag requirements with no task/evidence/test coverage;
  - flag tasks marked complete without matching implementation or evidence;
  - flag orphan tests/evidence that no longer map to an active requirement.
- `spec graph --mermaid`:
  - reuse the existing task DAG/evidence graph parser;
  - emit a deterministic Mermaid diagram for docs/reports or PR review.
- `spec context --phase <phase> --json`:
  - return the minimal files an agent should read for a phase;
  - report large artifacts and rough token estimates when available;
  - keep files unchanged by default.
- Advisory report commands for plan/spec critique, architecture drift, doc drift,
  and security-review reminders, reusing existing governance inputs where
  possible. These reports may inform `EvidenceAudit`, but should not become hard
  gates without an explicit repository approval artifact.

Tests:

- Doctor fixtures for missing feature context, missing artifacts, stale generated
  skill mirror, invalid extension metadata, and compatibility wrapper presence.
- Traceability fixtures for covered, uncovered, orphan, and phantom-complete
  tasks.
- Mermaid golden output for a small task graph.
- Context-budget fixtures for phase-specific file selection and large-artifact
  warnings.

### Stage 11 — Optional Workflow Layer

Only do this if there is a concrete need after stages 1-10.

Deliverables:

- Minimal `spec workflow run speckit` that executes this repo's known phases and
  hooks.
- Human gates rendered through Spectre.Console when interactive.
- JSON status output.

Do **not** port upstream's full YAML workflow engine unless the repo actually
uses general workflows. The local phase/hook model is enough for current needs.

## Compatibility Strategy

Use a three-step compatibility window:

1. **Introduce F# commands while old scripts remain.**
2. **Change old scripts into thin wrappers that delegate to F# commands.**
3. **Update skills/templates to call F# commands directly, then delete wrappers.**

This avoids breaking in-progress agent sessions that still have old instructions
in context. The deletion should be a separate small change after search proofs
show no active references remain.

## Testing Strategy

Primary test shape:

- Pure unit tests for parsing, naming, template resolution, hook merging, and
  feature context.
- Golden JSON tests for command outputs that skills consume.
- Temporary-repository integration tests for create/activate/setup/prereq.
- Console rendering tests through injected `IAnsiConsole`, not global console
  state.
- No tests should require `uv`, Python, jq, or upstream Spec Kit.

Suggested fixture matrix:

| Area | Fixture |
|---|---|
| Feature context | env override, feature.json relative, feature.json absolute, absent context |
| Feature naming | sequential, timestamp, explicit number, explicit short name, long slug |
| Prereqs | missing plan, missing tasks, docs present/absent |
| Templates | override wins, preset priority wins, extension fallback, core fallback, missing |
| Hooks | root config only, per-extension config only, duplicate command, priority ordering |
| Console | rich TTY, plain non-TTY, no color, JSON |
| Cache | hit, stale by file content, stale by tool version |

## Acceptance Criteria For The Completed Migration

- `specify-cli` is not required to create, activate, plan, task, or implement a
  feature in this repo.
- No active phase skill instructs an agent to run `.specify/scripts/bash/*`.
- `.agents/skills/**` remains canonical and `.claude/skills/**` remains
  generated.
- `.specify/feature.json` remains the active feature authority, or its
  replacement is explicitly wired into evidence and generated-product paths.
- All mechanical rules formerly in shell scripts have typed F# modules with
  tests.
- Human command output is clearer through Spectre.Console, while `--json` output
  is byte-stable and ANSI-free.
- `spec status`, `spec doctor`, `spec trace`, and `spec context` provide
  read-only visibility equivalent to the useful parts of the community status,
  health, traceability, and token-budget extensions.
- Optional worktree creation is either supported with explicit cache/state
  safety tests or deliberately deferred in documented scope.
- Cache is used only for derived data and command startup, never for validation
  verdicts.
- The repository no longer needs to reconcile upstream managed-file hashes with
  local forked files.

## Risks And Mitigations

| Risk | Impact | Mitigation |
|---|---|---|
| Feature creation semantics change unexpectedly | Agents create wrong branch/spec directory | Golden tests against current behavior; compatibility wrappers; explicit `spec activate` |
| Rich console output leaks into JSON | Agents fail parsing command output | Central output-mode abstraction; tests assert stdout is pure JSON |
| Cache hides stale data | Wrong setup/hook/template result | Content-hash keys; cache only derived data; `--no-cache`; never cache verdicts |
| Too much upstream workflow engine is copied | Recreates Python CLI complexity in F# | Port only local needs; defer generic workflow YAML |
| Community extension ideas cause scope creep | F# migration expands into a marketplace/runtime project | Treat catalog entries as research only; implement local read-only reports first |
| Worktree support corrupts shared build state | Parallel agents fight over `.fake`, cache, or generated artifacts | Keep worktrees opt-in; test cache/state paths before making it available |
| Prompt content gets buried in F# | Harder agent maintenance | Keep prompts Markdown; F# validates/generates only |
| Deleting `.specify` metadata breaks generated product assumptions | Generated consumers lose evidence context | Keep `.specify/feature.json`; change package/template paths only with explicit compatibility tests |

## Recommended First Feature Cut

The first implementation feature should be intentionally narrow:

1. Add Spectre.Console to build tooling.
2. Add `SpecFlow.Console`, `FeatureContext`, `FeaturePaths`, and `Prerequisites`.
3. Add `spec.sh status`, `spec.sh paths --json`, and
   `spec.sh prereq --json`.
4. Add tests for those pure modules and command outputs.
5. Leave all existing shell scripts in place.

That proves the command architecture, console abstraction, JSON discipline, and
explicit feature context before touching branch creation or templates.

## Sources

- GitHub Spec Kit releases: <https://github.com/github/spec-kit/releases>
- GitHub Spec Kit README / CLI reference: <https://github.com/github/spec-kit>
- GitHub Spec Kit upgrade guide: <https://github.github.com/spec-kit/upgrade.html>
- GitHub Spec Kit community extensions docs:
  <https://github.github.io/spec-kit/community/extensions.html>
- SpecKit community extension catalog:
  <https://speckit-community.github.io/extensions/all-extensions>
- Temporary local research clone: `github/spec-kit` tag `v0.10.2`
  (`pyproject.toml`, `scripts/bash/**`, `src/specify_cli/**`,
  `workflows/**`, `extensions/git/**`)
- Spectre.Console documentation: <https://spectreconsole.net/>
- Spectre.Console `0.57.0` release note:
  <https://spectreconsole.net/blog/2026-06-11-spectre-console-0-57-released>
- NuGet Spectre.Console.Cli `0.55.0`:
  <https://www.nuget.org/packages/Spectre.Console.Cli/>
- FSharp.Formatting content/frontmatter docs:
  <https://fsprojects.github.io/FSharp.Formatting/content.html>
