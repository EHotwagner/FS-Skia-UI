# Generated Default Launch Readiness

## T008 Template Source Tests

Added governance source coverage proving the generated graphical template contains:

- `viewerOptions`
- `generatedHost`
- `MapKey = mapKey`
- `Tick = tick`
- `Viewer.runApp viewerOptions generatedHost`
- Explicit bounded smoke and scene evidence flags

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated graphical template source defaults to persistent viewer host"` passed.

## T011 Generated Product Validation Expectations

Added generated product validation coverage requiring:

- persistent launch source/wiring through `Viewer.runApp viewerOptions generatedHost`
- generated host options, key mapping, and tick mapping
- explicit bounded smoke and scene evidence flags remaining available outside the default path

Verification:

- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated graphical template source defaults to persistent viewer host"` passed.

## T016 Packed SkiaViewer Surface Exercise

Packed feature-local packages:

- `FS.Skia.UI.Scene.0.1.16-persistent.1.nupkg`
- `FS.Skia.UI.KeyboardInput.0.1.16-persistent.1.nupkg`
- `FS.Skia.UI.SkiaViewer.0.1.16-persistent.1.nupkg`

Public package exercise:

- Script: `specs/016-persistent-viewer-contract/readiness/packed-skia-viewer-contract.fsx`
- Transcript: `specs/016-persistent-viewer-contract/readiness/packed-skia-viewer-contract.txt`

Observed result:

```text
persistent-window=true
bounded-smoke=true
keyboard-input=true
renderer-mode=skia
status=ok
mode=persistent-window
window-opened=true
exit-path=true
```

## T017 Packed Generated App Host Exercise

Public package exercise:

- Script: `specs/016-persistent-viewer-contract/readiness/packed-generated-app-host.fsx`
- Transcript: `specs/016-persistent-viewer-contract/readiness/packed-generated-app-host.txt`

Observed result:

```text
model-count=1
render-effect=true
tick-dispatch=true
status=ok
mode=persistent-window
window-opened=true
exit-path=true
```

## T018 Generated Product Default Path Tests

Feature-local package validation:

- Packed `FS.Skia.UI.*.0.1.16-persistent.1` packages under `specs/016-persistent-viewer-contract/readiness/package/`.
- Updated `template/base/Directory.Packages.props` to consume the feature-local package version.

Verification:

- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj /p:RestoreSources="/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/package;https://api.nuget.org/v3/index.json"` passed with 10 tests.
- `dotnet run --project template/base/src/Product/Product.fsproj --no-restore /p:RestoreSources="/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/package;https://api.nuget.org/v3/index.json"` wrote `generated-default-launch-run.txt`.

Observed default executable result:

```text
status=ok mode=persistent-window window-opened=true input-dispatch=not-applicable exit-path=true renderer-mode=skia
```

Real launch evidence:

- `readiness/supported-host-persistent-launch.txt` proves the generated app opened a real persistent Silk.NET window on `DISPLAY=:1`, rendered model-derived state, dispatched declared keyboard input, and closed intentionally.

## T019-T022 Implementation Notes

Implemented the public persistent API shell and generated template wiring:

- `Viewer.run`
- `Viewer.runApp`
- `Viewer.runtimeCapability`
- `ViewerLaunchOutcome`
- generated `viewerOptions`
- generated `generatedHost`
- generated default executable `Viewer.runApp viewerOptions generatedHost`

Current status:

- `Viewer.run` and `Viewer.runApp` route through the real Silk.NET persistent window lifecycle.
- Default generated app launch is wired through `Viewer.runApp viewerOptions generatedHost`.

Verification retained:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed with 18 tests.
- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --filter "generated graphical app default executable path uses persistent host"` passed.
- Supported-host launch artifact: `readiness/supported-host-persistent-launch.txt`.

## T023 Independent Validation Path

Command:

```bash
dotnet run --project template/base/src/Product/Product.fsproj --no-restore /p:RestoreSources="/home/developer/projects/FS-Skia-UI/specs/016-persistent-viewer-contract/readiness/package;https://api.nuget.org/v3/index.json"
```

Expected persistent-window mode:

- `mode=persistent-window`
- `window-opened=true`
- `input-dispatch=true` for keyboard-capable generated profiles
- `exit-path=true` after intentional user close

Current status:

- The command is wired to the persistent host.
- T024 captured real supported-host native window evidence with model-derived rendering, keyboard input dispatch, and intentional exit.
