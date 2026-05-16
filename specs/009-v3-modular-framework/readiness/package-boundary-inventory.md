# Package Boundary Inventory

Date: 2026-05-16

## Current Runtime Packages

| Package | Project | Current role | V3 disposition |
|---------|---------|--------------|----------------|
| `FS.Skia.UI` | `src/Lib/Lib.fsproj` | Broad core package containing scene primitives, Elmish viewer helpers, Vulkan/Skia host startup, and keyboard input. | Split into Scene, SkiaViewer, Elmish, and KeyboardInput capability packages while preserving compatibility during staging. |
| `FS.Skia.UI.Layout` | `src/Layout/Layout.fsproj` | Yoga-backed layout and graph scene builders. | Retarget to depend on the Scene capability instead of broad core ownership. |
| `FS.Skia.UI.Charts` | `src/Charts/Charts.fsproj` | Chart and DataGrid scene builders. | Retarget to depend on the Scene capability instead of broad core ownership. |

## Current Test Assets

| Area | Paths |
|------|-------|
| Core runtime | `tests/Lib.Tests/` |
| Layout | `tests/Layout.Tests/` |
| Charts | `tests/Charts.Tests/` |
| Package surface | `tests/Package.Tests/` |
| Repository governance | `tests/Governance.Tests/` |
| Smoke and parity | `tests/Smoke.Tests/`, `tests/Parity.Tests/` |

## Current Surface Baselines

| Baseline | Notes |
|----------|-------|
| `readiness/surface-baselines/FS.Skia.UI.txt` | Broad core surface baseline. |
| `readiness/surface-baselines/FS.Skia.UI.Layout.txt` | Layout package surface baseline. |
| `readiness/surface-baselines/FS.Skia.UI.Charts.txt` | Charts package surface baseline. |

## V3 Gap Summary

- Scene is not yet independently packable.
- Skia viewer hosting, Elmish integration, and keyboard input are currently owned by `src/Lib`.
- Layout and Charts still reference `src/Lib` rather than a base Scene package.
- Package surface validation is broad-package oriented and needs package-specific V3 baselines.
- Capability-owned skills do not yet exist beside each package root.
