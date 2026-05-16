# Package Surface Smoke Evidence Plan

## Scope

US3 requires public contract evidence for Scene, SkiaViewer, Elmish,
KeyboardInput, Layout, Charts, and Testing.

## Commands

```bash
./fake.sh build -t CapabilityCheck
./fake.sh build -t DependencyReport
./fake.sh build -t PackLocal
./fake.sh build -t PackageSurfaceCheck
dotnet test tests/Scene.Tests/Scene.Tests.fsproj
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
dotnet test tests/Elmish.Tests/Elmish.Tests.fsproj
dotnet test tests/KeyboardInput.Tests/KeyboardInput.Tests.fsproj
dotnet test tests/Testing.Tests/Testing.Tests.fsproj
```

## Evidence Paths

- `specs/009-v3-modular-framework/readiness/capability-catalog.md`
- `specs/009-v3-modular-framework/readiness/dependency-report.md`
- `specs/009-v3-modular-framework/readiness/package-surfaces/index.md`
- `specs/009-v3-modular-framework/readiness/logs/*tests.txt`
- `readiness/surface-baselines/FS.Skia.UI.*.txt`

## FSI / Packed-Library Smoke

The pack output under the local NuGet feed is checked by `PackLocal`. Focused
package semantic tests exercise public `.fsi` contracts directly through
project references during this implementation stage.
