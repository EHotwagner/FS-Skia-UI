module TemplateDriftTests

open Expecto
open GovernanceTestSupport

[<Tests>]
let templateDriftTests =
    testList "Template drift governance" [
        test "template ownership and deferral rules are documented" {
            expectFileContains
                "docs/template-profile.md"
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
                [ "templateOwnedPrefixes"
                  "alignmentPrefixes"
                  "readiness/template-deferrals.yml"
                  "Changed Template-Owned Paths"
                  "No drift blockers" ]
        }
    ]
