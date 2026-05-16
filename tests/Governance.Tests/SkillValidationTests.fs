module SkillValidationTests

open Expecto
open GovernanceTestSupport

let requiredSkillSections =
    [ "Scope"
      "Public Contract"
      "Build Commands"
      "Test Commands"
      "Evidence"
      "Package Boundary"
      "Generated Product" ]

[<Tests>]
let skillValidationTests =
    testList "V3 local skill validation" [
        test "capability skills declare required sections and command references" {
            readCatalogCapabilities ()
            |> List.iter (fun capability ->
                match capability.Skill with
                | Some skillPath ->
                    if fileExists skillPath then
                        let content = read skillPath

                        requiredSkillSections
                        |> List.iter (fun section -> Expect.stringContains content $"## {section}" $"{skillPath} contains {section}")

                        Expect.stringContains content "./fake.sh build -t" $"{skillPath} names FAKE verification commands"
                    else
                        Expect.isFalse (directoryExists "specs/009-v3-modular-framework") $"{capability.Id} skill exists at {skillPath}"
                | None -> failtestf "%s has no skill path" capability.Id)
        }

        test "generated product receives selected skills only" {
            expectFileContains
                "build.fsx"
                [ "copySelectedSkills"
                  "fs-skia-project"
                  "fs-skia-scene"
                  "fs-skia-skiaviewer"
                  "fs-skia-elmish"
                  "fs-skia-keyboard-input"
                  "fs-skia-layout"
                  "fs-skia-charts"
                  "unrelated capability skills" ]
        }
    ]
