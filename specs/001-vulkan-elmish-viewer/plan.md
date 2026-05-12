# Implementation Plan: Vulkan Elmish Viewer

**Branch**: `001-vulkan-elmish-viewer` | **Date**: 2026-05-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/001-vulkan-elmish-viewer/spec.md`

## Summary

Build a packable F# library that exposes an Elmish-only Model-View-Update surface for a desktop viewer using SkiaSharp 4 preview and a Vulkan-only rendering path. The first version supports Windows and Linux desktop, provides runnable sample applications, rejects non-Vulkan or non-Elmish configuration, and validates the public API through `.fsi`, semantic tests, FSI/prelude usage, packaging, and sample smoke tests.

## Technical Context

**Language/Version**: F# on .NET `net10.0` with `LangVersion=latest`  
**Primary Dependencies**: SkiaSharp `4.147.0-preview.2.1`; SkiaSharp native assets for Windows/Linux `4.147.0-preview.2.1`; Fable.Elmish `4.2.0`; Silk.NET.Windowing/Input/Vulkan `2.23.0`; Expecto `10.2.2` for tests  
**Storage**: Filesystem only for screenshot output and package artifacts; no persistent application data store  
**Testing**: `dotnet test`, `.fsi` surface checks, semantic tests through packed library or prelude, smoke runs for sample applications, Vulkan-unavailable startup diagnostics test  
**Target Platform**: Windows and Linux desktop only  
**Project Type**: Packable F# library plus runnable desktop sample applications  
**Performance Goals**: First visible frame within 2 seconds on Vulkan-capable developer workstation in 95% of smoke runs; interactive input visible within 1 second; subscription sample advances for 60 seconds  
**Constraints**: Vulkan-only; Elmish-only public integration; no fallback renderer; fail fast when Vulkan initialization is unavailable; public modules require `.fsi` signatures  
**Scale/Scope**: One reusable library, at least two runnable sample apps, semantic tests for pure MVU transitions and edge interpreter behavior, documentation for Vulkan-only environment requirements
**Compatibility Guidance**: This is a first-version public package with no prior FS-Skia-UI public API migration path. Documentation must still describe package compatibility expectations, preview dependency risk, and the absence of an older public API migration path.

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Principle I - Spec -> FSI -> Semantic Tests -> Implementation**: PASS. This is Tier 1 and the contract artifact defines the `.fsi` surface before implementation tasks.
- **Principle II - Visibility Lives in `.fsi`**: PASS. Public modules will have companion `.fsi` files; `.fs` top-level access modifiers remain forbidden.
- **Principle III - Idiomatic Simplicity**: PASS. The design uses records, discriminated unions, modules, and explicit effects; any mutation is limited to Vulkan/window lifetime edge code and must be commented at use sites.
- **Principle IV - Elmish/MVU Boundary**: PASS. The public contract exposes `Model`, `Msg`, `Effect`, `init`, `update`, `view`, subscriptions, and an interpreter/host boundary.
- **Principle V - Synthetic Evidence Disclosure**: PASS with constraint. Headless CI may need synthetic Vulkan-unavailable checks, but merge readiness requires real evidence for at least one supported Vulkan-capable environment or explicit synthetic disclosure.
- **Principle VI - Test Evidence Is Mandatory**: PASS. Plan requires pre-implementation semantic tests, package/prelude tests, and sample smoke tests.
- **Principle VII - Observability and Safe Failure**: PASS. Renderer initialization and frame failures are structured diagnostics and unsupported environments fail fast.
- **Engineering Constraints**: PASS. F#/.NET stack, net10.0 default, explicit dependency pinning, pack output, and SkiaSharp preview constraints are captured.
- **Specification Gate**: PASS after spec update. `spec.md` declares Tier 1, public API impact, and verification approach.

## Project Structure

### Documentation (this feature)

```text
specs/001-vulkan-elmish-viewer/
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
└── Lib/
    ├── Library.fsi
    ├── Library.fs
    └── Lib.fsproj

samples/
├── BasicViewer/
│   ├── BasicViewer.fsproj
│   └── Program.fs
└── InteractiveViewer/
    ├── InteractiveViewer.fsproj
    └── Program.fs

tests/
└── Lib.Tests/
    ├── Lib.Tests.fsproj
    ├── Tests.fs
    └── Program.fs

scripts/
└── prelude.fsx
```

**Structure Decision**: Keep the current packable library in `src/Lib` and add `samples/` for runnable smoke-test applications. Use the existing Expecto test project for semantic tests, FSI/prelude contract validation, diagnostics behavior, and non-rendering pure MVU coverage.

## Complexity Tracking

No constitution violations require justification.

## Phase 0: Research

See [research.md](./research.md). All technical unknowns have decisions: latest SkiaSharp 4 preview pin, native asset strategy, window/Vulkan binding choice, Elmish runtime choice, Vulkan fail-fast behavior, and test evidence strategy.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/public-api.md](./contracts/public-api.md), and [quickstart.md](./quickstart.md).

## Constitution Check - Post Design

- **Principle I**: PASS. `contracts/public-api.md` maps directly to planned `.fsi` modules and semantic tests.
- **Principle II**: PASS. Public modules are explicitly contract-first.
- **Principle III**: PASS. No advanced F# features are required in the planned public API.
- **Principle IV**: PASS. Data model and contract define pure update plus effect interpretation at the host edge.
- **Principle V**: PASS. Synthetic-only Vulkan tests are identified as unacceptable for merge readiness unless disclosed and overridden.
- **Principle VI**: PASS. Quickstart and contract require automated tests before implementation.
- **Principle VII**: PASS. Diagnostics are first-class data and part of the public contract.
- **Engineering Constraints**: PASS. Dependency versions are pinned and ownership belongs to this library.
