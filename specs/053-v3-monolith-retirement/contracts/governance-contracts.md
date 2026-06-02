# Phase 1 Contracts — V3 Stage 5 Closeout

This feature exposes **no new product/library public API** — it deletes one
(`FS.Skia.UI` monolith) and changes **governance/build contracts**. The contracts
below are the surfaces this stage must satisfy and that its tests assert.

## C1 — Routing contract: a public `.fsi` change Route-selects `PerPackageSurfaceDiff`

**Surface:** `build/Governance/Routing.fs` `package-surface` rule →
`validation.contract.yml` `routing_rules[package-surface].required_gates`.

**Contract:**
- Given a diff touching `src/<InScopePackage>/**/*.fsi`, `select` returns a selection
  whose `Gates` contains `Targets.PerPackageSurfaceDiff` (in addition to
  `PackageSurfaceCheck`, `FsiTranscripts`) and `Tier = FocusedAuthority`.
- `validation.contract.yml` (regenerated from `Routing.fs`) lists
  `PerPackageSurfaceDiff` under that rule's `required_gates`.
- `"PerPackageSurfaceDiff"` is on the `knownGates` allowlist; the contract validator
  accepts the regenerated file.
- `TargetMetadataDrift` reports zero drift (contract current vs `Routing.fs`).

**Test (`tests/Governance.Tests/RoutingTests.fs`):** the existing
"`src/**/*.fsi` escalates" test asserts `Expect.contains selection.Gates
Targets.PerPackageSurfaceDiff`; its diff input is a live package path
(`src/Scene/Foo.fsi`), not the deleted `src/Lib/Foo.fsi`.

**Enforcement evidence (SC-004):** a real unrecorded one-package `.fsi` edit fails
`PerPackageSurfaceDiff`; updating that package's baseline clears it; both reverted.

## C2 — Pack contract: `FS.Skia.UI` is no longer packed or published

**Surface:** `build/Governance/Front/Helpers.fs` `packProjects`; `PackLocal` flow;
`docs/reports/dependencies.md`.

**Contract:**
- `packProjects` contains the nine split packages + `FS.Skia.UI.Build`, and **no**
  `("src/Lib/Lib.fsproj","FS.Skia.UI")` entry.
- No `Directory.Packages.props` (root or `template/base`) pin names `FS.Skia.UI`.
- `docs/reports/dependencies.md` names no `FS.Skia.UI` monolith row/leak note.

**Test (`tests/Package.Tests/Tests.fs`, rewritten):** the packaging-contract suite
asserts the split-package pack shape and contains **no** `src/Lib/Lib.fsproj` /
`FS.Skia.UI` monolith expectation; it still asserts a real packaging contract (e.g.
the negative "Controls does not depend on `..\Lib\Lib.fsproj`" survives and the
split-package pack entries are present).

## C3 — Cleanliness contract: a generated default `app` is a clean consumer

**Surface:** `build/Governance/GeneratedProduct.fs` (`GeneratedProductCheck`
extension).

**Contract — a generated default `app` (and `governed` profile):**
- contains **no** `samples/`, **no** framework documentation set, **no** historical
  `specs/`, **no** framework `README` copy;
- **references** the split packages rather than copying framework projects.

**Negative contract:** the gate **fails** (naming the offending artifact) if any of
those are planted into a generated product. (FR-008 / SC-005.)

## C4 — Deletion contract: zero monolith references repo-wide

**Surface:** the whole repository (`src/** samples/** tests/** template/** build/**`,
`Directory.Packages.props`, `FS-Skia-UI.sln`).

**Contract:** a named grep for `Lib.fsproj`, `src/Lib`, and the `FS.Skia.UI` monolith
package (by `ProjectReference`/`PackageReference`/`PackageId`/`PackLocal`/path-string)
returns **zero** hits outside programme history/docs that intentionally record the
retirement. `src/Lib` does not exist on disk and is not in the solution. (SC-001.)

## C5 — Graph-invariant contract (unchanged, re-verified)

**Contract:** the package dependency graph stays acyclic and `FS.Skia.UI.Scene` stays
FSharp.Core-only — verified from project references (no back-edge, no new heavy
dependency introduced). (SC-008 / FR-015.) No edit this stage adds a reference.
