# Window-State Diagnostics (092)

status=deferred

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no host was launched in this run; feature 092 is internal
  interactive-state wiring and opens no window here. (The environment HAS a GPU and a display — a
  live Vulkan/Skia window can open — so this is a by-scope deferral, not a hardware limitation.)
- diagnostic-class=window-visibility — deferred (render-only / offscreen): no live window opened;
  all correctness evidence is pure/structural/identity-based and needs no window
  ([[fs-skia-evidence-mode]]).
- diagnostic-class=app-lifecycle — no host launch in this run; the wired seam is exercised through
  `ControlsElmish.resolveFocus`/`routeFocusedText` + `RetainedRender.init`/`step` directly (the
  production code path) under the Expecto suites, not via a live window.
- diagnostic-class=product-defect — none observed in the wired focus/text/render path
  (Controls.Tests 190 + Elmish.Tests 35 + SkiaViewer.Tests 62 pass, incl. the 092 suites).

## Observable-vs-unsupported native facts

native-handle=deferred (no window created in this render-only run)
visible=deferred
focusable=deferred
renderable-surface=deferred (no render-target surface created in this render-only run; not a GPU limitation — the environment has one)
input-devices=not-exercised (no live host; the wired seam is exercised offscreen with synthetic pointer/key inputs through the real routing)

No taskbar-entry or process-only success is claimed. A live launch is neither performed nor required
for this feature's evidence (render-only honesty).
