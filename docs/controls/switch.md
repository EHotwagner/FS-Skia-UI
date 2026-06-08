---
title: Switch
category: Controls
categoryindex: 2
index: 24
description: Compact Boolean setting.
---

<!-- BEGIN GENERATED: catalog-docs/switch -->
# Switch

- **Category:** selection
- **Purpose:** Compact Boolean setting.
- **API reference:** [FS.Skia.UI.Controls.Switch](../reference/fs-skia-ui-controls-switch.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/switch -->

## Overview

The **Switch** control is a `selection`-category control in the FS.Skia.UI control suite.
Compact Boolean setting. It has no required attributes — every attribute is optional. It raises `onChanged`. Its accessibility role is `CheckBox`.

Build it through the `FS.Skia.UI.Controls.Switch` module — the typed Props/MVU front door is the
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

![Switch render-only preview](../img/controls/switch.png)

A deterministic **render-only** preview of **Switch** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
