module ControlsCatalogTests

open System.IO
open Expecto
open FS.Skia.UI.Controls

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
            Expect.stringContains content "supportedCount: 47" "catalog declares supported count"
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
