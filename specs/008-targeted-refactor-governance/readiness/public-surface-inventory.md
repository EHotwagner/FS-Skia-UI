# Public Surface Inventory

## Stable Contract Inputs

| Surface | Path | Feature Decision |
|---------|------|------------------|
| Core public signature | `src/Lib/Library.fsi` | Must remain stable for this feature. |
| Keyboard input signature | `src/Lib/KeyboardInput.fsi` | Inventory-only; no public input API change planned. |
| Layout signatures | `src/Layout/Types.fsi`, `src/Layout/Layout.fsi`, `src/Layout/Graph*.fsi` | Yoga diagnostics must use existing fields. |
| Charts signatures | `src/Charts/*.fsi` | Inventory-only; no chart API change planned. |
| Package surface baselines | `readiness/surface-baselines/*.txt` | Must remain unchanged unless a separate public API spec approves it. |
| Samples | `samples/*/*.fsproj` and `--contract-smoke` paths | Must keep compiling against existing public contracts. |
| Package tests | `tests/Package.Tests/SurfaceAreaTests.fs` | Must fail on missing expected exports and unapproved helper-module exports. |

## Current Baseline Files

- `readiness/surface-baselines/FS.Skia.UI.txt`
- `readiness/surface-baselines/FS.Skia.UI.Layout.txt`
- `readiness/surface-baselines/FS.Skia.UI.Charts.txt`

## Guardrail

Implementation helpers introduced for this feature must either stay private to
existing implementation files or be declared as assembly-internal helper modules
behind paired `.fsi` files. No helper module may appear in package exported type
baselines.
