# Public Surface Evidence

Status: US4 readiness capture updated through T071.

## Required Surface Owners

| Package | Contract files | Baseline |
|---------|----------------|----------|
| `FS.Skia.UI.Controls` | `src/Controls/*.fsi` | `readiness/surface-baselines/FS.Skia.UI.Controls.txt` |
| `FS.Skia.UI.KeyboardInput` | `src/KeyboardInput/KeyboardInput.fsi` | `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt` |
| `FS.Skia.UI.Controls.Elmish` | planned `src/Controls.Elmish/ControlsElmish.fsi` | planned `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt` |

## Evidence To Produce

- Package surface check: `./fake.sh build -t PackageSurfaceCheck`
- FSI transcript run: `./fake.sh build -t FsiTranscripts`
- Intentional baseline refresh only after contract review: `./fake.sh build -t RefreshSurfaceBaselines`

## Red Test Evidence

- `readiness/logs/t008-public-surface-red.txt`: fails on the missing
  `Controls.Elmish` project/baseline and missing Controls runtime/rich
  rendering/DataGrid contract files.

## Foundation Evidence

- `readiness/logs/t014-controls-contracts.txt`: Controls package builds with
  `ControlRuntime`, `RichText`, and `DataGrid` contracts and implementations.
- `readiness/logs/t018-refresh-surface-baselines.txt`: refreshed Controls,
  KeyboardInput, and Controls.Elmish package baselines.
- `readiness/logs/t018-controls-fsi.txt`, `readiness/logs/t018-keyboardinput-fsi.txt`,
  and `readiness/logs/t018-controls-elmish-fsi.txt`: FSI transcript harnesses
  exercise Controls, KeyboardInput, and adapter public surfaces.
- `readiness/logs/t018-package-surface-check.txt`: package surface baseline
  tests pass after the intentional baseline refresh.
- `readiness/logs/t021-controls-runtime-fsi.txt`,
  `readiness/logs/t021-keyboardinput-fsi.txt`, and
  `readiness/logs/t021-controls-elmish-fsi.txt`: FSI sessions exercise public
  `init` / `update` paths and emitted effects for ControlRuntime,
  KeyboardInput, and the adapter.

## US1 Red Test Evidence

- `readiness/logs/t023-us1-semantic-red.txt`: custom controls still lack
  measurement, drawing, clipping, and effects hooks.
- `readiness/logs/t024-controlruntime-red.txt`: cancellation does not yet clear
  caret, selection, and composition state.
- `readiness/logs/t025-keyboardinput-red.txt`: focus loss does not yet emit a
  recovery diagnostic.
- `readiness/logs/t026-adapter-red.txt`: stale control targets are not yet
  mapped to adapter diagnostics.
- `readiness/logs/t027-rich-rendering-red.txt`: rich text unsupported effects
  are not yet diagnosed during measurement.

## Setup Observation

The repository currently has Controls, KeyboardInput, and Elmish package
contracts, but no `src/Controls.Elmish/` adapter package yet.

## US1 T033 Evidence

- `readiness/logs/t033-controls-fsi.txt`: Controls FSI exercises stable
  records, dispatch, render output, ControlRuntime focus/recovery, and catalog
  access through the public Controls package without referencing
  `FS.Skia.UI.dll`.
- `readiness/logs/t033-controls-elmish-fsi.txt`: Controls.Elmish FSI exercises
  public adapter program, keyboard-effect interpretation, and control-effect
  interpretation.
- `readiness/logs/t033-keyboardinput-fsi.txt`: KeyboardInput FSI exercises
  public runtime state, emitted effects, focus recovery, and state display.
- `readiness/logs/t033-scene-tests.txt`, `readiness/logs/t033-layout-tests.txt`,
  `readiness/logs/t033-controls-tests.txt`, and
  `readiness/logs/t033-elmish-tests.txt`: focused public-surface tests pass
  after moving Controls/Layout scene usage to `FS.Skia.UI.Scene`.

## US1 T034 Sample Evidence

- `readiness/logs/t034-controlsgallery-build.txt` and
  `readiness/logs/t034-controlsgallery-contract-smoke.txt`: ControlsGallery
  builds and exercises stable records, rich text, custom Skia rendering,
  product-owned ControlRuntime and KeyboardInput state, and Controls.Elmish
  adapter wiring.
- `readiness/logs/t034-keyboardinputgallery-build.txt` and
  `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt`:
  KeyboardInputGallery builds on the dedicated KeyboardInput package and
  emits state-display, focus-recovery, scene, and adapter evidence.
- `docs/controls.md`: public Controls documentation now shows stable records,
  rich text, ControlRuntime, KeyboardInput, and Controls.Elmish adapter usage.

## US1 Readiness Capture

| Command | Log | Verdict |
|---------|-----|---------|
| `dotnet build src/Controls/Controls.fsproj -m:1 --no-restore` | `readiness/logs/t033-controls-build-final.txt` | PASS |
| `dotnet build src/Controls.Elmish/Controls.Elmish.fsproj -m:1 --no-restore` | `readiness/logs/t033-controls-elmish-build-after-restore.txt` | PASS |
| `dotnet fsi scripts/controls-prelude.fsx` | `readiness/logs/t033-controls-fsi.txt` | PASS |
| `dotnet fsi scripts/controls-elmish-prelude.fsx` | `readiness/logs/t033-controls-elmish-fsi.txt` | PASS |
| `dotnet fsi scripts/keyboardinput-package-prelude.fsx` | `readiness/logs/t033-keyboardinput-fsi.txt` | PASS |
| `dotnet run --no-build --project samples/ControlsGallery/ControlsGallery.fsproj -- --contract-smoke` | `readiness/logs/t034-controlsgallery-contract-smoke.txt` | PASS |
| `dotnet run --no-build --project samples/KeyboardInputGallery/KeyboardInputGallery.fsproj -- --contract-smoke` | `readiness/logs/t034-keyboardinputgallery-contract-smoke.txt` | PASS |

The attempted full `tests/Smoke.Tests` run was stopped after hanging in its
nested `dotnet run`; the direct sample contract-smoke commands above completed
and are the US1 sample evidence.

US1 independent validation steps and unsupported/deferred conditions are
captured in `readiness/us1-validation.md`.

## US2 T042 Chart And Graph Evidence

- `readiness/logs/t042-controls-build.txt`: Controls builds with the
  Controls-owned chart/graph rendering changes.
- `readiness/logs/t042-chart-graph-semantic.txt`: focused Controls semantic
  tests pass, including chart and graph controls rendering as Controls-owned
  scene elements.
- `readiness/logs/t042-controls-fsi-chart-graph.txt`: public Controls FSI
  transcript constructs form controls, `LineChart`, `GraphView`, and
  `DataGrid` declarations through `FS.Skia.UI.Controls` without opening
  `FS.Skia.UI.Charts`.

## US2 T043 DataGrid Evidence

- `readiness/logs/t043-datagrid-scalability.txt`: public DataGrid model/update
  and render surface passes the 10,000-row visible-range and diagnostics
  contract.
- `readiness/logs/t043-controls-fsi-datagrid.txt`: Controls FSI transcript
  renders DataGrid rows from the public package surface.

## US2 T047 Surface Refresh Evidence

- `readiness/logs/t047-refresh-surface-baselines.txt`: active package surface
  baselines refreshed after Controls-owned chart, graph, and DataGrid changes.
- `readiness/logs/t047-controls-fsi.txt`: Controls FSI transcript exercises
  form controls, chart, graph, and DataGrid authoring from
  `FS.Skia.UI.Controls`.
- `readiness/logs/t047-package-surface.txt`: package surface tests pass after
  baseline refresh and Charts baseline removal.
- `readiness/logs/t047-surface-baseline-scan.txt`: active baselines include
  Controls chart/DataGrid exports and no `FS.Skia.UI.Charts` baseline.

## US4 T060 Package Surface Governance

- Added `tests/Package.Tests/SurfaceAreaTests.fs` coverage proving Controls,
  KeyboardInput, and Controls.Elmish implementation files have paired `.fsi`
  signatures, active surface baselines include expected exports, the Charts
  baseline is absent, and top-level visibility modifiers do not replace
  signature ownership.
- Pass log: `readiness/logs/t060-package-surface-governance.txt`.

## US4 T067 Surface Baseline Refresh

- Rebuilt `FS.Skia.UI` and the Controls.Elmish dependency chain before
  refreshing active package baselines.
- `scripts/refresh-surface-baselines.fsx` writes active baselines for
  `FS.Skia.UI`, `FS.Skia.UI.Layout`, `FS.Skia.UI.KeyboardInput`,
  `FS.Skia.UI.Controls`, and `FS.Skia.UI.Controls.Elmish`; it does not write a
  Charts package baseline.
- `build.fsx` package-surface report output now lists the Controls.Elmish
  baseline alongside the other active V3 package baselines.
- Evidence:
  - `readiness/logs/t067-lib-build.txt`
  - `readiness/logs/t067-controls-elmish-build.txt`
  - `readiness/logs/t067-refresh-surface-baselines.txt`
  - `readiness/logs/t067-package-surface.txt`
  - `readiness/logs/t067-surface-baseline-scan.txt`

## US4 T071 Public Surface Capture

| Evidence | Log | Verdict |
|----------|-----|---------|
| Controls.Elmish dependency-chain build | `readiness/logs/t067-controls-elmish-build.txt` | PASS |
| Active surface baseline refresh | `readiness/logs/t067-refresh-surface-baselines.txt` | PASS |
| Package surface tests | `readiness/logs/t067-package-surface.txt` | PASS |
| Active baseline scan | `readiness/logs/t067-surface-baseline-scan.txt` | PASS |

## T074 Package And FSI Verification

| Command | Log | Verdict | Duration |
|---------|-----|---------|----------|
| `./fake.sh build -t PackLocal` | `readiness/logs/t074-packlocal.txt` | PASS | 18s |
| `./fake.sh build -t PackageSurfaceCheck` | `readiness/logs/t074-package-surface-check.txt` | PASS | 4s |
| `./fake.sh build -t FsiTranscripts` | `readiness/logs/t074-fsi-transcripts.txt` | PASS | 6s |

`RefreshSurfaceBaselines` was not run for T074 because this task did not add an
intentional public API change beyond the already-approved T067 surface refresh.
The approved baseline diff remains: Controls and KeyboardInput exports expand
for the new runtime/rich/DataGrid contracts, `FS.Skia.UI.Controls.Elmish.txt`
is active, and the legacy `FS.Skia.UI.Charts.txt` baseline is removed from
active package review.
