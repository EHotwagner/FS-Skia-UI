# ADR 0001 — Governance-library placement and distribution (D1)

- **Status**: Accepted
- **Date**: 2026-05-31
- **Decision source**: foundations plan
  (`docs/reports/2026-05-31-1049-foundations-implementation-plan.md`) and
  `specs/039-foundations-baseline-spike/` (research R4, plan §Project Structure).
  This ADR *records* a decision resolved with the maintainer during planning.

## Context

The foundations programme extracts the governance/validation logic that
currently lives inline in the 4,688-line `build.fsx` into a reusable compiled F#
library. Two questions must be settled before any extraction: **where** the
library project lives in the repo, and **how** it is eventually distributed to
generated consumer products. The repo already ships eight runtime packages from
`src/**`, all swept by the runtime surface-baseline tooling
(`PackageSurfaceCheck`, `FsiTranscripts`); a governance/build-tooling library is
**not** a runtime package and must not be mistaken for one.

## Decision

1. **Placement:** the governance library project is
   `build/Governance/FS.Skia.UI.Build.fsproj`, under a new top-level `build/`
   root — **not** under `src/`. The repo build front-end project-references it
   in-solution.
2. **Distribution:** in-solution **project reference** now. Distribution to
   generated consumers via a **published NuGet package** is the D1 end-state but
   is **exercised only in Stage 4/5** — this feature creates the project; it does
   not pack or publish it.

## Alternatives considered

- **`src/Build/` (rejected):** risks being swept by runtime surface-baseline
  checks and read as a shipped runtime package, polluting the eight-package
  runtime surface contract.
- **Separate repository (rejected):** D1 chose in-solution project reference with
  a later published package; a separate repo adds cross-repo versioning and CI
  overhead with no benefit at this stage.
- **Pack-and-publish immediately (deferred, not rejected):** publishing is the
  end-state but is premature before the library has real content (Stage 4/5).

## Consequences / rationale

- Placing the library under `build/` keeps it out of the runtime package set and
  runtime surface-baseline tooling, while co-locating it with the front-end that
  drives it.
- The library still carries a curated `.fsi` per Principle II — visibility lives
  in the signature even for build tooling.
- Deferring distribution avoids freezing a package identity/version contract
  before the surface is real.

## Stages shaped

- **Stage 4** (Python evidence-engine port) populates this library and proves
  parity against the golden fixtures.
- **Stage 4/5** introduces packing/publishing and template/generated-consumer
  consumption of `FS.Skia.UI.Build` (out of scope for feature 039).

## Verification in feature 039

The project exists at the decided path and builds clean
(`dotnet build build/Governance/FS.Skia.UI.Build.fsproj -warnaserror` →
`0 warnings, 0 errors`); it is **not** packed or published.

## Addendum (feature 064, 2026-06-04) — distribution is now implemented

The Stage-4/5 deferral of pack-and-publish recorded above is **superseded**.
Feature `064-publish-nuget-distribution` realizes the D1 end-state: a published
NuGet.org distribution of all 11 `FS.Skia.UI.*` libraries **and** the
`FS.Skia.UI.Template` package, consumable by an external developer who never
cloned this repo.

What changed:

- **`Publish` target** (`build/Governance/`): feed-agnostic
  (`FSSKIA_PUBLISH_FEED`, default nuget.org), idempotent (`dotnet nuget push
  --skip-duplicate`), and dry-runnable (credential-free anonymous feed read →
  12-row push/skip plan).
- **`PrePublishCheck` target** + `PrePublish.fs(i)`: aborts a malformed release
  naming the offending package/field (pin parity, engine-pin match, no
  machine-local path in the emitted config, required metadata).
- **Fresh-consumer config**: the generated `NuGet.config` is **public-feed only**
  (no machine-local path); the in-repo dev loop keeps `~/.local/share/nuget-local`
  via a separate validation overlay.
- **Single-source pin**: one `<FsSkiaUiVersion>` in the template
  `Directory.Packages.props` drives every library pin **and** the `build.fsx`
  engine reference (resolved at runtime), so a consumer upgrade is one edit.
- **Package metadata** (FR-010): `RepositoryUrl` + per-package READMEs on every
  packable project and the template package.

`FS.Skia.UI.Build` is therefore now a **published** package (still under `build/`,
still curated `.fsi`), not merely an in-solution project reference. See
`docs/distribution.md` for the consumer install/upgrade flow and the maintainer
release sequence.
