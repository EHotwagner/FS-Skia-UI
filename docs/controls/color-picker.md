---
title: Color Picker
category: Controls
categoryindex: 8
index: 60
description: Palette swatch color selection.
---

<!-- BEGIN GENERATED: catalog-docs/color-picker -->
# Color Picker

- **Category:** selection
- **Purpose:** Palette swatch color selection.
- **API reference:** [FS.Skia.UI.Controls.ColorPicker](../reference/fs-skia-ui-controls-typed-colorpicker.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/color-picker -->

## Overview

The **Color Picker** control is a `selection`-category control in the FS.Skia.UI typed front door.
Palette swatch color selection. It requires the `swatches` attribute. It raises `onSelected`. Its accessibility role is `List`.

Build it through the `FS.Skia.UI.Controls.Typed.ColorPicker` module — the typed Props/MVU front door is the
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

![Color Picker render-only preview](../img/controls/color-picker.png)

A deterministic **render-only** preview of **Color Picker** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
