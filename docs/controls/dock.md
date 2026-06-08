---
title: Dock
category: Controls
categoryindex: 8
index: 34
description: Docked region composition.
---

<!-- BEGIN GENERATED: catalog-docs/dock -->
# Dock

- **Category:** layout
- **Purpose:** Docked region composition.
- **API reference:** [FS.Skia.UI.Controls.Dock](../reference/fs-skia-ui-controls-dock.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/dock -->

## Overview

The **Dock** control is a `layout`-category control in the FS.Skia.UI control suite.
Docked region composition. It requires the `children` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `StaticText`.

Build it through the `FS.Skia.UI.Controls.Dock` module — the typed Props/MVU front door is the
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

![Dock render-only preview](../img/controls/dock.png)

A deterministic **render-only** preview of **Dock** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
