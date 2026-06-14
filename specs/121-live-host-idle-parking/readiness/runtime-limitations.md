# Runtime limitations & failure diagnostics (feature 121)

## Platform / runtime support boundary

Feature 121 makes the live persistent interactive loop **consumer-paceable** (an additive,
defaulted `ViewerOptions.FrameRateCap` that gates both update and present cadence), makes the
live per-tick animation-clock advance **allocation-free when idle**, and **publishes** the
pointer interaction surface plus present-mode/environment guidance. It runs on a .NET 10 desktop
host rendering through OpenGL via the SkiaSharp preview native binding, on Windows and Linux
desktop (`net10.0`).

Platform boundary (single-line, exact tokens): .NET 10 desktop, OpenGL, SkiaSharp preview,
unsupported macOS/mobile/browser, no software-renderer fallback — a host without a working GL
context classifies as `UnsupportedEnvironment`, not a product defect.

This feature is no package/API/runtime support expansion in the consumer sense — it is an
additive, defaulted public `ViewerOptions` field plus an internal idle-tick optimization and
documentation of an existing public surface.

## The headless free-run is an environment limitation, not a product defect

The persistent interactive window cannot be driven in this headless/no-compositor CI
environment, exactly as prior features (118/119/120) recorded. On a host **without a blocking
compositor/vsync**, the native event loop (`src/SkiaViewer/Host/OpenGl.fs` `runEventLoop`) has
nothing to block on between frames, so it **free-runs toward the frame target**. Feature 121's
two mitigations are honest about this:

- The loop now gates **both** update and present by the frame interval (FR-002), so the
  `FrameRateCap` actually bounds render cadence rather than presenting every poll iteration.
- The consumer can lower `FrameRateCap` to bound CPU on such a host.

Neither makes a no-compositor host responsive — a truly responsive interactive window requires a
real desktop session (compositor + vsync). That residual is the environment's, not the
framework's, and is **recorded here rather than claimed as an interactive pass**. No interactive
run-and-use evidence is asserted for this feature; the persistent loop change is proven on the
extracted pure pacing decision (`GlHost.shouldAdvanceFrame`) plus reasoning, and the offscreen /
evidence (`runBounded`) path — which does **not** use the persistent loop — is unaffected.

## Already shipped, reconciled here (not re-implemented)

The ControlsShowcase4 feedback's premise was partly stale against 0.1.127. Verified against the
live path and reconciled rather than rebuilt (re-implementing shipped behaviour would create
synthetic/duplicate-behaviour evidence the audit blocks):

- **Live unchanged-frame paint-skip is already shipped** (feature 120): on `DirectToSwapchain`,
  `renderFrame`/`shouldPresent` does no clear/scene-walk/draw for an unchanged scene at an
  unchanged framebuffer size.
- **Graceful in-app quit is already available**: `InteractiveAppHost.Update` returns
  `ViewerEffect list`; returning `[ ViewerEffect.CloseWindow ]` propagates to `AppRequestedClose`
  + `Shutdown` (`SkiaViewer.fs`). No new host-contract field is added.

## Safe-failure diagnostics

- An invalid `FrameRateCap` (`Some n`, `n <= 0`) is rejected at option validation
  (`validateOptions`) with a clear startup diagnostic — `Window` / `ProductDefect` / `Startup`,
  "Viewer frame-rate cap must be positive." — consistent with the existing positive-size check,
  rather than launching a misconfigured window.
- The idle-tick optimization degrades safely: `RetainedRender.advanceStateClocks` only short-
  circuits when **no** clock is active; the instant any clock is active it advances exactly as
  the per-clock `advance` oracle, so a live cross-fade (features 099/103) never freezes.

## Determinism & oracle backstop

`GlHost.shouldAdvanceFrame` is a pure function of `(lastFrameTime, now, frameInterval)` — no wall
clock inside it — so the pacing decision is deterministic and unit-tested in isolation. The
no-alloc invariant is asserted by reference-equality on the all-inactive state map, with the
active path checked against the `advance` oracle, so the optimization changes cost, not behaviour.
</content>
