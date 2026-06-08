---
title: Text Area
category: Controls
categoryindex: 8
index: 20
description: Plain multi-line text entry.
---

<!-- BEGIN GENERATED: catalog-docs/text-area -->
# Text Area

- **Category:** input
- **Purpose:** Plain multi-line text entry.
- **API reference:** [FS.Skia.UI.Controls.TextArea](../reference/fs-skia-ui-controls-textarea.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/text-area -->

## Overview

The **Text Area** control is a `input`-category control in the FS.Skia.UI control suite.
Plain multi-line text entry. It requires the `value` attribute. It raises `onChanged`. Its accessibility role is `TextBox`.

Build it through the `FS.Skia.UI.Controls.TextArea` module — the typed Props/MVU front door is the
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

![Text Area render-only preview](../img/controls/text-area.png)

A deterministic **render-only** preview of **Text Area** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
