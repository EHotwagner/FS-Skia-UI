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
// The single source. Exactly the six 065 controls, in catalog file order (text-block #1,
// button #8, text-box #10, check-box #13, data-grid #22, stack #24). Adding/removing a fact
// is a contract change (parity + correspondence tests, currency gate), not a detail.
// ---------------------------------------------------------------------------------------
let catalogFacts : TypedCatalogFact list =
    [ { Id = "text-block"
        DisplayName = "Text Block"
        Category = "display"
        Module = "TextBlock"
        Purpose = "Static model-owned text display."
        RequiredAttributes = [ "text" ]
        Events = []
        AccessibilityRole = "StaticText" }
      { Id = "button"
        DisplayName = "Button"
        Category = "input"
        Module = "Button"
        Purpose = "Pointer and keyboard activatable command."
        RequiredAttributes = [ "text" ]
        Events = [ "onClick" ]
        AccessibilityRole = "Button" }
      { Id = "text-box"
        DisplayName = "Text Box"
        Category = "input"
        Module = "TextBox"
        Purpose = "Plain single-line text entry."
        RequiredAttributes = [ "value" ]
        Events = [ "onChanged" ]
        AccessibilityRole = "TextBox" }
      { Id = "check-box"
        DisplayName = "Check Box"
        Category = "selection"
        Module = "CheckBox"
        Purpose = "Boolean choice with checked state."
        RequiredAttributes = [ "text" ]
        Events = [ "onChanged" ]
        AccessibilityRole = "CheckBox" }
      { Id = "data-grid"
        DisplayName = "Data Grid"
        Category = "data"
        Module = "DataGrid"
        Purpose = "Table-like bounded visible-range data control with product-owned rows, selection, focus, sort, and filter metadata."
        RequiredAttributes = [ "columns"; "rows" ]
        Events = [ "onSelected"; "onFocusChanged"; "onSortChanged" ]
        AccessibilityRole = "Grid" }
      { Id = "stack"
        DisplayName = "Stack"
        Category = "layout"
        Module = "Stack"
        Purpose = "Ordered vertical or horizontal child composition."
        RequiredAttributes = [ "children" ]
        Events = []
        AccessibilityRole = "StaticText" } ]

// ---------------------------------------------------------------------------------------
// Renderers. Each reproduces the exact on-disk row bytes from the shared constants the
// hand-authored rows use, so the migration diff for the rows is empty (FR-004). `data-grid`
// is the one fact carrying the chart/data-grid evidence (mirrors `withChartDataGridEvidence`
// in Catalog.fs and the extra evidence path in catalog.yml).
// ---------------------------------------------------------------------------------------
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

    if fact.Id = "data-grid" then
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
        if fact.Id = "data-grid" then
            ymlBaseEvidence @ [ ymlChartDataGridEvidence ]
        else
            ymlBaseEvidence

    [ sprintf "  - id: %s" fact.Id
      sprintf "    displayName: %s" fact.DisplayName
      sprintf "    category: %s" fact.Category
      sprintf "    module: %s" fact.Module
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
