# Window-State Diagnostics (099, R4)

status=not-applicable

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no desktop session window is opened by this feature.
- diagnostic-class=window-visibility — not-applicable; no window is created, so there is no
  visible/invisible window state to observe.
- diagnostic-class=app-lifecycle — no persistent app lifecycle is started; the deterministic
  animates-vs-snaps / survival / determinism / identity-at-rest / GC / scoped-repaint suites run to
  completion and exit.
- diagnostic-class=product-defect — none observed; no window code path is exercised by this feature.

## Observable-vs-unsupported native facts

native-handle=not-applicable
visible=not-applicable
focusable=not-applicable
renderable-surface=not-applicable
input-devices=not-applicable

No taskbar-entry or process-only success is claimed, and no unsupported-host-only visibility claim is
made — there is simply no window in scope. The clock is exercised through the pure `advance` /
`updateClockForState` / `sampleOnPaint` core and the live retained seam (`RetainedRender.advance`/`step`
over `ControlRuntime.applyRuntimeVisualState`); the animates-vs-snaps proof uses the production render
path (off-window), not a windowed host.
