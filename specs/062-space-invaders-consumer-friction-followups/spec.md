# Feature Specification: Space-Invaders Consumer Friction Follow-ups

**Feature Branch**: `062-space-invaders-consumer-friction-followups`
**Created**: 2026-06-04
**Status**: Draft
**Input**: User description: "create specs from the feedback from the feedback hook in the sibling repo spaceinvadersdemo2 in specs/001.../feedback..."

## Context & Triage *(informative)*

A consumer implemented the Space Invaders arcade demo in a generated `FS.Skia.UI`
project (`SpaceInvaders2`) on the artifacts feature **061** merged (`993d27c`;
template `0.1.84`, libs `0.1.65-preview.1`). The per-phase feedback hook captured
**five** records under `SpaceInvaders2/specs/001-space-invaders/feedback/`
(`specify-`, `plan-`, `tasks-`, `analyze-`, `implement-2026-06-04.md`; no
`clarify`). Because the project was generated from the **post-061** packages,
every finding below is against the **current merged state**, not a stale build —
and 061's own deliverables are confirmed present and exercised in the consumer:
multi-file hook discovery is in all five phase skills, the feedback hook's
**fourth** prompt (`## Skill gaps`) fired and produced the skill-gap harvest
below, and `EvidenceGraph` printed its verdict line. These are the *next* layer of
friction.

Each finding is triaged against `060-asteroids-consumer-friction-followups` and
`061-breakout-consumer-friction-followups` (the two most recently merged
features), since several findings are the **residual** of a 060/061 fix rather
than wholly new:

| # | Sev | Finding | Status vs. 060 / 061 / current source |
|---|-----|---------|----------------------------------------|
| SI-1 | minor | Unresolved **precedence** between `settings.auto_execute_hooks: true` in `.specify/extensions.yml` and each phase skill's per-command rule that *optional* (`optional: true`) hooks be **surfaced** ("To execute") rather than auto-run. The two pull opposite ways, so the consumer surfaced the optional `after_specify` hooks and asked the user — a round-trip. No documented precedence rule exists. The feedback capture hook in particular still registers `optional: true` and must be triggered by hand each phase. | **Open & new.** 061 FR-001 fixed multi-file *discovery* and FR-002 added a skip *notice*, but neither resolved execute-vs-surface precedence when `auto_execute_hooks: true`. The natural next layer after 061. **Resolved by clarification (2026-06-04):** the feedback hook is promoted to `optional: false` (mandatory) so it auto-fires every phase; the precedence rule still governs the *remaining* optional hooks (git commit, etc.), which stay surfaced. |
| SI-2 | minor | No single **"durable vs. replaceable when you swap the scaffold model"** map. Planning required reading all six `src/SpaceInvaders2/*.fs`, both test files, `build.fsx`, `docs/product.md`, `docs/effects-boundary.md`, and the `docs/api-surface/*.fsi` before it was safe to decide what a feature may change. The decisive fact — `GovernanceTests.fs` is durable/model-agnostic while `BehaviorTests.fs` is the replaceable scaffold suite — lives only in an in-file Feature-060 comment. | **Open & new.** 060 split `Tests.fs` → `GovernanceTests`/`BehaviorTests`, but shipped no top-level map of which files/tests/source-scan strings must survive a model swap. |
| SI-3 | minor | `./fake.sh build -t Dev` is a green **log-writer that does not compile**; the real compile/test path is `Test`/`Verify` (`dotnet test`). Re-confirmed as a footgun: the consumer had to verify the distinction from `product.md` rather than from the target's own output. | **Partially covered by 061 FR-006 (docs), reproduced.** 061 added the `Dev`-vs-`Test`/`Verify` note to `README`/`product.md`; the residual ask is to surface the caveat **from `Dev`'s own emitted output**, not only the docs. |
| SI-4 | minor | Tasks-phase: (a) `skillist` ids are hard to get right because the **directory name is not guaranteed to equal the skill's `name:`**, forcing a `grep '^name:'` of each `SKILL.md`; the closed `owns:` vocabulary is likewise only in prose. (b) The **auto-injected Phase N+1 → Phase N checkpoint edges are never echoed**, so the *effective* DAG is invisible until a full `EvidenceGraph` run — a write-then-run-and-hope loop. | **Open & new.** 061 FR-007 added an `EvidenceGraph` *verdict line* only; it does not surface the resolved skillist-id set, the `owns:`→implied-skill table, or the computed graph with injected edges. |
| SI-5 | minor | Multi-file hook discovery, while now *correct*, is **tedious and error-prone to perform by hand**: the consumer manually deduped `(extension, command)` across `.specify/extensions.yml` and per-extension files to be sure the `feedback` hook wasn't missed. Wants a consolidated "effective hooks for phase X" view. | **Residual of 061 FR-001.** 061 made discovery multi-file in the *instructions*; the remaining cost is the manual reconciliation itself — a deterministic effective-hooks reconciler/reference. Pairs with SI-1. |
| SI-6 | minor | Analyze-phase: the highest-value findings were **cross-artifact symbol drift** caught only by eyeballing — the `Msg` case `ViewerKeyEventReceived` present in `data-model.md` + `tasks.md` (T004) but missing from `plan.md`, and an `Initial` start-state present in design but absent from spec FR-016. Wants a deterministic union/entity/ID name-set cross-check. | **Open & new.** `speckit-analyze` does prose consistency analysis but no mechanical symbol set-difference across `plan`/`data-model`/`tasks`. |
| SI-7 | major (consumer-rated minor) | The **evidence-engine readiness file formats are still not recoverable in-repo**; the consumer reverse-engineered them with `strings -el FS.Skia.UI.Build.dll`. The surface spans more than the readiness-contract scan: the `skill-loading-evidence.md` table schema (one row per (task,skill); `loaded_at < work_started_at`; resolved path `.agents/skills/<id>/SKILL.md`), the window-visibility `key=value` keys and `diagnostic-class=` value rows, and SEH acceptance tokens (`accepted-seh`, `synthetic-error-handling-approved`, no backticks). | **Partially addressed by 061 FR-004, reproduced on post-061 packages.** 061's T017 made the **readiness-contract scan** print its per-file schema, but the *rest* of the engine's format surface (skill-loading-evidence, window-visibility, SEH) still lives only in the compiled engine. Consumer-rated minor (self-resolved), but highest in-impact (decompiling a DLL is a real barrier). |
| SI-8 | minor | `Result.Ok`/`Result.Error` **shadowing**: `open FS.Skia.UI.SkiaViewer` brings `ViewerDiagnosticLevel.Error` into scope, so bare `Error`/`Ok` bind to the union case instead of `Result`; qualifying as `Result.Error`/`Result.Ok` fixes it. | **Open & new.** Sibling to the existing `Unknown`-collision note in `fs-skia-skiaviewer` and the 060/061 duplicate-DU notes — a one-line "Common pitfalls" addition. |
| SI-9 | minor | F# **record-label resolution cascades**: consumer entity `X`/`Y`/`Width`/`Height` collided with `FS.Skia.UI.Scene.Rect`, and `Score`/`Wave` shared between `Model` and `EvidenceOutcome` made bare record literals resolve to the last-declared type with misleading errors. Remedy: a single `Bounds: Rect` per entity, one `Projectile` type, annotate `model: Model`. | **Already covered by `fs-skia-scene`** (the pitfall is documented). Residual: the warning is only valuable if read **before** designing entity records — discoverability, foldable into the SI-2 map. |
| SI-10 | n/a (generalizable) | Game-agnostic helpers re-implemented **again** (3rd consecutive demo): a **deterministic seeded RNG** (`seedRng`/`nextRng`/`nextBelow`, xorshift64/splitmix64, threaded through pure `update`, no ambient `System.Random` — needed for FR-023-style replay) and **`reserveHudBand`** (reserve a fixed HUD band, clamp gameplay to the remainder, overdraw HUD last). | **Escalates 060 FR-008 / 061 FR-011.** 060 documented the HUD/gameplay *pattern*; 061 triaged four arcade helpers as **documented-not-shipped (D8)**. Re-implementation across Asteroids → Breakout → SpaceInvaders is strong evidence to **ship** the two most-repeated primitives, not document them a third time. |

**Skill-gap harvest (new fourth-prompt output).** The 061 fourth feedback prompt
fired this run and named five candidate skills, mapped here to the findings they
serve:
- *Spec Kit hook execution policy* — a reconciler for `auto_execute_hooks` vs
  optional/mandatory semantics (→ SI-1, SI-5).
- *Generated game simulation core* — a deterministic fixed-timestep loop:
  held-key continuous movement, seeded-RNG threading, documented
  collision-resolution order, bounded headless evidence run (→ SI-10, with the
  SI-2 durable-vs-replaceable map as a companion).
- *Speckit task-graph linter / explainer* — resolve+validate `skillist` ids
  against the live registry, check `owns:` against the closed vocabulary, render
  the effective DAG with injected checkpoint edges before `EvidenceGraph`
  (→ SI-4).
- *Cross-artifact symbol consistency* — symbol name-set diff across
  `plan`/`data-model`/`tasks` (→ SI-6).
- *Speckit evidence-format authoring* — the exact readiness-file contract the
  `FS.Skia.UI.Build` engine parses (→ SI-7).

**Scope note.** Per the house pattern (one consolidated "consumer friction
follow-ups" feature per demo — 060/061/034/022 — and the single-feature rule),
this is **one** feature consolidating all SpaceInvaders2 feedback, not one spec
per friction item. No new USER deliverable was requested this round (unlike 061's
fourth-prompt ask, which already shipped).

**Change classification.** **Tier 2 (internal/content change)** by default —
governance, skill, authoring-template, and self-describing-diagnostics content; no
package identities change. **Exception:** if planning confirms FR-010 (ship the
seeded-RNG and `reserveHudBand` helpers), those helpers add public
`FS.Skia.UI.SkillSupport` `.fsi` surface and **escalate to Tier 1**, pulling in
their per-package surface baseline. The authoritative tier and gate list is
whatever `./fake.sh build -t Route` prints for the actual diff.

## Clarifications

### Session 2026-06-04

- Q: How should the feedback hook be made to auto-fire on phase completion, and
  with what blast radius? → A: Promote the feedback capture hook to mandatory
  (`optional: false`) in its canonical template registration so it always
  auto-runs on every `after_<phase>` completion — scoped to the feedback hook
  only; git-commit and other optional hooks stay surfaced/manual, and FR-001's
  general precedence rule continues to govern that remaining optional set.
- Q: Are the generalizable code (helpers) and skill-gap candidates from the
  feedback in scope? → A: Yes — they remain in scope as FR-010 (helpers) and
  FR-011 (skill candidates), kept as triage-with-lean-to-ship: each MUST be
  dispositioned (ship / document / defer-with-rationale), with FR-010 leaning to
  ship seeded-RNG + `reserveHudBand`; the final per-helper / per-skill call is made
  in `/speckit-plan`. No requirement text changed.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Feedback is captured automatically, with deterministic hook precedence elsewhere (Priority: P1)

A consumer finishing a Spec Kit phase has the **feedback capture hook fire
automatically** — it no longer registers `optional: true` and no longer needs a
manual trigger — so a phase-completion feedback record is written every phase
without a round-trip. The *remaining* optional hooks (git commit, etc.) are still
**resolved deterministically**: a documented precedence rule states whether
`settings.auto_execute_hooks: true` overrides the per-command surface-only default,
and the effective merged hook set for the phase is presented in one consolidated
view rather than hand-reconciled across files.

**Independent test**: In a generated project, complete a phase and confirm the
`after_<phase>` feedback hook auto-runs (a record is written to
`specs/<feature>/feedback/<phase>-<date>.md`) **without** an explicit user nudge,
because its canonical registration is `optional: false`. Separately, with an
optional hook (e.g. git commit) registered, confirm the skill applies the
documented precedence and shows the effective hook set (merged, deduped by
`(extension, command)`) in a single notice.

### User Story 2 - Readiness/evidence formats are fully recoverable without decompiling (Priority: P1)

A consumer hitting any evidence-engine format failure — not only the
readiness-contract scan, but also `skill-loading-evidence.md`, the
window-visibility/`diagnostic-class` files, and SEH acceptance — can learn the
**full required shape** (file name, required tokens/fields, table structure) from
the failing diagnostics and/or a shipped in-repo reference, and never needs to run
`strings -el FS.Skia.UI.Build.dll` or copy a sibling project.

**Independent test**: In a freshly generated project with no passing sibling,
trigger each evidence-format failure class and reach a passing `EvidenceAudit`
using only the audit/graph output and/or shipped reference material — confirmed
with no DLL decompilation and no sibling copy.

### User Story 3 - Accurate build-target and task-graph visibility (Priority: P2)

A consumer understands from `Dev`'s **own output** (not only the docs) that `Dev`
writes logs/markers and does not compile, and that `Test`/`Verify` (`dotnet test`)
is the authoritative compile path; and an author generating tasks can see the
**effective DAG including the auto-injected phase-checkpoint edges** and the
**resolved `skillist`-id / `owns:` reference** before committing to a full
`EvidenceGraph` run.

**Independent test**: Run `./fake.sh build -t Dev` and confirm its output states
it does not compile and points to `Test`/`Verify`; render the task graph and
confirm the injected Phase N+1 → Phase N edges and the resolved skillist-id set
are visible without a separate decompile or `grep` sweep.

### User Story 4 - Mechanical cross-artifact symbol consistency (Priority: P2)

An author running analyze gets a **deterministic symbol set-difference** across
`plan.md`, `data-model.md`, and `tasks.md` — `Msg` cases, union/`Screen` variants,
entity record names, and FR-/SC- IDs — so drift like a `Msg` case present in
design but missing from the plan, or a start-state present in design but absent
from a spec FR, is reported mechanically instead of found by close reading.

**Independent test**: Seed a deliberate symbol drift (a `Msg` case in
`data-model.md`/`tasks.md` but not `plan.md`) and confirm the analyze cross-check
reports the set-difference.

### User Story 5 - Pitfalls cover Result-shadowing and read-before-design record labels (Priority: P3)

A consumer following the capability-skill pitfalls is warned that
`open FS.Skia.UI.SkiaViewer` shadows bare `Ok`/`Error` (qualify as
`Result.Ok`/`Result.Error`), and is pointed — *before* designing entity records —
at the existing `fs-skia-scene` record-label-collision pitfall via the
durable-vs-replaceable source map.

**Independent test**: Read the `fs-skia-skiaviewer` "Common pitfalls" note and
confirm it covers the `Result` shadowing case with the qualified remedy; confirm
the durable-vs-replaceable map references the `fs-skia-scene` record-label pitfall
as a pre-design step.

### User Story 6 - Recurring arcade helpers shipped, not re-documented (Priority: P3)

A consumer building any arcade demo can reach the deterministic seeded RNG and the
HUD-band reservation primitive from `FS.Skia.UI.SkillSupport` (shipped API with a
skill reference) instead of re-implementing them a fourth time; remaining helper
candidates are triaged per-helper with the ship-vs-document decision recorded.

**Independent test**: Confirm `seedRng`/`nextRng`/`nextBelow` and `reserveHudBand`
are shipped in `FS.Skia.UI.SkillSupport` (with surface baselines updated and a
skill reference), or — if planning overrides — that the per-helper decision and
rationale are recorded and the helper is documented as the canonical convention.

### Edge Cases

- `auto_execute_hooks: true` **and** an optional hook whose `condition` is
  non-empty → precedence rule must still leave condition evaluation to the
  executor; the notice reports the resolved decision, not a forced run.
- A future extension ships hooks only in its own
  `.specify/extensions/<ext>/<ext>.yml` → the consolidated effective-hooks view
  must include it, deduped, with no double-run when the same hook also appears
  centrally.
- A readiness/evidence file is *partially* correct (right name, one missing token)
  → the diagnostics/reference must make the missing token recoverable without
  trial-and-error rounds, for every format class (not just the readiness-contract
  scan).
- An analyze symbol cross-check sees a symbol that is *intentionally*
  design-only (not yet a spec FR) → it reports the set-difference for human
  judgment rather than hard-failing.
- A new packable game-helper candidate appears later → SkillSupport triage
  references the skill family/topic so the candidate is findable, not lost.
- The fourth feedback prompt is answered "none" for a phase (e.g. SpaceInvaders2's
  `analyze` "none — analyze is read-only") → the record still writes a well-formed
  `## Skill gaps` section (parity with the "Generalizable code: none" path).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The **feedback capture hook MUST be promoted to `optional: false`
  (mandatory)** in its canonical template registration
  (`template/feedback/extensions/feedback.yml` and every `after_<phase>` entry it
  declares), so it auto-fires on every phase completion and never needs a manual
  trigger — the direct fix for the consumer's "still registers as optional / must
  be triggered by hand" friction. Separately, a **documented precedence rule** MUST
  resolve, for the *remaining* optional hooks, the interaction between
  `settings.auto_execute_hooks: true` (in `.specify/extensions.yml`) and each
  `/speckit-*` phase skill's per-command rule that *optional* hooks be surfaced
  rather than auto-run — stating which wins, so a consumer never needs a clarifying
  round-trip. Both the promotion and the rule MUST be carried in the canonical
  template / phase-skill sources so generated projects inherit them. (Closes SI-1;
  builds on 061 FR-001/002. Resolved per the 2026-06-04 clarification.)
- **FR-002**: The phase skills' hook step MUST present the **effective merged hook
  set** for the phase — collected across `.specify/extensions.yml` and every
  `.specify/extensions/*/*.yml`, deduped by `(extension, command)`, with each
  entry's run/surface/skip decision — as a single consolidated notice, so the
  operator does not hand-reconcile multiple files. The now-mandatory feedback hook
  (FR-001) appears in this notice as auto-run, not as a surfaced optional. (Closes
  SI-5; residual of 061 FR-001.)
- **FR-003**: A **durable-vs-replaceable generated-source map** MUST be shipped as
  a single discoverable artifact (a generated `docs/**` page and/or a skill)
  naming, for a scaffold-model swap: which `src/**/*.fs` files are durable vs.
  replaceable, that `GovernanceTests.fs` is durable/model-agnostic while
  `BehaviorTests.fs` is the replaceable scaffold suite, which source-text scan
  strings must survive, and a pre-design pointer to the `fs-skia-scene`
  record-label-collision pitfall (SI-9). (Closes SI-2; folds SI-9.)
- **FR-004**: `./fake.sh build -t Dev` MUST state **in its own emitted output**
  that it writes logs/markers and does **not** compile, and that the authoritative
  compile/test path is `Test`/`Verify` (`dotnet test`) — extending 061 FR-006's
  docs-only fix so the caveat is visible at the target, not only in
  `README`/`product.md`. (Closes SI-3.)
- **FR-005**: The evidence engine MUST make the **full required shape of every
  readiness/evidence file recoverable without decompiling** — extending 061
  FR-004's readiness-contract schema printing to the remaining format classes the
  consumer had to `strings -el`: the `skill-loading-evidence.md` table schema (one
  row per (task,skill); `loaded_at < work_started_at`; resolved path
  `.agents/skills/<id>/SKILL.md`), the window-visibility `key=value` keys and
  `diagnostic-class=` value rows, and the SEH acceptance tokens (`accepted-seh`,
  `synthetic-error-handling-approved`, no backticks). Satisfied by the
  audit/graph diagnostics printing the complete per-file schema for each failing
  class **and/or** a shipped in-repo evidence-format reference; planning selects
  the mechanism, and the success criterion checks the outcome
  (recoverable-without-decompiling). (Closes SI-7; extends 061 FR-004.)
- **FR-006**: A **repo-local `skillist`/`owns:` quick reference**, generated from
  the live `SKILL.md` registry, MUST list the valid `skillist` ids (resolving the
  directory-name-vs-`name:` distinction) and the closed `owns:`→implied-skill
  table, so authors do not `grep '^name:'` each `SKILL.md`. (Closes SI-4a.)
- **FR-007**: `EvidenceGraph` MUST be able to render the **effective DAG including
  the auto-injected Phase N+1 → Phase N checkpoint edges** (and the resolved
  skillist-id set), so the effective graph is reviewable before/with validation
  rather than trusted from prose — extending 061 FR-007's verdict line. (Closes
  SI-4b.)
- **FR-008**: The analyze phase MUST gain a **mechanical cross-artifact
  symbol-consistency check** that extracts named symbols — `Msg` cases,
  union/`Screen` variants, entity record names, and FR-/SC- IDs — from `plan.md`,
  `data-model.md`, and `tasks.md` and reports set-differences, turning drift like
  the missing `Msg` case and the design-only start-state into a mechanical diff.
  (Closes SI-6.)
- **FR-009**: The `fs-skia-skiaviewer` "Common pitfalls" note MUST gain a one-line
  entry that `open FS.Skia.UI.SkiaViewer` brings `ViewerDiagnosticLevel.Error`
  (and peers) into scope so bare `Ok`/`Error` bind to the union case; the remedy
  is to qualify as `Result.Ok`/`Result.Error` — alongside the existing `Unknown`
  collision note. (Closes SI-8.)
- **FR-010**: The two thrice-re-implemented arcade primitives MUST be **shipped**
  into `FS.Skia.UI.SkillSupport` as real public API with skill references — a
  deterministic seeded RNG (`seedRng`/`nextRng`/`nextBelow`, xorshift64/splitmix64,
  pure and replayable, no ambient `System.Random`) and `reserveHudBand`
  (gameplay region = surface − reserved band; clamp gameplay; overdraw HUD last) —
  **unless** planning records a per-helper decision to keep one documented-only,
  with rationale. The remaining candidates (fixed-step accumulator,
  collision/reflection, paddle rebound) stay triaged per-helper with the
  ship-vs-document decision recorded. (Closes SI-10; escalates 060 FR-008 / 061
  FR-011 D8.)
- **FR-011**: The five fourth-prompt skill-gap candidates (hook execution policy,
  generated game simulation core, speckit task-graph linter/explainer,
  cross-artifact symbol consistency, speckit evidence-format authoring) MUST each
  be dispositioned — created as a skill, folded into an existing skill/FR above,
  or explicitly deferred with rationale — so no candidate is silently dropped and
  each is findable by family/topic.
- **FR-012**: Because the `.agents` skill tree is canonical and `.claude` is
  generated, all skill edits MUST be made in `.agents/skills/**` and regenerated
  (`RefreshSurfaceBaselines`), keeping `SkillSyncCheck` / `TargetMetadataDrift` /
  `SkillQualityCheck` green; and if FR-010 ships helpers, the new
  `FS.Skia.UI.SkillSupport` `.fsi` surface and its per-package surface baseline
  MUST be updated together (Tier-1 escalation for those helpers).

> Interacting / conflicting requirements: FR-001 promotes the **feedback** hook to
> mandatory (it always auto-runs) while its precedence rule still governs the
> *remaining* optional hooks vs FR-002 (surface the merged hook set) — resolution:
> the feedback hook is no longer optional, so it never appears as surfaced; for the
> rest, the precedence rule decides run-vs-surface and the consolidated notice
> reports that decision, never suppressing a mandatory hook nor force-running a
> `condition`-guarded one. FR-005's two satisfiers
> (diagnostics-print-schema vs. shipped reference) are **alternatives**, not
> both-required; planning picks one (or both) and SC-002 checks the *outcome*. FR-010
> leans ship (3rd-demo recurrence) but planning may keep a helper documented-only
> with recorded rationale — SC-006 checks the per-helper decision is recorded and
> the outcome (shipped-with-reference *or* documented-as-convention) holds.

### Framework Governance Prompts *(mandatory)*

- **Package impact**: No package *identities* change. Package *contents* change if
  FR-010 ships the seeded-RNG / `reserveHudBand` helpers into
  `FS.Skia.UI.SkillSupport`, and if generated projects gain new docs (FR-003) or
  evidence-format references (FR-005). The template package version is
  bumped/packed/installed so generated projects pick up the precedence rule,
  effective-hooks view, `Dev`-output caveat, and authoring references
  (FR-001/002/003/004/006). Generated package **consumers** change accordingly.
- **Public contract impact**: No framework `.fsi` *signatures* change **unless**
  FR-010 ships new `SkillSupport` helpers, which add public API (per-package
  surface-baseline update). Consumer-facing skill/template/docs content changes
  (FR-001/003/005/006/009).
- **State workflow impact**: No interpreter/effects/command behavior change. The
  seeded RNG (FR-010), if shipped, is a pure value-type utility for the Elmish
  core, not a host runtime change.
- **Layout/rendering impact**: No rendering-engine change. `reserveHudBand`
  (FR-010), if shipped, is a layout-region helper consistent with 060's documented
  HUD/gameplay pattern; no framework visual output changes.
- **Evidence obligations**: Real evidence under
  `specs/062-space-invaders-consumer-friction-followups/readiness/` — at minimum
  the Route-required escalated-tier artifacts (target-metadata, agent-ready
  verdict, skill-loading-evidence, aggregate-hang-diagnostics), plus, for FR-005,
  proof in a freshly generated project that every evidence-format class is
  recoverable without decompiling (`readiness-recoverability.md`-style log), and,
  if FR-010 ships helpers, their unit tests and surface baselines.
- **Unsupported scope**: No new game/demo is shipped; no new framework runtime
  capability, platform, release, or distribution change. Renaming framework or
  consumer DU cases for SI-8/SI-9 is out of scope — the pitfalls note (FR-009) and
  the source map (FR-003) are the remedies. The effective-hooks view (FR-002),
  symbol cross-check (FR-008), and task-graph reference (FR-006/007) are delivered
  as guidance/diagnostics, not as new hard merge gates, unless planning finds a
  low-cost executable check.
- **Build-target impact**: `Dev` changes to self-describe (FR-004).
  `EvidenceAudit`/`EvidenceGraph` change for the broader evidence-format schema
  printing (FR-005) and the effective-DAG render (FR-007). `TemplateCheck` /
  `GeneratedProductCheck` / `TemplateDrift` likely change for the new docs/
  references and any shipped helpers. `SkillSyncCheck` / `TargetMetadataDrift` /
  `SkillQualityCheck` must stay green after skill edits (FR-012). New checks may
  be needed for the precedence rule (FR-001) and the symbol cross-check (FR-008).
  The authoritative gate list is whatever `./fake.sh build -t Route` prints.

## Success Criteria *(mandatory)*

- **SC-001**: Completing a phase in a generated project auto-writes a feedback
  record **without an explicit user nudge**, because the feedback hook's canonical
  registration is `optional: false` (verified by inspecting the registration and a
  phase-completion run); and, for the remaining optional hooks, the phase resolves
  run-vs-surface by the documented precedence rule **without a clarifying
  round-trip**, with the effective merged hook set shown in one consolidated
  notice. (SI-1, SI-5)
- **SC-002**: In a freshly generated project with no passing sibling, every
  evidence-format class — readiness-contract, `skill-loading-evidence.md`,
  window-visibility/`diagnostic-class`, and SEH acceptance — has its required
  name, tokens, fields, and table shape obtainable from the audit/graph output
  and/or a shipped reference, verified by reaching a passing `EvidenceAudit`
  **without** `strings -el FS.Skia.UI.Build.dll` and without copying another
  project. (SI-7)
- **SC-003**: A single durable-vs-replaceable source map is discoverable (one
  `docs/**` page and/or skill) naming the durable vs. replaceable `src` files,
  the `GovernanceTests`-durable / `BehaviorTests`-replaceable split, the
  must-survive source-scan strings, and the pre-design `fs-skia-scene`
  record-label pointer. (SI-2, SI-9)
- **SC-004**: `./fake.sh build -t Dev`'s own output states it does not compile and
  points to `Test`/`Verify`; rendering the task graph shows the auto-injected
  phase-checkpoint edges and the resolved `skillist`-id set; and a repo-local
  `skillist`/`owns:` reference generated from the live registry exists. (SI-3,
  SI-4)
- **SC-005**: The analyze phase mechanically reports a seeded symbol set-difference
  across `plan.md`/`data-model.md`/`tasks.md` (a `Msg` case present in two but
  missing from the third is flagged), and the `fs-skia-skiaviewer` pitfalls note
  covers the `Result.Ok`/`Result.Error` shadowing case. (SI-6, SI-8)
- **SC-006**: The seeded RNG (`seedRng`/`nextRng`/`nextBelow`) and `reserveHudBand`
  are each either shipped in `FS.Skia.UI.SkillSupport` with a skill reference and
  surface baseline, or documented as the canonical convention with a recorded
  per-helper rationale; and each of the five fourth-prompt skill-gap candidates is
  created, folded, or explicitly deferred. (SI-10, FR-011)
- **SC-007**: All Route-printed gates for this change pass — including
  `SkillSyncCheck` / `TargetMetadataDrift` / `SkillQualityCheck` after `.agents`
  edits are regenerated, and any per-package surface baseline if FR-010 ships
  helpers — and `EvidenceAudit` returns `verdict=PASS` for
  `specs/062-space-invaders-consumer-friction-followups`.

## Assumptions

- SpaceInvaders2 was generated from the **061-merged** packages (template `0.1.84`,
  libs `0.1.65-preview.1`), so all findings are against the current merged state;
  this feature does not re-merge or re-verify 060/061 deliverables (the consumer
  already exercised 061's multi-file hook discovery, fourth feedback prompt, and
  `EvidenceGraph` verdict line).
- "Recoverable without decompiling" (FR-005) is satisfied by the audit/graph
  printing the full per-file schema **or** by a shipped in-repo evidence-format
  reference, or both; planning selects the mechanism and SC-002 checks the outcome.
- FR-010 leans toward **shipping** the seeded-RNG and `reserveHudBand` helpers
  because they are now re-implemented across three consecutive arcade demos
  (Asteroids/Breakout/SpaceInvaders), escalating 061's documented-only D8; planning
  makes the final per-helper Tier-1 call and records it.
- The effective-hooks view (FR-002), symbol cross-check (FR-008), and task-graph
  reference (FR-006/007) are delivered as guidance/diagnostics, not as new hard
  merge gates, unless planning finds a low-cost executable check.
- "One feature, not one-per-item" is the correct reading of "create specs" given
  the consolidated consumer-friction-followups house pattern (060/061/034/022) and
  the one-feature-per-`/speckit-specify` rule.
- The feedback-hook promotion to `optional: false` (FR-001) is made in the
  canonical template source (`template/feedback/extensions/feedback.yml`), and the
  precedence rule + effective-hooks view (FR-001/002) in the canonical
  `/speckit-*` phase-skill sources, so generated projects inherit both; the same
  precedence friction exists in this repo's own phase skills and is fixed there
  too. (Note: the feedback extension is not installed in *this* repo, so its
  promotion is verified in the template / a generated project, not locally.)
- No new USER deliverable was requested this round; the SpaceInvaders2 records were
  produced by the fourth-prompt feedback hook that 061 already shipped.
