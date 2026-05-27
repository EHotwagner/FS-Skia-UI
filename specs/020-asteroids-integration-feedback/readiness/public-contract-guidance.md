# Public Contract Guidance Evidence

## Status

T009 public contract FSI exercise passed. T019/T021 generated product and
governance guidance smoke paths passed.

## Commands

- `dotnet build src/Testing/Testing.fsproj --no-restore`
- `dotnet fsi --exec specs/020-asteroids-integration-feedback/readiness/public-contract-guidance.fsx`
- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --no-restore --logger "console;verbosity=minimal"`
- `dotnet test tests/Governance.Tests/Governance.Tests.fsproj --filter "Generated guidance hardening" --logger "console;verbosity=minimal"`

## Evidence

The FSI script constructs the public `FS.Skia.UI.Scene.LayoutEvidenceReport`
and `FS.Skia.UI.Testing.GeneratedLayoutValidation*` records through compiled
assemblies. It covers representative readable layout, deterministic-render-only
evidence, and unsupported layout inspection records.

```text
readable-proof=ReadableLayout text-bounds=1 gameplay-bounds=1
deterministic-proof=DeterministicRenderOnly render-hash=2d5653c59de8cf03e1e09865015164ade467beafe3450bfeeadf1bc07b958ff2
unsupported-proof=UnsupportedLayoutInspection reasons=1
validation-check-requires-readable=true accepted=true
```

Generated product smoke:

```text
Product.Tests: Passed 21 tests
Governance.Generated guidance hardening: Passed 12 tests
```

Source snippets exercised by tests:

```fsharp
let scene: FS.Skia.UI.Scene.Scene = { Nodes = [ Product.Program.view initialModel ] }
let host = Product.Program.generatedHost
let updated, _ = Product.Program.update NoOp initialModel
```
