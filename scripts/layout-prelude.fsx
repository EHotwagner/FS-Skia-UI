#r "nuget: Fable.Elmish, 4.2.0"
#r "nuget: Yoga.Net, 3.2.3"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"
#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"

open FS.Skia.UI
open FS.Skia.UI.Scene
open FS.Skia.UI.Layout

let config = Defaults.stackConfig 800.0 600.0
let child = Defaults.child (Scene.text (10.0, 20.0) "layout" Colors.white)
let scene = Layout.horizontalStack config [ child ]
printfn "layout-prelude scene=%A" (box scene |> isNull |> not)

let measured (text: string) =
    fun request ->
        { Width = min request.AvailableWidth (float text.Length * 8.0)
          Height = 18.0
          Diagnostics = [] }

let automaticChild id (text: string) grow =
    { Defaults.layoutNode id with
        Intent = { Defaults.layoutIntent with FlexGrow = grow }
        Measure = Some(measured text)
        Content = Some(Scene.text (0.0, 0.0) text Colors.white) }

let automaticRoot =
    { Defaults.layoutNode "root" with
        Intent =
            { Defaults.layoutIntent with
                Direction = Row
                Wrap = Wrap
                Padding = { Left = 12.0; Top = 12.0; Right = 12.0; Bottom = 12.0 }
                Gap = { Row = 8.0; Column = 8.0 } }
        Children =
            [ automaticChild "title" "Dashboard" 1.0
              automaticChild "status" "Ready" 0.0
              automaticChild "actions" "Save" 0.0 ] }

let automaticResult = Layout.evaluate (Defaults.availableSpace 640.0 240.0) automaticRoot
let automaticScene = Layout.renderComputed automaticResult automaticRoot
let snapPolicy = Defaults.pixelSnapPolicy 1.0
let hit = Layout.hitTestComputed snapPolicy automaticResult 24.0 24.0
let focusRegion =
    automaticResult.Bounds
    |> List.find (fun item -> item.NodeId = "actions")
    |> fun item -> Layout.snapBounds snapPolicy item.Bounds

let workflowModel, workflowEffects = Layout.initWorkflow (Defaults.availableSpace 640.0 240.0) automaticRoot
let resizedWorkflow, resizeEffects =
    Layout.updateWorkflow (LayoutHostResized(Defaults.availableSpace 480.0 240.0)) workflowModel
let completedWorkflow = Layout.interpretWorkflowEffect resizeEffects.Head resizedWorkflow
let readyWorkflow, _ = Layout.updateWorkflow completedWorkflow resizedWorkflow

printfn "automatic-layout bounds=%d diagnostics=%d" automaticResult.Bounds.Length automaticResult.Diagnostics.Length
printfn "automatic-layout hit=%A scene=%A" hit (Scene.describe automaticScene)
printfn "automatic-layout focus-region=%A" focusRegion
printfn "automatic-layout workflow-start-effects=%A resize-effects=%A result=%b" workflowEffects resizeEffects readyWorkflow.Result.IsSome
