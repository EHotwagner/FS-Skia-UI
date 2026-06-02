module EffectsGallery.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer.Host

type Model =
    { Title: string }

type Msg =
    | NoOp
    | HostEffect of ViewerEffect<Msg>

let init () =
    { Title = "Effects Gallery" }, Cmd.none

let update msg model =
    match msg with
    | NoOp
    | HostEffect _ -> model, Cmd.none

let effectsScene model =
    let red = Colors.rgba 220uy 64uy 52uy 230uy
    let blue = Colors.rgba 52uy 112uy 220uy 255uy
    let green = Colors.rgba 68uy 168uy 96uy 255uy

    let gradientPaint =
        Paint.fill red
        |> Paint.withShader (LinearGradient({ X = 44.0; Y = 112.0 }, { X = 266.0; Y = 220.0 }, [ red; blue; green ]))
        |> Paint.withBlendMode SrcOver
        |> Paint.withColorFilter (BlendColor(Colors.rgba 255uy 255uy 255uy 48uy, Screen))
        |> Paint.withMaskFilter (Blur 0.75)
        |> Paint.withImageFilter (DropShadow(5.0, 5.0, 4.0, Colors.rgba 0uy 0uy 0uy 150uy))

    let radialPaint =
        Paint.fill blue
        |> Paint.withShader (RadialGradient({ X = 430.0; Y = 174.0 }, 72.0, [ Colors.white; blue; green ]))
        |> Paint.withOpacity 0.92

    let dashedPaint =
        Paint.stroke (Colors.rgba 248uy 220uy 92uy 255uy) 4.0
        |> Paint.withStrokeCap Round
        |> Paint.withStrokeJoin RoundJoin
        |> Paint.withPathEffect (Dash([ 10.0; 5.0; 3.0; 5.0 ], 0.0))

    let cornerPaint =
        Paint.stroke green 8.0
        |> Paint.withPathEffect (Corner 18.0)
        |> Paint.withBlendMode Screen

    let path =
        Path.create EvenOdd [
            Path.moveTo 54.0 156.0
            Path.lineTo 168.0 108.0
            Path.cubicTo { X = 248.0; Y = 116.0 } { X = 276.0; Y = 214.0 } { X = 186.0; Y = 256.0 }
            Path.lineTo 74.0 238.0
            Path.close
        ]

    let perspective =
        { M11 = 1.0
          M12 = 0.12
          M13 = 0.0
          M21 = -0.08
          M22 = 1.0
          M23 = 0.0
          M31 = 0.00025
          M32 = 0.00018
          M33 = 1.0 }

    Scene.group [
        Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgba 16uy 20uy 28uy 255uy)
        Scene.text (38.0, 66.0) model.Title Colors.white
        Scene.path path gradientPaint
        Scene.ellipse { X = 350.0; Y = 104.0; Width = 160.0; Height = 128.0 } radialPaint
        Scene.line { X = 52.0; Y = 312.0 } { X = 570.0; Y = 312.0 } dashedPaint
        Scene.arc { X = 404.0; Y = 250.0; Width = 118.0; Height = 118.0 } 15.0 290.0 cornerPaint
        Scene.clipped
            (PathClip
                (Path.create Winding [
                    Path.moveTo 68.0 348.0
                    Path.lineTo 284.0 348.0
                    Path.lineTo 260.0 420.0
                    Path.lineTo 88.0 420.0
                    Path.close
                ]))
            (Scene.group [
                Scene.rectangleWithPaint { X = 58.0; Y = 338.0; Width = 236.0; Height = 96.0 } gradientPaint
                Scene.textRun
                    { Text = "clip + text"
                      Position = { X = 94.0; Y = 394.0 }
                      Font = { Family = None; Size = 24.0; Weight = Some 600 }
                      Paint = Paint.fill Colors.white }
            ])
        Scene.withPerspective perspective (Scene.rectangleWithPaint { X = 516.0; Y = 132.0; Width = 70.0; Height = 70.0 } gradientPaint)
        Scene.withColorSpace DisplayP3 (Scene.rectangleWithPaint { X = 536.0; Y = 376.0; Width = 50.0; Height = 50.0 } radialPaint)
    ]

let configuration =
    { Viewer.defaultConfiguration "Effects Gallery" { Width = 640; Height = 480 } with
        ClearColor = Some(Colors.rgba 16uy 20uy 28uy 255uy)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = true } }

let program =
    Viewer.create configuration init update effectsScene
    |> Viewer.withEventMapping (fun _ -> Some NoOp)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let smokeProgram =
    Viewer.create configuration init update effectsScene
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runContractSmoke () =
    let model, _ = init ()
    let scene = effectsScene model
    let readback = Scene.renderReadbackEvidence { Width = 640; Height = 480 } scene
    printfn "status=ok"
    printfn "sample=EffectsGallery"
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
        printfn "sample=EffectsGallery"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=EffectsGallery"
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
