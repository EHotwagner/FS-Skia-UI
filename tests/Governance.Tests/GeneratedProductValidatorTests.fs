module GeneratedProductValidatorTests

// Feature 045 (US3, T021): the relocated generated-product validators, exercised over real
// repository project files (real evidence). The dependency-ownership report is deterministic
// and reads the live src/* project files; its output path is redirected to a temp file.
open System
open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Engine.Model
open GovernanceTestSupport

let private baseModel = fst (init repositoryRoot)

[<Tests>]
let generatedProductValidatorTests =
    testList "GeneratedProduct validator (relocated)" [
        test "runDependencyOwnershipReport writes the report over the real project files" {
            let tmp = Path.Combine(Path.GetTempPath(), $"dep-{Guid.NewGuid():N}.md")
            let model = { baseModel with DependencyReportPath = tmp }

            try
                let _ = (try GeneratedProduct.runDependencyOwnershipReport model with _ -> ())
                Expect.isTrue (File.Exists tmp) "dependency-ownership report written"
                let report = File.ReadAllText tmp
                Expect.isTrue (report.Length > 0) "report is non-empty"
                Expect.stringContains report "#" "report renders Markdown headings"
            finally
                if File.Exists tmp then File.Delete tmp
        }
    ]
