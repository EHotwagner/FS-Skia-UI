module ConstitutionCheckTests

// Feature 046 (US1, T008, FR-013): failing-first typed unit tests for the
// Constitution-Check completeness validator. Tests assert typed
// ConstitutionCheckResult / ValidationFinding values — no string matching.
open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Guidance
open FS.Skia.UI.Build.Findings
open GovernanceTestSupport

let private fixture name =
    File.ReadAllText(fullPath $"tests/Governance.Tests/fixtures/constitution-check/{name}")

let private statusOf areaId result =
    match result with
    | TemplateRecognized areas ->
        areas |> List.tryPick (fun (area, status) -> if area.Id = areaId then Some status else None)
    | UnrecognizedTemplateRevision _ -> None

[<Tests>]
let constitutionCheckTests =
    testList "ConstitutionCheck validator" [
        test "all 11 areas filled => recognized, every area Filled, zero findings" {
            let result = classifyConstitutionCheck (fixture "complete-plan.md")

            match result with
            | TemplateRecognized areas ->
                Expect.equal (List.length areas) 11 "all 11 areas classified"
                Expect.all (areas |> List.map snd) (fun s -> s = Filled) "every area is Filled"
            | UnrecognizedTemplateRevision diag ->
                failtestf "expected TemplateRecognized, got UnrecognizedTemplateRevision %s" diag

            Expect.equal (constitutionCheckFindings "plan.md" result) [] "complete plan yields no findings"
        }

        test "a missing/empty area => Empty status + a finding naming the area id and plan path" {
            let result = classifyConstitutionCheck (fixture "missing-area-plan.md")
            Expect.equal (statusOf "deferred-scope" result) (Some Empty) "deferred-scope is Empty"

            let findings = constitutionCheckFindings "specs/x/plan.md" result
            Expect.equal (List.length findings) 1 "exactly one finding"
            let finding = List.head findings
            Expect.equal finding.Rule "deferred-scope" "finding names the missing area id"
            Expect.equal finding.Path "specs/x/plan.md" "finding carries the plan path"
            Expect.equal finding.ArtifactClass "constitution-check" "finding artifact class"
        }

        test "an area still carrying template boilerplate => StillBoilerplate + finding" {
            let result = classifyConstitutionCheck (fixture "boilerplate-plan.md")
            Expect.equal (statusOf "template-ownership" result) (Some StillBoilerplate) "template-ownership is StillBoilerplate"

            let findings = constitutionCheckFindings "plan.md" result
            Expect.equal (findings |> List.map (fun f -> f.Rule)) [ "template-ownership" ] "one finding for the boilerplate area"
        }

        test "an area with a NEEDS CLARIFICATION / unresolved-work placeholder => PlaceholderUnresolved + finding" {
            let result = classifyConstitutionCheck (fixture "placeholder-plan.md")
            Expect.equal (statusOf "dependency-impact" result) (Some PlaceholderUnresolved) "dependency-impact is PlaceholderUnresolved"

            let findings = constitutionCheckFindings "plan.md" result
            Expect.equal (findings |> List.map (fun f -> f.Rule)) [ "dependency-impact" ] "one finding for the placeholder area"
        }

        test "an area marked N/A-with-rationale => Filled, zero findings" {
            let result = classifyConstitutionCheck (fixture "na-rationale-plan.md")
            Expect.equal (statusOf "mvu-boundary" result) (Some Filled) "N/A-with-rationale counts as Filled"
            Expect.equal (constitutionCheckFindings "plan.md" result) [] "N/A-with-rationale plan yields no findings"
        }

        test "a template revision that no longer maps to the identifiers => UnrecognizedTemplateRevision + finding" {
            let result = classifyConstitutionCheck (fixture "unrecognized-template-plan.md")

            match result with
            | UnrecognizedTemplateRevision diag -> Expect.isNotEmpty diag "diagnostic is non-empty"
            | TemplateRecognized _ -> failtest "expected UnrecognizedTemplateRevision"

            let findings = constitutionCheckFindings "plan.md" result
            Expect.equal (findings |> List.map (fun f -> f.Rule)) [ "unrecognized-template-revision" ] "single unrecognized-revision finding"
        }

        test "the canonical required set is exactly the 11 documented stable ids in order" {
            let ids = requiredDecisionAreas |> List.map (fun a -> a.Id)
            Expect.equal ids
                [ "template-ownership"; "dependency-impact"; "command-surface"; "generated-project"
                  "evidence-paths"; "fsi-contract"; "mvu-boundary"; "synthetic-evidence"
                  "test-evidence"; "observability"; "deferred-scope" ]
                "canonical area ids and order"
        }

        test "the active feature's own plan.md passes the gate (no findings)" {
            let planContent = File.ReadAllText(fullPath "specs/046-foundations-rule-codification/plan.md")
            let result = classifyConstitutionCheck planContent
            Expect.equal (constitutionCheckFindings "plan.md" result) [] "this feature's plan.md is complete"
        }
    ]
