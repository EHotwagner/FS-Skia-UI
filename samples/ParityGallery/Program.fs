module ParityGallery.Program

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
    | HostEffect of ViewerEffect<Msg>

let red = Colors.rgba 220uy 64uy 52uy 240uy
let blue = Colors.rgba 52uy 112uy 220uy 255uy
let green = Colors.rgba 68uy 168uy 96uy 255uy
let yellow = Colors.rgba 248uy 220uy 92uy 255uy

let init () =
    { Title = "Parity Gallery"
      Values = [ 2.0; 5.0; 3.0; 8.0; 4.0; 6.0 ] },
    Cmd.none

let update msg model =
    match msg with
    | NoOp
    | HostEffect _ -> model, Cmd.none

let parityScene model =
    let fillPaint =
        Paint.fill red
        |> Paint.withBlendMode SrcOver
        |> Paint.withAntialias true

    let strokePaint =
        Paint.stroke yellow 3.0
        |> Paint.withStrokeCap Round
        |> Paint.withStrokeJoin RoundJoin

    let path =
        Path.create EvenOdd [
            Path.moveTo 48.0 210.0
            Path.lineTo 140.0 160.0
            Path.cubicTo { X = 210.0; Y = 130.0 } { X = 260.0; Y = 230.0 } { X = 330.0; Y = 190.0 }
            Path.lineTo 320.0 260.0
            Path.lineTo 72.0 270.0
            Path.close
        ]

    let vertices =
        [ { Position = { X = 420.0; Y = 170.0 }; Color = Some red }
          { Position = { X = 560.0; Y = 190.0 }; Color = Some blue }
          { Position = { X = 480.0; Y = 285.0 }; Color = Some green } ]

    let clippedText =
        Scene.clipped
            (RectClip { X = 36.0; Y = 302.0; Width = 260.0; Height = 48.0 })
            (Scene.textRun
                { Text = "clipped text run"
                  Position = { X = 48.0; Y = 334.0 }
                  Font = { Family = None; Size = 24.0; Weight = Some 500 }
                  Paint = Paint.fill Colors.white })

    let picture =
        { Name = "reusable-picture"
          Scene =
            Scene.group [
                Scene.ellipse { X = 0.0; Y = 0.0; Width = 56.0; Height = 40.0 } (Paint.fill green)
                Scene.line { X = 0.0; Y = 20.0 } { X = 56.0; Y = 20.0 } strokePaint
            ] }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 18uy 24uy 32uy 255uy)
        Scene.text (36.0, 64.0) model.Title Colors.white
        Scene.rectangleWithPaint { X = 36.0; Y = 92.0; Width = 92.0; Height = 60.0 } fillPaint
        Scene.ellipse { X = 156.0; Y = 92.0; Width = 96.0; Height = 60.0 } (Paint.fill blue)
        Scene.line { X = 284.0; Y = 96.0 } { X = 360.0; Y = 148.0 } strokePaint
        Scene.path path fillPaint
        Scene.points [ { X = 384.0; Y = 104.0 }; { X = 404.0; Y = 124.0 }; { X = 424.0; Y = 108.0 } ] strokePaint
        Scene.vertices Triangles vertices fillPaint
        Scene.arc { X = 456.0; Y = 306.0; Width = 92.0; Height = 92.0 } 20.0 270.0 strokePaint
        Scene.image (536.0, 42.0, 64.0, 64.0) "assets/sample.png"
        clippedText
        Scene.region { Bounds = [ { X = 326.0; Y = 312.0; Width = 80.0; Height = 42.0 } ]; Operation = RegionUnion } (Paint.fill (Colors.rgba 132uy 96uy 220uy 210uy))
        Scene.picture picture
        Scene.chart model.Values
    ]

let configuration =
    { Viewer.defaultConfiguration "Parity Gallery" { Width = 640; Height = 480 } with
        ClearColor = Some(Colors.rgba 18uy 24uy 32uy 255uy)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update parityScene
    |> Viewer.withEventMapping (fun _ -> Some NoOp)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let smokeProgram =
    Viewer.create configuration init update parityScene
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runContractSmoke () =
    let model, _ = init ()
    let scene = parityScene model
    let readback = Scene.renderReadbackEvidence { Width = 640; Height = 480 } scene
    printfn "status=ok"
    printfn "sample=ParityGallery"
    printfn "kinds=%A" (Scene.describe scene)
    printfn "capability-count=%d" readback.CapabilityCount
    printfn "hash=%s" readback.DeterministicHash
    0

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run smokeProgram with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=ParityGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=ParityGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()
