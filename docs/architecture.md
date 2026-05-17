---
title: Architecture Overview
category: Design
categoryindex: 4
index: 2
description: High-level architecture and component relationships for FS.Skia.UI.
---

# Architecture Overview

FS.Skia.UI is organized as packable libraries, sample hosts, tests, and a
governed project template. The runtime boundary is intentionally small:
product code creates Scene or Controls declarations, owns product state, and
lets the viewer or Controls.Elmish adapter translate events and effects at the
edge.

## Repository Shape

```text
src/Scene/             FS.Skia.UI.Scene primitives and deterministic readback
src/SkiaViewer/        FS.Skia.UI.SkiaViewer Silk.NET/Skia host integration
src/Elmish/            FS.Skia.UI.Elmish lower-level viewer adapter
src/KeyboardInput/     FS.Skia.UI.KeyboardInput runtime state and reducers
src/Layout/            FS.Skia.UI.Layout Yoga layout and graph validation
src/Controls/          FS.Skia.UI.Controls controls, rich rendering, charts, graph views, DataGrid
src/Controls.Elmish/   FS.Skia.UI.Controls.Elmish command/subscription/program adapter
src/Lib/               FS.Skia.UI compatibility package for lower-level paths
samples/               executable sample hosts and non-visual contract smoke paths
tests/                 semantic, package surface, smoke, parity, and governance tests
docs/                  technical, operational, dependency, template, and evidence docs
scripts/               FSI transcripts, dependency report, surface baselines, drift scan
```

Source directories: [src/Scene](../src/Scene/), [src/SkiaViewer](../src/SkiaViewer/),
[src/Elmish](../src/Elmish/), [src/KeyboardInput](../src/KeyboardInput/),
[src/Layout](../src/Layout/), [src/Controls](../src/Controls/),
[src/Controls.Elmish](../src/Controls.Elmish/), [src/Lib](../src/Lib/),
[samples](../samples/), [tests](../tests/), [docs](../docs/), and
[scripts](../scripts/).

The package dependency direction is one-way:

```text
FS.Skia.UI.Scene ─┬──> FS.Skia.UI.SkiaViewer ───> FS.Skia.UI.Elmish
                  ├──> FS.Skia.UI.KeyboardInput ─┐
                  └──> FS.Skia.UI.Layout ─────────┼──> FS.Skia.UI.Controls
                                                   └──> FS.Skia.UI.Controls.Elmish

Samples may reference one or more packages. Tests reference the package under
test and, where needed, the governance scripts or package surface baselines.
```

`FS.Skia.UI.SkiaViewer` owns platform rendering. Controls, KeyboardInput, and
Layout expose product-owned state, pure reducers, declarations, and Scene
output; they do not create windows, touch Vulkan, or own host-loop execution.

## Runtime Components

| Component | Location | Responsibility |
|-----------|----------|----------------|
| Scene model | [src/Scene/Scene.fsi](../src/Scene/Scene.fsi) | Immutable shape, text, image, clip, effect, picture, and deterministic readback primitives. |
| Viewer host | [src/SkiaViewer](../src/SkiaViewer/) | Silk.NET window/input integration, Vulkan/Skia renderer setup, frame rendering, screenshots, and shutdown. |
| Elmish viewer adapter | [src/Elmish](../src/Elmish/) | Lower-level Elmish integration for products that build Scene values directly. |
| Keyboard input | [src/KeyboardInput/KeyboardInput.fsi](../src/KeyboardInput/KeyboardInput.fsi) | Runtime state, mode stack, pending sequences, focus recovery, diagnostics, effects, and state display scenes. |
| Controls/Charts/DataGrid | [src/Controls](../src/Controls/) | Stable controls, rich rendering, chart controls, graph views, DataGrid, ControlRuntime, diagnostics, and catalog metadata. |
| Controls Elmish adapter | [src/Controls.Elmish](../src/Controls.Elmish/) | Command, subscription, and program wiring for Controls and KeyboardInput effects. |
| Layout/Graph | [src/Layout](../src/Layout/) | Yoga-backed layout evaluation, workflow effects, graph validation, graph layout, rendering, and hit testing. |

## Data Flow

```text
Host event
  -> ViewerEvent
  -> EventMapper
  -> Msg
  -> Update(Model, Msg)
  -> Model + Cmd<Msg>
  -> EffectMapper
  -> ViewerEffect
  -> Viewer interpreter
  -> Vulkan/Skia, screenshot, diagnostic, dispatch, or shutdown side effect

Model
  -> View(Model)
  -> Scene
  -> renderer walks Scene and paints a frame
```

The important architectural boundary is that application and subsystem code can
build models, messages, scenes, and effects without creating windows or invoking
Vulkan. Process, filesystem, window, GPU, screenshot, and shutdown work stays at
the viewer or build-script edge.

## Build And Governance Architecture

[`build.fsx`](../build.fsx) is the canonical command surface. `Dev` restores,
builds, and runs the default non-visual tests. `Verify` adds package checks,
FSI transcripts, sample contract smoke, template validation, dependency
governance, generated guidance checks, template drift, and evidence audit. `Ci`
delegates to `Verify`.

The `fs-skia-ui` template is built from the same repository through
[.template.config/template.json](../.template.config/template.json) and
[.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj).
Template validation installs from both the source directory and the local NuGet
template package, then generates the V3 `app`, `headless-scene`, `governed`,
and `sample-pack` projects and runs their `Dev` workflows. See [Template Profile](template-profile.md),
[Testing Workflow](testing.md), and [Evidence Policy](evidence.md) for the
governed artifact boundaries.

## Extension Points

Add new core rendering primitives in `FS.Skia.UI.Scene` when the concept is a
general scene element or viewer effect. Add Skia-rendered controls, chart
controls, graph views, DataGrid behavior, or rich rendering to
`FS.Skia.UI.Controls` when it belongs to the high-level Controls authoring
path. Add sample-host behavior in [samples](../samples/) and give it a
`--contract-smoke` path when it exercises public APIs without requiring a live
window.
