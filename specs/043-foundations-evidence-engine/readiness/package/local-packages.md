# Local Packages

Output directory: `/home/developer/.local/share/nuget-local`

## Package Inventory

| Package | Version | Project |
|---------|---------|---------|
| `FS.Skia.UI.Scene` | `0.1.45-preview.1` | `src/Scene/Scene.fsproj` |
| `FS.Skia.UI.SkiaViewer` | `0.1.45-preview.1` | `src/SkiaViewer/SkiaViewer.fsproj` |
| `FS.Skia.UI.Elmish` | `0.1.45-preview.1` | `src/Elmish/Elmish.fsproj` |
| `FS.Skia.UI.KeyboardInput` | `0.1.45-preview.1` | `src/KeyboardInput/KeyboardInput.fsproj` |
| `FS.Skia.UI.Controls.Elmish` | `0.1.45-preview.1` | `src/Controls.Elmish/Controls.Elmish.fsproj` |
| `FS.Skia.UI.Testing` | `0.1.45-preview.1` | `src/Testing/Testing.fsproj` |
| `FS.Skia.UI` | `0.1.45-preview.1` | `src/Lib/Lib.fsproj` |
| `FS.Skia.UI.Layout` | `0.1.45-preview.1` | `src/Layout/Layout.fsproj` |
| `FS.Skia.UI.Controls` | `0.1.45-preview.1` | `src/Controls/Controls.fsproj` |
| `FS.Skia.UI.Build` | `0.1.45-preview.1` | `build/Governance/FS.Skia.UI.Build.fsproj` |

## Consumer Package Configuration

```xml
  <ItemGroup>
    <PackageReference Include="FS.Skia.UI.Scene" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.SkiaViewer" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Elmish" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.KeyboardInput" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Controls.Elmish" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Testing" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Layout" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Controls" Version="0.1.45-preview.1" />
    <PackageReference Include="FS.Skia.UI.Build" Version="0.1.45-preview.1" />
  </ItemGroup>
```

## NuGet.config Snippet

```xml
  <packageSources>
    <clear />
    <add key="local" value="/home/developer/.local/share/nuget-local" />
    <add key="nuget" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
```

## Restore Command

`dotnet restore --source /home/developer/.local/share/nuget-local --source https://api.nuget.org/v3/index.json`

## Expected Local Artifacts

- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Scene.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.SkiaViewer.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Elmish.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.KeyboardInput.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Controls.Elmish.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Testing.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Layout.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Controls.0.1.45-preview.1.nupkg`
- `/home/developer/.local/share/nuget-local/FS.Skia.UI.Build.0.1.45-preview.1.nupkg`

## Drift Diagnostics

Missing or stale `.nupkg` files are setup drift before generated consumer build, input, or rendering failures. Re-run `./fake.sh build -t PackLocal` and verify the package identity, expected version, actual version, and feed path above.