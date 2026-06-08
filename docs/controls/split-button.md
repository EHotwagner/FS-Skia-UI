---
title: Split Button
category: Controls
categoryindex: 8
index: 57
description: Primary action plus a popup menu of secondary commands.
---

<!-- BEGIN GENERATED: catalog-docs/split-button -->
# Split Button

- **Category:** input
- **Purpose:** Primary action plus a popup menu of secondary commands.
- **API reference:** [FS.Skia.UI.Controls.SplitButton](../reference/fs-skia-ui-controls-typed-splitbutton.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/split-button -->

## Overview

The **Split Button** control is a `input`-category control in the FS.Skia.UI typed front door.
Primary action plus a popup menu of secondary commands. It requires the `text` attribute. It raises `onClick`, `onSelected`. Its accessibility role is `Menu`.

Build it through the `FS.Skia.UI.Controls.Typed.SplitButton` module — the typed Props/MVU front door is the
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

![Split Button render-only preview](../img/controls/split-button.png)

A deterministic **render-only** preview of **Split Button** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
