# Tasks: Keyboard State Display Element

**Feature branch**: `004-keyboard-state-display`
**Spec**: `specs/004-keyboard-state-display/spec.md`
**Plan**: `specs/004-keyboard-state-display/plan.md`

## Status Legend

- `[ ]` — pending
- `[X]` — done with real evidence
- `[S]` — done with synthetic evidence only (must be disclosed per Principle V)
- `[F]` — failed
- `[-]` — skipped (with written rationale)

The `[S*]` marker is computed, not written: any task whose dependency is
`[S]` or `[S*]` and which otherwise would be `[X]` is promoted to `[S*]` by
the evidence audit. See `readiness/task-graph.md` for the propagated view.

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

This rule does not apply to Setup, Foundation, Integration, or Polish
phase tasks; those are evaluated against their own phase verification.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, … — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The `speckit.evidence.graph` command refuses to
proceed with dangling references.

---

## Phase 1: Setup

- [X] T001 Confirm feature artifact set is present (`spec.md`, `plan.md`, `research.md`, `data-model.md`, `quickstart.md`, `contracts/public-api.md`)
- [X] T002 [P] Create readiness directories for FSI transcripts, sample smoke output, and surface baselines under `specs/004-keyboard-state-display/readiness/`
- [X] T003 [P] Identify existing keyboard input implementation, test, sample, prelude, smoke, and package baseline files affected by the feature
- [X] T004 Record Tier 1 classification, affected public module, no-new-dependency constraint, and evidence obligations for pure display model plus Skia scene renderer

---

## Phase 2: Foundation

- [X] T005 Draft `KeyboardInput.fsi` public contracts for display visibility, density, options, model records, omissions, and render/model functions
- [X] T006 [P] Record Elmish/MVU applicability: no new library `Model`/`Msg`/`Effect` contract is introduced, but tests must exercise existing `InputRuntime`, `InputMsg`, and `InputEffect` transitions through the public boundary
- [X] T007 [P] Add or update FSI/prelude coverage in `scripts/input-prelude.fsx` for compact, expanded, and hidden display construction through the public surface
- [X] T008 Add semantic test scaffolding in `tests/Lib.Tests/KeyboardInputTests.fs` for reusable display fixtures, recent-effect capture, diagnostics, invalid layouts, and scene descriptions
- [X] T009 Add or reserve surface-area baseline artifact path `specs/004-keyboard-state-display/readiness/surface-baselines/FS.Skia.UI.txt`
- [X] T010 Define failure-diagnostic expectations for missing/invalid layout and non-actionable diagnostic filtering
- [X] T011 Exercise the drafted `.fsi` with FSI and capture the transcript to `specs/004-keyboard-state-display/readiness/fsi/keyboard-state-display-prelude.txt`

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - See Current Keyboard Context

### Tests First (Principle I, Principle VI)

- [X] T012 [P] [US1] Add public-surface tests for default, compact, expanded, and hidden display options
- [X] T013 [P] [US1] Add semantic tests that assert active layout id/display name, active top context, active state, and hidden-mode empty model/scene
- [X] T014 [P] [US1] Add tests that drive `KeyboardInput.update` layout/state changes and assert state display updates from emitted runtime/effects
- [X] T015 [P] [US1] Add `Scene.describe` tests for `renderKeyboardStateDisplay` and `renderKeyboardStateDisplayAt` returning stable text/shape primitives without custom app drawing

### Implementation

- [X] T016 [US1] Implement display option defaults and the pure `keyboardStateDisplay` projection for visible/hidden orientation fields in `src/Lib/KeyboardInput.fs`
- [X] T017 [US1] Implement standard scene rendering for compact/expanded orientation and hidden empty scene behavior using existing `Scene` primitives
- [X] T018 [US1] Preserve compatibility for `layoutState`, `renderLayoutState`, and `renderLayoutStateAt`, delegating only where behavior remains compatible
- [X] T019 [US1] Document and capture US1 independent validation through FSI transcript and focused `dotnet test` output

**Checkpoint**: US1 is independently usable: applications can show or hide the standard state display and observe current layout/top context/state.

---

## Phase 4: User Story 2 (US2) - Understand Nested Layers and Permanent Contexts

### Tests First

- [X] T020 [P] [US2] Add tests for stack entry kind mapping: permanent/stateful, popup, temporary held, and unknown contexts
- [X] T021 [P] [US2] Add tests for ordered full stack, exactly one top context, persistent flags, entered-by keys, and stateful mode state retention
- [X] T022 [P] [US2] Add compact-mode tests for stack condensation and omission metadata when stack depth exceeds compact display limits
- [X] T023 [P] [US2] Add update-path tests for popup push/pop, held-layer push/release, nested layers, and focus-loss cleanup diagnostics
- [X] T041 [P] [US2] Add tests for multiple active held layers where one key is released out of order, including expected stack recovery and displayed diagnostic/context

### Implementation

- [X] T024 [US2] Implement stack entry derivation from `InputRuntime.ModeStack`, mode definitions, held frames, and current state
- [X] T025 [US2] Implement compact stack condensation and expanded full-stack preservation with `KeyboardStateDisplayOmission` metadata
- [X] T026 [US2] Render permanent/stateful, popup, held, active-top, and partial/unknown context distinctions without text overlap for representative stack depth

**Checkpoint**: US2 is independently testable: the display distinguishes persistent, stateful, popup, held, nested, and active-top context.

---

## Phase 5: User Story 3 (US3) - Learn Available Keys in the Current Context

### Tests First

- [X] T027 [P] [US3] Add tests that label hints include only bindings available in the active top context and matching state
- [X] T028 [P] [US3] Add tests for compact/expanded label caps and omitted-label counts
- [X] T029 [P] [US3] Add tests for pending sequence display, timeout/disambiguation fields, and option-controlled omission
- [X] T030 [P] [US3] Add tests for most recent resolved command selection from `InputEffect list`
- [X] T031 [P] [US3] Add tests for most recent actionable diagnostic selection and invalid-layout partial rendering

### Implementation

- [X] T032 [US3] Implement current-top-context label extraction, outcome text, state filtering, layout-label fallback, and label caps
- [X] T033 [US3] Implement pending sequence, recent command, diagnostic, omission, and `IsPartial` display-model behavior
- [X] T034 [US3] Render optional hints in compact and expanded density while preserving orientation fields ahead of lower-priority hints

**Checkpoint**: US3 is independently testable: optional hints explain current-context keys, pending input, recent command feedback, and latest actionable diagnostic.

---

## Phase 6: Integration & Polish

- [X] T035 Update `samples/KeyboardInputGallery/Program.fs` to consume `renderKeyboardStateDisplayAt` and demonstrate compact/expanded/hidden behavior without custom state visualization logic
- [X] T036 Update smoke evidence so `tests/Smoke.Tests/Tests.fs` captures `specs/004-keyboard-state-display/readiness/sample-smoke/keyboard-input-gallery-state-display.txt`
- [X] T037 Refresh package surface baseline evidence for `FS.Skia.UI.KeyboardInput`
- [X] T038 Run `dotnet test` and record the relevant passing output for semantic, package, and smoke coverage
- [X] T039 Run `dotnet fsi scripts/input-prelude.fsx` and confirm the public API transcript includes keyboard state display construction
- [X] T042 Measure representative `keyboardStateDisplay` model creation time and record evidence that it stays under 1 ms for compact, expanded, nested stack, 12-label, pending-sequence, recent-command, diagnostic, and partial-layout snapshots
- [X] T040 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/004-keyboard-state-display --graph-only` and then the full evidence audit before implementation is declared ready

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |
