---
title: Scatter Plot
category: Controls
categoryindex: 2
index: 54
description: Controls-owned point cloud visualization.
---

<!-- BEGIN GENERATED: catalog-docs/scatter-plot -->
# Scatter Plot

- **Category:** chart
- **Purpose:** Controls-owned point cloud visualization.
- **API reference:** [FS.Skia.UI.Controls.ScatterPlot](../reference/fs-skia-ui-controls-scatterplot.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/scatter-plot -->

## Overview

The **Scatter Plot** control is a `chart`-category control in the FS.Skia.UI control suite.
Controls-owned point cloud visualization. It requires the `series` attribute. It raises `onSelected`. Its accessibility role is `Chart`.

Build it through the `FS.Skia.UI.Controls.ScatterPlot` module — the typed Props/MVU front door is the
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

![Scatter Plot render-only preview](../img/controls/scatter-plot.png)

A deterministic **render-only** preview of **Scatter Plot** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
