# XML Documentation Validation

command: `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter Asteroids`
scanned files: `src/*/*.fsi`, `src/*/*.fsproj`, generated XML documentation files, packed NuGet package entries.
observed: public `.fsi` declarations have XML documentation comments; generated XML docs are expected to be non-empty; packed package entries must include the corresponding XML file.
missing: none.
failure class: XmlDocumentationValidation.
next action: run `./fake.sh build -t PackLocal` and inspect packed XML entries after `.fsi` documentation changes.

| Project path | Package id | `.fsi` paths | Generated XML path | Packed artifact entry | Status |
|--------------|------------|--------------|--------------------|-----------------------|--------|
| `src/Lib/Lib.fsproj` | `FS.Skia.UI` | `src/Lib/Library.fsi` | `src/Lib/bin/Release/net10.0/FS.Skia.UI.xml` | `FS.Skia.UI.xml` | expected non-empty |
| `src/Scene/Scene.fsproj` | `FS.Skia.UI.Scene` | `src/Scene/Scene.fsi` | `src/Scene/bin/Release/net10.0/FS.Skia.UI.Scene.xml` | `FS.Skia.UI.Scene.xml` | expected non-empty |
| `src/SkiaViewer/SkiaViewer.fsproj` | `FS.Skia.UI.SkiaViewer` | `src/SkiaViewer/SkiaViewer.fsi`, `src/SkiaViewer/Host/Diagnostics.fsi`, `src/SkiaViewer/Host/OpenGl.fsi`, `src/SkiaViewer/Host/Viewer.fsi` | `src/SkiaViewer/bin/Release/net10.0/FS.Skia.UI.SkiaViewer.xml` | `FS.Skia.UI.SkiaViewer.xml` | expected non-empty |
| `src/Elmish/Elmish.fsproj` | `FS.Skia.UI.Elmish` | `src/Elmish/Elmish.fsi` | `src/Elmish/bin/Release/net10.0/FS.Skia.UI.Elmish.xml` | `FS.Skia.UI.Elmish.xml` | expected non-empty |
| `src/KeyboardInput/KeyboardInput.fsproj` | `FS.Skia.UI.KeyboardInput` | `src/KeyboardInput/KeyboardInput.fsi` | `src/KeyboardInput/bin/Release/net10.0/FS.Skia.UI.KeyboardInput.xml` | `FS.Skia.UI.KeyboardInput.xml` | expected non-empty |
| `src/Input/Input.fsproj` | `FS.Skia.UI.Input` | `src/Input/KeyboardInput.fsi` | `src/Input/bin/Release/net10.0/FS.Skia.UI.Input.xml` | `FS.Skia.UI.Input.xml` | expected non-empty |
| `src/Layout/Layout.fsproj` | `FS.Skia.UI.Layout` | `src/Layout/Layout.fsi`, `src/Layout/Types.fsi`, `src/Layout/Graph.fsi`, `src/Layout/GraphValidation.fsi` | `src/Layout/bin/Release/net10.0/FS.Skia.UI.Layout.xml` | `FS.Skia.UI.Layout.xml` | expected non-empty |
| `src/Controls/Controls.fsproj` | `FS.Skia.UI.Controls` | `src/Controls/Accessibility.fsi`, `src/Controls/Attributes.fsi`, `src/Controls/Catalog.fsi`, `src/Controls/Charts.fsi`, `src/Controls/Collections.fsi`, `src/Controls/Control.fsi`, `src/Controls/ControlRuntime.fsi`, `src/Controls/CustomControl.fsi`, `src/Controls/DataGrid.fsi`, `src/Controls/Diagnostics.fsi`, `src/Controls/RichText.fsi`, `src/Controls/TextInput.fsi`, `src/Controls/Theme.fsi`, `src/Controls/Types.fsi` | `src/Controls/bin/Release/net10.0/FS.Skia.UI.Controls.xml` | `FS.Skia.UI.Controls.xml` | expected non-empty |
| `src/Controls.Elmish/Controls.Elmish.fsproj` | `FS.Skia.UI.Controls.Elmish` | `src/Controls.Elmish/ControlsElmish.fsi` | `src/Controls.Elmish/bin/Release/net10.0/FS.Skia.UI.Controls.Elmish.xml` | `FS.Skia.UI.Controls.Elmish.xml` | expected non-empty |
| `src/Testing/Testing.fsproj` | `FS.Skia.UI.Testing` | `src/Testing/Testing.fsi` | `src/Testing/bin/Release/net10.0/FS.Skia.UI.Testing.xml` | `FS.Skia.UI.Testing.xml` | expected non-empty |
