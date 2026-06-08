---
title: Tooltip
category: Controls
categoryindex: 2
index: 44
description: Auxiliary hover/focus explanation.
---

<!-- BEGIN GENERATED: catalog-docs/tooltip -->
# Tooltip

- **Category:** overlay
- **Purpose:** Auxiliary hover/focus explanation.
- **API reference:** [FS.Skia.UI.Controls.Tooltip](../reference/fs-skia-ui-controls-tooltip.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/tooltip -->

## Overview

The **Tooltip** control is a `overlay`-category control in the FS.Skia.UI control suite.
Auxiliary hover/focus explanation. It requires the `text` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `StaticText`.

Build it through the `FS.Skia.UI.Controls.Tooltip` module — the typed Props/MVU front door is the
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

![Tooltip render-only preview](../img/controls/tooltip.png)

A deterministic **render-only** preview of **Tooltip** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
