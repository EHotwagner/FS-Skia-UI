# Contract: Viewer Diagnostics

## Public Surface

Extend diagnostics with level, category, frame sampling, and capturable sink
support equivalent to:

```fsharp
type ViewerDiagnosticLevel =
    | Error
    | Warning
    | Info
    | Debug
    | Trace

type ViewerDiagnosticCategory =
    | Startup
    | Input
    | Frame
    | Renderer
    | Vulkan
    | Skia
    | Swapchain
    | Scene
    | Screenshot

type ViewerDiagnosticsOptions =
    { MinimumLevel: ViewerDiagnosticLevel
      Categories: Set<ViewerDiagnosticCategory>
      FrameLogLimit: int option
      Sink: (ViewerDiagnosticEvent -> unit) option
      Verbose: bool }
```

Exact naming may follow existing repository conventions. Existing verbose
configuration should remain as a compatibility shortcut if public.

## Required Behavior

- Startup diagnostics can be enabled without repeated per-frame logs.
- Frame-loop diagnostics appear only when the frame category or sampling
  enables them.
- Tests and hosts can capture diagnostic events in memory.
- Diagnostic events identify category, level, stage when relevant, and message.

## Evidence

- Tests for startup-only output excluding frame spam.
- Tests for frame category/sampling inclusion.
- Tests that assert captured diagnostics without reading process stderr.
- Readiness: `readiness/diagnostics.md`.
