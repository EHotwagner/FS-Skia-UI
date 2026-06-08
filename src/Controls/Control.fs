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
              "slider"; "progress-bar"; "numeric-input"; "switch"; "check-box"; "toggle-button"
              "date-picker"; "time-picker"; "color-picker"; "spinner"; "image"; "icon" ]

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

    let private scatterGeom theme (box: Rect) (pts: ChartPoint list) : Scene list =
        match pts with
        | [] -> emptyState theme box "(no data)"
        | _ ->
            let xs = pts |> List.map (fun p -> p.X)
            let ys = pts |> List.map (fun p -> p.Y)
            let minX, maxX = List.min xs, List.max xs
            let minY, maxY = List.min ys, List.max ys
            let sx = if maxX - minX < 1e-9 then 1.0 else maxX - minX
            let sy = if maxY - minY < 1e-9 then 1.0 else maxY - minY
            pts
            |> List.map (fun p ->
                let cx = box.X + (p.X - minX) / sx * box.Width
                let cy = box.Y + box.Height - (p.Y - minY) / sy * box.Height
                Scene.circle { X = cx; Y = cy } 4.5 theme.Accent)

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
                |> List.map (fun (a, b) -> Scene.line a b (Paint.stroke theme.Muted 1.5))
            let nodes = positions |> List.map (fun p -> Scene.circle p 7.0 theme.Accent)
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

    /// Generic tabular chrome for `data-grid` (its typed `DataGridRow` cells are not reachable
    /// from this file's compile position, so the preview shows a faithful header+rows+columns
    /// grid structure rather than a label-on-a-box; the prose describes it as a grid layout).
    let private gridGeom theme (box: Rect) : Scene list =
        let cols = 2
        let rows = 3
        let cw = box.Width / float cols
        let rh = box.Height / float (rows + 1)
        let frame = Scene.rectangleWithPaint box (Paint.stroke theme.Foreground 1.5)
        let header = Scene.rectangle (box.X, box.Y, box.Width, rh) theme.Muted
        let rowLines =
            [ for r in 1..rows -> Scene.line { X = box.X; Y = box.Y + float r * rh } { X = box.X + box.Width; Y = box.Y + float r * rh } (Paint.stroke theme.Muted 1.0) ]
        let colLines =
            [ for c in 1 .. cols - 1 -> Scene.line { X = box.X + float c * cw; Y = box.Y } { X = box.X + float c * cw; Y = box.Y + box.Height } (Paint.stroke theme.Muted 1.0) ]
        frame :: header :: (rowLines @ colLines)

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

    let private checkboxGeom theme (box: Rect) (on: bool) : Scene list =
        let s = 24.0
        let bx = box.X
        let by = box.Y + box.Height / 2.0 - s / 2.0
        let frame = Scene.rectangleWithPaint { X = bx; Y = by; Width = s; Height = s } (Paint.stroke theme.Foreground 2.0)
        let tick =
            if on then
                [ Scene.line { X = bx + 4.0; Y = by + 12.0 } { X = bx + 10.0; Y = by + 18.0 } (Paint.stroke theme.Accent 3.0)
                  Scene.line { X = bx + 10.0; Y = by + 18.0 } { X = bx + 20.0; Y = by + 5.0 } (Paint.stroke theme.Accent 3.0) ]
            else
                []
        frame :: tick

    let private toggleGeom theme (box: Rect) (on: bool) : Scene list =
        [ Scene.rectangle (box.X, box.Y, box.Width * 0.5, 30.0) (if on then theme.Accent else theme.Muted) ]

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
        let r = (min box.Width box.Height) / 2.0 - 4.0
        let cx = box.X + box.Width / 2.0
        let cy = box.Y + box.Height / 2.0
        let bounds: Rect = { X = cx - r; Y = cy - r; Width = 2.0 * r; Height = 2.0 * r }
        [ Scene.arc bounds -90.0 270.0 (Paint.stroke theme.Accent 4.0) ]

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

    /// Dispatch a rich-family control to its faithful geometry (within `box`, below the title).
    let faithfulContent (theme: Theme) (box: Rect) (control: Control<'msg>) : Scene list =
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
        | "data-grid" -> gridGeom theme box
        | "radio-group" -> radioGeom theme box (stringListOf "items" control) (textValueOf "value" control)
        | "tabs" -> tabsGeom theme box (stringListOf "items" control) (textValueOf "value" control)
        | "slider" -> sliderGeom theme box (floatValue "value" 0.5 control.Attributes)
        | "progress-bar" -> progressGeom theme box (floatValue "value" 0.0 control.Attributes)
        | "numeric-input" -> numericGeom theme box (floatValue "value" 0.0 control.Attributes)
        | "switch" -> switchGeom theme box (boolValue "selected" false control.Attributes)
        | "check-box" -> checkboxGeom theme box (boolValue "selected" false control.Attributes)
        | "toggle-button" -> toggleGeom theme box (boolValue "selected" false control.Attributes)
        | "date-picker"
        | "time-picker" -> pickerGeom theme box (control.Content |> Option.defaultValue control.Kind)
        | "color-picker" -> swatchGeom theme box
        | "spinner" -> spinnerGeom theme box
        | "image" -> imageGeom theme box (textValueOf "value" control |> Option.defaultValue "image")
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
            let title =
                Scene.clipped
                    (RectClip { X = 0.0; Y = y; Width = width; Height = titleH })
                    (mkText theme 8.0 (y + 19.0) 13.0 theme.Foreground label)
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
