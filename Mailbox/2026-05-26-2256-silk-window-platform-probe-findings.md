# Silk.NET Window Platform Probe Findings

Date: 2026-05-26 22:56 Europe/Vienna

Branch: `018-persistent-gui-runtime`

## Summary

I exhaustively re-tested the problems described in
`docs/2026-05-26-2227-silk-window-platform-analysis.md`.

The main result is that the native persistent-window failure is not a general
container, GPU, display, Vulkan, or `Viewer.runApp` failure in this environment.
It is specifically an FSI native probing failure.

A compiled executable probe can create a Silk.NET window and can run
`Viewer.runApp` successfully on this host. Plain `dotnet fsi` reproduces the
documented `GlfwPlatform - not applicable` error. The same FSI probe succeeds
when `LD_LIBRARY_PATH` points at the directory containing `libglfw.so.3`.

The generated-product blocker remains real and independent: the
`headless-scene/source` generated product still compiles app/viewer/control code
while its profile only selects the `FS.Skia.UI.Scene` package set. That keeps
generated verification non-authoritative and continues to block final audit
readiness.

## Environment Tested

The workspace used for the probe was a fresh clone of this branch:

```text
repository: https://github.com/EHotwagner/FS-Skia-UI.git
branch: 018-persistent-gui-runtime
head: b103e20d6b9dc70551b7228d381e1b627ae4631d
```

The feature-branch checkout already existed at:

```text
/home/developer/projects/FS-Skia-UI
```

and is also on:

```text
018-persistent-gui-runtime
```

The test host exposes desktop-session signals:

```text
DISPLAY=:1
WAYLAND_DISPLAY=wayland-0
XDG_RUNTIME_DIR=/run/user/1000
DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus
XAUTHORITY=/tmp/.Xauthority
```

Relevant sockets exist:

```text
/tmp/.X11-unix/X1
/run/user/1000/wayland-0
```

Socket ownership and mode:

```text
/tmp/.X11-unix/X1 developer developer 755 socket
/run/user/1000 developer developer 700 directory
/run/user/1000/wayland-0 developer developer 755 socket
```

GPU diagnostics are positive:

```text
vulkaninfo --summary
  deviceName = AMD Radeon Graphics (RADV RENOIR)
  driverName = radv

glxinfo -B
  direct rendering: Yes
  OpenGL renderer string: AMD Radeon Graphics (radeonsi, renoir, ACO, ...)
```

Two requested diagnostics could not be collected because the tools are not
installed in the container:

```text
xauth: command not found
xdpyinfo: command not found
```

## Temporary Probe

I added a temporary probe in the scratch checkout under:

```text
artifacts/window-platform-probe/
```

Probe files:

```text
WindowPlatformProbe.fsproj
Program.fs
fsi-direct-silk.fsx
fsi-direct-silk-glfw.fsx
fsi-runapp.fsx
```

The compiled probe has two modes:

- `direct-silk`: creates `WindowOptions.DefaultVulkan`, calls
  `Window.Create`, then `window.Initialize()`.
- `viewer-runApp`: references the local project outputs and calls
  `Viewer.runApp` with a host that renders one rectangle and requests close on
  the first tick.

The probe prints process, environment, socket, assembly, runtime asset, and
launch outcome fields.

## Probe Build Result

Command:

```bash
dotnet build artifacts/window-platform-probe/WindowPlatformProbe.fsproj
```

Result:

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

The compiled output includes the expected managed assemblies and native assets:

```text
Silk.NET.Windowing.Glfw.dll
Silk.NET.Input.Glfw.dll
Silk.NET.GLFW.dll
runtimes/linux-x64/native/libglfw.so.3
runtimes/linux-x64/native/libSkiaSharp.so
```

`ldd` on the copied `libglfw.so.3` showed no missing linked dependencies:

```text
libm.so.6 => /usr/lib/libm.so.6
libc.so.6 => /usr/lib/libc.so.6
```

The bundled GLFW binary contains both X11 and Wayland support strings:

```text
3.4.0 Wayland X11 GLX Null EGL OSMesa monotonic shared
VK_KHR_xlib_surface
VK_KHR_xcb_surface
VK_KHR_wayland_surface
```

## Compiled Executable Results

### Combined X11 And Wayland

Command:

```bash
timeout 20s dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
```

Result:

```text
silk.window.created=true
silk.window.initialized=True
status=ok
```

Command:

```bash
timeout 20s dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
```

Result:

```text
status=ok
outcome.mode=interactive-window
outcome.windowOpened=True
outcome.firstFramePresented=True
outcome.userCloseObserved=True
outcome.selfClosedForEvidence=False
outcome.inputDispatch=false
outcome.exitPath=True
```

The process emitted:

```text
Failed to load plugin: 'libgtk-3.so.0: cannot open shared object file: No such file or directory'
```

but that did not prevent window creation or `Viewer.runApp` completion.

### X11-Only

Command shape:

```bash
env -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
env -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
```

Result:

```text
direct-silk: status=ok
viewer-runApp: status=ok, windowOpened=True, firstFramePresented=True
```

### Wayland-Only

Command shape:

```bash
env -u DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
env -u DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
```

Result:

```text
direct-silk: status=ok
viewer-runApp: status=ok, windowOpened=True, firstFramePresented=True
```

### No DISPLAY And No WAYLAND_DISPLAY

Command:

```bash
env -u DISPLAY -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
```

Result:

```text
silk.window.created=true
silk.window.initialized=True
status=ok
```

Command:

```bash
env -u DISPLAY -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
```

Result:

```text
status=failed
failure.blockedStage=Window
failure.classification=UnsupportedEnvironment
failure.category=EnvironmentSession
failure.message=DISPLAY or WAYLAND_DISPLAY is missing; interactive Linux launch is blocked before app lifecycle debugging.
```

Raw Silk can still initialize in that variant, but `Viewer.runApp` intentionally
prechecks Linux desktop-session variables and rejects the launch before app
lifecycle debugging.

## FSI Results

Plain FSI reproduces the documented failure.

Command:

```bash
timeout 30s dotnet fsi artifacts/window-platform-probe/fsi-direct-silk.fsx
```

Result:

```text
status=failed
exceptionType=System.PlatformNotSupportedException
message=Couldn't_find_a_suitable_window_platform._(GlfwPlatform_-_not_applicable)_https://dotnet.github.io/Silk.NET/docs/hlu/troubleshooting.html
```

Command:

```bash
timeout 30s dotnet fsi artifacts/window-platform-probe/fsi-runapp.fsx
```

Result:

```text
status=unsupported
blocked-stage=Window
classification=UnsupportedEnvironment
category=Startup
message=Silk.NET_persistent_viewer_launch_failed:_Couldn't_find_a_suitable_window_platform._(GlfwPlatform_-_not_applicable)_https://dotnet.github.io/Silk.NET/docs/hlu/troubleshooting.html
```

Explicitly referencing `Silk.NET.Windowing.Glfw` and `Silk.NET.Input.Glfw`, and
forcing assembly loads for these assemblies:

```text
Silk.NET.GLFW
Silk.NET.Windowing.Glfw
Silk.NET.Input.Glfw
```

still failed when no native library search path was supplied.

With `LD_LIBRARY_PATH` set to the NuGet GLFW native asset directory, the same
FSI direct Silk probe passed:

```bash
env LD_LIBRARY_PATH=$HOME/.nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native \
  dotnet fsi artifacts/window-platform-probe/fsi-direct-silk-glfw.fsx
```

Result:

```text
forcedAssembly=Silk.NET.GLFW version=2.23.0.0
forcedAssembly=Silk.NET.Windowing.Glfw version=2.23.0.0
forcedAssembly=Silk.NET.Input.Glfw version=2.23.0.0
silk.window.created=true
silk.window.initialized=true
status=ok
```

With `LD_LIBRARY_PATH` set to the compiled probe output native asset directory,
the FSI `Viewer.runApp` probe also passed:

```bash
env LD_LIBRARY_PATH=/home/developer/projects/gputest/artifacts/window-platform-probe/bin/Debug/net10.0/runtimes/linux-x64/native \
  dotnet fsi artifacts/window-platform-probe/fsi-runapp.fsx
```

Result:

```text
status=ok
mode=interactive-window
window-opened=true
first-frame-presented=true
self-closed-for-evidence=false
user-close-observed=true
input-dispatch=false
exit-path=true
renderer-mode=skia
message=Persistent_generated_app_host_launch_completed_after_intentional_close.
```

## Revised Failure Boundary

The original document correctly separates GPU/device readiness from the
window-platform failure. The new probe narrows the boundary further:

- This container can create a Silk.NET/GLFW window from a compiled executable.
- `Viewer.runApp` can create a persistent interactive window from a compiled
  executable.
- The failure is not GPU passthrough.
- The failure is not Vulkan physical-device discovery.
- The failure is not missing copied `libglfw.so.3` in compiled app output.
- The failure is not a `Viewer.runApp` lifecycle defect in the compiled path.
- The failure is plain FSI native runtime probing unless `LD_LIBRARY_PATH` is
  set to the GLFW native asset directory.

The existing readiness artifact:

```text
status=unsupported mode=interactive-window command=dotnet-fsi-supported-host-runApp ...
message=Silk.NET_persistent_viewer_launch_failed:_Couldn't_find_a_suitable_window_platform._(GlfwPlatform_-_not_applicable)
```

is valid evidence that the current FSI evidence lane is unsupported. It should
not be interpreted as proof that compiled generated apps cannot open a native
window on this host.

## Generated Product Matrix Result

Command:

```bash
./fake.sh build -t GeneratedProductCheck
```

Result:

```text
headless-scene/source generated Test failed with exit code 1
```

The current failure matches the documented blocker. The generated
`headless-scene/source` product compiles app-oriented `Program.fs` code without
the app package set.

Representative missing namespaces/types:

```text
FS.Skia.UI.Controls
FS.Skia.UI.Controls.Elmish
FS.Skia.UI.KeyboardInput
FS.Skia.UI.SkiaViewer
ViewerKey
ViewerKeyEvent
ViewerRunRequest
ViewerRunEvidence
ViewerRunFailure
Viewer
ChartSeries
DataGridColumn
DataGridRow
RichTextBlock
ControlsElmish
```

Conclusion:

- `headless-scene` is expected to compile with `FS.Skia.UI.Scene` only.
- The generated `Program.fs` currently depends on viewer, keyboard, controls,
  controls-elmish, chart, DataGrid, and rich-text APIs.
- Generated verification remains non-authoritative until profile-specific
  source separation is fixed or the profile contract is intentionally widened.

## Evidence Audit Result

Command:

```bash
./fake.sh build -t EvidenceAudit
```

Result:

```text
verdict=FAIL
```

Current blockers include:

```text
supported-host-persistent-launch.txt (missing supported-host persistent launch evidence)
bounded-only substitution
unsupported-host-only persistent launch evidence
generated-verify.md (generated verification is non-authoritative)
```

The audit behavior is correct. It rejects:

- bounded evidence as a substitute for interactive lifecycle evidence;
- unsupported-host-only FSI evidence as supported-host evidence;
- non-authoritative generated verification.

Given the compiled executable result, the right response is to supply compiled
supported-host launch evidence, not to weaken the audit.

## Focused Test Results

SkiaViewer tests:

```bash
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore -m:1
```

Result:

```text
Passed: 29, Failed: 0
```

Testing helper tests:

```bash
dotnet test tests/Testing.Tests/Testing.Tests.fsproj --no-restore -m:1
```

Result:

```text
Passed: 13, Failed: 0
```

Filtered governance tests:

```bash
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore -m:1 --filter "persistent|generated|evidence|headless"
```

Result:

```text
Failed: 4, Passed: 70
```

The failures were governance/template guidance issues, not Silk window-platform
failures:

- two Controls boundary composition tests report that `template/base` copies a
  framework `readiness` asset;
- generated task guidance is missing the expected `implementation batch
  records` wording;
- `.agents/skills/speckit-implement/SKILL.md` is missing the expected
  `graph before/after` wording.

## Broad Repo Health Findings

These are outside the direct Silk/window-platform problem but appeared during
exhaustive validation.

After:

```bash
dotnet restore FS-Skia-UI.sln
```

this command failed:

```bash
dotnet build FS-Skia-UI.sln --no-restore
```

The failure is in `samples/DemoReel/Program.fs`, with 67 compile errors against
older viewer/runtime APIs such as:

```text
ViewerEvent
ViewerEffect
InitializeRenderer
RenderFrame
RenderTick
UpdateTick
Resized
Shutdown
CaptureScreenshot
Viewer.defaultConfiguration
Viewer.run
Paint.withMaskFilter
```

This is separate from the `headless-scene` generated-product issue, but it
means the full solution is not currently green.

This command timed out:

```bash
timeout 150s dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore -m:1
```

It reached:

```text
Test run for .../tests/Smoke.Tests/bin/Debug/net10.0/Smoke.Tests.dll
A total of 1 test files matched the specified pattern.
```

and produced no further output before the shell timeout. Treat this as a
smoke-test hang until isolated.

## Conclusions

### 1. The Current Host Supports Compiled Persistent Window Launches

The compiled executable probe opened and initialized a native Silk.NET window,
rendered through `Viewer.runApp`, observed a close path, and returned
`status=ok` with `mode=interactive-window`.

### 2. Plain FSI Is Not Authoritative Native Launch Evidence

Plain FSI fails because the GLFW native asset directory is not in the native
library search path. FSI passes when `LD_LIBRARY_PATH` points at
`libglfw.so.3`.

Use FSI for pure API, MVU, and semantic contract evidence. Do not use plain FSI
as final native persistent-window readiness evidence.

### 3. Audit Strictness Is Correct

The audit should continue rejecting bounded-only evidence and unsupported-host
FSI evidence. The fix is to supply real compiled supported-host evidence.

### 4. Generated Verification Still Needs Profile Separation

`headless-scene/source` must either receive profile-specific source that only
uses `FS.Skia.UI.Scene`, or the profile contract must intentionally widen its
package set. The conservative fix is profile-specific headless source.

## Recommended Fix Path

1. Replace the T050 supported-host persistent launch readiness lane with a
   compiled executable probe or generated product `dotnet run` evidence path.

2. Record a compiled supported-host artifact with fields like:

   ```text
   status=ok
   mode=interactive-window
   command=compiled-supported-host-runApp
   window-opened=true
   first-frame-presented=true
   self-closed-for-evidence=false
   exit-path=true
   renderer-mode=skia
   ```

3. Keep the FSI failure as diagnostic evidence unless invoking FSI with an
   explicit native probing path:

   ```bash
   LD_LIBRARY_PATH=$HOME/.nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native
   ```

4. Fix `headless-scene` source generation:

   - split `Program.fs` into profile-specific template sections; or
   - add a separate `HeadlessProgram.fs`; or
   - explicitly widen the `headless-scene` profile package contract and update
     docs/tests accordingly.

5. Update generated matrix tests so `headless-scene/source` must compile and
   test with only the scene package set unless the profile contract changes.

6. Address broad repo health separately:

   - update or quarantine `samples/DemoReel` so the full solution builds;
   - isolate the `Smoke.Tests` hang and ensure it times out with useful
     diagnostics.

## Commands Run

```bash
dotnet build artifacts/window-platform-probe/WindowPlatformProbe.fsproj
timeout 20s dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
timeout 20s dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
timeout 20s env -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
timeout 20s env -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
timeout 20s env -u DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
timeout 20s env -u DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
timeout 20s env -u DISPLAY -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- direct-silk
timeout 20s env -u DISPLAY -u WAYLAND_DISPLAY dotnet run --project artifacts/window-platform-probe/WindowPlatformProbe.fsproj -- viewer-runApp
timeout 30s dotnet fsi artifacts/window-platform-probe/fsi-direct-silk.fsx
timeout 30s dotnet fsi artifacts/window-platform-probe/fsi-runapp.fsx
timeout 30s dotnet fsi artifacts/window-platform-probe/fsi-direct-silk-glfw.fsx
timeout 30s env LD_LIBRARY_PATH=$HOME/.nuget/packages/ultz.native.glfw/3.4.0/runtimes/linux-x64/native dotnet fsi artifacts/window-platform-probe/fsi-direct-silk-glfw.fsx
timeout 30s env LD_LIBRARY_PATH=/home/developer/projects/gputest/artifacts/window-platform-probe/bin/Debug/net10.0/runtimes/linux-x64/native dotnet fsi artifacts/window-platform-probe/fsi-runapp.fsx
vulkaninfo --summary
glxinfo -B
ldd artifacts/window-platform-probe/bin/Debug/net10.0/runtimes/linux-x64/native/libglfw.so.3
./fake.sh build -t GeneratedProductCheck
./fake.sh build -t EvidenceAudit
dotnet restore FS-Skia-UI.sln
dotnet build FS-Skia-UI.sln --no-restore
dotnet test tests/SkiaViewer.Tests/SkiaViewer.Tests.fsproj --no-restore -m:1
dotnet test tests/Testing.Tests/Testing.Tests.fsproj --no-restore -m:1
dotnet test tests/Governance.Tests/Governance.Tests.fsproj --no-restore -m:1 --filter "persistent|generated|evidence|headless"
timeout 150s dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj --no-restore -m:1
```
