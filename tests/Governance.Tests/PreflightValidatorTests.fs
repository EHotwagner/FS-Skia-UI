module PreflightValidatorTests

// Feature 045 (US3, T021): the relocated process-health preflight, exercised over the real
// host (reads /proc, dotnet --info). Even on a fail-fast environment it writes its evidence
// report, which is the artifact under test (real evidence).
open System
open System.IO
open Expecto
open FS.Skia.UI.Build
open GovernanceTestSupport

[<Tests>]
let preflightValidatorTests =
    testList "Preflight validator (relocated)" [
        test "collectProcessHealth writes a process-health evidence report" {
            let outPath = Path.Combine(Path.GetTempPath(), $"ph-{Guid.NewGuid():N}.md")
            let verdictPath = Path.Combine(Path.GetTempPath(), $"phv-{Guid.NewGuid():N}.md")

            try
                // collectProcessHealth writes the report then may fail fast on an unhealthy
                // environment; the report artifact is written regardless.
                let _ = (try Preflight.collectProcessHealth repositoryRoot "Test" outPath verdictPath with _ -> ())
                Expect.isTrue (File.Exists outPath) "process-health report written"
                Expect.stringContains (File.ReadAllText outPath) "# Process Health Evidence" "report has the expected heading"
            finally
                if File.Exists outPath then File.Delete outPath
                if File.Exists verdictPath then File.Delete verdictPath
        }
    ]
