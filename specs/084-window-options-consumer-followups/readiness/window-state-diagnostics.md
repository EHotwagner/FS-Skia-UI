# Window-State Diagnostics (084)

status=ok

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — the display-capable host (`DISPLAY=:1`) granted a
  real desktop session; the windowed-fullscreen interpreter reads the default-monitor
  work area at the window edge.
- diagnostic-class=window-visibility — a real visible window was observed
  (`window-visible=observed:true`) for the no-flag windowed-fullscreen default and each
  supported startup state (see `interactive-visible-window.md`).
- diagnostic-class=app-lifecycle — the `WindowedFullscreen` arm
  (`applyWindowBehaviorToOptions`: hidden border + work-area geometry +
  `WindowState.Normal`) and the guarded `runAppWithWindowBehavior` launch wiring opened,
  presented a first frame, and self-closed (`close-reason=AppRequestedClose`).
- diagnostic-class=product-defect — none observed; every supported state launched a real
  window and the validation reclassification/default are proven by `tests/SkiaViewer.Tests`.

## Observable-vs-unsupported native facts

native-handle=observed:true
visible=observed:true
focusable=observed:true
renderable-surface=observed:true
input-devices=observed:true

No taskbar-entry or process-only success is claimed, and no unsupported-host-only
visible-window claim is made: the window was a real visible desktop window
(`window-visible=observed:true`, `renderer-mode=skia`) that presented its first frame and
then self-closed for evidence.
