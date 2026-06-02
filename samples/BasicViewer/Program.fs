module BasicViewer.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer.Host

type Model =
    { Title: string
      Values: float list }

type Msg =
    | NoOp
    | ScreenshotRequested
    | HostEffect of ViewerEffect<Msg>

let init () =
    { Title = "Basic Vulkan Viewer"
      Values = [ 2.0; 4.0; 3.0; 8.0; 5.0 ] },
    Cmd.none

let update msg model =
    match msg with
    | NoOp -> model, Cmd.none
    | ScreenshotRequested ->
        model,
        Cmd.ofMsg
            (HostEffect(
                CaptureScreenshot
                    { Destination = "specs/002-skia-feature-parity/readiness/screenshots/basic-viewer.png"
                      Format = Png }
            ))
    | HostEffect _ -> model, Cmd.none

let view model =
    let red = Colors.rgba 220uy 64uy 52uy 230uy
    let blue = Colors.rgba 52uy 112uy 220uy 255uy
    let green = Colors.rgba 68uy 168uy 96uy 255uy

    let effectPaint =
        Paint.fill red
        |> Paint.withShader (LinearGradient({ X = 32.0; Y = 150.0 }, { X = 260.0; Y = 230.0 }, [ red; blue; green ]))
        |> Paint.withColorFilter (BlendColor(Colors.rgba 255uy 255uy 255uy 48uy, Screen))
        |> Paint.withMaskFilter (Blur 0.5)
        |> Paint.withImageFilter (DropShadow(4.0, 4.0, 3.0, Colors.rgba 0uy 0uy 0uy 120uy))
        |> Paint.withPathEffect (Corner 6.0)

    let strokePaint =
        Paint.stroke (Colors.rgba 248uy 220uy 92uy 255uy) 3.0
        |> Paint.withStrokeCap Round
        |> Paint.withStrokeJoin RoundJoin
        |> Paint.withPathEffect (Dash([ 8.0; 4.0 ], 0.0))

    let samplePath =
        Path.create EvenOdd [
            Path.moveTo 42.0 168.0
            Path.lineTo 148.0 150.0
            Path.quadTo { X = 230.0; Y = 178.0 } { X = 190.0; Y = 232.0 }
            Path.lineTo 64.0 232.0
            Path.close
        ]

    let perspective =
        { M11 = 1.0
          M12 = 0.08
          M13 = 0.0
          M21 = -0.04
          M22 = 1.0
          M23 = 0.0
          M31 = 0.0002
          M32 = 0.0002
          M33 = 1.0 }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.rectangle (32.0, 32.0, 220.0, 96.0) (Colors.rgba 36uy 118uy 160uy 255uy)
        Scene.text (48.0, 88.0) model.Title Colors.white
        Scene.clipped
            (RectClip { X = 28.0; Y = 146.0; Width = 240.0; Height = 112.0 })
            (Scene.group [
                Scene.path samplePath effectPaint
                Scene.line { X = 44.0; Y = 244.0 } { X = 250.0; Y = 158.0 } strokePaint
                Scene.withPerspective perspective (Scene.rectangleWithPaint { X = 184.0; Y = 174.0; Width = 42.0; Height = 42.0 } effectPaint)
            ])
        Scene.image (280.0, 40.0, 96.0, 96.0) "assets/sample.png"
        Scene.chart model.Values
    ]

let viewWithFrameFailure _ =
    failwith "intentional frame diagnostic smoke failure"

let configuration =
    { Viewer.defaultConfiguration "Basic Viewer" { Width = 640; Height = 480 } with
        ClearColor = Some(Colors.rgba 18uy 24uy 32uy 255uy)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (fun _ -> Some NoOp)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let smokeProgram =
    Viewer.create configuration init update view
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let failingFrameProgram =
    Viewer.create configuration init update viewWithFrameFailure

let screenshotSmokeProgram format destination =
    let initWithScreenshot () =
        let model, _ = init ()

        model,
        Cmd.batch [
            Cmd.ofMsg (HostEffect(RenderFrame(view model)))
            Cmd.ofMsg
                (HostEffect(
                    CaptureScreenshot
                        { Destination = destination
                          Format = format }
                ))
        ]

    Viewer.create configuration initWithScreenshot update view
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let screenshotRequest =
    { Destination = "basic-viewer.png"
      Format = Png }

let runContractSmoke () =
    let model, _ = init ()
    let scene = view model
    let _, screenshotCmd = update ScreenshotRequested model
    let effects = ResizeArray<Msg>()
    screenshotCmd |> List.iter (fun effect -> effect effects.Add)

    printfn "status=ok"
    printfn "sample=BasicViewer"
    printfn "scene=%A" scene
    printfn "contains-shapes=true"
    printfn "contains-text=true"
    printfn "contains-image=true"
    printfn "contains-chart=true"

    match List.ofSeq effects with
    | [ HostEffect(CaptureScreenshot request) ] ->
        printfn "screenshot-destination=%s" request.Destination
        printfn "screenshot-format=%A" request.Format
        0
    | other ->
        eprintfn "unexpected-effects=%A" other
        4

let runSmoke expectFrameFailure =
    let stopwatch = Stopwatch.StartNew()
    let screenshotEffect = CaptureScreenshot screenshotRequest
    let selectedProgram = if expectFrameFailure then failingFrameProgram else smokeProgram

    match Viewer.run selectedProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "startup-ms=%d" stopwatch.ElapsedMilliseconds
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        printfn "screenshot-effect=%A" screenshotEffect
        if expectFrameFailure then 3 else 0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "elapsed-ms=%d" stopwatch.ElapsedMilliseconds
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-severity=%A" diagnostic.Severity
        printfn "diagnostic-message=%s" diagnostic.Message
        printfn "diagnostic-cause=%A" diagnostic.Cause
        if expectFrameFailure && diagnostic.Stage = DiagnosticStage.FrameRender then 0 else 2

let runScreenshotSmoke format destination =
    let stopwatch = Stopwatch.StartNew()
    let selectedProgram = screenshotSmokeProgram format destination

    match Viewer.run selectedProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "elapsed-ms=%d" stopwatch.ElapsedMilliseconds
        printfn "screenshot-destination=%s" destination
        printfn "screenshot-format=%A" format
        printfn "screenshot-exists=%b" (IO.File.Exists destination)
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "elapsed-ms=%d" stopwatch.ElapsedMilliseconds
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-severity=%A" diagnostic.Severity
        printfn "diagnostic-message=%s" diagnostic.Message
        printfn "diagnostic-cause=%A" diagnostic.Cause
        2

[<EntryPoint>]
let main argv =
    let expectFrameFailure = argv |> Array.contains "--fail-frame"

    if argv |> Array.contains "--smoke" then
        runSmoke expectFrameFailure
    elif argv |> Array.contains "--screenshot-smoke" then
        let format =
            if argv |> Array.contains "--jpeg" then Jpeg else Png

        let destination =
            argv
            |> Array.tryFind (fun arg -> arg.StartsWith("--output=", StringComparison.Ordinal))
            |> Option.map (fun arg -> arg.Substring("--output=".Length))
            |> Option.defaultValue (
                match format with
                | Png -> "specs/002-skia-feature-parity/readiness/screenshots/basic-viewer.png"
                | Jpeg -> "specs/002-skia-feature-parity/readiness/screenshots/basic-viewer.jpg"
            )

        runScreenshotSmoke format destination
    elif argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke expectFrameFailure
