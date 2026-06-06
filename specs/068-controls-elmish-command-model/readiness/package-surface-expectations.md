# Package-surface expectations — Controls.Elmish command model (068)

Feature-specific surface delta required by the `package-surface` routing rule
(`build/Governance/Routing.fs`, paths `src/**/*.fsi`), which matches
`src/Controls.Elmish/ControlsElmish.fsi`. The change is **additive-only** to the single
shipped `FS.Skia.UI.Controls.Elmish` package; no other package baseline changes (FR-007 /
SC-006).

## Regeneration

- Command: `./fake.sh build -t RefreshSurfaceBaselines` (regenerates the reflection
  baseline `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`). The raw
  per-package snapshot `readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`
  is the normalized `.fsi` text, regenerated from `PerPackageSurface.captureCurrent`.
- Reviewed gates (printed by `Route` for this `src/**/*.fsi` change): `PackageSurfaceCheck`
  (aggregate, `readiness/surface-baselines/`), `FsiTranscripts` (loads the new symbols from
  the packed library), and `PerPackageSurfaceDiff` (raw `.fsi` snapshot).

## Intentional additive delta (additive, 0 removed)

Reflection baseline (`readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`):

- `+ FS.Skia.UI.Controls.Elmish.AdapterCmd` — the new bridge module (a new type/module
  row). `widgetView`/`programOfWidget` are members added to the existing
  `ControlsElmish` module, so they add no new top-level type row.

Per-package `.fsi` snapshot (`readiness/per-package-surface/FS.Skia.UI.Controls.Elmish.fsi.txt`):

- `+ open Elmish` — names `Cmd<'msg>` from the already-referenced `Fable.Elmish`.
- `+ module AdapterCmd` with `val none`, `val ofMessage`, `val productMessages`, `val toCmd`.
- `+ ControlsElmish.widgetView: ('model -> Widget<'msg>) -> ('model -> Control<'msg>)`.
- `+ ControlsElmish.programOfWidget: … view: ('model -> Widget<'msg>) … -> AdapterProgram<…>`.

## Why the diff is safe

- **Zero removed lines** — every existing type/member is byte-stable: `AdapterDiagnostic`,
  `AdapterEffect`, `AdapterCommand`, `AdapterSubscription`, `AdapterProgram` (its `View`
  field stays `'model -> Control<'msg>`), and `ControlsElmish.program` /
  `interpretKeyboardEffect` / `interpretControlEffect` / `subscriptions` / `diagnostic`
  are unchanged (FR-002 / SC-004). Every existing consumer compiles with no source edit.
- The delta is confined to `FS.Skia.UI.Controls.Elmish`; the other nine in-scope
  per-package baselines and every other reflection baseline are unchanged (SC-006).
- No new dependency: `Cmd<'msg>` comes from the already-referenced `Fable.Elmish`. The base
  `FS.Skia.UI.Controls` package adds no `Fable.Elmish` reference (FR-006 / SC-005),
  asserted by the dependency guard in `ControlsElmishAdapterContractTests.fs` and
  `TypedControlsAdapterTests.fs`.
