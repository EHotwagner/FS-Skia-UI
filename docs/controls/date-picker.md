---
title: Date Picker
category: Controls
categoryindex: 8
index: 58
description: Typed date entry with a popup calendar.
---

<!-- BEGIN GENERATED: catalog-docs/date-picker -->
# Date Picker

- **Category:** input
- **Purpose:** Typed date entry with a popup calendar.
- **API reference:** [FS.Skia.UI.Controls.DatePicker](../reference/fs-skia-ui-controls-typed-datepicker.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/date-picker -->

## Overview

The **Date Picker** control is a `input`-category control in the FS.Skia.UI typed front door.
Typed date entry with a popup calendar. It has no required attributes — every attribute is optional. It raises `onChange`. Its accessibility role is `TextBox`.

Build it through the `FS.Skia.UI.Controls.Typed.DatePicker` module — the typed Props/MVU front door is the
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

![Date Picker render-only preview](../img/controls/date-picker.png)

A deterministic **render-only** preview of **Date Picker** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
