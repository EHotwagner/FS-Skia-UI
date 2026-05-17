module V2CommandContractTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let v2CommandContractTests =
    testList "V2 command contract" [
        test "build workflow exposes V2 target names and feature paths" {
            let content = read "build.fsx"

            [ "007-v2-template-packaging"
              "TemplatePack"
              "TemplateInstallSource"
              "TemplateInstallPackage"
              "TemplateInstantiate"
              "TemplateSmoke"
              "TemplateCheck"
              "DependencyReport"
              "GeneratedGuidanceCheck"
              "TemplateDrift"
              "artifacts"
              "templates"
              "template-check"
              "dependencies.md"
              "generated-guidance.md"
              "template-drift.md" ]
            |> List.iter (fun needle -> Expect.stringContains content needle $"build.fsx contains {needle}")
        }

        test "V2 workflow preserves MVU effect boundary and emitted effect assertions" {
            expectFileContains
                "build.fsx"
                [ "type BuildModel"
                  "type BuildMsg"
                  "type BuildEffect"
                  "InstallTemplate"
                  "InstantiateTemplates"
                  "ScanGeneratedProjects"
                  "ValidateTemplatePackage"
                  "GeneratedGuidanceScan"
                  "WorkflowSelfCheck"
                  "TargetCompleted must be a pure state transition with no effects" ]
        }

        test "Verify and Ci compose V1 plus V2 gates" {
            expectFileContains
                "build.fsx"
                [ "\"TemplateCheck\", [ \"TemplatePack\"; \"TemplateInstallSource\"; \"TemplateInstallPackage\"; \"TemplateInstantiate\"; \"TemplateSmoke\" ]"
                  "\"DependencyReport\", []"
                  "\"GeneratedGuidanceCheck\", []"
                  "\"TemplateDrift\", []"
                  "\"Ci\", [ \"CiPreflight\"; \"Verify\" ]"
                  "v1 plus v2 verification artifact set" ]
        }

        test "workflow self-check exercises V2 transition assertions" {
            let exitCode, stdout, stderr = runFakeTarget "BuildWorkflowCheck"
            let output = stdout + stderr
            Expect.equal exitCode 0 output
            Expect.stringContains output "BuildWorkflowCheck" "self-check target ran"
        }
    ]
