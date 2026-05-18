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
