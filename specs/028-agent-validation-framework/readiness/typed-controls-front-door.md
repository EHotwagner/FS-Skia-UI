# Typed Controls Front Door Evidence

Status: PASS

## Evidence

- `dotnet test tests/Controls.Tests/Controls.Tests.fsproj --no-restore` passed with 36 tests in `readiness/logs/t042-t045-us4-typed-controls-tests.txt`.
- The controls tests cover `KnownControl`, `KnownEvent`, and `KnownAttribute` union cases and confirm seeded misspellings are absent from the typed standard surface.
- Chart and DataGrid typed data front doors preserve typed payloads for `LineChart.series`, `DataGrid.columns`, `DataGrid.rows`, `DataGrid.visibleRange`, `DataGrid.selectedRows`, and `DataGrid.focusedCell`.
- `Catalog.validateStandardControl` reports missing required attributes, unsupported standard attributes, unsupported standard events, and visibly classified custom usage.
- `dotnet fsi readiness/fsi-session.fsx` regenerated `readiness/fsi-session.txt` with public typed standard and custom extension usage, including `Control.customControl`, `Attr.customAttribute`, `Attr.customEvent`, and `Catalog.validateStandardControl`.
- `./fake.sh build -t PackageSurfaceCheck` and `./fake.sh build -t FsiTranscripts` passed in `readiness/logs/t052-package-surface-check.txt` and `readiness/logs/t052-fsi-transcripts.txt`.

## Notes

The FAKE runner prints a pre-target `netstandard, Version=2.0.0.0` assembly-load
warning, but `PackageSurfaceCheck` and `FsiTranscripts` both completed
successfully after target execution. Full governance tests currently have
pre-existing agent-validation/generated-launch failures recorded in
`readiness/logs/t051-generated-controls-guidance.txt`; they were not used as
authoritative typed-controls evidence.
