# Surface invariant (071) — T004

**Invariant**: no shipped public `FS.Skia.UI.Controls` `.fsi` signature changes.

- The 41 typed modules (`FS.Skia.UI.Controls.Typed.*`) and their `.fsi`
  signatures shipped in `070`; this feature does not add, remove, or alter a
  shipped public signature.
- `catalog.yml` / `Catalog.fs` and the `CatalogGen` fact table are
  **generated/internal cross-check inputs**, not public surface. Regenerating
  them changes no `.fsi`.
- Therefore the `FS.Skia.UI.Controls` per-package surface baseline delta MUST be
  **additive-only or empty** (FR-010, SC-007, contract C11). Verified by
  `PackageSurfaceCheck` / `PerPackageSurfaceDiff` in T018.
- `custom-control` keeps its bridge-typed treatment (`Widget.ofControl`, `070`
  FR-006) — no fabricated required attribute (FR-006), `RequiredAttributes = []`.
