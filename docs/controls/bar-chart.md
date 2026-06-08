---
title: Bar Chart
category: Controls
categoryindex: 8
index: 52
description: Controls-owned bar data visualization.
---

<!-- BEGIN GENERATED: catalog-docs/bar-chart -->
# Bar Chart

- **Category:** chart
- **Purpose:** Controls-owned bar data visualization.
- **API reference:** [FS.Skia.UI.Controls.BarChart](../reference/fs-skia-ui-controls-barchart.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/bar-chart -->

## Overview

The **Bar Chart** control is a `chart`-category control in the FS.Skia.UI control suite.
Controls-owned bar data visualization. It requires the `series` attribute. It raises `onSelected`. Its accessibility role is `Chart`.

Build it through the `FS.Skia.UI.Controls.BarChart` module — the typed Props/MVU front door is the
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

![Bar Chart render-only preview](../img/controls/bar-chart.png)

A deterministic **render-only** preview of **Bar Chart** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
