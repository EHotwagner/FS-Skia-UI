# Phase 1 Data Model: Build Front-End + MEL Engine + Relocated Validators

Entities are **relocated, not redesigned** — the types below already exist in `build.fsx`; this
feature moves them into `FS.Skia.UI.Build` with curated `.fsi` surfaces and behaviour-identical
logic. Field lists mirror the current `build.fsx` definitions.

## 1. MEL engine (build-side MVU)

### `BuildModel` (state) — `Engine/Model.fs`
The durable build state: a record of repository-derived paths and progress. Fields (from
`build.fsx:197–234`): `RepositoryRoot`, `FeatureId`, `FeatureDir`, `ReadinessDir`, `LogDir`,
`FsiDir`, `SampleSmokeDir`, `PackageEvidenceDir`, `SurfaceBaselineDir`, `LocalPackageDir`,
`TemplateArtifactDir`, `TemplateWorkDir`, `TemplateEvidenceDir`, `GeneratedFileListsDir`,
`GeneratedProductVerifyDir`, `GeneratedProductRootsDir`, `PackageSurfaceReportDir`,
`CapabilityCatalogPath`, `CapabilityCatalogReportPath`, `SelectedSkillsReportPath`,
`DependencyReportPath`, `GeneratedGuidanceReportPath`, `TemplateDriftReportPath`,
`ProcessHealthPath`, `BootstrapRunnerPath`, `VerificationVerdictsPath`, `FocusedGatesReportPath`,
`TargetMetadataReportPath`, `TargetMetadataDriftReportPath`, `GovernanceScannersPath`,
`StaleBoundaryScanPath`, `GeneratedProductValidationPath`, `EvidenceGraphReportPath`,
`EvidenceAuditReportPath`, `DeferralsPath`, `CompletedTargets: string list`.
- **`init : root:string -> BuildModel * BuildEffect list`** — derive the path model from the
  repository root and emit any startup effects. Pure given `root`.

### `BuildMsg` (events) — `Engine/Model.fs`
`StartTarget of Targets.Target` | `TargetCompleted of string` | `TargetFailed of string * string`
| `ProcessHealthCollected of ProcessHealthSnapshot` | `BootstrapValidated of BootstrapValidation`
| `VerificationVerdictWritten of VerificationVerdict` | `FocusedGateCompleted of FocusedGateContract`.
**Note**: `StartTarget` carries the typed `Targets.Target` (no stringly-typed dispatch).

### `BuildEffect` (requested I/O, executed only at the edge) — `Engine/Model.fs`
The ~35-case effect DU (from `build.fsx:244–281`): directory/process/dotnet effects
(`EnsureDirectory`, `CleanDirectoryContents`, `RunProcess`, `RunDotnetAction`), template effects
(`InstallTemplate`, `InstantiateTemplates`, `ScanGeneratedProjects`, `ValidateTemplatePackage`),
generated-product effects (`GenerateV3Products`, `ScanV3GeneratedProducts`,
`ValidateGeneratedConsumer`), governance checks (`CapabilityCatalogCheck`, `SkillCatalogCheck`,
`PackageSurfaceReport`, `DependencyOwnershipReport`, `GeneratedGuidanceScan`, `WorkflowSelfCheck`,
`SkillSyncGate`), preflight (`CollectProcessHealth`, `ValidateRunnerBootstrap`), verdict/report
writers (`WriteVerificationVerdict`, `WriteFocusedGateSummary`, `CheckFocusedGateAssumptions`,
`WriteStructuredReport`, `WriteStructuredJsonReport`, `WriteFile`, `RequireFiles`, `FailWith`),
regeneration (`RegenerateSkillTree`, `RegenerateConstitutionFragments`), routing (`RouteSelect`),
and evidence (`EvidenceGraphCheck`, `EvidenceAuditCheck`).

### `update` (pure transition) — `Engine/Update.fs`
**`update : BuildMsg -> BuildModel -> BuildModel * BuildEffect list`** — pure. Given `StartTarget t`
returns the effect list for target `t`. **No filesystem / git / process / write I/O.** This is the
function FR-007/FR-013 unit-tests with typed effect-list assertions.

### `interpret` (edge) + `runTarget` — `Engine/Interpret.fs`
**`interpret : root:string -> BuildEffect -> unit`** — the only module that performs I/O; each arm
calls a relocated library function or a local I/O helper and writes reports.
**`runTarget : Targets.Target -> unit`** — `init` → `update (StartTarget t)` → `interpret` over the
effect list. The exe's `Target.create` bodies call this.

### Supporting value types (move with the engine)
`ProcessHealthThreshold`, `ProcessHealthSnapshot`, `BootstrapValidation`, `VerificationVerdict`
(+ `VerificationVerdictCategory`), `FocusedGateContract`, `TemplateInstallSource`, `TemplateRow`,
`V3GeneratedRow` — relocate alongside the modules that consume them (`Preflight.fs` /
`GeneratedProduct.fs` / `Engine`).

## 2. Typed target dispatch (consumed, already exists)

- **`Targets.Target`** — the closed DU of all targets (already in `build/Governance/Targets.fs`).
- **`Targets.dispatchTargets : Target list`** — the registration driver; the front-end iterates it.
- **`Targets.targetDependencyRows : (string * string list) list`** — derived `==>` edges.
- **`Targets.name : Target -> string`** — the runnable FAKE name.
Relationship: a `Target` case unhandled anywhere in dispatch/`update` is a **compile error**
(registration completeness, R4).

## 3. Relocated validators (typed findings, behaviour-identical)

| Module | Relocated from `build.fsx` | Public entry (curated `.fsi`) | Returns |
|---|---|---|---|
| `GeneratedProduct.fs` | `scanGeneratedProjects`, `runGenerateV3Products`, `runScanV3GeneratedProducts`, `runGeneratedConsumerValidation` (~2052–3500) | `validate* : BuildModel -> ...` | `Findings.ValidationFinding list` + rendered report |
| `Guidance.fs` | `runGeneratedGuidanceScan` + markdown/skill-section scanners (~3635–4300) | `scanGuidance : BuildModel -> string -> ...` | `Findings.ValidationFinding list` + rendered report |
| `Preflight.fs` | `collectProcessHealth`, `validateRunnerBootstrap` (+ health/bootstrap types, ~118–162 / 1431–1800) | `collectProcessHealth`/`validateRunnerBootstrap : root -> ... ` | typed snapshot/validation + report |

All three reuse `Findings.ValidationFinding` (the uniform finding type from feature 041) and emit
**byte-identical** report text vs the current script (proven by parity diff + unit tests).

## 4. Golden target-output baseline (parity oracle)

- **Entity**: a captured set, per target, of the deterministic governance reports/artifacts produced
  by the **current `build.fsx` path** before relocation.
- **Location**: `specs/045-foundations-build-frontend/readiness/parity/<target>/baseline/` (captured)
  and `…/after/` (post-migration); the diff result recorded alongside.
- **Comparison rules**: see [contracts/parity-oracle.md](./contracts/parity-oracle.md) — normalize
  timestamps/paths/ordering; verdict+report for test-shelling targets; exclude the two
  pre-existing-RED gates with stash-control proof.

## 5. Launchers / toolchain (rewired)

- **`fake.sh`** — `dotnet run --project build/Build.fsproj -- "$@"` (no `dotnet fake`/restore).
- **`fake.cmd`** — `dotnet run --project build/Build.fsproj -- %*` (preserve `%ERRORLEVEL%`).
- **`.config/dotnet-tools.json`** — `fake-cli` removed.
Contract: [contracts/front-end-cli.md](./contracts/front-end-cli.md).
