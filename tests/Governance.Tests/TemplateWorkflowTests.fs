module TemplateWorkflowTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let templateWorkflowTests =
    testList "Template workflow contract" [
        test "template package project declares local NuGet template metadata" {
            let templateProject = ".template.package/FS.Skia.UI.Template.fsproj"

            if fileExists templateProject then
                expectFileContains
                    templateProject
                    [ "<PackageType>Template</PackageType>"
                      "<PackageId>FS.Skia.UI.Template</PackageId>"
                      "artifacts"
                      "content"
                      ".template.config" ]
            else
                Expect.isFalse (directoryExists ".template.package") "generated projects do not carry packaging metadata"
        }

        test "TemplateCheck requires package install generation smoke and verdict artifacts" {
            expectFileContains
                "build.fsx"
                [ "TemplatePack"
                  "TemplateInstallSource"
                  "TemplateInstallPackage"
                  "TemplateInstantiate"
                  "TemplateSmoke"
                  "source-app"
                  "source-headless-scene"
                  "source-governed"
                  "source-sample-pack"
                  "package-app"
                  "package-headless-scene"
                  "package-governed"
                  "package-sample-pack"
                  "TemplateCheck Verdict" ]
        }

        test "V3 real interpreter evidence plan is recorded" {
            let plan = "specs/009-v3-modular-framework/readiness/us1-real-interpreter-evidence-plan.md"

            if fileExists plan then
                    expectFileContains
                        plan
                        [ "app-source"
                          "app-package"
                          "GeneratedProductCheck"
                          "selected local skills"
                          "Dev"
                          "Test"
                          "Verify" ]
            else
                Expect.isFalse (directoryExists "specs/009-v3-modular-framework") "generated projects do not carry source feature readiness"
        }
    ]
