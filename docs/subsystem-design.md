---
title: Subsystem Design
category: Design
categoryindex: 4
index: 4
description: Technical design notes for charts, DataGrid, layout, graph, keyboard input, samples, tests, and template governance.
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

Keyboard input is a reducer around `InputRuntime`. YAML configuration is parsed
into `InputConfiguration`, validated against a `CommandRegistry`, and initialized
with an active layout. Runtime changes enter through `InputMsg` or
`ViewerEvent`, then return an updated runtime plus `InputEffect` values such as
resolved commands, layout state changes, diagnostics, and recorded events.

The keyboard state display is also scene-based. `KeyboardInput.keyboardStateDisplay`
builds an inspectable model, while `renderKeyboardStateDisplay` and
`renderKeyboardStateDisplayAt` turn it into `Scene`.
The public keyboard input contract is in
[src/Lib/KeyboardInput.fsi](../src/Lib/KeyboardInput.fsi), and parsing,
validation, update, display projection, rendering, replay, and bigram analysis
are implemented in [src/Lib/KeyboardInput.fs](../src/Lib/KeyboardInput.fs).

## Charts And DataGrid

`FS.Skia.UI.Charts` owns pure chart and table scene builders. Shared chart
configuration lives in [src/Charts/Types.fsi](../src/Charts/Types.fsi):
`ChartConfig`, `AxisConfig`, `LegendConfig`, `DataSeries`, `DataPoint`, and
`ChartTarget`. Chart modules render finite input data to `Scene` and expose hit
testing where interaction state belongs to the caller. The concrete chart
modules live under [src/Charts](../src/Charts/).

`DataGrid` follows the same pattern: callers own rows, sorting, viewport, and
selection state. The subsystem provides sorting, visible-row calculation,
rendering, and hit testing through [src/Charts/DataGrid.fsi](../src/Charts/DataGrid.fsi)
and [src/Charts/DataGrid.fs](../src/Charts/DataGrid.fs).

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
| Core scene, viewer, diagnostics, keyboard input | [tests/Lib.Tests](../tests/Lib.Tests/) |
| Charts and DataGrid | [tests/Charts.Tests](../tests/Charts.Tests/) |
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
