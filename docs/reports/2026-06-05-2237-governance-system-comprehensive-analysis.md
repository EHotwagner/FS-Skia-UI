---
title: Governance System Comprehensive Analysis
category: Design history
categoryindex: 90
index: 17
description: Comprehensive analysis of the FS.Skia.UI agent-facing governance system, its mechanics, tradeoffs, alternatives, and an F# knowledge-system roadmap.
---

# Governance System Comprehensive Analysis

- **Timestamp:** 2026-06-05T22:37:32+02:00
- **Author:** Codex
- **Status:** Analysis and recommendation, not an implementation plan
- **Audience:** Agents and maintainers operating the FS.Skia.UI repository governance system
- **Baseline:** Builds on `docs/reports/2026-06-03-2018-governance-system-agent-analysis.md`; this report re-reads the current source and updates the assessment for the 2026-06-05 state.

## Executive Summary

The governance system is best understood as an **agent operating contract** for a repository whose changes often affect generated products, public package surfaces, evidence claims, local skills, and Spec Kit artifacts. It is not mainly a user-facing documentation system. Its job is to turn an agent's free-form development work into deterministic routing, typed gates, durable readiness artifacts, failure ownership, and machine-checkable claims.

The current architecture is heavy but coherent. The decisive design move has already happened: the old script/Python/Bash governance center was moved into compiled F# under `build/Governance/**`, with `fake.sh` delegating to a compiled `build/Build.fsproj` executable. Targets, route selection, evidence graph/audit, generated guidance checks, skill synchronization, package-surface checks, template validation, preflight checks, generated-product validation, publishing checks, and generated-product evidence execution now live mostly as typed F# modules.

The best parts are:

- `Route`: a compiled selector that reads the current diff and prints the minimal gate list.
- Typed `Targets.Target` and `Routing.RoutingRule` models: gate names are not free-form strings in the source of truth.
- Evidence graph/audit: task topology, skill metadata, synthetic propagation, readiness scans, and diff scans are computed in-process.
- Single-source generation: `validation.contract.yml`, `.claude/skills/**`, constitution fragments, governed prose blocks, API-surface docs, skill references, and typed controls catalog rows are generated or currency-checked from canonical sources.
- Readiness artifacts: per-feature proof is durable and reviewable instead of buried in chat history.

The main risks are:

- The active guidance corpus is still large: about **7,443 tracked lines** across `.agents/skills/*/SKILL.md` plus `.specify/**/*.md`; `.agents` plus generated `.claude` skill trees are about **9,222 tracked lines**.
- Some active guidance still mentions retired or ambiguous paths, especially root `build.fsx` and `dotnet fake`. Some `build.fsx` references are legitimate for `template/base/build.fsx`, but root-governance references should be tightened.
- `Routing.fs` still routes the old root `build.fsx`/`scripts/build/**` build-target contract path but does not explicitly route the current broad governance implementation home `build/Governance/**` except for publish/pre-publish files.
- `Route` is text-only; agents and tools would benefit from JSON and an explainable rule trace.
- `Route` selects from the whole dirty workspace. That is correct for merge-readiness, but it is too coarse for report authoring in a branch where another feature implementation is already in progress.
- `Route --enforce` checks artifact presence, not freshness or provenance.
- Some checks remain heuristic text scanners. That is acceptable where narrow and tested, but it is not the same as a structured contract.
- FAKE-backed gates are not concurrency-safe in a shared worktree. A second agent running Spec Kit implementation or validation can race with another governance run on `.fake`, build outputs, readiness artifacts, package caches, and active feature state.
- A pure F# facts/rules layer could bring real performance benefits for selection, explanation, stale-artifact detection, and next-action guidance, but it should not be confused with replacing proof gates such as tests, packing, template generation, or product validation.
- The packable `FS.Skia.UI.Build` library exposes a large public-looking surface. The distinction between generated-product APIs and repository-internal tooling should stay under active review.

The recommendation is not a rewrite. The system should continue moving mechanical governance into F#, but with a sharper boundary: F# should own identities, facts, rules, validators, generators, reports, route scopes, concurrency locks, and provenance; Markdown should remain for agent judgement, examples, specs, plans, tasks, and human-readable evidence. The next high-value step is a small **F# governance knowledge system**: a typed facts-and-rules layer that can answer "what changed?", "which gates?", "why?", "which artifacts?", "which skills?", "what is stale?", "is another governance run active?", and "what should the agent do next?" as structured data and rendered Markdown.

## What The Governance System Is

The governance system is a layered contract across six domains:

1. **Constitutional policy**: `.specify/memory/constitution.md` defines the repository's principles: Spec -> FSI -> tests -> implementation, `.fsi` visibility, simplicity, MVU/effect boundaries, synthetic-evidence disclosure, mandatory test evidence, and observable safe failure.
2. **Spec Kit authoring artifacts**: `spec.md`, `plan.md`, `tasks.md`, `tasks.deps.yml`, and `readiness/**` translate those principles into per-feature obligations.
3. **Agent skills**: `.agents/skills/**`, `.claude/skills/**`, `src/*/skill/SKILL.md`, and `template/fragments/*/skill/SKILL.md` describe specialized work instructions. Skill loading is part of implementation readiness, not optional reading.
4. **Compiled build/governance engine**: `FS.Skia.UI.Build` under `build/Governance/**` owns targets, routing, validation, evidence graph/audit, generated products, skill checks, preflight, publishing checks, and single-source generation.
5. **Generated product contract**: `template/**`, `template/capabilities.yml`, generated API-surface docs, generated product checks, and `template/base/build.fsx` govern what consumers receive from `dotnet new fs-skia-ui`.
6. **Reviewable evidence**: active feature readiness files record logs, task graphs, audit output, preflight diagnostics, generated product validation, FSI transcripts, package surfaces, template checks, and focused gate summaries.

For agents, this is a deterministic shell around nondeterministic work. The agent may choose how to implement, but it must prove completion through typed gates and durable artifacts.

## How It Works

### Entry Point: Compiled FAKE Front-End

The root `fake.sh` runs:

```bash
dotnet run --project build/Build.fsproj -- "$@"
```

`build/Program.fs` registers FAKE targets from `Targets.dispatchTargets`, wires dependency edges from `Targets.targetDependencyRows`, and delegates each target body to `Engine.Interpret.runTarget`.

The important design choice is that target identity is a closed F# union:

- source: `build/Governance/Targets.fsi`
- implementation: `build/Governance/Targets.fs`
- interpreter: `build/Governance/Engine/Interpret.fs`

This makes a mistyped target in the source policy a compile error. It also gives target metadata one canonical model.

### The Route Selector

The operational rule in `AGENTS.md` is correct: run `./fake.sh build -t Route` before validation, then run only the gates it prints.

`Route` works as follows:

1. The interpreter edge reads git state: branch-vs-`main`/`master` merge-base diff plus uncommitted/untracked working tree paths.
2. `Routing.selectForFeature` receives a pure `Diff`.
3. Compiled `Routing.rules` match path globs and select the highest applicable tier.
4. Required gates are de-duplicated in `Targets.allTargets` registry order.
5. The route report prints `developer-class`, `tier`, `gates`, `dogfood-forced`, and `matched-rules`.
6. `--enforce` additionally checks whether the selected rule's expected artifact paths exist.

Current tiers include `inner-loop`, `focused-authority`, `agent-ready`, `maintainer-verify`, and `automation-final`.

The rule set currently includes:

- `controls-public-surface` for `src/Controls/**`
- `generated-template` for `template/**`
- `evidence-governance` for task/readiness/evidence-extension paths
- `generated-guidance` and `specify-catchall` for `.specify/**`
- `docs-only` for `docs/**`, spec contracts, and quickstarts
- `package-surface` for public `.fsi` surfaces and surface baselines
- `skill-quality` for skill homes
- `build-target-contract` for root `build.fsx`, `scripts/build/**`, and `validation.contract.yml`
- `distribution` for publish/pre-publish sources and template package/version paths

`validation.contract.yml` is a generated compatibility view rendered by `ContractView.render` from `Routing.rules`, not an independent policy file.

### The Target Graph

The target graph mixes fast inner-loop work, focused proof, generated product checks, and broad aggregates.

Key targets:

- `Dev`: restore/build/test plus `SkillSyncCheck` through prerequisites.
- `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff`: public API proof.
- `TemplateCheck`: pack/install/instantiate/smoke the template matrix.
- `GeneratedProductCheck`: validate generated products and generated consumer behavior.
- `GeneratedGuidanceCheck`: validate spec/plan/task/skill guidance.
- `SkillSyncCheck`, `SkillQualityCheck`, `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck`: skill and skill-contract proof.
- `EvidenceGraph`, `EvidenceAudit`: task graph, synthetic propagation, readiness scans, and diff scans.
- `TargetMetadata`, `TargetMetadataDrift`: target metadata generation and drift detection.
- `PrePublishCheck`, `Publish`: distribution contract and idempotent publishing.
- `Verify`, `Ci`: broad aggregates with preflight and bootstrap checks.

The target bodies are modeled through a local MVU/effect algebra:

- `BuildModel`: paths, active feature, readiness directories, package directories, reports.
- `BuildMsg`: target start/completion/failure and operational events.
- `BuildEffect`: process execution, dotnet actions, template generation, report writes, evidence checks, route selection, publish, etc.
- `update`: pure transition.
- `interpret`: executes filesystem/process/git/environment effects at the edge.

This is a strong use of the repository constitution's Principle IV. The build workflow is stateful and I/O-heavy, so it is expressed as data and interpreted at the edge.

### Evidence Graph And Audit

`build/Governance/Evidence/**` is the most important formal subsystem.

Inputs are gathered at the interpreter edge:

- `tasks.md`
- `tasks.deps.yml`
- skill registry
- `readiness/skill-loading-evidence.md`
- readiness files
- audit-status regions
- audit pattern YAML
- unified git diff

The pure engine then:

- parses task lines and statuses;
- parses dependency metadata;
- validates task ids, deps, skill mirrors, and skill resolution;
- merges task records with skill ownership;
- detects cycles;
- topologically sorts tasks;
- propagates synthetic status to `[S*]`;
- computes accepted synthetic error-handling (`[SEH]`) summaries;
- scans readiness contract evidence;
- scans persistent launch, persistent GUI, and window visibility evidence;
- scans audit-status regions;
- scans the diff against blocking/advisory patterns;
- renders `task-graph.json`, `task-graph.md`, audit summaries, and hit JSON files.

This belongs in F#. The semantics are mechanical, review-blocking, and hard to express safely in prose.

### Skill Governance

Skills are not advisory-only. Task generation must write structured `skillist` metadata in `tasks.deps.yml`; `tasks.md` must mirror it visibly. Implementation must load the listed skills before code changes.

The skill system has several layers:

- `SkillRegistry`: discovers readable skills and maps ids.
- `Audit.validateSkillLoadingEvidence`: validates implementation-time loading proof.
- `SkillSyncCheck`: asserts `.claude/skills/**` is a current regeneration of canonical `.agents/skills/**`.
- `SkillQualityCheck`: checks FS-authored skills against required rubric sections.
- `SkillContractPathCheck`: validates skills that claim generated API-surface paths.
- `SkillistReference`: generates a reference document from the live registry and ownership vocabulary.

This is heavy but directly addresses a real agent failure mode: local specialized guidance is easy to skip unless task artifacts force it.

### Generated Product Governance

The generated product is treated as a consumer contract.

`template/capabilities.yml` maps capability ids to:

- package ids,
- projects,
- `.fsi` contracts,
- tests,
- skills,
- template fragments,
- dependencies,
- profiles,
- evidence classes,
- surface baselines,
- docs.

`Capabilities` validates the catalog. `ApiSurfaceGen` derives the generated `template/base/docs/api-surface/**` tree from the catalog's `contracts:` entries. `GeneratedProduct` packs, installs, instantiates, scans, and validates the generated product matrix.

Generated products do not copy the full governance implementation. Their `template/base/build.fsx` resolves the single `<FsSkiaUiVersion>` from `Directory.Packages.props`, reflection-loads the matching `FS.Skia.UI.Build` assembly, and invokes `Evidence.GeneratedRunner.run`. That keeps consumer evidence behavior aligned with the package version without copying Python or F# source into generated products.

### Single-Source Generation And Currency

This repository repeatedly uses the same pattern:

```
canonical source -> generated view -> currency check
```

Examples:

- `Routing.fs` -> `validation.contract.yml`
- `.agents/skills/**` -> `.claude/skills/**`
- `.specify/templates/constitution-template.md` -> `.specify/memory/constitution.md` and preset twin
- `.specify/memory/constitution.md` -> generated principle fragments in templates
- `GovernedBlocks.governedBlocks` -> repeated generated prose regions
- `CatalogGen.catalogFacts` -> `src/Controls/catalog.yml` and `src/Controls/Catalog.fs`
- `template/capabilities.yml` -> generated API-surface docs
- live `SkillRegistry` + owns vocabulary -> `docs/skillist-reference.md`

This is stronger than drift comparison alone. It reduces "two sources of truth" into one source plus rendered views.

## Inventory

### Main Source Homes

| Area | Source |
| --- | --- |
| Target identity and dependency graph | `build/Governance/Targets.fsi`, `Targets.fs` |
| Route selection | `build/Governance/Routing.fsi`, `Routing.fs` |
| Generated validation contract | `build/Governance/ContractView.fs`, `validation.contract.yml` |
| Build model/update/interpret | `build/Governance/Engine/**`, `build/Governance/Front/**` |
| Evidence graph/audit | `build/Governance/Evidence/**` |
| Capability catalog | `build/Governance/Capabilities.*`, `template/capabilities.yml` |
| Generated product validation | `build/Governance/GeneratedProduct.*`, `GeneratedProductContract.*` |
| Generated product evidence runner | `build/Governance/Evidence/GeneratedRunner.*`, `template/base/build.fsx` |
| Skill sync/quality/path checks | `SkillTreeGen.*`, `SkillSync.*`, `SkillQuality.*`, `SkillContractPath.*` |
| Generated guidance | `Guidance.*` |
| Target metadata | `TargetMetadata.*`, `Front/Helpers.fs` |
| Package surface diff | `PerPackageSurface.*` |
| Publish/pre-publish | `Publish.*`, `PrePublish.*` |
| Governed prose and constitution | `GovernedBlocks.fs`, `ConstitutionFragments.*` |
| Skill helper library | `src/SkillSupport/**` |

### Current Size

Measured from tracked files on 2026-06-05:

- Governance/build F# plus `src/SkillSupport`: **17,031 lines**
- Governance/SkillSupport tests: **9,219 lines**
- Active `.agents` skill files plus `.specify/**/*.md`: **7,443 lines**
- `.agents` plus generated `.claude` skill files: **9,222 lines**
- Existing spec/plan/task/deps files under `specs/**`: **201 files**

These numbers are large enough that "governance as text" cannot be trusted by attention alone. The typed gates are necessary.

## Pros

### Determinism For Agent Work

The strongest argument for the system is that it does not ask a model to remember the rules. It asks a model to run route-selected gates. That shifts compliance from memory to execution.

### Minimal Validation Instead Of Always-Verify

`Route` lets routine internal work run the light path while escalating contract changes. This keeps the system usable. Without `Route`, the governance burden would incentivize skipping validation.

### Compile-Time Gate Identity

`Targets.Target` and `Routing.RoutingRule.RequiredGates: Targets.Target list` are materially better than stringly typed YAML. A bad gate name fails at compile time in the source of truth.

### Pure Core, I/O Edge

The build engine, evidence engine, route selector, publish checks, surface diff, skill sync, and generated-guidance evaluators all try to keep deterministic logic pure and push filesystem/git/process reads to the edge. That makes the rules testable and explainable.

### Durable Readiness Memory

Feature readiness artifacts give reviewers a stable audit trail. This matters more for agent work than human-only development because the chat transcript is not a reliable system of record.

### Single-Source Generation

Generated views reduce drift across agents, templates, contracts, docs, and generated consumers. The controls catalog generation added after the prior report shows the pattern is spreading into product-facing metadata too.

### Failure Ownership

Target metadata and focused gate contracts carry failure ownership and category. Preflight and bootstrap checks help separate environment failures from product failures.

### Generated Consumer Alignment

Generated products use the packaged build engine for evidence graph/audit. That keeps consumer validation from becoming a stale fork of repository validation.

### Explicit Synthetic-Evidence Policy

Synthetic evidence is not merely discouraged; it is modeled, disclosed, propagated, and audited. The narrow `[SEH]` exception is structured and still visible.

## Cons And Risks

### Active Guidance Is Still Too Large

The active Markdown guidance corpus is large enough to hide contradictions. This is not fatal because many mechanical checks are in F#, but it is still costly for agents. The governance system should keep reducing prose that exists only to encode machine-checkable policy.

### Stale Or Ambiguous Root `build.fsx` References Remain

The root `build.fsx` is gone, but active files still mention it. Some references are historical or legitimate for generated products (`template/base/build.fsx`). Others are active prompts or route rules that still describe root build-governance paths in old terms.

The most important current issue is routing: `build-target-contract` names `build.fsx` and `scripts/build/**`, while the actual repository governance code lives in `build/Governance/**` and `build/Program.fs`. Unknown governance-path edits default-deny to broad `Verify`, so this is safe, but it is not explicit enough.

### Route Output Is Not Structured Enough

The plain text is readable:

```text
developer-class=framework-author
tier=focused-authority
gates=Dev, EvidenceGraph
```

But the system is mature enough to need:

- `Route --json`
- a route report artifact such as `readiness/route-selection.json`
- a rule trace listing changed paths, matched rules, gates, artifacts, and reasons

That would let agents and future tools consume the route without parsing prose.

### `--enforce` Is Presence-Oriented

`Route --enforce` checks whether expected artifact paths exist. It does not prove they were generated for the current diff, current commit, current gate set, or current feature state.

This is acceptable as a low-cost guard, but it should be documented precisely. A future provenance extension could store route selection, git SHA, command, exit code, started/finished timestamps, and input hash in each readiness artifact.

### Active Feature Coupling

Many gates write to the feature from `.specify/feature.json`. That is right for Spec Kit work, but it can surprise ad hoc report/documentation work. A docs-only report can route to `EvidenceGraph`, which validates the active feature's tasks, not the report itself.

Possible fix: let focused gates accept `--feature <path>` or let `Route` emit the feature it will validate.

### Whole-Workspace Routing Is Too Coarse For Report Authoring

The observed shortcoming is that `Route` intentionally reasons over the union of:

- branch-vs-default merge-base changes;
- uncommitted changes;
- untracked files.

That is a good model for merge-readiness. It answers "what must this worktree prove before it is safe to land?" It is a poor model for "what should I run after adding one report file?"

In a dirty feature workspace, a report-only change can inherit unrelated product, package-surface, controls, template, or Spec Kit changes made by another agent. The route can then escalate from a lightweight documentation check into `Dev`, package-surface checks, generated-product checks, controls catalog checks, `EvidenceGraph`, and `EvidenceAudit`. If those gates fail, the failure may be caused by the in-progress feature, not by the report. The agent then burns time validating and debugging work it did not touch.

The issue is not that the route selector is wrong. It is answering a different question than the report author needs. The system currently lacks a first-class distinction between:

- **authoring validation**: validate the files changed by this task, with minimal local checks;
- **merge validation**: validate the whole branch/worktree against the repository contract;
- **feature validation**: validate the active Spec Kit feature selected by `.specify/feature.json`;
- **generated-product validation**: validate consumer-facing generated outputs.

For report creation, the proper local check should usually be a scoped docs/report check: Markdown structure, frontmatter, broken local links when cheap, retired active-instruction terms when relevant, and maybe a route explanation artifact. It should not automatically validate the active feature task graph unless the report edits active Spec Kit artifacts or readiness evidence.

Concrete improvements:

- Add `Route --paths <path...>` for explicit path-scoped authoring decisions.
- Add `Route --staged` or `Route --since <base> --include-untracked=false` for intentional scope control.
- Add `Route --intent docs-report`, `--intent feature-work`, and `--intent merge-readiness`, with the default still conservative.
- Add a `DocsReportCheck` or `MarkdownReportCheck` that validates report files without touching `.fake` state-heavy product gates.
- Make `Route --json` report both "authoring gates" and "merge gates" when the two differ.
- Make `Route --explain` say when a docs-only edit is being escalated only because unrelated dirty paths are present.
- Record path provenance in readiness artifacts so stale or unrelated feature evidence is easier to identify.

This would make the governance system less burdensome without weakening merge safety. Agents could do quick local validation for their current artifact, then run full route-selected gates only when the branch is ready or when explicitly asked.

### AgentValidation Has A Parallel Selector

`AgentValidation.ValidationSelection.selectRules` parses `validation.contract.yml` and performs simplified pattern matching. The authoritative selector is `Routing.select`. Because `validation.contract.yml` is generated from `Routing.fs`, this is mostly a compatibility layer, but it is still a second implementation of selection semantics.

If external agents need the public `AgentValidation` API, consider making it call the same compiled routing facts directly or generating a richer structured rule model rather than re-parsing a YAML compatibility view.

### Heuristic Scanners Are Still Present

Some checks necessarily scan Markdown. The risk is when a human-prose heuristic becomes a hard gate:

- generated guidance concept anchors,
- skill quality heading/code/link heuristics,
- minimal validation-contract YAML parser,
- forbidden-stale-term scans,
- docs target-reference scans.

These can be pragmatic, but they should stay small, tested, and easy to explain. When a scanner starts modeling structured intent, move the intent into structured data.

### Public Surface Of `FS.Skia.UI.Build`

The governance library is packable because generated products consume the evidence engine. That is useful, but it means `.fsi` files are public-looking. There should be a clear split between:

- generated-product contract APIs that must remain stable,
- repository-internal build-front-end APIs that are implementation detail,
- test-only/provenance helpers.

Large `.fsi` surfaces such as `AgentValidation.fsi` should be reviewed with that in mind.

### FAKE State Serialization Is A Persistent Footgun

The docs correctly say FAKE-backed commands share `.fake` state and must not run concurrently. This is necessary but operationally brittle for agents, which otherwise parallelize aggressively. The route output could help by printing a serialized command plan and marking any non-FAKE checks as parallel-safe.

### Concurrent Governance Runs In One Worktree Are Unsafe

There are real thread-safety concerns when another agent is running Spec Kit implementation or governance gates in the same branch/worktree. This is not limited to theoretical deadlocks. The more likely problems are races, stale reads, interleaved writes, build-server contention, and nondeterministic failures, but hangs are also plausible when multiple build/test processes contend over shared state.

The shared resources include:

- repository `.fake` state;
- `dotnet` restore/build/test outputs under `bin` and `obj`;
- MSBuild and `dotnet build-server` state;
- NuGet/global package cache activity;
- generated package and template artifacts;
- active `.specify/feature.json`;
- feature `readiness/**` files;
- package-surface baselines and generated catalog files;
- logs and temporary directories used by FAKE target bodies.

If two agents run gates such as `Dev`, `PackageSurfaceCheck`, `PerPackageSurfaceDiff`, `TemplateCheck`, `GeneratedProductCheck`, `FsiTranscripts`, `EvidenceGraph`, or `EvidenceAudit` at the same time, either run can observe a moving workspace or partially written artifacts from the other. A failure can then be falsely attributed to the current task. In the worst case, one process can clean, overwrite, regenerate, or shut down build-server state while the other process expects it to exist.

During this report update, a non-governance process inspection found another agent already running a FAKE-backed package-surface gate in this worktree. That is exactly the scenario in which this report should not start any more governance gate activity.

Recommended controls:

- Treat all FAKE-backed targets as requiring an exclusive worktree-level lock.
- Add an advisory lock file, for example `.fake/governance.lock`, acquired by `fake.sh` or the compiled front-end before any target except read-only route/explain commands.
- Make `Route` or a future `GovernanceExplain` detect an active lock and print the owning PID, command, start time, and suggested action.
- Keep read-only analysis and file reads parallel-safe, but do not run FAKE targets concurrently in the same worktree.
- Use separate git worktrees or separate clones for independent agent threads that need to run gates.
- Make readiness writers atomic where possible: write to temp files, then rename.
- Include feature path, git SHA, command, start/end time, and route scope in generated readiness artifacts.

The immediate operational rule is simple: if another agent is running Spec Kit implementation or route-selected gates in this worktree, do not start governance gates. Continue with read-only investigation or use a separate worktree.

### Generated Product Reflection Is Clever

The generated `build.fsx` avoids a second version literal by reading `<FsSkiaUiVersion>` and reflection-loading the matching build engine. That solves a real pin-drift problem, but it is complex and can fail through global package cache or assembly-resolution issues. This is probably the right tradeoff for now, but it should remain heavily tested.

## Alternatives

### Keep Current Architecture And Harden It

This is the recommended path.

Pros:

- preserves working gates and parity;
- continues the typed F# trajectory;
- avoids breaking generated products;
- fixes the highest-risk issues incrementally.

Cons:

- does not immediately reduce total guidance size;
- leaves some complexity in place;
- requires disciplined maintenance of generated views and public surfaces.

### Full Rewrite

A new governance tool or new Spec Kit fork could be cleaner on paper.

Pros:

- clean API from day one;
- structured output could be built first;
- old stale references could be removed wholesale.

Cons:

- very high parity risk;
- likely reimplements the same target/routing/evidence/skill/template ideas;
- long period of duplicated systems;
- generated products may break;
- large validation cost.

Verdict: not warranted. The existing system has already completed the major rewrite from scripts to compiled F#.

### YAML/JSON Policy Files

Move more policy to structured data and interpret it.

Pros:

- easier to edit than F# for declarative facts;
- external tools can consume it;
- good for instance data like `tasks.deps.yml`.

Cons:

- gate names become strings unless generated from F#;
- semantic errors happen at runtime;
- functions such as path matching, tier selection, artifact derivation, and failure ownership become weaker or more complex.

Verdict: good for feature-authored instance data; not ideal for framework-owned policy that can be compiled.

### CI-Only Governance

Make GitHub Actions the main enforcement point.

Pros:

- centralizes enforcement;
- reduces local setup burden;
- easy to protect merges.

Cons:

- slow feedback for agents;
- weak per-feature readiness authoring;
- poor generated-product local validation;
- agents still need route decisions before pushing.

Verdict: CI should run the same gates, not replace local route-selected governance.

### External Policy Engine

Use OPA/Rego, Datalog, Prolog, Nix, Bazel/Starlark, or another rule engine.

Pros:

- naturally expresses facts and rules;
- may provide explainability and query APIs;
- can decouple policy from build implementation.

Cons:

- adds another language and toolchain;
- loses F# compile-time target identity unless carefully bridged;
- increases generated-product dependency complexity;
- most current rules already have good F# tests.

Verdict: not a good fit now. If the system needs Datalog-like inference, implement a small F# facts/rules layer first.

### Markdown-Only Agent Prompting

Shrink the system to instructions and trust agents.

Pros:

- simple;
- cheap to edit;
- no tooling burden.

Cons:

- no deterministic proof;
- no durable evidence;
- no reliable failure ownership;
- high risk of false readiness claims.

Verdict: reject.

## Could More Become F#?

Yes, but the right question is not "can prose be moved into F#?" It is "which decisions are facts and rules that agents should query rather than remember?"

### What Should Move Further Into F#

Move these further into F#:

- Route output as JSON and Markdown.
- Current governance path coverage as explicit routing facts.
- Active document registry: files that are active guidance, generated views, historical reports, or templates.
- Retired-term policy: root `build.fsx`, deleted scripts, old `dotnet fake` paths, etc., with scoped allowlists for historical docs and `template/base/build.fsx`.
- Evidence artifact registry: artifact id, producer target, stable/feature path, freshness/provenance fields, route rule that requires it.
- Skill registry facts: skill id, path, source home, canonical/derived status, owned capabilities, applies-to concepts, package/capability relation.
- Capability and package facts: package id, project, contracts, tests, templates, profiles, evidence classes, generated product obligations.
- Target/gate facts: target, prerequisites, cost, timeout, owner, authority, outputs.
- Task ownership facts: `owns:` vocabulary and implied skills.
- Risk-level inference: changed paths + feature metadata -> small/medium/broad risk, expected evidence, likely failure owner.

### What Should Stay Markdown

Keep these in Markdown:

- Skill bodies that teach an agent how to do specialized work.
- Specs, plans, tasks, and readiness notes.
- Explanatory docs and reports.
- Examples and recipes.
- Human rationale for why a rule exists.

F# can generate headers, tables, inventories, and required sections, but it should not become a dumping ground for long string-literal manuals.

## A Practical F# Knowledge System

The repository already has pieces of a knowledge system. The next step is to make that explicit.

### Proposed Model

Define a central governance facts model:

```fsharp
type GovernanceFact =
    | ChangedPath of string
    | ActiveFeature of string
    | Target of target: Targets.Target
    | RouteRule of id: string
    | RequiredGate of ruleId: string * target: Targets.Target
    | ExpectedArtifact of ruleId: string * artifactId: string
    | Skill of id: string * path: string * source: SkillSource
    | Capability of id: string * packageId: string option
    | Task of id: string * status: DeclaredStatus
    | TaskDependsOn of taskId: string * dependencyId: string
    | TaskSkill of taskId: string * skillId: string
    | TaskOwnsEvidence of taskId: string * evidence: EvidenceClass
    | ArtifactProducedBy of artifactId: string * target: Targets.Target
    | ArtifactObserved of artifactId: string * path: string * provenance: ArtifactProvenance
```

Then define rules as pure functions:

```fsharp
type GovernanceConclusion =
    | SelectedTier of Routing.Tier
    | SelectedGate of Targets.Target * reason: string
    | MissingArtifact of artifactId: string * reason: string
    | StaleArtifact of artifactId: string * reason: string
    | MissingSkill of taskId: string * skillId: string
    | SuggestedSkill of taskId: string * skillId: string * confidence: string
    | FailureOwner of owner: string * reason: string
    | NextAction of command: string * reason: string
```

This does not need a general-purpose Prolog. A small, typed rule engine can be enough:

- facts are gathered from git, `.specify/feature.json`, route rules, targets, skills, tasks, capabilities, and readiness files;
- rules are ordinary F# functions over immutable facts;
- conclusions are rendered to JSON and Markdown;
- every conclusion carries provenance: which fact/rule caused it.

### Performance Benefits Of A Pure F# Expert System

A pure F# expert system could bring meaningful performance benefits, but mainly by avoiding unnecessary work rather than by making expensive proof work magically cheap.

The current governance path often answers planning questions by starting FAKE targets or by re-running target logic that performs filesystem walks, git reads, Markdown parsing, YAML parsing, package-surface discovery, skill discovery, and readiness scans. Many of those operations are repeated across targets. A pure facts/rules layer could gather a single immutable `GovernanceSnapshot`, then answer multiple questions from that snapshot:

- selected route and matched rules;
- authoring gates versus merge gates;
- active feature path;
- expected artifacts;
- artifact presence and likely freshness;
- skills required by changed tasks;
- whether `.agents` and `.claude` trees are likely stale;
- whether a path is active guidance, generated view, fixture, or historical report;
- whether another governance run is active;
- next suggested command.

That can be fast because the heavy work becomes one bounded scan and parse pass, followed by in-memory pure evaluation. The likely savings are:

- fewer `dotnet run` and FAKE process startups for explain-only questions;
- fewer repeated repository-wide file scans;
- fewer repeated Markdown/YAML parses;
- fewer accidental product-gate runs for docs/report authoring;
- earlier detection that a different agent is already running an exclusive gate;
- clearer path-scoped decisions before any expensive target begins.

This is especially relevant for agents. A human can often decide "this is just a report" from context. An agent follows the written contract and may escalate to branch-wide validation unless the system gives it a structured, scoped answer. A fast F# expert layer could return that answer in milliseconds or low seconds, while reserving heavy FAKE-backed gates for proof.

The performance boundary is important:

- It can make `Route`, `Explain`, `ArtifactProvenance`, `SkillExplain`, and stale-evidence triage faster.
- It can prevent wrong-scope validation, which is often the biggest performance win.
- It can precompute facts that multiple targets currently rediscover.
- It cannot replace `dotnet test`, package packing, template instantiation, generated product execution, or API-surface proof.
- It should not become a generic inference engine whose overhead and opacity outweigh direct typed validators.

The best implementation shape is a compiled, pure `GovernanceSnapshot` plus typed rule functions. Expensive effects remain at the edge:

```fsharp
type GovernanceSnapshot =
    { Git: GitFacts
      ActiveFeature: FeatureFacts option
      ChangedPaths: ChangedPath list
      Targets: TargetFacts list
      RouteRules: RouteRuleFacts list
      Skills: SkillFacts list
      Capabilities: CapabilityFacts list
      Artifacts: ArtifactFacts list
      ConcurrentRuns: ConcurrentRunFacts list }

type GovernanceQuery =
    | ExplainRoute of scope: RouteScope
    | ExplainArtifacts of scope: RouteScope
    | ExplainSkills of feature: string option
    | ExplainConcurrency
    | ExplainNextActions of scope: RouteScope
```

The interpreter would gather facts once. The pure engine would answer queries repeatedly. Renderers would emit JSON and Markdown. FAKE targets would still execute the expensive proofs, but agents would be less likely to start them unnecessarily.

### Agent-Facing Commands

Useful commands could include:

- `./fake.sh build -t Route --json`
- `./fake.sh build -t GovernanceExplain`
- `./fake.sh build -t SkillExplain`
- `./fake.sh build -t EvidenceExplain`
- `./fake.sh build -t ArtifactProvenance`

Possible JSON shape:

```json
{
  "feature": "067-keyed-reconciliation",
  "changed_paths": ["src/Controls/Reconcile.fs"],
  "tier": "focused-authority",
  "matched_rules": [
    {
      "id": "controls-public-surface",
      "matched_paths": ["src/Controls/Reconcile.fs"],
      "required_gates": ["ControlsCatalogCheck", "PackageSurfaceCheck"],
      "expected_artifacts": ["readiness/typed-controls-front-door.md"]
    }
  ],
  "gate_plan": [
    {
      "target": "ControlsCatalogCheck",
      "command": "./fake.sh build -t ControlsCatalogCheck",
      "parallel_safe": false,
      "why": "controls-public-surface"
    }
  ]
}
```

### Why This Helps Agents

An agent needs answers more than prose:

- "What changed?"
- "What rule matched?"
- "What do I run?"
- "Can I run any of it in parallel?"
- "What artifact is missing?"
- "Is this failure product, template, governance, environment, stale prerequisite, or missing evidence?"
- "Which skill should I load and why?"
- "Which readiness file proves this claim?"

A typed knowledge system gives those answers without asking the model to infer them from hundreds of lines of instructions.

### Risks Of Overdoing It

Do not build a giant expert system that tries to judge everything. The bad version would:

- encode prose in F# strings;
- create opaque inference nobody can debug;
- introduce a new abstraction for every rule;
- replace clear direct validators with generic machinery;
- make ordinary changes require updating a central ontology.

The good version is modest: typed facts, typed conclusions, provenance, JSON/Markdown rendering, and reuse of existing validators.

## Recommended Roadmap

### Near Term

1. **Add explicit route coverage for current governance paths.** Include `build/Governance/**`, `build/Program.fs`, `build/Build.fsproj`, and wrapper/tooling paths under a governance/build-target rule. Keep `template/base/build.fsx` under distribution/template rules.
2. **Add `Route --json`.** Render selection, changed paths, matched rules, gates, expected artifacts, dogfood status, and developer class.
3. **Add route scope and intent.** Support path-scoped authoring decisions such as `Route --paths ...`, `Route --staged`, and `Route --intent docs-report`, while keeping whole-worktree merge validation available and conservative.
4. **Add a report/document authoring gate.** A `DocsReportCheck` or `MarkdownReportCheck` should validate report files without starting product, package-surface, generated-product, or active-feature evidence gates.
5. **Add governance-run locking.** Protect FAKE-backed targets with a worktree-level advisory lock and make read-only route/explain commands report active lock ownership.
6. **Write `readiness/route-selection.json` when `Route --enforce` runs.** Include commit/merge-base, working-tree dirty indicator, route scope, and whether unrelated dirty paths contributed to escalation.
7. **Clean active stale guidance.** Replace root `build.fsx` mentions in active prompts with compiled front-end wording, while preserving legitimate generated-product `template/base/build.fsx` references.
8. **Document enforce semantics.** Be explicit that it is artifact-presence unless/until provenance is added.
9. **Unify AgentValidation selection with Routing.** Avoid a simplified second selector where possible.

### Medium Term

1. **Introduce an artifact registry.** Each artifact should know producer target, expected path, route rule, and freshness fields.
2. **Add provenance to readiness artifacts.** Record target, command, feature, git SHA, merge-base, start/end time, exit code, and selected route.
3. **Create an active guidance registry.** Distinguish active instructions, generated views, historical reports, tests, and fixtures; apply retired-term scans only to active instruction homes.
4. **Typed skill facts.** Generate a skill inventory JSON from the live registry and use it in task generation and skill checks.
5. **Shared `GovernanceSnapshot`.** Gather git, route, skill, capability, artifact, active-feature, and concurrency facts once per command and feed route/explain/check logic from that snapshot.
6. **Public/internal split for `FS.Skia.UI.Build`.** Keep generated-product APIs stable and mark repository-internal APIs more deliberately.

### Long Term

1. **Governance knowledge base.** Gather facts from route, targets, capabilities, skills, tasks, artifacts, and readiness. Produce explanations and next actions.
2. **External tool integration.** Let CI and agents consume the same JSON route/evidence outputs.
3. **Policy explainability.** Every hard failure should be able to answer: rule id, fact matched, expected state, actual state, owning target, remediation command.

## Final Recommendation

Keep the system. It is heavy because it is doing real work for agents: route selection, evidence validation, generated-product proof, public-surface safety, skill loading, synthetic-evidence policing, and failure ownership. Replacing it with lighter prose would make it easier to read and easier to lie to.

The correct refinement is to make the heavy parts more structured:

- F# for identities, rules, facts, validators, generators, provenance, and JSON.
- Markdown for agent judgement, examples, plans, tasks, and human-readable evidence.
- Generated Markdown/YAML as views, not policy sources.
- Explicit route-selected gate plans, not implicit cultural knowledge.
- Scoped authoring validation for docs/report work, separate from whole-branch merge validation.
- Exclusive locks for FAKE-backed gates in a shared worktree.

The governance system is already closer to a useful expert system than to a docs folder. The next step is to admit that and give agents a queryable F# facts/rules layer with explainable outputs, scoped validation, concurrency awareness, and fast authoring checks, while resisting the temptation to move all natural-language guidance into code.
