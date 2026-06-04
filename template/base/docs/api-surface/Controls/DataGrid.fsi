namespace FS.Skia.UI.Controls

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridColumnType =
    | TextColumn
    | NumericColumn
    | BooleanColumn
    | CustomColumn of string

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridColumn =
    { Key: string
      Header: string
      Width: float
      ColumnType: DataGridColumnType }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridCell =
    { RowKey: string
      ColumnKey: string
      Value: string }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridRow =
    { Key: string
      Cells: DataGridCell list }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridSortDirection =
    | Ascending
    | Descending

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridSort =
    { ColumnKey: string
      Direction: DataGridSortDirection }

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridFocusedCell =
    { RowKey: string
      ColumnKey: string }

/// Public contract type exposed by this FS.Skia.UI package.
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

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridMsg =
    | ScrollRowsTo of int
    | SelectRow of string
    | ToggleRow of string
    | FocusCell of DataGridFocusedCell option
    | SortBy of string
    | ApplyFilter of string option
    | ReplaceRowCount of int

/// Public contract type exposed by this FS.Skia.UI package.
type DataGridEffect =
    | DataGridVisibleRangeChanged of VisibleRange
    | DataGridSelectionChanged of string list
    | DataGridFocusChanged of DataGridFocusedCell option
    | DataGridSortChanged of DataGridSort option
    | DataGridFilterChanged of string option
    | ReportDataGridDiagnostic of ControlDiagnostic

/// Public contract module exposed by this FS.Skia.UI package.
module DataGrid =
    /// Public contract function exposed by this FS.Skia.UI package.
    val init: controlId: ControlId -> columns: DataGridColumn list -> rowCount: int -> rowHeight: float -> viewportHeight: float -> DataGridModel * DataGridEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val update: msg: DataGridMsg -> model: DataGridModel -> DataGridModel * DataGridEffect list
    /// Public contract function exposed by this FS.Skia.UI package.
    val create: columns: DataGridColumn list -> attrs: Attr<'msg> list -> Control<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val columns: columns: DataGridColumn list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val rows: rows: DataGridRow list -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val visibleRange: visibleRange: VisibleRange -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val selectedRows: selectedRows: Set<string> -> Attr<'msg>
    /// Public contract function exposed by this FS.Skia.UI package.
    val focusedCell: focusedCell: DataGridFocusedCell option -> Attr<'msg>
