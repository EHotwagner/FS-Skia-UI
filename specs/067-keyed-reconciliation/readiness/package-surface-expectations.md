# Package-surface expectations — internal keyed reconciliation (067)

Feature-specific surface delta for the `package-surface` /
`controls-public-surface` rules. The change adds **zero** public surface to the
single shipped `FS.Skia.UI.Controls` package; no package baseline changes
(SC-005, FR-002).

## Regeneration

- Command: `./fake.sh build -t RefreshSurfaceBaselines` (re-run; baseline
  byte-stable).
- Reviewed gate: `PackageSurfaceCheck` (aggregate,
  `readiness/surface-baselines/FS.Skia.UI.Controls.txt`).

## Intentional delta: 0 added, 0 removed

The reconciler ships as `module internal FS.Skia.UI.Controls.Reconcile`. Because
it is declared `module internal` **and** is deliberately excluded from the
Controls capability `contracts:` `.fsi` list, `ApiSurfaceGen` reads no new entry
and the regenerated baseline is byte-for-byte identical to the prior commit.

- **Zero added lines** — `Reconcile.fsi`'s `FieldChange`, `AttrChange`,
  `NodePatch`, `UpdatePatch`, `ChildOp`, `ReconcileResult`, `diff`, and `apply`
  are all assembly-internal and never reach the public surface.
- **Zero removed lines** — every existing legacy module/type/member is
  byte-stable; no public `.fsi` was edited. All eight shipped split-package
  baselines are unchanged.

## Raw per-package `.fsi` snapshot (PerPackageSurfaceDiff)

`PerPackageSurfaceDiff` is a **separate, stricter** gate from `PackageSurfaceCheck`:
it captures the raw concatenated text of *every* `src/Controls/*.fsi` (comments
stripped, normalized), not just the `contracts:`-listed public api-surface. Adding
the new internal `Reconcile.fsi` therefore appends its normalized signature to the
raw snapshot `readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt` (+38
lines, 0 removed). This is the gate's intended "a `.fsi` changed — review and
refresh the baseline" flow; it is **not** a public-surface delta. The public
api-surface contract (`PackageSurfaceCheck` / `surface-baselines/*.txt`, computed
from the `contracts:` list) is byte-stable, so **SC-005 still holds**.

## Why the diff is safe

- `module internal` gives genuine assembly-internal accessibility — the symbols
  are unreachable from consumers, so "internal only" is enforced (FR-002), not a
  documentation claim.
- The only test-assembly coupling is the SDK-generated
  `InternalsVisibleTo("Controls.Tests")` attribute (declared as an
  `<InternalsVisibleTo>` MSBuild item in `Controls.fsproj`, not a source file).
  An `InternalsVisibleTo` attribute is not a public type/member, so it
  contributes nothing to the api-surface baseline.
- No new product dependency (FR-013): `Controls.fsproj` adds no
  `PackageReference`, so no package's dependency surface changes. The added
  `FsCheck` reference is test-only (`Controls.Tests.fsproj`).
