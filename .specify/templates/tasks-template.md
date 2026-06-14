<!-- Authoritative copy: for a `fsharp-opinionated` project the preset copy at
     `.specify/presets/fsharp-opinionated/templates/tasks-template.md` (with its
     `tasks-deps-template.yml` peer) is authoritative — edit there. This generic
     copy is the non-preset Spec Kit fallback only. -->

# Tasks: [FEATURE_NAME]

**Feature branch**: `[FEATURE_BRANCH]`
**Spec**: `specs/[FEATURE_ID]/spec.md`
**Plan**: `specs/[FEATURE_ID]/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

Approved synthetic error-handling work uses `[SEH]` plus the
`synthetic-error-handling-approved` label. It still remains `[S]` when
completed with synthetic-only malformed-input or explicit error-path evidence.
The classification must be assigned during design, planning, clarification, or
task generation. implementation-time relabeling is forbidden; newly discovered
needs go back to task/design review.

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/synthetic-disclosure -->
**V. Synthetic Evidence Requires Loud, Repeated Disclosure** — Synthetic evidence — mocks, stubs, fakes, hardcoded fixtures, in-memory substitutes, unfinished-code placeholder exceptions, TODO-style failing placeholders, canned responses, or any test that exercises only literal data — MAY be used when real evidence is unavailable or prohibitively expensive, AND a real-evidence path is either planned or explicitly documented as infeasible.
<!-- END GENERATED: constitution/synthetic-disclosure -->

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is
reachable from a user-facing entry point and that path was actually
exercised — an FSI session against the packed library, a smoke run of the
application, a manual walk-through with transcript, or a screenshot
captured under `readiness/`. Domain, model, or core-layer changes alone
do **not** satisfy `[X]` for a `[US*]` task, even if their unit tests
pass green. If the user-reachable surface is missing, stubbed, or not
yet wired, mark `[ ]` (work continues) or `[S]` with a disclosed reason
in the Synthetic-Evidence Inventory — never `[X]`.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and
the effect interpreter was run against real dependencies where safe.

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/mvu-boundary -->
**IV. Elmish/MVU Is the Boundary for Stateful or I/O Workflows** — Any feature with multi-step state, external I/O, retries, user interaction, background work, or operational recovery MUST model its behavior through an Elmish-style Model-View-Update boundary before implementation.
<!-- END GENERATED: constitution/mvu-boundary -->

This rule does not apply to Setup, Foundation, Integration, or Polish
phase tasks; those are evaluated against their own phase verification.

## Success-criterion → assertion mapping

Where a success criterion is mechanically testable (first-frame content, no-overlap,
determinism, a structural invariant), pair it with a concrete enforcing assertion so a
headline SC cannot be silently violated while every gate stays green. Note the mapping on
the task line or in the test name, e.g. `(SC-003)`. The worked example for this feature is
the split generated test suite: SC-003 ("governance scans survive a model swap") is enforced
by `GovernanceTests.fs` being model-agnostic (it reads source text, never the product model
API) compiled before the replaceable `BehaviorTests.fs` — so the durable governance unit
keeps compiling and passing when the model is swapped, and the SC has a real assertion behind
it rather than only prose.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with
  `synthetic-error-handling-approved`

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. Every task line MUST mirror the structured
`skillist` value using `[skillist: ...]`; use `[skillist: []]` when no
capability skill applies. The `speckit.evidence.graph` command refuses to
proceed with dangling references or invalid task skill metadata.

## Canonical Verification Targets

Generated tasks should call repository targets instead of duplicating raw
restore/build/test/package/evidence command order:

- `./fake.sh build -t Dev` for fast local verification.
- `./fake.sh build -t Verify` for the full governed workflow.
- `./fake.sh build -t PackLocal` for local package output.
- `./fake.sh build -t RefreshSurfaceBaselines` for intentional current surface
  baseline refreshes.
- `./fake.sh build -t PackageSurfaceCheck` for package surface review.
- `./fake.sh build -t FsiTranscripts` for public FSI evidence.
- `./fake.sh build -t SampleContractSmoke` for sample smoke evidence.
- `./fake.sh build -t TemplateCheck` for source/package default/minimal
  generated project validation.
- `./fake.sh build -t DependencyReport` for central package governance.
- `./fake.sh build -t GeneratedGuidanceCheck` for generated prompt,
  task-skill, and implementation guidance governance.
- `./fake.sh build -t TemplateDrift` for template-owned drift and deferral
  validation.
- `./fake.sh build -t EvidenceGraph` and `./fake.sh build -t EvidenceAudit`
  for graph and synthetic-evidence gates.

FAKE-backed commands (`./fake.sh`, `fake.cmd`, or `dotnet fake`) share
repository `.fake` state and are not safe to run concurrently. Generated task
lists must serialize multiple FAKE-backed tests or targets in deterministic
order, for example:

1. `./fake.sh build -t Dev`
2. `./fake.sh build -t GeneratedGuidanceCheck`
3. `./fake.sh build -t TemplateCheck`
4. `./fake.sh build -t GeneratedProductCheck`
5. `./fake.sh build -t EvidenceGraph`
6. `./fake.sh build -t EvidenceAudit`

Non-FAKE checks may be marked parallel-safe when they do not invoke FAKE or
depend on `.fake`. Race-like or unknown concurrent FAKE failures require a
sequential rerun order before product-regression claims.

After task generation, evaluate every task against available local capability
skills (`.agents/skills/*/SKILL.md`, `src/*/skill/SKILL.md`, and template
capability skills). Write the minimal ordered skills to structured
`skillist` metadata and mirror it in `tasks.md`. Capability skills are
preferred over generic guidance for matching tasks. Treat skill detection as a
confidence review rather than regex certainty: record confidence, matched signals,
ambiguity, and reviewer disposition for medium, low, indirect, false-positive,
or valid-empty cases.

Keep `tasks.deps.yml`, the `skillist` mirror, and the
`speckit.evidence.graph` status refresh requirements in generated task lists.
Generated implementation batch records must include a red-green evidence log,
graph before/after paths before and after every status change, skill-loading notes,
persistent launch rules for graphical defaults, and non-authoritative aggregate reporting.
Generated tasks should also name the small, medium, and broad
governance risk level, focused validation required for the selected level, when
broad validation is required, and how non-authoritative aggregate results are
recorded.

Task graph validator pitfall guidance must be visible before authors run
`EvidenceGraph`. Task titles are free-form — they are never scanned for
capability phrases. Keep `tasks.deps.yml` in object shape with exactly
one key per task id under the top-level `tasks:` wrapper: `T001: { deps: [], skillist: [] }`
is invalid YAML style for this repo, and bare `Tnnn:` keys with no `tasks:`
wrapper are rejected with one directive error. Use indented object fields
instead. Every task id in `tasks.md` must appear once in `tasks.deps.yml`,
dependency lists must use exact `Tnnn` ids, and the visible `[skillist: ...]`
mirror in `tasks.md` must match the structured `skillist` list exactly and in
order.

Evidence ownership is structured, not inferred from titles. A task declares the
gated evidence it owns through an optional `owns:` field in `tasks.deps.yml`,
drawn from the closed vocabulary `graph-validation` (implies
`speckit-evidence-graph`), `evidence-audit` (implies `speckit-evidence-audit`),
`task-generation` (implies `speckit-tasks`), `implementation-loading` (implies
`speckit-implement`), and `constitution` (implies `speckit-constitution`). Each
declared `owns:` value requires its implied skill in that task's `skillist`;
an unknown value is a directive error. Most tasks own nothing — omit `owns:` or
use `[]`. Earlier task files that relied on the removed title-trigger matcher
should re-express ownership via `owns:` and drop any title rewording (or the
`Complete readiness notes` prefix) that only existed to satisfy or dodge it.

The authoritative skill registry for `skillist` ids is built from readable
`SKILL.md` files under `.agents/skills/*/SKILL.md`,
`src/*/skill/SKILL.md`, and `template/fragments/*/skill/SKILL.md`. Declare the
`name:` value from the skill file, not the directory name, when they differ. In a
**generated product** the controls/widgets skill is exactly such a case: its directory
is `fs-skia-ui-widgets` but its declared `name:` is the project-prefixed form (e.g.
`<project>-widgets`), so a `skillist` id of `fs-skia-ui-widgets` dangles — read the
`name:` from `fs-skia-ui-widgets/SKILL.md` and declare that resolved value.

Advisory FS.Skia.UI capability hints are non-blocking: rendering or scene tasks
usually need `fs-skia-scene`; viewer or window-host tasks usually need
`fs-skia-skiaviewer`; Elmish workflow tasks usually need `fs-skia-elmish`;
keyboard or input tasks usually need `fs-skia-keyboard-input`; layout
readability tasks usually need `fs-skia-layout-readability`; controls, forms,
charts, graphs, or DataGrid tasks usually need `fs-skia-ui-widgets`; generated
game HUD readability and public-scene host update tasks usually need
`fs-skia-layout-readability`; deterministic evidence mode and host-warning
classification tasks usually need `fs-skia-evidence-mode`. These hints do not
create hard validation failures; every id resolves to a consumer-registerable
skill.

Generated task lists must make audit-enforced readiness files discoverable
before implementation starts. Setup or foundation work should create placeholders
or explicit tasks for the active feature's required readiness files, including
`readiness/governance-risk-levels.md`, `readiness/aggregate-hang-diagnostics.md`,
`readiness/runtime-limitations.md`, `readiness/generated-validation-authority.md`,
`readiness/skill-loading-evidence-workflow.md`, `readiness/audit-diagnostics.md`,
`readiness/readiness-contract-discovery.md`, `readiness/framework-guidance.md`,
`readiness/evidence-vocabulary.md`, `readiness/evidence-graph.md`, and
`readiness/evidence-audit.md` when those contracts apply. Do not leave
readiness discovery to the final audit.

For malformed-input and explicit error-path tasks, generated task lists MAY use
`[SEH]` only when the task itself validates an error behavior and real input is
infeasible, unsafe, impossible, or not representative of the error path.
Eligible examples include malformed parser input, corrupt file content, invalid
command arguments, protocol violations, missing required data, hostile payloads,
and forced error-result fixtures. Non-eligible examples include convenience mocks,
incomplete integrations, unavailable product capability, missing host support,
placeholder outputs, speed-only fixtures, and ordinary in-memory substitutes.
If a task is split, renamed, or rescoped, preserve `[SEH]` only
when the same approved rationale still applies; otherwise invalidate it.

For graphical viewer features, generated tasks MUST include a distinct
persistent graphical launch task that is reachable from the default executable
path. Bounded smoke, first-frame, frame-count, scene metadata, or
unsupported-host diagnostics may be generated as explicit helper evidence, but
they MUST NOT be described as completing interactive graphical readiness.
Task generation MUST reject viewer-backed default executable paths that only
print metadata, count controls, run bounded smoke, emit scene evidence, or
exit without a persistent launch attempt.

Visual demo task guidance must assign implementation skills before work starts:
scene rendering -> fs-skia-scene; screenshot capture -> fs-skia-skiaviewer;
layout readability -> fs-skia-layout-readability; persistent viewer launch ->
fs-skia-skiaviewer; deterministic evidence mode -> fs-skia-evidence-mode;
generated-package validation -> fs-skia-template-update; graph validation ->
speckit-evidence-graph; audit validation -> speckit-evidence-audit.
Multi-skill ordering must preserve implementation-before-evidence,
graph-before-audit, and debug-before-broad-rerun. Visible mirrors such as
`[skillist: speckit-tasks, fs-skia-layout-readability]` must match structured
metadata exactly.

Visual demo readiness scaffolds must be named before final audit discovery:
`readiness/visual-evidence-honesty.md`, `readiness/window-visibility.md`,
`readiness/governance-risk-levels.md`,
`readiness/aggregate-hang-diagnostics.md`, `readiness/runtime-limitations.md`,
`readiness/generated-guidance-validation.md`, and
`readiness/real-image-evidence.md`. Each scaffold records the authoritative
command, artifact path, failure class, and next action.

Template source: `.specify/presets/fsharp-opinionated/templates/tasks-template.md`.

---

## Phase 1: Setup

- [ ] T001 [skillist: []] Scaffold the feature directory and link spec + plan
- [ ] T002 [P] [skillist: []] Add baseline install or adoption documentation for the selected profile
- [ ] T003 [P] [skillist: []] Add readiness artifact scaffolding (`specs/[FEATURE_ID]/readiness/`) with audit-enforced placeholder files discoverable before implementation
- [ ] T004 [skillist: []] Record feature Tier, affected layer, public-API impact, Elmish/MVU applicability, and required evidence obligations

---

## Phase 2: Foundation

- [ ] T005 [skillist: []] Draft the public surface as `.fsi` signature(s), including `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, and interpreter boundary for stateful or I/O-bearing features
- [ ] T006 [P] [skillist: []] Add or update constitutional guidance that this feature touches
- [ ] T007 [P] [skillist: []] Define or update operational workflows, commands, reports, or scripts
- [ ] T008 [skillist: []] Exercise the draft `.fsi` from FSI (`scripts/prelude.fsx` or ad-hoc), including representative `init` / `update` paths when MVU applies, and capture the session transcript to `readiness/fsi-session.txt`
- [ ] T009 [skillist: []] Record surface-area baselines for the new / changed public modules
- [ ] T010 [skillist: []] Record unsupported-scope handling and failure diagnostics

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1)

### Tests First (Principle I, Principle VI)

<!-- Generated from .specify/memory/constitution.md by `./fake.sh build -t RefreshSurfaceBaselines`; do not hand-edit between the markers. -->
<!-- BEGIN GENERATED: constitution/tests-first -->
**VI. Test Evidence Is Mandatory** — Behavior-changing code MUST include automated tests that fail before the change and pass after.
<!-- END GENERATED: constitution/tests-first -->

- [ ] T011 [P] [US1] [skillist: []] Add semantic tests that load the packed library (or prelude), exercise the US1 surface, and assert MVU state transitions plus emitted effects when applicable
- [ ] T012 [P] [US1] [skillist: []] Add verification for the US1 outcome against the readiness artifact, including real interpreter evidence for effects where safe

### Implementation

- [ ] T013 [P] [US1] [skillist: []] Add story-specific contracts, docs, or fixtures
- [ ] T014 [P] [US1] [skillist: []] Add any required sample or schema artifacts
- [ ] T015 [US1] [skillist: []] Implement the primary user-facing behavior for the story, keeping MVU `update` pure when applicable
- [ ] T016 [US1] [skillist: []] Connect the story's effect interpreter to canonical readiness artifacts or workflows
- [ ] T017 [US1] [skillist: []] Add validation and actionable failure diagnostics
- [ ] T018 [US1] [skillist: []] Document the story's independent validation path

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2)

### Tests First

- [ ] T019 [P] [US2] [skillist: []] Add semantic tests exercising the US2 surface through FSI, including MVU transitions and effects when applicable
- [ ] T020 [P] [US2] [skillist: []] Add validation for the US2 readiness outcome, including real interpreter evidence where safe

### Implementation

- [ ] T021 [P] [US2] [skillist: []] Add story-specific contracts, docs, or fixtures
- [ ] T022 [US2] [skillist: []] Implement the primary user-facing behavior for the story

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: Integration & Polish

- [ ] T023 [skillist: []] Surface-area baseline refresh (Tier 1 only)
- [ ] T024 [skillist: []] Run the packed library through the numbered example scripts and confirm none are broken
- [ ] T025 [skillist: speckit-evidence-graph] Run `speckit.evidence.graph` — confirm no cycles, no dangling refs, no `[S*]` surprises
- [ ] T026 [skillist: speckit-evidence-audit] Run `speckit.evidence.audit` — confirm verdict PASS or document every `--accept-synthetic` override

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| _(none yet)_ | | | | | | | | |
