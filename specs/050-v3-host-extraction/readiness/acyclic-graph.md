# Package-graph standing invariants (FR-010 / FR-013 / SC-006; carried invariant 7)

Reference graph after Stage 1:

- `FS.Skia.UI.SkiaViewer -> { FS.Skia.UI.Scene, FS.Skia.UI.KeyboardInput }` + native packages
  (Silk.NET.*, SkiaSharp*, Fable.Elmish). **No** `SkiaViewer -> Lib` edge (removed; leak closed).
- `FS.Skia.UI.Scene -> ∅` (FSharp.Core-only; no `ProjectReference`/`PackageReference` in
  `Scene.fsproj`). **No** `Scene -> SkiaViewer` back-edge.
- The added Paint/Path functions + the `Scene.diagnostics` image check are pure (no SkiaSharp);
  `Scene` stays FSharp.Core-only.
- `FS.Skia.UI` (Lib, retiring) now references `Scene` + `SkiaViewer` for its residual rich
  `KeyboardInput` module. `Lib -> SkiaViewer -> Scene` is acyclic; nothing references `Lib` for
  host/scene any more (samples/tests repointed). `Governance.Tests` keeps a reduced `Lib` reference
  for `AgentValidation`; `InteractiveViewer`/`Lib.Tests` keep `Lib` for the rich `KeyboardInput`.

The package graph remains **acyclic**. No new `PackageVersion` was added outside
`Directory.Packages.props`. No FCS / dynamic compilation / runtime script-loading was introduced by
the host move (the host is the same Silk.NET/SkiaSharp Vulkan presenter, relocated and retyped).
