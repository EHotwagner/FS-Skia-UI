# Package Reference Material Evidence

Status: pass.

Generated reference root:
`specs/035-api-discovery-names/readiness/package/api-reference/`

Reference index:
`specs/035-api-discovery-names/readiness/package/api-reference/index.md`

Machine-readable report:
`specs/035-api-discovery-names/readiness/package/api-reference/report.json`

Generation command:
`dotnet fsi scripts/generate-package-api-reference.fsx`

Package-adjacent FAKE wiring:

- `PackLocal` runs `scripts/generate-package-api-reference.fsx` after package
  creation and before the local package report.
- `PackageSurfaceCheck` runs `scripts/generate-package-api-reference.fsx`
  before package API reference and surface tests.

Focused evidence:

- `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`
  passed: 32 tests.
- `./fake.sh build -t PackageSurfaceCheck` passed after reference-generation
  wiring.

| Package | Version | Source `.fsi` inputs | Reference output | Symbol count | Sampled symbols | Omitted-symbol reasons | Diagnostics |
|---------|---------|----------------------|------------------|--------------|-----------------|------------------------|-------------|
| `FS.Skia.UI.Scene` | `local` | `src/Scene/Scene.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Scene.md` | 339 | `type Rect =`; `Width: float`; `Height: float`; `LinearGradient of startPoint: Point * endPoint: Point * colors: Color list`; `DropShadow of dx: float * dy: float * blur: float * color: Color`; `TextRun`; `SceneElementKind` | `none` | `none` |
| `FS.Skia.UI.SkiaViewer` | `local` | `src/SkiaViewer/SkiaViewer.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.SkiaViewer.md` | 439 | `type ViewerOptions =`; `InitialSize: Size`; `ViewerWindowPosition`; `Coordinates of x: int * y: int` | `none` | `none` |
| `FS.Skia.UI.Elmish` | `local` | `src/Elmish/Elmish.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Elmish.md` | 19 | none | `none` | `none` |
| `FS.Skia.UI.KeyboardInput` | `local` | `src/KeyboardInput/KeyboardInput.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.KeyboardInput.md` | 73 | `KeyboardModel`; `KeyboardEvent`; `KeyDown`; `KeyUp` | `none` | `none` |
| `FS.Skia.UI.Layout` | `local` | `src/Layout/Layout.fsi`; `src/Layout/Types.fsi`; `src/Layout/Graph.fsi`; `src/Layout/GraphValidation.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Layout.md` | 235 | none | `none` | `none` |
| `FS.Skia.UI.Controls` | `local` | `src/Controls/Accessibility.fsi`; `src/Controls/Attributes.fsi`; `src/Controls/Catalog.fsi`; `src/Controls/Charts.fsi`; `src/Controls/Collections.fsi`; `src/Controls/Control.fsi`; `src/Controls/ControlRuntime.fsi`; `src/Controls/CustomControl.fsi`; `src/Controls/DataGrid.fsi`; `src/Controls/Diagnostics.fsi`; `src/Controls/RichText.fsi`; `src/Controls/TextInput.fsi`; `src/Controls/Theme.fsi`; `src/Controls/Types.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Controls.md` | 686 | `type Control<'msg>`; `KnownControl.TextBlock`; `StandardAttributeName.VisibleRange`; `DataGrid.create`; `LineChart.series`; `TextBox.onChanged` | `none` | `none` |
| `FS.Skia.UI.Controls.Elmish` | `local` | `src/Controls.Elmish/ControlsElmish.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Controls.Elmish.md` | 29 | none | `none` | `none` |
| `FS.Skia.UI.Testing` | `local` | `src/Testing/Testing.fsi` | `specs/035-api-discovery-names/readiness/package/api-reference/FS.Skia.UI.Testing.md` | 279 | none | `none` | `none` |

No-reflection confirmation:

- Reference generation reads curated `.fsi` files only.
- `index.md` records `generated-from: curated-fsi`,
  `assembly-reflection: false`, and
  `repository-source-authoring-fallback: false`.
- `PackageApiReferenceTests.fs` validates the index, per-package reference
  files, source-shaped samples, XML summary preservation, omitted-symbol
  reasons, unsupported-symbol diagnostics, and no-reflection metadata.
