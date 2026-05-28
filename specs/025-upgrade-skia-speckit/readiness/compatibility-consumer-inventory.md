# Compatibility Consumer Inventory

Evidence:

- Repository scan command: `rg --pcre2 -n "FS\\.Skia\\.UI(?!\\.)|open FS\\.Skia\\.UI$|PackageReference Include=\"FS\\.Skia\\.UI\"|ProjectReference Include=.*Lib\\.fsproj" src tests samples docs template .template.config -g '!**/bin/**' -g '!**/obj/**' -g '!docs/_site/**'`
- Scan output: `specs/025-upgrade-skia-speckit/readiness/logs/compatibility-consumer-scan.txt`
- Scan result count: 107 matched lines.

| Path | Consumer kind | Usage kind | Package mode | Focused replacement | Migration status | Notes |
|------|---------------|------------|--------------|---------------------|------------------|-------|
| `samples/BasicViewer/BasicViewer.fsproj` | sample | `ProjectReference` to `src/Lib/Lib.fsproj`; `PackageReference` to `FS.Skia.UI` under `UsePackedPackage` | source and packaged-mode guidance | `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer` for future focused sample | keep unchanged | Representative broad-package compatibility sample. |
| `samples/BasicViewer/Program.fs` | sample | namespace open | source | `FS.Skia.UI.Scene` + `FS.Skia.UI.SkiaViewer` | keep unchanged | Uses `open FS.Skia.UI`. |
| `samples/ScreenshotGallery/ScreenshotGallery.fsproj` | sample | `ProjectReference` and conditional `PackageReference Include="FS.Skia.UI"` | source and packaged-mode guidance | `FS.Skia.UI.Scene` + screenshot/viewer focused paths if migrated | keep unchanged | Supported broad-package screenshot sample remains compatibility evidence. |
| `samples/ScreenshotGallery/Program.fs` | sample | namespace open | source | focused scene/viewer packages | keep unchanged | Uses `open FS.Skia.UI`. |
| `samples/ParityGallery/*` | sample | `ProjectReference`, conditional `PackageReference`, namespace open | source and packaged-mode guidance | focused scene/viewer parity packages | keep unchanged | Parity evidence intentionally exercises broad compatibility surface. |
| `samples/InteractiveViewer/*` | sample | `ProjectReference`, conditional `PackageReference`, namespace open | source and packaged-mode guidance | `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Elmish` | keep unchanged | Interactive viewer sample remains broad-package compatibility evidence. |
| `samples/EffectsGallery/*` | sample | `ProjectReference`, conditional `PackageReference`, namespace open | source and packaged-mode guidance | `FS.Skia.UI.Scene` effect primitives plus viewer host | keep unchanged | Broad package stays stable for older effect sample flows. |
| `samples/DemoReel/DemoReel.fsproj` | sample | `ProjectReference` to `src/Lib/Lib.fsproj` | source | focused scene/viewer packages | keep unchanged | Demo reel still validates broad package rendering. |
| `tests/Lib.Tests/*` | test | `ProjectReference` and namespace open | source | none, package under test is `FS.Skia.UI` | keep unchanged | Compatibility package tests. |
| `tests/Smoke.Tests/Smoke.Tests.fsproj` | test | `ProjectReference` to `src/Lib/Lib.fsproj` | source | none | keep unchanged | Smoke tests cover compatibility package behavior. |
| `tests/Parity.Tests/*` | test | `ProjectReference` and namespace open | source | focused parity coverage may be added later | keep unchanged | Existing parity tests depend on broad package. |
| `tests/Package.Tests/*` | package metadata test | `ProjectReference` and package identity checks | source/package metadata | none | keep unchanged | Guards `FS.Skia.UI` public surface and package identity. |
| `src/SkiaViewer/SkiaViewer.fsproj` | focused package bridge | `ProjectReference` to `src/Lib/Lib.fsproj` | source | future facade removal only after renderer migration | keep unchanged | Viewer currently bridges to legacy Vulkan presenter for real frames. |
| `docs/` | documentation | compatibility-package analysis, architecture, dependency, subsystem docs | docs | focused packages for new guidance | update guidance only | Docs now name compatibility posture and deferred direction. |
| `.template.config/template.json` | template parameter metadata | default package prefix string `FS.Skia.UI` | template metadata | generated focused package references | keep unchanged | Prefix parameter is not a broad-package dependency. |
| `template/` | generated product guidance | docs mention FS.Skia.UI product family; generated package refs are focused | template | already focused | keep unchanged | No `FS.Skia.UI` PackageReference in generated product files. |

Reviewer trace keyword: focused replacement.
