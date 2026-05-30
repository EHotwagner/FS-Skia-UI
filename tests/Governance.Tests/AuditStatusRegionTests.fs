module AuditStatusRegionTests

(* US2 (spec 037): the audit reads machine-readable status only from the
   authoritative `audit-status` fenced region; prose, negations, markdown
   bullets, and illustrative code blocks are never read as status. These tests
   run the real scanner against the committed fixtures. *)

open System
open Expecto
open GovernanceTestSupport

let private scan (relativeFixture: string) =
    runProcess
        "python3"
        $".specify/extensions/evidence/scripts/python/audit-status-scan.py \"{fullPath relativeFixture}\""

let private fixtures = "specs/037-authoring-audit-robustness/readiness/audit-fixtures"

[<Tests>]
let auditStatusRegionTests =
    testList "US2 audit-status region resolution" [

        test "failing-first: prose fixture would block a naive substring scanner" {
            // The prose/negation fixture deliberately contains every blocker term
            // in explanatory text. A whole-file substring scanner (today's
            // behavior) would hard-block on these, which is the false positive
            // this story removes.
            let text = read $"{fixtures}/prose-negation-clean.md"
            [ "taskbar-only"; "nu1603"; "mismatch" ]
            |> List.iter (fun term ->
                Expect.stringContains text term $"prose fixture mentions blocker term {term} (substring scanner would block)")
        }

        test "prose/negation fixture resolves PASS after the fix (FR-004)" {
            let code, stdout, stderr = scan $"{fixtures}/prose-negation-clean.md"
            let output = stdout + stderr
            Expect.equal code 0 $"prose/negation fixture passes: {output}"
            Expect.stringContains output "PASS" "prose fixture reported PASS"
        }

        test "prose bullet does not override the authoritative region (US2 scenario 2)" {
            // The fixture has a markdown bullet `- exact-package-match=true: no ...`
            // and an illustrative `text` block with exact-package-match=false; only
            // the audit-status region (exact-package-match=true) is authoritative.
            let code, _stdout, _stderr = scan $"{fixtures}/prose-negation-clean.md"
            Expect.equal code 0 "prose bullet / illustrative block never override the region"
        }

        test "genuine violation still BLOCKS (FR-006, no true-positive regression)" {
            let code, stdout, stderr = scan $"{fixtures}/genuine-violation.md"
            let output = stdout + stderr
            Expect.equal code 2 $"genuine violation blocks: {output}"
            Expect.stringContains output "unresolved package mismatch" "names the package-mismatch violation"
            Expect.stringContains output "taskbar-only" "names the taskbar-only violation"
        }

        test "duplicate key within the region is a parse error (rule 2)" {
            let code, stdout, stderr = scan $"{fixtures}/duplicate-key.md"
            let output = stdout + stderr
            Expect.equal code 2 $"duplicate key blocks: {output}"
            Expect.stringContains output "duplicate key in audit-status region" "surfaces the duplicate-key parse error"
        }

        test "malformed entry within the region is a parse error (rule 4)" {
            let code, stdout, stderr = scan $"{fixtures}/malformed-key.md"
            let output = stdout + stderr
            Expect.equal code 2 $"malformed entry blocks: {output}"
            Expect.stringContains output "malformed entry" "surfaces the malformed-entry parse error"
        }
    ]
