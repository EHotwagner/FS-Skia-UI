module ControlsCatalogTests

open System.IO
open Expecto
open FS.Skia.UI.Controls

let repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "build.fsx")) then
            dir
        else
            Directory.GetParent(dir) |> Option.ofObj |> Option.map _.FullName |> Option.defaultValue dir |> find
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
        }

        test "structured catalog source and summary name Controls ownership" {
            let path = Path.Combine(repositoryRoot, "src", "Controls", "catalog.yml")
            let content = File.ReadAllText path
            Expect.stringContains content "owner: controls" "catalog is owned by Controls"
            Expect.stringContains content "supportedCount: 46" "catalog declares supported count"
            Expect.isFalse (content.Contains("owner: charts")) "catalog does not delegate chart rows to Charts"
            Expect.stringContains (Catalog.markdownSummary ()) "Supported controls" "markdown summary is available"
        }
    ]
