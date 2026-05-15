# Template Source Inventory

Feature: `007-v2-template-packaging`

## Template-Owned Source

The V2 template owns the repository assets that a generated product needs to
restore, build, test, document, and continue using Spec Kit governance:

- `.config/dotnet-tools.json`
- `.template.config/template.json`
- `Directory.Build.props`
- `Directory.Packages.props`
- `FS-Skia-UI.sln` for the default profile
- `build.fsx`, `fake.sh`, and `fake.cmd`
- `src/Lib/**`
- `src/Charts/**` in the default profile
- `src/Layout/**` in the default profile
- `tests/Lib.Tests/**`
- `tests/Package.Tests/**`
- `tests/Governance.Tests/**`
- `tests/Charts.Tests/**`, `tests/Layout.Tests/**`, `tests/Parity.Tests/**`, and
  `tests/Smoke.Tests/**` in the default profile
- `samples/BasicViewer/**`
- optional visual/layout/chart sample projects in the default profile
- `docs/build.md`, `docs/testing.md`, `docs/evidence.md`,
  `docs/template-profile.md`, `docs/dependencies.md`, and `docs/speckit.md`
- `.specify/templates/**`, `.specify/presets/fsharp-opinionated/**`, and
  `.specify/workflows/speckit/workflow.yml`
- `readiness/surface-baselines/**`

## Minimal Profile Contents

The minimal profile keeps the core library, one basic sample, core tests,
package checks, governance tests, docs, Spec Kit assets, command wrappers, and
central dependency policy. It excludes optional layout, charts, parity, smoke,
visual galleries, historical feature specs, feature readiness evidence, and
template package build outputs.

## Generated-Product Exclusions

Generated products must not include:

- `specs/001-*` through `specs/007-*`
- `artifacts/**`
- `.template.package/**`
- `bin/**` and `obj/**`
- `.git/**`
- historical readiness evidence under `specs/**/readiness/**`
- local NuGet/template package outputs

## Placeholder Tokens

Template validation scans generated products for unreplaced template-only
tokens:

- `FS-Skia-UI`
- `FS.Skia.UI`
- `fs-skia-ui`
- `[FEATURE NAME]`
- `[###-feature-name]`
- `[DATE]`
- `$ARGUMENTS`

The scanner allows these tokens only in historical source artifacts that are
excluded from generated products or in docs that intentionally explain the
template contract.
