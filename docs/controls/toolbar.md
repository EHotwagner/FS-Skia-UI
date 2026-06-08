---
title: Toolbar
category: Controls
categoryindex: 8
index: 43
description: Compact command group.
---

<!-- BEGIN GENERATED: catalog-docs/toolbar -->
# Toolbar

- **Category:** navigation
- **Purpose:** Compact command group.
- **API reference:** [FS.Skia.UI.Controls.Toolbar](../reference/fs-skia-ui-controls-toolbar.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/toolbar -->

## Overview

The **Toolbar** control is a `navigation`-category control in the FS.Skia.UI control suite.
Compact command group. It requires the `children` attribute. It raises `onClick`. Its accessibility role is `Menu`.

Build it through the `FS.Skia.UI.Controls.Toolbar` module — the typed Props/MVU front door is the
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

![Toolbar render-only preview](../img/controls/toolbar.png)

A deterministic **render-only** preview of **Toolbar** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
