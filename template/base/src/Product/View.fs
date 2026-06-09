module Product.View

open FS.Skia.UI.Scene
open Product.Model
//#if (profile == "governed" || profile == "headless-scene")

let view model =
    let textColor = { Red = 240uy; Green = 240uy; Blue = 240uy; Alpha = 255uy }

    Group(
        [ { Nodes = [ Rectangle((16.0, 16.0, 288.0, 128.0), { Red = 24uy; Green = 32uy; Blue = 44uy; Alpha = 255uy }) ] }
          { Nodes = [ Text((32.0, 56.0), $"Governed headless scene: {model.Name}", textColor) ] }
          { Nodes = [ Text((32.0, 88.0), $"renders: {model.RenderCount}", textColor) ] } ]
    )

//#else
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Elmish
open FS.Skia.UI.KeyboardInput
open FS.Skia.UI.Scene

let visibleRows model =
    { FirstIndex = 0
      Count = model.GridRows.Length
      Total = model.GridRows.Length }

let controlsExampleView model =
    Stack.create [
        Stack.children [
            TextBlock.create [ TextBlock.text "Product controls" ]
            RichText.create model.RichIntro []
            TextBox.create [
                TextBox.value model.Name
                TextBox.onChanged NameChanged
            ]
            Button.create [
                Button.text "Save"
                Button.enabled model.CanSave
                Button.onClick SaveRequested
            ]
            |> Control.withKey "save"
            LineChart.create [ LineChart.series model.Revenue ]
            GraphView.create [ GraphView.nodes [ "form"; "chart"; "grid" ] ]
            DataGrid.create model.GridColumns [
                gridColumnsAttr
                DataGrid.rows model.GridRows
                DataGrid.visibleRange (visibleRows model)
                DataGrid.selectedRows Set.empty
                DataGrid.focusedCell None
                Attr.width 360.0
                Attr.height 132.0
            ]
        ]
    ]

let adapterProgram =
    ControlsElmish.program Product.Model.init Product.Model.update controlsExampleView Product.Model.subscriptions

// The default scaffold `view` rasterizes the REAL example control tree through the
// production tree-render path (`Control.renderTree`) at the output extent, so the
// unmodified generated app shows actual styled controls — form, rich text, chart, graph,
// and DataGrid — laid out by the framework, not hand-drawn placeholder geometry (FR-003).
let contentArea: FS.Skia.UI.Scene.Size = { Width = 640; Height = 480 }

let view (model: Model) : SceneNode =
    let rendered = Control.renderTree Theme.light contentArea (controlsExampleView model)
    Group [ rendered.Scene ]

//#endif
