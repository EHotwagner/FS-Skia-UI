# Product Notes

This product is generated as an FS.Skia.UI V3 consumer. Product code should
reference selected capability packages and keep product evidence under the
product readiness paths.

Generated app profiles use `FS.Skia.UI.Controls` for ordinary controls, rich
text, chart controls, graph controls, and DataGrid. Keep product values and
messages in product source. Use `FS.Skia.UI.Controls.Elmish` only at the
program edge for adapter commands and subscriptions when Elmish integration is
selected.

Users migrating from the legacy Charts package should replace chart, graph, and
DataGrid authoring with Controls declarations. Generated products do not
include a compatibility shim.
