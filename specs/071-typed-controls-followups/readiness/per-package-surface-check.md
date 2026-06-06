# Per-package surface check (071) — T018 (FR-010 / SC-007 / contract C11)

The implementation diff touches `src/Controls/Catalog.fs` (generated catalog rows)
but **no shipped public `.fsi` signature**. Both surface gates pass on the diff:

- `./fake.sh build -t PackageSurfaceCheck` — **Status: Ok**.
- `./fake.sh build -t PerPackageSurfaceDiff` — **Status: Ok** (no drift; the
  `FS.Skia.UI.Controls` per-package surface baseline delta is additive-only or empty).

No shipped public signature changed — the 41 typed modules and their `.fsi` shipped
in `070`. The only public runtime-data change is `custom-control.RequiredAttributes`
normalized to `[]` (FR-006), which is catalog data, not a signature.
