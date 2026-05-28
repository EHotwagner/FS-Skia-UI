# Skia Native Viewer Validation

Evidence:

- Package-family source check: NuGet flat-container metadata for SkiaSharp and native asset packages.
- Focused package owner: `src/SkiaViewer/skill/SKILL.md`
- Package-surface command: `./fake.sh build -t PackageSurfaceCheck`

| Platform | Command | Result | Failure reason | Blocks acceptance |
|----------|---------|--------|----------------|-------------------|
| Linux development host | `./fake.sh build -t PackageSurfaceCheck` | pass | none | no |
| Linux development host | `./fake.sh build -t DependencyReport` | pass before edit | none | no |

Reviewer trace keywords: platform; command; failure reason; blocks acceptance.

No unsupported-host failure was observed during the pre-upgrade baseline
commands. Full persistent graphical viewer launch remains outside this
dependency-metadata task unless final validation reports a native/runtime
failure.
