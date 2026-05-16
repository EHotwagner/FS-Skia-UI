module DemoReel.Program

open System
open System.Diagnostics
open System.IO
open Elmish
open FS.Skia.UI
open FS.Skia.UI.Controls

module LayoutDefaults = FS.Skia.UI.Layout.Defaults

type Model =
    { Time: float
      ManualSlide: int option
      Size: Size
      Name: string
      Notes: string
      CanSave: bool
      DarkMode: bool
      SliderValue: float
      Quantity: float
      Mode: string
      SelectedTab: string
      SelectedMenu: string
      Collection: CollectionModel
      Diagnostics: RenderDiagnostic list
      ActionLog: string list }

type Msg =
    | ViewerInput of ViewerEvent
    | Advance of float
    | NextSlide
    | PreviousSlide
    | Restart
    | NameChanged of string
    | NotesChanged of string
    | SaveRequested
    | ToggleCanSave
    | CanSaveChanged of bool
    | DarkModeChanged of bool
    | SliderChanged of float
    | QuantityChanged of float
    | ModeChanged of string
    | TabChanged of string
    | MenuSelected of string
    | CollectionMsg of CollectionMsg
    | HostEffect of ViewerEffect<Msg>
    | NoOp

let width = 1280.0
let height = 720.0
let initialSize: Size = { Width = int width; Height = int height }
let slideCount = 7
let totalDuration = 63.0

let clampByte value =
    Math.Clamp(int value, 0, 255) |> byte

let rgba r g b a =
    Colors.rgba (clampByte r) (clampByte g) (clampByte b) (clampByte a)

let wave phase low high =
    low + (high - low) * ((sin phase + 1.0) * 0.5)

let waveColor phase =
    rgba (wave phase 60.0 245.0) (wave (phase + 2.1) 80.0 235.0) (wave (phase + 4.2) 95.0 255.0) 255.0

let textPaint size weight color =
    { Text = ""
      Position = { X = 0.0; Y = 0.0 }
      Font = { Family = None; Size = size; Weight = weight }
      Paint = Paint.fill color }

let textAt x y size weight color text =
    Scene.textRun { textPaint size weight color with Text = text; Position = { X = x; Y = y } }

let fittedTextAt x y maxWidth maxSize minSize weight color (text: string) =
    let fits size =
        let metrics =
            Scene.measureText
                text
                { Family = None
                  Size = size
                  Weight = weight }

        metrics.Width <= maxWidth

    let fittedSize =
        if fits maxSize then
            maxSize
        else
            let mutable low = minSize
            let mutable high = maxSize

            for _ in 0 .. 7 do
                let mid = (low + high) * 0.5

                if fits mid then
                    low <- mid
                else
                    high <- mid

            low

    textAt x y fittedSize weight color text

let titleText x y text size =
    textAt x y size (Some 700) Colors.white text

let labelText x y text =
    textAt x y 16.0 (Some 600) (rgba 232.0 238.0 250.0 255.0) text

let smallText x y text =
    textAt x y 13.0 (Some 400) (rgba 178.0 191.0 216.0 255.0) text

let identityPerspective =
    { M11 = 1.0
      M12 = 0.0
      M13 = 0.0
      M21 = 0.0
      M22 = 1.0
      M23 = 0.0
      M31 = 0.0
      M32 = 0.0
      M33 = 1.0 }

let translate x y scene =
    Scene.withPerspective { identityPerspective with M13 = x; M23 = y } scene

let panel x y w h =
    Scene.group [
        Scene.rectangle (x, y, w, h) (rgba 18.0 24.0 39.0 235.0)
        Scene.rectangleWithPaint { X = x; Y = y; Width = w; Height = h } (Paint.stroke (rgba 110.0 130.0 165.0 120.0) 1.0)
    ]

let background t =
    let rays =
        [ for i in 0 .. 34 ->
              let fi = float i
              let angle = fi * Math.PI * 2.0 / 35.0 + t * 0.16
              let r1 = 70.0 + wave (t + fi) 0.0 30.0
              let r2 = 680.0 + wave (t * 0.7 + fi) (-35.0) 60.0
              Scene.line
                  { X = width * 0.54 + cos angle * r1; Y = height * 0.54 + sin angle * r1 }
                  { X = width * 0.54 + cos angle * r2; Y = height * 0.54 + sin angle * r2 }
                  (Paint.stroke (waveColor (t + fi * 0.18)) 1.0 |> Paint.withOpacity 0.28) ]

    Scene.group [
        Scene.rectangle (0.0, 0.0, width, height) (rgba 7.0 10.0 21.0 255.0)
        yield! rays
        Scene.ellipse { X = 872.0 + wave (t * 0.6) (-48.0) 44.0; Y = 88.0; Width = 250.0; Height = 250.0 } (Paint.fill (rgba 51.0 113.0 255.0 80.0) |> Paint.withMaskFilter (MaskFilter.Blur 18.0) |> Paint.withBlendMode BlendMode.Screen)
        Scene.ellipse { X = 104.0; Y = 436.0 + wave (t * 0.5) (-30.0) 40.0; Width = 300.0; Height = 300.0 } (Paint.fill (rgba 58.0 214.0 148.0 55.0) |> Paint.withMaskFilter (MaskFilter.Blur 20.0) |> Paint.withBlendMode BlendMode.Screen)
    ]

let header title subtitle =
    Scene.group [
        Scene.rectangle (0.0, 0.0, width, 74.0) (rgba 9.0 13.0 27.0 245.0)
        titleText 36.0 44.0 title 27.0
        fittedTextAt 38.0 64.0 900.0 13.0 9.0 (Some 400) (rgba 178.0 191.0 216.0 255.0) subtitle
        fittedTextAt 976.0 42.0 278.0 13.0 9.0 (Some 400) (rgba 178.0 191.0 216.0 255.0) "Next: Right/Space  Prev: Left"
        fittedTextAt 976.0 62.0 278.0 13.0 9.0 (Some 400) (rgba 178.0 191.0 216.0 255.0) "Enter save  T type  S/D toggles"
    ]

let progress elapsed =
    let pct = Math.Clamp(elapsed / totalDuration, 0.0, 1.0)
    Scene.group [
        Scene.rectangle (0.0, height - 5.0, width, 5.0) (rgba 32.0 39.0 56.0 255.0)
        Scene.rectangle (0.0, height - 5.0, width * pct, 5.0) (rgba 91.0 170.0 245.0 255.0)
    ]

let themeFor model =
    let baseTheme = if model.DarkMode then Theme.dark else Theme.light
    baseTheme
    |> Theme.withDensity (0.9 + model.SliderValue * 0.25)
    |> Theme.withAccent (if model.DarkMode then rgba 125.0 211.0 252.0 255.0 else rgba 37.0 99.0 235.0 255.0)

let log message model =
    { model with ActionLog = (message :: model.ActionLog) |> List.truncate 7 }

let init () : Model * Cmd<Msg> =
    let collection, _ = Collections.init "catalog-data" 10_000 24.0 240.0

    { Time = 0.0
      ManualSlide = None
      Size = initialSize
      Name = "Ada Lovelace"
      Notes = "Model-owned plain text"
      CanSave = true
      DarkMode = true
      SliderValue = 0.42
      Quantity = 3.0
      Mode = "Form"
      SelectedTab = "Overview"
      SelectedMenu = "Inspect"
      Collection = collection
      Diagnostics = []
      ActionLog = [ "Loaded Controls demo reel" ] },
    Cmd.ofMsg (HostEffect InitializeRenderer)

let chartSeries t name phase =
    { Name = name
      Points =
        [ for i in 0 .. 13 ->
              { X = float i
                Y = 55.0 + wave (float i * 0.65 + t + phase) (-24.0) 36.0
                Label = None } ] }

let chartPoints t =
    [ for i in 0 .. 5 ->
          { X = float i
            Y = 20.0 + wave (t + float i * 0.9) 0.0 60.0
            Label = Some $"S{i + 1}" } ]

let formControls model =
    Stack.create [
        Attr.width 500.0
        Stack.children [
            TextBlock.create [ TextBlock.text "Form workflow: model -> view -> message"; Attr.width 500.0 ]
            Label.create [ Label.text "Profile name" ]
            TextBox.create [
                TextBox.value model.Name
                TextBox.validation (if model.Name.Trim() = "" then Invalid "Name required" else Valid)
                TextBox.onChanged NameChanged
                Attr.width 420.0
            ]
            |> Control.withKey "profile-name"
            TextArea.create [
                TextArea.value model.Notes
                TextArea.onChanged NotesChanged
                Attr.width 420.0
                Attr.height 44.0
            ]
            |> Control.withKey "notes"
            Button.create [
                Button.text (sprintf "Save profile (%.0f)" model.Quantity)
                Button.enabled model.CanSave
                Button.onClick SaveRequested
                Attr.width 240.0
            ]
            |> Control.withKey "save-button"
            IconButton.create [ IconButton.icon "spark"; IconButton.onClick NextSlide; Attr.width 180.0 ]
            |> Control.withKey "icon-next"
            CheckBox.create [
                CheckBox.text "Can save"
                CheckBox.checked' model.CanSave
                CheckBox.onChanged CanSaveChanged
            ]
            |> Control.withKey "can-save-check"
            Switch.create [ Switch.checked' model.DarkMode; Switch.onChanged DarkModeChanged ]
            |> Control.withKey "theme-switch"
            Slider.create [ Slider.value model.SliderValue; Slider.onChanged SliderChanged ]
            |> Control.withKey "density-slider"
            NumericInput.create [ NumericInput.value model.Quantity; NumericInput.onChanged QuantityChanged ]
            |> Control.withKey "quantity"
            RadioGroup.create [
                RadioGroup.items [ "Form"; "Dashboard"; "Data"; "Quality" ]
                RadioGroup.selected model.Mode
                RadioGroup.onChanged ModeChanged
            ]
            |> Control.withKey "mode"
            ValidationMessage.create [
                ValidationMessage.text (if model.CanSave then "Validation passed" else "Save is disabled")
                Attr.width 420.0
            ]
            ProgressBar.create [ ProgressBar.value model.SliderValue; Attr.width 420.0 ]
        ]
    ]
    |> Control.withKey "form-root"

let layoutNavigationControls model =
    Stack.create [
        Attr.width 520.0
        Stack.children [
            TextBlock.create [ TextBlock.text "Layout, navigation, and feedback"; Attr.width 520.0 ]
            Tabs.create [
                Tabs.items [ "Overview"; "Inputs"; "Charts"; "Diagnostics" ]
                Tabs.selected model.SelectedTab
                Tabs.onChanged TabChanged
            ]
            |> Control.withKey "tabs"
            Menu.create [
                Menu.items [ "Inspect"; "Render"; "Export"; "Reset" ]
                Menu.onSelected MenuSelected
            ]
            |> Control.withKey "menu"
            Toolbar.create [
                Toolbar.children [
                    Button.create [ Button.text "Save"; Button.onClick SaveRequested ] |> Control.withKey "toolbar-save"
                    IconButton.create [ IconButton.icon "refresh"; IconButton.onClick Restart ] |> Control.withKey "toolbar-restart"
                    Badge.create [ Badge.text $"Tab: {model.SelectedTab}" ]
                ]
            ]
            Grid.create [
                Grid.children [
                    Border.create [ Border.child (TextBlock.create [ TextBlock.text "Border child" ]) ]
                    Panel.create [
                        Panel.children [
                            Label.create [ Label.text "Panel" ]
                            Spinner.create []
                            Separator.create []
                        ]
                    ]
                ]
            ]
            Dock.create [
                Dock.children [
                    Image.create [ Image.source "placeholder://hero" ]
                    Icon.create [ Icon.name "chart-spline" ]
                    Badge.create [ Badge.text "new" ]
                ]
            ]
            Wrap.create [
                Wrap.children [
                    Tooltip.create [ Tooltip.text "Hover/focus help metadata" ]
                    Toast.create [ Toast.text "Toast: Saved" ]
                    Dialog.create [ Dialog.children [ TextBlock.create [ TextBlock.text "Dialog content" ]; Button.create [ Button.text "Close"; Button.onClick NoOp ] ] ]
                    Overlay.create [ Overlay.child (TextBlock.create [ TextBlock.text "Overlay child" ]) ]
                ]
            ]
        ]
    ]
    |> Control.withKey "layout-navigation-root"

let dataControls model t =
    Stack.create [
        Attr.width 520.0
        Stack.children [
            TextBlock.create [ TextBlock.text "Data, charts, graph, 10k list"; Attr.width 520.0 ]
            TextBlock.create [ TextBlock.text $"10k rows visible {model.Collection.VisibleRange.FirstIndex}..{model.Collection.VisibleRange.FirstIndex + model.Collection.VisibleRange.Count - 1}" ]
            LineChart.create [ LineChart.series [ chartSeries t "Revenue" 0.0; chartSeries t "Cost" 2.0 ]; Attr.width 420.0 ]
            BarChart.create [ BarChart.series [ chartSeries (t * 0.7) "Volume" 4.0 ]; Attr.width 420.0 ]
            PieChart.create [ PieChart.values (chartPoints t); Attr.width 420.0 ]
            ScatterPlot.create [ ScatterPlot.series [ chartSeries (t * 1.2) "Cluster" 6.0 ]; Attr.width 420.0 ]
            GraphView.create [ GraphView.nodes [ "model"; "view"; "dispatch"; "update"; "render" ]; Attr.width 420.0 ]
            Menu.create [
                Menu.items [ "row-001"; "row-128"; "row-2048"; "row-8192" ]
                Menu.onSelected (fun key -> CollectionMsg(SelectKey key))
            ]
        ]
    ]
    |> Control.withKey "data-root"

let customDefinition t =
    { Id = "custom-pulse"
      Render =
        fun () ->
            Scene.group [
                Scene.ellipse { X = 0.0; Y = 0.0; Width = 80.0; Height = 80.0 } (Paint.fill (waveColor t) |> Paint.withOpacity 0.85)
                Scene.text (14.0, 45.0) "custom" Colors.white
            ]
      Layout =
        fun () ->
            { LayoutDefaults.layoutNode "custom-pulse" with
                Intent =
                    { LayoutDefaults.layoutIntent with
                        Size = { Width = Some 120.0; Height = Some 42.0 } } }
      HitTest = fun x y -> x >= 0.0 && x <= 120.0 && y >= 0.0 && y <= 42.0
      Event = fun event -> if event.Kind = "click" then Some SaveRequested else None
      Accessibility = Some(Accessibility.defaultFor "custom-control" "Custom pulse")
      Diagnostics = [] }

let diagnosticControls t =
    let lowContrast =
        Accessibility.metadata
            AccessibilityRole.Button
            "Low contrast"
            [ "normal" ]
            (Some 4)
            (Accessibility.keyboard true [ "Enter"; "Space" ] [ "Tab" ])
            (Some(Accessibility.contrast Colors.black Colors.black 1.0 4.5))

    Stack.create [
        Attr.width 520.0
        Stack.children [
            TextBlock.create [ TextBlock.text "Diagnostics and extension points"; Attr.width 520.0 ]
            Button.create [ Button.text "First text"; Button.text "Duplicate text"; Button.onClick SaveRequested ]
            |> Control.withKey "duplicate-button"
            TextBox.create [] |> Control.withKey "missing-value"
            Button.create [ Button.text "Low contrast"; Attr.accessibility lowContrast; Button.onClick NoOp ]
            |> Control.withKey "low-contrast"
            CustomControl.create (customDefinition t) [ Attr.text "Custom pulse"; Attr.on "onCustom" SaveRequested ]
            ValidationMessage.create [ ValidationMessage.text "Expected diagnostics shown." ]
        ]
    ]
    |> Control.withKey "quality-root"

let catalogControls () =
    let rows =
        Catalog.supportedControls
        |> List.map (fun row ->
            TextBlock.create [
                TextBlock.text $"{row.DisplayName}  [{row.Category}]"
                Attr.width 460.0
            ])

    Stack.create [
        Attr.width 520.0
        Stack.children (
            TextBlock.create [ TextBlock.text $"Catalog source: {Catalog.supportedCount ()} supported rows"; Attr.width 520.0 ]
            :: rows
        )
    ]
    |> Control.withKey "catalog-root"

let featureRoot model t =
    Stack.create [
        Stack.children [
            formControls model
            layoutNavigationControls model
            dataControls model t
            diagnosticControls t
            catalogControls ()
        ]
    ]

let controlPanel x y w h model root =
    let rendered = Control.render (themeFor model) root
    let contentBounds =
        { X = x + 16.0
          Y = y + 18.0
          Width = max 0.0 (w - 32.0)
          Height = max 0.0 (h - 56.0) }

    Scene.group [
        panel x y w h
        Scene.clipped (RectClip contentBounds) (translate (x + 20.0) (y + 24.0) rendered.Scene)
        Scene.rectangle (x + 10.0, y + h - 34.0, w - 20.0, 24.0) (rgba 18.0 24.0 39.0 245.0)
        smallText (x + 20.0) (y + h - 20.0) $"nodes={rendered.NodeCount} diagnostics={rendered.Diagnostics.Length} bindings={rendered.EventBindings.Length}"
    ]

let metricCard x y label value accent =
    Scene.group [
        Scene.rectangle (x, y, 234.0, 96.0) (rgba 18.0 24.0 39.0 232.0)
        Scene.rectangleWithPaint { X = x; Y = y; Width = 234.0; Height = 96.0 } (Paint.stroke accent 1.2)
        smallText (x + 18.0) (y + 30.0) label
        titleText (x + 18.0) (y + 70.0) value 30.0
    ]

let logPanel model =
    let rows =
        model.ActionLog
        |> List.mapi (fun i item -> smallText 800.0 (424.0 + float i * 22.0) item)

    Scene.group [
        panel 760.0 376.0 458.0 214.0
        labelText 792.0 404.0 "Live event log"
        yield! rows
    ]

let introSlide model =
    let categorySummary =
        Catalog.supportedControls
        |> List.groupBy _.Category
        |> List.sortBy fst
        |> List.map (fun (category, rows) -> $"{category}: {rows.Length}")

    let categoryRows =
        categorySummary
        |> List.chunkBySize 5
        |> List.mapi (fun index row ->
            smallText 82.0 (430.0 + float index * 22.0) (String.concat "   " row))

    Scene.group [
        background model.Time
        header "Controls Demo Reel" "live window app for the new declarative Skia controls capability"
        titleText 72.0 170.0 "FS.Skia.UI.Controls" 60.0
        fittedTextAt 76.0 214.0 1110.0 20.0 12.0 (Some 500) (rgba 199.0 213.0 238.0 255.0) "Elmish view functions, typed attributes, model-owned state, diagnostics, catalog evidence"
        metricCard 78.0 278.0 "supported rows" $"{Catalog.supportedCount ()}" (rgba 96.0 165.0 250.0 255.0)
        metricCard 332.0 278.0 "categories" "10" (rgba 52.0 211.0 153.0 255.0)
        metricCard 586.0 278.0 "large data" "10k" (rgba 251.0 191.0 36.0 255.0)
        metricCard 840.0 278.0 "skills" "widgets" (rgba 248.0 113.0 113.0 255.0)
        yield! categoryRows
        controlPanel 78.0 482.0 560.0 150.0 model (Stack.create [ Stack.children [ Badge.create [ Badge.text "Charts and graphs"; Attr.width 420.0 ]; ProgressBar.create [ ProgressBar.value model.SliderValue; Attr.width 420.0 ]; Toast.create [ Toast.text "Generated apps use widgets"; Attr.width 420.0 ] ] ])
        logPanel model
        progress model.Time
    ]

let formSlide model =
    Scene.group [
        background model.Time
        header "Interactive Form Surface" "display, text entry, buttons, checkboxes, switches, sliders, numeric input, validation"
        controlPanel 52.0 102.0 572.0 552.0 model (formControls model)
        panel 680.0 112.0 520.0 242.0
        titleText 714.0 168.0 model.Name 34.0
        smallText 716.0 204.0 $"notes={model.Notes}"
        smallText 716.0 236.0 (sprintf "canSave=%b  dark=%b" model.CanSave model.DarkMode)
        smallText 716.0 264.0 (sprintf "density=%.2f  quantity=%.1f" model.SliderValue model.Quantity)
        smallText 716.0 292.0 $"mode={model.Mode}"
        fittedTextAt 716.0 316.0 440.0 13.0 9.0 (Some 400) (rgba 178.0 191.0 216.0 255.0) "Try Enter, T, S, D while the live window is focused."
        logPanel model
        progress model.Time
    ]

let layoutSlide model =
    Scene.group [
        background model.Time
        header "Layout, Navigation, Feedback" "Stack, Grid, Dock, Wrap, Border, Panel, Tabs, Menu, Toolbar, Tooltip, Dialog, Toast, Overlay"
        controlPanel 52.0 102.0 590.0 552.0 model (layoutNavigationControls model)
        panel 700.0 116.0 474.0 214.0
        titleText 734.0 172.0 model.SelectedTab 36.0
        smallText 736.0 210.0 $"selected menu action: {model.SelectedMenu}"
        smallText 736.0 242.0 "Declarative child order is preserved."
        smallText 736.0 266.0 "Create/attribute shape stays consistent."
        logPanel model
        progress model.Time
    ]

let dataSlide model =
    let range = model.Collection.VisibleRange
    let bars =
        [ for i in 0 .. 15 ->
              let h = wave (model.Time + float i * 0.7) 18.0 78.0
              Scene.rectangle (760.0 + float i * 24.0, 334.0 - h, 14.0, h) (waveColor (model.Time + float i * 0.4)) ]

    Scene.group [
        background model.Time
        header "Data, Charts, Graphs" "Controls-owned line, bar, pie, scatter, graph controls and 10,000-row visible ranges"
        controlPanel 52.0 102.0 590.0 552.0 model (dataControls model model.Time)
        panel 710.0 106.0 472.0 248.0
        labelText 744.0 146.0 "10,000-item collection"
        titleText 744.0 196.0 $"{range.FirstIndex}..{range.FirstIndex + range.Count - 1}" 44.0
        smallText 746.0 232.0 $"visible count={range.Count} total={range.Total} threshold={model.Collection.RecalculationThresholdMs}ms"
        yield! bars
        logPanel model
        progress model.Time
    ]

let qualitySlide model =
    let root = diagnosticControls model.Time
    let diagnostics = Control.diagnostics root
    let rows =
        diagnostics
        |> List.truncate 8
        |> List.mapi (fun i diagnostic ->
            smallText 736.0 (180.0 + float i * 24.0) $"{diagnostic.Code}: {diagnostic.ControlKind}")

    Scene.group [
        background model.Time
        header "Quality Gates And Extension" "accessibility metadata, deterministic diagnostics, custom control wrappers"
        controlPanel 52.0 102.0 590.0 552.0 model root
        panel 700.0 116.0 502.0 278.0
        labelText 736.0 152.0 "Expected validation diagnostics"
        yield! rows
        panel 700.0 424.0 502.0 120.0
        labelText 736.0 462.0 "Custom control wrapper"
        smallText 736.0 492.0 "Render, layout, hit-test, event hooks."
        smallText 736.0 516.0 "Accessibility and diagnostics metadata."
        progress model.Time
    ]

let catalogSlide model =
    let categories =
        Catalog.supportedControls
        |> List.groupBy _.Category
        |> List.sortBy fst

    let categoryBlocks =
        categories
        |> List.mapi (fun i (category, rows) ->
            let x = 680.0 + float (i % 2) * 250.0
            let y = 118.0 + float (i / 2) * 94.0
            Scene.group [
                Scene.rectangle (x, y, 224.0, 72.0) (rgba 18.0 24.0 39.0 232.0)
                labelText (x + 16.0) (y + 30.0) category
                titleText (x + 16.0) (y + 62.0) $"{rows.Length}" 26.0
            ])

    Scene.group [
        background model.Time
        header "Complete Catalog" "46 supported rows with purpose, attributes, events, visual states, accessibility, examples, tests, evidence"
        controlPanel 42.0 94.0 588.0 584.0 model (catalogControls ())
        yield! categoryBlocks
        smallText 684.0 626.0 "Catalog source: src/Controls/catalog.yml"
        smallText 684.0 650.0 "Evidence: catalog, interactions, rendering"
        progress model.Time
    ]

let finaleSlide model =
    let root = featureRoot model model.Time
    let rendered = Control.render (themeFor model) root

    Scene.group [
        background model.Time
        header "Ready For Product Screens" "default generated apps now receive Controls and fs-skia-ui-widgets"
        titleText 96.0 160.0 "All new Controls features" 40.0
        titleText 96.0 206.0 "in one live window" 40.0
        smallText 100.0 240.0 "Forms, layout, navigation, overlays, feedback, data, charts, graphs."
        smallText 100.0 264.0 "Accessibility, diagnostics, and custom wrappers."
        metricCard 102.0 284.0 "catalog rows" $"{Catalog.supportedCount ()}" (rgba 96.0 165.0 250.0 255.0)
        metricCard 356.0 284.0 "demo nodes" $"{rendered.NodeCount}" (rgba 52.0 211.0 153.0 255.0)
        metricCard 610.0 284.0 "event bindings" $"{rendered.EventBindings.Length}" (rgba 251.0 191.0 36.0 255.0)
        metricCard 864.0 284.0 "diagnostics" $"{rendered.Diagnostics.Length}" (rgba 248.0 113.0 113.0 255.0)
        panel 136.0 440.0 1008.0 134.0
        labelText 176.0 480.0 "Run"
        titleText 176.0 520.0 "dotnet run --project" 24.0
        titleText 176.0 552.0 "samples/DemoReel/DemoReel.fsproj" 24.0
        smallText 178.0 606.0 "Use Right/Space, Left, Enter, T, S, D, Up, Down, and R to drive the live model."
        progress model.Time
    ]

let sceneFor model =
    let automaticSlide =
        Math.Clamp(int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))), 0, slideCount - 1)

    let slide = model.ManualSlide |> Option.defaultValue automaticSlide

    match slide with
    | 0 -> introSlide model
    | 1 -> formSlide model
    | 2 -> layoutSlide model
    | 3 -> dataSlide model
    | 4 -> qualitySlide model
    | 5 -> catalogSlide model
    | _ -> finaleSlide model

let view model = sceneFor model

let render model =
    Cmd.ofMsg (HostEffect(RenderFrame(view model)))

let rec update msg model =
    let withRender next = next, render next

    match msg with
    | ViewerInput event ->
        match event with
        | Loaded -> model, render model
        | RenderTick elapsed
        | UpdateTick elapsed -> update (Advance elapsed) model
        | Resized size -> withRender { model with Size = size }
        | ViewerEvent.KeyDown "Right"
        | ViewerEvent.KeyDown "Space" -> update NextSlide model
        | ViewerEvent.KeyDown "Left" -> update PreviousSlide model
        | ViewerEvent.KeyDown "R" -> update Restart model
        | ViewerEvent.KeyDown "Enter" -> update SaveRequested model
        | ViewerEvent.KeyDown "T" ->
            let nextName =
                if model.Name.StartsWith("Ada", StringComparison.Ordinal) then
                    "Grace Hopper"
                else
                    "Ada Lovelace"

            update (NameChanged nextName) model
        | ViewerEvent.KeyDown "S" -> update ToggleCanSave model
        | ViewerEvent.KeyDown "D" -> update (DarkModeChanged(not model.DarkMode)) model
        | ViewerEvent.KeyDown "Up" -> update (CollectionMsg(ScrollTo(max 0.0 (model.Collection.ScrollOffset - 240.0)))) model
        | ViewerEvent.KeyDown "Down" -> update (CollectionMsg(ScrollTo(model.Collection.ScrollOffset + 240.0))) model
        | ViewerEvent.KeyDown "Escape" -> model, Cmd.ofMsg (HostEffect Shutdown)
        | CloseRequested -> model, Cmd.ofMsg (HostEffect Shutdown)
        | DiagnosticReported diagnostic ->
            { model with Diagnostics = diagnostic :: model.Diagnostics },
            Cmd.ofMsg (HostEffect(ReportDiagnostic diagnostic))
        | ViewerEvent.KeyDown _
        | ViewerEvent.KeyUp _
        | PointerMoved _
        | PointerPressed _
        | PointerReleased _ -> model, Cmd.none
    | Advance elapsed ->
        let next = { model with Time = (model.Time + max 0.0 elapsed) % totalDuration }
        withRender next
    | NextSlide ->
        let current =
            model.ManualSlide
            |> Option.defaultValue (int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))))

        withRender ({ model with ManualSlide = Some((current + 1) % slideCount) } |> log "slide next")
    | PreviousSlide ->
        let current =
            model.ManualSlide
            |> Option.defaultValue (int (Math.Floor((model.Time % totalDuration) / (totalDuration / float slideCount))))

        withRender ({ model with ManualSlide = Some((current + slideCount - 1) % slideCount) } |> log "slide previous")
    | Restart -> withRender ({ model with Time = 0.0; ManualSlide = None } |> log "auto reel restarted")
    | NameChanged value -> withRender ({ model with Name = value } |> log $"name -> {value}")
    | NotesChanged value -> withRender ({ model with Notes = value } |> log "notes changed")
    | SaveRequested -> withRender (model |> log $"saved {model.Name}")
    | ToggleCanSave -> withRender ({ model with CanSave = not model.CanSave } |> log "canSave toggled")
    | CanSaveChanged value -> withRender ({ model with CanSave = value } |> log $"canSave -> {value}")
    | DarkModeChanged value -> withRender ({ model with DarkMode = value } |> log $"dark mode -> {value}")
    | SliderChanged value -> withRender ({ model with SliderValue = Math.Clamp(value, 0.0, 1.0) } |> log (sprintf "density -> %.2f" value))
    | QuantityChanged value -> withRender ({ model with Quantity = Math.Clamp(value, 0.0, 99.0) } |> log (sprintf "quantity -> %.1f" value))
    | ModeChanged value -> withRender ({ model with Mode = value } |> log $"mode -> {value}")
    | TabChanged value -> withRender ({ model with SelectedTab = value } |> log $"tab -> {value}")
    | MenuSelected value -> withRender ({ model with SelectedMenu = value } |> log $"menu -> {value}")
    | CollectionMsg collectionMsg ->
        let collection, _ = Collections.update collectionMsg model.Collection
        withRender ({ model with Collection = collection } |> log $"range -> {collection.VisibleRange.FirstIndex}")
    | HostEffect _
    | NoOp -> model, Cmd.none

let configuration =
    { Viewer.defaultConfiguration "Controls Demo Reel" initialSize with
        ClearColor = Some(rgba 7.0 10.0 21.0 255.0)
        TargetFrameRate = Some 60
        Diagnostics = { Verbose = false } }

let program =
    Viewer.create configuration init update view
    |> Viewer.withEventMapping (ViewerInput >> Some)
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let collect cmd =
    let messages = ResizeArray<Msg>()
    cmd |> List.iter (fun effect -> effect messages.Add)
    List.ofSeq messages

let runContractSmoke () =
    let model, initCmd = init ()
    let root = featureRoot model 0.0
    let rendered = Control.render (themeFor model) root
    let afterSave, saveCmd = update SaveRequested model
    let afterText, textCmd = update (NameChanged "Grace Hopper") afterSave
    let afterScroll, scrollCmd = update (CollectionMsg(ScrollTo(24.0 * 250.0))) afterText
    let click =
        { Kind = "click"
          ControlId = Some "save-button"
          Origin = Keyboard
          Payload = Some "Enter" }

    let textChange =
        { Kind = "changed"
          ControlId = Some "profile-name"
          Origin = Text
          Payload = Some "Katherine Johnson" }

    let dispatches = Control.dispatch click (formControls model) @ Control.dispatch textChange (formControls model)
    let scenes = [ for slide in 0 .. slideCount - 1 -> view { afterScroll with ManualSlide = Some slide } |> Scene.describe ]
    let effects = [ initCmd; saveCmd; textCmd; scrollCmd ] |> List.collect collect
    let renderEffects =
        effects
        |> List.filter (function
            | HostEffect(RenderFrame _) -> true
            | _ -> false)
        |> List.length

    printfn "status=ok"
    printfn "sample=DemoReel"
    printfn "mode=controls-live-window"
    printfn "catalog-count=%d" (Catalog.supportedCount ())
    printfn "feature-node-count=%d" rendered.NodeCount
    printfn "event-bindings=%d" rendered.EventBindings.Length
    printfn "diagnostics=%d" rendered.Diagnostics.Length
    printfn "dispatches=%A" dispatches
    printfn "visible-range=%A" afterScroll.Collection.VisibleRange
    printfn "render-effects=%d" renderEffects
    printfn "slide-count=%d" slideCount
    printfn "scene-kinds=%A" (scenes |> List.collect id |> List.distinct)

    if Catalog.supportedCount () >= 46
       && rendered.NodeCount >= 80
       && rendered.EventBindings.Length >= 15
       && dispatches.Length = 2
       && afterScroll.Collection.VisibleRange.FirstIndex = 250
       && renderEffects >= 3 then
        0
    else
        4

let runSmoke () =
    let stopwatch = Stopwatch.StartNew()

    match Viewer.run program with
    | Ok() ->
        stopwatch.Stop()
        printfn "status=ok"
        printfn "sample=DemoReel"
        printfn "renderer=Vulkan"
        printfn "mode=controls-live-window"
        printfn "first-frame-ms=%d" stopwatch.ElapsedMilliseconds
        0
    | Result.Error diagnostic ->
        stopwatch.Stop()
        printfn "status=error"
        printfn "sample=DemoReel"
        printfn "diagnostic-stage=%A" diagnostic.Stage
        printfn "diagnostic-message=%s" diagnostic.Message
        2

let screenshotProgram slide (destination: string) =
    let initScreenshot () =
        let model, _ = init ()
        let slideModel =
            { model with
                ManualSlide = Some(Math.Clamp(slide, 0, slideCount - 1))
                Time = float slide * (totalDuration / float slideCount) }

        slideModel,
        Cmd.batch [
            Cmd.ofMsg (HostEffect(RenderFrame(view slideModel)))
            Cmd.ofMsg
                (HostEffect(
                    CaptureScreenshot
                        { Destination = destination
                          Format = Png }
                ))
            Cmd.ofMsg (HostEffect Shutdown)
        ]

    Viewer.create configuration initScreenshot update view
    |> Viewer.withEffectMapping (function
        | HostEffect effect -> Some effect
        | _ -> None)

let runScreenshotSlide slide (destination: string) =
    match Path.GetDirectoryName destination with
    | null
    | "" -> ()
    | directory -> Directory.CreateDirectory directory |> ignore

    if File.Exists destination then
        File.Delete destination

    match Viewer.run (screenshotProgram slide destination) with
    | Ok() ->
        printfn "status=ok"
        printfn "sample=DemoReel"
        printfn "screenshot-slide=%d" slide
        printfn "screenshot-destination=%s" destination
        printfn "screenshot-exists=%b" (File.Exists destination)
        if File.Exists destination then 0 else 4
    | Result.Error diagnostic ->
        eprintfn "status=error"
        eprintfn "screenshot-slide=%d" slide
        eprintfn "diagnostic-stage=%A" diagnostic.Stage
        eprintfn "diagnostic-message=%s" diagnostic.Message
        2

let runScreenshotAll (outputDirectory: string) =
    Directory.CreateDirectory outputDirectory |> ignore

    [ 0 .. slideCount - 1 ]
    |> List.map (fun slide ->
        let destination = Path.Combine(outputDirectory, $"demo-reel-slide-{slide}.png")
        runScreenshotSlide slide destination)
    |> List.max

[<EntryPoint>]
let main argv =
    if argv |> Array.contains "--contract-smoke" then
        runContractSmoke ()
    elif argv |> Array.contains "--screenshot-all" then
        let outputDirectory =
            argv
            |> Array.tryFind (fun arg -> arg.StartsWith("--output=", StringComparison.Ordinal))
            |> Option.map (fun arg -> arg.Substring("--output=".Length))
            |> Option.defaultValue "readiness/screenshots/demo-reel"

        runScreenshotAll outputDirectory
    elif argv |> Array.exists (fun arg -> arg.StartsWith("--screenshot-slide=", StringComparison.Ordinal)) then
        let slide =
            argv
            |> Array.tryFind (fun arg -> arg.StartsWith("--screenshot-slide=", StringComparison.Ordinal))
            |> Option.map (fun arg -> arg.Substring("--screenshot-slide=".Length))
            |> Option.bind (fun value ->
                match Int32.TryParse value with
                | true, parsed -> Some parsed
                | false, _ -> None)
            |> Option.defaultValue 0

        let destination =
            argv
            |> Array.tryFind (fun arg -> arg.StartsWith("--output=", StringComparison.Ordinal))
            |> Option.map (fun arg -> arg.Substring("--output=".Length))
            |> Option.defaultValue $"readiness/screenshots/demo-reel/demo-reel-slide-{slide}.png"

        runScreenshotSlide slide destination
    else
        runSmoke ()
