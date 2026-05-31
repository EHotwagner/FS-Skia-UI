module SkillExamplesTangleTests

(* Feature 040 / US4 — failing-first (Principle I/VI) semantic tests for the
   ` ```fsharp ` block extractor and the tangler that renders the generated
   examples-project source. These exercise the real
   `FS.Skia.UI.Build.SkillExamples` functions (no mocks); before
   SkillExamples.fs existed they did not compile. *)

open Expecto
open FS.Skia.UI.Build

let private sampleMarkdown =
    String.concat "\n"
        [ "# heading"
          "intro prose"
          ""
          "```fsharp"
          "let x = 1"
          "let y = x + 1"
          "```"
          ""
          "some text"
          ""
          "```bash"
          "echo not-fsharp"
          "```"
          ""
          "```fsharp"
          "open System"
          "let _ = Console.Out"
          "```"
          "" ]

[<Tests>]
let skillExamplesTangleTests =
    testList "Feature 040 SkillExamples extractor and tangler" [

        test "extractBlocks returns only fsharp blocks with stable per-skill identity, in document order" {
            let blocks = SkillExamples.extractBlocks "fsharp-parsing" sampleMarkdown
            Expect.equal (List.length blocks) 2 "two ```fsharp blocks (the ```bash block is ignored)"

            let first = blocks.[0]
            let second = blocks.[1]
            Expect.equal first.BlockIndex 1 "first block is index 1 (1-based, per skill)"
            Expect.equal second.BlockIndex 2 "second block is index 2"
            Expect.isLessThan first.StartLine second.StartLine "document order is preserved by start line"
            Expect.stringContains first.Source "let x = 1" "first block keeps its verbatim source"
            Expect.stringContains second.Source "open System" "second block keeps its verbatim source"
            Expect.isFalse (first.Source.Contains "```") "the fences themselves are excluded from Source"
        }

        test "moduleName encodes Skill.<slug_underscored>.Block<NN>" {
            let blocks = SkillExamples.extractBlocks "fsharp-parsing" sampleMarkdown
            Expect.equal (SkillExamples.moduleName blocks.[0]) "Skill.fsharp_parsing.Block01" "padded, underscored, fully qualified"
            Expect.equal (SkillExamples.moduleName blocks.[1]) "Skill.fsharp_parsing.Block02" "second block name"
        }

        test "renderSkillFile wraps each block in a source-tagged module and is deterministic" {
            let blocks = SkillExamples.extractBlocks "fsharp-parsing" sampleMarkdown
            let path = ".claude/skills/fsharp-parsing/SKILL.md"
            let rendered = SkillExamples.renderSkillFile path blocks

            Expect.stringContains rendered "namespace Skill.fsharp_parsing" "namespace header present"
            Expect.stringContains rendered "module Block01 =" "first block module wrapper"
            Expect.stringContains rendered "module Block02 =" "second block module wrapper"
            Expect.stringContains rendered $"// source: {path}:{blocks.[0].StartLine}" "block 1 carries its source coordinate"
            Expect.stringContains rendered $"// source: {path}:{blocks.[1].StartLine}" "block 2 carries its source coordinate"
            Expect.stringContains rendered "    let x = 1" "block source is indented under its module"

            let again = SkillExamples.renderSkillFile path blocks
            Expect.equal rendered again "tangling is deterministic across runs"
        }

        test "extractAll keys blocks by slug" {
            let result =
                SkillExamples.extractAll
                    [ "fsharp-parsing", ".claude/skills/fsharp-parsing/SKILL.md", sampleMarkdown
                      "fsharp-shell-process", ".claude/skills/fsharp-shell-process/SKILL.md", "no blocks here" ]

            Expect.equal (result |> List.map fst) [ "fsharp-parsing"; "fsharp-shell-process" ] "slugs preserved in order"
            Expect.equal (result |> List.find (fun (s, _) -> s = "fsharp-parsing") |> snd |> List.length) 2 "parsing has two blocks"
            Expect.isEmpty (result |> List.find (fun (s, _) -> s = "fsharp-shell-process") |> snd) "no blocks → empty list, not failure"
        }
    ]
