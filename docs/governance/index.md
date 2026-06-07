---
title: Governance deep dive
category: Governance
categoryindex: 5
index: 1
description: What the FS.Skia.UI governance system is, why it exists, how to run Route, and a map of the deep-dive pages.
---

# Governance deep dive

FS.Skia.UI ships with a governance system whose job is to turn an open-ended change
into a deterministic, provable one: given *what you edited*, it decides *which
checks you must run* and *what evidence proves the change is safe to merge*. The
distinguishing property of this system is that the rules are not prose to be
remembered and not loose YAML to be hand-edited — they are **compiled F#** under
[`build/Governance/**`](https://github.com/EHotwagner/FS-Skia-UI/tree/main/build/Governance),
the `FS.Skia.UI.Build` library. A mistyped gate or tier in the source of truth is a
*compile error*, not a silent runtime mismatch. This page explains what the system
is, why it is built that way, and how to use it day to day; the linked deep-dive
pages cover each subsystem in detail. For the higher-level shape of the build
front-end, see the [governance architecture overview](../architecture/governance.html).

## The philosophy: rules live in compiled F#, enforced by gates

The system rests on one idea: **mechanical policy belongs in code, and compliance
is proven by running gates rather than by trusting a description.** Three design
records anchor this.

- [ADR 0002](../adr/0002-build-front-end-form.html) made the build front-end a
  dedicated compiled F# executable (`build/Build.fsproj`) that references the
  modular `Fake.Core.Target` API and delegates target bodies to the compiled
  governance library — deliberately *not* the `dotnet fake` script runner, which
  pulls the F# compiler service and pays a compile tax on every invocation.
- [ADR 0001](../adr/0001-governance-library-placement-and-distribution.html) placed
  the governance library under `build/` rather than `src/`, keeping it out of the
  runtime package surface while co-locating it with the front-end that drives it.
  (Feature 064 later made `FS.Skia.UI.Build` a published package so generated
  consumer products can reuse its evidence engine.)
- [ADR 0009](../adr/0009-agentvalidation-placement.html) moved the
  `AgentValidation` contract parser out of the runtime monolith and into this same
  governance library, on the principle that governance code belongs with
  governance code.

The practical consequence is that the things that are easy to get wrong by hand —
gate names, tier ordering, the path globs that select rules — are *typed*. The
routing rules in [`Routing.fs`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/build/Governance/Routing.fs)
hold `RequiredGates: Targets.Target list`, where `Targets.Target` is a closed union
([`Targets.fsi`](https://github.com/EHotwagner/FS-Skia-UI/blob/main/build/Governance/Targets.fsi)).
There is no way to require a gate that does not exist.

## Why it exists

A change to this repository can touch very different kinds of contract: framework
internals under `src/**`, a public package surface (`*.fsi`), the generated
template that consumers receive from `dotnet new fs-skia-ui`, Spec Kit evidence
artifacts, or the governance rules themselves. Each kind of change needs a
different amount of proof. Requiring the full validation pipeline on every edit
would make routine work so slow that people would skip validation; requiring
nothing would let contract-breaking changes through. The governance system exists
to make that trade-off *automatically and deterministically*: a routine
framework-internal edit routes to a light inner-loop check, while a
consumer-contract change escalates to the broader proof path — and the decision is
computed from the actual diff, not chosen by judgement.

## Practitioner usage: run `Route` first

Before validating any change, run the route selector. It reads the working-tree
diff and prints the authoritative tier and the minimal list of gates for *this*
change. Run only the gates it prints.

```bash
./fake.sh build -t Route
```

The output is plain text, for example:

```text
developer-class=framework-author
tier=inner-loop
gates=Dev
dogfood-forced=false
matched-rules=(none)
```

Read it as follows:

- **`tier`** — how much proof this change needs. `inner-loop` is the light path;
  `maintainer-verify` is the broad consumer-contract path.
- **`gates`** — the exact targets to run, in order. For the example above you would
  run `./fake.sh build -t Dev` and nothing else.
- **`matched-rules`** — which routing rules fired. `(none)` means no escalation rule
  matched and the change took the base tier (or, for an unmatched non-`src/**`
  path, the default-deny fallback).
- **`dogfood-forced`** — whether a dogfood feature forced the full pipeline (see
  [routing and gates](./routing-and-gates.html)).

To additionally fail when an escalated change is missing a required evidence
artifact, add `--enforce`:

```bash
./fake.sh build -t Route --enforce
```

This names the missing artifact and the tier that requires it, so you know exactly
what evidence still has to be produced before the change is mergeable.

> **One caveat worth knowing.** `Route` reasons over the *whole* working tree — the
> union of the branch-vs-`main` merge-base diff and any uncommitted or untracked
> changes. That is the correct model for "is this branch safe to merge?", but in a
> dirty workspace it can escalate because of unrelated in-progress work, not the
> file you just touched. If a route looks heavier than your edit warrants, check
> `matched-rules` against what you actually changed.

A note on safe concurrency: FAKE-backed commands (`./fake.sh`, `fake.cmd`,
`dotnet fake`) share repository `.fake` state and must not run concurrently. Run
the gates `Route` prints sequentially in the order shown.

## Map of the deep-dive pages

The governance system has four subsystems, each with its own page:

- **[Routing and gates](./routing-and-gates.html)** — how the `Route` selector maps
  the diff to a tier and a minimal gate list: the tiers, the path-glob rules,
  default-deny, `--enforce`, and dogfood. Start here.
- **[Evidence and audit](./evidence-and-audit.html)** — the per-feature evidence
  model: task topology, `[S]`/`[S*]` synthetic-status propagation, readiness
  artifacts, and the `EvidenceGraph` / `EvidenceAudit` merge-gate audit.
- **[Single-source generation](./single-source-generation.html)** — the
  *canonical source → generated view → currency check* pattern that keeps
  `validation.contract.yml` in sync with `Routing.fs` and the `.claude` skill tree
  in sync with `.agents`, so the views can never drift from the source.
- **[Spec Kit placement](./speckit-placement.html)** — where each governance
  touchpoint applies across the Spec Kit phases (specify → clarify → plan → tasks →
  analyze → implement → merge), and the closing strengths/weaknesses analysis of
  the section.

For the typed API the governance modules expose, see the
[API reference](../reference/index.html).
