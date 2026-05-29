# Tasks: Bomberman Demo Feedback Follow-ups

**Feature branch**: `029-bomberman-demo-feedback`
**Spec**: `specs/029-bomberman-demo-feedback/spec.md`
**Plan**: `specs/029-bomberman-demo-feedback/plan.md`

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

## Vertical-slice rule (US phases)

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised: an FSI
session against the packed library, a smoke run of the generated application,
a manual walk-through with transcript, or a screenshot captured under
`readiness/`. Domain, model, or core-layer changes alone do **not** satisfy
`[X]` for a `[US*]` task.

For stateful or I/O-bearing stories, `[X]` also requires Elmish/MVU evidence:
the public `Model` / `Msg` / `Effect` or `Cmd<Msg>` contract was exercised,
pure `update` transitions were tested, emitted effects were asserted, and the
effect interpreter was run against real dependencies where safe.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change
- **[SEH]** — design-approved synthetic error-handling task paired with `synthetic-error-handling-approved`

Every task has a matching entry in `tasks.deps.yml`. Every task line mirrors
the structured `skillist` value using `[skillist: ...]`; `[skillist: []]`
means no capability skill is required.

## Risk-Level Evidence

- **Small**: docs-only or generated guidance text. Focused validation is the
  targeted governance check that compiles or scans the changed guidance.
- **Medium**: package helper, generated command, or template behavior with
  bounded user-facing impact. Focused validation includes the owning package
  tests plus generated product checks.
- **Broad**: public `.fsi`, generated default launch, screenshot capture, or
  cross-package/build command behavior. Broad validation requires package
  surface review, generated product validation, readiness artifacts, and
  `Verify`. Aggregate results are non-authoritative unless the named target
  itself is the documented authority; record them as summaries that point to
  the authoritative logs.

---

## Phase 1: Setup

- [X] T001 [skillist: []] Confirm branch, feature directory, existing spec/design artifacts, and dirty-worktree scope before edits
- [X] T002 [P] [skillist: []] Create required readiness placeholders for graph-command invocation, verify log cleanliness, screenshot probe, generated app wiring, scene layout authoring, governance risk levels, generated validation authority, capability-loading workflow, runtime limitations, and aggregate hang diagnostics
- [X] T003 [P] [skillist: fs-skia-layout-evidence] Record capability-skill evaluation notes for layout evidence, template updates, package surfaces, generated guidance, and valid empty task skill sets
- [X] T004 [skillist: []] Record Tier 1 scope, affected package/template layers, public API impact, MVU/effect applicability, and non-authoritative aggregate reporting rules in readiness notes
- [X] T005 [P] [skillist: fs-skia-template-update] Inspect generated-template ownership points and decide whether package-only, template-only, or combined template validation is required

---

## Phase 2: Foundation

- [X] T006 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish, fs-skia-scene, fs-skia-layout] Draft candidate `.fsi` surfaces for screenshot evidence, generated host wiring, and scene/layout construction helpers before `.fs` bodies
- [X] T007 [P] [skillist: fs-skia-skiaviewer, fs-skia-elmish, fs-skia-keyboard-input] Define the generated game MVU contract: app-owned `Model`, `Msg`, `Effect`, `init`, pure `update`, `view`, key mapping, tick mapping, and host interpreter boundary
- [X] T008 [P] [skillist: fs-skia-testing, fs-skia-skiaviewer] Define screenshot evidence report validation fields and failure classification vocabulary from the evidence workflow contract
- [X] T009 [P] [skillist: fs-skia-template-update] Define generated command workflow changes for shell-invoked Spec Kit scripts and text-only redirected verification logs
- [X] T010 [P] [skillist: fs-skia-scene, fs-skia-layout, fs-skia-layout-evidence] Define scene/layout authoring examples for coordinates, dimensions, diagnostics, state, and positions without adding host dependencies
- [X] T011 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish, fs-skia-scene, fs-skia-layout] Exercise drafted public surfaces from FSI against the prelude or packed libraries and capture `readiness/fsi-session.txt`
- [X] T012 [P] [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish, fs-skia-scene, fs-skia-layout] Record initial package surface baselines for every touched public module
- [X] T013 [P] [skillist: []] Record unsupported-scope handling for browser/mobile screenshots, release publishing, new Bomberman gameplay, renderer replacement, and platform expansion
- [X] T014 [skillist: []] Complete foundation readiness notes with chosen broad risk level, focused checks, broad checks, and real-evidence obligations for all four stories

**Checkpoint**: Foundation ready — user-story implementation may begin.

---

## Phase 3: User Story 1 (US1) - Reliable Evidence Commands

### Tests First

- [X] T015 [P] [US1] [skillist: fs-skia-template-update] Add generated-product test coverage proving graph/audit scripts are invoked through `bash` and do not require executable file mode
- [X] T016 [P] [US1] [skillist: fs-skia-template-update] Add generated verification-log tests that redirect passing and failing `Verify` output and assert readable text with zero embedded NUL bytes
- [X] T017 [P] [US1] [skillist: fs-skia-testing] Add governance validation for generated command authority, exit-code preservation, command path, output path, and diagnostic fields

### Implementation

- [X] T018 [US1] [skillist: fs-skia-template-update] Update generated graph and audit command wrappers to call the authoritative scripts through `bash`
- [X] T019 [US1] [skillist: fs-skia-template-update] Update generated verification output capture to use text APIs and preserve stdout/stderr diagnostics without binary padding
- [X] T020 [US1] [skillist: fs-skia-template-update] Update template guidance, generated build fragments, and profile documentation for the reliable evidence command workflows
- [X] T021 [US1] [skillist: fs-skia-testing] Add actionable failure diagnostics for missing scripts, failed command launch, nonzero exit codes, and unreadable readiness logs
- [X] T022 [US1] [skillist: fs-skia-template-update] Run generated app validation from a fresh checkout and capture `readiness/evidence-graph-invocation.md`
- [X] T023 [US1] [skillist: fs-skia-template-update] Run at least three redirected `Verify` checks and capture `readiness/verify-log-cleanliness.md` with the NUL-byte scan result

**Checkpoint**: US1 is independently testable from a fresh generated checkout.

---

## Phase 4: User Story 2 (US2) - Truthful Screenshot Evidence

### Tests First

- [S] T024 [P] [US2] [SEH] synthetic-error-handling-approved [skillist: fs-skia-testing, fs-skia-skiaviewer] Add negative report-parser tests rejecting `unsupported` screenshot reports that omit real capture probe detail
- [X] T025 [P] [US2] [skillist: fs-skia-testing, fs-skia-skiaviewer] Add screenshot evidence tests requiring `ok` reports to include a readiness-local, decodable, nonblank image artifact and capture source
- [X] T026 [P] [US2] [skillist: fs-skia-testing, fs-skia-skiaviewer] Add classification tests distinguishing successful capture, unsupported host capability, and app-command implementation errors

### Implementation

- [X] T027 [US2] [skillist: fs-skia-skiaviewer] Implement or refine the real screenshot capture probe path before unsupported fallback classification
- [X] T028 [US2] [skillist: fs-skia-testing] Implement stable `key=value` screenshot report validation for required fields, artifact validation, and pixel-content classification
- [X] T029 [US2] [skillist: fs-skia-template-update] Update generated screenshot evidence commands to record viewer open status, first-frame status, capture availability, capture source, fallback, blocked stage, classification, category, and diagnostics
- [X] T030 [US2] [skillist: fs-skia-testing] Ensure app-command implementation errors are reported as failed outcomes rather than unsupported host capability
- [X] T031 [US2] [skillist: fs-skia-template-update] Run the generated screenshot evidence command on the current host and capture `readiness/screenshot-evidence-probe.md`
- [X] T032 [US2] [skillist: fs-skia-skiaviewer, fs-skia-testing] Record current-host screenshot proof and separately satisfy SC-004 with a known supported-host nonblank artifact, or document the explicitly deferred real-evidence path if no supported host is available

**Checkpoint**: US2 proves real screenshot capability was attempted before any unsupported report.

---

## Phase 5: User Story 3 (US3) - Easier Generated Game Wiring

### Tests First

- [X] T033 [P] [US3] [skillist: fs-skia-elmish, fs-skia-skiaviewer] Add pure transition tests for app-owned `init`, `update`, emitted app effects, key messages, and tick messages
- [X] T034 [P] [US3] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Add host adapter tests asserting viewer/file/native effects are emitted only at the interpreter boundary
- [X] T035 [P] [US3] [skillist: fs-skia-template-update, fs-skia-keyboard-input, fs-skia-layout-evidence] Add generated product tests for persistent launch wiring, key input, tick input, scene rendering, and explicit bounded evidence mode

### Implementation

- [X] T036 [US3] [skillist: fs-skia-skiaviewer, fs-skia-elmish] Implement or refine the standard generated host path using pure app state transitions and separated host-side effect adaptation
- [X] T037 [US3] [skillist: fs-skia-keyboard-input, fs-skia-skiaviewer] Wire viewer key events and elapsed ticks into optional app messages without mutating app state
- [X] T038 [US3] [skillist: fs-skia-scene, fs-skia-skiaviewer] Wire pure model-to-scene rendering through the generated host without moving viewer dependencies into pure scene code
- [X] T039 [US3] [skillist: fs-skia-template-update, fs-skia-layout-evidence] Update generated app source, tests, and guidance so the default executable attempts persistent graphical launch and evidence mode remains explicit
- [X] T040 [US3] [skillist: fs-skia-skiaviewer, fs-skia-template-update] Run persistent launch validation or unsupported-host diagnostics from a generated app and capture `readiness/generated-app-wiring.md`
- [X] T041 [US3] [skillist: fs-skia-elmish, fs-skia-skiaviewer] Capture FSI or smoke evidence that the public host value, pure update, emitted effects, and interpreter path were exercised

**Checkpoint**: US3 launches through the common generated game wiring path with pure app behavior preserved.

---

## Phase 6: User Story 4 (US4) - Clearer Scene and Layout Authoring

### Tests First

- [X] T042 [P] [US4] [skillist: fs-skia-scene, fs-skia-layout] Add compile-time or guidance tests covering ambiguous coordinates, dimensions, diagnostics, state, and positions examples
- [X] T043 [P] [US4] [skillist: fs-skia-layout-evidence] Add generated guidance validation that rejects ambiguous record-heavy examples near overlapping open modules

### Implementation

- [X] T044 [US4] [skillist: fs-skia-scene, fs-skia-layout] Add or refine construction helpers, annotations, or module-qualified examples for coordinate and dimension records
- [X] T045 [US4] [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-layout-evidence] Add diagnostics and state authoring examples that distinguish viewer diagnostics, layout diagnostics, evidence diagnostics, app model state, viewer lifecycle state, and workflow state
- [X] T046 [US4] [skillist: fs-skia-scene, fs-skia-layout] Add position authoring examples for text positions, vertex positions, window positions, and layout positions
- [X] T047 [US4] [skillist: fs-skia-template-update, fs-skia-layout-evidence] Update generated guidance fragments and examples with the accepted disambiguation patterns
- [X] T048 [US4] [skillist: fs-skia-layout-evidence] Run generated guidance validation and capture `readiness/scene-layout-authoring.md`
- [X] T049 [US4] [skillist: fs-skia-scene, fs-skia-layout] Confirm no new viewer, host, keyboard, or controls dependencies were introduced into Scene/Layout packages

**Checkpoint**: US4 guidance covers all record ambiguity categories from the feedback.

---

## Phase 7: Integration & Polish

- [X] T050 [skillist: fs-skia-template-update] Run TemplateCheck, GeneratedProductCheck, and GeneratedGuidanceCheck; record authoritative generated validation paths and any aggregate-only summaries
- [X] T051 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish, fs-skia-scene, fs-skia-layout] Run focused package tests for every touched capability and record package-level evidence
- [X] T052 [skillist: fs-skia-skiaviewer, fs-skia-testing, fs-skia-elmish, fs-skia-scene, fs-skia-layout] Refresh package surface baselines for intentional public additions and run PackageSurfaceCheck
- [X] T053 [skillist: fs-skia-template-update] Run TemplateDrift and document whether generated template changes are applied or intentionally deferred
- [X] T054 [skillist: []] Complete readiness notes for runtime limitations, generated validation authority, audit diagnostics, and aggregate hang diagnostics
- [X] T055 [skillist: speckit-evidence-graph] Run graph-only readiness validation and commit refreshed `readiness/task-graph.md` plus `readiness/task-graph.json`
- [X] T056 [skillist: speckit-evidence-graph, speckit-evidence-audit] Run EvidenceAudit and document PASS or every remaining synthetic/blocking finding
- [X] T057 [skillist: []] Run full `Verify`, confirm required readiness artifacts exist, and summarize reviewer navigation for all Bomberman feedback items

---

## Skill Evaluation Notes

Capability matches were evaluated from `.agents/skills/*/SKILL.md` and
`src/*/skill/SKILL.md`.

- `fs-skia-template-update`: high confidence for generated template files,
  generated command wrappers, profile guidance, template drift, and generated
  product validation.
- `fs-skia-layout-evidence`: high confidence for generated game evidence,
  layout/readability-style guidance, persistent launch evidence, and guidance
  validation.
- `fs-skia-skiaviewer`: high confidence for viewer host contracts, screenshot
  capture, persistent launch, viewer effects, and generated viewer startup.
- `fs-skia-testing`: high confidence for report validators, generated product
  validation helpers, screenshot report classification, and readiness checks.
- `fs-skia-elmish`: high confidence for pure app state transitions, effect
  boundaries, and generated Elmish wiring.
- `fs-skia-scene`: high confidence for pure `SceneNode` construction,
  model-to-scene rendering, and Scene authoring helpers.
- `fs-skia-layout`: high confidence for Yoga-backed layout records and layout
  authoring helpers.
- `fs-skia-keyboard-input`: high confidence only where keyboard event mapping
  or generated keyboard guidance is directly involved.
- `speckit-evidence-graph` and `speckit-evidence-audit`: high confidence only
  for the final graph/audit validation tasks.
- Valid-empty tasks are setup, broad governance notes, unsupported-scope notes,
  readiness note completion, and full `Verify` summary tasks where no local
  capability skill materially helps implementation.

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.
For `[SEH]` rows, include the approval label, design-phase source, synthetic
input class, expected error behavior, and reviewer-visible acceptance status.

| Task | Reason | Real-evidence path | Tracking issue | Label | Design source | Synthetic input class | Expected error behavior | Acceptance status |
|------|--------|--------------------|----------------|-------|---------------|-----------------------|-------------------------|-------------------|
| T024 | Design-approved malformed screenshot report fixtures exercise negative parser/error paths only. | Generated screenshot evidence command plus real screenshot report validation in T031/T032. | none | synthetic-error-handling-approved | `specs/029-bomberman-demo-feedback/contracts/evidence-workflows.md` | malformed or missing `key=value` screenshot report fields | validator rejects the report with diagnostics naming missing real capture probe detail | accepted-seh |

## Approved Synthetic Error-Handling Tasks

T024 is pre-approved for `[SEH]` if completed with synthetic-only malformed
report fixtures. Design source: `specs/029-bomberman-demo-feedback/contracts/evidence-workflows.md`.
Rationale: the task validates rejection of invalid screenshot report content,
not a successful capability claim. Synthetic input class: malformed or missing
`key=value` screenshot report fields. Expected error behavior: validator rejects
the report with diagnostics naming the missing real capture probe detail.
Acceptance status: design-approved, but the Synthetic-Evidence Inventory row is
added only if the task is later marked `[S]`.
