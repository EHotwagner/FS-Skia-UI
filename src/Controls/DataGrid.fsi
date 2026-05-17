namespace FS.Skia.UI.Controls

type DataGridColumnType =
    | TextColumn
    | NumericColumn
    | BooleanColumn
    | CustomColumn of string

type DataGridColumn =
    { Key: string
      Header: string
      Width: float
      ColumnType: DataGridColumnType }

type DataGridCell =
    { RowKey: string
      ColumnKey: string
      Value: string }

type DataGridRow =
    { Key: string
      Cells: DataGridCell list }

type DataGridSortDirection =
    | Ascending
    | Descending

type DataGridSort =
    { ColumnKey: string
      Direction: DataGridSortDirection }

type DataGridFocusedCell =
    { RowKey: string
      ColumnKey: string }

type DataGridModel =
    { ControlId: ControlId
      Columns: DataGridColumn list
      RowCount: int
      RowHeight: float
      ViewportHeight: float
      VisibleRange: VisibleRange
      SelectedRows: Set<string>
      FocusedCell: DataGridFocusedCell option
      Sort: DataGridSort option
      FilterText: string option
      Diagnostics: ControlDiagnostic list }

type DataGridMsg =
    | ScrollRowsTo of int
    | SelectRow of string
    | ToggleRow of string
    | FocusCell of DataGridFocusedCell option
    | SortBy of string
    | ApplyFilter of string option
    | ReplaceRowCount of int

type DataGridEffect =
    | DataGridVisibleRangeChanged of VisibleRange
    | DataGridSelectionChanged of string list
    | DataGridFocusChanged of DataGridFocusedCell option
    | DataGridSortChanged of DataGridSort option
    | DataGridFilterChanged of string option
    | ReportDataGridDiagnostic of ControlDiagnostic

module DataGrid =
    val init: controlId: ControlId -> columns: DataGridColumn list -> rowCount: int -> rowHeight: float -> viewportHeight: float -> DataGridModel * DataGridEffect list
    val update: msg: DataGridMsg -> model: DataGridModel -> DataGridModel * DataGridEffect list
    val create: columns: DataGridColumn list -> attrs: Attr<'msg> list -> Control<'msg>
    val columns: columns: DataGridColumn list -> Attr<'msg>
    val rows: rows: DataGridRow list -> Attr<'msg>
    val visibleRange: visibleRange: VisibleRange -> Attr<'msg>
    val selectedRows: selectedRows: Set<string> -> Attr<'msg>
    val focusedCell: focusedCell: DataGridFocusedCell option -> Attr<'msg>
