# Tasks: Keyboard Input Framework

**Feature branch**: `003-keyboard-input-framework`
**Spec**: `specs/003-keyboard-input-framework/spec.md`
**Plan**: `specs/003-keyboard-input-framework/plan.md`

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

A task tagged `[US*]` may only be marked `[X]` when the change is reachable
from a user-facing entry point and that path was actually exercised: an FSI
session against the packed library, a smoke run of the sample application,
a manual walk-through with transcript, or a captured readiness artifact.
Domain, model, or core-layer changes alone do not satisfy `[X]` for a
`[US*]` task.

For this feature, `[X]` on any stateful story also requires Elmish/MVU
evidence: the public `InputRuntime` / `InputMsg` / `InputEffect` contract was
exercised, pure `update` transitions were tested, emitted effects were
asserted, and host-edge evidence was captured where safe.

## Task Annotations

- **[P]** — parallel-safe (no deps inside the current phase)
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The `speckit.evidence.graph` command refuses to
proceed with dangling references.

---

## Phase 1: Setup

- [X] T001 Confirm current branch, feature directory, and prerequisite artifacts for `specs/003-keyboard-input-framework/`
- [X] T002 [P] Create readiness scaffolding for FSI transcripts, input replay, sample configs, surface baselines, package output, and sample smoke evidence
- [X] T003 [P] Record the YAML parser adoption note and pinned `YamlDotNet` version in feature evidence docs
- [X] T004 [P] Inventory affected projects and expected file additions in `src/Lib`, `tests/Lib.Tests`, `tests/Package.Tests`, `tests/Smoke.Tests`, `scripts`, and `samples/KeyboardInputGallery`
- [X] T005 Record feature Tier 1 classification, public API impact, MVU applicability, and required real-evidence obligations
- [X] T006 Confirm Principle IV applies because `InputRuntime` is stateful and document that no synthetic evidence is planned

**Checkpoint**: Setup complete.

---

## Phase 2: Foundation

- [X] T007 Draft `src/Lib/KeyboardInput.fsi` with command registry, configuration, canonical model, mode stack, runtime, message, effect, layout-state, replay, bigram, diagnostics, and optional command-intent contracts
- [X] T008 Add `src/Lib/KeyboardInput.fs` implementation skeleton matching the `.fsi` without top-level visibility modifiers
- [X] T009 Add `YamlDotNet` `17.1.0` to `src/Lib/Lib.fsproj` and include `KeyboardInput.fsi` / `KeyboardInput.fs` in the correct compile order
- [X] T010 [P] Add `tests/Lib.Tests/KeyboardInputTests.fs` to the test project in the correct compile order
- [X] T011 [P] Create initial readiness sample config and invalid YAML fixtures under `specs/003-keyboard-input-framework/readiness/sample-configs/`
- [X] T012 [P] Create `scripts/input-prelude.fsx` that exercises registry creation, YAML parsing, validation, `init`, `update`, replay, and bigram analysis
- [X] T013 Define shared test helpers for command registries, layouts, stateful modes, popup modes, temporary held modes, and key positions
- [X] T014 Exercise the draft `.fsi` through `dotnet fsi scripts/input-prelude.fsx` and capture the transcript to readiness
- [X] T015 Record the initial surface-area baseline for `FS.Skia.UI.KeyboardInput`
- [X] T016 Record unsupported-scope behavior for touch, gamepad, automatic keymap rewriting, executable YAML host actions, and full command grammar execution

**Checkpoint**: Foundation ready - story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Define Keyboard-First Input Maps

### Tests First

- [X] T017 [P] [US1] Add semantic tests for valid modal YAML parsing into `InputConfiguration` and validation into `CanonicalInputModel`
- [X] T018 [P] [US1] Add pure transition tests for normal bindings, popup push/pop, temporary held push/release, and emitted `CommandResolved` / `LayoutStateChanged` effects
- [X] T019 [P] [US1] Add replay transcript fixture for representative movement, popup, and held-mode key events
- [X] T020 [P] [US1] Add FSI prelude assertions covering the US1 command registry, modal YAML, `init`, and representative `update` paths

### Implementation

- [X] T021 [US1] Implement command registry construction, duplicate command rejection, and canonical model validation for registered command identifiers
- [X] T022 [US1] Implement YAML parsing for version, layouts, modes, bindings, disambiguation, bigram profile, and display options
- [X] T023 [US1] Implement mode stack initialization with base standard or stateful mode and valid active layout
- [X] T024 [US1] Implement key-down binding resolution for positional keymaps, normal command bindings, popup mode push, and temporary mode push
- [X] T025 [US1] Implement key-up release behavior for temporary held modes and pressed-key tracking
- [X] T026 [US1] Implement deterministic event recording and `replay` folding over `InputMsg` lists
- [X] T027 [US1] Add actionable diagnostics for invalid YAML, unknown mode, unknown command, duplicate binding, and invalid host-action-like YAML
- [X] T028 [US1] Document US1 independent validation in `quickstart.md` with the exact FSI and `dotnet test` commands

**Checkpoint**: User Story 1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) - Preserve Stateful Modes

### Tests First

- [X] T029 [P] [US2] Add semantic tests for stateful mode default state validation, missing default rejection, and inspectable active state
- [X] T030 [P] [US2] Add pure transition tests for state transitions, state-dependent commands, popup restoration, and explicit popup state changes
- [X] T031 [P] [US2] Add replay evidence for focus loss, lost key-up recovery, and out-of-order release diagnostics
- [X] T032 [P] [US2] Extend FSI prelude assertions for stateful selection mode initialization and popup restoration

### Implementation

- [X] T033 [US2] Implement stateful mode validation requiring non-empty state sets and valid default states
- [X] T034 [US2] Implement `SetState` binding outcomes and state guards for mode-specific bindings
- [X] T035 [US2] Implement popup cancellation, timeout handling, and restoration of the underlying stateful frame
- [X] T036 [US2] Implement focus-loss cleanup for pressed keys and all active temporary held modes
- [X] T037 [US2] Emit diagnostics for stale input events, ambiguous sequences, invalid mode state, and lost key release recovery
- [X] T038 [US2] Update readiness replay artifacts and quickstart notes for state preservation behavior

**Checkpoint**: User Story 2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) - Tune Ergonomic Layouts

### Tests First

- [X] T039 [P] [US3] Add semantic tests for layout profile validation across QWERTY, Dvorak, Colemak-style, and custom labels
- [X] T040 [P] [US3] Add bigram report tests for top weighted pairs, same-finger risk, long-travel risk, awkward hold risk, and suggestion limit
- [X] T041 [P] [US3] Add non-mutation tests proving `analyzeBigrams` does not rewrite the canonical model or YAML-derived keymap

### Implementation

- [X] T042 [US3] Implement layout label resolution from physical key positions for display and analysis
- [X] T043 [US3] Implement bigram scoring inputs from command-pair weights, binding positions, hand, finger, row, and column metadata
- [X] T044 [US3] Implement `BigramReport` top pairs, risks, suggestions, and score summary without mutating configuration
- [X] T045 [US3] Add sample bigram data to `modal-input.yaml` and document analysis-only behavior in quickstart evidence

**Checkpoint**: User Story 3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) - Configure and Inspect Input Behavior

### Tests First

- [X] T046 [P] [US4] Add tests for invalid duplicate binding, unregistered command, invalid host action, unknown layout, and impossible transition fixtures
- [X] T047 [P] [US4] Add tests for `layoutState` and `LayoutStateChanged` effects when active modes, held keys, pending sequences, and layout labels change
- [X] T048 [P] [US4] Add package or FSI evidence that applications can inspect `CanonicalInputModel` without parsing raw YAML

### Implementation

- [X] T049 [US4] Implement full validation aggregation with diagnostics that identify affected mode, binding, transition, command, or layout entry
- [X] T050 [US4] Implement disambiguation policy handling for pending sequences, prefix conflicts, and timeout resolution
- [X] T051 [US4] Implement `SetLayout` handling and diagnostics for unknown layout changes after configuration load
- [X] T052 [US4] Implement `layoutState` with active mode stack, active stateful mode, held modes, pending popup or sequence, active layout, and labels
- [X] T053 [US4] Add `samples/KeyboardInputGallery` showing modal input, stateful selection, popup space mode, temporary copy/delete modes, bigram analysis, YAML failures, and optional layout-state display
- [X] T054 [US4] Add smoke-test coverage for the sample gallery startup path where practical
- [X] T055 [US4] Update quickstart with application-author workflow and sample smoke command

**Checkpoint**: User Story 4 is fully functional and testable independently.

---

## Phase 7: User Story 5 (US5) - Support Advanced Command Intent

### Tests First

- [X] T056 [P] [US5] Add tests proving standard key input works with command intent disabled and no command grammar configuration
- [X] T057 [P] [US5] Add contract tests for command intent, command plan status, failure report fields, approval state, and unsatisfied intent diagnostics

### Implementation

- [X] T058 [US5] Add optional command-intent data parsing and validation only where configured separately from standard key bindings
- [X] T059 [US5] Expose command intent and command plan status data without implementing grammar parsing or command execution
- [X] T060 [US5] Emit `UnsatisfiedCommandIntent` diagnostics for invalid optional intent policies
- [X] T061 [US5] Document that advanced command intent is opt-in and data-only in v1

**Checkpoint**: User Story 5 is fully functional and testable independently.

---

## Phase 8: Integration & Polish

- [X] T062 Refresh `tests/Package.Tests` surface-area baselines for the Tier 1 public API
- [X] T063 Run `dotnet fsi scripts/input-prelude.fsx` and store the final transcript under readiness
- [X] T064 Run `dotnet test tests/Lib.Tests/Lib.Tests.fsproj`
- [X] T065 Run `dotnet test tests/Package.Tests/Package.Tests.fsproj`
- [X] T066 Run `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj`
- [X] T067 Run full `dotnet test`
- [X] T068 Run sample gallery smoke command or document platform-specific Vulkan constraints in readiness evidence
- [X] T069 Verify performance evidence for 95% event resolution under 16 ms, 10,000-event replay under 1 second, and 500-binding / 2,000-pair bigram report under 1 second
- [X] T070 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/003-keyboard-input-framework --graph-only`
- [X] T072 Update `tasks.md` statuses only with real evidence paths or explicit synthetic-evidence disclosures
- [X] T071 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/003-keyboard-input-framework` and resolve or disclose any findings

**Checkpoint**: Feature evidence is complete and ready for review.

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| _(none yet)_ | | | |
