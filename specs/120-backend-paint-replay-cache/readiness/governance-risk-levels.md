# Governance risk levels (feature 120)

feature-tier=tier-1-contracted
affected-packages=FS.Skia.UI (Scene IR: additive CachedSubtree case + CacheBoundary); FS.Skia.UI.Controls (RetainedRender: hashScene fingerprint, PictureCacheKey→{Box;Fingerprint}, Fragment.Fingerprint, CachedSubtree emission, DirtyArea union, replay counters); FS.Skia.UI.Controls.Elmish (FrameMetrics: +PaintDuration/ComposeDuration non-golden, +Replay* golden); FS.Skia.UI.SkiaViewer (new internal PictureReplayCache; GlHost timing + idle-skip; ViewerOptions.PresentMode docstring; GlHost.lastPresentTiming/shouldPresent vals)
public-api-impact=additive only — new Scene case + record, new FrameMetrics fields, corrected docstring, two new SkiaViewer vals; no removals, no signature breaks (migration: additive)
mvu-applicability=the idle-skip is a pure present-or-skip decision (GlHost.shouldPresent) consulted in the RenderFrame interpreter edge; record/replay + per-phase timing execute in the interpreter, never in update; no new consumer Msg/Cmd contract
route-tier=agent-ready (Route printed the controls-public-surface set: Dev, PackageSurfaceCheck, PerPackageSurfaceDiff, FsiTranscripts, GeneratedProductCheck, ControlsCatalogCheck, ControlsCatalogGenerationCheck, DesignTokenDrift, ContrastCheck, ControlsDocCoverageCheck, ControlsInteractionCheck, ControlsRenderingCheck, GeneratedGuidanceCheck, TemplateDrift, EvidenceGraph, EvidenceAudit)
constitution=unchanged (no constitution amendment)

## Risk classification

- **small** — framework-internal `.fs` bodies + their tests: focused `Dev`.
- **medium** — live sample present-mode + perf-corpus golden updates: GeneratedProductCheck/TemplateCheck + golden diff.
- **broad** — additive public `.fsi` (Scene case + FrameMetrics fields + docstring + SkiaViewer vals): broad validation.

THIS feature is **broad** — additive public `.fsi` across four packages. The full routed
controls-public-surface gate set Route printed is run, sequentially. Non-authoritative aggregate
results are advisory only (`aggregate-hang-diagnostics.md`); the authoritative verdict is the focused
per-target rerun.

## Required evidence per risk level

- **broad (public .fsi):** regenerated per-package + top-level surface baselines (RefreshSurfaceBaselines);
  the additive delta shown by PerPackageSurfaceDiff (Scene CachedSubtree/CacheBoundary, FrameMetrics
  fields, SkiaViewer docstring + vals); FsiTranscripts exercising the new surface.
- **US1 (timing):** Feature120MetricsTests (timing Zero on deterministic path) + live distinct
  paint/compose durations (sample-smoke/live-host-evidence.txt).
- **US2 (idle-skip):** Feature120ReplayCacheTests idle-skip decision + live idle-frame (0,0) timing
  (smoke/idle-zero-redraw.md, sample-smoke/live-host-evidence.txt).
- **US3 (replay):** Feature120FingerprintTests + Feature120ReplayCacheTests (cache-on/off pixel parity,
  LRU/dispose, oracle) + Feature120MetricsTests (replay counters) (smoke/replay-readback-parity.md,
  smoke/forced-staleness.md).
- **US4 (cleanups):** DirtyArea union test, present-mode docstring/sample, dead-ref removal
  (smoke/present-mode.md).
- **merge gate:** EvidenceGraph + EvidenceAudit verdict=PASS (0 synthetic).
