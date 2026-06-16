---
title: Design System Governance Domain Detailed Design
index: 29
description: Detailed design for a design-system governance domain (Ant-style design rules made machine-governable) built on the existing governance inference kernel, with a two-tier check model — deterministic plugin checks plus kernel-dispatched agent review for rules that need judgment.
---

# Design System Governance Domain Detailed Design

- **Timestamp:** 2026-06-16T09:58:27+02:00
- **Author:** Claude (Opus 4.8)
- **Status:** Detailed design, not implemented
- **Audience:** Maintainers and agents building the FS.Skia.UI design-system governance layer
- **Builds on:**
  - `docs/reports/2026-06-07-0838-governance-kernel-split-detailed-design.md` (the inference substrate, fact model, and agent-authorization shape this design reuses)
  - `docs/reports/2026-06-09-1538-ant-design-ui-story-adoption-analysis.md` (the design-system policy model, token taxonomy, and rule material this design governs)
  - `docs/reports/2026-06-10-1010-controls-architecture-evolution-roadmap.md`
- **Does not run any governance machinery.** This is a design document only. No `./fake.sh` targets, no `Route`, no evidence gates were executed.

## Executive Summary

A design system such as Ant Design is, in practice, **two kinds of rules wearing one
name**:

1. Rules that are *mechanically decidable* from source or captured output — design-token
   drift, color/contrast policy conformance, the spacing scale, control-height defaults,
   public token surface, "every `ButtonIntent` is consumed by the renderer."
2. Rules that require *judgment* — does this screen read as **Natural / Certain /
   Meaningful / Growing** (Ant's four values); does the rendered control actually match
   the spec's intent; is this the right page pattern; is the motion restrained; is color
   used for information rather than decoration.

The first kind is what a linter/plugin does (`stylelint`, `eslint-plugin-antd`). The
second kind is what a human reviewer does — and is exactly the band that a **separate
review agent** can now cover at scale.

This document designs a **design-system governance domain** that makes both kinds of rules
first-class, governed, and explainable, by reusing the inference kernel from the
2026-06-07 kernel-split design rather than inventing a new engine. The core move is that
**every design rule declares its check tier**, and the governance layer routes
accordingly:

```text
DesignRule.Tier =
  | Deterministic   -> the governance plugin checks it (pure, in the fixed-point evaluator)
  | AgentReviewed   -> the kernel ORDERS a separate review agent to check it, then reasons
                       over the recorded verdict as a supplied fact
  | HumanOnly       -> the kernel blocks and requests a human decision
```

The kernel stays pure. It never invokes an LLM. For an `AgentReviewed` rule it emits a
typed **`ReviewRequest`** (what to check, against which spec source, over which evidence
artifact). The Build edge interprets that request by dispatching a review subagent — the
same way the existing evidence system reads files at the edge and the kernel reasons over
supplied text. The agent's structured verdict is **recorded as an evidence artifact** and
fed back as a supplied fact, so re-evaluation is deterministic and CI is reproducible.

The answer to the framing question — *"is this like Ant Design, something describable by
rules that a governance plugin checks automatically or orders an agent to check?"* — is
**yes, and the existing kernel is already the right shape to host it.** This design is the
third leg: kernel (engine) + Ant adoption (domain material) + **design-system governance
(the domain that binds them).**

## Position And Non-Goals

### The primary decision

Model a design system as a **governed rule domain over the existing inference substrate**,
with a *checkability classification* on every rule that decides whether it is enforced by
the deterministic plugin, by a dispatched agent, or by a human.

It should be:

```text
DesignSystemFact union  (a domain fact set, like GovernanceFact)
  + DesignRule records carrying a CheckTier
  + the existing FixedPoint.evaluate (no new evaluator)
  + ReviewRequest emission for judgment rules
  + recorded agent verdicts as supplied facts (reproducible)
  + provenance-rich design explanations
```

It should **not** be:

```text
a second rule engine
  + an LLM call inside the kernel (kills purity, determinism, replay)
  + a bespoke design-lint DSL
  + "ask an agent every time" (kills reproducibility and costs unboundedly)
```

### Non-goals

- This does not change `Theme`, `DesignTokens.fsi`, or any public control surface. It
  governs them; it does not redesign them.
- This does not implement the Ant token algorithm or the style resolver. Those are the
  subject of the Ant adoption roadmap; this layer *checks* their outputs.
- This does not introduce a new package before there is a second consumer. Per the kernel
  split doc, the substrate stays in `Governance.Core`; this domain is a module cluster
  inside it.

## How This Relates To The Two Prior Designs

| Prior design | What it provides | What this design adds |
|--------------|------------------|------------------------|
| Governance kernel split (2026-06-07) | `RuleId`/`FactId`/`RepoPath`, `ProvenanceStep`, `FactSet`, `Rule<'fact>`, `FixedPoint.evaluate`, `AgentAction`/`AgentDecision`, the closed-union + active-pattern discipline | A new `'fact` domain (`DesignSystemFact`), a `CheckTier` on rules, and a `NeedsDesignReview` decision that turns the kernel's "may the agent do this?" into "must an agent *check* this?" |
| Ant Design adoption (2026-06-09) | `DesignSystemPolicy` (`Wcag`/`Ant`/`Material`/`Fluent`), `ColorPolicy`, token taxonomy (seed→map→alias→component), the four values, interaction states, page patterns, the `fs-skia-ant-design` skill | The *governance* of all of the above: each Ant idea becomes one or more `DesignRule`s with an explicit tier, a spec source, and a verdict path |

The dependency direction is clean: the design-system governance domain `open`s the
inference substrate and the governance domain (`Targets`, `Tier`), and consumes the
`DesignSystemPolicy` type. It is one more `'fact` instantiation of a substrate that was
explicitly designed (kernel doc §"Standalone Engine Reuse Path") to host more than one
domain.

## The Three-Tier Check Model

The single new idea is that **checkability is a property of the rule, declared up front**,
and the governance layer is the thing that knows how to satisfy each tier.

```text
                         design change (tokens / fsi / showcase spec / rendered control)
                                              |
                          Build edge: normalize to supplied DesignSystemFacts
                                              |
                                  FixedPoint.evaluate (pure)
                                              |
                 +----------------------------+----------------------------+
                 |                            |                            |
        Deterministic rules          AgentReviewed rules           HumanOnly rules
                 |                            |                            |
        DeterministicVerdictFact     ReviewRequestFact (if no         DesignBlockerFact
        (Pass / Fail + reason)       fresh recorded verdict)          (NeedsHuman)
                 |                            |                            |
                 |                  Build edge dispatches review          |
                 |                  subagent; records DesignVerdict       |
                 |                  as an evidence artifact               |
                 |                            |                            |
                 |                  re-evaluate with RecordedReviewFact    |
                 |                            |                            |
                 +----------------------------+----------------------------+
                                              |
                              ExplainDesign / AuthorizeAgentAction
                                              |
                              Build edge: print, write artifacts,
                              decide which gates pass
```

Two invariants make this trustworthy:

1. **The kernel never calls an agent.** It only *describes* the review it needs. Dispatch
   is an effect at the Build edge, exactly like reading `tasks.md` is an effect today.
2. **A recorded verdict is just supplied text.** This is the same discipline the existing
   evidence engine already uses (kernel doc §"Evidence Engine Design": *read files at
   Build edge, parse/evaluate/render in Core, write artifacts at Build edge*). The agent's
   verdict is recorded as an artifact, hashed against the spec and the inspected artifact,
   and replayed. CI does not re-run the agent unless inputs changed.

## Domain Model

All types below live alongside the governance domain described in the kernel split doc
(`namespace FS.Skia.UI.Governance.Design`), and reuse `RuleId`, `FactId`, `ArtifactId`,
`RepoPath`, `ProvenanceStep`, `Rule<'fact>`, and the `Targets.Target`/`Tier` unions.

### Policy and check tier

```fsharp
namespace FS.Skia.UI.Governance.Design

// Selected design language. Mirrors the Ant-adoption DesignSystemPolicy so the template
// parameter (--design-system wcag|ant|...) and the governance rule set agree.
type DesignSystemPolicy =
    | Wcag
    | Ant
    | Material
    | Fluent
    | CustomPolicy of string

type CheckTier =
    | Deterministic          // the governance plugin decides it, purely
    | AgentReviewed          // the kernel orders a review agent; verdict is recorded
    | HumanOnly              // the kernel blocks and asks a human

// The thing a rule is asserted against. Every verdict cites one.
type SpecSource =
    | LocalPolicy of policyDoc: RepoPath          // e.g. the wcag/ant ColorPolicy table
    | AntSpec of url: string                       // e.g. https://ant.design/docs/spec/colors/
    | ShowcaseSpec of RepoPath                      // docs/testSpecs/Showcase/*.md
    | TokenContract of RepoPath                      // design-tokens.tokens.json / DesignTokens.fsi

// The concrete artifact a rule inspects.
type DesignArtifactRef =
    | TokenDocument of RepoPath                      // DTCG source
    | GeneratedTokenSurface of RepoPath              // DesignTokens.fs / .fsi
    | PublicControlSurface of RepoPath               // Types.fsi etc.
    | RenderedCapture of controlOrPage: string * RepoPath   // evidence screenshot / IR dump
    | ShowcasePage of RepoPath
    | GeneratedApp of profile: string
```

### Rules

A `DesignRule` is data plus, for the deterministic tier, a pure check; for the agent tier,
a review-request builder. This follows the existing `RoutingRule` pattern (a record with
an embedded function), not an `IRule` interface — the kernel doc is explicit that facts
and rules stay closed unions and records, never interfaces (§"Poor Interface
Candidates").

```fsharp
type DeterministicCheck =
    DesignArtifactRef -> DesignArtifactInputs -> DesignVerdict

// What a judgment rule asks an agent to decide. Pure data; carries no agent.
type ReviewRequest =
    { Id: RuleId
      Policy: DesignSystemPolicy
      Spec: SpecSource
      Artifact: DesignArtifactRef
      Question: string                 // the reviewer's instruction
      SpecHash: string                 // hash of the spec source at request time
      ArtifactHash: string }           // hash of the inspected artifact at request time

type DesignRule =
    { Id: RuleId
      Description: string
      Policies: DesignSystemPolicy list   // which policies this rule applies under
      Tier: CheckTier
      Spec: SpecSource
      Inspects: DesignArtifactRef list
      // present iff Tier = Deterministic
      Check: DeterministicCheck option
      // present iff Tier = AgentReviewed; builds the request, does not run it
      Review: (DesignArtifactRef -> ReviewRequest) option }
```

### Verdicts and facts

```fsharp
type DesignVerdict =
    | Pass
    | Fail of reason: string
    | Uncertain of reason: string        // agent low-confidence; treated as blocking

// A verdict produced by a review agent and persisted as an evidence artifact.
type RecordedReview =
    { Request: RuleId
      Verdict: DesignVerdict
      Reviewer: string                   // model/agent id, e.g. "claude-opus-4-8"
      Confidence: float
      Rationale: string
      CitedSpec: SpecSource
      SpecHash: string                   // inputs as seen by the reviewer
      ArtifactHash: string }

type DesignSystemFact =
    | SelectedPolicyFact of DesignSystemPolicy
    | DesignArtifactFact of DesignArtifactRef
    | ChangedDesignPathFact of RepoPath
    | DesignRuleFact of RuleId * CheckTier
    | DeterministicVerdictFact of rule: RuleId * verdict: DesignVerdict
    | ReviewRequestFact of ReviewRequest
    | RecordedReviewFact of RecordedReview
    | StaleReviewFact of rule: RuleId * reason: string
    | DesignBlockerFact of rule: RuleId * reason: string
    | NextDesignActionFact of command: string * reason: string

module DesignSystemFact =
    val identify: DesignSystemFact -> FactId
    val describe: DesignSystemFact -> string
```

This is one closed union supplying its own identity function — exactly the pattern
`GovernanceFact` uses (kernel doc §"Layer 2"). Adding a fact kind forces every rule
module, query, and renderer to acknowledge it.

## Rule Catalog: Ant Design As Governed Rules

The Ant adoption analysis is the rule source. Each durable idea becomes one or more
`DesignRule`s with an explicit tier. The table below is the heart of the design — it shows
*which Ant rules the plugin checks itself and which it hands to an agent.*

| # | Design rule (from Ant adoption doc) | Tier | Spec source | Inspects | Existing target |
|---|--------------------------------------|------|-------------|----------|-----------------|
| 1 | Generated tokens match DTCG source (no drift) | Deterministic | TokenContract | TokenDocument, GeneratedTokenSurface | `DesignTokenDrift` |
| 2 | Color/contrast conforms to the **selected policy** (`wcag` ratios *or* `ant` pairings) | Deterministic | LocalPolicy | GeneratedTokenSurface | `ContrastCheck` (→ policy-backed) |
| 3 | Spacing uses the semantic scale (`xs/sm/md/lg/xl`), not ad-hoc sizes | Deterministic | LocalPolicy | TokenDocument | (new) |
| 4 | Control height defaults to 32 / density modes are `Comfortable/Middle/Compact` | Deterministic | AntSpec(layout) | TokenDocument | (new) |
| 5 | New token names route through public-surface gates | Deterministic | TokenContract | PublicControlSurface | `RefreshSurfaceBaselines` / surface checks |
| 6 | Every `ButtonIntent`/`ControlIntent` is actually consumed by the renderer (no lowered-but-undrawn intent) | Deterministic | ShowcaseSpec | PublicControlSurface, RenderedCapture | `ControlFidelityCheck` |
| 7 | Every `VisualState` (hover/focus/pressed/disabled/selected/loading/validation) has a resolved style | Deterministic (coverage) + AgentReviewed (correctness) | AntSpec(reaction) | RenderedCapture | `ControlsInteractionCheck` |
| 8 | Rendered control **matches the spec's visual intent** | AgentReviewed | ShowcaseSpec / AntSpec | RenderedCapture | `ControlsRenderingCheck` |
| 9 | Screen embodies the four values — **Natural / Certain / Meaningful / Growing** | AgentReviewed | AntSpec(values) | ShowcasePage, GeneratedApp | (new) |
| 10 | Page uses the right **pattern** (list/form/detail/workbench/result/exception) | AgentReviewed | AntSpec(research-*) | GeneratedApp | (new) |
| 11 | Color is used for **information, not decoration**; primary/info/link/selection roles are distinct | AgentReviewed | AntSpec(colors) | ShowcasePage, RenderedCapture | (new) |
| 12 | Motion is restrained and explains cause/effect; evidence-friendly | AgentReviewed | AntSpec(motion) | RenderedCapture | (new) |
| 13 | Elevation/overlay used for layering only, not decoration | AgentReviewed | AntSpec(shadow) | RenderedCapture | (new) |
| 14 | Adopting a *new* design-system policy (e.g. `Material`) is sound | HumanOnly | LocalPolicy | TokenDocument | routed governance evidence |

Reading the table top-to-bottom is the whole thesis: rules 1–6 are the "plugin checks it"
band (and several already exist as targets — `DesignTokenDrift`, `ContrastCheck`,
`ControlFidelityCheck`); rules 7–13 are the "order an agent to check it" band; rule 14 is
the human band. The governance layer's job is to *route each rule to its tier with
provenance*, not to pretend everything is lintable.

### A deterministic rule (token drift)

```fsharp
let tokenDriftRule : DesignRule =
    { Id = RuleId.unsafe "design.token-drift"
      Description = "Generated DesignTokens.fs must match the DTCG source."
      Policies = [ Wcag; Ant; Material; Fluent ]
      Tier = Deterministic
      Spec = TokenContract (RepoPath.unsafe "src/Controls/design-tokens.tokens.json")
      Inspects =
        [ TokenDocument (RepoPath.unsafe "src/Controls/design-tokens.tokens.json")
          GeneratedTokenSurface (RepoPath.unsafe "src/Controls/DesignTokens.fs") ]
      Check =
        Some (fun _artifact inputs ->
            if inputs.RegeneratedMatchesCommitted then Pass
            else Fail "DesignTokens.fs differs from regeneration of the DTCG source.")
      Review = None }
```

### An agent-reviewed rule (four values)

```fsharp
let valuesRule : DesignRule =
    { Id = RuleId.unsafe "design.values.natural-certain-meaningful-growing"
      Description = "A generated screen should embody Ant's four design values."
      Policies = [ Ant ]
      Tier = AgentReviewed
      Spec = AntSpec "https://ant.design/docs/spec/values/"
      Inspects = [ ]      // bound at request time to the changed showcase page / app
      Check = None
      Review =
        Some (fun artifact ->
            { Id = RuleId.unsafe "design.values.natural-certain-meaningful-growing"
              Policy = Ant
              Spec = AntSpec "https://ant.design/docs/spec/values/"
              Artifact = artifact
              Question =
                "Assess this captured screen against Ant Design's four values \
                 (Natural, Certain, Meaningful, Growing). Cite the spec. \
                 Return Pass only if all four hold; Fail with the weakest value; \
                 Uncertain if the capture is insufficient to judge."
              SpecHash = ""        // filled by the edge from the fetched/cached spec
              ArtifactHash = "" })  // filled by the edge from the capture
    }
```

The rule carries the *instruction and the citation*, not the agent. That keeps the catalog
pure, reviewable, and diffable.

## The Governance Plugin (Deterministic Tier)

"A special governance plugin that automatically checks" maps to **a rule module plus a
Build target the existing `Route` already knows how to select.**

- **Module:** `DesignRules.fs` in `Governance.Core` emits `DeterministicVerdictFact`s for
  every `Deterministic` rule whose `Inspects` intersect the changed paths. It is an
  ordinary `Rule<DesignSystemFact> list` over `FixedPoint.evaluate`.
- **Target:** a single `DesignSystemCheck` target (or the existing cluster —
  `DesignTokenDrift`, `ContrastCheck`, `ControlFidelityCheck`,
  `ControlsInteractionCheck`) at the Build edge gathers inputs, runs the pure rules, and
  writes a verdict artifact.
- **Routing:** add `RoutingRule`s so changes under `src/Controls/**`,
  `src/Controls/design-tokens.tokens.json`, `src/Color/**`, and
  `docs/testSpecs/Showcase/**` select the design gates. This is the same
  `internalSourceRule`/`internalDocRule` mechanism already in `Routing.fs`.

The plugin produces, for a token change:

```text
SelectedPolicyFact Ant
ChangedDesignPathFact "src/Controls/design-tokens.tokens.json"
DesignRuleFact ("design.token-drift", Deterministic)
DeterministicVerdictFact ("design.token-drift", Pass)
DesignRuleFact ("design.color-policy", Deterministic)
DeterministicVerdictFact ("design.color-policy", Fail "ant.primary fails body/title pairing")
DesignBlockerFact ("design.color-policy", "selected policy 'ant' pairing not satisfied")
NextDesignActionFact ("review src/Controls/design-tokens.tokens.json colorPrimary", "...")
```

No agent is involved for these. This is the cheap, reproducible, every-commit tier.

## The Review Agent (Agent-Reviewed Tier)

This is the "orders a separate agent to check it" half, and the part that needs the most
discipline to stay reproducible.

### Emission (in the kernel, pure)

A rule module `DesignReviewRules.fs` examines, for each `AgentReviewed` rule whose
`Inspects` are touched, whether a **fresh** `RecordedReviewFact` already exists:

```fsharp
// pseudo-logic of the rule
for rule in agentReviewedRules do
    match freshRecordedReview facts rule with
    | Some review when review.Verdict = Pass -> ()                       // satisfied
    | Some review -> emit (DesignBlockerFact(rule.Id, blameFrom review)) // recorded Fail/Uncertain
    | None        -> emit (ReviewRequestFact(buildRequest rule))        // needs a review
```

"Fresh" means a recorded review exists whose `SpecHash` and `ArtifactHash` equal the
current spec and artifact hashes. If either the Ant spec text or the captured artifact
changed since the recorded verdict, the review is **stale** (`StaleReviewFact`) and a new
`ReviewRequest` is emitted. This is what stops a one-time "looks good" from rubber-stamping
later changes — the same role hashing plays in incremental build systems.

### Dispatch (at the Build edge, an effect)

The Build edge interprets `ReviewRequestFact`s exactly like any other effect in the
`BuildEffect` model (the `Engine/` MEL loop). For each request it:

1. Resolves the spec source (fetch+cache the Ant URL, or read the local policy doc) and
   the artifact (read the capture / fsi / token doc); computes `SpecHash`/`ArtifactHash`.
2. Dispatches a **design-reviewer subagent** with the rule's `Question`, the spec text,
   and the artifact (image capture, IR dump, or text), constrained to a structured output.
3. Persists the returned `RecordedReview` as an evidence artifact under
   `readiness/design-review/<ruleId>.json`.

The reviewer's output schema is fixed so the kernel can parse it as a supplied fact:

```json
{
  "request": "design.values.natural-certain-meaningful-growing",
  "verdict": "Fail",
  "reviewer": "claude-opus-4-8",
  "confidence": 0.82,
  "rationale": "Certain holds; Meaningful is weak — three decorative cards carry no task.",
  "citedSpec": "https://ant.design/docs/spec/values/",
  "specHash": "…",
  "artifactHash": "…"
}
```

### Reproducibility and trust

Three mechanisms keep an LLM in the loop without making governance flaky:

1. **Recorded, hashed verdicts.** CI re-evaluates over the recorded artifact and only
   re-dispatches when `SpecHash`/`ArtifactHash` change. A green build does not re-run the
   agent.
2. **Default-deny on uncertainty.** `Uncertain` and missing verdicts block, never pass.
   Low confidence below a policy threshold is treated as `Uncertain`.
3. **Adversarial verification for high-stakes rules.** For rules like #9 (the four values)
   and #10 (page pattern), the edge can dispatch *N* independent reviewers (a `Workflow`
   fan-out) and require a majority, recording each verdict. This is the
   perspective-diverse / adversarial-verify pattern; redundancy is opt-in per rule, not
   default, to bound cost.

The crucial design property: **the kernel reasons over recorded verdicts identically to
how it reasons over `tasks.md` today.** Agent judgment enters the system as evidence, not
as a side effect inside pure logic.

## Agent Authorization Integration

The kernel split doc already defines `AgentAction`/`AgentDecision` for "may the agent do
this?". This domain extends the decision with one case so that **editing design surfaces is
gated on design review**:

```fsharp
// addition to the governance AgentDecision union
type AgentDecision =
    | Allowed of reason: string
    | Denied of reason: string
    | NeedsEvidence of ArtifactId * reason: string
    | NeedsGate of Targets.Target * reason: string
    | NeedsHuman of reason: string
    | NeedsDesignReview of ReviewRequest * reason: string   // new
```

Rules:

- An agent editing `src/Controls/**` or `design-tokens.tokens.json` that touches an
  `AgentReviewed` design rule with no fresh verdict is told `NeedsDesignReview`, naming the
  request and the spec.
- An agent committing a design change with a recorded `Fail`/`Uncertain` is `Denied`,
  citing the rule and rationale.
- Adopting a new `DesignSystemPolicy` (rule #14) is `NeedsHuman`.
- Everything default-denies. Unknown design-affecting actions block.

This is what makes the system *governance* rather than advice: the design rules are not
only checked, they constrain what the build and other agents are allowed to do, with a
typed reason.

## Provenance And Explanation

Every design verdict — deterministic or agent — carries a `ProvenanceStep` tracing it to
the rule, the spec source, the inspected artifact, and the reviewer (for agent verdicts).
A new query view sits beside `ExplainRoute`:

```fsharp
type DesignQuery =
    | ExplainDesign of DesignArtifactRef list
    | ExplainPolicy of DesignSystemPolicy
    | ListPendingReviews
    | AuthorizeDesignEdit of RepoPath

type DesignConclusion =
    | PolicyInEffect of DesignSystemPolicy
    | RulePassed of RuleId * CheckTier
    | RuleFailed of RuleId * reason: string
    | ReviewPending of ReviewRequest
    | ReviewStale of RuleId * reason: string
    | DesignBlocked of RuleId * reason: string
```

Rendered (markdown), an explanation answers "why is this blocked?":

```text
policy=ant
design.token-drift            PASS   (deterministic; src/Controls/DesignTokens.fs)
design.color-policy           FAIL   (deterministic; ant body/title pairing on colorPrimary)
design.values…                REVIEW (agent; spec https://ant.design/docs/spec/values/;
                                       artifact readiness/captures/workbench.png; STALE —
                                       capture changed since last verdict)
blocked: design.color-policy, design.values…
next: fix colorPrimary pairing; re-run design review for workbench capture
```

The JSON surface uses DTO records (kernel doc §"Layer 5") so the external schema is stable.

## Multi-Policy

The `DesignSystemPolicy` selector parameterizes the rule set: each rule declares the
`Policies` it applies under, and the evaluator filters by the `SelectedPolicyFact`. `wcag`
selects the ratio-based deterministic color rule; `ant` selects the Ant pairing rule plus
the value/pattern agent rules; `material`/`fluent` plug in later through the same shape.
This is the governance counterpart of the Ant adoption doc's policy table — the template
parameter (`--design-system wcag|ant`) and the active governance rule set are the same
selector.

## Reuse Of The Inference Substrate

Nothing here needs a new engine. Concretely:

- `DesignSystemFact` is a `'fact` instantiation of `FactSet<'fact>` / `Rule<'fact>` /
  `FixedPoint.evaluate`.
- `RecordedReviewFact` values are `FactAssertion`s supplied by the Build edge — identical
  in kind to the evidence inputs the kernel already consumes.
- Deterministic and agent-emission rules are `Rule<DesignSystemFact>` values in plain
  modules.
- Provenance, diagnostics, and ID types are reused verbatim.

This is the second domain the kernel doc anticipated (§"Standalone Engine Reuse Path"):
proof that the substrate is genuinely reusable, without prematurely extracting a separate
package.

## Integration Points

- **Build/Engine:** add `BuildEffect` cases `RunDesignDeterministicCheck`,
  `DispatchDesignReview of ReviewRequest`, `RecordDesignVerdict`. The MEL interpreter owns
  fetch/dispatch/write; the kernel owns the facts.
- **Route:** add routing rules for `src/Controls/**`, `design-tokens.tokens.json`,
  `src/Color/**`, `docs/testSpecs/Showcase/**`, and generated-app profiles, mapping to the
  design gates. Behavior of existing routes is unchanged.
- **Spec Kit design stage:** the Ant adoption doc proposes a `speckit-design` stage
  (`specify → design → plan → tasks → implement`). Its artifacts
  (`design/token-taxonomy.md`, `design/interaction-states.md`, `design/page-patterns.md`)
  become **supplied `SpecSource`s and `DesignArtifactRef`s** — the design stage is where
  the rules' spec sources are pinned, so governance has something concrete to check
  against.
- **Skill:** `fs-skia-ant-design` (authored in `.agents`, mirrored to `.claude`) is the
  natural home for the review-agent instructions; the rule `Question` strings should be
  consistent with, or generated from, that skill.

## Testing Strategy

Following the kernel doc's split (pure core tests vs. integration tests):

**Core (`Governance.Core.Tests`, pure):**
- A toy non-design fixture proving the substrate hosts this domain (kernel doc pattern).
- Deterministic rule correctness: drift, color-policy pass/fail, spacing scale, intent
  coverage.
- Emission logic: missing verdict → `ReviewRequestFact`; `Fail` recorded →
  `DesignBlockerFact`; spec/artifact hash mismatch → `StaleReviewFact`.
- Default-deny: `Uncertain` and below-threshold confidence block.
- Every verdict and blocker has non-empty provenance citing a rule and a spec source.
- Multi-policy filtering selects the right rule subset per `SelectedPolicyFact`.
- Fixed-point idempotence/convergence with design facts present.

**Integration (`Governance.Tests`, effectful):**
- `DesignSystemCheck` target gathers inputs and writes the verdict artifact.
- Review dispatch with a **mocked reviewer** returning canned `RecordedReview` JSON →
  end-to-end block/pass without a live model (keeps CI deterministic and free).
- Staleness: mutating a capture invalidates a recorded verdict on next run.
- `Route` selects the design gates for design paths.

**Golden parity:** the deterministic design verdicts and the `ExplainDesign` text/JSON get
golden fixtures, compared before/after each phase.

## Implementation Phases

Phased so the cheap, reproducible value lands first and the agent tier is opt-in.

- **Phase 0 — Deterministic-only.** Add `DesignSystemFact`, `DesignRule` (Deterministic
  only), `DesignRules.fs`, and the `DesignSystemCheck` target wrapping the *existing*
  `DesignTokenDrift`/`ContrastCheck`/`ControlFidelityCheck`/`ControlsInteractionCheck` as
  facts. No agents. This alone makes the design system a governed fact domain.
- **Phase 1 — Checkability + emission.** Add `CheckTier`, the agent-reviewed rule catalog,
  `ReviewRequest`, staleness via hashing, and `ReviewRequestFact`/`StaleReviewFact`
  emission. Still no dispatch — the kernel just *says* what needs review.
- **Phase 2 — Agent dispatch.** Add the `BuildEffect` dispatch cases, the design-reviewer
  subagent, the recorded-verdict artifact, and re-evaluation over recorded facts. Mocked
  reviewer in tests.
- **Phase 3 — Authorization + explanation.** Add `NeedsDesignReview`, `ExplainDesign`,
  JSON/markdown renderers, `Route --json` design section.
- **Phase 4 — Multi-policy + adversarial review.** Policy filtering and opt-in N-reviewer
  majority for high-stakes rules (#9, #10).
- **Phase 5 — Spec Kit design stage wiring.** Pin spec sources from `speckit-design`
  artifacts; generated-app page-pattern review.

Each phase keeps existing `Route`/contract/evidence output byte-stable, per the kernel
doc's parity discipline.

## Risks And Open Questions

| Risk | Why it matters | Mitigation |
|------|----------------|------------|
| Agent nondeterminism | A governance gate that flickers is worse than none | Recorded, hashed verdicts; re-dispatch only on input change; default-deny on `Uncertain` |
| Cost of review | Per-commit LLM calls do not scale | Only `AgentReviewed` rules with changed artifacts dispatch; deterministic tier carries the bulk; adversarial N-vote is opt-in per rule |
| Over-escalation | Mislabeling a lintable rule as `AgentReviewed` wastes money and trust | The catalog table is the review surface; prefer Deterministic; require justification to move a rule up a tier |
| Spec drift | Ant docs change; cached spec goes stale | `SpecHash` over the fetched spec invalidates verdicts; spec snapshots stored with the verdict |
| Reviewer disagreement with humans | Agent verdict may be wrong | `NeedsHuman` override path; humans can record an authoritative verdict that supersedes the agent's |
| Capture fidelity | A bad screenshot makes the agent judge the wrong thing | `Uncertain` when the capture is insufficient; tie captures to the existing fidelity/evidence machinery |

Open questions worth deciding before Phase 2:

1. Capture format for `RenderedCapture` — PNG screenshot, control IR dump, or both? (IR is
   more deterministic to hash; image is what matches "does it look right".)
2. Where recorded verdicts live and whether they are committed (reproducible CI) or
   cached out-of-tree (smaller repo).
3. Confidence threshold per rule, and which rules warrant adversarial N-vote.
4. Whether the review-agent instructions are hand-written or generated from the
   `fs-skia-ant-design` skill (single source).

## Acceptance Criteria

The design-system governance domain is complete when:

- `DesignSystemFact` is a closed union over the existing `FixedPoint.evaluate`, with no new
  engine and no LLM call inside `Governance.Core`.
- Every `DesignRule` declares a `CheckTier`; the catalog table is the source of truth for
  which rules are deterministic vs. agent vs. human.
- Deterministic rules run purely and reproduce/extend the existing
  `DesignTokenDrift`/`ContrastCheck`/`ControlFidelityCheck`/`ControlsInteractionCheck`
  verdicts.
- `AgentReviewed` rules emit typed `ReviewRequest`s; the Build edge dispatches the reviewer
  and records hashed verdicts; the kernel reasons over recorded verdicts deterministically.
- Stale verdicts (spec or artifact changed) re-request review; `Uncertain`/missing verdicts
  block.
- `NeedsDesignReview` gates design edits; everything default-denies.
- Every verdict and blocker carries provenance citing a rule and a spec source.
- `ExplainDesign` renders stable text and JSON.
- The policy selector filters the active rule set; `wcag` is the compatibility default.
- CI is reproducible without a live model (mocked reviewer in tests; recorded verdicts in
  runs).

## Final Recommendation

Treat the design system as a **governed rule domain whose distinguishing feature is that
each rule knows its own checkability.** Reuse the inference kernel verbatim; add a
`DesignSystemFact` domain, a `CheckTier` on every rule, and a `ReviewRequest`/recorded-
verdict path that lets the kernel *order* a separate agent to check the judgment rules
while staying pure and reproducible itself.

This directly realizes the framing question: Ant Design *is* describable by rules; a
governance plugin *can* check most of them automatically; and for the rules that need
judgment, the same kernel that authorizes agents can dispatch one to check — recording the
verdict as evidence so the decision is auditable and replayable. The deterministic tier
gives cheap, every-commit enforcement; the agent tier covers the large middle band between
"lintable" and "human-only"; and the human tier is reserved for genuine policy decisions.
The result has the kernel doc's "robust substance" — typed facts, exhaustive rules,
provenance — applied to a design system, without a new language and without making an LLM a
hidden dependency of pure logic.
