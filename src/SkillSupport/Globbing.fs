namespace FS.Skia.UI.SkillSupport

open System.IO
open System.Text
open System.Text.RegularExpressions
open DiffPlex
open DiffPlex.DiffBuilder
open DiffPlex.DiffBuilder.Model

module Globbing =

    let private globToRegex (glob: string) =
        let g = glob.Replace('\\', '/')
        let sb = StringBuilder()
        sb.Append('^') |> ignore
        let mutable i = 0
        while i < g.Length do
            let c = g.[i]
            if c = '*' then
                if i + 1 < g.Length && g.[i + 1] = '*' then
                    // `**` crosses '/'. `**/` also matches zero directories.
                    if i + 2 < g.Length && g.[i + 2] = '/' then
                        sb.Append("(?:.*/)?") |> ignore
                        i <- i + 3
                    else
                        sb.Append(".*") |> ignore
                        i <- i + 2
                else
                    sb.Append("[^/]*") |> ignore
                    i <- i + 1
            elif c = '?' then
                sb.Append("[^/]") |> ignore
                i <- i + 1
            else
                match c with
                | '.' | '(' | ')' | '+' | '|' | '^' | '$' | '{' | '}' | '[' | ']' | '\\' ->
                    sb.Append('\\').Append(c) |> ignore
                | _ -> sb.Append(c) |> ignore
                i <- i + 1
        sb.Append('$') |> ignore
        sb.ToString()

    let isMatch (glob: string) (path: string) : bool =
        let normalized = path.Replace('\\', '/')
        Regex.IsMatch(normalized, globToRegex glob)

    let discover (root: string) (globs: string list) : string list =
        if not (Directory.Exists root) then
            []
        else
            let rootFull = Path.GetFullPath root
            Directory.EnumerateFiles(rootFull, "*", SearchOption.AllDirectories)
            |> Seq.map (fun full ->
                Path.GetRelativePath(rootFull, full).Replace('\\', '/'))
            |> Seq.filter (fun rel -> globs |> List.exists (fun glob -> isMatch glob rel))
            |> Seq.sort
            |> List.ofSeq

    let currencyDiff (expected: string) (actual: string) : string list =
        let model = InlineDiffBuilder(Differ()).BuildDiffModel(expected, actual)
        model.Lines
        |> Seq.filter (fun line -> line.Type <> ChangeType.Unchanged)
        |> Seq.map (fun line -> sprintf "%A | %s" line.Type line.Text)
        |> List.ofSeq
