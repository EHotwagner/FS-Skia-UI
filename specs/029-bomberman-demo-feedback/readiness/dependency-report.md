# Dependency Report

PASS: V3 dependency ownership report completed.

- Scene has no Elmish, Silk.NET, SkiaSharp, Yoga.Net, or YamlDotNet dependency.
- SkiaViewer owns Silk.NET and SkiaSharp host dependencies.
- Elmish owns Fable.Elmish adapter dependency.
- KeyboardInput owns YamlDotNet dependency.
- Layout owns Yoga.Net dependency.
- Controls owns form controls, rich rendering, chart controls, graph views, DataGrid, and ControlRuntime declarations.
- Controls depends only on Scene, Layout, and KeyboardInput and has no direct external PackageReference entries.
- Controls.Elmish owns Fable.Elmish command, subscription, and program adapter dependency.
- The removed Charts package is absent from active package, baseline, and generated product lists.
- Legacy Charts package/project is removed from active package, baseline, and generated product lists; migration guidance is documentation-only.
- Testing owns generated-product validation helpers.

Evidence:

- Command: `./fake.sh build -t DependencyReport`
- Source: `Directory.Packages.props`
- Active feature evidence: `specs/025-upgrade-skia-speckit/readiness/version-selection.md` when feature 025 is active.

## Before And After

| Package | Before | After | Owner | Status |
|---------|--------|-------|-------|--------|
| SkiaSharp | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | SkiaViewer/compatibility renderer host | aligned |
| SkiaSharp.NativeAssets.Linux | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Linux native renderer assets | aligned |
| SkiaSharp.NativeAssets.Win32 | `4.147.0-preview.2.1` | `4.147.0-preview.3.1` | Windows native renderer assets | aligned |
| Spec Kit metadata | `0.8.11` | `0.8.16` | project governance metadata | aligned to latest release metadata |

cycle status: no new project reference was added, so the package graph cycle status is unchanged.
unexpected spread review: no SkiaSharp reference was added to Scene, Layout, Controls, Controls.Elmish, KeyboardInput, Testing, generated product source, or generated product tests.
