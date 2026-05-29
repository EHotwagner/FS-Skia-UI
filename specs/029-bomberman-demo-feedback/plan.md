# Implementation Plan: Bomberman Demo Feedback Follow-ups

**Branch**: `029-bomberman-demo-feedback` | **Date**: 2026-05-29 | **Spec**: `specs/029-bomberman-demo-feedback/spec.md`
**Input**: Feature specification from `specs/029-bomberman-demo-feedback/spec.md`

## Summary

Address the Bomberman demo feedback by tightening generated-product evidence workflows, screenshot proof, generated game host wiring, and scene/layout authoring guidance. The approach is to update the active framework packages and generated template where needed, keep pure application transitions separate from viewer/filesystem/native work, and require real readiness evidence for every claimed fix. This is a contracted Tier 1-style change because it may add or refine public `.fsi` surfaces in `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Testing`, `FS.Skia.UI.Elmish`, `FS.Skia.UI.Scene`, or `FS.Skia.UI.Layout` and changes generated consumer behavior.

## Technical Context

**Language/Version**: F# on .NET `net10.0`
**Primary Dependencies**: Existing FS.Skia.UI packages; Elmish; SkiaSharp 4 preview packages; Silk.NET; Yoga.Net; Expecto; FAKE. No new dependency is planned.
**Testing**: Expecto package tests, governance tests, FAKE targets, FSI transcripts against packed/local packages, generated product evidence commands, and real readiness artifacts.
**Target Platform**: Windows and Linux. Browser, mobile, macOS expansion, new renderer backends, and release publishing are out of scope.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: `template/base/build.fsx`, `template/base/src/Product/EvidenceCommands.fs`, generated guidance fragments, `.template.config/template.json` inclusion, and generated profile guidance must be reviewed if helper files or command behavior change. If only source packages change, template drift evidence must document why the template does not change.
- **Dependency impact**: No new dependency is planned. If implementation discovers a required package, update `Directory.Packages.props`, `docs/dependencies.md`, generated package references, and `DependencyReport`.
- **Command-surface impact**: `Verify`, `TemplateCheck`, `GeneratedGuidanceCheck`, `EvidenceGraph`, and `EvidenceAudit` are in scope. `Dev` may be used for generated app persistent launch validation. `Ci` must continue to delegate to `Verify`. `PackLocal`, `DependencyReport`, and `TemplateDrift` change only if package surface, package contents, dependencies, or template files change.
- **Generated project impact**: Generated app source, tests, local readiness commands, local skills, validation logs, excluded-history scans, and generated `Dev` behavior are in scope. Evidence graph invocation must work from generated checkouts without executable-mode repair.
- **Evidence paths**: Required readiness paths are `specs/029-bomberman-demo-feedback/readiness/evidence-graph-invocation.md`, `specs/029-bomberman-demo-feedback/readiness/verify-log-cleanliness.md`, `specs/029-bomberman-demo-feedback/readiness/screenshot-evidence-probe.md`, `specs/029-bomberman-demo-feedback/readiness/generated-app-wiring.md`, and `specs/029-bomberman-demo-feedback/readiness/scene-layout-authoring.md`. Implementation should also refresh `readiness/task-graph.md`, `readiness/task-graph.json`, `readiness/evidence-audit.md`, and any package surface baselines touched by public API additions.
- **`.fsi` / contract impact**: Public helper additions or signature refinements must start in `.fsi` files before `.fs` bodies. Candidate surfaces are `src/SkiaViewer/SkiaViewer.fsi`, `src/Testing/Testing.fsi`, `src/Elmish/Elmish.fsi`, `src/Scene/Scene.fsi`, and `src/Layout/*.fsi`. Update package surface baselines and compatibility notes for every public addition.
- **MVU/effect boundary**: Generated game wiring must model pure app state as app-owned `Model`, `Msg`, `Effect` or app command values, `init`, `update`, `view`, key mapping, and tick mapping. Viewer launch, screenshot capture, file writing, process execution, native window work, and package validation stay at host/build interpreter edges.
- **Synthetic evidence**: Synthetic fixtures are allowed only for negative malformed-input/error-path tests and must use `[S]`, `[SEH]` where approved, code/test/spec disclosure, and audit inventory rows. Successful readiness proof cannot rely on synthetic screenshot, generated Verify, evidence graph, or app wiring artifacts.
- **Test evidence**: Add failing-first tests in the package or governance suite that owns the touched behavior. Expected suites include `tests/SkiaViewer.Tests`, `tests/Testing.Tests`, `tests/Elmish.Tests`, `tests/Scene.Tests`, `tests/Layout.Tests`, and `tests/Governance.Tests`, plus generated product validation through `TemplateCheck` or `GeneratedProductCheck`.
- **Observability**: Reports must include actionable status, command, output path, host facts, blocked stage, classification, category, fallback, capture probe detail, artifact validation, and diagnostics. Verification logs must be clean text and must not contain embedded NUL bytes.
- **Deferred scope**: New Bomberman gameplay, release publishing, package distribution, browser/mobile screenshot capture, renderer replacement, Vulkan redesign, charts, graph controls, DataGrid, and broad roadmap work are deferred out of this feature.

**Gate result before Phase 0**: PASS. The plan names public-surface, template, command, generated-product, MVU/effect, synthetic-evidence, and readiness obligations with no unresolved clarifications.

## Project Structure

### Feature Artifacts

```text
specs/029-bomberman-demo-feedback/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── evidence-workflows.md
│   ├── generated-app-wiring.md
│   └── scene-layout-authoring.md
└── readiness/
    ├── evidence-graph-invocation.md
    ├── verify-log-cleanliness.md
    ├── screenshot-evidence-probe.md
    ├── generated-app-wiring.md
    └── scene-layout-authoring.md
```

### Source And Test Touch Points

```text
src/SkiaViewer/SkiaViewer.fsi
src/SkiaViewer/SkiaViewer.fs
src/Testing/Testing.fsi
src/Testing/Testing.fs
src/Elmish/Elmish.fsi
src/Elmish/Elmish.fs
src/Scene/Scene.fsi
src/Scene/Scene.fs
src/Layout/*.fsi
src/Layout/*.fs
template/base/build.fsx
template/base/src/Product/EvidenceCommands.fs
template/fragments/*/README.md
tests/SkiaViewer.Tests/
tests/Testing.Tests/
tests/Elmish.Tests/
tests/Scene.Tests/
tests/Layout.Tests/
tests/Governance.Tests/
build.fsx
```

## Phase 0 Research

Research is captured in `specs/029-bomberman-demo-feedback/research.md`.

## Phase 1 Design

Design entities are captured in `specs/029-bomberman-demo-feedback/data-model.md`.

Contracts are captured in:

- `specs/029-bomberman-demo-feedback/contracts/evidence-workflows.md`
- `specs/029-bomberman-demo-feedback/contracts/generated-app-wiring.md`
- `specs/029-bomberman-demo-feedback/contracts/scene-layout-authoring.md`

Quickstart validation is captured in `specs/029-bomberman-demo-feedback/quickstart.md`.

## Constitution Check Post-Design

- **Spec -> FSI -> semantic tests -> implementation**: PASS. Contracts identify candidate `.fsi` surfaces, FSI transcript expectations, failing-first tests, and implementation order.
- **Visibility lives in `.fsi`**: PASS. Public additions must be declared only through corresponding `.fsi` files and surface baselines.
- **Idiomatic simplicity**: PASS. No custom operators, SRTP, reflection, type providers, or non-trivial computation expressions are planned.
- **MVU/effect boundary**: PASS. Generated app wiring and screenshot evidence both preserve pure update/result values and host-side interpreters.
- **Synthetic evidence disclosure**: PASS. Synthetic use is restricted to negative malformed-input/error-path fixtures and cannot satisfy successful readiness.
- **Test evidence mandatory**: PASS. Each user story has package/governance/generated-product tests plus named readiness evidence.
- **Observability and safe failure**: PASS. Evidence contracts require stable report fields, explicit unsupported/error classifications, and clean text logs.
