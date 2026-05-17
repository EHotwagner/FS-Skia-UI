# Chart And DataGrid Controls Evidence

Status: setup placeholder, awaiting US2 implementation.

## Current Assets

- Controls-owned chart surface: `src/Controls/Charts.fsi`, `src/Controls/Charts.fs`
- Legacy Charts package and DataGrid surface: `src/Charts/`
- Legacy tests: `tests/Charts.Tests/Tests.fs`
- Legacy samples: `samples/ChartsGallery/`, `samples/DataGridGallery/`

## Required Evidence

- Chart, graph, and DataGrid authoring works through `FS.Skia.UI.Controls`.
- No active `FS.Skia.UI.Charts` package or `charts` capability is required.
- DataGrid large-row behavior covers 10,000 items, visible ranges, bounded
  scene nodes, durations, selection/focus interaction, and diagnostics.

## US2 Red Test Evidence

- `readiness/logs/t037-us2-catalog-red.txt`: catalog rows for chart, graph,
  and DataGrid do not yet link this readiness evidence.
- `readiness/logs/t038-us2-public-api-fsi-red.txt`: package/public-surface
  tests fail because the Controls FSI transcript does not yet author
  `LineChart`, `GraphView`, and `DataGrid` through `FS.Skia.UI.Controls`.
- `readiness/logs/t039-us2-charts-package-red.txt`: package/public-surface
  tests fail because the legacy Charts project, generated product package
  enumeration, and Charts surface baseline remain active.
- `readiness/logs/t040-us2-datagrid-large-row-red.txt`: Controls tests fail
  because DataGrid does not yet clamp the final visible range, render visible
  row/header nodes for 10,000 rows, or report invalid viewport diagnostics.
- `readiness/logs/t041-us2-composition-red.txt`: governance composition tests
  fail because ControlsGallery and the generated product template do not yet
  combine form inputs, a chart, and a DataGrid through Controls only.

## US2 T042 Chart And Graph Implementation Evidence

- `readiness/logs/t042-controls-build.txt`: Controls builds after chart/graph
  render ownership moved into the Controls package.
- `readiness/logs/t042-chart-graph-semantic.txt`: focused semantic tests pass
  for Controls-owned `LineChart` and `GraphView` rendering and accessibility
  metadata.
- `readiness/logs/t042-controls-fsi-chart-graph.txt`: public FSI exercises
  `LineChart.create`, `GraphView.create`, and DataGrid declaration syntax from
  `FS.Skia.UI.Controls` with no Charts namespace.

## US2 T043 DataGrid Implementation Evidence

- `readiness/logs/t043-controls-build.txt`: Controls builds with the
  Controls-owned DataGrid model, visible-range, render subtree, and diagnostics
  changes.
- `readiness/logs/t043-datagrid-scalability.txt`: focused DataGrid tests pass
  for 10,000 rows, final-row visible-range clamping, selection/focus effects,
  bounded rendered node count, observed timings, and invalid viewport
  diagnostics.
- `readiness/logs/t043-controls-fsi-datagrid.txt`: public FSI constructs and
  renders DataGrid declarations through `FS.Skia.UI.Controls`.

## US2 T044 Legacy Charts Deactivation Evidence

- `readiness/logs/t044-package-boundary.txt`: `tests/Package.Tests` passes
  after removing active Charts project and surface-baseline participation.
- `readiness/logs/t044-active-charts-scan.txt`: active build/template/package
  scan reports no remaining Charts package or generated skill participation.

## US2 T045 Catalog Evidence

- `readiness/logs/t045-control-catalog.txt`: catalog tests pass for
  Controls-owned chart, graph, and DataGrid metadata and evidence links.
- `readiness/logs/t045-catalog-evidence-scan.txt`: catalog YAML and docs
  record the chart/DataGrid readiness evidence path and DataGrid data
  categorization.

## US2 T046 Sample Evidence

- `readiness/logs/t046-controlsgallery-build.txt` and
  `readiness/logs/t046-controlsgallery-contract-smoke.txt`: ControlsGallery
  builds and composes form inputs, chart, graph, and DataGrid through Controls.
- `readiness/logs/t046-datagridgallery-build.txt` and
  `readiness/logs/t046-datagridgallery-contract-smoke.txt`: DataGridGallery
  builds and smokes against Controls-owned DataGrid state/effects/rendering.
- `readiness/logs/t046-chartsgallery-build.txt` and
  `readiness/logs/t046-chartsgallery-contract-smoke.txt`: ChartsGallery now
  builds as a Controls-owned chart sample with no Charts package reference.
- `readiness/logs/t046-layoutgraphgallery-build.txt` and
  `readiness/logs/t046-layoutgraphgallery-contract-smoke.txt`:
  LayoutGraphGallery builds and smokes with Layout plus Controls-owned chart
  and DataGrid declarations.
- `readiness/logs/t046-sample-stale-reference-scan.txt`: affected samples and
  solution contain no `FS.Skia.UI.Charts` or `src/Charts/Charts.fsproj`
  references; Controls API markers are present.
- `readiness/logs/t046-composition-tests.txt`: sample composition test passes;
  generated product composition remains a US3 implementation gap.

## US2 T047 Surface Refresh Evidence

- `readiness/logs/t047-refresh-surface-baselines.txt`: refreshed active
  surface baselines for Controls-owned chart, graph, and DataGrid public
  contracts.
- `readiness/logs/t047-controls-fsi.txt`: public FSI transcript exercises
  Controls-owned chart, graph, and DataGrid authoring.
- `readiness/logs/t047-package-surface.txt`: package surface tests pass with
  no active Charts baseline.
- `readiness/logs/t047-surface-baseline-scan.txt`: Controls baseline contains
  chart/DataGrid exports; `FS.Skia.UI.Charts.txt` is absent.

## US2 T048 Readiness Capture

- `readiness/logs/t048-controls-tests.txt`: full Controls tests pass for
  chart/graph ownership, DataGrid large-row behavior, catalog metadata, and
  public surface diagnostics.
- `readiness/logs/t048-package-tests.txt`: package tests pass with no active
  Charts package, generated package enumeration, or Charts surface baseline.
- `readiness/logs/t048-stale-reference-scan.txt`: active paths are clean of
  Charts package/capability references; historical and migration references are
  still available in docs/spec readiness.

## US2 T049 Independent Validation

- `readiness/us2-validation.md`: independent validation path for Controls-owned
  chart, graph, and DataGrid usage, including DataGrid data-category evidence
  and remaining US3/US4 scope.

## T076 Catalog And Rendering Gates

| Gate | Log | Verdict |
|------|-----|---------|
| `./fake.sh build -t ControlsCatalogCheck` | `readiness/logs/t076-controls-catalog-check.txt` | PASS |
| `./fake.sh build -t ControlsRenderingCheck` | `readiness/logs/t076-controls-rendering-check.txt` | PASS |

The T076 catalog gate revalidates Controls-owned chart, graph, and DataGrid
catalog rows. The rendering gate revalidates graph/chart rendering plus
DataGrid 10,000-item visible-range behavior with bounded row counts and no
unsupported environment diagnostics for deterministic scene readback.
