module CommandContractTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let commandContractTests =
    testList "Canonical command contract" [
        test "compiled build front-end and wrappers are available" {
            // Feature 045: the front-end is a compiled exe (build/Build.fsproj) behind the
            // launchers; `dotnet fake` / `fake-cli` / `dotnet tool restore` are gone (FR-002/FR-003).
            let tools = read ".config/dotnet-tools.json"
            Expect.isFalse (tools.Contains "fake-cli") ".config/dotnet-tools.json no longer lists fake-cli"
            expectFileContains "fake.sh" [ "dotnet run --project build/Build.fsproj -- \"$@\"" ]
            expectFileContains "fake.cmd" [ "dotnet run --project build/Build.fsproj -- %*" ]
            let bash = read "fake.sh"
            Expect.isFalse (bash.Contains "dotnet fake") "fake.sh no longer invokes dotnet fake"
            Expect.isFalse (bash.Contains "dotnet tool restore") "fake.sh no longer restores the fake-cli tool"
            Expect.isTrue (File.Exists(fullPath "fake.sh")) "Bash wrapper exists"
            Expect.isTrue (File.Exists(fullPath "fake.cmd")) "Windows wrapper exists"
            Expect.isTrue (File.Exists(fullPath "build/Build.fsproj")) "compiled front-end project exists"
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

        test "compiled build front-end is organized by named concern modules" {
            // Feature 045: build.fsx's named comment sections became compiled modules with
            // curated .fsi surfaces (Principle II); assert the relocated module organization.
            expectFileContains
                "build.fsx"
                [ "module FS.Skia.UI.Build.Engine.Model"
                  "module FS.Skia.UI.Build.Engine.Update"
                  "module FS.Skia.UI.Build.Engine.Interpret"
                  "module FS.Skia.UI.Build.GeneratedProduct"
                  "module FS.Skia.UI.Build.Guidance"
                  "module FS.Skia.UI.Build.Preflight" ]
        }

        test "required targets are declared exactly once through the target graph" {
            // Target identity + the dependency graph derive from the typed Targets DU (FR-001);
            // assert against the real registry/rows rather than string-tuple literals.
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
            |> List.iter expectFakeTarget

            expectDependency "Build" [ "Restore" ]
            expectDependency "Test" [ "Build"; "SampleContractSmoke" ]
            // Feature 044 retired SkillExamplesCheck; Dev's only skill gate is now SkillSyncCheck.
            expectDependency "Dev" [ "Test"; "SkillSyncCheck" ]
            expectDependency "EvidenceAudit" [ "EvidenceGraph" ]
            expectDependency "Ci" [ "CiPreflight"; "Verify" ]
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

            expectDependency "GeneratedProductCheck" [ "CapabilityCheck"; "SkillCheck"; "Dev"; "TemplateCheck" ]
            expectFakeTarget "Verify"
            expectContains content "\"CapabilityCheck\"" "Verify includes CapabilityCheck"
            expectContains content "\"SkillCheck\"" "Verify includes SkillCheck"
            expectContains content "\"GeneratedProductCheck\"" "Verify includes GeneratedProductCheck"
            expectDependency "Ci" [ "CiPreflight"; "Verify" ]
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
              "controls-boundary-guidance" ]
            |> List.iter (fun needle -> expectContains content needle $"build.fsx wires {needle}")

            expectDependency "EvidenceAudit" [ "EvidenceGraph" ]

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
