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
| `FS.Skia.UI.SkiaViewer` | `FS.Skia.UI`, `FS.Skia.UI.Scene`, `FS.Skia.UI.KeyboardInput` | `Fable.Elmish`, `Silk.NET.Input`, `Silk.NET.Vulkan`, `Silk.NET.Vulkan.Extensions.KHR`, `Silk.NET.Windowing`, `Silk.NET.Windowing.Extensions`, `SkiaSharp`, `SkiaSharp.NativeAssets.Linux`, `SkiaSharp.NativeAssets.Win32` | Owns public generated-app viewer contracts, persistent launch validation, and desktop window hosting. It may bridge to the legacy `FS.Skia.UI` Vulkan presenter to commit real swapchain frames for visible interactive launches. |
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
| SkiaSharp | 4.147.0-preview.3.1 | Skia drawing APIs used by the renderer. | Runtime framework | OSS package accepted with preview review. | Reassess at each SkiaSharp 4 preview/stable change. | Preview package: package shape and native behavior may change. |
| SkiaSharp.NativeAssets.Linux | 4.147.0-preview.3.1 | Linux native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| SkiaSharp.NativeAssets.Win32 | 4.147.0-preview.3.1 | Windows native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| YamlDotNet | 17.1.0 | YAML parsing for keyboard input configuration. | Runtime framework | OSS package accepted for runtime use. | Review with schema changes. | None. |
| Yoga.Net | 3.2.3 | Yoga layout engine bindings. | Layout package | OSS package accepted for layout use. | Review with layout engine updates. | None. |
| YoloDev.Expecto.TestSdk | 0.15.3 | Expecto test SDK adapter for `dotnet test`. | Test infrastructure | OSS package accepted for tests. | Keep compatible with Expecto and test SDK versions. | None. |

## Build-Tooling Dependencies (build/** only — NOT shipped in any generated product)

Consumed as ordinary NuGet *libraries* by the compiled FAKE build front-end
(`build/Build.fsproj`) via `dotnet run` — not via the FSX script runner, and
not by any `src/**` project. These are scoped to `build/**`; `DependencyReport`
scans only `src/**` and therefore never counts them as runtime/shipped
dependencies.

| Package | Version | Purpose | Owner | License posture | Upgrade expectation | Preview risk |
|---------|---------|---------|-------|-----------------|---------------------|--------------|
| Fake.Core.Target | 6.1.4 | Register and run build targets from a compiled `dotnet run` exe (feature 039 D2 spike). The Target API is a plain library; no FSX runner. | Build tooling | OSS package accepted for build tooling only. | Keep aligned with the `fake-cli` local tool and `build.fsx.lock`. | None. |

`Fake.Core.Target` transitively brings the minimal `Fake.Core.*` companions its
API requires (Context, Process, Trace, …) at the same `6.1.4` line; these are
resolved transitively and are not declared as separate top-level entries.
**No `FSharp.Compiler.Service`** is introduced (FR-012) — verified by
`dotnet list build/Build.fsproj package --include-transitive` and recorded in
`docs/reports/_baselines/2026-05-31-spike-d2-outcome.md`.

## Validation-Only Exceptions

Sample projects may keep a conditional `PackageReference Include="FS.*"
Version="$(FsSkiaUiPackageVersion)"` only inside `UsePackedPackage` item
groups. Package consumer smoke tests may generate temporary projects with a
literal local package version. These paths validate local packages and are not
general dependency policy.

## Local Generated Consumer Packages

Generated graphical consumer validation for feature
`019-fix-window-visibility` must report the local feed path, package
identities, versions, consumer package configuration snippet, optional
`nuget.config` snippet, restore command, and stale or missing feed diagnostics
before build, input, or rendering failures are attributed to application code.
Run `./fake.sh build -t PackLocal` before `./fake.sh build -t
GeneratedProductCheck` when validating a fresh generated consumer from local
packages.

Package identities remain stable for this feature. Missing or stale local
packages are setup drift and should include the affected package id, expected
version, actual version when present, feed path, and remediation command.
Generated product source must reference FS.Skia.UI capabilities with
`PackageReference`; it must not copy repository implementation source as a
substitute for package consumption.

## 2026-05-28 SkiaSharp And Compatibility Posture

SkiaSharp managed and native asset packages are version-aligned at
`4.147.0-preview.3.1`, selected from official NuGet package metadata during
feature `025-upgrade-skia-speckit`.

FS.Skia.UI remains a stable compatibility surface for existing broad-package
consumers. Prefer focused packages for new generated products:
`FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Elmish`,
`FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Layout`, `FS.Skia.UI.Controls`,
`FS.Skia.UI.Controls.Elmish`, and `FS.Skia.UI.Testing`.

The deferred compatibility-package direction is deliberate: this upgrade does
not remove public APIs, collapse generated profiles, or decide permanent
facade/deprecation policy beyond the conservative broad-package guidance in the
feature readiness artifacts.
