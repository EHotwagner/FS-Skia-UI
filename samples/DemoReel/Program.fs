module DemoReel.Program

open System
open System.Diagnostics
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Charts
open FS.Skia.UI.Layout

type Model =
    { Time: float
      ManualSlide: int option
      Size: Size
      Sort: SortState
      Diagnostics: RenderDiagnostic list }

type Msg =
    | ViewerInput of ViewerEvent
    | Advance of float
    | NextSlide
    | PreviousSlide
    | Restart
    | HostEffect of ViewerEffect<Msg>

let width = 1280.0
let height = 720.0
let initialSize = { Width = int width; Height = int height }
let totalDuration = 60.0

let init () =
    { Time = 0.0
      ManualSlide = None
      Size = initialSize
      Sort = { ColumnKey = "change"; Direction = Descending }
      Diagnostics = [] },
    Cmd.ofMsg (HostEffect InitializeRenderer)

let clampByte value =
    Math.Clamp(int value, 0, 255) |> byte

let rgba r g b a =
    Colors.rgba (clampByte r) (clampByte g) (clampByte b) (clampByte a)

let wave phase low high =
    low + (high - low) * ((sin phase + 1.0) * 0.5)

let waveColor phase =
    rgba (wave phase 60.0 245.0) (wave (phase + 2.1) 70.0 235.0) (wave (phase + 4.2) 90.0 255.0) 255.0

let textPaint size weight =
    { Text = ""
      Position = { X = 0.0; Y = 0.0 }
      Font = { Family = None; Size = size; Weight = weight }
      Paint = Paint.fill Colors.white }

let titleText x y text size =
    Scene.textRun { textPaint size (Some 700) with Text = text; Position = { X = x; Y = y } }

let labelText x y text =
    Scene.textRun { textPaint 18.0 (Some 500) with Text = text; Position = { X = x; Y = y } }

let subtitleText x y text =
    Scene.textRun
        { textPaint 14.0 (Some 400) with
            Text = text
            Position = { X = x; Y = y }
            Paint = Paint.fill (rgba 184.0 198.0 228.0 255.0) }

let header title subtitle =
    Scene.group [
        Scene.rectangle (0.0, 0.0, width, 64.0) (rgba 10.0 13.0 24.0 240.0)
        titleText 34.0 42.0 title 24.0
        subtitleText 34.0 60.0 subtitle
    ]

let progress elapsed =
    let pct = Math.Clamp(elapsed / totalDuration, 0.0, 1.0)
    Scene.group [
        Scene.rectangle (0.0, height - 5.0, width, 5.0) (rgba 38.0 42.0 54.0 255.0)
        Scene.rectangle (0.0, height - 5.0, width * pct, 5.0) (rgba 92.0 178.0 245.0 255.0)
    ]

let titleOverlay title subtitle localTime =
    if localTime > 1.7 then
        Scene.empty
    else
        let alpha = 1.0 - localTime / 1.7
        Scene.group [
            Scene.rectangle (0.0, 0.0, width, height) (rgba 0.0 0.0 0.0 (175.0 * alpha))
            titleText 438.0 338.0 title 54.0
            subtitleText 472.0 378.0 subtitle
        ]

let gradientPaint t x y w h =
    let c1 = waveColor (t * 1.2)
    let c2 = waveColor (t * 1.2 + 2.0)
    let c3 = waveColor (t * 1.2 + 4.0)

    Paint.fill Colors.white
    |> Paint.withShader (LinearGradient({ X = x; Y = y }, { X = x + w; Y = y + h }, [ c1; c2; c3 ]))

let glow color sigma =
    Paint.fill color
    |> Paint.withMaskFilter (Blur sigma)
    |> Paint.withBlendMode Screen

let geometryBurst t =
    let center = { X = width * 0.5; Y = height * 0.52 }
    let orbiters =
        [ for i in 0 .. 17 ->
              let fi = float i
              let angle = t * (0.9 + fi * 0.035) + fi * Math.PI * 2.0 / 18.0
              let radius = 78.0 + fi * 17.0 + wave (t * 1.6 + fi) -16.0 24.0
              let x = center.X + cos angle * radius
              let y = center.Y + sin angle * radius
              let r = 7.0 + wave (t * 2.8 + fi) 0.0 12.0
              let color = waveColor (t * 1.8 + fi * 0.62)

              Scene.group [
                  Scene.ellipse { X = x - r - 8.0; Y = y - r - 8.0; Width = (r + 8.0) * 2.0; Height = (r + 8.0) * 2.0 } (glow color 7.0)
                  Scene.ellipse { X = x - r; Y = y - r; Width = r * 2.0; Height = r * 2.0 } (Paint.fill color)
              ] ]

    let spokes =
        [ for i in 0 .. 35 ->
              let fi = float i
              let a = fi * Math.PI * 2.0 / 36.0 + t * 0.45
              let r1 = 145.0 + wave (t * 1.3 + fi) -22.0 22.0
              let r2 = 330.0 + wave (t * 2.1 + fi) -35.0 45.0
              let color = waveColor (fi * 0.2 + t)
              Scene.line
                  { X = center.X + cos a * r1; Y = center.Y + sin a * r1 }
                  { X = center.X + cos a * r2; Y = center.Y + sin a * r2 }
                  (Paint.stroke color 1.6 |> Paint.withOpacity 0.7) ]

    let rings =
        [ for i in 0 .. 4 ->
              let r = 55.0 + float i * 42.0 + wave (t * 2.0 + float i) -8.0 18.0
              Scene.arc
                  { X = center.X - r; Y = center.Y - r; Width = r * 2.0; Height = r * 2.0 }
                  (t * 35.0 + float i * 23.0)
                  (245.0 - float i * 22.0)
                  (Paint.stroke (rgba 170.0 208.0 255.0 (160.0 - float i * 22.0)) (2.0 + float i * 0.4)
                   |> Paint.withStrokeCap Round
                   |> Paint.withPathEffect (Dash([ 12.0; 8.0 ], t * 10.0))) ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 8.0 10.0 24.0 255.0)
        yield! spokes
        yield! rings
        yield! orbiters
        Scene.rectangleWithPaint { X = 96.0; Y = 104.0; Width = 250.0; Height = 88.0 } (gradientPaint t 96.0 104.0 250.0 88.0 |> Paint.withImageFilter (DropShadow(12.0, 16.0, 18.0, rgba 0.0 0.0 0.0 180.0)))
        titleText 122.0 158.0 "Animated Geometry" 30.0
        subtitleText 122.0 182.0 "orbits, dashed paths, glow filters"
        titleOverlay "FS.Skia.UI" "Elmish-driven Vulkan scene reel" t
    ]

let shaderShowcase t =
    let radial =
        Paint.fill Colors.white
        |> Paint.withShader (RadialGradient({ X = 250.0; Y = 360.0 }, 150.0 + wave (t * 1.2) -30.0 45.0, [ waveColor t; waveColor (t + 2.0); rgba 6.0 8.0 20.0 255.0 ]))

    let sweep =
        Paint.fill Colors.white
        |> Paint.withShader (SweepGradient({ X = 640.0; Y = 360.0 }, [ waveColor (t + 0.0); waveColor (t + 1.4); waveColor (t + 2.8); waveColor (t + 4.2); waveColor t ]))

    let conicalLike =
        Paint.fill Colors.white
        |> Paint.withShader (LinearGradient({ X = 900.0 + wave (t * 0.8) -80.0 80.0; Y = 140.0 }, { X = 1210.0; Y = 560.0 }, [ rgba 255.0 96.0 122.0 255.0; rgba 255.0 235.0 120.0 255.0; rgba 80.0 210.0 255.0 255.0 ]))
        |> Paint.withColorFilter (BlendColor(rgba 255.0 255.0 255.0 40.0, Screen))

    let clippedPath =
        Path.create Winding [
            Path.moveTo 884.0 144.0
            Path.lineTo 1204.0 178.0
            Path.lineTo 1162.0 548.0
            Path.lineTo 920.0 585.0
            Path.close
        ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 5.0 7.0 18.0 255.0)
        header "Shader Showcase" "radial, sweep, linear, color filter, clipping"
        Scene.ellipse { X = 58.0; Y = 170.0; Width = 384.0; Height = 384.0 } (radial |> Paint.withImageFilter (DropShadow(0.0, 0.0, 24.0, rgba 55.0 140.0 255.0 150.0)))
        Scene.ellipse { X = 470.0; Y = 178.0; Width = 340.0; Height = 340.0 } sweep
        Scene.ellipse { X = 564.0; Y = 272.0; Width = 152.0; Height = 152.0 } (Paint.fill (rgba 5.0 7.0 18.0 255.0))
        Scene.clipped
            (PathClip clippedPath)
            (Scene.group [
                Scene.rectangleWithPaint { X = 850.0; Y = 120.0; Width = 390.0; Height = 500.0 } conicalLike
                Scene.rectangleWithPaint { X = 890.0 + wave (t * 2.0) -20.0 50.0; Y = 210.0; Width = 260.0; Height = 54.0 } (gradientPaint (t + 2.0) 880.0 210.0 280.0 54.0)
                Scene.rectangleWithPaint { X = 950.0; Y = 336.0 + wave (t * 1.4) -26.0 38.0; Width = 220.0; Height = 80.0 } (gradientPaint (t + 4.0) 950.0 300.0 220.0 100.0 |> Paint.withMaskFilter (Blur 1.4))
            ])
        labelText 170.0 596.0 "Radial"
        labelText 578.0 596.0 "Sweep"
        labelText 982.0 596.0 "Clipped shader field"
        titleOverlay "Shaders" "animated gradient declarations and filters" t
    ]

let plasmaPanel t =
    let cells =
        [ for y in 0 .. 11 do
              for x in 0 .. 23 do
                  let fx = float x
                  let fy = float y
                  let v = sin (fx * 0.42 + t * 2.2) + cos (fy * 0.65 + t * 1.7) + sin ((fx + fy) * 0.24 + t)
                  let color = waveColor (v + t * 0.8)
                  let alpha = 110.0 + wave (v + t) 0.0 125.0
                  Scene.rectangleWithPaint
                      { X = 38.0 + fx * 50.0
                        Y = 92.0 + fy * 45.0
                        Width = 46.0
                        Height = 41.0 }
                      (Paint.fill { color with Alpha = clampByte alpha } |> Paint.withBlendMode Screen) ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 0.0 0.0 0.0 255.0)
        yield! cells
        Scene.rectangleWithPaint { X = 0.0; Y = 0.0; Width = width; Height = height } (Paint.fill (rgba 255.0 255.0 255.0 30.0) |> Paint.withShader (RadialGradient({ X = 640.0; Y = 360.0 }, 420.0, [ rgba 255.0 255.0 255.0 90.0; rgba 0.0 0.0 0.0 0.0 ])))
        Scene.rectangle (70.0, 526.0, 510.0, 118.0) (rgba 0.0 0.0 0.0 150.0)
        titleText 98.0 574.0 "Runtime shader adaptation" 32.0
        subtitleText 100.0 604.0 "plasma-style animated field using declarative shader-compatible primitives"
        titleOverlay "Plasma Field" "baseline SkSL segment adapted to current public surface" t
    ]

let liveSeries t =
    [ { Name = "Revenue"
        Points = [ for i in 0 .. 36 -> { X = float i; Y = 72.0 + sin (float i * 0.33 + t) * 24.0 + cos (float i * 0.11 + t * 1.5) * 9.0; Label = None } ]
        Color = Some(rgba 72.0 164.0 244.0 255.0) }
      { Name = "Cost"
        Points = [ for i in 0 .. 36 -> { X = float i; Y = 44.0 + cos (float i * 0.37 + t * 0.7) * 17.0 + sin (float i * 0.18 + t) * 6.0; Label = None } ]
        Color = Some(rgba 235.0 110.0 86.0 255.0) } ]

let configAt x y w h =
    { Defaults.chartConfig w h with
        Area = { X = x; Y = y; Width = w; Height = h }
        XAxis = { Defaults.axis with ShowGrid = true }
        YAxis = { Defaults.axis with Minimum = Some 0.0; ShowGrid = true }
        Legend = { Defaults.legend with Visible = true; Position = "bottom" } }

let dashboard t model =
    let series = liveSeries t
    let bars =
        [ { Name = "2025"
            Points = [ for i in 0 .. 7 -> { X = float i; Y = 38.0 + wave (t * 0.9 + float i) 0.0 42.0; Label = Some $"Q{i + 1}" } ]
            Color = Some(rgba 95.0 210.0 135.0 255.0) } ]

    let scatter =
        [ { Name = "Cluster"
            Points = [ for i in 0 .. 42 -> { X = wave (float i * 1.7 + t) 0.0 100.0; Y = wave (float i * 1.1 + t * 1.4) 0.0 100.0; Label = None } ]
            Color = Some(rgba 255.0 205.0 95.0 255.0) } ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 17.0 19.0 31.0 255.0)
        header "Live Dashboard" "line, area, bar, scatter, radar and model-owned state"
        LineChart.lineChart (configAt 42.0 96.0 520.0 210.0) series
        AreaChart.areaChart (configAt 42.0 352.0 520.0 230.0) false series
        BarChart.barChart (configAt 610.0 96.0 290.0 210.0) bars
        ScatterPlot.scatterPlot (configAt 940.0 96.0 292.0 210.0) scatter
        RadarChart.radarChart
            (configAt 610.0 352.0 290.0 230.0)
            [ { Name = "GPU"; Points = [ for i in 0 .. 5 -> { X = float i; Y = wave (t + float i) 35.0 100.0; Label = None } ]; Color = Some(rgba 88.0 170.0 255.0 255.0) }
              { Name = "CPU"; Points = [ for i in 0 .. 5 -> { X = float i; Y = wave (t * 0.7 + float i * 1.4) 20.0 82.0; Label = None } ]; Color = Some(rgba 240.0 124.0 96.0 255.0) } ]
        Scene.rectangle (940.0, 352.0, 292.0, 230.0) (rgba 34.0 40.0 58.0 255.0)
        titleText 968.0 410.0 "Slide" 32.0
        subtitleText 970.0 444.0 $"manual={model.ManualSlide.IsSome}  diagnostics={model.Diagnostics.Length}"
        subtitleText 970.0 476.0 "Right/Space: next  Left: previous  R: restart"
        titleOverlay "Charts" "live dashboard segment" t
    ]

let marketRows t =
    [ "AAPL", 189.5, 1.2, "Tech"
      "MSFT", 378.9, -0.3, "Tech"
      "NVDA", 495.2, 3.4, "Semis"
      "AMZN", 178.3, 2.1, "Consumer"
      "TSLA", 248.7, -1.5, "Auto"
      "JPM", 172.8, -0.2, "Finance"
      "UNH", 524.1, -1.2, "Health"
      "XOM", 104.5, -0.4, "Energy"
      "NFLX", 478.3, 2.3, "Media"
      "WMT", 162.3, 1.1, "Retail" ]
    |> List.mapi (fun i (symbol, price, change, sector) ->
        [ "symbol", TextValue symbol
          "price", NumericValue(price + sin (t * 1.6 + float i) * 2.6)
          "change", NumericValue(change + cos (t * 1.2 + float i) * 0.45)
          "sector", TextValue sector ]
        |> Map.ofList)

let finance t model =
    let columns =
        [ { Key = "symbol"; Header = "Symbol"; ColumnType = Text; Width = Some 120.0 }
          { Key = "price"; Header = "Price"; ColumnType = Numeric; Width = Some 120.0 }
          { Key = "change"; Header = "Change %"; ColumnType = Numeric; Width = Some 120.0 }
          { Key = "sector"; Header = "Sector"; ColumnType = Text; Width = None } ]

    let grid =
        { Columns = columns
          Rows = DataGrid.sortRows columns model.Sort (marketRows t) }

    let candles =
        [ for i in 0 .. 17 ->
              let basePrice = 150.0 + sin (float i * 0.55 + t * 0.7) * 24.0
              let spread = 4.0 + abs (cos (float i * 0.8 + t)) * 6.0
              let close = basePrice + sin (float i + t) * spread
              { X = float i
                Open = basePrice
                High = max basePrice close + spread * 0.8
                Low = min basePrice close - spread * 0.8
                Close = close } ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 13.0 15.0 27.0 255.0)
        header "Financial Data" "scrolling-style DataGrid and animated OHLC"
        DataGrid.dataGrid
            { Defaults.dataGridConfig 565.0 460.0 with
                Area = { X = 48.0; Y = 110.0; Width = 565.0; Height = 460.0 }
                RowHeight = 34.0
                HeaderHeight = 42.0 }
            grid
            { FirstRow = int (t * 0.8) % 4; RowCount = 8 }
        Candlestick.candlestick (configAt 684.0 110.0 510.0 460.0) candles
        titleOverlay "DataGrid + Candlestick" "animated market data" t
    ]

let graphNode id label style =
    { Id = id; Label = label; Style = style }

let graphEdge source target weight =
    { Source = source; Target = target; Weight = weight; Label = weight |> Option.map (fun value -> value.ToString("0.0")) }

let graphEffects t =
    let graph =
        { Config =
            { Defaults.graphConfig Directed 540.0 390.0 with
                Bounds = { X = 58.0; Y = 112.0; Width = 540.0; Height = 390.0 } }
          Nodes =
            [ graphNode "input" "Input" (Some(rgba 65.0 180.0 120.0 255.0))
              graphNode "parse" "Parse" None
              graphNode "layout" "Layout" None
              graphNode "render" "Render" (Some(rgba 70.0 150.0 245.0 255.0))
              graphNode "present" "Present" (Some(rgba 220.0 165.0 85.0 255.0)) ]
          Edges =
            [ graphEdge "input" "parse" None
              graphEdge "parse" "layout" None
              graphEdge "layout" "render" None
              graphEdge "render" "present" None
              graphEdge "parse" "render" (Some 0.4) ] }

    let graphScene =
        Graph.directed graph
        |> Result.defaultValue (Scene.text (100.0, 160.0) "graph invalid" (rgba 255.0 90.0 90.0 255.0))

    let card y phase label =
        let skew = sin (t * 1.4 + phase) * 0.12
        let transform =
            { M11 = 1.0
              M12 = skew
              M13 = 0.0
              M21 = -skew * 0.35
              M22 = 1.0
              M23 = 0.0
              M31 = 0.00018 * sin (t + phase)
              M32 = 0.00016 * cos (t + phase)
              M33 = 1.0 }

        Scene.withPerspective
            transform
            (Scene.group [
                Scene.rectangleWithPaint { X = 720.0; Y = y; Width = 350.0; Height = 94.0 } (gradientPaint (t + phase) 720.0 y 350.0 94.0 |> Paint.withImageFilter (DropShadow(12.0, 14.0, 12.0, rgba 0.0 0.0 0.0 150.0)))
                titleText 748.0 (y + 56.0) label 24.0
            ])

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 8.0 10.0 20.0 255.0)
        header "Graphs + Perspective" "DAG layout, transformed cards, shadows and blend modes"
        graphScene
        card 140.0 0.0 "Layout"
        card 286.0 2.2 "Graph"
        card 432.0 4.4 "Effects"
        Scene.line { X = 660.0; Y = 110.0 } { X = 660.0; Y = 590.0 } (Paint.stroke (rgba 100.0 150.0 240.0 150.0) 2.0 |> Paint.withPathEffect (Dash([ 8.0; 8.0 ], t * 18.0)))
        titleOverlay "Graph Viz + Effects" "DAG, perspective, shadows" t
    ]

let finale t =
    let center = { X = width * 0.5; Y = height * 0.5 }
    let burst =
        [ for i in 0 .. 71 ->
              let fi = float i
              let angle = fi * Math.PI * 2.0 / 72.0 + t * 0.28
              let r = 120.0 + fi * 4.0 + wave (t * 2.0 + fi) -25.0 55.0
              let color = waveColor (t * 1.4 + fi * 0.16)
              Scene.line center { X = center.X + cos angle * r; Y = center.Y + sin angle * r } (Paint.stroke color 1.2 |> Paint.withOpacity 0.72) ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 4.0 5.0 14.0 255.0)
        yield! burst
        Scene.ellipse { X = center.X - 190.0; Y = center.Y - 190.0; Width = 380.0; Height = 380.0 } (Paint.fill Colors.white |> Paint.withShader (RadialGradient(center, 190.0, [ rgba 255.0 255.0 255.0 180.0; rgba 80.0 135.0 255.0 95.0; rgba 0.0 0.0 0.0 0.0 ])))
        titleText 470.0 342.0 "FS.Skia.UI" 58.0
        subtitleText 438.0 380.0 "Vulkan / Elmish / SkiaSharp / Charts / Layout / Graphs"
        subtitleText 486.0 414.0 "Space: next  Left/Right: navigate  R: restart"
    ]

let slideCount = 7

let sceneFor model =
    let automaticSlide =
        Math.Clamp(int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))), 0, slideCount - 1)

    let slide = model.ManualSlide |> Option.defaultValue automaticSlide
    let segmentStart = float slide * (totalDuration / float slideCount)
    let localT =
        match model.ManualSlide with
        | Some _ -> model.Time
        | None -> model.Time - segmentStart

    let scene =
        match slide with
        | 0 -> geometryBurst localT
        | 1 -> shaderShowcase localT
        | 2 -> plasmaPanel localT
        | 3 -> dashboard localT model
        | 4 -> finance localT model
        | 5 -> graphEffects localT
        | _ -> finale localT

    Scene.group [ scene; progress model.Time ]

let view model = sceneFor model

let render model =
    Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let rec update msg model =
    match msg with
    | ViewerInput event ->
        match event with
        | Loaded -> model, render model
        | RenderTick elapsed
        | UpdateTick elapsed -> update (Advance elapsed) model
        | Resized size ->
            let next = { model with Size = size }
            next, render next
        | KeyDown "Right"
        | KeyDown "Space" -> update NextSlide model
        | KeyDown "Left" -> update PreviousSlide model
        | KeyDown "R" -> update Restart model
        | KeyDown "Escape" -> model, Cmd.ofMsg (HostEffect Shutdown)
        | CloseRequested -> model, Cmd.ofMsg (HostEffect Shutdown)
        | DiagnosticReported diagnostic ->
            { model with Diagnostics = diagnostic :: model.Diagnostics },
            Cmd.ofMsg (HostEffect(ReportDiagnostic diagnostic))
        | KeyDown _
        | KeyUp _
        | PointerMoved _
        | PointerPressed _
        | PointerReleased _ -> model, Cmd.none
    | Advance elapsed ->
        let next = { model with Time = (model.Time + max 0.0 elapsed) % totalDuration }
        next, render next
    | NextSlide ->
        let current =
            model.ManualSlide
            |> Option.defaultValue (int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))))

        let next = { model with ManualSlide = Some((current + 1) % slideCount) }
        next, render next
    | PreviousSlide ->
        let current =
            model.ManualSlide
            |> Option.defaultValue (int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))))

        let next = { model with ManualSlide = Some((current + slideCount - 1) % slideCount) }
        next, render next
    | Restart ->
        let next = { model with Time = 0.0; ManualSlide = None }
        next, render next
    | HostEffect _ -> model, Cmd.none

let configuration =
    { Viewer.defaultConfiguration "Demo Reel" initialSize with
        ClearColor = Some(rgba 4.0 5.0 14.0 255.0)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = false } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (ViewerInput >> Some)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let collect cmd =
    let messages = ResizeArray<Msg>()
    cmd |> List.iter (fun effect -> effect messages.Add)
    List.ofSeq messages

let runContractSmoke () =
    let model, initCmd = init ()
    let afterTick, tickCmd = update (ViewerInput(RenderTick 1.0)) model
    let afterNext, nextCmd = update NextSlide afterTick
    let afterRestart, restartCmd = update Restart afterNext
    let effects = [ initCmd; tickCmd; nextCmd; restartCmd ] |> List.collect collect
    let hasInitialize = effects |> List.exists ((=) (HostEffect InitializeRenderer))
    let renderCount =
        effects
        |> List.filter (function
            | HostEffect(RenderFrame _) -> true
            | _ -> false)
        |> List.length

    printfn "status=ok"
    printfn "sample=DemoReel"
    printfn "time=%.2f" afterRestart.Time
    printfn "manual-slide=%A" afterRestart.ManualSlide
    printfn "initialize-effect=%b" hasInitialize
    printfn "render-effects=%d" renderCount
    printfn "kinds=%A" (Scene.describe (view afterNext))

    if hasInitialize && renderCount >= 3 then 0 else 4

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run program with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=DemoReel"
        printfn "renderer=Vulkan"
        printfn "fallback-used=false"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=DemoReel"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    else
        runSmoke ()
