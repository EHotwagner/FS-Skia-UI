---
title: Progress Bar
category: Controls
categoryindex: 8
index: 48
description: Determinate progress indicator.
---

<!-- BEGIN GENERATED: catalog-docs/progress-bar -->
# Progress Bar

- **Category:** feedback
- **Purpose:** Determinate progress indicator.
- **API reference:** [FS.Skia.UI.Controls.ProgressBar](../reference/fs-skia-ui-controls-progressbar.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/progress-bar -->

## Overview

The **Progress Bar** control is a `feedback`-category control in the FS.Skia.UI control suite.
Determinate progress indicator. It requires the `value` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `Progress`.

Build it through the `FS.Skia.UI.Controls.ProgressBar` module — the typed Props/MVU front door is the
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

![Progress Bar render-only preview](../img/controls/progress-bar.png)

A deterministic **render-only** preview of **Progress Bar** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
