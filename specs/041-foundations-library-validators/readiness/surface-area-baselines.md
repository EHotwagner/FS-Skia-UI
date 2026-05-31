# Surface-area baselines — new build/Governance modules (T008)

New build-tooling modules (each with a curated `.fsi`, Principle II); NOT part of the tracked
runtime surface baselines (they live under `build/`, outside the `src/**` sweep):

| Module | `.fsi` public surface |
|--------|-----------------------|
| `FS.Skia.UI.Build.Findings` | `ValidationFinding`, `finding`, `renderDetail` |
| `FS.Skia.UI.Build.Targets` | `Target` (40-case DU), `TargetSpec`, `allTargets`, `dispatchTargets`, `spec`, `name`, `directPrerequisites`, `requiredTargetNames`, `targetDependencyRows` |
| `FS.Skia.UI.Build.TargetMetadata` | `TargetMetadata`, `TargetMetadataDrift`, `validateMetadataDrift`, `validateAgainstRepo`, `driftDiagnostic`, `metadataJson`, `driftMarkdown` |
| `FS.Skia.UI.Build.Capabilities` | `CapabilityRow`, `readCatalog`, `validateRows`, `renderReport` |

## Unsupported-scope handling (edge case 3 — Stage-5 trigger)

A missing library DLL reference at extraction time is surfaced explicitly, not silently
re-inlined. The FSX front-end `#load`s the four `.fs` (with their `.fsi`) ahead of the build
model types; if `YamlDotNet` were absent from the paket header the script would fail to
compile (`open YamlDotNet.Serialization` unresolved) — observed and resolved during wiring by
adding `nuget YamlDotNet 17.1.0` to the header and clearing `build.fsx.lock` to re-resolve.
This is the explicit Stage-5 trigger behavior (Principle VII): no silent inline fallback.

Authoritative command: `dotnet build build/Governance/FS.Skia.UI.Build.fsproj` (clean under
TreatWarningsAsErrors). Failure class: `governance / build-tooling-surface`. Next action: if a
`#load` or reference fails, surface it as the Stage-5 trigger; do not re-inline the validator.
