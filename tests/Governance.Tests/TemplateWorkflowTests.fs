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
                  "source-default"
                  "source-minimal"
                  "package-default"
                  "package-minimal"
                  "TemplateCheck Verdict" ]
        }

        test "US1 real interpreter evidence plan is recorded" {
            let plan = "specs/007-v2-template-packaging/readiness/us1-real-interpreter-plan.md"

            if fileExists plan then
                expectFileContains
                    plan
                    [ "source install"
                      "package install"
                      "default"
                      "minimal"
                      "generated Dev" ]
            else
                Expect.isFalse (directoryExists "specs/007-v2-template-packaging") "generated projects do not carry source feature readiness"
        }
    ]
