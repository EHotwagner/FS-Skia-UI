# Contract: Spike Target (dedicated build front-end drives the governance library)

This is the verifiable contract proving **D2** — that a dedicated, compiled FAKE build front-end can reference and drive a compiled governance library in-process. (FR-005, FR-006, FR-007; SC-003, SC-004.)

## Interfaces under test

- **Governance library** `build/Governance/FS.Skia.UI.Build.fsproj`, public module `Spike` with a curated `Spike.fsi`:
  ```fsharp
  // Spike.fsi — the entire public surface for this feature
  module FS.Skia.UI.Build.Spike

  /// Trivial demonstration body invoked by the build front-end's spike target.
  /// Returns a stable, identifiable success message proving the body ran from the library.
  val run : unit -> string
  ```
- **Build front-end** `build/Build.fsproj` (Exe), `Program.fs` registers one target whose body is *only* a call into `Spike.run` (no inlined logic), run via `Fake.Core.Target.runOrDefault`.

## Invocation

```
dotnet run --project build/Build.fsproj -- SpikeHello
```

## Expected behaviour

| # | Given | When | Then | Maps to |
|---|---|---|---|---|
| 1 | a fresh checkout of this branch | both new projects are built | both compile clean under `net10.0` / `TreatWarningsAsErrors` with **zero warnings**, no `PackageVersion` declared outside `Directory.Packages.props` | AS-1, SC-003 |
| 2 | the front-end references the library | the `SpikeHello` target is invoked through `dotnet run` | the target executes and reports success, and the success line is the value returned by `Spike.run` (proving the body ran from the library, not inlined) | AS-2, FR-006 |
| 3 | the spike runs to a result | it completes | the outcome is recorded as exactly one of `"D2 confirmed"` or `"fallback triggered"` with a named, reproducible blocker | AS-3, FR-007, SC-004 |
| 4 | the restored package graph | `dotnet list build/Build.fsproj package --include-transitive` is inspected | **no** `FSharp.Compiler.Service` / `FSharp.Compiler.*` package is present | FR-012 |

## Confirm/fallback decision rule

- **`D2 confirmed`** ⟺ rows 1, 2, 4 all hold and row 2's target reached success.
- **`fallback triggered`** ⟺ any of rows 1, 2, 4 cannot be achieved; record the exact blocker and document the thin-`build.fsx` `#r`-the-DLL shim as the Stage 5 path.
- **Failure (not allowed)** ⟺ the run ends without a recorded confirm and without a reproducible blocker.

## Non-goals (explicitly out of scope here)

- Populating the library with real validators, the MEL engine, or any ported Python (Stages 3.2–5).
- Replacing `./fake.sh` / `build.fsx` or altering any existing target (FR-010, FR-011).
- Packaging or distributing `FS.Skia.UI.Build` to generated consumers (D1 end-state; Stage 4/5).
