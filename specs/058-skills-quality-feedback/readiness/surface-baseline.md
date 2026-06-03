# Per-Package Surface Baseline Evidence (T016)

`FS.Skia.UI.SkillSupport` is the 10th in-scope package
(`PerPackageSurface.packagesInScope` + `packageSourceDir "FS.Skia.UI.SkillSupport" ->
"SkillSupport"`). Its source lives at `src/SkillSupport/*.fsi`.

The normalized `.fsi` surface was captured via the real
`PerPackageSurface.captureCurrent` edge to
`readiness/per-package-surface/FS.Skia.UI.SkillSupport.fsi.txt` (the five family modules:
`Graph`, `Parsing`, `Globbing`, `CodeGen`, `ShellProcess`).

`./fake.sh build -t PerPackageSurfaceDiff` confirms the captured baseline matches the live
surface with **zero drift across the ten packages** (the re-pointed `PerPackageSurfaceTests`
load 10 committed baselines and capture 10 surfaces, `MissingBaselines` empty, `Drifted`
empty). Visibility lives in the `.fsi` only (Principle II); the surface is additive — no
existing public `.fsi` signature changed.
