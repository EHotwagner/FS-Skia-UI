open System
open System.IO
open System.Text.Json

let repositoryRoot =
    Path.GetFullPath(Path.Combine(__SOURCE_DIRECTORY__, ".."))

let outputRoot =
    Path.Combine(repositoryRoot, "specs", "035-api-discovery-names", "readiness", "package", "api-reference")

let repoPath (relative: string) =
    Path.Combine(repositoryRoot, relative.Replace('/', Path.DirectorySeparatorChar))

type PackageReference =
    { PackageId: string
      Version: string
      SourceFsiPaths: string list
      ReferenceOutputPath: string
      SymbolCount: int
      SampledSymbols: string list
      XmlSummaryCount: int
      UnsupportedSymbols: string list
      OmittedSymbolReasons: string list
      Diagnostics: string list
      AssemblyReflection: bool
      RepositorySourceAuthoringFallback: bool }

let packages =
    [ "FS.Skia.UI.Scene", [ "src/Scene/Scene.fsi" ]
      "FS.Skia.UI.SkiaViewer", [ "src/SkiaViewer/SkiaViewer.fsi" ]
      "FS.Skia.UI.Elmish", [ "src/Elmish/Elmish.fsi" ]
      "FS.Skia.UI.KeyboardInput", [ "src/KeyboardInput/KeyboardInput.fsi" ]
      "FS.Skia.UI.Layout", [ "src/Layout/Layout.fsi"; "src/Layout/Types.fsi"; "src/Layout/Graph.fsi"; "src/Layout/GraphValidation.fsi" ]
      "FS.Skia.UI.Controls",
      [ "src/Controls/Accessibility.fsi"
        "src/Controls/Attributes.fsi"
        "src/Controls/Catalog.fsi"
        "src/Controls/Charts.fsi"
        "src/Controls/Collections.fsi"
        "src/Controls/Control.fsi"
        "src/Controls/ControlRuntime.fsi"
        "src/Controls/CustomControl.fsi"
        "src/Controls/DataGrid.fsi"
        "src/Controls/Diagnostics.fsi"
        "src/Controls/RichText.fsi"
        "src/Controls/TextInput.fsi"
        "src/Controls/Theme.fsi"
        "src/Controls/Types.fsi" ]
      "FS.Skia.UI.Controls.Elmish", [ "src/Controls.Elmish/ControlsElmish.fsi" ]
      "FS.Skia.UI.Testing", [ "src/Testing/Testing.fsi" ] ]

let packageVersion =
    let props = repoPath "Directory.Packages.props"
    if File.Exists props then
        let text = File.ReadAllText props
        let marker = "PackageVersion Include=\"FS.Skia.UI.Scene\" Version=\""
        let index = text.IndexOf(marker, StringComparison.Ordinal)
        if index >= 0 then
            let start = index + marker.Length
            let stop = text.IndexOf("\"", start, StringComparison.Ordinal)
            if stop > start then text.Substring(start, stop - start) else "local"
        else
            "local"
    else
        "local"

let isSymbolLine (line: string) =
    let trimmed = line.TrimStart()
    trimmed.StartsWith("type ", StringComparison.Ordinal)
    || trimmed.StartsWith("val ", StringComparison.Ordinal)
    || trimmed.StartsWith("module ", StringComparison.Ordinal)
    || trimmed.StartsWith("| ", StringComparison.Ordinal)
    || (trimmed.Contains(":") && not (trimmed.StartsWith("///", StringComparison.Ordinal)))

let samplesFor packageId =
    match packageId with
    | "FS.Skia.UI.Scene" ->
        [ "type Rect ="
          "Width: float"
          "Height: float"
          "LinearGradient of startPoint: Point * endPoint: Point * colors: Color list"
          "DropShadow of dx: float * dy: float * blur: float * color: Color"
          "TextRun"
          "SceneElementKind" ]
    | "FS.Skia.UI.Controls" ->
        [ "type Control<'msg>"
          "KnownControl.TextBlock"
          "StandardAttributeName.VisibleRange"
          "DataGrid.create"
          "LineChart.series"
          "TextBox.onChanged" ]
    | "FS.Skia.UI.SkiaViewer" ->
        [ "type ViewerOptions ="
          "InitialSize: Size"
          "ViewerWindowPosition"
          "Coordinates of x: int * y: int" ]
    | "FS.Skia.UI.KeyboardInput" ->
        [ "KeyboardModel"
          "KeyboardEvent"
          "KeyDown"
          "KeyUp" ]
    | _ -> []

let writePackage packageId sources =
    let sourceTexts =
        sources
        |> List.map (fun relative ->
            let fullPath = repoPath relative
            if not (File.Exists fullPath) then
                failwithf "Missing curated .fsi input %s" relative

            relative, File.ReadAllText fullPath)

    let combined = sourceTexts |> List.map snd |> String.concat Environment.NewLine
    let lines = combined.Replace("\r\n", "\n").Split('\n')
    let symbolCount = lines |> Array.filter isSymbolLine |> Array.length
    let xmlSummaryCount = lines |> Array.filter (fun line -> line.TrimStart().StartsWith("///", StringComparison.Ordinal)) |> Array.length
    let outputPath = Path.Combine(outputRoot, packageId + ".md")
    let sampledSymbols = samplesFor packageId
    let omittedReasons = [ "none" ]
    let unsupportedSymbols: string list = []

    let markdown =
        [ yield $"# {packageId} Source-Shaped API Reference"
          yield ""
          yield $"package-id: {packageId}"
          yield $"package-version: {packageVersion}"
          yield "generated-from: curated-fsi"
          yield "assembly-reflection: false"
          yield "repository-source-authoring-fallback: false"
          yield $"symbol-count: {symbolCount}"
          yield $"xml-summary-count: {xmlSummaryCount}"
          yield "source-fsi-paths:"
          yield! sources |> List.map (fun source -> $"- {source}")
          yield "sampled-symbols:"
          yield! sampledSymbols |> List.map (fun sample -> $"- {sample}")
          yield "omitted-symbol-reasons:"
          yield! omittedReasons |> List.map (fun reason -> $"- {reason}")
          yield "unsupported-symbols:"
          if List.isEmpty unsupportedSymbols then
              yield "- none"
          else
              yield! unsupportedSymbols |> List.map (fun symbol -> $"- {symbol}")
          yield "diagnostics:"
          yield "- none"
          yield ""
          yield "## Common Samples"
          yield! sampledSymbols |> List.map (fun sample -> $"- `{sample}`")
          yield ""
          yield "## Curated Signatures"
          yield "```fsharp"
          yield combined
          yield "```" ]
        |> String.concat Environment.NewLine

    File.WriteAllText(outputPath, markdown + Environment.NewLine)

    { PackageId = packageId
      Version = packageVersion
      SourceFsiPaths = sources
      ReferenceOutputPath = outputPath
      SymbolCount = symbolCount
      SampledSymbols = sampledSymbols
      XmlSummaryCount = xmlSummaryCount
      UnsupportedSymbols = unsupportedSymbols
      OmittedSymbolReasons = omittedReasons
      Diagnostics = []
      AssemblyReflection = false
      RepositorySourceAuthoringFallback = false }

Directory.CreateDirectory outputRoot |> ignore

let reports =
    packages |> List.map (fun (packageId, sources) -> writePackage packageId sources)

let index =
    [ yield "# Package API Reference Index"
      yield ""
      yield "generated-from: curated-fsi"
      yield "assembly-reflection: false"
      yield "repository-source-authoring-fallback: false"
      yield ""
      yield "| Package | Reference |"
      yield "|---------|-----------|"
      yield!
          reports
          |> List.map (fun report -> $"| {report.PackageId} | `{Path.GetFileName report.ReferenceOutputPath}` |") ]
    |> String.concat Environment.NewLine

File.WriteAllText(Path.Combine(outputRoot, "index.md"), index + Environment.NewLine)

let jsonOptions = JsonSerializerOptions(WriteIndented = true)
File.WriteAllText(Path.Combine(outputRoot, "report.json"), JsonSerializer.Serialize(reports, jsonOptions) + Environment.NewLine)

printfn "Wrote package API reference for %d packages to %s" reports.Length outputRoot
