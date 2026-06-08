---
title: Custom Control
category: Controls
categoryindex: 2
index: 61
description: Product-owned wrapper for custom Skia content.
---

<!-- BEGIN GENERATED: catalog-docs/custom-control -->
# Custom Control

- **Category:** custom
- **Purpose:** Product-owned wrapper for custom Skia content.
- **API reference:** [FS.Skia.UI.Controls.CustomControl](../reference/fs-skia-ui-controls-customcontrol.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/custom-control -->

## Overview

The **Custom Control** control is a `custom`-category control in the FS.Skia.UI control suite.
Product-owned wrapper for custom Skia content. It has no required attributes — every attribute is optional. It raises `onCustom`. Its accessibility role is `Custom`.

Build it through the `FS.Skia.UI.Controls.CustomControl` module — the typed Props/MVU front door is the
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

![Custom Control render-only preview](../img/controls/custom-control.png)

A deterministic **render-only** preview of **Custom Control** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
