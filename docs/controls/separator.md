---
title: Separator
category: Controls
categoryindex: 8
index: 15
description: Visual divider between regions.
---

<!-- BEGIN GENERATED: catalog-docs/separator -->
# Separator

- **Category:** display
- **Purpose:** Visual divider between regions.
- **API reference:** [FS.Skia.UI.Controls.Separator](../reference/fs-skia-ui-controls-separator.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/separator -->

## Overview

The **Separator** control is a `display`-category control in the FS.Skia.UI control suite.
Visual divider between regions. It has no required attributes — every attribute is optional. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `StaticText`.

Build it through the `FS.Skia.UI.Controls.Separator` module — the typed Props/MVU front door is the
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

![Separator render-only preview](../img/controls/separator.png)

A deterministic **render-only** preview of **Separator** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
