module V2CommandContractTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let v2CommandContractTests =
    testList "V2 command contract" [
        test "build workflow exposes V2 target names and feature paths" {
            let content = read "build.fsx"

            // The active feature id is resolved authoritatively from
            // .specify/feature.json (spec 037, FR-001/FR-002); the former
            // hardcoded "007-v2-template-packaging" placeholder fallback was
            // removed, so it is no longer expected in build.fsx.
            [ "TemplatePack"
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
            // Feature 041: the dependency graph derives from the typed Targets DU (FR-001);
            // assert the rows against Targets.targetDependencyRows, not string-tuple literals.
            expectDependency "TemplateCheck" [ "TemplatePack"; "TemplateInstallSource"; "TemplateInstallPackage"; "TemplateInstantiate"; "TemplateSmoke" ]
            expectDependency "DependencyReport" []
            expectDependency "GeneratedGuidanceCheck" []
            expectDependency "TemplateDrift" []
            expectDependency "Ci" [ "CiPreflight"; "Verify" ]
            expectFileContains "build.fsx" [ "v1 plus v2 verification artifact set" ]
        }

        test "workflow self-check exercises V2 transition assertions" {
            let exitCode, stdout, stderr = runFakeTarget "BuildWorkflowCheck"
            let output = stdout + stderr
            Expect.equal exitCode 0 output
            Expect.stringContains output "BuildWorkflowCheck" "self-check target ran"
        }
    ]
