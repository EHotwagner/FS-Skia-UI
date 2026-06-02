module FS.Skia.UI.Build.PerPackageSurface

open System
open System.IO
open System.Text
open DiffPlex
open DiffPlex.Model

type PackageId = string

type Surface =
    { PackageId: PackageId
      NormalizedText: string }

type SurfaceLineChange =
    | Added of string
    | Removed of string

type PackageDrift =
    { PackageId: PackageId
      Changes: SurfaceLineChange list }

type DiffOutcome =
    { Drifted: PackageDrift list
      CheckedPackages: PackageId list
      MissingBaselines: PackageId list }

// The 8 public split packages, in declared scope order. The retiring monolith
// `FS.Skia.UI` (src/Lib) and the build-tooling `FS.Skia.UI.Build` are excluded by
// construction (plan §Packages-in-scope; FR-006).
let packagesInScope: PackageId list =
    [ "FS.Skia.UI.Scene"
      "FS.Skia.UI.SkiaViewer"
      "FS.Skia.UI.Elmish"
      "FS.Skia.UI.KeyboardInput"
      "FS.Skia.UI.Layout"
      "FS.Skia.UI.Controls"
      "FS.Skia.UI.Controls.Elmish"
      "FS.Skia.UI.Testing" ]

// Each in-scope PackageId maps to its `src/<dir>` source directory; `captureCurrent`
// aggregates every `*.fsi` under it in filename order (so Layout/Controls multi-file
// packages collapse to one deterministic surface — research.md D2).
let private packageSourceDir (packageId: PackageId) =
    match packageId with
    | "FS.Skia.UI.Scene" -> "Scene"
    | "FS.Skia.UI.SkiaViewer" -> "SkiaViewer"
    | "FS.Skia.UI.Elmish" -> "Elmish"
    | "FS.Skia.UI.KeyboardInput" -> "KeyboardInput"
    | "FS.Skia.UI.Layout" -> "Layout"
    | "FS.Skia.UI.Controls" -> "Controls"
    | "FS.Skia.UI.Controls.Elmish" -> "Controls.Elmish"
    | "FS.Skia.UI.Testing" -> "Testing"
    | other -> failwithf "PerPackageSurface: %s is not an in-scope split package" other

// --- pure core ----------------------------------------------------------------

// Strip `(* ... *)` block comments (nested-aware), preserving the newlines that sit
// OUTSIDE the comment markers so declarations on separate lines never merge.
let private stripBlockComments (text: string) =
    let sb = StringBuilder(text.Length)
    let n = text.Length
    let mutable depth = 0
    let mutable i = 0

    while i < n do
        if i + 1 < n && text.[i] = '(' && text.[i + 1] = '*' then
            depth <- depth + 1
            i <- i + 2
        elif i + 1 < n && text.[i] = '*' && text.[i + 1] = ')' && depth > 0 then
            depth <- depth - 1
            i <- i + 2
        elif depth > 0 then
            // inside a block comment: drop the character, but keep newlines so the
            // surrounding line structure is preserved.
            if text.[i] = '\n' then sb.Append('\n') |> ignore
            i <- i + 1
        else
            sb.Append(text.[i]) |> ignore
            i <- i + 1

    sb.ToString()

// Remove a `//` line comment (covers `///` doc comments) from a single line.
let private stripLineComment (line: string) =
    match line.IndexOf("//", StringComparison.Ordinal) with
    | -1 -> line
    | index -> line.Substring(0, index)

let normalize (raw: string) =
    let unified = raw.Replace("\r\n", "\n").Replace("\r", "\n")
    let noBlocks = stripBlockComments unified

    let lines =
        noBlocks.Split('\n')
        |> Array.map (fun line -> (stripLineComment line).TrimEnd())

    // Collapse runs of blank lines to a single blank line, then trim leading/trailing
    // blanks, preserving declaration order.
    let collapsed =
        lines
        |> Array.fold
            (fun acc line ->
                match acc with
                | last :: _ when last = "" && line = "" -> acc
                | _ -> line :: acc)
            []
        |> List.rev

    let trimmed =
        collapsed
        |> List.skipWhile (fun l -> l = "")
        |> List.rev
        |> List.skipWhile (fun l -> l = "")
        |> List.rev

    String.Join("\n", trimmed)

let private lineChanges (baselineText: string) (currentText: string) =
    let result = Differ.Instance.CreateLineDiffs(baselineText, currentText, false)

    [ for block in result.DiffBlocks do
          for i in 0 .. block.DeleteCountA - 1 do
              yield Removed(result.PiecesOld.[block.DeleteStartA + i])

          for i in 0 .. block.InsertCountB - 1 do
              yield Added(result.PiecesNew.[block.InsertStartB + i]) ]

let diffPackage (baseline: Surface) (current: Surface) =
    if baseline.NormalizedText = current.NormalizedText then
        None
    else
        let changes = lineChanges baseline.NormalizedText current.NormalizedText

        if List.isEmpty changes then
            None
        else
            Some
                { PackageId = current.PackageId
                  Changes = changes }

let diff (baselines: Surface list) (current: Surface list) =
    let baselineMap =
        baselines |> List.map (fun s -> s.PackageId, s) |> Map.ofList

    let drifted =
        current
        |> List.choose (fun cur ->
            match Map.tryFind cur.PackageId baselineMap with
            | Some baseline -> diffPackage baseline cur
            | None -> None)

    let missing =
        current
        |> List.filter (fun cur -> not (baselineMap.ContainsKey cur.PackageId))
        |> List.map (fun cur -> cur.PackageId)

    { Drifted = drifted
      CheckedPackages = current |> List.map (fun s -> s.PackageId)
      MissingBaselines = missing }

// --- edge interpreter ---------------------------------------------------------

let rec private findRepoRoot (dir: string) =
    if String.IsNullOrEmpty dir then
        Directory.GetCurrentDirectory()
    elif Directory.Exists(Path.Combine(dir, "src")) && File.Exists(Path.Combine(dir, ".specify", "feature.json")) then
        dir
    else
        match Path.GetDirectoryName dir with
        | null -> Directory.GetCurrentDirectory()
        | parent -> findRepoRoot parent

let captureCurrent (packages: PackageId list) =
    let root = findRepoRoot (Directory.GetCurrentDirectory())

    packages
    |> List.map (fun packageId ->
        let dir = Path.Combine(root, "src", packageSourceDir packageId)

        let raw =
            if Directory.Exists dir then
                Directory.GetFiles(dir, "*.fsi")
                |> Array.sortBy Path.GetFileName
                |> Array.map File.ReadAllText
                |> String.concat "\n"
            else
                failwithf "PerPackageSurface.captureCurrent: source directory not found: %s" dir

        { PackageId = packageId
          NormalizedText = normalize raw })

let loadBaselines (directory: string) =
    if not (Directory.Exists directory) then
        []
    else
        Directory.GetFiles(directory, "*.fsi.txt")
        |> Array.sortBy Path.GetFileName
        |> Array.map (fun file ->
            let packageId = Path.GetFileName(file).Replace(".fsi.txt", "")

            { PackageId = packageId
              NormalizedText = normalize (File.ReadAllText file) })
        |> Array.toList

let private renderChange change =
    match change with
    | Added line -> sprintf "    + %s" line
    | Removed line -> sprintf "    - %s" line

let runReport (reportPath: string) (outcome: DiffOutcome) =
    let clean = List.isEmpty outcome.Drifted && List.isEmpty outcome.MissingBaselines

    let body =
        [ yield "# Per-package surface diff"
          yield ""
          yield sprintf "Checked %d package(s) in scope." (List.length outcome.CheckedPackages)
          yield ""
          yield!
              (outcome.CheckedPackages
               |> List.map (fun p -> sprintf "- `%s`" p))
          yield ""
          if clean then
              yield sprintf "**Zero drift across %d packages.**" (List.length outcome.CheckedPackages)
          else
              if not (List.isEmpty outcome.MissingBaselines) then
                  yield "## Missing baselines (fail loud — Principle VII)"
                  yield ""

                  for packageId in outcome.MissingBaselines do
                      yield
                          sprintf
                              "- `%s` has no baseline at `readiness/per-package-surface/%s.fsi.txt` — capture it before this gate can pass."
                              packageId
                              packageId

                  yield ""

              if not (List.isEmpty outcome.Drifted) then
                  yield "## Drift"
                  yield ""

                  for drift in outcome.Drifted do
                      yield
                          sprintf
                              "### `%s` — update `readiness/per-package-surface/%s.fsi.txt`"
                              drift.PackageId
                              drift.PackageId
                      yield ""
                      yield "```diff"
                      yield! (drift.Changes |> List.map renderChange)
                      yield "```"
                      yield "" ]
        |> String.concat "\n"

    let parent = Path.GetDirectoryName reportPath

    if not (String.IsNullOrEmpty parent) then
        Directory.CreateDirectory parent |> ignore

    File.WriteAllText(reportPath, body + "\n")
    clean
