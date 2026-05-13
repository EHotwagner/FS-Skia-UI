# Feature Readiness Notes

Feature: `002-skia-feature-parity`

## Evidence Directories

| Directory | Purpose |
|-----------|---------|
| `readiness/transcripts/` | FSI and command transcripts that exercise public entry points |
| `readiness/screenshots/` | Deterministic gallery screenshots and screenshot-capture evidence |
| `readiness/parity/` | Parity matrix inputs, normalized reports, and capability mapping artifacts |
| `readiness/surface-baselines/` | Public API surface snapshots for `.fsi`-controlled modules |
| `readiness/package/` | Restore, pack, local NuGet, and clean-reference logs |
| `readiness/smoke/` | Sample application smoke logs and platform-specific runs |
| `readiness/logs/` | Consolidated restore/build/test/prelude logs |
| `readiness/sample-assets/` | Test images and fixtures used by semantic/rendering evidence |

## Local NuGet Conventions

- Local package output directory: `artifacts/nuget`
- Package verification logs: `specs/002-skia-feature-parity/readiness/package/`
- Expected package IDs: `FS.Skia.UI`, `FS.Skia.UI.Charts`, `FS.Skia.UI.Layout`
- Package reference tests must restore from the local output directory before using public APIs.

## Sample Conventions

- Core samples live under `samples/`.
- Planned parity samples are `BasicViewer`, `InteractiveViewer`, `ParityGallery`, `EffectsGallery`, `ChartsGallery`, `DataGridGallery`, `LayoutGraphGallery`, `ScreenshotGallery`, and `DemoReel`.
- Smoke logs for runnable samples are stored under `specs/002-skia-feature-parity/readiness/smoke/`.

## Evidence Command Conventions

Use these paths when capturing logs:

```bash
dotnet restore FS-Skia-UI.sln 2>&1 | tee specs/002-skia-feature-parity/readiness/logs/restore.txt
dotnet build FS-Skia-UI.sln 2>&1 | tee specs/002-skia-feature-parity/readiness/logs/build.txt
dotnet test FS-Skia-UI.sln 2>&1 | tee specs/002-skia-feature-parity/readiness/logs/test.txt
dotnet pack FS-Skia-UI.sln -c Release -o artifacts/nuget 2>&1 | tee specs/002-skia-feature-parity/readiness/package/pack.txt
dotnet fsi scripts/prelude.fsx 2>&1 | tee specs/002-skia-feature-parity/readiness/transcripts/core-prelude.txt
.specify/extensions/evidence/scripts/python/compute-task-graph.py specs/002-skia-feature-parity
```

When a command is expected to fail because the local host lacks Vulkan, keep the full log and mark the dependent task `[S]` only if the code/test uses synthetic evidence.
