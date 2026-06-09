namespace FS.Skia.UI.Controls

open System
open FS.Skia.UI.Scene

module LayoutDefaults = FS.Skia.UI.Layout.Defaults

module StandardControlKindHelpers =
    let toControlKind kind =
        match kind with
        | FS.Skia.UI.Controls.StandardControlKind.TextBlock -> "text-block"
        | FS.Skia.UI.Controls.StandardControlKind.Button -> "button"
        | FS.Skia.UI.Controls.StandardControlKind.TextBox -> "text-box"
        | FS.Skia.UI.Controls.StandardControlKind.LineChart -> "line-chart"
        | FS.Skia.UI.Controls.StandardControlKind.BarChart -> "bar-chart"
        | FS.Skia.UI.Controls.StandardControlKind.PieChart -> "pie-chart"
        | FS.Skia.UI.Controls.StandardControlKind.ScatterPlot -> "scatter-plot"
        | FS.Skia.UI.Controls.StandardControlKind.GraphView -> "graph-view"
        | FS.Skia.UI.Controls.StandardControlKind.DataGrid -> "data-grid"
        | FS.Skia.UI.Controls.StandardControlKind.Custom value -> value

module internal ControlInternals =
    let tryLast name (attrs: Attr<'msg> list) =
        attrs
        |> List.rev
        |> List.tryFind (fun attr -> attr.Name = name)

    let textFrom (attrs: Attr<'msg> list) =
        tryLast "text" attrs
        |> Option.orElseWith (fun () -> tryLast "value" attrs)
        |> Option.bind (fun attr ->
            match attr.Value with
            | TextValue value -> Some value
            | FloatValue value -> Some(string value)
            | BoolValue value -> Some(string value)
            | StringListValue values -> Some(String.concat ", " values)
            | ValidationValue Valid -> Some "valid"
            | ValidationValue(Invalid message) -> Some message
            | ValidationValue(Pending message) -> Some message
            | _ -> None)

    let boolValue name defaultValue (attrs: Attr<'msg> list) =
        tryLast name attrs
        |> Option.bind (fun attr ->
            match attr.Value with
            | BoolValue value -> Some value
            | _ -> None)
        |> Option.defaultValue defaultValue

    let floatValue name defaultValue (attrs: Attr<'msg> list) =
        tryLast name attrs
        |> Option.bind (fun attr ->
            match attr.Value with
            | FloatValue value -> Some value
            | _ -> None)
        |> Option.defaultValue defaultValue

    let accessibility kind (attrs: Attr<'msg> list) text =
        tryLast "accessibility" attrs
        |> Option.bind (fun attr ->
            match attr.Value with
            | AccessibilityValue value -> Some value
            | _ -> None)
        |> Option.orElseWith (fun () -> Some(Accessibility.defaultFor kind (text |> Option.defaultValue kind)))

    let childrenFrom (attrs: Attr<'msg> list) =
        attrs
        |> List.collect (fun attr ->
            match attr.Value with
            | ChildValue child -> [ child ]
            | ChildrenValue children -> children
            | _ -> [])

    let required kind =
        match kind with
        | "text-block"
        | "label"
        | "badge"
        | "button"
        | "validation-message"
        | "tooltip"
        | "toast" -> [ "text" ]
        | "text-box"
        | "text-area" -> [ "value" ]
        | "numeric-input"
        | "slider"
        | "progress-bar" -> [ "value" ]
        | "radio-group"
        | "tabs"
        | "menu" -> [ "items" ]
        | "line-chart"
        | "bar-chart"
        | "scatter-plot" -> [ "series" ]
        | "pie-chart" -> [ "values" ]
        | "graph-view" -> [ "nodes" ]
        | "data-grid" -> [ "columns"; "rows" ]
        | _ -> []

    let hasAttr name (attrs: Attr<'msg> list) =
        attrs |> List.exists (fun attr -> attr.Name = name)

    let disabledOrReadOnly (control: Control<'msg>) =
        let enabled = boolValue "enabled" true control.Attributes
        let readOnly = boolValue "readOnly" false control.Attributes
        not enabled || readOnly

    let eventKind attrName =
        match attrName with
        | "onClick" -> "click"
        | "onChanged" -> "changed"
        | "onSelected" -> "selected"
        | value when value.StartsWith("on", StringComparison.Ordinal) ->
            value.Substring(2).ToLowerInvariant()
        | value -> value

    let eventBindings (control: Control<'msg>) =
        let id = control.Key |> Option.defaultValue control.Kind

        control.Attributes
        |> List.choose (fun attr ->
            if attr.Category <> Event then
                None
            else
                let kind = eventKind attr.Name

                match attr.Value with
                | MessageValue msg -> Some { ControlId = id; EventKind = kind; Dispatch = fun _ -> msg }
                | EventValue map -> Some { ControlId = id; EventKind = kind; Dispatch = map }
                | _ -> None)

    let rec recursively collect (control: Control<'msg>) =
        collect control @ (control.Children |> List.collect (recursively collect))

    let fittedFontSize maxSize minSize width height family (label: string) =
        let availableWidth = max 1.0 (width - 16.0)
        let availableHeight = max 1.0 (height - 8.0)
        let upper = Math.Clamp(maxSize, minSize, max minSize availableHeight)
        let font size = { Family = family; Size = size; Weight = None }
        let fits size =
            let metrics = Scene.measureText label (font size)
            metrics.Width <= availableWidth && metrics.Height <= availableHeight

        if fits upper then
            upper
        else
            let rec search remaining low high =
                if remaining = 0 then
                    low
                else
                    let mid = (low + high) * 0.5

                    if fits mid then
                        search (remaining - 1) mid high
                    else
                        search (remaining - 1) low mid

            search 8 minSize upper

    let chartValues (control: Control<'msg>) : ChartPoint list =
        // Feature 080 (FR-002): read the structured `UntypedValue(ChartSeries list)` (series)
        // and `UntypedValue(ChartPoint list)` (pie) the typed front door actually stores,
        // preserving X/Y/Label. The flat `float list`/`float array`/`FloatValue` fallback is
        // retained for legacy untyped authoring (mapped to points with X = index). Pre-080 this
        // matched only the flat shapes, so typed charts silently yielded `[]` (root cause).
        let indexed (values: float list) =
            values |> List.mapi (fun index value -> { X = float index; Y = value; Label = None })

        let points name =
            tryLast name control.Attributes
            |> Option.bind (fun attr ->
                match attr.Value with
                | UntypedValue(:? (ChartSeries list) as series) ->
                    Some(series |> List.collect (fun s -> s.Points))
                | UntypedValue(:? (ChartPoint list) as pts) -> Some pts
                | UntypedValue(:? (float list) as values) -> Some(indexed values)
                | UntypedValue(:? (float array) as values) -> Some(indexed (Array.toList values))
                | FloatValue value -> Some [ { X = 0.0; Y = value; Label = None } ]
                | _ -> None)

        match control.Kind with
        | "line-chart"
        | "bar-chart"
        | "scatter-plot" ->
            points "series" |> Option.defaultValue []
        | "pie-chart" ->
            points "values" |> Option.defaultValue []
        | "graph-view" ->
            tryLast "nodes" control.Attributes
            |> Option.bind (fun attr ->
                match attr.Value with
                | StringListValue values ->
                    Some(values |> List.mapi (fun index label -> { X = float index; Y = float index; Label = Some label }))
                | _ -> None)
            |> Option.defaultValue []
        | _ -> []

    /// Read the field-name-free run projection (text, colour, size, weight) that `RichText.create`
    /// stashes in the `richTextRuns` attr, so the preview can draw real per-run colour/weight
    /// rather than the kind id. (Control.fs compiles before RichText.fs, so the typed
    /// `RichTextBlock` is intentionally not in scope here.)
    let richTextRuns (control: Control<'msg>) : (string * Color * float * int) list =
        tryLast "richTextRuns" control.Attributes
        |> Option.bind (fun attr ->
            match attr.Value with
            | UntypedValue(:? (list<string * Color * float * int>) as runs) -> Some runs
            | _ -> None)
        |> Option.defaultValue []

    // Feature 080 (FR-001/003/004/005/011) — faithful per-control preview geometry.
    //
    // Controls in `richFamilies` lower to control-specific geometry built from EXISTING Scene
    // primitives (polyline `Path` for line, `Rectangle`s for bars, `Arc`s for pie, `Circle`s for
    // scatter, item rows for collections, track+thumb/tick/toggle/tab chrome for value/selection
    // controls, a framed placeholder for `image`, a font-safe `Path` glyph for `icon`), laid out
    // BELOW the title band so the fidelity gate's "coverage outside the title band" criterion is
    // met. Every other control (text/containers — `button`, `label`, `stack`, …) keeps the
    // 079 box+label: those controls ARE their text, so a label-on-a-box is already faithful.
    let richFamilies =
        Set.ofList
            [ "line-chart"; "bar-chart"; "pie-chart"; "scatter-plot"; "graph-view"
              "list-view"; "list-box"; "multi-select-list"; "combo-box"; "tree-view"; "data-grid"
              "menu"; "context-menu"; "radio-group"; "tabs"
              "slider"; "progress-bar"; "numeric-input"; "switch"; "check-box"
              "button"; "icon-button"; "badge"; "toggle-button"; "split-button"
              "date-picker"; "time-picker"; "color-picker"; "spinner"; "image"; "icon"
              // layout / container families (built as single-Kind preview schematics, FR-001):
              "stack"; "grid"; "dock"; "wrap"; "panel"; "border"; "scroll-viewer"
              "split-view"; "toolbar"; "overlay"
              // feature 082 — text-input / rich-text / divider controls. These were previously in
              // the box+label fallback, which is faithful for static text (label/text-block) but
              // hid an editable field's chrome (text-box/text-area read as plain labels), dropped
              // rich-text's styled runs (it rendered its kind id), and drew `separator` as the word
              // "separator" instead of a divider rule. They now lower to control-specific geometry.
              "text-box"; "text-area"; "rich-text"; "separator" ]

    /// A human caption for the rich-family title band: "date-picker" -> "Date picker".
    /// Used so the thumbnail's title is the control's NAME, not its sample content (which the
    /// schematic below already shows) — fixing composite-lowering title bleed (e.g. "STACK").
    let prettyKind (kind: string) =
        match kind.Split('-') |> Array.toList with
        | [] -> kind
        | head :: tail ->
            let cap (w: string) = if w.Length = 0 then w else string (System.Char.ToUpper w[0]) + w.Substring 1
            cap head :: tail |> String.concat " "

    /// Preview node width: explicit `width` wins; rich families fill the preview canvas.
    let nodeWidth (control: Control<'msg>) =
        if hasAttr "width" control.Attributes then floatValue "width" 240.0 control.Attributes
        elif Set.contains control.Kind richFamilies then 304.0
        else 240.0

    /// Preview node height: explicit `height` wins; rich families get a tall box so geometry
    /// sits below the title band (a 24-px box would put everything inside the band).
    let nodeHeight (control: Control<'msg>) =
        if hasAttr "height" control.Attributes then max 20.0 (floatValue "height" 24.0 control.Attributes)
        elif Set.contains control.Kind richFamilies then 132.0
        else 24.0

    let private palette (theme: Theme) =
        [ theme.Accent
          Colors.rgb 210uy 95uy 75uy
          Colors.rgb 90uy 165uy 95uy
          Colors.rgb 150uy 110uy 205uy
          Colors.rgb 215uy 165uy 65uy
          Colors.rgb 80uy 150uy 205uy ]

    let private colorAt theme i =
        let p = palette theme
        List.item (((i % p.Length) + p.Length) % p.Length) p

    let private mkText (theme: Theme) (x: float) (baseline: float) (size: float) (color: Color) (s: string) =
        Scene.textRun
            { Text = s
              Position = { X = x; Y = baseline }
              Font = { Family = theme.FontFamily; Size = size; Weight = None }
              Paint = Paint.fill color }

    /// `mkText` with an explicit weight — used by the rich-text schematic to draw bold runs.
    let private mkTextW (theme: Theme) (x: float) (baseline: float) (size: float) (weight: int option) (color: Color) (s: string) =
        Scene.textRun
            { Text = s
              Position = { X = x; Y = baseline }
              Font = { Family = theme.FontFamily; Size = size; Weight = weight }
              Paint = Paint.fill color }

    let private stringListOf name (control: Control<'msg>) =
        tryLast name control.Attributes
        |> Option.bind (fun attr ->
            match attr.Value with
            | StringListValue values -> Some values
            | _ -> None)
        |> Option.defaultValue []

    let private textValueOf name (control: Control<'msg>) =
        tryLast name control.Attributes
        |> Option.bind (fun attr ->
            match attr.Value with
            | TextValue value -> Some value
            | _ -> None)

    /// Honest empty state (FR-011): a faint frame + a "(no data)" caption within bounds, so an
    /// empty/missing-data control reads as a recognizable empty control, never an off-canvas blank.
    let private emptyState (theme: Theme) (box: Rect) (caption: string) : Scene list =
        [ Scene.rectangleWithPaint box (Paint.stroke theme.Muted 1.0)
          mkText theme (box.X + 8.0) (box.Y + box.Height * 0.5) 12.0 theme.Muted caption ]

    // ---- chart geometry ---------------------------------------------------------------------

    let private normIndexed (box: Rect) (pts: ChartPoint list) : Point list =
        match pts with
        | [] -> []
        | _ ->
            let ys = pts |> List.map (fun p -> p.Y)
            let minY = min 0.0 (List.min ys)
            let maxY = List.max ys
            let span = if maxY - minY < 1e-9 then 1.0 else maxY - minY
            let n = List.length pts
            pts
            |> List.mapi (fun i p ->
                let fx = if n <= 1 then 0.5 else float i / float (n - 1)
                let fy = (p.Y - minY) / span
                { X = box.X + fx * box.Width; Y = box.Y + box.Height - fy * box.Height })

    let private lineGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match normIndexed box pts with
        | [] -> emptyState theme box "(no data)"
        | (head :: _) as ps ->
            let baseY = box.Y + box.Height
            let areaCmds =
                Path.moveTo head.X baseY
                :: (ps |> List.map (fun p -> Path.lineTo p.X p.Y))
                @ [ Path.lineTo (List.last ps).X baseY; Path.close ]
            let area = Scene.path (Path.create Winding areaCmds) (Paint.withOpacity 0.22 (Paint.fill theme.Accent))
            let lineCmds = Path.moveTo head.X head.Y :: (List.tail ps |> List.map (fun p -> Path.lineTo p.X p.Y))
            let stroke = Scene.path (Path.create Winding lineCmds) (Paint.stroke theme.Accent 3.0)
            let dots = ps |> List.map (fun p -> Scene.circle p 3.5 theme.Accent)
            area :: stroke :: dots

    let private barGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match pts with
        | [] -> emptyState theme box "(no data)"
        | _ ->
            let maxY = pts |> List.map (fun p -> max 0.0 p.Y) |> List.fold max 1e-9
            let n = List.length pts
            let gap = 6.0
            let bw = (box.Width - gap * float (n - 1)) / float n
            pts
            |> List.mapi (fun i p ->
                let h = (max 0.0 p.Y / maxY) * box.Height
                let bx = box.X + float i * (bw + gap)
                Scene.rectangle (bx, box.Y + box.Height - h, bw, h) (colorAt theme i))

    let private pieGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match pts with
        | [] -> emptyState theme box "(no data)"
        | _ ->
            let total = pts |> List.sumBy (fun p -> max 0.0 p.Y)
            let total = if total < 1e-9 then 1.0 else total
            let r = (min box.Width box.Height) / 2.0 - 2.0
            let cx = box.X + box.Width / 2.0
            let cy = box.Y + box.Height / 2.0
            let bounds: Rect = { X = cx - r; Y = cy - r; Width = 2.0 * r; Height = 2.0 * r }
            pts
            |> List.indexed
            |> List.fold
                (fun (start, acc) (i, p) ->
                    let sweep = (max 0.0 p.Y / total) * 360.0
                    start + sweep, Scene.arc bounds start sweep (Paint.fill (colorAt theme i)) :: acc)
                (-90.0, [])
            |> snd
            |> List.rev

    /// L-shaped axes (left + bottom) so a sparse point cloud reads as a plotted chart, not
    /// scattered dots floating on the canvas.
    let private axes theme (box: Rect) : Scene list =
        [ Scene.line { X = box.X; Y = box.Y } { X = box.X; Y = box.Y + box.Height } (Paint.stroke theme.Foreground 1.5)
          Scene.line { X = box.X; Y = box.Y + box.Height } { X = box.X + box.Width; Y = box.Y + box.Height } (Paint.stroke theme.Foreground 1.5) ]

    let private scatterGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match pts with
        | [] -> emptyState theme box "(no data)"
        | _ ->
            // Inset the plot area so axes and edge points stay inside the canvas.
            let plot: Rect = { X = box.X + 6.0; Y = box.Y + 4.0; Width = box.Width - 12.0; Height = box.Height - 12.0 }
            let xs = pts |> List.map (fun p -> p.X)
            let ys = pts |> List.map (fun p -> p.Y)
            let minX, maxX = List.min xs, List.max xs
            let minY, maxY = List.min ys, List.max ys
            let sx = if maxX - minX < 1e-9 then 1.0 else maxX - minX
            let sy = if maxY - minY < 1e-9 then 1.0 else maxY - minY
            let dots =
                pts
                |> List.map (fun p ->
                    let cx = plot.X + (p.X - minX) / sx * plot.Width
                    let cy = plot.Y + plot.Height - (p.Y - minY) / sy * plot.Height
                    Scene.circle { X = cx; Y = cy } 5.5 theme.Accent)
            axes theme plot @ dots

    let private graphGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match pts with
        | [] -> emptyState theme box "(no data)"
        | _ ->
            let n = List.length pts
            let cx = box.X + box.Width / 2.0
            let cy = box.Y + box.Height / 2.0
            let r = (min box.Width box.Height) / 2.0 - 12.0
            let positions =
                pts
                |> List.mapi (fun i _ ->
                    let a = float i / float n * 2.0 * System.Math.PI - System.Math.PI / 2.0
                    { X = cx + r * cos a; Y = cy + r * sin a })
            let edges =
                (positions @ [ List.head positions ])
                |> List.pairwise
                |> List.map (fun (a, b) -> Scene.line a b (Paint.stroke theme.Foreground 2.0))
            let nodes = positions |> List.map (fun p -> Scene.circle p 8.0 theme.Accent)
            edges @ nodes

    // ---- collection / selection / value geometry --------------------------------------------

    let private rowsGeom theme (box: Rect) (items: string list) (selected: Set<string>) : Scene list =
        match items with
        | [] -> emptyState theme box "(empty)"
        | _ ->
            let shown = items |> List.truncate 5
            let n = List.length shown
            let rowH = box.Height / float n
            shown
            |> List.mapi (fun i it ->
                let ry = box.Y + float i * rowH
                let bg =
                    if Set.contains it selected then theme.Accent
                    elif i % 2 = 0 then theme.Muted
                    else theme.Background
                Scene.group
                    [ Scene.rectangle (box.X, ry, box.Width, rowH - 1.5) bg
                      mkText theme (box.X + 8.0) (ry + rowH * 0.62) 12.0 theme.Foreground it ])

    /// Tabular chrome for `data-grid`: a header band, column/row rules, and sample cell text laid
    /// out row-major from `cells` (first `cols` entries are the header). The preview is built as a
    /// single-Kind node so the composite header/cell tree does not flatten into stray rows.
    let private gridGeom theme (box: Rect) (cells: string list) : Scene list =
        let cols = 2
        let rows = 2
        let cw = box.Width / float cols
        let rh = box.Height / float (rows + 1)
        let frame = Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 1.5)
        let header = Scene.rectangle (box.X, box.Y, box.Width, rh) theme.Muted
        let rowLines =
            [ for r in 1..rows -> Scene.line { X = box.X; Y = box.Y + float r * rh } { X = box.X + box.Width; Y = box.Y + float r * rh } (Paint.stroke theme.Muted 1.0) ]
        let colLines =
            [ for c in 1 .. cols - 1 -> Scene.line { X = box.X + float c * cw; Y = box.Y } { X = box.X + float c * cw; Y = box.Y + box.Height } (Paint.stroke theme.Muted 1.0) ]
        let texts =
            cells
            |> List.truncate (cols * (rows + 1))
            |> List.mapi (fun i s ->
                let r = i / cols
                let c = i % cols
                mkText theme (box.X + float c * cw + 6.0) (box.Y + float r * rh + rh * 0.66) 11.0 theme.Foreground s)
        frame :: header :: (rowLines @ colLines @ texts)

    let private radioGeom theme (box: Rect) (items: string list) (selected: string option) : Scene list =
        match items with
        | [] -> emptyState theme box "(empty)"
        | _ ->
            let rowH = min 28.0 (box.Height / float (List.length items))
            items
            |> List.mapi (fun i it ->
                let cy = box.Y + float i * rowH + rowH / 2.0
                let cx = box.X + 9.0
                let isSel = selected = Some it
                let outer = Scene.circle { X = cx; Y = cy } 7.0 (if isSel then theme.Accent else theme.Muted)
                let inner = if isSel then [ Scene.circle { X = cx; Y = cy } 3.0 theme.Background ] else []
                Scene.group (outer :: inner @ [ mkText theme (cx + 16.0) (cy + 4.0) 12.0 theme.Foreground it ]))

    let private tabsGeom theme (box: Rect) (items: string list) (selected: string option) : Scene list =
        match items with
        | [] -> emptyState theme box "(empty)"
        | _ ->
            let n = List.length items
            let tw = box.Width / float n
            let stripH = min 30.0 box.Height
            items
            |> List.mapi (fun i it ->
                let tx = box.X + float i * tw
                let active = selected = Some it
                Scene.group
                    [ Scene.rectangle (tx, box.Y, tw - 2.0, stripH) (if active then theme.Accent else theme.Muted)
                      mkText theme (tx + 6.0) (box.Y + stripH * 0.62) 11.0 theme.Foreground it ])

    let private sliderGeom theme (box: Rect) (value: float) : Scene list =
        let v = max 0.0 (min 1.0 value)
        let cy = box.Y + box.Height / 2.0
        [ Scene.rectangle (box.X, cy - 2.0, box.Width, 4.0) theme.Muted
          Scene.rectangle (box.X, cy - 2.0, box.Width * v, 4.0) theme.Accent
          Scene.circle { X = box.X + box.Width * v; Y = cy } 8.0 theme.Accent ]

    let private progressGeom theme (box: Rect) (value: float) : Scene list =
        let v = max 0.0 (min 1.0 value)
        let barH = 16.0
        let by = box.Y + box.Height / 2.0 - barH / 2.0
        [ Scene.rectangle (box.X, by, box.Width, barH) theme.Muted
          Scene.rectangle (box.X, by, box.Width * v, barH) theme.Accent ]

    let private numericGeom theme (box: Rect) (value: float) : Scene list =
        let cy = box.Y + box.Height / 2.0
        [ Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 2.0)
          mkText theme (box.X + 10.0) (cy + 5.0) 16.0 theme.Foreground (sprintf "%g" value)
          Scene.line { X = box.X + box.Width - 16.0; Y = cy } { X = box.X + box.Width - 6.0; Y = cy } (Paint.stroke theme.Muted 2.0) ]

    let private switchGeom theme (box: Rect) (on: bool) : Scene list =
        let cy = box.Y + box.Height / 2.0
        let w = 52.0
        let thumbX = if on then box.X + w - 12.0 else box.X + 12.0
        [ Scene.rectangle (box.X, cy - 12.0, w, 24.0) (if on then theme.Accent else theme.Muted)
          Scene.circle { X = thumbX; Y = cy } 10.0 theme.Background ]

    let private checkboxGeom theme (box: Rect) (on: bool) (label: string) : Scene list =
        let s = 28.0
        let bx = box.X
        let cy = box.Y + box.Height / 2.0
        let by = cy - s / 2.0
        // Filled accent box + white tick when checked; outlined empty box when not.
        let boxRect = { X = bx; Y = by; Width = s; Height = s }
        let fill =
            if on then [ Scene.rectangle (bx, by, s, s) theme.Accent ]
            else [ Scene.rectangleWithPaint boxRect (Paint.stroke theme.Foreground 2.0) ]
        let tick =
            if on then
                [ Scene.line { X = bx + 6.0; Y = by + 15.0 } { X = bx + 12.0; Y = by + 21.0 } (Paint.stroke theme.Background 3.0)
                  Scene.line { X = bx + 12.0; Y = by + 21.0 } { X = bx + 23.0; Y = by + 7.0 } (Paint.stroke theme.Background 3.0) ]
            else
                []
        let text = [ mkText theme (bx + s + 10.0) (cy + 5.0) 13.0 theme.Foreground label ]
        fill @ tick @ text

    let private toggleGeom theme (box: Rect) (on: bool) (label: string) : Scene list =
        // A button-shaped chip; filled accent when pressed (on), outlined when not.
        let h = 36.0
        let w = min box.Width 150.0
        let by = box.Y + box.Height / 2.0 - h / 2.0
        let rect = { X = box.X; Y = by; Width = w; Height = h }
        let textColor = if on then theme.Background else theme.Foreground
        let surface =
            if on then [ Scene.rectangle (box.X, by, w, h) theme.Accent ]
            else [ Scene.rectangleWithPaint rect (Paint.stroke theme.Accent 2.0) ]
        surface @ [ mkText theme (box.X + 12.0) (by + h / 2.0 + 5.0) 14.0 textColor label ]

    let private pickerGeom theme (box: Rect) (text: string) : Scene list =
        let frame = Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 2.0)
        let segs =
            [ for f in [ 0.34; 0.67 ] ->
                  Scene.line { X = box.X + box.Width * f; Y = box.Y } { X = box.X + box.Width * f; Y = box.Y + box.Height } (Paint.stroke theme.Muted 1.0) ]
        frame :: mkText theme (box.X + 8.0) (box.Y + box.Height / 2.0 + 5.0) 14.0 theme.Foreground text :: segs

    let private swatchGeom theme (box: Rect) : Scene list =
        let n = 5
        let sw = box.Width / float n
        [ for i in 0 .. n - 1 -> Scene.rectangle (box.X + float i * sw, box.Y, sw - 3.0, box.Height) (colorAt theme i) ]

    let private spinnerGeom theme (box: Rect) : Scene list =
        let r = (min box.Width box.Height) / 2.0 - 8.0
        let cx = box.X + box.Width / 2.0
        let cy = box.Y + box.Height / 2.0
        let bounds: Rect = { X = cx - r; Y = cy - r; Width = 2.0 * r; Height = 2.0 * r }
        // A faint full-circle track plus a bold accent sweep with a gap reads as a busy spinner.
        [ Scene.arc bounds 0.0 360.0 (Paint.stroke theme.Muted 7.0)
          Scene.arc bounds -90.0 280.0 (Paint.stroke theme.Accent 7.0) ]

    let private imageGeom theme (box: Rect) (source: string) : Scene list =
        [ Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 2.0)
          Scene.line { X = box.X; Y = box.Y } { X = box.X + box.Width; Y = box.Y + box.Height } (Paint.stroke theme.Muted 1.5)
          Scene.line { X = box.X + box.Width; Y = box.Y } { X = box.X; Y = box.Y + box.Height } (Paint.stroke theme.Muted 1.5)
          mkText theme (box.X + 6.0) (box.Y + box.Height - 6.0) 11.0 theme.Foreground source ]

    let private iconGeom theme (box: Rect) (name: string) : Scene list =
        // A font-independent house glyph from a `Path` (no `.notdef` box risk), plus the name.
        let cx = box.X + 22.0
        let cy = box.Y + box.Height / 2.0
        let r = 16.0
        let cmds =
            [ Path.moveTo (cx - r) cy
              Path.lineTo cx (cy - r)
              Path.lineTo (cx + r) cy
              Path.lineTo (cx + r - 3.0) cy
              Path.lineTo (cx + r - 3.0) (cy + r)
              Path.lineTo (cx - r + 3.0) (cy + r)
              Path.lineTo (cx - r + 3.0) cy
              Path.close ]
        [ Scene.path (Path.create Winding cmds) (Paint.fill theme.Accent)
          mkText theme (cx + r + 8.0) (cy + 5.0) 14.0 theme.Foreground name ]

    // ---- command / button geometry ----------------------------------------------------------

    /// A filled command button sized to its label, vertically centred. `primary` ⇒ accent fill
    /// with light text; otherwise an accent-outlined neutral surface.
    let private buttonGeom theme (box: Rect) (primary: bool) (label: string) : Scene list =
        let h = 38.0
        let textW = (Scene.measureText label { Family = theme.FontFamily; Size = 15.0; Weight = None }).Width
        let w = min box.Width (max 70.0 (textW + 32.0))
        let by = box.Y + box.Height / 2.0 - h / 2.0
        let rect = { X = box.X; Y = by; Width = w; Height = h }
        if primary then
            [ Scene.rectangle (box.X, by, w, h) theme.Accent
              mkText theme (box.X + 16.0) (by + h / 2.0 + 5.0) 15.0 theme.Background label ]
        else
            [ Scene.rectangleWithPaint rect (Paint.stroke theme.Accent 2.0)
              mkText theme (box.X + 16.0) (by + h / 2.0 + 5.0) 15.0 theme.Accent label ]

    /// A compact accent pill with light text — a status badge.
    let private badgeGeom theme (box: Rect) (label: string) : Scene list =
        let h = 26.0
        let textW = (Scene.measureText label { Family = theme.FontFamily; Size = 12.0; Weight = None }).Width
        let w = max 40.0 (textW + 20.0)
        let by = box.Y + box.Height / 2.0 - h / 2.0
        [ Scene.rectangle (box.X, by, w, h) theme.Accent
          mkText theme (box.X + 10.0) (by + h / 2.0 + 4.0) 12.0 theme.Background label ]

    /// A primary command button joined to a dropdown trigger (caret) — a split button.
    let private splitGeom theme (box: Rect) (label: string) : Scene list =
        let h = 38.0
        let by = box.Y + box.Height / 2.0 - h / 2.0
        let triggerW = 30.0
        let primaryW = min (box.Width - triggerW - 2.0) 160.0
        let caretX = box.X + primaryW + 2.0 + triggerW / 2.0
        let caretY = by + h / 2.0
        let caret =
            Path.create
                Winding
                [ Path.moveTo (caretX - 6.0) (caretY - 3.0)
                  Path.lineTo (caretX + 6.0) (caretY - 3.0)
                  Path.lineTo caretX (caretY + 5.0)
                  Path.close ]
        [ Scene.rectangle (box.X, by, primaryW, h) theme.Accent
          mkText theme (box.X + 14.0) (by + h / 2.0 + 5.0) 15.0 theme.Background label
          Scene.rectangle (box.X + primaryW + 2.0, by, triggerW, h) theme.Muted
          Scene.path caret (Paint.fill theme.Foreground) ]

    // ---- layout / container geometry --------------------------------------------------------

    /// A bordered, filled, labelled region — the building block for container schematics so every
    /// region is visible against the canvas (a `theme.Background` fill alone would be invisible).
    let private regionRect theme (x: float) (y: float) (w: float) (h: float) (fill: Color) (label: string) : Scene list =
        [ Scene.rectangle (x, y, w, h) fill
          Scene.rectangleWithPaint { X = x; Y = y; Width = w; Height = h } (Paint.stroke theme.Foreground 1.0)
          mkText theme (x + 6.0) (y + h / 2.0 + 4.0) 12.0 theme.Foreground label ]

    let private itemsOr (fallback: string list) (items: string list) =
        match items with
        | [] -> fallback
        | _ -> items

    /// Vertically stacked child regions — `stack`.
    let private stackGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "One"; "Two"; "Three" ] |> List.truncate 4
        let n = max 1 (List.length shown)
        let rowH = box.Height / float n
        shown |> List.mapi (fun i it -> regionRect theme box.X (box.Y + float i * rowH) box.Width (rowH - 4.0) theme.Muted it) |> List.concat

    /// A 2-column cell grid — `grid` (distinct from `data-grid`'s tabular `gridGeom`).
    let private gridLayoutGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "A1"; "B2"; "C3"; "D4" ] |> List.truncate 4
        let cols = 2
        let cw = box.Width / float cols
        let rows = max 1 ((List.length shown + cols - 1) / cols)
        let rh = box.Height / float rows
        shown
        |> List.mapi (fun i it -> regionRect theme (box.X + float (i % cols) * cw) (box.Y + float (i / cols) * rh) (cw - 5.0) (rh - 5.0) theme.Muted it)
        |> List.concat

    /// Small chips flowing left-to-right and wrapping — `wrap`.
    let private wrapGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "tag1"; "tag2"; "tag3" ] |> List.truncate 6
        let chipW = 66.0
        let chipH = 26.0
        let gap = 7.0
        let perRow = max 1 (int (box.Width / (chipW + gap)))
        shown
        |> List.mapi (fun i it ->
            let r = i / perRow
            let c = i % perRow
            regionRect theme (box.X + float c * (chipW + gap)) (box.Y + float r * (chipH + gap)) chipW chipH theme.Muted it)
        |> List.concat

    /// A docked top bar plus a left rail and a filled centre — `dock`.
    let private dockGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "Top"; "Fill" ]
        let topH = 26.0
        let leftW = 72.0
        let bodyY = box.Y + topH + 2.0
        let bodyH = box.Height - topH - 2.0
        regionRect theme box.X box.Y box.Width topH theme.Accent (List.tryItem 0 shown |> Option.defaultValue "Top")
        @ regionRect theme box.X bodyY leftW bodyH theme.Muted "Left"
        @ regionRect theme (box.X + leftW + 2.0) bodyY (box.Width - leftW - 2.0) bodyH theme.Background (List.tryItem 1 shown |> Option.defaultValue "Fill")

    /// Two side-by-side panes with a divider — `split-view`.
    let private splitViewGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "Left"; "Right" ]
        let half = box.Width / 2.0
        regionRect theme box.X box.Y (half - 4.0) box.Height theme.Muted (List.tryItem 0 shown |> Option.defaultValue "Left")
        @ [ Scene.rectangle (box.X + half - 2.0, box.Y, 4.0, box.Height) theme.Foreground ]
        @ regionRect theme (box.X + half + 4.0) box.Y (half - 4.0) box.Height theme.Background (List.tryItem 1 shown |> Option.defaultValue "Right")

    /// A command strip of horizontal buttons — `toolbar`.
    let private toolbarGeom theme (box: Rect) (items: string list) : Scene list =
        let shown = items |> itemsOr [ "B"; "I"; "U" ] |> List.truncate 6
        let stripH = 38.0
        let strip = Scene.rectangle (box.X, box.Y, box.Width, stripH) theme.Muted
        let bw = 42.0
        let btns =
            shown
            |> List.mapi (fun i it -> regionRect theme (box.X + 8.0 + float i * (bw + 6.0)) (box.Y + 5.0) bw (stripH - 10.0) theme.Background it)
            |> List.concat
        strip :: btns

    /// A surface with a header band and a body — `panel`.
    let private panelGeom theme (box: Rect) (label: string) : Scene list =
        let headH = 26.0
        [ Scene.rectangle (box.X, box.Y, box.Width, headH) theme.Accent
          Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 1.0) ]
        @ [ mkText theme (box.X + 8.0) (box.Y + box.Height / 2.0 + 8.0) 12.0 theme.Foreground label ]

    /// A thick border framing inner content — `border`.
    let private borderGeom theme (box: Rect) (label: string) : Scene list =
        let inset = 10.0
        [ Scene.rectangleWithPaint box (Paint.stroke theme.Accent 4.0) ]
        @ regionRect theme (box.X + inset) (box.Y + inset) (box.Width - 2.0 * inset) (box.Height - 2.0 * inset) theme.Muted label

    /// A scrollable viewport: content area plus a vertical scrollbar thumb — `scroll-viewer`.
    let private scrollViewerGeom theme (box: Rect) (label: string) : Scene list =
        let barW = 10.0
        let contentW = box.Width - barW - 4.0
        regionRect theme box.X box.Y contentW box.Height theme.Muted label
        @ [ Scene.rectangle (box.X + contentW + 4.0, box.Y, barW, box.Height) theme.Muted
            Scene.rectangle (box.X + contentW + 4.0, box.Y + 6.0, barW, box.Height * 0.4) theme.Accent ]

    /// Two layered, offset surfaces suggesting stacked content — `overlay`.
    let private overlayGeom theme (box: Rect) (label: string) : Scene list =
        let off = 16.0
        regionRect theme box.X box.Y (box.Width - off) (box.Height - off) theme.Muted ""
        @ regionRect theme (box.X + off) (box.Y + off) (box.Width - off) (box.Height - off) theme.Background label

    // ---- text-input / rich-text / divider geometry (feature 082) ----------------------------

    /// A bordered single-line input field showing its value text and a caret — `text-box`. The
    /// frame + caret are what distinguish an editable field from a static label.
    let private textFieldGeom theme (box: Rect) (value: string) : Scene list =
        let h = min box.Height 40.0
        let by = box.Y + box.Height / 2.0 - h / 2.0
        let field: Rect = { X = box.X; Y = by; Width = box.Width; Height = h }
        let textX = box.X + 10.0
        let baseline = by + h / 2.0 + 5.0
        let textW = (Scene.measureText value { Family = theme.FontFamily; Size = 15.0; Weight = None }).Width
        let caretX = min (box.X + box.Width - 8.0) (textX + textW + 3.0)
        [ Scene.rectangle (box.X, by, box.Width, h) theme.Background
          Scene.rectangleWithPaint field (Paint.stroke theme.Foreground 2.0)
          Scene.clipped
              (RectClip field)
              (mkText theme textX baseline 15.0 theme.Foreground value)
          Scene.line { X = caretX; Y = by + 7.0 } { X = caretX; Y = by + h - 7.0 } (Paint.stroke theme.Accent 2.0) ]

    /// A bordered multi-line input field showing each value line plus a caret — `text-area`.
    let private textAreaFieldGeom theme (box: Rect) (value: string) : Scene list =
        let lineH = 22.0
        let lines = value.Replace("\r\n", "\n").Split('\n') |> Array.toList |> List.truncate 4
        let firstBaseline = box.Y + 22.0
        let texts =
            lines
            |> List.mapi (fun i ln -> mkText theme (box.X + 10.0) (firstBaseline + float i * lineH) 14.0 theme.Foreground ln)
        let lastLine = lines |> List.tryLast |> Option.defaultValue ""
        let lastW = (Scene.measureText lastLine { Family = theme.FontFamily; Size = 14.0; Weight = None }).Width
        let caretX = min (box.X + box.Width - 8.0) (box.X + 10.0 + lastW + 3.0)
        let caretY = firstBaseline + float (max 0 (List.length lines - 1)) * lineH
        [ Scene.rectangle (box.X, box.Y, box.Width, box.Height) theme.Background
          Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 2.0)
          Scene.clipped (RectClip box) (Scene.group texts)
          Scene.line { X = caretX; Y = caretY - 13.0 } { X = caretX; Y = caretY + 3.0 } (Paint.stroke theme.Accent 2.0) ]

    /// Styled runs flowing left-to-right with per-run colour and weight — `rich-text`. Each run
    /// keeps its own `Foreground`/`Weight`, so the preview demonstrates rich formatting rather
    /// than collapsing to a single-colour label (or, pre-082, the kind id).
    let private richTextGeom theme (box: Rect) (runs: (string * Color * float * int) list) : Scene list =
        match runs with
        | [] -> emptyState theme box "(no runs)"
        | _ ->
            let baseline = box.Y + box.Height / 2.0 + 6.0
            runs
            |> List.fold
                (fun (x, acc) (text, fg, fontSize, weight) ->
                    let size = max 8.0 fontSize
                    let font: FontSpec = { Family = theme.FontFamily; Size = size; Weight = Some weight }
                    let w = (Scene.measureText text font).Width
                    let node = mkTextW theme x baseline size (Some weight) fg text
                    x + w, node :: acc)
                (box.X + 4.0, [])
            |> snd
            |> List.rev

    /// A horizontal divider rule centred in the canvas — `separator`.
    let private separatorGeom theme (box: Rect) : Scene list =
        let cy = box.Y + box.Height / 2.0
        [ Scene.line { X = box.X; Y = cy } { X = box.X + box.Width; Y = cy } (Paint.stroke theme.Foreground 3.0) ]

    /// Dispatch a rich-family control to its faithful geometry (within `box`, below the title).
    let faithfulContent (theme: Theme) (box: Rect) (control: Control<'msg>) : Scene list =
        let label = control.Content |> Option.defaultValue ""
        let items = stringListOf "items" control
        match control.Kind with
        | "line-chart" -> lineGeom theme box (chartValues control)
        | "bar-chart" -> barGeom theme box (chartValues control)
        | "pie-chart" -> pieGeom theme box (chartValues control)
        | "scatter-plot" -> scatterGeom theme box (chartValues control)
        | "graph-view" -> graphGeom theme box (chartValues control)
        | "list-view"
        | "list-box"
        | "multi-select-list"
        | "combo-box"
        | "tree-view"
        | "menu"
        | "context-menu" ->
            rowsGeom theme box (stringListOf "items" control) (stringListOf "selectedKeys" control |> Set.ofList)
        | "data-grid" -> gridGeom theme box (itemsOr [ "Name"; "Qty"; "Widget"; "12"; "Gadget"; "7" ] items)
        | "radio-group" -> radioGeom theme box (stringListOf "items" control) (textValueOf "value" control)
        | "tabs" -> tabsGeom theme box (stringListOf "items" control) (textValueOf "value" control)
        | "slider" -> sliderGeom theme box (floatValue "value" 0.5 control.Attributes)
        | "progress-bar" -> progressGeom theme box (floatValue "value" 0.0 control.Attributes)
        | "numeric-input" -> numericGeom theme box (floatValue "value" 0.0 control.Attributes)
        | "switch" -> switchGeom theme box (boolValue "selected" false control.Attributes)
        | "check-box" -> checkboxGeom theme box (boolValue "selected" false control.Attributes) label
        // command / button family
        | "button" -> buttonGeom theme box true label
        | "icon-button" -> buttonGeom theme box false label
        | "badge" -> badgeGeom theme box label
        | "toggle-button" -> toggleGeom theme box (boolValue "selected" true control.Attributes) label
        | "split-button" -> splitGeom theme box label
        // layout / container family
        | "stack" -> stackGeom theme box items
        | "grid" -> gridLayoutGeom theme box items
        | "dock" -> dockGeom theme box items
        | "wrap" -> wrapGeom theme box items
        | "split-view" -> splitViewGeom theme box items
        | "toolbar" -> toolbarGeom theme box items
        | "panel" -> panelGeom theme box (if label = "" then "Panel content" else label)
        | "border" -> borderGeom theme box (if label = "" then "Bordered" else label)
        | "scroll-viewer" -> scrollViewerGeom theme box (if label = "" then "Scrollable content" else label)
        | "overlay" -> overlayGeom theme box (if label = "" then "Overlaid content" else label)
        | "date-picker"
        | "time-picker" -> pickerGeom theme box (control.Content |> Option.defaultValue control.Kind)
        | "color-picker" -> swatchGeom theme box
        | "spinner" -> spinnerGeom theme box
        | "image" -> imageGeom theme box (textValueOf "value" control |> Option.defaultValue "image")
        // text-input / rich-text / divider family (feature 082)
        | "text-box" -> textFieldGeom theme box (textValueOf "value" control |> Option.defaultValue "")
        | "text-area" -> textAreaFieldGeom theme box (textValueOf "value" control |> Option.defaultValue "")
        | "rich-text" -> richTextGeom theme box (richTextRuns control)
        | "separator" -> separatorGeom theme box
        | "icon" ->
            let name =
                control.Content
                |> Option.orElseWith (fun () -> textValueOf "text" control)
                |> Option.defaultValue "icon"
            iconGeom theme box name
        | other -> emptyState theme box other

    let renderNode (theme: Theme) y (control: Control<'msg>) =
        let width = nodeWidth control
        let height = nodeHeight control
        let visible = boolValue "visible" true control.Attributes
        let label = control.Content |> Option.defaultValue control.Kind

        if not visible then
            Scene.group [ Scene.rectangle (0.0, y, width, height) Colors.transparent ]
        elif Set.contains control.Kind richFamilies then
            // Title band on top; control-specific geometry below it (within the canvas).
            let pad = 10.0
            let titleH = 30.0
            let box: Rect = { X = pad; Y = y + titleH; Width = width - 2.0 * pad; Height = height - titleH - pad }
            // Title band shows the control's NAME (the schematic below shows its content); this
            // fixes composite-lowering title bleed and content duplication for rich families.
            let title =
                Scene.clipped
                    (RectClip { X = 0.0; Y = y; Width = width; Height = titleH })
                    (mkText theme 8.0 (y + 19.0) 13.0 theme.Foreground (prettyKind control.Kind))
            Scene.group (title :: faithfulContent theme box control)
        else
            // Text / container controls: the control IS its text, so box + clipped label is faithful.
            let fill =
                if disabledOrReadOnly control then theme.Muted
                elif boolValue "selected" false control.Attributes then theme.Accent
                else theme.Background
            let fontSize = fittedFontSize theme.FontSize 6.0 width height theme.FontFamily label
            let textY = y + (height + fontSize) * 0.5 - 3.0
            let labelRun =
                { Text = label
                  Position = { X = 8.0; Y = textY }
                  Font = { Family = theme.FontFamily; Size = fontSize; Weight = None }
                  Paint = Paint.fill theme.Foreground }
            Scene.group [
                Scene.rectangle (0.0, y, width, height) fill
                Scene.clipped
                    (RectClip { X = 0.0; Y = y; Width = width; Height = height })
                    (Scene.textRun labelRun)
            ]

    let renderScene (theme: Theme) (control: Control<'msg>) =
        let controls = recursively (fun control -> [ control ]) control

        ((0.0, []), controls)
        ||> List.fold (fun (y, scenes) control ->
            let height = nodeHeight control
            y + height + 4.0, renderNode theme y control :: scenes)
        |> snd
        |> List.rev
        |> Scene.group

    let rec layoutNode (theme: Theme) (control: Control<'msg>) : FS.Skia.UI.Layout.LayoutNode =
        let id = control.Key |> Option.defaultValue control.Kind
        let width = floatValue "width" 240.0 control.Attributes
        let height = floatValue "height" 28.0 control.Attributes
        let content = renderScene theme control
        let children = control.Children |> List.map (layoutNode theme)

        { LayoutDefaults.layoutNode id with
            Intent =
                { LayoutDefaults.layoutIntent with
                    Size = { Width = Some width; Height = Some height } }
            Content = Some content
            Children = children }

    let duplicateDiagnostics (control: Control<'msg>) =
        control.Attributes
        |> List.countBy _.Name
        |> List.choose (fun (name, count) ->
            if count > 1 then
                Some(Diagnostics.duplicateAttribute control.Key control.Kind name)
            else
                None)

    let requiredDiagnostics (control: Control<'msg>) =
        required control.Kind
        |> List.choose (fun name ->
            if hasAttr name control.Attributes then
                None
            else
                Some(Diagnostics.missingRequired control.Key control.Kind name))

    let keyDiagnostics (control: Control<'msg>) =
        recursively (fun control -> [ control ]) control
        |> List.choose (fun control -> control.Key |> Option.map (fun key -> key, control.Kind))
        |> List.groupBy fst
        |> List.collect (fun (key, rows) ->
            if rows.Length > 1 then
                rows |> List.tail |> List.map (fun (_, kind) -> Diagnostics.keyCollision key kind)
            else
                [])

    let controlDiagnostics (control: Control<'msg>) =
        duplicateDiagnostics control
        @ requiredDiagnostics control
        @ Accessibility.validate control

module Control =
    let create kind (attrs: Attr<'msg> list) =
        let text = ControlInternals.textFrom attrs
        let children = ControlInternals.childrenFrom attrs

        { Kind = kind
          Key = None
          Attributes = attrs
          Children = children
          Content = text
          Accessibility = ControlInternals.accessibility kind attrs text }

    let standard kind attrs =
        create (StandardControlKindHelpers.toControlKind kind) attrs

    let customControl kind attrs =
        create kind attrs

    let lowerStandard (control: Control<'msg>) =
        control

    let lowerCustom (control: Control<'msg>) =
        control

    let withKey key (control: Control<'msg>) =
        { control with Key = Some key }

    let rec count (control: Control<'msg>) =
        1 + (control.Children |> List.sumBy count)

    let diagnostics (control: Control<'msg>) =
        ControlInternals.recursively ControlInternals.controlDiagnostics control
        @ ControlInternals.keyDiagnostics control

    let render (theme: Theme) (control: Control<'msg>) =
        { Scene = ControlInternals.renderScene theme control
          Layout = ControlInternals.layoutNode theme control
          Diagnostics = diagnostics control
          EventBindings = ControlInternals.recursively ControlInternals.eventBindings control
          NodeCount = count control }

    // Feature 085 (FR-001/FR-002/FR-003) — faithful NESTED-tree renderer.
    //
    // Unlike `render` (the 080 single-control preview, which flattens every descendant and
    // stacks them at fixed y offsets), `renderTree` runs a REAL recursive Yoga layout over the
    // nested tree at the supplied output `size`, then paints every node — containers AND their
    // children — at its COMPUTED bounds. Two structurally different trees therefore produce
    // visibly different scenes (SC-001). `render`/`Widget.render` are left untouched (FR-003).
    let renderTree (theme: Theme) (size: FS.Skia.UI.Scene.Size) (control: Control<'msg>) =
        let directionOf kind =
            match kind with
            | "toolbar"
            | "split-view"
            | "wrap"
            | "grid"
            | "dock" -> FS.Skia.UI.Layout.Row
            | _ -> FS.Skia.UI.Layout.Column

        let wrapOf kind =
            match kind with
            | "wrap"
            | "grid" -> FS.Skia.UI.Layout.Wrap
            | _ -> FS.Skia.UI.Layout.NoWrap

        // Build the nested layout tree: leaves carry an explicit size (their preview geometry),
        // containers let direction + children drive arrangement. Content is painted afterwards at
        // the computed bounds, so each node keeps Content = None here.
        let rec toLayout (c: Control<'msg>) : FS.Skia.UI.Layout.LayoutNode =
            let id = c.Key |> Option.defaultValue c.Kind
            let isLeaf = List.isEmpty c.Children

            let size: FS.Skia.UI.Layout.LayoutSize =
                if isLeaf then
                    { Width = Some(ControlInternals.nodeWidth c)
                      Height = Some(ControlInternals.nodeHeight c) }
                else
                    { Width =
                        (if ControlInternals.hasAttr "width" c.Attributes then
                             Some(ControlInternals.nodeWidth c)
                         else
                             None)
                      Height =
                        (if ControlInternals.hasAttr "height" c.Attributes then
                             Some(ControlInternals.nodeHeight c)
                         else
                             None) }

            { LayoutDefaults.layoutNode id with
                Intent =
                    { LayoutDefaults.layoutIntent with
                        Direction = directionOf c.Kind
                        Wrap = wrapOf c.Kind
                        Gap = { Row = 8.0; Column = 8.0 }
                        Padding = { Left = 8.0; Top = 8.0; Right = 8.0; Bottom = 8.0 }
                        Size = size }
                Children = c.Children |> List.map toLayout }

        let root = toLayout control

        let available: FS.Skia.UI.Layout.AvailableSpace =
            { Width = float size.Width
              WidthMode = FS.Skia.UI.Layout.Exactly
              Height = float size.Height
              HeightMode = FS.Skia.UI.Layout.Exactly }

        let result = FS.Skia.UI.Layout.Layout.evaluate available root

        let boundsById =
            result.Bounds
            |> List.map (fun (b: FS.Skia.UI.Layout.ComputedBounds) -> b.NodeId, b.Bounds)
            |> Map.ofList

        let paintLeaf (box: Rect) (c: Control<'msg>) : Scene list =
            if Set.contains c.Kind ControlInternals.richFamilies then
                // chart/button/field/etc. — its faithful per-control geometry, drawn into `box`.
                ControlInternals.faithfulContent theme box c
            else
                // text / static controls: the control IS its text, so a filled box + clipped label.
                let label = c.Content |> Option.defaultValue c.Kind

                let fill =
                    if ControlInternals.disabledOrReadOnly c then theme.Muted
                    elif ControlInternals.boolValue "selected" false c.Attributes then theme.Accent
                    else theme.Background

                let fontSize =
                    ControlInternals.fittedFontSize theme.FontSize 6.0 box.Width box.Height theme.FontFamily label

                let textY = box.Y + (box.Height + fontSize) * 0.5 - 3.0

                let labelRun =
                    { Text = label
                      Position = { X = box.X + 8.0; Y = textY }
                      Font = { Family = theme.FontFamily; Size = fontSize; Weight = None }
                      Paint = Paint.fill theme.Foreground }

                [ Scene.rectangle (box.X, box.Y, box.Width, box.Height) fill
                  Scene.clipped (RectClip box) (Scene.textRun labelRun) ]

        let rec paint (c: Control<'msg>) : Scene list =
            let id = c.Key |> Option.defaultValue c.Kind

            let here =
                match Map.tryFind id boundsById with
                | None -> []
                | Some(b: FS.Skia.UI.Layout.LayoutBounds) ->
                    let box: Rect = { X = b.X; Y = b.Y; Width = b.Width; Height = b.Height }

                    if List.isEmpty c.Children then
                        paintLeaf box c
                    else
                        // Container: a faint frame so the nesting is visible; the real children
                        // are painted (below) at their own computed bounds.
                        [ Scene.rectangleWithPaint box (Paint.stroke theme.Muted 1.0) ]

            here @ (c.Children |> List.collect paint)

        { Scene = paint control |> Scene.group
          Layout = root
          Diagnostics = diagnostics control
          EventBindings = ControlInternals.recursively ControlInternals.eventBindings control
          NodeCount = count control }

    let dispatch (event: ControlEvent) (control: Control<'msg>) =
        let rec loop (current: Control<'msg>) =
            let own =
                if ControlInternals.disabledOrReadOnly current then
                    []
                else
                    ControlInternals.eventBindings current
                    |> List.filter (fun binding ->
                        binding.EventKind = event.Kind
                        && (event.ControlId.IsNone || event.ControlId = Some binding.ControlId))
                    |> List.map (fun binding -> binding.Dispatch event)

            own @ (current.Children |> List.collect loop)

        loop control |> List.truncate 1

module TextBlock =
    let create attrs = Control.create "text-block" attrs
    let text value = Attr.text value

module Label =
    let create attrs = Control.create "label" attrs
    let text value = Attr.text value

module Image =
    let create attrs = Control.create "image" attrs
    let source value = Attr.value value

module Icon =
    let create attrs = Control.create "icon" attrs
    let name value = Attr.text value

module Separator =
    let create attrs = Control.create "separator" attrs

module Badge =
    let create attrs = Control.create "badge" attrs
    let text value = Attr.text value

module Button =
    let create attrs = Control.create "button" attrs
    let text value = Attr.text value
    let enabled value = Attr.enabled value
    let onClick msg = Attr.on "onClick" msg
    let onClickWith map = Attr.onWith "onClick" map

module IconButton =
    let create attrs = Control.create "icon-button" attrs
    let icon value = Attr.text value
    let onClick msg = Attr.on "onClick" msg

module CheckBox =
    let create attrs = Control.create "check-box" attrs
    let text value = Attr.text value
    let checked' value = Attr.selected value
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.exists ((=) "true") |> map)

module Switch =
    let create attrs = Control.create "switch" attrs
    let checked' value = Attr.selected value
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.exists ((=) "true") |> map)

module Slider =
    let create attrs = Control.create "slider" attrs
    let value value = Attr.create "value" Content (FloatValue value)
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.bind (fun value -> match Double.TryParse value with true, parsed -> Some parsed | _ -> None) |> Option.defaultValue 0.0 |> map)

module NumericInput =
    let create attrs = Control.create "numeric-input" attrs
    let value value = Attr.create "value" Content (FloatValue value)
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.bind (fun value -> match Double.TryParse value with true, parsed -> Some parsed | _ -> None) |> Option.defaultValue 0.0 |> map)

module TextBox =
    let create attrs = Control.create "text-box" attrs
    let value value = Attr.value value
    let readOnly value = Attr.readOnly value
    let validation state = Attr.validation state
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.defaultValue "" |> map)

module TextArea =
    let create attrs = Control.create "text-area" attrs
    let value value = Attr.value value
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.defaultValue "" |> map)

module RadioGroup =
    let create attrs = Control.create "radio-group" attrs
    let items values = Attr.items values
    let selected value = Attr.value value
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.defaultValue "" |> map)

module Stack =
    let create attrs = Control.create "stack" attrs
    let children controls = Attr.children controls

module Grid =
    let create attrs = Control.create "grid" attrs
    let children controls = Attr.children controls

module Dock =
    let create attrs = Control.create "dock" attrs
    let children controls = Attr.children controls

module Wrap =
    let create attrs = Control.create "wrap" attrs
    let children controls = Attr.children controls

module Border =
    let create attrs = Control.create "border" attrs
    let child control = Attr.child control

module Panel =
    let create attrs = Control.create "panel" attrs
    let children controls = Attr.children controls

module ProgressBar =
    let create attrs = Control.create "progress-bar" attrs
    let value value = Attr.create "value" Content (FloatValue value)

module Spinner =
    let create attrs = Control.create "spinner" attrs

module ValidationMessage =
    let create attrs = Control.create "validation-message" attrs
    let text value = Attr.text value

module Tabs =
    let create attrs = Control.create "tabs" attrs
    let items values = Attr.items values
    let selected value = Attr.value value
    let onChanged map = Attr.onWith "onChanged" (fun event -> event.Payload |> Option.defaultValue "" |> map)

module Menu =
    let create attrs = Control.create "menu" attrs
    let items values = Attr.items values
    let onSelected map = Attr.onWith "onSelected" (fun event -> event.Payload |> Option.defaultValue "" |> map)

module Toolbar =
    let create attrs = Control.create "toolbar" attrs
    let children controls = Attr.children controls

module Tooltip =
    let create attrs = Control.create "tooltip" attrs
    let text value = Attr.text value

module Dialog =
    let create attrs = Control.create "dialog" attrs
    let children controls = Attr.children controls

module Toast =
    let create attrs = Control.create "toast" attrs
    let text value = Attr.text value

module Overlay =
    let create attrs = Control.create "overlay" attrs
    let child control = Attr.child control
