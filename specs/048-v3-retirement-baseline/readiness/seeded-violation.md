# Seeded one-package violation (T018, SC-005)

A **real, reverted** scratch edit to one public `.fsi` proves the capability reports
drift for **exactly** the affected package and no other. This is a real diff over real
files — no mock, no synthetic fixture.

## Procedure

1. Seed a scratch public-signature edit in `src/Scene/Scene.fsi` (a `val scratchProbe: Color`
   added after `val transparent: Color` in `module Colors`).
2. Re-run the diff over the real source tree + committed baselines
   (`captureCurrent` / `loadBaselines` / `diff`, the same pure path the
   `PerPackageSurfaceDiff` target's edge interpreter runs).
3. `git checkout -- src/Scene/Scene.fsi` to revert.
4. Re-run the diff to confirm zero drift.

## Result (authoritative)

| Step | `Drifted` | `MissingBaselines` |
|------|-----------|--------------------|
| After seeded edit | `["FS.Skia.UI.Scene"]` | `[]` |
| After revert | `[]` | `[]` |

- Exactly **one** package (`FS.Skia.UI.Scene`) drifted; the other seven showed no drift
  (FR-008 / SC-005).
- The reported change was `Added "val scratchProbe: Color"` (the `///` doc comment is
  stripped by `normalize`, so only the signature line surfaces — confirming the
  signature-sensitive, comment-insensitive contract).
- After `git checkout --`, the working tree is clean (`git status --porcelain
  src/Scene/Scene.fsi` is empty) and the diff returns to zero drift — no runtime `.fsi`
  was permanently changed (SC-007).

## Reproduction

```bash
# seed one public signature in src/Scene/Scene.fsi, then:
./fake.sh build -t PerPackageSurfaceDiff      # fails, naming FS.Skia.UI.Scene only
git checkout -- src/Scene/Scene.fsi
./fake.sh build -t PerPackageSurfaceDiff      # green, zero drift across 8 packages
```
