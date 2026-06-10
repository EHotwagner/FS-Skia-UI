# Window-State Diagnostics (091)

status=deferred

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no host was launched in this run; feature 091 is internal
  render-path wiring and opens no window here. (The environment HAS a GPU and a display — a live
  Vulkan/Skia window can open — so this is a by-scope deferral, not a hardware limitation.)
- diagnostic-class=window-visibility — deferred (render-only / offscreen): no live window was
  opened; all correctness evidence is pure/structural and needs no window
  ([[fs-skia-evidence-mode]]).
- diagnostic-class=app-lifecycle — no host launch in this run; the wired retained path is
  exercised through `RetainedRender.init`/`step` directly (the production code path) under the
  Expecto suite, not via a live window.
- diagnostic-class=product-defect — none observed in the wired render/diff path (181/181 tests
  pass, including the wired round-trip/determinism/totality/identity-at-rest properties).

## Observable-vs-unsupported native facts

native-handle=deferred (no window created in this render-only run)
visible=deferred
focusable=deferred
renderable-surface=deferred (no render-target surface created in this render-only run; not a GPU limitation — the environment has one)
input-devices=not-exercised (no live host; the wired path is exercised offscreen)

No taskbar-entry or process-only success is claimed. A live launch is neither performed nor
required for this feature's evidence (render-only honesty).
