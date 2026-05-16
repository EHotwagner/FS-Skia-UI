namespace FS.Skia.UI.Controls

open System

type CatalogAccessibility =
    { Role: string
      NameSource: string
      StateMetadata: string list
      FocusBehavior: string
      KeyboardOperation: string
      ContrastEvidence: string }

type ControlDefinition =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      Purpose: string
      RequiredAttributes: string list
      CommonAttributes: string list
      Events: string list
      VisualStates: string list
      Accessibility: CatalogAccessibility
      Examples: string list
      Tests: string list
      Evidence: string list
      SupportStatus: string
      Owner: string }

module Catalog =
    let accessibility role =
        { Role = role
          NameSource = "text/value/accessibility attribute"
          StateMetadata = [ "enabled"; "visible"; "selected"; "validation"; "loading" ]
          FocusBehavior = "catalog focus order"
          KeyboardOperation = "Tab navigation and Enter/Space activation where interactive"
          ContrastEvidence = "readiness/layout-rendering.md" }

    let definition id displayName category moduleName purpose required common events states role =
        { Id = id
          DisplayName = displayName
          Category = category
          Module = moduleName
          Purpose = purpose
          RequiredAttributes = required
          CommonAttributes = common
          Events = events
          VisualStates = states
          Accessibility = accessibility role
          Examples = [ "samples/ControlsGallery/Program.fs" ]
          Tests =
            [ "tests/Controls.Tests/CatalogTests.fs"
              "tests/Controls.Tests/SemanticTests.fs"
              "tests/Controls.Tests/InteractionTests.fs"
              "tests/Controls.Tests/AccessibilityTests.fs"
              "tests/Controls.Tests/RenderingTests.fs" ]
          Evidence =
            [ "specs/010-skia-controls-library/readiness/control-catalog.md"
              "specs/010-skia-controls-library/readiness/layout-rendering.md" ]
          SupportStatus = "supported"
          Owner = "controls" }

    let common = [ "enabled"; "visible"; "width"; "height"; "padding"; "style"; "theme"; "accessibility" ]
    let states = [ "normal"; "disabled"; "hover"; "pressed"; "focused"; "selected"; "validation"; "loading" ]

    let supportedControls =
        [ definition "text-block" "Text Block" "display" "TextBlock" "Static model-owned text display." [ "text" ] common [] states "StaticText"
          definition "label" "Label" "display" "Label" "Short form label text." [ "text" ] common [] states "StaticText"
          definition "image" "Image" "display" "Image" "Image placeholder or drawing-surface reference." [ "value" ] common [] states "Image"
          definition "icon" "Icon" "display" "Icon" "Named icon glyph or product symbol." [ "text" ] common [] states "Image"
          definition "separator" "Separator" "display" "Separator" "Visual divider between regions." [] common [] states "StaticText"
          definition "badge" "Badge" "display" "Badge" "Compact status label." [ "text" ] common [] states "StaticText"
          definition "button" "Button" "input" "Button" "Pointer and keyboard activatable command." [ "text" ] common [ "onClick" ] states "Button"
          definition "icon-button" "Icon Button" "input" "IconButton" "Icon-only activatable command." [ "text" ] common [ "onClick" ] states "Button"
          definition "text-box" "Text Box" "input" "TextBox" "Plain single-line text entry." [ "value" ] common [ "onChanged" ] states "TextBox"
          definition "text-area" "Text Area" "input" "TextArea" "Plain multi-line text entry." [ "value" ] common [ "onChanged" ] states "TextBox"
          definition "numeric-input" "Numeric Input" "input" "NumericInput" "Model-owned numeric value editor." [ "value" ] common [ "onChanged" ] states "TextBox"
          definition "check-box" "Check Box" "selection" "CheckBox" "Boolean choice with checked state." [ "text" ] common [ "onChanged" ] states "CheckBox"
          definition "radio-group" "Radio Group" "selection" "RadioGroup" "Single selection from a visible option set." [ "items" ] common [ "onChanged" ] states "RadioGroup"
          definition "switch" "Switch" "selection" "Switch" "Compact Boolean setting." [] common [ "onChanged" ] states "CheckBox"
          definition "slider" "Slider" "input" "Slider" "Continuous numeric value selection." [ "value" ] common [ "onChanged" ] states "Slider"
          definition "list-view" "List View" "data" "Collections" "Bounded visible-range list display." [ "items" ] common [ "onSelected" ] states "List"
          definition "list-box" "List Box" "selection" "Collections" "Single-selection list box." [ "items" ] common [ "onSelected" ] states "List"
          definition "multi-select-list" "Multi Select List" "selection" "Collections" "Multiple-selection list with model-owned selected keys." [ "items" ] common [ "onChanged" ] states "List"
          definition "combo-box" "Combo Box" "selection" "Collections" "Compact selection list." [ "items" ] common [ "onChanged" ] states "List"
          definition "tree-view" "Tree View" "data" "Collections" "Hierarchical item display." [ "items" ] common [ "onSelected" ] states "List"
          definition "data-grid" "Data Grid" "data" "Collections" "Table-like bounded visible-range data control." [ "items" ] common [ "onSelected" ] states "Grid"
          definition "stack" "Stack" "layout" "Stack" "Ordered vertical or horizontal child composition." [ "children" ] common [] states "StaticText"
          definition "grid" "Grid" "layout" "Grid" "Structured child composition." [ "children" ] common [] states "StaticText"
          definition "dock" "Dock" "layout" "Dock" "Docked region composition." [ "children" ] common [] states "StaticText"
          definition "wrap" "Wrap" "layout" "Wrap" "Wrapping child layout." [ "children" ] common [] states "StaticText"
          definition "border" "Border" "layout" "Border" "Single child with border and padding." [ "child" ] common [] states "StaticText"
          definition "panel" "Panel" "layout" "Panel" "General-purpose child surface." [ "children" ] common [] states "StaticText"
          definition "scroll-viewer" "Scroll Viewer" "layout" "Collections" "Scrollable child viewport." [ "child" ] common [ "onChanged" ] states "List"
          definition "split-view" "Split View" "layout" "Collections" "Resizable two-region layout." [ "children" ] common [ "onChanged" ] states "StaticText"
          definition "tabs" "Tabs" "navigation" "Tabs" "Model-owned active page selection." [ "items" ] common [ "onChanged" ] states "Tab"
          definition "menu" "Menu" "navigation" "Menu" "Command menu selection." [ "items" ] common [ "onSelected" ] states "Menu"
          definition "context-menu" "Context Menu" "navigation" "Menu" "Contextual command menu." [ "items" ] common [ "onSelected" ] states "Menu"
          definition "toolbar" "Toolbar" "navigation" "Toolbar" "Compact command group." [ "children" ] common [ "onClick" ] states "Menu"
          definition "tooltip" "Tooltip" "overlay" "Tooltip" "Auxiliary hover/focus explanation." [ "text" ] common [] states "StaticText"
          definition "dialog" "Dialog" "overlay" "Dialog" "Modal content region." [ "children" ] common [ "onSelected" ] states "Dialog"
          definition "toast" "Toast" "feedback" "Toast" "Transient status message." [ "text" ] common [] states "StaticText"
          definition "overlay" "Overlay" "overlay" "Overlay" "Layered child content." [ "child" ] common [] states "Dialog"
          definition "progress-bar" "Progress Bar" "feedback" "ProgressBar" "Determinate progress indicator." [ "value" ] common [] states "Progress"
          definition "spinner" "Spinner" "feedback" "Spinner" "Indeterminate progress indicator." [] common [] states "Progress"
          definition "validation-message" "Validation Message" "feedback" "ValidationMessage" "Validation text tied to model state." [ "text" ] common [] states "StaticText"
          definition "line-chart" "Line Chart" "chart" "LineChart" "Controls-owned line data visualization." [ "series" ] common [ "onSelected" ] states "Chart"
          definition "bar-chart" "Bar Chart" "chart" "BarChart" "Controls-owned bar data visualization." [ "series" ] common [ "onSelected" ] states "Chart"
          definition "pie-chart" "Pie Chart" "chart" "PieChart" "Controls-owned part-to-whole visualization." [ "values" ] common [ "onSelected" ] states "Chart"
          definition "scatter-plot" "Scatter Plot" "chart" "ScatterPlot" "Controls-owned point cloud visualization." [ "series" ] common [ "onSelected" ] states "Chart"
          definition "graph-view" "Graph View" "graph" "GraphView" "Controls-owned node and edge visualization." [ "nodes" ] common [ "onSelected" ] states "Graph"
          definition "custom-control" "Custom Control" "custom" "CustomControl" "Product-owned wrapper for custom Skia content." [ "id"; "render"; "layout"; "hitTest"; "accessibility" ] common [ "onCustom" ] states "Custom" ]

    let supportedCount () =
        supportedControls
        |> List.filter (fun row -> row.SupportStatus = "supported")
        |> List.length

    let categories () =
        supportedControls
        |> List.map _.Category
        |> List.distinct
        |> List.sort

    let validate () =
        [ if supportedCount () < 30 then
              yield Diagnostics.create None "catalog" MissingRequiredAttribute Error "Catalog has fewer than 30 supported controls."

          for row in supportedControls do
              if row.Owner <> "controls" then
                  yield Diagnostics.create (Some row.Id) row.Id StaleGeneratedReference Error "Catalog row is not Controls-owned."
              if String.IsNullOrWhiteSpace row.Purpose then
                  yield Diagnostics.create (Some row.Id) row.Id MissingRequiredAttribute Error "Catalog row is missing purpose."
              if row.VisualStates.IsEmpty then
                  yield Diagnostics.create (Some row.Id) row.Id MissingRequiredAttribute Error "Catalog row is missing visual states."
              if row.Examples.IsEmpty || row.Tests.IsEmpty || row.Evidence.IsEmpty then
                  yield Diagnostics.create (Some row.Id) row.Id MissingRequiredAttribute Error "Catalog row is missing examples, tests, or evidence."
              if row.Accessibility.Role.Trim() = "" then
                  yield Diagnostics.create (Some row.Id) row.Id MissingAccessibilityMetadata Error "Catalog row is missing accessibility role." ]

    let markdownSummary () =
        [ "# Control Catalog"
          ""
          $"Supported controls: {supportedCount ()}"
          ""
          "| Id | Category | Module | Events |"
          "|----|----------|--------|--------|"
          yield!
              supportedControls
              |> List.map (fun row ->
                  let events = String.concat ", " row.Events
                  $"| {row.Id} | {row.Category} | {row.Module} | {events} |") ]
        |> String.concat Environment.NewLine
