# Interactive Visible Window Evidence (084)

status=ok
mode=interactive-window
window-visible=observed:true
accessible-window=true
first-frame-presented=true
self-closed-for-evidence=true

## Real visible-window launch (SC-001 / SC-002, captured on a display-capable host)

Captured on the display-capable host (`DISPLAY=:1`, 2026-06-09) by launching the
persistent interactive path (`Viewer.runApp` / `Viewer.runAppWithWindowBehavior`) with
a self-closing evidence host (it emits `CloseWindow` after the first frames present, so
the launch returns a real `ViewerLaunchOutcome`).

The **no-flag default** opened in windowed fullscreen and each supported startup state
produced its matching real window — every one returned `window-opened=true`,
`window-visible=observed:true`, `mode=interactive-window`, `first-frame-presented=true`,
`exit-path=true`, `renderer-mode=skia`, `close-reason=AppRequestedClose`,
`user-close-observed=false`. None reported "unsupported".

| launch | window-opened | window-visible | mode | first-frame |
|--------|---------------|----------------|------|-------------|
| no-flag default (windowed fullscreen) | true | observed:true | interactive-window | true |
| windowed-fullscreen (explicit) | true | observed:true | interactive-window | true |
| normal | true | observed:true | interactive-window | true |
| maximized | true | observed:true | interactive-window | true |
| fullscreen | true | observed:true | interactive-window | true |

No taskbar-only or process-only substitution is claimed; the window was a real visible
desktop window that presented its first frame and then self-closed for evidence
(`close-reason=AppRequestedClose`). The framework surface (new default + Honored
reclassification) is independently proven in `readiness/fsi-session.txt` and
`tests/SkiaViewer.Tests`. The decodable launch image is recorded in
`real-image-evidence.md` (`readiness/screenshots/windowed-fullscreen-launch.png`).
