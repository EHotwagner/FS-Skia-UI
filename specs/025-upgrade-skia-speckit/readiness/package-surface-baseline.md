# Package Surface Baseline

Evidence:

- Command: `./fake.sh build -t PackageSurfaceCheck`
- Pre-upgrade log: `specs/025-upgrade-skia-speckit/readiness/logs/pre-package-surface-check.log`
- Pre-upgrade exit code: `0`
- Stable baseline: `readiness/surface-baselines/FS.Skia.UI.txt`
- Package surface index: `specs/025-upgrade-skia-speckit/readiness/package-surfaces/index.md`

Status: unchanged.

Decision keyword: no `.fsi` change.

The Tier 1 `.fsi` review found no planned or required public API change for
`FS.Skia.UI`; therefore no `.fsi` sketch is required for this upgrade. The
compatibility package baseline remains `readiness/surface-baselines/FS.Skia.UI.txt`.

Any future public-surface delta must use the `.fsi`-first path with semantic or
FSI evidence, package-surface baseline refresh, docs, migration guidance, and
explicit approval.
