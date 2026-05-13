namespace FS.Skia.UI.Charts

open System
open FS.Skia.UI

type ChartArea =
    { X: float
      Y: float
      Width: float
      Height: float }

type AxisConfig =
    { Title: string option
      Minimum: float option
      Maximum: float option
      ShowGrid: bool }

type LegendConfig =
    { Visible: bool
      Position: string }

type Palette =
    { Colors: Color list }

type ChartConfig =
    { Area: ChartArea
      XAxis: AxisConfig
      YAxis: AxisConfig
      Legend: LegendConfig
      Palette: Palette }

type DataPoint =
    { X: float
      Y: float
      Label: string option }

type DataSeries =
    { Name: string
      Points: DataPoint list
      Color: Color option }

type ChartTarget =
    | Series of seriesName: string
    | Point of seriesName: string * index: int
    | LegendItem of seriesName: string

type ColumnType =
    | Text
    | Numeric
    | Boolean

type ColumnDef =
    { Key: string
      Header: string
      ColumnType: ColumnType
      Width: float option }

type CellValue =
    | TextValue of string
    | NumericValue of float
    | BooleanValue of bool
    | Empty

type SortDirection =
    | Ascending
    | Descending

type SortState =
    { ColumnKey: string
      Direction: SortDirection }

type DataGridConfig =
    { Area: ChartArea
      HeaderHeight: float
      RowHeight: float
      FixedHeader: bool }

type DataGridData =
    { Columns: ColumnDef list
      Rows: Map<string, CellValue> list }

type DataGridViewport =
    { FirstRow: int
      RowCount: int }

module Defaults =
    let axis =
        { Title = None
          Minimum = None
          Maximum = None
          ShowGrid = true }

    let legend =
        { Visible = true
          Position = "right" }

    let palette =
        { Colors =
            [ Colors.rgba 64uy 128uy 220uy 255uy
              Colors.rgba 222uy 92uy 96uy 255uy
              Colors.rgba 80uy 170uy 120uy 255uy
              Colors.rgba 238uy 184uy 80uy 255uy ] }

    let chartConfig width height =
        { Area = { X = 0.0; Y = 0.0; Width = width; Height = height }
          XAxis = axis
          YAxis = axis
          Legend = legend
          Palette = palette }

    let dataGridConfig width height =
        { Area = { X = 0.0; Y = 0.0; Width = width; Height = height }
          HeaderHeight = 28.0
          RowHeight = 24.0
          FixedHeader = true }

module Scale =
    let bounds values =
        let values = values |> Seq.filter Double.IsFinite |> Seq.toArray

        if values.Length = 0 then
            0.0, 0.0
        else
            Array.min values, Array.max values

    let project (minimum: float) (maximum: float) (low: float) (high: float) (value: float) =
        if maximum = minimum then
            low
        else
            low + ((value - minimum) / (maximum - minimum) * (high - low))

module ChartHelpers =
    let isInside (area: ChartArea) x y =
        x >= area.X
        && y >= area.Y
        && x <= area.X + area.Width
        && y <= area.Y + area.Height

    let finitePoints (series: DataSeries list) =
        series
        |> List.map (fun item ->
            { item with
                Points = item.Points |> List.filter (fun point -> Double.IsFinite point.X && Double.IsFinite point.Y) })

    let finiteValues (values: float list) =
        values |> List.filter Double.IsFinite

    let yValues (series: DataSeries list) =
        series |> finitePoints |> List.collect (fun item -> item.Points |> List.map _.Y)

    let yRange (config: ChartConfig) series =
        let minValue, maxValue = Scale.bounds (yValues series)
        config.YAxis.Minimum |> Option.defaultValue minValue, config.YAxis.Maximum |> Option.defaultValue maxValue

    let xRange (config: ChartConfig) series =
        let points = series |> finitePoints |> List.collect (fun item -> item.Points)
        let minValue, maxValue = points |> List.map _.X |> Scale.bounds
        config.XAxis.Minimum |> Option.defaultValue minValue, config.XAxis.Maximum |> Option.defaultValue maxValue

    let projectPoint (config: ChartConfig) xRange yRange point =
        let x = Scale.project (fst xRange) (snd xRange) config.Area.X (config.Area.X + config.Area.Width) point.X
        let y = Scale.project (fst yRange) (snd yRange) (config.Area.Y + config.Area.Height) config.Area.Y point.Y
        x, y

    let hitTestSeries (config: ChartConfig) series x y =
        if not (isInside config.Area x y) then
            None
        else
            let cleaned = finitePoints series
            let xRange = xRange config cleaned
            let yRange = yRange config cleaned

            cleaned
            |> List.collect (fun item ->
                item.Points
                |> List.mapi (fun index point ->
                    let px, py = projectPoint config xRange yRange point
                    let distance = sqrt ((px - x) ** 2.0 + (py - y) ** 2.0)
                    distance, Point(item.Name, index)))
            |> List.sortBy fst
            |> List.tryHead
            |> Option.bind (fun (distance, target) -> if distance <= 16.0 then Some target else None)

    let hitTestPie (config: ChartConfig) values x y =
        let centerX = config.Area.X + config.Area.Width / 2.0
        let centerY = config.Area.Y + config.Area.Height / 2.0
        let radius = min config.Area.Width config.Area.Height / 2.0
        let distance = sqrt ((x - centerX) ** 2.0 + (y - centerY) ** 2.0)

        if distance > radius then
            None
        else
            values
            |> List.filter (fun point -> Double.IsFinite point.Y && point.Y > 0.0)
            |> List.mapi (fun index point -> point.Y, Point(point.Label |> Option.defaultValue "values", index))
            |> List.sortByDescending fst
            |> List.tryHead
            |> Option.map snd
