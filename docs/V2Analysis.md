---
title: V2 Analysis
category: Design
categoryindex: 4
index: 7
description: Analysis of deterministic governance, FAKE build harness boundaries, Spec Kit workflow, and the remaining nondeterministic AI surface.
---

# V2 Analysis

FS.Skia.UI is not only an F# UI toolkit. It is also an experiment in moving as
much AI-assisted development as possible into a deterministic harness: curated
`.fsi` public contracts, semantic tests, FAKE targets, Spec Kit artifacts,
template drift checks, generated guidance checks, evidence audits, and
readiness records. That approach is feasible and useful, and this repository is
already unusually far along. The answer is not "fully achieved", though. The
project has strong deterministic governance around public surface, build
workflow, generated templates, and evidence collection, but the harness still
cannot prove all semantic intent. Some high-risk behavior remains represented by
synthetic models, path/section checks, and human-readable artifacts rather than
by executable contracts.

The current state is best summarized as: the project has achieved a practical
deterministic shell around nondeterministic AI work, but it has not yet made the
shell strong enough to treat AI output as merely an implementation detail.

## Research Scope

This analysis is based on three external references and the current repository:

| Source | Used for |
|--------|----------|
| [GitHub Spec Kit](https://github.github.com/spec-kit/) | The intended Spec Kit model: specification-driven development, structured artifacts, agent integrations, presets, extensions, and workflows. |
| [Spec Kit SDD concept](https://github.github.com/spec-kit/concepts/sdd.html) | The philosophy: intent-first specs, guardrails, multi-step refinement, and AI interpretation. |
| [Spec Kit CLI reference](https://github.github.com/spec-kit/reference/overview.html) | The command model: core commands, integrations, extensions, presets, and workflows. |
| [FAKE](https://fake.build/) | FAKE's role as an F# DSL for build tasks, target dependencies, modular build logic, and bootstrapping. |
| [FAKE API reference](https://fake.build/reference/index.html) | The available deterministic API surface, including `Fake.Core`, `Fake.DotNet`, target graph APIs, process execution, and filesystem helpers. |
| [FAKE Target module](https://fake.build/reference/fake-core-targetmodule.html) and [TargetOperators](https://fake.build/reference/fake-core-targetoperators.html) | The target/dependency model used by the local build graph. |

Local repository inputs reviewed:

| Area | Files and evidence |
|------|--------------------|
| Constitution | [.specify/memory/constitution.md](../.specify/memory/constitution.md) |
| Current plan | [specs/008-targeted-refactor-governance/plan.md](../specs/008-targeted-refactor-governance/plan.md) |
| Current spec and tasks | [spec.md](../specs/008-targeted-refactor-governance/spec.md), [tasks.md](../specs/008-targeted-refactor-governance/tasks.md), [research.md](../specs/008-targeted-refactor-governance/research.md) |
| Build harness | [build.fsx](../build.fsx), [fake.sh](../fake.sh), [fake.cmd](../fake.cmd) |
| Runtime source | [src/Lib](../src/Lib/), [src/Layout](../src/Layout/), [src/Charts](../src/Charts/) |
| New deterministic internals | [src/Lib/VulkanResources.fsi](../src/Lib/VulkanResources.fsi), [src/Lib/VulkanStartup.fsi](../src/Lib/VulkanStartup.fsi) |
| Governance scripts | [scripts/template-drift.fsx](../scripts/template-drift.fsx), [scripts/dependency-report.fsx](../scripts/dependency-report.fsx) |
| Tests | [tests/Governance.Tests](../tests/Governance.Tests/), [tests/Lib.Tests](../tests/Lib.Tests/), [tests/Layout.Tests](../tests/Layout.Tests/), [tests/Package.Tests](../tests/Package.Tests/) |
| Readiness evidence | [specs/008-targeted-refactor-governance/readiness](../specs/008-targeted-refactor-governance/readiness/) |

## The Intended Boundary

The project is trying to create this division of labor:

| Responsibility | Preferred owner |
|----------------|-----------------|
| Requirements, tradeoff exploration, documentation synthesis, feature slicing, and implementation proposals | AI plus human review |
| Public API shape | `.fsi` signatures, FSI transcripts, package surface baselines, and semantic tests |
| Stateful or I/O workflow shape | MVU-style models, messages, effects, pure `update`, and edge interpreters |
| Build and verification workflow | FAKE target graph plus shell wrappers |
| Template and generated-product governance | `dotnet new` template metadata plus generated project smoke checks |
| Spec Kit prompt quality | section-aware generated guidance checks |
| Template-owned drift | path-class alignment checks plus active feature evidence |
| Synthetic evidence | explicit disclosure, evidence graph, and evidence audit |
| Runtime native resource correctness | staged startup model, ownership ledger, tests, and real smoke evidence where available |

This is the right direction for the stated goal. AI remains useful where the
problem is open-ended, ambiguous, or design-heavy. Deterministic code owns the
places where the repository needs repeatable pass/fail decisions.

## Is It Achieved?

Partially, and meaningfully.

| Area | Verdict | Reason |
|------|---------|--------|
| Public API governance | Strong | `.fsi` files are the public contract, project files compile signatures before implementations, surface baselines exist, and package tests reject accidental exports. |
| Spec Kit governance | Strong | Specs, plans, tasks, graph validation, evidence audit, generated guidance, and readiness records create a real artifact chain before implementation. |
| FAKE command harness | Strong concept, medium implementation | `BuildModel`, `BuildMsg`, `BuildEffect`, `update`, and `interpret` put build behavior behind deterministic data, but the script is still large and not yet split into smaller compiled modules. |
| Template governance | Strong for non-visual scope | Source and package template paths, default and minimal profiles, generated project checks, dependency reports, guidance checks, and drift checks are all executable. |
| Generated guidance checks | Improved | The current build script parses Markdown headings and verifies prompts in the correct sections, deferred-scope placement, and active/preset parity. This is much better than substring-only checks. |
| Template drift checks | Improved, still imperfect | `scripts/template-drift.fsx` classifies changed paths and maps them to required alignment classes. It still relies partly on text evidence and same-diff path classes, not deep semantic comparison. |
| Runtime native ownership | Mixed | `VulkanResources` and `VulkanStartup` define a deterministic ledger and staged synthetic failure model, but most live Vulkan handles are still acquired and destroyed directly inside `VulkanHost.run`. |
| Yoga fallback diagnostics | Achieved for current surface | Recoverable Yoga failure now emits `FallbackBoundsApplied` through existing public diagnostic fields while preserving pure fallback bounds. |
| Full visual/runtime proof | Not achieved | Non-visual tests and smoke paths are useful, but they do not fully prove live Vulkan rendering quality, window behavior, GPU compatibility, or screenshot fidelity across environments. |
| AI nondeterminism containment | Good but not complete | The harness rejects many classes of bad output, but the AI can still produce plausible specs, readiness prose, and evidence text that satisfy checks without proving intent-level correctness. |

The important distinction is that the repository has made AI output auditable,
not deterministic. That is still valuable. The deterministic harness is a
review and rejection machine; it is not a replacement for judgment.

## What Works Well

The constitution is the strongest part of the design. It turns preferences into
reviewable rules: spec first, `.fsi` visibility, simple F#, MVU boundaries,
synthetic disclosure, mandatory tests, and structured diagnostics. That is a
good example of using natural language where it belongs: as a governing policy
that deterministic checks then enforce where possible.

The `.fsi` rule is especially effective. It is simple, compiler-backed, and
well matched to F#. The project does not scatter visibility decisions through
implementation files. Public shape lives in signatures and package surface
baselines, while implementation code can evolve behind them.

The FAKE harness is the right tool for the deterministic side. FAKE is F# code,
so the repository can model targets, command execution, filesystem effects,
reports, package checks, and generated project scans in one language. The local
`BuildModel`/`BuildMsg`/`BuildEffect` pattern is more testable than ad hoc shell
scripts because target intent can be inspected as values before the interpreter
runs side effects.

The Spec Kit integration matches the project goal. Spec Kit's public model is
not "ask AI once and trust it"; it is phased refinement through spec, plan,
tasks, and implementation artifacts. This repository adds more deterministic
constraints on top: generated prompt checks, task graph checks, evidence audit,
template drift rules, and readiness files.

The template work is pragmatic. Generated projects are not only inspected as
metadata; the repository creates source/default, source/minimal,
package/default, and package/minimal outputs and runs their local workflows.
That is exactly the kind of harness that catches AI or template drift without
depending on reviewer memory.

The current `008-targeted-refactor-governance` work improves previous weak
spots. Generated guidance validation is now section-aware. Template drift is
path-class aware. Yoga fallback is observable. Public record invariants are
inventoried with follow-up IDs instead of smuggled into an unrelated refactor.

## What Does Not Work Yet

The native startup model is not yet as deterministic as it looks. The new
`VulkanResources` and `VulkanStartup` modules are useful, but they are mostly a
symbolic ledger used by tests and readiness evidence. The live startup path in
`VulkanHost.run` still creates and destroys window, instance, surface, device,
swapchain, Skia context, and frame resources directly in a large function with
mutable state and a final cleanup block. That may be acceptable for this phase,
but it means the deterministic model is adjacent to the runtime, not fully
governing it.

`build.fsx` is still too large. The MVU/effect shape is good, but a single
script over 1,100 lines combines path resolution, target graph, process
execution, template packaging, guidance parsing, drift checks, package checks,
and self-checks. It is deterministic, but not as reviewable as it could be.
The plan acknowledges that a physical split may be brittle with FAKE loading.
That is a real tradeoff, but it leaves a maintenance hotspot.

The drift and guidance checks are stronger than before, but still gameable.
They parse headings and path classes, and they verify some active feature
evidence terms. They do not prove that the evidence is true, complete, fresh,
or causally connected to the changed behavior. A plausible paragraph can still
satisfy a text-evidence requirement.

Readiness artifacts are powerful but can become ritual. A deterministic harness
that checks "a file exists and contains the right words" is better than nothing,
but it can encourage documentation that exists for the gate rather than for
review clarity. This is the core danger of any AI-plus-governance workflow: the
AI can generate the artifacts that satisfy the gate unless the gate is tied to
fresh command output, concrete diffs, and executable assertions.

The visual evidence boundary remains intentionally deferred. That is reasonable
for V2 and for non-visual template validation, but it means the runtime quality
story is incomplete. Vulkan, Skia, swapchain, surface, window, and driver
behavior cannot be fully captured by non-visual tests or symbolic ledgers.

Public records are convenient but weak for invariants. The project correctly
keeps records idiomatic and easy to use, but many records can be constructed in
states that functions must either normalize or reject. The current invariant
inventory is a good intermediate artifact; it is not a substitute for future
validated helper APIs where invalid states matter.

## Deterministic Harness Strengths

The repository already has several high-value deterministic mechanisms:

| Mechanism | Why it matters |
|-----------|----------------|
| `.fsi` signatures | Compiler-enforced public boundaries. |
| Surface baselines | Detect accidental package-visible changes. |
| Expecto semantic tests | Verify behavior through public APIs. |
| FSI/prelude scripts | Exercise the library like a consumer would. |
| FAKE targets | One command surface for local and CI verification. |
| MVU/effect build model | Makes build intent inspectable as data. |
| Generated template smoke | Proves generated products can run their workflow. |
| Generated guidance check | Keeps future AI prompts aligned with repository policy. |
| Template drift check | Forces template-owned changes to update docs, specs, readiness, or deferrals. |
| Evidence graph/audit | Detects task graph defects and unresolved synthetic evidence. |
| Synthetic disclosure policy | Prevents fakes from quietly becoming "real" evidence. |

These are useful because they reduce the number of things reviewers must hold
in memory. The harness tells the reviewer where the change touches public API,
whether template-owned files drifted, whether the active plan mentions the
changed area, whether the generated prompts still ask required questions, and
whether evidence is real or synthetic.

## Nondeterministic Surface That Remains

The following parts still depend heavily on AI/human interpretation:

| Surface | Why it remains nondeterministic |
|---------|--------------------------------|
| Spec quality | A spec can be coherent but incomplete, overconstrained, or focused on the wrong risk. |
| Plan quality | A plan can satisfy template prompts while choosing weak implementation boundaries. |
| Readiness prose | A generated report can name evidence without proving that the evidence is meaningful. |
| Tradeoff judgment | Deciding whether to prefer deterministic strictness over expressivity is contextual. |
| Runtime architecture | Vulkan/Skia lifecycle design involves domain knowledge and environment-specific behavior. |
| Visual quality | A screenshot or live frame can be technically produced but visually wrong. |
| Public API ergonomics | `.fsi` can prove shape, not whether the shape is pleasant or durable. |

Trying to eliminate these surfaces entirely would make the project worse. The
better goal is to make them explicit, narrow, and reviewable.

## Feasibility And Usefulness

The approach is feasible if the project accepts three constraints.

First, the deterministic harness must be allowed to say "no" to convenient AI
output. That means some features will be harder to express, slower to land, or
split into smaller pieces because the harness cannot prove them yet. This is a
good tradeoff for a framework intended to become reusable infrastructure.

Second, the harness must stay simpler than the behavior it governs. A fake
build DSL, task graph, and evidence audit are useful when they catch real
classes of drift. They become harmful if maintaining the harness consumes more
attention than maintaining the product. The current repository is close to the
healthy side of that line, but `build.fsx` and template drift logic should be
watched carefully.

Third, deterministic checks must be connected to real artifacts. A path-class
check is useful. A path-class check plus command output, package diff, public
surface baseline, generated project run, and active feature traceability is
much better. The more the harness relies on prose alone, the easier it is for
AI to satisfy the form while missing the substance.

Used this way, the approach is useful. It turns AI from an unchecked code
generator into a proposal generator operating inside a repeatable rejection
system.

## Tradeoffs

The project deliberately gives up some expressivity.

Opinionated deterministic checks will reject changes that are probably fine but
not yet encoded in the harness. That is a cost. It is also the point. For this
repository, rejecting some valid-but-unproven changes is better than accepting
plausible-but-unreviewable changes.

The approach also favors small, reviewable features over broad exploration.
Spec Kit can support creative exploration, but this repository's constitution
pushes toward bounded specs, stable public surfaces, and evidence. That is a
good fit for package and template governance. It is less comfortable for early
UI design exploration, where a faster throwaway branch may be healthier.

Synthetic evidence is another tradeoff. Deterministic fake native handles are
the only practical way to force every Vulkan acquisition failure path. But they
can never prove the real driver, windowing layer, or Skia context. The current
policy is correct: synthetic evidence is allowed, loudly disclosed, and paired
with real smoke where possible.

The final tradeoff is cognitive load. The workflow has many artifact types:
specs, plans, tasks, dependency graphs, surface baselines, readiness logs,
template reports, drift reports, generated guidance reports, and follow-ups.
This is powerful for governance, but contributors need a clear mental model or
they will treat the process as bureaucracy.

## Alternatives

### Looser AI-First Workflow

The project could rely on AI-generated code plus human review and conventional
unit tests.

Pros:

- Faster for prototypes.
- Less process overhead.
- Easier for contributors unfamiliar with Spec Kit and FAKE.

Cons:

- Public API drift becomes easier.
- Template changes rely on reviewer memory.
- Synthetic evidence can hide.
- Generated products can break after source changes.

This is not a good match for the stated project intention.

### Fully Formal Specification

The project could encode more behavior in a formal model, property tests,
source generators, or a custom DSL that produces both tests and implementation
contracts.

Pros:

- Stronger determinism.
- Less room for prose-only evidence.
- Better at catching edge cases in small domains.

Cons:

- High upfront cost.
- Hard to apply to Vulkan/windowing/Skia behavior.
- Can become a second product.
- May reduce AI usefulness instead of channeling it.

This is worth considering only for narrow subsystems such as template drift,
public surface inventories, dependency metadata, and layout invariants.

### External Build Orchestrator Instead Of FAKE

The project could move governance to GitHub Actions, shell scripts, or a YAML
task runner.

Pros:

- Familiar CI integration.
- Less F# script complexity in the repository.
- Easier parallel jobs in hosted CI.

Cons:

- Workflow logic becomes distributed across YAML and shell.
- Harder to test as F# values.
- Less symmetry with the F# library and tests.

FAKE remains the better core harness for this repository. CI should call the
FAKE targets rather than replace them.

### Separate Template Repository

The governed `dotnet new` template could live in its own repository.

Pros:

- Cleaner package boundary.
- Less risk of source-only docs or readiness artifacts leaking into generated
  products.
- Simpler consumer-facing template history.

Cons:

- Synchronization burden.
- More release plumbing.
- Harder to prove source and template alignment in one local workflow.

This is a reasonable future release option, not a better current default.

### Machine-Readable Governance Manifests

Instead of relying mainly on Markdown readiness files, the project could record
feature requirements, evidence, drift classes, public records, and deferrals in
JSON or YAML, then render Markdown from that data.

Pros:

- Easier deterministic validation.
- Better freshness checks.
- Less prose-gate ambiguity.

Cons:

- More schema maintenance.
- Less pleasant for human-first review if overdone.
- Requires migration from current docs.

This is the most attractive next alternative because it strengthens the current
approach without replacing it.

## Recommended Improvements

### 1. Add Evidence Freshness Checks

Every readiness log should record the command, exit code, UTC timestamp, git
HEAD, dirty-worktree summary, and relevant input paths. The harness should fail
if a required evidence artifact predates a changed input file or was produced
against a different commit without an explicit deferral.

This would reduce the biggest weakness in AI-generated readiness prose: stale
or merely plausible evidence.

### 2. Move Governance Data Behind Schemas

Keep Markdown for review, but store key facts in machine-readable files:

- public record invariant inventory
- follow-up recommendations
- synthetic evidence inventory
- template drift path classes and alignment classes
- generated guidance prompt requirements
- feature traceability matrix

Then render Markdown from the structured data. FAKE can validate the schema and
the rendered docs.

### 3. Integrate Native Ownership With The Live Runtime

`VulkanResources` and `VulkanStartup` should eventually govern actual
`VulkanHost.run` acquisition and cleanup, not only symbolic tests. A good target
is a startup context that records each real acquire, transfer, and release
point, then emits the same ledger shape used by deterministic tests.

The goal is not a large disposal framework. The goal is one small real ledger
that removes the gap between test model and runtime behavior.

### 4. Shrink `build.fsx` Without Losing One Entry Point

The project should keep `build.fsx` as the canonical entry script, but move
testable logic into smaller F# modules or script includes if FAKE loading stays
reliable. If script splitting remains brittle, the next best option is stronger
named sections plus more self-checks around target/effect contracts.

The review target should be: a contributor can change generated guidance logic,
template drift logic, dependency governance, or target dependencies without
reading the whole build script.

### 5. Make Template Drift More Content-Aware

Path classes are useful, but the next level is content validation. Examples:

- docs changes that affect generated products must state include/exclude intent
- source changes must map to public surface, semantic tests, or explicit
  internal-only rationale
- template manifest changes must be compared against generated project scans
- readiness evidence must mention exact changed paths or stable feature IDs,
  not only generic terms like "documentation"

This should stay deterministic and small. Do not build a general semantic
understanding engine into the drift script.

### 6. Decide Template Ownership For Analysis Docs

Repository analysis docs such as this file should be deliberately classified.
Either generated products should include them because they teach the governance
model, or the template should exclude them because they are source-repository
review artifacts.

Leaving that decision implicit makes drift checks noisy and generated products
less predictable.

### 7. Add Visual Evidence As An Explicit Optional Gate

The project should keep non-visual validation as the default, but add an
opt-in visual/runtime target that records:

- OS and display server
- GPU and driver
- Vulkan instance/device/swapchain summary
- frame dimensions and hash
- screenshot artifact path
- whether fallback or unsupported-environment behavior occurred

This should not become mandatory on every workstation, but it should be
available for release readiness and renderer changes.

### 8. Add A Determinism Budget To Each Feature

Each plan should explicitly classify:

- what the AI may decide
- what the harness must prove
- what remains human judgment
- what evidence is synthetic
- what is intentionally impossible or too costly to prove

That would make the deterministic/nondeterministic boundary itself a first-class
review artifact.

## Strongest Current Pros

- The project uses F# signatures as a hard public contract instead of trusting
  implementation discipline.
- The build harness is executable and local, not only CI folklore.
- Spec Kit artifacts are not passive documents; they feed tasks, evidence, and
  validation.
- Generated products are exercised, not merely packaged.
- Synthetic evidence is named repeatedly instead of hidden.
- The current feature tightened previously weak governance checks.
- The approach is honest about deferred visual, release, and distribution work.

## Strongest Current Cons

- The deterministic harness is partly textual and can still be satisfied by
  plausible generated prose.
- The live Vulkan runtime is not yet fully governed by the new startup ledger.
- `build.fsx` is a large single-file control center.
- Full visual correctness remains outside the default pass/fail loop.
- Public record invariants are inventoried but not yet encoded as validated
  construction paths.
- The process has enough artifacts that contributors may optimize for passing
  gates rather than understanding the design unless the docs stay clear.

## Bottom Line

The approach is feasible and useful. It is especially useful for this project
because the product is both a framework and a template/governance system. The
right mental model is not "make AI deterministic." The better model is "make
AI proposals run inside a deterministic rejection harness."

The current repository mostly achieves that at the governance and public API
levels. It partially achieves it for runtime internals. The next step is to
reduce the remaining gap between deterministic models and live behavior:
freshness-stamped evidence, schema-backed readiness data, content-aware drift
checks, smaller build modules, and a real native ownership ledger wired into
`VulkanHost.run`.

Keep the bias opinionated and deterministic. Accept that some features will
become harder or impossible to express until the harness grows. That is a
reasonable price for a project whose central experiment is exploring where AI
expressivity should stop and executable structure should take over.
