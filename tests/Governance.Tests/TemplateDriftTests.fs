module TemplateDriftTests

open Expecto
open GovernanceTestSupport

let templateDriftFixtureOutput name =
    let root =
        if directoryExists "specs/008-targeted-refactor-governance" then
            "specs/008-targeted-refactor-governance/readiness/fixtures/template-drift"
        else
            "readiness/workflow/fixtures/template-drift"

    fullPath $"{root}/{name}.md"

[<Tests>]
let templateDriftTests =
    testList "Template drift governance" [
        test "template ownership and deferral rules are documented" {
            expectFileContains
                "docs/reports/template-profile.md"
                [ "Template-owned changes"
                  ".template.config/template.json"
                  "readiness/template-deferrals.yml"
                  "id"
                  "paths"
                  "rationale"
                  "owner"
                  "target_phase" ]
        }

        test "root deferral file exists with required schema comments" {
            expectFileContains
                "readiness/template-deferrals.yml"
                [ "schema_version"
                  "id"
                  "paths"
                  "rationale"
                  "owner"
                  "target_phase"
                  "accepted_deferrals: []" ]
        }

        test "drift script reports path diagnostics and alignment classes" {
            expectFileContains
                "scripts/template-drift.fsx"
                [ "pathClasses"
                  "alignmentClasses"
                  "readiness/template-deferrals.yml"
                  "Required Alignment Classes"
                  "Changed Template-Owned Paths"
                  "No drift blockers" ]
        }

        test "template drift fixture fails when template-owned path lacks required alignment" {
            if directoryExists "specs/008-targeted-refactor-governance" then
                let outputPath = templateDriftFixtureOutput "missing-alignment"
                let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/template-drift.fsx --fixture missing-alignment {outputPath}"
                let output = stdout + stderr

                Expect.notEqual exitCode 0 "fixture must fail"
                Expect.stringContains output "source-code" "diagnostic names path class"
                Expect.stringContains output "required alignment class" "diagnostic names missing alignment class"
            else
                Expect.isFalse (directoryExists ".template.config") "generated products skip source-only drift fixtures"
        }

        test "template drift fixture fails invalid deferrals with schema fields named" {
            if directoryExists "specs/008-targeted-refactor-governance" then
                let outputPath = templateDriftFixtureOutput "invalid-deferral"
                let exitCode, stdout, stderr = runProcess "dotnet" $"fsi scripts/template-drift.fsx --fixture invalid-deferral {outputPath}"
                let output = stdout + stderr

                Expect.notEqual exitCode 0 "fixture must fail"
                Expect.stringContains output "owner" "diagnostic names owner"
                Expect.stringContains output "target_phase" "diagnostic names target_phase"
            else
                Expect.isFalse (directoryExists ".template.config") "generated products skip source-only drift fixtures"
        }
    ]
