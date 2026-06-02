# No-consumer grep

command: `grep -rn -E "Lib\.fsproj|Include=\"FS\.Skia\.UI\"" samples tests src --include=*.fsproj`
artifact path: this file.
failure class: ConsumerReferenceResidue.
next action: the single remaining hit retires with the monolith in Stage 5.

## Result

```
tests/Package.Tests/Package.Tests.fsproj:20:    <ProjectReference Include="..\..\src\Lib\Lib.fsproj" Condition="Exists('..\..\src\Lib\Lib.fsproj')" />
```

- **Zero sample consumers** of `Lib.fsproj` / the `FS.Skia.UI` monolith package (SC-001).
  `samples/InteractiveViewer` now references `FS.Skia.UI.Input`; `samples/ParityGallery` was already
  on `Scene`+`SkiaViewer`.
- **Test consumers:** `tests/Lib.Tests` and `tests/Parity.Tests` no longer reference `Lib`. The **only**
  remaining `src/Lib` consumer is `tests/Package.Tests` — a deliberate **packaging-contract** consumer
  that asserts the still-published `FS.Skia.UI` surface (`typeof<FS.Skia.UI.ParityReport>.Assembly`, the
  `VulkanResources`/`VulkanStartup` non-exports, the `PackLocal` entry). It must keep referencing the
  monolith while `FS.Skia.UI` is a published package; it retires **with the monolith in Stage 5**
  (maintainer-confirmed; FR-011).

## Status

- `src/Lib` is still present and `FS.Skia.UI` is still packable (deletion/unpublish is the Stage 5
  boundary). Every keyboard-input + parity-bridge consumer is off `Lib` (FR-010).
- **SC-007 amended:** the "fully reference-free" end state is a Stage 5 outcome — it cannot hold while
  `FS.Skia.UI` is a published package under packaging-contract tests.
