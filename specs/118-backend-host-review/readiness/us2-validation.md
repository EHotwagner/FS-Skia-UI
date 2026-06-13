# US2 validation — the live backend reports whether it read back (feature 118)

**Story:** a live run provably reports its present mode and readback state, goldens untouched.

## FR-007 — live present-mode / readback diagnostic (Category = Swapchain)

- The backend emits the present-mode/readback fact as a `RenderDiagnostic` with
  `Stage = VulkanSwapchain`. `LegacyDiagnosticReported` (`SkiaViewer.fs`) maps
  `VulkanSwapchain → ViewerDiagnosticCategory.Swapchain` and `FrameRender → Frame` (else
  `Renderer`), so the consumer-facing `ViewerDiagnosticEvent` carries `Category = Swapchain`,
  **not** `Renderer`. Other stages keep `Renderer` (no regression — no existing test keyed the
  legacy mapping to `Renderer` for these stages).
- **Live evidence (real backend):** `readiness/live-host` run in `FEATURE118_MODE=direct`
  emitted exactly one diagnostic at `Stage=VulkanSwapchain`:
  `Warning VulkanSwapchain "SkiaSharp cannot wrap a Vulkan swapchain image as an SKSurface
  (managed-binding limitation, mono/SkiaSharp #1502); DirectToSwapchain is unavailable and the
  viewer uses the OffscreenReadback present path."` → surfaces to a consumer sink as
  `Category = Swapchain`. The `OffscreenReadback` run emitted zero diagnostics (clean default).
- The `DirectToSwapchain` announce (`Info VulkanSwapchain "present-mode=DirectToSwapchain
  readback=false …"`) is emitted by `renderFrameDirect` only when the direct path is actually
  available; on this SkiaSharp build the wrap is unavailable, so the honest live signal is the
  fallback Warning above. The Info-announce seam is retained for the OpenGL-backend resolution.
- **Deterministic test:** `Feature118PresentModeTests` asserts the FR-007 categories
  (`Swapchain`, `Frame`) exist and are distinct from `Renderer` — the consumer-facing surface
  the mapping targets. The actual mapping behaviour is proven by the live capture above.

## FR-008 / SC-007 — goldens untouched

- **No `FrameMetrics` field added.** `FrameMetrics` lives in `FS.Skia.UI.Controls.Elmish.Perf`
  (headless, no backend); a backend present field would be permanently zero/absent there.
- `Perf.runScript` metric goldens are **unchanged** — the present-mode diagnostic is emitted on
  the live present path only and never enters the headless metric path. The golden-absence is a
  positive evidence point: deterministic counts/booleans stay separate from live present timing.

## Independent test path

1. Build `readiness/live-host` (`LiveHost.fsproj`, references the local SkiaViewer).
2. `FEATURE118_MODE=direct dotnet LiveHost.dll` → observe the single `VulkanSwapchain` Warning
   on stderr (`DIAG:` line) and `RESULT: ok frames=40`.
3. `FEATURE118_MODE=offscreen dotnet LiveHost.dll` → zero diagnostics, `RESULT: ok frames=40`.
4. Confirm no `FrameMetrics` `.fsi` field was added (Controls.Elmish surface baseline unchanged).
