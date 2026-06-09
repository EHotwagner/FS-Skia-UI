module InteractiveHostEvidence.Program

// Feature 085 T018 — durable interactive-host launch evidence. Launches
// ControlsElmish.runInteractiveApp with a self-closing host (Tick -> CloseWindow after a few
// frames present) and prints the ViewerLaunchOutcome as key=value lines for the
// window-visibility evidence class. Compiled exe (Silk.NET Vulkan windowing needs a real project).

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

type Msg = SelfClose

let mutable ticks = 0

let host: InteractiveAppHost<int, Msg> =
    { Init = fun () -> 0, []
      Update = fun SelfClose model -> model, [ CloseWindow ]
      View =
        fun _ _ ->
            Stack.create
                [ Stack.children
                      [ TextBlock.create [ TextBlock.text "Feature 085 interactive host" ]
                        Button.create [ Button.text "Click"; Button.onClick SelfClose ] |> Control.withKey "click" ] ]
      Theme = Theme.light
      MapKey = fun _ _ -> None
      MapPointer = fun _ -> Some SelfClose
      Tick =
        fun _ ->
            ticks <- ticks + 1
            if ticks >= 4 then Some SelfClose else None
      Diagnostics = Viewer.defaultDiagnostics }

[<EntryPoint>]
let main _ =
    let options: ViewerOptions =
        { Title = "Feature085 Interactive Host"; InitialSize = { Width = 800; Height = 600 } }

    match ControlsElmish.runInteractiveApp options host with
    | Result.Ok outcome ->
        printfn "status=%s" outcome.Status
        printfn "mode=%s" outcome.Mode
        printfn "window-opened=%b" outcome.WindowOpened
        printfn "window-visible=%A" outcome.WindowVisible
        printfn "first-frame-presented=%b" outcome.FirstFramePresented
        printfn "close-reason=%A" outcome.CloseReason
        printfn "user-close-observed=%b" outcome.UserCloseObserved
        printfn "app-close-observed=%b" outcome.AppCloseObserved
        printfn "evidence-close-observed=%b" outcome.EvidenceCloseObserved
        printfn "self-closed-for-evidence=%b" outcome.SelfClosedForEvidence
        printfn "renderer-mode=%s" outcome.RendererMode
        printfn "exit-path=%b" outcome.ExitPath
        printfn "input-dispatch=%s" outcome.InputDispatch
        0
    | Result.Error failure ->
        printfn "BLOCKED stage=%A classification=%A category=%A" failure.BlockedStage failure.Classification failure.DiagnosticCategory
        printfn "message=%s" failure.Message
        1
