# Governance risk levels

This feature's change is classified **broad** risk.

- **small** — a single framework-internal edit with no public-surface or
  consumer-contract impact (e.g. one `src/Controls/**/*.fs` change). Focused
  validation: `Dev` only (inner-loop tier).
- **medium** — an additive public `.fsi` surface change confined to one shipped
  package (a focused per-package surface move). **Required evidence**: the
  regenerated surface baselines plus a clean `PackageSurfaceCheck` /
  `PerPackageSurfaceDiff`, the focused interaction tests, and an FSI transcript.
  This feature is larger than that tier.
- **broad** — a consumer-contract change touching the host `ViewerEvent`
  contract, public `src/**/*.fsi` across **three** shipped packages
  (`FS.Skia.UI.Controls` new `Pointer` module, `FS.Skia.UI.Controls.Elmish`
  `interpretPointerEffect`/`interpretPointerOutcome`, `FS.Skia.UI.SkiaViewer`
  `ViewerEvent` extension), and `template/**` (a new pointer sample fragment).
  **This feature sits here.** **Required evidence**: regenerated surface
  baselines (`PackageSurfaceCheck` + `PerPackageSurfaceDiff`), the `FsiTranscripts`
  gate, `ControlsInteractionCheck` / `ControlsRenderingCheck`, the runnable
  `PointerInteractionGallery` sample smoke, `GeneratedGuidanceCheck` /
  `TemplateDrift`, and **broad validation** via `GeneratedProductCheck` — every
  gate `./fake.sh build -t Route` prints for this diff.

The `ViewerEvent` `PointerPressed`/`PointerReleased` arity change is source-breaking
only for matchers; the sole in-tree matchers (`src/SkiaViewer/SkiaViewer.fs`, plus
the `InteractiveViewer` / `ScreenshotGallery` samples and `Lib.Tests`) are updated
in lockstep, and the two new cases (`PointerScrolled`, `PointerExited`) are purely
additive.

FAKE-backed gates run **sequentially** (shared `.fake` state); the authoritative
verdict is the per-target result, with `EvidenceAudit verdict=PASS` as the merge
gate. Any aggregate umbrella result is non-authoritative.
