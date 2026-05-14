# Implementation Plan: Yoga.Net Layout for UI Elements and Widgets

**Branch**: `005-add-yoga-net-layout` | **Date**: 2026-05-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/005-add-yoga-net-layout/spec.md`

## Summary

Add an automatic flex-style layout capability for FS-Skia-UI elements and widgets by adapting Yoga.Net behind the existing `FS.Skia.UI.Layout` package. The feature introduces public F# layout intent, tree, measurement, invalidation, diagnostic, computed-bounds, rendering, hit-test snapping, and widget participation contracts while preserving existing manual scene composition for absolute and overlay use cases.

## Technical Context

**Language/Version**: F# on .NET `net10.0` with `LangVersion=latest`; Yoga.Net is a C# `net10.0`-compatible dependency consumed from F#  
**Primary Dependencies**: Existing `FS.Skia.UI` and `FS.Skia.UI.Layout`; add `Yoga.Net` `3.2.3` to `src/Layout/Layout.fsproj` with explicit version pinning; existing test dependencies `Expecto`, `Microsoft.NET.Test.Sdk`, and `YoloDev.Expecto.TestSdk`  
**Storage**: N/A for runtime; filesystem only for checked-in examples, FSI transcripts, readiness evidence, and package surface-area baselines  
**Testing**: `dotnet test`; semantic tests through `Layout.fsi`/`Types.fsi`; FSI/prelude transcript for public API shape; package surface-area baseline tests; sample smoke evidence for automatic layout galleries  
**Target Platform**: Windows and Linux desktop, aligned with existing Skia/Vulkan viewer support  
**Project Type**: Packable F# library plus sample applications  
**Performance Goals**: Representative 200-node flex tree re-layout completes within one interactive frame budget during resize; repeated layout with identical inputs produces byte-for-byte equivalent logical bounds and diagnostics; unaffected sibling subtrees keep identical computed bounds after unrelated subtree invalidation  
**Constraints**: v1 exposes flex-style row, column, and wrap layout only; absolute and overlay positioning remain outside automatic layout; custom content measurement callbacks are supported for text and custom elements; layout uses logical UI coordinates and applies deterministic pixel snapping only at render and hit-test boundaries; recoverable failures return structured diagnostics plus bounded fallback geometry; every public module requires `.fsi`; no top-level visibility modifiers in `.fs`  
**Scale/Scope**: Extend `src/Layout` public contracts and implementation, add Yoga.Net adapter internals, update tests and baselines, add scripts/sample evidence for nested elements, mixed widgets, resizing, flexible sizing, hidden elements, custom measurement, diagnostics, invalidation, and pixel snapping

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is Tier 1 because it adds public layout APIs and a new dependency. The plan requires `.fsi` contract updates, semantic tests through the public surface, FSI/prelude evidence, surface-area baseline updates, and then `.fs` implementation.
- **Principle II - Visibility Lives in `.fsi`**: PASS. New public types and functions live in `src/Layout/Types.fsi`, `src/Layout/Layout.fsi`, and any new companion `.fsi` files. Implementation-only Yoga.Net adapter helpers remain omitted from signatures.
- **Principle III - Idiomatic Simplicity**: PASS. The public F# API uses records, discriminated unions, options, lists, and `Result`. Yoga.Net interop is isolated behind simple adapter functions. No custom operators, SRTP, reflection, dynamic dispatch, type providers, or non-trivial computation expressions are planned.
- **Principle IV - Elmish/MVU Boundary**: PASS. Layout calculation is pure library behavior. Host resize, widget updates, and content-measurement invalidation are stateful workflows and will be modeled as explicit messages/effects at the sample or host edge, while the layout evaluator remains deterministic and side-effect free except for invoking supplied measurement callbacks.
- **Principle V - Synthetic Evidence Disclosure**: PASS. No synthetic-only evidence is planned. Tests use real public API layout trees, real Yoga.Net evaluation through the adapter, checked-in samples, and deterministic content measurement callbacks.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Tests must cover flex rows/columns/wrap, margins, padding, gaps, alignment, flexible sizing, widget participation, custom measurement, invalidation locality, diagnostics, fallback bounds, logical coordinates, pixel snapping, hit-test alignment, and existing manual composition compatibility.
- **Principle VII - Observability and Safe Failure**: PASS. Invalid values, unsatisfied constraints, unmeasurable content, invalid available space, and fallback behavior are returned as structured `LayoutDiagnostic` data.
- **Change Classification**: PASS. Tier 1 contracted change: public API additions, dependency addition, observable layout behavior, docs, tests, and baselines required.
- **Engineering Constraints**: PASS. F#/.NET stack remains exclusive for project code; Yoga.Net is a .NET dependency with MIT license and `net10.0` target; all public surfaces keep `.fsi`; package and surface baselines are explicit; Vulkan/GPU constraints are unaffected.

## Project Structure

### Documentation (this feature)

```text
specs/005-add-yoga-net-layout/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-api.md
├── checklists/
│   └── requirements.md
└── tasks.md             # Phase 2 output from /speckit-tasks
```

### Source Code (repository root)

```text
src/
└── Layout/
    ├── Types.fsi
    ├── Types.fs
    ├── Layout.fsi
    ├── Layout.fs
    ├── YogaLayout.fsi      # new public automatic-layout surface if kept separate
    ├── YogaLayout.fs       # new Yoga.Net-backed implementation
    └── Layout.fsproj

tests/
├── Layout.Tests/
│   ├── Tests.fs
│   ├── Program.fs
│   └── Layout.Tests.fsproj
├── Package.Tests/
│   └── SurfaceAreaTests.fs
└── Smoke.Tests/
    └── Tests.fs

samples/
├── LayoutGraphGallery/
│   └── Program.fs
└── DemoReel/
    └── Program.fs

scripts/
└── layout-prelude.fsx

specs/005-add-yoga-net-layout/readiness/
├── fsi/
│   └── yoga-layout-prelude.txt
├── performance/
│   └── yoga-layout-200-node-resize.txt
├── sample-smoke/
│   └── automatic-layout-gallery.txt
└── surface-baselines/
    └── FS.Skia.UI.Layout.txt
```

**Structure Decision**: Extend `src/Layout` because the existing package already owns layout-facing records, stack/dock helpers, graph layout, and layout tests. Existing `Layout.measureHorizontal`, `Layout.measureVertical`, stack, dock, and graph APIs remain compatible. The Yoga.Net dependency is an implementation detail of the automatic-layout evaluator, not exposed directly to applications.

## Complexity Tracking

No constitution violations require justification. The new dependency is justified by the feature input and by the need for a proven Flexbox implementation instead of a hand-rolled layout engine.

## Phase 0: Research

See [research.md](./research.md). Decisions cover dependency pinning, public API placement, Yoga.Net adapter boundary, v1 flex scope, custom measurement, invalidation, diagnostics, fallback geometry, logical coordinates, pixel snapping, hit testing, tests, and examples.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-api.md](./contracts/public-api.md), and [quickstart.md](./quickstart.md).

Design summary:

- `Types.fsi` exposes layout geometry, spacing, sizing, alignment, flex, visibility, diagnostics, measurement, invalidation, and computed-result records/unions.
- `Layout.fsi` or a companion `YogaLayout.fsi` exposes pure creation/update/evaluate functions for automatic layout trees.
- Yoga.Net nodes are allocated, styled, measured, evaluated, read back, and disposed inside the adapter; callers only see F# data.
- Custom measurement callbacks receive logical available size plus measure modes and return preferred logical size or diagnostics.
- Layout invalidation marks only the changed node and ancestors needed for correct re-evaluation; unaffected siblings retain stable bounds.
- Recoverable problems return bounded fallback geometry and structured diagnostics instead of throwing at render time.
- Rendering and hit testing consume computed logical bounds and use the same deterministic pixel-snapping policy.
- Manual stack, dock, graph, absolute, and overlay composition remain supported as migration and escape paths.

## Constitution Check - Post Design

- **Principle I**: PASS. `contracts/public-api.md` defines the intended `.fsi` additions before implementation.
- **Principle II**: PASS. Public surface changes are limited to `.fsi` contracts; Yoga.Net adapter details stay private by omission.
- **Principle III**: PASS. The design uses simple F# data types and pure functions around an isolated dependency adapter.
- **Principle IV**: PASS. Stateful host workflows are modeled outside the evaluator; the automatic layout evaluator remains deterministic from inputs to outputs.
- **Principle V**: PASS. Planned evidence uses real public API trees, real Yoga.Net evaluation, checked-in examples, and deterministic measurement callbacks with no synthetic-only tasks expected.
- **Principle VI**: PASS. Each user story maps to automated semantic tests, surface baseline updates, FSI evidence, and sample smoke evidence.
- **Principle VII**: PASS. Diagnostics and safe fallback bounds are first-class result data.
- **Engineering Constraints**: PASS. Dependency version and owner are documented; package and baseline work are explicit.
