---
title: Technical Design
category: Design
categoryindex: 4
index: 1
description: Entry point for architecture, subsystem, and design decision documentation.
---

# Technical Design

FS.Skia.UI is an Elmish-first F# UI toolkit that models UI as immutable scene
data, runs application state through explicit messages and effects, and renders
through a Vulkan-only SkiaSharp desktop path. This page is the technical design
entry point for maintainers and contributors.

## Design Map

| Document | Purpose |
|----------|---------|
| [Architecture Overview](architecture.md) | Repository, package, runtime, sample, and governance structure, linked to [src](../src/) and [build.fsx](../build.fsx). |
| [Runtime Design](runtime-design.md) | Viewer program shape, event/effect flow, rendering, screenshots, diagnostics, and supported platform boundary in [src/Lib/Library.fsi](../src/Lib/Library.fsi) and [src/Lib/Library.fs](../src/Lib/Library.fs). |
| [Subsystem Design](subsystem-design.md) | Scene model, keyboard input, Controls-owned charts/DataGrid, layout, graph, samples, tests, and template governance linked to their source modules. |
| [Design Decisions](design-decisions.md) | Rationale for the major architectural choices and rejected alternatives. |
| [Agent Consumer Framework Analysis](2026-05-28-1557-agent-consumer-framework-analysis.md) | Timestamped analysis of FS.Skia.UI as a Spec Kit agent-consumed framework, including validation tiers, Controls typing, and build graph recommendations. |
| [Refactoring Analysis](2026-05-27-2204-refactoring-analysis.md) | Maintainability hotspots, duplication inventory, and phased cleanup plan for generated evidence, build automation, viewer internals, and compatibility surfaces. |

## Source Map

| Area | Primary source |
|------|----------------|
| Compatibility core package | [src/Lib/Library.fsi](../src/Lib/Library.fsi), [src/Lib/Library.fs](../src/Lib/Library.fs), and [src/Lib/KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi) |
| Scene primitives | [src/Scene/Scene.fsi](../src/Scene/Scene.fsi) and [src/Scene/Scene.fs](../src/Scene/Scene.fs) |
| Skia viewer host | [src/SkiaViewer/SkiaViewer.fsi](../src/SkiaViewer/SkiaViewer.fsi) and [src/SkiaViewer/SkiaViewer.fs](../src/SkiaViewer/SkiaViewer.fs) |
| Elmish viewer integration | [src/Elmish/Elmish.fsi](../src/Elmish/Elmish.fsi) and [src/Elmish/Elmish.fs](../src/Elmish/Elmish.fs) |
| Keyboard input package | [src/KeyboardInput/KeyboardInput.fsi](../src/KeyboardInput/KeyboardInput.fsi) and [src/KeyboardInput/KeyboardInput.fs](../src/KeyboardInput/KeyboardInput.fs) |
| Controls, charts, graph views, DataGrid, rich rendering | [src/Controls](../src/Controls/) |
| Controls Elmish adapter | [src/Controls.Elmish/ControlsElmish.fsi](../src/Controls.Elmish/ControlsElmish.fsi) and [src/Controls.Elmish/ControlsElmish.fs](../src/Controls.Elmish/ControlsElmish.fs) |
| Layout and lower-level graph helpers | [src/Layout](../src/Layout/) |
| Samples | [samples](../samples/) |
| Tests | [tests](../tests/) |
| Build and template workflow | [build.fsx](../build.fsx), [.template.config/template.json](../.template.config/template.json), and [.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj) |
| Dependency and drift scripts | [scripts/dependency-report.fsx](../scripts/dependency-report.fsx) and [scripts/template-drift.fsx](../scripts/template-drift.fsx) |

## Related Operational Documents

| Document | What it governs |
|----------|-----------------|
| [Build Workflow](build.md) | FAKE target graph and automation entry points. |
| [Testing Workflow](testing.md) | Test projects, FSI transcripts, sample smoke, and template validation matrix. |
| [Evidence Policy](evidence.md) | Required readiness artifacts and evidence boundaries. |
| [Dependency Governance](dependencies.md) | Central Package Management, package ownership, and validation-only version exceptions. |
| [Template Profile](template-profile.md) | `dotnet new fs-skia-ui` profiles, artifact boundaries, and drift classification. |
| [Spec Kit Governance](speckit.md) | Generated prompts, task `skillist` governance, implementation skill loading, and deferred roadmap boundaries. |

## How To Read This Set

Start with the architecture overview when changing package boundaries, project
references, or build targets. Use the runtime design before changing
`ViewerProgram`, `ViewerEvent`, `ViewerEffect`, rendering, diagnostics, or
screenshot behavior. Use subsystem design when changing Controls-owned chart
controls, DataGrid, layout, graph rendering, keyboard input, samples, or
template ownership. Record any new cross-cutting choice in the design decision
log.
