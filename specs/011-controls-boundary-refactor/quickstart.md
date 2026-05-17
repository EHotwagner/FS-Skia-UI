# Quickstart: Controls Boundary Refactor

This quickstart describes the validation workflow expected after implementation
of the controls boundary refactor.

## 1. Validate Framework Development Workflow

```bash
./fake.sh build -t Dev
./fake.sh build -t CapabilityCheck
./fake.sh build -t SkillCheck
./fake.sh build -t DependencyReport
```

Expected outcome:

- framework projects restore, build, and run default tests
- Controls is the active home for forms, rich text, chart controls, graph
  views, and DataGrid
- legacy Charts package/capability references are absent from active generated
  capability selection
- dependency report shows Controls depending on Scene, Layout, and
  KeyboardInput without hidden monolithic viewer/runtime coupling
- generated guidance points new widget/control work to Controls-owned guidance

## 2. Validate Public Surface And Package Boundary

```bash
./fake.sh build -t PackLocal
./fake.sh build -t PackageSurfaceCheck
./fake.sh build -t FsiTranscripts
```

Expected outcome:

- `FS.Skia.UI.Controls` exposes stable records, control runtime, rich rendering
  escape hatches, chart controls, graph views, and DataGrid through curated
  `.fsi` files
- `FS.Skia.UI.KeyboardInput` exposes the rich keyboard runtime, effects,
  diagnostics, update contracts, and state display contracts
- the Elmish adapter package or module exposes command/subscription/program
  integration without moving that dependency into ordinary Controls
  declarations
- `FS.Skia.UI.Charts` is removed from package surface validation and active
  generated products
- compatibility guidance documents the supported Charts replacement path

## 3. Validate Runtime Boundaries

```bash
./fake.sh build -t Verify
```

If the implementation adds focused targets, these should also pass:

```bash
./fake.sh build -t ControlsRuntimeCheck
./fake.sh build -t KeyboardInputCheck
./fake.sh build -t ControlsBoundaryCheck
```

Expected outcome:

- product-owned `ControlRuntime` transitions cover focus, hover, pressed,
  caret/selection, composition, drag, stale target, and recovery paths
- product business values stay outside the transient control runtime
- KeyboardInput runtime tests cover pressed keys, active layout, mode stack,
  persistent mode state, temporary held layer release, focus-loss recovery,
  emitted effects, diagnostics, and state display rendering
- adapter tests cover effect interpretation into Elmish commands,
  subscriptions, or program wiring

## 4. Validate Catalog, Rich Rendering, Charts, And DataGrid

```bash
./fake.sh build -t Verify
```

If split targets exist, run:

```bash
./fake.sh build -t ControlsCatalogCheck
./fake.sh build -t ControlsRenderingCheck
```

Expected outcome:

- catalog metadata presents Controls as Skia/Elmish-specific
- ordinary stable-record control examples and explicit Skia escape-hatch
  examples both compile through public contracts
- rich text or rich rendering evidence uses Skia-specific contracts
- chart and graph examples compile through Controls
- DataGrid appears as a data or collection control, not as chart-only
  terminology
- validation failures name the affected control, catalog entry, package, or
  unsupported environment condition

## 5. Validate Generated Products

```bash
./fake.sh build -t TemplateCheck
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateDrift
```

Expected outcome:

- generated app profiles that select Controls reference Controls as the single
  path for ordinary controls, rich text, charts, graph views, and DataGrid
- generated products include form plus data/chart Controls usage
- generated products show both generic message-based Controls flow and Elmish
  adapter flow when Elmish program integration is selected
- generated products do not reference `FS.Skia.UI.Charts`, `charts`
  capability, stale chart-only guidance, framework samples, historical specs,
  readiness evidence, or framework implementation projects

## 6. Validate Readiness Evidence

```bash
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
./fake.sh build -t Ci
```

Expected readiness evidence under
`specs/011-controls-boundary-refactor/readiness/`:

- `public-surface.md`
- `package-boundary.md`
- `elmish-adapter.md`
- `keyboardinput-package.md`
- `control-catalog.md`
- `control-runtime.md`
- `rich-rendering.md`
- `keyboard-input-elmish.md`
- `chart-datagrid-controls.md`
- `generated-product-usage.md`
- `dependency-report.md`
- `template-drift.md`
- `compatibility-impact.md`
- `evidence-graph.md`
- `evidence-audit.md`

## 7. Compatibility Scope Check

Review `readiness/compatibility-impact.md`.

Expected outcome:

- Charts package/capability removal is documented as deliberate
- replacement path through Controls is documented for chart, graph, and
  DataGrid users
- no compatibility shim, automatic migration tool, or release publishing work
  is promised
- lower-level Scene, Layout, KeyboardInput, SkiaViewer, and Elmish usage paths
  remain documented for products that do not choose Controls
