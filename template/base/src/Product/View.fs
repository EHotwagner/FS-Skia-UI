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

let private boardLayout (size: FS.Skia.UI.Scene.Size) =
    let cell = min ((float size.Width - 240.0) / 10.0) ((float size.Height - 120.0) / 20.0)
    let cell = max 12.0 cell
    let boardWidth = cell * 10.0
    let boardHeight = cell * 20.0
    let x = 32.0
    let y = 96.0
    x, y, cell, boardWidth, boardHeight

let view (model: Model) =
    let outputSize: FS.Skia.UI.Scene.Size = { Width = 640; Height = 480 }
    let boardX, boardY, cell, boardWidth, boardHeight = boardLayout outputSize
    let boardColor = { Red = 18uy; Green = 24uy; Blue = 32uy; Alpha = 255uy }
    let gridColor = { Red = 72uy; Green = 82uy; Blue = 96uy; Alpha = 255uy }
    let activeColor = { Red = 64uy; Green = 196uy; Blue = 255uy; Alpha = 255uy }
    let textColor = { Red = 240uy; Green = 240uy; Blue = 240uy; Alpha = 255uy }
    let linePaint = Paint.stroke gridColor 1.0

    let activeCells =
        [ model.ActiveColumn, model.ActiveRow
          model.ActiveColumn + 1, model.ActiveRow
          model.ActiveColumn, model.ActiveRow + 1
          model.ActiveColumn + 1, model.ActiveRow + 1 ]

    let settledCells =
        [ for row in 15..19 do
              for column in 0..3 do
                  column, row ]

    let boardCells =
        (activeCells @ settledCells)
        |> List.map (fun (column, row) ->
            Rectangle(
                (boardX + float column * cell + 1.0, boardY + float row * cell + 1.0, cell - 2.0, cell - 2.0),
                activeColor
            )
            |> fun node -> { Nodes = [ node ] })

    let gridLines =
        [ for column in 0..10 ->
              let x = boardX + float column * cell
              { Nodes = [ Line({ X = x; Y = boardY }, { X = x; Y = boardY + boardHeight }, linePaint) ] }
          for row in 0..20 ->
              let y = boardY + float row * cell
              { Nodes = [ Line({ X = boardX; Y = y }, { X = boardX + boardWidth; Y = y }, linePaint) ] } ]

    let sideX = boardX + boardWidth + 32.0
    let sideInfo =
        [ Text((sideX, boardY + 24.0), $"score: {model.Score}", textColor)
          Text((sideX, boardY + 52.0), $"level: {model.Level}", textColor)
          Text((sideX, boardY + 80.0), $"next: {model.NextPiece}", textColor)
          Text((sideX, boardY + 116.0), $"screen: {screenName model.Screen}", textColor)
          Text((sideX, boardY + 144.0), $"moves: {model.PrimaryInteractions}", textColor) ]
        |> List.map (fun node -> { Nodes = [ node ] })

    let circularMarkers =
        [ Circle({ X = sideX + 18.0; Y = boardY + 186.0 }, 10.0, activeColor)
          Circle({ X = sideX + 48.0; Y = boardY + 186.0 }, 8.0, textColor)
          FilledEllipse({ X = sideX + 72.0; Y = boardY + 176.0; Width = 34.0; Height = 20.0 }, gridColor) ]
        |> List.map (fun node -> { Nodes = [ node ] })

    Group(
        [ yield { Nodes = [ Rectangle((boardX, boardY, boardWidth, boardHeight), boardColor) ] }
          yield! boardCells
          yield! gridLines
          yield! sideInfo
          yield! circularMarkers ]
    )

//#endif
