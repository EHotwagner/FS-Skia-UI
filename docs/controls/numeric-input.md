---
title: Numeric Input
category: Controls
categoryindex: 8
index: 21
description: Model-owned numeric value editor.
---

<!-- BEGIN GENERATED: catalog-docs/numeric-input -->
# Numeric Input

- **Category:** input
- **Purpose:** Model-owned numeric value editor.
- **API reference:** [FS.Skia.UI.Controls.NumericInput](../reference/fs-skia-ui-controls-numericinput.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/numeric-input -->

## Overview

The **Numeric Input** control is a `input`-category control in the FS.Skia.UI control suite.
Model-owned numeric value editor. It requires the `value` attribute. It raises `onChanged`. Its accessibility role is `TextBox`.

Build it through the `FS.Skia.UI.Controls.NumericInput` module — the typed Props/MVU front door is the
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

![Numeric Input render-only preview](../img/controls/numeric-input.png)

A deterministic **render-only** preview of **Numeric Input** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
