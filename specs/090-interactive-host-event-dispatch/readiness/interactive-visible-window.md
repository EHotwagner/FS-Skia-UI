# Interactive Visible Window Evidence (090)

status=observed
mode=interactive-window
window-visible=observed:true
accessible-window=observed:true
first-frame-presented=observed:true
self-closed-for-evidence=false

## Observed

The production interactive host (`ControlsElmish.runInteractiveApp`, `live-host/`)
opened a **real live Vulkan/Skia window** here: `window-opened=true`,
`window-visible=Observed true`, `first-frame-presented=true`, renderer `skia`,
closing via `AppRequestedClose` after presenting the production render path
(`logs/live-window-launch.txt`, `live-window-launch.md`). No taskbar-only or
process-only substitution is claimed — the window genuinely opened and presented.
The on-screen pixel grab is not captured (no external screenshot tool in this
session); the decodable render-target frames live under `responds-proof/`.
