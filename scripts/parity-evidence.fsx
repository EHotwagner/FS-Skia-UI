open System
open System.IO

#r "nuget: Fable.Elmish, 4.2.0"
#r "../src/Lib/bin/Debug/net10.0/FS.Skia.UI.dll"

open FS.Skia.UI

let repoRoot = Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))
let historicalFeature = Path.Combine(repoRoot, "specs", "002-skia-feature-parity")
let historicalReadiness = Path.Combine(historicalFeature, "readiness")
let readiness =
    if File.Exists(Path.Combine(historicalFeature, "spec.md")) then
        historicalReadiness
    else
        Path.Combine(repoRoot, "readiness", "parity")
let evidencePath (relative: string) = relative.Replace("\\", "/")

let items =
    [ Parity.createItem
          "core-viewer"
          "Windowed Vulkan viewer, Elmish event mapping, lifecycle, input, diagnostics, screenshot requests"
          Adapted
          Smoke
          "dotnet run --project samples/InteractiveViewer/InteractiveViewer.fsproj -- --contract-smoke"
          (evidencePath "readiness/smoke/t061-interactiveviewer-contract.txt")
          "Baseline observable scene/input streams are adapted to Elmish Model/Msg/ViewerEffect; GL fallback is excluded by project constraint."
          false
      Parity.createItem
          "scene-dsl"
          "Scene creation, grouping, primitives, text, images, paths, transforms, clipping, paint helpers, reusable pictures"
          Supported
          SemanticTest
          "dotnet test tests/Lib.Tests/Lib.Tests.fsproj"
          (evidencePath "readiness/logs/t026-lib-tests.txt")
          "Implemented as immutable FS.Skia.UI scene declarations."
          false
      Parity.createItem
          "rendering-skia-translation"
          "Primitive rendering, frame recovery, diagnostics, Skia paint/shader/filter/path effect translation"
          Supported
          Screenshot
          "dotnet test tests/Lib.Tests/Lib.Tests.fsproj"
          (evidencePath "readiness/screenshots/us1-render-readback.txt")
          "Render-readback evidence is deterministic semantic evidence; Vulkan smoke evidence is captured separately where available."
          false
      Parity.createItem
          "shaders-effects"
          "Gradient, blend, color filter, mask filter, image filter, path effect, and effect gallery coverage"
          Adapted
          Smoke
          "dotnet run --project samples/EffectsGallery/EffectsGallery.fsproj -- --contract-smoke"
          (evidencePath "readiness/smoke/t029-effectsgallery-contract.txt")
          "Baseline effect vocabulary is mapped to supported Skia declarations; unavailable device capabilities report diagnostics."
          false
      Parity.createItem
          "screenshots"
          "PNG/JPEG screenshot workflow, post-frame gating, output path handling, capture diagnostics"
          Supported
          Smoke
          "dotnet run --project samples/ScreenshotGallery/ScreenshotGallery.fsproj -- --contract-smoke"
          (evidencePath "readiness/smoke/t061-screenshotgallery-contract.txt")
          "Screenshots are requested through Elmish effects and interpreted at the viewer edge."
          false
      Parity.createItem
          "performance-evidence"
          "Large scene, chart, DataGrid, graph, smoke, and first-frame evidence commands"
          Supported
          SemanticTest
          "dotnet test"
          (evidencePath "readiness/logs/final-pass-dotnet-test.txt")
          "Scale tests cover 100,000 chart points, 10,000 grid rows, 100-node DAG layout, and smoke timing artifacts."
          false
      Parity.createItem
          "charts"
          "Line, bar, pie/donut, scatter, area, histogram, candlestick, radar, axes, legends, palettes, scale helpers"
          Supported
          SemanticTest
          "dotnet test tests/Charts.Tests/Charts.Tests.fsproj"
          (evidencePath "readiness/logs/t038-charts-tests-rerun.txt")
          "Charts are pure view-layer scene builders."
          false
      Parity.createItem
          "datagrid"
          "Columns, typed cell values, sorting, scrolling/viewport behavior, fixed headers, configurable colors and text"
          Supported
          SemanticTest
          "dotnet test tests/Charts.Tests/Charts.Tests.fsproj"
          (evidencePath "readiness/logs/t038-charts-tests-rerun.txt")
          "DataGrid state remains consumer-owned and is projected into pure scene props."
          false
      Parity.createItem
          "layout"
          "HStack, VStack, Dock, sizing, spacing, padding, alignment, nested composition"
          Supported
          SemanticTest
          "dotnet test tests/Layout.Tests/Layout.Tests.fsproj"
          (evidencePath "readiness/logs/t049-layout-tests-rerun.txt")
          "Layout builders are pure scene composition helpers."
          false
      Parity.createItem
          "graphs"
          "Directed DAG rendering, undirected weighted graph rendering, validation, cycle and endpoint diagnostics"
          Supported
          SemanticTest
          "dotnet test tests/Layout.Tests/Layout.Tests.fsproj"
          (evidencePath "readiness/logs/t049-layout-tests-rerun.txt")
          "Graph layout and validation are deterministic pure helpers."
          false
      Parity.createItem
          "examples-demos"
          "Basic, interactive, parity, effects, charts, DataGrid, layout/graph, screenshot, and demo reel samples"
          Supported
          Smoke
          "dotnet test tests/Smoke.Tests/Smoke.Tests.fsproj"
          (evidencePath "readiness/smoke/")
          "Samples expose Vulkan-only Elmish workflows without fallback renderer controls."
          false
      Parity.createItem
          "documentation"
          "Quickstart, parity matrix, Vulkan-only adaptations, Elmish-only adaptations, package and sample commands"
          Adapted
          Documentation
          "dotnet test tests/Parity.Tests/Parity.Tests.fsproj"
          (evidencePath "readiness/parity/parity-matrix.md")
          "Documentation maps baseline reactive and fallback APIs into Elmish-only Vulkan workflows."
          false ]

let report = Parity.createReport "002-skia-feature-parity" items
let failures = Parity.validateMergeReady report

if failures.Length > 0 then
    failures |> List.iter (eprintfn "parity validation: %s")
    exit 2

let output = Path.Combine(readiness, "parity-evidence.json")
Parity.writeJson output report
printfn "wrote %s" output
