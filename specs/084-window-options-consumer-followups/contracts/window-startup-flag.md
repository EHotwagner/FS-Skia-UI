# Contract: generated-app `--window-startup` flag surface

The generated product's window-option flag surface
(`template/base/src/Product/WindowOptions.fs` + the launch wiring in
`Program.fs`).

## Flag values

```
--window-startup normal              → ViewerWindowStartupState.Normal              (honored)
--window-startup maximized           → ViewerWindowStartupState.Maximized           (honored)
--window-startup minimized           → ViewerWindowStartupState.Minimized           (unsupported)
--window-startup fullscreen          → ViewerWindowStartupState.Fullscreen          (honored)   [reclassified]
--window-startup windowed-fullscreen → ViewerWindowStartupState.WindowedFullscreen  (honored)   [NEW]
(no --window-startup flag)           → ViewerWindowStartupState.WindowedFullscreen  (honored)   [new default; was normal]
```

## Launch wiring (FR-004 / FR-005)

The durable literal MUST remain present and reachable:

```fsharp
// GovernanceTests.fs:105 asserts this literal stays in source text.
if windowFlagSupplied then
    Viewer.runAppWithWindowBehavior viewerOptions windowBehaviorRequest generatedHost
else
    Viewer.runApp viewerOptions generatedHost     // still reachable (no-flag path)
```

- The no-flag `runApp` branch still yields windowed fullscreen because the framework
  `defaultWindowBehavior` is now `WindowedFullscreen` (so the default is honored
  without the flag).
- Any explicit flag routes through `runAppWithWindowBehavior` so the **live window**
  honors the request (not only the diagnostic report).

## Conflict resolution

Multiple/conflicting `--window-startup` selections resolve to the **explicit, last-
specified** value (deterministic, documented).

## Honest-environment behavior

In a headless / unsupported environment the windowed-fullscreen default does not
fabricate a visible window; the render-only evidence path reports the environment
honestly (no false visible-window claim).
