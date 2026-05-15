# Task Graph — 008-targeted-refactor-governance

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 59 |
| [S] synthetic | 0 |
| [S*] auto-synthetic | 0 |

## Graph

```mermaid
graph TD
  T001["T001 Create feature readiness scaffolding under `specs/"]:::done
  T002["T002 Inventory current public surface inputs in `specs/"]:::done
  T003["T003 Inventory current `src/Lib/Library.fs` responsibil"]:::done
  T004["T004 Inventory current `build.fsx` concern areas and FA"]:::done
  T005["T005 Record feature Tier, public-API impact, MVU/effect"]:::done
  T006["T006 Create a traceability matrix in `specs/008-targete"]:::done
  T007["T007 Add shared governance test helpers in `tests/Gover"]:::done
  T008["T008 Add failing public surface stability checks in `te"]:::done
  T009["T009 Add failing runtime organization checks proving an"]:::done
  T010["T010 Add failing deterministic native lifecycle test sc"]:::done
  T011["T011 Add failing build organization checks in `tests/Go"]:::done
  T012["T012 Record failing-first foundation output under `spec"]:::done
  T013["T013 Define the internal helper contract and compile-or"]:::done
  T014["T014 Add native startup stage and ownership fixture con"]:::done
  T015["T015 Add governance fixture directories and sample file"]:::done
  T016["T016 Run foundation verification for package surface ch"]:::done
  T017["T017 Add packed-library and FSI-facing surface tests pr"]:::done
  T018["T018 Add runtime organization tests or fixtures proving"]:::done
  T019["T019 Add semantic runtime tests for `ViewerProgram`, `V"]:::done
  T020["T020 Move scene-state and runtime diagnostic helpers fr"]:::done
  T021["T021 Move drawing, frame, screenshot, and host-adjacent"]:::done
  T022["T022 Update `src/Lib/Lib.fsproj` compile order and inte"]:::done
  T023["T023 Run `./fake.sh build -t PackageSurfaceCheck`, focu"]:::done
  T024["T024 Finalize `specs/008-targeted-refactor-governance/r"]:::done
  T025["T025 Add deterministic injected acquisition failure tes"]:::done
  T026["T026 Add tests for startup diagnostic stage names, orig"]:::done
  T027["T027 Add a native smoke evidence plan that runs existin"]:::done
  T028["T028 Add `src/Lib/VulkanResources.fsi` and `src/Lib/Vul"]:::done
  T029["T029 Add `src/Lib/VulkanStartup.fsi` and `src/Lib/Vulka"]:::done
  T030["T030 Refactor `VulkanHost.run` in `src/Lib/Library.fs` "]:::done
  T031["T031 Wire synthetic disclosure into native failure test"]:::done
  T032["T032 Run focused native startup cleanup tests and real "]:::done
  T033["T033 Update `specs/008-targeted-refactor-governance/rea"]:::done
  T034["T034 Extend `tests/Governance.Tests/GeneratedGuidanceTe"]:::done
  T035["T035 Extend `tests/Governance.Tests/TemplateDriftTests."]:::done
  T036["T036 Extend command-contract tests for physical `build."]:::done
  T037["T037 Replace substring-only `GeneratedGuidanceCheck` lo"]:::done
  T038["T038 Update `.specify/templates/spec-template.md`, `.sp"]:::done
  T039["T039 Refactor `scripts/template-drift.fsx` to classify "]:::done
  T040["T040 Attempt physical `build.fsx` organization by conce"]:::done
  T041["T041 Run `./fake.sh build -t GeneratedGuidanceCheck`, `"]:::done
  T042["T042 Update `docs/build.md`, `docs/testing.md`, `docs/e"]:::done
  T043["T043 Add `tests/Layout.Tests/YogaFallbackDiagnosticsTes"]:::done
  T044["T044 Add public record invariant inventory tests in `te"]:::done
  T045["T045 Add follow-up proposal validation tests requiring "]:::done
  T046["T046 Evaluate the existing `src/Layout/Types.fsi` diagn"]:::done
  T047["T047 Update `src/Layout/Layout.fs` fallback handling so"]:::done
  T048["T048 Write `specs/008-targeted-refactor-governance/read"]:::done
  T049["T049 Write `specs/008-targeted-refactor-governance/read"]:::done
  T050["T050 Run focused `tests/Layout.Tests`, `tests/Governanc"]:::done
  T051["T051 Run `./fake.sh build -t Dev` and store the log und"]:::done
  T052["T052 Run `./fake.sh build -t PackageSurfaceCheck` and, "]:::done
  T053["T053 Run focused `dotnet test` commands for `tests/Lib."]:::done
  T054["T054 Run `./fake.sh build -t GeneratedGuidanceCheck` an"]:::done
  T055["T055 Run `./fake.sh build -t Verify` and `./fake.sh bui"]:::done
  T056["T056 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T057["T057 Run `.specify/extensions/evidence/scripts/bash/run"]:::done
  T058["T058 Complete the Synthetic-Evidence Inventory, final r"]:::done
  T059["T059 Prepare the merge summary with command results, re"]:::done
  T006 --> T007
  T006 --> T008
  T006 --> T009
  T006 --> T010
  T006 --> T011
  T007 --> T012
  T008 --> T012
  T009 --> T012
  T010 --> T012
  T011 --> T012
  T006 --> T012
  T009 --> T013
  T006 --> T013
  T010 --> T014
  T013 --> T014
  T006 --> T014
  T007 --> T015
  T011 --> T015
  T006 --> T015
  T008 --> T016
  T009 --> T016
  T010 --> T016
  T011 --> T016
  T012 --> T016
  T013 --> T016
  T014 --> T016
  T015 --> T016
  T006 --> T016
  T016 --> T017
  T016 --> T018
  T016 --> T019
  T018 --> T020
  T016 --> T020
  T018 --> T021
  T016 --> T021
  T017 --> T022
  T019 --> T022
  T020 --> T022
  T021 --> T022
  T016 --> T022
  T017 --> T023
  T019 --> T023
  T020 --> T023
  T021 --> T023
  T022 --> T023
  T016 --> T023
  T018 --> T024
  T020 --> T024
  T021 --> T024
  T022 --> T024
  T023 --> T024
  T016 --> T024
  T024 --> T025
  T024 --> T026
  T024 --> T027
  T025 --> T028
  T026 --> T028
  T024 --> T028
  T025 --> T029
  T026 --> T029
  T028 --> T029
  T024 --> T029
  T025 --> T030
  T026 --> T030
  T028 --> T030
  T029 --> T030
  T024 --> T030
  T027 --> T031
  T028 --> T031
  T029 --> T031
  T030 --> T031
  T024 --> T031
  T025 --> T032
  T026 --> T032
  T027 --> T032
  T028 --> T032
  T029 --> T032
  T030 --> T032
  T031 --> T032
  T024 --> T032
  T028 --> T033
  T029 --> T033
  T030 --> T033
  T031 --> T033
  T032 --> T033
  T024 --> T033
  T033 --> T034
  T033 --> T035
  T033 --> T036
  T034 --> T037
  T033 --> T037
  T034 --> T038
  T037 --> T038
  T033 --> T038
  T035 --> T039
  T033 --> T039
  T036 --> T040
  T033 --> T040
  T034 --> T041
  T035 --> T041
  T036 --> T041
  T037 --> T041
  T038 --> T041
  T039 --> T041
  T040 --> T041
  T033 --> T041
  T037 --> T042
  T038 --> T042
  T039 --> T042
  T040 --> T042
  T041 --> T042
  T033 --> T042
  T042 --> T043
  T042 --> T044
  T042 --> T045
  T043 --> T046
  T045 --> T046
  T042 --> T046
  T043 --> T047
  T046 --> T047
  T042 --> T047
  T044 --> T048
  T042 --> T048
  T045 --> T049
  T046 --> T049
  T048 --> T049
  T042 --> T049
  T043 --> T050
  T044 --> T050
  T045 --> T050
  T046 --> T050
  T047 --> T050
  T048 --> T050
  T049 --> T050
  T042 --> T050
  T050 --> T051
  T050 --> T052
  T050 --> T053
  T050 --> T054
  T051 --> T055
  T052 --> T055
  T053 --> T055
  T054 --> T055
  T050 --> T055
  T050 --> T056
  T055 --> T057
  T056 --> T057
  T050 --> T057
  T041 --> T058
  T050 --> T058
  T055 --> T058
  T057 --> T058
  T052 --> T059
  T053 --> T059
  T054 --> T059
  T055 --> T059
  T057 --> T059
  T058 --> T059
  T050 --> T059
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Create feature readiness scaffolding under `specs/008-targeted-refactor-governance/readiness/` for public surface, semantic tests, native cleanup, native smoke, build organization, generated guidance, template drift, Yoga fallback diagnostics, record invariants, follow-ups, graph output, and audit output
T002 [X] Inventory current public surface inputs in `specs/008-targeted-refactor-governance/readiness/public-surface-inventory.md`, including `src/Lib/Library.fsi`, package surface baselines under `readiness/surface-baselines/`, samples, and package tests
T003 [X] Inventory current `src/Lib/Library.fs` responsibilities in `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md`, covering scene state, diagnostics, drawing, native resources, frame flow, screenshots, and viewer hosting
T004 [X] Inventory current `build.fsx` concern areas and FAKE target load requirements in `specs/008-targeted-refactor-governance/readiness/build-organization.md`
T005 [X] Record feature Tier, public-API impact, MVU/effect-boundary applicability, synthetic native evidence policy, unsupported scope, and required evidence obligations in `specs/008-targeted-refactor-governance/readiness/evidence-obligations.md`
T006 [X] Create a traceability matrix in `specs/008-targeted-refactor-governance/readiness/traceability.md` mapping FR/SC/contract targets to tests, implementation files, commands, and readiness artifacts
T007 [X] Add shared governance test helpers in `tests/Governance.Tests/TestSupport.fs` for Markdown section spans, path-class fixtures, same-diff evidence fixtures, readiness table parsing, and command output assertions
T008 [X] Add failing public surface stability checks in `tests/Package.Tests/SurfaceAreaTests.fs` proving `src/Lib/Library.fsi`, package baselines, helper-module exports, and any new helper `.fsi` files do not introduce unapproved package-visible public modules or members for this feature
T009 [X] Add failing runtime organization checks proving any accepted `src/Lib/Library.fs` split uses paired `.fsi` contracts or documented named fallback sections without top-level visibility modifiers in `.fs` files
T010 [X] Add failing deterministic native lifecycle test scaffolding in `tests/Lib.Tests/NativeStartupCleanupTests.fs` for owned resource categories, injected acquisition failures, release order, and synthetic disclosure
T011 [X] Add failing build organization checks in `tests/Governance.Tests/CommandContractTests.fs` for `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, emitted effects, edge interpreter behavior, and `Dev`/`Verify`/`Ci` load contracts
T012 [X] Record failing-first foundation output under `specs/008-targeted-refactor-governance/readiness/logs/` for the public surface, runtime organization, native lifecycle, and build organization checks
T013 [X] Define the internal helper contract and compile-order strategy in `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md`, including accepted `.fsi` file pairs or named-section fallback rules
T014 [X] Add native startup stage and ownership fixture contracts used by tests without exposing new public API from `src/Lib/Library.fsi`
T015 [X] Add governance fixture directories and sample files for generated guidance section failures, deferred-scope placement failures, template drift path classes, same-diff alignment evidence, and accepted deferral records
T016 [X] Run foundation verification for package surface checks, governance tests, native lifecycle scaffolding, and command-contract checks; store output under `specs/008-targeted-refactor-governance/readiness/logs/`
T017 [X] Add packed-library and FSI-facing surface tests proving `src/Lib/Library.fsi`, public modules, samples, and package baselines remain source-compatible after internal splitting
T018 [X] Add runtime organization tests or fixtures proving scene model, diagnostics, drawing, native resources, frame flow, screenshots, and viewer hosting are separated by files or named sections with recorded reviewer notes
T019 [X] Add semantic runtime tests for `ViewerProgram`, `ViewerEffect`, pure update behavior, emitted effects, and real interpreter or smoke evidence where safe
T020 [X] Move scene-state and runtime diagnostic helpers from `src/Lib/Library.fs` into paired helper files such as `src/Lib/SceneModel.fsi`/`.fs` and `src/Lib/RuntimeDiagnostics.fsi`/`.fs`, or record named-section fallback evidence
T021 [X] Move drawing, frame, screenshot, and host-adjacent helpers into paired helper files such as `src/Lib/VulkanFrame.fsi`/`.fs`, or record named-section fallback evidence when the split is not compile-stable
T022 [X] Update `src/Lib/Lib.fsproj` compile order and internal call sites so the public `Library.fs` facade and `src/Lib/Library.fsi` contract remain stable
T023 [X] Run `./fake.sh build -t PackageSurfaceCheck`, focused `tests/Lib.Tests`, and packed-library or FSI evidence; store outputs in `specs/008-targeted-refactor-governance/readiness/public-surface.txt` and `semantic-tests.txt`
T024 [X] Finalize `specs/008-targeted-refactor-governance/readiness/runtime-responsibility-map.md` with accepted split files or named-section fallback rationale and reviewer notes
T025 [X] Add deterministic injected acquisition failure tests in `tests/Lib.Tests/NativeStartupCleanupTests.fs` for Vulkan instance, surface, device/queues, swapchain/images, command pool/buffers, fences, staging buffers/memory, and Skia GPU resources
T026 [X] Add tests for startup diagnostic stage names, original native error preservation, reverse cleanup order, ownership transfer points, successful shutdown, and repeated cleanup idempotency
T027 [X] Add a native smoke evidence plan that runs existing real Vulkan smoke where supported and records unsupported-environment diagnostics separately from implementation defects
T028 [X] Add `src/Lib/VulkanResources.fsi` and `src/Lib/VulkanResources.fs` ownership helpers or equivalent scoped rules for owner, acquire stage, transfer point, release action, release order, and disposal state
T029 [X] Add `src/Lib/VulkanStartup.fsi` and `src/Lib/VulkanStartup.fs` named startup stages with `Result`-based failure propagation and acquisition abstraction for deterministic tests
T030 [X] Refactor `VulkanHost.run` in `src/Lib/Library.fs` to use the staged startup pipeline, unwind ownership in reverse order on failure, and transfer resources only after successful initialization
T031 [X] Wire synthetic disclosure into native failure test names and readiness evidence, and preserve real native smoke invocation where the current environment supports it
T032 [X] Run focused native startup cleanup tests and real native smoke or unsupported-environment smoke diagnostics; store outputs in `native-startup-cleanup-tests.txt` and `native-smoke.txt`
T033 [X] Update `specs/008-targeted-refactor-governance/readiness/native-startup-cleanup.md` with every startup stage, acquired resource, cleanup owner, failure diagnostic, release order, transfer point, and synthetic/real evidence status
T034 [X] Extend `tests/Governance.Tests/GeneratedGuidanceTests.fs` with failing fixtures for missing headings, missing prompts, prompts only in deferred scope, wrong-section prompts, and active/preset parity mismatches
T035 [X] Extend `tests/Governance.Tests/TemplateDriftTests.fs` with failing fixtures for template-owned path classes, required alignment classes, same-diff evidence, active spec/plan/readiness mentions, and accepted deferral schema fields
T036 [X] Extend command-contract tests for physical `build.fsx` split acceptance or named-section fallback, preserving `BuildModel`, `BuildMsg`, `BuildEffect`, pure `update`, emitted effects, interpreter boundaries, and `Dev`/`Verify`/`Ci` target semantics
T037 [X] Replace substring-only `GeneratedGuidanceCheck` logic with structured Markdown section parsing, scoped prompt validation, deferred-scope detection, and active/preset parity diagnostics that name path, section, prompt, and mismatch class
T038 [X] Update `.specify/templates/spec-template.md`, `.specify/presets/fsharp-opinionated/templates/spec-template.md`, `.specify/templates/plan-template.md`, and `.specify/presets/fsharp-opinionated/templates/plan-template.md` only where required by the semantic guidance contract
T039 [X] Refactor `scripts/template-drift.fsx` to classify changed template-owned paths, map path classes to required alignment classes, validate same-diff alignment files plus active feature evidence, and report accepted deferrals with required fields
T040 [X] Attempt physical `build.fsx` organization by concern; accept it only if `Dev`, `Verify`, and `Ci` load cross-platform, otherwise keep one canonical `build.fsx` with path model, effects, interpreter, validation, governance, guidance, and target graph sections
T041 [X] Run `./fake.sh build -t GeneratedGuidanceCheck`, `./fake.sh build -t TemplateDrift`, and Linux plus Windows `Dev`/`Verify`/`Ci` load checks where available; store outputs or unsupported-platform rationale in `generated-guidance.md`, `template-drift.md`, and `build-organization.md`
T042 [X] Update `docs/build.md`, `docs/testing.md`, `docs/evidence.md`, `docs/speckit.md`, README, or workflow docs only where final diagnostics, target semantics, or deferral boundaries changed
T043 [X] Add `tests/Layout.Tests/YogaFallbackDiagnosticsTests.fs` forcing recoverable Yoga execution failure and asserting safe fallback bounds plus an observable diagnostic through existing `LayoutDiagnostic` fields when sufficient
T044 [X] Add public record invariant inventory tests in `tests/Governance.Tests/PublicRecordInvariantTests.fs` that enumerate public records from `FS.Skia.UI`, `FS.Skia.UI.Layout`, and `FS.Skia.UI.Charts` and fail on missing inventory rows
T045 [X] Add follow-up proposal validation tests requiring Yoga public-surface blockers and helper-constructor or validation-first recommendations to appear in `specs/008-targeted-refactor-governance/readiness/follow-ups.md` with stable IDs
T046 [X] Evaluate the existing `src/Layout/Types.fsi` diagnostic surface and either implement Yoga fallback diagnostics using existing fields or record a follow-up API proposal without changing public signatures
T047 [X] Update `src/Layout/Layout.fs` fallback handling so recoverable Yoga execution failure keeps deterministic safe bounds and emits `FallbackBoundsApplied` diagnostic data through existing fields when surface-sufficient
T048 [X] Write `specs/008-targeted-refactor-governance/readiness/record-invariants.md` with package, record name, fields, invariant, construction stance, decision, rationale, and follow-up ID where needed for every public record
T049 [X] Write `specs/008-targeted-refactor-governance/readiness/follow-ups.md` for any Yoga diagnostic surface gap or public record helper/validation API recommendation, keeping all public API work out of this feature
T050 [X] Run focused `tests/Layout.Tests`, `tests/Governance.Tests`, and package surface checks for Yoga fallback diagnostics, record inventory completeness, follow-up validation, safe fallback bounds, and no `Library.fsi` change; store outputs in `yoga-fallback-diagnostics.txt`, `record-invariants.md`, and `follow-ups.md`
T051 [X] Run `./fake.sh build -t Dev` and store the log under `specs/008-targeted-refactor-governance/readiness/logs/`
T052 [X] Run `./fake.sh build -t PackageSurfaceCheck` and, only if intentionally refreshing unchanged baselines is required, `./fake.sh build -t RefreshSurfaceBaselines`; store public surface output in `specs/008-targeted-refactor-governance/readiness/public-surface.txt`
T053 [X] Run focused `dotnet test` commands for `tests/Lib.Tests`, `tests/Layout.Tests`, `tests/Governance.Tests`, and `tests/Package.Tests`; store semantic and diagnostic logs under `specs/008-targeted-refactor-governance/readiness/logs/`
T054 [X] Run `./fake.sh build -t GeneratedGuidanceCheck` and `./fake.sh build -t TemplateDrift`; confirm readiness reports name missing sections, prompts, path classes, alignment classes, and accepted deferrals
T055 [X] Run `./fake.sh build -t Verify` and `./fake.sh build -t Ci`; confirm `Ci` delegates to `Verify`, required evidence artifacts exist, and build organization acceptance or fallback evidence is recorded
T056 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance --graph-only` and confirm no cycles, dangling references, orphaned tasks, or unexpected propagated statuses
T057 [X] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/008-targeted-refactor-governance` and confirm PASS, or document every unresolved synthetic-evidence or diff-scan blocker
T058 [X] Complete the Synthetic-Evidence Inventory, final readiness review, and follow-up proposal cross-links so no synthetic-only evidence or public API recommendation is hidden
T059 [X] Prepare the merge summary with command results, readiness evidence paths, public surface verdict, native cleanup verdict, governance diagnostic verdict, Yoga/record invariant verdict, and deferred public API work
```

