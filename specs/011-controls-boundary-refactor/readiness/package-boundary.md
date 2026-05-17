# Package Boundary Inventory

Status: active package boundary evidence captured through T071.

## Current Package Assets

| Area | Current assets |
|------|----------------|
| Controls | `src/Controls/Controls.fsproj`, `.fsi/.fs` pairs for `Types`, `Theme`, `Diagnostics`, `Accessibility`, `Attributes`, `Control`, `Catalog`, `TextInput`, `Collections`, `Charts`, `CustomControl`, `src/Controls/catalog.yml`, `src/Controls/skill/SKILL.md` |
| Charts | `src/Charts/Charts.fsproj`, `.fsi/.fs` pairs for chart modules and `DataGrid`, `tests/Charts.Tests/`, `samples/ChartsGallery/`, `readiness/surface-baselines/FS.Skia.UI.Charts.txt` |
| KeyboardInput | `src/KeyboardInput/KeyboardInput.fsproj`, `KeyboardInput.fsi`, `KeyboardInput.fs`, `src/KeyboardInput/skill/SKILL.md`, `tests/KeyboardInput.Tests/`, `samples/KeyboardInputGallery/` |
| Elmish | `src/Elmish/Elmish.fsproj`, `Elmish.fsi`, `Elmish.fs`, `src/Elmish/skill/SKILL.md`, `tests/Elmish.Tests/` |
| Layout | `src/Layout/`, `tests/Layout.Tests/`, `samples/LayoutGraphGallery/` |
| Scene | `src/Scene/`, `tests/Scene.Tests/` |
| SkiaViewer | `src/SkiaViewer/`, `tests/SkiaViewer.Tests/` |
| Monolithic runtime | `src/Lib/Lib.fsproj`, `src/Lib/Library.*`, `src/Lib/KeyboardInput.*`, `tests/Lib.Tests/` |
| Template | `template/capabilities.yml`, `template/profiles/*.yml`, `template/fragments/*` |
| Samples | `samples/BasicViewer`, `InteractiveViewer`, `ControlsGallery`, `ChartsGallery`, `DataGridGallery`, `KeyboardInputGallery`, `LayoutGraphGallery`, `ScreenshotGallery`, `DemoReel`, `EffectsGallery`, `ParityGallery` |
| Surface baselines | `readiness/surface-baselines/FS.Skia.UI*.txt`, including active Controls, KeyboardInput, Elmish, and legacy Charts baselines |

## Current Boundary Findings

- `build.fsx` active `PackLocal` project list excludes `src/Charts/Charts.fsproj`, but the legacy Charts project, tests, sample, and surface baseline still exist.
- Setup inventory found that `src/Controls/Controls.fsproj` referenced `src/Lib/Lib.fsproj`, `src/Layout/Layout.fsproj`, and `src/KeyboardInput/KeyboardInput.fsproj`.
- `src/Controls/Charts.*` already exposes Controls-owned chart types, but DataGrid still only appears under `src/Charts/`.
- The planned `FS.Skia.UI.Controls.Elmish` package does not exist yet; current Elmish package is `FS.Skia.UI.Elmish`.
- Several samples still reference `src/Lib/Lib.fsproj` for viewer hosting, which is acceptable for host samples but must not become a Controls package dependency unless explicitly documented.

## Verification Commands

- Inventory scan: `find src tests samples template readiness -maxdepth 3 -type f`
- Stale boundary scan: `rg -n "FS\\.Skia\\.UI\\.Charts|\\bcharts\\b|ChartsGallery|DataGrid|renderer-neutral|src/Lib|Lib\\.fsproj"`
- Red test log: `readiness/logs/t009-package-boundary-red.txt`

## US1 T033 Boundary Evidence

- `readiness/logs/t033-controls-references.txt`: `src/Controls/Controls.fsproj`
  directly references only `Scene`, `Layout`, and `KeyboardInput`.
- `readiness/logs/t033-layout-references.txt`: `src/Layout/Layout.fsproj`
  now references `Scene` instead of the monolithic `src/Lib` project.
- `readiness/logs/t033-controls-elmish-build-after-restore.txt`: restored and
  built Controls.Elmish through `Scene`, `Layout`, `KeyboardInput`,
  `Controls`, and `Controls.Elmish` with no `Lib` project build in the graph.
- `readiness/logs/t033-hidden-coupling-scan.txt`: source scan found no
  Controls/adapter references to `src/Lib`, `FS.Skia.UI.dll`, viewer host-loop
  types, `Cmd<`, `SkiaViewer`, or hidden `mutable` state.
- `readiness/logs/t033-keyboard-runtime-definition-scan.txt`: rich
  `KeyboardModel`, `KeyboardMsg`, and `KeyboardEffect` definitions appear only
  under `src/KeyboardInput/`.

## US2 T039 Red Package Boundary Evidence

- `readiness/logs/t039-us2-charts-package-red.txt`: package tests now reject
  active Charts project ownership, generated product package enumeration of
  `FS.Skia.UI.Charts`, active Charts surface-baseline participation, and
  chart-specific generated skill fragments.

## US2 T044 Charts Deactivation Evidence

- `readiness/logs/t044-package-boundary.txt`: package boundary tests pass with
  `src/Charts/Charts.fsproj` removed, no active `FS.Skia.UI.Charts` generated
  package enumeration, and no Charts surface baseline.
- `readiness/logs/t044-active-charts-scan.txt`: active package/template scan
  confirms no Charts package, project, capability, generated package, or
  chart-specific generated skill path remains active.

## US2 T048 Readiness Capture

- `readiness/logs/t048-package-tests.txt`: Package.Tests passes with active
  Charts project, generated package, capability, and surface-baseline
  participation removed.
- `readiness/logs/t048-stale-reference-scan.txt`: active build, template,
  package, and sample paths contain no Charts package/capability references;
  historical docs/spec references are preserved as migration context.

## US4 T067 Active Surface Boundary

- Active root baselines are `FS.Skia.UI`, `FS.Skia.UI.Layout`,
  `FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Controls`, and
  `FS.Skia.UI.Controls.Elmish`; package tests also require the V3 Scene,
  SkiaViewer, Elmish, and Testing baselines to exist.
- `readiness/surface-baselines/FS.Skia.UI.Charts.txt` remains absent from the
  active package-surface boundary.
- `build.fsx` package-surface report generation names the Controls.Elmish
  baseline so maintainer reports match the governed package set.
- Evidence:
  - `readiness/logs/t067-refresh-surface-baselines.txt`
  - `readiness/logs/t067-package-surface.txt`
  - `readiness/logs/t067-surface-baseline-scan.txt`

## US4 T071 Package Boundary Capture

| Evidence | Log | Verdict |
|----------|-----|---------|
| Dependency metadata and removed Charts project check | `readiness/logs/t066-dependency-governance.txt` | PASS |
| Package surface boundary check | `readiness/logs/t069-package-boundary.txt` | PASS |
| Smoke/sample boundary check | `readiness/logs/t069-smoke-sample-boundary.txt` | PASS |
| Generated source/package product scan | `readiness/logs/t070-generated-product-scan.txt` | PASS |

## T074 Package Boundary Verification

- `readiness/logs/t074-packlocal.txt`: `PackLocal` passed and packed the active
  package set, including `FS.Skia.UI.Controls`,
  `FS.Skia.UI.KeyboardInput`, and `FS.Skia.UI.Controls.Elmish`, with no active
  `FS.Skia.UI.Charts` package.
- `readiness/logs/t074-package-surface-check.txt`: `PackageSurfaceCheck`
  passed against the active surface baselines and the generated package-surface
  report.
- `readiness/logs/t074-fsi-transcripts.txt`: `FsiTranscripts` passed for the
  public prelude scripts, including Controls, KeyboardInput, Layout, and the
  Controls.Elmish adapter.
- No T074 baseline refresh was run; the approved baseline diff is the T067
  Controls/KeyboardInput/Controls.Elmish addition and Charts baseline removal.

## T077 Capability, Skill, And Dependency Gates

| Gate | Log | Verdict |
|------|-----|---------|
| `./fake.sh build -t CapabilityCheck` | `readiness/logs/t077-capability-check.txt` | PASS |
| `./fake.sh build -t SkillCheck` | `readiness/logs/t077-skill-check.txt` | PASS |
| `./fake.sh build -t DependencyReport` | `readiness/logs/t077-dependency-report.txt` | PASS |

The generated capability catalog keeps Controls as the active home for
controls, rich rendering, charts, graph views, and DataGrid. Selected generated
skills include Controls and KeyboardInput guidance without reintroducing a
chart-only skill. Dependency reporting confirms Controls has no direct
external packages and no active Charts package/project reference.

## T084 Stale Boundary Cleanup

- Removed leftover tracked legacy `src/Charts/*` source files after the package
  project had already been removed from active build and pack wiring.
- Removed leftover tracked `tests/Charts.Tests/*` source files after the legacy
  Charts test project had already been removed.
- Updated `.specify/memory/constitution.md` so current FS.Skia.UI capability
  skills list `fs-skia-ui-widgets` for Controls-owned chart, graph, and
  DataGrid guidance instead of the removed chart-only skill.
- Updated `docs/architecture.md` to describe the current Scene, SkiaViewer,
  Elmish, KeyboardInput, Layout, Controls, and Controls.Elmish package
  boundaries.
- Final scan: `readiness/logs/t084-stale-boundary-scan.txt`.
