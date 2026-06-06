# Package-surface expectations (FS.Skia.UI.Controls)

## Expected delta: additive-only

The `070` migration adds the 41 remaining typed controls to the
`FS.Skia.UI.Controls.Typed` namespace inside the existing `FS.Skia.UI.Controls`
package. The change is **additive-only**:

- **Added**: nine new compile units (`Widgets/Display`, `Input`, `TextAreaWidget`,
  `CollectionsWidgets`, `Containers`, `Navigation`, `Overlay`, `ChartsWidgets`,
  `CustomControlWidget`), each declaring its `Props` records and a module per
  catalog id with `defaults` + `view` (+ `init`/`update` for the stateful groups).
  This contributes **81 added per-package surface lines** in
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt`.
- **Removed / renamed / changed**: **none.** The legacy string-keyed modules
  (`Button`, `TextBox`, `LineChart`, `Stack`, …) and the `065` typed seam
  (`Widget`, `Primitives`, `TextBoxWidget`, `DataGridWidget`) are unchanged.

## Regenerated-baseline rationale

The per-package surface baseline is regenerated via
`./fake.sh build -t RefreshSurfaceBaselines` (never hand-edited). The committed
`readiness/surface-baselines/FS.Skia.UI.Controls.txt` diff shows **zero `-` lines
and only `+` lines** for the new `Typed.*` modules — confirming the additive-only
contract (SC-004). `PackageSurfaceCheck` / `PerPackageSurfaceDiff` gate this delta;
the legacy peer is byte-frozen (SC-009).

No new package dependency is added (FR-008/SC-006): `Controls.fsproj` references
only `Scene`, `Layout`, `KeyboardInput` — in particular not `Fable.Elmish`.
