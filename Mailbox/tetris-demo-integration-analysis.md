# Tetris Demo Integration Analysis

Date: 2026-05-18

Source context:
- Consumer project: `/home/developer/projects/TetrisDemoV2`
- FS.Skia.UI checkout: `/home/developer/projects/FS-Skia-UI`
- FS.Skia.UI branch/commit observed: `master` at `91cf991` (`Bump FS.Skia.UI packages to 0.1.12-preview.1`)
- Local package feed used by the consumer: `~/.local/share/nuget-local`

This note records the issues encountered while building and running a generated Tetris demo against the real FS.Skia.UI packages, then proposes improvements that would make future generated graphical apps easier to build, validate, and operate.

## Executive Summary

The FS.Skia.UI stack successfully supported a real graphical Skia/Vulkan render path for the Tetris demo. The viewer created a Silk.NET window, initialized Vulkan, created a swapchain and Skia GPU context, and rendered model-derived scene frames.

The most expensive integration work was not core rendering. It was stitching together the app lifecycle around the viewer: keyboard normalization, screen-specific key behavior, automated graphical smoke evidence, verbose diagnostics volume, and local package/feed alignment. These are good candidates for framework-level improvements because every generated interactive app is likely to need similar behavior.

The highest-impact improvements are:

1. Provide a normalized viewer keyboard API or helper that maps Silk.NET key names into stable application key identifiers.
2. Add bounded viewer execution helpers such as `runUntilFirstFrame`, `runForFrames`, or `runFor`.
3. Add diagnostic levels/categories so verbose startup logging can be enabled without per-frame log flooding.
4. Provide a headless/offscreen render or screenshot path for deterministic CI evidence.
5. Strengthen generated app templates around start/options/end keyboard flows, not only gameplay controls.

## What Worked Well

### Real Graphical Rendering

The graphical path was viable once the local packages were packed and referenced. The Tetris demo was able to run:

```text
dotnet run --project src/TetrisDemoV2/TetrisDemoV2.fsproj -- --graphical-smoke
```

The viewer entered the Silk.NET event loop and repeatedly rendered a Skia scene through the Vulkan swapchain. The log included frame evidence such as:

```text
rendering Skia scene for swapchain image index=...
drawing model-derived scene into Skia Vulkan surface
```

That means the integration was not limited to a fake console or text-mode path; it exercised the real viewer stack.

### Pure App Model Was Easy To Test

Once the Tetris game rules were kept separate from viewer effects, Expecto coverage could validate:

- start/options/game/end screen transitions
- piece movement and collision behavior
- hard drop behavior
- line clears and level progression
- option propagation into new games
- viewer keyboard events mapped into app messages

This is aligned with FS.Skia.UI's existing Elmish-style program model.

### Package Boundary Was Clear Enough

The generated consumer could reference packages such as:

- `FS.Skia.UI`
- `FS.Skia.UI.Scene`
- `FS.Skia.UI.SkiaViewer`
- `FS.Skia.UI.Elmish`
- `FS.Skia.UI.KeyboardInput`
- `FS.Skia.UI.Controls`
- `FS.Skia.UI.Controls.Elmish`

The package structure was understandable after local packing, and the real packages worked from the local NuGet feed.

## Problems Encountered

### 1. Graphical Keyboard Input Was Easy To Miswire

The first running graphical demo rendered correctly, but the user could not start or control the game. The pure gameplay handler accepted a `KeyPressed` message, and tests covered that path, but the actual viewer path only sent raw `ViewerEvent.KeyDown` strings. The app did not initially translate `Enter` or `Space` on the start screen into `StartGame`.

This created a false positive:

- Unit tests passed for pure gameplay input.
- The graphical window rendered.
- The actual interactive user flow was still broken.

The final Tetris app had to add screen-aware viewer key mapping:

- start screen: `Enter` or `Space` starts the game, `O` opens options
- options screen: arrows adjust options, `Space` toggles sound, `Enter` starts, `Esc`/`Backspace` returns
- playing screen: arrows move/rotate/drop one row, `Space` hard-drops
- game-over screen: `Enter` or `Space` restarts

The underlying FS.Skia.UI viewer currently exposes keys as strings from Silk.NET's `Key.ToString()` in `src/Lib/Library.fs`:

```text
dispatchViewerEvent program dispatch (KeyDown(key.ToString()))
dispatchViewerEvent program dispatch (KeyUp(key.ToString()))
```

The keyboard input subsystem also has normalization logic in `src/Lib/KeyboardInput.fs`, for example mapping viewer key strings into physical key ids. That logic is useful, but app authors still have to discover and compose it correctly.

Impact:
- Users can see a rendered window that appears frozen.
- Tests can pass while the live interaction path is not covered.
- Generated apps duplicate key normalization and screen-specific mapping.

Recommendation:
- Add a first-class normalized viewer key event model, or expose a simple helper in the viewer namespace that can map `ViewerEvent` to normalized key values without requiring apps to reach into lower-level keyboard runtime concepts.

### 2. Viewer Smoke Testing Requires External Timeout Control

The real graphical viewer is an interactive event loop. That is correct for desktop app behavior, but awkward for generated app verification. A graphical smoke test must prove that the window initializes and renders at least one frame, then return control to CI or a readiness script.

The Tetris demo used a command like:

```text
timeout 3s dotnet run --project src/TetrisDemoV2/TetrisDemoV2.fsproj -- --graphical-smoke
```

That works as evidence, but it treats timeout as success after seeing render-loop logs. This is brittle because the success condition is outside the program.

Impact:
- Smoke scripts have to infer success from timeout plus logs.
- Failures can look similar to expected bounded termination.
- Test evidence is harder to automate cleanly.

Recommendation:
- Add viewer execution helpers:
  - `Viewer.runUntilFirstFrame : ViewerProgram<'model,'msg> -> Result<FrameEvidence, RenderDiagnostic>`
  - `Viewer.runForFrames : frameCount:int -> ViewerProgram<'model,'msg> -> Result<FrameEvidence, RenderDiagnostic>`
  - `Viewer.runFor : duration:TimeSpan -> ViewerProgram<'model,'msg> -> Result<FrameEvidence, RenderDiagnostic>`
- Include frame count, elapsed time, surface size, backend, and last diagnostic summary in the returned evidence.

### 3. Verbose Diagnostics Are Too Noisy For App-Level Debugging

`ViewerConfiguration.Diagnostics.Verbose` is a useful switch, but it currently enables high-volume messages during the frame loop. The Tetris run produced repeated per-frame logs:

```text
querying swapchain images
rendering Skia scene for swapchain image index=...
skia maxSampleCount...
skia context abandoned=False...
drawing model-derived scene into Skia Vulkan surface
```

These messages are valuable when debugging swapchain or Skia details, but they drown out app-level lifecycle events during normal integration.

Impact:
- Logs become hard to scan.
- A short run can produce a large output file.
- The useful startup milestones are mixed with repetitive frame messages.

Recommendation:
- Replace `Verbose: bool` with a structured diagnostic configuration:
  - `MinimumLevel`
  - `Categories`, for example `Startup`, `Input`, `Frame`, `Vulkan`, `Skia`, `Swapchain`, `Screenshot`
  - optional frame-log sampling, for example first N frames or every Nth frame
- Keep current `Verbose` as a compatibility shortcut if needed, but route it through categories internally.

### 4. No Obvious Headless Or Offscreen CI Rendering Path

The Tetris demo could produce deterministic scene readback evidence through scene-level rendering, and it could run the real graphical viewer in this environment. However, the real viewer depends on a usable window/display/Vulkan setup. During the run, the host also emitted an `XDG_RUNTIME_DIR` warning.

For generated apps, a reliable CI path should validate visual output without assuming a desktop session. The viewer is excellent for interactive use, but app teams still need deterministic evidence when native windows are unavailable.

Impact:
- CI may need host-specific display setup.
- Generated projects may have to document unsupported renderer conditions even when the app code is correct.
- Screenshot evidence can become environment-dependent.

Recommendation:
- Add an official offscreen renderer path that can render a `SceneNode` to image bytes or hash evidence without opening a window.
- Add screenshot/readback helpers that share enough code with the real renderer to remain meaningful.
- Consider a `Viewer.tryRunHeadlessEvidence` fallback that returns an explicit diagnostic when GPU/window support is unavailable.

### 5. Generated Template Guidance Does Not Yet Prevent Screen-Flow Input Gaps

The Tetris app initially treated keyboard input as gameplay-only. That matched the narrow user requirement of arrows plus space, but a complete app also needs start/options/end screen activation keys. Generated examples and templates should make that lifecycle explicit.

Impact:
- App developers can build a visually correct first screen with no keyboard activation path.
- Tests focus on domain messages instead of actual viewer events.
- Accessibility and keyboard operation expectations are not carried through every screen.

Recommendation:
- Generated graphical apps should include a default input contract:
  - start screen activation
  - options navigation
  - escape/back behavior
  - game-over restart
  - focus-loss behavior where relevant
- Generated tests should include at least one `ViewerEvent.KeyDown` flow that starts the app from the initial screen and reaches the main experience.

### 6. Local Package Feed Setup Was Manual And Easy To Drift

The consumer app needed the real FS.Skia.UI packages from a local NuGet feed. This required packing FS.Skia.UI and then aligning package versions in the consumer's central package management file.

Impact:
- Generated projects can silently reference stale package builds.
- Local feed state is outside the repository, so reproducing integration can be harder.
- Build errors may look like app errors when they are actually package/feed drift.

Recommendation:
- Add a documented single command for consumer integration, for example:

```text
./fake.sh build -t PackLocalAndPrintConsumerProps
```

- Emit:
  - local feed path
  - package ids and versions
  - XML snippet for `Directory.Packages.props`
  - restore command for generated consumers
- Optionally provide a generated `nuget.config` template that includes the local feed.

### 7. App Authors Need To Understand Too Much Viewer Edge Plumbing

The Tetris demo had to manually combine:

- pure model/update
- host effects
- viewer event mapping
- render command mapping
- app smoke path
- graphical smoke path
- screen-specific keyboard behavior

The primitives are reasonable, but for generated apps this is a lot of repeated setup.

Impact:
- The first working graphical app takes more effort than expected.
- Different generated apps will likely implement inconsistent wrappers.
- Bugs occur at lifecycle boundaries rather than in domain logic.

Recommendation:
- Add a small application-host builder for common patterns:
  - `init`
  - `update`
  - `view`
  - `scene`
  - key map
  - tick interval
  - first-frame smoke mode
- Keep it optional so advanced users can still use the low-level viewer APIs.

## Specific Improvement Proposals

### Proposal A: Normalized Viewer Keyboard API

Add a public normalized key type:

```fsharp
type ViewerKey =
    | ArrowLeft
    | ArrowRight
    | ArrowUp
    | ArrowDown
    | Enter
    | Space
    | Escape
    | Backspace
    | Letter of char
    | Digit of int
    | Function of int
    | Unknown of raw: string
```

Add helpers:

```fsharp
module ViewerKeyboard =
    val normalize : raw: string -> ViewerKey
    val tryNormalizeEvent : event: ViewerEvent -> (ViewerKey * isDown: bool) option
```

Benefits:
- Apps do not depend on Silk.NET string names.
- Tests can use stable key values.
- KeyboardInput can reuse the same normalization.

### Proposal B: Bounded Graphical Runner

Add:

```fsharp
type ViewerRunEvidence =
    { FramesRendered: int
      ElapsedMs: int64
      InitialSize: Size
      Backend: string
      LastSceneDescription: SceneElementKind list
      Diagnostics: RenderDiagnostic list }

module Viewer =
    val runUntilFirstFrame : ViewerProgram<'model,'msg> -> Result<ViewerRunEvidence, RenderDiagnostic>
    val runForFrames : frameCount: int -> ViewerProgram<'model,'msg> -> Result<ViewerRunEvidence, RenderDiagnostic>
```

Benefits:
- CI can prove the real renderer path without shell `timeout`.
- Generated projects can include a clean `--graphical-smoke` mode that exits successfully.
- Evidence can become structured instead of log-scanned.

### Proposal C: Diagnostic Categories

Replace or extend:

```fsharp
type ViewerDiagnosticsOptions =
    { Verbose: bool }
```

with:

```fsharp
type DiagnosticCategory =
    | Startup
    | Input
    | Frame
    | Vulkan
    | Skia
    | Swapchain
    | Screenshot

type ViewerDiagnosticsOptions =
    { Verbose: bool
      Categories: Set<DiagnosticCategory>
      FrameLogLimit: int option
      Sink: (string -> unit) option }
```

Benefits:
- Startup debugging can be enabled without frame spam.
- Tests can capture diagnostics in memory.
- Applications can route logs through their own logging system.

### Proposal D: Headless Scene Evidence

Add an official test helper:

```fsharp
module SceneEvidence =
    val renderHash : size: Size -> scene: SceneNode -> Result<string, RenderDiagnostic>
    val renderPng : size: Size -> scene: SceneNode -> Result<byte[], RenderDiagnostic>
```

Benefits:
- Generated apps can collect visual evidence consistently.
- CI can validate visual output without native windows.
- Apps can still separately validate real viewer startup when available.

### Proposal E: Generated App Input Contract Tests

Template-generated apps that select viewer and keyboard capabilities should include tests for:

- start screen activation through `ViewerEvent.KeyDown`
- options navigation through `ViewerEvent.KeyDown`
- primary interaction through `ViewerEvent.KeyDown`
- end/restart flow through `ViewerEvent.KeyDown`

Benefits:
- Prevents the exact Tetris failure where pure input tests passed but live input did nothing.
- Encourages user-reachable smoke paths rather than only domain-message tests.

## Suggested Priority

### High Priority

1. Normalized viewer keyboard API.
2. Bounded graphical runner for first-frame/frame-count smoke tests.
3. Generated template tests for viewer-key-driven start and restart flows.

These directly address user-visible failures and CI evidence friction.

### Medium Priority

4. Diagnostic categories and log throttling.
5. Local package feed consumer guidance command.
6. Optional application-host builder for common generated app patterns.

These reduce integration cost and improve developer ergonomics.

### Longer Term

7. Headless/offscreen rendering parity with real viewer output.

This is highly valuable, but it may be more involved depending on how much Skia/Vulkan path sharing is desired.

## Concrete Acceptance Criteria For Improvements

For normalized keyboard:
- A consumer app can map `ViewerEvent.KeyDown` to `ViewerKey` without string matching.
- Both `"Left"` and `"ArrowLeft"` normalize to the same value where appropriate.
- Tests cover arrows, enter, space, escape, letters, and unknown keys.

For bounded runner:
- `runUntilFirstFrame` exits with success after at least one rendered frame.
- Failure before first frame returns a `RenderDiagnostic`.
- The returned evidence includes frame count and elapsed time.

For diagnostics:
- Startup-only verbose logs do not include per-frame swapchain messages.
- Frame logs can be enabled explicitly.
- A test can capture diagnostics without reading process stderr.

For generated templates:
- A generated graphical app includes a test that starts from the initial screen using a viewer key event.
- The template quickstart documents both interactive run and bounded smoke run.

## Closing Notes

The Tetris integration demonstrates that FS.Skia.UI already has the core pieces needed for a generated graphical app: scene primitives, controls, Elmish-style update flow, keyboard input runtime, and a real Skia/Vulkan viewer. The improvements above are mostly about making the common path harder to miswire and easier to validate.

The biggest lesson is that graphical app readiness should test the same path the user exercises. A pure `KeyPressed Space` test is not enough if the real viewer sends `ViewerEvent.KeyDown "Space"` and the current screen ignores it. Framework helpers and template defaults can close that gap for every generated app.
