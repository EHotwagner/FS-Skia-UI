# Diagnostics Readiness

## Scope

Readiness evidence for categorized viewer diagnostics, independent
level/category selection, frame-log sampling, readable summaries, and
capturable in-process sinks.

## Setup Notes

- Tier: Tier 1 contracted viewer diagnostics change.
- Affected areas: `src/SkiaViewer/`, diagnostic fixtures, smoke tests, and
  generated app host diagnostics.
- Public contract impact: `.fsi` signatures must cover diagnostic levels,
  categories, options, events, and sinks when public.
- Synthetic policy: scanner or diagnostic fixture inputs may be synthetic when
  disclosed; final readiness needs real public-surface or generated-product
  diagnostic capture where supported.

## Evidence

- Focused public/MVU diagnostics tests:
  `readiness/logs/skia-viewer-us3-diagnostics-tests.txt`
  - `diagnostic filtering honors categories and level thresholds across startup input renderer and readback categories`
    covers startup, input, frame, renderer, Vulkan, Skia, swapchain, scene,
    screenshot/readback categories and level filtering.
  - `frame sampling excludes repeated per-frame diagnostics unless enabled and bounded by the frame limit`
    proves startup-only diagnostics exclude repeated frame-loop messages and
    frame diagnostics appear only when explicitly enabled or sampled.
  - `diagnostic sink captures startup input renderer and frame categories in-process`
    proves app hosts and tests can assert diagnostics through `Sink` without
    process stderr scraping.
  - `viewer update emits categorized diagnostics for startup input scene frame and failure milestones`
    exercises the public MVU update path for startup, input, scene, frame, and
    swapchain failure diagnostics.
- Generated product scan:
  `readiness/logs/generated-product-us3-diagnostics.txt`
  - Confirms generated product source exposes `--bounded-smoke` for
    startup-focused diagnostics.
  - Confirms generated product source exposes
    `--bounded-smoke-frame-diagnostics` for explicit frame-loop diagnostics.
  - Confirms generated smoke reports write `diagnostic-mode` and captured
    `diagnostic-categories`.

## Category Examples

| Category | Evidence |
|----------|----------|
| Startup | `viewer window open requested for 'Product'`; bounded smoke startup mode |
| Input | `viewer input down: raw='Space' normalized='Space'` |
| Frame | `viewer frame presented at 640x480`; sampled frame diagnostics capped by `FrameLogLimit` |
| Renderer | generated bounded smoke captures renderer startup or unsupported-host diagnostics |
| Vulkan / Skia / Swapchain / Scene / Screenshot | filtering tests assert independent inclusion/exclusion and structured failure categories |

## Independent Validation

Run:

```bash
dotnet run --project tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj
./fake.sh build -t GeneratedProductCheck
```

The first command exercises the public `FS.Skia.UI.SkiaViewer` diagnostics
surface and MVU update effects. The second command verifies the generated app
template exposes startup-focused and frame-focused bounded smoke commands.

## Requirement Mapping

- FR-010: independent diagnostic level/category selection is covered by the
  filtering test and public `Viewer.shouldCaptureDiagnostic`.
- FR-011: frame-loop disable/sample/enable behavior is covered by
  `FrameLogLimit` tests and generated startup/frame smoke modes.
- FR-012: capturable diagnostics are covered by the sink test using
  `Viewer.captureDiagnostic`.
- FR-019: viewer diagnostics name categories, stages, input values, and
  generated smoke evidence paths.
- SC-005: startup-only diagnostics exclude frame messages unless frame
  diagnostics are explicitly enabled.
- SC-006: in-process capture asserts startup, input, renderer, and frame
  diagnostics without stderr scraping.
