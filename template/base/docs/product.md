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
Reducers return app commands such as `DispatchHostCommand`; generated host
boundaries turn updated models into viewer effects such as `RenderScene`.
Do not append viewer effects to app command lists or reuse one category name for
both effect kinds.

Viewer-backed profiles use `Viewer.runApp viewerOptions Product.Program.generatedHost`
as the persistent interactive launch contract. Deterministic scene evidence,
persistent launch evidence, and screenshot evidence are separate report kinds:
scene evidence does not prove a persistent window, and unsupported screenshot
capture must report `fallback=deterministic-scene-evidence` without claiming a
screenshot artifact. A report with `evidence-kind=screenshot` is screenshot
proof only when it records live viewer-window capture after first-frame
presentation; deterministic-scene-evidence must not claim screenshot proof.
Generated evidence commands use the `FS.Skia.UI.Testing.EvidenceReports`
convention for stable key ordering, stdout/file parity, parent-directory
creation, normalized status vocabulary, and unsupported-host reason/fallback
fields without forcing the default app profile to reference the Testing package.
Game entities reuse shared Scene geometry for layout, containment, collision,
and rendering evidence when the Scene model fits, rather than introducing local
duplicate bounds records.
If a generated product introduces its own geometry vocabulary, prefer
domain-specific names such as `WorldRect`, `WorldPoint`, `TrackBounds`,
`CarPose`, and `CheckpointBounds`. Keep generic `Rect`, `Point`, and `Size`
available for Scene and layout primitives.

For Linux desktop review sessions where the generated viewer should keep
running after the terminal closes, preserve launch diagnostics with:

```bash
setsid dotnet run --project src/Product/Product.fsproj > readiness/logs/product-viewer.log 2>&1 < /dev/null &
```

Keep the `readiness/logs/product-viewer.log` path with the review notes so
stdout, stderr, and startup facts remain available.

Users migrating from the legacy Charts package should replace chart, graph, and
DataGrid authoring with Controls declarations. Generated products do not
include a compatibility shim.
