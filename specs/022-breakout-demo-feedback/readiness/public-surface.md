# Public Surface

status=ok

Tier 1 public surface changed in:

- `FS.Skia.UI.Scene`: circle and filled ellipse nodes, constructors, and shape
  evidence helpers.
- `FS.Skia.UI.SkiaViewer`: screenshot evidence request/result/status contract
  and explicit unsupported-host result path.
- `FS.Skia.UI.Testing`: shared key-value evidence report helper contract.

Validation:

- `./fake.sh build -t RefreshSurfaceBaselines`
- `./fake.sh build -t PackageSurfaceCheck`
- `dotnet test tests/Scene.Tests/Scene.Tests.fsproj --no-restore`
- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore`
- `dotnet test tests/Testing.Tests/Testing.Tests.fsproj --no-restore`
