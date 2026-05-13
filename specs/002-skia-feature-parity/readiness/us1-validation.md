# US1 Drawing Coverage Validation

Task scope: render baseline drawing coverage for primitive elements, groups, images, points, vertices, arcs, reusable pictures, nested scenes, paint/effect declarations, clipping, regions, color handling, transforms, and diagnostics.

## Public Entry Evidence

| Evidence | Command | Result |
|----------|---------|--------|
| US1 prelude transcript | `dotnet fsi scripts/us1-prelude.fsx` | `readiness/transcripts/t026-us1-prelude.txt` reports public scene kinds, path measurement, and deterministic readback hash. |
| Diagnostics prelude transcript | inline FSI using `FS.Skia.UI.dll` | `readiness/transcripts/t028-diagnostics-prelude.txt` reports missing-image, malformed-path, and unavailable-font diagnostics through `Scene.diagnostics`. |
| Library tests | `dotnet test tests/Lib.Tests/Lib.Tests.fsproj --no-restore` | `readiness/logs/t028-lib-tests-rerun.txt` passes 29 tests, including US1 semantic constructors, paint/effect diagnostics, and render-readback evidence. |
| BasicViewer effects smoke | `scripts/us1-vulkan-smoke.sh specs/002-skia-feature-parity/readiness/smoke/t027-basicviewer-effects-smoke.txt` | Real Vulkan first-frame render reached `drawing model-derived scene into Skia Vulkan surface`; no fallback renderer used. |
| ParityGallery contract smoke | `dotnet run --project samples/ParityGallery/ParityGallery.fsproj -- --contract-smoke` | `readiness/smoke/t029-paritygallery-contract.txt` reports 15 public scene capability categories. |
| EffectsGallery contract smoke | `dotnet run --project samples/EffectsGallery/EffectsGallery.fsproj -- --contract-smoke` | `readiness/smoke/t029-effectsgallery-contract.txt` reports 11 public scene capability categories. |
| ParityGallery Vulkan smoke | `dotnet run --project samples/ParityGallery/ParityGallery.fsproj -- --smoke` | `readiness/smoke/t029-paritygallery-vulkan.txt` renders one Vulkan frame, `fallback-used=false`, first frame 312 ms. |
| EffectsGallery Vulkan smoke | `dotnet run --project samples/EffectsGallery/EffectsGallery.fsproj -- --smoke` | `readiness/smoke/t029-effectsgallery-vulkan.txt` renders one Vulkan frame, `fallback-used=false`, first frame 326 ms. |

## Gallery Coverage

`samples/ParityGallery` covers rectangles, ellipses, lines, paths, points, vertices, arcs, image declarations, clipping, regions, reusable pictures, text, and chart composition in a single model-derived scene.

`samples/EffectsGallery` covers gradients, blend modes, color filters, mask filters, image filters, path effects, path clipping, text runs, perspective transforms, and color-space scene wrappers.

## Status

US1 has real local evidence for the Vulkan rendering path on this workstation. Screenshot files are not required for this checkpoint because the accepted validation path is render evidence under `readiness/smoke/` plus deterministic readback artifacts under `readiness/screenshots/`.
