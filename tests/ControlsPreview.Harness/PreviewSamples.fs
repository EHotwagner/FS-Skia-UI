module ControlsPreview.Samples

// Feature 079 (US1, FR-002 / R1) — the SINGLE declared per-control sample source.
//
// This is the one reviewable answer to "why does each preview show what it shows."
// One entry per `FS.Skia.UI.Build.CatalogGen.catalogFacts` id, in catalog order. Each
// `Demonstrative` entry constructs a FIXED, representative sample state through the typed
// `FS.Skia.UI.Controls.Typed` front door (no bare `.defaults` for visible content, no
// clock / randomness / environment data — FR-008 determinism). The render harness
// (PreviewRender.fs) loops this list; the totality/explicitness/idempotence tests
// (PreviewHarnessTests.fs) prove it is total over `catalogFacts` and deterministic.
//
// `custom-control` is the one honest `Unsupported` entry (FR-007): a product-owned
// wrapper for custom Skia content has no canonical sample to depict render-only, so it
// commits no image and its detail page carries a `preview-status: unsupported` marker.

open System
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Scene

/// Demonstrative ⇒ renders sample content to a committed PNG. Unsupported ⇒ honest
/// no-image declaration (detail page carries the unsupported marker).
type SampleKind =
    | Demonstrative
    | Unsupported

/// One per-control sample definition. `Build` is `Some` only for `Demonstrative`.
/// `Canvas` is fixed and documented (never per-run variable) — uniform 320×160 keeps the
/// trivial-content byte floor comparable across the whole catalog (R3/R4).
type ControlSampleDefinition =
    { Id: string
      Kind: SampleKind
      Build: (unit -> Widget<unit>) option
      Canvas: int * int
      UsageNote: string }

/// Default canvas shared by every preview (R4). No per-control overrides are used, so the
/// near-empty baseline (~363 bytes) is uniform and the byte floor `T` is meaningful.
let defaultCanvas = (320, 160)

let private demo (id: string) (build: unit -> Widget<unit>) (usage: string) =
    { Id = id; Kind = Demonstrative; Build = Some build; Canvas = defaultCanvas; UsageNote = usage }

let private unsupported (id: string) (usage: string) =
    { Id = id; Kind = Unsupported; Build = None; Canvas = defaultCanvas; UsageNote = usage }

// ---- shared sample content (fixed literals) --------------------------------------------
let private lbl (t: string) : Widget<unit> = Label.view { Label.defaults with Text = t }
let private items4 = [ "Alpha"; "Beta"; "Gamma"; "Delta" ]
let private sampleSeries : ChartSeries list =
    [ { Name = "Sales"
        Points = [ { X = 0.0; Y = 3.0; Label = None }
                   { X = 1.0; Y = 7.0; Label = None }
                   { X = 2.0; Y = 5.0; Label = None }
                   { X = 3.0; Y = 9.0; Label = None } ] } ]

// ---- the single source: one entry per catalogFacts id, in catalog order ----------------
let samples : ControlSampleDefinition list =
    [ // display
      demo "text-block" (fun () -> TextBlock.view { TextBlock.defaults with Text = "Status: all systems nominal" })
          "Static model-owned text shown with real content."
      demo "rich-text" (fun () ->
          RichText.view
              { RichText.defaults with
                  Runs =
                      [ { Text = "Bold "; Style = { FontFamily = None; FontSize = 18.0; Weight = RichTextWeight.Bold; Foreground = Colors.black; Background = None; Underline = false; Italic = false }; Diagnostics = [] }
                        { Text = "and italic"; Style = { FontFamily = None; FontSize = 18.0; Weight = RichTextWeight.Regular; Foreground = Colors.rgb 40uy 80uy 160uy; Background = None; Underline = false; Italic = true }; Diagnostics = [] } ] })
          "Styled runs (weight, colour, italic) in one display."
      demo "label" (fun () -> lbl "Username") "Short form label text."
      demo "image" (fun () -> Image.view { Image.defaults with Value = "logo.png" }) "Image placeholder referencing a sample source."
      demo "icon" (fun () -> Icon.view { Icon.defaults with Text = "★ home" }) "A named icon glyph with its symbol."
      demo "separator" (fun () -> Separator.view { Separator.defaults with Id = None }) "A visual divider between regions."
      demo "badge" (fun () -> Badge.view { Badge.defaults with Text = "NEW" }) "A compact status label."
      // input
      demo "button" (fun () -> Button.view { Button.defaults with Text = "Save"; Intent = ButtonIntent.Primary }) "A primary command button with a visible label."
      demo "icon-button" (fun () -> IconButton.view { IconButton.defaults with Text = "⚙"; Intent = ButtonIntent.Secondary }) "An icon-only activatable command."
      demo "text-box" (fun () -> let p = { (TextBox.defaults "text-box") with Value = "jane@example.com" } in let m, _ = TextBox.init p in TextBox.view p m) "Single-line text entry with a populated value."
      demo "text-area" (fun () -> let p = { (TextArea.defaults "text-area") with Value = "Multi-line\nnotes here" } in let m, _ = TextArea.init p in TextArea.view p m) "Multi-line text entry with populated content."
      demo "numeric-input" (fun () -> NumericInput.view { NumericInput.defaults with Value = 42.0 }) "A model-owned numeric value editor."
      demo "check-box" (fun () -> CheckBox.view { CheckBox.defaults with Text = "Enable notifications"; Checked = true }) "A checked Boolean choice with its label."
      demo "radio-group" (fun () -> RadioGroup.view { RadioGroup.defaults with Items = [ "Low"; "Medium"; "High" ]; SelectedKey = Some "Medium" }) "A single selection from a visible option set."
      demo "switch" (fun () -> Switch.view { Switch.defaults with Checked = true }) "A compact Boolean setting, switched on."
      demo "slider" (fun () -> Slider.view { Slider.defaults with Value = 0.5 }) "Continuous value selection positioned mid-track."
      demo "list-view" (fun () -> let p = { (ListView.defaults "list-view") with Items = items4 } in let m, _ = ListView.init p in ListView.view p m) "A bounded visible-range list display."
      demo "list-box" (fun () -> let p = { (ListBox.defaults "list-box") with Items = items4 } in let m, _ = ListBox.init p in let m2, _ = ListBox.update (SelectKey "Beta") m in ListBox.view p m2) "A single-selection list with a highlighted row."
      demo "multi-select-list" (fun () -> let p = { (MultiSelectList.defaults "multi-select-list") with Items = items4 } in let m, _ = MultiSelectList.init p in let m2, _ = MultiSelectList.update (ToggleKey "Alpha") m in let m3, _ = MultiSelectList.update (ToggleKey "Gamma") m2 in MultiSelectList.view p m3) "A list with several selected keys."
      demo "combo-box" (fun () -> let p = { (ComboBox.defaults "combo-box") with Items = items4 } in let m, _ = ComboBox.init p in ComboBox.view p m) "A compact selection list."
      demo "tree-view" (fun () -> let p = { (TreeView.defaults "tree-view") with Items = [ "Root"; "  Child A"; "  Child B" ] } in let m, _ = TreeView.init p in TreeView.view p m) "A hierarchical item display."
      demo "data-grid" (fun () ->
          let cols = [ { Key = "name"; Header = "Name"; Width = 80.0; ColumnType = TextColumn }
                       { Key = "qty"; Header = "Qty"; Width = 50.0; ColumnType = NumericColumn } ]
          let rows = [ { Key = "r1"; Cells = [ { RowKey = "r1"; ColumnKey = "name"; Value = "Widget" }; { RowKey = "r1"; ColumnKey = "qty"; Value = "12" } ] }
                       { Key = "r2"; Cells = [ { RowKey = "r2"; ColumnKey = "name"; Value = "Gadget" }; { RowKey = "r2"; ColumnKey = "qty"; Value = "7" } ] } ]
          let p = { (DataGrid.defaults "data-grid") with Columns = cols; Rows = rows }
          let m, _ = DataGrid.init p
          let m2, _ = DataGrid.update (SelectRow "r1") m
          DataGrid.view p m2)
          "Columns and rows with a selected row (documented columns/rows usage)."
      // layout
      demo "stack" (fun () -> Stack.view { Stack.defaults with Children = [ lbl "One"; lbl "Two"; lbl "Three" ] }) "An ordered composition of child controls."
      demo "grid" (fun () -> Grid.view { Grid.defaults with Children = [ lbl "A1"; lbl "B2"; lbl "C3" ] }) "A structured child composition."
      demo "dock" (fun () -> Dock.view { Dock.defaults with Children = [ lbl "Top"; lbl "Fill" ] }) "A docked-region composition."
      demo "wrap" (fun () -> Wrap.view { Wrap.defaults with Children = [ lbl "tag1"; lbl "tag2"; lbl "tag3" ] }) "A wrapping child layout."
      demo "border" (fun () -> Border.view (Border.defaults (lbl "Bordered"))) "A single child with border and padding."
      demo "panel" (fun () -> Panel.view { Panel.defaults with Children = [ lbl "Panel content" ] }) "A general-purpose child surface."
      demo "scroll-viewer" (fun () -> ScrollViewer.view (ScrollViewer.defaults "scroll-viewer" (lbl "Scrollable content"))) "A scrollable child viewport."
      demo "split-view" (fun () -> SplitView.view { SplitView.defaults with Children = [ lbl "Left"; lbl "Right" ] }) "A resizable two-region layout."
      // navigation
      demo "tabs" (fun () -> Tabs.view { Tabs.defaults with Items = [ "Home"; "Profile"; "Settings" ]; SelectedKey = Some "Profile" }) "Active page selection across tabs."
      demo "menu" (fun () -> Menu.view { Menu.defaults with Items = [ "File"; "Edit"; "View" ] }) "A command menu."
      demo "context-menu" (fun () -> ContextMenu.view { ContextMenu.defaults with Items = [ "Cut"; "Copy"; "Paste" ] }) "A contextual command menu."
      demo "toolbar" (fun () -> Toolbar.view { Toolbar.defaults with Children = [ lbl "B"; lbl "I"; lbl "U" ] }) "A compact command group."
      // overlay / feedback
      demo "tooltip" (fun () -> Tooltip.view { Tooltip.defaults with Text = "Click to save your work" }) "An auxiliary hover/focus explanation."
      demo "dialog" (fun () -> Dialog.view { Dialog.defaults with Title = Some "Confirm"; IsOpen = true; Children = [ lbl "Are you sure?" ] }) "A modal content region (one static frame)."
      demo "toast" (fun () -> Toast.view { Toast.defaults with Text = "Saved successfully"; Severity = ValidationState.Valid }) "A transient status message (one static frame)."
      demo "overlay" (fun () -> Overlay.view (Overlay.defaults (lbl "Overlaid content"))) "Layered child content (one static frame)."
      demo "progress-bar" (fun () -> ProgressBar.view { ProgressBar.defaults with Value = 0.6 }) "A determinate progress indicator."
      demo "spinner" (fun () -> Spinner.view { Spinner.defaults with Id = None }) "An indeterminate progress indicator (one static frame)."
      demo "validation-message" (fun () -> ValidationMessage.view { ValidationMessage.defaults with Text = "Email is required"; Severity = ValidationState.Invalid "required" }) "Validation text tied to model state."
      // charts
      demo "line-chart" (fun () -> LineChart.view { LineChart.defaults with Series = sampleSeries }) "A line data visualization over a sample series."
      demo "bar-chart" (fun () -> BarChart.view { BarChart.defaults with Series = sampleSeries }) "A bar data visualization over a sample series."
      demo "pie-chart" (fun () -> PieChart.view { PieChart.defaults with Values = [ { X = 0.0; Y = 30.0; Label = Some "A" }; { X = 1.0; Y = 50.0; Label = Some "B" }; { X = 2.0; Y = 20.0; Label = Some "C" } ] }) "A part-to-whole visualization."
      demo "scatter-plot" (fun () -> ScatterPlot.view { ScatterPlot.defaults with Series = sampleSeries }) "A point-cloud visualization over a sample series."
      demo "graph-view" (fun () -> GraphView.view { GraphView.defaults with Nodes = [ "A"; "B"; "C"; "D" ] }) "A node-and-edge visualization."
      // 072 expansion
      demo "toggle-button" (fun () -> ToggleButton.view { ToggleButton.defaults with Text = "Bold"; IsOn = true }) "An on/off command shown pressed."
      demo "split-button" (fun () -> SplitButton.view { SplitButton.defaults with Text = "Export"; Items = [ { Key = "pdf"; Label = "PDF" }; { Key = "csv"; Label = "CSV" } ] }) "A primary action plus a secondary command menu."
      demo "date-picker" (fun () -> DatePicker.view { DatePicker.defaults with Value = Some(DateOnly(2026, 6, 8)) }) "Typed date entry with a fixed sample date."
      demo "time-picker" (fun () -> TimePicker.view { TimePicker.defaults with Value = Some(TimeOnly(9, 30)) }) "Typed time entry with hour and minute segments."
      demo "color-picker" (fun () ->
          let swatches = [ { Name = "Red"; Color = Colors.rgb 200uy 60uy 60uy }
                           { Name = "Green"; Color = Colors.rgb 60uy 160uy 80uy }
                           { Name = "Blue"; Color = Colors.rgb 60uy 90uy 200uy } ]
          ColorPicker.view { ColorPicker.defaults with Swatches = swatches; Selected = Some { Name = "Green"; Color = Colors.rgb 60uy 160uy 80uy } })
          "Palette swatches with a selected colour."
      // custom — the one honest Unsupported declaration (FR-007)
      unsupported "custom-control" "Product-owned wrapper for custom Skia content; no canonical sample to depict render-only." ]

/// Ids declared in the single source, in catalog order.
let sampleIds = samples |> List.map (fun s -> s.Id)

/// Demonstrative entries only (those that render a committed PNG).
let demonstrative = samples |> List.filter (fun s -> s.Kind = Demonstrative)

/// Unsupported entries only (no image; detail page carries the unsupported marker).
let unsupportedSamples = samples |> List.filter (fun s -> s.Kind = Unsupported)
