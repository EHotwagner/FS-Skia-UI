# US3 Layout And Graph Validation

Task scope: stack and dock layout contracts, resize/no-overlap behavior, graph validation, deterministic graph layout, graph scene rendering, hit-tests, and the LayoutGraphGallery sample.

## Public Entry Evidence

| Evidence | Command | Result |
|----------|---------|--------|
| Layout public tests | `dotnet test tests/Layout.Tests/Layout.Tests.fsproj --no-restore` | `readiness/logs/t049-layout-tests-rerun.txt` passes 11 tests covering layout props, resize/no-overlap, graph validation, 100-node DAG layout, 50-node weighted graph layout, graph rendering, labels/weights, and node/edge hit-tests. |
| Layout FSI prelude | `dotnet fsi scripts/layout-prelude.fsx` | `readiness/transcripts/t051-layout-prelude.txt` exercises public layout scene construction from FSI. |
| LayoutGraphGallery contract smoke | `dotnet run --project samples/LayoutGraphGallery/LayoutGraphGallery.fsproj -- --contract-smoke` | `readiness/smoke/t050-layoutgraphgallery-contract.txt` confirms model-owned focus state, invalid DAG diagnostics, directed layout output, graph scene categories, and chart/grid composition. |
| LayoutGraphGallery Vulkan smoke | `dotnet run --project samples/LayoutGraphGallery/LayoutGraphGallery.fsproj -- --smoke` | `readiness/smoke/t050-layoutgraphgallery-vulkan.txt` renders one Vulkan frame, `fallback-used=false`, first frame 438 ms. |

## Scale Evidence

The Layout test log includes:

- `graph layout handles one hundred node DAG within two seconds`
- `weighted undirected graph with fifty nodes has visible components and renders a scene`
- resize/no-overlap tests for 10 children across three sizes for both horizontal and vertical stacks

## Status

US3 has real local evidence for public layout/graph construction, deterministic layout helpers, validation diagnostics, scene rendering, and the Vulkan-hosted gallery sample.
