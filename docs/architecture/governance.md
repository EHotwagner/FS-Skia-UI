---
title: Governance front-end (overview)
category: Architecture
categoryindex: 3
---

# Governance front-end (overview)

FS.Skia.UI's build and governance logic is not a pile of shell and Python scripts —
it is a **compiled F# library**, `FS.Skia.UI.Build`, under
[`build/Governance/**`](https://github.com/EHotwagner/FS-Skia-UI/tree/main/build/Governance),
driven by a thin compiled FAKE front-end (`build/Build.fsproj`) that `fake.sh`
invokes with `dotnet run`. This page is the **overview**: what the front-end is,
the high-level shape of how a change flows from *what you edited* to *which gates
you must run* to *what evidence proves it* to *the merge-gate audit*, and why the
logic was put in compiled F# at all. For the mechanics — the full rule table, the
evidence graph internals, the single-source generation flow, and usage guidance —
see the governance deep dive at
[`../governance/index.html`](../governance/index.html) and, specifically,
[routing and gates](../governance/routing-and-gates.html).

## Why compiled F#

Two design records anchor the whole system. [ADR 0002](../adr/0002-build-front-end-form.html)
decided the front-end is a **dedicated compiled F# executable** that takes a
`PackageReference` on `Fake.Core.Target` (the modular FAKE API, *not* the FCS-pulling
`dotnet fake` script runner), with target bodies delegating to the compiled
governance library — removing the per-invocation FSX compile tax.
[ADR 0001](../adr/0001-governance-library-placement-and-distribution.html) decided
the library lives under `build/` rather than `src/`, deliberately keeping it out of
the eight-package runtime surface contract while co-locating it with the front-end
that drives it; [ADR 0009](../adr/0009-agentvalidation-placement.html) extends the
same principle by relocating `AgentValidation` (a governance contract parser) out of
the runtime monolith and into this library.

The practical payoff is that **policy is typed**. Target identity is a closed F#
union (`Targets.Target`), and routing rules are records (`Routing.RoutingRule`), so a
mistyped gate name in the source of truth is a *compile error* rather than a silent
typo in a YAML file. That single property is what lets the whole front-end claim to
be the authoritative source for "which gates apply".

## The shape: routing → gates → evidence → audit

### 1. Routing — what changed selects the tier and gate list

The operating rule is: run `./fake.sh build -t Route` first, then run **only** the
gates it prints. `Route` reads git state (the branch-vs-`main` merge-base diff unioned
with uncommitted/untracked paths) into a pure `Routing.Diff`, then
`Routing.selectForFeature` matches that diff against the compiled `Routing.rules`.

Each rule (`Routing.internalRule`) pairs a list of path globs with a **tier**, a
list of **required gates** (`Targets.Target` values), and the **expected evidence
artifacts** the tier demands. The crucial anti-drift trick: a rule's `Matches`
predicate is *derived from the same `Paths` list* the rendered contract shows, so the
typed predicate and the published `paths:` view cannot disagree.

Selection picks the **highest applicable tier** across all matched rules (tiers run
`inner-loop` → `focused-authority` → `agent-ready` → `maintainer-verify` →
`automation-final`), unions their required gates, and de-duplicates them in
`Targets.allTargets` registry order so the output is byte-stable. The default is the
light **inner-loop** tier (`Dev` only) for routine framework-internal `src/**`
changes; consumer-contract changes escalate automatically:

| Rule (id) | Roughly matches | Escalates to |
|---|---|---|
| `controls-public-surface` | `src/Controls/**` | focused-authority |
| `package-surface` | public `src/**/*.fsi`, surface baselines | focused-authority |
| `generated-template` | `template/**` | focused-authority |
| `generated-guidance` / `specify-catchall` | `.specify/**` | focused-authority |
| `skill-quality` | skill homes (`.agents/skills/**`, …) | focused-authority |
| `evidence-governance` | `tasks.md`, `tasks.deps.yml`, `readiness/**` | agent-ready |
| `docs-only` | `docs/**`, spec contracts, quickstarts | focused-authority |
| `build-target-contract` | `build.fsx`, `scripts/build/**`, `validation.contract.yml` | maintainer-verify |
| `distribution` | publish/pre-publish sources, template version pins | maintainer-verify |

Two safety behaviours are worth calling out. **Default-deny**: an unmatched path that
is *not* pure `src/**` framework source routes to the broad `Verify` fallback, never
to an empty success. **Dogfood forcing**: a feature in `dogfoodFeatureIds` (currently
`042`) is forced onto the full six-gate `maintainer-verify` pipeline regardless of its
diff. With `--enforce`, `Route` additionally fails when an escalated tier's expected
artifacts are not present, naming the missing artifact.

### 2. Gates — the typed target graph

The gates `Route` prints are `Targets.Target` values dispatched through the FAKE
front-end. They range from the fast `Dev` inner loop (restore/build/test plus skill
sync) through focused proof gates (`PackageSurfaceCheck`, `FsiTranscripts`,
`TemplateCheck`, `GeneratedProductCheck`, the controls catalog/token drift checks) up
to the broad aggregates (`Verify`, `Ci`). The escalated, serialized **maintainer-verify
path** — `Dev` → `GeneratedGuidanceCheck` → `TemplateCheck` → `GeneratedProductCheck`
→ `EvidenceGraph` → `EvidenceAudit` — is reserved for consumer-contract and dogfood
changes, not the unconditional default. (FAKE-backed gates share `.fake` state and are
**not** concurrency-safe; run more than one sequentially.)

### 3. Evidence — durable, reviewable proof

Passing gates emit **readiness artifacts** under the active feature's `readiness/**`
directory (logs, FSI transcripts, package surfaces, template-check output, generated
guidance, task graph, audit output, and so on; the catalogue is
[docs/reports/evidence.md](../reports/evidence.html)). The point is durability: a
feature's proof lives as files a reviewer can read, not as text buried in an agent's
chat history. Helper validators in `FS.Skia.UI.Testing`
([Testing & SkillSupport](./testing-skillsupport.html)) judge much of this evidence —
screenshot proofs, host-warning classification, package drift.

### 4. Audit — the merge gate

`EvidenceGraph` and `EvidenceAudit` are the closing gates. `EvidenceGraph` builds and
renders the task DAG and computes synthetic-evidence propagation (using the same
`SkillSupport.Graph` core described on the support page); `EvidenceAudit` runs the
synthetic-propagation check plus a diff scan and **hard-blocks** on either. This is
where synthetic-evidence disclosure (the `[S]` / `[SEH]` task discipline) is enforced
so that placeholder or canned evidence is visible and never silently treated as real
proof.

## Single source, generated not hand-synced

A defining property of the front-end is that its derived artifacts are **generated
from one canonical source**, with currency enforced by gates, so they cannot drift:

- `validation.contract.yml` is rendered from `Routing.rules` by `ContractView` — it is
  a compatibility *view*, not an independent policy file; `TargetMetadataDrift` checks
  its currency.
- The `.claude/skills/**` tree is generated from the canonical `.agents/skills/**`
  tree; `SkillSyncCheck` enforces the mirror.
- Target metadata, constitution fragments, governed prose blocks, API-surface docs,
  skill references, and the typed-controls catalog are likewise generated or
  currency-checked from canonical sources.

Regenerate the surface baselines with
`./fake.sh build -t RefreshSurfaceBaselines`. The deep dive's
[single-source-generation](../governance/single-source-generation.html) page covers
the generators in detail.

## Where to go next

This page is intentionally an overview. For the complete picture — the full rule
table and tier semantics, the evidence-graph/audit internals, the generation flow,
and how each governance touchpoint maps onto the Spec Kit phases — read the deep dive:

- [Governance deep dive index](../governance/index.html)
- [Routing and gates](../governance/routing-and-gates.html)
- The exhaustive
  [governance system comprehensive analysis](../reports/2026-06-05-2237-governance-system-comprehensive-analysis.html)

## Analysis

### Implementation strengths

- Target identity (`Targets.Target`) and routing rules (`Routing.RoutingRule`) are
  closed F# unions/records, so a mistyped gate in the source of truth is a compile
  error rather than a silently-wrong string in a config file.
- A rule's match predicate is derived from the very `Paths` list its rendered
  contract view shows (`Routing.internalRule` + `ContractView`), and de-duplication
  runs in `Targets.allTargets` registry order, so the typed policy and the published
  `validation.contract.yml` are byte-stable and cannot disagree.
- Selection has real safety behaviours encoded in code: `internalInnerLoopApplicable`
  plus the default-deny fallback route any unmatched non-`src/**` path to broad
  `Verify` (never empty success), and `selectForFeature` force-escalates dogfood
  features onto the full pipeline.
- Derived artifacts are generated and currency-checked from a single canonical source
  (`validation.contract.yml` from `Routing.rules`, `.claude` from `.agents`), so the
  agent-facing views provably track the compiled policy.

### Implementation weaknesses

- `Route` output is text-only — there is no JSON form or explainable per-rule trace —
  so tools and agents must parse human-readable lines and cannot easily ask "why was
  this gate selected?".
- `Route --enforce` checks artifact *presence*, not freshness or provenance, so a
  stale-but-present readiness file can satisfy enforcement without being regenerated.
- Routing covers the old root `build.fsx` / `scripts/build/**` build-target-contract
  path but does not explicitly route the current governance implementation home
  `build/Governance/**` (beyond the publish/pre-publish files), so an internal
  governance edit can route lighter than its blast radius warrants.
- Some checks remain heuristic text scanners rather than structured-contract checks;
  acceptable where narrow and tested, but not equivalent to a typed contract.
- FAKE-backed gates share `.fake` state and are not concurrency-safe, so two agents
  in one worktree can race on `.fake`, build outputs, and readiness artifacts.

### Design pros

- Moving governance into compiled F# with a `dotnet run` front-end removes the
  per-invocation FSX/FCS compile tax (ADR 0002) and gives the policy IDE-grade tooling
  and a typed model.
- Placing the library under `build/` rather than `src/` (ADR 0001) keeps governance
  code out of the runtime package surface contract while co-locating it with the
  front-end that drives it — the AgentValidation relocation (ADR 0009) applies the
  same separation.
- "Run `Route` first, run only what it prints" replaces an unconditional broad
  pipeline with the minimal gate set for *this* change, which is a large day-to-day
  cost saving while still escalating consumer-contract changes automatically.
- Durable per-feature readiness artifacts plus a hard-blocking `EvidenceAudit` make
  completion claims machine-checkable and reviewable rather than asserted in chat.

### Design cons

- The system is heavy and coherent but large: the active guidance corpus plus the
  compiled engine is a substantial surface to learn before a contributor can confidently
  reason about routing and gates.
- A compiled selector that reads the *whole* dirty workspace is correct for
  merge-readiness but too coarse for authoring in a branch where another feature is
  already in progress — the change under review and unrelated in-flight work are routed
  together.
- The packable `FS.Skia.UI.Build` exposes a large public-looking surface, and the line
  between genuine generated-product APIs and repository-internal tooling needs ongoing
  curation to stay meaningful.
- Encoding policy in compiled F# raises the bar to change a rule: adjusting routing or
  adding a gate is a code edit (plus regeneration and currency gates), which is more
  rigorous but less approachable than editing a config file.
