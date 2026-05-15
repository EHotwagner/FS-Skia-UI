# Dependency Inventory

Feature: `007-v2-template-packaging`

## Current Direct External Packages

| Package | Current version | Owner | Purpose | Preview risk |
|---------|-----------------|-------|---------|--------------|
| Fable.Elmish | 4.2.0 | Runtime | Elmish program model used by the viewer workflow | None |
| Silk.NET.Input | 2.23.0 | Runtime | Cross-platform input abstractions | None |
| Silk.NET.Vulkan | 2.23.0 | Runtime | Vulkan bindings | None |
| Silk.NET.Vulkan.Extensions.KHR | 2.23.0 | Runtime | Swapchain and KHR extension bindings | None |
| Silk.NET.Windowing | 2.23.0 | Runtime | Window lifecycle and graphics surface integration | None |
| Silk.NET.Windowing.Extensions | 2.23.0 | Runtime | Windowing extension helpers | None |
| SkiaSharp | 4.147.0-preview.2.1 | Runtime | Skia rendering APIs | Preview package |
| SkiaSharp.NativeAssets.Linux | 4.147.0-preview.2.1 | Runtime | Linux native Skia assets | Preview package |
| SkiaSharp.NativeAssets.Win32 | 4.147.0-preview.2.1 | Runtime | Windows native Skia assets | Preview package |
| YamlDotNet | 17.1.0 | Runtime | Keyboard input YAML configuration parsing | None |
| Yoga.Net | 3.2.3 | Layout | Yoga layout engine bindings | None |
| Expecto | 10.2.2 | Test | F# semantic tests | None |
| Microsoft.NET.Test.Sdk | 17.11.1 | Test | `dotnet test` adapter infrastructure | None |
| YoloDev.Expecto.TestSdk | 0.15.3 | Test | Expecto test SDK adapter | None |

## Validation-Only Exceptions

Local package consumer smoke tests and sample package-mode checks may carry a
package version property or inline generated project version only when the code
is intentionally validating a local package artifact. These exceptions are
documented in `docs/dependencies.md` and rejected anywhere else by
`DependencyReport`.
