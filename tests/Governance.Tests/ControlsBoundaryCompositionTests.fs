module ControlsBoundaryCompositionTests

open System
open Expecto
open GovernanceTestSupport

let expectControlsOnlyComposition relativePath =
    let source = read relativePath

    [ "open FS.Skia.UI.Controls"
      "TextBox.create"
      "Button.create"
      "LineChart.create"
      "DataGrid.create"
      "DataGrid.columns"
      "DataGrid.rows" ]
    |> List.iter (fun required ->
        Expect.stringContains source required $"{relativePath} composes form, chart, and DataGrid through Controls using {required}")

    [ "open FS.Skia.UI.Charts"
      "FS.Skia.UI.Charts"
      "#r \"../src/Charts" ]
    |> List.iter (fun forbidden ->
        Expect.isFalse (source.Contains(forbidden, StringComparison.Ordinal)) $"{relativePath} does not use legacy Charts authoring via {forbidden}")

let expectProjectUsesControlsOnly relativePath =
    let project = read relativePath

    let referencesControls =
        project.Contains("FS.Skia.UI.Controls", StringComparison.Ordinal)
        || project.Contains(@"src\Controls\Controls.fsproj", StringComparison.Ordinal)
        || project.Contains(@"..\..\src\Controls\Controls.fsproj", StringComparison.Ordinal)

    Expect.isTrue referencesControls $"{relativePath} references Controls"

    [ "FS.Skia.UI.Charts"
      "src/Charts/Charts.fsproj"
      "..\\..\\src\\Charts\\Charts.fsproj" ]
    |> List.iter (fun forbidden ->
        Expect.isFalse (project.Contains(forbidden, StringComparison.OrdinalIgnoreCase)) $"{relativePath} does not reference legacy Charts via {forbidden}")

let expectNoCopiedFrameworkAssets relativeRoot =
    [ "samples"
      "readiness"
      "src/Controls"
      "src/Charts"
      "src/Lib"
      "tests/Controls.Tests"
      "tests/Charts.Tests"
      "docs/architecture.md"
      "docs/subsystem-design.md"
      "specs/00" ]
    |> List.iter (fun forbidden ->
        let relative = $"{relativeRoot}/{forbidden}"
        Expect.isFalse (fileExists relative || directoryExists relative) $"{relativeRoot} does not copy framework asset {forbidden}")

let expectProductOwnedControlsTests relativePath =
    let source = read relativePath

    [ "controlsExampleView"
      "Control.count"
      "TextBox"
      "LineChart"
      "DataGrid" ]
    |> List.iter (fun required ->
        Expect.stringContains source required $"{relativePath} product tests cover {required}")

    [ "ControlsGallery"
      "ChartsGallery"
      "DataGridGallery"
      "specs/" ]
    |> List.iter (fun forbidden ->
        Expect.isFalse (source.Contains(forbidden, StringComparison.OrdinalIgnoreCase)) $"{relativePath} tests stay product-owned without {forbidden}")

[<Tests>]
let controlsBoundaryCompositionTests =
    testList "Controls boundary composition" [
        test "ControlsGallery sample composes form chart and DataGrid through Controls only" {
            expectControlsOnlyComposition "samples/ControlsGallery/Program.fs"
            expectProjectUsesControlsOnly "samples/ControlsGallery/ControlsGallery.fsproj"
        }

        test "generated product template composes form chart and DataGrid through Controls only" {
            expectControlsOnlyComposition "template/base/src/Product/Program.fs"
            expectProjectUsesControlsOnly "template/base/src/Product/Product.fsproj"
        }

        test "generated product template keeps Controls package references and source product-owned" {
            expectControlsOnlyComposition "template/base/src/Product/Program.fs"
            expectProjectUsesControlsOnly "template/base/src/Product/Product.fsproj"
            expectNoCopiedFrameworkAssets "template/base"
        }

        test "generated product tests cover product-owned form chart and DataGrid usage" {
            expectProductOwnedControlsTests "template/base/tests/Product.Tests/Tests.fs"
            expectNoCopiedFrameworkAssets "template/base"
        }
    ]
