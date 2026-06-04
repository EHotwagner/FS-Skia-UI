# Contract: API-Surface Emission (FR-003)

## Producer
`FS.Skia.UI.Build` api-surface generator, run as part of `RefreshSurfaceBaselines`.

## Input (single source)
`template/capabilities.yml` — each `capabilities[]` entry's `packageId`, `contracts[]`
(`.fsi` paths), and `profiles[]`.

## Output
For every in-profile capability contract, write
`template/base/docs/api-surface/<PkgLeaf>/<sourceFileName>.fsi` whose bytes equal the
source `.fsi`. Example mapping:

| Source contract              | Emitted path                                        |
|------------------------------|-----------------------------------------------------|
| `src/Scene/Scene.fsi`        | `template/base/docs/api-surface/Scene/Scene.fsi`    |
| `src/KeyboardInput/KeyboardInput.fsi` | `.../api-surface/KeyboardInput/KeyboardInput.fsi` |
| `src/Elmish/Elmish.fsi`      | `.../api-surface/Elmish/Elmish.fsi`                 |
| `src/SkiaViewer/SkiaViewer.fsi` | `.../api-surface/SkiaViewer/SkiaViewer.fsi`      |
| `src/Testing/Testing.fsi`    | `.../api-surface/Testing/Testing.fsi`               |
| `src/Layout/*.fsi` (4)       | `.../api-surface/Layout/<name>.fsi`                 |
| `src/Controls/*.fsi`         | `.../api-surface/Controls/<name>.fsi`               |

## Currency invariant
Re-running the generator produces no diff (no-drift). A stale tree fails the currency
gate (same pattern as `SkillSyncCheck`/`TargetMetadataDrift`).

## Template inclusion
`.template.config/.../template.json` includes `docs/api-surface/**`. The default `app`
profile emits the surface for the packages it pins; profile conditionals follow the
`capabilities[].profiles` set.

## Acceptance
- SC-002: every product-skill's claimed contract path exists in a freshly generated
  project; verified by `generated-project/api-surface.log`.
- The emitted `.fsi` content matches the source signatures (no DLL reflection needed).
