---
title: Data Grid
category: Controls
categoryindex: 2
index: 31
description: Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata.
---

<!-- BEGIN GENERATED: catalog-docs/data-grid -->
# Data Grid

- **Category:** data
- **Purpose:** Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata.
- **API reference:** [FS.Skia.UI.Controls.DataGrid](../reference/fs-skia-ui-controls-datagrid.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/data-grid -->

## Overview

The **Data Grid** control is a `data`-category control in the FS.Skia.UI control suite.
Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata. It requires the `columns`, `rows` attributes. It raises `onSelected`, `onFocusChanged`, `onSortChanged`. Its accessibility role is `Grid`.

Build it through the `FS.Skia.UI.Controls.DataGrid` module — the typed Props/MVU front door is the
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

![Data Grid render-only preview](../img/controls/data-grid.png)

A deterministic **render-only** preview of **Data Grid** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
