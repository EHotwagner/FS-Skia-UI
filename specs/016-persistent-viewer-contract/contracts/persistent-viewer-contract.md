# Contract: Persistent Viewer

## Public API Shape

```fsharp
module Viewer =
    val run :
        options: ViewerOptions ->
        scene: SceneNode ->
            Result<ViewerLaunchOutcome, ViewerRunFailure>

    val runtimeCapability :
        unit -> ViewerRuntimeCapability
```

`Viewer.run` is the product launch contract for scene-only graphical apps. It must open a persistent desktop window on supported hosts, render the provided scene, keep the event loop active until user exit, and return a structured outcome or failure.

## Required Behavior

- Validate `ViewerOptions` before native work begins.
- Attempt a persistent desktop window on supported Windows and Linux hosts.
- Render the provided `SceneNode`.
- Remain active until the user exits, the window closes, or the host reports a failure.
- Return `UnsupportedEnvironment` when the host lacks required display/window support.
- Return `ProductDefect` when the package or provided app contract is invalid.
- Preserve bounded APIs as separate evidence helpers.

## Non-Goals

- New platform support.
- Browser/mobile hosts.
- Replacing deterministic scene evidence.
- Treating first-frame evidence as persistent launch success.

## Evidence

Successful supported-host evidence must include:

```text
status=ok
mode=persistent-window
command=<default command>
window-opened=true
exit-path=true
```

Unsupported-host evidence must include:

```text
status=unsupported
mode=persistent-window
blocked-stage=<stage>
classification=UnsupportedEnvironment
message=<actionable reason>
```
