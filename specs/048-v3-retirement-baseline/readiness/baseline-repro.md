# Baseline reproduction (T010, SC-001/002)

Every headline metric in `docs/reports/_baselines/2026-06-02-v3-before.md` re-runs from
its recorded command at the pin `031e5607…`. Re-run results match the report.

## Pin
```
$ git rev-parse HEAD
031e56072779c736adf6dd8b0345e17b58a62e73
```

## Monolith LOC (matches §2)
```
$ wc -l src/Lib/*.fs src/Lib/*.fsi
   835 src/Lib/AgentValidation.fs
     6 src/Lib/InternalsVisibleTo.fs
  1398 src/Lib/KeyboardInput.fs
  2408 src/Lib/Library.fs
    92 src/Lib/VulkanResources.fs
   119 src/Lib/VulkanStartup.fs
   261 src/Lib/AgentValidation.fsi
   450 src/Lib/KeyboardInput.fsi
   552 src/Lib/Library.fsi
    64 src/Lib/VulkanResources.fsi
    29 src/Lib/VulkanStartup.fsi
  6214 total
```
Matches the report's per-file table and 6214 total.

## Duplicate-type inventory (matches §4)
```
$ comm -12 <(grep -oE '^type [A-Za-z0-9]+' src/Scene/Scene.fsi | sort -u) \
           <(grep -oE '^type [A-Za-z0-9]+' src/Lib/Library.fsi | sort -u) | wc -l
34
```
Matches the report's count of 34 shared types.

## Leak proof (matches §5, SC-002)
```
$ grep -n "Lib/Lib.fsproj" src/SkiaViewer/SkiaViewer.fsproj
21:    <ProjectReference Include="..\Lib\Lib.fsproj" />
$ dotnet list src/SkiaViewer/SkiaViewer.fsproj reference
  ../Lib/Lib.fsproj
  ../KeyboardInput/KeyboardInput.fsproj
  ../Scene/Scene.fsproj
$ dotnet list src/Elmish/Elmish.fsproj reference
  ../Scene/Scene.fsproj
  ../SkiaViewer/SkiaViewer.fsproj
```
`SkiaViewer → Lib` and `Elmish → SkiaViewer → Lib` confirmed: the leak reproduces.

## Consumer inventory (matches §6)
```
$ grep -rlE 'Lib[/\\]Lib\.fsproj|PackageReference Include="FS\.Skia\.UI"' --include=*.fsproj samples tests src
```
Reproduces the 12-consumer set: `src/SkiaViewer`; samples `BasicViewer`, `DemoReel`,
`EffectsGallery`, `InteractiveViewer`, `ParityGallery`, `ScreenshotGallery`; tests
`Lib.Tests`, `Smoke.Tests`, `Package.Tests`, `Parity.Tests`, `Governance.Tests`.

## ADR presence + cross-link (T020, SC-006)
```
$ ls docs/adr/0007-*.md docs/adr/0008-*.md docs/adr/0009-*.md docs/adr/0010-*.md docs/adr/0011-*.md
$ grep -c "0007\|0008\|0009\|0010\|0011" docs/reports/2026-06-02-v3-modular-distribution-implementation-plan.md
```
All five ADRs are present with the required sections (Status, Date, Decision source,
Context, Decision, Alternatives, Rationale, Affected stages) and are linked from the
programme implementation plan.
