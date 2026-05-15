module GeneratedProjectValidationTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let generatedProjectValidationTests =
    testList "Generated project validation contract" [
        test "TemplateSmoke scans placeholders excluded history minimal scope and generated Dev" {
            expectFileContains
                "build.fsx"
                [ "Placeholder scan: PASS"
                  "Excluded-history scan: PASS"
                  "Minimal optional exclusion scan: PASS"
                  "generated Dev"
                  "--allow-scripts yes"
                  "--skipGitInit true"
                  "non-visual V2 validation only"
                  "full visual evidence is deferred" ]
        }

        test "minimal required contents are enforced" {
            expectFileContains
                "build.fsx"
                [ "src/Lib/Lib.fsproj"
                  "tests/Lib.Tests/Lib.Tests.fsproj"
                  "tests/Package.Tests/Package.Tests.fsproj"
                  "tests/Governance.Tests/Governance.Tests.fsproj"
                  "samples/BasicViewer/BasicViewer.fsproj"
                  "Directory.Packages.props" ]
        }
    ]
