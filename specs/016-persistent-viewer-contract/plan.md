# Implementation Plan: Persistent Viewer Contract

**Branch**: `016-persistent-viewer-contract` | **Date**: 2026-05-26 | **Spec**: `specs/016-persistent-viewer-contract/spec.md`
**Input**: Feature specification from `/specs/016-persistent-viewer-contract/spec.md`

## Summary

Add a first-class persistent graphical viewer contract to `FS.Skia.UI.SkiaViewer`, make generated viewer-backed graphical apps launch through that persistent host by default, and harden governance so bounded smoke, first-frame, frame-count, scene metadata, and unsupported-host diagnostics remain explicit evidence helpers rather than substitutes for interactive app readiness.

The implementation is a Tier 1 contracted change. It updates the public `.fsi` surface, SkiaViewer semantic tests, generated app template startup, generated guidance checks, generated product validation, evidence graph/audit expectations, documentation, surface baselines, and readiness artifacts. The core API shape will be validated through FSI and semantic tests before `.fs` implementation, following the constitution's Spec -> FSI -> semantic tests -> implementation order.

## Technical Context

**Language/Version**: F# on .NET `net10.0`
**Primary Dependencies**: Existing SkiaSharp 4 preview package pins, Elmish, Expecto, FAKE; no new runtime package dependency planned
**Testing**: Expecto, FAKE targets, FSI transcripts, generated product validation, supported-host persistent launch evidence
**Target Platform**: Windows and Linux desktop hosts for persistent viewer attempts; unsupported host environments must report diagnostics instead of passing readiness
**Public Surface**: `src/SkiaViewer/SkiaViewer.fsi` gains persistent scene/app launch, runtime capability, launch outcome/evidence fields, and generated app host outcome support
**Generated Product Surface**: `template/base/src/Product/Program.fs`, `template/fragments/skiaviewer/README.md`, product tests, and generated validation paths change so default execution is persistent and bounded evidence is flag-only
**Governance Surface**: `build.fsx`, `tests/Governance.Tests/*`, `docs/build.md`, `docs/evidence.md`, `docs/generated-apps.md`, Spec Kit task guidance, `GeneratedGuidanceCheck`, `GeneratedProductCheck`, `EvidenceGraph`, and `EvidenceAudit` must distinguish persistent graphical launch artifacts from bounded evidence
**Evidence Requirement**: Completion requires at least one supported-host persistent launch artifact under `specs/016-persistent-viewer-contract/readiness/`; unsupported-host diagnostics are supplemental only
**Synthetic Evidence**: Synthetic bounded or fixture evidence may be used for negative governance cases, but any task depending on it must be marked `[S]`/`[S*]` until replaced by real supported-host launch evidence

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: Update `template/base/src/Product/Program.fs`, `template/base/tests/Product.Tests/Tests.fs`, `template/fragments/skiaviewer/README.md`, and likely `.template.config/template.json` only if packaged template inclusion changes. No deferral is allowed for default generated graphical app launch behavior.
- **Dependency impact**: No new dependency is planned. `Directory.Packages.props`, `template/base/Directory.Packages.props`, `docs/dependencies.md`, and `DependencyReport` still need verification because public package behavior and generated consumer restore/pack validation change.
- **Command-surface impact**: `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, `EvidenceAudit`, `Dev`, `Verify`, and `Ci` may require updates. `TemplateCheck` must verify the generated default path uses the persistent host contract; bounded evidence remains explicit flag coverage. `PackLocal` is required before generated consumer compatibility evidence.
- **Generated project impact**: Default generated `app` and `governed` graphical profiles must include a persistent generated app host skeleton with `Model`, `Msg`, `init`, pure `update`, `view`, `mapKey`, optional `tick`, `viewerOptions`, and `generatedHost`. Bounded smoke and scene evidence commands stay available only behind explicit CLI flags.
- **Evidence paths**: Required readiness paths are:
  - `specs/016-persistent-viewer-contract/readiness/persistent-viewer-contract.md`
  - `specs/016-persistent-viewer-contract/readiness/generated-default-launch.md`
  - `specs/016-persistent-viewer-contract/readiness/bounded-evidence-separation.md`
  - `specs/016-persistent-viewer-contract/readiness/runtime-capability-diagnostics.md`
  - `specs/016-persistent-viewer-contract/readiness/generated-guidance-check.md`
  - `specs/016-persistent-viewer-contract/readiness/evidence-graph.md`
  - `specs/016-persistent-viewer-contract/readiness/evidence-audit.md`
  - `specs/016-persistent-viewer-contract/readiness/supported-host-persistent-launch.txt`
- **`.fsi` / contract impact**: `src/SkiaViewer/SkiaViewer.fsi` and `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` must change. Public docs and migration guidance must explain compatibility for bounded-only generated apps.
- **MVU/effect boundary**: Persistent generated app hosting is stateful and I/O-bearing. The public contract must expose or wrap `Model`, `Msg`, `ViewerEffect`, `init`, pure `update`, `view`, `mapKey`, `tick`, and an interpreter at the viewer edge. Tests must cover pure transition behavior and real interpreter outcomes where host support exists.
- **Synthetic evidence**: Negative governance fixtures for bounded-only substitution and unsupported hosts may be synthetic. They must be disclosed in task metadata, test names, fixture banners, and PR notes when used. The supported-host launch artifact cannot be synthetic.
- **Test evidence**: Add failing-first semantic tests for `Viewer.run`, `Viewer.runApp`, runtime capability classification, keyboard dispatch, persistent outcome fields, bounded API separation, generated default launch source, guidance rejection, generated product validation, and evidence audit rejection of bounded-only readiness.
- **Observability**: Launch outcomes must report `status`, `mode=persistent-window`, `command`, `renderer-mode`, `window-opened`, `input-dispatch`, `exit-path`, `blocked-stage`, `classification`, `category`, and `message`. Unsupported environment and missing product/package capability must be separate classifications.
- **Deferred scope**: This feature does not add new platform support, mobile/browser support, macOS support, release distribution automation, game mechanics, or visual redesign. Native implementation may diagnose unsupported environments, but readiness still requires one supported-host launch artifact before completion.

**Pre-design gate result**: PASS. The spec declares the public API, generated template, MVU, diagnostics, and evidence impacts. No `NEEDS CLARIFICATION` items remain after the 2026-05-26 clarification requiring supported-host launch evidence.

## Project Structure

```text
src/SkiaViewer/
  SkiaViewer.fsi                 # Persistent viewer and generated app host public surface
  SkiaViewer.fs                  # Interpreter-edge implementation and capability diagnostics

tests/SkiaViewer.Tests/
  Tests.fs                       # Failing-first semantic and FSI-surface tests

template/base/src/Product/
  Program.fs                     # Default persistent launch path plus explicit evidence flags

template/base/tests/Product.Tests/
  Tests.fs                       # Generated product source/behavior expectations

template/fragments/skiaviewer/
  README.md                      # Generated product viewer guidance

tests/Governance.Tests/
  *.fs                           # Guidance, command, generated product, and evidence gate tests

docs/
  build.md
  evidence.md
  generated-apps.md
  v3Design.md                    # Contract and migration guidance

readiness/surface-baselines/
  FS.Skia.UI.SkiaViewer.txt      # Updated public surface baseline

specs/016-persistent-viewer-contract/
  plan.md
  research.md
  data-model.md
  quickstart.md
  contracts/
  readiness/
```

## Phase 0: Research

Research is complete in `specs/016-persistent-viewer-contract/research.md`. Key decisions:

- Add `Viewer.run` and `Viewer.runApp` as the first-class persistent APIs.
- Preserve `Viewer.runBounded`, `Viewer.runUntilFirstFrame`, and `Viewer.runForFrames` as explicit evidence APIs.
- Add runtime capability detection and launch outcome reporting to distinguish missing product capability from unsupported host environments.
- Require generated graphical templates to launch persistent host by default.
- Require evidence audit to reject bounded-only substitution.

## Phase 1: Design and Contracts

Design artifacts produced:

- `specs/016-persistent-viewer-contract/data-model.md`
- `specs/016-persistent-viewer-contract/contracts/persistent-viewer-contract.md`
- `specs/016-persistent-viewer-contract/contracts/generated-app-host-contract.md`
- `specs/016-persistent-viewer-contract/contracts/evidence-contract.md`
- `specs/016-persistent-viewer-contract/quickstart.md`

### Post-Design Constitution Check

- **Spec -> FSI -> tests -> implementation**: PASS. Contract artifacts define the expected `.fsi` changes and semantic tests before implementation.
- **Visibility in `.fsi`**: PASS. Public surface changes are assigned to `src/SkiaViewer/SkiaViewer.fsi` and the SkiaViewer surface baseline.
- **Idiomatic simplicity**: PASS. The API uses records, discriminated unions, functions, and explicit effects. No new complex F# feature is required.
- **MVU/effect boundary**: PASS. The generated host contract defines `Init`, `Update`, `View`, `MapKey`, `Tick`, `ViewerEffect`, and viewer-edge interpretation.
- **Synthetic disclosure**: PASS with constraint. Negative fixtures may be synthetic, but supported-host persistent launch evidence is mandatory and cannot be synthetic.
- **Test evidence**: PASS. The plan names failing-first semantic, governance, generated product, and supported-host launch evidence.
- **Observability and safe failure**: PASS. Launch and capability outcomes include blocked stage, classification, category, command, and user-facing message.

## Phase 2: Planning Boundary

Stop after design. Task generation should convert this plan into dependency-ordered tasks with `skillist` metadata. At minimum, implementation tasks must load `src/SkiaViewer/skill/SKILL.md` for SkiaViewer work, plus `src/Elmish/skill/SKILL.md` and `src/KeyboardInput/skill/SKILL.md` when updating generated host wiring and keyboard dispatch.
