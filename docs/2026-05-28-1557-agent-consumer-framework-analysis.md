---
title: Agent Consumer Framework Analysis
category: Design
categoryindex: 4
index: 7
description: Timestamped analysis of FS.Skia.UI as a Spec Kit agent-consumed framework, with recommendations for validation tiers, controls typing, and build graph design.
---

# Agent Consumer Framework Analysis

Timestamp: 2026-05-28T15:57:01+0200

This analysis evaluates FS.Skia.UI as a framework whose primary generated
consumer is an autonomous agent running Spec Kit, not a human developer typing
commands by hand. Under that assumption, the governance harness is not mainly a
usability burden. It is part of the framework contract: it gives an agent
command entry points, evidence paths, failure classes, and readiness obligations
that make completion auditable. The main risks are therefore not "too many
commands" or "too much ceremony" by themselves. The risks are slow routing,
ambiguous failure ownership, stringly typed public control contracts, and a
custom build target interpreter that duplicates parts of FAKE while remaining
harder for external tools to understand.

## Scope And Evidence

This document is based on local repository inspection and primary external
documentation.

Local sources:

- [README.md](../README.md)
- [docs/architecture.md](architecture.md)
- [docs/build.md](build.md)
- [docs/evidence.md](evidence.md)
- [docs/generated-apps.md](generated-apps.md)
- [docs/template-profile.md](template-profile.md)
- [specs/027-generated-evidence-workflow/plan.md](../specs/027-generated-evidence-workflow/plan.md)
- [build.fsx](../build.fsx)
- [template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs)
- [template/profiles/app.yml](../template/profiles/app.yml)
- [template/profiles/governed.yml](../template/profiles/governed.yml)
- [src/Scene/Scene.fsi](../src/Scene/Scene.fsi)
- [src/SkiaViewer/SkiaViewer.fsi](../src/SkiaViewer/SkiaViewer.fsi)
- [src/Controls/Types.fsi](../src/Controls/Types.fsi)
- [src/Controls/Attributes.fsi](../src/Controls/Attributes.fsi)
- [src/Controls/Control.fs](../src/Controls/Control.fs)
- [src/Controls/Charts.fs](../src/Controls/Charts.fs)
- [src/Controls/DataGrid.fs](../src/Controls/DataGrid.fs)
- [tests/Controls.Tests](../tests/Controls.Tests/)

External references:

- FAKE describes itself as an F# build task DSL and supports native target
  dependencies between targets.
  Source: <https://fake.build/>
- FAKE's target module documents `Target.create`, dependency operators such as
  `==>`, `Target.runOrDefault`, listing targets, `--single-target`, and
  `--parallel`.
  Source: <https://fake.build/guide/core-targets.html>
- Elmish describes the MVU core as UI-independent: immutable model,
  discriminated-union messages, pure init/update/view functions, and commands.
  Source: <https://elmish.github.io/elmish/>

## Reframed Consumer Model

If the consumer is a Spec Kit agent, framework quality is measured differently
than for a manually operated app starter.

Human-first questions:

- Is the generated project small?
- Can a developer understand the command list immediately?
- Is the entrypoint visually simple?

Agent-first questions:

- Can the agent determine the required proof from changed files and feature
  metadata?
- Can the agent run the smallest authoritative gate for the current change?
- Can failures be classified as product, environment, stale prerequisite,
  policy, or missing evidence without guesswork?
- Can generated evidence be re-created, validated, and cited?
- Can broad validation avoid rerunning expensive matrix rows when focused
  evidence is sufficient?

This repository already answers several of those questions better than a
minimal app template would. The current harness gives agents stable FAKE
targets, readiness directories, command logs, generated product validation,
evidence graph and audit outputs, and synthetic-evidence disclosure.

The improvement target should therefore be "more deterministic and cheaper for
agents" rather than "less governed".

## Current Strengths

### 1. The Runtime Boundary Is Sound

The conceptual runtime split is strong:

```text
Product Model/Msg/update/view
  -> Scene or Control declarations
  -> Viewer host edge
  -> window, input, Skia/Vulkan, screenshots, diagnostics, shutdown
```

This aligns with Elmish's UI-independent MVU description: immutable model,
message type, pure init/update/view, and commands. FS.Skia.UI does not require
product logic to own the native window, `SKCanvas`, GPU handles, or screenshot
machinery.

For agents, this is valuable because semantic tests can exercise reducers,
view functions, scene descriptions, control diagnostics, and layout evidence
without needing a reliable desktop session.

### 2. The Governance Harness Provides Real Agent Affordances

The FAKE target surface in [build.fsx](../build.fsx) defines named operations
that an agent can run and cite:

- `Dev`
- `PackageSurfaceCheck`
- `FsiTranscripts`
- `TemplateCheck`
- `CapabilityCheck`
- `SkillCheck`
- `GeneratedProductCheck`
- `GeneratedGuidanceCheck`
- `TemplateDrift`
- `EvidenceGraph`
- `EvidenceAudit`
- `Verify`
- `Ci`

The target outputs are not only pass/fail values. They also produce logs,
readiness Markdown files, JSON outputs, package surfaces, generated product
file lists, and focused-gate summaries. This is a strong design for an agentic
workflow because it creates durable evidence instead of relying on the final
assistant response.

### 3. Unsupported Host Behavior Is Treated As A Contract

The generated app and viewer evidence surfaces distinguish:

- persistent window launch
- bounded smoke
- deterministic scene evidence
- screenshot proof
- pixel readback
- unsupported desktop host
- product defect
- package resolution failure

That distinction matters for agents. Without it, an agent often treats a CI or
desktop-session limitation as a product failure, or worse, relabels a fallback
artifact as visual proof.

### 4. Generated Consumers Validate Packages, Not Source Copies

The generated consumer process uses local NuGet packages from `PackLocal`.
That is the right contract for a framework. It verifies the public package
surface a real generated product will consume, not repository internals.

### 5. Public `.fsi` Files Make Surface Review Concrete

The paired `.fsi` style creates a reviewable contract. This is especially
important for an agent-driven framework because agents tend to add helper
functions opportunistically. The `.fsi` boundary makes public API expansion
visible.

## Main Risks

### 1. Agent Routing Is Too Implicit

The repository documents many gates, but the gate selection rule is not yet a
first-class machine-readable contract. An agent can read [docs/build.md](build.md),
but it still has to infer which gates are authoritative for a given change.

Example: a change under [src/Controls](../src/Controls/) may need some subset
of:

- `Dev`
- `ControlsCatalogCheck`
- `ControlsInteractionCheck`
- `ControlsRenderingCheck`
- `PackageSurfaceCheck`
- `FsiTranscripts`
- `GeneratedProductCheck`
- `EvidenceGraph`
- `EvidenceAudit`
- `Verify`

Those requirements are understandable to a maintainer, but an agent needs an
explicit mapping from changed paths and feature risk to gates.

Risk: the agent either over-runs expensive broad validation or under-runs the
proof needed for final readiness.

### 2. Broad Validation Has High Latency And Large Failure Surface

`Verify` depends on many targets, including template validation, generated
product validation, dependency checks, guidance checks, drift checks, graph
generation, and audit. That is appropriate for final authority, but it is a
poor default inner loop.

The problem is not that broad validation exists. The problem is that without a
manifested routing contract, broad validation becomes the only obviously safe
choice for an agent that wants to avoid missing proof.

Risk: the agent spends most of its time proving unrelated matrix rows, and
environment-sensitive failures obscure product defects.

### 3. Some Generated App Policy Lives In The Product Entrypoint

[template/base/src/Product/Program.fs](../template/base/src/Product/Program.fs)
contains the normal product launch path plus many evidence modes:

- `--layout-evidence`
- `--launch-evidence`
- `--bounded-smoke`
- `--bounded-smoke-frame-diagnostics`
- `--scene-evidence`
- `--window-diagnostics`
- `--window-options`
- `--image-evidence`
- `--screenshot-evidence`
- `--pixel-readback-evidence`

This is not a human burden in the agent-consumer model, but it is still an
architecture risk. The product executable becomes both app and policy harness.
That makes the generated product's command-line surface part of governance
compatibility.

Risk: evidence policy evolution forces generated app churn and increases the
chance that a product command accidentally claims stronger proof than it has.

### 4. Controls Are Productive But Stringly Typed Internally And Publicly

The Controls surface exposes convenient typed modules such as `Button`,
`TextBox`, `LineChart`, and `DataGrid`, but the underlying contract is largely
string based:

```fsharp
type ControlId = string
type ControlKind = string

type Control<'msg> =
    { Kind: ControlKind
      Key: ControlId option
      Attributes: Attr<'msg> list
      Children: Control<'msg> list
      Content: string option
      Accessibility: AccessibilityMetadata option }

and Attr<'msg> =
    { Name: string
      Category: AttrCategory
      Value: AttrValue<'msg> }
```

`Attr.create` accepts arbitrary names. Event kinds are strings. Several data
paths use `UntypedValue of obj`. `ControlInternals.required` maps required
attributes from string control kinds to string attribute names. Event binding
normalization maps string attribute names such as `onClick` and `onChanged` to
event names such as `click` and `changed`.

This design is flexible and simple to extend. It is also weakly typed at the
point where agents most need compile-time guardrails.

Observed examples:

- `Attr.on: eventKind: string -> msg: 'msg -> Attr<'msg>`
- `Attr.onWith: eventKind: string -> (ControlEvent -> 'msg) -> Attr<'msg>`
- `Control.create: kind: ControlKind -> Attr<'msg> list -> Control<'msg>`
- chart values stored through `UntypedValue`
- DataGrid columns, rows, and visible range stored through `UntypedValue`

Risks:

- misspelled attribute names become diagnostics or missing behavior instead of
  compiler errors
- custom controls can bypass expected required attributes
- event names become a compatibility vocabulary without a typed owner
- agents may generate superficially valid controls with stale string keys
- surface baselines catch public names, but not semantic misuse of string
  attribute names

### 5. Build Graph Duplicates FAKE Target Infrastructure

The current [build.fsx](../build.fsx) implements a custom target system:

- `BuildModel`
- `BuildMsg`
- `BuildEffect`
- `update`
- `requiredTargets`
- `targetDependencies`
- `runWithDependencies`
- `targetFromArgs`
- effect interpreter

FAKE already provides target definition, dependency operators, target listing,
single-target execution, and parallel traversal support through its target
module. The repository's custom layer buys something useful: a pure transition
model that can be tested and a single place to emit structured reports. But it
also means external FAKE conventions are bypassed.

Risks:

- `fake build --list` does not naturally list true `Target.create` targets
- `--single-target`, native target arguments, FAKE context, build status, final
  targets, build-failure targets, and parallel options are either absent or
  reimplemented
- target names are string literals in several places
- dependencies are not typed and can drift from docs
- agents familiar with FAKE need repo-specific routing rules
- the custom runner becomes another framework to maintain

This is not automatically wrong. It is a deliberate tradeoff: testable build
workflow algebra versus idiomatic FAKE integration. The current implementation
leans heavily toward the custom algebra.

## Recommendations

### Recommendation 1: Add An Agent Validation Manifest

Add a machine-readable validation contract, for example:

```text
validation.contract.yml
```

The manifest should map path patterns, capability ids, and feature risk levels
to required gates, expected artifacts, timeout class, and failure ownership.

Sketch:

```yaml
version: 1
default:
  inner_loop: [Dev]
  final: [EvidenceGraph, EvidenceAudit, Verify]

rules:
  - id: controls-public-api
    paths:
      - src/Controls/**/*.fsi
      - src/Controls/**/*.fs
    gates:
      focused:
        - ControlsCatalogCheck
        - ControlsInteractionCheck
        - ControlsRenderingCheck
      surface:
        - PackageSurfaceCheck
        - FsiTranscripts
      generated_consumer:
        - GeneratedProductCheck
    artifacts:
      - readiness/control-catalog.md
      - readiness/interaction-tests.md
      - readiness/layout-rendering.md
    failure_owner: product

  - id: template-owned-change
    paths:
      - template/**
      - .template.config/**
      - template/profiles/**
    gates:
      focused:
        - TemplateCheck
        - GeneratedProductCheck
        - TemplateDrift
    failure_owner: template

  - id: speckit-evidence-workflow
    paths:
      - .specify/extensions/evidence/**
      - specs/**/tasks.md
      - specs/**/tasks.deps.yml
    gates:
      focused:
        - EvidenceGraph
        - EvidenceAudit
        - GeneratedGuidanceCheck
    failure_owner: governance
```

Agent benefit:

- the agent can choose the smallest authoritative gate
- final readiness can cite why a gate was required
- gate omissions become validation failures instead of reviewer discoveries

### Recommendation 2: Introduce Validation Tiers

Keep the existing target names, but formalize tier semantics.

Suggested tiers:

| Tier | Purpose | Example targets |
|------|---------|-----------------|
| `inner-loop` | fast product correctness | `Dev`, targeted tests |
| `focused-authority` | authoritative proof for one changed concern | `ControlsRenderingCheck`, `TemplateCheck`, `GeneratedProductCheck` |
| `agent-ready` | minimum feature-complete proof selected by manifest | path-derived focused gates plus `EvidenceGraph` and `EvidenceAudit` |
| `maintainer-verify` | broad repository confidence | `Verify` |
| `automation-final` | non-interactive final authority | `Ci` |

The new tier worth adding is `AgentReady`. It should not be a static clone of
`Verify`. It should read the validation manifest and run only required focused
gates plus evidence graph/audit.

### Recommendation 3: Produce One Consolidated Agent Verdict

Every focused and broad validation path should converge into one machine-readable
file, for example:

```text
readiness/agent-verdict.json
```

Minimum fields:

```json
{
  "status": "passed|failed|unsupported|degraded",
  "authority": "non-authoritative|focused-authoritative|broad-authoritative",
  "target": "ControlsRenderingCheck",
  "changed_rule_ids": ["controls-public-api"],
  "required_gates": ["ControlsRenderingCheck", "PackageSurfaceCheck"],
  "completed_gates": ["ControlsRenderingCheck"],
  "missing_gates": ["PackageSurfaceCheck"],
  "failure_owner": "product|environment|template|governance|prerequisite",
  "next_command": "./fake.sh build -t PackageSurfaceCheck",
  "artifacts": ["readiness/layout-rendering.md"],
  "diagnostics": []
}
```

This should supplement, not replace, the existing Markdown evidence. Agents
need a compact routing artifact, while reviewers still benefit from readable
reports.

### Recommendation 4: Keep Evidence Commands, But Move Policy Out Of Product Main

Generated products need evidence commands. They should not have to own all
evidence policy in `Program.fs`.

Recommended direction:

- keep product-owned facts in product modules:
  - `view`
  - `update`
  - `generatedHost`
  - layout facts
  - key mapping
- move command orchestration and report formatting into:
  - `FS.Skia.UI.Testing`
  - generated `Product.Evidence` module
  - FAKE targets

Preferred generated shape:

```text
src/Product/Program.fs          normal app launch and thin arg dispatch
src/Product/Evidence.fs         product evidence adapters
tests/Product.Tests             semantic checks
build.fsx                       orchestration and report contract
```

This keeps generated product code testable while reducing policy duplication
inside the app entrypoint.

### Recommendation 5: Gradually Type The Controls Contract

Do not remove the flexible `Control` and `Attr` representation immediately. It
is useful for generic rendering, diagnostics, catalogs, and generated controls.
Instead, add typed front doors while preserving the existing representation as
the lowered form.

#### 5.1 Add Typed Control Kinds

Replace public `ControlKind = string` usage at creation sites with a union or
single-case wrapper plus known values.

Option A: discriminated union:

```fsharp
type ControlKind =
    | TextBlock
    | Label
    | Button
    | TextBox
    | DataGrid
    | Chart of ChartKind
    | Custom of string
```

Option B: opaque wrapper:

```fsharp
type ControlKind = private ControlKind of string

module ControlKind =
    val button: ControlKind
    val textBox: ControlKind
    val custom: string -> ControlKind
    val value: ControlKind -> string
```

Option B is less disruptive because it preserves custom extensibility and
string output for catalogs.

#### 5.2 Add Typed Event Kinds

Introduce:

```fsharp
type ControlEventKind =
    | Click
    | Changed
    | Selected
    | TextCommitted
    | CustomEvent of string
```

Then expose:

```fsharp
module Attr =
    val onEvent: ControlEventKind -> 'msg -> Attr<'msg>
    val onEventWith: ControlEventKind -> (ControlEvent -> 'msg) -> Attr<'msg>
```

Keep `Attr.on` and `Attr.onWith` as compatibility APIs, but have typed modules
use `ControlEventKind`.

#### 5.3 Replace Common `UntypedValue` Paths

The highest-value typed attributes are chart and DataGrid data:

```fsharp
type AttrValue<'msg> =
    | TextValue of string
    | BoolValue of bool
    | FloatValue of float
    | StringListValue of string list
    | ChartSeriesValue of ChartSeries list
    | ChartPointValue of ChartPoint list
    | DataGridColumnsValue of DataGridColumn list
    | DataGridRowsValue of DataGridRow list
    | VisibleRangeValue of VisibleRange
    | MessageValue of 'msg
    | EventValue of (ControlEvent -> 'msg)
    | CustomValue of obj
```

Then keep `CustomValue` only for genuine extension cases.

#### 5.4 Add A Control Schema Registry

Today required attributes are in `ControlInternals.required` as string matches.
Move this into a schema table:

```fsharp
type ControlAttributeRequirement =
    { Name: AttributeName
      Category: AttrCategory
      Required: bool }

type ControlSchema =
    { Kind: ControlKind
      RequiredAttributes: ControlAttributeRequirement list
      SupportedEvents: ControlEventKind list
      AccessibilityRole: AccessibilityRole }
```

Agent benefit:

- generated controls can be validated against schema before rendering
- missing attributes can name typed requirements
- catalogs, docs, and diagnostics share one source of truth

#### 5.5 Keep The Lowered Form

The renderer can still consume:

```fsharp
Control<'msg>
Attr<'msg>
AttrValue<'msg>
```

The goal is not to make the renderer generic over many typed control records.
The goal is to give agents typed constructors that lower into the existing
representation.

### Recommendation 6: Reconcile The Custom Build Model With FAKE

There are three viable designs.

#### Option A: Keep The Custom Runner, Add A Manifest Layer

Keep `BuildModel`, `BuildMsg`, `BuildEffect`, and `runWithDependencies`.
Add explicit machine-readable metadata:

- target id
- description
- tier
- dependencies
- direct prerequisites
- output artifacts
- stale assumptions
- timeout class
- failure owner

Generate docs and agent verdicts from that metadata.

Pros:

- preserves current testable build algebra
- smallest change
- keeps existing governance tests mostly intact

Cons:

- still not idiomatic FAKE
- still bypasses native target listing and runner options
- the repository owns target traversal semantics forever

This is the pragmatic near-term path.

#### Option B: Use Native FAKE Targets, Keep Pure Planning Functions

Refactor so FAKE owns target registration and traversal:

```fsharp
Target.create "Dev" (fun _ -> runEffects (planTarget "Dev"))
Target.create "Verify" (fun _ -> runEffects (planTarget "Verify"))

open Fake.Core.TargetOperators

"Restore" ==> "Build" ==> "Test" ==> "Dev"
"EvidenceGraph" ==> "EvidenceAudit"
```

Keep pure functions:

```fsharp
val planTarget: BuildModel -> TargetId -> BuildEffect list
val targetMetadata: TargetId -> TargetMetadata
```

Pros:

- restores native FAKE conventions
- enables `--list`, `--single-target`, target arguments, and FAKE context
- reduces custom traversal code
- preserves testability of target planning

Cons:

- medium migration
- governance tests around custom traversal need updates
- target metadata must be kept in sync with FAKE registrations unless generated

This is the best medium-term design.

#### Option C: Move Build Orchestration Out Of FAKE

Use a purpose-built F# command-line executable for validation, and keep FAKE
only as a thin compatibility wrapper.

Pros:

- full control over agent manifest, JSON output, command routing, and failure
  classification
- easier to test as normal compiled code

Cons:

- larger change
- loses much of the benefit of choosing FAKE
- increases bootstrapping surface

This is only worth considering if the validation engine becomes too complex
for script-based FAKE.

Recommended path:

1. Near term: Option A plus target metadata and `agent-verdict.json`.
2. Medium term: Option B, native FAKE targets that call pure planning
   functions.
3. Avoid Option C unless FAKE script constraints become a direct blocker.

### Recommendation 7: Add Gate Cost And Authority Metadata

Agents need to reason about cost and authority. Add metadata such as:

```yaml
targets:
  Dev:
    tier: inner-loop
    cost: low
    authority: non-authoritative-final
    expected_duration: short
    failure_owner_default: product

  GeneratedProductCheck:
    tier: focused-authority
    cost: high
    authority: generated-consumer-authoritative
    expected_duration: long
    failure_owner_default: template

  Verify:
    tier: maintainer-verify
    cost: very-high
    authority: broad-authoritative
    expected_duration: long
    failure_owner_default: product-or-environment
```

This is more useful to an agent than prose alone.

### Recommendation 8: Make Environment Failures First-Class In The Verdict

The current docs already distinguish `environment-failure`. Extend that into
every focused gate and generated product row.

Important fields:

- `runner_stage`
- `desktop_session_required`
- `host_requirement`
- `unsupported_host_reason`
- `stale_prerequisite`
- `recommended_rerun_environment`
- `next_command`
- `product_code_touched`

Agent benefit:

- avoids editing product code when the runner is degraded
- makes retries intentional
- makes unsupported visual paths non-authoritative by construction

## Proposed Implementation Sequence

### Phase 1: Metadata Without Behavior Change

Add `validation.contract.yml` and target metadata. Teach existing
`GeneratedGuidanceCheck` or a new focused gate to validate:

- every documented target has metadata
- every metadata target exists in `requiredTargets`
- every focused gate declares outputs
- every changed path class maps to at least one validation rule
- `Verify` and `Ci` are not required as focused-gate prerequisites

No target behavior changes in this phase.

### Phase 2: AgentReady Target

Add:

```bash
./fake.sh build -t AgentReady
```

`AgentReady` should:

1. read the validation contract
2. compute changed-path rules from git diff or active feature metadata
3. run required focused gates
4. run `EvidenceGraph`
5. run `EvidenceAudit`
6. write `readiness/agent-verdict.json`

If changed-path detection is unavailable, it should degrade explicitly and
name the broad fallback command.

### Phase 3: Typed Controls Front Door

Add typed wrappers without removing compatibility APIs:

- `ControlKind` wrapper or union
- `AttributeName` wrapper
- `ControlEventKind`
- typed chart and DataGrid attr values
- schema registry

Update public module constructors first:

- `Button`
- `TextBox`
- `CheckBox`
- `Slider`
- `Tabs`
- `Menu`
- `LineChart`
- `DataGrid`

Keep `Attr.create` for advanced/custom cases, but move generated template code
to typed constructors.

### Phase 4: Native FAKE Target Registration

Introduce native `Target.create` wrappers around the pure target planner. This
can be done incrementally:

1. keep `planTarget` pure
2. register targets through FAKE
3. generate `targetDependencies` from metadata or remove custom traversal
4. keep command wrappers stable
5. update build workflow tests to assert native registration plus planning

## Recommendation Summary

The current architecture is directionally right for a Spec Kit agent. The
framework should not discard the harness. Instead, it should make the harness
more machine-routable.

Highest-value changes:

1. Add a validation manifest that maps paths and capabilities to required gates.
2. Add `AgentReady` as a manifest-driven middle tier between focused gates and
   full `Verify`.
3. Emit one compact `agent-verdict.json` for routing, authority, and next
   action.
4. Move generated evidence policy out of the product entrypoint where possible.
5. Add typed Controls front doors while keeping the current lowered
   representation.
6. Reconcile the custom build graph with native FAKE targets over time.

These changes preserve the core advantage of FS.Skia.UI for agent consumers:
the framework can prove its own generated products. The improvement is to make
that proof cheaper, more deterministic, and less dependent on implicit
maintainer knowledge.
