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
    let range rowHeight viewportHeight firstRow total =
        Collections.visibleRange rowHeight viewportHeight (float firstRow * rowHeight) total

    let viewportDiagnostics controlId rowHeight viewportHeight =
        [ if rowHeight <= 0.0 then
              yield
                  Diagnostics.create
                      (Some controlId)
                      "data-grid"
                      UnsupportedStateCombination
                      Error
                      "DataGrid rowHeight must be greater than zero."

          if viewportHeight <= 0.0 then
              yield
                  Diagnostics.create
                      (Some controlId)
                      "data-grid"
                      UnsupportedStateCombination
                      Error
                      "DataGrid viewportHeight must be greater than zero." ]

    let withDiagnosticEffects effects diagnostics =
        effects @ (diagnostics |> List.map ReportDataGridDiagnostic)

    let init controlId columns rowCount rowHeight viewportHeight =
        let rowCount = max 0 rowCount
        let visibleRange = range rowHeight viewportHeight 0 rowCount
        let diagnostics = viewportDiagnostics controlId rowHeight viewportHeight

        { ControlId = controlId
          Columns = columns
          RowCount = rowCount
          RowHeight = rowHeight
          ViewportHeight = viewportHeight
          VisibleRange = visibleRange
          SelectedRows = Set.empty
          FocusedCell = None
          Sort = None
          FilterText = None
          Diagnostics = diagnostics },
        withDiagnosticEffects [ DataGridVisibleRangeChanged visibleRange ] diagnostics

    let update msg model =
        match msg with
        | ScrollRowsTo firstRow ->
            let visibleRange =
                range model.RowHeight model.ViewportHeight (max 0 (min firstRow (max 0 (model.RowCount - 1)))) model.RowCount

            { model with VisibleRange = visibleRange }, [ DataGridVisibleRangeChanged visibleRange ]
        | SelectRow rowKey ->
            { model with SelectedRows = Set.singleton rowKey }, [ DataGridSelectionChanged [ rowKey ] ]
        | ToggleRow rowKey ->
            let selected =
                if model.SelectedRows.Contains rowKey then
                    model.SelectedRows.Remove rowKey
                else
                    model.SelectedRows.Add rowKey

            { model with SelectedRows = selected }, [ DataGridSelectionChanged(Set.toList selected) ]
        | FocusCell cell ->
            { model with FocusedCell = cell }, [ DataGridFocusChanged cell ]
        | SortBy columnKey ->
            if model.Columns |> List.exists (fun column -> column.Key = columnKey) |> not then
                let diagnostic =
                    Diagnostics.create
                        (Some model.ControlId)
                        "data-grid"
                        StaleGeneratedReference
                        Warning
                        $"DataGrid sort column '{columnKey}' does not exist."

                { model with Diagnostics = diagnostic :: model.Diagnostics }, [ ReportDataGridDiagnostic diagnostic ]
            else
                let direction =
                    match model.Sort with
                    | Some current when current.ColumnKey = columnKey && current.Direction = Ascending -> Descending
                    | _ -> Ascending

                let sort = Some { ColumnKey = columnKey; Direction = direction }
                { model with Sort = sort }, [ DataGridSortChanged sort ]
        | ApplyFilter filterText ->
            { model with FilterText = filterText }, [ DataGridFilterChanged filterText ]
        | ReplaceRowCount count ->
            let count = max 0 count
            let visibleRange = range model.RowHeight model.ViewportHeight model.VisibleRange.FirstIndex count
            { model with RowCount = count; VisibleRange = visibleRange }, [ DataGridVisibleRangeChanged visibleRange ]

    let tryLast (name: string) (attrs: Attr<'msg> list) =
        attrs
        |> List.rev
        |> List.tryFind (fun attr -> attr.Name = name)

    let rowsFrom (attrs: Attr<'msg> list) : DataGridRow list =
        tryLast "rows" attrs
        |> Option.bind (fun attr ->
            match attr.Value with
            | UntypedValue(:? (DataGridRow list) as rows) -> Some rows
            | UntypedValue(:? (DataGridRow array) as rows) -> Some(Array.toList rows)
            | _ -> None)
        |> Option.defaultValue []

    let visibleRangeFrom (rows: DataGridRow list) (attrs: Attr<'msg> list) =
        tryLast "visibleRange" attrs
        |> Option.bind (fun attr ->
            match attr.Value with
            | UntypedValue(:? VisibleRange as visibleRange) -> Some visibleRange
            | _ -> None)
        |> Option.defaultValue { FirstIndex = 0; Count = min rows.Length 30; Total = rows.Length }

    let hasAttr (name: string) (attrs: Attr<'msg> list) =
        attrs |> List.exists (fun attr -> attr.Name = name)

    let cellValue (row: DataGridRow) (column: DataGridColumn) =
        row.Cells
        |> List.tryFind (fun cell -> cell.ColumnKey = column.Key)
        |> Option.map _.Value
        |> Option.defaultValue ""

    let cellControl (row: DataGridRow) (column: DataGridColumn) : Control<'msg> =
        Control.create
            "data-grid-cell"
            [ Attr.text (cellValue row column)
              Attr.create "columnKey" Data (TextValue column.Key)
              Attr.create "rowKey" Data (TextValue row.Key) ]
        |> Control.withKey $"{row.Key}:{column.Key}"

    let headerCell (column: DataGridColumn) : Control<'msg> =
        Control.create
            "data-grid-header-cell"
            [ Attr.text column.Header
              Attr.create "columnKey" Data (TextValue column.Key) ]
        |> Control.withKey $"header:{column.Key}"

    let rowControl (columns: DataGridColumn list) (row: DataGridRow) : Control<'msg> =
        Control.create
            "data-grid-row"
            [ Attr.create "rowKey" Data (TextValue row.Key)
              Attr.children (columns |> List.map (cellControl row)) ]
        |> Control.withKey row.Key

    let visibleRows (rows: DataGridRow list) (visibleRange: VisibleRange) =
        rows
        |> List.skip (max 0 visibleRange.FirstIndex)
        |> List.truncate (max 0 visibleRange.Count)

    let create (columns: DataGridColumn list) (attrs: Attr<'msg> list) =
        let rows = rowsFrom attrs
        let visibleRange = visibleRangeFrom rows attrs
        let header = Control.create "data-grid-header" [ Attr.children (columns |> List.map headerCell) ]
        let children = header :: (visibleRows rows visibleRange |> List.map (rowControl columns))

        let attrs =
            attrs
            |> fun attrs -> if hasAttr "columns" attrs then attrs else Attr.create "columns" Data (UntypedValue columns) :: attrs
            |> fun attrs -> if hasAttr "visibleRange" attrs then attrs else Attr.create "visibleRange" State (UntypedValue visibleRange) :: attrs

        Control.create "data-grid" (Attr.children children :: attrs)

    let columns (columns: DataGridColumn list) =
        Attr.create "columns" Data (UntypedValue columns)

    let rows (rows: DataGridRow list) =
        Attr.create "rows" Data (UntypedValue rows)

    let visibleRange (visibleRange: VisibleRange) =
        Attr.create "visibleRange" State (UntypedValue visibleRange)

    let selectedRows (selectedRows: Set<string>) =
        Attr.create "selectedRows" State (UntypedValue selectedRows)

    let focusedCell (focusedCell: DataGridFocusedCell option) =
        let value =
            focusedCell
            |> Option.map (fun cell -> $"{cell.RowKey}:{cell.ColumnKey}")
            |> Option.defaultValue ""

        Attr.create "focusedCell" State (TextValue value)
