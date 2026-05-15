---
title: Architecture Overview
category: Design
categoryindex: 4
index: 2
description: High-level architecture and component relationships for FS.Skia.UI.
---

# Architecture Overview

FS.Skia.UI is organized as three packable libraries, sample hosts, tests, and a
governed project template. The runtime boundary is intentionally small: product
code creates an Elmish viewer program, returns a declarative `Scene`, and lets
the viewer host translate events and effects into Vulkan/Skia work.

## Repository Shape

```text
src/Lib/       FS.Skia.UI core scene, viewer, diagnostics, keyboard input
src/Charts/    FS.Skia.UI.Charts chart and DataGrid scene builders
src/Layout/    FS.Skia.UI.Layout layout, graph layout, graph validation
samples/       executable sample hosts and non-visual contract smoke paths
tests/         semantic, package surface, smoke, parity, and governance tests
docs/          technical, operational, dependency, template, and evidence docs
scripts/       FSI transcripts, dependency report, surface baselines, drift scan
```

Source directories: [src/Lib](../src/Lib/), [src/Charts](../src/Charts/),
[src/Layout](../src/Layout/), [samples](../samples/), [tests](../tests/),
[docs](../docs/), and [scripts](../scripts/).

The package dependency direction is one-way:

```text
FS.Skia.UI.Charts  ─┐
                    ├──> FS.Skia.UI
FS.Skia.UI.Layout  ─┘

Samples may reference one or more packages. Tests reference the package under
test and, where needed, the governance scripts or package surface baselines.
```

`FS.Skia.UI` is the only package that owns platform rendering. Charts and
layout return core `Scene` values; they do not create windows, touch Vulkan, or
own application state.

## Runtime Components

| Component | Location | Responsibility |
|-----------|----------|----------------|
| Scene model | [src/Lib/Library.fsi](../src/Lib/Library.fsi) | Immutable shape, text, image, clip, effect, picture, and chart placeholders. |
| Viewer host | [src/Lib/Library.fs](../src/Lib/Library.fs) | Elmish program execution, Silk.NET window/input integration, Vulkan/Skia renderer setup, frame rendering, screenshots, and shutdown. |
| Diagnostics | [src/Lib/Library.fsi](../src/Lib/Library.fsi) | Structured failure reporting by stage: platform, Vulkan, surface, swapchain, Skia, frame, screenshot, and shutdown. |
| Keyboard input | [src/Lib/KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi) | YAML-driven command bindings, mode stack, pending sequences, layout state, replay, bigram analysis, and state display scenes. |
| Charts/DataGrid | [src/Charts](../src/Charts/) | Pure scene builders and hit testing over finite chart or table data. |
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
template package, then generates default and minimal projects and runs their
`Dev` workflows. See [Template Profile](template-profile.md),
[Testing Workflow](testing.md), and [Evidence Policy](evidence.md) for the
governed artifact boundaries.

## Extension Points

Add new core rendering primitives in `FS.Skia.UI` when the concept is a general
scene element or viewer effect. Add pure widgets to `FS.Skia.UI.Charts` or
`FS.Skia.UI.Layout` when they can be expressed as `Scene` output without owning
host effects. Add sample-host behavior in [samples](../samples/) and give it a
`--contract-smoke` path when it exercises public APIs without requiring a live
window.
