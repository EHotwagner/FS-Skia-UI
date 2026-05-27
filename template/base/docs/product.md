# Product Notes

This product is generated as an FS.Skia.UI V3 consumer. Product code should
reference selected capability packages and keep product evidence under the
product readiness paths.

Generated app profiles use `FS.Skia.UI.Controls` for ordinary controls, rich
text, chart controls, graph controls, and DataGrid. Keep product values and
messages in product source. Use `FS.Skia.UI.Controls.Elmish` only at the
program edge for adapter commands and subscriptions when Elmish integration is
selected.

Generated game samples reserve a named HUD/status region and a named gameplay
region. The default validation size is 1280x720 and the documented constrained
size is 640x480. Generated evidence commands must report HUD text bounds,
active gameplay bounds, overlap diagnostics, and whether the result is
`ReadableLayout`, `DeterministicRenderOnly`, or `UnsupportedLayoutInspection`.

Generated public-contract examples should use `Product.Program.view` as the
`FS.Skia.UI.Scene.Scene` returning function, `Product.Program.generatedHost` as
the generated host value, and `Product.Program.update` for reducer checks.

Users migrating from the legacy Charts package should replace chart, graph, and
DataGrid authoring with Controls declarations. Generated products do not
include a compatibility shim.
