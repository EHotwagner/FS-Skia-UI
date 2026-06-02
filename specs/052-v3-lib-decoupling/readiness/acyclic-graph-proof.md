# Acyclic package-graph proof (rich input runtime home)

command: inspection of `*.fsproj` `ProjectReference` edges.
artifact path: this file.
failure class: PackageGraphCycle.
next action: none — the chosen home introduces no back-edge.

## Why the rich input runtime cannot live in the lean `FS.Skia.UI.KeyboardInput` package

The rich runtime's `.fsi` opens `FS.Skia.UI.Scene` **and** `FS.Skia.UI.SkiaViewer.Host`, so it depends
on `SkiaViewer`. Observed edges:

- `SkiaViewer → KeyboardInput (lean) + Scene`
- `Elmish → SkiaViewer + Scene`
- `Scene → FSharp.Core only`

Placing the rich runtime in the lean `FS.Skia.UI.KeyboardInput` package (which is **upstream** of
`SkiaViewer`) would form the cycle `KeyboardInput → SkiaViewer → KeyboardInput`. Rejected.

## Chosen home — a new package downstream of SkiaViewer

`FS.Skia.UI.Input` (`src/Input/Input.fsproj`) references `Scene` + `SkiaViewer`:

- `Input → SkiaViewer → {Scene, KeyboardInput}`
- `Input → Scene`

No package references `Input` (only the `InteractiveViewer` sample and `Input.Tests` do), so there is
**no back-edge**. `Scene` stays FSharp.Core-only. The graph remains acyclic (FR-008, SC-005).

## Consumer work-list (verified, feature 052)

`src/Lib` consumers at the pin: `samples/InteractiveViewer`, `tests/Lib.Tests`, `tests/Parity.Tests`,
`tests/Package.Tests`. `samples/ParityGallery` already monolith-free. Outcome: InteractiveViewer →
`FS.Skia.UI.Input`; Lib.Tests + Parity.Tests off `Lib`; Package.Tests retained (Stage 5).
