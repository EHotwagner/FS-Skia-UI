# Task Graph — 001-vulkan-elmish-viewer

## ✓ Graph is acyclic and consistent

## Status counts (effective)

| Status | Count |
|--------|-------|
| [X] done | 22 |
| [S] synthetic | 7 |
| [S*] auto-synthetic | 28 |

## Graph

```mermaid
graph TD
  T001["T001 Confirm the current feature branch and prerequisit"]:::done
  T002["T002 Create readiness scaffolding under `specs/001-vulk"]:::done
  T003["T003 Update project restore/build metadata for `net10.0"]:::done
  T004["T004 Add pinned package references for SkiaSharp `4.147"]:::done
  T005["T005 Record feature Tier 1, public API impact, Elmish/M"]:::done
  T006["T006 Draft the public `.fsi` surface in `src/Lib/Librar"]:::done
  T007["T007 Add internal implementation files and project orde"]:::done
  T008["T008 Replace placeholder tests with contract-first Expe"]:::done
  T009["T009 Add `scripts/prelude.fsx` coverage for constructin"]:::done
  T010["T010 Exercise the draft `.fsi` from FSI using `scripts/"]:::done
  T011["T011 Record the initial public surface baseline for the"]:::done
  T012["T012 Define unsupported-scope handling for macOS, mobil"]:::done
  T013["T013 Add structured diagnostic constructors or helpers "]:::done
  T014["T014 Run `dotnet restore`, `dotnet build`, and `dotnet "]:::done
  T015["T015 Add semantic tests that create a minimal Elmish vi"]:::done
  T016["T016 Add startup diagnostics tests that prove no OpenGL"]:::done
  T017["T017 Add a Vulkan-capable smoke-test script or document"]:::done
  T018["T018 Implement scene primitives for empty scenes, group"]:::done
  T019["T019 Implement `Viewer.create`, default subscription be"]:::done
  T020["T020 Implement the Vulkan-only host startup path with S"]:::done
  T021["T021 Connect `RenderFrame` interpretation so model-deri"]:::done
  T022["T022 Add runtime diagnostics for frame render failures "]:::done
  T023["T023 Run the US1 Vulkan smoke path on Windows and Linux"]:::synthetic
  T024["T024 Document the US1 independent validation path in `s"]:::autoSynthetic
  T025["T025 Add semantic tests for unsupported platform and un"]:::autoSynthetic
  T026["T026 Add tests that validate diagnostic messages identi"]:::autoSynthetic
  T027["T027 Add test fixtures or interpreter seams that can si"]:::synthetic
  T028["T028 Implement fail-fast startup validation for unsuppo"]:::synthetic
  T029["T029 Ensure `Viewer.run` returns `Result<unit, RenderDi"]:::autoSynthetic
  T030["T030 Capture unsupported-environment command output or "]:::synthetic
  T031["T031 Document unsupported environment diagnostics and s"]:::autoSynthetic
  T032["T032 Add pure transition tests for application model up"]:::autoSynthetic
  T033["T033 Add emitted-effect assertions for initialize, rend"]:::autoSynthetic
  T034["T034 Add subscription tests that verify a timer-style s"]:::autoSynthetic
  T035["T035 Implement viewer event mapping from Silk.NET keybo"]:::autoSynthetic
  T036["T036 Implement command/effect interpretation for Elmish"]:::autoSynthetic
  T037["T037 Add input-driven visible scene updates to `samples"]:::autoSynthetic
  T038["T038 Add subscription-driven scene updates that run for"]:::autoSynthetic
  T039["T039 Run the interactive sample smoke path and capture "]:::synthetic
  T040["T040 Document the US3 independent validation path in `s"]:::autoSynthetic
  T041["T041 Add compile and smoke-test coverage for `samples/B"]:::autoSynthetic
  T042["T042 Add screenshot capture tests for successful captur"]:::autoSynthetic
  T043["T043 Add representative example verification for simple"]:::autoSynthetic
  T044["T044 Create `samples/BasicViewer` with a declarative sc"]:::autoSynthetic
  T045["T045 Create `samples/InteractiveViewer` with Elmish mod"]:::autoSynthetic
  T046["T046 Implement screenshot capture through the Vulkan/Sk"]:::autoSynthetic
  T047["T047 Add documentation for SkiaSharp 4 preview dependen"]:::autoSynthetic
  T048["T048 Run both sample applications from documented comma"]:::synthetic
  T049["T049 Refresh the Tier 1 public surface baseline and com"]:::autoSynthetic
  T050["T050 Run `dotnet format` or the repository's formatting"]:::autoSynthetic
  T051["T051 Run `dotnet test` and capture final automated test"]:::autoSynthetic
  T052["T052 Run `dotnet pack src/Lib/Lib.fsproj -c Release -o "]:::autoSynthetic
  T053["T053 Run the packed library through `scripts/prelude.fs"]:::autoSynthetic
  T054["T054 Run documented sample commands from a clean checko"]:::autoSynthetic
  T055["T055 Run `.specify/extensions/evidence/scripts/bash/run"]:::autoSynthetic
  T056["T056 Run `.specify/extensions/evidence/scripts/bash/run"]:::synthetic
  T057["T057 Update the Synthetic-Evidence Inventory if any Vul"]:::autoSynthetic
  T005 --> T006
  T005 --> T007
  T005 --> T008
  T006 --> T009
  T005 --> T009
  T006 --> T010
  T009 --> T010
  T005 --> T010
  T006 --> T011
  T005 --> T011
  T005 --> T012
  T012 --> T013
  T005 --> T013
  T003 --> T014
  T004 --> T014
  T006 --> T014
  T007 --> T014
  T008 --> T014
  T009 --> T014
  T013 --> T014
  T005 --> T014
  T014 --> T015
  T014 --> T016
  T014 --> T017
  T014 --> T018
  T015 --> T019
  T018 --> T019
  T014 --> T019
  T004 --> T020
  T013 --> T020
  T016 --> T020
  T017 --> T020
  T014 --> T020
  T019 --> T021
  T020 --> T021
  T014 --> T021
  T013 --> T022
  T021 --> T022
  T014 --> T022
  T017 --> T023
  T021 --> T023
  T022 --> T023
  T014 --> T023
  T023 --> T024
  T014 --> T024
  T024 --> T025
  T024 --> T026
  T024 --> T027
  T013 --> T028
  T025 --> T028
  T026 --> T028
  T027 --> T028
  T024 --> T028
  T028 --> T029
  T024 --> T029
  T029 --> T030
  T024 --> T030
  T030 --> T031
  T024 --> T031
  T031 --> T032
  T031 --> T033
  T031 --> T034
  T032 --> T035
  T031 --> T035
  T033 --> T036
  T034 --> T036
  T035 --> T036
  T031 --> T036
  T035 --> T037
  T036 --> T037
  T031 --> T037
  T034 --> T038
  T036 --> T038
  T031 --> T038
  T037 --> T039
  T038 --> T039
  T031 --> T039
  T039 --> T040
  T031 --> T040
  T040 --> T041
  T040 --> T042
  T040 --> T043
  T041 --> T044
  T043 --> T044
  T040 --> T044
  T041 --> T045
  T043 --> T045
  T040 --> T045
  T042 --> T046
  T040 --> T046
  T044 --> T047
  T045 --> T047
  T046 --> T047
  T040 --> T047
  T044 --> T048
  T045 --> T048
  T046 --> T048
  T040 --> T048
  T053 --> T049
  T048 --> T049
  T048 --> T050
  T050 --> T051
  T048 --> T051
  T051 --> T052
  T048 --> T052
  T052 --> T053
  T048 --> T053
  T048 --> T054
  T052 --> T054
  T048 --> T055
  T055 --> T056
  T048 --> T056
  T023 --> T057
  T030 --> T057
  T039 --> T057
  T048 --> T057
  T056 --> T057
  classDef pending fill:#eeeeee,stroke:#999
  classDef done fill:#c8e6c9,stroke:#2e7d32
  classDef synthetic fill:#ffe0b2,stroke:#e65100,stroke-width:2px
  classDef autoSynthetic fill:#ffab91,stroke:#bf360c,stroke-width:2px,stroke-dasharray:5 3
  classDef failed fill:#ffcdd2,stroke:#b71c1c,stroke-width:2px
  classDef skipped fill:#f5f5f5,stroke:#666,stroke-dasharray:3 3
```

## ASCII view

```
T001 [X] Confirm the current feature branch and prerequisites with `.specify/scripts/bash/check-prerequisites.sh --json --require-plan`
T002 [X] Create readiness scaffolding under `specs/001-vulkan-elmish-viewer/readiness/` for FSI transcripts, smoke logs, screenshots, package output notes, and Vulkan diagnostics
T003 [X] Update project restore/build metadata for `net10.0`, `LangVersion=latest`, package identity, and packable library defaults in `Directory.Build.props` and `src/Lib/Lib.fsproj`
T004 [X] Add pinned package references for SkiaSharp `4.147.0-preview.2.1`, Windows/Linux native assets, Fable.Elmish `4.2.0`, and Silk.NET Windowing/Input/Vulkan `2.23.0`
T005 [X] Record feature Tier 1, public API impact, Elmish/MVU applicability, Vulkan-only scope, supported OS scope, and required real-evidence obligations in `specs/001-vulkan-elmish-viewer/readiness/evidence-obligations.md`
T006 [X] Draft the public `.fsi` surface in `src/Lib/Library.fsi` for `Size`, `Color`, `ViewerConfiguration`, diagnostics, `ViewerEvent`, `Scene`, screenshot types, `ViewerEffect<'msg>`, `ViewerProgram<'model,'msg>`, `Scene`, and `Viewer` modules
T007 [X] Add internal implementation files and project ordering for scene data, diagnostics, Elmish program construction, Vulkan host/interpreter boundaries, and screenshot handling
T008 [X] Replace placeholder tests with contract-first Expecto suites for public surface construction, configuration validation, pure update behavior, emitted effects, and diagnostic values
T009 [X] Add `scripts/prelude.fsx` coverage for constructing a configuration, scene, minimal program, subscription stub, and screenshot request through the public API
T010 [X] Exercise the draft `.fsi` from FSI using `scripts/prelude.fsx` and capture the transcript to `specs/001-vulkan-elmish-viewer/readiness/fsi-session.txt`
T011 [X] Record the initial public surface baseline for the library API in `specs/001-vulkan-elmish-viewer/readiness/public-surface.txt`
T012 [X] Define unsupported-scope handling for macOS, mobile, browser, headless presentation, non-Vulkan renderer attempts, and non-Elmish integration attempts
T013 [X] Add structured diagnostic constructors or helpers for platform checks, Vulkan instance/device/surface/swapchain failures, Skia context failures, frame errors, screenshot errors, and shutdown errors
T014 [X] Run `dotnet restore`, `dotnet build`, and `dotnet test`; capture command summaries in `specs/001-vulkan-elmish-viewer/readiness/foundation-commands.txt`
T015 [X] Add semantic tests that create a minimal Elmish viewer program, assert `view` produces a scene, and assert render effects are requested without renderer selection
T016 [X] Add startup diagnostics tests that prove no OpenGL, CPU, software, or fallback renderer option is exposed by public configuration or program creation
T017 [X] Add a Vulkan-capable smoke-test script or documented command that records first-frame timing, renderer path, and absence of fallback usage to readiness logs
T018 [X] Implement scene primitives for empty scenes, groups, rectangles, text, images, charts, and stable composition data
T019 [X] Implement `Viewer.create`, default subscription behavior, and program validation while keeping `init`, `update`, and `view` pure at the public boundary
T020 [X] Implement the Vulkan-only host startup path with Silk.NET window creation, Vulkan instance/device/surface/swapchain setup, and Skia GPU context ownership
T021 [X] Connect `RenderFrame` interpretation so model-derived scenes render into the active Vulkan/Skia frame without window recreation
T022 [X] Add runtime diagnostics for frame render failures and prove errors are reported without switching renderer path
T023 [S] Run the US1 Vulkan smoke path on Windows and Linux supported workstations, capture first-frame timing and renderer evidence for each OS, and store results in `specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt`   ← root cause
T024 [S*] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`   ← auto-synthetic
    └── T023 [S] Run the US1 Vulkan smoke path on Windows and Linux supported workstations, capture first-frame timing and renderer evidence for each OS, and store results in `specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt`
T025 [S*] Add semantic tests for unsupported platform and unavailable Vulkan capability diagnostics before window display   ← auto-synthetic
    └── T024 [S*] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
T026 [S*] Add tests that validate diagnostic messages identify Vulkan availability or initialization and never mention fallback rendering   ← auto-synthetic
    └── T024 [S*] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
T027 [S] Add test fixtures or interpreter seams that can simulate missing Vulkan instance, device, surface, and swapchain capabilities without invoking real GPU resources   ← root cause
T028 [S] Implement fail-fast startup validation for unsupported OS, headless surface absence, Vulkan instance failure, device selection failure, surface failure, swapchain failure, and Skia context failure   ← root cause
T029 [S*] Ensure `Viewer.run` returns `Result<unit, RenderDiagnostic>` for startup failures before rendering begins   ← auto-synthetic
    └── T024 [S*] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T028 [S] Implement fail-fast startup validation for unsupported OS, headless surface absence, Vulkan instance failure, device selection failure, surface failure, swapchain failure, and Skia context failure
T030 [S] Capture unsupported-environment command output or controlled fixture evidence in `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`   ← root cause
T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`   ← auto-synthetic
    └── T024 [S*] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T030 [S] Capture unsupported-environment command output or controlled fixture evidence in `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`
T032 [S*] Add pure transition tests for application model updates from keyboard, pointer, resize, close, lifecycle, diagnostic, frame, screenshot, and subscription messages   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
T033 [S*] Add emitted-effect assertions for initialize, render frame, capture screenshot, shutdown, diagnostic reporting, and dispatch effects   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
T034 [S*] Add subscription tests that verify a timer-style subscription dispatches messages without direct mutable scene pushes   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
T035 [S*] Implement viewer event mapping from Silk.NET keyboard, pointer, resize, close, lifecycle, and diagnostic callbacks to Elmish messages   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T032 [S*] Add pure transition tests for application model updates from keyboard, pointer, resize, close, lifecycle, diagnostic, frame, screenshot, and subscription messages
T036 [S*] Implement command/effect interpretation for Elmish dispatch, subscriptions, render scheduling, screenshot requests, and shutdown disposal   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T033 [S*] Add emitted-effect assertions for initialize, render frame, capture screenshot, shutdown, diagnostic reporting, and dispatch effects
    └── T034 [S*] Add subscription tests that verify a timer-style subscription dispatches messages without direct mutable scene pushes
    └── T035 [S*] Implement viewer event mapping from Silk.NET keyboard, pointer, resize, close, lifecycle, and diagnostic callbacks to Elmish messages
T037 [S*] Add input-driven visible scene updates to `samples/InteractiveViewer/Program.fs`   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T035 [S*] Implement viewer event mapping from Silk.NET keyboard, pointer, resize, close, lifecycle, and diagnostic callbacks to Elmish messages
    └── T036 [S*] Implement command/effect interpretation for Elmish dispatch, subscriptions, render scheduling, screenshot requests, and shutdown disposal
T038 [S*] Add subscription-driven scene updates that run for at least 60 seconds in `samples/InteractiveViewer/Program.fs`   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T034 [S*] Add subscription tests that verify a timer-style subscription dispatches messages without direct mutable scene pushes
    └── T036 [S*] Implement command/effect interpretation for Elmish dispatch, subscriptions, render scheduling, screenshot requests, and shutdown disposal
T039 [S] Run the interactive sample smoke path and capture input latency plus 60-second subscription evidence in `specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`   ← root cause
T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`   ← auto-synthetic
    └── T031 [S*] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T039 [S] Run the interactive sample smoke path and capture input latency plus 60-second subscription evidence in `specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`
T041 [S*] Add compile and smoke-test coverage for `samples/BasicViewer` and `samples/InteractiveViewer` consuming only the packed public API   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
T042 [S*] Add screenshot capture tests for successful capture after first frame and diagnostic capture before first successful frame   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
T043 [S*] Add representative example verification for simple scene rendering, input handling, state update, layout or chart composition, and screenshot capture   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
T044 [S*] Create `samples/BasicViewer` with a declarative scene containing shapes, text, image usage, layout composition, chart data, and screenshot command coverage   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T041 [S*] Add compile and smoke-test coverage for `samples/BasicViewer` and `samples/InteractiveViewer` consuming only the packed public API
    └── T043 [S*] Add representative example verification for simple scene rendering, input handling, state update, layout or chart composition, and screenshot capture
T045 [S*] Create `samples/InteractiveViewer` with Elmish model, messages, update, view, input handling, subscriptions, diagnostics, and screenshot capture   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T041 [S*] Add compile and smoke-test coverage for `samples/BasicViewer` and `samples/InteractiveViewer` consuming only the packed public API
    └── T043 [S*] Add representative example verification for simple scene rendering, input handling, state update, layout or chart composition, and screenshot capture
T046 [S*] Implement screenshot capture through the Vulkan/Skia frame path with PNG and JPEG output plus diagnostics for missing frame or write failure   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T042 [S*] Add screenshot capture tests for successful capture after first frame and diagnostic capture before first successful frame
T047 [S*] Add documentation for SkiaSharp 4 preview dependency behavior, Windows/Linux support, sample commands, package consumption, compatibility impact, and migration guidance stating this is a first-version package with no prior public API migration path   ← auto-synthetic
    └── T040 [S*] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`
    └── T044 [S*] Create `samples/BasicViewer` with a declarative scene containing shapes, text, image usage, layout composition, chart data, and screenshot command coverage
    └── T045 [S*] Create `samples/InteractiveViewer` with Elmish model, messages, update, view, input handling, subscriptions, diagnostics, and screenshot capture
    └── T046 [S*] Implement screenshot capture through the Vulkan/Skia frame path with PNG and JPEG output plus diagnostics for missing frame or write failure
T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised   ← root cause
T049 [S*] Refresh the Tier 1 public surface baseline and compare it against `contracts/public-api.md`   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T053 [S*] Run the packed library through `scripts/prelude.fsx` or an FSI session and capture final consumer evidence
T050 [S*] Run `dotnet format` or the repository's formatting command and fix only feature-owned formatting issues   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
T051 [S*] Run `dotnet test` and capture final automated test output in `specs/001-vulkan-elmish-viewer/readiness/final-test.txt`   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T050 [S*] Run `dotnet format` or the repository's formatting command and fix only feature-owned formatting issues
T052 [S*] Run `dotnet pack src/Lib/Lib.fsproj -c Release -o ~/.local/share/nuget-local/` and capture package output plus package artifact notes under `specs/001-vulkan-elmish-viewer/readiness/package/`   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T051 [S*] Run `dotnet test` and capture final automated test output in `specs/001-vulkan-elmish-viewer/readiness/final-test.txt`
T053 [S*] Run the packed library through `scripts/prelude.fsx` or an FSI session and capture final consumer evidence   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T052 [S*] Run `dotnet pack src/Lib/Lib.fsproj -c Release -o ~/.local/share/nuget-local/` and capture package output plus package artifact notes under `specs/001-vulkan-elmish-viewer/readiness/package/`
T054 [S*] Run documented sample commands from a clean checkout state or clean restore and capture final sample evidence   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T052 [S*] Run `dotnet pack src/Lib/Lib.fsproj -c Release -o ~/.local/share/nuget-local/` and capture package output plus package artifact notes under `specs/001-vulkan-elmish-viewer/readiness/package/`
T055 [S*] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-vulkan-elmish-viewer --graph-only` and confirm no cycles, dangling refs, or orphaned tasks   ← auto-synthetic
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
T056 [S] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-vulkan-elmish-viewer` and confirm PASS or document every `--accept-synthetic` override   ← root cause
T057 [S*] Update the Synthetic-Evidence Inventory if any Vulkan-capable smoke evidence cannot be collected on real supported hardware   ← auto-synthetic
    └── T023 [S] Run the US1 Vulkan smoke path on Windows and Linux supported workstations, capture first-frame timing and renderer evidence for each OS, and store results in `specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt`
    └── T030 [S] Capture unsupported-environment command output or controlled fixture evidence in `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`
    └── T039 [S] Run the interactive sample smoke path and capture input latency plus 60-second subscription evidence in `specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`
    └── T048 [S] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised
    └── T056 [S] Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-vulkan-elmish-viewer` and confirm PASS or document every `--accept-synthetic` override
```

## Propagation report

The following tasks are marked `[S*]` because at least one of their dependencies is synthetic-only. Clearing the upstream `[S]` tasks (real evidence) will automatically clear these.

- **T024** ([S*]) ← T023
- **T025** ([S*]) ← T024
- **T026** ([S*]) ← T024
- **T029** ([S*]) ← T024, T028
- **T031** ([S*]) ← T024, T030
- **T032** ([S*]) ← T031
- **T033** ([S*]) ← T031
- **T034** ([S*]) ← T031
- **T035** ([S*]) ← T031, T032
- **T036** ([S*]) ← T031, T033, T034, T035
- **T037** ([S*]) ← T031, T035, T036
- **T038** ([S*]) ← T031, T034, T036
- **T040** ([S*]) ← T031, T039
- **T041** ([S*]) ← T040
- **T042** ([S*]) ← T040
- **T043** ([S*]) ← T040
- **T044** ([S*]) ← T040, T041, T043
- **T045** ([S*]) ← T040, T041, T043
- **T046** ([S*]) ← T040, T042
- **T047** ([S*]) ← T040, T044, T045, T046
- **T049** ([S*]) ← T048, T053
- **T050** ([S*]) ← T048
- **T051** ([S*]) ← T048, T050
- **T052** ([S*]) ← T048, T051
- **T053** ([S*]) ← T048, T052
- **T054** ([S*]) ← T048, T052
- **T055** ([S*]) ← T048
- **T057** ([S*]) ← T023, T030, T039, T048, T056

