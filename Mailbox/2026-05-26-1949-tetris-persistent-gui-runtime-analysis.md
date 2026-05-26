# Tetris Persistent GUI Runtime Analysis

Date: 2026-05-26 19:49 Europe/Vienna

## Summary

While trying to run the generated `tetrisdemo1` app as an interactive graphical
Tetris game, two separate problems were exposed:

1. The container runtime did not provide a valid Linux desktop runtime session
   (`XDG_RUNTIME_DIR` was unset, despite `DISPLAY=:1` being present).
2. The restored `FS.Skia.UI.SkiaViewer` package treated `Viewer.runApp` as a
   launch-evidence helper: it opened a window, rendered one frame, then closed
   intentionally with `exit-path=true`.

The container launch script changes described by the user are the right fix for
the first problem. They are not sufficient for the second problem. A true
interactive game requires `Viewer.runApp` to keep the window open until the user
closes it, not to close after satisfying evidence conditions.

## Observed Behavior

Command:

```bash
dotnet run --project src/tetrisdemo1/tetrisdemo1.fsproj
```

Initial output included:

```text
error: XDG_RUNTIME_DIR is invalid or not set in the environment.
status=ok mode=persistent-window ... window-opened=true input-dispatch=false exit-path=true renderer-mode=skia
```

After creating a private runtime directory and relaunching:

```bash
mkdir -p /tmp/runtime-$(id -u)
chmod 700 /tmp/runtime-$(id -u)
XDG_RUNTIME_DIR=/tmp/runtime-$(id -u) dotnet run --project src/tetrisdemo1/tetrisdemo1.fsproj
```

the runtime warning disappeared, but the app still exited immediately:

```text
status=ok mode=persistent-window ... window-opened=true input-dispatch=false exit-path=true renderer-mode=skia
```

This proves the runtime-dir issue and the viewer lifecycle issue are distinct.

## Root Cause 1: Container Desktop Session Plumbing

Inside the container:

```text
XDG_RUNTIME_DIR=
DISPLAY=:1
WAYLAND_DISPLAY=
/run/user/1000: missing
```

Many Linux GUI stacks require a valid `XDG_RUNTIME_DIR` that points to a
user-owned `0700` directory, normally `/run/user/$UID`. The image cannot create
the host's real runtime directory at build time. It must be mounted or provided
when the container starts.

The proposed container script changes should fix this class of failure when the
container is launched from a real desktop session:

- Mount host `$XDG_RUNTIME_DIR` into `/run/user/$(id -u)` inside the container.
- Export `XDG_RUNTIME_DIR=/run/user/$(id -u)`.
- Pass `WAYLAND_DISPLAY` and `DBUS_SESSION_BUS_ADDRESS` when available.
- Preserve X11 support with `DISPLAY`, `/tmp/.X11-unix`, and auth handling.
- Provide a private `/tmp/runtime-$UID` fallback only when no host runtime is
  available.

That fallback is useful for avoiding basic runtime-dir errors, but it is not
equivalent to a real desktop session. Long-lived GUI integration still depends
on the host runtime socket, display socket, session bus, and permissions.

## Root Cause 2: `Viewer.runApp` Closes After Evidence Conditions

The package resolved by the generated app was initially not the requested
version. The project asks for `0.1.16-persistent.1`, but restore fell back to
`0.1.16-preview.1` until a local package source was added:

```text
warning NU1603: ... 0.1.16-persistent.1 was not found.
... 0.1.16-preview.1 was resolved instead.
```

After adding a local package source for `/tmp/fs-skia-t024-nuget`, restore did
resolve `FS.Skia.UI.SkiaViewer/0.1.16-persistent.1`. However, that package still
closed immediately.

Inspection of `src/SkiaViewer/SkiaViewer.fs` shows why. `runPersistentWindow`
uses a bounded loop:

```fsharp
while (not window.IsClosing && (not !framePresented || not (inputVerified ())) && stopwatch.Elapsed < timeout) do
    window.DoEvents()
    window.DoUpdate()
    window.DoRender()
    Thread.Sleep(1)

if !framePresented && inputVerified () then
    window.Close()
```

For the generated app, `inputVerified()` is true unless
`FS_SKIA_REQUIRE_INPUT_DISPATCH=1`. Therefore the first presented frame satisfies
the loop condition and the viewer closes itself. The outcome is internally
consistent as evidence:

```text
window-opened=true
exit-path=true
```

but it is not interactive persistence.

## Proposed Fixes

### 1. Split Interactive Launch From Evidence Launch

`Viewer.runApp` should be the interactive API and should keep the window open
until the user closes it or the host emits `CloseWindow`.

Evidence-oriented launch should be a separate API or explicit option, for
example:

```fsharp
type PersistentRunMode =
    | Interactive
    | Evidence of timeout: TimeSpan * requireInputDispatch: bool
```

or:

```fsharp
val runApp : ViewerOptions -> GeneratedAppHost<'model,'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>
val runAppEvidence : ViewerRunRequest -> ViewerOptions -> GeneratedAppHost<'model,'msg> -> Result<ViewerLaunchOutcome, ViewerRunFailure>
```

Default generated app `main` should call the interactive path.

Bounded smoke, first-frame evidence, and input-dispatch evidence should remain
behind explicit CLI flags such as `--bounded-smoke`, `--scene-evidence`, or a new
`--persistent-evidence`.

### 2. Stop Reporting First-Frame Evidence As "Persistent"

`mode=persistent-window` currently means "a persistent-capable window path was
used", not "the app remained available for user interaction." That distinction
was confusing during diagnosis.

Recommended outcome fields:

```text
mode=interactive-window | persistent-evidence | bounded-smoke
window-opened=true|false
first-frame-presented=true|false
user-close-observed=true|false
self-closed-for-evidence=true|false
input-dispatch=true|false|not-required
```

This makes it clear whether a command produced an interactive app or only
verified a launch path.

### 3. Add a Keep-Open Regression Test

Add a test or smoke path that proves `runApp` does not self-close after the
first frame in interactive mode. This likely needs a test seam around the native
window loop, since CI may not have a real desktop.

The important assertion is behavioral:

- first frame presented
- no `CloseWindow` emitted
- no evidence timeout reached
- loop remains active until simulated user close

### 4. Make Package Resolution Fail Fast

The generated app silently degraded from `0.1.16-persistent.1` to
`0.1.16-preview.1` with `NU1603` warnings. That made diagnosis harder because
the requested "persistent" package was not actually being used.

Recommended process/package fixes:

- Treat `NU1603` as an error in generated app verification.
- Add `NuGet.Config` during generation when local framework packages are
  expected.
- Add a restore verification task that checks `project.assets.json` contains
  the exact FS.Skia.UI package versions requested by `Directory.Packages.props`.
- Record package source and resolved package version in readiness evidence.

### 5. Container Launch Script Acceptance Test

Add a small diagnostic command to the container launch script or readiness docs:

```bash
test -n "$XDG_RUNTIME_DIR"
test -d "$XDG_RUNTIME_DIR"
test "$(stat -c %a "$XDG_RUNTIME_DIR")" = "700"
test -n "$DISPLAY$WAYLAND_DISPLAY"
```

For Wayland, also verify the socket exists:

```bash
test -S "$XDG_RUNTIME_DIR/$WAYLAND_DISPLAY"
```

For X11, verify:

```bash
test -S /tmp/.X11-unix/X${DISPLAY#:}
```

This catches the "DISPLAY is set but runtime/session is incomplete" case before
debugging starts in the app.

## Experimental Patch Tried Locally

During diagnosis, I patched `src/SkiaViewer/SkiaViewer.fs` locally to add:

```text
FS_SKIA_KEEP_PERSISTENT_WINDOW_OPEN=1
```

When set, `runPersistentWindow` enters an unbounded loop until `window.IsClosing`
instead of closing after first-frame evidence. This was only an experiment to
validate the lifecycle hypothesis. It should not be accepted as-is without API
design, tests, and documentation because an environment variable makes the
default contract ambiguous.

Preferred final fix: make interactive persistence the default `runApp` behavior
and move evidence behavior to an explicit API or command.

Current worktree note: at the time this analysis was written, that experimental
change is still present in `/home/developer/projects/FS-Skia-UI/src/SkiaViewer/SkiaViewer.fs`.
It was used only to validate the hypothesis that the current lifecycle closes
after evidence conditions. Treat it as diagnostic evidence, not as the proposed
final implementation.

## Other Process Problems Found During Tetris Implementation

### Generated Build Targets Were Too Shallow

`./fake.sh build -t Test` wrote "Test completed" but did not run the real
Expecto tests. Real confidence came from:

```bash
dotnet test tests/tetrisdemo1.Tests/tetrisdemo1.Tests.fsproj
```

Process improvement:

- Generated `Test` should run the generated test project.
- Generated `Verify` should include real `dotnet test`, package resolution
  checks, evidence graph, evidence audit, and any required scene/viewer evidence.
- If a generated target is intentionally a placeholder, readiness should label
  it non-authoritative.

### Tetris Tasks Were Too Numerous For Honest Per-Task Graph Refresh

The implement skill requires rerunning the evidence graph after every status
change. With 47 tasks, this is workable mechanically but not useful when many
tasks are delivered by one cohesive vertical implementation.

Process improvement:

- Support task batching with a recorded batch boundary:
  - tasks in batch
  - shared evidence
  - graph run before and after the batch
- Keep per-task skill-loading evidence but allow one graph refresh for a
  validated implementation batch.

### "Failing-First" Was Not Operationally Captured

Tasks requested failing-first tests, but the workflow did not provide a clean
place to preserve the red state for every individual task. The final evidence
shows green semantic tests, not the red-to-green transition for each test task.

Process improvement:

- Add an explicit `readiness/red-green-log.md` format.
- Allow grouped red/green evidence for related task clusters.
- Require the log to name the command, failing assertion, implementation commit
  or change, and final passing command.

### Synthetic Evidence Rules Were Clear, But Tooling Was Easy To Trip

T011 was design-approved `[SEH]`, but the audit expected exact metadata such as
`accepted-seh`. This is good rigor, but the failure mode was discovered late.

Process improvement:

- Add a pre-implementation check that validates `[SEH]` inventory rows before
  coding starts.
- Generate accepted row boilerplate with the exact strings the audit expects.
- Add a small command for "why is my accepted SEH not accepted?" diagnostics.

### Readiness Contract Requirements Were Implicit

The audit required:

- `governance-risk-levels.md`
- `aggregate-hang-diagnostics.md`
- `runtime-limitations.md`

These were not obvious from the task list until audit time.

Process improvement:

- Make readiness contract files explicit tasks during `speckit-tasks`.
- Include required keywords or a template body so agents do not discover the
  expected shape by failing audit.

### Scene Evidence Was Textual, Not Visual

The generated app produced deterministic text/metadata scene evidence. That is
useful for CI but weak for a game where the user's real concern is visual
interactivity and readability.

Process improvement:

- Add a screenshot-capable or pixel-readback scene evidence path for generated
  games.
- Separate "scene contract metadata" from "visual proof".
- Require at least one real viewer screenshot or explicit unsupported-host
  diagnostic for game features.

### The Generated App Render Surface Was Too Minimal

The Tetris implementation could model game state and evidence well, but the
runtime `view` is still a single text scene summarizing state. That is not an
adequate game presentation.

Process improvement:

- Provide generated examples for board/grid rendering in Scene.
- Provide simple retained-mode primitives for game loops:
  - fixed board layout
  - colored cell grid
  - side panel text
  - keyboard-driven model update
  - animation timeline rendering
- Add a generated game template profile distinct from form/control examples.

### Local Package Source Setup Was Not Reproducible

The Tetris project expected persistent packages, but the source was not
registered. A package cache existed under `/tmp/fs-skia-t024-nuget`, but the app
did not know about it until a `NuGet.Config` was added manually.

Process improvement:

- Generated projects should either:
  - include a local feed configuration when created from a local framework
    checkout, or
  - pin only published package versions available from configured sources.
- `quickstart.md` should include a concrete restore/source validation command.

## Recommended Priority Order

1. Fix `Viewer.runApp` interactive lifecycle semantics.
2. Keep evidence launch behavior, but move it behind explicit evidence APIs or
   flags.
3. Add exact package resolution checks and make `NU1603` fatal.
4. Land the container runtime-dir/display/session script changes.
5. Strengthen generated `Test`/`Verify` so they run real tests and package
   checks.
6. Add game-oriented visual evidence and scene rendering templates.
