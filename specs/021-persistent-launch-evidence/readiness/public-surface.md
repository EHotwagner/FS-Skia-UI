# Public Surface

Feature: `021-persistent-launch-evidence`

Changed public contracts:

- `FS.Skia.UI.SkiaViewer`: persistent launch stages, window observation result,
  observation/capture classifier, launch outcome fields, and evidence workflow
  helpers.
- `FS.Skia.UI.Testing`: persistent-launch artifact validation and host warning
  classification contracts.

Surface validation:

- `./fake.sh build -t RefreshSurfaceBaselines`
- `./fake.sh build -t PackageSurfaceCheck`

Logs:

- `readiness/logs/t035-refresh-surface-baselines.txt`
- `readiness/logs/t035-package-surface-check.txt`

