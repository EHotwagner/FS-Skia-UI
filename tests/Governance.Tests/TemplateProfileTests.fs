module TemplateProfileTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let templateProfileTests =
    testList "Template profile metadata" [
        test "template identity and profile choices are declared" {
            if fileExists ".template.config/template.json" then
                let content = read ".template.config/template.json"

                [ "\"shortName\": \"fs-skia-ui\""
                  "\"identity\": \"FS.Skia.UI.Template\""
                  "\"profile\""
                  "\"choice\": \"default\""
                  "\"choice\": \"minimal\""
                  "\"rootNamespace\""
                  "\"packagePrefix\""
                  "\"authors\""
                  "\"repositoryUrl\""
                  "\"targetFramework\""
                  "\"skipGitInit\""
                  "\"postActions\""
                  ".template.config/generated/"
                  "chmod +x"
                  "git init"
                  "3A7C4B45-1F5D-4A30-959A-51B88E82B5D2" ]
                |> List.iter (fun needle -> Expect.stringContains content needle $"template.json contains {needle}")
            else
                Expect.isFalse (directoryExists ".template.config") "generated projects do not carry template metadata"
        }

        test "source and minimal modifiers exclude source-only history and optional scope" {
            if fileExists ".template.config/template.json" then
                expectFileContains
                    ".template.config/template.json"
                    [ "specs/**"
                      ".template.package/**"
                      "artifacts/**"
                      "src/Charts/**"
                      "src/Layout/**"
                      "tests/Parity.Tests/**"
                      "samples/InteractiveViewer/**"
                      "samples/ScreenshotGallery/**" ]
            else
                Expect.isFalse (directoryExists "specs/007-v2-template-packaging") "generated projects exclude source-only history"
        }

        test "generated AGENTS guidance is not tied to source active feature" {
            if fileExists ".template.config/generated/AGENTS.md" then
                let content = read ".template.config/generated/AGENTS.md"
                Expect.stringContains content "specs/<feature>/plan.md" "generated AGENTS points at generated feature plans"
                Expect.isFalse (content.Contains "specs/008-targeted-refactor-governance") "generated AGENTS omits source-only active feature"
            else
                Expect.isFalse (directoryExists ".template.config") "generated projects do not carry template metadata"
        }
    ]
