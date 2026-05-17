# US2 Independent Validation

Status: US2 independently functional and testable.

## Goal

Product developers can use chart controls, graph views, and DataGrid through
`FS.Skia.UI.Controls` without selecting an active Charts package or capability.
DataGrid is documented and cataloged as a data control, not chart-only
terminology.

## Validation Path

| Step | Command | Evidence |
|------|---------|----------|
| Controls contracts and catalog | `dotnet test tests/Controls.Tests/Controls.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t048-controls-tests.txt` |
| Package and surface boundary | `dotnet test tests/Package.Tests/Package.Tests.fsproj -m:1 --no-restore` | `readiness/logs/t048-package-tests.txt` |
| Public Controls FSI | `dotnet fsi scripts/controls-prelude.fsx` | `readiness/logs/t047-controls-fsi.txt` |
| Sample composition | `dotnet run --no-build --project samples/ControlsGallery/ControlsGallery.fsproj -- --contract-smoke` | `readiness/logs/t046-controlsgallery-contract-smoke.txt` |
| DataGrid sample | `dotnet run --no-build --project samples/DataGridGallery/DataGridGallery.fsproj -- --contract-smoke` | `readiness/logs/t046-datagridgallery-contract-smoke.txt` |
| Chart sample | `dotnet run --no-build --project samples/ChartsGallery/ChartsGallery.fsproj -- --contract-smoke` | `readiness/logs/t046-chartsgallery-contract-smoke.txt` |
| Layout plus graph/chart/grid sample | `dotnet run --no-build --project samples/LayoutGraphGallery/LayoutGraphGallery.fsproj -- --contract-smoke` | `readiness/logs/t046-layoutgraphgallery-contract-smoke.txt` |
| Active stale-reference scan | `rg` scan over build, template, source, and samples | `readiness/logs/t048-stale-reference-scan.txt` |

## DataGrid Category Evidence

- `src/Controls/catalog.yml` records `data-grid` with `category: data` and
  `module: DataGrid`.
- `Catalog.supportedControls` exposes the same `data-grid` row with
  `Category = "data"` and `Module = "DataGrid"`.
- `docs/controls.md` describes DataGrid as a Controls data control with
  product-owned rows, visible range, selection/focus state, sort/filter
  metadata, rendering, and diagnostics.
- `readiness/logs/t045-control-catalog.txt` verifies these catalog obligations.
- `readiness/logs/t045-catalog-evidence-scan.txt` captures the category,
  module, and readiness evidence links.

## Remaining Scope

- Generated product examples and generated guidance composition remain US3
  work. `readiness/logs/t046-composition-tests.txt` shows the sample
  composition test passing and the generated product composition test still
  failing as expected.
- Broader maintainer governance checks, dependency reports, and compatibility
  checklist expansion remain US4 work.
