# Data Model: Publish FS.Skia.UI to NuGet.org

This feature is packaging/release tooling — its "entities" are build-engine values and release inputs, not
runtime domain types. The runtime MVU model is unchanged. The entities below live in the
`FS.Skia.UI.Build` front-end (`build/Governance/**`).

## PublishConfig

Release inputs, read from the environment **at the interpreter edge** (never committed). The pure `update`
receives it as data.

| Field | Type | Source / default | Notes |
|-------|------|------------------|-------|
| `FeedUrl` | `string` | env `FSSKIA_PUBLISH_FEED`, default `https://api.nuget.org/v3/index.json` | Parameterized so staging/private feed is config, not code (FR-001). |
| `ReadUrl` | `string` | derived from `FeedUrl` | Flat-container base for nuget.org; directory path for a local feed (R2). |
| `ApiKeyPresent` | `bool` | `FSSKIA_PUBLISH_API_KEY` set? | Real push fails fast if absent; dry-run never needs it (FR-002). |
| `DryRun` | `bool` | env/arg `FSSKIA_PUBLISH_DRYRUN` | Plan-only, no network push (FR-002). |
| `IsLocalFeed` | `bool` | `FeedUrl` is a directory path | Selects directory-listing vs flat-container read (R2). |

**Validation**: a non-dry-run with `ApiKeyPresent = false` aborts before any push, naming the missing key.

## PublishPlanRow (one per package; 12 rows)

Computed by the interpreter's anonymous feed read; rendered by dry-run and used to decide the push.

| Field | Type | Notes |
|-------|------|-------|
| `PackageId` | `string` | from `packProjects` (×11) + `FS.Skia.UI.Template`. |
| `Version` | `string` | from each `.fsproj` `<Version>` / template fsproj. |
| `FeedHasVersion` | `bool` | anonymous read result. |
| `Decision` | `Push \| Skip` | `Skip` when `FeedHasVersion`; idempotency (FR-002, SC-003). |

**Invariant**: exactly **12** rows (11 libs + template); a row count ≠ 12 is a gate failure.

## PrePublishFinding (pre-publish consistency, FR-006)

The pre-publish check produces a list of findings; a non-empty list **aborts** the publish.

| Field | Type | Notes |
|-------|------|-------|
| `Package` | `string` | offending package id (or `template`). |
| `Field` | `string` | offending field/pin (e.g. `RepositoryUrl`, `FsSkiaUiVersion`, `NuGet.config:local`). |
| `Rule` | `PinParity \| EnginePinMatch \| NoMachineLocalPath \| RequiredMetadata` | which check failed. |
| `Detail` | `string` | actionable message naming expected vs actual. |

**Rules (all must pass):**
1. **PinParity** — every `FS.Skia.UI.*` pin in `template/base/Directory.Packages.props` equals the version
   being shipped for that package (`packProjects` `<Version>`).
2. **EnginePinMatch** — `build.fsx`'s resolved engine version equals the shipped `FS.Skia.UI.Build`
   version (FR-004 single-source: both derive from `<FsSkiaUiVersion>`).
3. **NoMachineLocalPath** — the **consumer-emitted** `NuGet.config` (from `GeneratedProduct.fs`) contains
   no absolute local feed path (FR-003).
4. **RequiredMetadata** — every packable project **and** the template carry non-blank
   license / repository-url / authors / description / README (FR-010).

## VersionSource (single source of truth, FR-004)

| Field | Type | Notes |
|-------|------|-------|
| `Property` | `<FsSkiaUiVersion>` | defined once in `template/base/Directory.Packages.props`. |
| `LibraryPins` | `$(FsSkiaUiVersion)` ×11 | `<PackageVersion Include="FS.Skia.UI.*" Version="$(FsSkiaUiVersion)" />`. |
| `EngineReference` | runtime-read | `build.fsx` reads `Property` and binds the engine assembly (R1). |

**Invariant (generated project)**: exactly **one** literal `FS.Skia.UI` version value exists — the
`<FsSkiaUiVersion>` value. A second literal anywhere is a single-source defect (caught by EnginePinMatch).

## Target registry deltas

| Target | DirectPrerequisites | Routing tier | knownGates |
|--------|---------------------|--------------|------------|
| `PrePublishCheck` | `[ TemplateCheck ]` (composes pin-parity + metadata over the packed/template set) | distribution rule (FocusedAuthority+) | added |
| `Publish` | `[ PrePublishCheck; PackLocal; TemplatePack ]` | distribution rule | added |

Both targets are added to the `allTargets` registry (and thus the `TargetMetadata` rows + the generated
`validation.contract.yml`). The "metadata registry stays at N rows" comment near `Targets.fs:58` and any
registry-count test / `TargetMetadata` expectation must be bumped by **+2** in the same change — verify
the exact current count at implementation time (`Targets.fs` is the single source) rather than trusting a
hardcoded number here.
