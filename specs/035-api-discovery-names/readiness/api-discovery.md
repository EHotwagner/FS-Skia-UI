# API Discovery Evidence

Status: pass.

User-facing package discovery surfaces exercised:

- Source-shaped package reference index:
  `specs/035-api-discovery-names/readiness/package/api-reference/index.md`
- Per-package source-shaped references:
  `specs/035-api-discovery-names/readiness/package/api-reference/*.md`
- Package-shaped FSI authoring transcripts:
  `specs/035-api-discovery-names/readiness/fsi/scene-authoring.fsx`
  and `.log`
  `specs/035-api-discovery-names/readiness/fsi/viewer-keyboard-authoring.fsx`
  and `.log`
  `specs/035-api-discovery-names/readiness/fsi/controls-adjacent-authoring.fsx`
  and `.log`

Focused commands:

- `dotnet fsi specs/035-api-discovery-names/readiness/fsi/scene-authoring.fsx`
- `dotnet fsi specs/035-api-discovery-names/readiness/fsi/viewer-keyboard-authoring.fsx`
- `dotnet fsi specs/035-api-discovery-names/readiness/fsi/controls-adjacent-authoring.fsx`
- `dotnet test tests/Package.Tests/Package.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`

Results:

- All three FSI scripts completed and wrote `FSI transcript PASS` logs.
- `Package.Tests` passed: 32 tests.

Authoring shapes covered:

- Scene primitives and geometry:
  `Rect`, `Point`, `SceneElementKind.RectangleElement`
- Paint helpers:
  `Paint`, `Stroke`, `StrokeCap.Round`, `StrokeJoin.RoundJoin`,
  `BlendMode.SrcOver`
- Text and drawing samples:
  `TextRun`, `Font`, `Color`
- Viewer records and cases:
  `ViewerOptions`, `InitialSize`, `ViewerWindowPosition.Coordinates`
- Keyboard records and cases:
  `KeyboardModel`, `Keyboard.init`, `KeyboardMsg.KeyDown`,
  `KeyboardMsg.KeyUp`
- Controls front doors:
  `FS.Skia.UI.Controls.TextBlock.create`,
  `FS.Skia.UI.Controls.TextBox.onChanged`,
  `FS.Skia.UI.Controls.DataGrid.create`
- Controls.Elmish adapter:
  `ControlsElmish.diagnostic`

Package reference metadata:

See `specs/035-api-discovery-names/readiness/package-reference-material.md`
for package IDs, versions, source `.fsi` paths, generated reference paths,
sampled symbol counts, omitted-symbol reasons, diagnostics, and package
adjacent FAKE wiring.

No-reflection and no-source-inspection confirmation:

- FSI transcripts use `#r "nuget: ..."` package references.
- Transcript tests reject `#load`, `../src/`, `Assembly.Load`, and
  `GetExportedTypes`.
- Reference generation records `assembly-reflection: false` and
  `repository-source-authoring-fallback: false`.
