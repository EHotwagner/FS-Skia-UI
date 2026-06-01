module ContractViewTests

(* Feature 042 / Phase 7 — failing-first (Principle I/VI) currency-check tests for the
   generated `validation.contract.yml` view. `render` is the single emitter; `currencyDrift`
   compares an on-disk contract against the freshly rendered text. Before ContractView.fs
   existed these did not compile (SC-007 / FR-007). *)

open Expecto
open FS.Skia.UI.Build

[<Tests>]
let contractViewTests =
    testList "Feature 042 ContractView currency (typed cases)" [

        // SC-007 — the freshly rendered contract is, by definition, current.
        test "currencyDrift is None for the freshly rendered contract" {
            let rendered = ContractView.render Routing.rules Routing.dogfoodFeatureIds
            let drift = ContractView.currencyDrift rendered Routing.rules Routing.dogfoodFeatureIds
            Expect.isNone drift "a contract equal to render output is current"
        }

        // SC-007 — a hand-mutated contract is detected as stale with a regenerate diagnostic.
        test "currencyDrift is Some for a hand-mutated contract" {
            let mutated =
                ContractView.render Routing.rules Routing.dogfoodFeatureIds
                + "\n# hand edit that diverges from Routing.fs\n"

            match ContractView.currencyDrift mutated Routing.rules Routing.dogfoodFeatureIds with
            | None -> failtest "a hand-edited contract must be detected as stale"
            | Some diagnostic ->
                Expect.stringContains diagnostic "regenerate from Routing.fs" "the diagnostic points at the single source of truth"
        }

        // FR-007 — the rendered contract still names the targets its existing consumers scan for.
        test "render names the gate targets its consumers reference" {
            let rendered = ContractView.render Routing.rules Routing.dogfoodFeatureIds
            Expect.stringContains rendered "TemplateCheck" "the generated-template rule's gate is rendered"
            Expect.stringContains rendered "EvidenceGraph" "the final gates are rendered"
            Expect.stringContains rendered "dogfood_feature_ids" "the dogfood policy is rendered"
            Expect.stringContains rendered "\"042\"" "feature 042 is a rendered dogfood id"
        }
    ]
