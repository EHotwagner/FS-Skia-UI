---
title: Silk.NET Window Platform Failure Analysis
---

# Silk.NET Window Platform Failure Analysis

Date: 2026-05-26 22:27 Europe/Vienna

## Summary

The persistent GUI runtime evidence run is currently blocked by a native window
creation failure, not by missing GPU passthrough. The container can see the GPU
and can create Vulkan/OpenGL device-level diagnostics, but `Viewer.runApp`
fails before a persistent window is created:

```text
status=unsupported mode=interactive-window command=dotnet-fsi-supported-host-runApp blocked-stage=Window classification=UnsupportedEnvironment category=Startup message=Silk.NET_persistent_viewer_launch_failed:_Couldn't_find_a_suitable_window_platform._(GlfwPlatform_-_not_applicable)_https://dotnet.github.io/Silk.NET/docs/hlu/troubleshooting.html
```

This matters because the evidence contract for `018-persistent-gui-runtime`
requires a supported-host persistent interactive launch artifact. Bounded smoke,
first-frame evidence, screenshot evidence, pixel-readback evidence, and
unsupported-host diagnostics are useful, but they cannot be treated as proof
that a generated graphical app remains open for interactive play.

## Observed Environment

The container presents desktop-session signals:

```text
DISPLAY=:1
WAYLAND_DISPLAY=wayland-0
XDG_RUNTIME_DIR=/run/user/1000
DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus
```

The expected sockets are present:

```text
/run/user/1000/wayland-0
/tmp/.X11-unix/X1
```

GPU/device evidence is positive:

```text
vulkaninfo --summary
  deviceName = AMD Radeon Graphics (RADV RENOIR)
  driverName = radv

glxinfo -B
  direct rendering: Yes
  OpenGL renderer string: AMD Radeon Graphics (radeonsi, renoir, ACO, ...)
```

Silk.NET package evidence is also not obviously missing:

- `Silk.NET.Windowing` is restored.
- `Silk.NET.Windowing.Glfw` is restored.
- `Ultz.Native.GLFW` provides `runtimes/linux-x64/native/libglfw.so.3`.
- `ldd` on that bundled `libglfw.so.3` did not report missing native library dependencies.

## Current Failure Boundary

The failing operation is the Silk.NET window platform selection path. The code
reaches `Window.Create` / `window.Initialize`, and Silk reports:

```text
Couldn't find a suitable window platform. (GlfwPlatform - not applicable)
```

This is distinct from:

- Vulkan physical-device discovery failure
- Skia native asset loading failure
- missing package restore failure
- generated product package-version drift
- application update/render logic failure

The available evidence points at a platform/windowing bridge issue: Silk/GLFW
does not consider the current X11/Wayland environment applicable for creating a
native window, despite GPU and display variables being visible.

## Why GPU Passthrough Is Not Sufficient

GPU passthrough proves that rendering devices can be discovered and used by
tools such as Vulkan or GLX. A persistent GUI app needs additional layers:

1. A usable display server connection.
2. Window-system protocol compatibility: X11, Wayland, or a supported fallback.
3. Authentication/permissions for the display socket.
4. Native backend support in the chosen windowing library.
5. Runtime asset loading for managed and native Silk/GLFW assemblies.
6. The app's event loop successfully opening and holding a window.

This container satisfies at least some lower layers: GPU, Vulkan, GLX, visible
display variables, and visible sockets. The failure occurs at layer 4 or 5 from
the perspective of Silk.NET windowing.

## Evidence Audit Impact

`EvidenceAudit` still fails for final readiness because:

- synthetic evidence remains declared and propagated from `[S]` tasks;
- no `status=ok` supported-host persistent launch artifact exists;
- bounded evidence artifacts correctly remain rejected as substitutes for
  interactive lifecycle evidence;
- the native launch attempt produced `status=unsupported`;
- generated verification remains non-authoritative for the full generated
  product matrix.

The package-resolution blocker was narrowed: generated app-source restore/test
can pass against the local `0.1.17-preview.1` package set. That does not clear
the native persistent-window evidence requirement.

## Likely Causes

### 1. GLFW rejects the proxied display setup

Silk.NET 2.23 uses the GLFW windowing backend here. The message names
`GlfwPlatform - not applicable`, so GLFW platform discovery is the first
suspect.

Possible reasons:

- Wayland socket is mounted, but GLFW was built/packaged without Wayland support
  or chooses X11 differently than expected.
- X11 socket is present, but X authority is not usable by the process.
- `DISPLAY=:1` is reachable for `glxinfo`, but not usable by the bundled GLFW
  backend in the same runtime context.
- Required runtime environment such as `XAUTHORITY` is absent or points at a
  host path not visible in the container.

### 2. `dotnet fsi` is not representative of a normal executable

The recorded persistent launch attempt used an FSI script to load the built
assemblies. FSI is useful for public API evidence, but native desktop windowing
can differ from a normal executable because:

- runtime assets may resolve differently;
- native probing paths can differ;
- transitive package runtime assets are not copied beside an application in the
  same way;
- the process entry context is not the same as `dotnet run` or a published app.

This does not make the failure meaningless, but it means the next confirmation
should use a compiled executable.

### 3. Window backend support is too narrow

The runtime currently relies on Silk.NET/GLFW as the persistent window host.
If GLFW is not robust in this container class, the project may need either:

- better diagnostics explaining why GLFW rejected the environment;
- an alternate Silk.NET window backend if available and viable;
- a separate supported-host evidence lane outside this container;
- a headless/evidence renderer that stays clearly separated from interactive
  readiness.

### 4. Generated product matrix has a separate profile issue

`GeneratedProductCheck` also remains non-authoritative because the
`headless-scene/source` profile is compiling app-oriented `Program.fs` code
without the app package set. That is separate from the Silk window failure, but
it prevents the final generated verification record from becoming authoritative.

## Recommended Further Analysis

### A. Reproduce with a compiled minimal executable

Create a tiny console project that references `FS.Skia.UI.SkiaViewer`,
`FS.Skia.UI.Scene`, and the exact Silk package set, then calls `Viewer.runApp`
with a host that closes on the first tick after the first frame. Run it with:

```bash
dotnet run --project /tmp/fs-skia-window-probe/WindowProbe.fsproj
```

Record:

- stdout outcome fields;
- copied native assets under `bin/Debug/net10.0`;
- `ldd` output for copied `libglfw.so.3`;
- environment variables visible to the process;
- whether the failure changes from `GlfwPlatform - not applicable`.

If the compiled executable succeeds, the fix is to stop using FSI for native
persistent launch readiness and use an executable probe for T050 evidence.

If it fails the same way, the issue is container/window-platform compatibility.

### B. Run direct GLFW/Silk platform probes

Add a small diagnostic target or temporary probe that only does:

```fsharp
let windowOptions = WindowOptions.DefaultVulkan
let window = Window.Create windowOptions
window.Initialize()
```

Capture exception type, inner exceptions, loaded assemblies, and native probing
paths. This should be separate from Skia rendering so we know whether failure
happens before any renderer setup.

### C. Check X authority and Wayland/X11 selection

Collect:

```bash
echo "$XAUTHORITY"
xauth list "$DISPLAY"
xdpyinfo -display "$DISPLAY"
WAYLAND_DEBUG=1 dotnet run --project <probe>
```

Also test forced protocol choices if GLFW/Silk honors them in this environment:

```bash
unset WAYLAND_DISPLAY
dotnet run --project <probe>

unset DISPLAY
dotnet run --project <probe>
```

This will show whether GLFW succeeds on one protocol and fails on the other.

### D. Compare host vs container

Run the same compiled probe:

- directly on the host;
- in the current container;
- in the container with only X11 mounted;
- in the container with only Wayland mounted.

If host succeeds and container fails, document the container requirements:

- display socket mounts;
- `XAUTHORITY` propagation;
- user id alignment;
- `XDG_RUNTIME_DIR` ownership;
- required native packages;
- any `--device` or seccomp flags needed by the runtime.

### E. Improve runtime diagnostics

The current failure is classified correctly as `UnsupportedEnvironment`, but
the message is still generic. Improve `SkiaViewer` diagnostics to include:

- selected Silk windowing backend;
- display variables observed;
- whether X11 and Wayland sockets were visible;
- whether `XAUTHORITY` was present;
- whether `libglfw.so.3` was loadable from the app output;
- exception type and inner exception details;
- suggested command to run the compiled window probe.

This would make future audit evidence more actionable without weakening the
readiness rules.

### F. Fix generated product profile separation

The `headless-scene` generated profile should not compile app-only runtime code
that references `SkiaViewer`, `KeyboardInput`, `Controls`, or
`Controls.Elmish` unless those packages are selected. Possible fixes:

- split `Program.fs` into profile-specific template sections;
- add a separate `HeadlessProgram.fs` fragment;
- widen the headless profile package set only if that is an intentional product
  contract change;
- update generated matrix tests to enforce that headless source compiles with
  only `FS.Skia.UI.Scene`.

This fix is required for authoritative generated verification, independently
of the Silk.NET container window issue.

## Possible Fix Paths

### Path 1: Use a compiled persistent-window probe for readiness

This is the narrowest change if FSI is the only failing context. Replace the
current T050 native launch FSI exercise with a compiled app or generated product
`dotnet run` evidence command. Keep FSI for pure API and MVU transition tests.

Expected outcome:

- supported-host evidence can be recorded if the executable opens a window;
- no runtime API change required;
- audit remains strict.

### Path 2: Treat this container as unsupported and require host evidence

If compiled probes fail in the same way, the honest readiness state is:

- container has GPU passthrough;
- container cannot currently create a Silk.NET/GLFW window;
- native persistent launch evidence must be collected on a supported desktop
  host or a fixed container image.

Expected outcome:

- `T050` remains `[F]` until external evidence is supplied;
- readiness files should keep the unsupported-host diagnostics;
- no fake/synthetic launch should be upgraded to `[X]`.

### Path 3: Update container/windowing setup

If diagnosis points at X11/Wayland auth or missing native runtime setup, fix the
container invocation/image. Candidate changes:

- propagate `XAUTHORITY`;
- ensure the container user id matches the display socket owner;
- mount `/tmp/.X11-unix` and `/run/user/1000/wayland-0`;
- pass `XDG_RUNTIME_DIR` consistently;
- install any distro packages needed by the chosen GLFW backend;
- verify with the compiled probe before rerunning `EvidenceAudit`.

Expected outcome:

- Silk.NET can create a GLFW window;
- `supported-host-persistent-launch.txt` can become `status=ok`;
- persistent launch audit blockers can clear.

### Path 4: Add an alternate interactive backend

If GLFW remains unsuitable for the project's supported Linux container profile,
evaluate an alternate supported windowing path. This is larger and should be
planned explicitly because it changes runtime support scope.

Expected outcome:

- better support for the target container class;
- more implementation and test work;
- possible public contract or dependency impact.

## Recommended Next Step

Start with the compiled minimal executable probe. It gives the highest signal
with the least code churn:

1. If it passes, switch readiness evidence to the executable probe and rerun
   `EvidenceAudit`.
2. If it fails with the same `GlfwPlatform - not applicable` message, focus on
   container display/auth/backend compatibility.
3. In parallel, fix the generated `headless-scene` profile separation so
   generated verification can become authoritative once the native window
   evidence is available.
