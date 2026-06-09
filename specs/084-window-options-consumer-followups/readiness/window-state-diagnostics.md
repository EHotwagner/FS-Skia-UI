# Window-State Diagnostics (084)

status=deferred

## Diagnostic classes (separation preserved)

- diagnostic-class=environment-session — the framework repo opens no desktop session
  window; the windowed-fullscreen interpreter reads the default-monitor work area at
  the window edge on a display-capable host.
- diagnostic-class=window-visibility — deferred; the real visible-window launch is
  captured from the generated executable on a display-capable host, not here.
- diagnostic-class=app-lifecycle — the new `WindowedFullscreen` arm
  (`applyWindowBehaviorToOptions`: hidden border + work-area geometry +
  `WindowState.Normal`) and the guarded `runAppWithWindowBehavior` launch wiring are
  exercised by the unit tests and the FSI surface session.
- diagnostic-class=product-defect — none observed; the validation reclassification and
  default change are proven against the built library (`tests/SkiaViewer.Tests`).

## Observable-vs-unsupported native facts

native-handle=deferred
visible=deferred
focusable=deferred
renderable-surface=deferred
input-devices=deferred

No taskbar-entry or process-only success is claimed, and no unsupported-host-only
visible-window claim is made. The native facts are honestly `deferred` because the
authoritative visible-window capture runs on a display-capable host via the generated
product (the project's documented non-authoritative `GeneratedProductCheck` path); on
this framework dev host the launch degrades to render-only.
