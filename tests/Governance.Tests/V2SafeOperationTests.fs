module V2SafeOperationTests

open System.IO
open Expecto
open GovernanceTestSupport

[<Tests>]
let v2SafeOperationTests =
    testList "V2 safe operations and diagnostics" [
        test "Clean is target-owned and source-preserving" {
            expectFileContains
                "build.fsx"
                [ "CleanDirectoryContents model.TemplateEvidenceDir"
                  "CleanDirectoryContents model.TemplateWorkDir"
                  "CleanDirectoryContents model.TemplateArtifactDir" ]

            let content = read "build.fsx"
            Expect.isFalse (content.Contains("CleanDirectoryContents model.RepositoryRoot")) "Clean never deletes repository root"
        }

        test "dependency report negative fixture names remediation" {
            let output = Path.Combine(Path.GetTempPath(), "fs-skia-ui-dependency-negative.md")
            let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/dependency-report.fsx --fixture unmanaged-inline-version {output}"
            let text = stdout + stderr + if File.Exists output then File.ReadAllText output else ""
            Expect.notEqual exitCode 0 "fixture must fail"
            Expect.stringContains text "Move the version to Directory.Packages.props" "diagnostic names remediation"
            Expect.stringContains text "Expecto" "diagnostic names package"
        }

        test "template drift negative fixture rejects invalid deferrals" {
            let output = Path.Combine(Path.GetTempPath(), "fs-skia-ui-drift-negative.md")
            let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/template-drift.fsx --fixture invalid-deferral {output}"
            let text = stdout + stderr + if File.Exists output then File.ReadAllText output else ""
            Expect.notEqual exitCode 0 "fixture must fail"
            Expect.stringContains text "missing owner" "diagnostic names missing owner"
            Expect.stringContains text "target_phase" "diagnostic names missing target phase"
        }
    ]
