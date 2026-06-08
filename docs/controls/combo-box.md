---
title: Combo Box
category: Controls
categoryindex: 8
index: 29
description: Compact selection list.
---

<!-- BEGIN GENERATED: catalog-docs/combo-box -->
# Combo Box

- **Category:** selection
- **Purpose:** Compact selection list.
- **API reference:** [FS.Skia.UI.Controls.Collections](../reference/fs-skia-ui-controls-collections.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/combo-box -->

## Overview

The **Combo Box** control is a `selection`-category control in the FS.Skia.UI control suite.
Compact selection list. It requires the `items` attribute. It raises `onChanged`. Its accessibility role is `List`.

Build it through the `FS.Skia.UI.Controls.Collections` module — the typed Props/MVU front door is the
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

![Combo Box render-only preview](../img/controls/combo-box.png)

A deterministic **render-only** preview of **Combo Box** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
