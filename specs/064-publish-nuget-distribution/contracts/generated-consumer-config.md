# Contract: generated consumer `NuGet.config` + single-source version pin

The **distribution contract** the consumer sees in a generated project. Emitted by
`build/Governance/GeneratedProduct.fs` (`writeLocalNuGetConfig`, today at line 1372) and the template
version files.

## Generated `NuGet.config` (FR-003)

**Before** (machine-absolute local path — breaks a fresh consumer):
```xml
<packageSources>
  <clear />
  <add key="local" value="/home/developer/.local/share/nuget-local" />
  <add key="nuget" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```
**After** (public feed only):
```xml
<packageSources>
  <clear />
  <add key="nuget" value="https://api.nuget.org/v3/index.json" />
</packageSources>
```

- The emitted consumer config MUST contain **no** absolute local path (`/home/.../nuget-local`).
- A fresh consumer (no repo, no local feed) restores from nuget.org once the packages are published (US1).
- **In-repo validation** keeps using `~/.local/share/nuget-local`: `TemplateCheck` applies a **staging-feed
  overlay** (restore with an extra `-s <local feed>` / a validation-only config) so generated projects
  restore **before** the packages are on nuget.org. The overlay is **never** emitted into a consumer
  project. The two configs are independent (FR-003 conflict resolution).

## Single-source version pin (FR-004)

`template/base/Directory.Packages.props`:
```xml
<PropertyGroup>
  <FsSkiaUiVersion>0.1.67-preview.1</FsSkiaUiVersion>
</PropertyGroup>
<ItemGroup>
  <PackageVersion Include="FS.Skia.UI.Build"  Version="$(FsSkiaUiVersion)" />
  <PackageVersion Include="FS.Skia.UI.Scene"  Version="$(FsSkiaUiVersion)" />
  <!-- … all FS.Skia.UI.* pins reference $(FsSkiaUiVersion) … -->
</ItemGroup>
```
`template/base/build.fsx` reads `<FsSkiaUiVersion>` at runtime and binds the engine assembly from it
(research R1) — **no literal `#r` version**.

- **Exactly one literal** `FS.Skia.UI` version value in the generated project: the `<FsSkiaUiVersion>` value.
- A consumer upgrade is **one edit** to that value + `dotnet restore`; libs **and** engine move together
  (US3, SC-004). Pins stay **exact** (no floating ranges). Preview vs stable is explicit in the value.

## Consumer upgrade doc (FR-005)

`template/base/docs/UPGRADING.md` ships into every generated project: which single value to change, run
`dotnet restore`, how to verify, and how preview vs stable versions are selected. See
[quickstart.md](../quickstart.md).
