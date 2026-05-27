# T048 Dependency Governance

Status: PASS
Task: T048
Command: `./fake.sh build -t DependencyReport`
Log: `specs/019-fix-window-visibility/readiness/logs/t048-dependency-report.txt`
Generated reports:
- `specs/019-fix-window-visibility/readiness/dependency-report.md`
- `specs/019-fix-window-visibility/readiness/dependencies.md`

## Findings

- Central Package Management remains enabled.
- Repo-owned project files use versionless external `PackageReference` entries.
- DependencyReport passed.
- No new third-party package identity was introduced.
- Package placement changed: `FS.Skia.UI.SkiaViewer` now references repo-owned `FS.Skia.UI` so persistent launches can use the existing Vulkan/Skia presenter and commit real swapchain frames.
- Package placement changed: `FS.Skia.UI.SkiaViewer` now directly references existing governed package `Fable.Elmish` for the bridge command adapter required by the `FS.Skia.UI.Viewer` presenter API.
- `docs/dependencies.md` was updated to document the `FS.Skia.UI.SkiaViewer` package owner row and the feature-specific generated consumer package guidance for `019-fix-window-visibility`.

## Package Impact

The generated consumer package resolution remains authoritative after the presenter bridge:

- requested `FS.Skia.UI.SkiaViewer=0.1.18-preview.1`
- resolved `FS.Skia.UI.SkiaViewer=0.1.18-preview.1`
- transitive repo-owned presenter package resolved as `FS.Skia.UI=0.1.18-preview.1`
- restore warning count: `0`

The new dependency is intentional and exercised by `GeneratedProductCheck`; it replaces taskbar-only shell-window creation with real Vulkan/Skia presentation.
