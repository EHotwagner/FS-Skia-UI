---
title: Text Box
category: Controls
categoryindex: 2
index: 19
description: Plain single-line text entry.
---

<!-- BEGIN GENERATED: catalog-docs/text-box -->
# Text Box

- **Category:** input
- **Purpose:** Plain single-line text entry.
- **API reference:** [FS.Skia.UI.Controls.TextBox](../reference/fs-skia-ui-controls-textbox.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/text-box -->

## Overview

The **Text Box** control is a `input`-category control in the FS.Skia.UI control suite.
Plain single-line text entry. It requires the `value` attribute. It raises `onChanged`. Its accessibility role is `TextBox`.

Build it through the `FS.Skia.UI.Controls.TextBox` module — the typed Props/MVU front door is the
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

![Text Box render-only preview](../img/controls/text-box.png)

A deterministic **render-only** preview of **Text Box** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
