# AsteroidsDemo1 Windowing And Screenshot Report

Timestamp: 2026-05-27T00:56:19+02:00

## Summary

While testing the generated `AsteroidsDemo1` app from `/home/developer/projects/AsteroidsDemo1`, the normal graphical launch did not produce a usable game window. The app process stayed alive and KDE showed a taskbar entry, but the user could not access a visible, resizable, or maximizable window surface.

This appears to be a `FS.Skia.UI.SkiaViewer` native windowing issue rather than an Asteroids scene/gameplay issue. The Asteroids scene and evidence-mode paths build and pass tests, but the persistent viewer host has behavior that makes interactive testing unreliable.

## Environment Observed

- Desktop/compositor: KDE KWin 6.6.5 on Wayland.
- Display variables:
  - `WAYLAND_DISPLAY=wayland-0`
  - `DISPLAY=:1`
  - `XDG_RUNTIME_DIR=/run/user/1000`
  - `DBUS_SESSION_BUS_ADDRESS=unix:path=/run/user/1000/bus`
- KWin screen geometry: one screen, `1920x1080`, operation mode Wayland.
- The app printed this native warning during launch:

```text
Failed to load plugin: 'libgtk-3.so.0: cannot open shared object file: No such file or directory'
```

Despite that warning, the viewer reported success internally.

## Reproduction

From `/home/developer/projects/AsteroidsDemo1`:

```bash
dotnet run --project src/AsteroidsDemo1/AsteroidsDemo1.fsproj
```

Observed result:

- Process launched.
- KWin/taskbar showed an app entry.
- No usable game window appeared.
- Process exited quickly unless persistent mode was forced.

Output included:

```text
status=ok mode=interactive-window
window-opened=true
first-frame-presented=true
user-close-observed=true
self-closed-for-evidence=false
input-dispatch=false
renderer-mode=skia
message=Persistent generated app host launch completed after intentional close.
```

Then launched with input-dispatch verification:

```bash
FS_SKIA_REQUIRE_INPUT_DISPATCH=1 dotnet run --project src/AsteroidsDemo1/AsteroidsDemo1.fsproj
```

Observed result:

- Process stayed alive.
- A taskbar entry appeared.
- User still reported only a taskbar entry, with no visible resizable/maximizable game window.

## Important Source Findings

### 1. `runApp` closes after first frame by default

In `src/SkiaViewer/SkiaViewer.fs`, `runPersistentWindow` closes the window after the first rendered frame when `inputVerified()` returns true:

- `runPersistentWindow` starts at `SkiaViewer.fs:457`.
- First-frame render path is at `SkiaViewer.fs:497-513`.
- The close happens at `SkiaViewer.fs:511-513`:

```fsharp
if inputVerified () then
    closedIntentionally := true
    window.Close()
```

For generated app hosts, `inputVerified()` is defined at `SkiaViewer.fs:1132-1133`:

```fsharp
let inputVerified () =
    not (requireInputDispatchVerification ()) || inputDispatch = "true"
```

Because `FS_SKIA_REQUIRE_INPUT_DISPATCH` is normally unset, `inputVerified()` returns true immediately. That makes `Viewer.runApp` close on the first frame by default, which is not persistent interactive behavior.

Impact:

- Normal launch reports a successful persistent interactive launch.
- In practice it behaves like first-frame evidence unless an environment variable is set.
- The reported message says "intentional close", which hides the fact that this is not useful for interactive testing.

### 2. Key/tick handlers close the window when app update returns `CloseWindow`

In `runPersistentWindow`, key and tick handlers also close the native window when app update returns true:

- Tick close path: `SkiaViewer.fs:515-519`.
- Key-down close path: `SkiaViewer.fs:567-571`.
- Key-up close path: `SkiaViewer.fs:578-582`.

That part is expected for explicit close requests, but it compounds the current ambiguity because `closedIntentionally` is used both for real user close and framework-driven close.

### 3. `ViewerOptions` cannot request resize/maximize behavior

The public SkiaViewer contract exposes only title and initial size:

`src/SkiaViewer/SkiaViewer.fsi:7-9`

```fsharp
type ViewerOptions =
    { Title: string
      InitialSize: Size }
```

The app cannot set:

- resizable/window border policy,
- maximizable policy,
- initial window state,
- startup position,
- backend preference,
- visibility/focus behavior.

The implementation sets only:

- title,
- size,
- visible,
- Vulkan API,
- FPS/update rates.

Relevant lines: `SkiaViewer.fs:475-481`.

Impact:

- App-level workaround is limited to changing initial size.
- The Asteroids app was changed to start at `1024x768`, but the user still saw only a taskbar entry.
- A real fix likely needs a SkiaViewer API/implementation change.

### 4. App "screenshot evidence" is not an actual PNG screenshot

The user asked whether the app itself should have a screenshot function. The app does expose `--screenshot-evidence`, but `SceneEvidenceFormat.Png` currently writes a deterministic hash, not image bytes.

In `src/Scene/Scene.fs`:

- `SceneEvidenceFormat` includes `Png` at `Scene.fs:441-444`.
- `SceneEvidence.render` maps `Png` to `readback.DeterministicHash` at `Scene.fs:499-503`.
- `writeEvidence` uses `File.WriteAllText` at `Scene.fs:475-481`.
- `renderPng` returns UTF-8 bytes of the hash, not PNG bytes, at `Scene.fs:522-530`.

Impact:

- `--screenshot-evidence` is useful as deterministic scene metadata/hash evidence.
- It cannot diagnose native window visibility, taskbar-only state, compositor placement, or actual rendered pixels.
- The command name is misleading for visual inspection.

## Screenshot Attempt

There was no CLI screenshot tool installed in the environment:

- `gnome-screenshot`: not found
- `grim`: not found
- `spectacle`: not found
- `import`: not found
- `maim`: not found
- `scrot`: not found
- `xwd`: not found
- `xdotool`: not found
- `wmctrl`: not found

KWin exposes `org.kde.KWin.ScreenShot2`, but direct capture through `CaptureWorkspace` failed with Wayland authorization:

```text
GDBus.Error:org.kde.KWin.ScreenShot2.Error.NoAuthorized:
The process is not authorized to take a screenshot
```

The xdg-desktop-portal screenshot API returned a request object but did not produce a usable screenshot artifact during this automated session, likely because it requires an interactive approval flow.

## KWin Window Query

While the Asteroids process was running, `ps` showed:

```text
dotnet run --project src/AsteroidsDemo1/AsteroidsDemo1.fsproj
/home/developer/projects/AsteroidsDemo1/src/AsteroidsDemo1/bin/Debug/net10.0/AsteroidsDemo1
```

KWin `queryWindowInfo` did not identify an active `AsteroidsDemo1` surface when queried. It returned unrelated active windows such as `Plex` or the terminal. This supports the user's report that the app had only a taskbar entry and no accessible visible game window.

## Recommended Fixes

### A. Fix `Viewer.runApp` persistence semantics

`Viewer.runApp` should not auto-close after the first frame in normal interactive mode.

Recommended change:

- Remove the first-frame `inputVerified()` close from normal `runApp`.
- Keep bounded first-frame behavior only in `runBounded`, `runUntilFirstFrame`, or explicit evidence paths.
- If input-dispatch verification is needed, report it as diagnostic/evidence without closing a normal interactive app.

Current problematic path:

- `SkiaViewer.fs:497-513`
- `SkiaViewer.fs:1132-1135`

Expected behavior:

- `dotnet run --project ...` opens a persistent window and stays open until user close or explicit app `CloseWindow`.
- No environment variable should be required for basic interactive use.

### B. Split user close from framework/evidence close

`ViewerLaunchOutcome.UserCloseObserved` currently becomes true for framework-driven intentional closes. This makes output misleading.

Recommended change:

- Track separate flags:
  - `UserCloseObserved`
  - `AppCloseRequested`
  - `EvidenceCloseRequested`
  - `FrameworkAutoCloseRequested`
- Use these flags in logs and evidence reports.

This would have made the observed issue obvious: "first-frame framework close" instead of "user-close-observed=true".

### C. Add window behavior options

Extend `ViewerOptions` with fields such as:

```fsharp
Resizable: bool
Maximizable: bool
InitialWindowState: Normal | Maximized | Fullscreen
InitialPosition: Point option
Backend: Auto | Vulkan | OpenGL | Software
```

Then map those fields into Silk.NET `WindowOptions` in `runPersistentWindow`.

This is needed because generated apps cannot currently influence resize/maximize behavior. The Asteroids app can only change `InitialSize`.

### D. Add actual PNG screenshot output

Either:

1. Rename current `SceneEvidenceFormat.Png` behavior to `Hash`/`Metadata`, or
2. Implement real raster PNG encoding for `Png`.

Current behavior:

- `Png` returns and writes deterministic hash text.

Expected behavior:

- `Png` writes actual PNG bytes to `EvidencePath`.
- `renderPng` returns actual PNG bytes.

This would let generated apps produce inspectable visual artifacts without relying on compositor screenshot permissions.

### E. Add native window diagnostics for mapped/visible/focused state

`runPersistentWindow` should report more than `windowOpened` and `firstFramePresented`. Useful fields:

- `IsInitialized`
- `IsVisible`
- `IsClosing`
- actual native size after creation,
- actual framebuffer/output size,
- minimized/maximized state if available,
- backend/API selected,
- whether a surface/swapchain was created,
- whether input devices were discovered,
- number of input keyboards.

This would distinguish:

- window object created but not mapped,
- mapped but minimized,
- mapped but off-screen,
- first frame submitted but not visible,
- compositor denied/ignored surface.

## Current App-Level Workaround Tried

The Asteroids app changed its `viewerOptions.InitialSize` from `640x480` to `1024x768`.

Result:

- Tests still passed.
- User still reported only a taskbar entry.

This confirms the issue is probably not the app's scene size.

## Validation Performed

In `/home/developer/projects/AsteroidsDemo1`:

```bash
./fake.sh build -t Test
```

Result:

```text
Passed! - Failed: 0, Passed: 11, Skipped: 0, Total: 11
```

Asteroids app evidence commands also generated deterministic hash/report artifacts, but those are not actual desktop screenshots.

## Bottom Line

The generated Asteroids gameplay implementation is not the current blocker. The blocker is SkiaViewer's interactive window behavior:

1. Normal `runApp` closes on first frame by default.
2. Forcing persistence leaves a taskbar entry without an accessible game window in this KDE Wayland session.
3. The public viewer options do not expose resize/maximize/window-state controls.
4. The screenshot evidence path is hash metadata, not a real PNG.

This should be handled in `FS.Skia.UI.SkiaViewer` and `FS.Skia.UI.Scene` before generated game apps can provide reliable interactive/manual testing on this environment.
