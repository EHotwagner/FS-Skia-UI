module ScreenshotGallery.Program

open System
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer.Host

type Model =
    { Size: Size
      Frame: int
      LastScreenshot: ScreenshotRequest option
      Diagnostics: RenderDiagnostic list
      Recovering: bool
      Closing: bool }

type Msg =
    | ViewerInput of ViewerEvent
    | NextFrame
    | RequestScreenshot of ScreenshotFormat
    | SimulateRecoverableFrameError
    | ShutdownRequested
    | HostEffect of ViewerEffect<Msg>

let initialSize = { Width = 760; Height = 460 }

let screenshotPath format =
    match format with
    | Png -> "specs/002-skia-feature-parity/readiness/screenshots/screenshot-gallery.png"
    | Jpeg -> "specs/002-skia-feature-parity/readiness/screenshots/screenshot-gallery.jpg"

let init () =
    { Size = initialSize
      Frame = 0
      LastScreenshot = None
      Diagnostics = []
      Recovering = false
      Closing = false },
    Cmd.ofMsg (HostEffect InitializeRenderer)

let view model =
    let width = float model.Size.Width
    let height = float model.Size.Height
    let frameOffset = float (model.Frame % 90)

    let stateColor =
        if model.Recovering then
            Colors.rgba 224uy 96uy 72uy 255uy
        else
            Colors.rgba 64uy 168uy 126uy 255uy

    let screenshotLabel =
        model.LastScreenshot
        |> Option.map (fun request -> $"{request.Format}: {request.Destination}")
        |> Option.defaultValue "none"

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (Colors.rgba 18uy 22uy 30uy 255uy)
        Scene.rectangle (36.0, 42.0, width - 72.0, 92.0) (Colors.rgba 42uy 78uy 118uy 255uy)
        Scene.rectangle (64.0 + frameOffset, 172.0, 180.0, 72.0) stateColor
        Scene.text (56.0, 96.0) "Screenshot Gallery" Colors.white
        Scene.text (56.0, 286.0) $"frame: {model.Frame}" Colors.white
        Scene.text (56.0, 326.0) $"last screenshot: {screenshotLabel}" Colors.white
        Scene.text (56.0, 366.0) $"diagnostics: {model.Diagnostics.Length}" Colors.white
    ]

let render model =
    Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let rec update msg model =
    match msg with
    | ViewerInput event ->
        match event with
        | Loaded -> model, render model
        | RenderTick _ -> model, render model
        | UpdateTick _ -> model, Cmd.none
        | Resized size ->
            let next = { model with Size = size }
            next, render next
        | CloseRequested ->
            { model with Closing = true },
            Cmd.ofMsg (HostEffect Shutdown)
        | DiagnosticReported diagnostic ->
            { model with Diagnostics = diagnostic :: model.Diagnostics },
            Cmd.ofMsg (HostEffect(ReportDiagnostic diagnostic))
        | ViewerEvent.KeyDown "Space" ->
            let next = { model with Frame = model.Frame + 1; Recovering = false }
            next, render next
        | ViewerEvent.KeyDown "P" ->
            update (RequestScreenshot Png) model
        | ViewerEvent.KeyDown "J" ->
            update (RequestScreenshot Jpeg) model
        | ViewerEvent.KeyDown "R" ->
            update SimulateRecoverableFrameError model
        | ViewerEvent.KeyDown "Escape" ->
            update ShutdownRequested model
        | ViewerEvent.KeyDown _
        | ViewerEvent.KeyUp _
        | PointerMoved _
        | PointerPressed _
        | PointerReleased _
        | PointerScrolled _
        | PointerExited -> model, Cmd.none
    | NextFrame ->
        let next = { model with Frame = model.Frame + 1; Recovering = false }
        next, render next
    | RequestScreenshot format ->
        let request =
            { Destination = screenshotPath format
              Format = format }

        { model with LastScreenshot = Some request },
        Cmd.ofMsg (HostEffect(CaptureScreenshot request))
    | SimulateRecoverableFrameError ->
        let diagnostic = Diagnostics.frameRenderFailed "sample-requested recoverable frame diagnostic"
        let next =
            { model with
                Diagnostics = diagnostic :: model.Diagnostics
                Recovering = true }

        next,
        Cmd.batch [
            Cmd.ofMsg (HostEffect(ReportDiagnostic diagnostic))
            Cmd.ofMsg (HostEffect(RenderFrame(view next)))
        ]
    | ShutdownRequested ->
        { model with Closing = true },
        Cmd.ofMsg (HostEffect Shutdown)
    | HostEffect _ ->
        model, Cmd.none

let configuration =
    { Viewer.defaultConfiguration "Screenshot Gallery" initialSize with
        ClearColor = Some(Colors.rgba 18uy 22uy 30uy 255uy)
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (ViewerInput >> Some)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let collect cmd =
    let messages = ResizeArray<Msg>()
    cmd |> List.iter (fun effect -> effect messages.Add)
    List.ofSeq messages

let runContractSmoke () =
    let model, initCmd = init ()
    let afterLoaded, renderCmd = update (ViewerInput Loaded) model
    let afterPng, pngCmd = update (RequestScreenshot Png) afterLoaded
    let afterRecovery, recoveryCmd = update SimulateRecoverableFrameError afterPng
    let afterShutdown, shutdownCmd = update ShutdownRequested afterRecovery
    let effects = [ initCmd; renderCmd; pngCmd; recoveryCmd; shutdownCmd ] |> List.collect collect

    printfn "status=ok"
    printfn "sample=ScreenshotGallery"
    printfn "frame=%d" afterShutdown.Frame
    printfn "closing=%b" afterShutdown.Closing
    printfn "recovering=%b" afterShutdown.Recovering
    printfn "diagnostics=%d" afterShutdown.Diagnostics.Length
    printfn "scene=%A" (view afterShutdown)

    let hasInitialize = effects |> List.exists ((=) (HostEffect InitializeRenderer))
    let hasRender =
        effects
        |> List.exists (function
            | HostEffect(RenderFrame _) -> true
            | _ -> false)
    let hasScreenshot =
        effects
        |> List.exists (function
            | HostEffect(CaptureScreenshot request) when request.Format = Png -> true
            | _ -> false)
    let hasDiagnostic =
        effects
        |> List.exists (function
            | HostEffect(ReportDiagnostic diagnostic) when diagnostic.Stage = FrameRender -> true
            | _ -> false)
    let hasShutdown = effects |> List.exists ((=) (HostEffect Shutdown))

    printfn "initialize-effect=%b" hasInitialize
    printfn "render-effect=%b" hasRender
    printfn "screenshot-effect=%b" hasScreenshot
    printfn "recovery-diagnostic-effect=%b" hasDiagnostic
    printfn "shutdown-effect=%b" hasShutdown

    if hasInitialize && hasRender && hasScreenshot && hasDiagnostic && hasShutdown then 0 else 4

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        match Viewer.run program with
        | Ok() -> 0
        | Result.Error diagnostic ->
            eprintfn "diagnostic-stage=%A" diagnostic.Stage
            eprintfn "diagnostic-severity=%A" diagnostic.Severity
            eprintfn "diagnostic-message=%s" diagnostic.Message
            eprintfn "diagnostic-cause=%A" diagnostic.Cause
            2
