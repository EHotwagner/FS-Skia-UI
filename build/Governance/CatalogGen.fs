module FS.Skia.UI.Build.CatalogGen

open System.Text.RegularExpressions

// Feature 066 (US1, FR-001/FR-002): make the typed registry of the six 065 controls the
// single source for those rows' catalog facts, generated deterministically into both
// `src/Controls/catalog.yml` and `src/Controls/Catalog.fs`. This mirrors ContractView
// (Routing.fs -> validation.contract.yml) and the GovernedBlocks keyed-region splice: the
// canonical value + the renderers live together in the build front, and per-control
// `typed-catalog/<id>` marked regions carry each rendered row into its non-contiguous home.
// Pure: render/splice/currency are over in-memory text; the file read/write stays at the
// Engine/Interpret.fs edge (Principle IV).

type TypedCatalogFact =
    { Id: string
      DisplayName: string
      Category: string
      Module: string
      TypedModule: string
      Purpose: string
      RequiredAttributes: string list
      Events: string list
      AccessibilityRole: string }

type RegionStatus =
    | Current
    | Stale
    | Missing

type CatalogCurrency =
    { ControlId: string
      FilePath: string
      Status: RegionStatus }

let catalogYmlRel = "src/Controls/catalog.yml"
let catalogFsRel = "src/Controls/Catalog.fs"

let private regenCommand = "./fake.sh build -t RefreshSurfaceBaselines"

// ---------------------------------------------------------------------------------------
// The single source. Feature 071 (US1, FR-001) extends this from the six 065 reference
// controls to **all 47** catalog ids, in catalog file order. Every fact's values are copied
// from the matching hand-maintained `catalog.yml` / `Catalog.fs` row so the regenerated
// bytes equal the existing rows (markers-only diff), with two intentional single-source
// canonicalizations: `custom-control` carries `RequiredAttributes = []` (bridge-typed, no
// fabricated required attribute — FR-006/R3), and `rich-text`'s YAML evidence list is
// normalized to the base evidence (the generator is the single source). Adding/removing a
// fact is a contract change (parity + correspondence tests, currency gate), not a detail.
// ---------------------------------------------------------------------------------------
// Feature 089 (TYPED-SURFACE-1, FR-001/FR-002): `typedModule` is the
// `FS.Skia.UI.Controls.Typed` module name that realizes the control — the
// single-source pointer rendered into `catalog.yml` so a consumer resolves
// control id → typed module → the module's `*Props`/`view` in the published
// `docs/api-surface/Controls/` `.fsi` without reflecting the DLL. For most
// controls it equals the legacy `moduleName`; the ~8 collection/menu rows whose
// legacy builder module is shared (`Collections`/`Menu`) name their distinct
// typed module here. It is a pointer, never a copy of Props field names (those
// live only in the enrolled `.fsi`); every value MUST name a module declared in
// an enrolled `src/Controls/Widgets/*.fsi` (E1⟂E2 cross-check test).
let private fact id displayName category moduleName typedModule purpose required events role =
    { Id = id
      DisplayName = displayName
      Category = category
      Module = moduleName
      TypedModule = typedModule
      Purpose = purpose
      RequiredAttributes = required
      Events = events
      AccessibilityRole = role }

let catalogFacts : TypedCatalogFact list =
    [ fact "text-block" "Text Block" "display" "TextBlock" "TextBlock" "Static model-owned text display." [ "text" ] [] "StaticText"
      fact "rich-text" "Rich Text" "display" "RichText" "RichText" "Skia-specific rich text display with measurement, clipping, effects, diagnostics, and accessibility metadata." [ "runs" ] [] "StaticText"
      fact "label" "Label" "display" "Label" "Label" "Short form label text." [ "text" ] [] "StaticText"
      fact "image" "Image" "display" "Image" "Image" "Image placeholder or drawing-surface reference." [ "value" ] [] "Image"
      fact "icon" "Icon" "display" "Icon" "Icon" "Named icon glyph or product symbol." [ "text" ] [] "Image"
      fact "separator" "Separator" "display" "Separator" "Separator" "Visual divider between regions." [] [] "StaticText"
      fact "badge" "Badge" "display" "Badge" "Badge" "Compact status label." [ "text" ] [] "StaticText"
      fact "button" "Button" "input" "Button" "Button" "Pointer and keyboard activatable command." [ "text" ] [ "onClick" ] "Button"
      fact "icon-button" "Icon Button" "input" "IconButton" "IconButton" "Icon-only activatable command." [ "text" ] [ "onClick" ] "Button"
      fact "text-box" "Text Box" "input" "TextBox" "TextBox" "Plain single-line text entry." [ "value" ] [ "onChanged" ] "TextBox"
      fact "text-area" "Text Area" "input" "TextArea" "TextArea" "Plain multi-line text entry." [ "value" ] [ "onChanged" ] "TextBox"
      fact "numeric-input" "Numeric Input" "input" "NumericInput" "NumericInput" "Model-owned numeric value editor." [ "value" ] [ "onChanged" ] "TextBox"
      fact "check-box" "Check Box" "selection" "CheckBox" "CheckBox" "Boolean choice with checked state." [ "text" ] [ "onChanged" ] "CheckBox"
      fact "radio-group" "Radio Group" "selection" "RadioGroup" "RadioGroup" "Single selection from a visible option set." [ "items" ] [ "onChanged" ] "RadioGroup"
      fact "switch" "Switch" "selection" "Switch" "Switch" "Compact Boolean setting." [] [ "onChanged" ] "CheckBox"
      fact "slider" "Slider" "input" "Slider" "Slider" "Continuous numeric value selection." [ "value" ] [ "onChanged" ] "Slider"
      fact "list-view" "List View" "data" "Collections" "ListView" "Bounded visible-range list display." [ "items" ] [ "onSelected" ] "List"
      fact "list-box" "List Box" "selection" "Collections" "ListBox" "Single-selection list box." [ "items" ] [ "onSelected" ] "List"
      fact "multi-select-list" "Multi Select List" "selection" "Collections" "MultiSelectList" "Multiple-selection list with model-owned selected keys." [ "items" ] [ "onChanged" ] "List"
      fact "combo-box" "Combo Box" "selection" "Collections" "ComboBox" "Compact selection list." [ "items" ] [ "onChanged" ] "List"
      fact "tree-view" "Tree View" "data" "Collections" "TreeView" "Hierarchical item display." [ "items" ] [ "onSelected" ] "List"
      fact "data-grid" "Data Grid" "data" "DataGrid" "DataGrid" "Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata." [ "columns"; "rows" ] [ "onSelected"; "onFocusChanged"; "onSortChanged" ] "Grid"
      fact "stack" "Stack" "layout" "Stack" "Stack" "Ordered vertical or horizontal child composition." [ "children" ] [] "StaticText"
      fact "grid" "Grid" "layout" "Grid" "Grid" "Structured child composition." [ "children" ] [] "StaticText"
      fact "dock" "Dock" "layout" "Dock" "Dock" "Docked region composition." [ "children" ] [] "StaticText"
      fact "wrap" "Wrap" "layout" "Wrap" "Wrap" "Wrapping child layout." [ "children" ] [] "StaticText"
      fact "border" "Border" "layout" "Border" "Border" "Single child with border and padding." [ "child" ] [] "StaticText"
      fact "panel" "Panel" "layout" "Panel" "Panel" "General-purpose child surface." [ "children" ] [] "StaticText"
      fact "scroll-viewer" "Scroll Viewer" "layout" "Collections" "ScrollViewer" "Scrollable child viewport." [ "child" ] [ "onChanged" ] "List"
      fact "split-view" "Split View" "layout" "Collections" "SplitView" "Resizable two-region layout." [ "children" ] [ "onChanged" ] "StaticText"
      fact "tabs" "Tabs" "navigation" "Tabs" "Tabs" "Model-owned active page selection." [ "items" ] [ "onChanged" ] "Tab"
      fact "menu" "Menu" "navigation" "Menu" "Menu" "Command menu selection." [ "items" ] [ "onSelected" ] "Menu"
      fact "context-menu" "Context Menu" "navigation" "Menu" "ContextMenu" "Contextual command menu." [ "items" ] [ "onSelected" ] "Menu"
      fact "toolbar" "Toolbar" "navigation" "Toolbar" "Toolbar" "Compact command group." [ "children" ] [ "onClick" ] "Menu"
      fact "tooltip" "Tooltip" "overlay" "Tooltip" "Tooltip" "Auxiliary hover/focus explanation." [ "text" ] [] "StaticText"
      fact "dialog" "Dialog" "overlay" "Dialog" "Dialog" "Modal content region." [ "children" ] [ "onSelected" ] "Dialog"
      fact "toast" "Toast" "feedback" "Toast" "Toast" "Transient status message." [ "text" ] [] "StaticText"
      fact "overlay" "Overlay" "overlay" "Overlay" "Overlay" "Layered child content." [ "child" ] [] "Dialog"
      fact "progress-bar" "Progress Bar" "feedback" "ProgressBar" "ProgressBar" "Determinate progress indicator." [ "value" ] [] "Progress"
      fact "spinner" "Spinner" "feedback" "Spinner" "Spinner" "Indeterminate progress indicator." [] [] "Progress"
      fact "validation-message" "Validation Message" "feedback" "ValidationMessage" "ValidationMessage" "Validation text tied to model state." [ "text" ] [] "StaticText"
      fact "line-chart" "Line Chart" "chart" "LineChart" "LineChart" "Controls-owned line data visualization." [ "series" ] [ "onSelected" ] "Chart"
      fact "bar-chart" "Bar Chart" "chart" "BarChart" "BarChart" "Controls-owned bar data visualization." [ "series" ] [ "onSelected" ] "Chart"
      fact "pie-chart" "Pie Chart" "chart" "PieChart" "PieChart" "Controls-owned part-to-whole visualization." [ "values" ] [ "onSelected" ] "Chart"
      fact "scatter-plot" "Scatter Plot" "chart" "ScatterPlot" "ScatterPlot" "Controls-owned point cloud visualization." [ "series" ] [ "onSelected" ] "Chart"
      fact "graph-view" "Graph View" "graph" "GraphView" "GraphView" "Controls-owned node and edge visualization." [ "nodes" ] [ "onSelected" ] "Graph"
      // Feature 072 (FR-001) — the breadth-expansion reference slice: five genuinely new
      // controls spanning buttons / pickers / date-time, each a typed-first composition of
      // existing legacy builders (no new StandardControlKind variant, no renderer change).
      fact "toggle-button" "Toggle Button" "input" "ToggleButton" "ToggleButton" "On/off command with product-owned pressed state." [ "text" ] [ "onToggle" ] "Button"
      fact "split-button" "Split Button" "input" "SplitButton" "SplitButton" "Primary action plus a popup menu of secondary commands." [ "text" ] [ "onClick"; "onSelected" ] "Menu"
      fact "date-picker" "Date Picker" "input" "DatePicker" "DatePicker" "Typed date entry with a popup calendar." [] [ "onChange" ] "TextBox"
      fact "time-picker" "Time Picker" "input" "TimePicker" "TimePicker" "Typed time entry with hour and minute segments." [] [ "onChange" ] "TextBox"
      fact "color-picker" "Color Picker" "selection" "ColorPicker" "ColorPicker" "Palette swatch color selection." [ "swatches" ] [ "onSelected" ] "List"
      // custom-control is bridge-typed (Widget.ofControl) — no Props schema, no fabricated
      // required attribute (FR-006/R3). The fact carries RequiredAttributes = [].
      fact "custom-control" "Custom Control" "custom" "CustomControl" "CustomControl" "Product-owned wrapper; renderTree paints a labeled placeholder, not the custom Render/Draw content — build must-show geometry from primitive controls (Border/TextBlock/Stack)." [] [ "onCustom" ] "Custom" ]

// ---------------------------------------------------------------------------------------
// Renderers. Each reproduces the exact on-disk row bytes from the shared constants the
// hand-authored rows use, so the migration diff for the rows is empty (FR-004). Feature 071
// (US1, FR-004) generalizes the chart/data-grid evidence special-case from the single
// `data-grid` id to the **six** evidence-carrying ids below: exactly these rows append
// `|> withChartDataGridEvidence` (F#) and the chart/data-grid evidence path (YAML); no other
// row does. This is the set `grep withChartDataGridEvidence src/Controls/Catalog.fs` showed.
// ---------------------------------------------------------------------------------------
let private evidenceCarryingIds =
    Set.ofList [ "data-grid"; "line-chart"; "bar-chart"; "pie-chart"; "scatter-plot"; "graph-view" ]

let private fsIndent = "          " // the 10-space list-element indentation in Catalog.fs

let private fsList (items: string list) =
    match items with
    | [] -> "[]"
    | _ -> "[ " + (items |> List.map (fun s -> "\"" + s + "\"") |> String.concat "; ") + " ]"

let renderFSharpRow (fact: TypedCatalogFact) : string =
    let line =
        sprintf
            "%sdefinition \"%s\" \"%s\" \"%s\" \"%s\" \"%s\" %s common %s states \"%s\""
            fsIndent
            fact.Id
            fact.DisplayName
            fact.Category
            fact.Module
            fact.Purpose
            (fsList fact.RequiredAttributes)
            (fsList fact.Events)
            fact.AccessibilityRole

    if evidenceCarryingIds.Contains fact.Id then
        line + "\n" + fsIndent + "|> withChartDataGridEvidence"
    else
        line

let private ymlList (items: string list) = "[" + String.concat ", " items + "]"

let private ymlBaseEvidence =
    [ "specs/010-skia-controls-library/readiness/control-catalog.md"
      "specs/010-skia-controls-library/readiness/layout-rendering.md" ]

let private ymlChartDataGridEvidence =
    "specs/011-controls-boundary-refactor/readiness/chart-datagrid-controls.md"

let renderYamlRow (fact: TypedCatalogFact) : string =
    let evidence =
        if evidenceCarryingIds.Contains fact.Id then
            ymlBaseEvidence @ [ ymlChartDataGridEvidence ]
        else
            ymlBaseEvidence

    [ sprintf "  - id: %s" fact.Id
      sprintf "    displayName: %s" fact.DisplayName
      sprintf "    category: %s" fact.Category
      sprintf "    module: %s" fact.Module
      sprintf "    typedModule: %s" fact.TypedModule
      sprintf "    purpose: %s" fact.Purpose
      sprintf "    requiredAttributes: %s" (ymlList fact.RequiredAttributes)
      "    commonAttributes: [enabled, visible, width, height, padding, style, theme, accessibility]"
      sprintf "    events: %s" (ymlList fact.Events)
      "    visualStates: [normal, disabled, hover, pressed, focused, selected, validation, loading]"
      sprintf
          "    accessibility: { role: %s, nameSource: text/value/accessibility attribute, stateMetadata: [enabled, visible, selected, validation, loading], focusBehavior: catalog focus order, keyboardOperation: Tab navigation and Enter/Space activation where interactive, contrastEvidence: readiness/layout-rendering.md }"
          fact.AccessibilityRole
      "    examples: [samples/ControlsGallery/Program.fs]"
      "    tests: [tests/Controls.Tests/CatalogTests.fs, tests/Controls.Tests/SemanticTests.fs, tests/Controls.Tests/InteractionTests.fs, tests/Controls.Tests/AccessibilityTests.fs, tests/Controls.Tests/RenderingTests.fs]"
      sprintf "    evidence: %s" (ymlList evidence)
      "    supportStatus: supported"
      "    owner: controls" ]
    |> String.concat "\n"

// ---------------------------------------------------------------------------------------
// Per-control marked-region splice + currency (R2). The six rows are non-contiguous in both
// files, so each row carries its own `BEGIN/END GENERATED: typed-catalog/<id>` region with a
// file-appropriate comment token (`#` in YAML, `//` in F#). The 41 hand-authored rows carry
// no markers and are never matched. Region inner = the rendered row; currency compares it.
// ---------------------------------------------------------------------------------------
let private normalizeNewlines (text: string) = text.Replace("\r\n", "\n")

// Singleline so `.` crosses newlines; the non-greedy inner stops at the first matching END
// marker. The leading indent before the BEGIN marker is left outside the match (preserved);
// the END marker's indent is absorbed by `[ \t]*` and re-emitted from the known indent.
let private regionRegex (comment: string) =
    Regex(
        Regex.Escape comment
        + @" BEGIN GENERATED: typed-catalog/(?<id>[^\s]+)\r?\n(?<inner>.*?)\r?\n[ \t]*"
        + Regex.Escape comment
        + @" END GENERATED: typed-catalog/\k<id>",
        RegexOptions.Singleline ||| RegexOptions.Compiled)

let private renderedByFile (comment: string) =
    if comment = "//" then renderFSharpRow else renderYamlRow

let private endIndentByFile (comment: string) =
    if comment = "//" then fsIndent else "  "

let private spliceWith (comment: string) (fileText: string) : string =
    let re = regionRegex comment
    let render = renderedByFile comment
    let endIndent = endIndentByFile comment

    let byId =
        catalogFacts |> List.map (fun fact -> fact.Id, render fact) |> Map.ofList

    re.Replace(
        fileText,
        (fun (m: Match) ->
            let id = m.Groups.["id"].Value

            match Map.tryFind id byId with
            | Some rendered ->
                sprintf
                    "%s BEGIN GENERATED: typed-catalog/%s\n%s\n%s%s END GENERATED: typed-catalog/%s"
                    comment
                    id
                    rendered
                    endIndent
                    comment
                    id
            | None -> m.Value))

let spliceFSharp (fileText: string) : string = spliceWith "//" fileText
let spliceYaml (fileText: string) : string = spliceWith "#" fileText

let private currencyForFile (comment: string) (filePath: string) (fileText: string) : CatalogCurrency list =
    let re = regionRegex comment
    let render = renderedByFile comment

    let found =
        [ for m in re.Matches(normalizeNewlines fileText) -> m.Groups.["id"].Value, m.Groups.["inner"].Value ]
        |> Map.ofList

    catalogFacts
    |> List.map (fun fact ->
        let status =
            match Map.tryFind fact.Id found with
            | None -> Missing
            | Some inner -> if inner = render fact then Current else Stale

        { ControlId = fact.Id
          FilePath = filePath
          Status = status })

let currency (catalogYmlText: string) (catalogFsText: string) : CatalogCurrency list =
    currencyForFile "#" catalogYmlRel catalogYmlText
    @ currencyForFile "//" catalogFsRel catalogFsText

let isCurrent (currency: CatalogCurrency list) : bool =
    currency |> List.forall (fun c -> c.Status = Current)

let currencyDrift (currency: CatalogCurrency list) : string list =
    currency
    |> List.choose (fun c ->
        match c.Status with
        | Current -> None
        | Stale ->
            Some(
                sprintf
                    "%s is stale — its generated typed-catalog/%s region no longer matches CatalogGen.catalogFacts. Regenerate via %s."
                    c.FilePath
                    c.ControlId
                    regenCommand)
        | Missing ->
            Some(
                sprintf
                    "%s is missing the generated typed-catalog/%s region for control %s. Regenerate via %s."
                    c.FilePath
                    c.ControlId
                    c.ControlId
                    regenCommand))
