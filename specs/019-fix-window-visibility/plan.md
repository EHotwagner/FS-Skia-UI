# Implementation Plan: Fix Window Visibility

**Branch**: `019-fix-window-visibility` | **Date**: 2026-05-27 | **Spec**: `specs/019-fix-window-visibility/spec.md`
**Input**: Feature specification from `/specs/019-fix-window-visibility/spec.md`

## Summary

Make generated graphical game launches prove and preserve visible, usable desktop windows. Normal interactive runs must stay open until a real user/app/host close, classify close reasons accurately, and fail or degrade when only a process/taskbar entry exists without an accessible window surface. Evidence runs remain explicit and must produce real image artifacts when image evidence is requested, with metadata/hash outputs labeled as metadata. Window behavior requests for resize, maximize, startup state, startup position, and backend preference become part of the public launch contract and generated validation.

This is a Tier 1 contracted runtime/governance change. It affects public SkiaViewer signatures, generated product templates, launch outcomes, visual evidence contracts, verification targets, guidance checks, and readiness evidence. It does not add a new game engine, alter game mechanics, broaden chart/control/DataGrid scope, or require release/distribution automation.

## Technical Context

**Language/Version**: F# on .NET `net10.0` for runtime/package code, generated templates, governance tests, and FAKE targets  
**Primary Dependencies**: Existing SkiaSharp 4 preview stack, Silk.NET window/input integration, Expecto, FAKE, Spec Kit shell/evidence scripts; no new runtime package planned unless native image capture or window-state inspection exposes a justified missing capability during implementation  
**Testing**: Expecto semantic tests through `.fsi`, FAKE targets (`Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `DependencyReport`, `EvidenceGraph`, `EvidenceAudit`), FSI transcripts, generated product restore/test/evidence runs, supported-host visual/window evidence where available  
**Target Platform**: Windows and Linux generated graphical apps; Linux container and desktop diagnostics must distinguish display/session/window-manager/compositor limitations from app lifecycle defects  
**Public Surface**: `src/SkiaViewer/SkiaViewer.fsi` and surface baselines may change to expose window behavior requests, launch modes, close reasons, visibility diagnostics, image evidence contracts, and explicit evidence-vs-interactive outcomes. Generated template `Program.fs`/tests and documentation contracts also change.  
**Evidence Requirement**: Required real evidence paths are `specs/019-fix-window-visibility/readiness/interactive-visible-window.md`, `close-reason-separation.md`, `window-state-diagnostics.md`, `window-options.md`, `real-image-evidence.md`, `generated-validation.md`, and `evidence-audit.md`.  
**Synthetic Evidence**: Product lifecycle tests may use a fake/test window-loop only for unreachable native states such as unmapped, off-screen, minimized-only, or hidden-window classification when no reliable host fixture exists. Such tests must be disclosed as synthetic and cannot replace supported-host visible-window or image evidence. Synthetic error-handling prechecks may use design-approved `[SEH]` only when validating malformed metadata/error paths.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update generated product template sources under `template/base`, generated product tests, generated README/docs, and template validation expectations if launch flags, default window behavior, evidence artifacts, or window option files change. `.template.config/template.json` must be reviewed if new files such as evidence scripts, assets, or option defaults are included.
- **Dependency impact**: Review `Directory.Packages.props`, `template/base/Directory.Packages.props`, package versions, `docs/dependencies.md`, generated package source guidance, and `DependencyReport`. No new dependency is planned; any native screenshot/window-inspection dependency requires explicit version pinning and plan update.
- **Command-surface impact**: `Verify`, generated `Test`, `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`, `DependencyReport`, `EvidenceGraph`, and `EvidenceAudit` must change or gain coverage. `Dev`, `Ci`, `PackLocal`, and `TemplateDrift` may change only when they aggregate or validate affected workflows.
- **Generated project impact**: Default generated game runs must call the interactive visible-window path. Evidence/image commands must be explicit. Generated tests must execute and cover persistence, close reasons, window diagnostics, window options, and real image evidence claims.
- **Evidence paths**: Required readiness paths are:
  - `specs/019-fix-window-visibility/readiness/interactive-visible-window.md`
  - `specs/019-fix-window-visibility/readiness/close-reason-separation.md`
  - `specs/019-fix-window-visibility/readiness/window-state-diagnostics.md`
  - `specs/019-fix-window-visibility/readiness/window-options.md`
  - `specs/019-fix-window-visibility/readiness/real-image-evidence.md`
  - `specs/019-fix-window-visibility/readiness/generated-validation.md`
  - `specs/019-fix-window-visibility/readiness/evidence-audit.md`
- **`.fsi` / contract impact**: `src/SkiaViewer/SkiaViewer.fsi`, public docs, surface baselines, generated app host contract, launch outcome contract, close-reason contract, window behavior contract, and visual evidence compatibility notes must be reviewed.
- **MVU/effect boundary**: Launch visibility is stateful/I/O-bearing. Model states include not-started, checking-session, starting-window, window-created, visibility-checking, interactive-running, evidence-running, first-frame-presented, close-requested, user-close-observed, app-close-observed, evidence-close-observed, inaccessible-window, failed, and unsupported. Messages include start interactive, start evidence, window created, visibility observed, focus observed, resize/state observed, frame presented, input observed, close requested, diagnostic captured, image captured, timeout, and failure. Effects include open window, apply window options, query native window state, render, dispatch input, capture image, read pixels, close for evidence, write evidence, and emit diagnostics. `update` must be pure; native window/display/package/file work stays in interpreters.
- **Synthetic evidence**: PASS with restrictions. Fake window-loop fixtures may classify hidden/off-screen/minimized states and close-reason transitions where native host fixtures are unreliable. Supported-host visible-window and actual image evidence remain required for readiness unless the host is explicitly unsupported.
- **Test evidence**: Add failing-first semantic tests for public window behavior contracts, interactive visible persistence, close-reason separation, taskbar-only/inaccessible-window classification, window-state diagnostics, option honoring/fallback diagnostics, actual image artifact validation, generated test execution, placeholder evidence rejection, and readiness guidance checks.
- **Observability**: Diagnostics must name the failing class: environment/session, window-visibility, window-options, visual-evidence, package/verification, or app lifecycle. Window diagnostics must report created/visible/focusable/focused/minimized/maximized/size/surface/input/backend facts when observable, and must state when a fact is unsupported by the host.
- **Deferred scope**: No new game engine, no generated game mechanic changes, no unrelated chart/control/DataGrid changes, no release automation, no marketplace distribution, and no guarantees for unsupported desktop sessions beyond clear diagnostics and fallback evidence.

**Pre-design gate result**: PASS. The feature is Tier 1 and stateful, but the plan includes `.fsi` contract review, MVU/effect design, failing-first tests, real readiness evidence, explicit synthetic limitations, and actionable observability.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                         # Public launch/window/outcome/evidence contract
  SkiaViewer.fs                          # Interactive/evidence interpreters and diagnostics

template/base/
  src/Product/Program.fs                 # Generated default visible interactive path and evidence flags
  tests/Product.Tests/                   # Generated tests must execute under generated Test/Verify
  Directory.Packages.props               # Requested framework versions

tests/
  SkiaViewer.Tests/                      # Semantic launch/window contract tests if package tests exist/are added
  Governance.Tests/                      # Generated guidance, validation, audit, task workflow checks

docs/
  build.md
  evidence.md
  generated-apps.md
  runtime-design.md

specs/019-fix-window-visibility/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
    launch-visibility-contract.md
    generated-validation-contract.md
    readiness-evidence-contract.md
  readiness/
```

## Phase 0: Research

Research is complete in `specs/019-fix-window-visibility/research.md`. Key decisions:

- Interactive success requires an accessible visible/focusable window, not merely a running process or first frame.
- Close reasons are explicit enum-like outcomes and `user-close-observed` is true only after a real user close event.
- Native window diagnostics are best-effort but must distinguish observed false values from unsupported/unobservable values.
- Window behavior requests are public options with explicit honored/degraded diagnostics.
- Real image evidence must be a decodable image file; hashes and scene metadata stay useful but are labeled separately.
- Desktop visibility evidence and scene-rendering evidence are separate claims.
- Generated validation fails misleading or incomplete package, test, launch, diagnostic, and visual evidence claims.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/019-fix-window-visibility/data-model.md`
- `specs/019-fix-window-visibility/contracts/launch-visibility-contract.md`
- `specs/019-fix-window-visibility/contracts/generated-validation-contract.md`
- `specs/019-fix-window-visibility/contracts/readiness-evidence-contract.md`
- `specs/019-fix-window-visibility/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Public window behavior, close reason, diagnostics, and visual evidence changes start in `SkiaViewer.fsi`, then semantic/generated tests, then implementation.
- **Visibility in `.fsi`**: PASS. Public symbols must be declared in `.fsi`; any new public module requires matching signature and surface baseline updates.
- **Idiomatic simplicity**: PASS. Expected implementation uses records, discriminated unions, pure update functions, and edge interpreters. No complex F# features are planned.
- **MVU/effect boundary**: PASS. Visibility, diagnostics, window options, image evidence, and close reasons are modeled in `data-model.md`; interpreters own native/display/file/process effects.
- **Synthetic disclosure**: PASS with restrictions. Fake window-loop and synthetic hidden-window fixtures must be disclosed and cannot substitute for supported-host visible-window or real image evidence.
- **Test evidence**: PASS. The quickstart names failing-first tests and real commands for launch, generated validation, guidance, graph, audit, and visual evidence.
- **Observability and safe failure**: PASS. Contracts require actionable diagnostics and explicit failure classes before app lifecycle debugging proceeds.

## Phase 2: Planning Boundary

Stop after design. Task generation should produce dependency-ordered tasks with `skillist` metadata, required readiness files, failing-first test obligations, and acceptance keywords before implementation begins. Tasks that touch generated product runtime, window visibility, visual evidence, package verification, or Spec Kit evidence/audit guidance must load the applicable local capability or Spec Kit skills before edits.
