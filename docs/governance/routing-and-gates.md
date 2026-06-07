---
title: Routing and gates
category: Governance
categoryindex: 5
index: 2
description: How the Route selector reads the diff and prints the authoritative tier and minimal gate list — tiers, path-glob rules, default-deny, --enforce, and dogfood.
---

# Routing and gates

`Route` is the compiled selector that answers the only question you need before
validating a change: *given what I edited, which checks must I run?* It reads the
working-tree diff, matches the changed paths against a typed table of routing
rules, picks the highest applicable **tier**, and prints the **minimal gate list**
that satisfies every rule that matched. This page explains how that decision is
made, grounded in the rules in
[`Routing.fs`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/build/Governance/Routing.fs),
and how to respond when the route escalates or `--enforce` reports a failure. For
the surrounding philosophy and the other subsystems, see the
[governance deep-dive index](./index.html).

## What `Route` reads

`Route` does not ask you what you changed; it computes it. The interpreter edge
reads git state — the branch-vs-`main` merge-base diff plus uncommitted and
untracked working-tree paths — and hands the pure selector a `Diff`:

```fsharp
type Diff = { ChangedPaths: string list }
```

Everything downstream is a pure function of that list. The same `Diff` always
produces the same tier and the same gate list, which is what makes the route
deterministic and reproducible.

This whole-worktree view is deliberate. The question `Route` answers is "is this
branch safe to land?", so it considers the union of all dirty paths, not just the
file you most recently touched. The cost is that an unrelated in-progress change in
the same workspace can pull in gates you did not expect; the
[`matched-rules`](#reading-and-responding-to-the-output) line tells you which rule
fired so you can tell whether the escalation is yours.

## Tiers

A tier expresses *how much proof* a change needs. The selector computes the highest
applicable tier across the base tier and every rule that matched, ordered by
`tierRank` in `Routing.fs`:

| Tier | Rank | Meaning |
|---|---|---|
| `inner-loop` | 0 | Routine framework-internal work; the light path (`Dev` only). |
| `focused-authority` | 1 | A focused contract surface changed; run the rule's focused gates. |
| `agent-ready` | 2 | Evidence/governance artifacts changed; run the evidence gates. |
| `maintainer-verify` | 3 | Consumer-contract or build-contract change; the broad path. |
| `automation-final` | 4 | The CI aggregate (`Ci`). |

The two ranks that matter most in daily use are the endpoints. **Inner-loop** is
where a routine `src/**/*.fs` edit lands: no escalation rule matched, the change is
entirely framework source, so the route is just `Dev`. **Maintainer-verify** is the
escalated *consumer-contract / dogfood* path — the broad serialized order
documented in `AGENTS.md` and `CLAUDE.md` — and it is reached only when a rule that
carries that tier matches, when a dogfood feature forces it, or via default-deny
(below). It is no longer the unconditional default.

## How rules map path globs to tiers and gates

Each routing rule is a typed record. Its `Paths` are fnmatch-style globs (`**`
matches across `/`; `*` and `?` match within a single path segment), and the same
list both drives the match predicate and renders into the generated contract, so
the typed predicate and the published `paths:` view cannot drift. A rule carries
the tier it implies, the `Targets.Target` list it requires, and the evidence
artifacts it expects:

```fsharp
type RoutingRule =
    { Id: string
      Paths: string list
      Matches: Diff -> bool
      Tier: Tier
      RequiredGates: Targets.Target list
      ExpectedArtifacts: string list
      TimeoutClass: string
      FailureOwner: string }
```

The current rule set (from `Routing.rules`):

| Rule id | Path globs (abridged) | Tier | Required gates |
|---|---|---|---|
| `controls-public-surface` | `src/Controls/**` | focused-authority | `ControlsCatalogCheck`, `ControlsCatalogGenerationCheck`, `DesignTokenDrift`, `ControlsInteractionCheck`, `ControlsRenderingCheck`, `PackageSurfaceCheck`, `FsiTranscripts`, `GeneratedProductCheck` |
| `generated-template` | `template/base/**`, `template/fragments/**`, `template/**` | focused-authority | `TemplateCheck`, `GeneratedProductCheck`, `SkillContractPathCheck` |
| `evidence-governance` | `specs/**/tasks.md`, `specs/**/tasks.deps.yml`, `specs/**/readiness/**`, `.specify/extensions/evidence/**` | agent-ready | `EvidenceGraph`, `EvidenceAudit` |
| `generated-guidance` | `.specify/templates/**`, `.specify/presets/**`, `template/fragments/**/README.md`, `.../skill/SKILL.md` | focused-authority | `GeneratedGuidanceCheck`, `TemplateDrift` |
| `specify-catchall` | `.specify/**` | focused-authority | `GeneratedGuidanceCheck`, `TemplateDrift` |
| `docs-only` | `docs/**`, `specs/**/contracts/**`, `specs/**/quickstart.md` | focused-authority | `EvidenceGraph` |
| `package-surface` | `src/**/*.fsi`, `readiness/surface-baselines/**` | focused-authority | `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff` |
| `skill-quality` | `.agents/skills/**`, `src/**/skill/SKILL.md`, `template/product-skills/**`, ... | focused-authority | `SkillQualityCheck`, `SkillSyncCheck`, `SkillContractPathCheck`, `TemplateUpdateSkillPackageCheck` |
| `build-target-contract` | `build.fsx`, `scripts/build/**`, `validation.contract.yml` | maintainer-verify | `AgentReady`, `TargetMetadataDrift`, `EvidenceGraph`, `EvidenceAudit`, `Verify`, `Ci` |
| `distribution` | `build/Governance/Publish.fs(i)`, `build/Governance/PrePublish.fs(i)`, `template/base/build.fsx`, `template/base/Directory.Packages.props`, `template/base/docs/UPGRADING.md` | maintainer-verify | `PrePublishCheck`, `TemplateCheck`, `GeneratedProductCheck`, `TargetMetadataDrift`, `EvidenceGraph`, `EvidenceAudit` |

Selection composes these rules rather than choosing one:

1. The base tier comes from the developer class — `inner-loop` for a framework
   author, `focused-authority` as the floor for a consumer agent.
2. Every rule whose globs match a changed path is collected.
3. The result tier is the **maximum** of the base tier and all matched rule tiers
   by `tierRank`.
4. The gate list is `Dev` plus the union of every matched rule's `RequiredGates`,
   then **de-duplicated into `Targets.allTargets` registry order**. Two rules that
   both require `GeneratedProductCheck` produce it once, and the ordering is stable
   regardless of which rules contributed it.
5. The expected artifacts are the distinct union of the matched rules'
   `ExpectedArtifacts`.

So a change that edits both `src/Controls/Foo.fsi` and `template/base/x` matches
both `controls-public-surface`, `package-surface`, and `generated-template`, lands
at `focused-authority`, and gets the merged, de-duplicated gate set of all three.

## Default-deny

An unmatched path is never treated as "nothing to do". The selector only takes the
light inner-loop path when **every** changed path is framework source under
`src/**` (the empty diff trivially qualifies) and no escalation rule matched. Any
other unmatched path — an edit somewhere the rules do not name — falls through to a
default-deny fallback: the tier is raised to at least `maintainer-verify` and the
gate becomes the broad `Verify`. Unrecognised changes fail safe toward more proof,
not less.

```fsharp
// from Routing.select — the default-deny branch
let fallbackTier = [ baseTier; MaintainerVerify ] |> List.maxBy tierRank
{ ... Tier = fallbackTier; Gates = [ Targets.Verify ] ... }
```

This is why edits to governance code under `build/Governance/**` that are not the
specifically-named publish/pre-publish files still validate: they are not in any
rule's globs, so they default-deny to `Verify` rather than slipping through
unchecked.

## Dogfood

Some features intentionally exercise the full pipeline regardless of what they
touch. `Routing.dogfoodFeatureIds` lists those feature ids (currently `"042"`).
When the active feature is a dogfood feature, `selectForFeature` raises the tier to
at least `maintainer-verify`, replaces the gate list with the full pipeline, and
sets `dogfood-forced=true` in the output:

```fsharp
let fullPipelineGates =
    [ Targets.Dev
      Targets.GeneratedGuidanceCheck
      Targets.TemplateCheck
      Targets.GeneratedProductCheck
      Targets.EvidenceGraph
      Targets.EvidenceAudit ]
```

The feature id is matched on either the full directory slug
(`042-foundations-two-tier-process`) or its leading numeric segment (`042`), so the
policy is keyed on the feature number regardless of the directory name.

## `--enforce`

By default `Route` only *reports* the tier and gates. Adding `--enforce` makes it
also fail when a matched rule's `ExpectedArtifacts` are not present on disk:

```bash
./fake.sh build -t Route --enforce
```

The diagnostic names the tier and every missing artifact, for example:

```text
Route --enforce: tier 'focused-authority' requires evidence artifacts that are
not present: readiness/typed-controls-front-door.md, readiness/package-surface-expectations.md
```

The mechanism is straightforward: `unmetArtifacts` filters the selection's
expected artifacts against the set present on disk, and `enforceDiagnostic` renders
the message. Note the honest limitation — `--enforce` checks **presence**, not
freshness or provenance. It confirms the artifact files exist; it does not prove
they were generated for the current diff or commit. Treat it as a low-cost
"did you remember to produce the evidence?" guard, then actually run the gates to
prove the evidence is current.

## Reading and responding to the output

`renderSelection` prints five lines:

```text
developer-class=framework-author
tier=focused-authority
gates=Dev, PackageSurfaceCheck, FsiTranscripts, PerPackageSurfaceDiff
dogfood-forced=false
matched-rules=package-surface
```

How to respond:

- **Run the gates, in the order shown.** They are already de-duplicated and ordered
  in registry order. Because FAKE-backed targets share `.fake` state, run them
  sequentially — never concurrently.
- **A failing gate is a real signal, not a formality.** Each rule carries a
  `FailureOwner` (`product`, `template`, `governance`) indicating where a failure
  most likely originates; use it to triage.
- **If the route escalated unexpectedly**, compare `matched-rules` with what you
  actually changed. In a shared workspace the escalation may come from another
  agent's in-progress edits rather than yours (see
  [what `Route` reads](#what-route-reads)).
- **If `--enforce` lists a missing artifact**, produce that readiness artifact by
  running the gate that generates it, then re-run `Route --enforce` to confirm.
- **If you see `tier=maintainer-verify` with `gates=Verify` and
  `matched-rules=(none)`**, you hit default-deny — you changed a path no rule names.
  Either the change genuinely needs the broad path, or a routing rule should be
  added; the broad `Verify` is the safe interim.

## Why a mistyped gate cannot ship

Because `RequiredGates` is a `Targets.Target list` over a closed union, naming a
gate that does not exist is a compile error in `Routing.fs` — the single source of
truth. The generated `validation.contract.yml` is rendered *from* these rules (and
its `required_gates` are additionally checked against an allowlist,
`AgentValidation.ValidationContract.knownGates`, that rejects an unknown gate with
no success verdict). The selector cannot drift from the rules, and the published
contract cannot drift from the selector. How that generation and currency-checking
works is the subject of [single-source generation](./single-source-generation.html).

For the typed API behind these modules, see the
[API reference](../reference/index.html). For the per-feature evidence the focused
gates validate, see [evidence and audit](./evidence-and-audit.html).
