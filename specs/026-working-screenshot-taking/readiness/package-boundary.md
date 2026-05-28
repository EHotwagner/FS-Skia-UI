# Package Boundary

Status: reviewed.

`FS.Skia.UI.SkiaViewer` owns screenshot capture and PNG production through the
viewer render-target path. `FS.Skia.UI.Testing` owns validation of screenshot
records and PNG artifacts. The Testing package now references the repository's
existing pinned `SkiaSharp` package to decode screenshot artifacts; no new
external package family was introduced.

Scene, layout, launch, persistent-launch, deterministic-scene, pixel-readback,
and screenshot evidence remain distinct package responsibilities.
