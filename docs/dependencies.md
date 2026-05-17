# Dependency Governance

Direct external package versions are governed in `Directory.Packages.props`.
Repo-owned project files must use versionless external `PackageReference`
entries. Local package validation may use an inline version only for `FS.*`
packages under a `UsePackedPackage` condition or in generated consumer smoke
fixtures.

The Controls boundary refactor does not add a third-party runtime dependency.
It moves dependency placement into explicit package owners:

| Package owner | Direct project references | Direct external packages | Boundary rule |
|---------------|---------------------------|--------------------------|---------------|
| `FS.Skia.UI.Controls` | `FS.Skia.UI.Scene`, `FS.Skia.UI.Layout`, `FS.Skia.UI.KeyboardInput` | None | Owns form controls, rich rendering, chart controls, graph views, DataGrid, and product-owned `ControlRuntime` declarations without taking a direct Elmish or viewer dependency. |
| `FS.Skia.UI.KeyboardInput` | `FS.Skia.UI.Scene` | `YamlDotNet` | Owns the rich keyboard input runtime, reducer/effect contracts, diagnostics, and YAML configuration parsing. |
| `FS.Skia.UI.Controls.Elmish` | `FS.Skia.UI.Controls`, `FS.Skia.UI.KeyboardInput` | `Fable.Elmish` | Owns command, subscription, and program adapter integration so base Controls remains generic over product messages. |
| Legacy Charts package | None active | None | The former Charts package/project is removed from active package references and generated product dependencies; replacement authoring lives under `FS.Skia.UI.Controls`. |

| Package | Version | Purpose | Owner | License posture | Upgrade expectation | Preview risk |
|---------|---------|---------|-------|-----------------|---------------------|--------------|
| Expecto | 10.2.2 | F# semantic and governance test framework. | Test infrastructure | OSS package accepted for tests. | Review during SDK/test adapter upgrades. | None. |
| Fable.Elmish | 4.2.0 | Elmish model/update/effect workflow used by viewer APIs. | Runtime framework | OSS package accepted for runtime use. | Review before public API changes that affect Elmish contracts. | None. |
| FSharp.Core | 10.1.300 | F# core library required by SDK-style F# projects. | Runtime framework | Microsoft OSS package accepted for runtime and tests. | Keep aligned with the supported .NET SDK. | None. |
| Microsoft.NET.Test.Sdk | 17.11.1 | Test SDK adapter infrastructure for `dotnet test`. | Test infrastructure | Microsoft OSS package accepted for tests. | Keep aligned with supported .NET SDK. | None. |
| Silk.NET.Input | 2.23.0 | Input abstractions for keyboard and pointer integration. | Runtime framework | OSS package accepted for runtime use. | Review with Silk.NET platform updates. | None. |
| Silk.NET.Vulkan | 2.23.0 | Vulkan bindings for the renderer path. | Runtime framework | OSS package accepted for runtime use. | Review with Vulkan backend changes. | None. |
| Silk.NET.Vulkan.Extensions.KHR | 2.23.0 | Vulkan KHR extension bindings. | Runtime framework | OSS package accepted for runtime use. | Keep version-aligned with Silk.NET.Vulkan. | None. |
| Silk.NET.Windowing | 2.23.0 | Window lifecycle and presentation surface integration. | Runtime framework | OSS package accepted for runtime use. | Review with desktop host changes. | None. |
| Silk.NET.Windowing.Extensions | 2.23.0 | Windowing extension helpers. | Runtime framework | OSS package accepted for runtime use. | Keep version-aligned with Silk.NET.Windowing. | None. |
| SkiaSharp | 4.147.0-preview.2.1 | Skia drawing APIs used by the renderer. | Runtime framework | OSS package accepted with preview review. | Reassess at each SkiaSharp 4 preview/stable change. | Preview package: package shape and native behavior may change. |
| SkiaSharp.NativeAssets.Linux | 4.147.0-preview.2.1 | Linux native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| SkiaSharp.NativeAssets.Win32 | 4.147.0-preview.2.1 | Windows native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| YamlDotNet | 17.1.0 | YAML parsing for keyboard input configuration. | Runtime framework | OSS package accepted for runtime use. | Review with schema changes. | None. |
| Yoga.Net | 3.2.3 | Yoga layout engine bindings. | Layout package | OSS package accepted for layout use. | Review with layout engine updates. | None. |
| YoloDev.Expecto.TestSdk | 0.15.3 | Expecto test SDK adapter for `dotnet test`. | Test infrastructure | OSS package accepted for tests. | Keep compatible with Expecto and test SDK versions. | None. |

## Validation-Only Exceptions

Sample projects may keep a conditional `PackageReference Include="FS.*"
Version="$(FsSkiaUiPackageVersion)"` only inside `UsePackedPackage` item
groups. Package consumer smoke tests may generate temporary projects with a
literal local package version. These paths validate local packages and are not
general dependency policy.
