# Contract — `PerPackageSurfaceDiff` capability

The new, **additive** per-package surface-diff capability (FR-007/008). It does **not** replace or
weaken the existing aggregate `PackageSurfaceCheck` (FR-011).

## Public surface (curated `.fsi`, `build/Governance/PerPackageSurface.fsi`)

```fsharp
module FS.Skia.UI.Build.PerPackageSurface

type PackageId = string

type Surface =
    { PackageId: PackageId
      NormalizedText: string }

type SurfaceLineChange =
    | Added of string
    | Removed of string

type PackageDrift =
    { PackageId: PackageId
      Changes: SurfaceLineChange list }

type DiffOutcome =
    { Drifted: PackageDrift list
      CheckedPackages: PackageId list
      MissingBaselines: PackageId list }

/// The 8 public split packages in scope (monolith + FS.Skia.UI.Build excluded).
val packagesInScope: PackageId list

/// Normalize raw .fsi text deterministically (comments/whitespace/order rules).
val normalize: raw: string -> string

/// Diff one package's current surface against its baseline. None ⇒ zero drift.
val diffPackage: baseline: Surface -> current: Surface -> PackageDrift option

/// Diff all current surfaces against baselines. Pure; no I/O.
val diff: baselines: Surface list -> current: Surface list -> DiffOutcome

/// Edge: read each package's .fsi file(s) from the source tree and normalize.
val captureCurrent: packages: PackageId list -> Surface list

/// Edge: load committed baselines from readiness/per-package-surface/.
val loadBaselines: directory: string -> Surface list

/// Edge: write the per-package drift report; returns true when clean (no drift, no missing).
val runReport: reportPath: string -> outcome: DiffOutcome -> bool
```

## Inputs

- **Baselines**: `readiness/per-package-surface/<PackageId>.fsi.txt`, one per package in
  `packagesInScope`.
- **Current surfaces**: each package's `.fsi` file(s) under `src/<Package>/`. `Controls` aggregates
  its multiple `.fsi` files in filename order.

## Outputs

- **Report**: `specs/048-v3-retirement-baseline/readiness/per-package-surface-diff.md` — per-package
  drift, or an explicit "zero drift across N packages" line.
- **Verdict**: clean ⇔ `Drifted = []` **and** `MissingBaselines = []`. The `PerPackageSurfaceDiff`
  FAKE target fails on a non-clean verdict.

## Drift semantics

- A package drifts when its normalized current `.fsi` text differs from its baseline by ≥1 line
  (DiffPlex line diff). Drift lists the `Added`/`Removed` normalized lines.
- A package in scope with **no baseline file** is reported in `MissingBaselines` and **fails** the
  check — it is never silently treated as clean (Principle VII).
- Drift is reported for **exactly** the affected package(s); an unrelated package shows no drift
  (FR-008/SC-005).

## Acceptance (mapped to success criteria)

- **SC-004**: at the pin, `diff (loadBaselines …) (captureCurrent packagesInScope)` ⇒ `Drifted = []`,
  `MissingBaselines = []`; `PerPackageSurfaceDiff` target green.
- **SC-005**: a single experimental public-signature edit in one package ⇒ `Drifted` has exactly one
  `PackageDrift` (that package), zero others. Demonstrated by a scratch edit, reverted; evidence in
  `readiness/seeded-violation.md`.

## Non-goals (this feature)

- **No merge-gate enforcement**: the target runs green and additive; promoting per-package drift to a
  hard merge gate is programme **Stage 5** (deferred).
- **No change** to `PackageSurfaceCheck`, `readiness/surface-baselines/`, or the `package-surface`
  Routing rule.
