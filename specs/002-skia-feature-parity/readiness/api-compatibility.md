# API Compatibility And Migration Notes

## Packages

- Core viewer APIs are provided by `FS.Skia.UI`.
- Chart and DataGrid APIs are provided by `FS.Skia.UI.Charts`.
- Layout and graph APIs are provided by `FS.Skia.UI.Layout`.

## Viewer Migration

Baseline observable scene/input streams are adapted to Elmish:

- Put application state in `Model`.
- Represent user and host events as `Msg`.
- Keep `update` pure.
- Request host work with `ViewerEffect`.
- Render only through `view : Model -> Scene`.

## Renderer Constraint

No fallback renderer selector is exposed. Vulkan startup, swapchain, Skia context, screenshot, frame, and shutdown failures are reported as structured diagnostics.

## Public Surface

Public modules use `.fsi` signatures. Refreshed surface baselines are stored under `readiness/surface-baselines/`.
