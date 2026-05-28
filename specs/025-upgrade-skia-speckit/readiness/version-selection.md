# Version Selection

Checked at: 2026-05-28T09:53:25Z

Evidence:

- NuGet source checks used official flat-container package indexes for the SkiaSharp package family.
- Spec Kit source check used the GitHub latest-release API for `github/spec-kit`.

## Source Checks

| Asset | Current repository version | Implementation-time source check | Selected version/range | Source URL or governed path | Affected files | Risk notes | Validation status |
|-------|----------------------------|----------------------------------|------------------------|-----------------------------|----------------|------------|-------------------|
| SkiaSharp | `4.147.0-preview.2.1` | NuGet flat-container index contained `4.147.0-preview.3.1` as latest listed version. | `4.147.0-preview.3.1` | `https://api.nuget.org/v3-flatcontainer/skiasharp/index.json` | `Directory.Packages.props`, `docs/dependencies.md`, readiness dependency evidence | Preview package; keep managed/native assets aligned. | selected |
| SkiaSharp.NativeAssets.Linux | `4.147.0-preview.2.1` | NuGet flat-container index contained `4.147.0-preview.3.1` as latest listed version. | `4.147.0-preview.3.1` | `https://api.nuget.org/v3-flatcontainer/skiasharp.nativeassets.linux/index.json` | `Directory.Packages.props`, `docs/dependencies.md`, readiness dependency evidence | Native asset package must match managed SkiaSharp. | selected |
| SkiaSharp.NativeAssets.Win32 | `4.147.0-preview.2.1` | NuGet flat-container query was attempted; package-family alignment uses the official NuGet package family and managed/native Linux source checks until command output is captured in dependency evidence. | `4.147.0-preview.3.1` | `https://api.nuget.org/v3-flatcontainer/skiasharp.nativeassets.win32/index.json` | `Directory.Packages.props`, `docs/dependencies.md`, readiness dependency evidence | Native asset package must match managed SkiaSharp. | selected |
| Spec Kit CLI/assets | `.specify/init-options.json` records `0.8.11`; preset requires `>=0.7.0` | GitHub latest release API returned `v0.8.16`, published 2026-05-27T21:35:15Z. | `0.8.16` for recorded project asset posture; preserve local generated assets unless drift tooling requires copied content changes. | `https://api.github.com/repos/github/spec-kit/releases/latest`; `.specify/init-options.json`; `.specify/presets/fsharp-opinionated/preset.yml` | `.specify/init-options.json`, `.specify/*`, `template/base/.specify/*`, generated guidance checks | Version metadata may move without requiring local preset content changes; any copied-asset change must pass generated guidance/template drift. | selected |

## Acceptance

Repository maintainer acceptance for this implementation pass: select the newest official package-family versions observed during implementation, keep SkiaSharp managed and native asset packages aligned, update Spec Kit metadata to the latest observed release, and rely on governed checks before claiming readiness.
