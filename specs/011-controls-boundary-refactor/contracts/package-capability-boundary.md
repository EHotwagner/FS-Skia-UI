# Contract: Package And Capability Boundary

## Purpose

Define package, capability, and dependency obligations for removing the legacy
Charts boundary and keeping Controls independent from the monolithic
viewer/runtime surface.

## Controls Capability

- Capability id: `controls`
- Package id: `FS.Skia.UI.Controls`
- Project: `src/Controls/Controls.fsproj`
- Active generated app ownership: ordinary controls, rich text, chart
  controls, graph views, and DataGrid
- Allowed direct capability dependencies: Scene, Layout, KeyboardInput
- Disallowed hidden coupling: SkiaViewer host loop, `src/Lib`, application
  shutdown, window creation, update scheduling, release publishing

## Charts Removal

The following must be removed from active package/capability ownership:

- `FS.Skia.UI.Charts` package identity
- `src/Charts/Charts.fsproj`
- active `charts` capability metadata, if present
- generated `FS.Skia.UI.Charts` package references
- chart-only generated guidance and skills
- chart package surface baseline participation

Chart, graph, and DataGrid public authoring moves under Controls. Historical
or migration documentation may mention Charts only to describe the replacement
path.

## Dependency Report Rules

Dependency evidence must report:

- Controls package dependencies and their rationale
- absence of hidden `src/Lib`/viewer/runtime coupling from Controls
- KeyboardInput as the single package-owned rich input runtime surface
- Elmish adapter dependency placement
- removal of Charts package references from generated products and active
  capability selection

## Generated Product Rules

- Generated products that include Controls reference `FS.Skia.UI.Controls`.
- Generated products do not reference `FS.Skia.UI.Charts`.
- Generated examples are product-owned and include form plus data/chart usage.
- Generated products do not copy framework samples, galleries, historical
  specs, readiness evidence, framework docs, or framework implementation
  projects.

## Validation

- `CapabilityCheck` fails on stale active Charts capability references.
- `GeneratedProductCheck` fails on generated `FS.Skia.UI.Charts` references.
- `DependencyReport` fails on unexpected Controls dependency leaks.
- `TemplateDrift` and `GeneratedGuidanceCheck` fail on stale chart-only or
  renderer-neutral controls guidance.
