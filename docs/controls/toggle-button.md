---
title: Toggle Button
category: Controls
categoryindex: 2
index: 56
description: On/off command with product-owned pressed state.
---

<!-- BEGIN GENERATED: catalog-docs/toggle-button -->
# Toggle Button

- **Category:** input
- **Purpose:** On/off command with product-owned pressed state.
- **API reference:** [FS.Skia.UI.Controls.ToggleButton](../reference/fs-skia-ui-controls-typed-togglebutton.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/toggle-button -->

## Overview

The **Toggle Button** control is a `input`-category control in the FS.Skia.UI typed front door.
On/off command with product-owned pressed state. It requires the `text` attribute. It raises `onToggle`. Its accessibility role is `Button`.

Build it through the `FS.Skia.UI.Controls.Typed.ToggleButton` module — the typed Props/MVU front door is the
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

![Toggle Button render-only preview](../img/controls/toggle-button.png)

A deterministic **render-only** preview of **Toggle Button** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
