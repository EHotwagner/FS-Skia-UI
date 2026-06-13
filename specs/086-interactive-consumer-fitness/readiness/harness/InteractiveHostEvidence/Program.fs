module InteractiveHostEvidence.Program

// Feature 086 T026 (SC-002 live) — durable controls-family interactive-host launch evidence.
// Launches ControlsElmish.runInteractiveApp with the REAL example controls and a self-closing
// host (Tick -> CloseWindow after a few frames present), printing the ViewerLaunchOutcome as
// key=value lines for the window-visibility evidence class. Compiled exe (Vulkan needs a project).

open FS.Skia.UI.Scene
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.SkiaViewer

type Msg = SaveRequested | SelfClose
type Model = { Saved: bool }

let mutable ticks = 0

// The real example controls (mirrors the generated scaffold's controlsExampleView): a form,
// button, chart and graph laid out by Control.renderTree and painted to the live window.
let exampleView () : Control<Msg> =
    Stack.create
        [ Stack.children
              [ TextBlock.create [ TextBlock.text "Product controls" ]
                TextBox.create [ TextBox.value "Product" ]
                Button.create [ Button.text "Save"; Button.onClick SaveRequested ] |> Control.withKey "save"
                LineChart.create [ LineChart.series [ { Name = "Revenue"; Points = [ {X=0.0;Y=12.0;Label=Some "Q1"}; {X=1.0;Y=18.0;Label=Some "Q2"}; {X=2.0;Y=15.0;Label=Some "Q3"} ] } ] ]
                GraphView.create [ GraphView.nodes [ "form"; "chart"; "grid" ] ] ] ]

let host: InteractiveAppHost<Model, Msg> =
    { Init = fun () -> { Saved = false }, []
      Update =
        fun msg model ->
            match msg with
            | SaveRequested -> { model with Saved = true }, [ RenderScene SceneNode.Empty ]
            | SelfClose -> model, [ CloseWindow ]
      View = fun _ _ -> exampleView ()
      Theme = Theme.light
      MapKey = fun _ _ -> None
      MapPointer = fun i -> match i with | Click("save", _, _, _) -> Some SaveRequested | _ -> None
      Tick = fun _ -> (ticks <- ticks + 1; if ticks >= 4 then Some SelfClose else None)
      Diagnostics = Viewer.defaultDiagnostics }

[<EntryPoint>]
let main _ =
    let options: ViewerOptions = { Title = "Feature086 Controls Host"; InitialSize = { Width = 800; Height = 600 }; PresentMode = ViewerPresentMode.OffscreenReadback }
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
