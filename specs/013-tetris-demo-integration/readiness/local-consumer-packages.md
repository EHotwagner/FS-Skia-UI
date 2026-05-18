# Local Consumer Packages Readiness

## Scope

Readiness evidence for local package/feed guidance, package identity/version
reporting, consumer configuration snippets, restore commands, and setup-drift
diagnostics.

## Setup Notes

- Tier: Tier 1 contracted build/testing/guidance change.
- Affected areas: `build.fsx`, `scripts/dependency-report.fsx` if needed,
  generated guidance, package tests, and docs.
- Command-surface impact: `PackLocal`, `GeneratedGuidanceCheck`,
  `GeneratedProductCheck`, and possibly `DependencyReport`.
- Package identity constraint: package identities remain stable; stale or
  missing local feed contents must be reported as setup drift.
- Synthetic policy: stale package feed fixtures may be synthetic when
  disclosed; real readiness needs local package output and generated consumer
  guidance evidence.

## Evidence

- Focused Testing helper tests:
  `readiness/logs/testing-us5-local-consumer-tests.txt`
  - Verifies local package report includes feed path, package identities,
    versions, consumer `PackageReference` snippet, optional `NuGet.config`
    snippet, restore command, and generated consumer package set.
  - Verifies stale and missing package feed fixtures are classified as setup
    drift before generated consumer build/input/rendering failures.
  - Verifies drift diagnostics name `PackLocal` remediation.
- Real local package output:
  `readiness/logs/pack-local-us5.txt`
  - `./fake.sh build -t PackLocal` completed successfully.
  - Package inventory, versions, feed path, consumer snippets, restore command,
    expected local artifacts, and drift remediation were written to
    `readiness/package/local-packages.md`.
- Generated consumer validation:
  `readiness/generated-product-validation.md`
  - The generated consumer restored from `/home/developer/.local/share/nuget-local`.
  - The generated consumer used local package references rather than repository
    implementation source.

## Independent Validation

Run:

```bash
dotnet run --project tests/Testing.Tests/Testing.Tests.fsproj
./fake.sh build -t PackLocal
```

Generated consumers should restore from
`/home/developer/.local/share/nuget-local` with the package identities and
versions listed in `readiness/package/local-packages.md`.

## Requirement Mapping

- FR-015: local feed path, package identities, versions, consumer snippets, and
  restore command are written by `PackLocal`.
- FR-016: stale or missing local feed contents are reported as setup drift by
  `LocalConsumerPackages.classifyDrift`.
- FR-017: generated consumers can use the reported package set instead of
  repository implementation source.
- FR-019: package identity, expected version, actual version, feed path, and
  remediation command are named in diagnostics.
- SC-009: real `PackLocal` output is captured under the feature readiness
  package report.
- SC-010: generated consumer validation restored and tested from the local
  package feed.
