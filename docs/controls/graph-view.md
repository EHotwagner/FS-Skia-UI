---
title: Graph View
category: Controls
categoryindex: 2
index: 55
description: Controls-owned node and edge visualization.
---

<!-- BEGIN GENERATED: catalog-docs/graph-view -->
# Graph View

- **Category:** graph
- **Purpose:** Controls-owned node and edge visualization.
- **API reference:** [FS.Skia.UI.Controls.GraphView](../reference/fs-skia-ui-controls-graphview.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/graph-view -->

## Overview

The **Graph View** control is a `graph`-category control in the FS.Skia.UI control suite.
Controls-owned node and edge visualization. It requires the `nodes` attribute. It raises `onSelected`. Its accessibility role is `Graph`.

Build it through the `FS.Skia.UI.Controls.GraphView` module — the typed Props/MVU front door is the
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

![Graph View render-only preview](../img/controls/graph-view.png)

A deterministic **render-only** preview of **Graph View** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
