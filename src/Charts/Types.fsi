namespace FS.Skia.UI.Charts

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
    val axis : AxisConfig
    val legend : LegendConfig
    val palette : Palette
    val chartConfig : width: float -> height: float -> ChartConfig
    val dataGridConfig : width: float -> height: float -> DataGridConfig

module Scale =
    val bounds : values: float seq -> float * float
    val project : minimum: float -> maximum: float -> low: float -> high: float -> value: float -> float

module ChartHelpers =
    val finitePoints : series: DataSeries list -> DataSeries list
    val finiteValues : values: float list -> float list
    val yValues : series: DataSeries list -> float list
    val hitTestSeries : config: ChartConfig -> series: DataSeries list -> x: float -> y: float -> ChartTarget option
    val hitTestPie : config: ChartConfig -> values: DataPoint list -> x: float -> y: float -> ChartTarget option
