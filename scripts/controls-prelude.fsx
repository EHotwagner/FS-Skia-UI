#r "../src/Scene/bin/Debug/net10.0/FS.Skia.UI.Scene.dll"
#r "../src/KeyboardInput/bin/Debug/net10.0/FS.Skia.UI.KeyboardInput.dll"
#r "../src/Layout/bin/Debug/net10.0/FS.Skia.UI.Layout.dll"
#r "../src/Controls/bin/Debug/net10.0/FS.Skia.UI.Controls.dll"

open FS.Skia.UI.Scene
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
      Payload = Some "Grace" }

printfn "controls-node-count=%d" rendered.NodeCount
printfn "controls-diagnostics=%A" rendered.Diagnostics
printfn "controls-scene=%A" (Scene.describe rendered.Scene)
printfn "controls-catalog-count=%d" (Catalog.supportedCount ())
printfn "controls-chart-graph-datagrid=%A" (gridColumnsAttr.Name, Scene.describe rendered.Scene)
printfn "controls-text-dispatch=%A" (Control.dispatch changed root)
printfn "control-runtime-init-effects=%A" runtimeInitEffects
printfn "control-runtime-focus-effects=%A focused=%A" focusEffects focusedRuntime.FocusedControl
printfn "control-runtime-recovery-effects=%A diagnostics=%d" recoveryEffects recoveredRuntime.Diagnostics.Length
