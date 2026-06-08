module Feature083GovernanceTests

(* Feature 083 — failing-first (Principle I/VI) governance tests for the WCAG ContrastCheck
   gate: (T012) it is a known gate routed for src/Controls/** and src/Color/**; (T013) a token
   dropped below threshold makes the pure gate evaluator fail naming the pairing, measured, and
   required ratio, and restoring an accessible value makes it pass (SC-005). All cases call the
   real Routing.select / AgentValidation.knownGates / ContrastGate functions over typed values. *)

open System.IO
open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Routing

let private repositoryRoot =
    let rec find dir =
        if File.Exists(Path.Combine(dir, "FS-Skia-UI.sln")) then dir
        else
            match Path.GetDirectoryName dir with
            | null -> failwith "could not locate repository root"
            | parent -> find parent

    find (Directory.GetCurrentDirectory())

let private tokensJson () =
    File.ReadAllText(Path.Combine(repositoryRoot, DesignTokenGen.tokensJsonRel))

let private diff paths = { ChangedPaths = paths }

[<Tests>]
let contrastGateRoutingTests =
    testList "Feature 083 ContrastCheck routing + registration (T012, FR-011)" [

        test "ContrastCheck is a known gate" {
            Expect.contains AgentValidation.ValidationContract.knownGates "ContrastCheck" "knownGates must list ContrastCheck or the contract reports an unknown gate"
        }

        test "a src/Controls/** change routes ContrastCheck" {
            let selection = select FrameworkAuthor (diff [ "src/Controls/design-tokens.tokens.json" ])
            Expect.contains selection.Gates Targets.ContrastCheck "a design-token / theme edit selects the contrast gate"
            Expect.notEqual selection.Tier InnerLoop "a controls-surface change escalates"
        }

        test "a src/Color/** change routes ContrastCheck via the color-contrast rule" {
            let selection = select FrameworkAuthor (diff [ "src/Color/Contrast.fs" ])
            Expect.contains selection.Gates Targets.ContrastCheck "a Color package change selects the contrast gate"
            Expect.contains selection.MatchedRuleIds "color-contrast" "the color-contrast rule matches src/Color/**"
        }

        test "ContrastCheck is in the runnable target registry" {
            Expect.contains Targets.allTargets Targets.ContrastCheck "ContrastCheck is a runnable target"
            Expect.equal (Targets.name Targets.ContrastCheck) "ContrastCheck" "the name arm is wired"
        }
    ]

[<Tests>]
let contrastGateRegressionTests =
    testList "Feature 083 ContrastCheck gate regression (T013, SC-005)" [

        test "the shipped themes pass — no enforced pairing falls below threshold (SC-001)" {
            let outcomes = DesignTokenGen.parse (tokensJson ()) |> ContrastGate.outcomesFromFacts
            Expect.isNonEmpty outcomes "the gate evaluates a non-empty pairing set"
            Expect.isEmpty (ContrastGate.failures outcomes) "every enforced pairing passes on the shipped themes"
        }

        test "a token dropped below threshold makes the gate fail naming pairing, measured, and required" {
            // Poison dark.danger to sit near dark.background (#111827) via the DTCG source only.
            let poisoned = (tokensJson ()).Replace("\"#ff9592ff\"", "\"#1c2742ff\"")
            Expect.notEqual poisoned (tokensJson ()) "the poison actually changed the source"

            let outcomes = DesignTokenGen.parse poisoned |> ContrastGate.outcomesFromFacts
            let failures = ContrastGate.failures outcomes
            Expect.isNonEmpty failures "a sub-threshold token fails the gate"

            let dangerFailure =
                failures |> List.filter (fun f -> f.Theme = "dark" && f.Pairing.Foreground = "danger")
            Expect.isNonEmpty dangerFailure "the failing row is the dark danger/background pairing"

            let diagnostics = ContrastGate.failureDiagnostics outcomes
            Expect.isTrue (diagnostics |> List.exists (fun d -> d.Contains "danger" && d.Contains "background")) "the diagnostic names the pairing"
            Expect.isTrue (diagnostics |> List.exists (fun d -> d.Contains "measures" && d.Contains "requires")) "the diagnostic names the measured and required ratios"
        }

        test "restoring an accessible value makes the gate pass again" {
            let outcomes = DesignTokenGen.parse (tokensJson ()) |> ContrastGate.outcomesFromFacts
            Expect.isEmpty (ContrastGate.failures outcomes) "the restored (shipped) value passes"
        }
    ]
