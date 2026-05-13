# Implementation Plan: Skia Feature Parity

**Branch**: `002-skia-feature-parity` | **Date**: 2026-05-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/002-skia-feature-parity/spec.md`

## Summary

Expand FS-Skia-UI from the current Elmish-only Vulkan viewer into a parity-complete Skia UI library suite matching the observable capabilities of `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc`, while preserving this project's constraints: Vulkan-only rendering, Elmish-only state flow, Windows/Linux desktop support, and automated-first evidence.

The implementation is organized as three independently referenceable packages:

- `FS.Skia.UI`: core Elmish viewer, scene DSL, Skia feature coverage, screenshots, diagnostics, lifecycle, and Vulkan host.
- `FS.Skia.UI.Charts`: pure view-layer chart and DataGrid components that return scene elements.
- `FS.Skia.UI.Layout`: pure view-layer layout and graph components that return scene elements.

Charts, DataGrid, layout, and graph capabilities are view components. Application/domain state remains in the Elmish `Model`; pure projection helpers convert that model to component props; components return declarative scene elements from the `view` function.

## Technical Context

**Language/Version**: F# on .NET `net10.0` with `LangVersion=latest`  
**Primary Dependencies**: SkiaSharp `4.147.0-preview.2.1`; SkiaSharp native assets for Windows/Linux `4.147.0-preview.2.1`; Fable.Elmish `4.2.0`; Silk.NET.Windowing/Input/Vulkan `2.23.0`; Expecto `10.2.2`; Microsoft.NET.Test.Sdk `17.11.1`; YoloDev.Expecto.TestSdk `0.15.3`  
**Storage**: Filesystem only for screenshots, package artifacts, surface-area baselines, parity evidence reports, and generated test images  
**Testing**: `dotnet test`, FSI/prelude contract transcripts, surface-area baseline tests, semantic MVU tests, rendering/screenshot comparison tests, package restore/pack tests, sample smoke tests, Vulkan-unavailable diagnostics tests, parity evidence audit  
**Target Platform**: Windows and Linux desktop only  
**Project Type**: Packable F# library suite plus runnable desktop sample applications  
**Performance Goals**: Simple viewer first frame under 2 seconds on Vulkan-capable workstation in at least 95% of smoke runs; input response under 1 second in at least 95% of smoke runs; 100-node DAG layout within 2 seconds; 50-node undirected weighted graph visible; chart tests cover 100,000-point datasets; DataGrid tests cover 10,000 rows  
**Constraints**: Vulkan-only; Elmish-only public integration; no fallback renderer; all public modules require `.fsi`; visibility controlled by `.fsi`; automated-first evidence; manual visual review only for non-deterministic graphics differences; baseline pinned to `EHotwagner/SkiaViewer` commit `7aac43dd12903f93004d0c2bf7c6254318a366dc`  
**Scale/Scope**: Three packable packages, at least eight parity samples, full public contract coverage for core scene/viewer/chart/layout/graph surfaces, parity evidence for 100% of non-conflicting baseline capabilities

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is Tier 1; `contracts/public-api.md` defines the intended `.fsi` surface before implementation, and test tasks must exercise the packed packages or prelude scripts before `.fs` bodies are accepted.
- **Principle II - Visibility Lives in `.fsi`**: PASS. Every public module in all three packages requires a companion `.fsi`; source files must not use top-level `private`, `internal`, or `public` modifiers for visibility.
- **Principle III - Idiomatic Simplicity**: PASS. Scene, chart, layout, graph, and diagnostics contracts use records, discriminated unions, functions, and modules. Mutable code is limited to Vulkan/window lifetime, rendering caches, screenshot buffers, and measured hot paths with one-line justification comments.
- **Principle IV - Elmish/MVU Boundary**: PASS. Stateful viewer operation remains Elmish/MVU. Charts, DataGrid, layout, and graph are pure view-layer components used from `view`; interaction state such as selection, sort, zoom, and visible range remains in the Elmish `Model`.
- **Principle V - Synthetic Evidence Disclosure**: PASS with constraint. Headless or driver-limited environments may provide synthetic negative-path evidence only when marked and disclosed; merge readiness requires real Vulkan-capable evidence or an explicit synthetic override.
- **Principle VI - Test Evidence Is Mandatory**: PASS. The plan requires contract tests, semantic tests, rendering tests, smoke tests, packaging tests, and parity evidence before implementation is complete.
- **Principle VII - Observability and Safe Failure**: PASS. Startup, renderer capability, frame recovery, screenshot, and shutdown diagnostics are first-class contract data.
- **Change Classification**: PASS. The spec declares Tier 1 with public API impact.
- **Engineering Constraints**: PASS. The plan uses F#/.NET, net10.0, pinned dependencies, `.fsi` surfaces, package output, and Vulkan smoke evidence.

## Project Structure

### Documentation (this feature)

```text
specs/002-skia-feature-parity/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-api.md
├── checklists/
│   └── requirements.md
└── tasks.md
```

### Source Code (repository root)

```text
src/
├── Lib/
│   ├── Library.fsi
│   ├── Library.fs
│   └── Lib.fsproj
├── Charts/
│   ├── Types.fsi
│   ├── Types.fs
│   ├── LineChart.fsi
│   ├── LineChart.fs
│   ├── BarChart.fsi
│   ├── BarChart.fs
│   ├── PieChart.fsi
│   ├── PieChart.fs
│   ├── ScatterPlot.fsi
│   ├── ScatterPlot.fs
│   ├── AreaChart.fsi
│   ├── AreaChart.fs
│   ├── Histogram.fsi
│   ├── Histogram.fs
│   ├── Candlestick.fsi
│   ├── Candlestick.fs
│   ├── RadarChart.fsi
│   ├── RadarChart.fs
│   ├── DataGrid.fsi
│   ├── DataGrid.fs
│   └── Charts.fsproj
└── Layout/
    ├── Types.fsi
    ├── Types.fs
    ├── Layout.fsi
    ├── Layout.fs
    ├── Graph.fsi
    ├── Graph.fs
    ├── GraphValidation.fsi
    ├── GraphValidation.fs
    └── Layout.fsproj

samples/
├── BasicViewer/
├── InteractiveViewer/
├── ParityGallery/
├── EffectsGallery/
├── ChartsGallery/
├── DataGridGallery/
├── LayoutGraphGallery/
├── ScreenshotGallery/
└── DemoReel/

tests/
├── Lib.Tests/
├── Charts.Tests/
├── Layout.Tests/
├── Parity.Tests/
├── Package.Tests/
└── Smoke.Tests/

scripts/
├── prelude.fsx
├── charts-prelude.fsx
├── layout-prelude.fsx
└── parity-evidence.fsx

readiness/
└── parity-evidence.json
```

**Structure Decision**: Keep the existing core package in `src/Lib` and add separate `src/Charts` and `src/Layout` projects so consumers can reference chart/data-grid and layout/graph capabilities independently. Add focused test projects for each package plus cross-package parity, packaging, and smoke evidence.

## Complexity Tracking

No constitution violations require justification. The additional package projects are required by the clarified product requirement that core viewer, charts/data grid, and layout/graph capabilities be independently referenceable.

## Phase 0: Research

See [research.md](./research.md). Decisions cover the pinned baseline, package boundaries, Elmish view-component architecture, Skia feature coverage strategy, chart/DataGrid scale targets, graph layout approach, evidence model, diagnostics, screenshot verification, and dependency policy.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-api.md](./contracts/public-api.md), and [quickstart.md](./quickstart.md).

Design summary:

- Core package owns `Scene`, `Element`, `Paint`, `Shader`, `Filter`, `Path`, `ViewerProgram`, diagnostics, screenshot requests, and Vulkan host effects.
- Chart package owns immutable chart/data-grid config and data props. It produces core `Scene`/`Element` values and optional pure hit-test results. It does not own application state.
- Layout package owns immutable layout and graph definitions. It produces core `Scene`/`Element` values and pure validation/layout results. It does not own application state.
- Elmish application state, including selected chart point, sorted column, scroll position, zoom range, hovered node, and current graph focus, belongs to the consumer `Model` and changes through `Msg`/`update`.

## Constitution Check - Post Design

- **Principle I**: PASS. `contracts/public-api.md` defines `.fsi` modules and the quickstart requires prelude/packed-package usage before implementation completion.
- **Principle II**: PASS. All planned public modules have `.fsi` files and surface-area baselines.
- **Principle III**: PASS. Contracts use simple F# records, discriminated unions, modules, and pure functions. Planned mutation is constrained to rendering/resource hot paths.
- **Principle IV**: PASS. MVU boundary is explicit; view components are pure and state-free.
- **Principle V**: PASS. Synthetic-only negative evidence is disclosed as unacceptable for merge readiness unless overridden.
- **Principle VI**: PASS. Automated-first test evidence is required for every non-conflicting baseline capability.
- **Principle VII**: PASS. Diagnostics and safe failure are contract entities and tested outcomes.
- **Engineering Constraints**: PASS. Dependencies are pinned and minimized; package boundaries are documented.
