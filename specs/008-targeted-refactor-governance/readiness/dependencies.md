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

Validation-only exception: sample package-mode PackageReference versions for `FS.*` packages are allowed only under `UsePackedPackage` conditions.
