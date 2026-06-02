# Package-surface expectations — the public surface baseline policy

Every shipped package's public API surface is pinned by a captured baseline and validated by
`PackageSurfaceCheck` (aggregate, `readiness/surface-baselines/<Package>.txt`) and
`PerPackageSurfaceDiff` (per split package, `readiness/per-package-surface/<Package>.fsi.txt`). A
change to a public `.fsi` (or to a baseline) escalates via `Routing.fs` and must re-pass these gates.

## Expectations

- A public surface change is only accepted when its baseline is updated in the **same** change and the
  `PackageSurfaceCheck`/`PerPackageSurfaceDiff` reflection diff comes back **clean**.
- Visibility lives in the `.fsi`, never as `private`/`internal`/`public` on a top-level binding
  (Principle II).
- **Build-tooling is excluded**: `FS.Skia.UI.Build` (`build/Governance/**`) is never shipped, so it has
  no surface baseline. Relocating a governance module *out* of a shipped package therefore shrinks that
  package's baseline (the surface leaves the product) and adds **no** build-tooling baseline.
- The eight runtime split-package baselines are byte-stable unless their own `.fsi` changes.

See `readiness/per-package-surface-expectations.md` for the `PerPackageSurfaceDiff` capability detail.
Per-feature surface deltas and clean-diff evidence live under `specs/<feature>/readiness/`.
