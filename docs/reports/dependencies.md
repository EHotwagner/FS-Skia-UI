---
title: Dependency Governance
---

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
| `FS.Skia.UI.SkiaViewer` | `FS.Skia.UI.Scene`, `FS.Skia.UI.KeyboardInput` | `Fable.Elmish`, `Silk.NET.Input`, `Silk.NET.Vulkan`, `Silk.NET.Vulkan.Extensions.KHR`, `Silk.NET.Windowing`, `Silk.NET.Windowing.Extensions`, `SkiaSharp`, `SkiaSharp.NativeAssets.Linux`, `SkiaSharp.NativeAssets.Win32` | Owns public generated-app viewer contracts, persistent launch validation, and desktop window hosting. The Vulkan presenter and swapchain frame commit now live in `FS.Skia.UI.SkiaViewer.Host` (relocated out of the retired monolith in V3 Stage 1); there is no longer any bridge back to a legacy package. |
| `FS.Skia.UI.Controls` | `FS.Skia.UI.Scene`, `FS.Skia.UI.Layout`, `FS.Skia.UI.KeyboardInput` | None | Owns form controls, rich rendering, chart controls, graph views, DataGrid, and product-owned `ControlRuntime` declarations without taking a direct Elmish or viewer dependency. |
| `FS.Skia.UI.KeyboardInput` | `FS.Skia.UI.Scene` | `YamlDotNet` | Owns the rich keyboard input runtime, reducer/effect contracts, diagnostics, and YAML configuration parsing. |
| `FS.Skia.UI.Controls.Elmish` | `FS.Skia.UI.Controls`, `FS.Skia.UI.KeyboardInput` | `Fable.Elmish` | Owns command, subscription, and program adapter integration so base Controls remains generic over product messages. |
| Legacy Charts package | None active | None | The former Charts package/project is removed from active package references and generated product dependencies; replacement authoring lives under `FS.Skia.UI.Controls`. |

| Package | Version | Purpose | Owner | License posture | Upgrade expectation | Preview risk |
|---------|---------|---------|-------|-----------------|---------------------|--------------|
| Expecto | 10.2.2 | F# semantic and governance test framework. | Test infrastructure | OSS package accepted for tests. | Review during SDK/test adapter upgrades. | None. |
| Fable.Elmish | 5.0.2 | Elmish model/update/effect workflow used by viewer APIs. | Runtime framework | OSS package accepted for runtime use. | Review before public API changes that affect Elmish contracts. | None. |
| FSharp.Core | 10.1.301 | F# core library required by SDK-style F# projects. | Runtime framework | Microsoft OSS package accepted for runtime and tests. | Keep aligned with the supported .NET SDK. | None. |
| Microsoft.NET.Test.Sdk | 17.11.1 | Test SDK adapter infrastructure for `dotnet test`. | Test infrastructure | Microsoft OSS package accepted for tests. | Keep aligned with supported .NET SDK. | None. |
| Silk.NET.Input | 2.23.0 | Input abstractions for keyboard and pointer integration. | Runtime framework | OSS package accepted for runtime use. | Review with Silk.NET platform updates. | None. |
| Silk.NET.Vulkan | 2.23.0 | Vulkan bindings for the renderer path. | Runtime framework | OSS package accepted for runtime use. | Review with Vulkan backend changes. | None. |
| Silk.NET.Vulkan.Extensions.KHR | 2.23.0 | Vulkan KHR extension bindings. | Runtime framework | OSS package accepted for runtime use. | Keep version-aligned with Silk.NET.Vulkan. | None. |
| Silk.NET.Windowing | 2.23.0 | Window lifecycle and presentation surface integration. | Runtime framework | OSS package accepted for runtime use. | Review with desktop host changes. | None. |
| Silk.NET.Windowing.Extensions | 2.23.0 | Windowing extension helpers. | Runtime framework | OSS package accepted for runtime use. | Keep version-aligned with Silk.NET.Windowing. | None. |
| SkiaSharp | 4.147.0-preview.3.1 | Skia drawing APIs used by the renderer. | Runtime framework | OSS package accepted with preview review. | Reassess at each SkiaSharp 4 preview/stable change. | Preview package: package shape and native behavior may change. |
| SkiaSharp.NativeAssets.Linux | 4.147.0-preview.3.1 | Linux native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| SkiaSharp.NativeAssets.Win32 | 4.147.0-preview.3.1 | Windows native Skia assets. | Runtime framework | OSS package accepted with preview review. | Keep version-aligned with SkiaSharp. | Preview native asset package. |
| YamlDotNet | 18.0.0 | YAML parsing for keyboard input configuration. | Runtime framework | OSS package accepted for runtime use. | Review with schema changes. | None. |
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

### Published governance-library package (feature 043 — ADR D1 / research R8)

`FS.Skia.UI.Build` (`build/Governance/FS.Skia.UI.Build.fsproj`) is the only
`build/**` project that is **packed and published** (`IsPackable=true`, packed
by `PackLocal`). Generated `dotnet new fs-skia-ui` projects add a
`FS.Skia.UI.Build` `PackageVersion` pin (`template/base/Directory.Packages.props`)
and call its `FS.Skia.UI.Build.Evidence.*` engine in-process from their
`build.fsx`, replacing the copied Python + `run-audit.sh` evidence scripts
(FR-013). It carries the same package version line as the runtime
`FS.Skia.UI.*` packages and is bumped with them.

| Package | Version | Purpose | Owner | License posture | Upgrade expectation | Preview risk |
|---------|---------|---------|-------|-----------------|---------------------|--------------|
| FS.Skia.UI.Build | 0.1.45-preview.1 | Compiled-F# evidence graph + merge-gate audit engine consumed in-process by repo and generated-product build tooling (043). Transitively brings `YamlDotNet`. | Build tooling / governance | Repo-owned package. | Bumped with the other `FS.Skia.UI.*` packages. | None — no `FSharp.Compiler.*`. |

The published package's only library dependency is `YamlDotNet` (the typed
`tasks.deps.yml` / `audit-patterns.yml` reader); it introduces **no**
`FSharp.Compiler.*` (SC-004). As a repo-owned `FS.*` package its version lives
in the `.fsproj` `<Version>` (consistent with the other nine packages), not in
the central `Directory.Packages.props`; consumers pin it via their own
`Directory.Packages.props`.

### Capability adopt-set (feature 040 — `build/SkillExamples` + `tests/Governance.Tests` only)

The six `fsharp-*` capability skills carry compile-verified ` ```fsharp ` cookbook
snippets. The `SkillExamplesCheck` gate tangles every block into
`build/SkillExamples/SkillExamples.fsproj` and compiles it against the report's
minimal adopt set, so a passing build proves the snippets' API calls are correct
against the pinned versions. These packages are referenced **only** by that
examples project and `tests/Governance.Tests`; no `src/**` project references
them and none ships in any generated product. `DependencyReport` scans only
`src/**`, so it never counts them as runtime dependencies. **No
`FSharp.Compiler.Service`** (FR-008).

| Package | Version | Purpose | Owner | License posture | Upgrade expectation | Preview risk |
|---------|---------|---------|-------|-----------------|---------------------|--------------|
| FSharp.SystemTextJson | 1.4.36 | F# DU/record round-trip over `System.Text.Json` for the JSON read/write capability (C4, C5). | Build tooling / governance | OSS package accepted for build tooling only. | Review with `System.Text.Json` / SDK upgrades. | None. |
| XParsec | 1.0.0 | Pure-F# parser combinators for the line/region/diff grammar capability (C2, C3, C16) after the regex-port parity gate. | Build tooling / governance | MIT; accepted for build tooling only. | Pinned to 1.0.0 per the capability report; review on a new major. | None. |
| Microsoft.Extensions.FileSystemGlobbing | 10.0.9 | First-party `*`/`**` whitelist glob matching for the glob capability (C14). | Build tooling / governance | Microsoft OSS package accepted for build tooling only. | Keep aligned with the .NET 10 line. | None. |
| Fake.IO.FileSystem | 6.1.4 | FAKE-family file discovery / globbing for the discovery capability (C13). | Build tooling / governance | OSS package accepted for build tooling only. | Keep aligned with `Fake.Core.Target` 6.1.4 and `build.fsx.lock`. | None. |
| Fake.Tools.Git | 6.1.4 | FAKE-family git wrapping (base-ref resolve, `merge-base`, diff) for the git capability (C15). | Build tooling / governance | OSS package accepted for build tooling only. | Keep aligned with `Fake.Core.Target` 6.1.4. | None. |
| DiffPlex | 1.9.0 | Readable unified/side-by-side text diffs for the golden-parity / generation-currency capability (C19). | Build tooling / governance | OSS package accepted for build tooling only. | Review on a new minor. | None. |
| FsCheck | 3.3.3 | Property-based testing (v3) of the graph algorithms for the testing capability (C20). | Build tooling / governance | OSS package accepted for build tooling only. | Keep on the FsCheck 3 line, integrating with the in-tree Expecto 10.2.2. | None. |

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

The broad `FS.Skia.UI` compatibility monolith was retired in V3 Stage 5
(feature `053-v3-monolith-retirement`): the project is deleted and the package
is no longer published. All consumers — repository and generated products alike —
reference the focused split packages:
`FS.Skia.UI.Scene`, `FS.Skia.UI.SkiaViewer`, `FS.Skia.UI.Elmish`,
`FS.Skia.UI.KeyboardInput`, `FS.Skia.UI.Input`, `FS.Skia.UI.Layout`,
`FS.Skia.UI.Controls`, `FS.Skia.UI.Controls.Elmish`, and `FS.Skia.UI.Testing`.
The V2→V3 surface map and reference-move steps are documented in
`docs/migration/v2-to-v3.md`.
