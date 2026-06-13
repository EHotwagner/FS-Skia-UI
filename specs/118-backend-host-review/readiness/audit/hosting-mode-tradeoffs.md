# Hosting-mode tradeoffs (feature 118, US3 / FR-009)

Every host entry point, what it actually does, and its performance character. The headline:
**deterministic evidence / readback runs are a correctness proof, NOT a live-performance
proxy.** They render offscreen and read back (or just count window ticks); none of them
measures the live present cost, and none should be read as a frame-rate number.

## Host modes

| Mode | Where | What it does | Present path | Performance character |
|------|-------|--------------|--------------|-----------------------|
| `ControlsElmish.runInteractiveApp` | Controls.Elmish → `runPresentedPersistentWindow` → `VulkanHost.run` | Persistent interactive window; pointer/keyboard routed; renders each frame via `renderFrame` | Live: OffscreenReadback (DirectToSwapchain degrades to it, §audit) | Pays the per-frame readback round-trip (~120× draw, see present-path-audit §2). The real live cost. |
| `Viewer.runApp` / `runAppWithWindowBehavior` | SkiaViewer → `runPresentedPersistentWindow` → `VulkanHost.run` | Persistent windowed generated-app host | Live: OffscreenReadback | Same per-frame readback cost as `runInteractiveApp`. |
| `Viewer.runInteractiveViewer` (+`…WithWindowBehavior`) | SkiaViewer → `runPresentedPersistentWindow` → `VulkanHost.run` | Persistent, size/pointer-aware interactive window | Live: OffscreenReadback | Same per-frame readback cost. |
| `Viewer.runBounded` / `runForFrames` / `runUntilFirstFrame` | SkiaViewer | Opens a Silk.NET window and counts N render ticks for **lifecycle evidence**; `ignore scene` — does **not** invoke the Skia render path | No scene present (window-lifecycle only) | NOT a render benchmark; proves window open/first-frame/bounded-exit, nothing about present cost. |
| `Viewer.captureScreenshotEvidence` / `Feature 118` on-demand capture | SkiaViewer | Renders one offscreen surface and reads it back to encode an image | Offscreen readback (by design, on demand) | One-shot; readback is expected here and is *not* a per-frame live cost. |
| `ControlsElmish.Perf.runScript` | Controls.Elmish (headless) | Deterministic frame-metrics driver with **no window and no GPU backend** | None (headless) | Produces `FrameMetrics` counts/booleans only. Present mode is moot here (FR-008); no field added. |

## Evidence is not a live-performance proxy (SC-008)

- The deterministic `Perf.runScript` metrics are **counts and booleans** (view/diff/layout/paint
  ran, memo hits, virtual items, damage rects, …) produced by a **headless** driver with no
  backend. They prove *work reduction*, not wall-clock present cost. No `FrameMetrics` field was
  added for present mode (FR-008) precisely because the headless driver has no backend — such a
  field would be permanently zero/absent and misleading.
- The bounded evidence runs (`runBounded`/`runForFrames`) prove window **lifecycle**, not render
  throughput (they don't render the scene).
- On-demand screenshot capture proves *what* renders (pixels), not *how fast* the live path
  presents.
- The genuine live present cost (the readback round-trip) is a **human/diagnostic signal**
  (FR-011), surfaced live via the FR-007 `ViewerDiagnosticEvent` (Category = Swapchain), never a
  pass/fail gate and never a golden.

## Takeaway for maintainers

When asked "how fast is the viewer?", none of these modes answers it directly. The deterministic
suites answer "did we avoid unnecessary work?"; the live diagnostic answers "which present path
ran and did it read back?". The actual frame-rate win is gated on the OpenGL backend resolution
([opengl-backend-resolution.md](./opengl-backend-resolution.md)), not on any metric this feature
ships.
