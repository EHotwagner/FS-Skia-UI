---
title: The Spec Kit process
category: Spec Kit
categoryindex: 6
index: 1
description: The seven Spec Kit phases (specify → clarify → plan → tasks → analyze → implement → merge), what each produces, and the specific phases where custom FS Skia UI components and design tokens are created and consumed.
---

# The Spec Kit process

FS Skia UI develops features through **Spec Kit** ("speckit") — a phased,
artifact-driven workflow that takes a feature from a natural-language description
to a merged, version-bumped change with provable evidence behind it. The
repository does not hard-fork Spec Kit; it tracks upstream and layers
repo-specific behaviour as **extensions** and a **`fsharp-opinionated` preset**
over the vendored `.specify/` assets (see
[ADR 0004](https://github.com/EHotwagner/FS-Skia-UI/blob/main/docs/adr/0004-spec-kit-fork-stance.md)).
Each phase has a dedicated skill under `.agents/skills/speckit-*` (mirrored to
`.claude/skills/speckit-*`) that defines exactly what it reads, what it writes,
and when it is allowed to run.

This page walks the phases in order and then answers the question a control
author actually has: **at which phase(s) is a custom FS Skia UI component — a
typed control, or a design token — created and consumed?** That mapping is the
focus of the closing section. For how the governance gates attach to these same
phases, see the [governance speckit placement](../governance/speckit-placement.html)
page; for the control machinery itself see the
[typed front door](../controls-design/typed-front-door.html) and
[design tokens / Penpot](../controls-design/design-tokens-penpot.html) deep dives;
for the generated API surface see the [API reference](../reference/index.html).

## The phases

The workflow is a fixed sequence. Each phase consumes the artifacts of the phases
before it and is gated against running out of order — `clarify` expects a
`spec.md`, `analyze` refuses to run until `tasks.md` exists, and so on.

| Phase | Command / skill | Primary output |
|---|---|---|
| Specify | `/speckit.specify` | `spec.md` (+ requirements checklist) |
| Clarify | `/speckit.clarify` | clarification Q&A folded into `spec.md` |
| Plan | `/speckit.plan` | `plan.md`, `research.md`, `data-model.md`, `contracts/`, `quickstart.md` |
| Tasks | `/speckit.tasks` | `tasks.md` + `tasks.deps.yml` |
| Analyze | `/speckit.analyze` | read-only consistency report (no file writes) |
| Implement | `/speckit.implement` | source changes + readiness evidence |
| Merge | `/speckit.merge` | squash-merge to trunk + version bump + pack |

### 1. Specify

`/speckit.specify` turns the feature description into a structured **`spec.md`**
under `specs/<NNN-short-name>/`, derived from `.specify/templates/spec-template.md`.
The spec is deliberately implementation-free: it captures *what* and *why* — user
stories with priorities, testable functional requirements (FR-###), measurable
success criteria (SC-###), edge cases, and the repository's **Framework
Governance Prompts** (package impact, public-contract impact, state-workflow
impact, layout/rendering impact, evidence obligations, unsupported scope, and
build-target impact). The skill makes informed guesses for routine details and
emits at most a few `[NEEDS CLARIFICATION]` markers for the decisions that
genuinely move scope. It also writes a requirements-quality checklist and records
the resolved feature directory to `.specify/feature.json` so every later phase can
find the feature without relying on the git branch name.

### 2. Clarify

`/speckit.clarify` runs **before** planning. It scans the spec against a fixed
ambiguity taxonomy (functional scope, data model, interaction/UX, non-functional
attributes, integration, edge cases, constraints, terminology, completion
signals) and asks **up to five** highly targeted questions — one at a time, each
answerable by a short multiple-choice selection or a ≤5-word phrase. Every
accepted answer is folded straight back into the spec: a bullet under a
`## Clarifications` / `### Session YYYY-MM-DD` heading plus an edit to the section
the answer actually affects (functional requirement, data model, success
criterion, edge case, …). The product is a sharper `spec.md`, not a separate
document. Skipping clarify is allowed for exploratory spikes but raises
downstream rework risk.

### 3. Plan

`/speckit.plan` produces the design artifacts from the clarified spec:

- **`plan.md`** — technical context, the **Constitution Check** (with the
  *Repository Governance Decisions* areas, which are machine-enforced: an empty,
  boilerplate, or `NEEDS CLARIFICATION` area fails `GeneratedGuidanceCheck`), the
  project structure, and the phase outline.
- **`research.md`** — one entry per resolved unknown, each as *Decision /
  Rationale / Alternatives considered*.
- **`data-model.md`** — entities, fields, relationships, validation rules, and
  state transitions.
- **`contracts/`** — the interface contracts the change exposes (public API for a
  library, command schemas for a tool, UI contracts for an application).
- **`quickstart.md`** — the runbook to exercise the result.

Plan is also where the `.fsi`-first discipline lives: because the constitution
requires public visibility to be expressed in signature files, a new public
surface is *sketched as a contract here*, before any implementation. When planning
a generated FS Skia UI product the skill first reads `docs/scaffold-map.md` to
respect which files are framework-owned versus consumer-owned. Planning ends after
this design work; it does not write tasks.

### 4. Tasks

`/speckit.tasks` breaks the plan into an actionable, dependency-ordered checklist.
The preset requires **two files written in lockstep** in the feature directory:

- **`tasks.md`** — the human checklist. Each task carries a five-state status
  (`[ ]` pending, `[X]` done with real evidence, `[S]` synthetic-only, `[F]`
  failed, `[-]` skipped), a phase/user-story grouping, and a visible
  `[skillist: …]` mirror of its capability skills.
- **`tasks.deps.yml`** — the dependency topology and structured task metadata. It
  **must** begin with a `schema_version` scalar and a top-level `tasks:` mapping;
  every `Tnnn` id under it carries `deps`, `skillist`, and an optional `owns:`
  field. (The `tasks:` wrapper is the single structural fact that gates
  validation — bare top-level task keys fail loudly.)

Every task is evaluated against the available capability skills
(`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, template and generated-
product skills) and assigned the **minimal ordered** skill set that materially
helps the work. A task that *owns* gated evidence declares it via `owns:` (e.g.
`graph-validation` implies the `speckit-evidence-graph` skill). After writing both
files the skill validates the DAG with `./fake.sh build -t EvidenceGraph`, which
resolves the active feature from `.specify/feature.json` (override with
`SPECKIT_FEATURE_DIR`); confirm the echoed `feature-directory=` and `tasks=<n>`
match your feature before trusting the verdict.

> A 2026-06 process review
> ([report](https://github.com/EHotwagner/FS-Skia-UI/blob/main/docs/reports/2026-06-03-2128-speckit-tasks-governance-process-analysis.md))
> documented the friction here honestly: authoring the two artifacts is easy;
> validating them is where the sharp edges are (the `tasks:` wrapper requirement,
> resolving skill ids by their `name:` field rather than directory name, and
> confirming the validator targeted *your* feature rather than a sample). Read it
> before authoring tasks by hand.

### 5. Analyze

`/speckit.analyze` is a **strictly read-only** cross-artifact consistency pass
across `spec.md`, `plan.md`, and `tasks.md`. It builds a requirements inventory,
maps tasks to requirements/stories, and reports duplication, ambiguity,
underspecification, constitution conflicts (always CRITICAL), coverage gaps, and
terminology drift — as a severity-ranked report, with no file writes. A mechanical
symbol set-difference (`./fake.sh build -t SymbolCrossCheck`) flags symbols (`Msg`
cases, union variants, entity names, FR-/SC- ids) present in some artifacts but
missing from others. The output is advice plus an optional remediation plan the
user must explicitly approve; analyze never edits anything itself.

### 6. Implement

`/speckit.implement` executes the tasks against the plan, updating `tasks.md`
status as it goes. This phase is governed by hard honesty rules:

- **Skill loading.** Before changing code for a task, every declared skill id is
  resolved to exactly one readable `SKILL.md` and loaded in order; the loaded
  paths are recorded (one row per task/skill pair, `loaded_at` strictly before
  `work_started_at`) in `specs/<feature>/readiness/skill-loading-evidence.md`.
- **Evidence discipline.** A task is only `[X]` when real production paths were
  exercised against real dependencies. Anything resting on a mock, stub,
  placeholder, hardcoded literal, or another `[S]` task is `[S]` with a
  `// SYNTHETIC:` code comment, a `Synthetic`-named test, and a row in the
  Synthetic-Evidence Inventory.
- **Vertical-slice rule.** A user-story (`[US*]`) task is only `[X]` when the
  user-facing surface was driven end-to-end (an FSI transcript through the public
  entry point, a host smoke run, or a packed-library test) — domain code that
  nothing calls is never sufficient.
- **MVU discipline.** Stateful or I/O-bearing work goes through the Elmish/MVU
  boundary (pure `update`, effects executed by an interpreter at the edge), with
  transition/effect assertions.
- **Visibility discipline.** Any public-surface change ships its `.fsi` update in
  the *same* task — never as a follow-up.

After every status change the engine re-runs `speckit.evidence.graph` to refresh
the task DAG and recompute `[S*]` synthetic propagation.

### 7. Merge

`/speckit.merge` consolidates feature branches onto the trunk: it confirms a clean
working tree and a passing evidence audit, **squash-merges** each branch, deletes
it, and pushes. After a successful merge the bump-and-pack step is **mandatory**
whenever the repo has packable projects: every packable `.fsproj` has its patch
version strictly incremented and is packed locally (`./fake.sh build -t
PackLocal`), then the bump is committed and pushed, and the NuGet caches are
cleared so downstream FSI consumers see the fresh package. The merge is not "done"
until everything is bumped, packed, committed, and pushed.

## Where custom FS Skia UI components are created and consumed

A "custom FS Skia UI component" in this codebase is one of two concrete things,
and each has an explicit home in the phase sequence:

1. a **typed control** under `FS.Skia.UI.Controls.Typed` — an immutable `Props`
   record, its `defaults`, and a `view` (and, for stateful controls, `init` /
   `update`) that lowers to the existing legacy builder, proven by a per-control
   parity test (see the [typed front door](../controls-design/typed-front-door.html)
   deep dive); and
2. a **design token** — a theme primitive single-sourced from the DTCG JSON and
   generated into the typed `DesignTokens.Light` / `DesignTokens.Dark` surface
   (see the [design tokens / Penpot](../controls-design/design-tokens-penpot.html)
   deep dive).

These are created and consumed at distinct phases. The table is the short answer;
the notes ground each row.

| Component touchpoint | Phase(s) | What happens |
|---|---|---|
| Control **contract** sketched (`.fsi`-first) | **Plan** | The typed `Props` shape and any public signature is drafted as a contract before code |
| Control **modelled** (entities, `Model`/`Msg`/`Effect`) | **Plan** (data-model) → **Tasks** | Stateful controls reuse an existing MVU model; tasks emit `.fsi`, transition-test, and parity-test items |
| Control **created** (typed `Props`/`defaults`/`view` + parity test) | **Implement** | The typed front door is built and proven structurally equal to the legacy builder |
| Design token **authored / regenerated** | **Implement** | The DTCG `$value` is edited and `DesignTokens.fs` regenerated; the drift gate verifies currency |
| Control / token **consumed by app code** | **Implement** (then every later feature) | A `[US*]` task wires the control through to the user-facing surface; tokens are read via `DesignTokens.Light/Dark` |
| Surface / catalog / token **currency proven** | **Implement → Merge** | Baseline, catalog, and drift gates confirm the additive surface; merge bumps and packs it |

### Creation is a plan-then-implement arc

The contract comes first. Because the constitution puts public visibility in
signature files, a new typed control's **`Props` record and `.fsi` surface are
sketched during plan** (Phase 3) — captured as a contract under `contracts/` and a
data-model entry — and only **built during implement** (Phase 6). The typed-
controls migration is uniform: each control gets an immutable `Props` record
(every catalog `requiredAttribute` becomes a non-optional field, everything else
optional through `defaults`, each event an optional callback), a
`view : Props<'msg> -> Widget<'msg>` that calls the **exact same** legacy
`*.create` / `Attr` builders, and a **mandatory lowering-parity test** asserting
`view props |> Widget.toControl` is structurally equal to the hand-written legacy
output. For a stateful control, `init` / `update` delegate to the existing MVU
model and return its `Model` / `Msg` / `Effect` types — never a fork — with a
delegation test. The typed façade does not invent new catalog controls, new
dependencies, or new MVU models; it re-expresses an existing legacy builder, so
the surface stays additive and visibility lives in the `.fsi`.

Design tokens follow the same plan-then-implement shape but with a single edit
point. Authoring or changing a token is an **implement-phase** edit to the one
DTCG source `src/Controls/design-tokens.tokens.json`; the typed F# module
`src/Controls/DesignTokens.fs` is **generated**, never hand-edited
(`./fake.sh build -t RefreshSurfaceBaselines`). A value-only edit propagates
automatically: `Theme.fs` reads `DesignTokens.Light/Dark.*`, so one DTCG change
updates both the token and the theme. The `DesignTokenDrift` gate fails if the
generated module is not a byte-identical regeneration of the source, which is the
machine-checkable contract that keeps the design source and the typed surface in
agreement.

### Consumption is at implement — and at every later feature

A typed control or token is **consumed** first during the **implement phase** of
the feature that introduces it: a user-story task wires the control through its
public entry point to the user-facing surface (the vertical-slice rule forbids
marking it done otherwise), and app code reads token values through the typed
`DesignTokens.Light` / `DesignTokens.Dark` surface rather than hardcoding theme
literals. After the feature merges and is packed, the same control and tokens are
consumed by **every subsequent feature's** implement phase as ordinary published
API — at which point they are no longer "custom" work but part of the framework's
documented surface in the [API reference](../reference/index.html).

So, to name the phases directly for the practitioner test: a custom component's
**contract is created at plan**, the **component and its parity/drift proof are
built at implement**, **design tokens are authored/regenerated at implement**, and
**consumption is at implement (and at every later feature) through the typed
front door and the generated token surface** — with **merge** the point at which
the additive surface is bumped, packed, and published for downstream reuse.

## Related

- [Governance: where it applies in speckit](../governance/speckit-placement.html)
  — which gates attach to each phase above.
- [Typed front door](../controls-design/typed-front-door.html) — the typed
  `Props`/MVU control surface created at implement.
- [Design tokens / Penpot](../controls-design/design-tokens-penpot.html) — the
  DTCG single source and generated token surface.
- [API reference](../reference/index.html) — the published surface that custom
  components join after merge.
