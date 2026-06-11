#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Scene
open FS.Skia.UI.Layout
open FS.Skia.UI.Controls

type Msg =
    | Save
    | NameChanged of string

let chartSeries =
    [ { Name = "sales"
        Points =
            [ { X = 0.0; Y = 4.0; Label = Some "Q1" }
              { X = 1.0; Y = 8.0; Label = Some "Q2" } ] } ]

let gridColumns =
    [ { Key = "name"; Header = "Name"; Width = 180.0; ColumnType = TextColumn }
      { Key = "amount"; Header = "Amount"; Width = 96.0; ColumnType = NumericColumn } ]

let gridRows =
    [ { Key = "row-00001"
        Cells =
            [ { RowKey = "row-00001"; ColumnKey = "name"; Value = "Ada" }
              { RowKey = "row-00001"; ColumnKey = "amount"; Value = "42" } ] } ]

let gridColumnsAttr: Attr<Msg> = DataGrid.columns gridColumns

let view name canSave =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Controls FSI" ]
            TextBox.create [
                TextBox.value name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled canSave
                Button.onClick Save
            ]
            LineChart.create [ LineChart.series chartSeries ]
            GraphView.create [ GraphView.nodes [ "form"; "chart"; "grid" ] ]
            DataGrid.create gridColumns [ DataGrid.rows gridRows ]
        ]
    ]

let root = view "Ada" true
let rendered = Control.render Theme.light root
let runtime, runtimeInitEffects = ControlRuntime.init ()
let focusedRuntime, focusEffects = ControlRuntime.update (FocusControl(Some "save-button")) runtime
let recoveredRuntime, recoveryEffects = ControlRuntime.update (RecoverStaleTarget "missing-button") focusedRuntime
let changed =
    { Kind = "changed"
      ControlId = Some "text-box"
      Origin = ControlEventOrigin.Text
      Payload = Some "Grace"
      Nav = None }

printfn "controls-node-count=%d" rendered.NodeCount
printfn "controls-diagnostics=%A" rendered.Diagnostics
printfn "controls-scene=%A" (Scene.describe rendered.Scene)
printfn "controls-catalog-count=%d" (Catalog.supportedCount ())
printfn "controls-chart-graph-datagrid=%A" (gridColumnsAttr.Name, Scene.describe rendered.Scene)
printfn "controls-text-dispatch=%A" (Control.dispatch changed root)
printfn "control-runtime-init-effects=%A" runtimeInitEffects
printfn "control-runtime-focus-effects=%A focused=%A" focusEffects focusedRuntime.FocusedControl
printfn "control-runtime-recovery-effects=%A diagnostics=%d" recoveryEffects recoveredRuntime.Diagnostics.Length

// 075 — pointer front door (pure, host-independent, replayable). Two side-by-side
// buttons; a scripted hover/click/drag/scroll sequence reduces deterministically.
let pointerLayout: LayoutResult =
    { Bounds =
        [ { NodeId = "buttonA"; Bounds = { X = 0.0; Y = 0.0; Width = 100.0; Height = 40.0 }; Visibility = Visible }
          { NodeId = "buttonB"; Bounds = { X = 100.0; Y = 0.0; Width = 100.0; Height = 40.0 }; Visibility = Visible } ]
      Diagnostics = []
      Invalidated = []
      Revision = 0L }

let pointerPolicy: PixelSnapPolicy = Defaults.pixelSnapPolicy 1.0

let pointerScript =
    [ Move(50.0, 20.0) // hover A
      Move(150.0, 20.0) // leave A, enter B
      Down(PointerButton.Primary, 150.0, 20.0)
      Up(PointerButton.Primary, 150.0, 20.0) // click B
      Down(PointerButton.Primary, 50.0, 20.0)
      Move(50.0, 30.0) // drag begin (past 4px threshold)
      Move(50.0, 38.0) // drag move
      Up(PointerButton.Primary, 50.0, 38.0) // drag end
      WheelMsg(0.0, -3.0, 150.0, 20.0) ] // scroll B

let pointerFinal, pointerEffects = Pointer.replay pointerPolicy pointerLayout pointerScript (Pointer.init ())
let pointerReplayAgain = snd (Pointer.replay pointerPolicy pointerLayout pointerScript (Pointer.init ()))

printfn "pointer-origin=%A" Pointer.origin
printfn "pointer-effects=%A" pointerEffects
printfn "pointer-deterministic=%b" (pointerEffects = pointerReplayAgain)
printfn "pointer-final-hover=%A presses=%d" pointerFinal.Hover pointerFinal.Presses.Count
