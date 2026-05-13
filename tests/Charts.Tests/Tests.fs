module ChartsTests

open System
open System.Diagnostics
open Expecto
open FS.Skia.UI
open FS.Skia.UI.Charts

let sampleConfig =
    { Defaults.chartConfig 640.0 360.0 with
        XAxis = { Defaults.axis with Title = Some "time"; Minimum = Some 0.0; Maximum = Some 10.0 }
        YAxis = { Defaults.axis with Title = Some "value"; Minimum = Some -5.0; Maximum = Some 20.0 }
        Legend = { Defaults.legend with Visible = true; Position = "bottom" } }

let sampleColumns =
    [ { Key = "name"; Header = "Name"; ColumnType = Text; Width = Some 160.0 }
      { Key = "score"; Header = "Score"; ColumnType = Numeric; Width = Some 88.0 }
      { Key = "active"; Header = "Active"; ColumnType = Boolean; Width = None } ]

let row name score active =
    [ "name", TextValue name
      "score", NumericValue score
      "active", BooleanValue active ]
    |> Map.ofList

[<Tests>]
let contractTests =
    testList "Charts contract" [
        test "default chart config is constructible" {
            let config = Defaults.chartConfig 640.0 360.0
            Expect.equal config.Area.Width 640.0 "width is retained"
            Expect.equal config.Area.Height 360.0 "height is retained"
            Expect.isTrue config.XAxis.ShowGrid "x axis grid defaults on"
            Expect.isTrue config.YAxis.ShowGrid "y axis grid defaults on"
            Expect.isTrue config.Legend.Visible "legend defaults visible"
            Expect.isNonEmpty config.Palette.Colors "default palette contains colors"
        }

        test "axis legend and palette configuration records are immutable public props" {
            let customColor = Colors.rgba 12uy 34uy 56uy 255uy
            let config = { sampleConfig with Palette = { Colors = [ customColor ] } }

            Expect.equal config.XAxis.Title (Some "time") "x axis title is retained"
            Expect.equal config.XAxis.Minimum (Some 0.0) "x axis minimum is retained"
            Expect.equal config.YAxis.Maximum (Some 20.0) "y axis maximum is retained"
            Expect.equal config.Legend.Position "bottom" "legend position is retained"
            Expect.equal config.Palette.Colors [ customColor ] "custom palette is retained"
        }

        test "scale helpers ignore invalid values and project stable coordinates" {
            let minimum, maximum = Scale.bounds [ Double.NaN; Double.NegativeInfinity; 4.0; 12.0; Double.PositiveInfinity; -2.0 ]

            Expect.equal minimum -2.0 "finite minimum is used"
            Expect.equal maximum 12.0 "finite maximum is used"
            Expect.floatClose Accuracy.medium (Scale.project -2.0 12.0 100.0 240.0 5.0) 170.0 "value projects into target range"
            Expect.equal (Scale.project 4.0 4.0 10.0 99.0 4.0) 10.0 "flat domain projects to low coordinate"
        }

        test "empty and invalid chart data remains a pure scene element" {
            let emptySeries = [ { Name = "empty"; Points = []; Color = None } ]
            let invalidSeries =
                [ { Name = "invalid"
                    Points =
                        [ { X = 0.0; Y = Double.NaN; Label = None }
                          { X = 1.0; Y = Double.PositiveInfinity; Label = Some "bad" } ]
                    Color = None } ]

            Expect.contains (Scene.describe (LineChart.lineChart sampleConfig emptySeries)) ChartElement "empty line chart returns a chart scene"
            Expect.contains (Scene.describe (BarChart.barChart sampleConfig invalidSeries)) ChartElement "invalid values remain declarative chart data"
            Expect.equal (Scale.bounds (invalidSeries |> List.collect (fun series -> series.Points |> List.map _.Y))) (0.0, 0.0) "all-invalid bounds collapse safely"
            Expect.equal (LineChart.hitTest sampleConfig emptySeries 4.0 4.0) None "empty hit-test has no target"
        }

        test "line bar pie scatter area histogram candlestick and radar charts return pure scene elements" {
            let series =
                [ { Name = "alpha"
                    Points =
                        [ for index in 0 .. 11 ->
                              { X = float index
                                Y = float (index * index % 17)
                                Label = Some $"P{index}" } ]
                    Color = Some (Colors.rgba 64uy 128uy 220uy 255uy) } ]

            let pieValues =
                [ { X = 0.0; Y = 4.0; Label = Some "A" }
                  { X = 1.0; Y = 6.0; Label = Some "B" } ]

            let ohlc =
                [ for index in 0 .. 7 ->
                      { X = float index
                        Open = 10.0 + float index
                        High = 13.0 + float index
                        Low = 8.0 + float index
                        Close = 11.0 + float index } ]

            let scenes =
                [ LineChart.lineChart sampleConfig series
                  BarChart.barChart sampleConfig series
                  PieChart.pieChart sampleConfig pieValues
                  PieChart.donutChart sampleConfig 28.0 pieValues
                  ScatterPlot.scatterPlot sampleConfig series
                  AreaChart.areaChart sampleConfig false series
                  Histogram.histogram sampleConfig 4 [ 1.0; 2.0; 2.5; 4.0; 8.0 ]
                  Candlestick.candlestick sampleConfig ohlc
                  RadarChart.radarChart sampleConfig series ]

            scenes
            |> List.iter (fun scene -> Expect.contains (Scene.describe scene) ChartElement "chart builder returns a chart scene element")

            let bins = Histogram.bins 4 [ 0.0; 1.0; 2.0; 3.0; 4.0 ]
            Expect.equal bins.Length 4 "histogram creates requested bins"
            Expect.equal (bins |> List.sumBy (fun (_, _, count) -> count)) 5 "histogram bins preserve item count"
        }

        test "line chart accepts one hundred thousand points within the scale budget" {
            let points =
                [ for index in 0 .. 99_999 ->
                      { X = float index
                        Y = sin (float index / 100.0)
                        Label = None } ]

            let series = [ { Name = "large"; Points = points; Color = None } ]
            let stopwatch = Stopwatch.StartNew()
            let scene = LineChart.lineChart sampleConfig series
            stopwatch.Stop()

            Expect.contains (Scene.describe scene) ChartElement "large line chart remains a scene element"
            Expect.isLessThan stopwatch.ElapsedMilliseconds 2000L "100,000 point scene projection stays under two seconds"
        }

        test "data grid columns cells fixed headers sorting and viewport math are semantic" {
            let config = Defaults.dataGridConfig 480.0 260.0
            let rows = [ row "delta" 4.0 true; row "alpha" 10.0 false; row "charlie" 3.0 true ]
            let data = { Columns = sampleColumns; Rows = rows }
            let viewport = DataGrid.visibleRows config rows.Length 1
            let sorted = DataGrid.sortRows sampleColumns { ColumnKey = "name"; Direction = Ascending } rows
            let sortedDescending = DataGrid.sortRows sampleColumns { ColumnKey = "score"; Direction = Descending } rows

            Expect.isTrue config.FixedHeader "fixed header defaults on"
            Expect.equal config.HeaderHeight 28.0 "header height is retained"
            Expect.equal viewport.FirstRow 1 "scroll offset maps to first row"
            Expect.equal viewport.RowCount 2 "visible rows clamp to remaining rows"
            Expect.equal (sorted.Head |> Map.find "name") (TextValue "alpha") "text sort orders ascending"
            Expect.equal (sortedDescending.Head |> Map.find "score") (NumericValue 10.0) "numeric sort uses numeric values"
            Expect.equal sampleColumns[0].Width (Some 160.0) "explicit column width is retained"
            Expect.equal sampleColumns[2].Width None "auto-width column is retained"
            Expect.contains (Scene.describe (DataGrid.dataGrid config data viewport)) ChartElement "data grid returns a scene element"
            Expect.equal (DataGrid.hitTest config data viewport 20.0 10.0) (Some(Header "name")) "header hit-test returns column key"
            Expect.equal (DataGrid.hitTest config data viewport 20.0 58.0) (Some(Cell(2, "name"))) "cell hit-test returns absolute row and column"
            Expect.equal (DataGrid.hitTest config data viewport 900.0 58.0) None "outside grid hit-test returns no target"
        }

        test "data grid projects ten thousand rows within the scale budget" {
            let config = Defaults.dataGridConfig 800.0 420.0
            let rows = [ for index in 0 .. 9_999 -> row $"row-{index:D5}" (float index) (index % 2 = 0) ]
            let data = { Columns = sampleColumns; Rows = rows }
            let viewport = DataGrid.visibleRows config rows.Length 250
            let stopwatch = Stopwatch.StartNew()
            let scene = DataGrid.dataGrid config data viewport
            stopwatch.Stop()

            Expect.equal viewport.FirstRow 250 "large viewport starts at requested row"
            Expect.isGreaterThan viewport.RowCount 0 "large viewport contains visible rows"
            Expect.contains (Scene.describe scene) ChartElement "large grid remains a scene element"
            Expect.isLessThan stopwatch.ElapsedMilliseconds 2000L "10,000 row scene projection stays under two seconds"
        }

        test "charts and data grid compose as pure scene elements inside larger core scenes" {
            let series =
                [ { Name = "values"
                    Points = [ for index in 0 .. 4 -> { X = float index; Y = float (index + 1); Label = None } ]
                    Color = None } ]

            let gridData =
                { Columns = sampleColumns
                  Rows = [ row "alpha" 1.0 true; row "beta" 2.0 false ] }

            let chartScene = LineChart.lineChart sampleConfig series
            let gridScene = DataGrid.dataGrid (Defaults.dataGridConfig 360.0 160.0) gridData { FirstRow = 0; RowCount = 2 }

            let composed =
                Scene.group [
                    Scene.rectangle (0.0, 0.0, 720.0, 420.0) (Colors.rgba 18uy 24uy 32uy 255uy)
                    Scene.text (24.0, 40.0) "Composed data view" Colors.white
                    chartScene
                    gridScene
                ]

            let kinds = Scene.describe composed
            Expect.contains kinds GroupElement "composition is an ordinary scene group"
            Expect.contains kinds RectangleElement "core scene content is retained"
            Expect.contains kinds TextElement "core text content is retained"
            Expect.equal (kinds |> List.filter ((=) ChartElement) |> List.length) 2 "chart and grid are both pure scene elements"
        }

        test "chart hit-test projection helpers return nearest public targets" {
            let config =
                { sampleConfig with
                    Area = { X = 0.0; Y = 0.0; Width = 100.0; Height = 100.0 }
                    XAxis = { sampleConfig.XAxis with Minimum = Some 0.0; Maximum = Some 10.0 }
                    YAxis = { sampleConfig.YAxis with Minimum = Some 0.0; Maximum = Some 10.0 } }

            let series =
                [ { Name = "alpha"
                    Points =
                        [ { X = 0.0; Y = 0.0; Label = None }
                          { X = 5.0; Y = 5.0; Label = Some "center" }
                          { X = 10.0; Y = 10.0; Label = None } ]
                    Color = None } ]

            Expect.equal (LineChart.hitTest config series 50.0 50.0) (Some(Point("alpha", 1))) "line hit-test returns nearest point"
            Expect.equal (ScatterPlot.hitTest config series 150.0 50.0) None "outside chart area has no target"
            Expect.isSome (PieChart.hitTest config [ { X = 0.0; Y = 3.0; Label = Some "slice" } ] 50.0 50.0) "pie hit-test returns a slice target"
        }
    ]
