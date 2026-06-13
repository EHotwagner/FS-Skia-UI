module Feature120Evidence
// Feature 120 live evidence harness: drives the PRODUCTION Host.Viewer.run -> GlHost.run on real GL
// hardware, in DirectToSwapchain. It renders a changed scene then an UNCHANGED scene and reads
// GlHost.lastPresentTiming() after each: a presented frame reports non-zero paint+compose durations
// (US1); an unchanged frame is idle-skipped so its timing is (Zero, Zero) (US2). The present-mode
// readback=false diagnostic is captured (US4). Self-closes after a bounded run.
open System
open Elmish
open FS.Skia.UI.Scene
open FS.Skia.UI.SkiaViewer
open FS.Skia.UI.SkiaViewer.Host

type Msg = | RenderNow | Tick | HostFx of ViewerEffect<Msg>

[<EntryPoint>]
let main _argv =
    let mutable presentDiag = ""
    let mutable changedTiming = ""
    let mutable idleTiming = ""
    // a scene parameterized by a "phase": phases 0..14 change each tick; 15..40 hold one fixed scene (idle).
    let view (tick: int) : Scene =
        let phase = if tick >= 15 then 999 else tick
        { Nodes =
            (Scene.rectangle (0.0, 0.0, 640.0, 480.0) (Colors.rgb 18uy 22uy 30uy)).Nodes
            @ (Scene.filledRectangle { X = 40.0 + float (phase % 12) * 10.0; Y = 180.0; Width = 120.0; Height = 120.0 } (Colors.rgb 255uy 138uy 0uy)).Nodes
            @ (Scene.text (40.0, 60.0) "feature 120 idle-skip + timing" (Colors.rgb 235uy 235uy 240uy)).Nodes }
    let renderFx (s: Scene) : Msg = HostFx (ViewerEffect.RenderFrame s)
    let shutdownFx : Msg = HostFx ViewerEffect.Shutdown
    let config = { Host.Viewer.defaultConfiguration "fs-skia-f120-evidence" { Width = 640; Height = 480 } with Diagnostics = { Verbose = false } }
    let init () = 0, Cmd.none
    let update msg (model: int) =
        match msg with
        | RenderNow -> model, Cmd.ofEffect (fun d -> d (renderFx (view model)))
        | Tick ->
            let n = model + 1
            // capture timing AFTER the previous present
            if n = 10 then changedTiming <- sprintf "%A" (GlHost.lastPresentTiming())
            if n = 30 then idleTiming <- sprintf "%A" (GlHost.lastPresentTiming())
            if n >= 40 then n, Cmd.ofEffect (fun d -> d shutdownFx)
            else n, Cmd.ofEffect (fun d -> d (renderFx (view n)))
        | HostFx _ -> model, Cmd.none
    let program =
        { Host.Viewer.create config init update view with
            EventMapper =
                (fun ev ->
                    match ev with
                    | Loaded -> Some RenderNow
                    | RenderTick _ | UpdateTick _ -> Some Tick
                    | DiagnosticReported d -> (if d.Message.Contains "present-mode" then presentDiag <- d.Message); None
                    | _ -> None)
            EffectMapper = (fun msg -> match msg with HostFx fx -> Some fx | _ -> None) }
    let result = Host.Viewer.run program
    printfn "EVIDENCE result=%A presentDiag=[%s] changedTiming=%s idleTiming=%s" result presentDiag changedTiming idleTiming
    0
