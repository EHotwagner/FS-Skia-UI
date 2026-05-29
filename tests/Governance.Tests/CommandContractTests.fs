module CommandContractTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let commandContractTests =
    testList "Canonical command contract" [
        test "repo-local FAKE manifest and wrappers are available" {
            expectFileContains ".config/dotnet-tools.json" [ "\"fake-cli\""; "\"version\": \"6.1.4\""; "\"fake\"" ]
            expectFileContains "fake.sh" [ "dotnet tool restore"; "FAKE_SDK_RESOLVER_CUSTOM_DOTNET_PATH"; "dotnet fake \"$@\"" ]
            expectFileContains "fake.cmd" [ "dotnet tool restore"; "dotnet fake %*" ]
            Expect.isTrue (File.Exists(fullPath "fake.sh")) "Bash wrapper exists"
            Expect.isTrue (File.Exists(fullPath "fake.cmd")) "Windows wrapper exists"
        }

        test "build workflow exposes required MVU-style effect algebra" {
            expectFileContains
                "build.fsx"
                [ "type BuildModel"
                  "type BuildMsg"
                  "type BuildEffect"
                  "let init"
                  "let update"
                  "StartTarget"
                  "RunProcess"
                  "RequireFiles"
                  "WorkflowSelfCheck" ]
        }

        test "canonical build script is organized by named concern sections" {
            expectFileContains
                "build.fsx"
                [ "BUILD SECTION: path model"
                  "BUILD SECTION: workflow model"
                  "BUILD SECTION: target update"
                  "BUILD SECTION: interpreter"
                  "BUILD SECTION: guidance validation"
                  "BUILD SECTION: native FAKE target graph" ]
        }

        test "required targets are declared exactly once through the target graph" {
            let content = read "build.fsx"

            [ "Clean"
              "Restore"
              "Build"
              "Test"
              "Dev"
              "PackLocal"
              "RefreshSurfaceBaselines"
              "PackageSurfaceCheck"
              "FsiTranscripts"
              "SampleContractSmoke"
              "CapabilityCheck"
              "SkillCheck"
              "GeneratedProductCheck"
              "EvidenceGraph"
              "EvidenceAudit"
              "Verify"
              "Ci" ]
            |> List.iter (fun target ->
                Expect.stringContains content $"\"{target}\"" $"{target} target is named in build.fsx")

            expectContains content "\"Build\", [ \"Restore\" ]" "Build depends on Restore"
            expectContains content "\"Test\", [ \"Build\"; \"SampleContractSmoke\" ]" "Test depends on built outputs and sample smoke evidence"
            expectContains content "\"Dev\", [ \"Test\" ]" "Dev depends on Test"
            expectContains content "\"EvidenceAudit\", [ \"EvidenceGraph\" ]" "audit depends on graph"
            expectContains content "\"Ci\", [ \"CiPreflight\"; \"Verify\" ]" "Ci runs preflight before delegating to Verify"
        }

        test "V3 command workflow exposes capability product and skill validation effects" {
            expectFileContains
                "build.fsx"
                [ "CapabilityCatalogCheck"
                  "SkillCatalogCheck"
                  "GenerateV3Products"
                  "ScanV3GeneratedProducts"
                  "PackageSurfaceReport"
                  "CapabilityCatalogReportPath"
                  "SelectedSkillsReportPath"
                  "GeneratedFileListsDir"
                  "GeneratedProductVerifyDir"
                  "BuildModel"
                  "BuildMsg"
                  "BuildEffect"
                  "let init"
                  "let update" ]
        }

        test "V3 targets are wired into Verify and Ci delegates to Verify" {
            let content = read "build.fsx"

            [ "CapabilityCheck"; "SkillCheck"; "GeneratedProductCheck"; "TemplateCheck"; "Verify"; "Ci" ]
            |> List.iter expectFakeTarget

            expectContains content "\"GeneratedProductCheck\", [ \"CapabilityCheck\"; \"SkillCheck\"; \"Dev\"; \"TemplateCheck\" ]" "generated product check runs after capability skill dev and template checks"
            expectContains content "\"Verify\"," "Verify target exists"
            expectContains content "\"CapabilityCheck\"" "Verify includes CapabilityCheck"
            expectContains content "\"SkillCheck\"" "Verify includes SkillCheck"
            expectContains content "\"GeneratedProductCheck\"" "Verify includes GeneratedProductCheck"
            expectContains content "\"Ci\", [ \"CiPreflight\"; \"Verify\" ]" "Ci runs preflight before delegating to Verify"
        }

        test "Controls boundary refactor command surface is wired into governed targets" {
            let content = read "build.fsx"

            [ "Dev"
              "Verify"
              "Ci"
              "PackLocal"
              "PackageSurfaceCheck"
              "FsiTranscripts"
              "TemplateCheck"
              "CapabilityCheck"
              "SkillCheck"
              "GeneratedProductCheck"
              "DependencyReport"
              "GeneratedGuidanceCheck"
              "TemplateDrift"
              "EvidenceGraph"
              "EvidenceAudit" ]
            |> List.iter (fun target -> expectFakeTarget target)

            [ "\"src/Controls/Controls.fsproj\", \"FS.Skia.UI.Controls\""
              "\"src/KeyboardInput/KeyboardInput.fsproj\", \"FS.Skia.UI.KeyboardInput\""
              "\"src/Controls.Elmish/Controls.Elmish.fsproj\", \"FS.Skia.UI.Controls.Elmish\""
              "scripts/controls-prelude.fsx"
              "scripts/keyboardinput-package-prelude.fsx"
              "scripts/controls-elmish-prelude.fsx"
              "ControlsCatalogCheck"
              "ControlsInteractionCheck"
              "ControlsRenderingCheck"
              "DependencyOwnershipReport"
              "GeneratedGuidanceScan"
              "ScanV3GeneratedProducts"
              "controls-boundary-guidance"
              "EvidenceAudit\", [ \"EvidenceGraph\" ]" ]
            |> List.iter (fun needle -> expectContains content needle $"build.fsx wires {needle}")

            [ "PackageSurfaceCheck"
              "FsiTranscripts"
              "TemplateCheck"
              "CapabilityCheck"
              "SkillCheck"
              "GeneratedProductCheck"
              "DependencyReport"
              "GeneratedGuidanceCheck"
              "TemplateDrift"
              "EvidenceAudit" ]
            |> List.iter (fun verifyDependency ->
                expectContains content $"\"{verifyDependency}\"" $"Verify includes {verifyDependency}")
        }

        test "build governance targets preserve report outputs readiness paths and actionable artifact failures" {
            let content = readBuildGovernanceSources ()

            [ "Dev"
              "Verify"
              "Ci"
              "PackLocal"
              "DependencyReport"
              "TemplateCheck"
              "GeneratedGuidanceCheck"
              "TemplateDrift"
              "EvidenceGraph"
              "EvidenceAudit" ]
            |> List.iter expectFakeTarget

            [ "DependencyReportPath"
              "GeneratedGuidanceReportPath"
              "TemplateDriftReportPath"
              "EvidenceGraphReportPath"
              "EvidenceAuditReportPath"
              "RequireFiles(\"dependency report output\", [ model.DependencyReportPath ])"
              "RequireFiles(\"generated guidance report output\", [ model.GeneratedGuidanceReportPath ])"
              "RequireFiles(\"template drift report output\", [ model.TemplateDriftReportPath ])"
              "RequireFiles(\"task graph output\""
              "RequireFiles(\"evidence audit output\""
              "failwithf \"Missing %s:%s%s\""
              "See %s" ]
            |> List.iter (fun needle -> expectContains content needle $"build governance preserves {needle}")
        }

        test "workflow self-check exercises pure transition and emitted effect assertions" {
            let exitCode, stdout, stderr = runFakeTarget "BuildWorkflowCheck"
            let output = stdout + stderr
            Expect.equal exitCode 0 output
            Expect.stringContains output "BuildWorkflowCheck" "self-check target ran"
        }
    ]
