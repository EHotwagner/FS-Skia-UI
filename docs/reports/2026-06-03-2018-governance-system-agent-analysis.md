---
title: Governance System Agent Analysis
category: Design
categoryindex: 4
index: 16
description: Timestamped analysis of the FS.Skia.UI Spec Kit governance system as an agent-facing contract, including strengths, weaknesses, alternatives, and F# versus Markdown placement.
---

# Governance System Agent Analysis

- **Timestamp:** 2026-06-03T20:18:10+02:00
- **Author:** Codex
- **Status:** Analysis and recommendation, not an implementation plan
- **Scope:** The repository governance system used by Spec Kit agents: routing,
  FAKE targets, evidence graph and audit, skill loading, skill quality, generated
  guidance, template/generated-product checks, single-source generation, and the
  readiness artifact model.

## Executive Verdict

The governance system is heavy, but its weight is mostly in the right place:
it is an agent-facing contract, not a human-facing product workflow. Its job is
to give an autonomous agent deterministic commands, typed invariants, readiness
artifacts, and failure ownership. On that premise, the core design is sound.

A rewrite is **not warranted**. The project already completed the highest-value
rewrite: the old script/Python/Bash governance path was moved into compiled F#
under `FS.Skia.UI.Build`, with a dedicated build front-end, typed targets,
compiled routing, in-process evidence graph and audit, generated
`validation.contract.yml`, and generated `.claude` skill mirrors. Starting over
would mostly recreate those decisions and risk losing the hard-won parity and
tests.

What is warranted is a focused second pass:

1. Keep codifying mechanical rules in F#.
2. Keep Markdown for agent judgement, examples, and durable evidence.
3. Remove or regenerate stale active guidance that still names retired paths.
4. Add better machine outputs for agents, especially `Route --json`.
5. Make routing coverage explicit for `build/Governance/**` and other current
   governance homes, instead of depending on default-deny fallback behavior.

The most important conclusion: "put it in F#" should mean "make the invariant
typed, testable, and self-enforcing." It should not mean "hide prose inside F#
string literals and render the same long Markdown later."

## Premise: The User Is The Agent

This governance should not be judged like a human developer onboarding guide.
For a human, the system is too large. For a Spec Kit agent, the useful question
is different:

- Can the agent determine the authoritative gate for a change?
- Can it run the smallest valid proof?
- Can failures identify product, template, governance, stale prerequisite,
  unsupported host, missing evidence, or environment ownership?
- Can reviewers inspect durable artifacts instead of trusting the final chat?
- Can agent uncertainty be converted into deterministic validation?

The system answers many of those questions well. The burden is carried by the
agent and the build, not by the end user of a generated FS.Skia.UI app. That
does not make all complexity acceptable, but it changes the standard: the system
can be heavy if the heaviness buys determinism, observability, and safe failure.

## Current Architecture

### 1. Compiled Build Front-End

The root build no longer depends on a repository `build.fsx`. `fake.sh` runs the
compiled `build/Build.fsproj` executable. `build/Program.fs` registers every
FAKE target from the typed `Targets.dispatchTargets` list and delegates target
bodies to `Engine.Interpret.runTarget`.

That is a strong boundary:

- target names are `Targets.Target` union cases, not free strings;
- dependency rows are derived from the same target model;
- a mistyped gate fails at compile time;
- the build front-end is normal F# code with project references and tests.

Relevant sources:

- [`build/Program.fs`](../../build/Program.fs)
- [`build/Governance/Targets.fsi`](../../build/Governance/Targets.fsi)
- [`build/Governance/Targets.fs`](../../build/Governance/Targets.fs)
- [`build/Governance/Engine/Model.fsi`](../../build/Governance/Engine/Model.fsi)
- [`build/Governance/Engine/Update.fs`](../../build/Governance/Engine/Update.fs)
- [`build/Governance/Engine/Interpret.fs`](../../build/Governance/Engine/Interpret.fs)

### 2. `Route` As The Agent Entry Point

`Route` is the best architectural feature in the governance system. It reads
the union of branch-vs-`main` merge-base diff and current working tree changes,
runs the pure `Routing.selectForFeature` selector, and prints the tier plus the
minimal gate list.

The important design choices are correct:

- routine internal `src/**/*.fs` work routes to `inner-loop`;
- public `.fsi`, template, `.specify`, readiness, docs, skill, and generated
  guidance paths escalate;
- unknown non-inner-loop paths default-deny to a broad fallback;
- dogfood features can force the full serialized pipeline;
- `validation.contract.yml` is generated from the compiled routing table, not
  hand-maintained.

Relevant sources:

- [`build/Governance/Routing.fsi`](../../build/Governance/Routing.fsi)
- [`build/Governance/Routing.fs`](../../build/Governance/Routing.fs)
- [`build/Governance/ContractView.fs`](../../build/Governance/ContractView.fs)
- [`validation.contract.yml`](../../validation.contract.yml)
- [`tests/Governance.Tests/RoutingTests.fs`](../../tests/Governance.Tests/RoutingTests.fs)

### 3. Evidence Graph And Audit

The evidence system has been moved into compiled F#. It parses `tasks.md`,
`tasks.deps.yml`, the skill registry, skill-loading evidence, readiness files,
audit-status regions, audit patterns, and the unified git diff. It computes the
task graph, validates metadata, detects cycles, performs topological sort,
propagates synthetic evidence, and renders JSON/Markdown/audit artifacts.

This is exactly the kind of logic that belongs in F# rather than Markdown:
the rules are mechanical, central, and review-blocking.

Relevant sources:

- [`build/Governance/Evidence/Engine.fsi`](../../build/Governance/Evidence/Engine.fsi)
- [`build/Governance/Evidence/Engine.fs`](../../build/Governance/Evidence/Engine.fs)
- [`build/Governance/Evidence/Graph.fsi`](../../build/Governance/Evidence/Graph.fsi)
- [`build/Governance/Evidence/Graph.fs`](../../build/Governance/Evidence/Graph.fs)
- [`tests/Governance.Tests/EvidenceAlgorithmTests.fs`](../../tests/Governance.Tests/EvidenceAlgorithmTests.fs)

### 4. Single-Source Generation And Currency

The project has moved away from hand-synced duplicates:

- `.agents/skills/**` is canonical; `.claude/skills/**` is generated;
- `validation.contract.yml` is rendered from `Routing.fs`;
- constitution and governed template fragments are spliced/generated from
  canonical sources;
- target metadata is derived from the typed target registry and currency checked.

This is stronger than merely comparing two files. A drift check says "we have
two sources of truth and hope they agree." Generation says "there is one source
of truth and the other artifact is a view."

Relevant sources:

- [`build/Governance/SkillTreeGen.fs`](../../build/Governance/SkillTreeGen.fs)
- [`build/Governance/SkillSync.fs`](../../build/Governance/SkillSync.fs)
- [`build/Governance/GovernedBlocks.fs`](../../build/Governance/GovernedBlocks.fs)
- [`build/Governance/ConstitutionFragments.fs`](../../build/Governance/ConstitutionFragments.fs)
- [`build/Governance/TargetMetadata.fs`](../../build/Governance/TargetMetadata.fs)
- [`tests/Governance.Tests/SkillSyncTests.fs`](../../tests/Governance.Tests/SkillSyncTests.fs)

### 5. Skill Governance

The governance system treats skills as first-class inputs to agent work:

- task generation writes structured `skillist` metadata into `tasks.deps.yml`;
- `tasks.md` mirrors the visible skill list;
- implementation must load declared skills and record evidence;
- `EvidenceGraph` validates resolution, mirrors, ordering, and obvious omissions;
- `SkillQualityCheck` now checks FS-authored skills against a section rubric;
- `FS.Skia.UI.SkillSupport` gives the `fsharp-*` skills a shipped backing API.

This is heavy, but it targets a real failure mode: agents often skip specialized
local guidance unless the task itself forces discovery and loading.

Relevant sources:

- [`build/Governance/SkillQuality.fsi`](../../build/Governance/SkillQuality.fsi)
- [`build/Governance/SkillQuality.fs`](../../build/Governance/SkillQuality.fs)
- [`src/SkillSupport/SkillSupport.fsproj`](../../src/SkillSupport/SkillSupport.fsproj)
- [`src/SkillSupport/Graph.fsi`](../../src/SkillSupport/Graph.fsi)
- [`src/SkillSupport/Parsing.fsi`](../../src/SkillSupport/Parsing.fsi)
- [`src/SkillSupport/Globbing.fsi`](../../src/SkillSupport/Globbing.fsi)
- [`src/SkillSupport/CodeGen.fsi`](../../src/SkillSupport/CodeGen.fsi)
- [`src/SkillSupport/ShellProcess.fsi`](../../src/SkillSupport/ShellProcess.fsi)

### 6. Generated Product And Template Contract

The generated project path is governed as a consumer contract, not a convenient
sample. Template checks, generated product checks, capability catalog validation,
selected skills, package references, local packages, API surface bundles, and
evidence commands are all validated.

The versioned generated-product structural contract is particularly important:
it gives the system a deprecation model instead of turning every template change
into a hard break.

Relevant sources:

- [`build/Governance/GeneratedProductContract.fsi`](../../build/Governance/GeneratedProductContract.fsi)
- [`build/Governance/GeneratedProductContract.fs`](../../build/Governance/GeneratedProductContract.fs)
- [`build/Governance/GeneratedProduct.fs`](../../build/Governance/GeneratedProduct.fs)
- [`template/capabilities.yml`](../../template/capabilities.yml)

## Strengths

### Determinism Around Nondeterministic Agents

The governance system's core thesis is right. An agent can be creative in
implementation, but completion is checked by deterministic F# gates. This
reduces the chance that a final response claims readiness without proof.

The strongest examples are `Route`, `EvidenceGraph`, `EvidenceAudit`,
`TargetMetadataDrift`, `SkillSyncCheck`, and `SkillQualityCheck`.

### Minimal Gate Selection Is A Major Quality Improvement

The old failure mode was "run everything because missing proof is worse than
over-validation." `Route` changes that. A routine internal change can run `Dev`
only, while contract changes still escalate. That makes the system heavy only
when the changed surface justifies it.

For an agent, this is more useful than a shorter manual README. It gives the
agent an authoritative action.

### Typed Targets And Compiled Routing Remove A Whole Class Of Drift

The target union and `RoutingRule.RequiredGates: Targets.Target list` make
incorrect gate names hard to represent. This is a decisive improvement over
stringly typed YAML or Markdown rules.

### The Evidence Engine Is In The Right Language

Task parsing, dependency validation, cycle detection, topological sorting,
synthetic propagation, audit-status scanning, diff scanning, and report
rendering are all better as compiled F# than as prose or ad hoc scripts.

The graph tests include typed unit tests and property checks over the real
logic. That is a major strength because synthetic propagation is exactly the
kind of rule an agent might misunderstand if it lived only in text.

### Single-Source Generation Is The Right Drift Strategy

The generated `.claude` tree and generated validation contract are good
examples of a mature approach. Agents still receive Markdown where Markdown is
the right medium, but maintainers edit one canonical source.

### Readiness Artifacts Create Reviewable Memory

Per-feature `readiness/` files let a reviewer inspect what happened without
depending on chat history. This is especially important with agents because
the final answer is a lossy summary.

### Failure Ownership Is Explicit

Targets and contract rules carry failure-owner concepts such as product,
template, governance, missing evidence, unsupported host, and stale
prerequisite. That helps agents avoid wasting time on the wrong layer.

### Skills Are Treated As Operational Inputs

The skill governance system is opinionated, but it solves a real agent problem:
the agent must load the right specialized instructions before it edits code.
Structured `skillist` metadata plus evidence validation makes that enforceable.

### The Current System Is Already A Successful Partial Rewrite

The measured current shape is not the pre-refactor state:

- no tracked root `build.fsx`;
- no tracked `.py` evidence scripts;
- no tracked `run-audit.sh`;
- compiled governance and skill-support code under normal `.fsproj` projects;
- `.agents` and `.claude` skill trees present as canonical and generated views.

Live measurement during this report:

```bash
git ls-files 'build.fsx' '.specify/**/*.py' '**/run-audit.sh' \
  '**/compute-task-graph.py' '**/audit-status-scan.py'
# no tracked files

git ls-files 'build/Governance/*.fs' 'build/Governance/*.fsi' \
  'build/Governance/**/*.fs' 'build/Governance/**/*.fsi' \
  'build/*.fs' 'src/SkillSupport/*.fs' 'src/SkillSupport/*.fsi' \
  | sort -u | xargs wc -l | tail -1
# 13896 total

git ls-files 'tests/Governance.Tests/*.fs' 'tests/SkillSupport.Tests/*.fs' \
  | xargs wc -l | tail -1
# 8005 total
```

Those numbers show the governance system is substantial, but substantially
codified.

## Weaknesses And Risks

### The Agent-Facing Prose Is Still Large

The current `.agents/skills/**/*.md` plus `.specify/**/*.md` corpus measures
about 7,038 lines. The `.agents` plus generated `.claude` skill trees together
measure about 8,494 lines. That is no longer the old "rules only in prose"
problem, but it is still a lot of text for agents to rank, reconcile, and load.

This matters even if the burden is agent-borne:

- context is finite;
- tokens cost money;
- long prompts hide contradictions;
- stale active instructions can override newer architectural facts in the
  model's attention.

### Some Active Guidance Still Names Retired Paths

Repository search still finds active, non-historical guidance mentioning the
old root `build.fsx`, deleted Python scripts, and deleted `run-audit.sh`. Some
of this is harmless in historical reports and golden fixtures. Some is not
harmless because it lives in active skills or templates an agent may load.

Examples found during this analysis include:

- `.agents/skills/speckit-evidence-audit/SKILL.md` still describing the diff
  read as occurring at a `build.fsx` edge and naming
  `.specify/extensions/evidence/scripts/python/audit-status-scan.py`;
- `.agents/skills/speckit-tasks/SKILL.md` still showing a `run-audit.sh`
  command path;
- `.specify` plan templates still asking whether `build.fsx` changed.

This is a high-priority cleanup because it is active agent guidance, not merely
archive text. The current F# implementation can be correct while the agent is
still told to think in retired terms.

### Routing Has A Current-Path Coverage Gap

`validation.contract.yml` and `Routing.fs` still name the old `build.fsx` and
`scripts/build/**` build-target contract paths. The new governance code lives
under `build/Governance/**` and `build/Program.fs`.

Those paths are not silently accepted because `Route` default-denies unmatched
non-`src/**` paths to broad `Verify`, which is safe. But relying on default-deny
for the primary governance implementation is not as good as naming the rule
explicitly. It hides intent from both the agent and the generated contract.

Recommendation: add explicit `build/Governance/**`, `build/Program.fs`,
`build/Build.fsproj`, and possibly `build/**` routing coverage under a
governance/build-target contract rule.

### `Route` Output Is Human-Readable, Not Fully Machine-Readable

`Route` prints stable lines such as `tier=...` and `gates=...`. That is fine for
a chat agent, but the governance system is clearly moving toward machine
contracts. A JSON mode would be better:

```json
{
  "developer_class": "framework-author",
  "tier": "focused-authority",
  "gates": ["Dev", "EvidenceGraph"],
  "matched_rules": ["docs-only"],
  "expected_artifacts": ["readiness/validation-contract.md"],
  "dogfood_forced": false
}
```

This would let future tools run exactly the printed gates without string
splitting, and it would make final responses cite structured facts.

### `--enforce` Checks Artifact Presence, Not Full Freshness

The `Route --enforce` core checks whether expected artifact paths exist. That
is useful, but existence is weaker than freshness:

- the artifact may be from an earlier diff;
- the artifact may have been produced by a different gate set;
- the artifact may be present but stale after a generated source changed.

Some freshness is covered by specific gates such as `TargetMetadataDrift` and
`SkillSyncCheck`, but route-level enforce semantics should be documented as
"presence gate, not proof of current execution."

### The Current Feature Directory Is A Global State Coupling

Many gates write into the active feature resolved by `.specify/feature.json`.
That is convenient for Spec Kit, but it couples validation of a docs-only report
or a small internal change to the active feature's task graph and readiness
state. For an agent, this is usually acceptable because a Spec Kit run has an
active feature. For ad hoc analysis, it can be surprising.

Possible improvement: let `Route` or focused gates accept an explicit
`--feature <id>` override for validation-only runs, while keeping the current
default.

### Some Validators Remain Heuristic Text Scanners

Not everything can or should be parsed as a formal language, but several checks
still rely on substrings, headings, or simplified line grammars:

- `SkillQualityCheck` infers sections from headings, URL counts, code-fence
  counts, and `[[...]]` related links;
- `GeneratedGuidanceCheck` evaluates concept anchors and forbidden terms;
- `ValidationContract.parse` is a minimal scanner for the generated YAML view;
- route glob matching is custom.

These are pragmatic choices, but they can create false positives or false
negatives. The key is to keep them narrow and backed by tests.

### The Governance Library Has A Large Public-Looking Surface

`FS.Skia.UI.Build` is packable and consumed by generated products. That is
useful, but it raises the bar for separating stable consumer APIs from internal
repository machinery. `AgentValidation.fsi`, evidence APIs, generated product
contract APIs, and build-front-end modules should be reviewed as if accidental
public expansion matters.

The new `FS.Skia.UI.SkillSupport` library is a good pattern because it exposes
small, family-specific APIs through `.fsi` files. The build governance library
should keep moving in that direction: small signatures, fewer broad modules.

### Target Count And Concept Count Are Still High

The system has many named gates. That is not inherently wrong for an agent, but
it increases the chance that a failure is classified by the wrong target or that
two targets overlap awkwardly. `Route` mitigates this, but the next phase should
look for target consolidation only where two gates own the same evidence class.

Avoid consolidation for its own sake. It is useful only if it makes ownership
clearer or removes duplicated work.

## Alternatives

### Alternative A: Keep The Current System And Only Patch Stale Guidance

This is the lowest-risk option.

Pros:

- preserves the working architecture;
- avoids another large refactor;
- fixes the immediate agent-risk from stale active instructions;
- leaves compiled routing and evidence gates intact.

Cons:

- does not reduce overall guidance size much;
- leaves route output text-only;
- leaves some heuristic scanners in place;
- does not improve current-path routing coverage except where patched.

Verdict: reasonable short-term path.

### Alternative B: Full Rewrite Of Governance

Rewrite the governance system from scratch as a new tool, new schemas, new
targets, and new Spec Kit integration.

Pros:

- maximum freedom to simplify;
- can design a clean public API without historical baggage;
- can make JSON outputs and typed contracts first-class from day one.

Cons:

- high risk of losing parity with current evidence behavior;
- high risk of breaking generated projects;
- expensive to re-prove every rule;
- likely to rediscover the same need for routing, evidence graph, skill
  metadata, template validation, and generated artifacts;
- creates a long interval where old and new systems overlap.

Verdict: not warranted. The keystone rewrite already happened. Continue
incrementally.

### Alternative C: All-In F# Governance DSL

Represent most governance facts as F# records and discriminated unions, then
generate Markdown, YAML, reports, and skill skeletons from that model.

Pros:

- compile-time ids for gates, skills, evidence classes, and generated artifacts;
- one source of truth for more policy;
- easier to generate consistent Markdown for agents;
- easier to add JSON output beside Markdown output.

Cons:

- authoring prose in F# string literals is unpleasant;
- the agent still needs rendered natural language;
- over-modeling skill guidance could make the system rigid;
- high churn in prose would create noisy F# diffs.

Verdict: good for identifiers, rule metadata, target lists, generated skeletons,
and provenance. Bad as a wholesale replacement for agent-readable prose.

### Alternative D: Data-Driven YAML/JSON Schemas

Move rules from Markdown into structured YAML or JSON, validate schemas, and
let the build interpret the data.

Pros:

- easier to edit than F# for non-code authors;
- inert data, no arbitrary execution;
- can be consumed by external tooling;
- works for high-churn instance data such as task dependencies.

Cons:

- target and gate names become strings again;
- path predicates and rule composition are weaker than compiled F# functions;
- schema validity is not semantic validity;
- runtime parse errors are worse than compile errors for framework-owned policy.

Verdict: keep for agent-authored instance data such as `tasks.deps.yml`. Do not
use for framework-owned routing and target policy that can be compiled.

### Alternative E: CI-Only Governance

Move most governance to GitHub Actions or another CI pipeline and keep local
agent guidance minimal.

Pros:

- central enforcement;
- less local machinery;
- clearer separation between local editing and final validation.

Cons:

- slow feedback for agents;
- weak support for per-feature readiness artifacts during local work;
- poor fit for generated products that need local package validation;
- agents would still need local route decisions before pushing.

Verdict: CI should consume the same gates, not replace local governance.

### Alternative F: Vanilla Spec Kit With Minimal Custom Rules

Drop most custom governance and rely on upstream Spec Kit conventions.

Pros:

- lower maintenance burden;
- easier upstream compatibility;
- smaller local rule set.

Cons:

- loses FS.Skia.UI-specific public-surface, rendering, template, skill, and
  synthetic-evidence controls;
- weakens the agent contract exactly where this repo is unusual;
- would likely reintroduce prose-only compliance.

Verdict: not a good fit. The repo has already accepted a custom Spec Kit fork
stance for governance-critical behavior.

### Alternative G: Agent Prompt Only, No Gates

Use concise instructions and trust the model.

Pros:

- simplest possible system;
- fast local iteration.

Cons:

- no deterministic proof;
- no durable evidence;
- no failure ownership;
- high risk of false readiness claims;
- contradicts the framework's stated design philosophy.

Verdict: reject.

## F# Versus Markdown

### What Should Be In F#

Put a governance element in F# when it is:

- a closed set of identities, such as targets, tiers, required decision areas,
  failure owners, or evidence classes;
- a deterministic rule, such as route selection, skill section requirements,
  task dependency validation, synthetic propagation, or target metadata drift;
- a generator, such as `validation.contract.yml`, `.claude` skill mirrors,
  governed blocks, or generated contract headers;
- a parser for constrained syntax, such as `tasks.md` task lines,
  `tasks.deps.yml`, audit-status regions, or diff-scan patterns;
- a rule that should fail the build, not merely advise an agent.

The current system already follows this direction for the most important rules.
The remaining good candidates are:

- route output as typed JSON;
- explicit routing for `build/Governance/**`;
- stronger structured report metadata alongside Markdown readiness reports;
- a typed registry of active agent-facing documents and their retired-term
  cleanup obligations;
- generated skill skeletons from typed metadata, while keeping prose bodies in
  Markdown;
- stronger public/internal separation in `FS.Skia.UI.Build` signatures.

### What Should Stay In Markdown

Keep a governance element in Markdown when it is:

- natural-language agent instruction;
- a runnable example or recipe;
- a report intended for review;
- a spec, plan, task list, or readiness note;
- an explanation of why a rule exists;
- a context artifact that agents need to cite in final answers.

The reason is practical: agents consume natural language well. The problem with
Markdown is not that it exists. The problem is when Markdown carries a
machine-checkable invariant that should instead fail in F#.

### What Should Be Hybrid

Several artifacts should remain Markdown views generated or checked by F#:

- `validation.contract.yml`: keep as generated compatibility view, source in F#;
- `.claude/skills/**`: keep as generated Markdown, source in `.agents`;
- skill quality reports: Markdown output, F# rubric;
- task graph and audit reports: Markdown output, F# graph/audit engine;
- spec/plan/task templates: Markdown authoring, F# checks for required concepts;
- docs/reports: Markdown narrative, not a governance source of truth.

## Rewrite Assessment

### Is A Rewrite Warranted?

No. A ground-up rewrite would be a net negative today.

The major architectural correction already landed:

- root script removed;
- evidence Python/Bash removed;
- target model typed;
- routing compiled;
- generated contract derived from compiled policy;
- skill mirror generated;
- evidence graph and audit run in process;
- governance logic lives in normal F# projects.

That is the rewrite one would have recommended before the foundations work.
The remaining issues are cleanup, hardening, and information architecture, not
a need to start over.

### What Kind Of Refactor Is Warranted?

A focused hardening refactor is warranted:

1. **Active guidance cleanup:** remove stale active references to root
   `build.fsx`, deleted Python scripts, and deleted `run-audit.sh`.
2. **Routing coverage update:** explicitly route current governance homes such
   as `build/Governance/**` and `build/Program.fs`.
3. **Machine output:** add `Route --json` and possibly write
   `readiness/route-selection.json`.
4. **Freshness semantics:** document `--enforce` as artifact-presence only, or
   extend it with gate timestamp/hash provenance.
5. **Public API discipline:** review `FS.Skia.UI.Build` packable signatures and
   split stable consumer APIs from repo-internal front-end modules where needed.
6. **Guidance minimization:** shrink active skill prose after stale references
   are gone, but keep useful agent examples and sources.

## Final Recommendation

Keep the governance system. It is heavy, but it is heavy for a reason: it is the
agent's deterministic operating contract. The important rule is not "make it
small"; the important rule is "make the agent read only judgement guidance, and
make everything mechanical enforce itself."

Do not rewrite. Continue the current trajectory:

- compiled F# for rules, route selection, identities, parsers, validators, and
  generators;
- Markdown for agent judgement, examples, specs, tasks, and evidence reports;
- generated Markdown where duplication would otherwise drift;
- explicit `Route`-selected gates before validation;
- focused cleanup of stale active guidance and current-path routing gaps.

The governance system is now closer to the right architecture than to the old
problem it replaced. The next work should make it clearer, less stale, and more
machine-readable, not throw it away.
