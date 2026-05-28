# Compatibility Sample Migration

Evidence:

- Consumer scan: `specs/025-upgrade-skia-speckit/readiness/logs/compatibility-consumer-scan.txt`
- Sample project paths under `samples/`
- Pre-upgrade package-surface and dependency logs under `readiness/logs/`

| Sample or package-mode path | Current compatibility usage | Decision | Evidence | Notes |
|-----------------------------|-----------------------------|----------|----------|-------|
| `samples/BasicViewer` | `src/Lib/Lib.fsproj`, conditional packaged `FS.Skia.UI`, `open FS.Skia.UI` | keep unchanged | scan rows in compatibility consumer log | Representative simple broad-package consumer. |
| `samples/ScreenshotGallery` | `src/Lib/Lib.fsproj`, conditional packaged `FS.Skia.UI`, `open FS.Skia.UI` | keep unchanged | scan rows in compatibility consumer log | Supported sample behavior preserved. |
| `samples/ParityGallery` | broad package project/package reference and namespace open | keep unchanged | scan rows in compatibility consumer log | Parity tests still exercise compatibility behavior. |
| `samples/InteractiveViewer` | broad package project/package reference and namespace open | keep unchanged | scan rows in compatibility consumer log | Viewer migration is deferred. |
| `samples/EffectsGallery` | broad package project/package reference and namespace open | keep unchanged | scan rows in compatibility consumer log | Effects sample remains compatibility evidence. |
| `samples/DemoReel` | broad package project reference | keep unchanged | scan rows in compatibility consumer log | No package-mode migration in this upgrade. |

supported sample behavior is preserved by design. Future focused-package sample
migration should be a separate feature with package-surface, sample smoke, and
migration documentation evidence.
