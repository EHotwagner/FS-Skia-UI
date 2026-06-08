---
title: Validation Message
category: Controls
categoryindex: 2
index: 50
description: Validation text tied to model state.
---

<!-- BEGIN GENERATED: catalog-docs/validation-message -->
# Validation Message

- **Category:** feedback
- **Purpose:** Validation text tied to model state.
- **API reference:** [FS.Skia.UI.Controls.ValidationMessage](../reference/fs-skia-ui-controls-validationmessage.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/validation-message -->

## Overview

The **Validation Message** control is a `feedback`-category control in the FS.Skia.UI control suite.
Validation text tied to model state. It requires the `text` attribute. It raises no events (it is a non-interactive, display-only control). Its accessibility role is `StaticText`.

Build it through the `FS.Skia.UI.Controls.ValidationMessage` module — the typed Props/MVU front door is the
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

![Validation Message render-only preview](../img/controls/validation-message.png)

A deterministic **render-only** preview of **Validation Message** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
