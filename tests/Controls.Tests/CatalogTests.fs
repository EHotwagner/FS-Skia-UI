module ControlsCatalogTests

open System.IO
open Expecto
open FS.Skia.UI.Controls
open FS.Skia.UI.Build
open FS.Skia.UI.Controls.Typed
open FSharp.Reflection

let repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then
            dir
        else
            (match Directory.GetParent dir |> Option.ofObj with Some p -> find p.FullName | None -> dir)
    find __SOURCE_DIRECTORY__

[<Tests>]
let catalogTests =
    testList "Controls catalog contract" [
        test "catalog exposes at least thirty supported Controls-owned rows" {
            Expect.isGreaterThanOrEqual (Catalog.supportedCount ()) 30 "minimum supported catalog size"
            Expect.isEmpty (Catalog.validate ()) "catalog rows are complete"

            let categories = Catalog.categories () |> Set.ofList

            [ "display"; "input"; "selection"; "navigation"; "layout"; "feedback"; "data"; "chart"; "graph"; "custom" ]
            |> List.iter (fun category -> Expect.isTrue (categories.Contains category) $"catalog contains {category}")

            Catalog.supportedControls
            |> List.iter (fun row ->
                Expect.equal row.Owner "controls" $"{row.Id} is Controls-owned"
                Expect.isNonEmpty row.Examples $"{row.Id} has examples"
                Expect.isNonEmpty row.Tests $"{row.Id} has tests"
                Expect.isNonEmpty row.Evidence $"{row.Id} has evidence")

            let byId =
                Catalog.supportedControls
                |> List.map (fun row -> row.Id, row)
                |> Map.ofList

            Expect.equal byId["rich-text"].Module "RichText" "rich text is cataloged under Controls RichText"
            Expect.equal byId["data-grid"].Module "DataGrid" "DataGrid is cataloged under Controls DataGrid"
            Expect.equal byId["data-grid"].Category "data" "DataGrid is categorized as data"
        }

        test "structured catalog source and summary name Controls ownership" {
            let path = Path.Combine(repositoryRoot, "src", "Controls", "catalog.yml")
            let content = File.ReadAllText path
            Expect.stringContains content "owner: controls" "catalog is owned by Controls"
            Expect.stringContains content "supportedCount: 52" "catalog declares supported count"
            Expect.stringContains content "id: rich-text" "catalog source includes rich text"
            Expect.stringContains content "module: DataGrid" "catalog source puts DataGrid under the Controls DataGrid module"
            Expect.isFalse (content.Contains("owner: charts")) "catalog does not delegate chart rows to Charts"
            Expect.stringContains (Catalog.markdownSummary ()) "Supported controls" "markdown summary is available"
        }

        test "chart graph and DataGrid rows carry full US2 Controls-owned metadata" {
            let byId =
                Catalog.supportedControls
                |> List.map (fun row -> row.Id, row)
                |> Map.ofList

            [ "line-chart", "chart", "LineChart", "Chart", [ "series" ]
              "bar-chart", "chart", "BarChart", "Chart", [ "series" ]
              "pie-chart", "chart", "PieChart", "Chart", [ "values" ]
              "scatter-plot", "chart", "ScatterPlot", "Chart", [ "series" ]
              "graph-view", "graph", "GraphView", "Graph", [ "nodes" ]
              "data-grid", "data", "DataGrid", "Grid", [ "columns"; "rows" ] ]
            |> List.iter (fun (id, category, moduleName, role, required) ->
                let row = byId[id]

                Expect.equal row.Owner "controls" $"{id} is owned by Controls"
                Expect.equal row.Category category $"{id} has the required category"
                Expect.equal row.Module moduleName $"{id} is exposed through the Controls module"
                Expect.equal row.Accessibility.Role role $"{id} declares accessibility role"
                required
                |> List.iter (fun attribute ->
                    Expect.contains row.RequiredAttributes attribute $"{id} declares required attribute {attribute}")
                Expect.isNonEmpty row.VisualStates $"{id} declares supported visual states"
                Expect.isNonEmpty row.Events $"{id} declares interaction metadata"
                Expect.isNonEmpty row.Examples $"{id} has examples"
                Expect.isNonEmpty row.Tests $"{id} has tests"
                Expect.exists row.Evidence (fun path -> path.EndsWith("chart-datagrid-controls.md")) $"{id} links chart/DataGrid readiness evidence")
        }
    ]

// Feature 066 — typed catalog generation. The six 065 controls' catalog rows are generated
// from the single `CatalogGen.catalogFacts` source into both `catalog.yml` and `Catalog.fs`.
// These tests assert: (US2) the generated rows are byte-identical to the captured
// pre-migration rows and to the on-disk regions; (US1) the drift gate flags a hand-mutated
// region naming the control, and one regeneration restores both files deterministically; and
// that the fact table corresponds to exactly the six `FS.Skia.UI.Controls.Typed` modules.
[<Tests>]
let catalogGenerationTests =
    let catalogFsPath = Path.Combine(repositoryRoot, "src", "Controls", "Catalog.fs")
    let catalogYmlPath = Path.Combine(repositoryRoot, "src", "Controls", "catalog.yml")
    let fixturesDir =
        Path.Combine(repositoryRoot, "specs", "066-typed-catalog-generation", "readiness", "parity-fixtures")

    let factsById =
        CatalogGen.catalogFacts |> List.map (fun f -> f.Id, f) |> Map.ofList

    // 071 (T006): the lockstep map grows from the 6 reference ids to all 47 catalog
    // ids so the cross-check iterates the complete catalog. Each id maps to its typed
    // Props record, except `custom-control`, which is bridge-typed (Widget.ofControl)
    // and has no Props schema -> None (excluded from the Props-field assertion, C10).
    let typedPropsById : Map<string, System.Type option> =
        [ "text-block", Some typeof<TextBlockProps<int>>
          "rich-text", Some typeof<RichTextProps<int>>
          "label", Some typeof<LabelProps<int>>
          "image", Some typeof<ImageProps<int>>
          "icon", Some typeof<IconProps<int>>
          "separator", Some typeof<SeparatorProps<int>>
          "badge", Some typeof<BadgeProps<int>>
          "button", Some typeof<ButtonProps<int>>
          "icon-button", Some typeof<IconButtonProps<int>>
          "text-box", Some typeof<TextBoxProps<int>>
          "text-area", Some typeof<TextAreaProps<int>>
          "numeric-input", Some typeof<NumericInputProps<int>>
          "check-box", Some typeof<CheckBoxProps<int>>
          "radio-group", Some typeof<RadioGroupProps<int>>
          "switch", Some typeof<SwitchProps<int>>
          "slider", Some typeof<SliderProps<int>>
          "list-view", Some typeof<ListViewProps<int>>
          "list-box", Some typeof<ListBoxProps<int>>
          "multi-select-list", Some typeof<MultiSelectListProps<int>>
          "combo-box", Some typeof<ComboBoxProps<int>>
          "tree-view", Some typeof<TreeViewProps<int>>
          "data-grid", Some typeof<DataGridProps<int>>
          "stack", Some typeof<StackProps<int>>
          "grid", Some typeof<GridProps<int>>
          "dock", Some typeof<DockProps<int>>
          "wrap", Some typeof<WrapProps<int>>
          "border", Some typeof<BorderProps<int>>
          "panel", Some typeof<PanelProps<int>>
          "scroll-viewer", Some typeof<ScrollViewerProps<int>>
          "split-view", Some typeof<SplitViewProps<int>>
          "tabs", Some typeof<TabsProps<int>>
          "menu", Some typeof<MenuProps<int>>
          "context-menu", Some typeof<ContextMenuProps<int>>
          "toolbar", Some typeof<ToolbarProps<int>>
          "tooltip", Some typeof<TooltipProps<int>>
          "dialog", Some typeof<DialogProps<int>>
          "toast", Some typeof<ToastProps<int>>
          "overlay", Some typeof<OverlayProps<int>>
          "progress-bar", Some typeof<ProgressBarProps<int>>
          "spinner", Some typeof<SpinnerProps<int>>
          "validation-message", Some typeof<ValidationMessageProps<int>>
          "line-chart", Some typeof<LineChartProps<int>>
          "bar-chart", Some typeof<BarChartProps<int>>
          "pie-chart", Some typeof<PieChartProps<int>>
          "scatter-plot", Some typeof<ScatterPlotProps<int>>
          "graph-view", Some typeof<GraphViewProps<int>>
          // Feature 072 breadth expansion — five new typed-first controls.
          "toggle-button", Some typeof<ToggleButtonProps<int>>
          "split-button", Some typeof<SplitButtonProps<int>>
          "date-picker", Some typeof<DatePickerProps<int>>
          "time-picker", Some typeof<TimePickerProps<int>>
          "color-picker", Some typeof<ColorPickerProps<int>>
          "custom-control", None ]
        |> Map.ofList

    let recordFields (t: System.Type) =
        FSharpType.GetRecordFields t |> Array.map (fun p -> p.Name) |> Set.ofArray

    let pascalCase (s: string) = s.Substring(0, 1).ToUpper() + s.Substring(1)

    testList "Feature 066 typed catalog generation" [
        test "each generated row is byte-identical to its captured pre-migration row (SC-002, FR-004)" {
            for fact in CatalogGen.catalogFacts do
                let fsFixture =
                    File.ReadAllText(Path.Combine(fixturesDir, $"Catalog.fs.{fact.Id}.txt")).Replace("\r\n", "\n").TrimEnd('\n')

                Expect.equal (CatalogGen.renderFSharpRow fact) fsFixture $"{fact.Id} F# row matches pre-migration fixture"

                let ymlFixture =
                    File.ReadAllText(Path.Combine(fixturesDir, $"catalog.yml.{fact.Id}.txt")).Replace("\r\n", "\n").TrimEnd('\n')

                Expect.equal (CatalogGen.renderYamlRow fact) ymlFixture $"{fact.Id} YAML row matches pre-migration fixture"
        }

        test "the committed catalog files are a current, byte-identical regeneration (SC-002, SC-003)" {
            let currency =
                CatalogGen.currency (File.ReadAllText catalogYmlPath) (File.ReadAllText catalogFsPath)

            Expect.isTrue (CatalogGen.isCurrent currency) "every generated region is current on the committed tree"
            Expect.isEmpty (CatalogGen.currencyDrift currency) "a clean tree produces no drift diagnostics"
        }

        test "regeneration splice is idempotent on a current tree and updates both files (FR-002)" {
            let yml = File.ReadAllText catalogYmlPath
            let fs = File.ReadAllText catalogFsPath

            Expect.equal (CatalogGen.spliceYaml yml) yml "spliceYaml is a no-op on a current catalog.yml"
            Expect.equal (CatalogGen.spliceFSharp fs) fs "spliceFSharp is a no-op on a current Catalog.fs"
        }

        test "the drift gate flags a hand-mutated region and names the divergent control (SC-003, FR-005)" {
            let fs = File.ReadAllText catalogFsPath
            let yml = File.ReadAllText catalogYmlPath

            // Hand-edit the generated button row WITHOUT regenerating (simulates the SC-003 check).
            let mutated =
                fs.Replace(
                    "definition \"button\" \"Button\" \"input\" \"Button\" \"Pointer and keyboard activatable command.\"",
                    "definition \"button\" \"Button\" \"input\" \"Button\" \"MUTATED purpose.\"")

            Expect.notEqual mutated fs "the mutation changed the button row"

            let currency = CatalogGen.currency yml mutated
            Expect.isFalse (CatalogGen.isCurrent currency) "a hand-mutated region is not current"

            let drift = CatalogGen.currencyDrift currency
            Expect.isNonEmpty drift "a stale region yields a drift diagnostic"

            let buttonDrift = drift |> List.filter (fun d -> d.Contains "typed-catalog/button")
            Expect.isNonEmpty buttonDrift "the diagnostic names the divergent control (button)"

            buttonDrift
            |> List.iter (fun d ->
                Expect.stringContains d "Catalog.fs" "the diagnostic names the stale file"
                Expect.stringContains d "RefreshSurfaceBaselines" "the diagnostic names the regeneration command")

            // Re-splicing the mutated tree from the single source restores it exactly.
            Expect.equal (CatalogGen.spliceFSharp mutated) fs "one regeneration restores the mutated file byte-for-byte"
        }

        test "a missing region is reported loudly, never silently passed (edge case)" {
            // Strip the button markers + row from Catalog.fs entirely.
            let fs = File.ReadAllText catalogFsPath
            let lines = fs.Replace("\r\n", "\n").Split('\n') |> Array.toList

            let stripped =
                let rec drop acc inRegion remaining =
                    match remaining with
                    | [] -> List.rev acc
                    | (line: string) :: rest when line.Contains "BEGIN GENERATED: typed-catalog/button" -> drop acc true rest
                    | line :: rest when line.Contains "END GENERATED: typed-catalog/button" -> drop acc false rest
                    | _ :: rest when inRegion -> drop acc true rest
                    | line :: rest -> drop (line :: acc) false rest

                drop [] false lines |> String.concat "\n"

            let currency = CatalogGen.currency (File.ReadAllText catalogYmlPath) stripped
            let buttonStatus = currency |> List.find (fun c -> c.ControlId = "button" && c.FilePath = CatalogGen.catalogFsRel)
            Expect.equal buttonStatus.Status CatalogGen.Missing "a removed region is reported Missing"

            let drift = CatalogGen.currencyDrift currency |> List.filter (fun d -> d.Contains "typed-catalog/button")
            Expect.isNonEmpty drift "the missing region fails loudly with a diagnostic"
        }

        test "catalogFacts corresponds to exactly the 52 catalog typed ids (FR-001, R5, SC-003)" {
            Expect.equal
                (factsById |> Map.toList |> List.map fst |> List.sort)
                (typedPropsById |> Map.toList |> List.map fst |> List.sort)
                "the fact table covers exactly the 52 catalog typed ids"

            // The 52 facts span the full 11-value catalog taxonomy (data-model E1),
            // including `overlay` (tooltip/dialog/overlay) which the 6-fact-era set omitted.
            let categories =
                [ "display"; "input"; "selection"; "navigation"; "layout"; "feedback"; "data"; "overlay"; "chart"; "graph"; "custom" ]
                |> Set.ofList

            for fact in CatalogGen.catalogFacts do
                Expect.isTrue (categories.Contains fact.Category) $"{fact.Id} uses an existing catalog category"

                // custom-control is bridge-typed (None) -> excluded from the Props-field
                // assertion (contract C10); every other id maps each required attribute
                // (PascalCased) to a field on its typed Props record.
                match typedPropsById.[fact.Id] with
                | Some propsType ->
                    let fields = recordFields propsType

                    for required in fact.RequiredAttributes do
                        Expect.isTrue
                            (fields.Contains(pascalCase required))
                            $"{fact.Id} required attribute '{required}' maps to a typed Props field"
                | None ->
                    Expect.equal fact.RequiredAttributes [] $"{fact.Id} is bridge-typed and declares no required attribute"

            Expect.equal factsById.["data-grid"].Category "data" "data-grid keeps Category = data"
            Expect.equal factsById.["data-grid"].Module "DataGrid" "data-grid keeps Module = DataGrid"
        }

        test "the fact table agrees with the public Catalog.supportedControls rows it generates (FR-007)" {
            let byId =
                Catalog.supportedControls |> List.map (fun row -> row.Id, row) |> Map.ofList

            for fact in CatalogGen.catalogFacts do
                let row = byId.[fact.Id]
                Expect.equal row.DisplayName fact.DisplayName $"{fact.Id} display name agrees with the generated row"
                Expect.equal row.Category fact.Category $"{fact.Id} category agrees with the generated row"
                Expect.equal row.Module fact.Module $"{fact.Id} module agrees with the generated row"
                Expect.equal row.Purpose fact.Purpose $"{fact.Id} purpose agrees with the generated row"
                Expect.equal row.RequiredAttributes fact.RequiredAttributes $"{fact.Id} required attributes agree"
                Expect.equal row.Events fact.Events $"{fact.Id} events agree with the generated row"
                Expect.equal row.Accessibility.Role fact.AccessibilityRole $"{fact.Id} accessibility role agrees"
        }
    ]
