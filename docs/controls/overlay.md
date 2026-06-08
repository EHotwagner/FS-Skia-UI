---
title: Overlay
category: Controls
categoryindex: 8
index: 47
description: Layered child content.
---

<!-- BEGIN GENERATED: catalog-docs/overlay -->
# Overlay

- **Category:** overlay
- **Purpose:** Layered child content.
- **API reference:** [FS.Skia.UI.Controls.Overlay](../reference/fs-skia-ui-controls-overlay.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/overlay -->

## Overview

The **Overlay** control is a `overlay`-category control in the FS.Skia.UI control suite.
Layered child content. It requires the `child` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `Dialog`.

Build it through the `FS.Skia.UI.Controls.Overlay` module — the typed Props/MVU front door is the
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

![Overlay render-only preview](../img/controls/overlay.png)

A deterministic **render-only** preview of **Overlay** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
