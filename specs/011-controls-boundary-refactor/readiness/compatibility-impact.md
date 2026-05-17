# Compatibility Impact Evidence

Status: US4 compatibility evidence captured through T071.

## Required Evidence

- Existing Charts users receive a documented replacement path through Controls.
- The feature does not promise a Charts compatibility shim.
- The feature does not promise automated external-app migration.
- The feature does not promise release publishing automation.
- Lower-level Scene, Layout, KeyboardInput, SkiaViewer, and Elmish paths remain
  available for products that do not select Controls.

## US2 Replacement Path

Existing chart users should migrate authoring to `FS.Skia.UI.Controls`:

- `LineChart`, `BarChart`, `PieChart`, and `ScatterPlot` replace new chart
  authoring through the legacy Charts package.
- `GraphView` covers Controls-owned graph-view authoring for product screens;
  lower-level `FS.Skia.UI.Layout` graph helpers remain available separately.
- `DataGrid` is a Controls data control with product-owned rows, visible range,
  selection, focus, sort/filter metadata, render subtree, and diagnostics.

This refactor removes active Charts package participation. It does not add a
compatibility shim, automated external-app migration, release publishing
automation, or a promise that old `FS.Skia.UI.Charts` source remains buildable
as a package.

## US2 T048 Readiness Capture

- `readiness/logs/t048-controls-tests.txt`: Controls-owned chart, graph, and
  DataGrid behavior passes repository tests.
- `readiness/logs/t048-package-tests.txt`: active package boundary excludes
  Charts package/surface participation.
- `readiness/logs/t048-stale-reference-scan.txt`: active paths are clean while
  historical docs/spec references remain for migration guidance.

## US4 T063 Compatibility Guidance Test

- Added `tests/Governance.Tests/DocsGuidanceTests.fs` coverage proving the
  compatibility guidance documents the Controls replacement path for chart,
  graph, and DataGrid users; explicitly avoids compatibility shim, automated
  external-app migration, and release publishing automation promises; and
  preserves lower-level Scene, Layout, KeyboardInput, SkiaViewer, and Elmish
  paths.
- Pass log: `readiness/logs/t063-compatibility-docs.txt`.

## US4 T068 Documentation Update

- `docs/controls.md` now includes a Controls-versus-lower-level-path table:
  Controls for high-level controls/chart/DataGrid authoring, Scene for raw
  scene primitives, Layout for lower-level layout/graph helpers, KeyboardInput
  for keyboard runtime state, SkiaViewer for the host boundary, Elmish for the
  general viewer adapter, and Controls.Elmish for Controls-specific adapter
  effects.
- `docs/build.md`, `docs/testing.md`, `docs/technical-design.md`, and
  `docs/subsystem-design.md` now describe active Controls, KeyboardInput,
  Controls.Elmish, Scene, SkiaViewer, Elmish, and Layout project/test ownership
  instead of treating Charts as an active package/test surface.
- Replacement path remains explicit: chart controls, graph views, and DataGrid
  authoring move to `FS.Skia.UI.Controls`; there is no compatibility shim,
  automated external-app migration, or release publishing automation promise.
- Evidence:
  - `readiness/logs/t068-docs-guidance.txt`
  - `readiness/logs/t068-docs-scan.txt`

## US4 T071 Compatibility Capture

| Evidence | Log | Verdict |
|----------|-----|---------|
| Compatibility guidance governance test | `readiness/logs/t068-docs-guidance.txt` | PASS |
| Active docs lower-level path scan | `readiness/logs/t068-docs-scan.txt` | PASS |
| Governance readiness evidence coverage | `readiness/logs/t069-governance-evidence.txt` | PASS |
