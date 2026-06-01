module GuidanceValidatorTests

// Feature 045 (US3, T021): the relocated generated-guidance scanner, exercised end-to-end
// over the REAL repository guidance (real evidence — no fixtures/mocks).
open System
open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open GovernanceTestSupport

let private model = fst (init repositoryRoot)

[<Tests>]
let guidanceValidatorTests =
    testList "Guidance validator (relocated)" [
        test "runGeneratedGuidanceScan writes a PASS report over the real repository guidance" {
            let outPath = Path.Combine(Path.GetTempPath(), $"guidance-{Guid.NewGuid():N}.md")

            try
                Guidance.runGeneratedGuidanceScan model outPath
                Expect.isTrue (File.Exists outPath) "guidance report written"
                let report = File.ReadAllText outPath
                Expect.stringContains report "# Generated Guidance Check" "report has the expected heading"
                Expect.stringContains report "PASS" "real repository guidance passes the relocated scanner"
            finally
                if File.Exists outPath then File.Delete outPath
        }
    ]
