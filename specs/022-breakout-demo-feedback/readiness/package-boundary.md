# Package Boundary

status=ok

Package-boundary decisions:

- Scene shape construction and deterministic shape facts stay in
  `FS.Skia.UI.Scene`.
- Screenshot host capability and unsupported-host classification stay in
  `FS.Skia.UI.SkiaViewer`.
- Reusable evidence report construction stays in `FS.Skia.UI.Testing`.
- Generated default app profiles follow the report convention without adding an
  unselected Testing package dependency.
- Native window and screenshot effects remain at the viewer/host boundary.

Validation:

- `./fake.sh build -t PackageSurfaceCheck`
- `./fake.sh build -t TemplateCheck`
- generated `/tmp/fs-skia-ui-report-check` evidence command run
