# Per-package surface diff (FR-011 / SC-007)

**Verdict:** clean against the updated baselines.

- Command: `./fake.sh build -t PerPackageSurfaceDiff` -> `Status: Ok` (zero drift in scope).
- Aggregate `./fake.sh build -t PackageSurfaceCheck` -> `Status: Ok`.

Surface deltas recorded and justified:

- `FS.Skia.UI.SkiaViewer` per-package `.fsi` surface (`readiness/per-package-surface/FS.Skia.UI.SkiaViewer.fsi.txt`)
  is **unchanged**: the moved host lives under `src/SkiaViewer/Host/*.fsi`, which the per-package
  capture (top-level `src/SkiaViewer/*.fsi` only) does not include, and `SkiaViewer.fsi`'s outward
  signatures are preserved (the wrapper already re-exposed the host API).
- `FS.Skia.UI.Scene` per-package baseline (`readiness/per-package-surface/FS.Skia.UI.Scene.fsi.txt`)
  was regenerated to add the canonical `Paint` (`withAntialias`/`withStrokeJoin`/`withMiter`/
  `withShader`/`withColorFilter`/`withMaskFilter`/`withImageFilter`) and `Path`
  (`quadTo`/`cubicTo`/`bounds`/`measure`/`segment`/`combine`) functions the retyped host, parity
  seeds, and repointed samples require. Scene stays FSharp.Core-only.
- Aggregate reflection baselines (`readiness/surface-baselines/`): `FS.Skia.UI.txt` regenerated via
  `scripts/refresh-surface-baselines.fsx` (scene/host/viewer types removed; `KeyboardInput` +
  `AgentValidation` + `Parity` retained). The `FS.Skia.UI.SkiaViewer.Host.*` types are public package
  surface (consumed by samples/tests) but are not part of the curated SkiaViewer subset baseline.
