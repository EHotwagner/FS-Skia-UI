module LiveHost.Program

// Feature 090 — launch the REAL live Vulkan window running the production interactive path
// (ControlsElmish.runInteractiveApp) with an authored Button.onClick binding, present the
// production render path, repaint on each dispatched message (the loop that was DEAD in the
// ControlsShowcase2 bug), and self-close for evidence. Prints the ViewerLaunchOutcome.

open System
open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

type Msg =
    | FrameTick        // one per presented frame; advances the deterministic evidence script

type Model = { Count: int; Frames: int }

// `Tick` receives a PER-FRAME delta (~16ms), not cumulative time, so the evidence script
// accumulates frame count in the model rather than thresholding on elapsed.
let private incrementEvery = 30   // bump the counter every ~0.5s of presented frames
let private closeAfter = 120      // self-close after ~2s of presented frames

// The production interactive host: a leaf-keyed Button authored the documented way (onClick),
// no MapPointer hand-routing. The live window renders host.View via Control.renderTree and
// repaints on each dispatched message — the exact loop that was DEAD in ControlsShowcase2.
let host: InteractiveAppHost<Model, Msg> =
    { Init = fun () -> { Count = 0; Frames = 0 }, []
      Update =
        fun msg model ->
            match msg with
            | FrameTick ->
                let frames = model.Frames + 1
                let model' = { Count = frames / incrementEvery; Frames = frames }
                let effect = if frames >= closeAfter then CloseWindow else RenderScene SceneNode.Empty
                model', [ effect ]
      View =
        fun _size model ->
            Stack.create
                [ Stack.children
                      [ TextBlock.create [ TextBlock.text (sprintf "Count = %d" model.Count) ]
                        Button.create [ Button.text "+1"; Button.onClick FrameTick ] |> Control.withKey "inc" ] ]
      Theme = Theme.light
      MapKey = fun _ _ -> None
      MapPointer = fun _ -> None
      Tick = fun elapsed -> if elapsed >= TimeSpan.FromMilliseconds 16.0 then Some FrameTick else None
      Diagnostics = Viewer.defaultDiagnostics }

[<EntryPoint>]
let main _ =
    let options: ViewerOptions = { Title = "FS.Skia.UI 090 — live interactive host"; InitialSize = { Width = 480; Height = 240 }; PresentMode = ViewerPresentMode.OffscreenReadback }
    match ControlsElmish.runInteractiveApp options host with
    | Ok outcome ->
        printfn "LIVE-LAUNCH-OUTCOME"
        printfn "status=%s" outcome.Status
        printfn "mode=%s" outcome.Mode
        printfn "renderer-mode=%s" outcome.RendererMode
        printfn "window-opened=%b" outcome.WindowOpened
        printfn "window-visible=%A" outcome.WindowVisible
        printfn "first-frame-presented=%b" outcome.FirstFramePresented
        printfn "self-closed-for-evidence=%b" outcome.SelfClosedForEvidence
        printfn "close-reason=%A" outcome.CloseReason
        printfn "input-dispatch=%s" outcome.InputDispatch
        printfn "message=%s" outcome.Message
        0
    | other ->
        printfn "LIVE-LAUNCH-FAILED"
        printfn "%A" other
        1
