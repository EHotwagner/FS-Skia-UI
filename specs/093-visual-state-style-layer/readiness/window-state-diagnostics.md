# Window-State Diagnostics (093)

status=deferred

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — no host was launched in this run;
  feature 093 is a pure styling-layer change and opens no window here. (The
  environment HAS a GPU and a display — a live Vulkan/Skia window can open — so
  this is a by-scope deferral, not a hardware limitation.)
- diagnostic-class=window-visibility — deferred (render-only / offscreen): no
  live window opened; all correctness evidence is pure `ResolvedStyle` /
  structural-`Scene` equality and needs no window ([[fs-skia-evidence-mode]]).
- diagnostic-class=app-lifecycle — no host launch in this run; the resolver and
  the migrated render path are exercised through `Style.resolve` and
  `ControlInternals.faithfulContent` / `RetainedRender.init`/`step` directly (the
  production code path) under the Expecto suites, not via a live window.
- diagnostic-class=product-defect — none observed in the resolver or the migrated
  paint path (Controls.Tests 213 pass, including the 6 feature-093 suites and the
  080/085/086/091/092 parity guards).

## Observable-vs-unsupported native facts

native-handle=deferred (no window created in this render-only run)
visible=deferred
focusable=deferred
renderable-surface=deferred (no render-target surface created in this render-only run; not a GPU limitation — the environment has one)
input-devices=not-exercised (no live host; the migrated render path is exercised offscreen via structural-Scene comparison through the real code path)

No taskbar-entry or process-only success is claimed. A live launch is neither
performed nor required for this feature's evidence (render-only honesty).
