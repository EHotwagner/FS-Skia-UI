# Surface baseline diff

command: `./fake.sh build -t PerPackageSurfaceDiff` + `./fake.sh build -t PackageSurfaceCheck`
artifact path: this file + `readiness/per-package-surface/**` + `readiness/surface-baselines/**`.
failure class: SurfaceBaselineDrift.
next action: none — both surface gates are clean.

## Per-package surface (`PerPackageSurfaceDiff`)

- Scope grew 8 → **9** packages: `FS.Skia.UI.Input` registered in
  `build/Governance/PerPackageSurface.fs` (`packagesInScope` + `packageSourceDir`); the
  `PerPackageSurfaceTests` count assertions and `per-package-surface-expectations.md` updated to nine.
- New baseline `readiness/per-package-surface/FS.Skia.UI.Input.fsi.txt` (normalized `src/Input/KeyboardInput.fsi`).
- `PerPackageSurfaceDiff` → **Status Ok**, zero drift across nine packages (FR-009, SC-006).

## Aggregate reflection surface (`PackageSurfaceCheck`)

- `readiness/surface-baselines/FS.Skia.UI.txt` shrank to **7** names (the `Parity` helper:
  `EvidenceType(+Tags)`, `Parity`, `ParityEvidenceItem`, `ParityReport`, `ParityStatus(+Tags)`) — the
  rich `KeyboardInput` types are gone from the monolith assembly.
- New `readiness/surface-baselines/FS.Skia.UI.Input.txt` (76 names, the rich runtime under
  `FS.Skia.UI.Input.*`). `FS.Skia.UI.Input` added to `packProjects` (Helpers.fs).
- `PackageSurfaceCheck` → **Status Ok**.

## Contract currency

- `validation.contract.yml` is **unchanged** (no `Routing.fs` rule change; the per-package Route-gating
  rule is Stage 5).
