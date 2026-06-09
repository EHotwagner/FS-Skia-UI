# Window visibility — feature 086

Live persistent-window visibility facts for the controls-family default app (SC-002/SC-003),
captured on a display-capable host. See `interactive-visible-window.md` for the full record.

| Field | Value |
|-------|-------|
| status | ok |
| mode | interactive-window |
| window-visible | observed:true |
| accessible-window | true |
| first-frame-presented | observed:true |
| self-closed-for-evidence | true |

Captured on `DISPLAY=:1` (X11-normalized) via a compiled self-closing host
(`readiness/harness/InteractiveHostEvidence`) launching the controls-family governed default
`ControlsElmish.runInteractiveApp` with the real example controls; `Tick -> CloseWindow` after
the first frames present. Log: `readiness/logs/interactive-launch.txt`.

The keyboard warm-up keystroke-delivery proof (SC-007) requires native keystroke *injection*
within the focus window, which is deferred (see `key-warmup-delivery.txt`); the live window
itself is proven here.
