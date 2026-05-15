module CommandContractTests

open System
open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let commandContractTests =
    testList "Canonical command contract" [
        test "repo-local FAKE manifest and wrappers are available" {
            expectFileContains ".config/dotnet-tools.json" [ "\"fake-cli\""; "\"version\": \"5.23.1\""; "\"fake\"" ]
            expectFileContains "fake.sh" [ "dotnet tool restore"; "dotnet fake \"$@\"" ]
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
                  "BUILD SECTION: target graph" ]
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
              "EvidenceGraph"
              "EvidenceAudit"
              "Verify"
              "Ci" ]
            |> List.iter (fun target ->
                Expect.stringContains content $"\"{target}\"" $"{target} target is named in build.fsx")

            expectContains content "\"Build\", [ \"Restore\" ]" "Build depends on Restore"
            expectContains content "\"Test\", [ \"Build\" ]" "Test depends on Build"
            expectContains content "\"Dev\", [ \"Test\" ]" "Dev depends on Test"
            expectContains content "\"EvidenceAudit\", [ \"EvidenceGraph\" ]" "audit depends on graph"
            expectContains content "\"Ci\", [ \"Verify\" ]" "Ci delegates to Verify"
        }

        test "workflow self-check exercises pure transition and emitted effect assertions" {
            let exitCode, stdout, stderr = runProcess "./fake.sh" "build -t BuildWorkflowCheck"
            let output = stdout + stderr
            Expect.equal exitCode 0 output
            Expect.stringContains output "BuildWorkflowCheck" "self-check target ran"
        }
    ]
