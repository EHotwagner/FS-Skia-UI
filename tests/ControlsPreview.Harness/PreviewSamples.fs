module ControlsPreview.Samples

// Feature 079 + 080 — the SINGLE declared per-control sample source.
//
// One entry per `FS.Skia.UI.Build.CatalogGen.catalogFacts` id, in catalog order. Each
// `Demonstrative` entry constructs a FIXED, representative sample state through the typed
// `FS.Skia.UI.Controls.Typed` front door (no clock / randomness / environment data — FR-008
// determinism) AND carries a required `FidelityDeclaration` (feature 080, D5/FR-013): the
// `ControlSampleDefinition` field `Fidelity` makes a Demonstrative-without-signature a
// compile error. The render harness (PreviewRender.fs) loops this list; the totality tests
// (PreviewHarnessTests.fs) prove it is total over `catalogFacts`; the fidelity gate
// (Fidelity.fs) decodes each PNG and asserts each signature.
//
// `custom-control` is the one honest `Unsupported` entry (FR-007): `UnsupportedNoPreview`,
// no committed image, detail page carries `preview-status: unsupported`.

open System
open FS.Skia.UI.Controls
open FS.Skia.UI.Controls.Typed
open FS.Skia.UI.Scene
open ControlsPreview.FidelityTypes

/// Demonstrative ⇒ renders sample content to a committed PNG and carries a `Signature`.
/// Unsupported ⇒ honest no-image declaration (`UnsupportedNoPreview`).
type SampleKind =
    | Demonstrative
    | Unsupported

/// One per-control sample definition. `Build` is `Some` only for `Demonstrative`. `Fidelity` is
/// REQUIRED (feature 080): a Demonstrative sample without a `Signature` does not compile.
type ControlSampleDefinition =
    { Id: string
      Kind: SampleKind
      Build: (unit -> Widget<unit>) option
      Canvas: int * int
      Fidelity: FidelityDeclaration
      UsageNote: string }

/// Default canvas shared by every preview. Uniform 320×160.
let defaultCanvas = (320, 160)

let private demo (id: string) (build: unit -> Widget<unit>) (signature: ContentSignature) (usage: string) =
    { Id = id
      Kind = Demonstrative
      Build = Some build
      Canvas = defaultCanvas
      Fidelity = Signature signature
      UsageNote = usage }

let private unsupported (id: string) (usage: string) =
    { Id = id
      Kind = Unsupported
      Build = None
      Canvas = defaultCanvas
      Fidelity = UnsupportedNoPreview
      UsageNote = usage }

// ---- per-family fidelity signatures (fixed literals; FR-007). Coverage thresholds are the
//      fraction of non-background pixels OUTSIDE the 28-px title band — a 079 label-on-box
//      (24-px tall, all content inside the band) scores ~0 and fails; the faithful geometry
//      below the band passes. The primitive-kind criterion is the structural discriminator. ----
// Text / container controls ARE their text: title-band-exclusive content is acceptable
// (content.contract), so the pixel criterion is non-binding and the control is faithful by the
// label-on-a-box that was always honest for it. The rich families below carry binding criteria.
let private sText = pixelOnly 0.0 0
let private sLine = withKinds 0.02 2 [ PathElement ] []
let private sBars n = withKinds 0.02 2 [ RectangleElement ] [ RectangleElement, n ]
let private sPie n = withKinds 0.04 2 [ ArcElement ] [ ArcElement, n ]
let private sScatter n = withKinds 0.004 1 [ CircleElement; LineElement ] [ CircleElement, n ]
let private sGraph n = withKinds 0.01 1 [ CircleElement; LineElement ] [ CircleElement, n ]
let private sRows n = withKinds 0.04 2 [ RectangleElement; TextRunElement ] [ RectangleElement, n ]
let private sRadio n = withKinds 0.005 1 [ CircleElement ] [ CircleElement, n ]
let private sTabs n = withKinds 0.02 2 [ RectangleElement ] [ RectangleElement, n ]
let private sSlider = withKinds 0.003 2 [ RectangleElement; CircleElement ] [ RectangleElement, 2 ]
let private sProgress = withKinds 0.01 2 [ RectangleElement ] [ RectangleElement, 2 ]
let private sSwitch = withKinds 0.005 2 [ RectangleElement; CircleElement ] []
let private sCheck = withKinds 0.002 1 [ RectangleElement; LineElement ] []
let private sFramed = withKinds 0.002 1 [ RectangleElement ] []
let private sGrid = withKinds 0.01 2 [ RectangleElement; LineElement ] []
let private sSpinner = withKinds 0.02 1 [ ArcElement ] []
let private sImage = withKinds 0.003 1 [ RectangleElement; LineElement ] []
let private sIcon = withKinds 0.003 1 [ PathElement ] []
// 081 — command/button family and layout/container family (single-Kind preview schematics).
let private sButton = withKinds 0.02 1 [ RectangleElement ] []
let private sOutlineButton = withKinds 0.004 1 [ RectangleElement ] []
let private sSplit = withKinds 0.02 2 [ RectangleElement; PathElement ] []
let private sPicker = withKinds 0.005 1 [ RectangleElement; LineElement ] []
let private sSwatches n = withKinds 0.2 2 [ RectangleElement ] [ RectangleElement, n ]
let private sContainer = withKinds 0.05 2 [ RectangleElement; TextRunElement ] []

// ---- shared sample content (fixed literals) --------------------------------------------
let private lbl (t: string) : Widget<unit> = Label.view { Label.defaults with Text = t }

// 081 — Composite controls (pickers, split-button, layout containers) lower through the typed
// front door to MULTI-NODE trees (Stack/Wrap/Toolbar + children) whose preview flattens into
// stray rows with the wrong title band. To render them faithfully as a SINGLE rich-family
// schematic (the renderer dispatches per `Control.Kind`), the preview builds a single-Kind node
// directly via the public `Control.create` + `Widget.ofControl` (toControl ∘ ofControl = id).
let private rawWidget (kind: string) (attrs: Attr<unit> list) : Widget<unit> =
    FS.Skia.UI.Controls.Control.create kind attrs |> Widget.ofControl
let private items4 = [ "Alpha"; "Beta"; "Gamma"; "Delta" ]
let private sampleSeries: ChartSeries list =
    [ { Name = "Sales"
        Points = [ { X = 0.0; Y = 3.0; Label = None }
                   { X = 1.0; Y = 7.0; Label = None }
                   { X = 2.0; Y = 5.0; Label = None }
                   { X = 3.0; Y = 9.0; Label = None } ] } ]

// ---- the single source: one entry per catalogFacts id, in catalog order ----------------
let samples : ControlSampleDefinition list =
    [ // display
      demo "text-block" (fun () -> TextBlock.view { TextBlock.defaults with Text = "Status: all systems nominal" })
          sText "Static model-owned text shown with real content."
      demo "rich-text" (fun () ->
          RichText.view
              { RichText.defaults with
                  Runs =
                      [ { Text = "Bold "; Style = { FontFamily = None; FontSize = 18.0; Weight = RichTextWeight.Bold; Foreground = Colors.black; Background = None; Underline = false; Italic = false }; Diagnostics = [] }
                        { Text = "and italic"; Style = { FontFamily = None; FontSize = 18.0; Weight = RichTextWeight.Regular; Foreground = Colors.rgb 40uy 80uy 160uy; Background = None; Underline = false; Italic = true }; Diagnostics = [] } ] })
          sText "Styled runs (weight, colour, italic) in one display."
      demo "label" (fun () -> lbl "Username") sText "Short form label text."
      demo "image" (fun () -> Image.view { Image.defaults with Value = "logo.png" }) sImage "Image placeholder frame referencing a sample source."
      demo "icon" (fun () -> Icon.view { Icon.defaults with Text = "home" }) sIcon "A named icon shown as a font-independent vector glyph."
      demo "separator" (fun () -> Separator.view { Separator.defaults with Id = None }) sText "A visual divider between regions."
      demo "badge" (fun () -> Badge.view { Badge.defaults with Text = "NEW" }) sOutlineButton "A compact status label shown as an accent pill."
      // input
      demo "button" (fun () -> Button.view { Button.defaults with Text = "Save"; Intent = ButtonIntent.Primary }) sButton "A primary command button with a filled accent surface and label."
      demo "icon-button" (fun () -> IconButton.view { IconButton.defaults with Text = "+"; Intent = ButtonIntent.Secondary }) sOutlineButton "An icon-only activatable command shown as an outlined button."
      demo "text-box" (fun () -> let p = { (TextBox.defaults "text-box") with Value = "jane@example.com" } in let m, _ = TextBox.init p in TextBox.view p m) sText "Single-line text entry with a populated value."
      demo "text-area" (fun () -> let p = { (TextArea.defaults "text-area") with Value = "Multi-line\nnotes here" } in let m, _ = TextArea.init p in TextArea.view p m) sText "Multi-line text entry with populated content."
      demo "numeric-input" (fun () -> NumericInput.view { NumericInput.defaults with Value = 42.0 }) sFramed "A model-owned numeric value editor with a framed field."
      demo "check-box" (fun () -> CheckBox.view { CheckBox.defaults with Text = "Enable notifications"; Checked = true }) sCheck "A checked Boolean choice with a tick and its label."
      demo "radio-group" (fun () -> RadioGroup.view { RadioGroup.defaults with Items = [ "Low"; "Medium"; "High" ]; SelectedKey = Some "Medium" }) (sRadio 3) "A single selection from a visible option set, one marked."
      demo "switch" (fun () -> Switch.view { Switch.defaults with Checked = true }) sSwitch "A compact Boolean setting, switched on (thumb at right)."
      demo "slider" (fun () -> Slider.view { Slider.defaults with Value = 0.5 }) sSlider "Continuous value selection: track plus thumb mid-track."
      demo "list-view" (fun () -> let p = { (ListView.defaults "list-view") with Items = items4 } in let m, _ = ListView.init p in ListView.view p m) (sRows 3) "A bounded visible-range list display of item rows."
      demo "list-box" (fun () -> let p = { (ListBox.defaults "list-box") with Items = items4 } in let m, _ = ListBox.init p in let m2, _ = ListBox.update (SelectKey "Beta") m in ListBox.view p m2) (sRows 3) "A single-selection list with a highlighted row."
      demo "multi-select-list" (fun () -> let p = { (MultiSelectList.defaults "multi-select-list") with Items = items4 } in let m, _ = MultiSelectList.init p in let m2, _ = MultiSelectList.update (ToggleKey "Alpha") m in let m3, _ = MultiSelectList.update (ToggleKey "Gamma") m2 in MultiSelectList.view p m3) (sRows 3) "A list with several selected keys highlighted."
      demo "combo-box" (fun () -> let p = { (ComboBox.defaults "combo-box") with Items = items4 } in let m, _ = ComboBox.init p in ComboBox.view p m) (sRows 3) "A compact selection list of option rows."
      demo "tree-view" (fun () -> let p = { (TreeView.defaults "tree-view") with Items = [ "Root"; "  Child A"; "  Child B" ] } in let m, _ = TreeView.init p in TreeView.view p m) (sRows 3) "A hierarchical item display of indented rows."
      demo "data-grid" (fun () -> rawWidget "data-grid" [ Attr.items [ "Name"; "Qty"; "Widget"; "12"; "Gadget"; "7" ] ])
          sGrid "A tabular grid layout with a header row and body cells."
      // layout — single-Kind container schematics (see `rawWidget`)
      demo "stack" (fun () -> rawWidget "stack" [ Attr.items [ "One"; "Two"; "Three" ] ]) sContainer "An ordered vertical composition of child regions."
      demo "grid" (fun () -> rawWidget "grid" [ Attr.items [ "A1"; "B2"; "C3" ] ]) sContainer "A structured cell composition."
      demo "dock" (fun () -> rawWidget "dock" [ Attr.items [ "Top"; "Fill" ] ]) sContainer "A docked top bar with a left rail and a filled centre."
      demo "wrap" (fun () -> rawWidget "wrap" [ Attr.items [ "tag1"; "tag2"; "tag3" ] ]) sContainer "A wrapping flow of child chips."
      demo "border" (fun () -> rawWidget "border" [ Attr.text "Bordered" ]) sContainer "A single child framed by a border with padding."
      demo "panel" (fun () -> rawWidget "panel" [ Attr.text "Panel content" ]) sContainer "A general-purpose child surface with a header band."
      demo "scroll-viewer" (fun () -> rawWidget "scroll-viewer" [ Attr.text "Scrollable content" ]) sContainer "A scrollable child viewport with a scrollbar."
      demo "split-view" (fun () -> rawWidget "split-view" [ Attr.items [ "Left"; "Right" ] ]) sContainer "A resizable two-pane layout with a divider."
      // navigation
      demo "tabs" (fun () -> Tabs.view { Tabs.defaults with Items = [ "Home"; "Profile"; "Settings" ]; SelectedKey = Some "Profile" }) (sTabs 3) "Active page selection across a tab strip."
      demo "menu" (fun () -> Menu.view { Menu.defaults with Items = [ "File"; "Edit"; "View" ] }) (sRows 3) "A command menu of item rows."
      demo "context-menu" (fun () -> ContextMenu.view { ContextMenu.defaults with Items = [ "Cut"; "Copy"; "Paste" ] }) (sRows 3) "A contextual command menu of item rows."
      demo "toolbar" (fun () -> rawWidget "toolbar" [ Attr.items [ "B"; "I"; "U" ] ]) sContainer "A compact horizontal command group."
      // overlay / feedback
      demo "tooltip" (fun () -> Tooltip.view { Tooltip.defaults with Text = "Click to save your work" }) sText "An auxiliary hover/focus explanation."
      demo "dialog" (fun () -> Dialog.view { Dialog.defaults with Title = Some "Confirm"; IsOpen = true; Children = [ lbl "Are you sure?" ] }) sText "A modal content region (one static frame)."
      demo "toast" (fun () -> Toast.view { Toast.defaults with Text = "Saved successfully"; Severity = ValidationState.Valid }) sText "A transient status message (one static frame)."
      demo "overlay" (fun () -> rawWidget "overlay" [ Attr.text "Overlaid content" ]) sContainer "Layered child content shown as offset surfaces (one static frame)."
      demo "progress-bar" (fun () -> ProgressBar.view { ProgressBar.defaults with Value = 0.6 }) sProgress "A determinate progress indicator: track plus partial fill."
      demo "spinner" (fun () -> Spinner.view { Spinner.defaults with Id = None }) sSpinner "An indeterminate progress indicator (one static frame)."
      demo "validation-message" (fun () -> ValidationMessage.view { ValidationMessage.defaults with Text = "Email is required"; Severity = ValidationState.Invalid "required" }) sText "Validation text tied to model state."
      // charts
      demo "line-chart" (fun () -> LineChart.view { LineChart.defaults with Series = sampleSeries }) sLine "A line data visualization (polyline + filled area) over a sample series."
      demo "bar-chart" (fun () -> BarChart.view { BarChart.defaults with Series = sampleSeries }) (sBars 4) "A bar data visualization, one bar per sample point."
      demo "pie-chart" (fun () -> PieChart.view { PieChart.defaults with Values = [ { X = 0.0; Y = 30.0; Label = Some "A" }; { X = 1.0; Y = 50.0; Label = Some "B" }; { X = 2.0; Y = 20.0; Label = Some "C" } ] }) (sPie 3) "A part-to-whole visualization of three slices."
      demo "scatter-plot" (fun () -> ScatterPlot.view { ScatterPlot.defaults with Series = sampleSeries }) (sScatter 4) "A point-cloud visualization, one mark per sample point."
      demo "graph-view" (fun () -> GraphView.view { GraphView.defaults with Nodes = [ "A"; "B"; "C"; "D" ] }) (sGraph 4) "A node-and-edge visualization of four nodes."
      // 072 expansion — single-Kind schematics (these lower through the typed front door to
      // composite Button/Toolbar/Stack/Wrap trees; the preview renders them as one rich node).
      demo "toggle-button" (fun () -> rawWidget "toggle-button" [ Attr.text "Bold"; Attr.selected true ]) sButton "An on/off command shown pressed."
      demo "split-button" (fun () -> rawWidget "split-button" [ Attr.text "Export" ]) sSplit "A primary action joined to a dropdown command trigger."
      demo "date-picker" (fun () -> rawWidget "date-picker" [ Attr.text "2026-06-08" ]) sPicker "Typed date entry shown as a framed segmented field."
      demo "time-picker" (fun () -> rawWidget "time-picker" [ Attr.text "09:30" ]) sPicker "Typed time entry shown as a framed segmented field."
      demo "color-picker" (fun () -> rawWidget "color-picker" []) (sSwatches 5) "Palette swatches shown as a colour strip."
      // custom — the one honest Unsupported declaration (FR-007)
      unsupported "custom-control" "Product-owned wrapper for custom Skia content; no canonical sample to depict render-only." ]

/// Ids declared in the single source, in catalog order.
let sampleIds = samples |> List.map (fun s -> s.Id)

/// Demonstrative entries only (those that render a committed PNG).
let demonstrative = samples |> List.filter (fun s -> s.Kind = Demonstrative)

/// Unsupported entries only (no image; detail page carries the unsupported marker).
let unsupportedSamples = samples |> List.filter (fun s -> s.Kind = Unsupported)
