# Surface baseline unchanged (FR-004 / SC-007) — feature 076

Adding `///` XML doc comments to `.fsi` signature files changes doc text only.
The surface-baseline normalizer (`build/Governance/PerPackageSurface.fs`) strips
`///` / `//` line comments and `(* *)` blocks before hashing, so no surface
baseline moves and no `.fsi` signature shape changes.

- **Authoritative commands**: `./fake.sh build -t PackageSurfaceCheck` and
  `./fake.sh build -t PerPackageSurfaceDiff`.
- **Artifact path**: this file + `readiness/logs/` build logs.
- **Failure class**: surface-baseline drift (would mean a signature shape changed,
  not just doc text — investigate the offending `.fsi`).
- **Next action**: if a diff appears, confirm only comments were added; a real
  shape change is out of scope for this feature (record as follow-up, do not make
  it here).

## Result

**STATUS: PASS — contract unchanged (SC-007 / FR-004 held).**

```
./fake.sh build -t PackageSurfaceCheck    -> EXIT 0  (aggregate surface hash unchanged)
./fake.sh build -t PerPackageSurfaceDiff  -> EXIT 0  (after baseline recapture, see below)
./fake.sh build -t DesignTokenDrift       -> Status: Ok  (generated tokens vs DTCG source unchanged)
```

### Per-package baseline recapture (cosmetic, non-contract)

The first `PerPackageSurfaceDiff` run flagged `FS.Skia.UI.Controls` drift =
**exactly 20 added blank lines**, with **zero** signature/name/type/arity content
changes. Cause: the per-package normalizer (`PerPackageSurface.normalize`) strips
`//`/`///` comment *text* but leaves the (now-isolated) blank line; the 20 new
`///` summaries on the `DesignTokens.Light`/`Dark` token vals each left one blank
line. The aggregate `PackageSurfaceCheck` (the actual contract hash) was unchanged
and passed, confirming **no contract change**.

Remediation (the documented procedure — `RefreshSurfaceBaselines` deliberately
skips per-package snapshots): recaptured the Controls baseline via
`PerPackageSurface.captureCurrent` and overwrote
`readiness/per-package-surface/FS.Skia.UI.Controls.fsi.txt`. `git diff` of the
baseline shows only blank-line insertions (no added signature text). Re-run is
green. FR-004 intent (public contract / signature shapes unchanged) is preserved;
only the cosmetic blank-line layout of the line-level snapshot moved.
