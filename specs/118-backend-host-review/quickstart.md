# Quickstart: Opt into the direct present path (Feature 118)

Constitution Principle I: exercise the new public surface through FSI — the same way a
consumer would — before relying on it.

## 1. Default mode is unchanged (FR-001 / byte-identity)

Every existing `ViewerOptions` construction gains the new field at its default; behavior is
byte-identical to before:

```fsharp
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer

let options =
    { Title = "My App"
      InitialSize = { Width = 1280; Height = 720 }
      PresentMode = ViewerPresentMode.OffscreenReadback }   // default; today's readback path
```

`OffscreenReadback` ⇒ the live present path, screenshots, window diagnostics, and visual
output are exactly the pre-feature baseline.

## 2. Opt into the readback-free direct path (FR-002)

Flip one field:

```fsharp
let fastOptions = { options with PresentMode = ViewerPresentMode.DirectToSwapchain }
// runInteractiveViewer fastOptions host
// runApp fastOptions host
```

Ordinary live frames now render straight onto the swapchain image — no GPU→CPU readback,
no per-frame staging buffer/command pool, no `vkQueueWaitIdle` stall. If the backend can't
create the direct render target, the viewer falls back to the readback path and emits a
`Warning` diagnostic — it never crashes or presents a corrupt frame (FR-005).

## 3. Observe which path ran (FR-007)

Attach a diagnostics sink; the live backend reports the active mode and whether ordinary
frames read back. The fact arrives over the existing `ViewerDiagnosticEvent` channel with
`Category = Swapchain` (or `Frame`) — it is **live-only** and never appears in
`Perf.runScript` goldens:

```fsharp
let diagnostics =
    { Viewer.defaultDiagnostics with
        Sink = Some (fun ev ->
            match ev.Category with
            | ViewerDiagnosticCategory.Swapchain
            | ViewerDiagnosticCategory.Frame -> printfn "%A: %s" ev.Level ev.Message
            | _ -> ()) }

// host = { ... ; Diagnostics = diagnostics }
```

Direct-mode run ⇒ the sink reports the direct mode and zero per-frame readback;
default-mode run ⇒ it reports readback per frame.

## 4. Screenshots/evidence still work under both modes (FR-004)

Explicit capture continues to use the offscreen render-plus-readback routine **on demand**
(only when a capture is requested), decoupled from per-frame present. Opting into direct
present does **not** disable visual evidence:

```fsharp
// runForFrames / runBounded / captureScreenshotEvidence behave identically under both modes;
// they render their own offscreen surface and read it back only for the capture.
```

## 5. Headless metrics are unchanged (FR-008)

`Perf.runScript` has no window/backend, so present mode is irrelevant there: `FrameMetrics`
gains **no** field and metric goldens do not change (SC-007). Backend timing is a
human/diagnostic signal only — never a pass/fail gate (FR-011).

## Surface check

```
dotnet fsi  // then #r the packed FS.Skia.UI.SkiaViewer and open FS.Skia.UI.SkiaViewer
// confirm: ViewerPresentMode.OffscreenReadback / .DirectToSwapchain resolve,
//          { Title=...; InitialSize=...; PresentMode = ViewerPresentMode.OffscreenReadback } type-checks.
```
