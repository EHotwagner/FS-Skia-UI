# Public Surface Readiness

## Scope

Feature `013-tetris-demo-integration` changes Tier 1 public surfaces for
normalized viewer input, bounded viewer smoke and diagnostics, deterministic
scene evidence, generated app host flows, and local generated consumer
validation.

## Evidence

- Surface baselines refreshed by `./fake.sh build -t RefreshSurfaceBaselines`.
- `./fake.sh build -t PackageSurfaceCheck` passed.
- `./fake.sh build -t FsiTranscripts` passed and wrote
  `readiness/fsi/*.txt`.
- Public contract readiness is linked from:
  - `readiness/normalized-viewer-input.md`
  - `readiness/bounded-viewer-smoke.md`
  - `readiness/diagnostics.md`
  - `readiness/headless-scene-evidence.md`

## Result

The changed public contracts are represented by `.fsi` files, package surface
baselines, focused package checks, and FSI transcript evidence.
