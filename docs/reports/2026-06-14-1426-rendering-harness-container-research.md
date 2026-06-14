---
title: Rendering Harness Container Research
index: 28
description: Research and local environment analysis for a faithful rendering-development harness in the current GPU-passthrough Wayland/X11 container, covering screenshot evidence, frame pacing, performance measurement, and mouse/keyboard automation.
---

# Rendering Harness Container Research

- **Timestamp:** 2026-06-14T14:26:00+02:00
- **Status:** Research / implementation guidance.
- **Scope:** FS.Skia.UI rendering development in the current Linux container, with GPU passthrough, host-provided X11/Wayland sockets, and unreliable Wayland behavior.
- **Requested constraint:** The user can install software and change container settings. The report focuses on what would make screenshotting, performance, and mouse/keyboard input faithful enough for rendering development.
- **Non-scope:** This report does not run repository governance machinery and does not propose adding another broad governance layer. The recommendations explicitly separate lightweight render development loops from higher-cost host fidelity checks.

---

## 1. Executive Summary

The harness should not try to make one environment prove everything. FS.Skia.UI already has three different evidence categories, and the container/display stack has different trust levels for each:

1. **Deterministic renderer proof:** Use the existing offscreen/readback paths (`Viewer.captureScreenshotEvidence`, `Viewer.runBounded`, `ControlsElmish.captureRespondsProof`, and `ControlsElmish.Perf.runScript`) for fast development. These prove scene rendering, pixel output, routing decisions, and frame-metric regressions without relying on a visible desktop window.
2. **Live-window proof:** Add an X11 integration harness that can find the real window, capture it from the X server, and inject X11 input. This needs `xdpyinfo`, `xrandr`, `xinput`, `xdotool`, `xwd` or `maim`/ImageMagick, and a window manager or compositor that gives the window stable focus/visibility.
3. **Faithful performance proof:** Do not use Xvfb as the performance authority. Xvfb is useful for "can create a window" tests, but it is a memory framebuffer with no physical display or input hardware model. Vsync, pacing, tearing, compositor latency, and present timing need a real KMS/display path: a host Xorg session on the GPU, a dummy HDMI plug, or a host-level virtual KMS (`vkms`) setup with a real Xorg/DRM stack.
4. **Wayland should remain out of the critical path for now.** This container currently exposes `WAYLAND_DISPLAY=wayland-0`, but the user reports that rendering only works reliably when Wayland is deactivated. Wayland screenshot and input automation are portal/compositor-mediated by design; X11 is the lower-friction automation backend for this project today.

The local probes show that the current container is close to useful:

- `DISPLAY=:1` works.
- OpenGL is hardware accelerated through AMD/Mesa (`AMD Radeon Graphics`, Mesa `26.1.2`).
- A short `glxgears` probe reported about **119.8 FPS** with default synchronization and about **9411 FPS** with `vblank_mode=0`, which strongly suggests the current X display is pacing buffer swaps against a real refresh source.
- Vulkan exposes `VK_EXT_present_timing`, `VK_KHR_present_wait`, `VK_KHR_present_id`, Xlib/XCB/Wayland surfaces, `VK_KHR_display`, and `VK_EXT_headless_surface`; this is useful background, but FS.Skia.UI's live path is currently OpenGL/Skia, so the immediate timing surface is GLX/EGL swap behavior and the host's measured paint/compose durations.
- The missing pieces are mostly tooling and container/device access: no `xdpyinfo`, `xrandr`, `xinput`, `xdotool`, `xwd`, `maim`, `grim`, `weston`, `Xvfb`, `Xephyr`, `xpra`, or `/dev/uinput` are currently present.

The recommended implementation is a **tiered harness**, not one "perfect" runner:

| Tier | Purpose | Display dependency | Authoritative for |
|------|---------|--------------------|-------------------|
| T0 | Pure scene/control rendering and retained routing | None | Determinism, tree equality, routing semantics, non-blank offscreen PNGs |
| T1 | Offscreen GPU/CPU screenshot capture | EGL/Skia offscreen or current viewer readback | Pixel output of renderer path, not desktop visibility |
| T2 | X11 live-window smoke | Xorg/Xwayland-compatible X11 server, window manager | Window creation, visibility, focus, X11 input, desktop screenshot |
| T3 | Faithful frame pacing/performance | Xorg or DRM/KMS with real vblank | Vsync, frame pacing, swap blocking, end-to-end latency |
| T4 | Manual/interactive diagnostics | Developer desktop | Visual inspection, exploratory debugging |

The important policy decision: **each artifact must state what it proves**. A render-target PNG is valid render evidence but not desktop visibility proof. A captured X11 window is live-window proof but not necessarily frame-pacing proof. A timing run is only frame-pacing proof when the report records the display mode, swap interval, present backend, and observed refresh source.

---

## 2. Local Environment Findings

### 2.1 Session and Device Facts

Observed environment:

```text
DISPLAY=:1
WAYLAND_DISPLAY=wayland-0
XAUTHORITY=/tmp/.Xauthority
XDG_RUNTIME_DIR=/run/user/1000
```

Display sockets:

```text
/tmp/.X11-unix/X1 developer:developer 755
/tmp/.X11-unix/X0 nobody:nobody 777
/run/user/1000/wayland-0 developer:developer 755
/run/user/1000/bus developer:developer 666
/run/user/1000/pipewire-0 developer:developer 666
```

GPU devices:

```text
/dev/dri/card1
/dev/dri/renderD128
```

Kernel modules visible from the container include `amdgpu`; `vkms` was not loaded. `/dev/uinput` is not present.

Interpretation:

- The container is not running its own display server. It is consuming host-provided X11/Wayland sockets.
- The current X11 display is usable and GPU-accelerated.
- The container can run graphics clients but cannot currently inspect the X server deeply because the standard X utilities are missing.
- Kernel-level synthetic input cannot currently be tested because `/dev/uinput` is absent.
- Host-level KMS changes, `vkms`, dummy outputs, or seat assignments cannot be solved purely inside this container unless the host/container runtime grants the needed devices and privileges.

### 2.2 GL, EGL, and Vulkan Facts

`glxinfo -B` reported:

```text
direct rendering: Yes
OpenGL vendor string: AMD
OpenGL renderer string: AMD Radeon Graphics (radeonsi, renoir, ACO, DRM 3.64, 7.0.11-arch1-1)
OpenGL version string: 4.6 (Compatibility Profile) Mesa 26.1.2-arch1.1
```

Relevant GLX swap-control extensions are present:

```text
GLX_EXT_swap_control
GLX_EXT_swap_control_tear
GLX_MESA_swap_control
GLX_OML_sync_control
GLX_SGI_swap_control
GLX_SGI_video_sync
```

`eglinfo -B` reported AMD/Mesa rendering for GBM, Wayland, X11, and surfaceless platforms. It also printed:

```text
_amdgpu_device_initialize: amdgpu_query_info(ACCEL_WORKING) failed (-13)
```

That warning did not prevent accelerated GL from working.

`vulkaninfo --summary` reported:

```text
Vulkan Instance Version: 1.4.350
GPU0: AMD Radeon Graphics (RADV RENOIR)
driverInfo: Mesa 26.1.2-arch1.1
```

Relevant Vulkan extensions visible in the full output include:

```text
VK_KHR_xcb_surface
VK_KHR_xlib_surface
VK_KHR_wayland_surface
VK_KHR_display
VK_EXT_headless_surface
VK_EXT_display_surface_counter
VK_KHR_present_id
VK_KHR_present_id2
VK_KHR_present_wait
VK_KHR_present_wait2
VK_EXT_present_timing
```

Interpretation:

- The GPU path is real enough for rendering development.
- The immediate FS.Skia.UI live-host path is OpenGL, not Vulkan, so GLX/EGL swap behavior is the first timing target.
- Vulkan timing support is promising for a future backend or low-level probe, but it should not be presented as proof of the current OpenGL viewer until wired into the actual host.

### 2.3 Local Frame-Pacing Probe

Default `glxgears` for five seconds:

```text
Running synchronized to the vertical refresh.  The framerate should be
approximately the same as the monitor refresh rate.
600 frames in 5.0 seconds = 119.816 FPS
```

With synchronization disabled:

```text
env vblank_mode=0 glxgears
47056 frames in 5.0 seconds = 9411.038 FPS
```

Interpretation:

- The current X display appears to provide a real refresh cadence near 120 Hz.
- The large `vblank_mode=0` delta is a useful sanity check: the default path is not merely CPU limited at 120 FPS.
- This is not yet an FS.Skia.UI performance result. It proves that the display stack can pace a simple GL client. The FS.Skia.UI harness still needs to measure its own `PaintDuration`, `ComposeDuration`, frame intervals, and swap behavior.

### 2.4 Missing Local Tools

Installed:

```text
mesa 1:26.1.2-1
mesa-utils 9.0.0-7
vulkan-radeon 1:26.1.2-1
vulkan-tools 1.4.350.0-1
dbus-run-session
glxinfo
eglinfo
vulkaninfo
```

Missing:

```text
xrandr
xdpyinfo
xinput
xdotool
xwd
import
scrot
maim
grim
slurp
weston
Xvfb
Xorg
Xephyr
xpra
```

The current environment can prove "GL works" but cannot yet run a faithful live-window harness because it lacks:

- X server extension inspection (`xdpyinfo`)
- output/mode inspection (`xrandr`)
- input device inspection (`xinput`)
- X11 automation (`xdotool`)
- X11 screenshot capture (`xwd`, `maim`, or ImageMagick `import`)
- nested or virtual display alternatives (`Xvfb`, `Xephyr`, `xpra`, `weston`)

---

## 3. Repo-Specific Starting Point

### 3.1 The Viewer Already Separates Evidence Classes

`ViewerOptions.PresentMode` currently distinguishes:

- `DirectToSwapchain`: default live path, OpenGL, draws straight to the window/default framebuffer and presents via toolkit buffer swap.
- `OffscreenReadback`: evidence/screenshot path that renders offscreen and reads back pixels.

The public evidence model also distinguishes:

- `ProvesSceneRendering`
- `ProvesDesktopVisibility`
- `ScreenshotCaptureSource.LiveViewerWindow`
- `ScreenshotCaptureSource.DeterministicSceneRender`
- `ScreenshotCaptureSource.PixelReadbackSource`

That is exactly the right shape. The harness should strengthen this separation rather than collapse it.

### 3.2 Existing Deterministic Rendering and Input Proofs

The repo already has good fast-loop seams:

- `Viewer.captureScreenshotEvidence`: render a `SceneNode` to a PNG evidence result.
- `Viewer.runBounded`: bounded evidence run without a persistent interactive window.
- `ControlsElmish.captureRespondsProof`: input-to-visible-change proof through the production control render tree.
- `ControlsElmish.Perf.runScript`: deterministic scripted input frames and frame metrics.
- `FrameMetrics`: deterministic counts plus live-only timings (`FrameDuration`, `PaintDuration`, `ComposeDuration`).

These are valuable and should remain the default for day-to-day rendering development. They avoid making a changing platform depend on a fragile live desktop just to validate every small paint/layout change.

### 3.3 Current Gap

The missing harness is not "more offscreen evidence." The missing harness is a controlled way to answer live questions:

- Did a real OS window open?
- Was it visible and focusable?
- Did the first frame appear in the desktop compositor?
- Can mouse and keyboard input enter through the same path a user uses?
- Are buffer swaps paced by a real refresh source?
- What are the frame interval, paint, compose/swap, and input-to-present distributions?

Those questions require X11/DRM/compositor instrumentation and should live in a separate live-host harness tier.

---

## 4. Online Research Findings

### 4.1 Xvfb Is Useful but Not Faithful for Performance

The Xvfb man page describes Xvfb as an X server that runs with no display hardware or physical input devices and emulates a framebuffer in virtual memory. Its original purpose was server testing, with other uses including batch processing and testing clients against unusual screen configurations. It can even store its framebuffer as `xwd` files when started with `-fbdir` ([Xvfb man page](https://linux.die.net/man/1/xvfb)).

Implication:

- Xvfb is fine for "can the app connect to an X server and draw something?"
- Xvfb is not a faithful authority for vsync, desktop compositor behavior, hardware page flips, physical display timing, input latency, or tearing.
- Xvfb should be a T2 fallback for window creation and basic screenshot automation, not the T3 performance tier.

### 4.2 VKMS Can Provide a Headless KMS Display, But It Is Host-Level

The Linux kernel VKMS documentation describes VKMS as a software-only KMS driver useful for testing and for running X or similar on headless machines. It is loaded with `modprobe vkms` and can be configured through configfs for virtual display pipelines ([kernel VKMS documentation](https://docs.kernel.org/gpu/vkms.html)).

Implication:

- VKMS is a credible path for a headless but KMS-shaped display harness.
- VKMS must be configured at the host/kernel level, not as a normal unprivileged container package.
- VKMS may be enough for KMS/page-flip semantics, but it is still a virtual display. For final hardware-performance truth, a physical output or dummy HDMI plug remains stronger.

### 4.3 Swap Interval Is Only Meaningful with a Display Timeline

The GLX `EXT_swap_control` spec says `glXSwapIntervalEXT` sets the minimum number of video frame periods per buffer swap for a drawable; interval `0` means swaps are not synchronized to a video frame ([GLX_EXT_swap_control](https://registry.khronos.org/OpenGL/extensions/EXT/EXT_swap_control.txt)). Mesa's `MESA_swap_control` spec similarly defines the video-frame-period semantics and notes a default swap interval of `0` for that extension ([MESA_swap_control](https://docs.mesa3d.org/specs/MESA_swap_control.spec)).

Implication:

- A performance harness must record whether swap control is available and what interval is active.
- It must also record what "video frame period" means in the current run: physical monitor, dummy HDMI, VKMS/Xorg output, or memory-only server.
- Measuring `SwapBuffers` duration without knowing the display backend can produce misleading results.

### 4.4 DRM/KMS Events Are the Ground Truth Beneath Desktop Presentation

The DRM userspace API documents vblank events and page-flip completion events. `DRM_MODE_PAGE_FLIP_EVENT` requests an event when a page flip is done; asynchronous page flips may tear because they do not wait for vblank ([DRM userspace API](https://dri.freedesktop.org/docs/drm/gpu/drm-uapi.html)).

Implication:

- A truly faithful T3 harness should prefer a KMS-backed display path where vblank/page-flip timing exists.
- Xorg on a real/dummy output is acceptable because it is backed by DRM/KMS.
- Xvfb is not acceptable for final frame-pacing claims because it lacks that scanout timeline.

### 4.5 Vulkan Has Better Modern Present Timing, But It Is Not the Current Host

`VK_GOOGLE_display_timing` allows applications to query presentation-engine timing and schedule presentation to reduce stutter ([Vulkan `VK_GOOGLE_display_timing`](https://docs.vulkan.org/refpages/latest/refpages/source/VK_GOOGLE_display_timing.html)). `VK_KHR_present_wait` provides a wait API intended to finish when the present is visible to the user, but with loose timing guarantees ([Vulkan `VK_KHR_present_wait`](https://docs.vulkan.org/refpages/latest/refpages/source/VK_KHR_present_wait.html)). `VK_EXT_present_timing` is a newer proposal that exposes refresh timing and per-present timing stages such as first-pixel-out and first-pixel-visible ([Vulkan `VK_EXT_present_timing`](https://docs.vulkan.org/features/latest/features/proposals/VK_EXT_present_timing.html)).

Implication:

- The local Vulkan stack has promising timing extensions.
- Those extensions can support separate low-level diagnostics or a future Vulkan host.
- They should not be used as primary proof for today's OpenGL/Skia host unless the host has a Vulkan backend or an explicit cross-check.

### 4.6 X11 Input Automation Is Straightforward

The XTEST extension exists to test an X11 server with no user intervention and can synthesize key, button, and motion events ([XTEST protocol](https://xorg.freedesktop.org/archive/X11R7.7/doc/xextproto/xtest.html)). `xdotool` uses XTEST and Xlib to simulate keyboard and mouse activity and manipulate windows ([xdotool project page](https://www.semicomplete.com/projects/xdotool/)).

Implication:

- For X11 live-window tests, `xdotool` is the pragmatic first automation layer.
- It is sufficient to verify focus, typing, click, drag, wheel, hover, and close behavior through the real window system.
- It is not a hardware-level input test. For that, use `/dev/uinput` or `evemu` in a higher-privilege harness.

### 4.7 Kernel-Level Input Injection Needs `/dev/uinput`

The Linux `uinput` documentation says userspace can create virtual input devices by writing to `/dev/uinput`; events from those devices are delivered to userspace and in-kernel consumers. It also recommends `libevdev` as less error-prone than direct `uinput` access ([kernel uinput documentation](https://docs.kernel.org/input/uinput.html)).

Implication:

- A hardware-like input harness needs `/dev/uinput` mounted into the container and permissions to write it.
- This is stronger than XTEST because it exercises the normal evdev/libinput path.
- It is also more invasive and should be an opt-in T3/T4 tool, not a default dev loop.

### 4.8 Wayland Screenshot and Input Are Portal/Compositor Mediated

`xdg-desktop-portal` exposes D-Bus portal interfaces under `org.freedesktop.portal.Desktop`, with backend implementations supplied by the desktop environment ([xdg-desktop-portal overview](https://flatpak.github.io/xdg-desktop-portal/)). The screenshot portal lets applications request screenshots and supports screen, window, area, and active-window targets ([screenshot portal documentation](https://flatpak.github.io/xdg-desktop-portal/docs/doc-org.freedesktop.portal.Screenshot.html)). `libei` is an emulated-input protocol primarily aimed at Wayland; its events feed into the compositor input stack while remaining distinguishable to the compositor for access control ([libei documentation](https://libinput.pages.freedesktop.org/libei/)).

Weston documents multiple backends: DRM/KMS, nested Wayland, nested X11, RDP, headless, and PipeWire. Its DRM backend uses KMS/evdev; headless runs without input/output and is useful for tests; the X11 backend nests in an X server ([Weston running documentation](https://wayland.pages.freedesktop.org/weston/toc/running-weston.html), [Weston man page](https://man.archlinux.org/man/weston.1.en)).

Implication:

- Wayland can be tested, but faithful automation is compositor-specific unless using portals/libei.
- Given the user's current Wayland instability, Wayland should not be the primary rendering-development harness.
- Keep Wayland as a future compatibility tier after X11 live-host proof is stable.

---

## 5. What Faithful Screenshotting Requires

There are three screenshot types. The harness should keep them separate.

### 5.1 Render-Target Screenshot

This is the current `OffscreenReadback` evidence path.

Required:

- Known scene/control input.
- Fixed dimensions and scale.
- Renderer mode recorded.
- PNG decode validation.
- Blank/non-blank validation.
- Optional pixel hash or perceptual diff.

Proves:

- The renderer produced pixels for the scene.
- Regressions in paint/layout/text can be caught deterministically.

Does not prove:

- The app opened a real OS window.
- The desktop compositor displayed it.
- The window was unoccluded.
- Input focus or vsync worked.

Recommendation:

- Keep this as the default rendering harness.
- Add a "proof level" field to reports if needed: `render-target`, `live-window`, `presentation-timing`.
- Do not require a live desktop for these tests.

### 5.2 Live X11 Window Screenshot

This captures the real window from the X server.

Required packages:

```text
xorg-xdpyinfo
xorg-xrandr
xorg-xinput
xdotool
xorg-xwd
maim or imagemagick
```

Required runtime conditions:

- `DISPLAY` and `XAUTHORITY` point to the intended X server.
- `WAYLAND_DISPLAY` is unset or ignored for the process under test.
- The viewer starts with a normal window state, not minimized or windowed fullscreen unless that specific mode is being tested.
- A window manager/compositor gives predictable focus and stacking.
- The harness can discover the window by title/class/PID.
- The harness can wait for first frame, then capture by window ID.

Candidate commands:

```bash
WAYLAND_DISPLAY= XDG_SESSION_TYPE=x11 ./path/to/sample --window-startup normal
xdotool search --sync --name 'FS.Skia.UI Harness' windowactivate --sync
xwd -id "$WINDOW_ID" -out live-window.xwd
magick live-window.xwd live-window.png
```

Alternative:

```bash
maim --window "$WINDOW_ID" live-window.png
```

ImageMagick's `import` can capture an X server screen/window to a file ([ImageMagick import](https://imagemagick.org/import/)). `maim` is an X screenshot utility that encodes PNG/JPG/BMP/WebP ([maim man page](https://man.archlinux.org/man/maim.1.en)).

Proves:

- A real X11 window existed and was capturable.
- The compositor/X server presented the contents into the desktop.
- Captured pixels match what a desktop screenshot sees.

Does not prove by itself:

- Vsync pacing.
- Low latency.
- Physical hardware scanout.

Recommendation:

- This should be the main "faithful screenshot" harness for the current container.
- Use X11 first. Wayland screenshots should be a separate compatibility track.

### 5.3 Desktop/Output Screenshot

This captures the whole output, not just the window.

Required:

- X11 root capture (`xwd -root`, `maim`, `import -window root`) or Wayland compositor screenshot through a portal/compositor tool.
- Stable output size and scale.
- Known window placement.
- Occlusion control.

Proves:

- The window is visible in the desktop composition.
- Placement/size/window-state options are honored.

Risks:

- Other desktop content can pollute the image.
- Compositor effects, shadows, scaling, and color management can change pixels.
- It is less deterministic than render-target and window-only capture.

Recommendation:

- Use this only for window-management proof, not pixel-perfect renderer regression.
- Keep strict pixel assertions on render-target PNGs, not whole-desktop captures.

---

## 6. What Faithful Performance Requires

Performance evidence must define the timeline being measured.

### 6.1 Minimum Viable T3 Performance Harness

Required runtime:

- Xorg-backed display with a real refresh source:
  - physical monitor,
  - dummy HDMI plug,
  - or host-level VKMS/Xorg if physical output is impossible.
- GPU device access to both `/dev/dri/card*` and `/dev/dri/renderD*`.
- `DISPLAY`/`XAUTHORITY` for that Xorg server.
- Wayland disabled for the viewer process.
- Stable CPU governor and power mode if comparing timings across commits.

Required tools:

```text
mesa-utils
vulkan-tools
xorg-xdpyinfo
xorg-xrandr
xorg-xinput
xdotool
xorg-xwd or maim
imagemagick
ffmpeg
apitrace
perf
radeontop or other AMD GPU telemetry tool
```

Optional but useful:

```text
mangohud
gamescope
weston
xpra
igt-gpu-tools
```

Required recorded facts per run:

- OS, kernel, Mesa, GPU, driver.
- `DISPLAY`, `WAYLAND_DISPLAY`, `XDG_SESSION_TYPE`.
- `glxinfo -B`.
- GLX swap-control extensions.
- X11 extension list: Present, DRI3, XTEST, RANDR.
- `xrandr --verbose` output: resolution, refresh, scaling.
- Viewer `PresentMode`, `FrameRateCap`, window state, size.
- Whether `vblank_mode` or Mesa env vars are set.
- Frame count, duration, dropped/late frame count.
- Per-frame:
  - update duration
  - paint duration
  - compose/swap duration
  - frame interval
  - input event timestamp if applicable
  - first visible response timestamp when testing latency

The current repo already has `PaintDuration` and `ComposeDuration` fields. The live harness should persist them with frame timestamps and percentile summaries.

### 6.2 What Not To Treat as Performance Truth

Do not use these as final performance proof:

- Xvfb FPS.
- Offscreen render-only FPS alone.
- A free-running loop with `vblank_mode=0` unless the test explicitly measures raw throughput.
- A Wayland/Xwayland session that the user already reports as unstable.
- A host/bind-mounted display where output mode, compositor, and swap interval are not recorded.

They are still useful:

- Xvfb catches gross window/event bugs.
- Offscreen FPS catches renderer throughput regressions.
- `vblank_mode=0` can expose CPU/GPU render ceiling.
- Wayland can become a compatibility matrix entry later.

### 6.3 Performance Modes To Add

The harness should expose named modes:

| Mode | Purpose | Suggested env |
|------|---------|---------------|
| `throughput` | Render as fast as possible | `vblank_mode=0`, no frame cap |
| `paced-60` | App cap behavior independent of 120 Hz monitor | `FrameRateCap=60`, normal swap |
| `paced-native` | Match native refresh | no app cap or cap to `xrandr` refresh |
| `stress-resize` | Surface recreation and layout stability | scripted window resize |
| `input-latency` | input-to-visible-change latency | XTEST/uinput timestamps + screenshot/video |

Each mode should state whether it is deterministic, live-host, or timing evidence.

### 6.4 Vsync Validation Procedure

After installing tools, run:

```bash
xdpyinfo -ext XTEST -ext Present -ext RANDR | tee harness-xdpyinfo.txt
xrandr --verbose | tee harness-xrandr.txt
glxinfo -B | tee harness-glxinfo.txt
glxinfo | rg -i 'swap|oml|sgi_video|present|dri3' | tee harness-glx-extensions.txt
timeout 7s glxgears | tee harness-glxgears-default.txt
timeout 7s env vblank_mode=0 glxgears | tee harness-glxgears-unthrottled.txt
```

Acceptance:

- Default `glxgears` is near display refresh.
- `vblank_mode=0` is much higher than refresh.
- `xrandr` reports the expected output and refresh.
- XTEST is present for automation.
- Present/DRI3 are present for modern X presentation.

For FS.Skia.UI:

```bash
WAYLAND_DISPLAY= XDG_SESSION_TYPE=x11 \
  ./path/to/render-harness --mode paced-native --frames 600 --window-startup normal
```

Acceptance:

- Median frame interval near refresh interval.
- P95/P99 frame intervals within agreed thresholds for the scenario.
- `ComposeDuration` shows blocking/pacing when vsync is active.
- No evidence report claims "vsync" unless the run includes the display-mode and swap-control facts.

---

## 7. What Faithful Mouse and Keyboard Input Requires

### 7.1 X11 Input Tier

For current FS.Skia.UI development, start here.

Required:

```text
xdotool
xorg-xinput
xorg-xdpyinfo
```

Procedure:

```bash
WINDOW_ID="$(xdotool search --sync --name 'FS.Skia.UI Harness' | head -1)"
xdotool windowactivate --sync "$WINDOW_ID"
xdotool mousemove --window "$WINDOW_ID" 40 40
xdotool click 1
xdotool key --window "$WINDOW_ID" A BackSpace Return
```

Proves:

- The app receives X11 keyboard/mouse events through the live window system.
- Focus and window activation work.
- The FS.Skia.UI live input bridge and retained routing respond in a real window.

Limitations:

- XTEST events are synthetic X server events, not hardware evdev events.
- Compositor/window-manager behavior can affect focus and activation.
- Pointer acceleration and libinput behavior are not exercised at the kernel level.

### 7.2 Kernel/uinput Tier

Use this for higher-fidelity input or Wayland compositor tests.

Required:

- `/dev/uinput` mounted into container.
- User/group permission to write `/dev/uinput`.
- `evemu`, `libevdev`, or `ydotool`.
- For observation: `libinput debug-events` or `evtest` if `/dev/input/event*` is exposed.

Container/runtime examples:

```bash
--device /dev/uinput
--device /dev/input
--group-add input
```

Security note:

- `/dev/uinput` can inject arbitrary host input. It should be enabled only for a dedicated harness container or a trusted local dev setup.

Proves:

- Input enters through the same kernel evdev/libinput path as a physical device.
- Useful for compositor-level input behavior and Wayland/libei comparisons.

Limitations:

- More fragile and privileged.
- Requires host cooperation and careful isolation.

### 7.3 Wayland Input Tier

Use only after the X11 tier is stable.

Options:

- Desktop portal remote-desktop/input APIs.
- `libei` where the compositor supports it.
- Compositor-specific protocols/tools.
- `/dev/uinput` as a lower-level workaround if permitted.

Risk:

- Wayland intentionally avoids global input/screenshot powers for arbitrary clients.
- Automation capability differs by compositor and portal backend.
- This is exactly why it should not be the first faithful harness for this project.

---

## 8. Recommended Container and Host Setup

### 8.1 Fastest Reliable Path: Host Xorg + Container Client

This is the recommended first target.

Host:

- Run a real Xorg session on the GPU.
- Prefer a physical monitor or dummy HDMI plug.
- Disable the Wayland path for this project.
- Ensure the X server allows the container user through Xauthority, not broad `xhost +`.

Container mounts/devices:

```bash
-e DISPLAY=:1
-e XAUTHORITY=/tmp/.Xauthority
-v /tmp/.X11-unix:/tmp/.X11-unix
-v /tmp/.Xauthority:/tmp/.Xauthority:ro
--device /dev/dri/card1
--device /dev/dri/renderD128
```

For X11 input automation:

```bash
# no extra kernel device required for xdotool/XTEST
```

For hardware-like input:

```bash
--device /dev/uinput
--device /dev/input
--group-add input
```

Viewer process environment:

```bash
WAYLAND_DISPLAY=
XDG_SESSION_TYPE=x11
GDK_BACKEND=x11
QT_QPA_PLATFORM=xcb
SDL_VIDEODRIVER=x11
```

The GTK/Qt/SDL variables are defensive. FS.Skia.UI uses Silk.NET, so the final necessary knobs should be confirmed against the actual Silk.NET backend in use. The important part is that `WAYLAND_DISPLAY` is unset for the viewer process.

### 8.2 If No Physical Display Exists: Dummy HDMI First, VKMS Second

Best headless-but-faithful options:

1. **Dummy HDMI plug** on the GPU. This gives the GPU a real connector/mode and keeps Xorg/KMS behavior closest to a normal desktop.
2. **Host-level VKMS + Xorg** when physical dummy output is impossible. This gives KMS-shaped behavior and can run Xorg on a virtual display. It is better than Xvfb for frame-pacing structure, but still not the same as physical scanout.
3. **Xvfb/Xdummy** only for window smoke and non-performance screenshot tests.

### 8.3 Package Install Set for Arch

Baseline:

```bash
sudo pacman -S --needed \
  mesa mesa-utils vulkan-radeon vulkan-tools \
  xorg-server xorg-xinit xorg-xdpyinfo xorg-xrandr xorg-xinput xorg-xwd \
  xdotool maim imagemagick ffmpeg
```

Virtual/nested display options:

```bash
sudo pacman -S --needed \
  xorg-server-xvfb xorg-server-xephyr weston xpra
```

Input and performance tools:

```bash
sudo pacman -S --needed \
  libinput evtest evemu ydotool perf radeontop apitrace mangohud
```

Optional DRM/KMS tooling:

```bash
sudo pacman -S --needed igt-gpu-tools
```

Package names may vary slightly by repository state; check `pacman -Ss` before scripting them into a setup file.

---

## 9. Proposed Harness Design

### 9.1 Directory and CLI Shape

Add a dedicated render harness project instead of embedding more behavior into governance:

```text
tests/Rendering.Harness/
  Rendering.Harness.fsproj
  Program.fs
  EnvironmentProbe.fs
  OffscreenCapture.fs
  LiveX11Probe.fs
  InputScript.fs
  TimingRun.fs
  ReportWriter.fs
```

Suggested CLI:

```bash
dotnet run --project tests/Rendering.Harness -- probe
dotnet run --project tests/Rendering.Harness -- offscreen --scenario button --out artifacts/render/offscreen
dotnet run --project tests/Rendering.Harness -- live-x11 --scenario controls-gallery --frames 120 --out artifacts/render/live-x11
dotnet run --project tests/Rendering.Harness -- perf --mode paced-native --frames 600 --out artifacts/render/perf
dotnet run --project tests/Rendering.Harness -- input --script click-type-scroll --out artifacts/render/input
```

This can live outside the governance path. It should generate artifacts developers can inspect and compare locally.

### 9.2 Artifact Contract

Every run should write:

```text
run.json
environment.txt
stdout.txt
stderr.txt
metrics.csv
summary.md
```

Optional per mode:

```text
offscreen.png
live-window.png
desktop.png
frames/
input-log.jsonl
glxinfo.txt
xdpyinfo.txt
xrandr.txt
vulkaninfo-summary.txt
```

`run.json` should include:

```json
{
  "proofLevel": "render-target | live-window | presentation-timing | input-routing",
  "authoritativeFor": ["scene-rendering"],
  "notAuthoritativeFor": ["desktop-visibility", "vsync"],
  "display": ":1",
  "waylandDisplay": null,
  "renderer": "OpenGL",
  "presentMode": "DirectToSwapchain",
  "frameRateCap": 60,
  "window": {
    "title": "FS.Skia.UI Harness",
    "size": "800x600",
    "startupState": "Normal"
  },
  "timing": {
    "frames": 600,
    "medianFrameMs": 8.33,
    "p95FrameMs": 8.80,
    "p99FrameMs": 12.00,
    "medianPaintMs": 1.20,
    "medianComposeMs": 6.90
  }
}
```

The `authoritativeFor` / `notAuthoritativeFor` fields are not bureaucratic overhead; they prevent future evidence overclaiming.

### 9.3 Screenshot Comparison Policy

Use different comparisons per tier:

- T0/T1: strict pixel hash or bounded pixel diff, stable dimensions, fixed font/render mode.
- T2: non-blank, window contains expected visual sentinel, optional loose diff. Do not require pixel-perfect desktop captures.
- T3: timings and histograms first; screenshots only corroborate that the scenario was visible.

### 9.4 Performance Metrics

Record per-frame metrics:

```text
frameIndex
frameCause
startedAtMonotonicNs
endedAtMonotonicNs
frameIntervalMs
updateMs
viewMs
layoutMs
paintMs
composeMs
swapMs
dirtyRectCount
dirtyArea
repaintedNodeCount
pointerSamplesReceived
pointerMovesProcessed
```

Some fields already exist in `FrameMetrics`; others may need live-host instrumentation.

Summaries:

- min/median/p95/p99/max frame interval
- min/median/p95/p99 paint
- min/median/p95/p99 compose/swap
- missed-native-refresh count
- long-frame clusters
- input-to-first-visible-change latency if input script is active

### 9.5 Input Scripts

Use small declarative scripts:

```text
focus window
move 40 40
click primary
key "A"
key "BackSpace"
wheel -1
wait-frames 2
capture live-window
```

Backends:

- `x11-xtest`: `xdotool`, first implementation.
- `uinput`: `ydotool`/`evemu`, opt-in.
- `pure`: maps to `ControlsElmish.Perf.runScript`/`captureRespondsProof` for deterministic comparison.

This gives one scenario with multiple evidence strengths.

---

## 10. Practical Next Steps

### Step 1: Install X11 Inspection and Automation Tools

Install:

```bash
sudo pacman -S --needed xorg-xdpyinfo xorg-xrandr xorg-xinput xorg-xwd xdotool maim imagemagick
```

Then capture baseline:

```bash
xdpyinfo -ext XTEST -ext Present -ext RANDR > artifacts/render/probe/xdpyinfo.txt
xrandr --verbose > artifacts/render/probe/xrandr.txt
xinput list > artifacts/render/probe/xinput.txt
glxinfo -B > artifacts/render/probe/glxinfo-B.txt
```

### Step 2: Force FS.Skia.UI Runs Onto X11

Run viewer scenarios with:

```bash
env -u WAYLAND_DISPLAY \
  XDG_SESSION_TYPE=x11 \
  GDK_BACKEND=x11 \
  QT_QPA_PLATFORM=xcb \
  SDL_VIDEODRIVER=x11 \
  <viewer-command>
```

Use normal startup state for automation:

```text
ViewerWindowStartupState.Normal
ViewerWindowPosition.Coordinates(80, 80)
InitialSize = 800x600
```

Avoid windowed fullscreen for harness automation unless the test specifically targets it.

### Step 3: Build Live X11 Smoke

Minimum acceptance:

- Viewer launches.
- Window ID discovered by title.
- Window activated.
- First frame visible.
- `xwd`/`maim` captures non-blank window PNG.
- `xdotool` click/key changes app state.
- Second capture shows expected visible change.

### Step 4: Add Performance Mode

Minimum acceptance:

- Run 600 frames.
- Persist frame metrics.
- Persist environment facts.
- Show default swap pacing near refresh.
- Show raw throughput mode separately with `vblank_mode=0`.
- Refuse to label performance run as "vsync faithful" if `xrandr`/swap-control facts are missing.

### Step 5: Decide on Physical/Dummy/VKMS Display

Use the decision tree:

1. If a physical monitor is available: use it.
2. If no monitor but GPU has HDMI/DP: use a dummy plug.
3. If no physical connector is practical: configure host-level VKMS and run Xorg against it.
4. If neither is possible: use current host X display for development and mark performance evidence as "host-display dependent."
5. Use Xvfb only for smoke, not performance.

### Step 6: Optional `/dev/uinput` Tier

Only after XTEST is stable:

- Mount `/dev/uinput`.
- Install `ydotool`, `evemu`, `evtest`.
- Add an opt-in `input-backend=uinput` mode.
- Keep it off by default.

---

## 11. Risks and Guardrails

### Risk: One Harness Becomes Oppressive

The user explicitly called out oppressive test/governance bloat. The rendering harness should be:

- opt-in for heavy live/performance modes,
- fast by default,
- explicit about proof level,
- not wired as a mandatory full gate for every rendering edit.

Recommended default:

- T0/T1 for normal development.
- T2 before claiming live window behavior.
- T3 before claiming performance/pacing.

### Risk: Screenshots Overclaim

Render-target screenshots are easy to overclaim. The artifact schema must say:

```text
provesSceneRendering=true
provesDesktopVisibility=false
provesPresentationTiming=false
```

Only a live-window capture can set `provesDesktopVisibility=true`.

Only a timing run with display/swap facts can set `provesPresentationTiming=true`.

### Risk: X11 Host Is Accidentally Xwayland/Wayland-Dependent

Because `WAYLAND_DISPLAY` is currently present, every live harness command should print the effective backend facts. If the viewer uses Wayland despite `DISPLAY`, the run should fail or classify itself as Wayland, not silently proceed.

### Risk: Host Display Changes Break Baselines

Desktop captures and performance timings depend on host state. Persist environment facts every time and compare only like-with-like.

### Risk: `/dev/uinput` Security

Do not enable `/dev/uinput` in general-purpose containers. Use a dedicated dev harness profile.

---

## 12. Recommended Position

Build the harness in layers, starting with X11:

1. **Keep deterministic render proof as the normal inner loop.** It is the least oppressive and most reproducible path.
2. **Add a small, explicit X11 live-window harness.** This gives faithful screenshotting and practical mouse/keyboard automation in the user's current "Wayland off" reality.
3. **Treat performance as a separate mode requiring display facts.** The current container appears capable of vsync-paced GL on `DISPLAY=:1`, but that must be recorded and rechecked per run.
4. **Do not use Xvfb for vsync/performance claims.** It is valuable for basic window smoke only.
5. **Use physical/dummy-output Xorg for highest confidence.** If that is unavailable, use host-level VKMS/Xorg before falling back to Xvfb.
6. **Leave Wayland as a later compatibility tier.** Portals/libei are the right concepts, but current project productivity is better served by a stable X11 harness.

This approach keeps rendering development light while still giving a path to faithful evidence when a claim actually needs it.

---

## 13. Source Notes

Primary external sources:

- [Xvfb man page](https://linux.die.net/man/1/xvfb): Xvfb runs without display hardware/input and uses virtual memory framebuffer.
- [Linux VKMS documentation](https://docs.kernel.org/gpu/vkms.html): VKMS is a software KMS driver for testing and headless X-like use.
- [GLX_EXT_swap_control](https://registry.khronos.org/OpenGL/extensions/EXT/EXT_swap_control.txt): swap interval is measured in video frame periods; interval 0 is unsynchronized.
- [Mesa MESA_swap_control spec](https://docs.mesa3d.org/specs/MESA_swap_control.spec): Mesa swap-interval semantics and default.
- [DRM userspace API](https://dri.freedesktop.org/docs/drm/gpu/drm-uapi.html): vblank and page-flip completion events.
- [Vulkan VK_GOOGLE_display_timing](https://docs.vulkan.org/refpages/latest/refpages/source/VK_GOOGLE_display_timing.html), [VK_KHR_present_wait](https://docs.vulkan.org/refpages/latest/refpages/source/VK_KHR_present_wait.html), and [VK_EXT_present_timing](https://docs.vulkan.org/features/latest/features/proposals/VK_EXT_present_timing.html): modern present-timing capabilities and caveats.
- [XTEST protocol](https://xorg.freedesktop.org/archive/X11R7.7/doc/xextproto/xtest.html) and [xdotool](https://www.semicomplete.com/projects/xdotool/): X11 synthetic input.
- [Linux uinput documentation](https://docs.kernel.org/input/uinput.html): userspace virtual input devices.
- [xdg-desktop-portal overview](https://flatpak.github.io/xdg-desktop-portal/) and [screenshot portal](https://flatpak.github.io/xdg-desktop-portal/docs/doc-org.freedesktop.portal.Screenshot.html): portal-mediated Wayland/sandbox capture.
- [libei documentation](https://libinput.pages.freedesktop.org/libei/): Wayland-oriented emulated input protocol.
- [Weston running documentation](https://wayland.pages.freedesktop.org/weston/toc/running-weston.html) and [Weston man page](https://man.archlinux.org/man/weston.1.en): Wayland compositor backend options.
- [ImageMagick import](https://imagemagick.org/import/) and [maim man page](https://man.archlinux.org/man/maim.1.en): X11 screenshot capture tools.

Local repo references:

- `src/SkiaViewer/SkiaViewer.fsi`
- `src/Controls.Elmish/ControlsElmish.fsi`
- `docs/architecture/host-skiaviewer.md`
- `docs/architecture/testing-skillsupport.md`
- `tests/SkiaViewer.Tests/Feature121LiveHostPacingTests.fs`
- `tests/ControlsPreview.Harness/PreviewRender.fs`
- `tests/Elmish.Tests/Feature108MetricsTests.fs`
- `tests/Elmish.Tests/Feature090DispatchTests.fs`
