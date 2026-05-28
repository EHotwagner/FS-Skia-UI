# Package Surface Baseline Evidence

Status: pending implementation.

This file will capture intentional public surface changes for
`FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Testing`, including matching `.fsi`
contracts, FSI transcript coverage, and package surface baseline updates.

## Foundation Contract Build Check

- `dotnet build src/SkiaViewer/SkiaViewer.fsproj --no-restore`: PASS
- `dotnet build src/Testing/Testing.fsproj --no-restore`: PASS

The public `.fsi` contracts for the initial screenshot request/result,
workflow, evidence record parsing, and artifact validation shapes have matching
implementation type shapes. Full package surface baseline refresh remains
pending T007/T033.

## FSI Contract Transcript

- Script: `specs/026-working-screenshot-taking/readiness/fsi/screenshot-contracts.fsx`
- Transcript: `specs/026-working-screenshot-taking/readiness/fsi/screenshot-contracts.txt`
- Scope: SkiaViewer `initEvidenceWorkflow` / `updateEvidenceWorkflow` and
  Testing `EvidenceReports.validateScreenshotEvidence`.

## Initial Feature Surface Expectations

- `specs/026-working-screenshot-taking/readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
- `specs/026-working-screenshot-taking/readiness/surface-baselines/FS.Skia.UI.Testing.txt`

These feature-local baselines focus on the screenshot contract additions. The
repository-wide `readiness/surface-baselines/` refresh remains pending the
integration task once implementation stabilizes.

## Integration Validation

- `./fake.sh build -t PackageSurfaceCheck`: PASS
- `./fake.sh build -t FsiTranscripts`: PASS through `Verify`
- `./fake.sh build -t Verify`: PASS
