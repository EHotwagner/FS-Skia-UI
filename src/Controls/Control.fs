namespace FS.Skia.UI.Controls

open System
open FS.Skia.UI.Scene

module LayoutDefaults = FS.Skia.UI.Layout.Defaults

module ControlInternals =
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

    let chartValues (control: Control<'msg>) =
        let floatValues name =
            tryLast name control.Attributes
            |> Option.bind (fun attr ->
                match attr.Value with
                | UntypedValue(:? (float list) as values) -> Some values
                | UntypedValue(:? (float array) as values) -> Some(Array.toList values)
                | FloatValue value -> Some [ value ]
                | _ -> None)

        match control.Kind with
        | "line-chart"
        | "bar-chart"
        | "scatter-plot" ->
            floatValues "series" |> Option.defaultValue []
        | "pie-chart" ->
            floatValues "values" |> Option.defaultValue []
        | "graph-view" ->
            tryLast "nodes" control.Attributes
            |> Option.bind (fun attr ->
                match attr.Value with
                | StringListValue values -> Some(values |> List.mapi (fun index _ -> float index))
                | _ -> None)
            |> Option.defaultValue []
        | _ -> []

    let isChartLike kind =
        match kind with
        | "line-chart"
        | "bar-chart"
        | "pie-chart"
        | "scatter-plot"
        | "graph-view" -> true
        | _ -> false

    let renderNode (theme: Theme) y (control: Control<'msg>) =
        let width = floatValue "width" 240.0 control.Attributes
        let height = max 20.0 (floatValue "height" 24.0 control.Attributes)
        let visible = boolValue "visible" true control.Attributes
        let fill =
            if not visible then
                Colors.transparent
            elif disabledOrReadOnly control then
                theme.Muted
            elif boolValue "selected" false control.Attributes then
                theme.Accent
            else
                theme.Background

        let label = control.Content |> Option.defaultValue control.Kind
        let fontSize = fittedFontSize theme.FontSize 6.0 width height theme.FontFamily label
        let textY = y + (height + fontSize) * 0.5 - 3.0
        let labelRun =
            { Text = label
              Position = { X = 8.0; Y = textY }
              Font = { Family = theme.FontFamily; Size = fontSize; Weight = None }
              Paint = Paint.fill theme.Foreground }

        if isChartLike control.Kind then
            Scene.group [
                Scene.rectangle (0.0, y, width, height) fill
                Scene.chart (chartValues control)
                Scene.clipped
                    (RectClip { X = 0.0; Y = y; Width = width; Height = height })
                    (Scene.textRun labelRun)
            ]
        else
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
            let height = max 20.0 (floatValue "height" 24.0 control.Attributes)
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
