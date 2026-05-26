# Implementation Plan: Persistent GUI Runtime

**Branch**: `018-persistent-gui-runtime` | **Date**: 2026-05-26 | **Spec**: `specs/018-persistent-gui-runtime/spec.md`
**Input**: Feature specification from `/specs/018-persistent-gui-runtime/spec.md`

## Summary

Split generated graphical game launch into a true interactive runtime and an explicit bounded evidence runtime. `Viewer.runApp` and the generated default executable path must keep the game window open until user/host close, while first-frame, input-dispatch, screenshot, and pixel-readback checks move behind explicit evidence commands and unambiguous outcome fields. Generated verification must also fail exact package-version drift, run generated tests, diagnose desktop-session prerequisites before app lifecycle debugging, and record readiness evidence that separates environment, package, verification-depth, and lifecycle failures.

This is a Tier 1 contracted runtime/governance change. It changes public SkiaViewer package contracts, generated product behavior, verification targets, template/guidance checks, package evidence, and readiness contracts. It does not add a new game engine, broaden control/chart/DataGrid scope, or require unrelated rendering migrations.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for package/runtime code, generated templates, governance tests, and FAKE targets  
**Primary Dependencies**: Existing SkiaSharp 4 preview stack, Silk.NET window/input integration, Expecto, FAKE, Spec Kit shell/evidence scripts; no new runtime package planned unless screenshot/pixel evidence exposes an existing missing package that must be justified during implementation  
**Testing**: Expecto semantic tests through `.fsi`, FAKE targets (`Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit`), FSI transcripts, generated product restore/test/evidence runs, supported-host visual evidence where available  
**Target Platform**: Windows and Linux generated graphical apps; Linux container readiness must distinguish Wayland/X11 runtime/display/socket/session-bus prerequisites from app defects  
**Public Surface**: `src/SkiaViewer/SkiaViewer.fsi` and surface baselines may change to expose launch mode, outcome fields, interactive/evidence separation, and visual evidence contracts. Generated template `Program.fs`/tests and documentation contracts also change.  
**Evidence Requirement**: Required real evidence paths are `specs/018-persistent-gui-runtime/readiness/interactive-lifecycle.md`, `evidence-launch-mode.md`, `container-session-diagnostics.md`, `package-resolution.md`, `generated-verify.md`, `game-visual-evidence.md`, `task-workflow-guidance.md`, and `evidence-audit.md`.  
**Synthetic Evidence**: Product lifecycle tests may use a fake/test window-loop only for the keep-open regression when no native desktop is available in CI. Such tests must be disclosed as synthetic and cannot replace supported-host screenshot or pixel-readback evidence. Synthetic error-handling prechecks may use design-approved `[SEH]` only when validating malformed metadata/error paths.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update generated product template sources under `template/base`, generated product tests, generated README/docs, and template validation expectations if launch flags, default path, package-source files, or game-example content change. `.template.config/template.json` must be reviewed if new files such as `NuGet.config`, readiness templates, or game profile assets are included.
- **Dependency impact**: Review `Directory.Packages.props`, `template/base/Directory.Packages.props`, package versions, `docs/dependencies.md`, local package feed guidance, and `DependencyReport`. Requested `FS.Skia.UI.*` versions must resolve exactly for generated verification; `NU1603` fallback is a failure.
- **Command-surface impact**: `Verify`, generated `Test`, package verification, generated guidance checks, `EvidenceGraph`, and `EvidenceAudit` must change or gain coverage. `Dev`, `Ci`, `PackLocal`, `TemplateCheck`, `DependencyReport`, and `TemplateDrift` may change only when they aggregate or validate the affected workflows.
- **Generated project impact**: Default generated game runs must call the interactive launch path. Evidence commands must be explicit. Generated tests must actually execute when present. Game examples must show board/grid, side information, keyboard updates, and time-based progression.
- **Evidence paths**: Required readiness paths are:
  - `specs/018-persistent-gui-runtime/readiness/interactive-lifecycle.md`
  - `specs/018-persistent-gui-runtime/readiness/evidence-launch-mode.md`
  - `specs/018-persistent-gui-runtime/readiness/container-session-diagnostics.md`
  - `specs/018-persistent-gui-runtime/readiness/package-resolution.md`
  - `specs/018-persistent-gui-runtime/readiness/generated-verify.md`
  - `specs/018-persistent-gui-runtime/readiness/game-visual-evidence.md`
  - `specs/018-persistent-gui-runtime/readiness/task-workflow-guidance.md`
  - `specs/018-persistent-gui-runtime/readiness/evidence-audit.md`
- **`.fsi` / contract impact**: `src/SkiaViewer/SkiaViewer.fsi`, public docs, surface baselines, generated app host contract, launch outcome contract, and compatibility notes must be reviewed. Public outcome fields should distinguish `interactive-window`, `persistent-evidence`, and bounded smoke/scene evidence.
- **MVU/effect boundary**: Launch lifecycle is stateful/I/O-bearing. Model states include not-started, starting, interactive-running, evidence-running, first-frame-presented, input-observed, user-close-observed, self-closed-for-evidence, failed, and unsupported. Messages include start interactive, start evidence, frame presented, input dispatched, user close, evidence target reached, timeout, diagnostic captured, and failure. Effects include open window, render, dispatch input, capture screenshot, read pixels, close for evidence, write evidence, and emit diagnostic. `update` must be pure; native window/display/package/file work stays in interpreters.
- **Synthetic evidence**: PASS with restrictions. CI may use disclosed fake window-loop fixtures for regression of "interactive does not self-close after first frame"; supported-host visual evidence or explicit unsupported-host diagnostics remain required. Package-resolution and generated-test evidence must be real command evidence.
- **Test evidence**: Add failing-first semantic tests for public launch contracts, interactive keep-open behavior, explicit evidence self-close behavior, outcome fields, desktop diagnostic validation, exact package resolution, generated test execution, placeholder evidence rejection, screenshot/pixel fallback selection, and readiness guidance checks.
- **Observability**: Diagnostics must name the failing class: environment/session, package-resolution, verification-depth, or app lifecycle. Desktop diagnostics must report runtime directory presence/ownership/permissions, display variable, display socket, session bus when relevant, fallback status, and unsupported-host reason.
- **Deferred scope**: No new game engine, no unrelated chart/control/DataGrid changes, no release automation, no marketplace distribution, and no non-game generated app changes beyond shared launch/verification contracts.

**Pre-design gate result**: PASS. The feature is Tier 1 and stateful, but the plan includes `.fsi` contract review, MVU/effect design, failing-first tests, real evidence paths, and explicit synthetic limitations.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                         # Public launch/outcome/evidence contract
  SkiaViewer.fs                          # Interactive/evidence interpreters and diagnostics

template/base/
  src/Product/Program.fs                 # Generated default interactive path and evidence flags
  tests/Product.Tests/                   # Generated tests must execute under generated Test/Verify
  Directory.Packages.props               # Requested framework versions
  NuGet.config                           # Included only if local package feeds are required

tests/
  SkiaViewer.Tests/                      # Semantic launch contract tests if package tests exist/are added
  Governance.Tests/                      # Generated guidance, verification, audit, task workflow checks

docs/
  build.md
  evidence.md
  generated-apps.md
  runtime-design.md

specs/018-persistent-gui-runtime/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    launch-runtime-contract.md
    generated-verification-contract.md
    readiness-evidence-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/018-persistent-gui-runtime/research.md`. Key decisions:

- `Viewer.runApp` remains the normal generated interactive API and must not self-close after first frame.
- Bounded launch/evidence behavior remains explicit through a separate evidence mode/API/flag.
- Launch outcomes use explicit mode and close/input fields instead of overloading `mode=persistent-window`.
- Desktop-session readiness runs before app lifecycle diagnosis and treats private runtime directories as diagnostic/evidence fallbacks, not full desktop sessions.
- Generated verification fails on exact package-version mismatch and `NU1603`, records package sources/resolved versions, and executes generated tests.
- Screenshot is the preferred visual game proof; pixel-readback is acceptable only when screenshot capture is unavailable but rendered pixels can still be inspected.
- Task workflow guidance supports implementation batches and red-green evidence logs without weakening graph/audit gates.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/018-persistent-gui-runtime/data-model.md`
- `specs/018-persistent-gui-runtime/contracts/launch-runtime-contract.md`
- `specs/018-persistent-gui-runtime/contracts/generated-verification-contract.md`
- `specs/018-persistent-gui-runtime/contracts/readiness-evidence-contract.md`
- `specs/018-persistent-gui-runtime/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public launch/outcome changes start in `SkiaViewer.fsi`, then semantic tests and generated product tests, then implementation.
- **Visibility in `.fsi`**: PASS. Public symbols must be declared in `.fsi`; any new public module requires matching signature and surface baseline updates.
- **Idiomatic simplicity**: PASS. Expected implementation uses records, discriminated unions, pure update functions, and edge interpreters. No complex F# features are planned.
- **MVU/effect boundary**: PASS. Lifecycle, diagnostics, evidence capture, and package verification states are modeled in `data-model.md`; interpreters own native/display/file/process effects.
- **Synthetic disclosure**: PASS with restrictions. Fake window-loop and synthetic error metadata checks must be disclosed and cannot substitute for real generated verification or visual evidence.
- **Test evidence**: PASS. The quickstart names failing-first tests and real commands for launch, generated verification, guidance, graph, audit, and visual evidence.
- **Observability and safe failure**: PASS. Contracts require actionable diagnostics and explicit failure classes before app debugging proceeds.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with `skillist` metadata, including required readiness files and acceptance keywords before implementation begins. Tasks that touch generated product runtime, package verification, visual evidence, or Spec Kit evidence/audit guidance must load the applicable local capability or Spec Kit skills before edits.
