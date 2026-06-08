---
title: Button
category: Controls
categoryindex: 8
index: 17
description: Pointer and keyboard activatable command.
---

<!-- BEGIN GENERATED: catalog-docs/button -->
# Button

- **Category:** input
- **Purpose:** Pointer and keyboard activatable command.
- **API reference:** [FS.Skia.UI.Controls.Button](../reference/fs-skia-ui-controls-button.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/button -->

## Overview

The **Button** control is a `input`-category control in the FS.Skia.UI control suite.
Pointer and keyboard activatable command. It requires the `text` attribute. It raises `onClick`. Its accessibility role is `Button`.

Build it through the `FS.Skia.UI.Controls.Button` module — the typed Props/MVU front door is the
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

![Button render-only preview](../img/controls/button.png)

A deterministic **render-only** preview of **Button** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
