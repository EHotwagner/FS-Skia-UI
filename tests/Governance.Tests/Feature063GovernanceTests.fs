module Feature063GovernanceTests

// Feature 063 (lunar-lander-consumer-friction-followups): executable coverage for
// the SymbolCrossCheck FAKE-target wiring (FR-003/SC-003) and the relabelled
// readiness-contract diagnostic (FR-004/SC-004). Pure-function assertions over the
// real governance library — never log scraping.

open Expecto
open FS.Skia.UI.Build
open FS.Skia.UI.Build.Evidence

[<Tests>]
let feature063GovernanceTests =
    testList "feature 063 lunar-lander consumer-friction follow-ups" [

        // --- FR-004 / SC-004 (T021/T022): one absent token reads distinctly ----
        test "FR-004 one absent token prints full-required-set and absent-from-file under distinct labels" {
            let hit =
                { ScanHit.basic "specs/x/readiness/skill-loading-evidence.md" "partial readiness contract" with
                    Status = Some "incomplete"
                    Required = Some [ "task"; "skill id"; "loaded_at" ]
                    MissingTerms = Some [ "loaded_at" ] }

            let rc: ScanResult = { Area = "readiness-contract"; Hits = [ hit ]; BlockingCount = 1 }
            let rendered = Render.readinessContractDiagnostics rc

            Expect.stringContains rendered "full-required-set: task, skill id, loaded_at" "the full required set under its own label"
            Expect.stringContains rendered "absent-from-file: loaded_at" "the absent subset under a distinct label"
            Expect.isFalse (rendered.Contains "required-tokens:") "the old ambiguous label is gone"
            Expect.isFalse (rendered.Contains "missing:") "the old ambiguous label is gone"
        }

        // --- FR-003 / SC-003 (T016/T020): SymbolCrossCheck target is wired -----
        test "FR-003 SymbolCrossCheck is a registered target" {
            Expect.contains Targets.allTargets Targets.SymbolCrossCheck "SymbolCrossCheck is in the metadata registry"
            Expect.equal (Targets.name Targets.SymbolCrossCheck) "SymbolCrossCheck" "target name"
            Expect.equal (Targets.directPrerequisites Targets.SymbolCrossCheck) [] "no prerequisites — reads the feature dir directly"
        }

        test "FR-003 SymbolCrossCheck is an allowed gate (no unknown-gate diagnostic)" {
            Expect.contains
                AgentValidation.ValidationContract.knownGates
                "SymbolCrossCheck"
                "knownGates must list SymbolCrossCheck or Governance.Tests reports an unknown gate"
        }

        // --- FR-003 (T020): the no-drift path is a well-formed, empty section ----
        test "FR-003 a no-drift cross-check renders a well-formed empty section" {
            let rendered = SymbolCrossCheck.render []
            Expect.stringContains rendered "## Symbol consistency (analyze pass G)" "header is always present"
            Expect.stringContains rendered "no cross-artifact set-differences detected" "empty body, never absent"
        }

        // --- FR-003 (T020): a seeded Msg-case drift is a proper-subset finding --
        test "FR-003 a Msg-case present in data-model+tasks but absent from plan is reported" {
            let plan = "no symbols here"
            let dataModel = "the `ZzzDriftProbe` Msg case is handled"
            let tasks = "the `ZzzDriftProbe` Msg case is dispatched"
            let findings = SymbolCrossCheck.diff plan dataModel tasks

            let probe =
                findings |> List.tryFind (fun s -> s.Name = "ZzzDriftProbe")

            Expect.isSome probe "the seeded Msg case is found"
            Expect.equal probe.Value.Kind SymbolCrossCheck.MsgCase "classified as a msg-case"
            Expect.equal
                probe.Value.PresentIn
                (set [ SymbolCrossCheck.DataModel; SymbolCrossCheck.Tasks ])
                "present in data-model+tasks, missing from plan"

            let rendered = SymbolCrossCheck.render findings
            Expect.stringContains
                rendered
                "msg-case ZzzDriftProbe — in {data-model, tasks}, missing from {plan}"
                "renders the documented proper-subset line"
        }
    ]
