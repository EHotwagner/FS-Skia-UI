# Window-State Diagnostics (080)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by this feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so there is no
  visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle; the render-only preview path and
  the decode gate run to completion and exit.
- diagnostic-class=product-defect — none observed; no window code path is exercised.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only visibility
claim is made — there is simply no window in scope. The preview renders use
`SkiaViewer.captureScreenshotEvidence` with `CaptureMode = ViewerRenderTargetPng` (off-window
raster), not a windowed host. Native-Skia-absent decoding is classified as a blocking host
warning by the fidelity gate, separately from any window state.
