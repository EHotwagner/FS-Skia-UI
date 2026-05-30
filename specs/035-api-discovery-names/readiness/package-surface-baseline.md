# Package Surface Baseline Evidence

Status: pass.

Surface baseline paths reviewed:

- `readiness/surface-baselines/FS.Skia.UI.txt`
- `readiness/surface-baselines/FS.Skia.UI.Scene.txt`
- `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt`
- `readiness/surface-baselines/FS.Skia.UI.Elmish.txt`
- `readiness/surface-baselines/FS.Skia.UI.KeyboardInput.txt`
- `readiness/surface-baselines/FS.Skia.UI.Layout.txt`
- `readiness/surface-baselines/FS.Skia.UI.Controls.txt`
- `readiness/surface-baselines/FS.Skia.UI.Controls.Elmish.txt`
- `readiness/surface-baselines/FS.Skia.UI.Testing.txt`

Public contract summary:

- No new public `.fsi` signatures were changed for this feature.
- Controls collision safety uses existing `[<RequireQualifiedAccess>]`
  contracts in `src/Controls/Types.fsi`.
- Package API reference material is package-adjacent readiness output generated
  from curated `.fsi` files; it does not change compiled public symbols.

Command result:

- `./fake.sh build -t PackageSurfaceCheck` passed.
- Log path: `readiness/logs/package-surface-check.txt`.

Reconciliation notes:

- No surface baseline entry required refresh for US1 or US2.
- Package reference artifacts were regenerated under
  `specs/035-api-discovery-names/readiness/package/api-reference/`.
