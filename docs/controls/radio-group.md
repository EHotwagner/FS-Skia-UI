---
title: Radio Group
category: Controls
categoryindex: 8
index: 23
description: Single selection from a visible option set.
---

<!-- BEGIN GENERATED: catalog-docs/radio-group -->
# Radio Group

- **Category:** selection
- **Purpose:** Single selection from a visible option set.
- **API reference:** [FS.Skia.UI.Controls.RadioGroup](../reference/fs-skia-ui-controls-radiogroup.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/radio-group -->

## Overview

The **Radio Group** control is a `selection`-category control in the FS.Skia.UI control suite.
Single selection from a visible option set. It requires the `items` attribute. It raises `onChanged`. Its accessibility role is `RadioGroup`.

Build it through the `FS.Skia.UI.Controls.RadioGroup` module — the typed Props/MVU front door is the
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

![Radio Group render-only preview](../img/controls/radio-group.png)

A deterministic **render-only** preview of **Radio Group** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
