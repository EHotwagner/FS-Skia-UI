# Quickstart: Fix Window Visibility

## Design the Public Surface First

1. Update `src/SkiaViewer/SkiaViewer.fsi` with window behavior requests, close reasons, visibility diagnostics, image evidence fields, and unambiguous launch outcomes.
2. Add failing semantic tests through the `.fsi` surface before editing `SkiaViewer.fs`.
3. Refresh surface baselines only after the contract is intentional.

## Validate Interactive Visible Window

Expected regression coverage:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "Interactive|Visibility"
```

The failing-first tests must prove that first-frame presentation does not close `Viewer.runApp`, that taskbar-only or invisible windows are not reported as successful visible launches, and that user close is not inferred from evidence/framework close.

On a supported desktop host:

```bash
dotnet run --project src/Product/Product.fsproj
```

Record `readiness/interactive-visible-window.md` with `mode=interactive-window`, `window-visible=observed:true`, `first-frame-presented=true`, and `self-closed-for-evidence=false`.

## Validate Close Reason Separation

Exercise user, app, evidence, timeout, and failure close paths where supported:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "CloseReason"
```

Record `readiness/close-reason-separation.md`. No evidence or framework close path may report `user-close-observed=true`.

## Validate Window Diagnostics and Options

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --filter "WindowDiagnostics|WindowOptions"
```

On a supported desktop host, launch generated apps with resize policy, maximize policy, startup state, startup position, and backend preference variations. Record observed option results in `readiness/window-options.md` and native facts in `readiness/window-state-diagnostics.md`.

## Validate Real Image Evidence

Run the generated app image evidence command:

```bash
dotnet run --project src/Product/Product.fsproj -- --visual-evidence specs/019-fix-window-visibility/readiness/real-image-evidence.md
```

The command must produce a decodable image when image evidence is requested. Pixel readback or metadata/hash evidence must be labeled as fallback or metadata and must state whether it proves scene rendering, desktop visibility, or both.

## Validate Generated Tests and Verification

```bash
dotnet restore src/Product/Product.fsproj --configfile NuGet.config --no-cache
dotnet test tests/Product.Tests/Product.Tests.fsproj
./fake.sh build -t GeneratedProductCheck
```

Generated validation must fail on `NU1603`, package version mismatch, generated tests not running, misleading image evidence, or bounded evidence substituted for interactive visible-window proof. Record the result in `readiness/generated-validation.md`.

## Validate Governance

```bash
./fake.sh build -t GeneratedGuidanceCheck
./fake.sh build -t TemplateCheck
./fake.sh build -t DependencyReport
./fake.sh build -t EvidenceGraph
./fake.sh build -t EvidenceAudit
./fake.sh build -t Verify
```

Expected readiness outputs:

- `specs/019-fix-window-visibility/readiness/interactive-visible-window.md`
- `specs/019-fix-window-visibility/readiness/close-reason-separation.md`
- `specs/019-fix-window-visibility/readiness/window-state-diagnostics.md`
- `specs/019-fix-window-visibility/readiness/window-options.md`
- `specs/019-fix-window-visibility/readiness/real-image-evidence.md`
- `specs/019-fix-window-visibility/readiness/generated-validation.md`
- `specs/019-fix-window-visibility/readiness/evidence-audit.md`
