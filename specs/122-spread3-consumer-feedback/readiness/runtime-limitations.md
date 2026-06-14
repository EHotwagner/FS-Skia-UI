# Runtime limitations & failure diagnostics (feature 122)

## Platform / runtime support boundary

Feature 122 fixes a live-present defect (the `DirectToSwapchain` idle path now re-presents the cached
last good frame to keep every swapchain buffer populated, instead of skipping the buffer swap), threads
window behavior into the live controls launch, guards `CustomControl` against null content, and corrects
governance/skill documentation. It runs on a .NET 10 desktop host rendering through OpenGL via the
SkiaSharp preview native binding, on Windows and Linux desktop (`net10.0`).

Platform boundary (single-line, exact tokens): .NET 10 desktop, OpenGL, SkiaSharp preview,
unsupported macOS/mobile/browser, no software-renderer fallback — a host without a working GL context
classifies as `UnsupportedEnvironment`, not a product defect.

This feature is no package/API/runtime support expansion in the consumer sense — it is an additive,
defaulted public overload plus an internal present-path correctness fix and documentation.

## The Wayland windowed-fullscreen visual blink is not reproducible headless

The reported symptom — interleaved-black frames on a **Wayland windowed-fullscreen**
`DirectToSwapchain` window with a static scene — requires a real Wayland windowed-fullscreen
compositor with a 3+ buffer swapchain. That environment is **not available in this headless / no
windowed-fullscreen-compositor CI**, exactly as prior host features (118/119/120/121) recorded the
persistent window is not drivable here. The fix is therefore proven on:

- the **pure** `GlHost.planPresent` decision and the host's bounded buffer-fill state machine
  (`Feature122PresentPathTests`): after a change, `bufferFillDepth` buffer-filling presents occur
  before any `SkipPresent`, so no buffer in the rotation is ever undrawn; a static run still reaches
  full idle (`SkipPresent`), preserving the feature-120/121 no-scene-walk idle win;
- the **untouched** offscreen/readback path → byte-identical screenshot goldens (`Dev`).

The end-to-end Wayland visual no-blink observation is a disclosed `[-]` item (tasks.md T016) with this
rationale — it is **not** asserted as a synthetic pass.

## Root cause (verified) vs the consumer's inference

The Spread3 reporter inferred feature-120 skip-paint was "ruled out" because a forced per-frame delta
still blinked. Dogfood-verify found the real mechanism: the idle branch skipped the buffer **swap**
entirely (`OpenGl.fs`), betting on double-buffering; a 3+ buffer compositor then rotated an **undrawn**
buffer into view. The fix keeps the no-scene-walk idle win but re-presents the cached last good frame
(a single image blit) until all buffers are filled.

## Safe-failure diagnostics

- The present-path change is observability-friendly: the host tracks `skippedPresentCount` (full idle)
  and `representedCount` (bounded re-present), so a regression that reintroduces undrawn-buffer skips is
  visible.
- A null/blank `CustomControl` `Id` (or a null effect string) degrades safely: `validate` returns a
  missing-required diagnostic and `create` falls back to a safe id, instead of throwing an NRE.
- `runInteractiveAppWithWindowBehavior` reuses the same launch/validation seam as
  `runInteractiveViewerWithWindowBehavior`; an unsupported window option diagnoses under the existing
  `window-options` classification, never silently ignored.

## Determinism & oracle backstop

`GlHost.planPresent` is a pure function of `(prev, next, sizeChanged, idleRepresentsRemaining)` — no wall
clock — so the present decision is deterministic and unit-tested in isolation. The offscreen/readback
path is unchanged, so the fix changes which buffer is presented, not what is drawn.
