module Feature118.LiveHost.Program

// Feature 118 — launch the REAL live Vulkan window through the production present path
// (Host.Viewer.run → VulkanHost.renderFrame) in a selectable ViewerPresentMode, present live
// frames, capture an on-demand screenshot through the offscreen readback routine (FR-004),
// record the live-only present-mode / fallback diagnostics (FR-005/FR-007), and self-close
// for bounded evidence. Mode + screenshot path come from env vars so the same binary proves
// both DirectToSwapchain and OffscreenReadback.

open System
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.SkiaViewer.Host

let initialSize = { Width = 480; Height = 320 }

let modeArg =
    match Environment.GetEnvironmentVariable "FEATURE118_MODE" with
    | null
    | "" -> "offscreen"
    | value -> value.Trim().ToLowerInvariant()

let presentMode =
    if modeArg = "direct" then
        ViewerPresentMode.DirectToSwapchain
    else
        ViewerPresentMode.OffscreenReadback

let screenshotPath =
    match Environment.GetEnvironmentVariable "FEATURE118_SHOT" with
    | null
    | "" -> $"specs/118-backend-host-review/readiness/smoke/{modeArg}-frame.png"
    | value -> value

let captureFrame = 24
let shutdownFrame = 40

type Msg =
    | ViewerInput of ViewerEvent
    | RequestScreenshot
    | HostEffect of ViewerEffect<Msg>

type Model =
    { Frame: int
      Captured: bool
      Closing: bool
      Diagnostics: RenderDiagnostic list }

let lastModel = ref { Frame = 0; Captured = false; Closing = false; Diagnostics = [] }

let view _ =
    Scene.group [
        Scene.rectangle (0.0, 0.0, 480.0, 320.0) (Colors.rgba 18uy 22uy 30uy 255uy)
        Scene.rectangle (40.0, 48.0, 400.0, 96.0) (Colors.rgba 64uy 168uy 126uy 255uy)
        Scene.rectangle (80.0, 176.0, 200.0, 80.0) (Colors.rgba 224uy 150uy 72uy 255uy)
        Scene.text (56.0, 104.0) "Feature 118 present path" Colors.white
        Scene.text (56.0, 232.0) "DirectToSwapchain / OffscreenReadback" Colors.white
    ]

let init () =
    { Frame = 0; Captured = false; Closing = false; Diagnostics = [] },
    Cmd.ofMsg (HostEffect InitializeRenderer)

let render model = Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let update msg model =
    let next, cmd =
        match msg with
        | ViewerInput event ->
            match event with
            | Loaded -> model, render model
            | RenderTick _ ->
                let advanced = { model with Frame = model.Frame + 1 }

                if advanced.Frame = captureFrame && not advanced.Captured then
                    advanced, Cmd.batch [ render advanced; Cmd.ofMsg RequestScreenshot ]
                elif advanced.Frame >= shutdownFrame then
                    { advanced with Closing = true }, Cmd.ofMsg (HostEffect Shutdown)
                else
                    advanced, render advanced
            | UpdateTick _ -> model, Cmd.none
            | DiagnosticReported diagnostic ->
                eprintfn "DIAG: %A %A %s" diagnostic.Severity diagnostic.Stage diagnostic.Message
                { model with Diagnostics = diagnostic :: model.Diagnostics }, Cmd.none
            | CloseRequested -> { model with Closing = true }, Cmd.ofMsg (HostEffect Shutdown)
            | _ -> model, Cmd.none
        | RequestScreenshot ->
            let request: ScreenshotRequest = { Destination = screenshotPath; Format = Png }
            eprintfn "CAPTURE: %s" screenshotPath
            { model with Captured = true }, Cmd.ofMsg (HostEffect(CaptureScreenshot request))
        | HostEffect _ -> model, Cmd.none

    lastModel.Value <- next
    next, cmd

[<EntryPoint>]
let main _ =
    eprintfn "MODE: %s" modeArg

    let configuration =
        { Viewer.defaultConfiguration "Feature 118 Live" initialSize with
            ClearColor = Some(Colors.rgba 18uy 22uy 30uy 255uy)
            Diagnostics = { Verbose = false }
            PresentMode = presentMode }

    let program =
        Viewer.create configuration init update view
        |> Viewer.withEventMapping (ViewerInput >> Some)
        |> Viewer.withEffectMapping (function
            | HostEffect effect -> Some effect
            | _ -> None)

    match Viewer.run program with
    | Ok() ->
        let m = lastModel.Value
        printfn "RESULT: ok frames=%d captured=%b diagnostics=%d" m.Frame m.Captured m.Diagnostics.Length
        0
    | Result.Error diagnostic ->
        printfn "RESULT: error %A %s" diagnostic.Stage diagnostic.Message
        1
