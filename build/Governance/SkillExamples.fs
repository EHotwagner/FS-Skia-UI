module FS.Skia.UI.Build.SkillExamples

open System
open System.Text

type CodeBlock =
    { SkillSlug: string
      BlockIndex: int
      StartLine: int
      Source: string }

let underscoreSlug (slug: string) = slug.Replace('-', '_')

let private blockSuffix (index: int) = sprintf "Block%02d" index

let moduleName (block: CodeBlock) =
    $"Skill.{underscoreSlug block.SkillSlug}.{blockSuffix block.BlockIndex}"

// Split on \n keeping behaviour identical across CRLF/LF (we strip a trailing
// \r per line) so extraction is line-ending agnostic for the fence scan while
// the original Source text is preserved verbatim between the fences.
let private splitLines (text: string) =
    text.Replace("\r\n", "\n").Split('\n')

let extractBlocks (skillSlug: string) (markdown: string) =
    let lines = splitLines markdown

    let rec loop i nextIndex acc =
        if i >= lines.Length then
            List.rev acc
        else
            let trimmed = lines.[i].Trim()

            if trimmed = "```fsharp" then
                // collect until the closing fence
                let startLine = i + 1
                let mutable j = i + 1
                let body = StringBuilder()
                let mutable first = true

                while j < lines.Length && lines.[j].Trim() <> "```" do
                    if not first then
                        body.Append('\n') |> ignore

                    body.Append(lines.[j]) |> ignore
                    first <- false
                    j <- j + 1

                let block =
                    { SkillSlug = skillSlug
                      BlockIndex = nextIndex
                      StartLine = startLine
                      Source = body.ToString() }

                // resume after the closing fence (j points at ``` or EOF)
                loop (j + 1) (nextIndex + 1) (block :: acc)
            else
                loop (i + 1) nextIndex acc

    loop 0 1 []

let private indentSource (source: string) =
    splitLines source
    |> Array.map (fun line -> if line.Length = 0 then "" else "    " + line)
    |> fun arr -> String.Join("\n", arr)

let renderSkillFile (skillRelPath: string) (blocks: CodeBlock list) =
    let sb = StringBuilder()
    sb.Append("// GENERATED — do not edit. Tangled from the SKILL.md ```fsharp blocks by\n") |> ignore
    sb.Append("// FS.Skia.UI.Build.SkillExamples (feature 040). Regenerate via SkillExamplesCheck.\n") |> ignore

    match blocks with
    | [] -> ()
    | first :: _ ->
        sb.Append($"namespace Skill.{underscoreSlug first.SkillSlug}\n\n") |> ignore

        for block in blocks do
            sb.Append($"// source: {skillRelPath}:{block.StartLine}\n") |> ignore
            sb.Append($"module {blockSuffix block.BlockIndex} =\n") |> ignore
            sb.Append(indentSource block.Source) |> ignore
            sb.Append("\n\n") |> ignore

    sb.ToString()

let extractAll (skills: (string * string * string) list) =
    skills
    |> List.map (fun (slug, _path, markdown) -> slug, extractBlocks slug markdown)
