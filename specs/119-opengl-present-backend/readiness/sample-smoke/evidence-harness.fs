module GlHostEvidence
open System
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.SkiaViewer.Host

type Msg =
    | RenderNow
    | Tick
    | HostFx of FS.Skia.UI.SkiaViewer.Host.ViewerEffect<Msg>

[<EntryPoint>]
let main argv =
    let shotPath = if argv.Length > 0 then argv.[0] else "/tmp/glhost/frame.png"
    let mutable presentDiag = ""
    let mutable captured = false
    let view (tick: int) : Scene =
        let x = 40.0 + float (tick % 40) * 12.0
        { Nodes =
            (Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgb 18uy 22uy 30uy)).Nodes
            @ (Scene.filledRectangle { X = x; Y = 180.0; Width = 120.0; Height = 120.0 } (Colors.rgb 255uy 138uy 0uy)).Nodes
            @ (Scene.text (40.0, 60.0) "GL direct present feature 119" (Colors.rgb 235uy 235uy 240uy)).Nodes }

    let renderFx (s: Scene) : Msg = HostFx (FS.Skia.UI.SkiaViewer.Host.ViewerEffect.RenderFrame s)
    let shutdownFx : Msg = HostFx FS.Skia.UI.SkiaViewer.Host.ViewerEffect.Shutdown
    let captureFx : Msg = HostFx (FS.Skia.UI.SkiaViewer.Host.ViewerEffect.CaptureScreenshot { Destination = shotPath; Format = Png })

    let config =
        { Host.Viewer.defaultConfiguration "fs-skia-gl-host-evidence" { Width = 640; Height = 480 } with
            Diagnostics = { Verbose = false } }
    let init () = 0, Cmd.none
    let update msg (model: int) =
        match msg with
        | RenderNow -> model, Cmd.ofEffect (fun d -> d (renderFx (view model)))
        | Tick ->
            let n = model + 1
            if n = 25 && not captured then
                captured <- true
                n, Cmd.batch [ Cmd.ofEffect (fun d -> d captureFx); Cmd.ofEffect (fun d -> d (renderFx (view n))) ]
            elif n >= 60 then n, Cmd.ofEffect (fun d -> d shutdownFx)
            else n, Cmd.ofEffect (fun d -> d (renderFx (view n)))
        | HostFx _ -> model, Cmd.none
    let program =
        { Host.Viewer.create config init update view with
            EventMapper =
                (fun ev ->
                    match ev with
                    | Loaded -> Some RenderNow
                    | RenderTick _ -> Some Tick
                    | UpdateTick _ -> Some Tick
                    | DiagnosticReported d ->
                        if d.Message.Contains "present-mode" then presentDiag <- d.Message
                        None
                    | _ -> None)
            EffectMapper = (fun msg -> match msg with HostFx fx -> Some fx | _ -> None) }
    let result = Host.Viewer.run program
    let shotExists = IO.File.Exists shotPath
    let shotLen = if shotExists then (IO.FileInfo shotPath).Length else 0L
    printfn "EVIDENCE result=%A presentDiag=[%s] screenshot=%b bytes=%d" result presentDiag shotExists shotLen
    match result with Ok () -> 0 | Result.Error _ -> 1
