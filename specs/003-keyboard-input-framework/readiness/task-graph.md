# Task Graph — 003-keyboard-input-framework

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 72 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Confirm current branch, feature directory, and pre"]:::done
  T002["T002 Create readiness scaffolding for FSI transcripts, "]:::done
  T003["T003 Record the YAML parser adoption note and pinned `Y"]:::done
  T004["T004 Inventory affected projects and expected file addi"]:::done
  T005["T005 Record feature Tier 1 classification, public API i"]:::done
  T006["T006 Confirm Principle IV applies because `InputRuntime"]:::done
  T007["T007 Draft `src/Lib/KeyboardInput.fsi` with command reg"]:::done
  T008["T008 Add `src/Lib/KeyboardInput.fs` implementation skel"]:::done
  T009["T009 Add `YamlDotNet` `17.1.0` to `src/Lib/Lib.fsproj` "]:::done
  T010["T010 Add `tests/Lib.Tests/KeyboardInputTests.fs` to the"]:::done
  T011["T011 Create initial readiness sample config and invalid"]:::done
  T012["T012 Create `scripts/input-prelude.fsx` that exercises "]:::done
  T013["T013 Define shared test helpers for command registries,"]:::done
  T014["T014 Exercise the draft `.fsi` through `dotnet fsi scri"]:::done
  T015["T015 Record the initial surface-area baseline for `FS.S"]:::done
  T016["T016 Record unsupported-scope behavior for touch, gamep"]:::done
  T017["T017 Add semantic tests for valid modal YAML parsing in"]:::done
  T018["T018 Add pure transition tests for normal bindings, pop"]:::done
  T019["T019 Add replay transcript fixture for representative m"]:::done
  T020["T020 Add FSI prelude assertions covering the US1 comman"]:::done
  T021["T021 Implement command registry construction, duplicate"]:::done
  T022["T022 Implement YAML parsing for version, layouts, modes"]:::done
  T023["T023 Implement mode stack initialization with base stan"]:::done
  T024["T024 Implement key-down binding resolution for position"]:::done
  T025["T025 Implement key-up release behavior for temporary he"]:::done
  T026["T026 Implement deterministic event recording and `repla"]:::done
  T027["T027 Add actionable diagnostics for invalid YAML, unkno"]:::done
  T028["T028 Document US1 independent validation in `quickstart"]:::done
  T029["T029 Add semantic tests for stateful mode default state"]:::done
  T030["T030 Add pure transition tests for state transitions, s"]:::done
  T031["T031 Add replay evidence for focus loss, lost key-up re"]:::done
  T032["T032 Extend FSI prelude assertions for stateful selecti"]:::done
  T033["T033 Implement stateful mode validation requiring non-e"]:::done
  T034["T034 Implement `SetState` binding outcomes and state gu"]:::done
  T035["T035 Implement popup cancellation, timeout handling, an"]:::done
  T036["T036 Implement focus-loss cleanup for pressed keys and "]:::done
  T037["T037 Emit diagnostics for stale input events, ambiguous"]:::done
  T038["T038 Update readiness replay artifacts and quickstart n"]:::done
  T039["T039 Add semantic tests for layout profile validation a"]:::done
  T040["T040 Add bigram report tests for top weighted pairs, sa"]:::done
  T041["T041 Add non-mutation tests proving `analyzeBigrams` do"]:::done
  T042["T042 Implement layout label resolution from physical ke"]:::done
  T043["T043 Implement bigram scoring inputs from command-pair "]:::done
  T044["T044 Implement `BigramReport` top pairs, risks, suggest"]:::done
  T045["T045 Add sample bigram data to `modal-input.yaml` and d"]:::done
  T046["T046 Add tests for invalid duplicate binding, unregiste"]:::done
  T047["T047 Add tests for `layoutState` and `LayoutStateChange"]:::done
  T048["T048 Add package or FSI evidence that applications can "]:::done
  T049["T049 Implement full validation aggregation with diagnos"]:::done
  T050["T050 Implement disambiguation policy handling for pendi"]:::done
  T051["T051 Implement `SetLayout` handling and diagnostics for"]:::done
  T052["T052 Implement `layoutState` with active mode stack, ac"]:::done
  T053["T053 Add `samples/KeyboardInputGallery` showing modal i"]:::done
  T054["T054 Add smoke-test coverage for the sample gallery sta"]:::done
  T055["T055 Update quickstart with application-author workflow"]:::done
  T056["T056 Add tests proving standard key input works with co"]:::done
  T057["T057 Add contract tests for command intent, command pla"]:::done
  T058["T058 Add optional command-intent data parsing and valid"]:::done
  T059["T059 Expose command intent and command plan status data"]:::done
  T060["T060 Emit `UnsatisfiedCommandIntent` diagnostics for in"]:::done
  T061["T061 Document that advanced command intent is opt-in an"]:::done
  T062["T062 Refresh `tests/Package.Tests` surface-area baselin"]:::done
  T063["T063 Run `dotnet fsi scripts/input-prelude.fsx` and sto"]:::done
  T064["T064 Run `dotnet test tests/Lib.Tests/Lib.Tests.fsproj`"]:::done
  T065["T065 Run `dotnet test tests/Package.Tests/Package.Tests"]:::done
  T066["T066 Run `dotnet test tests/Smoke.Tests/Smoke.Tests.fsp"]:::done
  T067["T067 Run full `dotnet test`"]:::done
  T068["T068 Run sample gallery smoke command or document platf"]:::done
  T069["T069 Verify performance evidence for 95% event resoluti"]:::done
  T070["T070 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T071["T071 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T072["T072 Update `tasks.md` statuses only with real evidence"]:::done
  T005 --> T006
  T006 --> T007
  T007 --> T008
  T006 --> T008
  T007 --> T009
  T008 --> T009
  T006 --> T009
  T006 --> T010
  T006 --> T011
  T007 --> T012
  T006 --> T012
  T007 --> T013
  T010 --> T013
  T006 --> T013
  T007 --> T014
  T012 --> T014
  T006 --> T014
  T007 --> T015
  T006 --> T015
  T007 --> T016
  T006 --> T016
  T016 --> T017
  T016 --> T018
  T016 --> T019
  T016 --> T020
  T017 --> T021
  T013 --> T021
  T016 --> T021
  T017 --> T022
  T011 --> T022
  T021 --> T022
  T016 --> T022
  T018 --> T023
  T021 --> T023
  T016 --> T023
  T018 --> T024
  T023 --> T024
  T016 --> T024
  T018 --> T025
  T024 --> T025
  T016 --> T025
  T019 --> T026
  T024 --> T026
  T025 --> T026
  T016 --> T026
  T017 --> T027
  T022 --> T027
  T016 --> T027
  T020 --> T028
  T024 --> T028
  T026 --> T028
  T016 --> T028
  T028 --> T029
  T028 --> T030
  T028 --> T031
  T028 --> T032
  T029 --> T033
  T013 --> T033
  T028 --> T033
  T030 --> T034
  T033 --> T034
  T028 --> T034
  T030 --> T035
  T034 --> T035
  T028 --> T035
  T031 --> T036
  T035 --> T036
  T028 --> T036
  T031 --> T037
  T036 --> T037
  T028 --> T037
  T032 --> T038
  T035 --> T038
  T036 --> T038
  T028 --> T038
  T038 --> T039
  T038 --> T040
  T038 --> T041
  T039 --> T042
  T013 --> T042
  T038 --> T042
  T040 --> T043
  T042 --> T043
  T038 --> T043
  T040 --> T044
  T041 --> T044
  T043 --> T044
  T038 --> T044
  T044 --> T045
  T038 --> T045
  T045 --> T046
  T045 --> T047
  T045 --> T048
  T046 --> T049
  T027 --> T049
  T033 --> T049
  T045 --> T049
  T047 --> T050
  T049 --> T050
  T045 --> T050
  T047 --> T051
  T049 --> T051
  T045 --> T051
  T047 --> T052
  T050 --> T052
  T051 --> T052
  T045 --> T052
  T048 --> T053
  T052 --> T053
  T044 --> T053
  T045 --> T053
  T053 --> T054
  T045 --> T054
  T052 --> T055
  T053 --> T055
  T045 --> T055
  T055 --> T056
  T055 --> T057
  T057 --> T058
  T049 --> T058
  T055 --> T058
  T056 --> T059
  T058 --> T059
  T055 --> T059
  T057 --> T060
  T058 --> T060
  T055 --> T060
  T056 --> T061
  T059 --> T061
  T060 --> T061
  T055 --> T061
  T061 --> T062
  T028 --> T063
  T038 --> T063
  T045 --> T063
  T055 --> T063
  T061 --> T063
  T027 --> T064
  T037 --> T064
  T044 --> T064
  T052 --> T064
  T060 --> T064
  T061 --> T064
  T062 --> T065
  T061 --> T065
  T054 --> T066
  T061 --> T066
  T064 --> T067
  T065 --> T067
  T066 --> T067
  T061 --> T067
  T053 --> T068
  T054 --> T068
  T061 --> T068
  T026 --> T069
  T044 --> T069
  T067 --> T069
  T061 --> T069
  T061 --> T070
  T070 --> T071
  T072 --> T071
  T061 --> T071
  T063 --> T072
  T064 --> T072
  T065 --> T072
  T066 --> T072
  T067 --> T072
  T068 --> T072
  T069 --> T072
  T061 --> T072
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Confirm current branch, feature directory, and prerequisite artifacts for `specs/003-keyboard-input-framework/`
T002 [X] Create readiness scaffolding for FSI transcripts, input replay, sample configs, surface baselines, package output, and sample smoke evidence
T003 [X] Record the YAML parser adoption note and pinned `YamlDotNet` version in feature evidence docs
T004 [X] Inventory affected projects and expected file additions in `src/Lib`, `tests/Lib.Tests`, `tests/Package.Tests`, `tests/Smoke.Tests`, `scripts`, and `samples/KeyboardInputGallery`
T005 [X] Record feature Tier 1 classification, public API impact, MVU applicability, and required real-evidence obligations
T006 [X] Confirm Principle IV applies because `InputRuntime` is stateful and document that no synthetic evidence is planned
T007 [X] Draft `src/Lib/KeyboardInput.fsi` with command registry, configuration, canonical model, mode stack, runtime, message, effect, layout-state, replay, bigram, diagnostics, and optional command-intent contracts
T008 [X] Add `src/Lib/KeyboardInput.fs` implementation skeleton matching the `.fsi` without top-level visibility modifiers
T009 [X] Add `YamlDotNet` `17.1.0` to `src/Lib/Lib.fsproj` and include `KeyboardInput.fsi` / `KeyboardInput.fs` in the correct compile order
T010 [X] Add `tests/Lib.Tests/KeyboardInputTests.fs` to the test project in the correct compile order
T011 [X] Create initial readiness sample config and invalid YAML fixtures under `specs/003-keyboard-input-framework/readiness/sample-configs/`
T012 [X] Create `scripts/input-prelude.fsx` that exercises registry creation, YAML parsing, validation, `init`, `update`, replay, and bigram analysis
T013 [X] Define shared test helpers for command registries, layouts, stateful modes, popup modes, temporary held modes, and key positions
T014 [X] Exercise the draft `.fsi` through `dotnet fsi scripts/input-prelude.fsx` and capture the transcript to readiness
T015 [X] Record the initial surface-area baseline for `FS.Skia.UI.KeyboardInput`
T016 [X] Record unsupported-scope behavior for touch, gamepad, automatic keymap rewriting, executable YAML host actions, and full command grammar execution
T017 [X] Add semantic tests for valid modal YAML parsing into `InputConfiguration` and validation into `CanonicalInputModel`
T018 [X] Add pure transition tests for normal bindings, popup push/pop, temporary held push/release, and emitted `CommandResolved` / `LayoutStateChanged` effects
T019 [X] Add replay transcript fixture for representative movement, popup, and held-mode key events
T020 [X] Add FSI prelude assertions covering the US1 command registry, modal YAML, `init`, and representative `update` paths
T021 [X] Implement command registry construction, duplicate command rejection, and canonical model validation for registered command identifiers
T022 [X] Implement YAML parsing for version, layouts, modes, bindings, disambiguation, bigram profile, and display options
T023 [X] Implement mode stack initialization with base standard or stateful mode and valid active layout
T024 [X] Implement key-down binding resolution for positional keymaps, normal command bindings, popup mode push, and temporary mode push
T025 [X] Implement key-up release behavior for temporary held modes and pressed-key tracking
T026 [X] Implement deterministic event recording and `replay` folding over `InputMsg` lists
T027 [X] Add actionable diagnostics for invalid YAML, unknown mode, unknown command, duplicate binding, and invalid host-action-like YAML
T028 [X] Document US1 independent validation in `quickstart.md` with the exact FSI and `dotnet test` commands
T029 [X] Add semantic tests for stateful mode default state validation, missing default rejection, and inspectable active state
T030 [X] Add pure transition tests for state transitions, state-dependent commands, popup restoration, and explicit popup state changes
T031 [X] Add replay evidence for focus loss, lost key-up recovery, and out-of-order release diagnostics
T032 [X] Extend FSI prelude assertions for stateful selection mode initialization and popup restoration
T033 [X] Implement stateful mode validation requiring non-empty state sets and valid default states
T034 [X] Implement `SetState` binding outcomes and state guards for mode-specific bindings
T035 [X] Implement popup cancellation, timeout handling, and restoration of the underlying stateful frame
T036 [X] Implement focus-loss cleanup for pressed keys and all active temporary held modes
T037 [X] Emit diagnostics for stale input events, ambiguous sequences, invalid mode state, and lost key release recovery
T038 [X] Update readiness replay artifacts and quickstart notes for state preservation behavior
T039 [X] Add semantic tests for layout profile validation across QWERTY, Dvorak, Colemak-style, and custom labels
T040 [X] Add bigram report tests for top weighted pairs, same-finger risk, long-travel risk, awkward hold risk, and suggestion limit
T041 [X] Add non-mutation tests proving `analyzeBigrams` does not rewrite the canonical model or YAML-derived keymap
T042 [X] Implement layout label resolution from physical key positions for display and analysis
T043 [X] Implement bigram scoring inputs from command-pair weights, binding positions, hand, finger, row, and column metadata
T044 [X] Implement `BigramReport` top pairs, risks, suggestions, and score summary without mutating configuration
T045 [X] Add sample bigram data to `modal-input.yaml` and document analysis-only behavior in quickstart evidence
T046 [X] Add tests for invalid duplicate binding, unregistered command, invalid host action, unknown layout, and impossible transition fixtures
T047 [X] Add tests for `layoutState` and `LayoutStateChanged` effects when active modes, held keys, pending sequences, and layout labels change
T048 [X] Add package or FSI evidence that applications can inspect `CanonicalInputModel` without parsing raw YAML
T049 [X] Implement full validation aggregation with diagnostics that identify affected mode, binding, transition, command, or layout entry
T050 [X] Implement disambiguation policy handling for pending sequences, prefix conflicts, and timeout resolution
T051 [X] Implement `SetLayout` handling and diagnostics for unknown layout changes after configuration load
T052 [X] Implement `layoutState` with active mode stack, active stateful mode, held modes, pending popup or sequence, active layout, and labels
T053 [X] Add `samples/KeyboardInputGallery` showing modal input, stateful selection, popup space mode, temporary copy/delete modes, bigram analysis, YAML failures, and optional layout-state display
T054 [X] Add smoke-test coverage for the sample gallery startup path where practical
T055 [X] Update quickstart with application-author workflow and sample smoke command
T056 [X] Add tests proving standard key input works with command intent disabled and no command grammar configuration
T057 [X] Add contract tests for command intent, command plan status, failure report fields, approval state, and unsatisfied intent diagnostics
T058 [X] Add optional command-intent data parsing and validation only where configured separately from standard key bindings
T059 [X] Expose command intent and command plan status data without implementing grammar parsing or command execution
T060 [X] Emit `UnsatisfiedCommandIntent` diagnostics for invalid optional intent policies
T061 [X] Document that advanced command intent is opt-in and data-only in v1
T062 [X] Refresh `tests/Package.Tests` surface-area baselines for the Tier 1 public API
T063 [X] Run `dotnet fsi scripts/input-prelude.fsx` and store the final transcript under readiness
T064 [X] Run `dotnet test tests/Lib.Tests/Lib.Tests.fsproj`
T065 [X] Run `dotnet test tests/Package.Tests/Package.Tests.fsproj`
T066 [X] Run `dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj`
T067 [X] Run full `dotnet test`
T068 [X] Run sample gallery smoke command or document platform-specific Vulkan constraints in readiness evidence
T069 [X] Verify performance evidence for 95% event resolution under 16 ms, 10,000-event replay under 1 second, and 500-binding / 2,000-pair bigram report under 1 second
T070 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/003-keyboard-input-framework --graph-only`
T071 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/003-keyboard-input-framework` and resolve or disclose any findings
T072 [X] Update `tasks.md` statuses only with real evidence paths or explicit synthetic-evidence disclosures
```

