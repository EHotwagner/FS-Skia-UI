# Window Visibility — Feature 090

status=observed
mode=interactive-window
window-visible=observed:true

The production `runInteractiveApp` host opened a **real live Vulkan/Skia window**
in this session (`window-opened=true`, `window-visible=Observed true`,
`first-frame-presented=true`; `logs/live-window-launch.txt`,
`live-window-launch.md`). The dual Wayland/X11 `libdecor-gtk` hazard is avoided by
forcing the X11 path (see `live-window-launch.md`). See also
`interactive-visible-window.md` and `real-image-evidence.md`.
