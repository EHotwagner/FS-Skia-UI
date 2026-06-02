# Per-package surface expectations

The additive `PerPackageSurfaceDiff` capability (feature 048, FR-007/008) compares each
of the eight public split packages' captured baseline against its current normalized
`.fsi` surface and reports drift per package.

## Route-gating deferred (runtime-coupling finding)

A Stage-0 `per-package-surface` **Routing rule was intentionally not added**. A rule would
render `PerPackageSurfaceDiff` into `validation.contract.yml`'s `routing_rules`, and the
contract validator's known-gate allowlist lives in the **runtime monolith**
(`src/Lib/AgentValidation.fs` `knownGates`). Teaching it the new gate would modify runtime
code, violating this feature's defining constraint — Stage 0 is record-and-oracle only,
`src/**` byte-unchanged (FR-010/SC-007). So the target stays **additive and runnable
directly** (`./fake.sh build -t PerPackageSurfaceDiff`, the escalated gate set, and the
quickstart); **Route-gating it is deferred with the Stage-5 hard-gate enforcement** (which
the capability contract already defers). When Stage 2 relocates `AgentValidation` into the
governance library (ADR 0009), the known-gate allowlist becomes governance config and the
rule can be added without touching runtime code.

## Packages in scope (8)

`FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Elmish`,
`FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Layout`, `FS.Skia.UI.Controls`,
`FS.Skia.UI.Controls.Elmish`, `FS.Skia.UI.Testing`.

**Excluded:** the retiring monolith `FS.Skia.UI` (`src/Lib`) and the build-tooling
library `FS.Skia.UI.Build`.

## Baselines

- Stored at `readiness/per-package-surface/<PackageId>.fsi.txt`, one per package.
- Each baseline is the **normalized full `.fsi` surface text** of the package
  (comments stripped, trailing whitespace trimmed, blank-line runs collapsed, newlines
  `\n`, declaration order preserved). Multi-file packages (`Layout`, `Controls`)
  aggregate their `.fsi` files in filename order.

## Contract

- **Clean** ⇔ `Drifted = []` **and** `MissingBaselines = []`. The `PerPackageSurfaceDiff`
  FAKE target fails on any non-clean verdict.
- A package in scope with **no baseline** is reported in `MissingBaselines` and **fails**
  — it is never silently treated as clean (Principle VII).
- Drift is reported for **exactly** the affected package(s); an unrelated package shows
  no drift (FR-008 / SC-005).
- The run report is written to
  `specs/048-v3-retirement-baseline/readiness/per-package-surface-diff.md`.

## Non-goals (this feature)

- **No merge-gate enforcement** — the target runs green and additive; promoting
  per-package drift to a hard merge gate is programme **Stage 5** (deferred).
- **No change** to the existing aggregate `PackageSurfaceCheck`,
  `readiness/surface-baselines/`, or the `package-surface` Routing rule (FR-011).
