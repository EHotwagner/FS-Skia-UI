# Implementation Plan: Tetris Demo Integration Improvements

**Branch**: `013-tetris-demo-integration` | **Date**: 2026-05-18 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/013-tetris-demo-integration/spec.md`

## Summary

Close the integration gaps found while running a generated Tetris graphical
consumer against real FS.Skia.UI packages: stable viewer keyboard input,
bounded first-frame smoke evidence, categorized diagnostics, deterministic
scene-level visual evidence, generated template input-flow validation, and
local package/feed guidance.

The implementation is a Tier 1 contracted change across viewer, keyboard,
testing, template, build, guidance, and readiness surfaces. Public F# contracts
must be drafted in `.fsi` first, validated through semantic/FSI tests, then
implemented. Real viewer startup evidence and deterministic headless scene
evidence are both required and must remain separate.

## Technical Context

**Language/Version**: F# on .NET `net10.0`; SDK-style projects; FAKE
`build.fsx`; generated products consume local NuGet packages rather than
repository implementation source.
**Primary Dependencies**: Existing FSharp.Core, Expecto, FAKE, SkiaSharp 4
preview packages, Silk.NET/Vulkan viewer dependencies already owned by the
repository, and BCL filesystem/process/time APIs. No new third-party dependency
is planned for this feature.
**Storage**: Filesystem evidence only: readiness reports, captured diagnostics,
generated consumer guidance output, local package inventory, smoke run
evidence, scene visual evidence metadata, evidence graph, and audit output
under `specs/013-tetris-demo-integration/readiness/`.
**Testing**: Expecto semantic tests, packed-library/FSI transcripts, viewer
input tests, SkiaViewer smoke tests, Testing package evidence tests, generated
template validation tests, governance command-contract tests, FAKE target
evidence, evidence graph, and evidence audit.
**Target Platform**: Windows and Linux developer/CI environments. Real bounded
viewer smoke evidence requires a supported desktop/window/Vulkan host and must
return unsupported-environment diagnostics when unavailable. Headless
scene-level evidence must not open a native window.
**Project Type**: Governed F# framework/template repository with public
packages, generated product templates, samples, tests, local package feed
workflow, Spec Kit assets, and readiness evidence.
**Performance Goals**: Bounded first-frame smoke exits without external
timeout after first-frame evidence or a structured pre-frame failure. Generated
consumer validation from fresh local package output to first-frame or visual
evidence completes within 10 minutes on a supported local development machine.
**Constraints**: Preserve current package identities. Do not change
game-specific Tetris rules, replace the renderer backend, guarantee native
window support on every CI host, publish remote packages, migrate external apps
automatically, or move Controls/DataGrid/Charts ownership. Public surfaces
remain governed by `.fsi` and surface baselines.
**Scale/Scope**: Public viewer input, diagnostics, bounded smoke, scene
evidence, optional app-host convenience, generated template/tests/guidance,
build targets, local consumer package reporting, docs, and readiness evidence.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

### Repository Governance Decisions

- **Template ownership**: PASS. Generated graphical templates, selected
  capability guidance, generated tests, generated quickstarts, template drift
  evidence, and `.template.config/template.json` inclusion must be updated if
  new files or fragments are added. Controls package ownership remains on the
  active Controls path; legacy Charts migration is deferred.
- **Dependency impact**: PASS. No new third-party dependency is planned. If
  implementation discovers a need for a new renderer, image, logging, or YAML
  dependency, planning must be updated first with
  `Directory.Packages.props`, `docs/dependencies.md`, dependency-report
  evidence, version pinning, and maintenance ownership.
- **Command-surface impact**: PASS. `Verify`, `Ci`, `PackLocal`,
  `GeneratedProductCheck`, `GeneratedGuidanceCheck`, `TemplateCheck`,
  `TemplateDrift`, `EvidenceGraph`, and `EvidenceAudit` may change.
  `DependencyReport` may change only for package inventory guidance. `Dev`
  should change only if needed for the interactive generated app workflow.
- **Generated project impact**: PASS. Generated graphical apps must include
  documented user-reachable input flows, viewer-key-driven tests, interactive
  run guidance, bounded smoke/visual evidence guidance, and local consumer
  package setup snippets. Generated consumers must continue consuming public
  packages rather than copying framework source.
- **Evidence paths**: PASS. Required readiness files are listed in Project
  Structure. Real viewer startup evidence, deterministic scene-level evidence,
  generated input-flow evidence, package guidance, generated consumer
  validation, evidence graph, and evidence audit are all mandatory.
- **`.fsi` / contract impact**: PASS. Public signatures and surface baselines
  are expected for normalized viewer input, diagnostics, bounded smoke, scene
  evidence helpers, and optional app-host convenience APIs. Draft `.fsi`
  contracts must precede `.fs` implementation and semantic tests must exercise
  packed/public surfaces.
- **MVU/effect boundary**: PASS. Viewer lifecycle, bounded run behavior,
  generated app host workflow, diagnostic capture, and local package reporting
  are stateful or I/O-bearing. Public or internal workflow surfaces must expose
  or wrap `Model`, `Msg`, `Effect`/`Cmd<Msg>`, `init`, pure `update`, and edge
  interpreters where the workflow owns state or I/O.
- **Synthetic evidence**: PASS. Synthetic fixtures may be used for forced
  pre-frame failure tests, unsupported-environment classification, scanner
  fixtures, and deterministic non-window scene examples, but final readiness
  cannot rely on synthetic-only proof. Any `[S]` task must carry the
  constitution disclosures.
- **Test evidence**: PASS. Failing-first tests are required for raw-key
  normalization, viewer-event conversion, generated start/options/restart
  flows, bounded first-frame success/failure, diagnostic category filtering and
  capture, headless scene evidence success/unsupported results, local package
  drift classification, and generated consumer validation.
- **Observability**: PASS. Failures must name the affected app flow, input
  value, screen, viewer/rendering stage, diagnostic category, package identity,
  package version, feed path, renderer mode, evidence path, or unsupported host
  capability.
- **Deferred scope**: PASS. Renderer replacement, Tetris game-rule changes,
  full visual redesign, guaranteed CI desktop support, remote package
  publishing, automatic external migration, and Charts package migration are
  out of scope.

### Constitution Gate Result

PASS. All planning unknowns are resolved in [research.md](./research.md). The
feature is treated as a contracted public/API and generated-template change
with explicit `.fsi`, semantic-test, readiness, and audit obligations.

## Project Structure

### Documentation (this feature)

```text
specs/013-tetris-demo-integration/
|-- spec.md
|-- plan.md
|-- research.md
|-- data-model.md
|-- quickstart.md
|-- contracts/
|   |-- normalized-viewer-input.md
|   |-- bounded-viewer-smoke.md
|   |-- diagnostics.md
|   |-- headless-scene-evidence.md
|   |-- generated-template-input-flows.md
|   |-- local-consumer-packages.md
|   `-- generated-consumer-validation.md
|-- checklists/
|   `-- requirements.md
`-- readiness/
    |-- normalized-viewer-input.md
    |-- bounded-viewer-smoke.md
    |-- diagnostics.md
    |-- headless-scene-evidence.md
    |-- generated-template-input-flows.md
    |-- local-consumer-packages.md
    |-- generated-consumer-validation.md
    |-- evidence-graph.md
    |-- evidence-audit.md
    `-- logs/
```

### Source Code (repository root)

```text
build.fsx
Directory.Packages.props
docs/
|-- build.md
|-- dependencies.md
|-- evidence.md
`-- generated-apps.md

scripts/
|-- dependency-report.fsx
`-- template-drift.fsx

src/
|-- KeyboardInput/
|-- SkiaViewer/
|-- Scene/
|-- Testing/
|-- Elmish/
`-- Lib/

tests/
|-- KeyboardInput.Tests/
|-- SkiaViewer.Tests/
|-- Scene.Tests/
|-- Testing.Tests/
|-- Governance.Tests/
|-- Smoke.Tests/
`-- Package.Tests/

template/
|-- base/
`-- fragments/
```

## Phase 0: Research

Research decisions are recorded in [research.md](./research.md). All planning
unknowns from the Technical Context are resolved.

## Phase 1: Design And Contracts

Design artifacts are recorded in:

- [data-model.md](./data-model.md)
- [contracts/normalized-viewer-input.md](./contracts/normalized-viewer-input.md)
- [contracts/bounded-viewer-smoke.md](./contracts/bounded-viewer-smoke.md)
- [contracts/diagnostics.md](./contracts/diagnostics.md)
- [contracts/headless-scene-evidence.md](./contracts/headless-scene-evidence.md)
- [contracts/generated-template-input-flows.md](./contracts/generated-template-input-flows.md)
- [contracts/local-consumer-packages.md](./contracts/local-consumer-packages.md)
- [contracts/generated-consumer-validation.md](./contracts/generated-consumer-validation.md)
- [quickstart.md](./quickstart.md)

### Post-Design Constitution Check

PASS. The design preserves public-surface governance through `.fsi` contracts
and baselines, keeps viewer/app workflows MVU-shaped where state or I/O is
owned, separates real viewer startup from headless scene evidence, and names
all readiness paths needed before implementation can be marked complete.
