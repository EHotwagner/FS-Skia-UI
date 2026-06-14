---
title: Custom Control
category: Controls
categoryindex: 8
index: 61
description: Product-owned wrapper for custom Skia content.
---

<!-- BEGIN GENERATED: catalog-docs/custom-control -->
# Custom Control

- **Category:** custom
- **Purpose:** Product-owned wrapper; renderTree paints a labeled placeholder, not the custom Render/Draw content — build must-show geometry from primitive controls (Border/TextBlock/Stack).
- **API reference:** [FS.Skia.UI.Controls.CustomControl](../reference/fs-skia-ui-controls-customcontrol.html)

[← Back to the controls catalog](catalog.html)
<!-- END GENERATED: catalog-docs/custom-control -->

## Overview

The **Custom Control** control is a `custom`-category control in the FS.Skia.UI control suite.
Product-owned wrapper for custom Skia content. It has no required attributes — every attribute is optional. It raises `onCustom`. Its accessibility role is `Custom`.

Build it through the `FS.Skia.UI.Controls.CustomControl` module — the typed Props/MVU front door is the
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

_No render-only preview is committed for **Custom Control**._

<!-- preview-status: unsupported -->

**Custom Control** is a product-owned wrapper for arbitrary custom Skia content, so there is
no canonical sample state to depict through the deterministic render-only path. Rather than
commit a fabricated, placeholder, or 1×1 image, its preview is honestly declared
**unsupported** (FR-007): the control renders whatever product-defined content the consumer
supplies. The reconciled preview ledger counts it under *unsupported* (see
`specs/079-doc-preview-examples/readiness/controls-preview-evidence.md`).

[← Back to the controls catalog](catalog.html)
