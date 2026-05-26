# Generated Guidance Check Readiness

Status: scaffolded for generated guidance and persistent-default validation evidence.

## T040 Bounded API Regression

SkiaViewer regression coverage now directly exercises the explicit bounded
helper APIs:

- `Viewer.runBounded`
- `Viewer.runUntilFirstFrame`
- `Viewer.runForFrames`

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "bounded helper APIs"` passed.

## T041 Generated Product Bounded Commands

Generated product tests cover:

- explicit `--bounded-smoke`
- explicit `--bounded-smoke-frame-diagnostics`
- deterministic `--scene-evidence`
- default executable path still using `Viewer.runApp viewerOptions generatedHost`

Verification:

- `dotnet test template/base/tests/Product.Tests/Product.Tests.fsproj --filter "bounded smoke command|deterministic scene evidence|default executable path"` passed.

## T042 Bounded Implementation Preservation

`Viewer.runBounded`, `Viewer.runUntilFirstFrame`, and `Viewer.runForFrames`
remain implemented as explicit evidence helpers. Their validation and failure
classification stay separate from persistent launch success classification.

Verification:

- `dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj` passed.

## T044 Generated Guidance Check Evidence

`GeneratedGuidanceCheck` validates that generated task guidance preserves:

- persistent graphical launch as the default readiness path
- bounded smoke, first-frame, frame-count, and scene evidence as explicit helper commands
- rejection of print-only and bounded-only default graphical paths

Verification:

- `./fake.sh build -t GeneratedGuidanceCheck` passed.

## T050 Guidance Gate

`GeneratedGuidanceCheck` confirms generated task and implementation guidance
preserve persistent launch, `skillist`, synthetic disclosure, and risk-level
rules.

Verification:

- `./fake.sh build -t GeneratedGuidanceCheck` passed.

## T045 Surface Baseline Refresh

SkiaViewer public surface baselines were refreshed and package surface checks
were rerun.

Verification:

- `./fake.sh build -t RefreshSurfaceBaselines` passed.
- `PackageSurfaceCheck` initially reported missing `tests/Package.Tests` restore/build artifacts.
- `dotnet restore tests/Package.Tests/Package.Tests.fsproj` passed.
- `dotnet build tests/Package.Tests/Package.Tests.fsproj --no-restore` passed.
- `./fake.sh build -t PackageSurfaceCheck` passed.
