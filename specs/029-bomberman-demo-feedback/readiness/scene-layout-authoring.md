# Scene Layout Authoring

Status: complete.

Tasks: T042-T049
Captured: 2026-05-29T12:16:00+02:00

## Validation

Commands:

```text
dotnet test tests/Scene.Tests/Scene.Tests.fsproj --logger "console;verbosity=minimal"
dotnet test tests/Layout.Tests/Layout.Tests.fsproj --logger "console;verbosity=minimal"
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "generated app guidance|record|layout|Scene.Point|Product.Program.view" --logger "console;verbosity=minimal"
```

Results:

- Scene.Tests: 11 passed, 0 failed.
- Layout.Tests: 23 passed, 0 failed.
- Governance.Tests: 20 passed, 0 failed.

## Covered Authoring Categories

- Coordinates: `Scene.Point`, layout coordinates, gameplay region bounds, text positions, and vertex positions.
- Dimensions: `Scene.Size`, `Layout.LayoutSize`, `LayoutBounds`, output sizes, image dimensions.
- Diagnostics: viewer diagnostics, layout diagnostics, evidence diagnostics, validation diagnostics.
- State: app model state, viewer lifecycle state, layout workflow model, evidence workflow state.
- Positions: text positions, vertex positions, window startup positions, layout bounds positions.

## Dependency Boundary

Command:

```text
rg -n "SkiaViewer|KeyboardInput|Controls|Silk|SkiaSharp|Elmish" src/Scene src/Layout -g "*.fsproj" -g "*.fs" -g "*.fsi"
dotnet list src/Scene/Scene.fsproj package
dotnet list src/Layout/Layout.fsproj package
```

Result:

- No forbidden viewer, host, keyboard, controls, Silk, SkiaSharp, or Elmish references were found in Scene/Layout source.
- Scene package references: `FSharp.Core`.
- Layout package references: `FSharp.Core`, `Yoga.Net`.
