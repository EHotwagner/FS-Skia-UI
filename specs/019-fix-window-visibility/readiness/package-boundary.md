# Package Boundary

Status: changed and validated.

`FS.Skia.UI.SkiaViewer` now references repo-owned `FS.Skia.UI` and existing governed `Fable.Elmish` so persistent generated launches use the real Vulkan/Skia presenter rather than a shell-only window.

Evidence:

- PackLocal: `readiness/logs/t047-retry-pack-local-presenter-tests.txt`
- DependencyReport: `readiness/logs/t048-dependency-report-after-docs.txt`
- Dependency governance: `readiness/dependency-governance.md`
- Generated exact package validation: `readiness/generated-validation.md`
