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
                  "\"targetFramework\"" ]
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
    ]
