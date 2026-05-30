# Contract: Generated Guidance

## Purpose

Generated product guidance must lead agents toward packaged source-shaped API
reference and stable qualification rules before authoring FS.Skia.UI code.

## Required Guidance

- Name the package API reference location or package validation report.
- Tell agents not to use assembly reflection or repository source inspection as
  the first authoring strategy.
- Include a compact source-shaped API map for Scene primitives, `Paint`,
  viewer host records, keyboard keys, and Controls front doors.
- State the mixed Scene/Controls rule: qualify collision-prone Scene records,
  Controls modules, event origins, and builder helpers explicitly when both
  namespaces are in scope.
- Include examples using stable names such as `FS.Skia.UI.Scene.Point`,
  `FS.Skia.UI.Scene.Rect`, `Scene.text`, `TextBlock.text`, `Button.onClick`,
  and `Controls.Control.dispatch` where ambiguity is possible.

## Acceptance

- `GeneratedGuidanceCheck` scans required terms and rejects stale guidance that
  recommends reflection, open-order dependence, or copied framework source.
- Generated products compile the mixed Scene/Controls sample from package
  references.
