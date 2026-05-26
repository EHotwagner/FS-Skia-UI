# Dependency Governance Report

PASS: Central Package Management is enabled and repo-owned project files use versionless external PackageReference entries.

## Governed Packages

| Package | Central version declared | Metadata documented |
|---------|--------------------------|---------------------|
| Expecto | yes | yes |
| FS.Skia.UI | validation-only local package | yes |
| Fable.Elmish | yes | yes |
| Microsoft.NET.Test.Sdk | yes | yes |
| Silk.NET.Input | yes | yes |
| Silk.NET.Vulkan | yes | yes |
| Silk.NET.Vulkan.Extensions.KHR | yes | yes |
| Silk.NET.Windowing | yes | yes |
| Silk.NET.Windowing.Extensions | yes | yes |
| SkiaSharp | yes | yes |
| SkiaSharp.NativeAssets.Linux | yes | yes |
| SkiaSharp.NativeAssets.Win32 | yes | yes |
| YamlDotNet | yes | yes |
| Yoga.Net | yes | yes |
| YoloDev.Expecto.TestSdk | yes | yes |

## Controls Boundary Package Placement

| Package | Project references | External packages | Boundary rule |
|---------|--------------------|-------------------|---------------|
| FS.Skia.UI.Controls | ../KeyboardInput/KeyboardInput.fsproj, ../Layout/Layout.fsproj, ../Scene/Scene.fsproj | none | Owns form controls, rich rendering, chart controls, graph views, DataGrid, and product-owned ControlRuntime declarations. |
| FS.Skia.UI.KeyboardInput | ../Scene/Scene.fsproj | YamlDotNet | Owns the rich keyboard input runtime, reducer/effect contracts, diagnostics, and YAML configuration parsing. |
| FS.Skia.UI.Controls.Elmish | ../Controls/Controls.fsproj, ../KeyboardInput/KeyboardInput.fsproj | Fable.Elmish | Owns command, subscription, and program adapter integration so base Controls stays generic over product messages. |

PASS: Legacy Charts package/project references are absent from active repo-owned project files.

Validation-only exception: sample package-mode PackageReference versions for `FS.*` packages are allowed only under `UsePackedPackage` conditions.
