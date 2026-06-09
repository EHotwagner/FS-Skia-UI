// PerPackageSurface.fsi — curated public surface of the additive per-package
// surface-diff capability (feature 048, FR-007/008; Principle II).
//
// Build-tooling only. The capability is a PURE comparison (`normalize`/`diffPackage`/
// `diff`) over captured `.fsi` text, with file reads/writes confined to a thin edge
// interpreter (`captureCurrent`/`loadBaselines`/`runReport`) — no `Model`/`Msg`/`Cmd`
// (research.md D6, Principle IV). It is strictly ADDITIVE: it neither replaces nor
// weakens the existing aggregate `PackageSurfaceCheck` (FR-011).
module FS.Skia.UI.Build.PerPackageSurface

/// A package's NuGet PackageId (e.g. "FS.Skia.UI.Scene").
type PackageId = string

/// A captured or current normalized public surface for one package.
type Surface =
    { PackageId: PackageId
      NormalizedText: string }

/// A single normalized-line delta between a baseline and a current surface.
type SurfaceLineChange =
    | Added of string
    | Removed of string

/// Non-empty only when a package's current surface differs from its baseline.
type PackageDrift =
    { PackageId: PackageId
      Changes: SurfaceLineChange list }

/// The whole-run outcome: which packages drifted, which were checked, and which are
/// in scope but have no committed baseline (the never-silently-pass case, Principle VII).
type DiffOutcome =
    { Drifted: PackageDrift list
      CheckedPackages: PackageId list
      MissingBaselines: PackageId list }

/// The 8 public split packages in scope. The monolith `FS.Skia.UI` and the build-tooling
/// `FS.Skia.UI.Build` are deliberately excluded (data-model.md, plan §Packages-in-scope).
val packagesInScope: PackageId list

/// Normalize raw `.fsi` text deterministically: strip `//` line and `(* *)` block
/// comments, trim trailing whitespace, collapse blank-line runs, normalize newlines to
/// `\n`, preserve written declaration order. Idempotent.
val normalize: raw: string -> string

/// Diff one package's current surface against its baseline. `None` ⇒ zero drift.
val diffPackage: baseline: Surface -> current: Surface -> PackageDrift option

/// Diff all current surfaces against baselines. Pure; no I/O. A current package with no
/// baseline lands in `MissingBaselines` and is never treated as clean (Principle VII).
val diff: baselines: Surface list -> current: Surface list -> DiffOutcome

/// FR-005/006: the canonical byte-idempotent on-disk form of a baseline — the normalized
/// surface text plus exactly one trailing newline. Serializing twice (even after a
/// read-and-renormalize round-trip) is byte-identical, so a re-capture on an unchanged tree
/// produces no trailing-newline/whitespace churn (SC-006). Pure.
val serializeBaseline: surface: Surface -> string

/// Edge: capture every in-scope package's current surface and write each as a byte-idempotent
/// baseline at `<directory>/<PackageId>.fsi.txt`. Returns the written package ids in scope
/// order. One run regenerates all per-package baselines; a second run on an unchanged tree
/// rewrites byte-equal files (FR-005/006).
val captureBaselines: directory: string -> packages: PackageId list -> PackageId list

/// Edge: read each in-scope package's `.fsi` file(s) from the source tree and normalize,
/// aggregating a package's multiple `.fsi` files in filename order. Discovers the
/// repository root by walking up for the `src` + `.specify/feature.json` markers.
val captureCurrent: packages: PackageId list -> Surface list

/// Edge: load committed baselines from `readiness/per-package-surface/<PackageId>.fsi.txt`.
val loadBaselines: directory: string -> Surface list

/// Edge: write the per-package drift report; returns `true` when clean (no drift and no
/// missing baseline). Fails loud with the package, the added/removed lines, and the
/// baseline path on drift.
val runReport: reportPath: string -> outcome: DiffOutcome -> bool
