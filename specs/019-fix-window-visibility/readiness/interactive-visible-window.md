# Interactive Visible Window Evidence

status=ok
mode=interactive-window
window-visible=observed:true
accessible-window=true
first-frame-presented=true
self-closed-for-evidence=false
command=timeout 5s dotnet run --project template/base/src/Product/Product.fsproj --no-build
source-log=specs/019-fix-window-visibility/readiness/logs/t024-supported-host-persistent-attempt.txt

## Observed Output

- `window-opened=true`
- `window-visible=observed:true`
- `accessible-window=true`
- `first-frame-presented=true`
- `self-closed-for-evidence=false`
- `input-dispatch=not-verified`
- `exit-path=true`
- `diagnostic-class=environment-session-ready`
- `display-variable=WAYLAND_DISPLAY=wayland-0`
- `display-socket-exists=true`

The generated default executable path ran `Viewer.runApp viewerOptions
generatedHost` and reported `mode=interactive-window`, not bounded evidence
mode. The command returned normally on this desktop session. The log also
contains `Failed to load plugin: 'libgtk-3.so.0...'`; that message did not
block the Silk.NET window path for this run, but it remains a host diagnostic
to watch during broader validation.

## First-Frame Persistence

Focused SkiaViewer tests in
`readiness/logs/t017-skiaviewer-interactive-tests.txt` and FSI transcript
`readiness/fsi/t017-interactive-launch.txt` prove first-frame presentation
does not emit `CloseWindow`, does not set `user-close-observed=true`, and keeps
the interactive lifecycle running until an explicit user/app/host/failure close
transition.

## Synthetic Disclosure

No fake window-loop was used for this readiness record. The command evidence
uses the generated executable path on the current desktop session.
