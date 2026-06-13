# Before-state pin snapshot (feature 115, T004)

Captured 2026-06-13 on branch `115-dependency-updates` **before** any pin edit. This is the baseline the
after-state diffs against.

## `Directory.Packages.props` (PackageVersion pins)

| Package | Version | Class |
|---|---|---|
| Fable.Elmish | 4.2.0 | held |
| FSharp.Core | 10.1.300 | safe → 10.1.301 |
| Silk.NET.Input | 2.23.0 | current |
| Silk.NET.Vulkan | 2.23.0 | current |
| Silk.NET.Vulkan.Extensions.KHR | 2.23.0 | current |
| Silk.NET.Windowing | 2.23.0 | current |
| Silk.NET.Windowing.Extensions | 2.23.0 | current |
| SkiaSharp | 4.147.0-preview.3.1 | out-of-scope (preview line) |
| SkiaSharp.NativeAssets.Linux | 4.147.0-preview.3.1 | out-of-scope |
| SkiaSharp.NativeAssets.Win32 | 4.147.0-preview.3.1 | out-of-scope |
| YamlDotNet | 17.1.0 | held |
| Yoga.Net | 3.2.3 | current |
| Expecto | 10.2.2 | held (cluster) |
| Microsoft.NET.Test.Sdk | 17.11.1 | held (cluster) |
| YoloDev.Expecto.TestSdk | 0.15.3 | held (cluster) |
| Fake.Core.Target | 6.1.4 | out-of-scope (build.fsx.lock) |
| FSharp.SystemTextJson | 1.4.36 | current |
| XParsec | 1.0.0 | current |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.8 | safe → 10.0.9 |
| Fake.IO.FileSystem | 6.1.4 | out-of-scope |
| Fake.Tools.Git | 6.1.4 | out-of-scope |
| DiffPlex | 1.9.0 | current |
| FsCheck | 3.3.3 | current |

## `.specify/init-options.json`

- `speckit_version` = `0.8.16`  (target: `0.10.2`)

## Installed .NET SDK (`dotnet --list-sdks`)

```
6.0.428 [/usr/share/dotnet/sdk]
10.0.300 [/usr/share/dotnet/sdk]
```

**Note / honesty correction:** the plan and research assumed the floating .NET SDK was already at
`10.0.301`. The actually-installed `net10` SDK on this machine is **`10.0.300`**. There is no `global.json`
pin, so the SDK floats to whatever is installed — currently `10.0.300`. The "bump to 10.0.301" is therefore
**not realized on this toolchain** (nothing to edit, and the newer SDK is not present); it is recorded for
completeness and will be reflected truthfully in `us1-validation.md` (the float follows the installed SDK,
which is `10.0.300` here, not `10.0.301`).

## Baseline gate state

`./fake.sh build -t Dev` on this clean tree (before pin edits) = **green** (4m10s) — see
`readiness/logs/dev-baseline.txt`. This is the green baseline a bad bump would turn red.
