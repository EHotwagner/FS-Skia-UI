# US2 Data Visuals Validation

Task scope: chart and DataGrid props, scaling helpers, chart builders, DataGrid viewport/sort/hit-test helpers, pure composition into core scenes, and gallery samples with Elmish-owned interaction state.

## Public Entry Evidence

| Evidence | Command | Result |
|----------|---------|--------|
| Charts public tests | `dotnet test tests/Charts.Tests/Charts.Tests.fsproj --no-restore` | `readiness/logs/t038-charts-tests-rerun.txt` passes 10 tests covering chart defaults, scale helpers, all chart builders, 100,000-point data, DataGrid sorting/viewport/hit-tests, 10,000-row data, and pure composition. |
| Charts FSI prelude | `dotnet fsi scripts/charts-prelude.fsx` | `readiness/transcripts/t031-charts-prelude.txt` exercises public chart construction from FSI. |
| ChartsGallery contract smoke | `dotnet run --project samples/ChartsGallery/ChartsGallery.fsproj -- --contract-smoke` | `readiness/smoke/t039-chartsgallery-contract.txt` confirms chart scene composition and model-owned selection state. |
| DataGridGallery contract smoke | `dotnet run --project samples/DataGridGallery/DataGridGallery.fsproj -- --contract-smoke` | `readiness/smoke/t039-datagridgallery-contract.txt` confirms DataGrid scene composition and model-owned sort/scroll/selection state. |
| ChartsGallery Vulkan smoke | `dotnet run --project samples/ChartsGallery/ChartsGallery.fsproj -- --smoke` | `readiness/smoke/t039-chartsgallery-vulkan.txt` renders one Vulkan frame, `fallback-used=false`, first frame 355 ms. |
| DataGridGallery Vulkan smoke | `dotnet run --project samples/DataGridGallery/DataGridGallery.fsproj -- --smoke` | `readiness/smoke/t039-datagridgallery-vulkan.txt` renders one Vulkan frame, `fallback-used=false`, first frame 394 ms. |
| Solution build | `dotnet build FS-Skia-UI.sln` | `readiness/logs/t039-solution-build.txt` builds the full solution including ChartsGallery and DataGridGallery. |

## Scale Evidence

The Charts test log includes:

- `line chart accepts one hundred thousand points within the scale budget`
- `data grid projects ten thousand rows within the scale budget`

Both tests exercise public package APIs and assert projection under two seconds on this workstation.

## Status

US2 has real local evidence for public API construction, pure composition, large-data projection, and Vulkan-hosted sample rendering. Chart/DataGrid interaction state remains in each sample `Model`; component builders return declarative `Scene` values only.
