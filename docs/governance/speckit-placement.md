---
title: Spec Kit placement
category: Governance
categoryindex: 5
index: 5
description: Where each governance touchpoint lands across the Spec Kit phases (specify → clarify → plan → tasks → analyze → implement → merge) — how to run each and how to respond — and the closing strengths/weaknesses analysis of the governance section.
---

# Spec Kit placement

The governance subsystems documented in this section do not run at one moment;
they are spread across the Spec Kit workflow this repository drives every feature
through. This page is the section closer: it walks the Spec Kit phases in order —
**specify → clarify → plan → tasks → analyze → implement → merge** — names which
governance touchpoint applies in each, and says how a practitioner runs it and
responds to it. The phase→touchpoint mapping is made explicit (a table) so that,
for any touchpoint, you can state its phase and your response without guessing. It
then closes the section with an honest, both-sided analysis of the governance
design itself. For the mechanics behind each touchpoint, see
[routing and gates](./routing-and-gates.html),
[evidence and audit](./evidence-and-audit.html), and
[single-source generation](./single-source-generation.html); the overview is the
[governance index](./index.html), and the Spec Kit workflow itself is described in
[the process page](../speckit/process.html).

This repository does not hard-fork Spec Kit. As recorded in
[ADR 0004](../adr/0004-spec-kit-fork-stance.html), vanilla Spec Kit assets are
vendored under `.specify/` and all repository-specific governance behaviour is
layered as **extensions** and **presets** plus a synchronized skill mirror — so
the touchpoints below are overlays on the standard phases, not replacements for
them.

## The phases in order

Each Spec Kit phase is invoked as a skill (the `speckit-*` skills under
`.claude/skills/`, mirrored from the canonical `.agents/skills/`). The seven
phases, in the order a feature moves through them:

1. **specify** — turn a feature description into `spec.md`: requirements, user
   scenarios, measurable success criteria.
2. **clarify** — ask up to five targeted questions and encode the answers back
   into the spec, reducing downstream rework risk before planning.
3. **plan** — produce the design artifacts (`plan.md`, `research.md`,
   `data-model.md`, `contracts/`, `quickstart.md`) and evaluate the Constitution
   Check.
4. **tasks** — break the plan into `tasks.md` (the human checklist) and
   `tasks.deps.yml` (the dependency topology and skill metadata), in lockstep.
5. **analyze** — a strictly read-only cross-artifact consistency pass over
   `spec.md`, `plan.md`, and `tasks.md` before any code is written.
6. **implement** — execute the tasks against the plan, updating task statuses as
   real or synthetic evidence accrues.
7. **merge** — squash-merge the feature branch onto the trunk, then bump and pack
   the packable projects.

A **constitution** phase sits logically before specify: it populates
`.specify/memory/constitution.md` from the preset template. It is not run per
feature — the constitution is established (or amended) once and then enforced in
every later phase — but it is where the principles the other touchpoints depend on
are authored, so it is included in the map below.

## Phase → touchpoint map

This is the load-bearing table. For any governance touchpoint, read across to its
phase and the practitioner response. A touchpoint can apply in more than one phase
(for example `Route` is consulted at plan time to anticipate gates and again at
implement time to select them).

| Governance touchpoint | Spec Kit phase(s) | How to run | How to respond |
|---|---|---|---|
| **Constitution** / Constitution Check | constitution; plan | `/speckit-constitution` to author; the Constitution Check section is filled and re-evaluated during `/speckit-plan` | Fill every required *Repository Governance Decisions* area (N/A-with-rationale counts); a constitution conflict found later is automatically CRITICAL and is fixed in the spec/plan/tasks, never by diluting the principle |
| **`Route` tier/gate selection** (anticipate) | plan | `./fake.sh build -t Route` over the in-progress diff while planning | Read the printed `tier` and `gates` to anticipate the proof the change will need and shape the plan/tasks around it |
| **`Route` tier/gate selection** (validate) | implement | `./fake.sh build -t Route`, then run only the gates it prints, in order, sequentially | Run the gate list; if it escalated unexpectedly, compare `matched-rules` against what you actually changed |
| **Surface baselines / `PackageSurfaceCheck`** | plan; implement | At plan time, decide which `*.fsi` you will touch; at implement time `Route` selects `PackageSurfaceCheck`, `FsiTranscripts`, `PerPackageSurfaceDiff` for any `src/**/*.fsi` change | Regenerate/refresh the affected surface baseline and run the selected surface gates; a surface diff is a deliberate contract change to review, not noise |
| **Evidence model `[S]` / `[S*]`** | tasks; implement | Author `[S]`/`[X]`/`[F]`/`[-]` states in `tasks.md` (never write `[S*]` by hand); during implement, mark each task honestly as evidence accrues | Disclose every `[S]` in the Synthetic-Evidence Inventory (Principle V); fix the root-cause `[S]` to clear computed `[S*]` taint downstream |
| **`EvidenceGraph`** | implement (after every status change); analyze | `./fake.sh build -t EvidenceGraph` (set `SPECKIT_FEATURE_DIR` to your feature) | Run it right after `/speckit-tasks` to confirm the DAG is well-formed, and after each status change to refresh `[S*]` propagation cheaply; fix any structural error before proceeding |
| **`EvidenceAudit`** | merge (gate) | `./fake.sh build -t EvidenceAudit` before landing | Treat NEEDS-EVIDENCE as a hard block: upgrade `[S]` to `[X]` with real evidence, or fix blocking diff-scan hits; `--accept-synthetic` discloses but never clears the block |
| **Single-source regeneration** (`RefreshSurfaceBaselines` + currency gates) | implement (whenever `Routing.fs` or a `.agents`/canonical source changes) | `./fake.sh build -t RefreshSurfaceBaselines`, then commit source + regenerated views together | Edit the canonical source, regenerate, commit both; `TargetMetadataDrift` / `SkillSyncCheck` / `DesignTokenDrift` / `ControlsCatalogGenerationCheck` will block a stale committed view |

## Touchpoint-by-touchpoint

### Constitution and the Constitution Check (constitution phase + plan gate)

The constitution (`.specify/memory/constitution.md`) is authored once via
`/speckit-constitution` from the preset template. Its seven Core Principles and the
governance/workflow sections are marked `<!-- LOCKED -->` — shared doctrine across
every project on the `fsharp-opinionated` preset — so a per-feature edit to them
must be escalated to an upstream preset change rather than made locally.

The Constitution Check then re-enters at **plan** time: `/speckit-plan` fills the
Constitution Check section of `plan.md` and re-evaluates it after design. This is
**machine-enforced** — `GeneratedGuidanceCheck` fails the build if any required
*Repository Governance Decisions* area is empty, still boilerplate, or carries a
`NEEDS CLARIFICATION`/`TODO` placeholder (N/A-with-rationale counts as filled).
Respond by completing every area honestly; if a later phase surfaces a conflict
with a principle, the fix goes into the spec/plan/tasks, never into reinterpreting
the principle.

### `Route` tier/gate selection (plan to anticipate; implement to validate)

`Route` is the compiled selector that maps the working-tree diff to a tier and a
minimal gate list. It is consulted twice in the workflow. At **plan** time, running
`./fake.sh build -t Route` over the in-progress change tells you which contract
surfaces you are about to touch and therefore what proof the feature will need —
useful input to the plan and the task breakdown. At **implement** time it is the
operating rule: run `Route` first, then run only the gates it prints, in the order
shown, sequentially (FAKE-backed targets share `.fake` state and are not
concurrency-safe).

Respond to the output by reading `tier`, `gates`, and `matched-rules`. A routine
`src/**/*.fs` edit routes to `inner-loop` (`Dev` only); a consumer-contract change
escalates. If the route looks heavier than your edit warrants, check
`matched-rules` — `Route` reasons over the whole dirty worktree, so unrelated
in-progress work can pull in gates. See [routing and gates](./routing-and-gates.html)
for the full tier table, default-deny, `--enforce`, and dogfood behaviour.

### Surface baselines and `PackageSurfaceCheck` (plan + implement, for `.fsi` changes)

A public package surface is a consumer contract, so changes to `src/**/*.fsi` carry
their own proof. At **plan** time, decide which `.fsi` surfaces the feature will
change and record that in the plan's contract-impact decisions. At **implement**
time, `Route` selects the `package-surface` rule — `PackageSurfaceCheck`,
`FsiTranscripts`, and the per-package DiffPlex `PerPackageSurfaceDiff` — for any
`.fsi` edit (and the `controls-public-surface` rule adds the same surface checks for
`src/Controls/**`).

Respond by refreshing the affected surface baseline and running the selected gates.
A surface diff is meaningful: it is a deliberate change to what consumers see, to be
reviewed as a contract decision rather than waved through. The stable baselines are
among the artifacts `RefreshSurfaceBaselines` regenerates (see below).

### The evidence model `[S]` / `[S*]` (tasks + implement)

Every task in `tasks.md` carries a status. The five **written** states are `[ ]`
pending, `[X]` done with real evidence, `[S]` done with synthetic evidence only,
`[F]` failed, and `[-]` skipped. You author these during **tasks** and update them
honestly during **implement**. The crucial discipline: never mark `[X]` when any
synthetic condition applies (a mock, a placeholder, a hardcoded literal standing in
for a real source, a test that exercises only synthetic fixtures), and never write
`[S*]` by hand — `[S*]` is **computed** by the evidence gates as propagated taint
when an otherwise-`[X]` task depends on an `[S]`/`[S*]` task.

Respond by disclosing every `[S]` in the Synthetic-Evidence Inventory at the bottom
of `tasks.md`, as Principle V requires. To clear a computed `[S*]`, you do not edit
it — you upgrade its root-cause `[S]` upstream to `[X]` and re-run the gate. The
narrow `[SEH]` exception (design-approved synthetic error-handling) may only be
classified during specify/clarify/plan/tasks, never at implement time. The full
model is in [evidence and audit](./evidence-and-audit.html).

### `EvidenceGraph` (after every status change during implement; and at analyze)

`EvidenceGraph` validates the task DAG and refreshes the computed views
(`readiness/task-graph.json`, `readiness/task-graph.md`). It is validation and
rendering only — it does not by itself block a merge. Run it
`./fake.sh build -t EvidenceGraph` early and often: right after `/speckit-tasks` to
confirm the initial DAG is well-formed, conceptually alongside the **analyze** pass
(which is itself a read-only consistency check over the artifacts), and after each
status change during **implement** to refresh `[S*]` propagation cheaply before the
expensive audit.

Respond to a failure by fixing the named structural problem — a dangling ref, an
orphaned key, a cycle, a duplicate id, a missing/invalid `skillist`, or an
unresolved skill — and not proceeding until the graph is clean. One operational
gotcha worth knowing: the gate resolves the feature from `SPECKIT_FEATURE_DIR` (or
the branch), and a missing setting can silently validate a bundled sample feature
and report a false green — set the feature directory explicitly.

### `EvidenceAudit` (merge gate)

`EvidenceAudit` is the merge-gate verdict. It re-runs the graph compute, counts any
remaining `[S]`/`[S*]` against merge-readiness, and scans the feature diff against
the blocking/advisory pattern library — and it **hard-blocks on either** signal.
Run it `./fake.sh build -t EvidenceAudit` before **merge**; a feature should reach
`/speckit-merge` only after the audit passes (or after an explicitly disclosed
`--accept-synthetic` override).

Respond to a NEEDS-EVIDENCE verdict by walking the report top to bottom: upgrade
declared `[S]` tasks to `[X]` with real evidence where you can, leave auto-`[S*]`
tasks alone (they clear when their root-cause `[S]` clears), and fix blocking
diff-scan hits in the code. `--accept-synthetic` records a written justification but
**does not change the exit code** — it is disclosure, not a bypass.

### Single-source regeneration (`RefreshSurfaceBaselines` + currency gates) — whenever `Routing.fs` or `.agents` change, during implement

Several committed files are generated **views** of a canonical source, not policy:
`validation.contract.yml` is rendered from `Routing.fs`, the `.claude/skills/**`
tree from `.agents/skills/**`, plus design tokens, the controls catalog, generated
docs, and the constitution fragments. Whenever you edit one of those canonical
sources during **implement**, regenerate every view with one command —
`./fake.sh build -t RefreshSurfaceBaselines` — and commit the source together with
the regenerated views.

Respond to the currency gates as a forcing function, not an obstacle: if you skip
regeneration, `TargetMetadataDrift` (for the contract and metadata views),
`SkillSyncCheck` (for the `.claude` mirror), `DesignTokenDrift`, or
`ControlsCatalogGenerationCheck` will fail with a diagnostic naming the stale file
and the exact command to fix it. There is one source to edit and one command to run.
The pattern is detailed in [single-source generation](./single-source-generation.html).

## Analysis

This closes the governance section. The bullets below assess the **governance
design** itself — both what it gets right and where it costs the practitioner —
grounded in the routing rules, the evidence engine, and the two governance analyses
under `docs/reports/`.

### Implementation strengths

- **Gate identity is compile-checked.** `Routing.RoutingRule.RequiredGates` is a
  `Targets.Target list` over a closed union, so a mistyped gate in the single source
  of truth (`Routing.fs`) is a compile error rather than a silent runtime mismatch —
  the published `validation.contract.yml` is *rendered from* those same rules, so the
  contract cannot drift from the selector. The cost is that adding or renaming a gate
  is an F# edit plus a regeneration step, not a one-line YAML change.
- **Compliance is shifted from memory to execution.** The practitioner does not have
  to remember which checks apply; `Route` computes them from the actual diff and
  prints a minimal, de-duplicated, registry-ordered gate list. The flip side, noted
  in the comprehensive analysis, is that `Route` output is plain text only — there is
  no `--json` or rule trace yet, so tools and agents must parse prose.
- **The evidence engine runs in-process in compiled F#**
  (`Evidence.Engine.runGraph` / `runAudit`), making the mechanical, review-blocking
  semantics — cycle detection, `[S*]` propagation, diff scanning — testable and
  deterministic; the `/speckit.tasks` field report confirms the error messages are
  precise and actionable and that propagation behaves as documented. The same report
  also shows the engine's "compulsory skill evaluation" is in practice trust-the-
  author for most tasks (substring-on-title for the rest), so the proof is narrower
  than the surrounding ceremony implies.
- **Synthetic evidence is modeled, not merely discouraged.** `[S]` is disclosed,
  `[S*]` is computed and transitive, `[SEH]` is a structured and still-visible
  exception, and `--accept-synthetic` discloses without ever changing the verdict.
  The weakness is that this only works if authors are honest about direct `[S]`
  declarations — the diff scan catches many but not all evasions.

### Implementation weaknesses

- **`Route --enforce` checks presence, not freshness.** It confirms the expected
  readiness artifact *files exist*; it does not prove they were generated for the
  current diff, commit, or feature state. It is a low-cost "did you remember the
  evidence?" guard, and treating it as proof of currency would be a mistake — the
  gates themselves must still run.
- **Whole-worktree routing is too coarse for authoring.** `Route` deliberately
  reasons over the union of merge-base diff, uncommitted, and untracked paths, which
  is correct for "is this branch safe to land?" but wrong for "what should I run
  after adding one report file?" In a dirty feature workspace a docs-only change can
  inherit unrelated product/surface/template gates and fail on work the author never
  touched. There is no first-class `--paths`/`--intent` authoring scope today.
- **The evidence gate can silently validate the wrong feature.** As reproduced in the
  `/speckit.tasks` analysis, `EvidenceGraph` without `SPECKIT_FEATURE_DIR` could
  default to a bundled sample feature and report `verdict=ok, exit 0` — a confident
  false green against none of the authored tasks. The mitigation is operator
  discipline (set the env var), which is exactly the kind of unwritten knowledge a
  governance system is supposed to eliminate.
- **FAKE-backed gates are not concurrency-safe in a shared worktree.** All
  `./fake.sh` targets share `.fake` state, `bin`/`obj`, and active `feature.json`,
  so a second agent running gates in the same worktree can race the first and produce
  failures misattributed to the current task. There is no advisory lock; the only
  current control is the procedural rule to run them sequentially and not start gates
  while another run is active.

### Design pros

- **The two-tier `Route` model keeps governance usable.** By letting routine internal
  work take the light `inner-loop` path while escalating only consumer-contract
  changes, the design avoids the failure mode where always-verify is so slow that
  practitioners skip validation entirely. The serialized six-target order is reserved
  for the escalated and dogfood paths, not imposed unconditionally.
- **Default-deny fails safe.** An edit to a path no rule names does not slip through —
  it routes to at least `maintainer-verify` with the broad `Verify` gate. This means
  un-routed governance code (most of `build/Governance/**`) is still validated, at
  the price of occasionally over-validating a genuinely trivial unrouted change until
  a rule is added.
- **Single-source generation removes a whole class of drift by construction.** A fact
  that must appear in several places is authored once and rendered into the others,
  with a currency gate that re-renders and compares — so there is no "other copy" to
  forget. The trade-off is indirection: the practitioner must know which files are
  editable sources and which are generated output, and must remember to run
  `RefreshSurfaceBaselines` before committing.
- **The overlay (not fork) stance keeps upstream Spec Kit adoptable.** Per ADR 0004,
  vendoring vanilla assets and layering behaviour as extensions/presets keeps upstream
  improvements mergeable and isolates governance behaviour in versionable, testable
  manifests. The cost is the synchronized skill-mirror invariant (`.agents` →
  `.claude`) that must itself be machine-checked to stay honest.

### Design cons

- **The active guidance corpus is large.** The comprehensive analysis measures roughly
  7,443 tracked lines of active skill and `.specify` Markdown (about 9,222 including
  the generated `.claude` mirror); a corpus that size can hide contradictions, and the
  `/speckit.tasks` report documented concrete cases — a documented validator command
  that does not exist, two skills giving contradictory validator instructions, and
  hint tables pointing at skill ids that do not resolve.
- **Governance couples human-readable prose to substring matchers.** Task-title trigger
  phrases and Markdown heuristic scanners (skill-quality headings, stale-term scans,
  concept anchors) put English and a regex into a brittle contract: a paraphrased title
  can silently flip a task into or out of a blocking group. These are pragmatic where
  narrow and tested, but they are weaker than structured intent and impose real
  cognitive load.
- **There is no first-class separation between authoring, feature, merge, and
  generated-product validation.** Because many gates write to the active feature from
  `.specify/feature.json` and `Route` scopes to the whole worktree, the system cannot
  cheaply answer "validate just the file I touched." The design optimizes for
  merge-readiness, which is the right default for safety but a poor fit for iterative
  authoring.
- **Power is concentrated in one heavy, packable library.** Putting all rules in
  `FS.Skia.UI.Build` gives coherence and compile-time safety, but the same library is
  packed for generated products to reuse its evidence engine, so its `.fsi` surface is
  public-looking and the line between stable generated-product API and
  repository-internal tooling is not sharply drawn. The recommended direction is a
  clearer public/internal split rather than a rewrite — the major migration from
  scripts to compiled F# is already done.

---

See also: [governance index](./index.html) ·
[routing and gates](./routing-and-gates.html) ·
[evidence and audit](./evidence-and-audit.html) ·
[single-source generation](./single-source-generation.html) ·
[Spec Kit process](../speckit/process.html) ·
[API reference](../reference/index.html).
