# Task Graph — 005-add-yoga-net-layout

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 55 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Confirm the current `src/Layout` compile order, pu"]:::done
  T002["T002 Add the pinned `Yoga.Net` `3.2.3` package referenc"]:::done
  T003["T003 Create readiness scaffolding under `specs/005-add-"]:::done
  T004["T004 Record Tier 1 evidence obligations, dependency pin"]:::done
  T005["T005 Draft automatic layout geometry, sizing, visibilit"]:::done
  T006["T006 Draft default constructors and public evaluator fu"]:::done
  T007["T007 Add matching implementation placeholders and compi"]:::done
  T008["T008 Add semantic test helpers in `tests/Layout.Tests/T"]:::done
  T009["T009 Add `scripts/layout-prelude.fsx` coverage for the "]:::done
  T010["T010 Add or update package surface baseline generation "]:::done
  T011["T011 Define validation rules for duplicate node ids, in"]:::done
  T012["T012 Define the Yoga.Net adapter boundary so Yoga nodes"]:::done
  T013["T013 Define the deterministic fallback geometry policy "]:::done
  T014["T014 Define the invalidation model for node intent, vis"]:::done
  T015["T015 Build the foundation surface and capture an initia"]:::done
  T016["T016 Add semantic tests for row, column, and wrap conta"]:::done
  T017["T017 Add semantic tests for parent padding, child margi"]:::done
  T018["T018 Add semantic tests for fixed, min, max, flex grow,"]:::done
  T019["T019 Add public API tests for custom content measuremen"]:::done
  T020["T020 Implement default automatic layout records and hel"]:::done
  T021["T021 Implement Yoga.Net style mapping for direction, wr"]:::done
  T022["T022 Implement recursive layout tree evaluation through"]:::done
  T023["T023 Implement custom leaf measurement callback bridgin"]:::done
  T024["T024 Implement deterministic result ordering, finite lo"]:::done
  T025["T025 Capture US1 readiness evidence through `dotnet tes"]:::done
  T026["T026 Add semantic tests for mixed standard widgets and "]:::done
  T027["T027 Add resize and incremental evaluation tests provin"]:::done
  T028["T028 Add invalidation locality tests proving unaffected"]:::done
  T029["T029 Add render and hit-test tests proving computed log"]:::done
  T030["T030 Add fixtures or adapters that let existing element"]:::done
  T031["T031 Implement `evaluateIncremental` revision handling,"]:::done
  T032["T032 Implement `renderComputed` so scene content is pos"]:::done
  T033["T033 Implement `snapBounds` and `hitTestComputed` with "]:::done
  T034["T034 Add or update `samples/LayoutGraphGallery` and `sa"]:::done
  T035["T035 Capture US2 readiness evidence through widget/inva"]:::done
  T036["T036 Add semantic tests for invalid available space, in"]:::done
  T037["T037 Add semantic tests for unmeasurable content, inval"]:::done
  T038["T038 Add tests proving recoverable failures return stru"]:::done
  T039["T039 Implement structured diagnostic creation with node"]:::done
  T040["T040 Implement validation and normalization for availab"]:::done
  T041["T041 Implement safe fallback bounds for recoverable con"]:::done
  T042["T042 Implement hidden and collapsed node behavior so vi"]:::done
  T043["T043 Capture US3 readiness evidence for invalid/conflic"]:::done
  T044["T044 Refresh `FS.Skia.UI.Layout` surface-area baseline "]:::done
  T045["T045 Update `specs/005-add-yoga-net-layout/quickstart.m"]:::done
  T046["T046 Add performance evidence for representative 200-no"]:::done
  T047["T047 Run `dotnet restore`, `dotnet build`, and `dotnet "]:::done
  T048["T048 Run the layout FSI prelude and save `readiness/fsi"]:::done
  T049["T049 Run automatic layout sample smoke checks and save "]:::done
  T050["T050 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T051["T051 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T052["T052 Draft the stateful host/sample layout workflow con"]:::done
  T053["T053 Add pure transition tests for the host/sample layo"]:::done
  T054["T054 Add emitted-effect assertions and real interpreter"]:::done
  T055["T055 Document and test keyboard focus region alignment "]:::done
  T001 --> T004
  T002 --> T004
  T003 --> T004
  T004 --> T005
  T005 --> T006
  T004 --> T006
  T005 --> T007
  T006 --> T007
  T004 --> T007
  T005 --> T008
  T006 --> T008
  T004 --> T008
  T005 --> T009
  T006 --> T009
  T004 --> T009
  T005 --> T010
  T006 --> T010
  T004 --> T010
  T005 --> T011
  T006 --> T011
  T004 --> T011
  T005 --> T012
  T006 --> T012
  T007 --> T012
  T004 --> T012
  T011 --> T013
  T012 --> T013
  T004 --> T013
  T005 --> T014
  T006 --> T014
  T004 --> T014
  T007 --> T015
  T009 --> T015
  T012 --> T015
  T013 --> T015
  T014 --> T015
  T004 --> T015
  T054 --> T016
  T054 --> T017
  T054 --> T018
  T054 --> T019
  T054 --> T020
  T016 --> T021
  T017 --> T021
  T018 --> T021
  T020 --> T021
  T054 --> T021
  T016 --> T022
  T017 --> T022
  T018 --> T022
  T021 --> T022
  T054 --> T022
  T019 --> T023
  T021 --> T023
  T022 --> T023
  T054 --> T023
  T016 --> T024
  T017 --> T024
  T018 --> T024
  T022 --> T024
  T023 --> T024
  T054 --> T024
  T016 --> T025
  T017 --> T025
  T018 --> T025
  T019 --> T025
  T022 --> T025
  T023 --> T025
  T024 --> T025
  T054 --> T025
  T025 --> T026
  T052 --> T027
  T053 --> T027
  T025 --> T027
  T025 --> T028
  T025 --> T029
  T025 --> T030
  T027 --> T031
  T028 --> T031
  T030 --> T031
  T052 --> T031
  T053 --> T031
  T054 --> T031
  T025 --> T031
  T026 --> T032
  T029 --> T032
  T030 --> T032
  T031 --> T032
  T025 --> T032
  T029 --> T033
  T032 --> T033
  T025 --> T033
  T030 --> T034
  T032 --> T034
  T033 --> T034
  T055 --> T034
  T025 --> T034
  T026 --> T035
  T027 --> T035
  T028 --> T035
  T029 --> T035
  T031 --> T035
  T032 --> T035
  T033 --> T035
  T034 --> T035
  T052 --> T035
  T053 --> T035
  T054 --> T035
  T055 --> T035
  T025 --> T035
  T035 --> T036
  T035 --> T037
  T035 --> T038
  T035 --> T039
  T036 --> T040
  T039 --> T040
  T035 --> T040
  T036 --> T041
  T038 --> T041
  T039 --> T041
  T040 --> T041
  T035 --> T041
  T037 --> T042
  T039 --> T042
  T040 --> T042
  T041 --> T042
  T035 --> T042
  T036 --> T043
  T037 --> T043
  T038 --> T043
  T040 --> T043
  T041 --> T043
  T042 --> T043
  T035 --> T043
  T043 --> T044
  T055 --> T045
  T043 --> T045
  T043 --> T046
  T044 --> T047
  T043 --> T047
  T045 --> T048
  T047 --> T048
  T043 --> T048
  T034 --> T049
  T047 --> T049
  T043 --> T049
  T043 --> T050
  T047 --> T051
  T048 --> T051
  T049 --> T051
  T050 --> T051
  T043 --> T051
  T005 --> T052
  T006 --> T052
  T014 --> T052
  T004 --> T052
  T052 --> T053
  T004 --> T053
  T052 --> T054
  T053 --> T054
  T004 --> T054
  T029 --> T055
  T033 --> T055
  T025 --> T055
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Confirm the current `src/Layout` compile order, public `.fsi` boundaries, test projects, and sample entry points affected by automatic layout.
T002 [X] Add the pinned `Yoga.Net` `3.2.3` package reference to `src/Layout/Layout.fsproj` without exposing Yoga.Net types in public signatures.
T003 [X] Create readiness scaffolding under `specs/005-add-yoga-net-layout/readiness/` for FSI, logs, performance, sample smoke, and surface baselines.
T004 [X] Record Tier 1 evidence obligations, dependency pinning rationale, MVU applicability, and unsupported v1 scope in `specs/005-add-yoga-net-layout/readiness/evidence-obligations.md`.
T005 [X] Draft automatic layout geometry, sizing, visibility, measurement, diagnostics, result, and pixel snapping types in `src/Layout/Types.fsi`.
T006 [X] Draft default constructors and public evaluator functions in `src/Layout/Layout.fsi` or `src/Layout/YogaLayout.fsi`, including `evaluate`, `evaluateIncremental`, `renderComputed`, `snapBounds`, and `hitTestComputed`.
T007 [X] Add matching implementation placeholders and compile-order entries in `src/Layout/Types.fs`, `src/Layout/Layout.fs`, and any new Yoga layout implementation files.
T008 [X] Add semantic test helpers in `tests/Layout.Tests/Tests.fs` for reading computed bounds, asserting non-overlap, deterministic results, diagnostics, and visibility.
T009 [X] Add `scripts/layout-prelude.fsx` coverage for the intended public automatic layout API and expected readiness transcript path.
T010 [X] Add or update package surface baseline generation notes for `FS.Skia.UI.Layout` under `specs/005-add-yoga-net-layout/readiness/surface-baselines/`.
T011 [X] Define validation rules for duplicate node ids, invalid available space, invalid numeric style values, invalid measurement output, min/max conflicts, and v1 rejection of absolute or overlay intent.
T012 [X] Define the Yoga.Net adapter boundary so Yoga nodes are allocated, styled, measured, evaluated, read back, and disposed without leaking Yoga.Net types through `.fsi`.
T013 [X] Define the deterministic fallback geometry policy for recoverable failures, including bounded root and child rectangles plus `LayoutDiagnostic` records.
T014 [X] Define the invalidation model for node intent, visibility, child structure, parent size, and content measurement changes, including stable unchanged sibling bounds.
T015 [X] Build the foundation surface and capture an initial FSI or build transcript proving the public signatures are reachable from the package.
T016 [X] Add semantic tests for row, column, and wrap containers with non-overlapping child bounds inside the parent.
T017 [X] Add semantic tests for parent padding, child margin, row and column gaps, and main/cross-axis alignment.
T018 [X] Add semantic tests for fixed, min, max, flex grow, flex shrink, flex basis, and deterministic repeated evaluation.
T019 [X] Add public API tests for custom content measurement callbacks that return preferred logical sizes and diagnostics.
T020 [X] Implement default automatic layout records and helper constructors for layout intents, nodes, available space, and pixel snap policies.
T021 [X] Implement Yoga.Net style mapping for direction, wrap, alignment, justification, padding, margin, gap, fixed/min/max size, grow, shrink, and basis.
T022 [X] Implement recursive layout tree evaluation through Yoga.Net and read back one logical `ComputedBounds` entry per participating node.
T023 [X] Implement custom leaf measurement callback bridging between public `MeasureRequest` / `MeasureResponse` and Yoga.Net measure callbacks.
T024 [X] Implement deterministic result ordering, finite logical bounds normalization, and valid-layout non-overlap guarantees.
T025 [X] Capture US1 readiness evidence through `dotnet test --filter` or equivalent and the FSI prelude transcript for nested element layout.
T026 [X] Add semantic tests for mixed standard widgets and custom elements participating in one automatic layout tree.
T027 [X] Add resize and incremental evaluation tests proving changed parent size or child measurement updates bounds without stale overlap.
T028 [X] Add invalidation locality tests proving unaffected sibling subtrees keep byte-for-byte equivalent computed bounds after unrelated changes.
T029 [X] Add render and hit-test tests proving computed logical bounds are consumed without independent layout recalculation.
T030 [X] Add fixtures or adapters that let existing element/widget scene content attach to `LayoutNode.Content` while keeping automatic layout opt-in.
T031 [X] Implement `evaluateIncremental` revision handling, invalidated node reporting, and changed-node ancestor propagation.
T032 [X] Implement `renderComputed` so scene content is positioned from computed bounds while existing manual stack, dock, graph, absolute, and overlay composition keep working.
T033 [X] Implement `snapBounds` and `hitTestComputed` with one deterministic `PixelSnapPolicy` shared by rendering and hit testing.
T034 [X] Add or update `samples/LayoutGraphGallery` and `samples/DemoReel` automatic layout examples for nested elements, mixed widgets, resizing, flexible sizing, and hidden elements.
T035 [X] Capture US2 readiness evidence through widget/invalidation tests and sample smoke transcript under `readiness/sample-smoke/`.
T036 [X] Add semantic tests for invalid available space, invalid style values, min/max conflicts, and size requests larger than the parent.
T037 [X] Add semantic tests for unmeasurable content, invalid measurement callback output, hidden nodes, and collapsed nodes.
T038 [X] Add tests proving recoverable failures return structured diagnostics and bounded fallback geometry without terminating render flow.
T039 [X] Implement structured diagnostic creation with node id, code, severity, message, constraint, and fallback flags.
T040 [X] Implement validation and normalization for available space, layout intent values, measurement output, and unsupported automatic-layout scope.
T041 [X] Implement safe fallback bounds for recoverable constraints and propagate diagnostics through `LayoutResult.Diagnostics`.
T042 [X] Implement hidden and collapsed node behavior so visibility diagnostics are distinguishable from layout failures and visible siblings stay stable.
T043 [X] Capture US3 readiness evidence for invalid/conflicting layouts and diagnostic samples.
T044 [X] Refresh `FS.Skia.UI.Layout` surface-area baseline and package tests for the new public contract.
T045 [X] Update `specs/005-add-yoga-net-layout/quickstart.md` and layout interaction docs for final public names, pointer hit testing, keyboard focus regions, visual bounds, and pixel snapping behavior.
T046 [X] Add performance evidence for representative 200-node resize/re-layout under `readiness/performance/yoga-layout-200-node-resize.txt`, including command, hardware/runtime profile, iteration count, median/p95 timing, and pass/fail against the SC-004 threshold.
T047 [X] Run `dotnet restore`, `dotnet build`, and `dotnet test`, saving final logs under `readiness/logs/`.
T048 [X] Run the layout FSI prelude and save `readiness/fsi/yoga-layout-prelude.txt`.
T049 [X] Run automatic layout sample smoke checks and save `readiness/sample-smoke/automatic-layout-gallery.txt`.
T050 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/005-add-yoga-net-layout --graph-only` and confirm no dangling refs, cycles, or unexpected `[S*]` propagation.
T051 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/005-add-yoga-net-layout` and document PASS or every explicit synthetic-evidence override.
T052 [X] Draft the stateful host/sample layout workflow contract for resize, widget updates, and content-measurement invalidation, including `Model`, `Msg`, `Effect` or `Cmd<Msg>`, `init`, `update`, and interpreter boundary.
T053 [X] Add pure transition tests for the host/sample layout workflow covering resize, visibility changes, layout-intent changes, and content-measurement changes.
T054 [X] Add emitted-effect assertions and real interpreter evidence for the host/sample layout workflow where safe, saving transcript output under `readiness/fsi/`.
T055 [X] Document and test keyboard focus region alignment with computed visual bounds and pointer hit-test bounds after pixel snapping.
```

