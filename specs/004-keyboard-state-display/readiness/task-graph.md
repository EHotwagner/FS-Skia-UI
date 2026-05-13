# Task Graph — 004-keyboard-state-display

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 42 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Confirm feature artifact set is present (`spec.md`"]:::done
  T002["T002 Create readiness directories for FSI transcripts, "]:::done
  T003["T003 Identify existing keyboard input implementation, t"]:::done
  T004["T004 Record Tier 1 classification, affected public modu"]:::done
  T005["T005 Draft `KeyboardInput.fsi` public contracts for dis"]:::done
  T006["T006 Record Elmish/MVU applicability: no new library `M"]:::done
  T007["T007 Add or update FSI/prelude coverage in `scripts/inp"]:::done
  T008["T008 Add semantic test scaffolding in `tests/Lib.Tests/"]:::done
  T009["T009 Add or reserve surface-area baseline artifact path"]:::done
  T010["T010 Define failure-diagnostic expectations for missing"]:::done
  T011["T011 Exercise the drafted `.fsi` with FSI and capture t"]:::done
  T012["T012 Add public-surface tests for default, compact, exp"]:::done
  T013["T013 Add semantic tests that assert active layout id/di"]:::done
  T014["T014 Add tests that drive `KeyboardInput.update` layout"]:::done
  T015["T015 Add `Scene.describe` tests for `renderKeyboardStat"]:::done
  T016["T016 Implement display option defaults and the pure `ke"]:::done
  T017["T017 Implement standard scene rendering for compact/exp"]:::done
  T018["T018 Preserve compatibility for `layoutState`, `renderL"]:::done
  T019["T019 Document and capture US1 independent validation th"]:::done
  T020["T020 Add tests for stack entry kind mapping: permanent/"]:::done
  T021["T021 Add tests for ordered full stack, exactly one top "]:::done
  T022["T022 Add compact-mode tests for stack condensation and "]:::done
  T023["T023 Add update-path tests for popup push/pop, held-lay"]:::done
  T024["T024 Implement stack entry derivation from `InputRuntim"]:::done
  T025["T025 Implement compact stack condensation and expanded "]:::done
  T026["T026 Render permanent/stateful, popup, held, active-top"]:::done
  T027["T027 Add tests that label hints include only bindings a"]:::done
  T028["T028 Add tests for compact/expanded label caps and omit"]:::done
  T029["T029 Add tests for pending sequence display, timeout/di"]:::done
  T030["T030 Add tests for most recent resolved command selecti"]:::done
  T031["T031 Add tests for most recent actionable diagnostic se"]:::done
  T032["T032 Implement current-top-context label extraction, ou"]:::done
  T033["T033 Implement pending sequence, recent command, diagno"]:::done
  T034["T034 Render optional hints in compact and expanded dens"]:::done
  T035["T035 Update `samples/KeyboardInputGallery/Program.fs` t"]:::done
  T036["T036 Update smoke evidence so `tests/Smoke.Tests/Tests."]:::done
  T037["T037 Refresh package surface baseline evidence for `FS."]:::done
  T038["T038 Run `dotnet test` and record the relevant passing "]:::done
  T039["T039 Run `dotnet fsi scripts/input-prelude.fsx` and con"]:::done
  T040["T040 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T041["T041 Add tests for multiple active held layers where on"]:::done
  T042["T042 Measure representative `keyboardStateDisplay` mode"]:::done
  T004 --> T005
  T004 --> T006
  T004 --> T007
  T004 --> T008
  T004 --> T009
  T004 --> T010
  T005 --> T011
  T007 --> T011
  T004 --> T011
  T011 --> T012
  T011 --> T013
  T011 --> T014
  T011 --> T015
  T005 --> T016
  T012 --> T016
  T013 --> T016
  T014 --> T016
  T011 --> T016
  T015 --> T017
  T016 --> T017
  T011 --> T017
  T016 --> T018
  T017 --> T018
  T011 --> T018
  T011 --> T019
  T016 --> T019
  T017 --> T019
  T018 --> T019
  T019 --> T020
  T019 --> T021
  T019 --> T022
  T019 --> T023
  T020 --> T024
  T021 --> T024
  T023 --> T024
  T041 --> T024
  T019 --> T024
  T022 --> T025
  T024 --> T025
  T019 --> T025
  T024 --> T026
  T025 --> T026
  T019 --> T026
  T026 --> T027
  T026 --> T028
  T026 --> T029
  T026 --> T030
  T026 --> T031
  T027 --> T032
  T028 --> T032
  T026 --> T032
  T029 --> T033
  T030 --> T033
  T031 --> T033
  T026 --> T033
  T032 --> T034
  T033 --> T034
  T026 --> T034
  T017 --> T035
  T026 --> T035
  T034 --> T035
  T035 --> T036
  T034 --> T036
  T005 --> T037
  T016 --> T037
  T024 --> T037
  T032 --> T037
  T033 --> T037
  T034 --> T037
  T019 --> T038
  T026 --> T038
  T034 --> T038
  T036 --> T038
  T037 --> T038
  T011 --> T039
  T019 --> T039
  T034 --> T039
  T038 --> T040
  T039 --> T040
  T042 --> T040
  T034 --> T040
  T019 --> T041
  T034 --> T042
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Confirm feature artifact set is present (`spec.md`, `plan.md`, `research.md`, `data-model.md`, `quickstart.md`, `contracts/public-api.md`)
T002 [X] Create readiness directories for FSI transcripts, sample smoke output, and surface baselines under `specs/004-keyboard-state-display/readiness/`
T003 [X] Identify existing keyboard input implementation, test, sample, prelude, smoke, and package baseline files affected by the feature
T004 [X] Record Tier 1 classification, affected public module, no-new-dependency constraint, and evidence obligations for pure display model plus Skia scene renderer
T005 [X] Draft `KeyboardInput.fsi` public contracts for display visibility, density, options, model records, omissions, and render/model functions
T006 [X] Record Elmish/MVU applicability: no new library `Model`/`Msg`/`Effect` contract is introduced, but tests must exercise existing `InputRuntime`, `InputMsg`, and `InputEffect` transitions through the public boundary
T007 [X] Add or update FSI/prelude coverage in `scripts/input-prelude.fsx` for compact, expanded, and hidden display construction through the public surface
T008 [X] Add semantic test scaffolding in `tests/Lib.Tests/KeyboardInputTests.fs` for reusable display fixtures, recent-effect capture, diagnostics, invalid layouts, and scene descriptions
T009 [X] Add or reserve surface-area baseline artifact path `specs/004-keyboard-state-display/readiness/surface-baselines/FS.Skia.UI.txt`
T010 [X] Define failure-diagnostic expectations for missing/invalid layout and non-actionable diagnostic filtering
T011 [X] Exercise the drafted `.fsi` with FSI and capture the transcript to `specs/004-keyboard-state-display/readiness/fsi/keyboard-state-display-prelude.txt`
T012 [X] Add public-surface tests for default, compact, expanded, and hidden display options
T013 [X] Add semantic tests that assert active layout id/display name, active top context, active state, and hidden-mode empty model/scene
T014 [X] Add tests that drive `KeyboardInput.update` layout/state changes and assert state display updates from emitted runtime/effects
T015 [X] Add `Scene.describe` tests for `renderKeyboardStateDisplay` and `renderKeyboardStateDisplayAt` returning stable text/shape primitives without custom app drawing
T016 [X] Implement display option defaults and the pure `keyboardStateDisplay` projection for visible/hidden orientation fields in `src/Lib/KeyboardInput.fs`
T017 [X] Implement standard scene rendering for compact/expanded orientation and hidden empty scene behavior using existing `Scene` primitives
T018 [X] Preserve compatibility for `layoutState`, `renderLayoutState`, and `renderLayoutStateAt`, delegating only where behavior remains compatible
T019 [X] Document and capture US1 independent validation through FSI transcript and focused `dotnet test` output
T020 [X] Add tests for stack entry kind mapping: permanent/stateful, popup, temporary held, and unknown contexts
T021 [X] Add tests for ordered full stack, exactly one top context, persistent flags, entered-by keys, and stateful mode state retention
T022 [X] Add compact-mode tests for stack condensation and omission metadata when stack depth exceeds compact display limits
T023 [X] Add update-path tests for popup push/pop, held-layer push/release, nested layers, and focus-loss cleanup diagnostics
T024 [X] Implement stack entry derivation from `InputRuntime.ModeStack`, mode definitions, held frames, and current state
T025 [X] Implement compact stack condensation and expanded full-stack preservation with `KeyboardStateDisplayOmission` metadata
T026 [X] Render permanent/stateful, popup, held, active-top, and partial/unknown context distinctions without text overlap for representative stack depth
T027 [X] Add tests that label hints include only bindings available in the active top context and matching state
T028 [X] Add tests for compact/expanded label caps and omitted-label counts
T029 [X] Add tests for pending sequence display, timeout/disambiguation fields, and option-controlled omission
T030 [X] Add tests for most recent resolved command selection from `InputEffect list`
T031 [X] Add tests for most recent actionable diagnostic selection and invalid-layout partial rendering
T032 [X] Implement current-top-context label extraction, outcome text, state filtering, layout-label fallback, and label caps
T033 [X] Implement pending sequence, recent command, diagnostic, omission, and `IsPartial` display-model behavior
T034 [X] Render optional hints in compact and expanded density while preserving orientation fields ahead of lower-priority hints
T035 [X] Update `samples/KeyboardInputGallery/Program.fs` to consume `renderKeyboardStateDisplayAt` and demonstrate compact/expanded/hidden behavior without custom state visualization logic
T036 [X] Update smoke evidence so `tests/Smoke.Tests/Tests.fs` captures `specs/004-keyboard-state-display/readiness/sample-smoke/keyboard-input-gallery-state-display.txt`
T037 [X] Refresh package surface baseline evidence for `FS.Skia.UI.KeyboardInput`
T038 [X] Run `dotnet test` and record the relevant passing output for semantic, package, and smoke coverage
T039 [X] Run `dotnet fsi scripts/input-prelude.fsx` and confirm the public API transcript includes keyboard state display construction
T040 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/004-keyboard-state-display --graph-only` and then the full evidence audit before implementation is declared ready
T041 [X] Add tests for multiple active held layers where one key is released out of order, including expected stack recovery and displayed diagnostic/context
T042 [X] Measure representative `keyboardStateDisplay` model creation time and record evidence that it stays under 1 ms for compact, expanded, nested stack, 12-label, pending-sequence, recent-command, diagnostic, and partial-layout snapshots
```

