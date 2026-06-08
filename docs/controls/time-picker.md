---
title: Time Picker
category: Controls
categoryindex: 8
index: 59
description: Typed time entry with hour and minute segments.
---

<!-- BEGIN GENERATED: catalog-docs/time-picker -->
# Time Picker

- **Category:** input
- **Purpose:** Typed time entry with hour and minute segments.
- **API reference:** [FS.Skia.UI.Controls.TimePicker](../reference/fs-skia-ui-controls-typed-timepicker.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/time-picker -->

## Overview

The **Time Picker** control is a `input`-category control in the FS.Skia.UI typed front door.
Typed time entry with hour and minute segments. It has no required attributes — every attribute is optional. It raises `onChange`. Its accessibility role is `TextBox`.

Build it through the `FS.Skia.UI.Controls.Typed.TimePicker` module — the typed Props/MVU front door is the
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

![Time Picker render-only preview](../img/controls/time-picker.png)

A deterministic **render-only** preview of **Time Picker** (320×160), produced through the
render-only evidence path (`Widget.render` → `SkiaViewer.captureScreenshotEvidence`,
`ViewerRenderTargetPng`) and validated decodable / non-1×1 / non-trivial via
`Testing.readPngArtifact`. It shows the control rendered against the default
`DesignTokens.Light` theme.

[← Back to the controls catalog](catalog.html)
