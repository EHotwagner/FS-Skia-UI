# Single-edit upgrade (US3, SC-004)

Changing the **one** `<FsSkiaUiVersion>` value in a generated project's
`Directory.Packages.props` and running `dotnet restore` upgrades **both** the library packages
**and** the build engine to the new version — no second edit.

## Setup

A minimal generated-like project: `Directory.Packages.props` with one `<FsSkiaUiVersion>` and a
`FS.Skia.UI.Scene` pin referencing `$(FsSkiaUiVersion)`; an app `.fsproj` with a CPM
`<PackageReference Include="FS.Skia.UI.Scene" />`. The engine path is resolved exactly as the
generated `build.fsx` does — `~/.nuget/packages/fs.skia.ui.build/<FsSkiaUiVersion>/lib/net10.0/`.

## Edit 1 — `<FsSkiaUiVersion>0.1.66-preview.1</FsSkiaUiVersion>`

```
Restored .../App.fsproj
lib resolved:  "fs.skia.ui.scene/0.1.66-preview.1"
engine path:   ~/.nuget/packages/fs.skia.ui.build/0.1.66-preview.1/lib/net10.0/FS.Skia.UI.Build.dll
```

## The single edit — `0.1.66-preview.1` → `0.1.67-preview.1`

(one value changed in `Directory.Packages.props`, then `dotnet restore`)

```
Restored .../App.fsproj
lib resolved:  "fs.skia.ui.scene/0.1.67-preview.1"
engine path:   ~/.nuget/packages/fs.skia.ui.build/0.1.67-preview.1/lib/net10.0/FS.Skia.UI.Build.dll (exists=true)
```

## Verdict

- The **library** (`FS.Skia.UI.Scene`) and the **build engine** (`FS.Skia.UI.Build`) both moved
  from `0.1.66-preview.1` to `0.1.67-preview.1` with **one** edit + `dotnet restore` — no second
  edit (SC-004).
- Exactly **one** literal `FS.Skia.UI` version value exists in the project (the
  `<FsSkiaUiVersion>` property); the build engine resolves from it at runtime — there is no
  `#r "nuget: FS.Skia.UI.Build, <version>"` literal. Asserted by `Feature064PublishTests.T022`.
- Pins stay **exact** (no floating ranges); preview vs stable is explicit in the value. The
  procedure ships in the generated project at `docs/UPGRADING.md` (FR-005).
