---
title: Line Chart
category: Controls
categoryindex: 2
index: 51
description: Controls-owned line data visualization.
---

<!-- BEGIN GENERATED: catalog-docs/line-chart -->
# Line Chart

- **Category:** chart
- **Purpose:** Controls-owned line data visualization.
- **API reference:** [FS.Skia.UI.Controls.LineChart](../reference/fs-skia-ui-controls-linechart.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/line-chart -->

## Overview

The **Line Chart** control is a `chart`-category control in the FS.Skia.UI control suite.
Controls-owned line data visualization. It requires the `series` attribute. It raises `onSelected`. Its accessibility role is `Chart`.

Build it through the `FS.Skia.UI.Controls.LineChart` module — the typed Props/MVU front door is the
preferred authoring surface (see
[Typed control front door](../controls-design/typed-front-door.html)). Every
attribute is owned by the model; the control renders against the active `Theme`,
so its colours, sizing, and density follow the design tokens (see
[Controls in the Spec Kit workflow](spec-kit-workflow.html)).

## Usage example

A runnable example that exercises this control lives in the controls gallery
sample, `samples/ControlsGallery/Program.fs`. It is compiled as part of the build,
so the example stays in lock-step with the public API.

## Preview

![Line Chart render-only preview](../img/controls/line-chart.png)

A deterministic **render-only** preview of **Line Chart** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
