---
title: Label
category: Controls
categoryindex: 8
index: 12
description: Short form label text.
---

<!-- BEGIN GENERATED: catalog-docs/label -->
# Label

- **Category:** display
- **Purpose:** Short form label text.
- **API reference:** [FS.Skia.UI.Controls.Label](../reference/fs-skia-ui-controls-label.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/label -->

## Overview

The **Label** control is a `display`-category control in the FS.Skia.UI control suite.
Short form label text. It requires the `text` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `StaticText`.

Build it through the `FS.Skia.UI.Controls.Label` module — the typed Props/MVU front door is the
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

![Label render-only preview](../img/controls/label.png)

A deterministic **render-only** preview of **Label** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
