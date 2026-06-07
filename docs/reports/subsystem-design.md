---
title: Subsystem Design
index: 4
description: Technical design notes for Controls-owned charts, DataGrid, layout, graph, keyboard input, samples, tests, and template governance.
---

# Subsystem Design

Most subsystems are pure builders or reducers over public data types. They
return `Scene`, state, diagnostics, hit-test results, or workflow effects. They
do not create windows or perform rendering side effects.

## Scene And Paint Primitives

The core scene layer defines the reusable vocabulary for all higher-level
subsystems: colors, paints, paths, clips, regions, color spaces, perspective,
text runs, pictures, vertices, and scene grouping. Subsystems should prefer
these primitives over adding renderer-specific APIs. New primitives belong in
`FS.Skia.UI` only when they are broadly useful outside one widget family. The
public declarations are in [src/Lib/Library.fsi](../src/Lib/Library.fsi), with
implementation in [src/Lib/Library.fs](../src/Lib/Library.fs).

## Keyboard Input

Keyboard input is a package-owned reducer around `KeyboardModel`. YAML
configuration is parsed, validated against command bindings, and initialized
with an active layout. Runtime changes enter through `KeyboardMsg` or mapped
viewer events, then return an updated model plus `KeyboardEffect` values such
as resolved commands, layout state changes, diagnostics, and recorded events.

The keyboard state display is also scene-based. `KeyboardInput.keyboardStateDisplay`
builds an inspectable model, while `renderKeyboardStateDisplay` and
`renderKeyboardStateDisplayAt` turn it into `Scene`.
The public keyboard input contract is in
[src/KeyboardInput/KeyboardInput.fsi](../src/KeyboardInput/KeyboardInput.fsi),
and parsing, validation, update, display projection, rendering, replay, and
bigram analysis are implemented in
[src/KeyboardInput/KeyboardInput.fs](../src/KeyboardInput/KeyboardInput.fs).

## Charts And DataGrid

`FS.Skia.UI.Controls` owns chart controls, graph views, and DataGrid for current
product authoring. Shared chart configuration and builders live in
[src/Controls/Charts.fsi](../src/Controls/Charts.fsi) and
[src/Controls/Charts.fs](../src/Controls/Charts.fs). Chart controls render
finite product-owned data to `Scene` and expose diagnostics while interaction
state belongs to the caller or the explicit `ControlRuntime`.

`DataGrid` follows the same ownership rule: callers own rows, sorting,
viewport, focus, and selection state. Controls provides column/row declarations,
visible-row calculation, rendering, hit testing, accessibility metadata, and
diagnostics through [src/Controls/DataGrid.fsi](../src/Controls/DataGrid.fsi)
and [src/Controls/DataGrid.fs](../src/Controls/DataGrid.fs).

Products that need only lower-level layout or graph helpers can continue to use
`FS.Skia.UI.Layout` directly; they do not need to select Controls.

## Layout And Graph

`FS.Skia.UI.Layout` wraps layout intent, content measurement, computed bounds,
pixel snapping, and rendering. `Layout.evaluate` computes a `LayoutResult` from
`AvailableSpace` and a `LayoutNode` tree. `renderComputed` turns the result back
into scene output, while `hitTestComputed` maps positions to node ids. The
public layout types are in [src/Layout/Types.fsi](../src/Layout/Types.fsi), and
the evaluator is in [src/Layout/Layout.fs](../src/Layout/Layout.fs).

Stateful layout workflow is modeled explicitly:

```text
LayoutWorkflowMsg
  -> updateWorkflow
  -> LayoutWorkflowModel + LayoutWorkflowEffect
  -> interpretWorkflowEffect
  -> LayoutWorkflowMsg
```

Graph support is built on layout package types. `GraphValidation.validate`
checks node/edge integrity and cycles. `Graph.layout` produces node bounds and
edge routing data, and `Graph.directed` or `Graph.undirected` renders a scene
when validation succeeds. The graph code is in [src/Layout/GraphValidation.fs](../src/Layout/GraphValidation.fs)
and [src/Layout/Graph.fs](../src/Layout/Graph.fs).

## Samples

Samples are executable documentation and smoke coverage. Every current sample
has a `--contract-smoke` mode that exercises public APIs without opening a live
window. Package-aware samples also support `UsePackedPackage=true` to validate
the packed `FS.Skia.UI` surface. See [README Samples](../README.md#samples) for
the current sample matrix.

## Tests And Evidence

Tests are split by ownership:

| Area | Test project |
|------|--------------|
| Compatibility core package | [tests/Lib.Tests](../tests/Lib.Tests/) |
| Scene primitives | [tests/Scene.Tests](../tests/Scene.Tests/) |
| Skia viewer host | [tests/SkiaViewer.Tests](../tests/SkiaViewer.Tests/) |
| Elmish viewer integration | [tests/Elmish.Tests](../tests/Elmish.Tests/) |
| Keyboard input package | [tests/KeyboardInput.Tests](../tests/KeyboardInput.Tests/) |
| Controls, chart controls, graph views, DataGrid, rich rendering | [tests/Controls.Tests](../tests/Controls.Tests/) |
| Layout and graph | [tests/Layout.Tests](../tests/Layout.Tests/) |
| Skia feature parity semantics | [tests/Parity.Tests](../tests/Parity.Tests/) |
| Package surface and packed consumer checks | [tests/Package.Tests](../tests/Package.Tests/) |
| Sample contract checks | [tests/Smoke.Tests](../tests/Smoke.Tests/) |
| Build, docs, template, dependency, and drift governance | [tests/Governance.Tests](../tests/Governance.Tests/) |

Feature evidence is stored under the active feature readiness directory. Stable
package surface baselines live under root `readiness/surface-baselines/`.

## Template Governance

The repository is also the governed source for the `fs-skia-ui` project
template. Template-owned changes include source, samples, tests, docs, build
targets, dependency policy, Spec Kit templates, command wrappers, and template
metadata. `TemplateCheck`, `DependencyReport`, `GeneratedGuidanceCheck`, and
`TemplateDrift` keep generated projects aligned with the source repository. The
template metadata is in [.template.config/template.json](../.template.config/template.json),
the local package project is [.template.package/FS.Skia.UI.Template.fsproj](../.template.package/FS.Skia.UI.Template.fsproj),
and the workflow is implemented in [build.fsx](../build.fsx).
