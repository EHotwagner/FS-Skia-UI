namespace FS.Skia.UI.Charts

open System
open FS.Skia.UI

type DataGridTarget =
    | Header of columnKey: string
    | Cell of rowIndex: int * columnKey: string

module DataGrid =
    let defaultConfig width height = Defaults.dataGridConfig width height

    let columnWidth (config: DataGridConfig) columnCount (column: ColumnDef) =
        match column.Width with
        | Some width when width > 0.0 -> width
        | _ -> config.Area.Width / float (max 1 columnCount)

    let columnAt (config: DataGridConfig) (columns: ColumnDef list) x =
        if x < config.Area.X || x > config.Area.X + config.Area.Width then
            None
        else
            columns
            |> List.scan
                (fun (left, _) column ->
                    let width = columnWidth config columns.Length column
                    left + width, Some(column, left, left + width))
                (config.Area.X, None)
            |> List.choose snd
            |> List.tryFind (fun (_, left, right) -> x >= left && x <= right)
            |> Option.map (fun (column, _, _) -> column)

    let dataGrid (_: DataGridConfig) (data: DataGridData) (viewport: DataGridViewport) =
        let values =
            data.Rows
            |> List.skip (max 0 viewport.FirstRow)
            |> List.truncate (max 0 viewport.RowCount)
            |> List.mapi (fun index _ -> float index)

        Scene.chart values

    let visibleRows config totalRows scrollOffset =
        let bodyHeight = max 0.0 (config.Area.Height - config.HeaderHeight)
        let visibleCount = int (floor (bodyHeight / max 1.0 config.RowHeight))
        let firstRow = scrollOffset |> max 0 |> min (max 0 (totalRows - 1))

        { FirstRow = firstRow
          RowCount = max 0 (min visibleCount (totalRows - firstRow)) }

    let cellText cell =
        match cell with
        | TextValue value -> value
        | NumericValue value -> value.ToString("G17", Globalization.CultureInfo.InvariantCulture)
        | BooleanValue value -> string value
        | Empty -> ""

    let sortKey (columns: ColumnDef list) (sort: SortState) row =
        let columnType =
            columns
            |> List.tryFind (fun column -> column.Key = sort.ColumnKey)
            |> Option.map _.ColumnType
            |> Option.defaultValue Text

        let cell = row |> Map.tryFind sort.ColumnKey |> Option.defaultValue Empty

        match columnType, cell with
        | Numeric, NumericValue value -> struct (0, value, "")
        | Boolean, BooleanValue value ->
            let numeric = if value then 1.0 else 0.0
            struct (0, numeric, "")
        | _, Empty -> struct (1, 0.0, "")
        | _ -> struct (0, 0.0, cellText cell)

    let sortRows columns (sort: SortState) (rows: Map<string, CellValue> list) =
        let sorted = rows |> List.sortBy (sortKey columns sort)

        match sort.Direction with
        | Ascending -> sorted
        | Descending -> List.rev sorted

    let hitTest (config: DataGridConfig) (data: DataGridData) viewport x y =
        if x < config.Area.X || x > config.Area.X + config.Area.Width || y < config.Area.Y || y > config.Area.Y + config.Area.Height then
            None
        else
            match columnAt config data.Columns x with
            | None -> None
            | Some column ->
                if y <= config.Area.Y + config.HeaderHeight then
                    Some(Header column.Key)
                else
                    let bodyY = y - config.Area.Y - config.HeaderHeight
                    let rowOffset = int (floor (bodyY / max 1.0 config.RowHeight))

                    if rowOffset < 0 || rowOffset >= viewport.RowCount then
                        None
                    else
                        let rowIndex = viewport.FirstRow + rowOffset

                        if rowIndex >= data.Rows.Length then
                            None
                        else
                            Some(Cell(rowIndex, column.Key))
