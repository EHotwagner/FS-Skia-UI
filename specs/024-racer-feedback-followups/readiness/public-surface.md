# Public Surface

## T010 Baseline Expectations

Recorded at: 2026-05-28T08:22:10+02:00

Public packages changed by this feature:

| Package | Signature | Stable baseline | Baseline SHA-256 |
|---------|-----------|-----------------|------------------|
| `FS.Skia.UI.SkiaViewer` | `src/SkiaViewer/SkiaViewer.fsi` | `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` | `f43cb654265b0afc164bddd4cb6472b993cbd6ab149bad91f4dda2076d83998e` |
| `FS.Skia.UI.Testing` | `src/Testing/Testing.fsi` | `readiness/surface-baselines/FS.Skia.UI.Testing.txt` | `a3652567982fdef8bd5a635c44cf11473468b84ab9e4011ed579ae4fd9dbf105` |

Expected intentional refresh later:

- Additive SkiaViewer screenshot capability and evidence workflow contracts.
- Additive Testing screenshot evidence report validator contracts.

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `./fake.sh build -t PackageSurfaceCheck` | PASS baseline check before intentional refresh | `readiness/logs/t010-package-surface-check.txt` |

## T030 Surface Baseline Refresh

Recorded at: 2026-05-28T08:34:39+02:00

The intentional refresh target and package surface check both passed. The
SkiaViewer and Testing stable baseline hashes remain the same as the T010
expectation after refresh:

| Package | Stable baseline | SHA-256 after refresh |
|---------|-----------------|----------------------|
| `FS.Skia.UI.SkiaViewer` | `readiness/surface-baselines/FS.Skia.UI.SkiaViewer.txt` | `f43cb654265b0afc164bddd4cb6472b993cbd6ab149bad91f4dda2076d83998e` |
| `FS.Skia.UI.Testing` | `readiness/surface-baselines/FS.Skia.UI.Testing.txt` | `a3652567982fdef8bd5a635c44cf11473468b84ab9e4011ed579ae4fd9dbf105` |

Verification:

| Command | Result | Evidence |
|---------|--------|----------|
| `./fake.sh build -t RefreshSurfaceBaselines` | PASS | `readiness/logs/t030-refresh-surface-baselines.txt` |
| `./fake.sh build -t PackageSurfaceCheck` | PASS | `readiness/logs/t030-package-surface-check.txt` |
