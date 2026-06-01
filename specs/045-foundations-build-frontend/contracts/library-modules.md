# Contract: Curated `.fsi` Surfaces for the Relocated Modules

Per Principle II, every relocated public module ships a curated `.fsi` that is the **sole**
declaration of its public surface. These are **build-tooling** surfaces (under `build/Governance/`)
and are **not** product surface — they do not appear in product surface baselines and do not change
`PackageSurfaceCheck`/`FsiTranscripts` product diffs. Signatures below are indicative; exact shapes
are finalized against the live `build.fsx` types during implementation (behaviour-identical).

## `Engine/Model.fsi`

```fsharp
namespace FS.Skia.UI.Build.Engine

type BuildModel = { /* repository-derived paths + CompletedTargets, as in data-model.md */ }

type BuildMsg =
    | StartTarget of FS.Skia.UI.Build.Targets.Target
    | TargetCompleted of string
    | TargetFailed of string * string
    | ProcessHealthCollected of Preflight.ProcessHealthSnapshot
    | BootstrapValidated of Preflight.BootstrapValidation
    | VerificationVerdictWritten of VerificationVerdict
    | FocusedGateCompleted of FocusedGateContract

type BuildEffect = /* the ~35-case effect DU, relocated verbatim */

val init : root: string -> BuildModel * BuildEffect list
```

## `Engine/Update.fsi` — the pure boundary

```fsharp
namespace FS.Skia.UI.Build.Engine

/// Pure transition. MUST expose no I/O. Given a Msg + Model, returns the next
/// Model and the requested effects; interpretation happens only in Interpret.
val update : BuildMsg -> BuildModel -> BuildModel * BuildEffect list
```

The `.fsi` deliberately omits any filesystem/git/process symbol so `update`'s body cannot perform
I/O and compile against this surface (Principle IV enforced by the compiler).

## `Engine/Interpret.fsi` — the edge

```fsharp
namespace FS.Skia.UI.Build.Engine

/// Executes one effect against the real filesystem/git/process. The only module
/// that performs I/O.
val interpret : root: string -> BuildEffect -> unit

/// init -> update (StartTarget t) -> interpret over the emitted effects.
val runTarget : FS.Skia.UI.Build.Targets.Target -> unit
```

## `GeneratedProduct.fsi`

```fsharp
namespace FS.Skia.UI.Build

module GeneratedProduct =
    /// Behaviour-identical relocation of the ~800-line generated-product
    /// structural validation. Returns typed findings; the report text is
    /// byte-identical to the current build.fsx output. NO schema_version /
    /// deprecation window (Stage 6.4, out of scope).
    val validateGeneratedConsumer : model: Engine.BuildModel -> Findings.ValidationFinding list
    val scanGeneratedProjects : model: Engine.BuildModel -> outputPath: string -> Findings.ValidationFinding list
    // + generateV3Products / scanV3GeneratedProducts entries as relocated
```

## `Guidance.fsi`

```fsharp
namespace FS.Skia.UI.Build

module Guidance =
    /// Generated-guidance / skill-section scanners (~200 lines), behaviour-preserving.
    val scanGeneratedGuidance : model: Engine.BuildModel -> outputPath: string -> Findings.ValidationFinding list
```

## `Preflight.fsi`

```fsharp
namespace FS.Skia.UI.Build

module Preflight =
    type ProcessHealthThreshold = { /* relocated verbatim */ }
    type ProcessHealthSnapshot = { /* relocated verbatim */ }
    type BootstrapValidation = { /* relocated verbatim */ }

    /// Process-health / bootstrap preflight (~267 lines), behaviour-preserving.
    val collectProcessHealth : root: string -> target: string -> outputPath: string -> verdictPath: string -> ProcessHealthSnapshot
    val validateRunnerBootstrap : root: string -> target: string -> outputPath: string -> verdictPath: string -> BootstrapValidation
```

## Constraints (all modules)

- Compile clean under `net10.0` / `TreatWarningsAsErrors` / `FS0078`-as-error; inherit
  `Directory.Build.props`; no `PackageVersion` outside `Directory.Packages.props` (FR-015).
- No `private`/`internal`/`public` modifiers in the `.fs`; visibility is the `.fsi` (Principle II).
- No `FSharp.Compiler.*` reference; no runtime script loading (FR-004).
- Reuse `Findings.ValidationFinding` for typed results (uniform finding type, feature 041).
