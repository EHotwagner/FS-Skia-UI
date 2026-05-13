namespace FS.Skia.UI.Charts

open FS.Skia.UI

type DataGridTarget =
    | Header of columnKey: string
    | Cell of rowIndex: int * columnKey: string

module DataGrid =
    val defaultConfig : width: float -> height: float -> DataGridConfig
    val dataGrid : config: DataGridConfig -> data: DataGridData -> viewport: DataGridViewport -> Scene
    val visibleRows : config: DataGridConfig -> totalRows: int -> scrollOffset: int -> DataGridViewport
    val sortRows : columns: ColumnDef list -> sort: SortState -> rows: Map<string, CellValue> list -> Map<string, CellValue> list
    val hitTest : config: DataGridConfig -> data: DataGridData -> viewport: DataGridViewport -> x: float -> y: float -> DataGridTarget option
