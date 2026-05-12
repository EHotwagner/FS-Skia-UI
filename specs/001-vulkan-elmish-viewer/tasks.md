# Tasks: Vulkan Elmish Viewer

**Feature branch**: `001-vulkan-elmish-viewer`
**Spec**: `specs/001-vulkan-elmish-viewer/spec.md`
**Plan**: `specs/001-vulkan-elmish-viewer/plan.md`

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
- **[US1]**, **[US2]**, ... — user-story scope
- **[T1]** / **[T2]** — Tier 1 (contracted) vs Tier 2 (internal) change

Every task must have a matching entry in `tasks.deps.yml` even if its
dependency list is empty. The `speckit.evidence.graph` command refuses to
proceed with dangling references.

---

## Phase 1: Setup

- [X] T001 Confirm the current feature branch and prerequisites with `.specify/scripts/bash/check-prerequisites.sh --json --require-plan`
- [X] T002 [P] Create readiness scaffolding under `specs/001-vulkan-elmish-viewer/readiness/` for FSI transcripts, smoke logs, screenshots, package output notes, and Vulkan diagnostics
- [X] T003 [P] Update project restore/build metadata for `net10.0`, `LangVersion=latest`, package identity, and packable library defaults in `Directory.Build.props` and `src/Lib/Lib.fsproj`
- [X] T004 [P] Add pinned package references for SkiaSharp `4.147.0-preview.2.1`, Windows/Linux native assets, Fable.Elmish `4.2.0`, and Silk.NET Windowing/Input/Vulkan `2.23.0`
- [X] T005 Record feature Tier 1, public API impact, Elmish/MVU applicability, Vulkan-only scope, supported OS scope, and required real-evidence obligations in `specs/001-vulkan-elmish-viewer/readiness/evidence-obligations.md`

---

## Phase 2: Foundation

- [X] T006 Draft the public `.fsi` surface in `src/Lib/Library.fsi` for `Size`, `Color`, `ViewerConfiguration`, diagnostics, `ViewerEvent`, `Scene`, screenshot types, `ViewerEffect<'msg>`, `ViewerProgram<'model,'msg>`, `Scene`, and `Viewer` modules
- [X] T007 [P] Add internal implementation files and project ordering for scene data, diagnostics, Elmish program construction, Vulkan host/interpreter boundaries, and screenshot handling
- [X] T008 [P] Replace placeholder tests with contract-first Expecto suites for public surface construction, configuration validation, pure update behavior, emitted effects, and diagnostic values
- [X] T009 Add `scripts/prelude.fsx` coverage for constructing a configuration, scene, minimal program, subscription stub, and screenshot request through the public API
- [X] T010 Exercise the draft `.fsi` from FSI using `scripts/prelude.fsx` and capture the transcript to `specs/001-vulkan-elmish-viewer/readiness/fsi-session.txt`
- [X] T011 Record the initial public surface baseline for the library API in `specs/001-vulkan-elmish-viewer/readiness/public-surface.txt`
- [X] T012 Define unsupported-scope handling for macOS, mobile, browser, headless presentation, non-Vulkan renderer attempts, and non-Elmish integration attempts
- [X] T013 Add structured diagnostic constructors or helpers for platform checks, Vulkan instance/device/surface/swapchain failures, Skia context failures, frame errors, screenshot errors, and shutdown errors
- [X] T014 Run `dotnet restore`, `dotnet build`, and `dotnet test`; capture command summaries in `specs/001-vulkan-elmish-viewer/readiness/foundation-commands.txt`

**Checkpoint**: Foundation ready — story implementation may begin in parallel.

---

## Phase 3: User Story 1 (US1) - Run Vulkan-Only Viewer

### Tests First (Principle I, Principle VI)

- [X] T015 [P] [US1] Add semantic tests that create a minimal Elmish viewer program, assert `view` produces a scene, and assert render effects are requested without renderer selection
- [X] T016 [P] [US1] Add startup diagnostics tests that prove no OpenGL, CPU, software, or fallback renderer option is exposed by public configuration or program creation
- [X] T017 [P] [US1] Add a Vulkan-capable smoke-test script or documented command that records first-frame timing, renderer path, and absence of fallback usage to readiness logs

### Implementation

- [X] T018 [P] [US1] Implement scene primitives for empty scenes, groups, rectangles, text, images, charts, and stable composition data
- [X] T019 [US1] Implement `Viewer.create`, default subscription behavior, and program validation while keeping `init`, `update`, and `view` pure at the public boundary
- [X] T020 [US1] Implement the Vulkan-only host startup path with Silk.NET window creation, Vulkan instance/device/surface/swapchain setup, and Skia GPU context ownership
- [X] T021 [US1] Connect `RenderFrame` interpretation so model-derived scenes render into the active Vulkan/Skia frame without window recreation
- [X] T022 [US1] Add runtime diagnostics for frame render failures and prove errors are reported without switching renderer path
- [S] T023 [US1] Run the US1 Vulkan smoke path on Windows and Linux supported workstations, capture first-frame timing and renderer evidence for each OS, and store results in `specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke.txt`
- [X] T024 [US1] Document the US1 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`

**Checkpoint**: US1 is fully functional and testable independently.

---

## Phase 4: User Story 2 (US2) - Fail Clearly Without Vulkan

### Tests First

- [X] T025 [P] [US2] Add semantic tests for unsupported platform and unavailable Vulkan capability diagnostics before window display
- [X] T026 [P] [US2] Add tests that validate diagnostic messages identify Vulkan availability or initialization and never mention fallback rendering

### Implementation

- [S] T027 [P] [US2] Add test fixtures or interpreter seams that can simulate missing Vulkan instance, device, surface, and swapchain capabilities without invoking real GPU resources
- [S] T028 [US2] Implement fail-fast startup validation for unsupported OS, headless surface absence, Vulkan instance failure, device selection failure, surface failure, swapchain failure, and Skia context failure
- [X] T029 [US2] Ensure `Viewer.run` returns `Result<unit, RenderDiagnostic>` for startup failures before rendering begins
- [S] T030 [US2] Capture unsupported-environment command output or controlled fixture evidence in `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`
- [X] T031 [US2] Document unsupported environment diagnostics and supported OS/Vulkan requirements in `README.md` and `specs/001-vulkan-elmish-viewer/quickstart.md`

**Checkpoint**: US2 is fully functional and testable independently.

---

## Phase 5: User Story 3 (US3) - Drive UI With Elmish Flow

### Tests First

- [X] T032 [P] [US3] Add pure transition tests for application model updates from keyboard, pointer, resize, close, lifecycle, diagnostic, frame, screenshot, and subscription messages
- [X] T033 [P] [US3] Add emitted-effect assertions for initialize, render frame, capture screenshot, shutdown, diagnostic reporting, and dispatch effects
- [X] T034 [P] [US3] Add subscription tests that verify a timer-style subscription dispatches messages without direct mutable scene pushes

### Implementation

- [X] T035 [P] [US3] Implement viewer event mapping from Silk.NET keyboard, pointer, resize, close, lifecycle, and diagnostic callbacks to Elmish messages
- [X] T036 [US3] Implement command/effect interpretation for Elmish dispatch, subscriptions, render scheduling, screenshot requests, and shutdown disposal
- [X] T037 [US3] Add input-driven visible scene updates to `samples/InteractiveViewer/Program.fs`
- [X] T038 [US3] Add subscription-driven scene updates that run for at least 60 seconds in `samples/InteractiveViewer/Program.fs`
- [S] T039 [US3] Run the interactive sample smoke path and capture input latency plus 60-second subscription evidence in `specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`
- [X] T040 [US3] Document the US3 independent validation path in `specs/001-vulkan-elmish-viewer/quickstart.md`

**Checkpoint**: US3 is fully functional and testable independently.

---

## Phase 6: User Story 4 (US4) - Provide Complete Elmish Viewer Examples

### Tests First

- [X] T041 [P] [US4] Add compile and smoke-test coverage for `samples/BasicViewer` and `samples/InteractiveViewer` consuming only the packed public API
- [X] T042 [P] [US4] Add screenshot capture tests for successful capture after first frame and diagnostic capture before first successful frame
- [X] T043 [P] [US4] Add representative example verification for simple scene rendering, input handling, state update, layout or chart composition, and screenshot capture

### Implementation

- [X] T044 [P] [US4] Create `samples/BasicViewer` with a declarative scene containing shapes, text, image usage, layout composition, chart data, and screenshot command coverage
- [X] T045 [US4] Create `samples/InteractiveViewer` with Elmish model, messages, update, view, input handling, subscriptions, diagnostics, and screenshot capture
- [X] T046 [US4] Implement screenshot capture through the Vulkan/Skia frame path with PNG and JPEG output plus diagnostics for missing frame or write failure
- [X] T047 [US4] Add documentation for SkiaSharp 4 preview dependency behavior, Windows/Linux support, sample commands, package consumption, compatibility impact, and migration guidance stating this is a first-version package with no prior public API migration path
- [S] T048 [US4] Run both sample applications from documented commands on Windows and Linux where available, capture smoke logs plus any screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`, and disclose missing platform evidence as synthetic if one OS cannot be exercised

**Checkpoint**: US4 is fully functional and testable independently.

---

## Phase 7: Integration & Polish

- [X] T049 Refresh the Tier 1 public surface baseline and compare it against `contracts/public-api.md`
- [X] T050 Run `dotnet format` or the repository's formatting command and fix only feature-owned formatting issues
- [X] T051 Run `dotnet test` and capture final automated test output in `specs/001-vulkan-elmish-viewer/readiness/final-test.txt`
- [X] T052 Run `dotnet pack src/Lib/Lib.fsproj -c Release -o ~/.local/share/nuget-local/` and capture package output plus package artifact notes under `specs/001-vulkan-elmish-viewer/readiness/package/`
- [X] T053 Run the packed library through `scripts/prelude.fsx` or an FSI session and capture final consumer evidence
- [X] T054 Run documented sample commands from a clean checkout state or clean restore and capture final sample evidence
- [X] T055 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-vulkan-elmish-viewer --graph-only` and confirm no cycles, dangling refs, or orphaned tasks
- [S] T056 Run `.specify/extensions/evidence/scripts/bash/run-audit.sh specs/001-vulkan-elmish-viewer` and confirm PASS or document every `--accept-synthetic` override
- [X] T057 Update the Synthetic-Evidence Inventory if any Vulkan-capable smoke evidence cannot be collected on real supported hardware

---

## Synthetic-Evidence Inventory

List every `[S]` task here with its Principle V disclosures. This section is
the source for the PR description's synthetic-evidence section.

| Task | Reason | Real-evidence path | Tracking issue |
|------|--------|---------------------|----------------|
| T023 | Linux Vulkan smoke evidence is real, but this environment cannot exercise the required Windows workstation path. No code or test fixture is synthetic; the task status is synthetic because one required OS evidence leg is missing. | Run `scripts/us1-vulkan-smoke.sh specs/001-vulkan-elmish-viewer/readiness/us1-vulkan-smoke-windows.txt` on a Windows workstation with a Vulkan-capable GPU and driver. | https://github.com/EHotwagner/FS-Skia-UI/issues/1 |
| T027 | Missing Vulkan instance/device/surface/swapchain cases are covered by an explicitly named synthetic test fixture because this workstation has usable Vulkan and its driver/hardware state was not mutated. | Capture real or controlled unsupported-Vulkan startup diagnostics under `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`. | https://github.com/EHotwagner/FS-Skia-UI/issues/2 |
| T028 | Fail-fast startup validation is implemented in production code, but the missing Vulkan instance/device/surface/swapchain verification path depends on the synthetic fixture disclosed for T027. | Capture real or controlled unsupported-Vulkan startup diagnostics under `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt`. | https://github.com/EHotwagner/FS-Skia-UI/issues/2 |
| T030 | Readiness evidence captures the controlled synthetic fixture output rather than a real unsupported Vulkan workstation/container. | Replace `specs/001-vulkan-elmish-viewer/readiness/us2-vulkan-unavailable.txt` with real unsupported-environment command output. | https://github.com/EHotwagner/FS-Skia-UI/issues/2 |
| T039 | Readiness evidence uses the InteractiveViewer controlled CLI smoke path rather than a live Vulkan window with real keyboard/pointer interaction and 60 seconds of wall-clock timer subscription runtime. | Run `dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj` on a supported Vulkan workstation, interact with keyboard/pointer input, let the timer subscription run for at least 60 wall-clock seconds, and save the log/screenshot/transcript to `specs/001-vulkan-elmish-viewer/readiness/us3-interactive-smoke.txt`. | https://github.com/EHotwagner/FS-Skia-UI/issues/3 |
| T048 | Linux BasicViewer Vulkan smoke and screenshot artifacts are real, and both samples consume the packed public API in contract-smoke mode. This environment cannot exercise the required Windows live sample path or a live InteractiveViewer keyboard/pointer walkthrough. | Run both documented sample applications on Windows and Linux Vulkan workstations, capture live BasicViewer and InteractiveViewer logs, and add screenshot artifacts under `specs/001-vulkan-elmish-viewer/readiness/`. | https://github.com/EHotwagner/FS-Skia-UI/issues/4 |
| T056 | Full evidence audit records the `--accept-synthetic` justification in `readiness/synthetic-evidence.json`, but still exits NEEDS-EVIDENCE because declared synthetic tasks remain. | Resolve T023, T027, T028, T030, T039, and T048 with real supported-hardware evidence, then rerun the full audit without `--accept-synthetic`. | https://github.com/EHotwagner/FS-Skia-UI/issues/1, https://github.com/EHotwagner/FS-Skia-UI/issues/2, https://github.com/EHotwagner/FS-Skia-UI/issues/3, https://github.com/EHotwagner/FS-Skia-UI/issues/4 |
