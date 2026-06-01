module AuditStatusRegionTests

(* US2 (spec 037): the audit reads machine-readable status only from the
   authoritative `audit-status` fenced region; prose, negations, markdown
   bullets, and illustrative code blocks are never read as status.

   Feature 043 (T025): re-pointed off `python3 audit-status-scan.py` onto the
   typed `Evidence.StatusRegion` library — these tests now exercise the compiled
   F# scanner against the same committed fixtures and assert typed results. *)

open Expecto
open GovernanceTestSupport
open FS.Skia.UI.Build.Evidence

let private fixtures = "specs/037-authoring-audit-robustness/readiness/audit-fixtures"

let private scan (relativeFixture: string) : StatusScan =
    StatusRegion.scanText relativeFixture (read relativeFixture)

let private reasons (s: StatusScan) = String.concat " | " s.Reasons

[<Tests>]
let auditStatusRegionTests =
    testList "US2 audit-status region resolution" [

        test "failing-first: prose fixture would block a naive substring scanner" {
            let text = read $"{fixtures}/prose-negation-clean.md"
            [ "taskbar-only"; "nu1603"; "mismatch" ]
            |> List.iter (fun term ->
                Expect.stringContains text term $"prose fixture mentions blocker term {term} (substring scanner would block)")
        }

        test "prose/negation fixture resolves PASS after the fix (FR-004)" {
            let s = scan $"{fixtures}/prose-negation-clean.md"
            Expect.isFalse s.Blocking $"prose/negation fixture passes: {reasons s}"
        }

        test "prose bullet does not override the authoritative region (US2 scenario 2)" {
            let s = scan $"{fixtures}/prose-negation-clean.md"
            Expect.isFalse s.Blocking "prose bullet / illustrative block never override the region"
        }

        test "genuine violation still BLOCKS (FR-006, no true-positive regression)" {
            let s = scan $"{fixtures}/genuine-violation.md"
            Expect.isTrue s.Blocking $"genuine violation blocks: {reasons s}"
            Expect.isTrue (s.Reasons |> List.exists (fun r -> r.Contains "unresolved package mismatch")) "names the package-mismatch violation"
            Expect.isTrue (s.Reasons |> List.exists (fun r -> r.Contains "taskbar-only")) "names the taskbar-only violation"
        }

        test "duplicate key within the region is a parse error (rule 2)" {
            let s = scan $"{fixtures}/duplicate-key.md"
            Expect.isTrue s.Blocking $"duplicate key blocks: {reasons s}"
            Expect.isTrue (s.Reasons |> List.exists (fun r -> r.Contains "duplicate key in audit-status region")) "surfaces the duplicate-key parse error"
        }

        test "malformed entry within the region is a parse error (rule 4)" {
            let s = scan $"{fixtures}/malformed-key.md"
            Expect.isTrue s.Blocking $"malformed entry blocks: {reasons s}"
            Expect.isTrue (s.Reasons |> List.exists (fun r -> r.Contains "malformed entry")) "surfaces the malformed-entry parse error"
        }
    ]
